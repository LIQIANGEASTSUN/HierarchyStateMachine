using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;

#if A
using GenPB;



public interface ISkill
{
    void SetSkill(Skill skill);
}

public class AbilityHSM : IAbilityEnvironment, IAction
{
    private Skill _skill;

    private HSMStateMachine _hsmStateMachine = null;
    private IConditionCheck _iconditionCheck = null;
    private AbilityInputExtend _abilityInputExtend = null;

    public readonly static string SkillRequestState  = "Skill_Change_State";
    public readonly static string EnableFire         = "EnableFire";
    public readonly static string EnergyEnougth      = "EnergyEnougth";
    public readonly static string EnableActive       = "EnableActive";
    public readonly static string Phase_End          = "PhaseEnd";
    public readonly static string Hold               = "Hold";
    public readonly static string FocoFull           = "FocoFull";
    public readonly static string ShotTimeEnd        = "ShotTimeEnd";
    public readonly static string RemainingFireTimes = "RemainingFireTimes";
    public readonly static string SkillTimeEnd       = "SkillTimeEnd";
    public readonly static string Skill_State        = "Skill_State";

    protected Dictionary<int, string> _btnNameDic = new Dictionary<int, string>() {
        {(int)AbilityButtonType.GENERAL,      "GenericBtn" }, // 普通技能按钮
        {(int)AbilityButtonType.SKILL_DEPUTY, "DeputyBtn" },  // 副技能按钮
        {(int)AbilityButtonType.SKILL_UNIQUE, "UniqueBtn" },  // 大招按钮
        {(int)AbilityButtonType.FIRE,         "FireBtn" },    // 释放按钮
    };

    protected Dictionary<int, string> _skillConfigDic = new Dictionary<int, string>() {
        { (int)SkillHandleType.CONTINUOUS_FIRE,        "AbilityGenericHSM"},     // 普攻-连射     //已测试
        { (int)SkillHandleType.ROLL_BRUSH,             "AbilityRoleBrushHSM"},   // 普攻-滚动
        { (int)SkillHandleType.FOCO_SINGLE_FIRE,       "AbilityFocoSingleHSM"},  // 普攻-蓄力单发 //已测试
        { (int)SkillHandleType.FOCO_CONTINUOUS_FIRE,   "AbilityFocoContinueHSM"},// 普攻-蓄力连射 //已测试 
        { (int)SkillHandleType.SWING_BRUSH,            "AbilitySwingBruseHSM"},  // 刷子-甩
        { (int)SkillHandleType.SINGLE_SHOT,            "AbilitySingleShotHSM"},  // 普攻-单发
        { (int)SkillHandleType.DEPUTY_SKILL_THROW,     "AbilityThrowHSM"},       // 副技能-投掷   //已测试
        { (int)SkillHandleType.DEPUTY_SKILL_FOCO,      "AbilityFocoFullHSM"},    // 副技能-蓄力   //已测试
        { (int)SkillHandleType.UNIQUE_INSTANT_SKILL,   "AbilityUniqueHSM"},      // 大招-瞬发     //已测试
        { (int)SkillHandleType.UNIQUE_FIRE_CONTINUOUS, "AbilityUniqueRayHSM"},   // 大招-激光     //已测试
        { (int)SkillHandleType.UNIQUE_FIRE_SKILL,      "AbilityUniqueFireHSM" }, // 大招-射击     //已测试
        { (int)SkillHandleType.UNIQUE_DEPUTY_SKILL,    "" },                     // 大招-副技能
        { (int)SkillHandleType.TRANSFER,               "AbilityTransferHSM" },   // 传送技能      //已测试
        { (int)SkillHandleType.NONE,                   ""}
    };

    public AbilityHSM(Skill skill)
    {
        _skill = skill;
        Init();
    }

    public void Init()
    {
        string configName = string.Empty;
        if (!_skillConfigDic.TryGetValue(_skill.SkillData.HandleType, out configName))
        {
            return;
        }

        TextAsset textAsset = Resources.Load<TextAsset>(configName);
        if (null == textAsset)
        {
            return;
        }

        _iconditionCheck = new ConditionCheck();
        _abilityInputExtend = new AbilityInputExtend();

        HSMAnalysis analysis = new HSMAnalysis();
        _hsmStateMachine = analysis.Analysis(textAsset.text, _iconditionCheck, this);
        _hsmStateMachine.SetAutoTransitionState(false);
        SetSkill();

        Clear();
    }


    public void Update()
    {
        UpdateEnvironment();

        if (_skill.weaponId == 112103)
        {
            int a = 0;
        }

        if (null != _hsmStateMachine)
        {
            _hsmStateMachine.Execute();
        }
    }

    private void SetSkill()
    {
        for (int i = 0; i < _hsmStateMachine.StateList.Count; ++i)
        {
            HSMState hsmState = _hsmStateMachine.StateList[i];
            if (typeof(ISkill).IsAssignableFrom(hsmState.GetType()))
            {
                ISkill state = (ISkill)hsmState;
                state.SetSkill(_skill);
            }
        }
    }

#region ConditionCheck
    public ConditionCheck ConditionCheck
    {
        get { return (ConditionCheck)_iconditionCheck; }
    }

    public bool Input(AbilityButtonType buttonType, AbilityHandleType handleType)
    {
        if (handleType == AbilityHandleType.PRESS)  // 只处理 Down、Up
        {
            return false;
        }

        string btnName = string.Empty;
        if (!_btnNameDic.TryGetValue((int)buttonType, out btnName))
        {
            return false;
        }

        //Debug.LogError("Input:" + btnName + "   " + (int)handleType);
        ConditionCheck.SetParameter(btnName, (int)handleType);

        if (null != _hsmStateMachine)
        {
            bool value = _hsmStateMachine.Execute();
            //Debug.LogError("value:" + value);
            return value;
        }
        return false;
    }

    public void ChangeState(SkillConfigSkillPhaseType phaseType)
    {
        bool value = (phaseType == SkillConfigSkillPhaseType.STANDBY_PHASE) ? true : false;
        if (null == ConditionCheck)
        {
            return;
        }

        ConditionCheck.SetParameter(Phase_End, value);
        ConditionCheck.SetParameter(Hold, value);

        for (int i = 0; i < _hsmStateMachine.StateList.Count; ++i)
        {
            HSMState hsmState = _hsmStateMachine.StateList[i];
            if (typeof(HSMState).IsAssignableFrom(hsmState.GetType()))
            {
                StateSkill stateSkill = (StateSkill)hsmState;
                if (stateSkill.PhaseType == phaseType)
                {
                    _hsmStateMachine.ChangeState(stateSkill.StateId);
                    break;
                }
            }
        }
    }
#endregion

    public void SkillEnd()
    {
        Clear();
    }

    private void Clear()
    {
        ConditionCheck.SetParameter(Phase_End, false);
        ConditionCheck.SetParameter(Hold, false);
        ConditionCheck.SetParameter(FocoFull, false);
    }

    public void Receive(int weaponId, SkillPhaseType type, float focoPercentage, int ret)
    {
        _abilityInputExtend.RemoveRequest(type);
    }

    public void Request(int roleId, int weaponId, int type, float focoPercentage)
    {
        if (_abilityInputExtend.IsWaitRequest((SkillPhaseType)type))
        {
            return;
        }
        
        //Debug.LogError("Request: roleId:" + roleId + "    weaponId:" + weaponId + "    type:" + (SkillPhaseType)type + "    " + Extend.GameUtils.GetMillisecond());
        _abilityInputExtend.AddRequest((SkillPhaseType)type);
        SkillEventHandler.Instance.SendSkillOper(roleId, _skill.weaponId, (int)type, focoPercentage, 0);
    }

    public void SkillEvent(SkillConfigSkillPhaseType phaseType, SkillConfigSkillCustomEvent customEvent)
    {
        if (customEvent.EventBase.EventType != (int)SkillEventType.HOLD)
        {
            return;
        }
        //Debug.LogError("Hold:" + phaseType + "    " + (SkillEventType)(customEvent.EventBase.EventType));
        ConditionCheck.SetParameter(Hold, true);
    }

    public void PhaseEnd(SkillConfigSkillPhaseType phaseType)
    {
        //Debug.LogError("PhaseEnd");
        ConditionCheck.SetParameter(Phase_End, true);
    }

#region IAction
    public void DoAction(HSMState state, int toStateId)
    {
        HSMState toState = (_hsmStateMachine.GetState(toStateId));
        if (null != state && null != toState)
        {
            state.DoAction(toState);
        }
    }
#endregion

#region IAbilityEnvironment
    public void UpdateEnvironment()
    {
        if (null == ConditionCheck)
        {
            return;
        }
        int result = 0;

        ConditionCheck.SetParameter(EnableFire, _skill.EnableFire(ref result));
        ConditionCheck.SetParameter(EnergyEnougth, _skill.SkillEnergyEnough());
        ConditionCheck.SetParameter(EnableActive, _skill.EnableActive(ref result));
        ConditionCheck.SetParameter(ShotTimeEnd, _skill.ShotEndTime());
        ConditionCheck.SetParameter(RemainingFireTimes, _skill.RemainingFireTimes());
        ConditionCheck.SetParameter(SkillTimeEnd, _skill.TimeEnd);

        //Debug.LogError(_skill.weaponId + "    energyEnougth:" + _skill.SkillEnergyEnough() + "    enableFire:" + _skill.EnableFire(ref result));
        FocoEnergia();
    }

    private void FocoEnergia()
    {
        if (null == _skill || null == _skill.FocoEnergia || !_skill.FocoEnergia.IsStart())
        {
            return;
        }
        bool focoFull = (_skill.FocoEnergia.GetPercentage() >= 1);
        ConditionCheck.SetParameter(FocoFull, focoFull);
    }
#endregion

}

#endif
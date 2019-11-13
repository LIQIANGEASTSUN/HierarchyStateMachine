using UnityEngine;
using HSMTree;

#if A

using GenPB;
using System;

public class AbilityHSM : IAbilityEnvironment, IAction, IAbilityInput
{
    private Skill _skill;

    private HSMStateMachine _hsmStateMachine = null;
    private IConditionCheck _iconditionCheck = null;
    private AbilityInput _abilityInputExtend = null;

    private Action<int> _skillEnterCallBack = null;
    private Action<int> _skillExitCallBack = null;

    public AbilityHSM(Skill skill)
    {
        _skill = skill;
        Init();
    }

    public void Init()
    {
        string configName = string.Empty;
        if (!StateTool._skillConfigDic.TryGetValue(_skill.SkillData.HandleType, out configName))
        {
            return;
        }

        TextAsset textAsset = Resources.Load<TextAsset>(configName);
        if (null == textAsset)
        {
            return;
        }

        _iconditionCheck = new ConditionCheck();
        _abilityInputExtend = new AbilityInput();

        HSMAnalysis analysis = new HSMAnalysis();
        _hsmStateMachine = analysis.Analysis(textAsset.text, _iconditionCheck, this);
        _hsmStateMachine.SetAutoTransitionState(false);
        SetSkill();

        Clear();
    }


    public void Update()
    {
        UpdateEnvironment();

        if (null != _hsmStateMachine)
        {
            //if (_skill.weaponId < 112200)
            //{
            //    Debug.LogError("AbilityHSM:" + _skill.weaponId);
            //}
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

    public void Input(AbilityButtonType buttonType, AbilityHandleType handleType)
    {
        if (handleType == AbilityHandleType.PRESS)  // 只处理 Down、Up
        {
            return;
        }

        string btnName = string.Empty;
        if (!StateTool._btnNameDic.TryGetValue((int)buttonType, out btnName))
        {
            return;
        }

        ConditionCheck.SetParameter(btnName, (int)handleType);
    }

    public void ChangeState(SkillConfigSkillPhaseType phaseType)
    {
        bool value = (phaseType == SkillConfigSkillPhaseType.STANDBY_PHASE) ? true : false;
        if (null == ConditionCheck)
        {
            return;
        }

        ConditionCheck.SetParameter(StateTool.Phase_End, value);
        ConditionCheck.SetParameter(StateTool.Hold, value);

        for (int i = 0; i < _hsmStateMachine.StateList.Count; ++i)
        {
            HSMState hsmState = _hsmStateMachine.StateList[i];
            if (typeof(HSMState).IsAssignableFrom(hsmState.GetType()))
            {
                SkillPhaseState skillPhaseState = (SkillPhaseState)hsmState;
                if (skillPhaseState.PhaseType == phaseType)
                {
                    _hsmStateMachine.ChangeState(skillPhaseState.StateId);
                    break;
                }
            }
        }
    }
#endregion

    public void SetSkillEndCallBack(Action<int> skillEnterCallBack, Action<int> skillEndCallBack)
    {
        _skillEnterCallBack = skillEnterCallBack;
        _skillExitCallBack = skillEndCallBack;
    }

    public void SkillActive()
    {
        if (null != _skillEnterCallBack)
        {
            _skillEnterCallBack(_skill.weaponId);
        }
    }

    public void SkillEnd()
    {
        //Debug.LogError("SkillEnd:" + _skill.weaponId);
        Clear();

        if (null != _skillExitCallBack)
        {
            _skillExitCallBack(_skill.weaponId);
        }
    }

    private void Clear()
    {
        ConditionCheck.SetParameter(StateTool.Phase_End, false);
        ConditionCheck.SetParameter(StateTool.Hold, false);
        ConditionCheck.SetParameter(StateTool.FocoFull, false);
        _abilityInputExtend.Clear();
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
        ConditionCheck.SetParameter(StateTool.Hold, true);
    }

    public void PhaseEnd(SkillConfigSkillPhaseType phaseType)
    {
        //Debug.LogError("PhaseEnd");
        ConditionCheck.SetParameter(StateTool.Phase_End, true);
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

        ConditionCheck.SetParameter(StateTool.EnableFire, _skill.EnableFire(ref result));
        ConditionCheck.SetParameter(StateTool.EnergyEnougth, _skill.SkillEnergyEnough());
        ConditionCheck.SetParameter(StateTool.EnableActive, _skill.EnableActive(ref result));
        ConditionCheck.SetParameter(StateTool.ShotTimeEnd, _skill.ShotEndTime());
        ConditionCheck.SetParameter(StateTool.RemainingFireTimes, _skill.RemainingFireTimes());
        ConditionCheck.SetParameter(StateTool.SkillTimeEnd, _skill.TimeEnd);

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
        ConditionCheck.SetParameter(StateTool.FocoFull, focoFull);
    }
#endregion

}

#endif
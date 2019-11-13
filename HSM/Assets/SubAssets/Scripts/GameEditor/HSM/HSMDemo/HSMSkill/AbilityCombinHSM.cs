using HSMTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if A

using GenPB;

public class AbilityCombinHSM : IAbilityEnvironment, IAction, IAbilityInput
{
    private List<Skill> _skillList = new List<Skill>();
    private HSMStateMachine _hsmStateMachine = null;
    private IConditionCheck _iconditionCheck = null;
    private Dictionary<int, SkillAbilityState> _abliltyStateDic = new Dictionary<int, SkillAbilityState>();
    private SkillEmptyState _emptyState = null;

    private string[] _abilityArr = new string[] { StateTool.SkillNormalEnd, StateTool.SkillJumpEnd, StateTool.SkillSpecialEnd };

    public AbilityCombinHSM(List<Skill> skillList)
    {
        _skillList.AddRange(skillList);
        Init();
    }

    public void Init()
    {
        string configName = "AbilityCombinTestHSM";
        TextAsset textAsset = Resources.Load<TextAsset>(configName);
        if (null == textAsset)
        {
            return;
        }

        _iconditionCheck = new ConditionCheck();

        HSMAnalysis analysis = new HSMAnalysis();
        _hsmStateMachine = analysis.Analysis(textAsset.text, _iconditionCheck, this);
        SetSkill();
        Clear();
    }

    public void Update()
    {
        UpdateEnvironment();
        if (null != _hsmStateMachine)
        {
            //Debug.LogError("AbilityCombinHSM");
            _hsmStateMachine.Execute();
        }
    }

    private void SetSkill()
    {
        for (int i = 0; i < _hsmStateMachine.StateList.Count; ++i)
        {
            HSMState hsmState = _hsmStateMachine.StateList[i];
            if (typeof(SkillEmptyState).IsAssignableFrom(hsmState.GetType()))
            {
                SkillEmptyState state = (SkillEmptyState)hsmState;
                state.SetEnterCallBack(EmptyStateEnter);
                _emptyState = state;
            }

            if (typeof(SkillAbilityState).IsAssignableFrom(hsmState.GetType()))
            {
                SkillAbilityState state = (SkillAbilityState)hsmState;
                if (state.SkillIndex < 0 || state.SkillIndex >= _skillList.Count)
                {
                    Debug.LogError("SkillIndex not valid:" + state.SkillIndex);
                    break;
                }

                Skill skill = _skillList[state.SkillIndex];
                state.SetSkill(skill);
                state.SetAbilityCallBack(AbilityEnterCallBack, AbilityExitCallBack);
                _abliltyStateDic[skill.weaponId] = state;
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

        //Debug.LogError("AbilityCombin Input:" + btnName + "   " + (int)handleType);
        ConditionCheck.SetParameter(btnName, (int)handleType);

        foreach(var kv in _abliltyStateDic)
        {
            kv.Value.AbilityHSM.Input(buttonType, handleType);
        }
    }

    private void EmptyStateEnter()
    {
        //Debug.LogError("EmptyStateEnter");
        Clear();
    }

    public void AbilityEnterCallBack(int weaponId)
    {
        //Debug.LogError("AbilityEnterCallBack:" + weaponId);
        SkillAbilityState abilityState = null;
        if (_abliltyStateDic.TryGetValue(weaponId, out abilityState))
        {
        }
    }

    public void AbilityExitCallBack(int weaponId)
    {
        //Debug.LogError("AbilityExitCallBack:" + weaponId);
        SkillAbilityState abilityState = null;
        if (_abliltyStateDic.TryGetValue(weaponId, out abilityState))
        {
            ConditionCheck.SetParameter(_abilityArr[abilityState.SkillIndex], true);
        }
    }
#endregion

    private void Clear()
    {
        ConditionCheck.SetParameter(StateTool.JumpBtn, -1);
        ConditionCheck.SetParameter(StateTool.SkillNormalEnd, false);
        ConditionCheck.SetParameter(StateTool.SkillJumpEnd, false);
        ConditionCheck.SetParameter(StateTool.SkillSpecialEnd, false);

        foreach (var kv in _abliltyStateDic)
        {
            kv.Value.Clear();
        }
    }

#region IAction
    public void DoAction(HSMState state, int toStateId)
    {
        HSMState toState = (_hsmStateMachine.GetState(toStateId));
        if (null != state && null != toState)
        {
            Debug.LogError("DoAction");
            //state.DoAction(toState);
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

        //ConditionCheck.SetParameter(StateTool.EnableFire, _skill.EnableFire(ref result));
        //ConditionCheck.SetParameter(StateTool.EnergyEnougth, _skill.SkillEnergyEnough());
        //ConditionCheck.SetParameter(StateTool.EnableActive, _skill.EnableActive(ref result));
        //ConditionCheck.SetParameter(StateTool.ShotTimeEnd, _skill.ShotEndTime());
        //ConditionCheck.SetParameter(StateTool.RemainingFireTimes, _skill.RemainingFireTimes());
        //ConditionCheck.SetParameter(StateTool.SkillTimeEnd, _skill.TimeEnd);

        //Debug.LogError(_skill.weaponId + "    energyEnougth:" + _skill.SkillEnergyEnough() + "    enableFire:" + _skill.EnableFire(ref result));
    }
#endregion


}


#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;

public class StateSkill : HSMState/*, ISkill*/
{
    private static CustomIdentification _customIdentification = new CustomIdentification("技能/技能状态节点", IDENTIFICATION.SKILL_STATE, typeof(StateSkill), NODE_TYPE.STATE);

    //private Skill _skill;
    //private SkillConfigSkillPhaseType _phaseType = SkillConfigSkillPhaseType.NONE;

    private int _uiCount = 6;
    private int _enterShowUi = -1;
    private int _startShowUi = -1;
    private string endShowUI = "EndShowUI";
    private string startShowUI = "StartShowUI";
    public StateSkill() : base()
    {

    }

    public override void Init()
    {
        SetState();
    }

    public override void Enter()
    {
        base.Enter();
        EndShowUI();
    }

    public override int Execute(ref bool result)
    {
        return base.Execute(ref result);

        //result = false;
        //int toStateId = base.Execute(ref result);
        //if (result)
        //{
        //    _iAction.DoAction( this, toStateId);
        //}

        //return toStateId;
    }

    public override void Exit()
    {
        base.Exit();
        StartShowUI();
    }

    private void SetState()
    {
        for (int i = 0; i < _parameterList.Count; ++i)
        {
            HSMParameter parameter = _parameterList[i];
            //if (parameter.parameterName.CompareTo(AbilityHSM.Skill_State) == 0)
            //{
            //    int value = parameter.intValue;
            //    _phaseType = (SkillConfigSkillPhaseType)parameter.intValue;
            //    continue;
            //}
            if (parameter.parameterName.CompareTo(startShowUI) == 0)
            {
                _startShowUi = parameter.intValue;
                continue;
            }
            if (parameter.parameterName.CompareTo(endShowUI) == 0)
            {
                _enterShowUi = parameter.intValue;
                continue;
            }
        }
    }

    //public SkillConfigSkillPhaseType PhaseType
    //{
    //    get { return _phaseType; }
    //}

    public static CustomIdentification CustomIdentification()
    {
        return _customIdentification;
    }

    private void StartShowUI()
    {
        SetUIActive(_startShowUi);
    }

    private void EndShowUI()
    {
        SetUIActive(_enterShowUi);
    }

    private void SetUIActive(int type)
    {
        if (type <= 0)
        {
            return;
        }
        //if (_phaseType != SkillConfigSkillPhaseType.NONE)
        //{
        //    return;
        //}

        //for (int i = 1; i <= _uiCount; ++i)
        //{
        //    int index = i;
        //    bool value = ((1 << i) & type) > 0;
        //    SkillEventHandler.Instance.SkillOperationShowUI( index, value);
        //}
    }

    //public void SetSkill(Skill skill)
    //{
    //    _skill = skill;
    //}

    public override void DoAction(HSMState toState)
    {
        //StateTool.ChangeState(toState, _skill);
    }
}
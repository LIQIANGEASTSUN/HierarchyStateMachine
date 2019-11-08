using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;

public class StateSkill : HSMState
{
    private static CustomIdentification _customIdentification = new CustomIdentification("技能状态节点", IDENTIFICATION.SKILL_STATE, typeof(StateSkill), NODE_TYPE.STATE);
    private SkillConfigSkillPhaseType _phaseType = SkillConfigSkillPhaseType.NONE;

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
    }

    public override int Execute(ref bool result)
    {
        result = false;
        int toStateId = base.Execute(ref result);
        if (result)
        {
            _iAction.DoAction(toStateId);
        }
        
        return toStateId;
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void SetState()
    {
        for (int i = 0; i < _parameterList.Count; ++i)
        {
            HSMParameter parameter = _parameterList[i];
            if (parameter.parameterName == "Skill_State")
            {
                int value = parameter.intValue;
                _phaseType = (SkillConfigSkillPhaseType)parameter.intValue;

                Debug.LogError("PhaseType:" + _phaseType);
                break;
            }
        }
    }

    public SkillConfigSkillPhaseType PhaseType
    {
        get { return _phaseType; }
    }

    public static CustomIdentification CustomIdentification()
    {
        return _customIdentification;
    }

}

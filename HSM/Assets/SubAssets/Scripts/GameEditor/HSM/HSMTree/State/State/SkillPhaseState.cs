using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;
using GenPB;


public class SkillPhaseState : HSMState
{
    public static CustomIdentification _customIdentification = new CustomIdentification("技能/技能状态", IDENTIFICATION.SKILL_PHASE_STATE, typeof(SkillPhaseState), NODE_TYPE.STATE);

    //private Skill _skill;
    //private SkillConfigSkillPhaseType _phaseType = SkillConfigSkillPhaseType.NONE;

    public SkillPhaseState() : base()
    {

    }

    public override void Init()
    {
        SetState();
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.LogError("SkillPhaseState Enter:" + NodeId);
    }

    public override int Execute(ref bool result)
    {
        return base.Execute(ref result);
    }

    public override void Exit()
    {
        //Debug.LogError("SkillPhaseState Exit:" + NodeId);
    }

    private void SetState()
    {
        for (int i = 0; i < _parameterList.Count; ++i)
        {
            SkillHsmConfigHSMParameter parameter = _parameterList[i];
            if (parameter.ParameterName.CompareTo(StateTool.Skill_State) == 0)
            {
                int value = parameter.IntValue;
                //_phaseType = (SkillConfigSkillPhaseType)parameter.IntValue;
                continue;
            }
        }
    }

    //public SkillConfigSkillPhaseType PhaseType
    //{
    //    get { return _phaseType; }
    //}

    //public void SetSkill(Skill skill)
    //{
    //    _skill = skill;
    //}

    public override void ChangeToThisState()
    {
        //Debug.LogError("ChangeToThisState: currentState:" + ((null != currentState ? currentState.NodeId : -1)) + "   toNodeId:" + NodeId + "     PhaseType:" + PhaseType  + "    time:" + Extend.GameUtils.GetMillisecond());
        int skillConfigIndex = 0;
        if (null != _parentSubMachine)
        {
            skillConfigIndex = ((SubMachineSkill)_parentSubMachine).SkillConfigIndex;
        }
        //_skill.ChangeToState(skillConfigIndex, PhaseType);
    }

}

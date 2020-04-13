using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;
using GenPB;

public class SubMachineSkill : HSMSubStateMachine
{
    //public static CustomIdentification _customIdentification = new CustomIdentification("技能节点", IDENTIFICATION.SKILL_SUB_MACHINE, typeof(SubMachineSkill), NODE_TYPE.SUB_STATE_MACHINE);
    private int _skillConfigIndex = -1;
    //private Skill _skill;

    public SubMachineSkill():base(NODE_TYPE.SUB_STATE_MACHINE)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override int Execute(ref bool result)
    {
        return base.Execute(ref result);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Init()
    {
        base.Init();
        SetState();
    }

    private void SetState()
    {
        for (int i = 0; i < _parameterList.Count; ++i)
        {
            SkillHsmConfigHSMParameter parameter = _parameterList[i];
            if (parameter.ParameterName.CompareTo(StateTool.SkillConfig) == 0)
            {
                _skillConfigIndex = parameter.IntValue;
                break;
            }
        }
    }

    public int SkillConfigIndex
    {
        get { return _skillConfigIndex; }
    }

    //public void SetSkill(Skill skill)
    //{
    //    _skill = skill;
    //}
}

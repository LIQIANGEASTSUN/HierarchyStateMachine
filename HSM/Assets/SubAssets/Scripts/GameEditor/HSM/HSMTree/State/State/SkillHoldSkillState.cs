using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;
using GenPB;

public class SkillHoldSkillState : HSMState
{
    public static CustomIdentification _customIdentification = new CustomIdentification("技能/技能挂起", IDENTIFICATION.SKILL_HOLD, typeof(SkillHoldSkillState), NODE_TYPE.STATE);
    //private Skill _skill;

    public SkillHoldSkillState() : base()
    {
        AutoTransition = true;
    }

    public override void Init()
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.LogError("SkillHoldState Enter:" + NodeId);
        //_skill.HoldSkill = true;
    }

    public override int Execute(ref bool result)
    {
        return base.Execute(ref result);
    }

    public override void Exit()
    {
        //Debug.LogError("SkillHoldState Exit:" + NodeId);
    }

    //public void SetSkill(Skill skill)
    //{
    //    _skill = skill;
    //}
}

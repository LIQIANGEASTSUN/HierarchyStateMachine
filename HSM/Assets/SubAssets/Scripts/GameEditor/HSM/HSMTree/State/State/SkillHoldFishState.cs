using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;
using GenPB;

public class SkillHoldFishState : HSMState
{
    public static CustomIdentification _customIdentification = new CustomIdentification("技能/鱼挂起", IDENTIFICATION.FISH_HOLD, typeof(SkillHoldFishState), NODE_TYPE.STATE);
    //private Skill _skill;

    public SkillHoldFishState() : base()
    {
        AutoTransition = true;
    }

    public override void Init()
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.LogError("SkillHoldFishState Enter:" + NodeId);
        //_skill.FishHold = true;
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

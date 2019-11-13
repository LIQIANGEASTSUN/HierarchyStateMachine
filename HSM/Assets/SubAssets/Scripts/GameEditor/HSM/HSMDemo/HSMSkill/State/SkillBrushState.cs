using HSMTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if A
public class SkillBrushState : HSMState, ISkill
{
    private static CustomIdentification _customIdentification = new CustomIdentification("技能/刷子甩", IDENTIFICATION.SKILL_SWING_BRUSH, typeof(SkillBrushState), NODE_TYPE.STATE);

    private Skill _skill;

    public SkillBrushState():base()
    {
    }

    public override void Init()
    {
        AutoTranision = true;
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

    public static CustomIdentification CustomIdentification()
    {
        return _customIdentification;
    }

    public void SetSkill(Skill skill)
    {
        _skill = skill;
    }

    public override void DoAction(HSMState toState)
    {
        StateTool.ChangeState(toState, _skill);
        _skill.AbilityHSM.ConditionCheck.SetParameter("GenericBtn", 1);

        if (null != _skill.AttackSpecial && null != _skill.AttackSpecial.AbilityHSM)
        {
            _skill.AttackSpecial.AbilityHSM.ConditionCheck.SetParameter("GenericBtn", 1);
        }
    }

}

#endif
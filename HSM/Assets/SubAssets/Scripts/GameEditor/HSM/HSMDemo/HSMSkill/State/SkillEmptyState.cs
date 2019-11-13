using HSMTree;
using UnityEngine;
using System;

public class SkillEmptyState : HSMState
{
    private static CustomIdentification _customIdentification = new CustomIdentification("技能/空状态节点", IDENTIFICATION.SKILL_EMPTY, typeof(SkillEmptyState), NODE_TYPE.STATE);

    private Action EnterCallBack = null;
    public SkillEmptyState() : base()
    {

    }

    public override void Init()
    {
        AutoTranision = true;
    }

    public override void Enter()
    {
        base.Enter();
        if (null != EnterCallBack)
        {
            EnterCallBack();
        }
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

    public void SetEnterCallBack(Action callBack)
    {
        EnterCallBack = callBack;
    }

    public override void DoAction(HSMState toState)
    {
    }
}

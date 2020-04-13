using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;

public class SkillNotifyState : HSMState
{
    public static CustomIdentification _customIdentification = new CustomIdentification("技能/技能通知", IDENTIFICATION.SKILL_NOTIFY, typeof(SkillNotifyState), NODE_TYPE.STATE);

    public SkillNotifyState() : base()
    {
        AutoTransition = true;
    }

    public override void Init()
    {

    }

    public override void Enter()
    {
        base.Enter();
        //UnityEngine.Debug.LogError("SkillNotifyState Enter: 能量不足:" + NodeId);
        //SkillEventHandler.Instance.SendSkillEneryNotEnougth();
    }

    /// <summary>
    /// 执行节点抽象方法
    /// </summary>
    /// <returns>返回执行结果</returns>
    public override int Execute(ref bool result)
    {
        int toStateId = -1;
        toStateId = base.Execute(ref result);
        return toStateId;
    }

    public override void Exit()
    {
        //UnityEngine.Debug.LogError("SkillNotifyState Exit: 能量不足");
    }
}

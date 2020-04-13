using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;
using System;

public class SkillExitState : HSMState
{
    public static CustomIdentification _customIdentification = new CustomIdentification("技能/技能退出", IDENTIFICATION.SKILL_EXIT_STATE, typeof(SkillExitState), NODE_TYPE.STATE);
    public Action<AbstractNode> _enterCallBack = null;

    public SkillExitState() : base()
    {
        AutoTransition = true;
    }

    public override void Init()
    {

    }

    public override void Enter()
    {
        base.Enter();
        //UnityEngine.Debug.LogError("SkillExitState Enter: 技能退出");
        if (null != _enterCallBack)
        {
            _enterCallBack(this);
        }
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
        //UnityEngine.Debug.LogError("SkillExitState Exit: 技能退出");
    }

    public void SetEnterCallBack(Action<AbstractNode> callBack)
    {
        _enterCallBack = callBack;
    }

}

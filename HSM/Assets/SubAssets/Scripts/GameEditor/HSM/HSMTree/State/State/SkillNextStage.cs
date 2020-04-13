using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;
using System;

public class SkillNextStage : HSMState
{
    public static CustomIdentification _customIdentification = new CustomIdentification("技能/技能下一阶段", IDENTIFICATION.SKILL_NEXT_STAGE, typeof(SkillNextStage), NODE_TYPE.STATE);
    public Action<AbstractNode> _enterCallBack = null;

    public SkillNextStage() : base()
    {
        AutoTransition = true;
    }

    public override void Init()
    {

    }

    public override void Enter()
    {
        base.Enter();
        //UnityEngine.Debug.LogError("SkillNextStage Enter: 下一个配置文件");
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
        //UnityEngine.Debug.LogError("SkillNextStage Exit: 下一个配置文件");
    }

    public void SetEnterCallBack(Action<AbstractNode> callBack)
    {
        _enterCallBack = callBack;
    }

}

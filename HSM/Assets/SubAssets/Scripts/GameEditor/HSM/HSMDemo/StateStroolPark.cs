using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;

/// <summary>
/// 逛公园
/// </summary>
public class StateStroolPark : HSMState, IHuman
{
    private Human human;
    public StateStroolPark() : base()
    {
        AutoTransition = true;
    }

    public override void Enter()
    {
        base.Enter();

        Debug.LogError("开始逛公园");
    }

    public override void Exit()
    {
        Debug.LogError("结束逛公园");
    }

    public override void Init()
    {
    }

    public void SetHuman(Human human)
    {
        this.human = human;
    }

    public override void DoAction(AbstractNode node)
    {
        //Debug.LogError("StroolPark DoAction:" + node.NodeId + "    " + NodeId);
        human.StroolPark();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;
using GenPB;

/// <summary>
/// 移动到某处
/// </summary>
public class StateMoveTo : HSMState, IHuman
{
    private Human human;
    public StateMoveTo() : base()
    {
        AutoTransition = true;
    }

    public override void Enter()
    {
        base.Enter();

        Debug.LogError("移动到某处");
    }

    public override void Exit()
    {
        Debug.LogError("移动到某处");
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

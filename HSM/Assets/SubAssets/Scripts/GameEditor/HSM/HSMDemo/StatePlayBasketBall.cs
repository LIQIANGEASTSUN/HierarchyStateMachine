using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;


/// <summary>
/// 打篮球
/// </summary>
public class StatePlayBasketBall : HSMState, IHuman
{
    private Human human;
    public StatePlayBasketBall() : base()
    {
        AutoTransition = true;
    }

    public override void Enter()
    {
        base.Enter();

        Debug.LogError("开始打篮球");
    }

    public override void Exit()
    {
        Debug.LogError("结束打篮球");
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
        //Debug.LogError("PlayBasketBall DoAction:" + node.NodeId + "    " + NodeId);
        human.PlayBasketBall();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;


/// <summary>
/// 打篮球
/// </summary>
public class StatePlayBasketBall : HSMState
{
    public StatePlayBasketBall() : base()
    {

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
}

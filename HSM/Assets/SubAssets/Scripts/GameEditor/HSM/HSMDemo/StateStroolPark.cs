using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;

/// <summary>
/// 逛公园
/// </summary>
public class StateStroolPark : HSMState
{

    public StateStroolPark() : base()
    {

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
}

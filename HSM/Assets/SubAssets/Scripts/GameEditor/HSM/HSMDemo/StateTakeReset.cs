using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;

public class StateTakeReset : HSMState
{

    public StateTakeReset() : base()
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.LogError("开始休息");
    }

    public override void Exit()
    {
        Debug.LogError("结束休息");
    }

    public override void Init()
    {
    }
}

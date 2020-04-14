﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;

public class StateTakeReset : HSMState, IHuman
{
    private Human human;

    public StateTakeReset() : base()
    {
        AutoTransition = true;
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

    public void SetHuman(Human human)
    {
        this.human = human;
    }

    public override void DoAction(AbstractNode node)
    {
        //Debug.LogError("TakeReset DoAction:" + node.NodeId + "    " + NodeId);
        human.ResetEnergy();
    }
}

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
    private int moveTo;
    public StateMoveTo() : base()
    {
        AutoTransition = true;
    }

    public override void Enter()
    {
        base.Enter();

        for (int i = 0; i < _parameterList.Count; ++i)
        {
            SkillHsmConfigHSMParameter parameter = _parameterList[i];
            if (parameter.ParameterName.CompareTo("MoveTo") == 0)
            {
                moveTo = parameter.IntValue;
            }
        }
        Debug.LogError("移动到某处:" + moveTo);
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
        human.MoveTo(moveTo);
    }
}

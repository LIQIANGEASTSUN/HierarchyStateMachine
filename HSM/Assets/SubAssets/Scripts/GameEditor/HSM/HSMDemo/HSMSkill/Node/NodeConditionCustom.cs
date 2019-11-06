using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;

public class NodeConditionCustom : StateBase
{
    protected List<HSMParameter> _parameterList = new List<HSMParameter>();
    protected IConditionCheck _iconditionCheck = null;
    private static CustomIdentification _customIdentification = new CustomIdentification("通用条件节点", IDENTIFICATION.COMMON_CONDITION, typeof(NodeConditionCustom), NODE_TYPE.STATE);

    public NodeConditionCustom() : base(NODE_TYPE.STATE)
    {

    }

    public override void Execute()
    {
        NodeNotify.NotifyExecute(NodeId, Time.realtimeSinceStartup);
        bool result = _iconditionCheck.Condition(_parameterList);
        //ResultType resultType = result ? ResultType.Success : ResultType.Fail;
        //return resultType;
    }

    public static CustomIdentification CustomIdentification()
    {
        return _customIdentification;
    }

    public void SetConditionCheck(IConditionCheck iConditionCheck)
    {
        _iconditionCheck = iConditionCheck;
    }

    public void SetParameters(List<HSMParameter> parameterList)
    {
        if (parameterList.Count > 0)
        {
            _parameterList.AddRange(parameterList);
        }
    }
}
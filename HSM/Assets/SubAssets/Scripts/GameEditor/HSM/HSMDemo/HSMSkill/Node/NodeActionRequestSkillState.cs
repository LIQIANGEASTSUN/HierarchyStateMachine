using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;

public class NodeActionRequestSkillState : StateBase
{
    protected IAction iAction;
    protected List<HSMParameter> _parameterList = new List<HSMParameter>();

    private static CustomIdentification _customIdentification = new CustomIdentification("切换状态节点", IDENTIFICATION.SKILL_STATE_REQUEST, typeof(NodeActionRequestSkillState), NODE_TYPE.STATE);

    public NodeActionRequestSkillState():base(NODE_TYPE.STATE)
    {

    }

    public override ResultType Execute()
    {
        NodeNotify.NotifyExecute(NodeId, Time.realtimeSinceStartup);
        bool result = iAction.DoAction(NodeId, _parameterList);
        return result ? ResultType.Success : ResultType.Fail;
    }

    public static CustomIdentification CustomIdentification()
    {
        return _customIdentification;
    }

    public void SetIAction(IAction iA)
    {
        iAction = iA;
    }

    public void SetParameters(List<HSMParameter> parameterList)
    {
        if (parameterList.Count > 0)
        {
            _parameterList.AddRange(parameterList);
        }
    }
}

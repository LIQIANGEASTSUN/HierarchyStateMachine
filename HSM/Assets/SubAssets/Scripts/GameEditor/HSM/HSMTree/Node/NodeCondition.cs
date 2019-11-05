using System.Collections.Generic;

namespace HSMTree
{
    /// <summary>
    /// 条件节点(叶节点)
    /// </summary>
    public class NodeCondition : NodeLeaf
    {
        protected List<HSMParameter> _parameterList = new List<HSMParameter>();
        protected IConditionCheck _iconditionCheck = null;

        public NodeCondition() : base(NODE_TYPE.CONDITION)
        {
        }

        public override ResultType Execute()
        {
            return ResultType.Success;
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
}
using System.Collections.Generic;

namespace HSMTree
{
    /// <summary>
    /// 行为节点(叶节点)
    /// </summary>
    public abstract class NodeAction : NodeLeaf
    {
        protected IAction iAction;
        protected List<HSMParameter> _parameterList = new List<HSMParameter>();

        public NodeAction() : base(NODE_TYPE.ACTION)
        {
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

}
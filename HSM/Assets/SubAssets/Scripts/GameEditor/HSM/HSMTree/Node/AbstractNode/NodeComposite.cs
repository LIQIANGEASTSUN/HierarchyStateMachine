using System.Collections.Generic;

namespace HSMTree
{
    /// <summary>
    /// 组合节点
    /// </summary>
    public abstract class NodeComposite : StateBase
    {
        // 保存子节点
        protected List<StateBase> nodeChildList = new List<StateBase>();

        public NodeComposite(NODE_TYPE nodeType) : base(nodeType)
        {}

        public void AddNode(StateBase nodeRoot)
        {
            int count = nodeChildList.Count;
            nodeRoot.NodeIndex = count;
            nodeChildList.Add(nodeRoot);
        }

        public List<StateBase> GetChilds()
        {
            return nodeChildList;
        }

        public override ResultType Execute()
        {
            return ResultType.Fail;
        }
    }
}
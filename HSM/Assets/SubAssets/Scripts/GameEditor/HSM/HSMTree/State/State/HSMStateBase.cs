using GenPB;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{
    /// <summary>
    /// 节点超类
    /// </summary>
    public abstract class HSMStateBase : AbstractNode
    {

        public HSMStateBase(NODE_TYPE nodeType) : base()
        {
            this._nodeType = nodeType;
        }

        public override void Enter()
        {
            //Debug.LogError("Enter:" + NodeId);
        }

        /// <summary>
        /// 执行节点抽象方法
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override int Execute(ref bool result)
        {
            int toStateId = -1;
            toStateId = base.Execute(ref result);
            return toStateId;
        }

    }
}
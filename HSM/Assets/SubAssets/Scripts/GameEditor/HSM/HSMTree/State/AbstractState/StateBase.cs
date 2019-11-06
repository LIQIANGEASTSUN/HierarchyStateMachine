using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{
    /// <summary>
    /// 节点超类
    /// </summary>
    public abstract class StateBase
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        protected NODE_TYPE _nodeType;

        /// <summary>
        /// 节点Id
        /// </summary>
        private int _nodeId;

        protected List<Transition> _transitionList = new List<Transition>();

        protected IConditionCheck _iconditionCheck = null;

        public StateBase(NODE_TYPE nodeType)
        {
            this._nodeType = nodeType;
        }

        /// <summary>
        /// 执行节点抽象方法
        /// </summary>
        /// <returns>返回执行结果</returns>
        public virtual void Execute()
        {
            NodeNotify.NotifyExecute(NodeId, Time.realtimeSinceStartup);

            for (int i = 0; i < _transitionList.Count; ++i)
            {
                bool result = _iconditionCheck.Condition(_transitionList[i].parameterList);
            }
        }

        public int NodeId
        {
            get { return _nodeId; }
            set { _nodeId = value; }
        }

        public void AddTransition(List<Transition> transitionList)
        {
            _transitionList.AddRange(transitionList);
        }

        public void AddTransition(Transition transition)
        {
            _transitionList.Add(transition);
        }

        public void SetConditionCheck(IConditionCheck iConditionCheck)
        {
            _iconditionCheck = iConditionCheck;
        }

    }
}
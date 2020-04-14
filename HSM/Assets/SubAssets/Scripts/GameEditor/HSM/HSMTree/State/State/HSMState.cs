using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{
    public abstract class HSMState : AbstractNode
    {
        private bool _autoTransition = false;
        public HSMState():base()
        {

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

        public virtual void ChangeToThisState()
        {

        }

        public bool AutoTransition
        {
            get { return _autoTransition; }
            set { _autoTransition = value; }
        }

    }

}


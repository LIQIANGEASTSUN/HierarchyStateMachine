using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HSMTree
{
    public class HSMStateExit : HSMState
    {
        public static CustomIdentification _customIdentification = new CustomIdentification("Node/Exit", IDENTIFICATION.STATE_EXIT, typeof(HSMStateExit), NODE_TYPE.EXIT);

        public Action<AbstractNode> _enterCallBack = null;
        public HSMStateExit() : base()
        {

        }

        public override void Init()
        {

        }

        public override void Enter()
        {
            //Debug.LogError("HSMStateExit Enter:" + NodeId);
            base.Enter();

            if (null != _enterCallBack)
            {
                _enterCallBack(this);
            }
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

        public override void Exit()
        {
            //Debug.LogError("HSMStateExit Exit:" + NodeId);
        }

        public void SetEnterCallBack(Action<AbstractNode> callBack)
        {
            _enterCallBack = callBack;
        }

    }
}
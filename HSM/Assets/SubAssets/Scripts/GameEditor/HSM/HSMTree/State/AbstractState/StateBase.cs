using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{
    /// <summary>
    /// 节点超类
    /// </summary>
    public abstract class StateBase
    {
        protected HSMStateMachine _stateMachine;
        protected IAction _iAction;

        /// <summary>
        /// 节点类型
        /// </summary>
        protected NODE_TYPE _nodeType;

        /// <summary>
        /// 节点Id
        /// </summary>
        private int _stateId;
        protected List<HSMParameter> _parameterList = new List<HSMParameter>();
        protected List<Transition> _transitionList = new List<Transition>();
        protected IConditionCheck _iconditionCheck = null;

        public StateBase(NODE_TYPE nodeType)
        {
            this._nodeType = nodeType;
        }

        public abstract void Init();

        public virtual void Enter()
        {

        }

        /// <summary>
        /// 执行节点抽象方法
        /// </summary>
        /// <returns>返回执行结果</returns>
        public virtual int Execute(ref bool result)
        {
            result = false;
            NodeNotify.NotifyExecute(StateId, Time.realtimeSinceStartup);
            int toStateId = -1;
            for (int i = 0; i < _transitionList.Count; ++i)
            {
                Transition transition = _transitionList[i];
                result = _iconditionCheck.Condition(transition.parameterList);
                if (result)
                {
                    toStateId = transition.toStateId;
                    break;
                }
            }

            return toStateId;
        }

        public virtual void Exit()
        {

        }

        public int StateId
        {
            get { return _stateId; }
            set { _stateId = value; }
        }

        public void SetStateMachine(HSMStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void SetIAction(IAction iaction)
        {
            _iAction = iaction;
        }

        public void AddParameter(List<HSMParameter> parameterList)
        {
            _parameterList.AddRange(parameterList);
        }

        public void AddParameter(HSMParameter parameter)
        {
            _parameterList.Add(parameter);
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
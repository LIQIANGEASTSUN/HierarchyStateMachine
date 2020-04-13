using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenPB;

namespace HSMTree
{

    public abstract class AbstractNode
    {
        /// <summary>
        /// 节点Id
        /// </summary>
        protected int _nodeId;
        protected NODE_TYPE _nodeType;
        protected int _parentId;
        protected List<SkillHsmConfigHSMParameter> _parameterList = new List<SkillHsmConfigHSMParameter>();
        protected List<SkillHsmConfigTransition> _transitionList = new List<SkillHsmConfigTransition>();
        protected IConditionCheck _iconditionCheck = null;
        protected Dictionary<int, ConditionParameter> _conditionParameterDic = new Dictionary<int, ConditionParameter>();
        protected List<int> _childIdList = new List<int>();
        protected HSMSubStateMachine _parentSubMachine = null;

        public AbstractNode()
        {

        }

        public abstract void Init();

        public abstract void Enter();

        /// <summary>
        /// 执行节点抽象方法
        /// </summary>
        /// <returns>返回执行结果</returns>
        public virtual int Execute(ref bool result)
        {
            result = false;
#if UNITY_EDITOR
            NodeNotify.NotifyExecute(NodeId, Time.realtimeSinceStartup);
#endif
            int toStateId = -1;
            for (int i = 0; i < _transitionList.Count; ++i)
            {
                SkillHsmConfigTransition transition = _transitionList[i];
                ConditionParameter conditionParameter = null;
                if (_conditionParameterDic.TryGetValue(transition.TransitionId, out conditionParameter))
                {
                    result = _iconditionCheck.Condition(conditionParameter);
                }
                if (result)
                {
                    toStateId = transition.ToStateId;
                    break;
                }
            }

            return toStateId;
        }

        public abstract void Exit();

        public virtual void AddParameter(List<SkillHsmConfigHSMParameter> parameterList)
        {
            _parameterList.AddRange(parameterList);
        }

        public virtual void AddParameter(SkillHsmConfigHSMParameter parameter)
        {
            _parameterList.Add(parameter);
        }

        public virtual void AddTransition(List<SkillHsmConfigTransition> transitionList)
        {
            for (int i = 0; i < transitionList.Count; ++i)
            {
                AddTransition(transitionList[i]);
            }
        }

        public virtual void AddTransition(SkillHsmConfigTransition transition)
        {
            _transitionList.Add(transition);

            ConditionParameter conditionParametr = new ConditionParameter();
            conditionParametr.SetGroup(transition);
            _conditionParameterDic[transition.TransitionId] = conditionParametr;
        }

        public virtual void SetConditionCheck(IConditionCheck iConditionCheck)
        {
            _iconditionCheck = iConditionCheck;
        }

        public int NodeId
        {
            get { return _nodeId; }
            set { _nodeId = value; }
        }

        public NODE_TYPE NodeType
        {
            get { return _nodeType; }
            set { _nodeType = value; }
        }

        public int ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        public List<int> ChildIdList
        {
            get { return _childIdList; }
            set { _childIdList = value; }
        }

        public virtual void AddChildNode(AbstractNode node)
        {
        }

        public HSMSubStateMachine ParentSubMachine
        {
            get { return _parentSubMachine; }
        }

        public virtual void SetParentSubMachine(AbstractNode node)
        {
            _parentSubMachine = (HSMSubStateMachine)node;
        }
    }
}
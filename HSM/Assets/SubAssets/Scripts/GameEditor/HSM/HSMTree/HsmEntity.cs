using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenPB;

namespace HSMTree
{
    public class HsmEntity : IRegisterNode
    {
        private HSMStateMachine _hsmStateMachine = null;
        private IConditionCheck _iconditionCheck = null;
        private List<AbstractNode> _nodeList = new List<AbstractNode>();

        public HsmEntity(SkillHsmConfigHSMTreeData data)
        {
            _iconditionCheck = new ConditionCheck();
            HSMAnalysis analysis = new HSMAnalysis();
            analysis.Analysis(data, _iconditionCheck, this, ref _hsmStateMachine);
            if (null != _hsmStateMachine)
            {
                _hsmStateMachine.SetAutoTransitionState(false);
            }
        }

        public ConditionCheck ConditionCheck
        {
            get { return (ConditionCheck)_iconditionCheck; }
        }

        public List<AbstractNode> NodeList
        {
            get
            {
                return _nodeList;
            }
        }

        public void RegisterNode(AbstractNode node)
        {
            _nodeList.Add(node);
        }

        public void Execute()
        {
            if (null != _hsmStateMachine)
            {
                _hsmStateMachine.Execute();
            }
        }

        public void Clear()
        {
            ConditionCheck.InitParmeter();
        }
    }
}



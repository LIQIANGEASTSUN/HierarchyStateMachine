using System.Collections.Generic;

namespace HSMTree
{
    public class HSMStateMachine
    {
        private Dictionary<int, AbstractNode> _allNodeDic = new Dictionary<int, AbstractNode>();
        private HSMStateMachineTransition _iStateMachineTransition;

        public HSMStateMachine()
        {
            _iStateMachineTransition = new HSMStateMachineTransition();
        }

        public void Execute()
        {
            _iStateMachineTransition.Execute();
        }

        public void AddAllNode(Dictionary<int, AbstractNode> abstractNodeDic)
        {
            _allNodeDic = abstractNodeDic;
        }

        public void AddChildNode(AbstractNode node)
        {
            _iStateMachineTransition.AddNode(node);
        }

        public void SetAutoTransitionState(bool value)
        {
            _iStateMachineTransition.SetAutoTransitionState(value);
        }

        public void SetDefaultStateId(int id)
        {
            //_defaultStateId = id;
        }

        public void Clear()
        {
            _iStateMachineTransition.Clear();
        }

        public HSMStateMachineTransition StateMachineTransition
        {
            get { return _iStateMachineTransition; }
        }

        public AbstractNode GetNode(int nodeId)
        {
            AbstractNode node = null;
            if (_allNodeDic.TryGetValue(nodeId, out node))
            {
                return node;
            }

            return node;
        }

    }
}

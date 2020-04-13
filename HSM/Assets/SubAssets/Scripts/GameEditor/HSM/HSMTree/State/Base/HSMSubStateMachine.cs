using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenPB;

namespace HSMTree
{
    public class HSMSubStateMachine : AbstractNode
    {
        private HSMStateMachineTransition _iStateMachineTransition;

        public HSMSubStateMachine(NODE_TYPE nodeType) : base()
        {
            this._nodeType = nodeType;
            _iStateMachineTransition = new HSMStateMachineTransition();
        }

        public override void Init()
        {

        }

        public override void Enter()
        {

        }

        public override int Execute(ref bool result)
        {
            return base.Execute(ref result);
        }

        public override void Exit()
        {

        }

        public override void AddChildNode(AbstractNode node)
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

        public virtual void Clear()
        {
            _iStateMachineTransition.Clear();
        }

        public override void SetParentSubMachine(AbstractNode node)
        {
            _parentSubMachine = (HSMSubStateMachine)node;
        }

        public HSMStateMachineTransition StateMachineTransition
        {
            get { return _iStateMachineTransition; }
        }

    }
}


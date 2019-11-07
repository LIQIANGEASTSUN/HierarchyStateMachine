using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{
    public class HSMStateMachine
    {
        private int defaultStateId;
        private int currentStateId;
        private HSMState currentState;
        private List<HSMState> _stateList = new List<HSMState>();
        
        public HSMStateMachine()
        {

        }

        public void Execute()
        {
            if (null == currentState)
            {
                ChangeState(defaultStateId);
            }

            if (null != currentState)
            {
                bool result = false;
                int toStateId = currentState.Execute(ref result);
                if (result)
                {
                    ChangeState(toStateId);
                }
            }
        }

        public void AddState(HSMState state)
        {
            _stateList.Add(state);
        }

        public int CurrentStateId
        {
            get { return currentStateId; }
            set { currentStateId = value; }
        }

        public void SetDefaultStateId(int id)
        {
            defaultStateId = id;
        }

        public void ChangeState(int id)
        {
            HSMState newState = null;
            for (int i = 0; i < _stateList.Count; ++i)
            {
                HSMState state = _stateList[i];
                if (state.StateId == id)
                {
                    newState = state;
                    break;
                }
            }

            ChangeState(newState);
        }

        private void ChangeState(HSMState state)
        {
            if (null != currentState)
            {
                currentState.Exit();
            }

            currentState = state;
            if (null != currentState)
            {
                currentState.Enter();
            }
        }

        public void Clear()
        {
            if (null != currentState)
            {
                currentState.Exit();
            }
        }

    }
}

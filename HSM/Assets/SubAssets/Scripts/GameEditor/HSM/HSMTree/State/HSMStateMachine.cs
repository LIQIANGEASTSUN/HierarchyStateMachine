using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{
    public class HSMStateMachine
    {
        private HSMTreeData _data;
        private int defaultStateId;
        private int currentStateId;
        private HSMState currentState;
        private List<HSMState> _stateList = new List<HSMState>();
        private bool _autoTransitionState = true;
        
        public HSMStateMachine()
        {

        }

        public void Execute()
        {
            if (null == currentState)
            {
                ChangeState(defaultStateId);
            }

            if (null == currentState)
            {
                return;
            }

            bool result = false;
            int toStateId = currentState.Execute(ref result);
            if (currentState.StateId == 0)
            {

            }
            if (_autoTransitionState && result)
            {
                ChangeState(toStateId);
            }
        }

        public void AddState(HSMState state)
        {
            _stateList.Add(state);
        }

        public void SetAutoTransitionState(bool value)
        {
            _autoTransitionState = value;
        }

        public int CurrentStateId
        {
            get { return currentStateId; }
            set { currentStateId = value; }
        }

        public void SetData(HSMTreeData data)
        {
            _data = data;
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

            if (null == newState)
            {
                return;
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

        public HSMState GetState(int stateId)
        {
            HSMState state = null;
            for (int i = 0; i < _stateList.Count; ++i)
            {
                HSMState temp = _stateList[i];
                if (temp.StateId == stateId)
                {
                    state = temp;
                    break;
                }
            }

            return state;
        }

        public List<HSMState> StateList
        {
            get { return _stateList; }
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

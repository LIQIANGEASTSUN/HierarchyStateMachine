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

        private List<HSMSubStateMachine> _subMachineList = new List<HSMSubStateMachine>();

        private bool _autoTransitionState = true;
        
        public HSMStateMachine()
        {

        }

        public bool Execute()
        {
            if (null == currentState)
            {
                ChangeState(defaultStateId);
            }

            if (null == currentState)
            {
                return false;
            }

            bool result = false;
            int toStateId = currentState.Execute(ref result);
            if (result)
            {
                HSMState toState = GetState(toStateId);
                if (_autoTransitionState || toState.AutoTranision)
                {
                    ChangeState(toStateId);
                }
                else
                {
                    currentState.IAction.DoAction(currentState, toStateId);
                }
            }

            return result;
        }

        public void AddState(HSMState state)
        {
            _stateList.Add(state);
        }

        public void AddSubMachine(HSMSubStateMachine subStateMachine)
        {
            _subMachineList.Add(subStateMachine);
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

            //Debug.LogError("ChangeState:" + state.StateId);
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

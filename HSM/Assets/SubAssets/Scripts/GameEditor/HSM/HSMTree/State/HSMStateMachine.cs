﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{
    public class HSMStateMachine
    {
        private HSMState currentState;
        private List<HSMState> _stateList = new List<HSMState>();
        
        public HSMStateMachine()
        {

        }


        public void Execute()
        {
            if (null != currentState)
            {
                currentState.Execute();
            }
        }

        public void AddState(HSMState state)
        {
            _stateList.Add(state);
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

            if (null != state)
            {
                state.Enter();
            }
        }

    }
}
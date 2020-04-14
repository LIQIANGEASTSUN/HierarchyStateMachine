using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{
    public interface IStateMachineTransition
    {
        void Execute();

        void ChangeNode(int toNodeId);

        void ChangeState(int id);

        void ChangeSubMachine(int id);

        void SetAutoTransitionState(bool value);

        void Clear();

    }

    public class HSMStateMachineTransition : IStateMachineTransition
    {
        private bool _autoTransitionState = true;
        private HSMState _currentState;
        private HSMSubStateMachine _currentSubMachine = null;

        private HSMStateEntry _stateEntry;
        private HSMStateExit _stateExit;
        private Dictionary<int, HSMState> _stateDic = new Dictionary<int, HSMState>();
        private Dictionary<int, HSMSubStateMachine> _subMachineDic = new Dictionary<int, HSMSubStateMachine>();

        public void Execute()
        {
            bool result = false;
            int toNodeId = 0;

            if (null != _currentState)
            {
                toNodeId = _currentState.Execute(ref result);
                if (result)
                {
                    ChangeNode(toNodeId);
                }
            }

            if (null == _currentState && null == _currentSubMachine)
            {
                toNodeId = _stateEntry.Execute(ref result);
                if (result)
                {
                    ChangeNode(toNodeId);
                }
            }

            if (null != _currentSubMachine)
            {
                toNodeId = _currentSubMachine.Execute(ref result);
                if (result)
                {
                    ChangeNode(toNodeId);
                }
                else
                {
                    _currentSubMachine.StateMachineTransition.Execute();
                }
            }

        }

        public bool ChangeDestinationNode(HSMState state)
        {
            bool result = false;
            if (null == state)
            {
                return result;
            }

            if (_stateDic.ContainsKey(state.NodeId))
            {
                ChangeState(state.NodeId);
                result = true;
                return result;
            }
            else
            {
                HSMSubStateMachine parentSubMachine = state.ParentSubMachine;
                int toSubMachineId = -1;
                while (null != parentSubMachine)
                {
                    int id = parentSubMachine.NodeId;
                    if (_subMachineDic.ContainsKey(id))
                    {
                        toSubMachineId = id;
                        parentSubMachine = null;
                        break;
                    }

                    parentSubMachine = parentSubMachine.ParentSubMachine;
                }

                if (toSubMachineId >= 0)
                {
                    if (null != _currentSubMachine && _currentSubMachine.NodeId != toSubMachineId)
                    {
                        ChangeSubMachine(toSubMachineId);
                    }
                    if (null != _currentSubMachine && _currentSubMachine.NodeId == toSubMachineId)
                    {
                        result = _currentSubMachine.StateMachineTransition.ChangeDestinationNode(state);
                    }
                }
            }

            return result;
        }

        public void ChangeNode(int toNodeId)
        {
            // Debug.LogError("ChangeNode:" + toNodeId);
            if (IsState(toNodeId))
            {
                HSMState toState = GetState(toNodeId);
                bool needChange = (null == _currentState || _currentState.NodeId != toState.NodeId);
                if (needChange)
                {
                    if ((_autoTransitionState || toState.AutoTransition))
                    {
                        ChangeState(toNodeId);
                    }
                    else
                    {
                        toState.ChangeToThisState();
                    }
                }
            }
            else if (IsSubMachine(toNodeId))
            {
                bool needChange = ((null == _currentSubMachine) || (_currentSubMachine.NodeId != toNodeId));
                ChangeSubMachine(toNodeId);
            }
            else if (null != _stateExit && _stateExit.NodeId == toNodeId)
            {
                ChangeExit();
            }
        }

        public void ChangeState(int id)
        {
            HSMState newState = null;
            if (!_stateDic.TryGetValue(id, out newState))
            {
                return;
            }

            if (null == newState)
            {
                return;
            }

            if (null != _currentState)
            {
                _currentState.Exit();
            }

            _currentState = newState;
            _currentState.Enter();

            if (null != _currentSubMachine)
            {
                _currentSubMachine.Exit();
                _currentSubMachine = null;
            }
        }

        public void ChangeSubMachine(int id)
        {
            HSMSubStateMachine newSubMachine = null;
            if (!_subMachineDic.TryGetValue(id, out newSubMachine))
            {
                return;
            }

            if (null == newSubMachine)
            {
                return;
            }

            if (null != _currentSubMachine)
            {
                _currentSubMachine.Exit();
            }

            _currentSubMachine = newSubMachine;
            _currentSubMachine.Enter();

            if (null != _currentState)
            {
                _currentState.Exit();
                _currentState = null;
            }
        }

        private void ChangeExit()
        {
            ExitState();
            _stateExit.Enter();
        }

        private void ExitState()
        {
            if (null != _currentState)
            {
                _currentState.Exit();
                _currentState = null;
            }

            if (null != _currentSubMachine)
            {
                _currentSubMachine.Exit();
                _currentSubMachine = null;
            }
        }

        public HSMState GetState(int stateId)
        {
            HSMState state = null;
            if (_stateDic.TryGetValue(stateId, out state))
            {
                return state;
            }

            return state;
        }

        public void AddNode(AbstractNode node)
        {
            if (node.NodeType == NODE_TYPE.ENTRY)
            {
                _stateEntry = (HSMStateEntry)node;
            }
            else if (node.NodeType == NODE_TYPE.EXIT)
            {
                _stateExit = (HSMStateExit)node;
            }
            else if (node.NodeType == NODE_TYPE.STATE)
            {
                _stateDic[node.NodeId] = (HSMState)node;
            }
            else if (node.NodeType == NODE_TYPE.SUB_STATE_MACHINE)
            {
                _subMachineDic[node.NodeId] = (HSMSubStateMachine)node;
            }
        }

        public bool IsState(int stateId)
        {
            return _stateDic.ContainsKey(stateId);
        }

        public bool IsSubMachine(int stateId)
        {
            return _subMachineDic.ContainsKey(stateId);
        }

        public void SetAutoTransitionState(bool value)
        {
            _autoTransitionState = value;
            foreach(var kv in _subMachineDic)
            {
                kv.Value.SetAutoTransitionState(value);
            }
        }

        public void Clear()
        {
            foreach(var kv in _subMachineDic)
            {
                AbstractNode node = kv.Value;
                HSMSubStateMachine subMachine = (HSMSubStateMachine)node;
                //UnityEngine.Debug.LogError("Clear:" + subMachine.NodeId);
                subMachine.Clear();
            }

            ExitState();
        }

    }

}


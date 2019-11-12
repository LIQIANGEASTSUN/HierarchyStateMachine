using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HSMTree
{
    public class HSMSubStateMachine
    {
        private Dictionary<int, bool> _childStateList = new Dictionary<int, bool>();

        public HSMSubStateMachine()
        {

        }

        public void AddChildState(int childStateId)
        {
            _childStateList[childStateId] = true;
        }

        public bool IsChildNode(int childStateId)
        {
            bool value = false;
            _childStateList.TryGetValue(childStateId, out value);
            return value;
        }

    }
}


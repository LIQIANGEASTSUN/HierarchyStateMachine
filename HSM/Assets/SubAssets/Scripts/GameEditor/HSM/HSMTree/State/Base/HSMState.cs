using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{
    public abstract class HSMState : HSMStateBase
    {
        private bool _autoTransition = false;
        public HSMState():base(NODE_TYPE.STATE)
        {

        }

        public virtual void ChangeToThisState()
        {

        }

        public bool AutoTransition
        {
            get { return _autoTransition; }
            set { _autoTransition = value; }
        }

    }

}


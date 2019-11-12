using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{
    public abstract class HSMState : StateBase
    {
        private bool _autoTranision = false;

        public HSMState():base(NODE_TYPE.STATE)
        {

        }

        public bool AutoTranision
        {
            get { return _autoTranision; }
            set { _autoTranision = value; }
        }

        public virtual void DoAction(int toStateId)
        {

        }

        public virtual void DoAction(HSMState state)
        {

        }
    }

}


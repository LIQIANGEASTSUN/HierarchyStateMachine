using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{
    public abstract class HSMState : StateBase
    {
        public HSMState():base(NODE_TYPE.STATE)
        {

        }

    }

}


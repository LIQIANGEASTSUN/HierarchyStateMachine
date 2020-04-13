using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{
    public interface IAction
    {
        void DoAction(int skillConfigIndex, HSMState toState);
    }

    public interface IRegisterNode
    {
        void RegisterNode(AbstractNode node);
    }

}


using System.Collections;
using System.Collections.Generic;

namespace HSMTree
{
    public interface IAction
    {
        bool DoAction(int nodeId, List<HSMParameter> parameterList);

        bool DoAction(int nodeId, HSMParameter parameter);
    }
}



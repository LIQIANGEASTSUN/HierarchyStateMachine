using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{

    public interface IConditionCheck
    {
        void AddParameter(List<HSMParameter> parameterList);

        void SetParameter(string parameterName, bool boolValue);

        void SetParameter(string parameterName, float floatValue);

        void SetParameter(string parameterName, int intValue);

        void SetParameter(HSMParameter parameter);

        bool Condition(HSMParameter parameter);

        bool Condition(List<HSMParameter> parameterList);

        List<HSMParameter> GetAllParameter();
    }

}


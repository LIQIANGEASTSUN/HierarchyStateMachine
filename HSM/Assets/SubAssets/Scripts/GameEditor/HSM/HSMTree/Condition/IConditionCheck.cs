using GenPB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{

    public interface IConditionCheck
    {
        void InitParmeter();

        void AddParameter(List<SkillHsmConfigHSMParameter> parameterList);

        void SetParameter(string parameterName, bool boolValue);

        void SetParameter(string parameterName, float floatValue);

        void SetParameter(string parameterName, int intValue);

        void SetParameter(SkillHsmConfigHSMParameter parameter);

        bool Condition(SkillHsmConfigHSMParameter parameter);

        bool ConditionAllAnd(List<SkillHsmConfigHSMParameter> parameterList);

        bool Condition(ConditionParameter conditionParameter);

        List<SkillHsmConfigHSMParameter> GetAllParameter();
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenPB;

public class TransitionGroupParameter
{
    public List<SkillHsmConfigHSMParameter> parameterList = new List<SkillHsmConfigHSMParameter>();

    public void AddParameter(SkillHsmConfigHSMParameter parameter)
    {
        parameterList.Add(parameter);
    }

}

public class ConditionParameter
{
    public List<TransitionGroupParameter> groupList = new List<TransitionGroupParameter>();

    public ConditionParameter()
    {

    }

    public void SetGroup(SkillHsmConfigTransition transition)
    {
        for (int i = 0; i < transition.GroupList.Count; ++i)
        {
            SkillHsmConfigTransitionGroup transitionGroup = transition.GroupList[i];
            TransitionGroupParameter group = GetParameter(transitionGroup, transition.ParameterList);
            groupList.Add(group);
        }
    }

    private TransitionGroupParameter GetParameter(SkillHsmConfigTransitionGroup transitionGroup, List<SkillHsmConfigHSMParameter> parameterList)
    {
        TransitionGroupParameter group = new TransitionGroupParameter();
        for (int i = 0; i < transitionGroup.ParameterList.Count; ++i)
        {
            string parameter = transitionGroup.ParameterList[i];
            for (int j = 0; j < parameterList.Count; ++j)
            {
                if (parameter.CompareTo(parameterList[j].ParameterName) == 0)
                {
                    group.parameterList.Add(parameterList[j]);
                }
            }
        }

        return group;
    }

}

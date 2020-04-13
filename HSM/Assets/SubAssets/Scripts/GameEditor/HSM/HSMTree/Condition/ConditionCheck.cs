using GenPB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{

    public class ConditionCheck : IConditionCheck
    {
        //存储所有能用到的参数，数据来源于配置，在Init时候存到_environmentParameterDic中去，如果除了初始化时没别的地方用到，可以省略
        private List<SkillHsmConfigHSMParameter> _parameterList = new List<SkillHsmConfigHSMParameter>();
        //缓存当前行为树使用到的所有参数类型,保存当前世界状态中所有参数动态变化的值
        private Dictionary<string, SkillHsmConfigHSMParameter> _environmentParameterDic = new Dictionary<string, SkillHsmConfigHSMParameter>();

        public ConditionCheck()
        {

        }

        public void SetParameter(string parameterName, bool boolValue)
        {
            SkillHsmConfigHSMParameter parameter = null;
            if (!_environmentParameterDic.TryGetValue(parameterName, out parameter)) // 当前行为树不需要的参数值就不保存了
            {
                return;
            }

            if (parameter.ParameterType == (int)HSMParameterType.Bool)
            {
                parameter.BoolValue = boolValue;
                _environmentParameterDic[parameterName] = parameter;
            }
        }

        public void SetParameter(string parameterName, float floatValue)
        {
            SkillHsmConfigHSMParameter parameter = null;
            if (!_environmentParameterDic.TryGetValue(parameterName, out parameter)) // 当前行为树不需要的参数值就不保存了
            {
                return;
            }

            if (parameter.ParameterType == (int)HSMParameterType.Float)
            {
                parameter.FloatValue = floatValue;
                _environmentParameterDic[parameterName] = parameter;
            }
        }

        public void SetParameter(string parameterName, int intValue)
        {
            SkillHsmConfigHSMParameter parameter = null;
            if (!_environmentParameterDic.TryGetValue(parameterName, out parameter)) // 当前行为树不需要的参数值就不保存了
            {
                return;
            }

            if (parameter.ParameterType == (int)HSMParameterType.Int)
            {
                parameter.IntValue = intValue;
                _environmentParameterDic[parameterName] = parameter;
            }
        }

        public void SetParameter(SkillHsmConfigHSMParameter parameter)
        {
            SkillHsmConfigHSMParameter cacheParameter = null;
            if (!_environmentParameterDic.TryGetValue(parameter.ParameterName, out cacheParameter)) // 当前行为树不需要的参数值就不保存了
            {
                return;
            }

            if (parameter.ParameterType != cacheParameter.ParameterType)
            {
                Debug.LogError("parameter type invalid:" + parameter.ParameterName);
                return;
            }

            cacheParameter.CloneFrom(parameter);
            _environmentParameterDic[parameter.ParameterName] = cacheParameter;
        }

        //将配置好的Parameter存到环境dic中
        public void InitParmeter()
        {
            for (int i = 0; i < _parameterList.Count; ++i)
            {
                SkillHsmConfigHSMParameter parameter = _parameterList[i];

                SkillHsmConfigHSMParameter cacheParaemter = null;
                if (!_environmentParameterDic.TryGetValue(parameter.ParameterName, out cacheParaemter))
                {
                    cacheParaemter = parameter.Clone();
                    _environmentParameterDic[parameter.ParameterName] = cacheParaemter;
                }

                cacheParaemter.IntValue = parameter.IntValue;
                cacheParaemter.FloatValue = parameter.FloatValue;
                cacheParaemter.BoolValue = parameter.BoolValue;
            }
        }

        public void AddParameter(List<SkillHsmConfigHSMParameter> parameterList)
        {
            _parameterList.AddRange(parameterList);
            InitParmeter();
        }


        public bool Condition(SkillHsmConfigHSMParameter parameter)
        {
            SkillHsmConfigHSMParameter environmentParameter = null;
            if (!_environmentParameterDic.TryGetValue(parameter.ParameterName, out environmentParameter))
            {
                return false;
            }

            if (environmentParameter.ParameterType != parameter.ParameterType)
            {
                Debug.LogError("parameter Type not Equal:" + environmentParameter.ParameterName + "    " + environmentParameter.ParameterType + "    " + parameter.ParameterType);
                return false;
            }

            HSMCompare hsmCompare = environmentParameter.Compare(parameter);
            return (parameter.Compare & (int)hsmCompare) > 0;
        }

        public bool ConditionAllAnd(List<SkillHsmConfigHSMParameter> parameterList)
        {
            bool result = true;
            for (int i = 0; i < parameterList.Count; ++i)
            {
                SkillHsmConfigHSMParameter temp = parameterList[i];
                bool value = Condition(temp);
                if (!value)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public bool Condition(ConditionParameter conditionParameter)
        {
            bool result = true;
            for (int i = 0; i < conditionParameter.groupList.Count; ++i)
            {
                TransitionGroupParameter groupParameter = conditionParameter.groupList[i];
                result = true;

                for (int j = 0; j < groupParameter.parameterList.Count; ++j)
                {
                    SkillHsmConfigHSMParameter parameter = groupParameter.parameterList[j];
                    bool value = Condition(parameter);
                    if (!value)
                    {
                        result = false;
                        break;
                    }
                }

                if (result)
                {
                    break;
                }
            }

            return result;
        }

        public List<SkillHsmConfigHSMParameter> GetAllParameter()
        {
            List<SkillHsmConfigHSMParameter> parameterList = new List<SkillHsmConfigHSMParameter>();
            foreach (var kv in _environmentParameterDic)
            {
                parameterList.Add(kv.Value);
            }

            return parameterList;
        }

    }
}



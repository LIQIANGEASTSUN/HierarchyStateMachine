using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{

    public class ConditionCheck : IConditionCheck
    {
        // 缓存当前行为树使用到的所有参数类型,保存当前世界状态中所有参数动态变化的值
        private Dictionary<string, HSMParameter> _environmentParameterDic = new Dictionary<string, HSMParameter>();

        public ConditionCheck()
        {

        }

        public void SetParameter(string parameterName, bool boolValue)
        {
            HSMParameter parameter = null;
            if (!_environmentParameterDic.TryGetValue(parameterName, out parameter)) // 当前行为树不需要的参数值就不保存了
            {
                return;
            }

            if (parameter.parameterType == (int)HSMParameterType.Bool)
            {
                parameter.boolValue = boolValue;
                _environmentParameterDic[parameterName] = parameter;
            }
        }

        public void SetParameter(string parameterName, float floatValue)
        {
            HSMParameter parameter = null;
            if (!_environmentParameterDic.TryGetValue(parameterName, out parameter)) // 当前行为树不需要的参数值就不保存了
            {
                return;
            }

            if (parameter.parameterType == (int)HSMParameterType.Float)
            {
                parameter.floatValue = floatValue;
                _environmentParameterDic[parameterName] = parameter;
            }
        }

        public void SetParameter(string parameterName, int intValue)
        {
            HSMParameter parameter = null;
            if (!_environmentParameterDic.TryGetValue(parameterName, out parameter)) // 当前行为树不需要的参数值就不保存了
            {
                return;
            }

            if (parameter.parameterType == (int)HSMParameterType.Int)
            {
                parameter.intValue = intValue;
                _environmentParameterDic[parameterName] = parameter;
            }
        }

        public void SetParameter(HSMParameter parameter)
        {
            HSMParameter cacheParameter = null;
            if (!_environmentParameterDic.TryGetValue(parameter.parameterName, out cacheParameter)) // 当前行为树不需要的参数值就不保存了
            {
                return;
            }

            if (parameter.parameterType != cacheParameter.parameterType)
            {
                Debug.LogError("parameter type invalid:" + parameter.parameterName);
                return;
            }

            cacheParameter.CloneFrom(parameter);
            _environmentParameterDic[parameter.parameterName] = cacheParameter;
        }

        public void AddParameter(List<HSMParameter> parameterList)
        {
            for (int i = 0; i < parameterList.Count; ++i)
            {
                HSMParameter parameter = parameterList[i];
                if (_environmentParameterDic.ContainsKey(parameter.parameterName))
                {
                    continue;
                }

                _environmentParameterDic[parameter.parameterName] = parameter.Clone();
                _environmentParameterDic[parameter.parameterName].intValue = parameter.intValue;
                _environmentParameterDic[parameter.parameterName].floatValue = parameter.floatValue;
                _environmentParameterDic[parameter.parameterName].boolValue = parameter.boolValue;
            }
        }

        public bool Condition(HSMParameter parameter)
        {
            HSMParameter environmentParameter = null;
            if (!_environmentParameterDic.TryGetValue(parameter.parameterName, out environmentParameter))
            {
                return false;
            }

            if (environmentParameter.parameterType != parameter.parameterType)
            {
                Debug.LogError("parameter Type not Equal:" + environmentParameter.parameterName + "    " + environmentParameter.parameterType + "    " + parameter.parameterType);
                return false;
            }

            HSMCompare hsmCompare = environmentParameter.Compare(parameter);
            return (parameter.compare & (int)hsmCompare) > 0;
        }

        public bool Condition(List<HSMParameter> parameterList)
        {
            bool result = true;
            for (int i = 0; i < parameterList.Count; ++i)
            {
                HSMParameter temp1 = parameterList[i];
                bool value = Condition(temp1);
                if (!temp1.useGroup)
                {
                    if (!value)
                    {
                        result = value;
                        break;
                    }
                    continue;
                }

                #region UseGroup
                if (value)
                {
                    continue;
                }

                result = false;
                // 下面至少有一个为 true, result 才为 true
                for (int j = 0; j < parameterList.Count; ++j)
                {
                    HSMParameter temp2 = parameterList[j];
                    if (  !temp2.useGroup 
                        || temp2.orGroup != temp1.orGroup)
                    {
                        continue;
                    }

                    value = Condition(temp2);
                    if (value)
                    {
                        result = value;
                        break;
                    }
                }
                #endregion
            }

            return result;
        }

        public List<HSMParameter> GetAllParameter()
        {
            List<HSMParameter> parameterList = new List<HSMParameter>();
            foreach (var kv in _environmentParameterDic)
            {
                parameterList.Add(kv.Value);
            }

            return parameterList;
        }

    }
}



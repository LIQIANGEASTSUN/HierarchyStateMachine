using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;

public class ConditionCheck : IConditionCheck
{
    // 缓存当前行为树使用到的所有参数类型,保存当前世界状态中所有参数动态变化的值
    private Dictionary<string, HSMParameter> _allParameterDic = new Dictionary<string, HSMParameter>();

    public ConditionCheck()
    {

    }

    public void SetParameter(string parameterName, bool boolValue)
    {
        HSMParameter parameter = null;
        if (!_allParameterDic.TryGetValue(parameterName, out parameter)) // 当前行为树不需要的参数值就不保存了
        {
            return;
        }

        if (parameter.parameterType == (int)HSMParameterType.Bool)
        {
            parameter.boolValue = boolValue;
            _allParameterDic[parameterName] = parameter;
        }
    }

    public void SetParameter(string parameterName, float floatValue)
    {
        HSMParameter parameter = null;
        if (!_allParameterDic.TryGetValue(parameterName, out parameter)) // 当前行为树不需要的参数值就不保存了
        {
            return;
        }

        if (parameter.parameterType == (int)HSMParameterType.Float)
        {
            parameter.floatValue = floatValue;
            _allParameterDic[parameterName] = parameter;
        }
    }

    public void SetParameter(string parameterName, int intValue)
    {
        HSMParameter parameter = null;
        if (!_allParameterDic.TryGetValue(parameterName, out parameter)) // 当前行为树不需要的参数值就不保存了
        {
            return;
        }

        if (parameter.parameterType == (int)HSMParameterType.Int)
        {
            parameter.intValue = intValue;
            _allParameterDic[parameterName] = parameter;
        }
    }

    public void SetParameter(HSMParameter parameter)
    {
        HSMParameter cacheParameter = null;
        if (!_allParameterDic.TryGetValue(parameter.parameterName, out cacheParameter)) // 当前行为树不需要的参数值就不保存了
        {
            return;
        }

        if (parameter.parameterType != cacheParameter.parameterType)
        {
            Debug.LogError("parameter type invalid:" + parameter.parameterName);
            return;
        }

        cacheParameter.CloneFrom(parameter);
        _allParameterDic[parameter.parameterName] = cacheParameter;
    }

    public void AddParameter(List<HSMParameter> parameterList)
    {
        for (int i = 0; i < parameterList.Count; ++i)
        {
            HSMParameter parameter = parameterList[i];
            if (_allParameterDic.ContainsKey(parameter.parameterName))
            {
                continue;
            }

            _allParameterDic[parameter.parameterName] = parameter.Clone();
            _allParameterDic[parameter.parameterName].intValue = parameter.intValue;
            _allParameterDic[parameter.parameterName].floatValue = parameter.floatValue;
            _allParameterDic[parameter.parameterName].boolValue = parameter.boolValue;
        }
    }

    public bool CompareParameter(HSMParameter parameter)
    {
        HSMParameter cacheParameter = null;
        if (!_allParameterDic.TryGetValue(parameter.parameterName, out cacheParameter))
        {
            return false;
        }

        if (cacheParameter.parameterType != parameter.parameterType)
        {
            Debug.LogError("parameter Type not Equal:" + cacheParameter.parameterName + "    " + cacheParameter.parameterType + "    " + parameter.parameterType);
            return false;
        }

        return cacheParameter.Compare(parameter);
    }

    public bool Condition(HSMParameter parameter)
    {
        return CompareParameter(parameter);
    }

    public bool Condition(List<HSMParameter> parameterList)
    {
        bool result = true;
        for (int i = 0; i < parameterList.Count; ++i)
        {
            HSMParameter parameter = parameterList[i];
            bool value = Condition(parameter);
            if (!value)
            {
                result = value;
                break;
            }
        }

        return result;
    }

    public List<HSMParameter> GetAllParameter()
    {
        List<HSMParameter> parameterList = new List<HSMParameter>();
        foreach(var kv in _allParameterDic)
        {
            parameterList.Add(kv.Value);
        }

        return parameterList;
    }

}

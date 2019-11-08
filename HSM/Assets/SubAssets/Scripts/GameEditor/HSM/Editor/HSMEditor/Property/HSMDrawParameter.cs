using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace HSMTree
{
    public enum HSMDrawParameterType
    {
        NODE_PARAMETER = 0,
        HSM_PARAMETER,
        HSM_PARAMETER_ADD,
        RUNTIME_PARAMETER,
    }

    public class HSMDrawParameter
    {
        public static HSMParameter Draw(HSMParameter HSMParameter, HSMDrawParameterType drawParameterType, Action DelCallBack)
        {
            if (null == HSMParameter)
            {
                return HSMParameter;
            }

            {
                string[] parameterNameArr = EnumNames.GetEnumNames<HSMParameterType>();
                int index = EnumNames.GetEnumIndex<HSMParameterType>((HSMParameterType)(HSMParameter.parameterType));
                HSMParameterType HSMParameterType = EnumNames.GetEnum<HSMParameterType>(index);

                bool enableChangeType = (drawParameterType == HSMDrawParameterType.HSM_PARAMETER_ADD);
                GUI.enabled = enableChangeType;
                {
                    index = EditorGUILayout.Popup(index, parameterNameArr);
                    HSMParameter.parameterType = (int)EnumNames.GetEnum<HSMParameterType>(index);
                    GUILayout.Space(5);
                }
                GUI.enabled = true;
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (drawParameterType == HSMDrawParameterType.NODE_PARAMETER)
                {
                    List<HSMParameter> parameterList = HSMManager.Instance.HSMTreeData.parameterList;
                    string[] parameterArr = new string[parameterList.Count];
                    int index = -1;
                    for (int i = 0; i < parameterList.Count; ++i)
                    {
                        HSMParameter p = parameterList[i];
                        parameterArr[i] = p.parameterName;
                        if (HSMParameter.parameterName.CompareTo(p.parameterName) == 0)
                        {
                            index = i;
                        }
                    }
                    
                    int result = EditorGUILayout.Popup(index, parameterArr, GUILayout.ExpandWidth(true));
                    if (result != index)
                    {
                        HSMParameter.parameterName = parameterArr[result];
                    }
                }
                else if (drawParameterType == HSMDrawParameterType.HSM_PARAMETER
                    || (drawParameterType == HSMDrawParameterType.HSM_PARAMETER_ADD)
                    || drawParameterType == HSMDrawParameterType.RUNTIME_PARAMETER)
                {
                    GUI.enabled = (drawParameterType == HSMDrawParameterType.HSM_PARAMETER_ADD);
                    HSMParameter.parameterName = EditorGUILayout.TextField(HSMParameter.parameterName);
                    GUI.enabled = true;
                }

                HSMCompare[] compareEnumArr = new HSMCompare[] { };
                if (HSMParameter.parameterType == (int)HSMParameterType.Int)
                {
                    compareEnumArr = new HSMCompare[] { HSMCompare.GREATER, HSMCompare.GREATER_EQUALS, HSMCompare.LESS_EQUAL, HSMCompare.LESS, HSMCompare.EQUALS, HSMCompare.NOT_EQUAL };
                }
                if (HSMParameter.parameterType == (int)HSMParameterType.Float)
                {
                    compareEnumArr = new HSMCompare[] { HSMCompare.GREATER, HSMCompare.LESS };
                }
                if (HSMParameter.parameterType == (int)HSMParameterType.Bool)
                {
                    compareEnumArr = new HSMCompare[] { HSMCompare.EQUALS, HSMCompare.NOT_EQUAL };
                }
                string[] compareArr = new string[compareEnumArr.Length];
                int compare = HSMParameter.compare;
                bool found = false;
                for (int i = 0; i < compareEnumArr.Length; ++i)
                {
                    string name = System.Enum.GetName(typeof(HSMCompare), compareEnumArr[i]);
                    compareArr[i] = name;
                    if ((HSMCompare)HSMParameter.compare == compareEnumArr[i])
                    {
                        compare = i;
                        found = true;
                    }
                }

                if (!found)
                {
                    compare = 0;
                }

                GUI.enabled = (drawParameterType != HSMDrawParameterType.HSM_PARAMETER) && (drawParameterType != HSMDrawParameterType.RUNTIME_PARAMETER);
                {
                    compare = EditorGUILayout.Popup(compare, compareArr, GUILayout.Width(65));
                    HSMParameter.compare = (int)(compareEnumArr[compare]);
                }
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            {
                GUI.enabled = true;// (drawParameterType != DrawParameterType.HSM_PARAMETER);
                {
                    if (HSMParameter.parameterType == (int)HSMParameterType.Int)
                    {
                        HSMParameter.intValue = EditorGUILayout.IntField("IntValue", HSMParameter.intValue);
                    }

                    if (HSMParameter.parameterType == (int)HSMParameterType.Float)
                    {
                        HSMParameter.floatValue = EditorGUILayout.FloatField("FloatValue", HSMParameter.floatValue);
                    }

                    if (HSMParameter.parameterType == (int)HSMParameterType.Bool)
                    {
                        HSMParameter.boolValue = EditorGUILayout.Toggle("BoolValue", HSMParameter.boolValue);
                    }
                }
                GUI.enabled = true;

                if (drawParameterType == HSMDrawParameterType.NODE_PARAMETER || drawParameterType == HSMDrawParameterType.HSM_PARAMETER)
                {
                    if (GUILayout.Button("Del"))
                    {
                        if (null != DelCallBack)
                        {
                            DelCallBack();
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return HSMParameter;
        }

    }
}



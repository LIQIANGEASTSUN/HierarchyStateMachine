using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using GenPB;
using System.Text.RegularExpressions;

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
        public static SkillHsmConfigHSMParameter Draw(SkillHsmConfigHSMParameter hSMParameter, HSMDrawParameterType drawParameterType, Action DelCallBack)
        {
            if (null == hSMParameter)
            {
                return hSMParameter;
            }

            EditorGUILayout.BeginHorizontal();
            {
                string[] parameterNameArr = EnumNames.GetEnumNames<HSMParameterType>();
                int index = EnumNames.GetEnumIndex<HSMParameterType>((HSMParameterType)(hSMParameter.ParameterType));
                HSMParameterType HSMParameterType = EnumNames.GetEnum<HSMParameterType>(index);

                bool enableChangeType = (drawParameterType == HSMDrawParameterType.HSM_PARAMETER_ADD);
                GUI.enabled = enableChangeType;
                {
                    GUI.enabled = false;
                    if (drawParameterType == HSMDrawParameterType.NODE_PARAMETER)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            hSMParameter.Index = EditorGUILayout.IntField(hSMParameter.Index, GUILayout.Width(30));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    GUI.enabled = enableChangeType;

                    index = EditorGUILayout.Popup(index, parameterNameArr, GUILayout.ExpandWidth(true));
                    hSMParameter.ParameterType = (int)EnumNames.GetEnum<HSMParameterType>(index);
                    GUILayout.Space(5);
                }
                GUI.enabled = true;

                if (drawParameterType == HSMDrawParameterType.NODE_PARAMETER || drawParameterType == HSMDrawParameterType.HSM_PARAMETER)
                {
                    if (GUILayout.Button("删除", GUILayout.Width(45)))
                    {
                        if (null != DelCallBack)
                        {
                            DelCallBack();
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if (drawParameterType == HSMDrawParameterType.NODE_PARAMETER)
                {
                    List<SkillHsmConfigHSMParameter> parameterList = HSMManager.Instance.HSMTreeData.ParameterList;
                    string[] parameterArr = new string[parameterList.Count];
                    int index = -1;
                    for (int i = 0; i < parameterList.Count; ++i)
                    {
                        SkillHsmConfigHSMParameter p = parameterList[i];
                        parameterArr[i] = p.CNName; //p.ParameterName;
                        if (hSMParameter.ParameterName.CompareTo(p.ParameterName) == 0)
                        {
                            index = i;
                        }
                    }
                    
                    int result = EditorGUILayout.Popup(index, parameterArr, GUILayout.ExpandWidth(true));
                    if (result != index)
                    {
                        hSMParameter.ParameterName = parameterList[result].ParameterName; //parameterArr[result];
                    }
                }
                else if (drawParameterType == HSMDrawParameterType.HSM_PARAMETER
                    || drawParameterType == HSMDrawParameterType.RUNTIME_PARAMETER)
                {
                    GUI.enabled = (drawParameterType == HSMDrawParameterType.HSM_PARAMETER_ADD);
                    hSMParameter.ParameterName = EditorGUILayout.TextField(hSMParameter.ParameterName);
                    hSMParameter.CNName = EditorGUILayout.TextField(hSMParameter.CNName);
                    GUI.enabled = true;
                }
                else if (drawParameterType == HSMDrawParameterType.HSM_PARAMETER_ADD)
                {
                    EditorGUILayout.BeginVertical();
                    {
                        string oldName = hSMParameter.ParameterName;
                        hSMParameter.ParameterName = EditorGUILayout.TextField("英文:", hSMParameter.ParameterName);
                        if (oldName.CompareTo(hSMParameter.ParameterName) != 0)
                        {
                            bool isNumOrAlp = IsNumOrAlp(hSMParameter.ParameterName);
                            if (!isNumOrAlp)
                            {
                                string msg = string.Format("参数名只能包含:数字、字母、下划线，且数字不能放在第一个字符位置");
                                HSMNodeWindow.window.ShowNotification(msg);
                                hSMParameter.ParameterName = oldName;
                            }
                        }

                        hSMParameter.CNName = EditorGUILayout.TextField("中文", hSMParameter.CNName);
                    }
                    EditorGUILayout.EndVertical();
                }
                HSMCompare[] compareEnumArr = new HSMCompare[] { };
                if (hSMParameter.ParameterType == (int)HSMParameterType.Int)
                {
                    compareEnumArr = new HSMCompare[] { HSMCompare.GREATER, HSMCompare.GREATER_EQUALS, HSMCompare.LESS_EQUAL, HSMCompare.LESS, HSMCompare.EQUALS, HSMCompare.NOT_EQUAL };
                }
                if (hSMParameter.ParameterType == (int)HSMParameterType.Float)
                {
                    compareEnumArr = new HSMCompare[] { HSMCompare.GREATER, HSMCompare.LESS };
                }
                if (hSMParameter.ParameterType == (int)HSMParameterType.Bool)
                {
                    compareEnumArr = new HSMCompare[] { HSMCompare.EQUALS, HSMCompare.NOT_EQUAL };
                }
                string[] compareArr = new string[compareEnumArr.Length];
                int compare = hSMParameter.Compare;
                bool found = false;
                for (int i = 0; i < compareEnumArr.Length; ++i)
                {
                    int index = EnumNames.GetEnumIndex<HSMCompare>(compareEnumArr[i]);
                    string typeName = EnumNames.GetEnumName<HSMCompare>(index);
                    compareArr[i] = typeName;
                    if ((HSMCompare)hSMParameter.Compare == compareEnumArr[i])
                    {
                        compare = i;
                        found = true;
                    }
                }

                if (!found)
                {
                    compare = 0;
                }

                bool value = (drawParameterType != HSMDrawParameterType.HSM_PARAMETER) && (drawParameterType != HSMDrawParameterType.RUNTIME_PARAMETER) && (drawParameterType != HSMDrawParameterType.HSM_PARAMETER_ADD);
                bool boolType = (hSMParameter.ParameterType == (int)HSMParameterType.Bool);
                if (boolType)
                {
                    hSMParameter.Compare = (int)HSMCompare.EQUALS;
                }
                else if (value)
                {
                    compare = EditorGUILayout.Popup(compare, compareArr, GUILayout.Width(65));
                    hSMParameter.Compare = (int)(compareEnumArr[compare]);
                }

                {
                    if (hSMParameter.ParameterType == (int)HSMParameterType.Int)
                    {
                        hSMParameter.IntValue = EditorGUILayout.IntField(hSMParameter.IntValue, GUILayout.Width(50));
                    }

                    if (hSMParameter.ParameterType == (int)HSMParameterType.Float)
                    {
                        hSMParameter.FloatValue = EditorGUILayout.FloatField( hSMParameter.FloatValue, GUILayout.Width(50));
                    }

                    if (hSMParameter.ParameterType == (int)HSMParameterType.Bool)
                    {
                        hSMParameter.BoolValue = EditorGUILayout.Toggle(hSMParameter.BoolValue, GUILayout.Width(50));
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return hSMParameter;
        }

        private static bool IsNumOrAlp(string str)
        {
            string pattern = @"^[a-zA-Z_][A-Za-z0-9_]*$";
            Match match = Regex.Match(str, pattern);
            return match.Success;
        }

    }
}



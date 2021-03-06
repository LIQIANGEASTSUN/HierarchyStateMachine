﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using GenPB;

namespace HSMTree
{

    public class HSMParameterController
    {
        private HSMParameterModel _parameterModel;
        private HSMParameterView _parameterView;

        public HSMParameterController()
        {
            Init();
        }

        public void Init()
        {
            _parameterModel = new HSMParameterModel();
            _parameterView = new HSMParameterView();
        }

        public void OnDestroy()
        {

        }

        public void OnGUI()
        {
            List<SkillHsmConfigHSMParameter> parameterList = _parameterModel.ParameterList;
            _parameterView.Draw(parameterList);
        }

    }

    public class HSMParameterModel
    {
        public HSMParameterModel()
        {
        }

        public List<SkillHsmConfigHSMParameter> ParameterList
        {
            get
            {
                return HSMManager.Instance.HSMTreeData.ParameterList;
            }
        }
    }

    public class HSMParameterView
    {

        private Vector2 scrollPos = Vector2.zero;
        public void Draw(List<SkillHsmConfigHSMParameter> parameterList)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("全部变量");
                GUILayout.Space(50);
                if (GUILayout.Button("导入变量"))
                {
                    HSMFileHandle.ImportParameter();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            {
                EditorGUILayout.LabelField("条件参数");
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
                {
                    GUI.backgroundColor = new Color(0.85f, 0.85f, 0.85f, 1f);
                    for (int i = 0; i < parameterList.Count; ++i)
                    {
                        SkillHsmConfigHSMParameter hsmParameter = parameterList[i];

                        Action DelCallBack = () =>
                        {
                            if (null != HSMManager.hSMNodeAddDelParameter)
                            {
                                HSMManager.parameterChange(hsmParameter, false);
                            }
                        };

                        EditorGUILayout.BeginVertical("box");
                        {
                            hsmParameter = HSMDrawParameter.Draw(hsmParameter, HSMDrawParameterType.HSM_PARAMETER, DelCallBack);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical("box");
            {
                DrawAddParameter();
            }
            EditorGUILayout.EndVertical();
        }

        private SkillHsmConfigHSMParameter newAddParameter = new SkillHsmConfigHSMParameter();
        private void DrawAddParameter()
        {
            if (null == newAddParameter)
            {
                newAddParameter = new SkillHsmConfigHSMParameter();
            }

            EditorGUILayout.BeginVertical("box");
            {
                newAddParameter = HSMDrawParameter.Draw(newAddParameter, HSMDrawParameterType.HSM_PARAMETER_ADD, null);
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);

            if (GUILayout.Button("添加条件"))
            {
                if (null != HSMManager.hSMNodeAddDelParameter)
                {
                    HSMManager.parameterChange(newAddParameter, true);
                }
            }
        }

    }

}


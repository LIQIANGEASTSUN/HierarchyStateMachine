using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

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
            List<HSMParameter> parameterList = _parameterModel.ParameterList;
            _parameterView.Draw(parameterList);
        }

    }

    public class HSMParameterModel
    {
        public HSMParameterModel()
        {
        }

        public List<HSMParameter> ParameterList
        {
            get
            {
                return HSMManager.Instance.HSMTreeData.parameterList;
            }
        }
    }

    public class HSMParameterView
    {

        private Vector2 scrollPos = Vector2.zero;
        public void Draw(List<HSMParameter> parameterList)
        {
            EditorGUILayout.LabelField("全部变量");

            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            {
                EditorGUILayout.LabelField("条件参数");
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
                {
                    GUI.backgroundColor = new Color(0.85f, 0.85f, 0.85f, 1f);
                    for (int i = 0; i < parameterList.Count; ++i)
                    {
                        HSMParameter HSMParameter = parameterList[i];

                        Action DelCallBack = () =>
                        {
                            if (null != HSMManager.hSMNodeParameter)
                            {
                                HSMManager.parameterChange(HSMParameter, false);
                            }
                        };

                        EditorGUILayout.BeginVertical("box");
                        {
                            HSMParameter = DrawParameter.Draw(HSMParameter, DrawParameterType.HSM_PARAMETER, DelCallBack);
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

        private HSMParameter newAddParameter = new HSMParameter();
        private void DrawAddParameter()
        {
            if (null == newAddParameter)
            {
                newAddParameter = new HSMParameter();
            }

            EditorGUILayout.BeginVertical("box");
            {
                newAddParameter = DrawParameter.Draw(newAddParameter, DrawParameterType.HSM_PARAMETER_ADD, null);
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);

            if (GUILayout.Button("添加条件"))
            {
                if (null != HSMManager.hSMNodeParameter)
                {
                    HSMManager.parameterChange(newAddParameter, true);
                }
            }
        }

    }

}


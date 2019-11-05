using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace HSMTree
{

    public class HSMRuntimeParameter
    {
        private HSMRuntimeParameterModel _runtimeParameterModel;
        private HSMRuntimeParameterView _runtimeParameterView;

        public HSMRuntimeParameter()
        {
            Init();
        }

        public void Init()
        {
            _runtimeParameterModel = new HSMRuntimeParameterModel();
            _runtimeParameterView = new HSMRuntimeParameterView();
        }

        public void OnDestroy()
        {

        }

        public void OnGUI()
        {
            if (_runtimeParameterModel.ParameterList.Count <= 0)
            {
                List<HSMParameter> parameterList = HSMRunTime.Instance.ConditionCheck.GetAllParameter();
                _runtimeParameterModel.AddParameter(parameterList);
            }

            _runtimeParameterView.Draw(_runtimeParameterModel.ParameterList);
        }

    }

    public class HSMRuntimeParameterModel
    {
        private List<HSMParameter> _parameterList = new List<HSMParameter>();

        public HSMRuntimeParameterModel()
        {
        }

        public void AddParameter(List<HSMParameter> parameterList)
        {
            _parameterList = parameterList;
        }

        public List<HSMParameter> ParameterList
        {
            get
            {
                return _parameterList;
            }
        }

    }

    public class HSMRuntimeParameterView
    {
        private Vector2 scrollPos = Vector2.zero;
        public void Draw(List<HSMParameter> parameterList)
        {
            EditorGUILayout.LabelField("运行时变量");

            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            {
                EditorGUILayout.LabelField("条件参数");
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
                {
                    GUI.backgroundColor = new Color(0.85f, 0.85f, 0.85f, 1f);
                    for (int i = 0; i < parameterList.Count; ++i)
                    {
                        HSMParameter HSMParameter = parameterList[i];
                        EditorGUILayout.BeginVertical("box");
                        {
                            HSMParameter = DrawParameter.Draw(HSMParameter, DrawParameterType.RUNTIME_PARAMETER, null);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

    }

}






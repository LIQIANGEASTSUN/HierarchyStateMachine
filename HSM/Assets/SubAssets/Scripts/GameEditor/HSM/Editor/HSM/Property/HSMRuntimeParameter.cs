using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using GenPB;

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
            HSMManager.hSMRuntimePlay += RuntimePlay;
        }

        public void OnDestroy()
        {
            HSMManager.hSMRuntimePlay -= RuntimePlay;
        }

        public void OnGUI()
        {
            _runtimeParameterView.Draw(_runtimeParameterModel.ParameterList);
        }

        private void RuntimePlay(HSMPlayType state)
        {
            if (state == HSMPlayType.PLAY && null != HSMRunTime.Instance.ConditionCheck)
            {
                List<SkillHsmConfigHSMParameter> parameterList = HSMRunTime.Instance.ConditionCheck.GetAllParameter();
                _runtimeParameterModel.AddParameter(parameterList);
            }
        }
    }

    public class HSMRuntimeParameterModel
    {
        private List<SkillHsmConfigHSMParameter> _parameterList = new List<SkillHsmConfigHSMParameter>();

        public HSMRuntimeParameterModel()
        {
        }

        public void AddParameter(List<SkillHsmConfigHSMParameter> parameterList)
        {
            _parameterList = parameterList;
        }

        public List<SkillHsmConfigHSMParameter> ParameterList
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
        public void Draw(List<SkillHsmConfigHSMParameter> parameterList)
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
                        SkillHsmConfigHSMParameter HSMParameter = parameterList[i];
                        EditorGUILayout.BeginVertical("box");
                        {
                            HSMParameter = HSMDrawParameter.Draw(HSMParameter, HSMDrawParameterType.RUNTIME_PARAMETER, null);
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






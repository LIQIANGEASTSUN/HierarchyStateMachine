using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GenPB;

namespace HSMTree
{
    public class HSMDescriptController
    {
        private HSMDescriptModel _descriptModel;
        private HSMDescriptView _descriptView;

        public HSMDescriptController()
        {
            Init();
        }

        public void Init()
        {
            _descriptModel = new HSMDescriptModel();
            _descriptView = new HSMDescriptView();
        }

        public void OnDestroy()
        {

        }

        public void OnGUI()
        {
            SkillHsmConfigHSMTreeData data = _descriptModel.GetData();
            _descriptView.Draw(data);
        }

    }


    public class HSMDescriptModel
    {
        public SkillHsmConfigHSMTreeData GetData()
        {
            return HSMManager.Instance.HSMTreeData;
        }
    }

    public class HSMDescriptView
    {

        public void Draw(SkillHsmConfigHSMTreeData data)
        {
            Rect rect = GUILayoutUtility.GetRect(0f, 0, GUILayout.ExpandWidth(true));
            EditorGUILayout.BeginVertical("box");
            {
                data.Descript = EditorGUILayout.TextArea(data.Descript, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            EditorGUILayout.EndVertical();
        }

    }


}
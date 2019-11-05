using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
            HSMTreeData data = _descriptModel.GetData();
            _descriptView.Draw(data);
        }

    }


    public class HSMDescriptModel
    {
        public HSMTreeData GetData()
        {
            return HSMManager.Instance.HSMTreeData;
        }
    }

    public class HSMDescriptView
    {

        public void Draw(HSMTreeData data)
        {
            Rect rect = GUILayoutUtility.GetRect(0f, 0, GUILayout.ExpandWidth(true));
            EditorGUILayout.BeginVertical("box");
            {
                data.descript = EditorGUILayout.TextArea(data.descript, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            EditorGUILayout.EndVertical();
        }

    }


}
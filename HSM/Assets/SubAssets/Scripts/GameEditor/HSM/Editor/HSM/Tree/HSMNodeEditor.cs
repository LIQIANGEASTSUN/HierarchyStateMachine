using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GenPB;

namespace HSMTree
{
    public class HSMNodeEditor
    {

        private static int height = 35;

        private static Color GetColor(SkillHsmConfigNodeData nodeValue, int selectNodeId, int defaultStateId)
        {
            Color color = Color.white;

            if (nodeValue.NodeType == (int)NODE_TYPE.ENTRY)
            {
                color = (Color)(new Color32(32, 181, 12, 255));
            }

            if (nodeValue.NodeType == (int)NODE_TYPE.EXIT)
            {
                color = (Color)(new Color32(180, 15, 11, 255));
            }

            if (nodeValue.Id == selectNodeId)
            {
                color = new Color(0, 1, 0, 0.15f);
            }

            if (nodeValue.Id == defaultStateId)
            {
                color = (Color)(new Color32(207, 156, 5, 255));
            }

            return color;
        }

        public static void Draw(SkillHsmConfigNodeData nodeValue, int selectNodeId, int defaultStateId, float value = 0f)
        {
            EditorGUILayout.BeginVertical("box");
            {
                GUI.backgroundColor = GetColor(nodeValue, selectNodeId, defaultStateId);
                //if (nodeValue.Id == selectNodeId)
                {
                    GUI.Box(new Rect(5, 20, nodeValue.Position.Width - 10, height), string.Empty);
                }
                GUI.backgroundColor = Color.white;

                GUIStyle guiStyle = new GUIStyle();
                //guiStyle.fontSize = 20;
                guiStyle.fontStyle = FontStyle.Bold;
                guiStyle.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField(nodeValue.NodeName, guiStyle);//EditorGUILayout.TextField(nodeValue.Descript);
                //GUILayout.Space(5);

                float slider = NodeNotify.NodeDraw(nodeValue.Id);
                Rect runRect = GUILayoutUtility.GetRect(0, 8, GUILayout.ExpandWidth(true));
                GUI.Box(runRect, string.Empty);
                if (slider > 0)
                {
                    runRect.width = runRect.width * slider;
                    GUI.backgroundColor = Color.green;
                    GUI.Box(runRect, string.Empty);
                    GUI.backgroundColor = Color.white;
                }
            }
            EditorGUILayout.EndVertical();
        }

        public static string GetTitle(NODE_TYPE nodeType)
        {
            int index = EnumNames.GetEnumIndex<NODE_TYPE>(nodeType);
            string title = EnumNames.GetEnumName<NODE_TYPE>(index);
            return title;
        }
    }
}

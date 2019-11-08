using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace HSMTree
{
    public class HSMNodeEditor
    {

        private static int height = 75;

        private static Color GetColor(int nodeId, int selectNodeId, int defaultStateId)
        {
            Color color = Color.white;
            if (nodeId == selectNodeId)
            {
                color = new Color(0, 1, 0, 0.15f);
            }

            if (nodeId == defaultStateId)
            {
                color = (Color)(new Color32(207, 156, 5, 255));
            }

            return color;
        }

        public static void Draw(NodeData nodeValue, int selectNodeId, int defaultStateId, float value = 0f)
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Height(height));
            {
                GUI.backgroundColor = GetColor(nodeValue.id, selectNodeId, defaultStateId);
                //if (nodeValue.id == selectNodeId)
                {
                    GUI.Box(new Rect(5, 20, nodeValue.position.width - 10, height), string.Empty);
                }
                GUI.backgroundColor = Color.white;

                nodeValue.descript = EditorGUILayout.TextArea(nodeValue.descript);

                {
                    float slider = NodeNotify.NodeDraw(nodeValue.id);

                    Rect runRect = new Rect(8, 80, nodeValue.position.width - 16, 8);
                    GUI.Box(runRect, string.Empty);
                    if (slider > 0)
                    {
                        runRect.width = runRect.width * slider;
                        GUI.backgroundColor = Color.green;
                        GUI.Box(runRect, string.Empty);
                        GUI.backgroundColor = Color.white;
                    }
                }

                //GUILayout.HorizontalSlider(value, 0, 1);
            }
            EditorGUILayout.EndVertical();

            SetHight(nodeValue);
        }

        private static void SetHight(NodeData nodeValue)
        {
            RectT rect = nodeValue.position;
            rect.height = 95;
            nodeValue.position = rect;
        }

        public static string GetTitle(NODE_TYPE nodeType)
        {
            int index = EnumNames.GetEnumIndex<NODE_TYPE>(nodeType);
            string title = EnumNames.GetEnumName<NODE_TYPE>(index);
            return title;
        }
    }
}

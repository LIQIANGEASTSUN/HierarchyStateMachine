using GenPB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class HSMTransitionGroup
{
    private static int nodeId = -1;
    private static int selectIndex = -1;
    public static SkillHsmConfigTransitionGroup DrawTransitionGroup(SkillHsmConfigNodeData nodeData, SkillHsmConfigTransition transition)
    {
        if (null == transition)
        {
            return null;
        }

        SkillHsmConfigTransitionGroup group = null;
        for (int i = 0; i < transition.GroupList.Count; ++i)
        {
            SkillHsmConfigTransitionGroup transitionGroup = transition.GroupList[i];
            SkillHsmConfigTransitionGroup temp = DrawGroup(nodeData, transition, transitionGroup);
            if (null != temp)
            {
                group = temp;
            }
        }

        return group;
    }

    private static SkillHsmConfigTransitionGroup DrawGroup(SkillHsmConfigNodeData nodeData, SkillHsmConfigTransition transition, SkillHsmConfigTransitionGroup group)
    {
        Rect area = GUILayoutUtility.GetRect(0f, 1, GUILayout.ExpandWidth(true));
        bool select = (selectIndex == group.Index);

        EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
        {
            if (selectIndex < 0 || nodeId < 0 || nodeId != nodeData.Id)
            {
                nodeId = nodeData.Id;
                selectIndex = group.Index;
            }

            Rect rect = new Rect(area.x, area.y, area.width, 30);
            GUI.backgroundColor = select ? new Color(0, 1, 0, 1) : Color.white;// 
            GUI.Box(rect, string.Empty);
            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("选择", GUILayout.Width(50)))
            {
                selectIndex = group.Index;
            }

            for (int i = group.ParameterList.Count - 1; i >= 0; --i)
            {
                string parameter = group.ParameterList[i];
                SkillHsmConfigHSMParameter hSMParameter = transition.ParameterList.Find(a => (a.ParameterName.CompareTo(parameter) == 0));
                if (null == hSMParameter)
                {
                    group.ParameterList.RemoveAt(i);
                }
            }

            GUI.enabled = select;
            for (int i = 0; i < transition.ParameterList.Count; ++i)
            {
                SkillHsmConfigHSMParameter parameter = transition.ParameterList[i];
                string name = group.ParameterList.Find( a => (a.CompareTo(parameter.ParameterName) == 0));

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(10));
                    bool value = !string.IsNullOrEmpty(name);
                    bool oldValue = value;
                    value = EditorGUILayout.Toggle(value, GUILayout.Width(10));
                    if (value)
                    {
                        if (!oldValue)
                        {
                            group.ParameterList.Add(parameter.ParameterName);
                            break;
                        }
                    }
                    else
                    {
                        if (oldValue)
                        {
                            group.ParameterList.Remove(parameter.ParameterName);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            GUI.enabled = true;

            if (GUILayout.Button("删除"))
            {
                if (null != HSMManager.hSMNodeTransitionAddDelGroup)
                {
                    HSMManager.hSMNodeTransitionAddDelGroup(nodeData.Id, transition.TransitionId, group.Index, false);
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        if (select)
        {
            return group;
        }
        return null;
    }

}

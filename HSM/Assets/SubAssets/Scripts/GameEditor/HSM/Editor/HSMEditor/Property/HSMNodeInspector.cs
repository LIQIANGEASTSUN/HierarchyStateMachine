using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace HSMTree
{

    public class HSMNodeInspector
    {
        private HSMNodeInspectorModel _nodeInspectorModel;
        private HSMNodeInspectorView _nodeInspectorView;

        public HSMNodeInspector()
        {
            Init();
        }

        public void Init()
        {
            _nodeInspectorModel = new HSMNodeInspectorModel();
            _nodeInspectorView = new HSMNodeInspectorView();
        }

        public void OnDestroy()
        {

        }

        public void OnGUI()
        {
            NodeValue nodeValue = _nodeInspectorModel.GetCurrentSelectNode();
            _nodeInspectorView.Draw(nodeValue);
        }

    }


    public class HSMNodeInspectorModel
    {
        private int transitionId = -1;
        public NodeValue GetCurrentSelectNode()
        {
            return HSMManager.Instance.CurrentNode;
        }
    }

    
    public class HSMNodeInspectorView
    {

        public void Draw(NodeValue nodeValue)
        {
            if (null == nodeValue)
            {
                EditorGUILayout.LabelField("未选择节点");
                return;
            }

            EditorGUILayout.BeginVertical("box");
            {
                string nodeId = string.Format("节点_{0}", nodeValue.id);
                EditorGUILayout.LabelField(nodeId);

                if (nodeValue.NodeType == (int)NODE_TYPE.STATE
                    || nodeValue.NodeType == (int)NODE_TYPE.SUB_STATE_MACHINE)
                {
                    int index = EnumNames.GetEnumIndex<NODE_TYPE>((NODE_TYPE)nodeValue.NodeType);
                    string name = EnumNames.GetEnumName<NODE_TYPE>(index);
                    EditorGUILayout.LabelField(name);
                    GUILayout.Space(5);
                }

                string nodeName = nodeValue.nodeName;
                EditorGUILayout.LabelField(nodeName);

                if (nodeValue.identification > 0)
                {
                    string identificationName = string.Format("类标识_{0}", nodeValue.identification);
                    EditorGUILayout.LabelField(identificationName);

                    CustomIdentification customIdentification = CustomNode.Instance.GetIdentification((IDENTIFICATION)nodeValue.identification);
                    string className = customIdentification.Type.Name;
                    EditorGUILayout.LabelField(className);
                }

                nodeValue.descript = EditorGUILayout.TextArea(nodeValue.descript, GUILayout.Height(50));
            }
            EditorGUILayout.EndVertical();

            DrawNode(nodeValue, "参数");
        }

        private Vector2 scrollPos = Vector2.zero;
        private void DrawNode(NodeValue nodeValue, string title)
        {
            if (nodeValue.NodeType != (int)NODE_TYPE.STATE 
                && nodeValue.NodeType != (int)NODE_TYPE.SUB_STATE_MACHINE)
            {
                return;
            }

            Transition transition = null;
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("选择Transition查看/添加/删除参数");
                for (int i = 0; i < nodeValue.transitionList.Count; ++i)
                {
                    Transition temp = nodeValue.transitionList[i];
                    string name = string.Format("TransitionTo:{0}", temp.toStateId);
                    bool lastValue = (nodeValue.id * 1000 + temp.transitionId) == HSMManager.Instance.CurrentTransitionId;
                    if (lastValue)
                    {
                        transition = temp;
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        bool value = EditorGUILayout.Toggle(new GUIContent(name), lastValue);
                        if (value && !lastValue)
                        {
                            if (null != HSMManager.hSMChangeSelectTransitionId)
                            {
                                int id = nodeValue.id * 1000 + temp.transitionId;
                                HSMManager.hSMChangeSelectTransitionId(id);
                            }
                        }

                        if (GUILayout.Button("Delete"))
                        {
                            if (null != HSMManager.hSMNodeChangeTransition)
                            {
                                HSMManager.hSMNodeChangeTransition(nodeValue.id, temp.toStateId, false);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

            int transitionId = (null != transition) ? transition.transitionId : -1;

            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            {
                EditorGUILayout.LabelField(title);

                List<HSMParameter> parametersList = (null != transition) ? transition.parameterList : new List<HSMParameter>();
                int height = (parametersList.Count * 70) + 20;
                height = height <= 300 ? height : 300;
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(height));
                {
                    GUI.backgroundColor = new Color(0.85f, 0.85f, 0.85f, 1f);
                    for (int i = 0; i < parametersList.Count; ++i)
                    {
                        HSMParameter HSMParameter = parametersList[i];

                        Action DelCallBack = () =>
                        {
                            if (null != HSMManager.hSMNodeParameter)
                            {
                                HSMManager.hSMNodeParameter(nodeValue.id, transitionId, HSMParameter, false);
                            }
                        };

                        EditorGUILayout.BeginVertical("box");
                        {
                            string parameterName = HSMParameter.parameterName;

                            HSMParameter tempParameter = HSMParameter.Clone();
                            tempParameter = DrawParameter.Draw(HSMParameter, DrawParameterType.NODE_PARAMETER, DelCallBack);
                            if (parameterName.CompareTo(HSMParameter.parameterName) != 0)
                            {
                                if (null != HSMManager.hSMNodeChangeParameter)
                                {
                                    HSMManager.hSMNodeChangeParameter(nodeValue.id, transitionId, parameterName, HSMParameter.parameterName);
                                }
                            }
                            else
                            {
                                HSMParameter.CloneFrom(tempParameter);
                            }
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
                DrawAddParameter(nodeValue, transitionId);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawAddParameter(NodeValue nodeValue, int transitionId)
        {
            GUI.enabled = (transitionId >= 0);

            if (GUILayout.Button("添加条件"))
            {
                if (HSMManager.Instance.HSMTreeData.parameterList.Count <= 0)
                {
                    string msg = "没有参数可添加，请先添加参数";

                    if (TreeNodeWindow.window != null)
                    {
                        TreeNodeWindow.window.ShowNotification(msg);
                    }
                }
                else
                {
                    if (null != HSMManager.hSMNodeParameter)
                    {
                        HSMParameter hSMParameter = HSMManager.Instance.HSMTreeData.parameterList[0];
                        HSMManager.hSMNodeParameter(nodeValue.id, transitionId, hSMParameter, true);
                    }
                }
            }

            GUI.enabled = true;
        }

    }


}
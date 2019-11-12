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
            NodeData nodeValue = _nodeInspectorModel.GetCurrentSelectNode();
            _nodeInspectorView.Draw(nodeValue);
        }

    }


    public class HSMNodeInspectorModel
    {
        public NodeData GetCurrentSelectNode()
        {
            return HSMManager.Instance.CurrentNode;
        }
    }

    
    public class HSMNodeInspectorView
    {

        public void Draw(NodeData nodeValue)
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

            DrawNode(nodeValue);
        }

        private Dictionary<int, int> _groupColorDic = new Dictionary<int, int>(); // 
        private Color32[] colorArr = new Color32[] { new Color32(178, 226, 221, 255), new Color32(220, 226, 178, 255), new Color32(209, 178, 226, 255),  new Color32(178, 185, 226, 255) };
        private bool selectNodeParameter = false;
        private Vector2 scrollPos = Vector2.zero;
        private void DrawNode(NodeData nodeData)
        {
            if (nodeData.NodeType != (int)NODE_TYPE.STATE 
                && nodeData.NodeType != (int)NODE_TYPE.SUB_STATE_MACHINE)
            {
                return;
            }

            string selectTitle = string.Empty;
            GUI.backgroundColor = selectNodeParameter ? new Color(0, 1, 0, 1) : Color.white;// 
            EditorGUILayout.BeginVertical("box");
            {
                selectNodeParameter = EditorGUILayout.Toggle(new GUIContent("NodeParameter"), selectNodeParameter);
                if (selectNodeParameter && HSMManager.Instance.CurrentTransitionId >= 0 && null != HSMManager.hSMChangeSelectTransitionId)
                {
                    HSMManager.hSMChangeSelectTransitionId(-1);
                }
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;

            Transition transition = null;
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("选择Transition查看/添加/删除参数");
                for (int i = 0; i < nodeData.transitionList.Count; ++i)
                {
                    Transition temp = nodeData.transitionList[i];
                    string name = string.Format("TransitionParameter:{0}", temp.toStateId);
                    bool lastValue = (nodeData.id * 1000 + temp.transitionId) == HSMManager.Instance.CurrentTransitionId;
                    if (lastValue)
                    {
                        transition = temp;
                        selectTitle = string.Format("{0} 参数", name);
                    }

                    GUI.backgroundColor = lastValue ? new Color(0, 1, 0, 1) : Color.white;// 
                    EditorGUILayout.BeginHorizontal("box");
                    {
                        GUIStyle guiStyle = new GUIStyle();
                        guiStyle.normal.textColor = lastValue ? Color.green : Color.black;
                        bool value = EditorGUILayout.Toggle(new GUIContent(name), lastValue);
                        if (value && !lastValue)
                        {
                            selectNodeParameter = false;
                            if (null != HSMManager.hSMChangeSelectTransitionId)
                            {
                                int id = nodeData.id * 1000 + temp.transitionId;
                                HSMManager.hSMChangeSelectTransitionId(id);
                            }
                        }

                        if (GUILayout.Button("Delete"))
                        {
                            if (null != HSMManager.hSMNodeChangeTransition)
                            {
                                HSMManager.hSMNodeChangeTransition(nodeData.id, temp.toStateId, false);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUI.backgroundColor = Color.white;
                }
            }
            EditorGUILayout.EndVertical();

            if (null != transition)
            {
                DrawTransitionParameter(nodeData, transition, selectTitle);
            }
            if (selectNodeParameter)
            {
                DrawNodeParameter(nodeData, selectTitle);
            }
        }

        private void DrawTransitionParameter(NodeData nodeData, Transition transition, string title)
        {
            EditorGUILayout.LabelField(title);
            EditorGUILayout.BeginVertical("box");
            {
                if (null != transition)
                {
                    NodeData toNode = HSMManager.Instance.GetNode(transition.toStateId);
                    string toNodeDescript = (null != toNode) ? toNode.descript : string.Empty;
                    string msg = string.Format("{0}_{1} -> {2}_{3}", nodeData.id, nodeData.descript, transition.toStateId, toNodeDescript);
                    EditorGUILayout.LabelField(msg);
                }
            }
            EditorGUILayout.EndVertical();

            int transitionId = (null != transition) ? transition.transitionId : -1;
            List<HSMParameter> parametersList = (null != transition) ? transition.parameterList : new List<HSMParameter>();

            Action<HSMParameter> DelCallBack = (hSMParameter) =>
            {
                if (null != HSMManager.hSMNodeAddDelParameter)
                {
                    HSMManager.hSMNodeAddDelParameter(nodeData.id, transitionId, hSMParameter, false);
                }
            };

            Action<string> ChangeParameter = (parameterName) =>
            {
                if (null != HSMManager.hSMNodeChangeParameter)
                {
                    HSMManager.hSMNodeChangeParameter(nodeData.id, transitionId, parameterName);
                }
            };

            DrawParameter(nodeData, parametersList, title, DelCallBack, ChangeParameter);

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical("box");
            {
                DrawTransitionAddParameter(nodeData, transitionId);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawNodeParameter(NodeData nodeData, string title)
        {
            EditorGUILayout.LabelField(title);
            List<HSMParameter> parametersList = nodeData.parameterList;

            Action<HSMParameter> DelCallBack = (hSMParameter) =>
            {
                if (null != HSMManager.hSMNodeAddDelParameter)
                {
                    HSMManager.hSMNodeAddDelParameter(nodeData.id, -1, hSMParameter, false);
                }
            };

            Action<string> ChangeParameter = (parameterName) =>
            {
                if (null != HSMManager.hSMNodeChangeParameter)
                {
                    HSMManager.hSMNodeChangeParameter(nodeData.id, -1, parameterName);
                }
            };

            DrawParameter(nodeData, parametersList, title, DelCallBack, ChangeParameter);

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical("box");
            {
                DrawNodeAddParameter(nodeData);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawParameter(NodeData nodeData, List<HSMParameter> parametersList, string title, Action<HSMParameter> delCallBack, Action<string> ChangeParameter)
        {
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            {
                int height = 0;
                for (int i = 0; i < parametersList.Count; ++i)
                {
                    HSMParameter parameter = parametersList[i];
                    height += (parameter.useGroup ? 85 : 50);
                }

                height = height <= 300 ? height : 300;
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(height));
                {
                    for (int i = 0; i < parametersList.Count; ++i)
                    {
                        HSMParameter hSMParameter = parametersList[i];

                        Action DelCallBack = () =>
                        {
                            if (null != delCallBack)
                            {
                                delCallBack(hSMParameter);
                            }
                        };

                        Color color = Color.white;
                        if (hSMParameter.useGroup)
                        {
                            int value = -1;
                            if (!_groupColorDic.TryGetValue(hSMParameter.orGroup, out value))
                            {
                                value = hSMParameter.orGroup % colorArr.Length;
                                _groupColorDic[hSMParameter.orGroup] = value;
                            }
                            if (value >= 0)
                            {
                                color = colorArr[value];
                            }
                        }

                        GUI.backgroundColor = color; // new Color(0.85f, 0.85f, 0.85f, 1f);
                        EditorGUILayout.BeginVertical("box");
                        {
                            string parameterName = hSMParameter.parameterName;

                            HSMParameter tempParameter = hSMParameter.Clone();
                            tempParameter = HSMDrawParameter.Draw(hSMParameter, HSMDrawParameterType.NODE_PARAMETER, DelCallBack);
                            if (parameterName.CompareTo(hSMParameter.parameterName) != 0)
                            {
                                if (null != ChangeParameter)
                                {
                                    ChangeParameter(hSMParameter.parameterName);
                                }
                            }
                            else
                            {
                                hSMParameter.CloneFrom(tempParameter);
                            }
                        }
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(5);
                        GUI.backgroundColor = Color.white;
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawTransitionAddParameter(NodeData nodeValue, int transitionId)
        {
            GUI.enabled = (transitionId >= 0);

            if (GUILayout.Button("添加条件"))
            {
                if (HSMManager.Instance.HSMTreeData.parameterList.Count <= 0)
                {
                    string msg = "没有参数可添加，请先添加参数";

                    if (HSMNodeWindow.window != null)
                    {
                        HSMNodeWindow.window.ShowNotification(msg);
                    }
                }
                else
                {
                    if (null != HSMManager.hSMNodeAddDelParameter)
                    {
                        HSMParameter hSMParameter = HSMManager.Instance.HSMTreeData.parameterList[0];
                        HSMManager.hSMNodeAddDelParameter(nodeValue.id, transitionId, hSMParameter, true);
                    }
                }
            }

            GUI.enabled = true;
        }

        private void DrawNodeAddParameter(NodeData nodeValue)
        {
            if (GUILayout.Button("添加条件"))
            {
                if (HSMManager.Instance.HSMTreeData.parameterList.Count <= 0)
                {
                    string msg = "没有参数可添加，请先添加参数";

                    if (HSMNodeWindow.window != null)
                    {
                        HSMNodeWindow.window.ShowNotification(msg);
                    }
                }
                else
                {
                    if (null != HSMManager.hSMNodeAddDelParameter)
                    {
                        HSMParameter hSMParameter = HSMManager.Instance.HSMTreeData.parameterList[0];
                        HSMManager.hSMNodeAddDelParameter(nodeValue.id, -1, hSMParameter, true);
                    }
                }
            }
        }

    }


}
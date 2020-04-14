using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using GenPB;

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
            SkillHsmConfigNodeData nodeValue = _nodeInspectorModel.GetCurrentSelectNode();
            _nodeInspectorView.Draw(nodeValue);
        }

    }


    public class HSMNodeInspectorModel
    {
        public SkillHsmConfigNodeData GetCurrentSelectNode()
        {
            return HSMManager.Instance.CurrentNode;
        }
    }

    
    public class HSMNodeInspectorView
    {

        public void Draw(SkillHsmConfigNodeData nodeValue)
        {
            if (null == nodeValue)
            {
                EditorGUILayout.LabelField("未选择节点");
                return;
            }

            EditorGUILayout.BeginVertical("box");
            {
                //string nodeId = string.Format("节点  ID:{0}", nodeValue.Id);
                //EditorGUILayout.LabelField(nodeId);

                int index = EnumNames.GetEnumIndex<NODE_TYPE>((NODE_TYPE)nodeValue.NodeType);
                string name = EnumNames.GetEnumName<NODE_TYPE>(index);
                name = string.Format("节点类型:{0}", name);
                EditorGUILayout.LabelField(name);

                string identification = string.Format("Identification:{0}", nodeValue.Identification);
                EditorGUILayout.LabelField(identification);

                if (nodeValue.Identification > 0)
                {
                    CustomIdentification customIdentification = CustomNode.Instance.GetIdentification(nodeValue.Identification);
                    string typeName = string.Format("类     型:{0}", customIdentification.Name);
                    EditorGUILayout.LabelField(typeName);

                    string className = string.Format("类     名:{0}", customIdentification.ClassType.Name);
                    EditorGUILayout.LabelField(className);

                    //string identificationName = string.Format("类标识  :{0}", nodeValue.Identification);
                    //EditorGUILayout.LabelField(identificationName);
                }
                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("节点名:", GUILayout.Width(50));
                    nodeValue.NodeName = EditorGUILayout.TextField(nodeValue.NodeName, GUILayout.ExpandWidth(true));
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("节点描述:");
                nodeValue.Descript = EditorGUILayout.TextArea(nodeValue.Descript, GUILayout.Height(30));
                GUILayout.Space(5);
            }
            EditorGUILayout.EndVertical();

            DrawNode(nodeValue);
        }

        private Dictionary<int, int> _groupColorDic = new Dictionary<int, int>(); // 
        private Color32[] colorArr = new Color32[] { new Color32(178, 226, 221, 255), new Color32(220, 226, 178, 255), new Color32(209, 178, 226, 255),  new Color32(178, 185, 226, 255) };
        private bool selectNodeParameter = false;
        private Vector2 scrollPos = Vector2.zero;
        private void DrawNode(SkillHsmConfigNodeData nodeData)
        {
            string selectTitle = string.Empty;

            int selectTransitionId = -1;
            bool selectTransition = SelectTransition(nodeData, ref selectTransitionId);

            selectNodeParameter = !selectTransition;
            GUI.backgroundColor = selectNodeParameter ? new Color(0, 1, 0, 1) : Color.white;// 
            EditorGUILayout.BeginVertical("box");
            {
                bool oldValue = selectNodeParameter;
                selectNodeParameter = EditorGUILayout.Toggle(new GUIContent("节点参数"), selectNodeParameter);
                if ((!oldValue && selectNodeParameter) && HSMManager.Instance.CurrentTransitionId >= 0 && null != HSMManager.hSMChangeSelectTransitionId)
                {
                    HSMManager.hSMChangeSelectTransitionId(-1);
                }
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;

            SkillHsmConfigTransition transition = null;
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("选择转换连线查看/添加/删除参数");
                for (int i = 0; i < nodeData.TransitionList.Count; ++i)
                {
                    SkillHsmConfigTransition temp = nodeData.TransitionList[i];
                    string name = string.Format("转换条件参数:连线ID_{0}", temp.ToStateId);
                    bool lastValue = (selectTransitionId == temp.TransitionId);
                    if (lastValue)
                    {
                        transition = temp;
                        selectTitle = name;//string.Format("{0} 参数", name);
                    }

                    GUI.backgroundColor = lastValue ? new Color(0, 1, 0, 1) : Color.white;// 
                    EditorGUILayout.BeginHorizontal("box");
                    {
                        GUIStyle guiStyle = new GUIStyle();
                        guiStyle.normal.textColor = lastValue ? Color.green : Color.black;
                        bool value = EditorGUILayout.Toggle(new GUIContent(name), lastValue);
                        if (value && !lastValue)
                        {
                            if (null != HSMManager.hSMChangeSelectTransitionId)
                            {
                                int id = nodeData.Id * 1000 + temp.TransitionId;
                                HSMManager.hSMChangeSelectTransitionId(id);
                            }
                        }

                        if (GUILayout.Button("复制"))
                        {
                            HSMManager.copyTransition = temp.Clone();
                        }

                        if (GUILayout.Button("粘贴"))
                        {
                            if(null != HSMManager.copyTransition)
                            {
                                HSMManager.copyTransition.TransitionId = temp.TransitionId;
                                HSMManager.copyTransition.ToStateId = temp.ToStateId;
                                temp = HSMManager.copyTransition.Clone();
                                nodeData.TransitionList[i] = temp;
                                HSMManager.copyTransition = null;
                            }
                        }

                        if (GUILayout.Button("删除"))
                        {
                            if (null != HSMManager.hSMNodeChangeTransition)
                            {
                                HSMManager.hSMNodeChangeTransition(nodeData.Id, temp.ToStateId, false);
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

        private bool SelectTransition(SkillHsmConfigNodeData nodeData, ref int id)
        {
            bool selectTransition = false;
            for (int i = 0; i < nodeData.TransitionList.Count; ++i)
            {
                SkillHsmConfigTransition temp = nodeData.TransitionList[i];
                selectTransition = (nodeData.Id * 1000 + temp.TransitionId) == HSMManager.Instance.CurrentTransitionId;
                if (selectTransition)
                {
                    id = temp.TransitionId;
                    break;
                }
            }

            return selectTransition;
        }

        private void DrawTransitionParameter(SkillHsmConfigNodeData nodeData, SkillHsmConfigTransition transition, string title)
        {
            EditorGUILayout.BeginVertical("box");
            {
                if (null != transition)
                {
                    SkillHsmConfigNodeData toNode = HSMManager.Instance.GetNode(transition.ToStateId);
                    string toNodeDescript = (null != toNode) ? toNode.Descript : string.Empty;
                    string msg = string.Format("节点{0} -> 节点{1}", nodeData.Id, transition.ToStateId);
                    EditorGUILayout.LabelField(msg);
                }
            }
            EditorGUILayout.EndVertical();

            int transitionId = (null != transition) ? transition.TransitionId : -1;
            List<SkillHsmConfigHSMParameter> parametersList = (null != transition) ? transition.ParameterList : new List<SkillHsmConfigHSMParameter>();

            Action<SkillHsmConfigHSMParameter> DelCallBack = (hSMParameter) =>
            {
                if (null != HSMManager.hSMNodeAddDelParameter)
                {
                    HSMManager.hSMNodeAddDelParameter(nodeData.Id, transitionId, hSMParameter, false);
                }
            };

            Action<string> ChangeParameter = (parameterName) =>
            {
                if (null != HSMManager.hSMNodeChangeParameter)
                {
                    HSMManager.hSMNodeChangeParameter(nodeData.Id, transitionId, parameterName);
                }
            };

            SkillHsmConfigTransitionGroup group = null;
            EditorGUILayout.BeginVertical("box");
            {
                group = HSMTransitionGroup.DrawTransitionGroup(nodeData, transition);
                DrawTransitionAddGroup(nodeData, transitionId);
            }
            EditorGUILayout.EndVertical();

            for (int i = 0; i < parametersList.Count; ++i)
            {
                parametersList[i].Index = i;
            }
            DrawParameter(nodeData, parametersList, DelCallBack, ChangeParameter, group);

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical("box");
            {
                DrawTransitionAddParameter(nodeData, transitionId);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawNodeParameter(SkillHsmConfigNodeData nodeData, string title)
        {
            EditorGUILayout.LabelField(title);
            List<SkillHsmConfigHSMParameter> parametersList = nodeData.ParameterList;

            Action<SkillHsmConfigHSMParameter> DelCallBack = (hSMParameter) =>
            {
                if (null != HSMManager.hSMNodeAddDelParameter)
                {
                    HSMManager.hSMNodeAddDelParameter(nodeData.Id, -1, hSMParameter, false);
                }
            };

            Action<string> ChangeParameter = (parameterName) =>
            {
                if (null != HSMManager.hSMNodeChangeParameter)
                {
                    HSMManager.hSMNodeChangeParameter(nodeData.Id, -1, parameterName);
                }
            };

            DrawParameter(nodeData, parametersList, DelCallBack, ChangeParameter, null);

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical("box");
            {
                DrawNodeAddParameter(nodeData);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawParameter(SkillHsmConfigNodeData nodeData, List<SkillHsmConfigHSMParameter> parametersList, Action<SkillHsmConfigHSMParameter> delCallBack, Action<string> ChangeParameter, SkillHsmConfigTransitionGroup group)
        {
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            {
                int height = 0;
                for (int i = 0; i < parametersList.Count; ++i)
                {
                    SkillHsmConfigHSMParameter parameter = parametersList[i];
                    height += 50;
                }

                height = height <= 300 ? height : 300;
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(height));
                {
                    for (int i = 0; i < parametersList.Count; ++i)
                    {
                        SkillHsmConfigHSMParameter hSMParameter = parametersList[i];

                        Action DelCallBack = () =>
                        {
                            if (null != delCallBack)
                            {
                                delCallBack(hSMParameter);
                            }
                        };

                        Color color = Color.white;
                        if (null != group)
                        {
                            string name = group.ParameterList.Find(a => (a.CompareTo(hSMParameter.ParameterName) == 0));
                            if (!string.IsNullOrEmpty(name))
                            {
                                color = colorArr[0];
                            }
                        }

                        GUI.backgroundColor = color; // new Color(0.85f, 0.85f, 0.85f, 1f);
                        EditorGUILayout.BeginVertical("box");
                        {
                            string parameterName = hSMParameter.ParameterName;

                            SkillHsmConfigHSMParameter tempParameter = hSMParameter.Clone();
                            tempParameter = HSMDrawParameter.Draw(hSMParameter, HSMDrawParameterType.NODE_PARAMETER, DelCallBack);
                            if (parameterName.CompareTo(hSMParameter.ParameterName) != 0)
                            {
                                if (null != ChangeParameter)
                                {
                                    ChangeParameter(hSMParameter.ParameterName);
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

        private void DrawTransitionAddParameter(SkillHsmConfigNodeData nodeValue, int transitionId)
        {
            GUI.enabled = (transitionId >= 0);

            if (GUILayout.Button("添加条件"))
            {
                if (HSMManager.Instance.HSMTreeData.ParameterList.Count <= 0)
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
                        SkillHsmConfigHSMParameter hSMParameter = HSMManager.Instance.HSMTreeData.ParameterList[0];
                        HSMManager.hSMNodeAddDelParameter(nodeValue.Id, transitionId, hSMParameter, true);
                    }
                }
            }

            GUI.enabled = true;
        }

        private void DrawTransitionAddGroup(SkillHsmConfigNodeData nodeValue, int transitionId)
        {
            if (transitionId < 0)
            {
                return;
            }

            if (GUILayout.Button("添加组"))
            {
                if (null != HSMManager.hSMNodeTransitionAddDelGroup)
                {
                    HSMManager.hSMNodeTransitionAddDelGroup(nodeValue.Id, transitionId, -1, true);
                }
            }
        }

        private void DrawNodeAddParameter(SkillHsmConfigNodeData nodeValue)
        {
            if (GUILayout.Button("添加条件"))
            {
                if (HSMManager.Instance.HSMTreeData.ParameterList.Count <= 0)
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
                        SkillHsmConfigHSMParameter hSMParameter = HSMManager.Instance.HSMTreeData.ParameterList[0];
                        HSMManager.hSMNodeAddDelParameter(nodeValue.Id, -1, hSMParameter, true);
                    }
                }
            }
        }

    }


}
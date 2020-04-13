using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HSMTree;
using System;
using GenPB;

namespace HSMTree
{
    public class HSMDrawController
    {
        private HSMNodeWindow _treeNodeWindow = null;

        public HSMDrawModel _HSMDrawModel = null;
        private HSMDrawView _HSMDrawView = null;

        public void Init()
        {
            _HSMDrawModel = new HSMDrawModel();
            _HSMDrawView = new HSMDrawView();
            _HSMDrawView.Init(this, _HSMDrawModel);
        }

        public void OnDestroy()
        {

        }

        public void OnGUI(HSMNodeWindow window)
        {
            _treeNodeWindow = window;
            _HSMDrawView.SetWindow(_treeNodeWindow);

            SkillHsmConfigNodeData currentNode = _HSMDrawModel.GetCurrentSelectNode();
            List<SkillHsmConfigNodeData> nodeList = new List<SkillHsmConfigNodeData>();
            if (HSMManager.Instance.CurrentOpenSubMachineId >= 0)
            {
                nodeList = HSMManager.Instance.GetNodeChild(HSMManager.Instance.CurrentOpenSubMachineId);
            }
            else
            {
                nodeList = _HSMDrawModel.GetBaseNode();
            }

            _HSMDrawView.Draw(_treeNodeWindow.position, currentNode, nodeList);
        }
    }

    public class Node_Draw_Info
    {
        public string _nodeTypeName;
        public List<KeyValuePair<string, Node_Draw_Info_Item>> _nodeArr = new List<KeyValuePair<string, Node_Draw_Info_Item>>();

        public Node_Draw_Info(string name)
        {
            _nodeTypeName = name;
        }

        public void AddNodeType(NODE_TYPE nodeType)
        {
            Node_Draw_Info_Item item = new Node_Draw_Info_Item(nodeType);
            item.GetTypeName();
            string name = string.Format("{0}/{1}", _nodeTypeName, item._nodeName);
            KeyValuePair<string, Node_Draw_Info_Item> kv = new KeyValuePair<string, Node_Draw_Info_Item>(name, item);
            _nodeArr.Add(kv);
        }

        public void AddNodeType(NODE_TYPE nodeType, string nodeName, int identification)
        {
            Node_Draw_Info_Item item = new Node_Draw_Info_Item(nodeType);
            item.SetName(nodeName);
            item.SetIdentification(identification);
            string name = string.Format("{0}/{1}", _nodeTypeName, nodeName);
            KeyValuePair<string, Node_Draw_Info_Item> kv = new KeyValuePair<string, Node_Draw_Info_Item>(name, item);
            _nodeArr.Add(kv);
        }

    }

    public class Node_Draw_Info_Item
    {
        public string _nodeName = string.Empty;
        public NODE_TYPE _nodeType;
        public int _identification = -1;

        public Node_Draw_Info_Item(NODE_TYPE nodeType)
        {
            _nodeType = nodeType;
        }

        public void GetTypeName()
        {
            int index = EnumNames.GetEnumIndex<NODE_TYPE>(_nodeType);
            _nodeName = EnumNames.GetEnumName<NODE_TYPE>(index);
        }

        public void SetName(string name)
        {
            _nodeName = name;
        }

        public void SetIdentification(int identification)
        {
            _identification = identification;
        }

    }

    public class HSMDrawModel
    {
        private List<Node_Draw_Info> infoList = new List<Node_Draw_Info>();
        public HSMDrawModel()
        {
            infoList.Clear();
            SetInfoList();
        }

        public SkillHsmConfigNodeData GetCurrentSelectNode()
        {
            return HSMManager.Instance.CurrentNode;
        }

        public List<SkillHsmConfigNodeData> GetNodeList()
        {
            return HSMManager.Instance.NodeList;
        }

        private List<NODE_TYPE[]> nodeList = new List<NODE_TYPE[]>() {
            new NODE_TYPE[] { NODE_TYPE.STATE },           // 状态节点
            new NODE_TYPE[]{ NODE_TYPE.SUB_STATE_MACHINE}, // 子状态机节点
        };
        private void SetInfoList()
        {
            // 状态节点
            Node_Draw_Info stateDrawInfo = new Node_Draw_Info("CreateState");
            //stateDrawInfo.AddNodeType(NODE_TYPE.STATE);
            infoList.Add(stateDrawInfo);

            // 子状态机节点
            Node_Draw_Info subMachineDrawInfo = new Node_Draw_Info("Create-Sub State Machine");
            subMachineDrawInfo.AddNodeType(NODE_TYPE.SUB_STATE_MACHINE);
            infoList.Add(subMachineDrawInfo);

            List<CustomIdentification> nodeList = CustomNode.Instance.GetNodeList();
            for (int i = 0; i < nodeList.Count; ++i)
            {
                CustomIdentification customIdentification = nodeList[i];
                if (customIdentification.NodeType == NODE_TYPE.STATE)
                {
                    stateDrawInfo.AddNodeType(NODE_TYPE.STATE, customIdentification.Name, (int)customIdentification.Identification);
                }
                else if (customIdentification.NodeType == NODE_TYPE.SUB_STATE_MACHINE)
                {
                    subMachineDrawInfo.AddNodeType(NODE_TYPE.SUB_STATE_MACHINE, customIdentification.Name, (int)customIdentification.Identification);
                }
            }
        }

        public List<SkillHsmConfigNodeData> GetBaseNode()
        {
            List<SkillHsmConfigNodeData> allNodeList = GetNodeList();
            List<SkillHsmConfigNodeData> drawList = new List<SkillHsmConfigNodeData>();

            HashSet<int> childNodeHash = new HashSet<int>();
            for (int i = 0; i < allNodeList.Count; ++i)
            {
                SkillHsmConfigNodeData nodeValue = allNodeList[i];
                for (int j = 0; j < nodeValue.ChildIdList.Count; ++j)
                {
                    int childId = nodeValue.ChildIdList[j];
                    if (!childNodeHash.Contains(childId))
                    {
                        childNodeHash.Add(childId);
                    }
                }
            }

            for (int i = 0; i < allNodeList.Count; ++i)
            {
                SkillHsmConfigNodeData nodeValue = allNodeList[i];
                if (childNodeHash.Contains(nodeValue.Id))
                {
                    continue;
                }

                drawList.Add(nodeValue);
            }

            return drawList;
        }

        public List<Node_Draw_Info> InfoList
        {
            get
            {
                return infoList;
            }
        }

        public string[] GetOptionArr(ref int selectIndex, ref List<int> nodeList)
        {
            nodeList = new List<int>();
            List<string> optionList = new List<string>();
            nodeList.Add(-1);
            optionList.Add("Base");
            selectIndex = nodeList.Count - 1;

            if (HSMManager.Instance.CurrentOpenSubMachineId >= 0)
            {
                int nodeId = HSMManager.Instance.CurrentOpenSubMachineId;
                SkillHsmConfigNodeData nodeValue = HSMManager.Instance.GetNode(nodeId);
                while (null != nodeValue && nodeValue.NodeType == (int)NODE_TYPE.SUB_STATE_MACHINE)
                {
                    nodeList.Insert(1, nodeValue.Id);
                    selectIndex = nodeList.Count - 1;

                    string name = GetNodeName(nodeValue);
                    optionList.Insert(1, name);

                    if (nodeValue.ParentId > 0)
                    {
                        nodeValue = HSMManager.Instance.GetNode(nodeValue.ParentId);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return optionList.ToArray();
        }

        private string GetNodeName(SkillHsmConfigNodeData nodeValue)
        {
            int nodeIndex = EnumNames.GetEnumIndex<NODE_TYPE>((NODE_TYPE)nodeValue.NodeType);
            string name = EnumNames.GetEnumName<NODE_TYPE>(nodeIndex);
            return string.Format("{0}_{1}", name, nodeValue.Id);
        }
    }

    public class HSMDrawView
    {

        private HSMNodeWindow _treeNodeWindow = null;
        private HSMDrawController _drawController = null;
        private HSMDrawModel _drawModel = null;

        // 鼠标的位置
        private Vector2 mousePosition;
        // 添加连线
        private bool makeTransitionMode = false;

        private Vector3 scrollPos = Vector2.zero;
        private Rect scrollRect = new Rect(0, 0, 1500, 1000);
        private Rect contentRect = new Rect(0, 0, 3000, 2000);

        private List<SkillHsmConfigNodeData> _nodeList = new List<SkillHsmConfigNodeData>();

        public void Init(HSMDrawController drawController, HSMDrawModel drawModel)
        {
            _drawController = drawController;
            _drawModel = drawModel;
        }

        public void SetWindow(HSMNodeWindow window)
        {
            _treeNodeWindow = window;
        }

        public void Draw(Rect windowsPosition, SkillHsmConfigNodeData currentNode, List<SkillHsmConfigNodeData> nodeList)
        {
            _nodeList = nodeList;
            DrawTitle();

            Rect rect = GUILayoutUtility.GetRect(0f, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            scrollRect = rect;

            contentRect.x = rect.x;
            contentRect.y = rect.y;

            //创建 scrollView  窗口  
            scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, contentRect);
            {
                NodeMakeTransition(currentNode);
                DrawNodeWindows();
                SelectTransition();
                ResetScrollPos();
            }
            GUI.EndScrollView();  //结束 ScrollView 窗口  
        }

        private string[] optionArr = new string[] { "Base", "SubMachine"};

        private void DrawTitle()
        {
            int selectIndex = 0;
            List<int> idList = new List<int>();
            string[] optionArr = _drawModel.GetOptionArr(ref selectIndex, ref idList);
            int option = selectIndex;

            option = GUILayout.Toolbar(option, optionArr, EditorStyles.toolbarButton, GUILayout.Width(optionArr.Length * 200));
            if (option != selectIndex)
            {
                if (null != HSMManager.hsmOpenSubMachine)
                {
                    int nodeId = idList[option];
                    HSMManager.hsmOpenSubMachine(nodeId);
                }
            }
        }

        private void NodeMakeTransition(SkillHsmConfigNodeData currentNode)
        {
            Event _event = Event.current;
            mousePosition = _event.mousePosition;

            if (_event.type == EventType.MouseDown)
            {
                if (_event.button == 0)  // 鼠标左键
                {
                    if (makeTransitionMode)
                    {
                        SkillHsmConfigNodeData nodeValue = GetMouseInNode(_nodeList);
                        // 如果按下鼠标时，选中了一个节点，则将 新选中根节点 添加为 selectNode 的子节点
                        bool nodeTypeValid = CheckTranshtion(currentNode, nodeValue);
                        if (null != nodeValue && currentNode.Id != nodeValue.Id && nodeTypeValid)
                        {
                            if (null != HSMManager.hSMNodeChangeTransition)
                            {
                                HSMManager.hSMNodeChangeTransition(currentNode.Id, nodeValue.Id, true);
                            }
                        }

                        // 取消连线状态
                        makeTransitionMode = false;
                    }
                    else
                    {
                        SkillHsmConfigNodeData nodeValue = GetMouseInNode(_nodeList);
                        if ((null != nodeValue))
                        {
                            ClickNode(nodeValue);
                        }
                    }
                }

                if (_event.button == 1)  // 鼠标右键
                {
                    if ((!makeTransitionMode))
                    {
                        SkillHsmConfigNodeData nodeValue = GetMouseInNode(_nodeList);
                        ShowMenu(currentNode, nodeValue);
                    }
                }
            }

            if (makeTransitionMode && currentNode != null)
            {
                SkillHsmConfigRectT mouseRect = new SkillHsmConfigRectT();
                mouseRect.X = mousePosition.x;
                mouseRect.Y = mousePosition.y;
                mouseRect.Width = 10;
                mouseRect.Height = 10;

                DrawNodeCurve(currentNode.Position, mouseRect, Color.black);
            }
        }

        private bool CheckTranshtion(SkillHsmConfigNodeData fromNode, SkillHsmConfigNodeData toNode)
        {
            if (null == toNode)
            {
                return false;
            }
            if (toNode.NodeType == (int)NODE_TYPE.ENTRY)
            {
                return false;
            }

            if (fromNode.NodeType == (int)NODE_TYPE.ENTRY && toNode.NodeType == (int)NODE_TYPE.EXIT)
            {
                return false;
            }

            return true;
        }

        private int lastClickNodeTime = 0;
        private void ClickNode(SkillHsmConfigNodeData nodeValue)
        {
            if (null == nodeValue)
            {
                return;
            }
            int nodeId = (null != nodeValue) ? nodeValue.Id : -1;
            if (HSMManager.hSMChangeSelectId != null)
            {
                HSMManager.hSMChangeSelectId(nodeId);
            }

            if (nodeValue.NodeType == (int)NODE_TYPE.SUB_STATE_MACHINE)
            {
                int currentTime = (int)(Time.realtimeSinceStartup * 1000);
                if (currentTime - lastClickNodeTime <= 200)
                {
                    if (null != HSMManager.hsmOpenSubMachine)
                    {
                        HSMManager.hsmOpenSubMachine(nodeId);
                    }
                }
                lastClickNodeTime = (int)(Time.realtimeSinceStartup * 1000);
            }
        }

        // 绘制节点
        private void DrawNodeWindows()
        {
            Action CallBack = () =>
            {
                for (int i = 0; i < _nodeList.Count; i++)
                {
                    SkillHsmConfigNodeData nodeValue = _nodeList[i];
                    int index = EnumNames.GetEnumIndex<NODE_TYPE>((NODE_TYPE)nodeValue.NodeType);
                    string name = EnumNames.GetEnumName<NODE_TYPE>(index);
                    name = string.Format("{0}_{1}", name, nodeValue.Id);
                    Rect rect = GUI.Window(i, RectTExtension.RectTToRect(nodeValue.Position), DrawNodeWindow, name);
                    nodeValue.Position = RectTExtension.RectToRectT(rect);
                    DrawToChildCurve(nodeValue);
                }
            };

            _treeNodeWindow.DrawWindow(CallBack);
        }

        private void SelectTransition()
        {
            Event _event = Event.current;
            Vector3 mousePos = _event.mousePosition;

            if (_event.type != EventType.MouseDown || (_event.button != 0)) // 鼠标左键
            {
                return;
            }

            for (int i = 0; i < _nodeList.Count; i++)
            {
                SkillHsmConfigNodeData nodeValue = _nodeList[i];

                for (int j = 0; j < nodeValue.TransitionList.Count; ++j)
                {
                    SkillHsmConfigTransition transition = nodeValue.TransitionList[j];
                    int toId = transition.ToStateId;
                    SkillHsmConfigNodeData toNode = HSMManager.Instance.GetNode(toId);
                    if (null == toNode)
                    {
                        continue;
                    }

                    int transitionId = nodeValue.Id * 1000 + transition.TransitionId;
                    Vector3 startPos = Vector3.zero;
                    Vector3 endPos = Vector3.zero;
                    CalculateTranstion(nodeValue.Position, toNode.Position, ref startPos, ref endPos);

                    Vector3 AB = endPos - startPos;
                    Vector3 AP = mousePos - startPos;
                    Vector3 BP = mousePos - endPos;
                    float dotAP_AB = Vector3.Dot(AP, AB.normalized);
                    float dotBP_BA = Vector3.Dot(BP, (AB * -1).normalized);
                    if (dotAP_AB < 0 || dotBP_BA < 0)
                    {
                        continue;
                    }

                    float distance = Vector3.Cross(AB, AP).magnitude / AB.magnitude;

                    bool value = (distance < 10) && (Mathf.Abs(dotAP_AB) < AB.magnitude);
                    if (value)
                    {
                        if (null != HSMManager.hSMChangeSelectTransitionId)
                        {
                            int id = nodeValue.Id * 1000 + transition.TransitionId;
                            HSMManager.hSMChangeSelectTransitionId(id);
                        }

                        if (null != HSMManager.hSMChangeSelectId)
                        {
                            HSMManager.hSMChangeSelectId(nodeValue.Id);
                        }
                    }
                    //float distance = Vector3.Cross(AB, AP).magnitude / AB.magnitude;
                    //return distance <= (sRadius + cRadius);
                }
            }
        }

        void DrawNodeWindow(int id)
        {
            if (id >= _nodeList.Count)
            {
                return;
            }
            SkillHsmConfigNodeData nodeValue = _nodeList[id];
            HSMNodeEditor.Draw(nodeValue, HSMManager.Instance.CurrentSelectId, HSMManager.Instance.DefaultStateId);
            GUI.DragWindow();
        }

        // 获取鼠标所在位置的节点
        private SkillHsmConfigNodeData GetMouseInNode(List<SkillHsmConfigNodeData> nodeList)
        {
            SkillHsmConfigNodeData selectNode = null;
            for (int i = 0; i < nodeList.Count; i++)
            {
                SkillHsmConfigNodeData nodeValue = nodeList[i];
                // 如果鼠标位置 包含在 节点的 Rect 范围，则视为可以选择的节点
                if (RectTExtension.RectTToRect(nodeValue.Position).Contains(mousePosition))
                {
                    selectNode = nodeValue;
                    break;
                }
            }

            return selectNode;
        }

        private void ShowMenu(SkillHsmConfigNodeData currentNode, SkillHsmConfigNodeData nodeValue)
        {
            int menuType = (nodeValue != null) ? 1 : 0;

            GenericMenu menu = new GenericMenu();
            if (menuType == 0)
            {
                GenericMenu.MenuFunction2 CallBack = (object userData) => {
                    if (null != HSMManager.hSMAddState)
                    {
                        int subMachineId = -1;
                        if (HSMManager.Instance.CurrentOpenSubMachineId >= 0)
                        {
                            subMachineId = HSMManager.Instance.CurrentOpenSubMachineId;
                        }
                        HSMManager.hSMAddState((Node_Draw_Info_Item)userData, mousePosition, subMachineId);
                    }
                };

                List<Node_Draw_Info> nodeList = _drawController._HSMDrawModel.InfoList;
                for (int i = 0; i < nodeList.Count; ++i)
                {
                    Node_Draw_Info draw_Info = nodeList[i];
                    for (int j = 0; j < draw_Info._nodeArr.Count; ++j)
                    {
                        KeyValuePair<string, Node_Draw_Info_Item> kv = draw_Info._nodeArr[j];
                        string itemName = string.Format("{0}", kv.Key);
                        menu.AddItem(new GUIContent(itemName), false, CallBack, kv.Value);
                    }
                }
            }
            else
            {
                if (null != currentNode && nodeValue.Id == currentNode.Id && nodeValue.NodeType != (int)NODE_TYPE.EXIT)
                {
                    // 连线子节点
                    menu.AddItem(new GUIContent("Make Transition"), false, MakeTransition);
                    menu.AddSeparator("");
                }

                if (null != currentNode && (currentNode.NodeType != (int)NODE_TYPE.ENTRY && (currentNode.NodeType != (int)NODE_TYPE.EXIT)))
                // 删除节点
                menu.AddItem(new GUIContent("Delete State"), false, DeleteNode);

                if (nodeValue.NodeType != (int)NODE_TYPE.ENTRY && nodeValue.NodeType != (int)NODE_TYPE.EXIT)
                {
                    // 设置默认节点
                    menu.AddItem(new GUIContent("Set Default State"), false, SetDefaultState);
                }
            }

            menu.ShowAsContext();
            Event.current.Use();
        }

        // 连线子节点
        private void MakeTransition()
        {
            makeTransitionMode = true;
        }

        // 删除节点
        private void DeleteNode()
        {
            SkillHsmConfigNodeData nodeValue = GetMouseInNode(_nodeList);

            if (!EditorUtility.DisplayDialog("提示", "确定要删除节点吗", "Yes", "No"))
            {
                return;
            }

            if (null != HSMManager.hSMDeleteNode)
            {
                HSMManager.hSMDeleteNode(nodeValue.Id);
            }
        }

        private void SetDefaultState()
        {
            SkillHsmConfigNodeData nodeValue = GetMouseInNode(_nodeList);
            if (null == nodeValue)
            {
                return;
            }

            if (null != HSMManager.hSMSetDefaultState)
            {
                HSMManager.hSMSetDefaultState(nodeValue.Id);
            }
        }

        /// 每帧绘制从 节点到所有子节点的连线
        private void DrawToChildCurve(SkillHsmConfigNodeData nodeValue)
        {
            for (int i = nodeValue.TransitionList.Count - 1; i >= 0; --i)
            {
                int toId = nodeValue.TransitionList[i].ToStateId;
                SkillHsmConfigNodeData toNode = HSMManager.Instance.GetNode(toId);

                int transitionId = nodeValue.Id * 1000 + nodeValue.TransitionList[i].TransitionId;
                Color color = (transitionId == HSMManager.Instance.CurrentTransitionId) ? (Color)(new Color32(90, 210, 111, 255)) : Color.black;
                DrawNodeCurve(nodeValue.Position, toNode.Position, color);
            }
        }

        private const float coefficient = 0.5f;
        // 绘制线
        public static void DrawNodeCurve(SkillHsmConfigRectT start, SkillHsmConfigRectT end, Color color)
        {
            Vector3 startPos = Vector3.zero;
            Vector3 endPos = Vector3.zero;
            Vector3 middle = (startPos + endPos) * 0.5f;
            Handles.color = Color.white;
            CalculateTranstion(start, end, ref startPos, ref endPos);
            DrawArrow(startPos, endPos, color);
        }

        private static void CalculateTransitionPoint(SkillHsmConfigRectT start, SkillHsmConfigRectT end, ref Vector3 startCenter, ref Vector3 endCenter)
        {
            startCenter = new Vector3(start.X + start.Width * coefficient, start.Y + start.Height * coefficient);
            endCenter = new Vector3(end.X + end.Width * coefficient, end.Y + end.Height * coefficient);
        }

        private static void CalculateTranstion(SkillHsmConfigRectT start, SkillHsmConfigRectT end, ref Vector3 startPoint, ref Vector3 endPoint)
        {
            Vector3 startCenter = Vector3.zero;
            Vector3 endCenter = Vector3.zero;
            CalculateTransitionPoint(start, end, ref startCenter, ref endCenter);

            Vector3 axis = Vector3.Cross((endCenter - startCenter), new Vector3(0, 0, 1)).normalized;
            startPoint = startCenter + 10 * axis;
            endPoint = endCenter + 10 * axis;

            GUI.Box(new Rect(startPoint, Vector2.one * 10), "1");
            GUI.Box(new Rect(endPoint, Vector2.one * 10), "1");
        }

        private static void DrawArrow(Vector2 from, Vector2 to, Color color)
        {
            Handles.BeginGUI();
            Handles.color = color;
            Handles.DrawAAPolyLine(3, from, to);
            Vector2 v0 = from - to;
            v0 *= 10 / v0.magnitude;
            Vector2 v1 = new Vector2(v0.x * 0.866f - v0.y * 0.5f, v0.x * 0.5f + v0.y * 0.866f);
            Vector2 v2 = new Vector2(v0.x * 0.866f + v0.y * 0.5f, v0.x * -0.5f + v0.y * 0.866f);
            Vector2 middle = (from + to) * 0.5f;
            Handles.DrawAAPolyLine(5, middle + v1, middle, middle + v2);
            Handles.EndGUI();
        }

        private void ResetScrollPos()
        {
            if (_nodeList.Count <= 0)
            {
                return;
            }

            SkillHsmConfigNodeData rightmostNode = null;
            SkillHsmConfigNodeData bottomNode = null;
            for (int i = 0; i < _nodeList.Count; ++i)
            {
                SkillHsmConfigNodeData nodeValue = _nodeList[i];
                if (rightmostNode == null || (nodeValue.Position.X > rightmostNode.Position.X))
                {
                    rightmostNode = nodeValue;
                }
                if (bottomNode == null || (nodeValue.Position.Y > bottomNode.Position.Y))
                {
                    bottomNode = nodeValue;
                }
            }

            if ((rightmostNode.Position.X + rightmostNode.Position.Width) > contentRect.width)
            {
                contentRect.width = rightmostNode.Position.X + rightmostNode.Position.Width + 50;
            }

            if ((bottomNode.Position.Y + bottomNode.Position.Height) > contentRect.height)
            {
                contentRect.height = bottomNode.Position.Y + +bottomNode.Position.Height + 50;
            }
        }

    }

}

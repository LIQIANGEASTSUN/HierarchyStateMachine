using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HSMTree;
using System.IO;

public class HSMManager
{
    public static readonly HSMManager Instance = new HSMManager();

    public delegate void HSMChangeRootNode(int stateId);
    public delegate void HSMChangeSelectId(int stateId);
    public delegate void HSMAddState(Node_Draw_Info_Item info, Vector3 mousePosition);
    public delegate void HSMDeleteNode(int stateId);
    public delegate void HSMSetDefaultState(int stateId);
    public delegate void HSMLoadFile(string fileName);
    public delegate void HSMSaveFile(string fileName);
    public delegate void HSMDeleteFile(string fileName);
    public delegate void HSMNodeChangeTransition(int parentId, int childId, bool isAdd);
    public delegate void HSMNodeChangeParameter(int stateId, int transitionId, string newParameter);
    public delegate void HSMNodeAddDelParameter(int stateId, int transitionId, HSMParameter parameter, bool isAdd);
    public delegate void HSMParameterChange(HSMParameter parameter, bool isAdd);
    public delegate void HSMRuntimePlay(HSMPlayType state);
    public delegate void HSMChangeSelectTransitionId(int id);

    private string _filePath = string.Empty;
    private string _fileName = string.Empty;
    private HSMTreeData _HSMTreeData;
    private HSMPlayType _playState = HSMPlayType.STOP;

    // 当前选择的节点
    private int _currentSelectId = 0;

    private int _currentTransitionId = -1;

    public static HSMChangeSelectId hSMChangeSelectId;
    public static HSMAddState hSMAddState;
    public static HSMDeleteNode hSMDeleteNode;
    public static HSMSetDefaultState hSMSetDefaultState;
    public static HSMLoadFile hSMLoadFile;
    public static HSMSaveFile hSMSaveFile;
    public static HSMDeleteFile hSMDeleteFile;
    public static HSMNodeChangeTransition hSMNodeChangeTransition;
    public static HSMNodeChangeParameter hSMNodeChangeParameter;
    public static HSMNodeAddDelParameter hSMNodeAddDelParameter;
    public static HSMParameterChange parameterChange;
    public static HSMRuntimePlay hSMRuntimePlay;
    public static HSMChangeSelectTransitionId hSMChangeSelectTransitionId;

    public void Init()
    {
        _filePath = "Assets/SubAssets/GameData/HSM/";
        _fileName = string.Empty;
        _HSMTreeData = new HSMTreeData();

        hSMChangeSelectId += ChangeSelectId;
        hSMAddState += AddNode;
        hSMDeleteNode += DeleteNode;
        hSMSetDefaultState += SetDefaultState;
        hSMLoadFile += LoadFile;
        hSMSaveFile += SaveFile;
        hSMDeleteFile += DeleteFile;
        hSMNodeChangeTransition += NodeChangeTransition;
        hSMNodeAddDelParameter += NodeAddDelParameter;
        parameterChange += ParameterChange;
        hSMNodeChangeParameter += NodeChangeParameter;
        hSMRuntimePlay += RuntimePlay;
        hSMChangeSelectTransitionId += ChangeSelectTransition;

        _playState = HSMPlayType.STOP;
    }

    public void OnDestroy()
    {
        hSMChangeSelectId -= ChangeSelectId;
        hSMAddState -= AddNode;
        hSMDeleteNode -= DeleteNode;
        hSMSetDefaultState -= SetDefaultState;
        hSMLoadFile -= LoadFile;
        hSMSaveFile -= SaveFile;
        hSMDeleteFile -= DeleteFile;
        hSMNodeChangeTransition -= NodeChangeTransition;
        hSMNodeAddDelParameter -= NodeAddDelParameter;
        parameterChange -= ParameterChange;
        hSMNodeChangeParameter -= NodeChangeParameter;
        hSMRuntimePlay -= RuntimePlay;
        hSMChangeSelectTransitionId += ChangeSelectTransition;

        _playState = HSMPlayType.STOP;

        AssetDatabase.Refresh();
        UnityEngine.Caching.ClearCache();
    }

    public void Update()
    {
        CheckNode(_HSMTreeData.nodeList);
    }

    public string FileName
    {
        get { return _fileName; }
        set { _fileName = value; }
    }

    public string FilePath
    {
        get { return _filePath; }
    }

    public string GetFilePath(string fileName)
    {
        string path = string.Format("{0}{1}.bytes", FilePath, fileName);
        return path;
    }

    public int CurrentSelectId
    {
        get { return _currentSelectId; }
    }

    public int CurrentTransitionId
    {
        get { return _currentTransitionId; }
    }

    public HSMTreeData HSMTreeData
    {
        get { return _HSMTreeData; }
    }

    public HSMPlayType PlayType
    {
        get { return _playState; }
    }

    public NodeData CurrentNode
    {
        get
        {
            return GetNode(_currentSelectId);
        }
    }

    public int DefaultStateId
    {
        get { return _HSMTreeData.defaultStateId; }
    }

    public List<NodeData> NodeList
    {
        get
        {
            return _HSMTreeData.nodeList;
        }
    }

    private void LoadFile(string fileName)
    {
        string path = GetFilePath(fileName);
        if (!File.Exists(path))
        {
            if (!EditorUtility.DisplayDialog("提示", "文件不存在", "yes"))
            { }
            return;
        }

        _playState = HSMPlayType.STOP;
        NodeNotify.SetPlayState((int)_playState);

        HSMReadWrite readWrite = new HSMReadWrite();
        HSMTreeData HSMTreeData = readWrite.ReadJson(path);
        if (null == HSMTreeData)
        {
            Debug.LogError("file is null:" + fileName);
            return;
        }

        _fileName = fileName;
        _HSMTreeData = HSMTreeData;

        HSMRunTime.Instance.Reset(HSMTreeData);
    }

    private void SaveFile(string fileName)
    {
        if (_HSMTreeData == null)
        {
            Debug.LogError("rootNode is null");
            return;
        }

        string path = GetFilePath(fileName);
        if (File.Exists(path))
        {
            if (!EditorUtility.DisplayDialog("已存在文件", "是否替换新文件", "替换", "取消"))
            {
                return;
            }
        }

        // 如果项目总不包含该路径，创建一个
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        HSMReadWrite readWrite = new HSMReadWrite();
        readWrite.WriteJson(_HSMTreeData, path);
    }

    private void DeleteFile(string fileName)
    {
        string path = GetFilePath(fileName);
        if (!File.Exists(path))
        {
            if (!EditorUtility.DisplayDialog("提示", "文件不存在", "yes"))
            { }
            return;
        }

        File.Delete(path);
    }

    private void NodeChangeTransition(int fromId, int toId, bool isAdd)
    {
        NodeData fromNode = GetNode(fromId);
        NodeData toNode = GetNode(toId);

        if (null == fromNode || null == toNode)
        {
            Debug.LogError("node is null");
            return;
        }

        if (isAdd)
        {
            NodeAddTransition(fromNode, toNode);
        }
        else
        {
            NodeDeleteTransition(fromNode, toNode);
        }
    }

    private void NodeAddTransition(NodeData fromNode, NodeData toNode)
    {
        for (int i = 0; i < fromNode.transitionList.Count; ++i)
        {
            Transition temp = fromNode.transitionList[i];
            if (temp.toStateId == toNode.id)
            {
                return;
            }
        }

        int transitionId = -1;
        for (int i = 0; i < fromNode.transitionList.Count; ++i)
        {
            bool result = true;
            for (int j = 0; j < fromNode.transitionList.Count; ++j)
            {
                Transition temp = fromNode.transitionList[j];
                if (i == temp.transitionId)
                {
                    result = false;
                    break;
                }
            }

            if (result)
            {
                transitionId = i;
                break;
            }
        }

        if (transitionId == -1)
        {
            transitionId = fromNode.transitionList.Count;
        }

        Transition transition = new Transition();
        transition.transitionId = transitionId;
        transition.toStateId = toNode.id;
        transition.parameterList = new List<HSMParameter>();
        fromNode.transitionList.Add(transition);
    }

    private void NodeDeleteTransition(NodeData fromNode, NodeData toNode)
    {
        for (int i = 0; i < fromNode.transitionList.Count; ++i)
        {
            Transition temp = fromNode.transitionList[i];
            if (temp.toStateId == toNode.id)
            {
                fromNode.transitionList.RemoveAt(i);
                break;
            }
        }
    }

    private void NodeAddDelParameter(int stateId, int transitionId, HSMParameter parameter, bool isAdd)
    {
        NodeData nodeValue = GetNode(stateId);
        if (null == nodeValue)
        {
            return;
        }

        List<HSMParameter> parameterList = new List<HSMParameter>();
        if (transitionId >= 0)
        {
            Transition transition = null;
            for (int i = 0; i < nodeValue.transitionList.Count; ++i)
            {
                if (nodeValue.transitionList[i].transitionId == transitionId)
                {
                    transition = nodeValue.transitionList[i];
                    break;
                }
            }

            if (null == transition)
            {
                return;
            }

            parameterList = transition.parameterList;
        }
        else
        {
            parameterList = nodeValue.parameterList;
        }

        if (isAdd)
        {
            AddParameter(parameterList, parameter, true);
        }
        else
        {
            DelParameter(parameterList, parameter);
        }
    }

    private void NodeChangeParameter(int stateId, int transitionId, string newParameter)
    {
        NodeData nodeValue = GetNode(stateId);
        if (null == nodeValue)
        {
            return;
        }

        HSMParameter parameter = null;
        for (int i = 0; i < _HSMTreeData.parameterList.Count; ++i)
        {
            HSMParameter temp = _HSMTreeData.parameterList[i];
            if (temp.parameterName.CompareTo(newParameter) == 0)
            {
                parameter = temp;
            }
        }

        if (null == parameter)
        {
            return;
        }

        List<HSMParameter> parameterList = new List<HSMParameter>();
        if (transitionId >= 0)
        {
            Transition transition = null;
            for (int i = 0; i < nodeValue.transitionList.Count; ++i)
            {
                if (nodeValue.transitionList[i].transitionId == transitionId)
                {
                    transition = nodeValue.transitionList[i];
                    break;
                }
            }

            if (null == transition)
            {
                return;
            }

            parameterList = transition.parameterList;
        }
        else
        {
            parameterList = nodeValue.parameterList;
        }

        for (int i = 0; i < parameterList.Count; ++i)
        {
            HSMParameter temp = parameterList[i];
            if (temp.parameterName.CompareTo(parameter.parameterName) == 0)
            {
                parameterList[i] = parameter.Clone();
                break;
            }
        }
    }

    private void RuntimePlay(HSMPlayType state)
    {
        NodeNotify.SetPlayState((int)state);
        _playState = state;
    }

    private void ChangeSelectTransition(int id)
    {
        _currentTransitionId = id;
    }

    private void ParameterChange(HSMParameter parameter, bool isAdd)
    {
        if (isAdd)
        {
            AddParameter(_HSMTreeData.parameterList, parameter);
        }
        else
        {
            DelParameter(_HSMTreeData.parameterList, parameter);
        }

        HSMRunTime.Instance.Reset(HSMTreeData);
    }

    private bool AddParameter(List<HSMParameter> parameterList, HSMParameter parameter, bool repeatAdd = false)
    {
        bool result = true;
        if (string.IsNullOrEmpty(parameter.parameterName))
        {
            string meg = string.Format("条件参数不能为空", parameter.parameterName);
            TreeNodeWindow.window.ShowNotification(meg);
            result = false;
        }

        for (int i = 0; i < parameterList.Count; ++i)
        {
            HSMParameter tempParameter = parameterList[i];
            if (!repeatAdd && tempParameter.parameterName.CompareTo(parameter.parameterName) == 0)
            {
                string meg = string.Format("条件参数:{0} 已存在", parameter.parameterName);
                TreeNodeWindow.window.ShowNotification(meg);
                result = false;
                break;
            }
        }

        if (result)
        {
            HSMParameter newParameter = parameter.Clone();
            parameterList.Add(newParameter);
        }

        return result;
    }

    private void DelParameter(List<HSMParameter> parameterList, HSMParameter parameter)
    {
        for (int i = 0; i < parameterList.Count; ++i)
        {
            HSMParameter tempParameter = parameterList[i];
            if (tempParameter.parameterName.CompareTo(parameter.parameterName) == 0)
            {
                parameterList.RemoveAt(i);
                break;
            }
        }
    }

    private void ChangeSelectId(int stateId)
    {
        _currentSelectId = stateId;
    }

    public NodeData GetNode(int stateId)
    {
        for (int i = 0; i < NodeList.Count; ++i)
        {
            NodeData nodeValue = NodeList[i];
            if (nodeValue.id == stateId)
            {
                return nodeValue;
            }
        }

        return null;
    }

    // 添加节点
    private void AddNode(Node_Draw_Info_Item info, Vector3 mousePosition)
    {
        NodeData newNodeValue = new NodeData();
        newNodeValue.id = GetNewstateId();
        if (_HSMTreeData.defaultStateId < 0)
        {
            _HSMTreeData.defaultStateId = newNodeValue.id;
        }

        newNodeValue.nodeName = info._nodeName;
        newNodeValue.identification = info._identification;
        newNodeValue.NodeType = (int)info._nodeType;

        Rect rect = new Rect(mousePosition.x, mousePosition.y, 100, 100);
        newNodeValue.position = RectTool.RectToRectT(rect);

        NodeList.Add(newNodeValue);
    }

    private int GetNewstateId()
    {
        int id = -1;
        int index = -1;
        while (id == -1)
        {
            ++index;
            id = index;
            for (int i = 0; i < NodeList.Count; ++i)
            {
                if (NodeList[i].id == index)
                {
                    id = -1;
                }
            }
        }

        return id;
    }

    // 删除节点
    private void DeleteNode(int stateId)
    {
        NodeData delNode = null;
        for (int i = 0; i < NodeList.Count; ++i)
        {
            NodeData nodeValue = NodeList[i];
            if (nodeValue.id != stateId)
            {
                continue;
            }

            delNode = nodeValue;
            NodeList.RemoveAt(i);
            break;
        }

        if (null == delNode)
        {
            return;
        }

        for (int i = 0; i < NodeList.Count; ++i)
        {
            NodeData nodeValue = NodeList[i];
            for (int j = 0; j < nodeValue.transitionList.Count; ++j)
            {
                Transition transition = nodeValue.transitionList[j];
                if (transition.toStateId == delNode.id)
                {
                    nodeValue.transitionList.RemoveAt(j);
                }
            }
        }

    }

    // 设置默认状态
    private void SetDefaultState(int stateId)
    {
        _HSMTreeData.defaultStateId = stateId;
    }

    private static void CheckNode(List<NodeData> nodeValueList)
    {
        string meg = string.Empty;
        if (TreeNodeWindow.window != null && !string.IsNullOrEmpty(meg))
        {
            TreeNodeWindow.window.ShowNotification(meg);
        }
    }
}

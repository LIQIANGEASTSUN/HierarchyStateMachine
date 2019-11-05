using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HSMTree;
using System.IO;

public class HSMManager
{
    public static readonly HSMManager Instance = new HSMManager();

    public delegate void HSMChangeRootNode(int nodeId);
    public delegate void HSMChangeSelectId(int nodeId);
    public delegate void HSMAddNode(Node_Draw_Info_Item info, Vector3 mousePosition);
    public delegate void HSMDeleteNode(int nodeId);
    public delegate void HSMLoadFile(string fileName);
    public delegate void HSMSaveFile(string fileName);
    public delegate void HSMDeleteFile(string fileName);
    public delegate void HSMNodeAddChild(int parentId, int childId);
    public delegate void HSMNodeParameter(int nodeId, HSMParameter parameter, bool isAdd);
    public delegate void HSMParameterChange(HSMParameter parameter, bool isAdd);
    public delegate void HSMNodeChangeParameter(int nodeId, string oldParameter, string newParameter);
    public delegate void HSMRuntimePlay(HSMPlayType state);

    private string _filePath = string.Empty;
    private string _fileName = string.Empty;
    private HSMTreeData _HSMTreeData;
    private HSMPlayType _playState = HSMPlayType.STOP;

    // 当前选择的节点
    private int _currentSelectId = 0;

    public static HSMChangeSelectId hSMChangeSelectId;
    public static HSMAddNode hSMAddNode;
    public static HSMDeleteNode hSMDeleteNode;
    public static HSMLoadFile hSMLoadFile;
    public static HSMSaveFile hSMSaveFile;
    public static HSMDeleteFile hSMDeleteFile;
    public static HSMNodeAddChild hSMNodeAddChild;
    public static HSMNodeParameter hSMNodeParameter;
    public static HSMParameterChange parameterChange;
    public static HSMNodeChangeParameter hSMNodeChangeParameter;
    public static HSMRuntimePlay hSMRuntimePlay;

    public void Init()
    {
        _filePath = "Assets/SubAssets/GameData/HSM/";
        _fileName = string.Empty;
        _HSMTreeData = new HSMTreeData();

        hSMChangeSelectId += ChangeSelectId;
        hSMAddNode += AddNode;
        hSMDeleteNode += DeleteNode;
        hSMLoadFile += LoadFile;
        hSMSaveFile += SaveFile;
        hSMDeleteFile += DeleteFile;
        hSMNodeAddChild += NodeAddChild;
        hSMNodeParameter += NodeParameterChange;
        parameterChange += ParameterChange;
        hSMNodeChangeParameter += NodeChangeParameter;
        hSMRuntimePlay += RuntimePlay;

        _playState = HSMPlayType.STOP;
    }

    public void OnDestroy()
    {
        hSMChangeSelectId -= ChangeSelectId;
        hSMAddNode -= AddNode;
        hSMDeleteNode -= DeleteNode;
        hSMLoadFile -= LoadFile;
        hSMSaveFile -= SaveFile;
        hSMDeleteFile -= DeleteFile;
        hSMNodeAddChild -= NodeAddChild;
        hSMNodeParameter -= NodeParameterChange;
        parameterChange -= ParameterChange;
        hSMNodeChangeParameter -= NodeChangeParameter;
        hSMRuntimePlay -= RuntimePlay;

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

    public HSMTreeData HSMTreeData
    {
        get { return _HSMTreeData; }
    }

    public HSMPlayType PlayType
    {
        get { return _playState; }
    }

    public NodeValue CurrentNode
    {
        get
        {
            return GetNode(_currentSelectId);
        }
    }

    public List<NodeValue> NodeList
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

    private void NodeAddChild(int parentId, int childId)
    {
        NodeValue parentNode = GetNode(parentId);
        NodeValue childNode = GetNode(childId);

        if (null == parentNode || null == childNode)
        {
            Debug.LogError("node is null");
            return;
        }

        string msg = string.Empty;
        bool result = true;
        for (int i = 0; i < parentNode.childNodeList.Count; ++i)
        {
            if (parentNode.childNodeList[i] == childId)
            {
                result = false;
                break;
            }
        }

        if (!result)
        {
            return;
        }

        parentNode.childNodeList.Add(childNode.id);
    }

    private void NodeParameterChange(int nodeId, HSMParameter parameter, bool isAdd)
    {
        NodeValue nodeValue = GetNode(nodeId);
        if (null == nodeValue)
        {
            return;
        }

        if (isAdd)
        {
            AddParameter(nodeValue.parameterList, parameter, true);
        }
        else
        {
            DelParameter(nodeValue.parameterList, parameter);
        }
    }

    private void NodeChangeParameter(int nodeId, string oldParameter, string newParameter)
    {
        NodeValue nodeValue = GetNode(nodeId);
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

        for (int i = 0; i < nodeValue.parameterList.Count; ++i)
        {
            HSMParameter temp = nodeValue.parameterList[i];
            if (temp.parameterName.CompareTo(parameter.parameterName) == 0)
            {
                nodeValue.parameterList[i] = parameter.Clone();
                break;
            }
        }
    }

    private void RuntimePlay(HSMPlayType state)
    {
        NodeNotify.SetPlayState((int)state);
        _playState = state;
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

    private void ChangeSelectId(int nodeId)
    {
        _currentSelectId = nodeId;
    }

    public NodeValue GetNode(int nodeId)
    {
        for (int i = 0; i < NodeList.Count; ++i)
        {
            NodeValue nodeValue = NodeList[i];
            if (nodeValue.id == nodeId)
            {
                return nodeValue;
            }
        }

        return null;
    }

    // 添加节点
    private void AddNode(Node_Draw_Info_Item info, Vector3 mousePosition)
    {
        NodeValue newNodeValue = new NodeValue();
        newNodeValue.id = GetNewNodeId();
        if (_HSMTreeData.rootNodeId < 0)
        {
            _HSMTreeData.rootNodeId = newNodeValue.id;
        }

        newNodeValue.nodeName = info._nodeName;
        newNodeValue.identification = info._identification;
        newNodeValue.NodeType = (int)info._nodeType;

        Rect rect = new Rect(mousePosition.x, mousePosition.y, 100, 100);
        newNodeValue.position = RectTool.RectToRectT(rect);

        NodeList.Add(newNodeValue);
    }

    private int GetNewNodeId()
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
    private void DeleteNode(int nodeId)
    {
        for (int i = 0; i < NodeList.Count; ++i)
        {
            NodeValue nodeValue = NodeList[i];
            if (nodeValue.id != nodeId)
            {
                continue;
            }

            for (int j = 0; j < nodeValue.childNodeList.Count; ++j)
            {
                int childId = nodeValue.childNodeList[j];
                NodeValue childNode = GetNode(childId);
            }
            NodeList.RemoveAt(i);
            break;
        }
    }

    private static void CheckNode(List<NodeValue> nodeValueList)
    {
        string meg = string.Empty;
        if (TreeNodeWindow.window != null && !string.IsNullOrEmpty(meg))
        {
            TreeNodeWindow.window.ShowNotification(meg);
        }
    }
}

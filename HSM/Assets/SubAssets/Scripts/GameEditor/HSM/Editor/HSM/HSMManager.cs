using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HSMTree;
using System.IO;
using GenPB;

public class HSMManager
{
    public static readonly HSMManager Instance = new HSMManager();

    public delegate void HSMChangeRootNode(int stateId);
    public delegate void HSMChangeSelectId(int stateId);
    public delegate void HSMAddState(Node_Draw_Info_Item info, Vector3 mousePosition, int subMachineId);
    public delegate void HSMDeleteNode(int stateId);
    public delegate void HSMSetDefaultState(int stateId);
    public delegate void HSMLoadFile(string fileName);
    public delegate void HSMSaveFile(string fileName);
    public delegate void HSMDeleteFile(string fileName);
    public delegate void HSMNodeChangeTransition(int parentId, int childId, bool isAdd);
    public delegate void HSMNodeChangeParameter(int stateId, int transitionId, string newParameter);
    public delegate void HSMNodeAddDelParameter(int stateId, int transitionId, SkillHsmConfigHSMParameter parameter, bool isAdd);
    public delegate void HSMNodeTransitionAddDelGroup(int stateId, int transitionId, int groupId, bool isAdd);
    public delegate void HSMParameterChange(SkillHsmConfigHSMParameter parameter, bool isAdd);
    public delegate void HSMRuntimePlay(HSMPlayType state);
    public delegate void HSMChangeSelectTransitionId(int id);
    public delegate void HSMOpenSubMachine(int stateId);

    private string _fileName = string.Empty;
    private SkillHsmConfigHSMTreeData _HSMTreeData;
    private HSMPlayType _playState = HSMPlayType.STOP;

    public static SkillHsmConfigTransition copyTransition;
    // 当前选择的节点
    private int _currentSelectId = -1;

    private int _currentTransitionId = -1;
    private int _currentOpenSubMachineId = -1;

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
    public static HSMNodeTransitionAddDelGroup hSMNodeTransitionAddDelGroup;
    public static HSMParameterChange parameterChange;
    public static HSMRuntimePlay hSMRuntimePlay;
    public static HSMChangeSelectTransitionId hSMChangeSelectTransitionId;
    public static HSMOpenSubMachine hsmOpenSubMachine;

    public void Init()
    {
        CreateData();

        hSMChangeSelectId += ChangeSelectId;
        hSMAddState += AddNode;
        hSMDeleteNode += DeleteNode;
        hSMSetDefaultState += SetDefaultState;
        hSMLoadFile += LoadFile;
        hSMSaveFile += SaveFile;
        hSMDeleteFile += DeleteFile;
        hSMNodeChangeTransition += NodeChangeTransition;
        hSMNodeAddDelParameter += NodeAddDelParameter;
        hSMNodeTransitionAddDelGroup += NodeTransitionAddDelGroup;
        parameterChange += ParameterChange;
        hSMNodeChangeParameter += NodeChangeParameter;
        hSMRuntimePlay += RuntimePlay;
        hSMChangeSelectTransitionId += ChangeSelectTransition;
        hsmOpenSubMachine += OpenSubMachine;

        _playState = HSMPlayType.STOP;
    }

    public void OnDestroy()
    {
        _fileName = string.Empty;
        EditorPrefs.DeleteKey("HsmFile");

        hSMChangeSelectId -= ChangeSelectId;
        hSMAddState -= AddNode;
        hSMDeleteNode -= DeleteNode;
        hSMSetDefaultState -= SetDefaultState;
        hSMLoadFile -= LoadFile;
        hSMSaveFile -= SaveFile;
        hSMDeleteFile -= DeleteFile;
        hSMNodeChangeTransition -= NodeChangeTransition;
        hSMNodeAddDelParameter -= NodeAddDelParameter;
        hSMNodeTransitionAddDelGroup -= NodeTransitionAddDelGroup;
        parameterChange -= ParameterChange;
        hSMNodeChangeParameter -= NodeChangeParameter;
        hSMRuntimePlay -= RuntimePlay;
        hSMChangeSelectTransitionId += ChangeSelectTransition;
        hsmOpenSubMachine -= OpenSubMachine;

        _playState = HSMPlayType.STOP;

        AssetDatabase.Refresh();
        UnityEngine.Caching.ClearCache();
    }

    public void Update()
    {

    }

    private void CreateData()
    {
        if (EditorPrefs.HasKey("HsmFile"))
        {
            _fileName = EditorPrefs.GetString("HsmFile");
        }

        if (string.IsNullOrEmpty(_fileName))
        {
            _HSMTreeData = new SkillHsmConfigHSMTreeData();
            AddNode("Entry", IDENTIFICATION.STATE_ENTRY, NODE_TYPE.ENTRY, new Vector3(500, 150, 0), -1);
            AddNode("Exit", IDENTIFICATION.STATE_EXIT, NODE_TYPE.EXIT, new Vector3(500, 550, 0), -1);
        }
        else
        {
            LoadFile(_fileName);
        }
    }

    public string FileName
    {
        get { return _fileName; }
        set { _fileName = value; }
    }

    public string FilePath
    {
        get { return Extend.GameUtils.CombinePath("Assets", "SubAssets", "GameData", "HSM"); }// "Assets/SubAssets/GameData/HSM/";
    }

    public string GetFilePath(string fileName)
    {
        string path = Extend.GameUtils.CombinePath(FilePath, string.Format("{0}.txt", fileName)); //string.Format("{0}{1}.txt", FilePath, fileName);
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

    public int CurrentOpenSubMachineId
    {
        get { return _currentOpenSubMachineId; }
    }

    public SkillHsmConfigHSMTreeData HSMTreeData
    {
        get { return _HSMTreeData; }
    }

    public HSMPlayType PlayType
    {
        get { return _playState; }
    }

    public SkillHsmConfigNodeData CurrentNode
    {
        get
        {
            return GetNode(_currentSelectId);
        }
    }

    public int DefaultStateId
    {
        get { return _HSMTreeData.DefaultStateId; }
    }

    public List<SkillHsmConfigNodeData> NodeList
    {
        get
        {
            return _HSMTreeData.NodeList;
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
        SkillHsmConfigHSMTreeData HSMTreeData = readWrite.ReadJson(path);
        if (null == HSMTreeData)
        {
            Debug.LogError("file is null:" + fileName);
            return;
        }

        _fileName = fileName;
        _HSMTreeData = HSMTreeData;
        _currentOpenSubMachineId = -1;

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

        string name = System.IO.Path.GetFileNameWithoutExtension(path);
        CheckData(_HSMTreeData);

        HSMReadWrite readWrite = new HSMReadWrite();
        _HSMTreeData.FileName = System.IO.Path.GetFileNameWithoutExtension(path);
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

    public void CheckData(SkillHsmConfigHSMTreeData data)
    {
        for (int i = 0; i < data.NodeList.Count; ++i)
        {
            SkillHsmConfigNodeData nodeData = data.NodeList[i];
            for (int j = 0; j < nodeData.TransitionList.Count; ++j)
            {
                SkillHsmConfigTransition transition = nodeData.TransitionList[j];
                CheckTransition(transition);
            }
        }
    }

    private void CheckTransition(SkillHsmConfigTransition transition)
    {
        for (int i = transition.GroupList.Count - 1; i >= 0; --i)
        {
            SkillHsmConfigTransitionGroup group = transition.GroupList[i];
            bool validGroup = false;
            for (int j = group.ParameterList.Count - 1; j >= 0; --j)
            {
                string parameter = group.ParameterList[j];
                SkillHsmConfigHSMParameter hSMParameter = transition.ParameterList.Find(a => (a.ParameterName.CompareTo(parameter) == 0));
                if (null == hSMParameter)
                {
                    group.ParameterList.RemoveAt(j);
                    Debug.LogError("group ParameterList remove :" + parameter);
                }
                else
                {
                    validGroup = true;
                }
            }

            if (!validGroup)
            {
                transition.GroupList.RemoveAt(i);
                Debug.LogError("RemoveGroup :" + group.Index);
            }
        }

    }

    private void NodeChangeTransition(int fromId, int toId, bool isAdd)
    {
        SkillHsmConfigNodeData fromNode = GetNode(fromId);
        SkillHsmConfigNodeData toNode = GetNode(toId);

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

    private void NodeAddTransition(SkillHsmConfigNodeData fromNode, SkillHsmConfigNodeData toNode)
    {
        for (int i = 0; i < fromNode.TransitionList.Count; ++i)
        {
            SkillHsmConfigTransition temp = fromNode.TransitionList[i];
            if (temp.ToStateId == toNode.Id)
            {
                return;
            }
        }

        int transitionId = -1;
        for (int i = 0; i < fromNode.TransitionList.Count; ++i)
        {
            bool result = true;
            for (int j = 0; j < fromNode.TransitionList.Count; ++j)
            {
                SkillHsmConfigTransition temp = fromNode.TransitionList[j];
                if (i == temp.TransitionId)
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
            transitionId = fromNode.TransitionList.Count;
        }

        SkillHsmConfigTransition transition = new SkillHsmConfigTransition();
        transition.TransitionId = transitionId;
        transition.ToStateId = toNode.Id;
        fromNode.TransitionList.Add(transition);
    }

    private void NodeDeleteTransition(SkillHsmConfigNodeData fromNode, SkillHsmConfigNodeData toNode)
    {
        for (int i = 0; i < fromNode.TransitionList.Count; ++i)
        {
            SkillHsmConfigTransition temp = fromNode.TransitionList[i];
            if (temp.ToStateId == toNode.Id)
            {
                fromNode.TransitionList.RemoveAt(i);
                break;
            }
        }
    }

    private void NodeAddDelParameter(int stateId, int transitionId, SkillHsmConfigHSMParameter parameter, bool isAdd)
    {
        SkillHsmConfigNodeData nodeValue = GetNode(stateId);
        if (null == nodeValue)
        {
            return;
        }

        List<SkillHsmConfigHSMParameter> parameterList = new List<SkillHsmConfigHSMParameter>();
        if (transitionId >= 0)
        {
            SkillHsmConfigTransition transition = null;
            for (int i = 0; i < nodeValue.TransitionList.Count; ++i)
            {
                if (nodeValue.TransitionList[i].TransitionId == transitionId)
                {
                    transition = nodeValue.TransitionList[i];
                    break;
                }
            }

            if (null == transition)
            {
                return;
            }

            parameterList = transition.ParameterList;
        }
        else
        {
            parameterList = nodeValue.ParameterList;
        }

        if (isAdd)
        {
            AddParameter(parameterList, parameter, true);
        }
        else
        {
            DelParameter(parameterList, parameter);
        }

        for (int i = 0; i < parameterList.Count; ++i)
        {
            SkillHsmConfigHSMParameter p = parameterList[i];
            p.Index = i;
        }
    }

    private void NodeTransitionAddDelGroup(int stateId, int transitionId, int groupId, bool isAdd)
    {
        SkillHsmConfigNodeData nodeValue = GetNode(stateId);
        if (null == nodeValue)
        {
            return;
        }

        if (isAdd)
        {
            for (int i = 0; i < nodeValue.TransitionList.Count; ++i)
            {
                SkillHsmConfigTransition transition = null;
                if (nodeValue.TransitionList[i].TransitionId != transitionId)
                {
                    continue;
                }

                transition = nodeValue.TransitionList[i];

                for (int j = 0; j < transition.GroupList.Count + 1; ++j)
                {
                    SkillHsmConfigTransitionGroup transitionGroup = transition.GroupList.Find( a => a.Index == j);
                    if (null == transitionGroup)
                    {
                        transitionGroup = new SkillHsmConfigTransitionGroup();
                        transitionGroup.Index = j;
                        transition.GroupList.Add(transitionGroup);
                        break;
                    }
                }

                if (transition.GroupList.Count <= 0)
                {
                    SkillHsmConfigTransitionGroup transitionGroup = new SkillHsmConfigTransitionGroup();
                    transitionGroup.Index = 0;
                    transition.GroupList.Add(transitionGroup);
                }
            }
        }
        else
        {
            for (int i = 0; i < nodeValue.TransitionList.Count; ++i)
            {
                SkillHsmConfigTransition transition = null;
                if (nodeValue.TransitionList[i].TransitionId != transitionId)
                {
                    continue;
                }

                transition = nodeValue.TransitionList[i];
                for (int j = 0; j < transition.GroupList.Count; ++j)
                {
                    if (transition.GroupList[j].Index == groupId)
                    {
                        transition.GroupList.RemoveAt(j);
                    }
                }
            }
        }
    }

    private void NodeChangeParameter(int stateId, int transitionId, string newParameter)
    {
        SkillHsmConfigNodeData nodeValue = GetNode(stateId);
        if (null == nodeValue)
        {
            return;
        }

        SkillHsmConfigHSMParameter parameter = null;
        for (int i = 0; i < _HSMTreeData.ParameterList.Count; ++i)
        {
            SkillHsmConfigHSMParameter temp = _HSMTreeData.ParameterList[i];
            if (temp.ParameterName.CompareTo(newParameter) == 0)
            {
                parameter = temp;
            }
        }

        if (null == parameter)
        {
            return;
        }

        List<SkillHsmConfigHSMParameter> parameterList = new List<SkillHsmConfigHSMParameter>();
        if (transitionId >= 0)
        {
            SkillHsmConfigTransition transition = null;
            for (int i = 0; i < nodeValue.TransitionList.Count; ++i)
            {
                if (nodeValue.TransitionList[i].TransitionId == transitionId)
                {
                    transition = nodeValue.TransitionList[i];
                    break;
                }
            }

            if (null == transition)
            {
                return;
            }

            parameterList = transition.ParameterList;
        }
        else
        {
            parameterList = nodeValue.ParameterList;
        }

        for (int i = 0; i < parameterList.Count; ++i)
        {
            SkillHsmConfigHSMParameter temp = parameterList[i];
            if (temp.ParameterName.CompareTo(parameter.ParameterName) == 0)
            {
                parameterList[i] = parameter.Clone();
                break;
            }
        }

        for (int i = 0; i < parameterList.Count; ++i)
        {
            parameterList[i].Index = i;
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

    private void OpenSubMachine(int stateId)
    {
        _currentOpenSubMachineId = stateId;
    }

    private void ParameterChange(SkillHsmConfigHSMParameter parameter, bool isAdd)
    {
        if (isAdd)
        {
            AddParameter(_HSMTreeData.ParameterList, parameter);
        }
        else
        {
            DelParameter(_HSMTreeData.ParameterList, parameter);
        }

        HSMRunTime.Instance.Reset(HSMTreeData);
    }

    private bool AddParameter(List<SkillHsmConfigHSMParameter> parameterList, SkillHsmConfigHSMParameter parameter, bool repeatAdd = false)
    {
        bool result = true;
        if (string.IsNullOrEmpty(parameter.ParameterName))
        {
            string meg = string.Format("条件参数不能为空", parameter.ParameterName);
            HSMNodeWindow.window.ShowNotification(meg);
            result = false;
        }

        for (int i = 0; i < parameterList.Count; ++i)
        {
            SkillHsmConfigHSMParameter tempParameter = parameterList[i];
            if (!repeatAdd && tempParameter.ParameterName.CompareTo(parameter.ParameterName) == 0)
            {
                string meg = string.Format("条件参数:{0} 已存在", parameter.ParameterName);
                HSMNodeWindow.window.ShowNotification(meg);
                result = false;
                break;
            }
        }

        if (result)
        {
            SkillHsmConfigHSMParameter newParameter = parameter.Clone();
            parameterList.Add(newParameter);
        }

        return result;
    }

    private void DelParameter(List<SkillHsmConfigHSMParameter> parameterList, SkillHsmConfigHSMParameter parameter)
    {
        for (int i = 0; i < parameterList.Count; ++i)
        {
            SkillHsmConfigHSMParameter tempParameter = parameterList[i];
            if (tempParameter.ParameterName.CompareTo(parameter.ParameterName) == 0)
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

    public SkillHsmConfigNodeData GetNode(int stateId)
    {
        for (int i = 0; i < NodeList.Count; ++i)
        {
            SkillHsmConfigNodeData nodeValue = NodeList[i];
            if (nodeValue.Id == stateId)
            {
                return nodeValue;
            }
        }

        return null;
    }

    // 添加节点
    private void AddNode(Node_Draw_Info_Item info, Vector3 mousePosition, int subMachineId)
    {
        AddNode(info._nodeName, info._identification, info._nodeType, mousePosition, subMachineId);
    }

    private void AddNode(string nodeName, int identification, NODE_TYPE nodeType, Vector3 position, int subMachineId)
    {
        SkillHsmConfigNodeData newNodeValue = new SkillHsmConfigNodeData();
        newNodeValue.Id = GetNewstateId();

        newNodeValue.NodeName = nodeName;
        newNodeValue.Identification = (int)identification;
        newNodeValue.NodeType = (int)nodeType;

        Rect rect = new Rect(position.x, position.y, 150, 50);
        newNodeValue.Position = RectTExtension.RectToRectT(rect);

        if (subMachineId >= 0)
        {
            newNodeValue.ParentId = subMachineId;

            SkillHsmConfigNodeData subMachineNode = GetNode(subMachineId);
            if (subMachineNode == null || subMachineNode.NodeType != (int)NODE_TYPE.SUB_STATE_MACHINE)
            {
                Debug.LogError("Node is not SubMachine:" + subMachineId);
                return;
            }

            subMachineNode.ChildIdList.Add(newNodeValue.Id);
        }

        NodeList.Add(newNodeValue);

        if (_HSMTreeData.DefaultStateId < 0)
        {
            SetDefaultState(newNodeValue);
        }

        if (nodeType == NODE_TYPE.SUB_STATE_MACHINE)
        {
            AddNode("Entry", IDENTIFICATION.STATE_ENTRY, NODE_TYPE.ENTRY, new Vector3(900, 150, 0), newNodeValue.Id);
            AddNode("Exit", IDENTIFICATION.STATE_EXIT, NODE_TYPE.EXIT, new Vector3(900, 550, 0), newNodeValue.Id);
        }
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
                if (NodeList[i].Id == index)
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
        SkillHsmConfigNodeData delNode = GetNode(stateId);
        if (null == delNode)
        {
            return;
        }

        List<SkillHsmConfigNodeData> delList = new List<SkillHsmConfigNodeData>() { delNode};
        GetNodeAllChild(stateId, ref delList);

        HashSet<int> delHash = new HashSet<int>();
        for (int i = 0; i < delList.Count; ++i)
        {
            delHash.Add(delList[i].Id);
            //Debug.LogError(delList[i].Id);
        }

        for (int i = NodeList.Count - 1; i >= 0; --i)
        {
            SkillHsmConfigNodeData nodeValue = NodeList[i];
            for (int j = 0; j < nodeValue.TransitionList.Count; ++j)
            {
                SkillHsmConfigTransition transition = nodeValue.TransitionList[j];
                if (delHash.Contains(transition.ToStateId))
                {
                    nodeValue.TransitionList.RemoveAt(j);
                }
            }

            for (int j = nodeValue.ChildIdList.Count - 1; j >= 0; --j)
            {
                int childId = nodeValue.ChildIdList[j];
                if (delHash.Contains(childId))
                {
                    nodeValue.ChildIdList.RemoveAt(j);
                }
            }

            if (delHash.Contains(nodeValue.Id))
            {
                NodeList.RemoveAt(i);
                //Debug.LogError("Remove:" + nodeValue.Id);
            }
        }
    }

    // 设置默认状态
    private void SetDefaultState(int stateId)
    {
        _HSMTreeData.DefaultStateId = stateId;
    }

    private void SetDefaultState(SkillHsmConfigNodeData nodeValue)
    {
        if (nodeValue.NodeType != (int)NODE_TYPE.ENTRY && nodeValue.NodeType != (int)NODE_TYPE.EXIT)
        {
            SetDefaultState(nodeValue.Id);
        }
    }

    public SkillHsmConfigHSMTreeData GetSkillHsmData(string handleFile)
    {
        DirectoryInfo dInfo = new DirectoryInfo(FilePath);
        FileInfo[] fileInfoArr = dInfo.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < fileInfoArr.Length; ++i)
        {
            string fullName = fileInfoArr[i].FullName;
            HSMReadWrite readWrite = new HSMReadWrite();
            SkillHsmConfigHSMTreeData skillHsmData = readWrite.ReadJson(fullName);

            if (Path.GetFileNameWithoutExtension(fullName).CompareTo(handleFile) == 0)
            {
                return skillHsmData;
            }
        }

        return null;
    }

    public List<SkillHsmConfigNodeData> GetNodeChild(int nodeId)
    {
        List<SkillHsmConfigNodeData> dataList = new List<SkillHsmConfigNodeData>();
        SkillHsmConfigNodeData nodeValue = GetNode(nodeId);
        if (null == nodeValue || nodeValue.ChildIdList.Count <= 0)
        {
            return dataList;
        }

        for (int i = 0; i < nodeValue.ChildIdList.Count; ++i)
        {
            int childId = nodeValue.ChildIdList[i];
            SkillHsmConfigNodeData childNode = GetNode(childId);
            if (null != childNode)
            {
                dataList.Add(childNode);
            }
        }

        return dataList;
    }

    private void GetNodeAllChild(int nodeId, ref List<SkillHsmConfigNodeData> dataList)
    {
        List<SkillHsmConfigNodeData> childList = GetNodeChild(nodeId);
        dataList.AddRange(childList);

        List<int> hasChildIdList = new List<int>();
        for (int i = 0; i < childList.Count; ++i)
        {
            SkillHsmConfigNodeData nodeValue = childList[i];
            List<SkillHsmConfigNodeData> list = GetNodeChild(nodeValue.Id);
            dataList.AddRange(list);

            for (int j = 0; j < list.Count; ++j)
            {
                SkillHsmConfigNodeData childNode = list[j];
                if (childNode.ChildIdList.Count > 0)
                {
                    hasChildIdList.Add(childNode.Id);
                }
            }
        }

        for (int i = 0; i < hasChildIdList.Count; ++i)
        {
            GetNodeAllChild(hasChildIdList[i], ref dataList);
        }
    }

}


public class PBConfigWriteFile
{
    public string filePath;
    public byte[] byteData;
}
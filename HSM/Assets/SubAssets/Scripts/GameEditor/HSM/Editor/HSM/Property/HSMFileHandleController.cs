using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;
using GenPB;

namespace HSMTree
{

    public class HSMFileHandleController
    {
        private HSMFileHandleModel _fileHandleModel;
        private HSMFileHandleView _fileHandleView;

        public HSMFileHandleController()
        {
            Init();
        }

        public void Init()
        {
            _fileHandleModel = new HSMFileHandleModel();
            _fileHandleView = new HSMFileHandleView();
        }

        public void OnDestroy()
        {

        }

        public void OnGUI()
        {
            _fileHandleView.Draw();
        }

    }

    public class HSMFileHandleModel
    {


    }

    public class HSMFileHandleView
    {

        public void Draw()
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("选择文件"))
                    {
                        HSMFileHandle.SelectFile(HSMManager.Instance.FilePath);
                    }

                    if (GUILayout.Button("保存"))
                    {
                        HSMFileHandle.CreateSaveFile(HSMManager.Instance.FileName);
                        AssetDatabase.Refresh();
                    }

                    if (GUILayout.Button("删除"))
                    {
                        HSMFileHandle.DeleteFile(HSMManager.Instance.FileName);
                        AssetDatabase.Refresh();
                    }

                    if (GUILayout.Button("批量更新"))
                    {
                        HSMFileHandle.UpdateAllFile(HSMManager.Instance.FilePath);
                        AssetDatabase.Refresh();
                    }

                    if (GUILayout.Button("批量合并"))
                    {
                        HSMFileHandle.BeatchMergeAllFile();
                        AssetDatabase.Refresh();
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("文件名", GUILayout.Width(80));
                    HSMManager.Instance.FileName = EditorGUILayout.TextField(HSMManager.Instance.FileName);
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            EditorGUILayout.EndVertical();
        }
    }

    public class HSMFileHandle
    {
        public static void SelectFile(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            GUILayout.Space(8);

            string filePath = EditorUtility.OpenFilePanel("选择技能ID文件", path, "txt");
            if (!string.IsNullOrEmpty(filePath))
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (null != HSMManager.hSMLoadFile)
                    {
                        HSMManager.hSMLoadFile(fileName);
                    }
                }
            }
        }

        public static void CreateSaveFile(string fileName)
        {
            if (null != HSMManager.hSMSaveFile)
            {
                HSMManager.hSMSaveFile(fileName);
            }
        }

        public static void DeleteFile(string fileName)
        {
            if (null != HSMManager.hSMDeleteFile)
            {
                HSMManager.hSMDeleteFile(fileName);
            }
        }

        public static void UpdateAllFile(string filePath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(filePath);
            FileInfo[] fileInfoArr = dInfo.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < fileInfoArr.Length; ++i)
            {
                string fullName = fileInfoArr[i].FullName;
                HSMReadWrite readWrite = new HSMReadWrite();
                SkillHsmConfigHSMTreeData skillHsmData = readWrite.ReadJson(fullName);

                string fileName = System.IO.Path.GetFileNameWithoutExtension(fullName);
                skillHsmData.FileName = fileName;

                HSMManager.Instance.CheckData(skillHsmData);

                skillHsmData = UpdateData(skillHsmData);

                string jsonFilePath = Extend.GameUtils.CombinePath(filePath, "Json", System.IO.Path.GetFileName(fullName));// System.IO.Path.GetDirectoryName(filePath) + "/Json/" + System.IO.Path.GetFileName(fullName);
                jsonFilePath = System.IO.Path.ChangeExtension(jsonFilePath, "txt");
                bool value = readWrite.WriteJson(skillHsmData, jsonFilePath);
                if (!value)
                {
                    Debug.LogError("WriteError:" + jsonFilePath);
                }
            }
        }

        public static SkillHsmConfigHSMTreeData UpdateData(SkillHsmConfigHSMTreeData skillHsmData)
        {
            HashSet<string> useParameter = new HashSet<string>();
            for (int i = 0; i < skillHsmData.NodeList.Count; ++i)
            {
                SkillHsmConfigNodeData nodeData = skillHsmData.NodeList[i];
                for (int j = 0; j < nodeData.TransitionList.Count; ++j)
                {
                    SkillHsmConfigTransition transition = nodeData.TransitionList[j];

                    for (int k = 0; k < transition.GroupList.Count; ++k)
                    {
                        SkillHsmConfigTransitionGroup transitionGroup = transition.GroupList[k];
                        for (int n = 0; n < transitionGroup.ParameterList.Count; ++n)
                        {
                            string parameter = transitionGroup.ParameterList[n];
                            if (!useParameter.Contains(parameter))
                            {
                                useParameter.Add(parameter);
                            }

                            if (parameter.CompareTo("ForcedAbortSkill") == 0)
                            {
                                Debug.LogError(skillHsmData.FileName + "     ForcedAbortSkill:" + nodeData.Id + "    " + transition.ToStateId);
                            }

                            if (parameter.CompareTo("ForcedAbortSkillToFish") == 0)
                            {
                                Debug.LogError(skillHsmData.FileName + "     ForcedAbortSkillToFish:" + nodeData.Id + "    " + transition.ToStateId);
                            }
                        }
                    }
                }
            }

            for (int k = skillHsmData.ParameterList.Count - 1; k >= 0; --k)
            {
                SkillHsmConfigHSMParameter parameter = skillHsmData.ParameterList[k];
                if (!useParameter.Contains(parameter.ParameterName))
                {
                    Debug.LogError(skillHsmData.FileName + "    " + parameter.ParameterName + "    " + parameter.CNName);
                    skillHsmData.ParameterList.RemoveAt(k);
                }
            }

            return skillHsmData;
        }

        public static void BeatchMergeAllFile()
        {
            string filePath = HSMManager.Instance.FilePath;
            DirectoryInfo dInfo = new DirectoryInfo(filePath);
            FileInfo[] fileInfoArr = dInfo.GetFiles("*.txt", SearchOption.TopDirectoryOnly);

            List<PBConfigWriteFile> fileList = new List<PBConfigWriteFile>();
            for (int i = 0; i < fileInfoArr.Length; ++i)
            {
                string fullName = fileInfoArr[i].FullName;
                HSMReadWrite readWrite = new HSMReadWrite();
                SkillHsmConfigHSMTreeData skillHsmData = readWrite.ReadJson(fullName);
                skillHsmData = FormatData(skillHsmData);

                HSMManager.Instance.CheckData(skillHsmData);

                string fileName = System.IO.Path.GetFileNameWithoutExtension(fullName);
                //skillHsmData.FileName = fileName;

                byte[] byteData = ProtoDataUtils.ObjectToBytes<SkillHsmConfigHSMTreeData>(skillHsmData);
                if (byteData.Length <= 0)
                {
                    Debug.LogError("无效得配置文件");
                    return;
                }

                PBConfigWriteFile skillConfigWriteFile = new PBConfigWriteFile();
                skillConfigWriteFile.filePath = filePath;
                skillConfigWriteFile.byteData = byteData;
                fileList.Add(skillConfigWriteFile);

                Debug.Log("end mergeFile:" + filePath);
            }

            ByteBufferWrite bbw = new ByteBufferWrite();
            bbw.WriteInt32(fileList.Count);

            int start = 4 + fileList.Count * (4 + 4);
            for (int i = 0; i < fileList.Count; ++i)
            {
                PBConfigWriteFile skillConfigWriteFile = fileList[i];
                bbw.WriteInt32(start);
                bbw.WriteInt32(skillConfigWriteFile.byteData.Length);
                start += skillConfigWriteFile.byteData.Length;
            }

            for (int i = 0; i < fileList.Count; ++i)
            {
                PBConfigWriteFile skillHsmWriteFile = fileList[i];
                bbw.WriteBytes(skillHsmWriteFile.byteData, skillHsmWriteFile.byteData.Length);
            }

            {
                string mergeFilePath = Extend.GameUtils.CombinePath(Application.dataPath, "StreamingAssets", "Bina", "SkillHsmConfig.bytes"); //string.Format("{0}/StreamingAssets/Bina/SkillHsmConfig.bytes", Application.dataPath);   // string.Format("{0}/MergeFile/SkillConfig.txt", path);

                if (System.IO.File.Exists(mergeFilePath))
                {
                    System.IO.File.Delete(mergeFilePath);
                    AssetDatabase.Refresh();
                }
                byte[] byteData = bbw.GetBytes();
                FileReadWrite.Write(mergeFilePath, byteData);
            }
        }

        private static SkillHsmConfigHSMTreeData FormatData(SkillHsmConfigHSMTreeData skillHsmData)
        {
            Dictionary<int, SkillHsmConfigNodeData> nodeDic = new Dictionary<int, SkillHsmConfigNodeData>();
            for (int i = 0; i < skillHsmData.NodeList.Count; ++i)
            {
                SkillHsmConfigNodeData nodeValue = skillHsmData.NodeList[i];
                nodeDic[nodeValue.Id] = nodeValue;
            }

            for (int i = 0; i < skillHsmData.NodeList.Count; ++i)
            {
                SkillHsmConfigNodeData nodeValue = skillHsmData.NodeList[i];
                if (nodeValue.ChildIdList.Count <= 0)
                {
                    continue;
                }

                for (int j = 0; j < nodeValue.ChildIdList.Count; ++j)
                {
                    int childId = nodeValue.ChildIdList[j];
                    SkillHsmConfigNodeData childNode = nodeDic[childId];
                    if (null == childNode)
                    {
                        continue;
                    }

                    childNode.ParentId = nodeValue.Id;
                }
            }

            return skillHsmData;
        }

        public static void ImportParameter()
        {
            Debug.LogError("ImportParameter");
            //string path = "Assets/StreamingAssets/CSV/";
            //if (!System.IO.Directory.Exists(path))
            //{
            //    System.IO.Directory.CreateDirectory(path);
            //}
            //GUILayout.Space(8);

            //string filePath = EditorUtility.OpenFilePanel("选择技能ID文件", path, "csv");
            //if (string.IsNullOrEmpty(filePath))
            //{
            //    return;
            //}

            //string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            //if (string.IsNullOrEmpty(fileName))
            //{
            //    return;
            //}

            string fileName = "Hsm";

            SkillHsmConfigHSMTreeData hsmData = HSMManager.Instance.HSMTreeData;
            hsmData = ImportParameter(hsmData, fileName);
        }

        private static SkillHsmConfigHSMTreeData ImportParameter(SkillHsmConfigHSMTreeData hsmData, string fileName)
        {
            Debug.LogError(hsmData.FileName);
            TableRead.Instance.Init();
            string csvPath = Extend.GameUtils.CombinePath(Application.dataPath, "StreamingAssets", "CSV"); //string.Format("{0}/StreamingAssets/CSV/", Application.dataPath);
            TableRead.Instance.ReadCustomPath(csvPath);

            // Debug.LogError(filePath + "   " + fileName);
            List<int> keyList = TableRead.Instance.GetKeyList(fileName);

            Dictionary<string, SkillHsmConfigHSMParameter> parameterDic = new Dictionary<string, SkillHsmConfigHSMParameter>();
            for (int i = 0; i < hsmData.ParameterList.Count; ++i)
            {
                SkillHsmConfigHSMParameter parameter = hsmData.ParameterList[i];
                parameterDic[parameter.ParameterName] = parameter;
            }

            for (int i = 0; i < keyList.Count; ++i)
            {
                int key = keyList[i];
                string EnName = TableRead.Instance.GetData(fileName, key, "EnName");
                string cnName = TableRead.Instance.GetData(fileName, key, "CnName");
                string typeName = TableRead.Instance.GetData(fileName, key, "Type");
                int type = int.Parse(typeName);

                string floatContent = TableRead.Instance.GetData(fileName, key, "FloatValue");
                float floatValue = float.Parse(floatContent);

                string intContent = TableRead.Instance.GetData(fileName, key, "IntValue");
                int intValue = int.Parse(intContent);

                string boolContent = TableRead.Instance.GetData(fileName, key, "BoolValue");
                bool boolValue = (int.Parse(boolContent) == 1);

                if (parameterDic.ContainsKey(EnName))
                {
                    if (parameterDic[EnName].ParameterType != type)
                    {
                        Debug.LogError("已经存在参数:" + EnName + "   type:" + (HSMParameterType)parameterDic[EnName].ParameterType + "   newType:" + (HSMParameterType)type);
                    }
                    else
                    {
                        Debug.LogError("已经存在参数:" + EnName);
                    }
                    parameterDic.Remove(EnName);

                    for (int j = 0; j < hsmData.ParameterList.Count; ++j)
                    {
                        SkillHsmConfigHSMParameter cacheParameter = hsmData.ParameterList[j];
                        if (cacheParameter.ParameterName == EnName)
                        {
                            hsmData.ParameterList.RemoveAt(j);
                            break;
                        }
                    }

                    //continue;
                }

                //Debug.LogError(EnName + "    " +cnName + "    " + typeName);

                SkillHsmConfigHSMParameter parameter = new SkillHsmConfigHSMParameter();
                parameter.ParameterName = EnName;
                parameter.CNName = cnName;
                parameter.Compare = (int)HSMCompare.EQUALS;
                parameter.ParameterType = type;
                parameter.BoolValue = false;

                if (type == (int)HSMParameterType.Float)
                {
                    parameter.FloatValue = floatValue;
                }

                if (type == (int)HSMParameterType.Int)
                {
                    parameter.IntValue = intValue;
                }

                if (type == (int)HSMParameterType.Float)
                {
                    parameter.BoolValue = boolValue;
                }

                hsmData.ParameterList.Add(parameter);
            }

            foreach (var kv in parameterDic)
            {
                Debug.LogError("==========缺失的参数:" + kv.Key);
            }

            return hsmData;
        }

    }

}

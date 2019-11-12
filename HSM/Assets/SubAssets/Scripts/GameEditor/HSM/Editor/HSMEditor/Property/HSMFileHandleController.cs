using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;

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
                        SelectFile(HSMManager.Instance.FilePath);
                    }

                    if (GUILayout.Button("保存"))
                    {
                        CreateSaveFile(HSMManager.Instance.FileName);
                        AssetDatabase.Refresh();
                    }

                    if (GUILayout.Button("删除"))
                    {
                        DeleteFile(HSMManager.Instance.FileName);
                        AssetDatabase.Refresh();
                    }

                    if (GUILayout.Button("批量更新"))
                    {
                        UpdateAllFile(HSMManager.Instance.FilePath);
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
            }
            EditorGUILayout.EndVertical();
        }

        private static void SelectFile(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            GUILayout.Space(8);

            string filePath = EditorUtility.OpenFilePanel("选择技能ID文件", path, "bytes");
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

        private static void CreateSaveFile(string fileName)
        {
            if (null != HSMManager.hSMSaveFile)
            {
                HSMManager.hSMSaveFile(fileName);
            }
        }

        private static void DeleteFile(string fileName)
        {
            if (null != HSMManager.hSMDeleteFile)
            {
                HSMManager.hSMDeleteFile(fileName);
            }
        }

        private static void UpdateAllFile(string filePath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(filePath);
            FileInfo[] fileInfoArr = dInfo.GetFiles("*.bytes", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < fileInfoArr.Length; ++i)
            {
                string fullName = fileInfoArr[i].FullName;
                HSMReadWrite readWrite = new HSMReadWrite();
                HSMTreeData treeData = readWrite.ReadJson(fullName);

                treeData = UpdateData(treeData);

                string jsonFilePath = System.IO.Path.GetDirectoryName(filePath) + "/Json/" + System.IO.Path.GetFileName(fullName);
                bool value = readWrite.WriteJson(treeData, jsonFilePath);
                if (!value)
                {
                    Debug.LogError("WriteError:" + jsonFilePath);
                }
            }
        }

        private static HSMTreeData UpdateData(HSMTreeData treeData)
        {
            for (int i = 0; i < treeData.nodeList.Count; ++i)
            {
                NodeData nodeData = treeData.nodeList[i];
                for (int j = 0; j < nodeData.transitionList.Count; ++j)
                {
                    Transition transition = nodeData.transitionList[j];
                    for (int k = 0; k < transition.parameterList.Count; ++k)
                    {
                        HSMParameter hsmParameter = transition.parameterList[k];
                        if (hsmParameter.parameterType == (int)HSMParameterType.Bool)
                        {
                            hsmParameter.compare = (int)HSMCompare.EQUALS;
                        }
                    }
                }
            }
            return treeData;
        }

    }
}

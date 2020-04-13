using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;
using GenPB;

namespace HSMTree
{
    public class HSMReadWrite
    {
        #region HSMTreeData
        public bool WriteJson(SkillHsmConfigHSMTreeData data, string filePath)
        {
            string content = LitJson.JsonMapper.ToJson(data);
            bool value = FileReadWrite.Write(filePath, content);

            if (value)
            {
                //Debug.Log("Write Sucess:" + filePath);
            }
            else
            {
                //Debug.LogError("Write Fail:" + filePath);
            }

            return value;
        }

        public SkillHsmConfigHSMTreeData ReadJson(string filePath)
        {
            //Debug.Log("Read:" + filePath);
            SkillHsmConfigHSMTreeData HSMData = new SkillHsmConfigHSMTreeData();

            string content = FileReadWrite.Read(filePath);
            if (string.IsNullOrEmpty(content))
            {
                return HSMData;
            }

            JsonData jsonData = JsonMapper.ToObject(content);

            if (((IDictionary)jsonData).Contains("FileName"))
            {
                HSMData.FileName = jsonData["FileName"].ToString();
            }
            HSMData.DefaultStateId = int.Parse(jsonData["DefaultStateId"].ToString());

            if (((IDictionary)jsonData).Contains("NodeList"))
            {
                JsonData nodeList = jsonData["NodeList"];
                List<SkillHsmConfigNodeData> dataList = GetNodeList(nodeList);
                HSMData.NodeList.AddRange(dataList);
            }

            if (((IDictionary)jsonData).Contains("ParameterList"))
            {
                JsonData parameterList = jsonData["ParameterList"];
                List<SkillHsmConfigHSMParameter> dataList = GetParameterList(parameterList);
                HSMData.ParameterList.AddRange(dataList);
            }

            HSMData.Descript = jsonData["Descript"].ToString();

            return HSMData;
        }

        private List<SkillHsmConfigNodeData> GetNodeList(JsonData data)
        {
            List<SkillHsmConfigNodeData> nodeList = new List<SkillHsmConfigNodeData>();

            foreach (JsonData item in data)
            {
                SkillHsmConfigNodeData nodeValue = new SkillHsmConfigNodeData();
                nodeValue.Id = int.Parse(item["Id"].ToString());
                nodeValue.NodeType = int.Parse(item["NodeType"].ToString());
                nodeValue.NodeName = item["NodeName"].ToString();
                nodeValue.Identification = int.Parse(item["Identification"].ToString());
                nodeValue.Descript = item["Descript"].ToString();


                if (((IDictionary)item).Contains("ParameterList"))
                {
                    JsonData parameterList = item["ParameterList"];
                    List<SkillHsmConfigHSMParameter> dataList = GetParameterList(parameterList);
                    nodeValue.ParameterList.AddRange(dataList);
                }

                if (((IDictionary)item).Contains("TransitionList"))
                {
                    JsonData transitionList = item["TransitionList"];
                    List<SkillHsmConfigTransition> dataList = GetTransitionList(transitionList);
                    nodeValue.TransitionList.AddRange(dataList);
                }

                if (((IDictionary)item).Contains("Position"))
                {
                    JsonData position = item["Position"];
                    nodeValue.Position = GetPosition(position);
                }

                if (((IDictionary)item).Contains("ChildIdList"))
                {
                    JsonData childIdList = item["ChildIdList"];
                    nodeValue.ChildIdList.AddRange(GetIntList(childIdList));
                }

                if (((IDictionary)item).Contains("ParentId"))
                {
                    nodeValue.ParentId = int.Parse(item["ParentId"].ToString());
                }

                nodeList.Add(nodeValue);
            }

            return nodeList;
        }

        private List<SkillHsmConfigTransition> GetTransitionList(JsonData jsonData)
        {
            List<SkillHsmConfigTransition> transitionList = new List<SkillHsmConfigTransition>();
            foreach (JsonData item in jsonData)
            {
                SkillHsmConfigTransition transition = new SkillHsmConfigTransition();
                transition.TransitionId = int.Parse(item["TransitionId"].ToString());
                transition.ToStateId = int.Parse(item["ToStateId"].ToString());
                List<SkillHsmConfigHSMParameter> dataList = GetParameterList(item["ParameterList"]);
                transition.ParameterList.AddRange(dataList);

                List<SkillHsmConfigTransitionGroup> groupList = GetTransitionGroup(item["GroupList"]);
                transition.GroupList.AddRange(groupList);

                transitionList.Add(transition);
            }

            return transitionList;
        }

        private List<SkillHsmConfigTransitionGroup> GetTransitionGroup(JsonData jsonData)
        {
            List<SkillHsmConfigTransitionGroup> groupList = new List<SkillHsmConfigTransitionGroup>();
            foreach (JsonData item in jsonData)
            {
                SkillHsmConfigTransitionGroup group = new SkillHsmConfigTransitionGroup();
                group.Index = int.Parse(item["Index"].ToString());

                List<string> parameterList = GetStringList(item["ParameterList"]);
                group.ParameterList.AddRange(parameterList);

                groupList.Add(group);
            }

            return groupList;
        }

        private List<int> GetIntList(JsonData jsonData)
        {
            List<int> valueList = new List<int>();

            for (int i = 0; i < jsonData.Count; ++i)
            {
                int value = int.Parse(jsonData[i].ToString());
                valueList.Add(value);
            }

            return valueList;
        }

        private List<string> GetStringList(JsonData jsonData)
        {
            List<string> stringList = new List<string>();

            for (int i = 0; i < jsonData.Count; ++i)
            {
                string str = jsonData[i].ToString();
                stringList.Add(str);
            }

            return stringList;
        }

        private SkillHsmConfigRectT GetPosition(JsonData data)
        {
            SkillHsmConfigRectT position = new SkillHsmConfigRectT();

            position.X = int.Parse(data["X"].ToString());
            position.Y = int.Parse(data["Y"].ToString());
            position.Width = int.Parse(data["Width"].ToString());
            position.Height = int.Parse(data["Height"].ToString());

            return position;
        }

        private List<SkillHsmConfigHSMParameter> GetParameterList(JsonData data)
        {
            List<SkillHsmConfigHSMParameter> dataList = new List<SkillHsmConfigHSMParameter>();
            foreach (JsonData item in data)
            {
                SkillHsmConfigHSMParameter parameter = new SkillHsmConfigHSMParameter();
                parameter.ParameterType = int.Parse(item["ParameterType"].ToString());
                parameter.ParameterName = item["ParameterName"].ToString();
                parameter.CNName = item["CNName"].ToString();
                parameter.IntValue = int.Parse(item["IntValue"].ToString());
                parameter.FloatValue = float.Parse(item["FloatValue"].ToString());
                parameter.BoolValue = bool.Parse(item["BoolValue"].ToString());
                parameter.Compare = int.Parse(item["Compare"].ToString());
                parameter.Index = int.Parse(item["Index"].ToString());

                dataList.Add(parameter);
            }

            return dataList;
        }
        #endregion
    }

}

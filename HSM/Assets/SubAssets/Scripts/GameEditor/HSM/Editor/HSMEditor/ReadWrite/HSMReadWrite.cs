using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;

namespace HSMTree
{
    public class HSMReadWrite
    {
        #region HSMTreeData
        public bool WriteJson(HSMTreeData data, string filePath)
        {
            string content = LitJson.JsonMapper.ToJson(data);
            bool value = FileReadWrite.Write(filePath, content);

            if (value)
            {
                Debug.Log("Write Sucess:" + filePath);
            }
            else
            {
                Debug.LogError("Write Fail:" + filePath);
            }

            return value;
        }

        public HSMTreeData ReadJson(string filePath)
        {
            Debug.Log("Read:" + filePath);
            HSMTreeData HSMData = new HSMTreeData();

            string content = FileReadWrite.Read(filePath);
            if (string.IsNullOrEmpty(content))
            {
                return HSMData;
            }

            JsonData jsonData = JsonMapper.ToObject(content);

            HSMData.defaultStateId = int.Parse(jsonData["defaultStateId"].ToString());

            if (((IDictionary)jsonData).Contains("nodeList"))
            {
                JsonData nodeList = jsonData["nodeList"];
                HSMData.nodeList = GetNodeList(nodeList);
            }

            if (((IDictionary)jsonData).Contains("parameterList"))
            {
                JsonData parameterList = jsonData["parameterList"];
                HSMData.parameterList = GetParameterList(parameterList);
            }

            HSMData.descript = jsonData["descript"].ToString();

            return HSMData;
        }

        private List<NodeData> GetNodeList(JsonData data)
        {
            List<NodeData> nodeList = new List<NodeData>();

            foreach (JsonData item in data)
            {
                NodeData nodeValue = new NodeData();
                nodeValue.id = int.Parse(item["id"].ToString());
                nodeValue.NodeType = int.Parse(item["NodeType"].ToString());
                nodeValue.nodeName = item["nodeName"].ToString();
                nodeValue.identification = int.Parse(item["identification"].ToString());
                nodeValue.descript = item["descript"].ToString();


                if (((IDictionary)item).Contains("parameterList"))
                {
                    JsonData parameterList = item["parameterList"];
                    nodeValue.parameterList = GetParameterList(parameterList);
                }

                if (((IDictionary)item).Contains("transitionList"))
                {
                    JsonData transitionList = item["transitionList"];
                    nodeValue.transitionList = GetTransitionList(transitionList);
                }

                if (((IDictionary)item).Contains("position"))
                {
                    JsonData position = item["position"];
                    nodeValue.position = GetPosition(position);
                }

                nodeList.Add(nodeValue);
            }

            return nodeList;
        }

        private List<Transition> GetTransitionList(JsonData jsonData)
        {
            List<Transition> transitionList = new List<Transition>();
            foreach (JsonData item in jsonData)
            {
                Transition transition = new Transition();
                transition.transitionId = int.Parse(item["transitionId"].ToString());
                transition.toStateId = int.Parse(item["toStateId"].ToString());
                transition.parameterList = GetParameterList(item["parameterList"]);

                transitionList.Add(transition);
            }

            return transitionList;
        }

        private RectT GetPosition(JsonData data)
        {
            float x = int.Parse(data["x"].ToString());
            float y = int.Parse(data["y"].ToString());
            float width = int.Parse(data["width"].ToString());
            float height = int.Parse(data["height"].ToString());

            RectT position = new RectT(x, y, width, height);
            return position;
        }

        private List<HSMParameter> GetParameterList(JsonData data)
        {
            List<HSMParameter> dataList = new List<HSMParameter>();
            foreach (JsonData item in data)
            {
                HSMParameter parameter = new HSMParameter();
                parameter.parameterType = int.Parse(item["parameterType"].ToString());
                parameter.parameterName = item["parameterName"].ToString();
                parameter.intValue = int.Parse(item["intValue"].ToString());
                parameter.floatValue = float.Parse(item["floatValue"].ToString());
                parameter.boolValue = bool.Parse(item["boolValue"].ToString());
                parameter.compare = int.Parse(item["compare"].ToString());

                dataList.Add(parameter);
            }

            return dataList;
        }
        #endregion
    }

}

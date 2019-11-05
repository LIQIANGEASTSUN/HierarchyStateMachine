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

            HSMData.rootNodeId = int.Parse(jsonData["rootNodeId"].ToString());

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

        private List<NodeValue> GetNodeList(JsonData data)
        {
            List<NodeValue> nodeList = new List<NodeValue>();

            foreach (JsonData item in data)
            {
                NodeValue nodeValue = new NodeValue();
                nodeValue.id = int.Parse(item["id"].ToString());
                nodeValue.NodeType = int.Parse(item["NodeType"].ToString());
                nodeValue.repeatTimes = int.Parse(item["repeatTimes"].ToString());
                nodeValue.nodeName = item["nodeName"].ToString();
                nodeValue.identification = int.Parse(item["identification"].ToString());
                nodeValue.descript = item["descript"].ToString();

                if (((IDictionary)item).Contains("childNodeList"))
                {
                    JsonData childNodeList = item["childNodeList"];
                    nodeValue.childNodeList = GetChildIdList(childNodeList);
                }

                if (((IDictionary)item).Contains("parameterList"))
                {
                    JsonData parameterList = item["parameterList"];
                    nodeValue.parameterList = GetParameterList(parameterList);
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

        private List<int> GetChildIdList(JsonData jsonData)
        {
            List<int> childIdList = new List<int>();
            for (int i = 0; i < jsonData.Count; ++i)
            {
                int value = int.Parse(jsonData[i].ToString());
                childIdList.Add(value);
            }

            return childIdList;
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

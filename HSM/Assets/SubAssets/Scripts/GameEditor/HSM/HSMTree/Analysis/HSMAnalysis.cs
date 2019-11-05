using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;


namespace HSMTree
{

    public class HSMAnalysis
    {
        public HSMAnalysis()
        {

        }

        public StateBase Analysis(string content, IAction iAction, IConditionCheck iConditionCheck)
        {
            HSMTreeData HSMTreeData = JsonMapper.ToObject<HSMTreeData>(content);
            if (null == HSMTreeData)
            {
                Debug.LogError("HSMTreeData is null");
                return null;
            }

            iConditionCheck.AddParameter(HSMTreeData.parameterList);
            return Analysis(HSMTreeData, iAction, iConditionCheck);
        }

        public StateBase Analysis(HSMTreeData data, IAction iAction, IConditionCheck iConditionCheck)
        {
            StateBase rootNode = null;

            if (null == data)
            {
                Debug.LogError("数据无效");
                return rootNode;
            }

            if (data.rootNodeId < 0)
            {
                Debug.LogError("没有跟节点");
                return rootNode;
            }

            iConditionCheck.AddParameter(data.parameterList);

            Dictionary<int, StateBase> allNodeDic = new Dictionary<int, StateBase>();
            Dictionary<int, List<int>> childDic = new Dictionary<int, List<int>>();
            for (int i = 0; i < data.nodeList.Count; ++i)
            {
                NodeValue nodeValue = data.nodeList[i];
                StateBase nodeBase = AnalysisNode(nodeValue, iAction, iConditionCheck);
                //nodeBase.NodeId = nodeValue.id;
                //if (nodeValue.NodeType == (int)NODE_TYPE.CONDITION     // 条件节点
                //    || (nodeValue.NodeType == (int)NODE_TYPE.ACTION))  // 行为节点
                //{
                //    nodeLeafList.Add((NodeLeaf)nodeBase);
                //}

                if (null == nodeBase)
                {
                    Debug.LogError("AllNODE:" + nodeValue.id + "     " + (null != nodeBase));
                }
                allNodeDic[nodeValue.id] = nodeBase;
            }

            return rootNode;
        }

        private bool IsLeafNode(int type)
        {
            return (type == (int)NODE_TYPE.STATE) || (type == (int)NODE_TYPE.STATE);
        }

        private StateBase AnalysisNode(NodeValue nodeValue, IAction iAction, IConditionCheck iConditionCheck)
        {
            StateBase node = null;
            if (nodeValue.NodeType == (int)NODE_TYPE.STATE)  // 条件节点
            {
                //return GetCondition(nodeValue, iConditionCheck);
            }

            if (nodeValue.NodeType == (int)NODE_TYPE.STATE)  // 行为节点
            {
                //return GetAction(nodeValue, iAction);
            }

            return node;
        }

        //public NodeCondition GetCondition(NodeValue nodeValue, IConditionCheck iConditionCheck)
        //{
        //    NodeCondition condition = (NodeCondition)CustomNode.Instance.GetNode((IDENTIFICATION)nodeValue.identification);
        //    condition.SetParameters(nodeValue.parameterList);
        //    condition.SetConditionCheck(iConditionCheck);
        //    return condition;
        //}

        //public NodeAction GetAction(NodeValue nodeValue, IAction iAction)
        //{
        //    NodeAction action = (NodeAction)CustomNode.Instance.GetNode((IDENTIFICATION)nodeValue.identification);
        //    action.SetIAction(iAction);
        //    action.SetParameters(nodeValue.parameterList);
        //    return action;
        //}

    }

}


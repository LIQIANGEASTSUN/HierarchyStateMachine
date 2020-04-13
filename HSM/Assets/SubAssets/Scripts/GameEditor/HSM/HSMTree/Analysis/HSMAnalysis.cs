using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using GenPB;

namespace HSMTree
{

    public class HSMAnalysis
    {
        public HSMAnalysis()
        {

        }

        public void Analysis(string content, IConditionCheck iConditionCheck, IRegisterNode iRegisterNode, ref HSMStateMachine hsmStateMachine)
        {
            SkillHsmConfigHSMTreeData hSMTreeData = JsonMapper.ToObject<SkillHsmConfigHSMTreeData>(content);
            if (null == hSMTreeData)
            {
                Debug.LogError("HSMTreeData is null");
                return;
            }

            iConditionCheck.AddParameter(hSMTreeData.ParameterList);
            Analysis(hSMTreeData, iConditionCheck, iRegisterNode, ref hsmStateMachine);
        }

        public void Analysis(SkillHsmConfigHSMTreeData data, IConditionCheck iConditionCheck, IRegisterNode iRegisterNode, ref HSMStateMachine hsmStateMachine)
        {
            hsmStateMachine = new HSMStateMachine();

            if (null == data)
            {
                Debug.LogError("数据无效");
                return;
            }

            hsmStateMachine.SetDefaultStateId(data.DefaultStateId);
            iConditionCheck.AddParameter(data.ParameterList);

            Dictionary<int, AbstractNode> abstractNodeDic = AnalysisAllNode(data.NodeList, iConditionCheck);
            hsmStateMachine.AddAllNode(abstractNodeDic);
            foreach(var kv in abstractNodeDic)
            {
                AbstractNode parentNode = kv.Value;
                if (parentNode.ParentId < 0)
                {
                    hsmStateMachine.AddChildNode(parentNode);
                    iRegisterNode.RegisterNode(parentNode);
                }

                if (parentNode.ChildIdList.Count <= 0)
                {
                    continue;
                }

                for (int j = 0; j < parentNode.ChildIdList.Count; ++j)
                {
                    int childId = parentNode.ChildIdList[j];

                    AbstractNode childNode = null;
                    if (!abstractNodeDic.TryGetValue(childId, out childNode))
                    {
                        continue;
                    }

                    childNode.SetParentSubMachine(parentNode);
                    parentNode.AddChildNode(childNode);
                    iRegisterNode.RegisterNode(childNode);
                }
            }
        }

        private Dictionary<int, AbstractNode> AnalysisAllNode(List<SkillHsmConfigNodeData> nodeList , IConditionCheck iConditionCheck)
        {
            Dictionary<int, AbstractNode> abstractNodeDic = new Dictionary<int, AbstractNode>();

            nodeList.Sort((a, b) => {
                return b.ChildIdList.Count - a.ChildIdList.Count;
            });

            for (int i = 0; i < nodeList.Count; ++i)
            {
                SkillHsmConfigNodeData nodeValue = nodeList[i];
                AbstractNode abstractNode = AnalysisNode(nodeValue);
                abstractNode.NodeId = nodeValue.Id;
                abstractNode.ParentId = nodeValue.ParentId;
                abstractNode.ChildIdList.AddRange(nodeValue.ChildIdList);
                abstractNode.NodeType = (NODE_TYPE)nodeValue.NodeType;
                abstractNode.AddParameter(nodeValue.ParameterList);
                abstractNode.AddTransition(nodeValue.TransitionList);
                abstractNode.SetConditionCheck(iConditionCheck);
                abstractNode.Init();

                abstractNodeDic[nodeValue.Id] = abstractNode;
            }

            return abstractNodeDic;
        }

        private AbstractNode AnalysisNode(SkillHsmConfigNodeData nodeValue)
        {
            AbstractNode node = (AbstractNode)CustomNode.Instance.GetState(nodeValue.Identification);
            return node;
        }

    }

}


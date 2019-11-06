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

        public HSMStateMachine Analysis(string content, IConditionCheck iConditionCheck)
        {
            HSMTreeData HSMTreeData = JsonMapper.ToObject<HSMTreeData>(content);
            if (null == HSMTreeData)
            {
                Debug.LogError("HSMTreeData is null");
                return null;
            }

            iConditionCheck.AddParameter(HSMTreeData.parameterList);
            return Analysis(HSMTreeData, iConditionCheck);
        }

        public HSMStateMachine Analysis(HSMTreeData data, IConditionCheck iConditionCheck)
        {
            HSMStateMachine hsmStateMachine = new HSMStateMachine();

            if (null == data)
            {
                Debug.LogError("数据无效");
                return hsmStateMachine;
            }

            if (data.defaultStateId < 0)
            {
                Debug.LogError("没有默认节点");
                return hsmStateMachine;
            }

            hsmStateMachine.CurrentStateId = data.defaultStateId;

            iConditionCheck.AddParameter(data.parameterList);

            for (int i = 0; i < data.nodeList.Count; ++i)
            {
                NodeValue nodeValue = data.nodeList[i];
                HSMState stateBase = AnalysisNode(nodeValue, iConditionCheck);
                if (null == stateBase)
                {
                    Debug.LogError("AllNODE:" + nodeValue.id + "     " + (null != stateBase));
                }
                stateBase.SetConditionCheck(iConditionCheck);
                hsmStateMachine.AddState(stateBase);
            }

            return hsmStateMachine;
        }

        private HSMState AnalysisNode(NodeValue nodeValue, IConditionCheck iConditionCheck)
        {
            HSMState state = (HSMState)CustomNode.Instance.GetState((IDENTIFICATION)nodeValue.identification);
            state.StateId = nodeValue.id;
            state.AddTransition(nodeValue.transitionList);
            return state;
        }

    }

}


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

        public HSMStateMachine Analysis(string content, IConditionCheck iConditionCheck, IAction iAction)
        {
            HSMTreeData hSMTreeData = JsonMapper.ToObject<HSMTreeData>(content);
            if (null == hSMTreeData)
            {
                Debug.LogError("HSMTreeData is null");
                return null;
            }

            iConditionCheck.AddParameter(hSMTreeData.parameterList);
            return Analysis(hSMTreeData, iConditionCheck, iAction);
        }

        public HSMStateMachine Analysis(HSMTreeData data, IConditionCheck iConditionCheck, IAction iAction)
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

            hsmStateMachine.SetData(data);
            hsmStateMachine.SetDefaultStateId(data.defaultStateId);

            iConditionCheck.AddParameter(data.parameterList);

            for (int i = 0; i < data.nodeList.Count; ++i)
            {
                NodeData nodeValue = data.nodeList[i];
                if (nodeValue.NodeType == (int)(NODE_TYPE.STATE))
                {
                    HSMState stateBase = AnalysisNode(nodeValue, iConditionCheck);
                    if (null == stateBase)
                    {
                        Debug.LogError("AllNODE:" + nodeValue.id + "     " + (null != stateBase));
                    }
                    stateBase.StateId = nodeValue.id;
                    stateBase.AddParameter(nodeValue.parameterList);
                    stateBase.AddTransition(nodeValue.transitionList);
                    stateBase.SetStateMachine(hsmStateMachine);
                    stateBase.SetIAction(iAction);
                    stateBase.SetConditionCheck(iConditionCheck);
                    stateBase.Init();
                    hsmStateMachine.AddState(stateBase);
                }
                else if (nodeValue.NodeType == (int)NODE_TYPE.SUB_STATE_MACHINE)
                {
                    HSMSubStateMachine subStateMachine = new HSMSubStateMachine();
                    for (int j = 0; j < nodeValue.transitionList.Count; ++i)
                    {
                        Transition transition = nodeValue.transitionList[i];
                        subStateMachine.AddChildState(transition.toStateId);
                    }

                    hsmStateMachine.AddSubMachine(subStateMachine);
                }
            }

            return hsmStateMachine;
        }

        private HSMState AnalysisNode(NodeData nodeValue, IConditionCheck iConditionCheck)
        {
            HSMState state = (HSMState)CustomNode.Instance.GetState((IDENTIFICATION)nodeValue.identification);
            
            return state;
        }

    }

}


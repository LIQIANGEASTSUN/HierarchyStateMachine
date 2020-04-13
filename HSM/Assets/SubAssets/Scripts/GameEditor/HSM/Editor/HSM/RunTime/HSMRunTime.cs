using GenPB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMTree
{

    public class HSMRunTime : IRegisterNode, IAction
    {
        public static readonly HSMRunTime Instance = new HSMRunTime();

        private HSMStateMachine _hsmStateMachine = null;
        private IConditionCheck _iconditionCheck = null;

        private RunTimeRotateGo _runtimeRotateGo;
        private HSMRunTime()
        {
        }

        public void Init()
        {
            _runtimeRotateGo = new RunTimeRotateGo();
            HSMManager.hSMRuntimePlay += RuntimePlay;
#if UNITY_EDITOR
            NodeNotify.Clear();
#endif
        }

        public void OnDestroy()
        {
            _runtimeRotateGo.OnDestroy();
            HSMManager.hSMRuntimePlay -= RuntimePlay;
        }

        public void Reset(SkillHsmConfigHSMTreeData HSMTreeData)
        {
            HSMAnalysis analysis = new HSMAnalysis();
            _iconditionCheck = new ConditionCheck();
            analysis.Analysis(HSMTreeData, _iconditionCheck, this, ref _hsmStateMachine);
            _hsmStateMachine.SetAutoTransitionState(true);
        }

        public ConditionCheck ConditionCheck
        {
            get { return (ConditionCheck)_iconditionCheck; }
        }

        public void Update()
        {
            _runtimeRotateGo.Update();
            Execute();
        }

        public void Execute()
        {
            if (HSMManager.Instance.PlayType == HSMPlayType.STOP
                || (HSMManager.Instance.PlayType == HSMPlayType.PAUSE))
            {
                return;
            }

            if (null != _hsmStateMachine)
            {
                _hsmStateMachine.Execute();
            }
        }

        private void RuntimePlay(HSMPlayType state)
        {
            if (state == HSMPlayType.PLAY || state == HSMPlayType.STOP)
            {
                if (null != _hsmStateMachine)
                {
                    _hsmStateMachine.Clear();
                }
            }
        }

        #region IAction
        public void DoAction(int skillConfigIndex, HSMState toState)
        {
            Debug.Log("DoAction: skillConfigIndex:" + skillConfigIndex + "      nodeId:" + toState.NodeId);
        }
        #endregion

        #region IRegisterNode
        public void RegisterNode(AbstractNode node)
        {
            //Debug.Log("RegisterNode:" + node.NodeId);
        }
        #endregion
    }

}


// 编辑器模式下如果所有物体都静止  Time.realtimeSinceStartup 有时候会出bug，数值一直不变
public class RunTimeRotateGo
{
    private GameObject go;

    public RunTimeRotateGo()
    {

    }

    public void OnDestroy()
    {
        GameObject.DestroyImmediate(go);
    }

    public void Update()
    {
        if (!go)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = Vector3.zero;
            go.transform.localScale = Vector3.one * 0.001f;
        }
        go.transform.Rotate(0, 5, 0);
    }

}


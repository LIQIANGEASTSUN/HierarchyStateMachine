using HSMTree;
using GenPB;
using System.Collections.Generic;

#if A

using Utility;

public class AbilityHSM : IAbilityEnvironment, IRegisterNode
{
    private Skill _skill;

    private HSMStateMachine _hsmStateMachine = null;
    private IConditionCheck _iconditionCheck = null;
    private Dictionary<int, SkillPhaseState> _stateDic = new Dictionary<int, SkillPhaseState>();
    private int _lastConfigIndex = -1;
    private static SeedSpawn _seedSpawn = new SeedSpawn(-10, 50);

    public AbilityHSM(Skill skill)
    {
        _skill = skill;
        Init();
    }

    public void Init()
    {
        SkillHsmConfigHSMTreeData skillHsmData = SkillEventHandler.Instance.GetSkillHsmConfigData(_skill.SkillData.HandleFile);
        if (null == skillHsmData)
        {
            UnityEngine.Debug.LogError("SkillHsmData is null:" + _skill.weaponId + "    " + _skill.SkillData.HandleFile);
            return;
        }

        _iconditionCheck = new ConditionCheck();

        HSMAnalysis analysis = new HSMAnalysis();
        analysis.Analysis(skillHsmData, _iconditionCheck, this, ref _hsmStateMachine);
        if (null != _hsmStateMachine)
        {
            _hsmStateMachine.SetAutoTransitionState(false);
        }

        Clear();
    }

    public void Update()
    {
        UpdateEnvironment();
        if (null != _hsmStateMachine)
        {
            _hsmStateMachine.Execute();
        }
    }

#region ConditionCheck
    public ConditionCheck ConditionCheck
    {
        get { return (ConditionCheck)_iconditionCheck; }
    }

    public void SetBtnState(string parameter, bool value)
    {
        //Debug("SetBtn:" + parameter + "    " + value);
        ConditionCheck.SetParameter(parameter, value);
    }

    private void ChangeState(int configIndex, SkillPhaseType phaseType)
    {
        if (phaseType < SkillPhaseType.INITIAL_PHASE || phaseType > SkillPhaseType.NONE)
        {
            return;
        }

        if (_lastConfigIndex != configIndex)
        {
            _lastConfigIndex = configIndex;
            NextAct(false);
        }

        int intValue = (int)phaseType;
        UpdateEnvironment();
        SkillPhaseState state = GetPhaseState(configIndex, (SkillConfigSkillPhaseType)intValue);
        bool result = _hsmStateMachine.StateMachineTransition.ChangeDestinationNode(state);
        //Debug("++++++++++++++++ ChangeNode:" + _skill.weaponId + "    configIndex:" + configIndex + "   phaseType:" + phaseType + "   nodeId:" + state.NodeId + "    " + result + "    time:" + Extend.GameUtils.GetMillisecond());
    }

    private SkillPhaseState GetPhaseState(int configInde, SkillConfigSkillPhaseType phaseType)
    {
        SkillPhaseState state = null;
        int nodeUid = NodeUid(configInde, phaseType);

        if (_stateDic.TryGetValue(nodeUid, out state))
        {
            return state;
        }
        return state;
    }
#endregion
    public void Receive(int weaponId, int configIndex, SkillPhaseType type, float focoPercentage, int ret)
    {
        //Debug("Receive:" + type);
        ChangeState(configIndex, type);
        if (type == SkillPhaseType.SKILL_END)
        {
            Clear();
            //Debug("=============Clear+++++++++++++++++");
        }
    }

    public void Request(int roleId, int weaponId, int configIndex, int type, float focoPercentage)
    {
        if (!CheckRequest((SkillPhaseType)type))
        {
            if ((SkillPhaseType)type == SkillPhaseType.SKILL_END)
            {
                Clear();
            }
            return;
        }

        //Debug("Request: roleId:" + roleId + "    weaponId:" + weaponId + "     configIndex:" + configIndex + "    type:" + (SkillPhaseType)type + "    " + Extend.GameUtils.GetMillisecond());
        int seed = 0;
        if (type == (int)(SkillPhaseType.FIRE_PHASE))
        {
            seed = _seedSpawn.Next();
        }

        SkillEventHandler.Instance.SendSkillOper(roleId, _skill.weaponId, configIndex, (int)type, focoPercentage, 0, seed);
    }

    private bool CheckRequest(SkillPhaseType type)
    {
        bool result = true;
        if (_skill.IsActive)
        {
            result = (type != SkillPhaseType.START);
        }
        else
        {
            result = (type != SkillPhaseType.SKILL_END);
        }
        return result;
    }

    public void NextAct(bool value)
    {
        //Debug("NextAct:" + value);
        ConditionCheck.SetParameter(StateTool.NextAct, value);
    }

    private void ExitCallBack(AbstractNode node)
    {
        //Debug("ExitCallBack:" + node.NodeId + "    parent:" + node.ParentId);
        if (node.ParentId < 0)
        {
            Request(_skill.RoleId, _skill.weaponId, _skill.CurrentConfigIndex, (int)SkillPhaseType.SKILL_END, 0);
        }
    }

    private void EnterNextStage(AbstractNode node)
    {
        //UnityEngine.Debug.LogError("EnterNextStage");
        NextAct(true);
    }

    private void EixtSkill(AbstractNode node)
    {
        //UnityEngine.Debug.LogError("ExitSkill");
        ConditionCheck.SetParameter(StateTool.SkillExit, true);
    }

#region IRegisterNode
    public void RegisterNode(AbstractNode node)
    {
        if (node.NodeType == NODE_TYPE.STATE)
        {
            RegistState(node);
        }
        else if (node.NodeType == NODE_TYPE.EXIT)
        {
            RegistExit(node);
        }
        else if (node.NodeType == NODE_TYPE.SUB_STATE_MACHINE)
        {
            RegistSubMachine(node);
        }
    }

    private void RegistState(AbstractNode node)
    {
        if (typeof(SkillPhaseState).IsAssignableFrom(node.GetType()))
        {
            SkillPhaseState skillPhaseState = (SkillPhaseState)node;
            skillPhaseState.SetSkill(_skill);

            AbstractNode parentNode = _hsmStateMachine.GetNode(node.ParentId);
            int configIndex = (null != parentNode) ? ((SubMachineSkill)parentNode).SkillConfigIndex : -1;
            int nodeUid = NodeUid(configIndex, skillPhaseState.PhaseType);
            _stateDic[nodeUid] = skillPhaseState;
        }
        else if (typeof(SkillHoldSkillState).IsAssignableFrom(node.GetType()))
        {
            SkillHoldSkillState skillHoldSkillState = (SkillHoldSkillState)node;
            skillHoldSkillState.SetSkill(_skill);
        }
        else if (typeof(SkillHoldFishState).IsAssignableFrom(node.GetType()))
        {
            SkillHoldFishState skillHoldFishState = (SkillHoldFishState)node;
            skillHoldFishState.SetSkill(_skill);
        }
        else if (typeof(SkillNextStage).IsAssignableFrom(node.GetType()))
        {
            SkillNextStage skillNextStage = (SkillNextStage)node;
            skillNextStage.SetEnterCallBack(EnterNextStage);
        }
        else if (typeof(SkillExitState).IsAssignableFrom(node.GetType()))
        {
            SkillExitState skillExitState = (SkillExitState)node;
            skillExitState.SetEnterCallBack(EixtSkill);
        }
    }

    private void RegistExit(AbstractNode node)
    {
        HSMStateExit stateExit = (HSMStateExit)node;
        stateExit.SetEnterCallBack(ExitCallBack);
    }

    private void RegistSubMachine(AbstractNode node)
    {
        if (typeof(SubMachineSkill).IsAssignableFrom(node.GetType()))
        {
            SubMachineSkill subMachine = (SubMachineSkill)node;
            subMachine.SetSkill(_skill);
        }
    }

    private int NodeUid(int configIndex, SkillConfigSkillPhaseType phaseType)
    {
        return (configIndex + 1) * 1000 + (int)phaseType;
    }
#endregion

#region IAbilityEnvironment
    public void UpdateEnvironment()
    {
        if (null == ConditionCheck)
        {
            return;
        }

        ConditionCheck.SetParameter(StateTool.CDValid, !_skill.IsCoolDown());
        ConditionCheck.SetParameter(StateTool.SkillEnergyValid, _skill.SkillEnergyEnough());
        ConditionCheck.SetParameter(StateTool.TrajectoryEnergyValid, _skill.TrajectoryEnergyEnough());
        ConditionCheck.SetParameter(StateTool.FireEnergyValid, _skill.FireEnergyEnough());

        ConditionCheck.SetParameter(StateTool.PhaseFinish, _skill.CurrentSkillEntity.skillStateMachine.currentState.TimeFinish);
        ConditionCheck.SetParameter(StateTool.EnableAbortPhase, _skill.CurrentSkillEntity.skillStateMachine.currentState.EnableAbortPhase);
        ConditionCheck.SetParameter(StateTool.EnableAbortSkill, _skill.EnableAbortSkill);
        ConditionCheck.SetParameter(StateTool.EnableAbortSkillToFish, _skill.EnableAbortSkillToFish);

        if (null != _skill.CurrentSkillEntity.FocoEnergia)
        {
            bool focoFull = (_skill.CurrentSkillEntity.FocoEnergia.GetPercentage() >= 1);
            ConditionCheck.SetParameter(StateTool.FocoOverflow, focoFull);
        }
        ConditionCheck.SetParameter(StateTool.ShotTimeFinish, _skill.ShotEndTime());
        ConditionCheck.SetParameter(StateTool.SkillTimeFinish, _skill.TimeEnd);
        ConditionCheck.SetParameter(StateTool.ShotCount, _skill.ShotCountValid());
        ConditionCheck.SetParameter(StateTool.ShotInterval, _skill.EnableNextShot());
        ConditionCheck.SetParameter(StateTool.ShotTarget, _skill.HasTarget());
        ConditionCheck.SetParameter(StateTool.FishHold, _skill.FishHold);
        ConditionCheck.SetParameter(StateTool.Moving, _skill.SpriteMove);

        foreach (var kv in SkillHandleLogicStack.keyDic)
        {
            SetBtnState(kv.Key, kv.Value);
        }

        foreach (var kv in SkillHandleLogicStack._skillHoldDic)
        {
            ConditionCheck.SetParameter(kv.Key, kv.Value);
        }
    }
#endregion

    public void Clear()
    {
        ConditionCheck.InitParmeter();
        if (null != _hsmStateMachine)
        {
            _hsmStateMachine.Clear();
        }
    }

    private void Debug(string msg)
    {
        UnityEngine.Debug.LogError(msg);
    }
}

#endif
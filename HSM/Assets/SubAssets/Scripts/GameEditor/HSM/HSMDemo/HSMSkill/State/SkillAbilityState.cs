using HSMTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if A

public class SkillAbilityState : HSMState, ISkill
{
    private static CustomIdentification _customIdentification = new CustomIdentification("技能/技能节点", IDENTIFICATION.SKILL_ABILITY, typeof(SkillAbilityState), NODE_TYPE.STATE);

    private Skill _skill;
    //private AbilityHSM _abilityHSM;
    private int _skillIndex = -1;
    private bool _end = false;

    private Action<int> _abilityEnterCallBack = null;
    private Action<int> _abilityEndCallBack = null;

    public SkillAbilityState() : base()
    {

    }

    public override void Init()
    {
        SetState();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override int Execute(ref bool result)
    {
        if (!_end)
        {
            //AbilityHSM.Update();
        }
        return base.Execute(ref result);
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void SetState()
    {
        for (int i = 0; i < _parameterList.Count; ++i)
        {
            HSMParameter parameter = _parameterList[i];
            if (parameter.parameterName.CompareTo(StateTool.AbilityHSM) == 0)
            {
                _skillIndex = parameter.intValue;
                continue;
            }
        }
    }

    public int SkillIndex { get { return _skillIndex; } }

    //public AbilityHSM AbilityHSM
    //{
    //    get { return _abilityHSM; }
    //}

    public static CustomIdentification CustomIdentification()
    {
        return _customIdentification;
    }

    public override void DoAction(HSMState toState)
    {
        Debug.LogError("DoAction");
        //StateTool.ChangeState(toState, _skill);
    }

    public void SetAbilityCallBack(Action<int> enterCallBack, Action<int> endCallBack)
    {
        _abilityEnterCallBack = enterCallBack;
        _abilityEndCallBack = endCallBack;
    }

    public void SetSkill(Skill skill)
    {
        _skill = skill;
        _abilityHSM = _skill.AbilityHSM;
        _abilityHSM.SetSkillEndCallBack(SkillEnterCallBack, SkillExitCallBack);
        Debug.LogError("SetSkill:" + skill.weaponId);
    }

    public void SkillEnterCallBack(int weaponId)
    {
        if (_skill.weaponId == weaponId && (null != _abilityEndCallBack))
        {
            _abilityEnterCallBack(weaponId);
        }
    }

    private void SkillExitCallBack(int weaponId)
    {
        if (_skill.weaponId == weaponId && (null != _abilityEndCallBack))
        {
            _abilityEndCallBack(weaponId);
        }

        _end = true;
    }

    public void Clear()
    {
        _end = false;
    }

}


#endif
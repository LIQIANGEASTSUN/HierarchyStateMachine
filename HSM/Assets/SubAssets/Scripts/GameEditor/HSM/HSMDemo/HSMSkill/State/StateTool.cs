using HSMTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if A

public class StateTool
{
    public readonly static string SkillRequestState = "Skill_Change_State";
    public readonly static string EnableFire = "EnableFire";
    public readonly static string EnergyEnougth = "EnergyEnougth";
    public readonly static string EnableActive = "EnableActive";
    public readonly static string Phase_End = "PhaseEnd";
    public readonly static string Hold = "Hold";
    public readonly static string FocoFull = "FocoFull";
    public readonly static string ShotTimeEnd = "ShotTimeEnd";
    public readonly static string RemainingFireTimes = "RemainingFireTimes";
    public readonly static string SkillTimeEnd = "SkillTimeEnd";
    public readonly static string Skill_State = "Skill_State";
    public readonly static string AbilityHSM = "AbilityHSM";
    public readonly static string JumpBtn = "JumpBtn";
    public readonly static string SkillNormalEnd = "SkillNormalEnd";
    public readonly static string SkillJumpEnd = "SkillJumpEnd";
    public readonly static string SkillSpecialEnd = "SkillSpecialEnd";


    public static Dictionary<int, string> _btnNameDic = new Dictionary<int, string>() {
        {(int)AbilityButtonType.GENERAL,      "GenericBtn" }, // 普通技能按钮
        {(int)AbilityButtonType.SKILL_DEPUTY, "DeputyBtn" },  // 副技能按钮
        {(int)AbilityButtonType.SKILL_UNIQUE, "UniqueBtn" },  // 大招按钮
        {(int)AbilityButtonType.FIRE,         "FireBtn" },    // 释放按钮
    };

    public static Dictionary<int, string> _skillConfigDic = new Dictionary<int, string>() {
        { (int)SkillHandleType.CONTINUOUS_FIRE,        "AbilityGenericHSM"},     // 普攻-连射     //已测试
        { (int)SkillHandleType.ROLL_BRUSH,             "AbilityRoleBrushHSM"},   // 普攻-滚动
        { (int)SkillHandleType.FOCO_SINGLE_FIRE,       "AbilityFocoSingleHSM"},  // 普攻-蓄力单发 //已测试
        { (int)SkillHandleType.FOCO_CONTINUOUS_FIRE,   "AbilityFocoContinueHSM"},// 普攻-蓄力连射 //已测试 
        { (int)SkillHandleType.SWING_BRUSH,            "AbilitySwingBruseHSM"},  // 刷子-甩
        { (int)SkillHandleType.SINGLE_SHOT,            "AbilitySingleShotHSM"},  // 普攻-单发
        { (int)SkillHandleType.DEPUTY_SKILL_THROW,     "AbilityThrowHSM"},       // 副技能-投掷   //已测试
        { (int)SkillHandleType.DEPUTY_SKILL_FOCO,      "AbilityFocoFullHSM"},    // 副技能-蓄力   //已测试
        { (int)SkillHandleType.UNIQUE_INSTANT_SKILL,   "AbilityUniqueHSM"},      // 大招-瞬发     //已测试
        { (int)SkillHandleType.UNIQUE_FIRE_CONTINUOUS, "AbilityUniqueRayHSM"},   // 大招-激光     //已测试
        { (int)SkillHandleType.UNIQUE_FIRE_SKILL,      "AbilityUniqueFireHSM" }, // 大招-射击     //已测试
        { (int)SkillHandleType.UNIQUE_DEPUTY_SKILL,    "" },                     // 大招-副技能
        { (int)SkillHandleType.TRANSFER,               "AbilityTransferHSM" },   // 传送技能      //已测试
        { (int)SkillHandleType.NONE,                   ""}
    };

    public static void ChangeState(HSMState toState, Skill skill)
    {
        if (typeof(SkillPhaseState).IsAssignableFrom(toState.GetType()))
        {
            SkillConfigSkillPhaseType type = ((SkillPhaseState)toState).PhaseType;
            SkillStateBase skillStateBase = skill.skillStateMachine.GetState(type);
            if (null != skillStateBase)
            {
                skillStateBase.WillToThisState();
            }
        }
    }

}


#endif
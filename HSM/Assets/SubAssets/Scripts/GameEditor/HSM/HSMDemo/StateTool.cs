using System.Collections.Generic;
using System;

public class StateTool
{
    public readonly static string SkillConfig              = "SkillConfig";            // 第N个配置
    public readonly static string Skill_State              = "Skill_State";            // 技能状态
    public readonly static string CDValid                  = "CDValid";                // CD有效
    public readonly static string SkillEnergyValid         = "SkillEnergyValid";       // 技能能量足够
    public readonly static string TrajectoryEnergyValid    = "TrajectoryEnergyValid";  // 弹道能量足够
    public readonly static string FireEnergyValid          = "FireEnergyValid";        // 释放阶段能量足够
    public readonly static string PhaseFinish              = "PhaseFinish";            // 阶段结束
    public readonly static string EnableAbortPhase         = "EnableAbortPhase";       // 阶段可以中断
    public readonly static string EnableAbortSkill         = "EnableAbortSkill";       // 技能可以中断
    public readonly static string EnableAbortSkillToFish   = "EnableAbortSkillToFish"; // 可以强制中断技能变鱼
    public readonly static string FocoOverflow             = "FocoOverflow";           // 蓄力蓄满
    public readonly static string ShotTimeFinish           = "ShotTimeFinish";         // 射击时间结束
    public readonly static string SkillTimeFinish          = "SkillTimeFinish";        // 技能时间结束
    public readonly static string ShotCount                = "ShotCount";              // 射击次数>0
    public readonly static string ShotInterval             = "ShotInterval";           // 射击间隔
    public readonly static string ShotTarget               = "ShotTarget";             // 射击目标
    public readonly static string ForcedAbortSkill         = "ForcedAbortSkill";       // 强制中断技能
    public readonly static string ForcedAbortSkillToFish   = "ForcedAbortSkillToFish"; // 触发中断技能变鱼
    public readonly static string NextAct                  = "NextAct";                // 到下一个配置
    public readonly static string TransferStart            = "TransferStart";          // 开始传送
    public readonly static string TransferEnd              = "TransferEnd";            // 开始传送
    public readonly static string SkillExit                = "SkillExit";              // 技能退出
    public readonly static string GenericHold              = "GenericHold";            // 普攻挂起
    public readonly static string DeputyHold               = "DeputyHold";             // 副技能挂起
    public readonly static string UniqueHold               = "UniqueHold";             // 大招挂起
    public readonly static string FishHold                 = "FishHold";               // 鱼挂起
    public readonly static string Moving                   = "Moving";                 // 移动中

#if B

    public static Dictionary<AbilityButtonType, string[]> _btnDic = new Dictionary<AbilityButtonType, string[]>() {
        { AbilityButtonType.GENERAL,      new string[]{ "GenericDown",  "GenericUp" } },
        { AbilityButtonType.SKILL_DEPUTY, new string[]{ "DeputyDown",   "DeputyUp" }},
        { AbilityButtonType.SKILL_UNIQUE, new string[]{ "UniqueDown",   "UniqueUp"} },
        { AbilityButtonType.FIRE,         new string[]{ "FireDown",     "FireUp"} },
        { AbilityButtonType.TRANSFER,     new string[]{ "TransferDown", "TransferUp"} },
    };

    public static void Input(AbilityButtonType buttonType, AbilityHandleType handleType, Action<string, bool> callBack)
    {
        if (handleType == AbilityHandleType.PRESS)  // 只处理 Down、Up
        {
            return;
        }

        string[] nameArr = null; ;
        if (!_btnDic.TryGetValue(buttonType, out nameArr))
        {
            return;
        }

        int index = (handleType == AbilityHandleType.DOWN) ? 0 : 1;
        for (int i = 0; i < nameArr.Length; ++i)
        {
            if (null != callBack)
            {
                callBack(nameArr[i], (i == index));
            }
        }
    }

    private static string[] fishArr = new string[] { "FishDown", "FishUp"};
    public static void InputFish(AbilityHandleType handleType, Action<string, bool> callBack)
    {
        if (handleType == AbilityHandleType.PRESS)  // 只处理 Down、Up
        {
            return;
        }

        int index = (handleType == AbilityHandleType.DOWN) ? 0 : 1;
        for (int i = 0; i < fishArr.Length; ++i)
        {
            if (null != callBack)
            {
                callBack(fishArr[i], (i == index));
            }
        }
    }
#endif

}

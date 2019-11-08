using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SkillUIType
{
    /// <summary>
    /// 圆形
    /// </summary>
    [EnumAttirbute("圆形")]
    CIRCLE    = 0,

    /// <summary>
    /// 箭头
    /// </summary>
    [EnumAttirbute("箭头")]
    ARROWS    = 1,

    /// <summary>
    /// 无
    /// </summary>
    [EnumAttirbute("无")]
    NONE      = 2,
}

public enum SkillHandleType
{
    /// <summary>
    /// 普攻-连射：按下【射击】按钮进入释放状态（释放状态本身为循环），每次循环释放消耗一次能量，
    /// 当能量不足时，结束释放阶段跳转到技能的循环阶段。松开【射击】按钮时离开技能状态。在过程中
    /// 英雄可以移动和跳跃。
    /// </summary>
    [EnumAttirbute("普攻-连射")]
    CONTINUOUS_FIRE       = 0,

    /// <summary>
    /// 普攻-滚动：按【射击】时持续按住则进行滚动，松开按钮离开技能状态。在过程中英雄可以移动和跳跃。
    /// </summary>
    [EnumAttirbute("普攻-滚动")]
    ROLL_BRUSH = 1,

    /// <summary>
    /// 普攻-蓄力单发：按住【射击】按钮就会进行蓄力，松开时发射一次。在过程中英雄可以移动和跳跃。
    /// </summary>
    [EnumAttirbute("普攻-蓄力单发")]
    FOCO_SINGLE_FIRE      = 2,

    /// <summary>
    /// 普攻-蓄力连射：按住【射击】按钮就会进行蓄力，松开时自动进行连续发射。在过程中英雄可以移动和跳跃。
    /// </summary>
    [EnumAttirbute("普攻-蓄力连射")]
    FOCO_CONTINUOUS_FIRE  = 3,

    /// <summary>
    /// 刷子-甩：按住【射击】按钮射击，长按触发第二个技能.(如：刷子，按下射击按钮甩刷子攻击，长按射击按钮推地攻击)
    /// </summary>
    [EnumAttirbute("刷子-甩")]
    SWING_BRUSH = 4,

    /// <summary>
    /// 普攻-单发：按下或按住【射击】进入释放状态，释放频率跟点击按钮频率相关，与按键时长无关。在过程中英雄可以移动和跳跃。
    /// </summary>
    [EnumAttirbute("普攻-单发")]
    SINGLE_SHOT = 5,

    /// <summary>
    /// 副技能-投掷：按住【技能】按钮时进入循环阶段，松开时释放。在过程中英雄可以移动和跳跃。
    /// </summary>
    [EnumAttirbute("副技能-投掷")]
    DEPUTY_SKILL_THROW    = 100,

    /// <summary>
    /// 副技能-蓄力：按住【技能】按钮进行蓄力，松开或者达到最大蓄力时间时释放（冰壶类）：在过程中英雄可以移动和跳跃。
    /// </summary>
    [EnumAttirbute("副技能-蓄力")]
    DEPUTY_SKILL_FOCO     = 101,

    /// <summary>
    /// 大招-瞬发：点击【大招】按钮直接释放。除角色死亡外不可以被打断。在释放过程中英雄所有移动、技能、跳跃、变身无效（复位有效）。
    /// </summary>
    [EnumAttirbute("大招-瞬发")]
    UNIQUE_INSTANT_SKILL  = 200,

    /// <summary>
    /// 大招-激光：点击【大招】按钮激活大招状态，在该状态下按住【大招发射】按钮持续发射，状态时间结束后退出。可以移动、跳跃。（替换射击按钮图标）
    /// </summary>
    [EnumAttirbute("大招-激光")]
    UNIQUE_FIRE_CONTINUOUS = 201,

    /// <summary>
    /// 大招-射击：点击【大招】按钮激活大招状态，在该状态下每次点击【大招发射】按钮发射，状态时间结束时或大招次数用尽时退出。可以移动、跳跃。
    /// </summary>
    [EnumAttirbute("大招-射击")]
    UNIQUE_FIRE_SKILL      = 202,

    /// <summary>
    /// 大招-副技能：点击【大招】按钮激活大招状态，按住【大招发射】按钮时进入循环阶段，松开时释放。状态时间结束时或大招次数用尽时退出。可以移动、跳跃。
    /// </summary>
    [EnumAttirbute("大招-副技能")]
    UNIQUE_DEPUTY_SKILL    = 203,

    /// <summary>
    /// 传送技能：
    /// </summary>
    [EnumAttirbute("传送技能")]
    TRANSFER = 301,

    /// <summary>
    /// 无
    /// </summary>
    [EnumAttirbute("无")]
    NONE = 10000,

}

public enum SkillConfigSkillPhaseType
{
    /// <summary>
    /// 起手阶段
    /// </summary>
    [EnumAttirbute("起手阶段")]
    INITIAL_PHASE    = 0,

    /// <summary>
    /// 待机(循环)阶段
    /// </summary>
    [EnumAttirbute("待机(循环)阶段")]
    STANDBY_PHASE    = 1,

    /// <summary>
    /// 释放阶段
    /// </summary>
    [EnumAttirbute("释放阶段")]
    FIRE_PHASE       = 2,

    /// <summary>
    /// 结束收尾阶段
    /// </summary>
    [EnumAttirbute("结束收尾阶段")]
    END_PHASE       = 3,

    [EnumAttirbute("无效值")]
    NONE              = 4,
}

public enum SkillEventType
{
    /// <summary>
    /// 特效事件
    /// </summary>
    [EnumAttirbute("特效")]
    EFFECT = 0,

    /// <summary>
    /// 动作事件
    /// </summary>
    [EnumAttirbute("动作")]
    ANIMATION = 1,

    /// <summary>
    /// 音效事件
    /// </summary>
    [EnumAttirbute("音效")]
    AUDIO = 2,

    /// <summary>
    /// 更换武器事件
    /// </summary>
    [EnumAttirbute("更换武器")]
    CHANGE_WEAPON = 3,

    /// <summary>
    /// 摄像机事件
    /// </summary>
    [EnumAttirbute("摄像机事件")]
    CAMERA = 4,

    /// <summary>
    /// 弹道事件
    /// </summary>
    [EnumAttirbute("弹道配置文件")]
    TRAJECTORY = 5,

    /// <summary>
    /// Hold 事件
    /// </summary>
    [EnumAttirbute("Hold 事件")]
    HOLD = 6,

    /// <summary>
    /// 循环事件
    /// </summary>
    [EnumAttirbute("循环事件")]
    LOOP = 7,

    /// <summary>
    /// 阶段结束
    /// </summary>
    [EnumAttirbute("阶段结束")]
    PHASE_END = 8,
}

/// <summary>
/// 摄像机控制类型
/// </summary>
public enum SkillCamaeraChange
{
    /// <summary>
    /// 重置
    /// </summary>
    [EnumAttirbute("重置")]
    RESET = 0,

    /// <summary>
    /// 改变
    /// </summary>
    [EnumAttirbute("改变")]
    CHANGE = 1,
}

/// <summary>
/// 技能 Hold 类型
/// </summary>
public enum SkillHoldType
{
    /// <summary>
    /// 切掉当前阶段
    /// </summary>
    [EnumAttirbute("打断阶段")]
    Abort_PHASE = 0,

    /// <summary>
    /// 切掉当前技能
    /// </summary>
    [EnumAttirbute("打断技能")]
    Abort_SKILL = 10,
}

/// <summary>
/// 模型身上挂点
/// </summary>
public enum ModelMountPoint
{
    /// <summary>
    /// 脚底中心
    /// </summary>
    [EnumAttirbute("脚底中心")]
    FOOT_CENTER = 0,

    /// <summary>
    /// 头部
    /// </summary>
    [EnumAttirbute("头部")]
    HEAD = 1,

    /// <summary>
    /// 左手
    /// </summary>
    [EnumAttirbute("左手")]
    HAND_LEFT = 10,

    /// <summary>
    /// 右手
    /// </summary>
    [EnumAttirbute("右手")]
    HAND_RIGHT = 11,

    /// <summary>
    /// 胸部
    /// </summary>
    [EnumAttirbute("胸部")]
    CHEST = 20,

    /// <summary>
    /// 特殊部位1
    /// </summary>
    [EnumAttirbute("特殊部位1")]
    SPECIAL_0 = 101,

    /// <summary>
    /// 特殊部位1
    /// </summary>
    [EnumAttirbute("特殊部位2")]
    SPECIAL_1 =102,

    /// <summary>
    /// 特殊部位1
    /// </summary>
    [EnumAttirbute("特殊部位3")]
    SPECIAL_2 = 103,

    /// <summary>
    /// 特殊部位1
    /// </summary>
    [EnumAttirbute("特殊部位4")]
    SPECIAL_3 = 104,

    /// <summary>
    /// 特殊部位1
    /// </summary>
    [EnumAttirbute("特殊部位5")]
    SPECIAL_4 = 105,

    /// <summary>
    /// 左手武器挂点
    /// </summary>
    [EnumAttirbute("左手武器挂点")]
    HAND_LEFT_WEAPON_TWO = 201,

    /// <summary>
    /// 右手武器挂点
    /// </summary>
    [EnumAttirbute("右手武器挂点")]
    HAND_RIGHT_WEAPON_ONE = 210,

    /// <summary>
    /// 右手武器挂点
    /// </summary>
    [EnumAttirbute("大招武器挂点")]
    UNIQUE_WEAPON_ONE = 251,

    /// <summary>
    /// 副技能绝对点
    /// </summary>
    [EnumAttirbute("副技能绝对点")]
    DEPUTY_FIXED_POINT = 261,

    /// <summary>
    /// 大招绝对点
    /// </summary>
    [EnumAttirbute("大招绝对点")]
    UNIQUE_FIXED_POINT = 270,

    /// <summary>
    /// 射击旋转点 0
    /// </summary>
    [EnumAttirbute("射击旋转点 0")]
    SHOT_ROTATION_ONE = 401,

    /// <summary>
    /// 副技能旋转点 1
    /// </summary>
    [EnumAttirbute("副技能旋转点 1")]
    DEPUTY_ROTATION_ONE = 410,

    /// <summary>
    /// 大招旋转点 1
    /// </summary>
    [EnumAttirbute("大招旋转点 1")]
    UNIQUE_ROTATION_ONE = 420,

    /// <summary>
    /// 传送点
    /// </summary>
    [EnumAttirbute("传送点")]
    TRANSFER = 1000,
}

/// <summary>
/// 武器上的挂点
/// </summary>
public enum WeaponMountPoint
{
    /// <summary>
    /// 无
    /// </summary>
    [EnumAttirbute("无")]
    NONE = 0,

    /// <summary>
    /// 武器枪口挂点
    /// </summary>
    [EnumAttirbute("武器枪口挂点")]
    WEAPON_SHOT = 1,

    /// <summary>
    /// 武器上的挂点
    /// </summary>
    [EnumAttirbute("武器上的挂点")]
    WEAPON_HANDLE = 5,

    /// <summary>
    /// 武器上特效挂点
    /// </summary>
    [EnumAttirbute("武器上特效挂点")]
    WEAPON_EFFECT = 10,

    /// <summary>
    /// 固定枪口 1
    /// </summary>
    [EnumAttirbute("(左)固定枪口 1")]
    FIXED_SHOT1  = 50,

    /// <summary>
    /// 固定枪口 2
    /// </summary>
    [EnumAttirbute("(右)固定枪口 1")]
    FIXED_SHOT2 = 60,

    /// <summary>
    /// (左)固定枪口特效 1
    /// </summary>
    [EnumAttirbute("(左)固定枪口特效 1")]
    FIXED_SHOT_EFFECT1 = 70,

    /// <summary>
    /// (右)固定枪口特效 1
    /// </summary>
    [EnumAttirbute("(右)固定枪口特效 1")]
    FIXED_SHOT_EFFECT2 = 80,

    /// <summary>
    /// 固定枪口副技能出弹点
    /// </summary>
    [EnumAttirbute("固定枪口副技能出弹点")]
    FIXED_DEPUTY_SHOT_POINT = 90,

    /// <summary>
    /// 固定枪口大招出弹点
    /// </summary>
    [EnumAttirbute("固定枪口大招出弹点")]
    FIXED_UNIQUE_SHOT_POINT = 100,
}

public enum SkillRemoveWeaponType
{
    /// <summary>
    /// 技能结束删除
    /// </summary>
    [EnumAttirbute("技能结束删除")]
    SKILL_END = 0,

    /// <summary>
    /// 时间轴点删除
    /// </summary>
    [EnumAttirbute("时间轴点删除")]
    TIME_POINT = 1,
}

/// <summary>
/// 特效生成类型
/// </summary>
public enum SkillEffectCreate
{
    /// <summary>
    /// 按时间
    /// </summary>
    [EnumAttirbute("按时间生成")]
    TIME = 0,

    /// <summary>
    /// 技能蓄力蓄满
    /// </summary>
    [EnumAttirbute("技能蓄力蓄满")]
    SKILL_FOCO_FULL = 1,
}


/// <summary>
/// 特效刪除类型
/// </summary>
public enum SkillEffectRemoveType
{
    /// <summary>
    /// 特效时间结束删除
    /// </summary>
    [EnumAttirbute("特效时间结束删除")]
    TIME_END = 1 << 0,

    /// <summary>
    /// 技能切换阶段删除
    /// </summary>
    [EnumAttirbute("技能切换阶段删除")]
    SKILL_PHASE_CHANGE = 1 << 1,

    /// <summary>
    /// 特定阶段删除
    /// </summary>
    [EnumAttirbute("特定阶段删除")]
    SKILL_PHASE_TIME = 1 << 2,

    /// <summary>
    /// 技能蓄力蓄满
    /// </summary>
    [EnumAttirbute("技能蓄力蓄满")]
    SKILL_FOCO_FULL = 1 << 3,
}

/// <summary>
/// 技能重置类型
/// </summary>
public enum SkillResetType
{
    /// <summary>
    /// 不清除(无效值)
    /// </summary>
    [EnumAttirbute("不清除(无效值)")]
    INVALID = -1,

    /// <summary>
    /// 清除状态
    /// </summary>
    [EnumAttirbute("清除状态")]
    STATE_CLEAR = 2,

    /// <summary>
    /// 清除 CD
    /// </summary>
    [EnumAttirbute("清除 CD")]
    CD_CLEAR = 4,
}
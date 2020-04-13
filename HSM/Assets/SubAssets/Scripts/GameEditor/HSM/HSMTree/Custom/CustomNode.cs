using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HSMTree
{

    public enum IDENTIFICATION
    {
        #region
        /// <summary>
        /// 技能状态节点
        /// </summary>
        [EnumAttirbute("技能状态")]
        SKILL_PHASE_STATE = 10000,

        /// <summary>
        /// 技能挂起
        /// </summary>
        [EnumAttirbute("技能挂起")]
        SKILL_HOLD = 10005,

        /// <summary>
        /// 鱼挂起
        /// </summary>
        [EnumAttirbute("鱼挂起")]
        FISH_HOLD = 10006,

        /// <summary>
        /// 通知
        /// </summary>
        [EnumAttirbute("通知")]
        SKILL_NOTIFY = 10010,

        /// <summary>
        /// 技能下一阶段
        /// </summary>
        [EnumAttirbute("技能下一阶段")]
        SKILL_NEXT_STAGE = 10020,

        /// <summary>
        /// 技能结束
        /// </summary>
        [EnumAttirbute("技能结束")]
        SKILL_EXIT_STATE = 10030,

        /// <summary>
        /// Sub-MachineSkill
        /// </summary>
        [EnumAttirbute("Sub-MachineSkill")]
        SKILL_SUB_MACHINE = 20000,

        /// <summary>
        /// STATE_ENTRY
        /// </summary>
        [EnumAttirbute("STATE_ENTRY")]
        STATE_ENTRY = 30000,

        /// <summary>
        /// STATE_EXIT
        /// </summary>
        [EnumAttirbute("STATE_EXIT")]
        STATE_EXIT = 31000,
        #endregion
    }

    public struct CustomIdentification
    {
        private string name;
        private IDENTIFICATION identification;
        private Type type;
        private NODE_TYPE nodeType;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="identification"></param>
        public CustomIdentification(string name, IDENTIFICATION identification, Type t, NODE_TYPE nodeType)
        {
            this.name = name;
            this.identification = identification;
            this.type = t;
            this.nodeType = nodeType;
        }

        public string Name
        {
            get { return name; }
        }
    
        public IDENTIFICATION Identification
        {
            get { return identification; }
        }

        public Type Type
        {
            get { return type; }
        }

        public NODE_TYPE NodeType
        {
            get { return nodeType; }
        }

        public bool Valid()
        {
            return identification > 0;
        }

        public object Create()
        {
            object o = Activator.CreateInstance(type);
            return o;
        }
    }

    /// <summary>
    /// 自定义节点
    /// 只有 条件节点、行为节点 需要自定义
    /// </summary>
    public class CustomNode
    {
        public readonly static CustomNode Instance = new CustomNode();

        private List<CustomIdentification> nodeList = new List<CustomIdentification>();

        public CustomNode()
        {
        }

        public object GetState(IDENTIFICATION identification)
        {
            object obj = null;
            CustomIdentification info = GetIdentification(identification);
            if (info.Valid())
            {
                obj = info.Create();
            }
            return obj;
        }

        public CustomIdentification GetIdentification(IDENTIFICATION identification)
        {
            GetNodeList();

            for (int i = 0; i < nodeList.Count; ++i)
            {
                CustomIdentification info = nodeList[i];
                if (info.Identification == identification)
                {
                    return info;
                }
            }

            return new CustomIdentification();
        }

        public List<CustomIdentification> GetNodeList()
        {
            if (nodeList.Count > 0)
            {
                return nodeList;
            }

            #region Skill
            CustomIdentification skillPhaseState = SkillPhaseState._customIdentification;
            nodeList.Add(skillPhaseState);

            CustomIdentification skillHold = SkillHoldSkillState._customIdentification;
            nodeList.Add(skillHold);

            CustomIdentification fishHold = SkillHoldFishState._customIdentification;
            nodeList.Add(fishHold);

            CustomIdentification skillNotify = SkillNotifyState._customIdentification;
            nodeList.Add(skillNotify);

            CustomIdentification nextStage = SkillNextStage._customIdentification;
            nodeList.Add(nextStage);

            CustomIdentification exitSkill = SkillExitState._customIdentification;
            nodeList.Add(exitSkill);
            #endregion

            #region Sub-Machine
            CustomIdentification subMachineSkill = SubMachineSkill._customIdentification;
            nodeList.Add(subMachineSkill);
            #endregion

            #region Entry
            CustomIdentification entry = HSMStateEntry._customIdentification;
            nodeList.Add(entry);
            #endregion

            #region Exit
            CustomIdentification exit = HSMStateExit._customIdentification;
            nodeList.Add(exit);
            #endregion

            HashSet<IDENTIFICATION> hash = new HashSet<IDENTIFICATION>();
            for (int i = 0; i < nodeList.Count; ++i)
            {
                CustomIdentification identificationi = nodeList[i];
                if (hash.Contains(identificationi.Identification))
                {
                    Debug.LogError("重复的 Identification:" + identificationi.Identification);
                    break;
                }
                hash.Add(identificationi.Identification);
            }

            return nodeList;
        }

    }

}




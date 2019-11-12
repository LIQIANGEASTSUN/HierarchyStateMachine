﻿using System.Collections;
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
        [EnumAttirbute("技能状态节点")]
        SKILL_STATE = 10000,

        /// <summary>
        /// 技能刷子甩
        /// </summary>
        [EnumAttirbute("技能刷子甩")]
        SKILL_SWING_BRUSH = 10001,

        #endregion
    }

    public struct CustomIdentification
    {
        private string name;
        private IDENTIFICATION identification;
        private Type type;
        private NODE_TYPE nodeType;
        
        /// <summary>
        /// 条件节点从 20000 开始， 行为节点从 10000 开始
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
            CustomIdentification stateSkill = StateSkill.CustomIdentification();
            nodeList.Add(stateSkill);

            CustomIdentification skillBrush = SkillBrush.CustomIdentification();
            nodeList.Add(skillBrush);

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




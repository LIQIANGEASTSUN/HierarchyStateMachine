using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GenPB;

namespace HSMTree
{
    public enum HSMCompare
    {
        INVALID = 0,
        /// <summary>
        /// 大于
        /// </summary>
        [EnumAttirbute("大于")]
        GREATER = 1 << 0,

        /// <summary>
        /// 小于
        /// </summary>
        [EnumAttirbute("小于")]
        LESS = 1 << 1,

        /// <summary>
        /// 等于
        /// </summary>
        [EnumAttirbute("等于")]
        EQUALS = 1 << 2,

        /// <summary>
        /// 不等于
        /// </summary>
        [EnumAttirbute("不等于")]
        NOT_EQUAL = 1 << 3,

        /// <summary>
        /// 大于等于
        /// </summary>
        [EnumAttirbute("大于等于")]
        GREATER_EQUALS = 1 << 4,

        /// <summary>
        /// 小于等于
        /// </summary>
        [EnumAttirbute("小于等于")]
        LESS_EQUAL = 1 << 5,
    }

    public enum HSMParameterType
    {
        /// <summary>
        /// Float
        /// </summary>
        [EnumAttirbute("Float")]
        Float = 0,

        /// <summary>
        /// Int
        /// </summary>
        [EnumAttirbute("Int")]
        Int = 2,

        /// <summary>
        /// Bool
        /// </summary>
        [EnumAttirbute("Bool")]
        Bool = 5,
    }

    public static class HSMParameterExtension
    {
        public static SkillHsmConfigHSMParameter Clone(this SkillHsmConfigHSMParameter self)
        {
            SkillHsmConfigHSMParameter newParameter = new SkillHsmConfigHSMParameter();
            newParameter.CloneFrom(self);
            return newParameter;
        }

        public static void CloneFrom(this SkillHsmConfigHSMParameter self, SkillHsmConfigHSMParameter parameter)
        {
            self.ParameterType = parameter.ParameterType;
            self.CNName = parameter.CNName;
            self.ParameterName = parameter.ParameterName;
            self.IntValue = parameter.IntValue;
            self.FloatValue = parameter.FloatValue;
            self.BoolValue = parameter.BoolValue;
            self.Compare = parameter.Compare;
        }

        public static HSMCompare Compare(this SkillHsmConfigHSMParameter self, SkillHsmConfigHSMParameter parameter)
        {
            HSMCompare hSMCompare = HSMCompare.NOT_EQUAL;
            if (self.ParameterType != parameter.ParameterType)
            {
                Debug.LogError("parameter Type not Equal:" + parameter.ParameterName + "    " + parameter.ParameterType + "    " + self.ParameterType);
                return hSMCompare;
            }

            if (self.ParameterType == (int)HSMParameterType.Float)
            {
                hSMCompare = self.CompareFloat(parameter);
                return hSMCompare; //  (compare & (int)HSMCompare) > 0;
            }
            else if (self.ParameterType == (int)HSMParameterType.Int)
            {
                hSMCompare = self.CompareInt(parameter);
            }
            else
            {
                hSMCompare = self.CompareBool(parameter);
            }

            return hSMCompare;
        }

        public static HSMCompare CompareFloat(this SkillHsmConfigHSMParameter self, SkillHsmConfigHSMParameter parameter)
        {
            HSMCompare hSMCompare = HSMCompare.INVALID;
            if (self.FloatValue > parameter.FloatValue)
            {
                hSMCompare |= HSMCompare.GREATER;
            }

            if (self.FloatValue < parameter.FloatValue)
            {
                hSMCompare |= HSMCompare.LESS;
            }

            return hSMCompare;
        }

        public static HSMCompare CompareInt(this SkillHsmConfigHSMParameter self, SkillHsmConfigHSMParameter parameter)
        {
            HSMCompare hSMCompare = HSMCompare.INVALID;
            hSMCompare = self.CompareFloat(parameter);

            if (self.IntValue > parameter.IntValue)
            {
                hSMCompare |= HSMCompare.GREATER;
            }

            if (self.IntValue < parameter.IntValue)
            {
                hSMCompare |= HSMCompare.LESS;
            }

            if (self.IntValue == parameter.IntValue)
            {
                hSMCompare |= HSMCompare.EQUALS;
            }

            if (self.IntValue != parameter.IntValue)
            {
                hSMCompare |= HSMCompare.NOT_EQUAL;
            }

            if (self.IntValue >= parameter.IntValue)
            {
                hSMCompare |= HSMCompare.GREATER_EQUALS;
            }

            if (self.IntValue <= parameter.IntValue)
            {
                hSMCompare |= HSMCompare.LESS_EQUAL;
            }

            return hSMCompare;
        }

        public static HSMCompare CompareBool(this SkillHsmConfigHSMParameter self, SkillHsmConfigHSMParameter parameter)
        {
            HSMCompare hSMCompare = (self.BoolValue == parameter.BoolValue) ? HSMCompare.EQUALS : HSMCompare.NOT_EQUAL;
            return hSMCompare;
        }
    }

    public static class SkillHsmConfigTransitionExtension
    {
        public static SkillHsmConfigTransition Clone(this SkillHsmConfigTransition self)
        {
            SkillHsmConfigTransition newTransition = new SkillHsmConfigTransition();
            newTransition.CloneFrom(self);
            return newTransition;
        }

        public static void CloneFrom(this SkillHsmConfigTransition self, SkillHsmConfigTransition transition)
        {
            self.TransitionId = transition.TransitionId;
            self.ToStateId = transition.ToStateId;
            
            for (int i = 0; i < transition.ParameterList.Count; ++i)
            {
                SkillHsmConfigHSMParameter parameter = transition.ParameterList[i];
                self.ParameterList.Add(parameter.Clone());
            }

            for (int i = 0; i <transition.GroupList.Count; ++i)
            {
                SkillHsmConfigTransitionGroup group = transition.GroupList[i];

                SkillHsmConfigTransitionGroup clone = new SkillHsmConfigTransitionGroup();
                clone.Index = group.Index;
                for (int j = 0; j < group.ParameterList.Count; ++j)
                {
                    clone.ParameterList.Add(group.ParameterList[j]);
                }

                self.GroupList.Add(clone);
            }
        }
    }


    public class RectTExtension
    {
        public static Rect RectTToRect(SkillHsmConfigRectT rectT)
        {
            return new Rect(rectT.X, rectT.Y, rectT.Width, rectT.Height);
        }

        public static SkillHsmConfigRectT RectToRectT(Rect rect)
        {
            SkillHsmConfigRectT rectT = new SkillHsmConfigRectT();
            rectT.X = rect.x;
            rectT.Y = rect.y;
            rectT.Width = rect.width;
            rectT.Height = rect.height;

            return rectT;
        }
    }


}
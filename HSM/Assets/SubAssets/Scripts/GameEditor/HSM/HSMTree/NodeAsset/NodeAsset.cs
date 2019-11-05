using UnityEngine;
using HSMTree;
using System;
using System.Collections.Generic;

namespace HSMTree
{
    public class HSMTreeData
    {
        public int rootNodeId = -1;
        public List<NodeValue> nodeList = new List<NodeValue>();
        public List<HSMParameter> parameterList = new List<HSMParameter>();
        public string descript = string.Empty;
    }

    public class NodeValue
    {
        public int id = 0;
        public int NodeType = (int)(NODE_TYPE.STATE);     // 节点类型 // NODE_TYPE NodeType = NODE_TYPE.SELECT;
        public List<int> childNodeList = new List<int>();  // 子节点集合
        public List<HSMParameter> parameterList = new List<HSMParameter>();
        public List<Transition> transitionList = new List<Transition>();
        public int repeatTimes = 0;
        public string nodeName = string.Empty;
        public int identification = -1;
        public string descript = string.Empty;

        public RectT position = new RectT(0, 0, 100, 100); // 节点位置（编辑器显示使用）
    }

    public class Transition
    {
        public int id;
        public int fromNodeId;
        public int toNodeId;
        public List<HSMParameter> parameterList = new List<HSMParameter>();
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

    public enum HSMCompare
    {
        INVALID = 0,
        /// <summary>
        /// 大于
        /// </summary>
        GREATER = 1 << 0,

        /// <summary>
        /// 小于
        /// </summary>
        LESS = 1 << 1,

        /// <summary>
        /// 等于
        /// </summary>
        EQUALS = 1 << 2,

        /// <summary>
        /// 不等于
        /// </summary>
        NOT_EQUAL = 1 << 3,

        /// <summary>
        /// 大于等于
        /// </summary>
        GREATER_EQUALS = 1 << 4,

        /// <summary>
        /// 小于等于
        /// </summary>
        LESS_EQUAL = 1 << 5,
    }

    [SerializeField]
    [Serializable]
    public class HSMParameter
    {
        public int parameterType = 0;
        public string parameterName = string.Empty;
        public int intValue = 0;
        public float floatValue = 0;
        public bool boolValue = true;
        public int compare;

        public HSMParameter Clone()
        {
            HSMParameter newParameter = new HSMParameter();
            newParameter.CloneFrom(this);
            return newParameter;
        }

        public void CloneFrom(HSMParameter parameter)
        {
            parameterType = parameter.parameterType;
            parameterName = parameter.parameterName;
            intValue =  parameter.intValue;
            floatValue = parameter.floatValue;
            boolValue = parameter.boolValue;
            compare = parameter.compare;
        }

        private HSMCompare Compare(int value)
        {
            HSMCompare HSMCompare = Compare(intValue, value);
            return HSMCompare;
        }

        private HSMCompare Compare(float value)
        {
            HSMCompare HSMCompare = Compare(floatValue, value);
            return HSMCompare;
        }

        private HSMCompare Compare(float a, float b)
        {
            HSMCompare HSMCompare = HSMCompare.INVALID;
            if (a > b)
            {
                HSMCompare |= HSMCompare.GREATER;
            }

            if (a >= b)
            {
                HSMCompare |= HSMCompare.GREATER_EQUALS;
            }

            if (a == b)
            {
                HSMCompare |= HSMCompare.EQUALS;
            }

            if (a <= b)
            {
                HSMCompare |= HSMCompare.LESS_EQUAL;
            }

            if (a < b)
            {
                HSMCompare |= HSMCompare.LESS;
            }

            return HSMCompare;
        }

        private HSMCompare Compare(bool value)
        {
            return (boolValue == value) ? HSMCompare.EQUALS : HSMCompare.NOT_EQUAL;
        }

        public bool Compare(HSMParameter parameter)
        {
            if (parameterType != parameter.parameterType)
            {
                Debug.LogError("parameter Type not Equal:" + parameter.parameterName + "    " + parameter.parameterType + "    " + parameterType);
                return false;
            }

            HSMCompare HSMCompare = HSMCompare.NOT_EQUAL;
            if (parameterType == (int)HSMParameterType.Float)
            {
                HSMCompare = (Compare(parameter.floatValue));
            }
            else if (parameterType == (int)HSMParameterType.Int)
            {
                HSMCompare = (Compare(parameter.intValue));
            }
            else
            {
                HSMCompare = (Compare(parameter.boolValue));
            }

            return (compare & (int)HSMCompare) > 0;
        }
    }
    
    public class RectT
    {
        public float x;
        public float y;
        public float width;
        public float height;

        public RectT()
        {

        }

        public RectT(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }

    public class RectTool
    {
        public static Rect RectTToRect(RectT rectT)
        {
            return new Rect(rectT.x, rectT.y, rectT.width, rectT.height);
        }

        public static RectT RectToRectT(Rect rect)
        {
            return new RectT(rect.x, rect.y, rect.width, rect.height);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HSMTree
{

    public struct CustomIdentification
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="identification"></param>
        public CustomIdentification(string name, int identification, Type t, NODE_TYPE nodeType)
        {
            Name = name;
            Identification = identification;
            ClassType = t;
            NodeType = nodeType;
        }

        public string Name
        {
            get;
            private set;
        }

        public int Identification
        {
            get;
            private set;
        }

        public Type ClassType
        {
            get;
            private set;
        }

        public NODE_TYPE NodeType
        {
            get;
            private set;
        }

        public bool Valid()
        {
            return Identification > 0;
        }

        public object Create()
        {
            object o = Activator.CreateInstance(ClassType);
            return o;
        }
    }
}


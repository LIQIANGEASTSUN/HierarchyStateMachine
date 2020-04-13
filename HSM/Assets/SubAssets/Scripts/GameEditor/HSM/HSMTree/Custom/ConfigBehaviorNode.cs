using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HSMTree
{
    public static class IDENTIFICATION
    {
        #region

        // 打篮球
        public const int PLAY_BASKETBALL = 1000;

        // 逛公园
        public const int STROOL_PARK = 1001;

        //休息
        public const int TAKE_RESET = 1002;

        /// <summary>
        /// 子状态机
        /// </summary>
        public const int SUB_MACHINE = 20000;

        /// <summary>
        /// 状态入口
        /// </summary>
        public const int STATE_ENTRY = 30000;

        /// <summary>
        /// 状态出口
        /// </summary>
        public const int STATE_EXIT = 31000;
        #endregion
    }

    public class ConfigBehaviorNode
    {
        private Action<CustomIdentification> ConfigEvent;

        public void Init()
        {

            #region Human
            Config<StatePlayBasketBall>("Human/打篮球", IDENTIFICATION.PLAY_BASKETBALL, NODE_TYPE.STATE);
            Config<StatePlayBasketBall>("Human/逛公园", IDENTIFICATION.STROOL_PARK, NODE_TYPE.STATE);
            Config<StatePlayBasketBall>("Human/休息", IDENTIFICATION.TAKE_RESET, NODE_TYPE.STATE);
            #endregion

            Config<HSMStateEntry>("Entry", IDENTIFICATION.STATE_ENTRY, NODE_TYPE.STATE);
            Config<HSMStateExit>("Exit", IDENTIFICATION.STATE_EXIT, NODE_TYPE.STATE);

            //Config<NodeConditionCustom>("通用条件节点", 20002);
        }

        private void Config<T>(string name, int identification, NODE_TYPE nodeType)
        {
            CustomIdentification customIdentification = new CustomIdentification(name, identification, typeof(T), nodeType);

            if (null != ConfigEvent)
            {
                ConfigEvent(customIdentification);
            }
        }

        public void AddEvent(Action<CustomIdentification> callBack)
        {
            ConfigEvent += callBack;
        }

        public void RemoveEvent(Action<CustomIdentification> callBack)
        {
            ConfigEvent -= callBack;
        }

    }
}

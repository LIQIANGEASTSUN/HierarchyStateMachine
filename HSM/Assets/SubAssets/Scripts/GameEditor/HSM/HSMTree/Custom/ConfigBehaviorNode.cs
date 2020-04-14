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

        // 移动到某处
        public const int MOVE_TO = 1003;

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
            Config<StateStroolPark>("Human/逛公园", IDENTIFICATION.STROOL_PARK, NODE_TYPE.STATE);
            Config<StateTakeReset>("Human/休息", IDENTIFICATION.TAKE_RESET, NODE_TYPE.STATE);
            Config<StateMoveTo>("Human/移动到某处", IDENTIFICATION.MOVE_TO, NODE_TYPE.STATE);
            #endregion

            Config<HSMSubStateMachine>("SubMachine", IDENTIFICATION.SUB_MACHINE, NODE_TYPE.SUB_STATE_MACHINE);
            Config<HSMStateEntry>("Entry", IDENTIFICATION.STATE_ENTRY, NODE_TYPE.STATE, true);
            Config<HSMStateExit>("Exit", IDENTIFICATION.STATE_EXIT, NODE_TYPE.STATE, true);
        }

        private void Config<T>(string name, int identification, NODE_TYPE nodeType, bool isAutoCreate = false)
        {
            CustomIdentification customIdentification = new CustomIdentification(name, identification, typeof(T), nodeType, isAutoCreate);

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HSMTree
{
    public class ConfigBehaviorNode
    {
        private Action<CustomIdentification> ConfigEvent;

        public void Init()
        {


            //#region Human
            //Config<NodeActionCooking>("行为-做饭", 11000);
            //Config<NodeActionEat>("行为-吃饭", 11001);
            //Config<NodeActionMove>("行为-移动到目标", 11002);
            //Config<NodeActionWatchTV>("行为-看电视", 11003);
            //#endregion


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

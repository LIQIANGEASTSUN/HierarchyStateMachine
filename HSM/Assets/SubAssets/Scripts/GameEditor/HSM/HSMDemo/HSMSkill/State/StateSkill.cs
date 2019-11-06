using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;

public class StateSkill : HSMState
{
    private static CustomIdentification _customIdentification = new CustomIdentification("技能状态节点", IDENTIFICATION.SKILL_STATE, typeof(StateSkill), NODE_TYPE.STATE);



    public static CustomIdentification CustomIdentification()
    {
        return _customIdentification;
    }

}

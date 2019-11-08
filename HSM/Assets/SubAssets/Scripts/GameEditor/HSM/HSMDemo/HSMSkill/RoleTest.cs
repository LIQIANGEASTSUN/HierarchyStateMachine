using HSMTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleTest : MonoBehaviour
{
    public static RoleTest Instance = null;
    private RolePlayer _role;
    public List<HSMParameter> parameterList = new List<HSMParameter>();
    void Start()
    {
        Instance = this;

        _role = new RolePlayer();
        _role.Init();

        UpdateCondition();
    }

    // Update is called once per frame
    void Update()
    {
        _role.Update();

        if (Input.GetKeyDown(KeyCode.A))
        {
            _role.GeneralMsg(0);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            _role.GeneralMsg(2);
        }
    }

    public void UpdateCondition()
    {
        parameterList = _role.ConditionCheck.GetAllParameter();
    }

}


public class RolePlayer : IAction
{
    private HSMStateMachine hsmStateMachine = null;
    private IConditionCheck _iconditionCheck = null;

    public void Init()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("AbilityGeneric");
        SetData(textAsset.text);
    }

    // Update is called once per frame
    public void Update()
    {
        if (null != hsmStateMachine)
        {
            hsmStateMachine.Execute();
        }
    }

    public void GeneralMsg(int type)
    {
        _iconditionCheck.SetParameter("GenericBtn", type);
    }

    public void SetData(HSMTreeData HSMTreeData)
    {
        HSMAnalysis analysis = new HSMAnalysis();
        _iconditionCheck = new ConditionCheck();
        hsmStateMachine = analysis.Analysis(HSMTreeData, _iconditionCheck, this);
    }

    public void SetData(string content)
    {
        HSMAnalysis analysis = new HSMAnalysis();
        _iconditionCheck = new ConditionCheck();
        hsmStateMachine = analysis.Analysis(content, _iconditionCheck, this);
    }
    
    public void DoAction(int toStateId)
    {

    }

    public bool DoAction(int nodeId, List<HSMParameter> parameterList)
    {
        bool result = true;
        for (int i = 0; i < parameterList.Count; ++i)
        {
            bool value = DoAction(nodeId, parameterList[i]);
            if (!value)
            {
                result = false;
            }
        }

        return result;
    }

    public bool DoAction(int nodeId, HSMParameter parameter)
    {
        Debug.LogError("Execute:" + parameter.parameterName + "   bool:" + parameter.boolValue + "    float:" + parameter.floatValue + "    int:" + parameter.intValue);

        if (parameter.parameterName.CompareTo("Skill_Change_State") == 0)
        {
            ConditionCheck.SetParameter("Skill_State", parameter.intValue);
        }

        RoleTest.Instance.UpdateCondition();
        return true;
    }

    public ConditionCheck ConditionCheck
    {
        get { return (ConditionCheck)_iconditionCheck; }
    }

}
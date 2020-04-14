using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;
using GenPB;

public interface IHuman
{
    void SetHuman(Human human);
}

public class HumanConstant
{
    public const string StroolPark = "StroolPark";
    public const string PlayBasketBall = "PlayBasketBall";
    public const string Reset = "Reset";
    public const string GoHome = "GoHome";
    public const string FullEnergy = "FullEnergy";
}

public class Human : MonoBehaviour
{
    // 公园
    private Transform park;
    // 篮球场
    private Transform basketballStadium;
    // 休息区
    private Transform resetArea;

    private HsmEntity hsmEntity;

    private float energy = 100;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HsmTreeData.Instance.LoadConfig());
        StartCoroutine(LoadConfigData());
    }

    // Update is called once per frame
    void Update()
    {
        if (null != hsmEntity)
        {
            hsmEntity.Execute();
        }
    }

    private IEnumerator LoadConfigData()
    {
        yield return new WaitForSeconds(1);

        park = Create("公园", new Vector3(0, 0, 0), new Vector3(90, 0, 0));
        basketballStadium = Create("篮球场", new Vector3(5, 0, 0), new Vector3(90, 0, 0));
        resetArea = Create("休息区", new Vector3(0, 0, 5), new Vector3(90, 0, 0));

        SkillHsmConfigHSMTreeData hsmConfigData = HsmTreeData.Instance.GetHsmInfo("Human");
        hsmEntity = new HsmEntity(hsmConfigData);

        for (int i = 0; i < hsmEntity.NodeList.Count; ++i)
        {
            AbstractNode abstractNode = hsmEntity.NodeList[i];

            if (typeof(IHuman).IsAssignableFrom(abstractNode.GetType()))
            {
                IHuman iHuman = abstractNode as IHuman;
                iHuman.SetHuman(this);
            }
        }

        Energy = 100;
    }

    private void UpdateEnvironment()
    {

    }

    public void StroolPark()
    {
        Energy -= 0.2f;
    }

    public void PlayBasketBall()
    {
        Energy -= 0.2f;
    }

    public void ResetEnergy()
    {
        Energy += 0.2f;
    }

    private float Energy
    {
        get { return energy; }
        set
        {
            energy = value;
            if (energy >= 100)
            {
                energy = 100;
                hsmEntity.ConditionCheck.SetParameter(HumanConstant.FullEnergy, true);

                int random = Random.Range(1, 100);
                hsmEntity.ConditionCheck.SetParameter(HumanConstant.PlayBasketBall, (random > 50));
                hsmEntity.ConditionCheck.SetParameter(HumanConstant.StroolPark, (random <= 50));
            }
            else if (energy <= 20)
            {
                hsmEntity.ConditionCheck.SetParameter(HumanConstant.FullEnergy, false);
            }
        }
    }

    private Transform Create(string name, Vector3 pos, Vector3 rot)
    {
        GameObject go = new GameObject(name);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.Euler(rot);
        go.transform.localScale = Vector3.one * 0.1f;

        TextMesh textMesh = go.AddComponent<TextMesh>();
        if (null != textMesh)
        {
            textMesh.text = name;
            textMesh.fontSize = 100;
        }

        return go.transform;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;
using GenPB;

public class Human : MonoBehaviour
{
    // 厨房
    private Transform kitchen;
    // 餐桌
    private Transform diningTable;
    // TV
    private Transform TV;

    private HsmEntity hsmEntity;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HsmTreeData.Instance.LoadConfig());
        StartCoroutine(LoadConfigData());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadConfigData()
    {
        yield return new WaitForSeconds(1);

        kitchen = Create("厨房", new Vector3(0, 0, 0), new Vector3(90, 0, 0));
        diningTable = Create("餐桌", new Vector3(5, 0, 0), new Vector3(90, 0, 0));
        TV = Create("电视", new Vector3(0, 0, 5), new Vector3(90, 0, 0));

        SkillHsmConfigHSMTreeData hsmConfigData = HsmTreeData.Instance.GetHsmInfo("Human");
        hsmEntity = new HsmEntity(hsmConfigData);
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

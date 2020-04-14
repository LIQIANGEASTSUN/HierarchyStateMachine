using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMTree;
using LitJson;
using GenPB;

public class HsmTreeData
{
    public static readonly HsmTreeData Instance = new HsmTreeData();

    #region  HsmTreeData
    private Dictionary<string, SkillHsmConfigHSMTreeData> _behaviorDic = new Dictionary<string, SkillHsmConfigHSMTreeData>();
    public void LoadData(byte[] loadByteData)
    {
        AnalysisBin.AnalysisData(loadByteData, Analysis);
    }

    private void Analysis(byte[] byteData)
    {
        SkillHsmConfigHSMTreeData skillHsmData = ProtoDataUtils.BytesToObject<SkillHsmConfigHSMTreeData>(byteData);
        _behaviorDic[skillHsmData.FileName] = skillHsmData;
    }

    public SkillHsmConfigHSMTreeData GetHsmInfo(string handleFile)
    {
        SkillHsmConfigHSMTreeData hsmTreeData = null;
        if (_behaviorDic.TryGetValue(handleFile, out hsmTreeData))
        {
            return hsmTreeData;
        }

        return hsmTreeData;
    }

    public IEnumerator LoadConfig()
    {
        string filePath = string.Format("{0}/StreamingAssets/Bina/SkillHsmConfig.bytes", Application.dataPath);
        WWW www = new WWW(filePath);
        yield return www.isDone;

        byte[] byteData = www.bytes;
        LoadData(byteData);
    }

    #endregion

}

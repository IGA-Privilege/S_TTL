using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_RunePower : Singleton<M_RunePower>
{
    public List<RunePowerInfo> runePowerInfos = new List<RunePowerInfo>();
    public UI_PowerSlot powerSlot1;
    public UI_PowerSlot powerSlot2;
    public UI_PowerSlot powerSlot3;
    public Canvas runePowerUI;

    public void ShowThreeRandomRuneUpgrades()
    {
        runePowerUI.gameObject.SetActive(true);
        RunePowerInfo[] runePowerInfos = GetThreeRandomRunePower();
        powerSlot1.SetRunePower(runePowerInfos[0].powerType, runePowerInfos[0].titleText, runePowerInfos[0].descriptionText, runePowerInfos[0].iconImage);
        powerSlot2.SetRunePower(runePowerInfos[1].powerType, runePowerInfos[1].titleText, runePowerInfos[1].descriptionText, runePowerInfos[1].iconImage);
        powerSlot3.SetRunePower(runePowerInfos[2].powerType, runePowerInfos[2].titleText, runePowerInfos[2].descriptionText, runePowerInfos[2].iconImage);
    }

    public void SelectRunePower(RunePower runePower)
    {
        M_Weapon.Instance.runeActivationDic[runePower] = true;
        runePowerUI.gameObject.SetActive(false);
        Debug.Log("获得符文升级："+ runePower.ToString() + "！");
    }

    private RunePowerInfo[] GetThreeRandomRunePower()
    {
        RunePowerInfo[] returnInfos = new RunePowerInfo[3];

        List<RunePowerInfo> nonactiveRunePowers = new List<RunePowerInfo>();

        foreach (var runePowerInfo in runePowerInfos)
        {
            if (!M_Weapon.Instance.runeActivationDic[runePowerInfo.powerType])
            {
                nonactiveRunePowers.Add(runePowerInfo);
            }
        }

        if (runePowerInfos.Count < 3)
        {
            Debug.LogError("剩余升级数量已不足3个！");
        }
        else
        {
            List<int> randomedThreeNumber = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                int randomInt = UnityEngine.Random.Range(0, runePowerInfos.Count);
                while (randomedThreeNumber.Contains(randomInt))
                {
                    randomInt = UnityEngine.Random.Range(0, runePowerInfos.Count);
                }
                randomedThreeNumber.Add(randomInt);
            }


            returnInfos[0] = runePowerInfos[randomedThreeNumber[0]];
            returnInfos[1] = runePowerInfos[randomedThreeNumber[1]];
            returnInfos[2] = runePowerInfos[randomedThreeNumber[2]];
        }

        return returnInfos;
    } 

    [System.Serializable]
    public struct RunePowerInfo
    {
        public RunePower powerType;
        public string titleText;
        public string descriptionText;
        public Sprite iconImage;
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_RunePower : Singleton<M_RunePower>
{
    public List<RunePowerInfo> runePowerInfos = new List<RunePowerInfo>();
    public UI_PowerSlot powerSlot1;
    public UI_PowerSlot powerSlot2;
    public UI_PowerSlot powerSlot3;
    public RectTransform runePowerUI;

    public void ShowThreeRandomRuneUpgrades()
    {
        runePowerUI.gameObject.SetActive(true);
        Time.timeScale = 0f;
        RunePowerInfo[] runePowerInfos = GetThreeRandomRunePower();
        powerSlot1.SetRunePower(runePowerInfos[0].powerType, runePowerInfos[0].titleText, runePowerInfos[0].descriptionText, runePowerInfos[0].iconImage);
        powerSlot2.SetRunePower(runePowerInfos[1].powerType, runePowerInfos[1].titleText, runePowerInfos[1].descriptionText, runePowerInfos[1].iconImage);
        powerSlot3.SetRunePower(runePowerInfos[2].powerType, runePowerInfos[2].titleText, runePowerInfos[2].descriptionText, runePowerInfos[2].iconImage);
    }

    public void SelectRunePower(RunePower runePower)
    {
        M_Weapon.Instance.runeActivationDic[runePower] = true;
        runePowerUI.gameObject.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("获得符文升级："+ runePower.ToString() + "！");
    }

    private RunePowerInfo[] GetThreeRandomRunePower()
    {
        RunePowerInfo[] returnInfos = new RunePowerInfo[3];

        List<RunePowerInfo> nonactiveRunePowers = new List<RunePowerInfo>();

        foreach (var runePowerInfo in runePowerInfos)
        {
            if (!M_Weapon.Instance.runeActivationDic[runePowerInfo.powerType] && runePowerInfo.powerType != RunePower.None)
            {
                nonactiveRunePowers.Add(runePowerInfo);
            }
        }

        int canUpgradeRuneNumber = Mathf.Min(3, nonactiveRunePowers.Count);

        List<int> randomedThreeNumber = new List<int>();
        for (int i = 0; i < canUpgradeRuneNumber; i++)
        {
            int randomInt = UnityEngine.Random.Range(0, nonactiveRunePowers.Count);
            while (randomedThreeNumber.Contains(randomInt))
            {
                randomInt = UnityEngine.Random.Range(0, nonactiveRunePowers.Count);
            }
            randomedThreeNumber.Add(randomInt);
        }

        if (canUpgradeRuneNumber == 3)
        {
            returnInfos[0] = nonactiveRunePowers[randomedThreeNumber[0]];
            returnInfos[1] = nonactiveRunePowers[randomedThreeNumber[1]];
            returnInfos[2] = nonactiveRunePowers[randomedThreeNumber[2]];
        }
        else if (canUpgradeRuneNumber == 2)
        {
            returnInfos[0] = nonactiveRunePowers[randomedThreeNumber[0]];
            returnInfos[1] = nonactiveRunePowers[randomedThreeNumber[1]];
            returnInfos[2] = RunePowerInfo.NoneInfo();
        }
        else if (canUpgradeRuneNumber == 1)
        {
            returnInfos[0] = nonactiveRunePowers[randomedThreeNumber[0]];
            returnInfos[1] = RunePowerInfo.NoneInfo();
            returnInfos[2] = RunePowerInfo.NoneInfo();
        }
        else if (canUpgradeRuneNumber == 0)
        {
            returnInfos[0] = RunePowerInfo.NoneInfo();
            returnInfos[1] = RunePowerInfo.NoneInfo();
            returnInfos[2] = RunePowerInfo.NoneInfo();
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

        public static RunePowerInfo NoneInfo()
        {
            RunePowerInfo noneInfo = new RunePowerInfo();
            noneInfo.powerType = RunePower.None;
            noneInfo.descriptionText = "已经没有可供选择的符文升级了！";
            noneInfo.iconImage = null;
            return noneInfo;
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PowerSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text descriptionText;
    private RunePower runePowerType;

    public void SetRunePower(RunePower type, string title, string description, Sprite sprite)
    {
        runePowerType = type;
        titleText.text = type.ToString();// 后续应改为title
        descriptionText.text = description;
        iconImage.sprite = sprite;
    }

    public void OnButtonClick()
    {
        M_RunePower.Instance.SelectRunePower(runePowerType);
    }
}

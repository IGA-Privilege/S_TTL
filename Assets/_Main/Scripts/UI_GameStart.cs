using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameStart : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    private bool _changeFlag = true;

    private void Update()
    {
        float changeSpeed = 0.5f;
        if (_changeFlag)
        {
            buttonImage.color = new Color(1f, 1f, 1f, buttonImage.color.a - changeSpeed * Time.deltaTime);
        }
        else
        {
            buttonImage.color = new Color(1f, 1f, 1f, buttonImage.color.a + changeSpeed * Time.deltaTime);
        }

        if (buttonImage.color.a > 1f || buttonImage.color.a < 0f)
        {
            _changeFlag = !_changeFlag;
        }
    }
}

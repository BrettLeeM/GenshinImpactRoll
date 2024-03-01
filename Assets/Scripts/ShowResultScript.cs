using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowResultScript : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI text;
    public Sprite Star3;
    public Sprite Star4;
    public Sprite Star5;
    public void SetName(WishAnimationInfo.StarType starType, StudentInfo info)
    {
        text.SetText(info.name);
        switch (starType)
        {
            case WishAnimationInfo.StarType.Star_3:
                background.sprite = Star3;
                break;
            case WishAnimationInfo.StarType.Star_4:
                background.sprite = Star4;
                break;
            case WishAnimationInfo.StarType.Star_5:
                background.sprite = Star5;
                break;
            default:
                break;
        }
    }
}

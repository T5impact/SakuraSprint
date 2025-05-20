using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreboardElement : MonoBehaviour
{
    [SerializeField] Image backgroundImage;
    [SerializeField] TMP_Text placeText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text pointsText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] Color[] placeColors;
    [SerializeField] Color defaultBackgroundColor;
    [SerializeField] Color selectedBackgroundColor;

    public void SetScoreboardElement(int place, HorseAttributes attributes, int points, bool selected)
    {
        switch (place)
        {
            case 0:
                placeText.text = "1st";
                placeText.color = placeColors[0];
                break;
            case 1:
                placeText.text = "2nd";
                placeText.color = placeColors[1];
                break;
            case 2:
                placeText.text = "3rd";
                placeText.color = placeColors[2];
                break;
            default:
                placeText.text = $"{place + 1}th";
                placeText.color = placeColors[3];
                break;
        }

        nameText.text = attributes.name;
        timeText.text = string.Format("{0:0.0000}", attributes.horseWeight);
        pointsText.text = $"+{points}";

        backgroundImage.color = selected ? selectedBackgroundColor : defaultBackgroundColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreboardVisuals : MonoBehaviour
{
    [SerializeField] ScoreboardElement[] scoreboardElements;
    [SerializeField] TMP_Text pointsText;
    [SerializeField] TMP_Text pointsAddedText;
    [SerializeField] GameObject scoreboardObj;

    public void HideScoreboard()
    {
        scoreboardObj.SetActive(false);
    }

    public void SetScoreboard (int[] scoreboard, int[] inverseScoreboard, HorseAttributes[] horseAttributes, int currentPoints, int selectedHorse, int[] scores)
    {
        scoreboardObj.SetActive(true);

        for (int i = 0; i < scoreboard.Length; i++)
        {
            scoreboardElements[i].SetScoreboardElement(i, horseAttributes[scoreboard[i]], scores[i], selectedHorse == scoreboard[i]);
        }

        pointsText.text = $"Points: {currentPoints}";
        pointsAddedText.text = $"+{scores[inverseScoreboard[selectedHorse]]}";
    }
}

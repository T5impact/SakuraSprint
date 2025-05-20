using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game State", menuName = "Game State")]
public class GameState : ScriptableObject
{
    public bool raceFinished = true;
    public int selectedHorse = -1;
    public int points;
    public float[] currentHorseWeights;

    public GameSave GetGameSave()
    {
        GameSave gameSave = new GameSave();
        gameSave.raceFinished = raceFinished;
        gameSave.selectedHorse = selectedHorse;
        gameSave.points = points;

        gameSave.currentHorseWeights = new float[currentHorseWeights.Length];
        for (int i = 0; i < currentHorseWeights.Length; i++)
        {
            gameSave.currentHorseWeights[i] = currentHorseWeights[i];
        }

        return gameSave;
    }

    public void SetGameSave(GameSave save)
    {
        raceFinished = save.raceFinished;
        selectedHorse = save.selectedHorse;
        points = save.points;

        currentHorseWeights = new float[save.currentHorseWeights.Length];
        for (int i = 0; i < currentHorseWeights.Length; i++)
        {
            currentHorseWeights[i] = save.currentHorseWeights[i];
        }
    }
}

public class GameSave
{
    public bool raceFinished = true;
    public int selectedHorse = -1;
    public int points;
    public float[] currentHorseWeights;
}

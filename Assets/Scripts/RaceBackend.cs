using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceBackend
{
    public float raceLength;

    public int[] scoreboard; //Takes scoreboard position and gives horse index
    public int[] scoreboardInverse; //Takes horse index and gives scoreboard position

    public int[] scores;

    public RaceBackend()
    {
    }
    public RaceBackend(HorseAttributes[] horses, float raceLength, int seed, GameState gameState)
    {
        //Create random object from seed to allow for more controllability
        System.Random rng = new System.Random(seed);

        this.raceLength = raceLength;

        //Assign horse weights randomly based on horse stamina and horse strength
        for (int i = 0; i < horses.Length; i++)
        {
            horses[i].horseWeight = rng.Next(2, 100) * (1.257f * horses[i].HorseStamina) * (horses[i].HorseStrength * horses[i].HorseStrength);
        }

        //Find min and max horse weights
        float min = float.MaxValue;
        float max = float.MinValue;
        for (int i = 0; i < horses.Length; i++)
        {
            if (horses[i].horseWeight > max)
                max = horses[i].horseWeight;

            if (horses[i].horseWeight < min)
                min = horses[i].horseWeight;
        }

        if (gameState.currentHorseWeights == null)
            gameState.currentHorseWeights = new float[horses.Length];

        for (int i = 0; i < horses.Length; i++)
        {
            //Remap horse weights to calculate how long it takes for a horse to complete the race
            horses[i].horseWeight = Remap(horses[i].horseWeight, max, min, (raceLength * 0.75f) * (rng.Next(90, 110) / 100f), raceLength * (rng.Next(95, 100) / 100f));

            if (gameState.raceFinished)
            {
                gameState.currentHorseWeights[i] = horses[i].horseWeight;
            }
            else
            {
                horses[i].horseWeight = gameState.currentHorseWeights[i];
            }
        }

        //Initialize scoreboard and scoreboard inverse
        scoreboard = scoreboardInverse = new int[horses.Length];
        for (int i = 0; i < horses.Length; i++)
        {
            scoreboard[i] = i;
        }

        //Sort scoreboard using bubble sort
        scoreboard = SortScoreboard(scoreboard, horses);

        //Assign inverse scoreboard which takes horse index and gives scoreboard position
        for (int i = 0; i < horses.Length; i++)
        {
            scoreboardInverse[scoreboard[i]] = i;
        }

        //Intialize and declare scores
        scores = new int[scoreboard.Length];
        for (int i = 0; i < scores.Length; i++)
        {
            switch(i)
            {
                case 0:
                    scores[i] = 500;
                    break;
                case 1:
                    scores[i] = 300;
                    break;
                case 2:
                    scores[i] = 200;
                    break;
                default:
                    scores[i] = 100 - 20 * (i - 3);
                    break;
            }
        }
    }

    public float Remap(float value, float currentLow, float currentHigh, float newLow, float newHigh)
    {
        return newLow + (value - currentLow) * (newHigh - newLow) / (currentHigh - currentLow);
    }

    /// <summary>
    /// Use bubble sort to sort the scoreboard based on horse weights
    /// </summary>
    /// <param name="scoreboard"></param>
    /// <param name="horses"></param>
    /// <returns></returns>
    int[] SortScoreboard(int[] scoreboard, HorseAttributes[] horses)
    {
        int[] newScoreboard = new int[scoreboard.Length];
        for (int e = 0; e < newScoreboard.Length; e++)
        {
            newScoreboard[e] = scoreboard[e];
        }

        int i, j, temp;
        for (i = 0; i < newScoreboard.Length - 1; i++)
        {
            for (j = i + 1; j < newScoreboard.Length; j++)
            {
                if (horses[newScoreboard[i]].horseWeight > horses[newScoreboard[j]].horseWeight)
                {
                    temp = newScoreboard[i];
                    newScoreboard[i] = newScoreboard[j];
                    newScoreboard[j] = temp;
                }
            }
        }
        return newScoreboard;
    }

    /// <summary>
    /// Saves the game state in a json file using the Serialization Manager
    /// </summary>
    /// <param name="gameState"></param>
    public void SaveGame(GameState gameState)
    {
        SerializationManager manager = new SerializationManager();
        manager.SaveGame(gameState);
    }
    /// <summary>
    /// Loads the game state from a json file using the Serialization Manager
    /// </summary>
    /// <param name="gameState"></param>
    public void LoadGame(GameState gameState)
    {
        SerializationManager manager = new SerializationManager();
        manager.LoadGame(gameState);
    }
}

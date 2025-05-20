using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SerializationManager
{
    public void SaveGame(GameState gameState)
    {
        string saveFile = $"{Application.persistentDataPath}/saveinformation.json";

        if (File.Exists(saveFile))
        {
            //clear the file before writing to it
            File.Delete(saveFile);
        }

        GameSave saveGame = gameState.GetGameSave();

        string json = JsonUtility.ToJson(saveGame);

        File.WriteAllText(saveFile, json);
    }
    public void LoadGame(GameState gameState)
    {
        string saveFile = $"{Application.persistentDataPath}/saveinformation.json";

        if (File.Exists(saveFile))
        {
            string json = File.ReadAllText(saveFile);

            GameSave gameSave = new GameSave();

            //read the data from file and set jsondata
            JsonUtility.FromJsonOverwrite(json, gameSave);

            gameState.SetGameSave(gameSave);
        }
    }
}

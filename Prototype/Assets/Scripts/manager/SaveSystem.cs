using UnityEngine;
using System.IO;

public class SaveSystem
{
    private static string saveFilePath = Application.persistentDataPath + "/savefile.json";

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved to " + saveFilePath);
    }

    public static SaveData LoadGame()
    {
        if(File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.LogWarning("Save File Not Found!");
            return null;
        }
    }
}

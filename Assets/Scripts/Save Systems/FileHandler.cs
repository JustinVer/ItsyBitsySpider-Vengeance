using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public SaveData()
    {
        InitilizeData();
    }

    public int currentLevel;

    public void InitilizeData()
    {
        currentLevel = 0;
    }
}

public class FileHandler
{
    public enum eSaveType
    {
        PlayerPrefs,
        Binary,
        Json
    }

    private static eSaveType SaveType = eSaveType.Binary;

    //Paths to savev files to
    private static string binaryPath = Application.persistentDataPath + "/binarySave.dat";
    private static string jsonPath = Application.persistentDataPath + "/jsonSave.json";

    static bool savePresent = false;
    public static bool SavePresent => savePresent;

    private static SaveData loadedData = null;

    public static void SaveGame(SaveData saveData)
    {
        switch (SaveType)
        {
            case eSaveType.PlayerPrefs:
                {
                    PlayerPrefs.SetInt("CurrentLevel", saveData.currentLevel);
                    PlayerPrefs.Save();
                    break;
                }

            case eSaveType.Binary:
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    FileStream fileStream = File.Create(binaryPath);

                    binaryFormatter.Serialize(fileStream, saveData);

                    fileStream.Close();

                    break;
                }

            case eSaveType.Json:
                {
                    string jsonData = JsonUtility.ToJson(saveData);

                    File.WriteAllText(jsonPath, jsonData);

                    break;
                }
        }
    }

    public static SaveData LoadGame()
    {
        if (loadedData == null)
        {
            loadedData = new SaveData();
            switch (SaveType)
            {
                case eSaveType.PlayerPrefs:
                    {
                        if (PlayerPrefs.HasKey("CurrentLevel"))
                        {
                            loadedData.currentLevel = PlayerPrefs.GetInt("CurrentLevel");
                            savePresent = true;
                        }
                        break;
                    }

                case eSaveType.Binary:
                    {
                        if (File.Exists(binaryPath))
                        {
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            FileStream fileStream = File.OpenRead(binaryPath);

                            try
                            {
                                loadedData = (SaveData)binaryFormatter.Deserialize(fileStream);
                                savePresent = true;
                            }
                            catch (Exception)
                            {
                                File.Delete(binaryPath);
                                loadedData = new SaveData();
                            }

                            fileStream.Close();
                        }
                        break;
                    }

                case eSaveType.Json:
                    {
                        if (File.Exists(jsonPath))
                        {
                            string json = File.ReadAllText(jsonPath);

                            try
                            {
                                loadedData = JsonUtility.FromJson<SaveData>(json);
                                savePresent = true;
                            }
                            catch (Exception)
                            {
                                File.Delete(jsonPath);
                                loadedData = new SaveData();
                            }
                        }
                        break;
                    }
            }
            return loadedData;
        }
        if (loadedData == null)
        {
            return new SaveData();
        }
        else
        {
            return loadedData;
        }
    }

    public static void CleanSave()
    {
        savePresent = false;
        switch (SaveType)
        {
            case eSaveType.PlayerPrefs:
                {
                    PlayerPrefs.DeleteAll();
                    break;
                }

            case eSaveType.Binary:
                {
                    if (File.Exists(binaryPath))
                    {
                        File.Delete(binaryPath);
                    }
                    break;
                }

            case eSaveType.Json:
                {
                    if (File.Exists(jsonPath))
                    {
                        File.Delete(jsonPath);
                    }
                    break;
                }
        }
    }
}

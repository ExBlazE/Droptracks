using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private int bestScore;
    private string bestScoreName;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        LoadData();
    }

    void Start()
    {
        
    }

    public bool CheckBestScore(int score)
    {
        if (score > bestScore)
        {
            return true;
        }
        else
            return false;
    }

    public void SetBestScore(int score)
    {
        bestScore = score;
    }

    public int GetBestScore()
    {
        return bestScore;
    }

    public void SetScoreName(string name)
    {
        bestScoreName = name;
    }

    public string GetScoreName()
    {
        return bestScoreName;
    }

    [System.Serializable]
    public class GameData
    {
        public int score;
        public string name;
        public float volume;
    }

    public void SaveData()
    {
        GameData gameData = new GameData();
        
        gameData.score = bestScore;
        gameData.name = bestScoreName;
        gameData.volume = MusicManager.instance.GetVolume();

        string json = JsonUtility.ToJson(gameData);

        File.WriteAllText(Application.persistentDataPath + "/gamedata.json", json);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/gamedata.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            bestScore = gameData.score;
            bestScoreName = gameData.name;
            MusicManager.instance.SetVolume(gameData.volume);
        }
    }
}

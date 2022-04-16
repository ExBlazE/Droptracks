using System.Collections;
using System.Collections.Generic;
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
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
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
}

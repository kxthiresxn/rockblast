using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;
    
    private int _coinCount, _highScore, _currentLevel;

    public int COINCOUNT
    {
        get
        {
            _coinCount = PlayerPrefs.GetInt("CoinCount"); 
            return _coinCount;
        }
        set
        {
            _coinCount = value; PlayerPrefs.SetInt("CoinCount", value);
        }
    }

    public int HIGHSCORE
    {
        get
        {
            _highScore = PlayerPrefs.GetInt("HighScore"); 
            return _highScore;
        }
        set
        {
            _highScore = value; PlayerPrefs.SetInt("HighScore", value);
        }
    }

    public int CURRENTLEVEL
    {
        get { 
            _currentLevel = PlayerPrefs.GetInt("CurrentLevel"); 
            return _currentLevel; 
        }
        set
        {
            _currentLevel = value; PlayerPrefs.SetInt("CurrentLevel", value);
        }
    }

    private void Awake()
    {
        AssignInstance();
    }

    private void AssignInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


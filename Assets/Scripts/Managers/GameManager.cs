using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;

    public static Action<GameState> OnGameStateChange;
    
    private int _score = 0;
    private readonly int _maxScore = 200;

    public static int Score
    {
        get => Instance._score;
        set
        {
            Instance._score = value;
            UIManager.Instance.UpdateScoreText(Instance._score);
            Instance.CheckLevelStatus();
        }
    }

    private void Awake()
    {
        AssignInstance();
    }

    private void Start()
    {
        _score = 0;
        ChangeGameState(GameState.MainMenu);
    }
    
    public static void ChangeGameState(GameState gameState)
    {
        OnGameStateChange?.Invoke(gameState);
        
        switch (gameState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1;
                break;
            case GameState.BeginGame:
                ChangeGameState(GameState.InGame);
                break;
            case GameState.InGame:
                Time.timeScale = 1;
                break;
            case GameState.Paused:
                Time.timeScale = 0;
                break;
            case GameState.GameOver:
                Time.timeScale = 0;
                break;
        }
    }

    private void CheckLevelStatus()
    {
        if (_score >= _maxScore)
        {
            ChangeGameState(GameState.GameOver);
        }
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

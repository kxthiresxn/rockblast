using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [Header("Panels")] public GameObject[] _panels;
    
    [Header("Main Menu Panel")]
    [SerializeField] private Animation _introAnimation;
    [SerializeField] private Button _startGameButton;

    [Header("In Game Panel")]
    [SerializeField] private GameObject _inGamePanel;
    [SerializeField] private TMP_Text _coinCountText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Slider _levelProgressBar;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _restartButton;
    
    [SerializeField]
    private void Awake()
    {
        AssignInstance();
    }

    private void OnEnable()
    {
        _startGameButton.onClick.AddListener(StartGame);
        _restartButton.onClick.AddListener(RestartGame);
        GameManager.OnGameStateChange += OnGameStateChanged;
    }
    
    private void OnDisable()
    {
        _startGameButton.onClick.RemoveAllListeners();
        GameManager.OnGameStateChange -= OnGameStateChanged;
    }

   void OnGameStateChanged(GameState gameState)
    {
        OpenPanel(gameState);
        
        switch (gameState)
        {
            case GameState.MainMenu:
                _introAnimation.Play();
                break;
            case GameState.InGame:
                _introAnimation.Stop();
                break;
            case GameState.Paused:
                break;
            case GameState.GameOver:
                break;
           
        }
    }

    private void StartGame()
    {
        _scoreText.text = "SCORE: 0";
        _coinCountText.text = PlayerData.Instance.COINCOUNT.ToString();
        _levelText.text = PlayerData.Instance.CURRENTLEVEL.ToString();
        _levelProgressBar.value = 0;
        GameManager.ChangeGameState(GameState.BeginGame);
    }

    private void OpenPanel(GameState gameState)
    {
        for (int i = 0; i < _panels.Length; i++)
        {
            _panels[i].SetActive(i == (int)gameState);
        }
    }

    public void UpdateCoinsCountText(int value)
    {
        PlayerData.Instance.COINCOUNT += value;
        _coinCountText.text = "Coins: " + PlayerData.Instance.COINCOUNT.ToString();
    }

    public void UpdateScoreText(int value)
    {
        _scoreText.text = "Score: " + value.ToString();
        _levelProgressBar.value = value * 0.005f;
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
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

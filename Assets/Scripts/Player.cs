using System;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform _bulletPivotTransform;
    [SerializeField] SpriteRenderer _gunPoint;
    [SerializeField] private Vector2 _startPosition;
    
    [Header("Animation")] [SerializeField]
    private Animation _introAnimation;

    [Header("Wheel")]
    [SerializeField] private float _wheelRotateDuration;
    [SerializeField] private float _wheelRadius = 0.5f;
    [SerializeField] private float _rotationSpeedMultiplier = 1f;
    [SerializeField] private Transform _wheel1Transform;
    [SerializeField] private Transform _wheel2Transform;

    private Vector2 _previousMousePosition;
    private int _playerTweenID;

    private float _maxPlayerBound;
    private float _previousPlayerXPos;
    private float _fireCooldownTime;
   
    private bool _fireAllowed;
    private bool _isMovingRight;

    private const string _coinTag = "Coin";

    private Tween _wheel1RotateTween, _wheel2RotateTween;

    private void OnEnable()
    {
        
        // GameManager.OnGameStateChange += OnGameStateChange;
    }
    
    private void OnDisable()
    {
        InputManager.UnSubscribeToMouseEvent(OnMouseDownEvent, OnMouseUpEvent, OnMouseClickAndHold);
        InputManager.UnSubscribeToGetMousePosition(OnMousePositionUpdate);
        // GameManager.OnGameStateChange -= OnGameStateChange;
    }
    
    // private void OnGameStateChange(GameState gameState)
    // {
    //     switch (gameState)
    //     {
    //         case GameState.MainMenu:
    //             EnablePlayerControl(false);
    //             _introAnimation.Play();
    //             break;
    //         case GameState.BeginGame:
    //             EnablePlayerControl(true);
    //             break;
    //         case GameState.InGame:
    //             _introAnimation.Stop();
    //             break;
    //         case GameState.GameOver:
    //             OnGameOver();
    //             break;
    //     }
    // }
    
    public void Start()
    {
        InputManager.SubscribeToMouseEvent(OnMouseDownEvent, OnMouseUpEvent, OnMouseClickAndHold);
        InputManager.SubscribeToGetMousePosition(OnMousePositionUpdate);

        EnablePlayerControl(true);

        BoxCollider2D playerCollider = transform.GetComponent<BoxCollider2D>();
        _maxPlayerBound = playerCollider.bounds.size.x;

        _playerTweenID = (int)DateTime.Now.Ticks;
        _fireCooldownTime = 0;
    }
    
    
    private void Update()
    {
        if ((_wheel1RotateTween.IsActive() || _wheel2RotateTween.IsActive()) &&
            Mathf.Approximately(transform.position.x, _previousPlayerXPos))
        {
            _wheel1Transform.DOKill();
            _wheel2Transform.DOKill();
        }
    }
    
    private void OnMouseUpEvent()
    {
        EnablePlayerControl(false);
        _fireCooldownTime = Constants.MAX_FIRE_COOLDOWN;
    }

    private void OnMouseDownEvent()
    {
        EnablePlayerControl(true);
    }
    
    private void OnMouseClickAndHold()
    {
        if (!_fireAllowed) return;

        if (_fireCooldownTime <= 0)
        {
            _fireCooldownTime = Constants.MAX_FIRE_COOLDOWN;
            FireBullet();
        }
        else
        {
            _fireCooldownTime -= Time.deltaTime;
        }
    }
    
    private void FireBullet()
    {
        // LevelManager.Instance.SpawnBulletObject(_bulletPivotTransform.position);
    }
    
    private void OnMousePositionUpdate(Vector2 inMousePos)
    {
        float currentPlayerX = transform.position.x;

        float cappedPosition = GetClampedPosition(inMousePos, currentPlayerX);

        _isMovingRight = cappedPosition > currentPlayerX;

        _previousPlayerXPos = cappedPosition;
        RotateWheelsBasedOnDirection();

        UpdatePlayerX(cappedPosition);
    }
    
    private float GetClampedPosition(Vector2 inMousePos, float inCurrentPlayerX)
    {
        float halfWidth = _maxPlayerBound / 2;
        float nextPos = Mathf.Abs(inMousePos.x);
        float borderX = 3f;
        float roundedPosition = borderX - halfWidth;

        if ((nextPos + halfWidth) <= borderX)
            roundedPosition = inMousePos.x;
        else if (inMousePos.x < 0)
            roundedPosition = -borderX + halfWidth;

        return roundedPosition;
    }
    
    private void UpdatePlayerX(float inDistance)
    {
        float moveSpeed = 0.10f;
        DOTween.Kill(_playerTweenID);
        transform.DOMoveX(inDistance, moveSpeed, false).SetEase(Ease.Linear).SetId(_playerTweenID);
    }

    private void RotateWheelsBasedOnDirection()
    {
        if (_isMovingRight)
        {
            _wheel1RotateTween = _wheel1Transform
                .DORotate(new Vector3(0f, 0f, -180f), _wheelRotateDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
            _wheel2RotateTween = _wheel2Transform
                .DORotate(new Vector3(0f, 0f, -180f), _wheelRotateDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }
        else
        {
            _wheel1RotateTween = _wheel1Transform
                .DORotate(new Vector3(0f, 0f, 180f), _wheelRotateDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
            _wheel2RotateTween = _wheel2Transform
                .DORotate(new Vector3(0f, 0f, 180f), _wheelRotateDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(_coinTag))
        {
            // Coin coin = other.GetComponent<Coin>();
            // coin.Hide();
            // UIManager.Instance.UpdateCoinsCountText(coin.Value);
            // LevelManager.Instance.DespawnCoinObject(coin.gameObject);
        }
    }
    
    private void EnablePlayerControl(bool inEnable)
    {
        _fireAllowed = inEnable;

        _wheel1Transform.DOKill();
        _wheel2Transform.DOKill();
    }

    private void OnGameOver()
    {
        EnablePlayerControl(false);
    }

    public void OnLivesLost()
    {
        EnablePlayerControl(false);

        transform.GetComponent<BoxCollider2D>().enabled = false;

        // GameManager.ChangeGameState(GameState.GameOver);
    }
}
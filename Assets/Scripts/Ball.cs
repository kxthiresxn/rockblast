using System;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class Ball : MonoBehaviour
{
    public ObjectPool<Ball> Pool { get; set; }
    
    [Header("References")]
    [SerializeField] protected Rigidbody2D _rigidBody;
    [SerializeField] TMP_Text _countText;

    private BallSize _currentBallSizeLevel;
    private Vector2 _ballForce;
    
    private uint _maxBalHitCountToSplit;
    private uint _ballHitCounter;

    private float _maxYBounceForce;
    
    private bool _isBallBouncingTowardsRight;
    private bool _isInitialisationComplete;
    
    private readonly string _bulletTag = "Bullet";
    private readonly string _borderTag = "Border";
    private readonly string _playerTag = "Player";
    
    Tween _rotationTween;

    public virtual void Awake()
    {
        _ballForce = Vector2.zero;
        _ballHitCounter = 0;
        _isInitialisationComplete = false;
        _isBallBouncingTowardsRight = false;

        UpdateCountText(0);

        Vector3 rotation = Vector3.one;
        rotation.z = 360f;
        float rotationDur = 3f;

        _rotationTween = transform.DORotate(rotation, rotationDur, RotateMode.LocalAxisAdd)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
        _rotationTween.Pause();
    }
    
    public void InitNewBall(uint maxHitCount, Vector3 position, BallSize ballSize, float xVelocity)
    {
        InitializeBall(maxHitCount, position, ballSize, xVelocity);

        Vector3 spawnPosition = GetSpawnPosition(position);
        AnimateSlowSpawn(spawnPosition, () => SetRigidBodyType(RigidbodyType2D.Dynamic));
    }
    
    private void AnimateSlowSpawn(Vector3 spawnPosition, Action OnComplete = null)
    {
        transform.DOMoveX(spawnPosition.x, 1f, false)
            .OnComplete(() => OnComplete?.Invoke())
            .SetEase(Ease.Linear);
    }

    public void InitSplitBall(uint maxHitCount, Vector3 position, BallSize ballSize, float xVelocity)
    {
        InitializeBall(maxHitCount, position, ballSize, xVelocity);

        _ballForce.x = xVelocity;
        _ballForce.y = 5f;

        SetRigidBodyType(RigidbodyType2D.Dynamic);
        ApplyForce(_ballForce);
    }

    private void InitializeBall(uint maxHitCount, Vector3 position, BallSize ballSize, float xVelocity)
    {
        _maxBalHitCountToSplit = maxHitCount;
        UpdateCountText(_maxBalHitCountToSplit);
        ResetBallProperties();
        transform.position = position;
        _currentBallSizeLevel = ballSize;
        UpdateScale();
        Show();
        
        _maxYBounceForce = GetBounceForce();

        _isBallBouncingTowardsRight = xVelocity > 0;
        _isInitialisationComplete = true;
    }

    private void SetRigidBodyType(RigidbodyType2D inBodyType)
    {
        _rigidBody.bodyType = inBodyType;
    }
    
    private void Show()
    {
        gameObject.SetActive(true);
        _rotationTween.Play();
    }
    
    private void Hide()
    {
        var resetPos = transform.position;
        resetPos.x = resetPos.y = 0;
        transform.position = resetPos;

        _rotationTween.Pause();

        Pool.Release(this);
    }
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isInitialisationComplete) return;
        
        if (other.CompareTag(_bulletTag))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            
            OnHitBullet(bullet.BulletDamage);
            bullet.Hide();
        }

        if (other.CompareTag(_borderTag))
        {
            Border border = other.gameObject.GetComponent<Border>();
            
            if(border.BorderDirection == Direction.None)
                return;
            
            if(border.BorderDirection == Direction.Bottom && transform.localScale.x == 0.4f)
                CameraManager.Instance.StartCameraShake();
            
            UpdateBallForceForTheDirection(border.BorderDirection, ref _ballForce);
            ApplyForce(_ballForce);
        }

        // if (other.CompareTag(_playerTag))
        // {
        //     Player player = other.gameObject.GetComponent<Player>();
        //     player.OnLivesLost();
        // }
    }
    
   
    private void OnHitBullet(uint inDamage)
    {
        _ballHitCounter += inDamage;
        UpdateCountText(_maxBalHitCountToSplit - _ballHitCounter);
        
        // GameManager.Score += (int)inDamage;

        if (_currentBallSizeLevel == BallSize.Level_0 && _ballHitCounter >= _maxBalHitCountToSplit)
        {
            LevelManager.Instance.SpawnCoinObject(transform.position);
            Hide();
        }
        else
        {
            if (_ballHitCounter >= _maxBalHitCountToSplit)
            {
                var currentPosition = transform.position;
                
                BallSize decreasedSize = _currentBallSizeLevel - 1;
                _currentBallSizeLevel = decreasedSize == BallSize.Level_0 ? BallSize.Level_0 : decreasedSize;
                
                LevelManager.Instance.SpawnSplitBallObject(_maxBalHitCountToSplit, currentPosition, _currentBallSizeLevel);
                
                Hide();
            }
        }
    }

    private void UpdateBallForceForTheDirection(Direction borderDirection, ref Vector2 outForce)
    {
        if (borderDirection == Direction.None) return;

        outForce = _rigidBody.velocity;

        float xVelocity = GetXVelocity();

        if (borderDirection == Direction.Left || borderDirection == Direction.Right)
        {
            // Left or right Border
            if (borderDirection == Direction.Right)
            {
                outForce.x = -xVelocity;
                _isBallBouncingTowardsRight = false;
            }
            else
            {
                outForce.x = xVelocity;
                _isBallBouncingTowardsRight = true;
            }
        }
        else
        {
            //to make sure not going out of the screeen
            outForce.y = -2;

            if (borderDirection == Direction.Bottom)
            {
                outForce.y = _maxYBounceForce;

                outForce.x = -xVelocity; //Deafult right

                if (_isBallBouncingTowardsRight)
                    outForce.x = xVelocity; // To Left
            }
        }
    }

    private void ApplyForce(Vector2 inForce)
    {
        _rigidBody.velocity = inForce;
    }
    
    private void UpdateScale()
    {
        Vector2 scale = _currentBallSizeLevel switch
        {
            BallSize.Level_0 => Vector2.one * 0.2f,
            BallSize.Level_1 => Vector2.one * 0.4f,
            BallSize.Level_2 => Vector2.one * 0.6f,
            BallSize.Level_3 => Vector2.one * 0.8f,
            _ => Vector2.one // Default case (optional)
        };

        transform.localScale = scale;
    }

    private float GetXVelocity()
    {
        float percentage = _currentBallSizeLevel switch
        {
            BallSize.Level_0 => 0.50f,
            BallSize.Level_1 => 0.65f,
            BallSize.Level_2 => 0.80f,
            BallSize.Level_3 => 1.00f,
            _ => 1.00f // Default case (optional)
        };

        return Constants.MAX_X_VELOCITY * percentage;
    }

    private float GetBounceForce()
    {
        return _currentBallSizeLevel switch
        {
            BallSize.Level_0 => 8f,
            BallSize.Level_1 => 10f,
            BallSize.Level_2 => 12f,
            BallSize.Level_3 => 14f,
            _ => 25f // Default case (optional)
        };
    }

    private Vector3 GetSpawnPosition(Vector3 inPos)
    {
        Vector3 spawnPoint = transform.position;
        spawnPoint.x = transform.position.x - 1.5f;
        if (inPos.x < 0)
            spawnPoint.x = transform.position.x + 1.5f;

        spawnPoint.y = transform.position.y - 2.35f;
        return spawnPoint;
    }

    private void ResetBallProperties()
    {
        SetRigidBodyType(RigidbodyType2D.Static);
        _isBallBouncingTowardsRight = false;
        _isInitialisationComplete = false;
        _ballForce = Vector2.zero;
        _ballHitCounter = 0;
    }
    
    private void UpdateCountText(uint inCount)
    {
        _countText.text = inCount.ToString();
    }
}

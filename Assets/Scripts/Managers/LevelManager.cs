using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Pool;

public class LevelManager : MonoBehaviour
{
   public static LevelManager Instance;
   
   [Header("BulletPool")]
   public Bullet BulletPrefab;
   private ObjectPool<Bullet> _bulletPool;
   
   [Header("BallPool")]
   public Ball BallPrefab;
   private ObjectPool<Ball> _ballPool;
   
   [Header("CoinPool")]
   public Coin CoinPrefab;
   private ObjectPool<Coin> _coinPool;
   
   [Header("Ball Spawn Point References")]
   [SerializeField] private Transform [] _ballSpawnPoints;
   
   private int _currentBallCount;
   private bool _spawnAllowed;
   
   [Header("Ball Spawn Settings")]
   [SerializeField] private int _maxBallsOnScreen = 5;
   
   void Awake()
   {
      AssignInstance();
      
      // PoolBullets();
      // PoolBalls();
      // PoolCoins();
   }

   private void PoolBullets()
   {
      _bulletPool = new ObjectPool<Bullet>(
         createFunc: () => Instantiate(BulletPrefab, gameObject.transform),
         actionOnGet: bullet => bullet.gameObject.SetActive(true),
         actionOnRelease: bullet => bullet.gameObject.SetActive(false),
         actionOnDestroy: bullet => Destroy(bullet.gameObject),
         collectionCheck: false,
         defaultCapacity: 20,
         maxSize: 30
      );
   }
   
   private void PoolBalls()
   {
      _ballPool = new ObjectPool<Ball>(
         createFunc: () => Instantiate(BallPrefab, gameObject.transform),
         actionOnGet: ball => ball.gameObject.SetActive(true),
         actionOnRelease: ball =>
         {
            _currentBallCount--;
            ball.gameObject.SetActive(false);
         },
         actionOnDestroy: ball => Destroy(ball.gameObject),
         collectionCheck: false,
         defaultCapacity: 20,
         maxSize: 30
      );
   }
   
   private void PoolCoins()
   {
      _coinPool = new ObjectPool<Coin>(
         createFunc: () => Instantiate(CoinPrefab, gameObject.transform),
         actionOnGet: coin => coin.gameObject.SetActive(true),
         actionOnRelease: coin => coin.gameObject.SetActive(false),
         actionOnDestroy: coin => Destroy(coin.gameObject),
         collectionCheck: false,
         defaultCapacity: 20,
         maxSize: 30
      );
   }

   private void Start()
   {
      // StartGame();
   }

   private void OnEnable()
   {
      // GameManager.OnGameStateChange += OnGameStateChange;
   }

   private void OnDisable()
   {
      // GameManager.OnGameStateChange -= OnGameStateChange;
   }
   
   // private void OnGameStateChange(GameState gameState)
   // {
   //    switch (gameState)
   //    {
   //       case GameState.MainMenu:
   //          break;
   //       case GameState.BeginGame:
   //          StartGame();
   //          break;
   //       case GameState.InGame:
   //          break;
   //       case GameState.Paused:
   //          break;
   //       case GameState.GameOver:
   //          EndGame();
   //          break;
   //    }
   // }

   private void StartGame()
   {
      _currentBallCount = 0;
      _spawnAllowed = true;
      StartCoroutine(CheckAndSpawnBall());
   }

   private void EndGame()
   {
      _currentBallCount = 0;
      _spawnAllowed = false;
      StopCoroutine(CheckAndSpawnBall());
   }

   private readonly WaitForSeconds _waitForSeconds = new WaitForSeconds(0.5f);
   IEnumerator CheckAndSpawnBall()
   {
      while (_spawnAllowed)
      {
         if (_currentBallCount < _maxBallsOnScreen)
         {
            SpawnBallObject(5, GetBallSpawnPoint(), BallSize.Level_1);
         }
         yield return _waitForSeconds;
      }
   }

   private Vector3 GetBallSpawnPoint()
   {
      int totalRandomPositions = _ballSpawnPoints.Length;
      int radomIndex = UnityEngine.Random.Range(0, totalRandomPositions);

      Vector3 position = _ballSpawnPoints[radomIndex].position;
      position.z = GetZOrderValue();

      return position;
   }
   
   private void SpawnBallObject(uint maxHitCount, Vector3 position, BallSize ballSize)
   {
      Ball ball = _ballPool.Get();
      ball.Pool = _ballPool;
      ball.InitNewBall(maxHitCount, position, ballSize, Constants.SPLIT_BALL_INIT_X_VELOCITY);
      _currentBallCount += 3; 
   }

   public void SpawnSplitBallObject(uint maxHitCount, Vector3 position, BallSize ballSize)
   {
      int numberOfSpawnForSplit = 2;
      bool toLeft = false;
      float xVelocity;

      for (int i = 0; i < numberOfSpawnForSplit; i++)
      {
         Vector3 _position = position;
         
         xVelocity = Constants.SPLIT_BALL_INIT_X_VELOCITY;

         if (toLeft)
         {
            xVelocity = -Constants.SPLIT_BALL_INIT_X_VELOCITY;
         }
         
         position.z = GetZOrderValue();
        
         Ball ball = _ballPool.Get();
         ball.Pool = _ballPool;
         ball.InitSplitBall(maxHitCount, _position, ballSize, xVelocity);
         toLeft = !toLeft;
         // _currentBallCount += 2; 
      }
   }

   public void SpawnBulletObject(Vector3 position)
   {
      Bullet bullet = _bulletPool.Get();
      bullet.transform.position = position;
      bullet.Pool = _bulletPool;
      bullet.Fire();
   }
   
   public void SpawnCoinObject(Vector3 position)
   {
      Coin coin = _coinPool.Get();
      coin.transform.position = position;
      coin.Pool = _coinPool;
      coin.Show();
   }
   
   private float GetZOrderValue()
   {
      return UnityEngine.Random.Range(-0.1f, -1f);
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

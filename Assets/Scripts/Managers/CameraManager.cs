using System;
using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
   public static CameraManager Instance;
   
   private Camera _camera;
   private Tween _cameraShakeTween;

   private void Awake()
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
   
   private void Start()
   {
      _camera = GetComponent<Camera>();
   }

   public void StartCameraShake()
   {
      _camera.transform.DOKill();

      _camera.transform.position = new Vector3(0f, 0f, _camera.transform.position.z);
      _cameraShakeTween = _camera.DOShakePosition(0.3f, 0.05f, 5).OnComplete(() =>
      {
         _camera.transform.position = new Vector3(0f, 0f, _camera.transform.position.z);
      });
   }
}

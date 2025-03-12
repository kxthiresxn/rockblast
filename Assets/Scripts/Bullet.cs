using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    public ObjectPool<Bullet> Pool { get; set; }
    public uint BulletDamage { get ; private set; }

    private Tween _bulletMovingTween;
    
    private int _id => transform.GetInstanceID();

    private readonly float _tweenDuration = 0.70f;
    
    private void Awake()
    {
        BulletDamage = GetBulletDamage();
    }

    public void Fire()
    {
        gameObject.SetActive(true);
            
        DOTween.Kill(_id);
        
        BulletDamage = GetBulletDamage();
            
        _bulletMovingTween = transform.DOMoveY(Constants.TOP_BORDER, _tweenDuration, false)
            .SetEase(Ease.Linear)
            .SetId(_id).OnComplete(Hide);
    }

    public void Hide()
    {
        _bulletMovingTween.Pause();
        Pool.Release(this);
    }
    
    private uint GetBulletDamage()
    {
        return Constants.BULLET_DAMAGE;
    }
}


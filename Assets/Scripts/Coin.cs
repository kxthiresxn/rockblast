using UnityEngine;
using UnityEngine.Pool;

public class Coin : MonoBehaviour
{
  public ObjectPool<Coin> Pool { get; set; }

  [SerializeField] private Rigidbody2D _rigidbody2D;

  public int Value => Constants.COIN_VALUE;

  public void Show()
  {
    transform.rotation = Quaternion.identity;
    SetRigidBodyType(RigidbodyType2D.Dynamic);
    _rigidbody2D.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
  }

  public void Hide()
  {
    SetRigidBodyType(RigidbodyType2D.Static);
    Pool.Release(this);
  }
  
  private void SetRigidBodyType(RigidbodyType2D inBodyType)
  {
    _rigidbody2D.bodyType = inBodyType;
  }
}

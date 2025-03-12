using UnityEngine;

public class Border : MonoBehaviour
{
    [SerializeField] private Direction _borderDirection;
    public Direction BorderDirection => _borderDirection;
}

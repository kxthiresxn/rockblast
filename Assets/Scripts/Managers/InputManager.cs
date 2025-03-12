using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager Instance;
    private event Action MouseDownEvent;
    private event Action MouseClickAndHoldEvent;
    private event Action MouseUpEvent;
    private event Action<Vector2> MousePositionUpdateEvent;

    private Vector2 _previousMousePosition;
    
    private bool _inputEnabled = true;
   
    private void Awake()
    {
        AssignInstance();
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChange += OnGameStateChange;   
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;   
    }

    private void OnGameStateChange(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.InGame:
                _inputEnabled = true;
                break;
            default:
                _inputEnabled = false;
                break;
        }
    }

    public static void SubscribeToGetMousePosition(Action<Vector2> inCallBack)
    {
        Instance.MousePositionUpdateEvent += inCallBack;
    }

    public static void UnSubscribeToGetMousePosition(Action<Vector2> inCallBack)
    {
        Instance.MousePositionUpdateEvent -= inCallBack;
    }
    
    public static void SubscribeToMouseEvent(Action inOnMouseDown, Action inOnMouseUp, Action inMouseClickAndHold)
    {
        Instance.MouseDownEvent += inOnMouseDown;
        Instance.MouseUpEvent += inOnMouseUp;
        Instance.MouseClickAndHoldEvent += inMouseClickAndHold;
    }
    
    public static void UnSubscribeToMouseEvent(Action inOnMouseDown, Action inOnMouseUp, Action inMouseClickAndHold)
    {
        Instance.MouseDownEvent -= inOnMouseDown; 
        Instance.MouseUpEvent -= inOnMouseUp;
        Instance.MouseClickAndHoldEvent -= inMouseClickAndHold;
    }

    private void Update()
    {
        if(!_inputEnabled)
            return;
        
        if (Input.GetMouseButtonUp(0))
        {
            MouseUpEvent?.Invoke();
        }
        if (Input.GetMouseButtonDown(0))
        {
            MouseDownEvent?.Invoke();
        }
        if (Input.GetMouseButton(0))
        {
            MouseClickAndHoldEvent?.Invoke();
            
            Vector2 inputMouse = Input.mousePosition;
            Vector2 currentMousePosInWorld = Camera.main.ScreenToWorldPoint(inputMouse);

            if (Mathf.Approximately(inputMouse.x, _previousMousePosition.x) || MousePositionUpdateEvent == null) return;

            _previousMousePosition = inputMouse;
            MousePositionUpdateEvent?.Invoke(currentMousePosInWorld);
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
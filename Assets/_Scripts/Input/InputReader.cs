using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject, PlayerInput.IGameplayActions
{
    public event Action<Vector2> MoveEvent;
    public event Action JumpEvent;

    private PlayerInput _playerInput;


    private void OnEnable()
    {
        if (_playerInput == null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            _playerInput = new PlayerInput();
                
            _playerInput.Gameplay.SetCallbacks(this);
            _playerInput.Gameplay.Enable();
        }
    }

    private void OnDisable()
    {
        if (_playerInput != null)
        {
            _playerInput.Gameplay.Disable();
            _playerInput.Gameplay.SetCallbacks(null);
        }
    }

    public void OnMove(InputAction.CallbackContext context) => MoveEvent?.Invoke(context.ReadValue<Vector2>());

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            JumpEvent?.Invoke();
    }
}
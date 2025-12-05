using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles reading inputs from the Unity Input System and exposing them as events.
/// Implements the <see cref="InputSystem_Actions.IPlayerActions"/> interface to receive input callbacks.
/// </summary>
public class PlayerInputReader : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    // Action Events
    public event Action<Vector2> OnMoveInputChanged;
    public event Action OnJumpPerformed;
    public event Action OnAttackPerformed;
    public event Action OnInteractPerformed;
    public event Action OnPreviousPerformed;
    public event Action OnNextPerformed;
    public event Action OnInventoryPerformed;

    // State Action Events
    public event Action OnSprintStarted;
    public event Action OnSprintCanceled;
    public event Action OnCrouchStarted;
    public event Action OnCrouchCanceled;

    private InputSystem_Actions _inputSystemActions;

    /// <summary>
    /// Initializes the Input System reference, connects the callbacks and enables the "Player" action map
    /// when the script is enabled.
    /// </summary>
    private void OnEnable()
    {
        if (_inputSystemActions == null)
        {
            _inputSystemActions = new InputSystem_Actions();
            _inputSystemActions.Player.SetCallbacks(this);
        }
        _inputSystemActions.Player.Enable();
    }

    /// <summary>
    /// Disables the "Player" action map when the script is disabled.
    /// </summary>
    private void OnDisable()
    {
        _inputSystemActions.Player.Disable();
    }

    #region Input Callbacks

    /// <summary>
    /// Callback for the Move input action. Reads the vector value.
    /// </summary>
    /// <param name="context">Context containing input data.</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        OnMoveInputChanged?.Invoke(MoveInput);
    }

    /// <summary>
    /// Callback for the Look input action. Reads the vector value.
    /// </summary>
    /// <param name="context">Context containing input data.</param>
    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Callback for the Attack input action.
    /// </summary>
    /// <param name="context">Context containing input data.</param>
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) OnAttackPerformed?.Invoke();
    }

    /// <summary>
    /// Callback for the Interact input action.
    /// </summary>
    /// <param name="context">Context containing input data.</param>
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) OnInteractPerformed?.Invoke();
    }

    /// <summary>
    /// Callback for the Jump input action.
    /// </summary>
    /// <param name="context">Context containing input data.</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) OnJumpPerformed?.Invoke();
    }

    /// <summary>
    /// Callback for switching to the previous item.
    /// </summary>
    /// <param name="context">Context containing input data.</param>
    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (context.performed) OnPreviousPerformed?.Invoke();
    }

    /// <summary>
    /// Callback for switching to the next item.
    /// </summary>
    /// <param name="context">Context containing input data.</param>
    public void OnNext(InputAction.CallbackContext context)
    {
        if (context.performed) OnNextPerformed?.Invoke();
    }

    /// <summary>
    /// Callback for the Sprint input action. Handles both start (press) and cancel (release).
    /// </summary>
    /// <param name="context">Context containing input data.</param>
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
            OnSprintStarted?.Invoke();
        else if (context.canceled)
            OnSprintCanceled?.Invoke();
    }

    /// <summary>
    /// Callback for the Crouch input action. Handles both start (press) and cancel (release).
    /// </summary>
    /// <param name="context">Context containing input data.</param>
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
            OnCrouchStarted?.Invoke();
        else if (context.canceled)
            OnCrouchCanceled?.Invoke();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed) OnInventoryPerformed?.Invoke();
    }
    

    #endregion
}
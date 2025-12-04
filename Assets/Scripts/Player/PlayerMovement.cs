using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles character movement logic using the Unity CharacterController and the Input System.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    #region Dependencies
    private InputSystem_Actions _inputs;
    private CharacterController _controller;
    #endregion

    private Vector2 _moveInput;

    [Header("Speed Settings")]
    [SerializeField] public float forwardSpeed = 4.5f;
    [SerializeField] public float backwardSpeed = 2f;
    [SerializeField] public float strafeSpeed = 3.5f;

    public event Action<Vector2> OnMoveInputChanged;

    /// <summary>
    /// Initializes component references and input action instance.
    /// </summary>
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _inputs = new InputSystem_Actions();
    }

    /// <summary>
    /// Enables input actions and subscribes to movement callbacks.
    /// </summary>
    void OnEnable()
    {
        _inputs.Player.Enable();
        _inputs.Player.Move.performed += OnMove;
        _inputs.Player.Move.canceled += OnMoveCanceled;
    }

    /// <summary>
    /// Disables input actions and unsubscribes from movement callbacks.
    /// </summary>
    void OnDisable()
    {
        _inputs.Player.Move.performed -= OnMove;
        _inputs.Player.Move.canceled -= OnMoveCanceled;
        _inputs.Player.Disable();
    }

    /// <summary>
    /// Calculates movement logic every frame based on input state and defined speeds.
    /// </summary>
    void Update()
    {
        CalculateAndApplyMovement();
    }

    /// <summary>
    /// Callback invoked when the Move action is performed.
    /// </summary>
    /// <param name="context">Context containing input data.</param>
    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Callback invoked when the Move action is canceled (input released).
    /// </summary>
    /// <param name="context">Context containing input data.</param>
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    /// <summary>
    /// Determines the speed based on direction and moves the CharacterController.
    /// </summary>
    private void CalculateAndApplyMovement()
    {
        float targetSpeedZ = (_moveInput.y >= 0) ? forwardSpeed : backwardSpeed;
        float targetSpeedX = strafeSpeed;

        var movementVector = new Vector3(_moveInput.x * targetSpeedX, 0f, _moveInput.y * targetSpeedZ);

        _controller.Move(movementVector * Time.deltaTime);

        OnMoveInputChanged?.Invoke(_moveInput);
    }
}
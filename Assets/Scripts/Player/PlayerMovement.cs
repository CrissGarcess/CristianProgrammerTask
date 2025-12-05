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
    private PlayerInputReader _playerInputReader;
    private CharacterController _controller;
    [SerializeField] private Transform _cameraTransform;
    #endregion

    private Vector2 _moveInput;

    [Header("Speed Settings")]
    [SerializeField] public float forwardSpeed = 4.5f;
    [SerializeField] public float backwardSpeed = 2f;
    [SerializeField] public float strafeSpeed = 3.5f;
    [SerializeField] public float rotationSpeed = 3.5f;

    public event Action<Vector2> OnMoveInputChanged;

    /// <summary>
    /// Initializes component references and input action instance.
    /// </summary>
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerInputReader = GetComponent<PlayerInputReader>();
    }

    /// <summary>
    /// Enables input actions and subscribes to movement callbacks.
    /// </summary>
    void OnEnable()
    {
        _playerInputReader.OnMoveInputChanged += OnMove;
    }

    /// <summary>
    /// Disables input actions and unsubscribes from movement callbacks.
    /// </summary>
    void OnDisable()
    {
        _playerInputReader.OnMoveInputChanged -= OnMove;
    }

    /// <summary>
    /// Calculates movement logic every frame based on input state and defined speeds.
    /// </summary>
    void Update()
    {
        CalculateMovement();
    }

    /// <summary>
    /// Callback invoked when the Move action is performed.
    /// </summary>
    /// <param name="context">Context containing input data.</param>
    private void OnMove(Vector2 moveValue)
    {
        _moveInput = moveValue;
    }

    /// <summary>
    /// Determines the appropriate speed based on direction and camera-relative movement.
    /// </summary>
    private void CalculateMovement()
    {
        float targetSpeedY = (_moveInput.y >= 0) ? forwardSpeed : backwardSpeed;
        float targetSpeedX = strafeSpeed;

        // Camera-relative directions
        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 movement = (camForward * _moveInput.y * targetSpeedY) + (camRight * _moveInput.x * targetSpeedX);

        ApplyMovement(movement);
        ApplyRotation(camForward);
    }

    /// <summary>
    /// Applies translation to the CharacterController.
    /// </summary>
    /// <param name="movement">World-space movement vector.</param>
    private void ApplyMovement(Vector3 movement)
    {
        _controller.Move(movement * Time.deltaTime);
        OnMoveInputChanged?.Invoke(_moveInput);
    }

    /// <summary>
    /// Applies rotation depending on the camera's forward direction.
    /// </summary>
    /// <param name="camForward">Flattened camera forward vector.</param>
    private void ApplyRotation(Vector3 camForward)
    {
        camForward.y = 0f;
        if (camForward.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
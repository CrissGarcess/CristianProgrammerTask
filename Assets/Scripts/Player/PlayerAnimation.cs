using UnityEngine;

/// <summary>
/// Manages the player's animation states by subscribing to input data events from the 
/// <see cref="PlayerMovement"/>.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimation : MonoBehaviour
{
    #region Dependencies
    private Animator _animator;
    private PlayerMovement _playerMovement;
    #endregion

    [Header("Settings")]
    [SerializeField] private float animationSmoothTime = 0.1f;

    [Header("Animator Parameters")]
    [SerializeField] private string _inputX = "Strafe";
    [SerializeField] private string _inputY = "Speed";

    private int _inputXHash;
    private int _inputYHash;

    /// <summary>
    /// Initializes component references and create Animator parameter hashes.
    /// </summary>
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();

        _inputXHash = Animator.StringToHash(_inputX);
        _inputYHash = Animator.StringToHash(_inputY);
    }

    /// <summary>
    /// Subscribes to the movement event when the object is enabled.
    /// </summary>
    void OnEnable()
    {
        if (_playerMovement != null)
        {
            _playerMovement.OnMoveInputChanged += UpdateMovementAnimation;
        }
    }

    /// <summary>
    /// Unsubscribes from the movement event when disabled.
    /// </summary>
    void OnDisable()
    {
        if (_playerMovement != null)
        {
            _playerMovement.OnMoveInputChanged -= UpdateMovementAnimation;
        }
    }

    /// <summary>
    /// Updates the Animator float parameters based on the provided movement input vector.
    /// </summary>
    /// <param name="moveInput">A Vector2 representing the player's input (X,Y).</param>
    private void UpdateMovementAnimation(Vector2 moveInput)
    {
        _animator.SetFloat(_inputXHash, moveInput.x, animationSmoothTime, Time.deltaTime);
        _animator.SetFloat(_inputYHash, moveInput.y, animationSmoothTime, Time.deltaTime);
    }
}
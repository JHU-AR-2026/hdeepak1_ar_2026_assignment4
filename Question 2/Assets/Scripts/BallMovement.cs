using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Rolls the ball using WASD or the arrow keys and jumps with Space.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BallMovement : MonoBehaviour
{
    [SerializeField] private float moveForce = 14f;
    [SerializeField] private float maximumSpeed = 8f;
    [SerializeField] private float jumpForce = 6.5f;

    private Rigidbody ballBody;
    private bool isGrounded;
    private bool jumpRequested;
    private bool gameStarted;

    public bool IsPowerUpActive { get; private set; }

    private void Awake()
    {
        ballBody = GetComponent<Rigidbody>();
        ballBody.useGravity = false;

        // Excluding the ball from raycasts lets the ground check see the floor beneath it.
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void Update()
    {
        if (!gameStarted)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            // Queue the input so the physics impulse is applied during FixedUpdate.
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        if (!gameStarted)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            0.65f,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);

        // Support both common control schemes without requiring an Input Action asset.
        float horizontal = ReadAxis(keyboard.aKey, keyboard.dKey, keyboard.leftArrowKey, keyboard.rightArrowKey);
        float vertical = ReadAxis(keyboard.sKey, keyboard.wKey, keyboard.downArrowKey, keyboard.upArrowKey);
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        ballBody.AddForce(direction * moveForce, ForceMode.Acceleration);

        if (jumpRequested && isGrounded)
        {
            ballBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        jumpRequested = false;

        // Keep the ball controllable instead of allowing it to accelerate forever.
        Vector3 horizontalVelocity = new Vector3(ballBody.linearVelocity.x, 0f, ballBody.linearVelocity.z);
        if (horizontalVelocity.magnitude > maximumSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maximumSpeed;
            ballBody.linearVelocity = new Vector3(horizontalVelocity.x, ballBody.linearVelocity.y, horizontalVelocity.z);
        }
    }

    private static float ReadAxis(
        KeyControl negative,
        KeyControl positive,
        KeyControl alternateNegative,
        KeyControl alternatePositive)
    {
        float value = 0f;
        value -= negative.isPressed || alternateNegative.isPressed ? 1f : 0f;
        value += positive.isPressed || alternatePositive.isPressed ? 1f : 0f;
        return value;
    }

    public void ActivatePowerUp()
    {
        if (IsPowerUpActive)
        {
            return;
        }

        IsPowerUpActive = true;
        moveForce *= 1.35f;
        maximumSpeed *= 1.75f;

        // Cyan provides an unmistakable visual indication that the boost is active.
        GetComponent<Renderer>().material.color = new Color(0.05f, 0.9f, 1f);
    }

    public void BeginGame()
    {
        // The Start button is the only place that unlocks physics and player input.
        gameStarted = true;
        ballBody.useGravity = true;
    }
}

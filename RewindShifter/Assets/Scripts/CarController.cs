using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public float acceleration = 20f;   // how fast we speed up
    public float maxForwardSpeed = 25f;
    public float maxReverseSpeed = 10f;
    public float turnSpeed = 80f;
    public float deceleration = 10f;   // how fast car slows down when no input

    private Rigidbody rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0f; // turn off automatic drag
    }

    void FixedUpdate()
    {
        Vector3 forward = transform.forward;
        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // acceleration and decceleration
        if (moveInput.y > 0)
        {
            // forward
            horizontalVel += forward * moveInput.y * acceleration * Time.fixedDeltaTime;
            horizontalVel = Vector3.ClampMagnitude(horizontalVel, maxForwardSpeed);
        }
        else if (moveInput.y < 0)
        {
            // reverse
            horizontalVel += forward * moveInput.y * acceleration * Time.fixedDeltaTime;
            horizontalVel = Vector3.ClampMagnitude(horizontalVel, maxReverseSpeed);
        }
        else
        {
            // no input -> decelerate smoothly
            horizontalVel = Vector3.MoveTowards(horizontalVel, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = new Vector3(horizontalVel.x, rb.linearVelocity.y, horizontalVel.z);

        // steering
        if (horizontalVel.magnitude > 0.1f)
        {
            float turn = moveInput.x;
            float turnAmount = turn * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turnAmount, 0f));
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}
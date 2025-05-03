using UnityEngine;

public class WristInteraction : MonoBehaviour
{
    // The wrist Transform that the object will interact with
    public Transform wrist;

    // The object that will move towards and orbit around the wrist
    public Transform movingObject;

    // The speed at which the object moves towards the wrist
    public float movementSpeed = 1.0f;

    // The distance threshold to detect proximity between the object and the wrist
    public float thresholdDistance = 0.1f;

    // The radius of the orbit once the object reaches the wrist
    public float orbitRadius = 0.1f;

    // The speed of the orbit (degrees per second)
    public float orbitSpeed = 90.0f;

    // The initial angle position around the wrist
    public float initialAngle = 0.0f;

    // Internal variable to track the current angle during orbit
    private float currentAngle;

    // Flag to determine the current state of the object
    private enum State { Idle, MovingToWrist, Orbiting }
    private State currentState = State.Idle;

    void Start()
    {
        // Initialize the current angle
        currentAngle = initialAngle;
    }

    void Update()
    {
        // Check the current state and act accordingly
        switch (currentState)
        {
            case State.Idle:
                CheckWristProximity();
                break;
            case State.MovingToWrist:
                MoveTowardsWrist();
                break;
            case State.Orbiting:
                OrbitAroundWrist();
                break;
        }
    }

    void CheckWristProximity()
    {
        if (wrist == null || movingObject == null)
            return;

        // Calculate the distance between the moving object and the wrist
        float distance = Vector3.Distance(movingObject.position, wrist.position);

        if (distance < thresholdDistance)
        {
            // Start moving towards the wrist
            currentState = State.MovingToWrist;
        }
    }

    void MoveTowardsWrist()
    {
        // Move the object towards the wrist at the specified speed
        movingObject.position = Vector3.MoveTowards(movingObject.position, wrist.position, movementSpeed * Time.deltaTime);

        // Check if the object has reached the wrist
        if (Vector3.Distance(movingObject.position, wrist.position) < 0.001f)
        {
            // Switch to orbiting state
            currentState = State.Orbiting;
            // Reset the current angle
            currentAngle = initialAngle;
        }
    }

    void OrbitAroundWrist()
    {
        if (wrist == null || movingObject == null)
            return;

        // Update the angle based on the orbit speed and time
        currentAngle += orbitSpeed * Time.deltaTime;

        // Keep the angle between 0 and 360 degrees
        currentAngle %= 360.0f;

        // Calculate the new position around the wrist
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * orbitRadius;

        // Set the position of the orbiting object relative to the wrist
        movingObject.position = wrist.position + offset;
    }

    // Optional: Reset the object to idle state (call this method as needed)
    public void ResetInteraction()
    {
        currentState = State.Idle;
    }
}
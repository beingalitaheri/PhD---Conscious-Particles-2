using UnityEngine;
using System.Collections;
public class MotherAgentController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform centerTransform; // The central transform from which points are selected
    public float minRadius = 5f;      // Minimum radius
    public float maxRadius = 15f;     // Maximum radius

    [Header("Movement Settings")]
    public float speed = 3f;              // Speed of the object's movement
    public float reachThreshold = 0.1f;   // Distance threshold to consider the target as reached

    // New Settings for Proximity-Based Behavior
    [Header("Proximity Settings")]
    public Transform specificTransform;    // The specific Transform to monitor
    public float thresholdDistance = 5f;  // Distance to trigger behavior change
    public Transform[] targetTransforms;   // Array of Transforms to move towards when triggered

    private Vector3 targetPosition;
    private Transform currentTarget;        // Current movement target
    private bool isTriggered = false;       // Flag to check if proximity condition is met

    void Start()
    {
        if (centerTransform == null)
        {
            centerTransform = this.transform; // If the central transform is not set, use the object's own transform
        }

        SetNewTarget();
    }

    void Update()
    {
        // Check proximity and potentially switch target
        CheckProximity();

        MoveTowardsTarget();
    }

    void SetNewTarget()
    {
        float randomRadius = Random.Range(minRadius, maxRadius);
        // Select a random angle
        float randomAngle = Random.Range(0f, Mathf.PI * 2);

        // Calculate the target position using the random radius and angle
        float x = centerTransform.position.x + randomRadius * Mathf.Cos(randomAngle);
        float z = centerTransform.position.z + randomRadius * Mathf.Sin(randomAngle);
        float y = centerTransform.position.y + Random.Range(-10f, 10f);

        targetPosition = new Vector3(x, y, z);
        currentTarget = null; // Reset current target to default behavior
    }

    void MoveTowardsTarget()
    {
        Vector3 destination;

        if (isTriggered && currentTarget != null)
        {
            // Move towards the currentTarget from targetTransforms
            destination = currentTarget.position;
        }
        else
        {
            // Default random movement
            destination = targetPosition;
        }

        // Calculate the direction of movement
        Vector3 direction = destination - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= reachThreshold)
        {
            if (isTriggered)
            {
                // Reached the triggered target, reset to default behavior
                isTriggered = false;
                SetNewTarget();
            }
            else
            {
                // Reached the random target, set a new random target
                SetNewTarget();
            }
        }
        else
        {
            // Move towards the destination
            Vector3 move = direction.normalized * distanceThisFrame;
            // Ensure the object does not overshoot the target
            if (move.magnitude > direction.magnitude)
            {
                move = direction;
            }
            transform.Translate(move, Space.World);
        }
    }

    /// <summary>
    /// Checks the distance to the specificTransform and updates the currentTarget accordingly.
    /// </summary>
    void CheckProximity()
    {
        if (specificTransform == null || targetTransforms == null || targetTransforms.Length == 0)
            return;

        // Calculate the distance between the specific Transform and the current object
        float distance = Vector3.Distance(transform.position, specificTransform.position);

        if (distance < thresholdDistance && !isTriggered)
        {
            // Proximity condition met, find the nearest Transform from the targetTransforms array
            Transform nearest = FindNearestTransform();

            if (nearest != null)
            {
                currentTarget = nearest;
                isTriggered = true;
            }
        }
        else if (distance >= thresholdDistance && isTriggered)
        {
            // Proximity condition no longer met, revert to default behavior
            isTriggered = false;
            SetNewTarget();
        }
    }

    /// <summary>
    /// Finds the nearest Transform from the targetTransforms array based on distance.
    /// </summary>
    /// <returns>The nearest Transform.</returns>
    Transform FindNearestTransform()
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform t in targetTransforms)
        {
            if (t == null)
                continue;

            float dist = Vector3.Distance(currentPosition, t.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = t;
            }
        }

        return nearest;
    }

    // (Optional) Display the targets and radii in the editor for easier visualization
    void OnDrawGizmosSelected()
    {
        if (centerTransform != null)
        {
            // Draw min and max radius spheres
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(centerTransform.position, minRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(centerTransform.position, maxRadius);

            // Draw the current target position
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(targetPosition, 0.3f);
        }

        if (specificTransform != null)
        {
            // Draw the threshold distance sphere around the specificTransform
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(specificTransform.position, thresholdDistance);
        }

        if (isTriggered && currentTarget != null)
        {
            // Draw a line to the currentTarget
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, currentTarget.position);
            Gizmos.DrawSphere(currentTarget.position, 0.3f);
        }
    }
}

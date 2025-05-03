using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using MBaske.Sensors.Grid;
using System.Collections.Generic;
using System.Linq;

public class JetAgent_Child_3 : Agent
{
    [SerializeField] private GridSensorComponent3D sensorComponent;
    public float moveSpeed = 10f;
    public float rotateSpeed = 100f;
    public float targetFollowAngle = 45f;
    public float targetFollowDistance = 100f;
    public float maxAllowedDistance = 65f;
    public float generateDistanceEnviro = 6f;
    private Rigidbody rb;

    // Reference to the TargetManager
    public Target_Manager targetManager;

    private GameObject currentTarget;
    private GameObject previousTarget; // To track the previous target

    // Reward and penalty parameters
    public float rewardCloseToTarget = 5.0f; // Reward for being close to target
    public float penaltyTooFar = -0.1f; // Penalty for being too far from target
    public float penaltyOutOfArea = -3.0f; // Penalty for going out of area
    public float targetRadius = 1.0f; // Radius within which agent gets positive reward
    public float detectionRange = 10f; // Range to detect new targets

    public bool isCollide = true;
    public bool updatePose = true;

    public GameObject agentMother_Transform; // Reference to the environment center

    // LayerMask for walls
    [SerializeField] private LayerMask wallLayerMask;

    // Flag to track if the agent has reached the current target
    private bool hasReachedTarget = false;

    // To track the previous distance to the target for shaping rewards
    private float previousDistanceToTarget = 0f;

    // حداکثر تعداد اهداف شناسایی شده
    public int maxDetectedObjects = 10;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure GridSensorComponent3D is assigned
        if (sensorComponent == null)
        {
            sensorComponent = GetComponent<GridSensorComponent3D>();
            if (sensorComponent == null)
            {
                Debug.LogError("GridSensorComponent3D is not assigned.");
            }
        }

        // Ensure TargetManager is assigned
        if (targetManager == null)
        {
            targetManager = GetComponent<Target_Manager>();
            if (targetManager == null)
            {
                Debug.LogError("TargetManager is not assigned.");
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        if (updatePose)
        {
            UpdatePosition();
        }

        // Reset velocities and rotation
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;

        // Reset the flag and previous distance
        hasReachedTarget = false;
        previousDistanceToTarget = float.MaxValue;

        // Select the initial target
        SelectCurrentTarget();

        // Initialize previousDistanceToTarget
        if (currentTarget != null)
        {
            previousDistanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        }
        else
        {
            previousDistanceToTarget = 0f;
        }
    }

    /// <summary>
    /// Updates the agent's position to a random location within the spawn range.
    /// </summary>
    private void UpdatePosition()
    {
        // تعریف حداکثر و حداقل فاصله از مرکز محیط
        float minDistance = generateDistanceEnviro;
        float maxDistance = 2 * generateDistanceEnviro;

        // انتخاب جهت تصادفی با نرمال‌سازی
        Vector3 randomDirection = Random.insideUnitSphere.normalized;

        // انتخاب فاصله تصادفی در بازه مشخص شده
        float randomDistance = Random.Range(minDistance, maxDistance);

        // تعیین موقعیت جدید
        Vector3 newPosition = agentMother_Transform.transform.position + randomDirection * randomDistance;

        // تنظیم موقعیت جدید
        transform.position = newPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 pos = transform.position;

        // اضافه کردن موقعیت و سرعت عامل
        sensor.AddObservation(pos);
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(agentMother_Transform.transform.position);
        // استفاده از sensorComponent برای شناسایی اهداف
        var detectedColliders = sensorComponent.GetDetectedGameObjects(targetManager.TargetTag);
        // Debug.Log("Detected Colliders Count: " + detectedColliders.Count);

        // تبدیل Colliders به GameObjects
        List<GameObject> detectedTargets = new List<GameObject>();
        foreach (var collider in detectedColliders)
        {
            if (collider != null && collider.gameObject != null)
            {
                detectedTargets.Add(collider.gameObject);
                // Debug.Log("Detected Target Position: " + collider.gameObject.transform.position);
            }
            else
            {
                Debug.LogError("Detected object is not a valid GameObject.");
            }
        }

        // پیدا کردن نزدیک‌ترین هدف
        GameObject nearestTarget = null;
        float minDistance = float.MaxValue;

        foreach (var target in detectedTargets)
        {
            float distance = Vector3.Distance(pos, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = target;
            }
        }

        currentTarget = nearestTarget;

        // اضافه کردن اطلاعات نزدیک‌ترین هدف
        if (currentTarget != null)
        {
            Vector3 delta = currentTarget.transform.position - pos;
            sensor.AddObservation(delta.normalized); // جهت به سمت هدف
            sensor.AddObservation(delta.magnitude); // فاصله به هدف
        }
        else
        {
            // اگر هیچ هدفی شناسایی نشده است، پر کردن با مقادیر پیش‌فرض
            sensor.AddObservation(Vector3.zero); // جهت پیش‌فرض
            sensor.AddObservation(0f); // فاصله پیش‌فرض
        }

        // اگر تعداد اهداف کمتر از حد مورد انتظار است، پر کردن با مقادیر پیش‌فرض
        for (int i = 1; i < maxDetectedObjects; i++) // شروع از ۱ چون نزدیک‌ترین هدف را اضافه کردیم
        {
            sensor.AddObservation(Vector3.zero); // جهت پیش‌فرض
            sensor.AddObservation(0f); // فاصله پیش‌فرض
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // پردازش اکشن‌ها
        ProcessActions(actionBuffers.ContinuousActions);
        float distance2 = Vector3.Distance(transform.position, agentMother_Transform.transform.position);

        if (distance2 > maxAllowedDistance)
        {
            AddReward(penaltyOutOfArea);
            Debug.LogWarning("Penalty: Out of area. Enviro");

            EndEpisode();
            return;
        }
        // بررسی وضعیت هدف فعلی
        CheckTargets();

        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

            // بررسی خروج از محدوده مجاز
            if (distance > maxAllowedDistance)
            {
                AddReward(penaltyOutOfArea);
#if UNITY_EDITOR
                Debug.LogWarning("Penalty: Out of area.");
#endif
                EndEpisode();
                return;
            }

            // محاسبه تغییر فاصله برای شکل‌دهی پاداش
            float distanceChange = previousDistanceToTarget - distance;
            previousDistanceToTarget = distance;

            // پاداش یا تنبیه بر اساس تغییر فاصله
            if (distanceChange > 0)
            {
                AddReward(distanceChange * 0.1f); // پاداش برای نزدیک شدن
            }
            else
            {
                AddReward(distanceChange * 0.1f); // تنبیه برای دور شدن
            }

            // پاداش منفی برای فاصله زیاد
            if (distance > targetFollowDistance)
            {
                AddReward(penaltyTooFar);
            }

            // تشویق به رسیدن دقیق به هدف
            if (distance < targetRadius)
            {
                string targetTag = currentTarget.tag;

                if (targetTag == "NormalTarget")
                {
                    AddReward(rewardCloseToTarget);
                }
                else if (targetTag == "SpecialTarget")
                {
                    AddReward(rewardCloseToTarget * 2); // پاداش بیشتر برای اهداف ویژه
                }

#if UNITY_EDITOR
                Debug.LogWarning("Reward: Reached the target.");
#endif
                previousTarget = currentTarget;
                hasReachedTarget = true;
                SelectCurrentTarget();
            }
        }
        else
        {
            // اگر هیچ هدفی انتخاب نشده است، تنبیه کوچک
            AddReward(penaltyTooFar);
            //AddReward(penaltyNoTarget);
        }

        // تشخیص و مدیریت اهداف جدید
        DetectNewTargets();
    }

    /// <summary>
    /// Processes continuous actions received from the policy.
    /// </summary>
    /// <param name="actions">The continuous actions.</param>
    private void ProcessActions(ActionSegment<float> actions)
    {
        float moveX = actions[0];
        float moveY = actions[1];
        float moveZ = actions[2];
        Vector3 movement = new Vector3(moveX, moveY, moveZ) * moveSpeed * Time.deltaTime;
        rb.AddForce(movement, ForceMode.VelocityChange);

        // Clamp the agent's velocity to prevent excessive speed
        float maxSpeed = 20f;
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // پاداش کوچک برای فعال ماندن
        AddReward(-0.001f); // کاهش مقدار پاداش برای جلوگیری از حرکت بی‌هدف
    }

    /// <summary>
    /// Checks the status of the current target and assigns rewards or penalties accordingly.
    /// </summary>
    private void CheckTargets()
    {
        if (currentTarget != null)
        {
            Vector3 pos = transform.position;
            Vector3 delta = currentTarget.transform.position - pos;
            float distance = delta.magnitude;

            // Reward for reaching the target
            if (distance < targetRadius)
            {
                string targetTag = currentTarget.tag;

                if (targetTag == "NormalTarget")
                {
                    AddReward(rewardCloseToTarget);
                }
                else if (targetTag == "SpecialTarget")
                {
                    AddReward(rewardCloseToTarget * 2); // پاداش بیشتر برای اهداف ویژه
                }

#if UNITY_EDITOR
                Debug.LogWarning("Reward: Reached the target.");
#endif
                // Optionally deactivate the target or reset its position
                // currentTarget.SetActive(false); // اگر نیاز است
                previousTarget = currentTarget;
                hasReachedTarget = true;
                SelectCurrentTarget();
            }
        }
    }

    /// <summary>
    /// Selects the closest active target as the current target.
    /// </summary>
    private void SelectCurrentTarget()
    {
        if (targetManager.Targets.Count > 0)
        {
            // Select the closest active target
            currentTarget = targetManager.GetClosestActiveTarget(transform.position);
        }
        else
        {
            currentTarget = null;
        }
    }

    /// <summary>
    /// Detects new targets within the detection range and handles target switching with rewards and penalties.
    /// </summary>
    private void DetectNewTargets()
    {
        if (currentTarget == null)
            return;

        // Only allow switching if the current target has been reached
        if (!hasReachedTarget)
            return;

        // Find all active targets within the detection range
        var nearbyTargets = targetManager.GetNearbyTargets(transform.position, detectionRange);

        if (nearbyTargets.Count > 1) // More than one target in range
        {
            // Select the closest new target different from the current and previous targets
            var closestNewTarget = nearbyTargets.OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
                                                 .FirstOrDefault(t => t != currentTarget && t != previousTarget);

            if (closestNewTarget != null)
            {
                // Switch to the new target
                SwitchToNewTarget(closestNewTarget);
            }
        }
    }

    /// <summary>
    /// Switches to a new target and applies a higher reward.
    /// </summary>
    /// <param name="newTarget">The new target to switch to.</param>
    private void SwitchToNewTarget(GameObject newTarget)
    {
        previousTarget = currentTarget;
        currentTarget = newTarget;
        AddReward(0.5f);
#if UNITY_EDITOR
        Debug.LogWarning("Reward: Switched to a new target.");
#endif
        hasReachedTarget = false; // Reset the flag for the new target
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isCollide)
        {
            // بررسی برخورد با دیوار با استفاده از لایه
            if (((1 << collision.gameObject.layer) & wallLayerMask) != 0)
            {
                AddReward(penaltyOutOfArea);
#if UNITY_EDITOR
                Debug.LogWarning("Penalty: Collided with a wall.");
#endif
                EndEpisode();
            }

            // بررسی برخورد با هدف
            if (collision.gameObject == currentTarget)
            {
                string targetTag = currentTarget.tag;

                if (targetTag == "NormalTarget")
                {
                    AddReward(rewardCloseToTarget);
                }
                else if (targetTag == "SpecialTarget")
                {
                    AddReward(rewardCloseToTarget * 2); // پاداش بیشتر برای اهداف ویژه
                }

#if UNITY_EDITOR
                Debug.LogWarning("Reward: Collided with a Target.");
#endif
                previousTarget = currentTarget;
                hasReachedTarget = true;
                SelectCurrentTarget();
            }
        }
    }

    /// <summary>
    /// Defines the heuristic (manual control) for testing the agent.
    /// </summary>
    /// <param name="actionsOut">The action buffers to output actions.</param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal"); // 
        continuousActionsOut[1] = Input.GetAxis("Vertical");   // 
        continuousActionsOut[2] = Input.GetAxis("Depth");      // 

    }

    void OnDrawGizmos()
    {
        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentTarget.transform.position, targetRadius);
        }
    }
}

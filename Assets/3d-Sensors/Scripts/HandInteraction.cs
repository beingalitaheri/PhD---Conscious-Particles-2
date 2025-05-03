using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandInteraction : MonoBehaviour
{
    // Arrays of Transforms for each finger's joints (including fingertip)
    public Transform[] thumbJoints;
    public Transform[] indexJoints;
    public Transform[] middleJoints;
    public Transform[] ringJoints;
    public Transform[] pinkyJoints;

    // Wrist Transform
    public Transform wrist;

    // Speed at which the object moves towards the target joint
    public float movementSpeed = 1.0f;

    // Distance threshold to detect proximity between any finger joint and the object
    public float threshold = 0.02f;

    // The object that will move towards the hand joints
    public Transform movingObject;

    // Flag to prevent multiple coroutines from running simultaneously
    private bool isMoving = false;

    void Update()
    {
        if (!isMoving)
        {
            CheckFingerJointsProximity();
        }
    }

    void CheckFingerJointsProximity()
    {
        // List of all finger joints
        Transform[][] allFingerJoints = new Transform[][]
        {
            thumbJoints,
            indexJoints,
            middleJoints,
            ringJoints,
            pinkyJoints
        };

        // Iterate through all finger joints
        for (int fingerIndex = 0; fingerIndex < allFingerJoints.Length; fingerIndex++)
        {
            Transform[] fingerJoints = allFingerJoints[fingerIndex];

            foreach (Transform joint in fingerJoints)
            {
                float distance = Vector3.Distance(movingObject.position, joint.position);

                if (distance < threshold)
                {
                    // Start moving along the finger towards the wrist
                    StartCoroutine(MoveAlongFingerToWrist(fingerJoints));
                    return; // Exit once a close joint is found
                }
            }
        }
    }

    IEnumerator MoveAlongFingerToWrist(Transform[] fingerJoints)
    {
        isMoving = true;

        // List of points to move through from the current joint to the wrist
        List<Transform> pointsToMove = new List<Transform>(fingerJoints);

        // Add the wrist as the final target
        if (wrist != null)
        {
            pointsToMove.Add(wrist);
        }

        // Move the object along the joints towards the wrist
        foreach (Transform target in pointsToMove)
        {
            // Optional: Perform an action at each joint (e.g., change color)
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }

            while (Vector3.Distance(movingObject.position, target.position) > 0.001f)
            {
                movingObject.position = Vector3.MoveTowards(movingObject.position, target.position, movementSpeed * Time.deltaTime);
                yield return null;
            }
        }

        isMoving = false;
    }
}

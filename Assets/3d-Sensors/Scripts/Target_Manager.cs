using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Target_Manager : MonoBehaviour
{

    public List<GameObject> Targets = new List<GameObject>();

    public string targetTag = "NormalTarget";
    public string TargetTag => targetTag;

    public void AddTarget(GameObject target)
    {
        if (!Targets.Contains(target))
        {
            Targets.Add(target);
        }
    }

    public void RemoveTarget(GameObject target)
    {
        if (Targets.Contains(target))
        {
            Targets.Remove(target);
        }
    }

    public GameObject GetClosestActiveTarget(Vector3 currentPosition)
    {
        return Targets
            .Where(t => t.activeSelf)
            .OrderBy(t => Vector3.Distance(currentPosition, t.transform.position))
            .FirstOrDefault();
    }

    public List<GameObject> GetNearbyTargets(Vector3 currentPosition, float range)
    {
        return Targets
            .Where(t => t.activeSelf && Vector3.Distance(currentPosition, t.transform.position) <= range)
            .ToList();
    }
}
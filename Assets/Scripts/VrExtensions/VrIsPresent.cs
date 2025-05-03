using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace VrExtensions
{
    internal static class VrIsPresent
    {
        public static bool isPresent()
        {
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
            foreach (var xrDisplay in xrDisplaySubsystems)
            {
                if (xrDisplay.running)
                {
                    return true;
                }
            }
            return false;
        }
    }

}

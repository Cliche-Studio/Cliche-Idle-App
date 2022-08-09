using System;
using UnityEngine;

namespace Cliche.System
{
    public partial class Manifests
    {        
        public static T GetByID<T>(string manifestID) where T : ScriptableObject
        {
            string typeLocationPath = Paths[typeof(T)];
            T manifest = Resources.Load<T>($"{typeLocationPath}/{manifestID}");
            if (manifest == null)
            {
                manifest = Resources.Load<T>($"{typeLocationPath}/{manifestID}/{manifestID}");
            }
            if (manifest == null)
            {
                Debug.LogError($"Manifest<{typeof(T)}> could not be found at neither path:\nRESOURCES://{typeLocationPath}/{manifestID} \nRESOURCES://{typeLocationPath}/{manifestID}/{manifestID}.");
            }
            return manifest;
        }

        /*
        public static object GetByObject(Item item)
        {
            Type manifestType;
            string typeLocationPath = Paths[typeof(T)];
            T manifest = Resources.Load<T>($"{typeLocationPath}/{manifestID}");
            if (manifest == null)
            {
                manifest = Resources.Load<T>($"{typeLocationPath}/{manifestID}/{manifestID}");
            }
            if (manifest == null)
            {
                Debug.LogError($"Manifest<{typeof(T)}> could not be found at neither path:\nRESOURCES://{typeLocationPath}/{manifestID} \nRESOURCES://{typeLocationPath}/{manifestID}/{manifestID}.");
            }
            return manifest;
        }
        */
    }
}
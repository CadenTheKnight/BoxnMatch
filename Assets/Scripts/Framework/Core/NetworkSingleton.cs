using UnityEngine;
using Unity.Netcode;

namespace Assets.Scripts.Framework.Core
{
    /// <summary>
    /// Base class for all singleton classes that require network functionality.
    /// </summary>
    public abstract class NetworkSingleton<T> : NetworkBehaviour where T : Component
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
                Destroy(gameObject);
        }
    }
}
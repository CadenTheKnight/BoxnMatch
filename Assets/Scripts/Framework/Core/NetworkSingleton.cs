using UnityEngine;
using Unity.Netcode;

namespace Assets.Scripts.Framework.Core
{
    /// <summary>
    /// Base class for networked singleton classes.
    /// </summary>
    public abstract class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        private static T _instance;

        [SerializeField] private bool dontDestroyOnLoad = true;
        [SerializeField] private bool logDebugInfo = true;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;

                if (dontDestroyOnLoad)
                {
                    if (transform.parent != null)
                        transform.SetParent(null);
                    DontDestroyOnLoad(gameObject);
                }

                if (logDebugInfo)
                    Debug.Log($"[NetworkSingleton] {typeof(T).Name} instance initialized");
            }
            else if (_instance != this)
            {
                if (logDebugInfo)
                    Debug.LogWarning($"[NetworkSingleton] Duplicate {typeof(T).Name} instance detected. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (logDebugInfo)
                Debug.Log($"[NetworkSingleton] {typeof(T).Name} OnNetworkSpawn - IsServer: {IsServer}, IsClient: {IsClient}");
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                        Debug.LogWarning($"[NetworkSingleton] No instance of {typeof(T).Name} found. NetworkSingleton objects should be placed in the scene.");
                }

                return _instance;
            }
        }

        /// <summary>
        /// Whether this singleton exists
        /// </summary>
        public static bool Exists => _instance != null;
    }
}
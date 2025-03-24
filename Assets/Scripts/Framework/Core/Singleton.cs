using UnityEngine;

namespace Assets.Scripts.Framework.Core
{
    /// <summary>
    /// Base class for all singleton classes.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        [SerializeField] private bool logDebugInfo = true;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;

                if (transform.parent != null)
                    transform.SetParent(null);
                DontDestroyOnLoad(gameObject);

                if (logDebugInfo)
                    Debug.Log($"[Singleton] {typeof(T).Name} instance initialized");
            }
            else if (_instance != this)
            {
                if (logDebugInfo)
                    Debug.LogWarning($"[Singleton] Duplicate {typeof(T).Name} instance detected. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        T existingInstance = FindObjectOfType<T>(true);
                        if (existingInstance != null)
                        {
                            _instance = existingInstance;
                            existingInstance.gameObject.SetActive(true);
                            Debug.Log($"[Singleton] Found inactive {typeof(T).Name} instance and activated it");
                            return _instance;
                        }

                        T[] instances = FindObjectsOfType<T>();
                        if (instances.Length == 0 && _instance is Singleton<T>)
                        {
                            GameObject obj = new(typeof(T).Name);
                            _instance = obj.AddComponent<T>();
                            Debug.Log($"[Singleton] Created new {typeof(T).Name} instance");
                        }
                        else
                        {
                            Debug.LogWarning($"[Singleton] No instance of {typeof(T).Name} found in scene");
                        }
                    }
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
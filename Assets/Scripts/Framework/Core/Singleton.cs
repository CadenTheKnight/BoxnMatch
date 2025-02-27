using UnityEngine;

namespace Assets.Scripts.Framework.Core
{
    /// <summary>
    /// Base class for all singleton classes.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        /// <summary>
        /// The singleton instance of this class.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        GameObject obj = new();
                        _instance = obj.AddComponent<T>();
                        obj.name = typeof(T).Name;
                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Ensures that only one instance of this singleton exists.
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate {typeof(T)} instance destroyed");
                Destroy(gameObject);
            }
        }
    }
}
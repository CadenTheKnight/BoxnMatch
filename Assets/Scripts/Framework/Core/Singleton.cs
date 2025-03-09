using UnityEngine;

namespace Assets.Scripts.Framework.Core
{
    /// <summary>
    /// Base class for all singleton classes.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Component
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
                    T[] instances = FindObjectsOfType<T>();
                    if (instances.Length > 0)
                    {
                        T instance = instances[0];
                        _instance = instance;
                    }
                    else
                    {
                        GameObject obj = new()
                        {
                            name = typeof(T).Name
                        };
                        _instance = obj.AddComponent<T>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }
    }
}
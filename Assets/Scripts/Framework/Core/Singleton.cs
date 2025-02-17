using UnityEngine;

namespace Assets.Scripts.Framework.Core
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    T[] objects = FindObjectsOfType<T>();
                    if (objects.Length > 0)
                    {
                        T instance = objects[0];
                        _instance = instance;
                    }
                    else
                    {
                        GameObject go = new()
                        {
                            name = typeof(T).Name
                        };
                        _instance = go.AddComponent<T>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
    }
}
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JCCode
{
    namespace ObjectPool
    {
        public sealed class ObjectPool : MonoBehaviour
        {
            [System.Serializable]
            public class StartUpPool
            {
                public int count;
                public GameObject prefab;
            }
            // all objectpool 
            private Dictionary<GameObject, GameObject> _spawnedObjects = new Dictionary<GameObject, GameObject>();
            // objectpool spawn object , add this ditionary when recyle remove object
            private Dictionary<GameObject, List<GameObject>> _pooledObjects = new Dictionary<GameObject, List<GameObject>>();

            // spawn same (count) prefab ->gameobject in game start
            public StartUpPool[] startUpPools;

            private static ObjectPool _instance;

            public static ObjectPool instance
            {
                get
                {
                    if (_instance == null)
                        _instance = GameObject.FindObjectOfType<ObjectPool>();

                    if (_instance == null)
                        _instance = new GameObject("ObjectPool").AddComponent<ObjectPool>();

                    return _instance;
                }
            }

            // create strtup pool only call once
            private bool _isCreated = false;

            #region CreatePool
            /// <summary>
            /// Creates the start up pools.
            /// in game start spawn object and create objectpool
            /// </summary>
            static void CreateStartUpPools()
            {
                if (!instance._isCreated)
                {
                    instance._isCreated = true;

                    var pools = _instance.startUpPools;
                    if (pools != null)
                    {
                        if (pools.Length > 0)
                        {
                            for (int i = 0; i < pools.Length; i++)
                            {
                                CreatePool(pools[i].prefab, pools[i].count);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Creates the pool.
            /// by startuppool -> prafabs and count
            /// </summary>
            /// <param name="prefab">Prefab.</param>
            /// <param name="initialPoolCount">Initial pool count.</param>
            public static void CreatePool(GameObject prefab, int initialPoolCount)
            {
                if (prefab != null)
                {
                    if (!instance._pooledObjects.ContainsKey(prefab))
                    {
                        List<GameObject> list = new List<GameObject>();
                        _instance._pooledObjects.Add(prefab, list);

                        if (initialPoolCount > 0)
                        {
                            bool active = prefab.activeSelf;
                            prefab.SetActive(false);//set prefabs active is false 
                            Transform parent = _instance.transform;

                            GameObject obj = null;
                            do
                            {
                                obj = Object.Instantiate(prefab);
                                obj.transform.SetParent(parent);
                                list.Add(obj);
                            }
                            while (list.Count < initialPoolCount);

                            prefab.SetActive(active);//reset prefabs active 
                        }
                    }
                }
            }

            // create pool with code
            public static void CreatePool<T>(T prefab, int initialPoolCount) where T : Component
            {
                CreatePool(prefab.gameObject, initialPoolCount);
            }
            #endregion

            #region Spawn
            /// <summary>
            /// Spawn the specified prefab, parent, position and rotation.
            /// spawn object by objectpool if have object pop one or create new object and add spawnedpool
            /// </summary>
            /// <param name="prefab">Prefab.</param>
            /// <param name="parent">Parent.</param>
            /// <param name="position">Position.</param>
            /// <param name="rotation">Rotation.</param>
            public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
            {
                List<GameObject> list;
                Transform trans;
                GameObject obj;
                if (_instance._pooledObjects.TryGetValue(prefab, out list))
                {
                    obj = null;
                    if (list.Count > 0)
                    {
                        obj = list[0];
                        list.RemoveAt(0);

                        if (obj != null)
                        {
                            trans = obj.transform;
                            trans.SetParent(parent);
                            trans.localPosition = position;
                            trans.localRotation = rotation;
                            obj.SetActive(true);
                            _instance._spawnedObjects.Add(obj, prefab);
                            return obj;
                        }
                    }
                    obj = Object.Instantiate(prefab) as GameObject;
                    trans = obj.transform;
                    trans.SetParent(parent);
                    trans.localPosition = position;
                    trans.localRotation = rotation;
                    obj.SetActive(true);
                    _instance._spawnedObjects.Add(obj, prefab);
                    return obj;
                }
                else
                {
                    obj = Object.Instantiate(prefab) as GameObject;
                    trans = obj.transform;
                    trans.SetParent(parent);
                    trans.localPosition = position;
                    trans.localRotation = rotation;
                    obj.SetActive(true);
                    return obj;
                }
            }
            public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position)
            {
                return Spawn(prefab, parent, position, Quaternion.identity);
            }
            public static GameObject Spawn(GameObject prefab, Transform parent, Quaternion rotation)
            {
                return Spawn(prefab, parent, Vector3.zero, rotation);
            }
            public static GameObject Spawn(GameObject prefab, Transform parent)
            {
                return Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
            }
            public static GameObject Spawn(GameObject prefab, Vector3 position)
            {
                return Spawn(prefab, null, position, Quaternion.identity);
            }
            public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
            {
                return Spawn(prefab, null, position, rotation);
            }
            public static GameObject Spawn(GameObject prefab)
            {
                return Spawn(prefab, null, Vector3.zero, Quaternion.identity);
            }

            public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
            {
                return Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
            }
            public static T Spawn<T>(T prefab, Transform parent, Vector3 position) where T : Component
            {
                return Spawn(prefab.gameObject, parent, position).GetComponent<T>();
            }
            public static T Spawn<T>(T prefab, Transform parent, Quaternion rotation) where T : Component
            {
                return Spawn(prefab.gameObject, parent, rotation).GetComponent<T>();
            }
            public static T Spawn<T>(T prefab, Transform parent) where T : Component
            {
                return Spawn(prefab.gameObject, parent).GetComponent<T>();
            }
            public static T Spawn<T>(T prefab, Vector3 position) where T : Component
            {
                return Spawn(prefab.gameObject, position).GetComponent<T>();
            }
            public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
            {
                return Spawn(prefab.gameObject, position, rotation).GetComponent<T>();
            }
            public static T Spawn<T>(T prefab) where T : Component
            {
                return Spawn(prefab).GetComponent<T>();
            }
            #endregion

            #region Recycle
            /// <summary>
            /// Recycle the specified obj and prefab.
            /// add to pooledobject dictionary
            /// remove from spawnedobject dictionary
            /// </summary>
            /// <param name="obj">Object.</param>
            /// <param name="prefab">Prefab.</param>
            static void Recycle(GameObject obj, GameObject prefab)
            {
                _instance._pooledObjects[prefab].Add(obj);
                _instance._spawnedObjects.Remove(obj);

                obj.transform.SetParent(_instance.transform);
                obj.SetActive(false);
            }
            /// <summary>
            /// Recycle the specified obj.
            /// Main Recycle 
            /// </summary>
            /// <param name="obj">Object.</param>
            public static void Recycle(GameObject obj)
            {
                GameObject prefab;
                if (_instance._spawnedObjects.TryGetValue(obj, out prefab))
                {
                    Recycle(obj, prefab);
                }
                else
                {
                    Object.Destroy(obj);
                }
            }

            public static void RecycleAll(GameObject prefab)
            {
                List<GameObject> tempList = new List<GameObject>();

                foreach (var item in _instance._spawnedObjects)
                {
                    if (item.Value.Equals(prefab))
                    {
                        tempList.Add(item.Key);
                    }
                }
                for (int i = 0; i < tempList.Count; i++)
                {
                    Recycle(tempList[i]);
                }
                tempList.Clear();
            }

            public static void RecycleAll()
            {
                List<GameObject> tempList = new List<GameObject>();

                tempList.AddRange(_instance._spawnedObjects.Keys);
                for (int i = 0; i < tempList.Count; i++)
                {
                    Recycle(tempList[i]);
                }
                tempList.Clear();
            }

            public static void Recycle<T>(T obj) where T : Component
            {
                Recycle(obj.gameObject);
            }

            public static void RecycleAll<T>(T prefab) where T : Component
            {
                RecycleAll(prefab.gameObject);
            }


            #endregion

            #region ObjectPool Status

            public static int CountInPooled(GameObject prefab)
            {
                List<GameObject> list;
                if (_instance._pooledObjects.TryGetValue(prefab, out list))
                    return list.Count;
                return 0;
            }

            public static int CountInPooled<T>(T prefab) where T : Component
            {
                return CountInPooled(prefab.gameObject);
            }

            public static List<GameObject> GetObjectPool(GameObject prefab)
            {
                List<GameObject> list;
                _instance._pooledObjects.TryGetValue(prefab, out list);
                return list;
            }

            public static List<T> GetObjectPool<T>(T prefab) where T : Component
            {
                List<GameObject> list;
                _instance._pooledObjects.TryGetValue(prefab.gameObject, out list);
                List<T> listT = new List<T>();
                for (int i = 0; i < list.Count; i++)
                    listT.Add(list[i].GetComponent<T>());
                return listT;
            }

            public static bool HasObject(GameObject obj)
            {
                return _instance._pooledObjects.ContainsKey(obj);
            }

            public static bool HasObject<T>(T obj) where T : Component
            {
                return HasObject(obj.gameObject);
            }

            #endregion

            #region Destroy
            /// <summary>
            /// Destroies the pool.
            /// Only Destroy PooledObject
            /// And Remove Key from Dictionary
            /// </summary>
            /// <param name="prefab">Prefab.</param>
            public static void DestroyPool(GameObject prefab)
            {
                List<GameObject> pooled;
                if (instance._pooledObjects.TryGetValue(prefab, out pooled))
                {
                    for (int i = 0; i < pooled.Count; i++)
                        GameObject.Destroy(pooled[i]);
                    pooled.Clear();

                    instance._pooledObjects.Remove(prefab);
                }
            }

            /// <summary>
            /// Destroies the pool.
            /// Only Destroy PooledObject
            /// </summary>
            /// <param name="prefab">Prefab.</param>
            /// <typeparam name="T">The 1st type parameter.</typeparam>
            public static void DestroyPool<T>(T prefab) where T : Component
            {
                DestroyPool(prefab.gameObject);
            }

            public static void DestroyAllPool(GameObject prefab)
            {
                RecycleAll(prefab);
                DestroyPool(prefab);
            }

            public static void DestroyAllPool<T>(T prefab) where T : Component
            {
                DestroyAllPool(prefab.gameObject);
            }

            #endregion

            #region System
            void Awake()
            {
                if (_instance == null)
                {
                    _instance = this;
                    CreateStartUpPools();
                }
            }
            #endregion

        }


        public static class ObjectPoolExtension
        {
            #region CreatePool
            public static void CreatePool<T>(this T prefab) where T : Component
            {
                ObjectPool.CreatePool(prefab, 0);
            }

            public static void CreatePool<T>(this T prefab, int initialCount) where T : Component
            {
                ObjectPool.CreatePool(prefab, initialCount);
            }

            public static void CreatePool(this GameObject prefab)
            {
                ObjectPool.CreatePool(prefab, 0);
            }
            public static void CreatePool(this GameObject prefab, int initialCount)
            {
                ObjectPool.CreatePool(prefab, initialCount);
            }
            #endregion

            #region Spawn
            public static T Spawn<T>(this T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
            {
                return ObjectPool.Spawn(prefab, parent, position, rotation);
            }
            public static T Spawn<T>(this T prefab, Vector3 position) where T : Component
            {
                return ObjectPool.Spawn(prefab, null, position, Quaternion.identity);
            }
            public static T Spawn<T>(this T prefab, Transform parent, Vector3 position) where T : Component
            {
                return ObjectPool.Spawn(prefab, parent, position);
            }
            public static T Spawn<T>(this T prefab, Transform parent) where T : Component
            {
                return ObjectPool.Spawn(prefab, parent);
            }
            public static T Spawn<T>(this T prefab) where T : Component
            {
                return ObjectPool.Spawn(prefab);
            }
            public static T Spawn<T>(this T prefab, Transform parent, Quaternion rotation) where T : Component
            {
                return ObjectPool.Spawn(prefab, parent, rotation);
            }
            public static T Spawn<T>(this T prefab, Vector3 position, Quaternion rotation) where T : Component
            {
                return ObjectPool.Spawn(prefab, position, rotation);
            }

            public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
            {
                return ObjectPool.Spawn(prefab, parent, position, rotation);
            }
            public static GameObject Spawn(this GameObject prefab, Vector3 position)
            {
                return ObjectPool.Spawn(prefab, null, position, Quaternion.identity);
            }
            public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position)
            {
                return ObjectPool.Spawn(prefab, parent, position);
            }
            public static GameObject Spawn(this GameObject prefab, Transform parent)
            {
                return ObjectPool.Spawn(prefab, parent);
            }
            public static GameObject Spawn(this GameObject prefab)
            {
                return ObjectPool.Spawn(prefab);
            }
            public static GameObject Spawn(this GameObject prefab, Transform parent, Quaternion rotation)
            {
                return ObjectPool.Spawn(prefab, parent, rotation);
            }
            public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation)
            {
                return ObjectPool.Spawn(prefab, position, rotation);
            }

            #endregion

            #region Recycle
            public static void Recycle(this GameObject prefab)
            {
                ObjectPool.Recycle(prefab);
            }
            public static void RecycleAll(this GameObject prefab)
            {
                ObjectPool.RecycleAll(prefab);
            }

            public static void Recycle<T>(this T prefab) where T : Component
            {
                ObjectPool.Recycle(prefab);
            }
            public static void RecycleAll<T>(this T prefab) where T : Component
            {
                ObjectPool.RecycleAll(prefab);
            }
            #endregion

            #region ObjectPool Status

            public static int CountInPooled(this GameObject prefab)
            {
                return ObjectPool.CountInPooled(prefab);
            }

            public static int CountInPooled<T>(this T prefab) where T : Component
            {
                return ObjectPool.CountInPooled(prefab);
            }

            public static List<GameObject> GetObjectPool(this GameObject prefab)
            {
                return ObjectPool.GetObjectPool(prefab);
            }

            public static List<T> GetObjectPool<T>(this T prefab) where T : Component
            {
                return ObjectPool.GetObjectPool(prefab);
            }

            public static bool HasObject(this GameObject obj)
            {
                return ObjectPool.HasObject(obj);
            }

            public static bool HasObject<T>(this T obj) where T : Component
            {
                return ObjectPool.HasObject(obj);
            }

            #endregion

            #region Destroy
            public static void DestroyPool(this GameObject prefab)
            {
                ObjectPool.DestroyPool(prefab);
            }

            public static void DestroyPool<T>(this T prefab) where T : Component
            {
                ObjectPool.DestroyPool(prefab);
            }

            public static void DestroyAllPool(this GameObject prefab)
            {
                ObjectPool.DestroyAllPool(prefab);
            }

            public static void DestroyAllPool<T>(this T prefab) where T : Component
            {
                ObjectPool.DestroyAllPool(prefab);
            }
            #endregion

        }
    }

}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PoolSystem
{
    public class ObjectsPool : MonoBehaviour
    {
        //Singleton
        public static ObjectsPool Instance => _instance;
        private static ObjectsPool _instance;
        
        [Serializable]
        public class PoolItem
        {
            public GameObject gameObject;
            public int count;
        }
        
        public List<PoolItem> poolItems = new List<PoolItem>();
        public Dictionary<GameObject, List<GameObject>> poolDictionary = new Dictionary<GameObject, List<GameObject>>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
            
            //if amount variables of items are higher than 0, instantiate them
            foreach (var item in poolItems)
            {
                if (item.count > 0)
                {
                    for (int i = 0; i < item.count; i++)
                    {
                        var obj = Instantiate(item.gameObject, transform);
                        obj.SetActive(false);
                        AddToPool(item.gameObject, obj);
                    }
                }
            }
        }

        /*
        private void Start()
        {

        }
        */
        
        public GameObject GetFromPool(GameObject gameObject)
        {
            //if pooled item exists, return it
            if (poolDictionary.ContainsKey(gameObject))
            {
                foreach (var item in poolDictionary[gameObject])
                {
                    if (!item.activeInHierarchy)
                    {
                       // item.SetActive(true);
                        return item;
                    }
                }
            }
            //if not instantiate a new one and add it to the pool
            var obj = Instantiate(gameObject, transform);
           // obj.SetActive(true);
            AddToPool(gameObject, obj, 10);
            return obj;
        }

        
        public void AddToPool(GameObject gameObject, GameObject obj, int count = 1)
        {
            
            if (!poolDictionary.ContainsKey(gameObject))
            {
                //add to pool dictionary with count

                for (int i = 0; i < count; i++)
                {
                    poolDictionary.Add(gameObject, new List<GameObject>());
                }
            }
            poolDictionary[gameObject].Add(obj);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _AfterimagePool : MonoBehaviour
{
[SerializeField]
 private GameObject afterimagePrefab;

 private Queue<GameObject> availableObjects = new Queue<GameObject>();

public static _AfterimagePool Instance { get; set; }

 private void Awake()
{
     Instance = this;
     GrowPool();
 }
  private void GrowPool()
 {
  for (int i = 0; i < 10; i++)
 {
       var instanceToAdd = Instantiate(afterimagePrefab);
          instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
      }
  }
    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
        {
            GrowPool();
        }

        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}

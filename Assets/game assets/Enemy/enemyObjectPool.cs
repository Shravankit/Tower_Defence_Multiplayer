using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class enemyObjectPool : MonoBehaviour
{

    [SerializeField] GameObject enemyPool;
    [SerializeField][Range(0.1f, 50f)] int poolsize = 5;
    [SerializeField][Range(0.1f, 30f)] float timer = 2f;

    GameObject[] pool;

    void Awake()
    {
        PopulatePool();
    }

    void Start()
    {
        // StartCoroutine(EnemyObjectPool());
    }

    void PopulatePool()
    {
        pool = new GameObject[poolsize];

        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(enemyPool, transform);
            pool[i].SetActive(false);
        }
    }

    void EnabledPoolObjects()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i].activeInHierarchy == false)
            {
                pool[i].SetActive(true);
                return;
            }
        }
    }

    IEnumerator EnemyObjectPool()
    {
        while (enemyPool)
        {
            EnabledPoolObjects();
            yield return new WaitForSeconds(timer);
        }
    }

    public void StartEnemies()
    {
        StartCoroutine(EnemyObjectPool());
    }
}

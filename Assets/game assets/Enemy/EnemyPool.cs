using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] GameObject EnemyOject;
    [SerializeField] int size;
    [SerializeField] int timer;

    GameObject[] pool;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void PopulatePool()
    {
        pool = new GameObject[size];
        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(EnemyOject, transform);
            pool[i].SetActive(false);
        }
    }

    void EnablePoolObjects()
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
        while (EnemyOject)
        {
            EnablePoolObjects();
            yield return new WaitForSeconds(timer);
        }
    }
}

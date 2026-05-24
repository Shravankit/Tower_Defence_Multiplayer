using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Enemy))]
public class EnemyHealth : MonoBehaviour
{

    [SerializeField] int maxHitPoints = 5;

    [Tooltip("it will increase the enemy health by 1 after enemy dies everytime")]
    [SerializeField] int hitRamp = 1;
    [SerializeField] int currentHit = 0;

    Enemy enemy;


    void OnEnable() {
        currentHit = maxHitPoints;
    }

    private void Start() 
    {
        enemy = GetComponentInChildren<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("Enemy component not found on the same GameObject as EnemyHealth.");
        }
    }


    void OnParticleCollision(GameObject other)
    {
        ProcessHit();
    }

    private void ProcessHit()
    {
        currentHit--;
        if (currentHit <= 0)
        {
            gameObject.SetActive(false);
            maxHitPoints += hitRamp;
            enemy.GoldDeposit();
        }
    }
}

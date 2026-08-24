using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Enemy))]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHitPoints = 5;

    [Tooltip("it will increase the enemy health by 1 after enemy dies everytime")]
    [SerializeField] int hitRamp = 1;
    [SerializeField] int currentHit = 0;

    [Header("HealthBar")]
    [SerializeField] Slider healthBar;
    [SerializeField] float minHealth;
    [SerializeField] float maxHealth;

    [SerializeField] float reduceValue;

    Enemy enemy;


    void OnEnable()
    {
        currentHit = maxHitPoints;

        //health bar

        healthBar.value = maxHealth;

        reduceValue = healthBar.maxValue / maxHitPoints;
    }

    private void Start()
    {
        enemy = GetComponentInChildren<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("Enemy component not found on the same GameObject as EnemyHealth.");
        }

        //health values
        healthBar.minValue = minHealth;
        healthBar.maxValue = maxHealth;
    }


    void OnParticleCollision(GameObject other)
    {
        ProcessHit();
    }

    private void ProcessHit()
    {
        healthBar.value -= reduceValue;
        currentHit--;
        if (currentHit <= 0)
        {
            gameObject.SetActive(false);
            maxHitPoints += hitRamp;
            enemy.GoldDeposit();
        }
    }
}

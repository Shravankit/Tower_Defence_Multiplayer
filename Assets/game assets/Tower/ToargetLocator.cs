using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToargetLocator : MonoBehaviour
{

    Transform enemyTarget;
    [SerializeField] Transform weapon;
    [SerializeField] ParticleSystem towerParticles;
    [SerializeField] float towerRange = 15f;


    void Update()
    {
        AimWeapon();
        FindClosestTarget();
    }

    void FindClosestTarget()
    {
        Enemies[] enemies = FindObjectsOfType<Enemies>();
        Transform closestTarget = null;
        float maxDistance = Mathf.Infinity;

        foreach (Enemies enemy in enemies)
        {
            float targetDistance = Vector3.Distance(transform.position, enemy.transform.position);

            if(targetDistance < maxDistance)
            {
                closestTarget = enemy.transform;
                maxDistance = targetDistance;
            }
        }

        enemyTarget = closestTarget;
    }

    void AimWeapon()
    {
        if(enemyTarget != null)
        {
            float targetDistance = Vector3.Distance(transform.position, enemyTarget.position);

            weapon.LookAt(enemyTarget);

            if(targetDistance < towerRange)
            {
                Attack(true); 
            }
            else
            {
                Attack(false);
            }
        }
    }

    void Attack(bool isActive)
    {
        var ParticleEmission = towerParticles.emission;
        
        ParticleEmission.enabled = isActive;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerMultiplayer : MonoBehaviour
{
    [SerializeField] int cost = 50;

    public bool TowerInstantiate(TowerMultiplayer tower, Vector3 pos)
    {
        Banking banking = FindObjectOfType<Banking>();

        if (banking == null) return false;

        if (banking.CurrentBalance >= cost)
        {
            Instantiate(tower.gameObject, pos, Quaternion.identity);
            banking.Withdraw(cost);
            return true;
        }

        return false;
    }
}

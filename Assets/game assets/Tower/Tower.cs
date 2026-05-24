using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] int cost = 50;

    public bool TowerInstantiate(Tower tower, Vector3 position)
    {

        Banking banking = FindObjectOfType<Banking>();

        if(banking == null)
        {
            return false;
        }

        if(banking.CurrentBalance >= cost)
        {
            Instantiate(tower.gameObject, position, Quaternion.identity);
            banking.Withdraw(cost);
            return true;
        }    

        return false;   
    }
}

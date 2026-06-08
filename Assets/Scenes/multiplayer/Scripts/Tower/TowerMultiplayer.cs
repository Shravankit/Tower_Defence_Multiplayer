using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerMultiplayer : MonoBehaviour
{
    [SerializeField] int cost = 50;
    [SerializeField] TMP_Text name;

    public bool TowerInstantiate(TowerMultiplayer tower, Vector3 pos, string _name)
    {
        Banking banking = FindObjectOfType<Banking>();

        if (banking == null) return false;

        if (banking.CurrentBalance >= cost)
        {
            Instantiate(tower.gameObject, pos, Quaternion.identity);
            name.text = _name;
            banking.Withdraw(cost);
            return true;
        }

        return false;
    }
}

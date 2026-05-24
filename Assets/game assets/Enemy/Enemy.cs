using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] int goldReward = 25;
    [SerializeField] int goldPenalty = 25;
    
    Banking banking;

    void Start()
    {
        banking = FindObjectOfType<Banking>();
    }

    // Update is called once per frame
    public void GoldDeposit()
    {
        if(banking == null) 
        { 
            Debug.LogError("something wrong in banking");
            return; 
        }
        banking.Deposit(goldReward);
    }

    public void GoldWithdraw()
    {
        if(banking == null) 
        {
            Debug.LogError("something wrong in banking");
            return; 
        }
        banking.Withdraw(goldPenalty);
    }
}

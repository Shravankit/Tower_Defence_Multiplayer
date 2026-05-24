using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Banking : MonoBehaviour
{

    [SerializeField] int bankStartBalance = 150;
    [SerializeField] int currentBalance;

    [SerializeField] TextMeshProUGUI score;
    [SerializeField] TextMeshProUGUI winText;

    public int CurrentBalance
    {
        get
        {
            return currentBalance;
        }
    }

    void Awake()
    {
        currentBalance = bankStartBalance;
        ScoreResult();
    }

    // private void Update() {
            
    // }
    
    public void Deposit(int amount)
    {
        currentBalance += Mathf.Abs(amount);
        ScoreResult();
    }

    public void Withdraw(int amount)
    {
        currentBalance -= Mathf.Abs(amount);
        ScoreResult();

        if(currentBalance < 0)
        {
            //lose the game then re start again
            ReloadScene();
        }
    }


    void ScoreResult()
    {
        score.text = currentBalance.ToString();
    }
    void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // void StopScene()
    // {
    //     Scene stop = SceneManager.GetActiveScene();
    //     stop
    // }
}

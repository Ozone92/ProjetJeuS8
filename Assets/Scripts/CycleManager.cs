using System;
using System.Collections;
using UnityEngine;

public class CycleManager : MonoBehaviour
{
    public int CurrentCycle { get; private set; } = 0;

    [SerializeField]
    private Professor professor;
    
    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField] 
    private MiniJeuManager CleaningGameManagerPrefab;
    
    [SerializeField]
    private BalanceMiniGameManager BalanceGameManagerPrefab;

    [SerializeField] 
    private GameObject backgroundClassMusic;

    private IEnumerator DoCleaningGame()
    {
        MiniJeuManager miniJeuManager =  Instantiate<MiniJeuManager>(CleaningGameManagerPrefab);
        yield return new WaitUntil(() => miniJeuManager.JeuTermine);
        int score = miniJeuManager.Score;

        int toAdd = 0;
        if (score < 20)
        {
            toAdd = -10;
        }
        else if (score < 25)
        {
            toAdd = -7;
        }
        else if (score >= 60)
        {
            toAdd = 9;
        }
        else if (score >= 50)
        {
            toAdd = 7;
        }
        else if (score >= 40)
        {
            toAdd = 5;
        }
        else if (score >= 30)
        {
            toAdd = 3;
        }
        
        if (playerStats.Get("DoMinigame2") != 0f)
        {
            toAdd *= 3;
        }
                
        playerStats.Add("GroupScore", toAdd/2);
        playerStats.Add("TpScore", toAdd);
        
        yield return new WaitForSecondsRealtime(2f);
        Destroy(miniJeuManager.gameObject);
    }

    private IEnumerator DoBalanceGame()
    {
        BalanceMiniGameManager balanceMiniGameManager = Instantiate<BalanceMiniGameManager>(BalanceGameManagerPrefab);
        yield return new WaitUntil(() => balanceMiniGameManager.gameEnded);

        int toAdd = balanceMiniGameManager.score switch
        {
            >= 70 => 15,
            >= 60 => 10,
            >= 40 => 5,
            _ => -15
        };

        playerStats.Add("GroupScore", toAdd/2);
        playerStats.Add("TpScore", toAdd);
        
        yield return new WaitForSecondsRealtime(2f);
        Destroy(balanceMiniGameManager.gameObject);
    }

    IEnumerator Start()
    {
        while (CurrentCycle <= 3)
        {
            professor.PlayStartDialog();

            yield return new WaitUntil(() => playerStats.Get("CycleFinish" + CurrentCycle) != 0f);
            yield return new WaitUntil(() => !DialogData.InDialog);

            string gameToLaunch = "";
            switch (CurrentCycle)
            {
                case 2:
                    gameToLaunch = "DoCleaningGame";
                    break;
                case 3:
                    gameToLaunch = "DoBalanceGame";
                    break;
            }

            if (gameToLaunch != "")
            {
                backgroundClassMusic.SetActive(false);
                yield return StartCoroutine(gameToLaunch);
                backgroundClassMusic.SetActive(true);
            }

            Debug.Log("New Cycle");
            CurrentCycle++;
        }
        
        Debug.Log("END");
        Debug.Log($"Group Score: {playerStats.Get("GroupScore")} |  TpScore: {playerStats.Get("TpScore")}");
    }
}

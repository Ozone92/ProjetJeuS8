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

    private IEnumerator DoCleaningGame()
    {
        MiniJeuManager miniJeuManager =  Instantiate<MiniJeuManager>(CleaningGameManagerPrefab);
        yield return new WaitUntil(() => miniJeuManager.JeuTermine);
        int score = miniJeuManager.Score;

        int toAdd = 0;
        if (score < 5)
        {
            toAdd = -1;
        }
        else if (score > 15)
        {
            toAdd = 1;
        }
        
        if (playerStats.Get("DoMinigame2") != 0f)
        {
            toAdd *= 2;
        }
                
        playerStats.Add("GroupScore", toAdd);
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
            >= 80 => 2,
            >= 50 => 1,
            _ => 0
        };

        playerStats.Add("GroupScore", toAdd);
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

            switch (CurrentCycle)
            {
                case 2:
                    yield return DoCleaningGame();
                    break;
                case 3:
                    yield return DoBalanceGame();
                    break;
            }

            Debug.Log("New Cycle");
            CurrentCycle++;
        }
        
        Debug.Log("END");
        Debug.Log($"Group Score: {playerStats.Get("GroupScore")} |  TpScore: {playerStats.Get("TpScore")}");
    }
}

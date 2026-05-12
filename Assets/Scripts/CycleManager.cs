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
    private MiniJeuManager jeuManagerPrefab;
    

    IEnumerator Start()
    {
        while (CurrentCycle <= 3)
        {
            professor.PlayStartDialog();

            yield return new WaitUntil(() => playerStats.Get("CycleFinish" + CurrentCycle) != 0f);
            yield return new WaitUntil(() => !DialogData.InDialog);

            if (playerStats.Get("DoMinigame" + CurrentCycle) != 0f)
            {
                // mainCamera.SetActive(false);
                // Launch Mini Game here
                MiniJeuManager miniJeuManager =  Instantiate<MiniJeuManager>(jeuManagerPrefab);
                yield return new WaitUntil(() => miniJeuManager.JeuTermine);
                int score = miniJeuManager.Score;
                Destroy(miniJeuManager.gameObject);
                // mainCamera.SetActive(true);

                int toAdd = 0;
                if (score < 5)
                {
                    toAdd = -1;
                }
                else if (score > 15)
                {
                    toAdd = 1;
                }
                
                playerStats.Add("GroupScore", toAdd);
                playerStats.Add("TpScore", toAdd);
            }

            Debug.Log("New Cycle");
            CurrentCycle++;
        }
        
        Debug.Log("END");
        Debug.Log($"Group Score: {playerStats.Get("GroupScore")} |  TpScore: {playerStats.Get("TpScore")}");
    }
}

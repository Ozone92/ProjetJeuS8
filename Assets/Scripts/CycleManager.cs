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
    

    IEnumerator Start()
    {
        while (CurrentCycle <= 3)
        {
            professor.PlayStartDialog();

            yield return new WaitUntil(() => playerStats.Get("CycleFinish" + CurrentCycle) != 0f);

            if (playerStats.Get("DoMinigame" + CurrentCycle) != 0f)
            {
                // Launch Mini Game here
            }

            CurrentCycle++;
        }
    }
}

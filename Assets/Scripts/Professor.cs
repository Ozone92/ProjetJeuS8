using UnityEngine;

public class Professor : MonoBehaviour
{
    [SerializeField] private CycleManager cycleManager;
    [SerializeField] private PlayerStats playerStats;
    
    [SerializeField] private DialogData cycle0Dialog;
    [SerializeField] private DialogData cycle1Dialog;
    [SerializeField] private DialogData cycle2Dialog;
    [SerializeField] private DialogData cycle3Dialog;

    public void PlayStartDialog()
    {
        DialogData toPlay = null;
        switch (cycleManager.CurrentCycle)
        {
            case 0:
                toPlay = cycle0Dialog;
                break;
            case 1:
                toPlay = cycle1Dialog;
                break;
            case 2:
                toPlay = cycle2Dialog;
                break;
            case 3:
                toPlay = cycle3Dialog;
                break;
        }

        if (toPlay != null)
        {
            StartCoroutine(toPlay.Play());
        }
    }
}

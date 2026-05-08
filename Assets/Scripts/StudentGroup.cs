using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentGroup : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private CycleManager cycleManager;
    [SerializeField] private int groupNumber;
    
    [SerializeField] private DialogData cycle0Dialog;

    [System.Serializable]
    struct CycleDialog
    {
        public DialogData dialog;
        public DialogData choiceDialog;
    }
    
    [SerializeField] private List<CycleDialog> cycleDialogs;
    
    
    public void Interact()
    {
        StartCoroutine(LaunchDialog());
    }

    private IEnumerator LaunchDialog()
    {
        if (cycleManager.CurrentCycle == 0)
        {
            StartCoroutine(cycle0Dialog.Play());
        }
        else
        {
            CycleDialog currentDialog = cycleDialogs[cycleManager.CurrentCycle - 1];

            yield return StartCoroutine(currentDialog.dialog.Play());

            if ((int)playerStats.Get("Group") == groupNumber)
            {
                StartCoroutine(currentDialog.choiceDialog.Play());
            }
        }
    }
}

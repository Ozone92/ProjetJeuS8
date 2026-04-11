using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogs/DialogData")]
public class DialogData : ScriptableObject
{
    [System.Serializable]
    public struct Choice
    {
        public string text;
        public string idToGo;
    }
    
    [System.Serializable]
    public struct Dialog
    {
        public string id;
        public string speaker;
        public string text;
        public Vector3 cameraPosition;
        
        public string statsToChange;
        public float amount;
        
        public List<Choice> choices;
        
    }
    
    public Dialog[] dialogs;
    public DialogBoxHandler dialogBoxPrefab;
    public PlayerStats playerStats;

    public IEnumerator Play()
    {
        // Instantiate boite de dialogue
        string nextId = dialogs[0].id;
        while (nextId != "")
        {
            var dialog = Array.Find(dialogs, dialog => dialog.id == nextId);
            
            var dialogBox = Instantiate(dialogBoxPrefab);
            Camera.main.transform.position = dialog.cameraPosition;
            
            // Modifier le statsToChange si != empty
            if (dialog.statsToChange != "")
            {
                playerStats.add(dialog.statsToChange, dialog.amount);
            }
            
            // Fill la boite avec dialog
            dialogBox.fill(dialog);
            
            // WaitUntil dialogFinish == true
            yield return new WaitUntil(() => dialogBox.ChoiceMade);
            
            // Recuperer le nextId
            nextId = dialogBox.ChoiceIndex;
            Destroy(dialogBox.gameObject);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogs/DialogData")]
public class DialogData : ScriptableObject
{
    [System.Serializable]
    public struct Choice
    {
        public string text;
        public string idToGo;
        
        public List<Stats> statsToChange;
        public List<Stats> minimalCondition;
    }

    [System.Serializable]
    public struct Stats
    {
        public string name;
        public float amount;
    }
    
    [System.Serializable]
    public struct Dialog
    {
        public string id;
        public string speaker;
        public string text;
        
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;
        
        public List<Choice> choices;
        
    }

    public static bool InDialog { get; private set; } = false;
    
    public Dialog[] dialogs;
    public DialogBoxHandler dialogBoxPrefab;
    public PlayerStats playerStats;

    public IEnumerator Play()
    {
        Debug.Log($"Launch Dialog: ${this.name}");
        InDialog = true;
        
        Camera mainCamera = Camera.main;
        var dialogCamera = Resources.FindObjectsOfTypeAll<Camera>().First(camera => camera.gameObject.CompareTag("DialogCamera"));

        if (!mainCamera)
        {
            Debug.LogError("Main camera not found");
            InDialog = false;
            yield break;
        }
        
        mainCamera.gameObject.SetActive(false);
        dialogCamera.gameObject.SetActive(true);
        
        string nextId = dialogs[0].id;
        while (nextId != "")
        {
            var dialog = Array.Find(dialogs, dialog => dialog.id == nextId);
            dialogCamera.gameObject.transform.position = dialog.cameraPosition;
            dialogCamera.gameObject.transform.eulerAngles = dialog.cameraRotation;
            
            var dialogBox = Instantiate(dialogBoxPrefab);
            dialogBox.transform.position = dialog.cameraPosition;
            dialogBox.transform.eulerAngles = dialog.cameraRotation;
            
            dialogBox.fill(dialog, playerStats);
            
            yield return new WaitUntil(() => dialogBox.ChoiceMade);
            
            nextId = dialogBox.ChoiceIndex;
            Destroy(dialogBox.gameObject);
        }
        
        mainCamera.gameObject.SetActive(true);
        dialogCamera.gameObject.SetActive(false);
        InDialog = false;
    }
}

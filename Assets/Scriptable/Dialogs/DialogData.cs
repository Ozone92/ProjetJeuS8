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

        public bool useDefaultCameraTransform;
        public bool useSpecificCameraTransform;
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;

        public List<Choice> choices;
    }

    public static bool InDialog { get; private set; } = false;

    [Header("Dialogs")] public Dialog[] dialogs;
    public DialogBoxHandler dialogBoxPrefab;
    public PlayerStats playerStats;

    [Header("Default Camera Setup")] public Vector3 defaultCameraPosition;
    public Vector3 defaultCameraRotation;

    [Header("Camera Transform Mapper")] public CameraTransformMapper cameraTransformMapper;

    public IEnumerator Play()
    {
        Debug.Log($"Launch Dialog: {name}");
        InDialog = true;

        Camera mainCamera = Camera.main;
        var dialogCamera = Resources.FindObjectsOfTypeAll<Camera>()
            .First(camera => camera.gameObject.CompareTag("DialogCamera"));

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
            var dialog = Array.Find(dialogs, d => d.id == nextId);

            if (dialog.useDefaultCameraTransform)
            {
                dialogCamera.transform.position = defaultCameraPosition;
                dialogCamera.transform.eulerAngles = defaultCameraRotation;
            }
            else if (!dialog.useSpecificCameraTransform && cameraTransformMapper &&
                     cameraTransformMapper.TryGet(dialog.speaker, out var transform))
            {
                dialogCamera.transform.position = transform.Item1;
                dialogCamera.transform.eulerAngles = transform.Item2;
            }
            else
            {
                dialogCamera.transform.position = dialog.cameraPosition;
                dialogCamera.transform.eulerAngles = dialog.cameraRotation;
            }

            var dialogBox = Instantiate(dialogBoxPrefab);
            dialogBox.fill(dialog, playerStats);

            yield return new WaitUntil(() => dialogBox.ChoiceMade);

            nextId = dialogBox.ChoiceIndex;
            Destroy(dialogBox.gameObject);
        }

        dialogCamera.transform.position = defaultCameraPosition;
        dialogCamera.transform.eulerAngles = defaultCameraRotation;

        mainCamera.gameObject.SetActive(true);
        dialogCamera.gameObject.SetActive(false);
        InDialog = false;
    }
}
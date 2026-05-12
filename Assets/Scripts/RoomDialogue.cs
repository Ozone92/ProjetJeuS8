
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoomDialogue : MonoBehaviour
{
    [SerializeField] private Button testButton;
    [SerializeField] private DialogData cycle0Dialog;

    private void Start()
    {
        testButton.interactable = false;
        testButton.onClick.AddListener(GoToClass);
        StartCoroutine(StartDialogThenEnableButton());
    }

    private IEnumerator StartDialogThenEnableButton()
    {
        yield return StartCoroutine(cycle0Dialog.Play());
        testButton.interactable = true;
    }

    private void GoToClass()
    {
        SceneManager.LoadScene("Class");
    }
}
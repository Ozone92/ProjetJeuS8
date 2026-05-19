
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoomDialogue : MonoBehaviour
{
    [SerializeField] private Button testButton;
    [SerializeField] private DialogData cycle0Dialog;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Transform playerTransform;
    private IEnumerator Start()
    {
        testButton.gameObject.SetActive(false);
        testButton.onClick.AddListener(GoToClass);
        StartCoroutine(StartDialogThenEnableButton());
        yield return new WaitUntil(() => playerStats.Get("Reveil") != 0f);
        playerTransform.SetPositionAndRotation(
            new Vector3(-0.039f, 0.02f, -1.828f),
            Quaternion.Euler(0f, 90f, 0f)
        );
    }

    private IEnumerator StartDialogThenEnableButton()
    {
        yield return StartCoroutine(cycle0Dialog.Play());
        testButton.gameObject.SetActive(true);
    }

    private void GoToClass()
    {
        SceneManager.LoadScene("Salle de classe");
    }
}
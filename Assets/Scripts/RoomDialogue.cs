using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoomDialogue : MonoBehaviour
{
    [SerializeField] private Button testButton;
    [SerializeField] private Button BackToMenuButton;
    [SerializeField] private Canvas rendormiCanvas;
    [SerializeField] private CanvasGroup rendormiCanvasGroup;
    [SerializeField] private float fadeDuration = 5f;
    [SerializeField] private DialogData cycle0Dialog;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Transform playerTransform;

    private IEnumerator Start()
    {
        testButton.gameObject.SetActive(false);
        BackToMenuButton.gameObject.SetActive(false);

        rendormiCanvas.gameObject.SetActive(true);
        rendormiCanvasGroup.alpha = 0f;
        rendormiCanvasGroup.interactable = false;
        rendormiCanvasGroup.blocksRaycasts = false;

        testButton.onClick.AddListener(GoToClass);
        BackToMenuButton.onClick.AddListener(BackToMenu);

        StartCoroutine(StartDialogThenEnableButton());

        yield return new WaitUntil(() => playerStats.Get("Reveil") != 0f);

        playerTransform.SetPositionAndRotation(
            new Vector3(-0.039f, 0.02f, -1.828f),
            Quaternion.Euler(0f, 90f, 0f)
        );

        yield return new WaitUntil(() => playerStats.Get("finito") != 0f);
        playerTransform.SetPositionAndRotation(
            new Vector3(0.081f, 0.1f, -2.048f),
            Quaternion.Euler(-90f, 0f, 90f)
        );
        testButton.gameObject.SetActive(false);
        
        yield return StartCoroutine(FadeInRendormi());

        BackToMenuButton.gameObject.SetActive(true);
        rendormiCanvasGroup.interactable = true;
        rendormiCanvasGroup.blocksRaycasts = true;
    }

    private IEnumerator StartDialogThenEnableButton()
    {
        yield return StartCoroutine(cycle0Dialog.Play());
        testButton.gameObject.SetActive(true);
    }

    private IEnumerator FadeInRendormi()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            rendormiCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        rendormiCanvasGroup.alpha = 1f;
    }

    private void GoToClass()
    {
        SceneManager.LoadScene("Salle de classe");
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("WelcomeMenu");
    }
}
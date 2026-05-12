using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameLauncher : MonoBehaviour, IInteractable
{
    [SerializeField] private string sceneName;

    public void Interact()
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("MiniGameLauncher has no scene name.", this);
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}

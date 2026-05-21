using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CycleManager : MonoBehaviour
{
    [System.Serializable]
    public struct EndingDialog
    {
        public DialogData goodTpGoodGroup;
        public DialogData goodTpBadGroup;
        public DialogData badTpGoodGroup;
        public DialogData badTpBadGroup;
    }
    
    [System.Serializable]
    private class EndingData
    {
        public string statKey;
        public string message;
    }
    [SerializeField] private Image image_speed;
    
    [SerializeField] private Button BackToMenuButton;
    [SerializeField] private Canvas rendormiCanvas;
    [SerializeField] private CanvasGroup rendormiCanvasGroup;
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private TMP_Text endText;

    private bool endingTriggered = false;

    private EndingData[] endings = new EndingData[]
    {
        new EndingData { statKey = "G1_snap", message = "D'un claquement de doigts, la moitié des élèves de la classe furent exterminés par Professeur T" },
        new EndingData { statKey = "G1_amuse", message = "Si seulement Uryu savait que j'avais fait exprès de rater ce TP pour maintenir ma moyenne à un 10 parfait" },
        new EndingData { statKey = "G1_learn", message = "on a appris" },
        new EndingData { statKey = "G1_wnerd", message = "uryu le goat" },
        new EndingData { statKey = "G2_snap", message = "D'un claquement de doigts, la moitié des élèves de la classe furent exterminés par Professeur T" },
        new EndingData { statKey = "G2_decu", message = "Fin 6" },
        new EndingData { statKey = "G2_passion", message = "Fin 7" },
        new EndingData { statKey = "G2_wxiao", message = "Fin 8" },
        new EndingData { statKey = "G3_snap", message = "D'un claquement de doigts, la moitié des élèves de la classe furent exterminés par Professeur T" },
        new EndingData { statKey = "G3_decu", message = "Fin 10" },
        new EndingData { statKey = "G3_larp", message = "Fin 11" },
        new EndingData { statKey = "G3_cook", message = "Fin 12" }
    };

    public int CurrentCycle { get; private set; } = 0;

    [SerializeField] private Professor professor;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private MiniJeuManager CleaningGameManagerPrefab;
    [SerializeField] private BalanceMiniGameManager BalanceGameManagerPrefab;
    [SerializeField] private GameObject backgroundClassMusic;
    [SerializeField] private DialogData endingDialogBeginning;
    [SerializeField] private List<EndingDialog> endingDialogs;

    private IEnumerator DoCleaningGame()
    {
        MiniJeuManager miniJeuManager = Instantiate(CleaningGameManagerPrefab);
        yield return new WaitUntil(() => miniJeuManager.JeuTermine);

        int score = miniJeuManager.Score;
        int toAdd = 0;

        if (score < 20)
        {
            toAdd = -8;
        }
        else if (score < 25)
        {
            toAdd = -4;
        }
        else if (score >= 50)
        {
            toAdd = 10;
        }
        else if (score >= 40)
        {
            toAdd = 7;
        }
        else if (score >= 30)
        {
            toAdd = 5;
        }
        
        if (playerStats.Get("DoMinigame2") != 0f)
            toAdd *= 3;

        playerStats.Add("GroupScore", toAdd);
        playerStats.Add("TpScore", toAdd);
        
        Destroy(miniJeuManager.gameObject);
    }

    private IEnumerator DoBalanceGame()
    {
        BalanceMiniGameManager balanceMiniGameManager = Instantiate(BalanceGameManagerPrefab);
        yield return new WaitUntil(() => balanceMiniGameManager.gameEnded);

        int toAdd = balanceMiniGameManager.score switch
        {
            >= 70 => 18,
            >= 50 => 13,
            >= 30 => 7,
            _ => -15
        };

        playerStats.Add("GroupScore", toAdd);
        playerStats.Add("TpScore", toAdd);
        
        Destroy(balanceMiniGameManager.gameObject);
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("WelcomeMenu");
    }

    private IEnumerator WatchEnding(EndingData ending)
    {
        yield return new WaitUntil(() => playerStats.Get(ending.statKey) != 0f);

        if (endingTriggered)
            yield break;

        endingTriggered = true;
        yield return StartCoroutine(ShowEndScreen(ending.message));
    }
    
    private IEnumerator WaitForSpeedPhoneImage()
    {
        yield return new WaitUntil(() => playerStats.Get("speed_phone") != 0f);

        image_speed.gameObject.SetActive(true);
    }
    private IEnumerator ShowEndScreen(string message)
    {
        endText.text = message;

        yield return StartCoroutine(FadeInRendormi());

        BackToMenuButton.gameObject.SetActive(true);
        rendormiCanvasGroup.interactable = true;
        rendormiCanvasGroup.blocksRaycasts = true;
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

    private void StartEndingWatchers()
    {
        for (int i = 0; i < endings.Length; i++)
        {
            StartCoroutine(WatchEnding(endings[i]));
        }
    }

    IEnumerator Start()
    {
        BackToMenuButton.gameObject.SetActive(false);

        rendormiCanvas.gameObject.SetActive(true);
        rendormiCanvasGroup.alpha = 0f;
        rendormiCanvasGroup.interactable = false;
        rendormiCanvasGroup.blocksRaycasts = false;
        image_speed.gameObject.SetActive(false);

        BackToMenuButton.onClick.AddListener(BackToMenu);
        StartCoroutine(WaitForSpeedPhoneImage());

        while (CurrentCycle <= 3)
        {
            professor.PlayStartDialog();

            yield return new WaitUntil(() => playerStats.Get("CycleFinish" + CurrentCycle) != 0f);
            yield return new WaitUntil(() => !DialogData.InDialog);

            string gameToLaunch = "";
            switch (CurrentCycle)
            {
                case 2:
                    gameToLaunch = "DoCleaningGame";
                    break;
                case 3:
                    gameToLaunch = "DoBalanceGame";
                    break;
            }

            if (gameToLaunch != "")
            {
                backgroundClassMusic.SetActive(false);
                yield return StartCoroutine(gameToLaunch);
                backgroundClassMusic.SetActive(true);
            }

            Debug.Log("New Cycle");
            CurrentCycle++;
        }

        yield return endingDialogBeginning.Play();

        bool tpGood = playerStats.Get("TpScore") >= 75f;
        bool groupGood = playerStats.Get("GroupScore") >= 75f;
        var dialogs = endingDialogs[(int)playerStats.Get("Group") - 1];

        DialogData toPlay;
        if (tpGood)
        {
            if (groupGood)
                toPlay = dialogs.goodTpGoodGroup;
            else
                toPlay = dialogs.goodTpBadGroup;
        }
        else
        {
            if (groupGood)
                toPlay = dialogs.badTpGoodGroup;
            else
                toPlay = dialogs.badTpBadGroup;
        }
        
        StartEndingWatchers();

        yield return toPlay.Play();
    }
}
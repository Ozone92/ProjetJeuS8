using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BalanceMiniGameManager : MonoBehaviour
{
    [Header("Interface UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI stableTimeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;

    [Header("Barres")]
    [SerializeField] private BalanceBar firstBar;
    [SerializeField] private BalanceBar secondBar;

    [Header("Parametres")]
    [SerializeField] private float gameDuration = 25f;
    [SerializeField] private float requiredStableTime = 15f;
    [SerializeField] private bool loseStableTimeWhenUnstable = true;
    [SerializeField] private float unstablePenaltySpeed = 1.5f;
    [SerializeField] private int startScore = 100;
    [SerializeField] private float unstableScorePenaltyInterval = 0.34f;
    [SerializeField] private int unstableScorePenalty = 1;
    [SerializeField] private string returnSceneName = "SampleScene";

    private float remainingTime;
    private float stableTime;
    private float unstablePenaltyTimer;
    public int score;
    private bool gameRunning;
    public bool gameEnded;
    private bool wasStable = true;
    private bool waitingForNextButton;
    private TextMeshProUGUI startButtonLabel;
    private TextMeshProUGUI startInstructionsText;
    private const string StartInstructions = "Gardez les deux barres dans la zone verte.\nA/D controle la barre de gauche, <- et -> controle la barre de droite.";

    private void Awake()
    {
        EnsurePlayableUi();
        CacheStartScreenTexts();
    }

    private void Start()
    {
        remainingTime = gameDuration;
        score = startScore;
        UpdateTexts("Pret ?");
        SetStartButtonText("START");
        SetStartInstructions(StartInstructions);

        if (startScreen)
        {
            startScreen.SetActive(true);
        }

        if (endScreen)
        {
            endScreen.SetActive(false);
        }
    }

    private void Update()
    {
        if (!gameRunning || gameEnded)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        bool stable = firstBar && secondBar && firstBar.IsStable && secondBar.IsStable;
        if (stable)
        {
            unstablePenaltyTimer = 0f;
            stableTime += Time.deltaTime;
            wasStable = true;
            UpdateTexts();
        }
        else
        {
            if (wasStable)
            {
                ApplyScorePenaltyOnce();
                unstablePenaltyTimer = 0f;
            }

            if (loseStableTimeWhenUnstable)
            {
                stableTime = Mathf.Max(0f, stableTime - Time.deltaTime * unstablePenaltySpeed);
            }

            wasStable = false;
            ApplyUnstableScorePenalty();
            UpdateTexts();
        }

        if (stableTime >= requiredStableTime || remainingTime <= 0f)
        {
            EndGame();
        }
    }

    public void StartGame()
    {
        if (waitingForNextButton)
        {
            ConfirmEndGame();
            return;
        }

        if (gameRunning)
        {
            return;
        }

        StartCoroutine(StartSequence());
    }

    public void ReturnToMainScene()
    {
        SceneManager.LoadScene(returnSceneName);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator StartSequence()
    {
        waitingForNextButton = false;
        SetStartButtonText("START");
        SetStartInstructions(StartInstructions);

        if (startScreen)
        {
            startScreen.SetActive(false);
        }

        if (endScreen)
        {
            endScreen.SetActive(false);
        }

        SetStatus("3");
        yield return new WaitForSeconds(1f);
        SetStatus("2");
        yield return new WaitForSeconds(1f);
        SetStatus("1");
        yield return new WaitForSeconds(1f);
        SetStatus("GO !");
        yield return new WaitForSeconds(0.5f);

        remainingTime = gameDuration;
        stableTime = 0f;
        unstablePenaltyTimer = 0f;
        score = startScore;
        wasStable = true;
        gameRunning = true;
        gameEnded = false;

        if (firstBar)
        {
            firstBar.Begin();
        }

        if (secondBar)
        {
            secondBar.Begin();
        }
    }

    private void EndGame()
    {
        gameRunning = false;
        waitingForNextButton = true;

        if (firstBar)
        {
            firstBar.Stop();
        }

        if (secondBar)
        {
            secondBar.Stop();
        }

        UpdateTexts(GetFinalMessage());

        if (endScreen)
        {
            endScreen.SetActive(true);
        }

        SetStartButtonText("SUIVANT");
        SetStartInstructions("Score final : " + score);

        if (startScreen)
        {
            startScreen.SetActive(true);
        }
    }

    private void ConfirmEndGame()
    {
        waitingForNextButton = false;
        gameEnded = true;

        if (startScreen)
        {
            startScreen.SetActive(false);
        }

        if (endScreen)
        {
            endScreen.SetActive(false);
        }
    }

    private void UpdateTexts(string status = "")
    {
        string timerValue = "Temps : " + Mathf.CeilToInt(Mathf.Max(0f, remainingTime)) + "s";

        if (timerText)
        {
            timerText.text = timerValue;
        }

        if (statusText)
        {
            statusText.text = string.IsNullOrEmpty(status) ? timerValue : status;
        }

        if (stableTimeText)
        {
            stableTimeText.text = "Equilibre : " + Mathf.FloorToInt(stableTime) + " / " + Mathf.CeilToInt(requiredStableTime) + "s";
        }

        if (scoreText)
        {
            scoreText.text = "Score : " + score;
        }
    }

    private void SetStatus(string status)
    {
        if (statusText)
        {
            statusText.text = status;
        }
    }

    private void EnsurePlayableUi()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (!canvas)
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
        }

        timerText = timerText ? timerText : FindText("TimerText");
        statusText = statusText ? statusText : FindText("StatusText");
        stableTimeText = stableTimeText ? stableTimeText : FindText("StableTimeText");
        scoreText = scoreText ? scoreText : FindText("ScoreText");

        if (!scoreText)
        {
            scoreText = CreateText(canvas.transform, "ScoreText", "Score : 100");
        }

        PositionText(timerText, new Vector2(0.5f, 1f), new Vector2(0f, -24f), new Vector2(360f, 70f), TextAlignmentOptions.Top);
        PositionText(scoreText, new Vector2(0.5f, 1f), new Vector2(0f, -96f), new Vector2(360f, 70f), TextAlignmentOptions.Top);
        PositionText(statusText, new Vector2(0.5f, 1f), new Vector2(0f, -168f), new Vector2(440f, 70f), TextAlignmentOptions.Top);
        PositionText(stableTimeText, new Vector2(1f, 1f), new Vector2(-20f, -24f), new Vector2(430f, 70f), TextAlignmentOptions.TopRight);

        if (!firstBar)
        {
            firstBar = CreateBar(canvas.transform, "BalanceBarLeft", new Vector2(-360f, -80f), Key.A, Key.D);
        }

        if (!secondBar)
        {
            secondBar = CreateBar(canvas.transform, "BalanceBarRight", new Vector2(360f, -80f), Key.LeftArrow, Key.RightArrow);
        }

        if (!startScreen)
        {
            startScreen = CreateStartButton(canvas.transform);
        }
    }

    private void CacheStartScreenTexts()
    {
        if (!startScreen)
        {
            return;
        }

        Button startButton = startScreen.GetComponentInChildren<Button>(true);
        if (startButton)
        {
            startButtonLabel = startButton.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        foreach (TextMeshProUGUI text in startScreen.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (text != startButtonLabel)
            {
                startInstructionsText = text;
                break;
            }
        }
    }

    private void SetStartButtonText(string text)
    {
        if (startButtonLabel)
        {
            startButtonLabel.text = text;
        }
    }

    private void SetStartInstructions(string text)
    {
        if (startInstructionsText)
        {
            startInstructionsText.text = text;
        }
    }

    private TextMeshProUGUI FindText(string objectName)
    {
        GameObject textObject = GameObject.Find(objectName);
        return textObject ? textObject.GetComponent<TextMeshProUGUI>() : null;
    }

    private TextMeshProUGUI CreateText(Transform parent, string objectName, string text)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI textComponent = textObject.GetComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.color = Color.white;
        textComponent.fontSize = 36f;
        textComponent.alignment = TextAlignmentOptions.TopLeft;
        return textComponent;
    }

    private void PositionText(TextMeshProUGUI text, Vector2 anchor, Vector2 anchoredPosition, Vector2 size, TextAlignmentOptions alignment)
    {
        if (!text)
        {
            return;
        }

        RectTransform rectTransform = text.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = anchor;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;
        text.alignment = alignment;
        text.fontSize = 36f;
    }

    private BalanceBar CreateBar(Transform parent, string objectName, Vector2 position, Key decreaseKey, Key increaseKey)
    {
        GameObject root = new GameObject(objectName, typeof(RectTransform), typeof(Slider), typeof(BalanceBar));
        root.transform.SetParent(parent, false);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = position;
        rootRect.sizeDelta = new Vector2(520f, 64f);

        Image backgroundImage = CreateImage(root.transform, "Background", new Color(0.12f, 0.12f, 0.12f, 0.85f));
        RectTransform backgroundRect = backgroundImage.rectTransform;
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        Image stableZoneImage = CreateImage(root.transform, "StableZone", new Color(0.15f, 0.8f, 0.25f, 0.35f));
        RectTransform stableZoneRect = stableZoneImage.rectTransform;
        stableZoneRect.anchorMin = new Vector2(0.4f, 0f);
        stableZoneRect.anchorMax = new Vector2(0.6f, 1f);
        stableZoneRect.offsetMin = Vector2.zero;
        stableZoneRect.offsetMax = Vector2.zero;

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(root.transform, false);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(6f, 6f);
        fillAreaRect.offsetMax = new Vector2(-6f, -6f);

        Image fillImage = CreateImage(fillArea.transform, "Fill", new Color(0.9f, 0.25f, 0.2f, 0.9f));
        RectTransform fillRect = fillImage.rectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        Slider slider = root.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 50f;
        slider.interactable = false;
        slider.transition = Selectable.Transition.None;
        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;
        slider.direction = Slider.Direction.LeftToRight;

        BalanceBar bar = root.GetComponent<BalanceBar>();
        bar.Configure(slider, fillImage, decreaseKey, increaseKey);
        return bar;
    }

    private Image CreateImage(Transform parent, string objectName, Color color)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private GameObject CreateStartButton(Transform parent)
    {
        GameObject startScreenObject = new GameObject("StartScreen", typeof(RectTransform));
        startScreenObject.transform.SetParent(parent, false);
        RectTransform startScreenRect = startScreenObject.GetComponent<RectTransform>();
        startScreenRect.anchorMin = Vector2.zero;
        startScreenRect.anchorMax = Vector2.one;
        startScreenRect.offsetMin = Vector2.zero;
        startScreenRect.offsetMax = Vector2.zero;

        GameObject instructionPanel = new GameObject("StartInstructionsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        instructionPanel.transform.SetParent(startScreenObject.transform, false);
        RectTransform panelRect = instructionPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = new Vector2(0f, 170f);
        panelRect.sizeDelta = new Vector2(1040f, 170f);

        Image panelImage = instructionPanel.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject instructionObject = new GameObject("StartInstructions", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        instructionObject.transform.SetParent(instructionPanel.transform, false);
        RectTransform instructionRect = instructionObject.GetComponent<RectTransform>();
        instructionRect.anchorMin = Vector2.zero;
        instructionRect.anchorMax = Vector2.one;
        instructionRect.offsetMin = new Vector2(28f, 18f);
        instructionRect.offsetMax = new Vector2(-28f, -18f);

        TextMeshProUGUI instructions = instructionObject.GetComponent<TextMeshProUGUI>();
        instructions.text = StartInstructions;
        instructions.fontSize = 34f;
        instructions.color = Color.white;
        instructions.alignment = TextAlignmentOptions.Center;

        GameObject buttonObject = new GameObject("StartButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(startScreenObject.transform, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0f);
        buttonRect.anchorMax = new Vector2(0.5f, 0f);
        buttonRect.pivot = new Vector2(0.5f, 0f);
        buttonRect.anchoredPosition = new Vector2(0f, 80f);
        buttonRect.sizeDelta = new Vector2(760f, 130f);

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(StartGame);

        GameObject labelObject = new GameObject("Text (TMP)", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.text = "START";
        label.fontSize = 34f;
        label.color = Color.black;
        label.alignment = TextAlignmentOptions.Center;

        return startScreenObject;
    }

    private void ApplyUnstableScorePenalty()
    {
        if (score <= 0 || unstableScorePenaltyInterval <= 0f)
        {
            return;
        }

        unstablePenaltyTimer += Time.deltaTime;
        while (unstablePenaltyTimer >= unstableScorePenaltyInterval)
        {
            unstablePenaltyTimer -= unstableScorePenaltyInterval;
            score = Mathf.Max(0, score - unstableScorePenalty);
        }
    }

    private void ApplyScorePenaltyOnce()
    {
        if (score <= 0)
        {
            return;
        }

        score = Mathf.Max(0, score - unstableScorePenalty);
    }

    private string GetFinalMessage()
    {
        if (score >= 70)
        {
            return "Bien joue";
        }

        if (score >= 50)
        {
            return "Pas mal";
        }

        if (score >= 30)
        {
            return "Moyen";
        }

        return "Peut\u00A0mieux\u00A0faire";
    }
}

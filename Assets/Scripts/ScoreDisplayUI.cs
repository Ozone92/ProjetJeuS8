using UnityEngine;
using UnityEngine.UI; // Nécessaire pour les Images
using TMPro; // Nécessaire pour TextMeshPro

public class ScoreDisplayUI : MonoBehaviour
{
    [Header("Data Source")]
    [Tooltip("Glisse ton ScriptableObject PlayerStats ici")]
    public PlayerStats playerStats;

    [Header("TP Score UI")]
    public Image tpScoreFillImage;
    public TextMeshProUGUI tpScoreText;

    [Header("Group Score UI")]
    public Image groupScoreFillImage;
    public TextMeshProUGUI groupScoreText;

    [Header("Settings")]
    public float maxScore = 100f;

    void Update()
    {
        // On s'assure que le ScriptableObject est bien assigné
        if (playerStats == null) return;

        UpdateTpScoreUI();
        UpdateGroupScoreUI();
    }

    private void UpdateTpScoreUI()
    {
        float currentScore = playerStats.Get("TpScore");
        
        // Mise à jour de la jauge (Fill Amount va de 0 à 1, donc on divise par le max)
        if (tpScoreFillImage != null)
        {
            tpScoreFillImage.fillAmount = currentScore / maxScore;
        }

        // Mise à jour du texte (ex: "50/100"). "0" permet d'arrondir à l'entier pour l'affichage.
        if (tpScoreText != null)
        {
            tpScoreText.text = $"{currentScore.ToString("0")}/{maxScore} TP";
        }
    }

    private void UpdateGroupScoreUI()
    {
        float currentScore = playerStats.Get("GroupScore");
        
        if (groupScoreFillImage != null)
        {
            groupScoreFillImage.fillAmount = currentScore / maxScore;
        }

        if (groupScoreText != null)
        {
            groupScoreText.text = $"{currentScore.ToString("0")}/{maxScore} VIBES";
        }
    }
}
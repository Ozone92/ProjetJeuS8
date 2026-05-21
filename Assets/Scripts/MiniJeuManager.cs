using UnityEngine;
using TMPro;
using System.Collections;

public class MiniJeuManager : MonoBehaviour
{
    public static MiniJeuManager Instance;

    [Header("Interface UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public GameObject ecranDemarrage;
    public GameObject texteExplication;

    [Header("Paramètres du Jeu")]
    public GameObject prefabTache;
    public RectTransform zoneDeSpawn;
    public int nombreDeTachesSimultanees = 4;

    [Header("Audio")]
    public AudioSource sourceSFX; // Le haut-parleur pour les bruitages
    public AudioClip sonNettoyage; // Le fichier son à jouer

    public int Score { get; private set; }
    public bool JeuTermine { get; private set; }

    private float tempsRestant = 20f;
    private bool jeuEnCours = false;
    private bool jeuTermine = false;
    private bool finEnAttente = false;
    private bool sequenceDemarrageEnCours = false;
    private TextMeshProUGUI texteBoutonPrincipal;
    private TextMeshProUGUI texteExplicationContent;

    private void Awake()
    {
        Instance = this;
        texteBoutonPrincipal = ecranDemarrage != null ? ecranDemarrage.GetComponentInChildren<TextMeshProUGUI>(true) : null;
        texteExplicationContent = texteExplication != null ? texteExplication.GetComponentInChildren<TextMeshProUGUI>(true) : null;
    }

    private void Start()
    {
        scoreText.text = "Score : 0";
        timerText.text = "Prêt ?";
        SetButtonText("START");
        ecranDemarrage.SetActive(true);
        if (texteExplication != null)
        {
            texteExplication.SetActive(true);
        }
    }

    public void DemarrerJeu()
    {
        if (finEnAttente)
        {
            ConfirmerFin();
            return;
        }

        if (jeuEnCours || sequenceDemarrageEnCours)
        {
            return;
        }

        StartCoroutine(SequenceDeDemarrage());
    }

    IEnumerator SequenceDeDemarrage()
    {
        sequenceDemarrageEnCours = true;
        ecranDemarrage.SetActive(false);
        if (texteExplication != null)
        {
            texteExplication.SetActive(false);
        }

        timerText.text = "3";
        yield return new WaitForSeconds(1f);

        timerText.text = "2";
        yield return new WaitForSeconds(1f);

        timerText.text = "1";
        yield return new WaitForSeconds(1f);

        timerText.text = "GO !";
        yield return new WaitForSeconds(0.5f);

        sequenceDemarrageEnCours = false;
        LancerPartie();
    }

    private void LancerPartie()
    {
        jeuEnCours = true;

        for (int i = 0; i < nombreDeTachesSimultanees; i++)
        {
            FaireApparaitreTache();
        }
    }

    private void Update()
    {
        if (!jeuEnCours || jeuTermine) return;

        tempsRestant -= Time.deltaTime;
        timerText.text = "Temps : " + Mathf.CeilToInt(tempsRestant).ToString() + "s";

        if (tempsRestant <= 0)
        {
            FinDuJeu();
        }
    }

    public void FaireApparaitreTache()
    {
        if (JeuTermine || jeuTermine) return;

        GameObject nouvelleTache = Instantiate(prefabTache, zoneDeSpawn);
        RectTransform rectTache = nouvelleTache.GetComponent<RectTransform>();

        float randomX = Random.Range(zoneDeSpawn.rect.xMin, zoneDeSpawn.rect.xMax);
        float randomY = Random.Range(zoneDeSpawn.rect.yMin, zoneDeSpawn.rect.yMax);

        rectTache.anchoredPosition = new Vector2(randomX, randomY);
    }

    public void AjouterScore(int points)
    {
        if (!jeuEnCours || jeuTermine) return;

        Score += points;
        scoreText.text = "Score : " + Score.ToString();

        // Joue le bruitage de nettoyage à chaque point marqué
        if (sourceSFX != null && sonNettoyage != null)
        {
            sourceSFX.PlayOneShot(sonNettoyage);
        }

        FaireApparaitreTache();
    }

    private void FinDuJeu()
    {
        jeuTermine = true;
        jeuEnCours = false;
        finEnAttente = true;
        tempsRestant = 0;
        timerText.text = GetFinalMessage();
        SetButtonText("SUIVANT");
        ecranDemarrage.SetActive(true);

        if (texteExplicationContent != null)
        {
            texteExplicationContent.text = "Score final : " + Score;
        }

        if (texteExplication != null)
        {
            texteExplication.SetActive(true);
        }

        foreach (Transform child in zoneDeSpawn) {
            Destroy(child.gameObject);
        }

        Debug.Log("Score final : " + Score);
    }

    private void ConfirmerFin()
    {
        finEnAttente = false;
        JeuTermine = true;
        ecranDemarrage.SetActive(false);

        if (texteExplication != null)
        {
            texteExplication.SetActive(false);
        }
    }

    private string GetFinalMessage()
    {
        if (Score >= 50)
        {
            return "Excellent";
        }

        if (Score >= 30)
        {
            return "Bien joue";
        }

        if (Score >= 20)
        {
            return "Moyen";
        }

        return "Peut\u00A0mieux\u00A0faire";
    }

    private void SetButtonText(string text)
    {
        if (texteBoutonPrincipal != null)
        {
            texteBoutonPrincipal.text = text;
        }
    }
}

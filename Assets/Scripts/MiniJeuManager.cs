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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        scoreText.text = "Score : 0";
        timerText.text = "Prêt ?";
        ecranDemarrage.SetActive(true);
        if (texteExplication != null)
        {
            texteExplication.SetActive(true);
        }
    }

    public void DemarrerJeu()
    {
        StartCoroutine(SequenceDeDemarrage());
    }

    IEnumerator SequenceDeDemarrage()
    {
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
        if (JeuTermine) return;

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
        JeuTermine = true;
        tempsRestant = 0;
        timerText.text = "Terminé !";

        foreach (Transform child in zoneDeSpawn) {
            Destroy(child.gameObject);
        }

        Debug.Log("Score final : " + Score);
    }
}

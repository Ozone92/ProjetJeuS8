using UnityEngine;
using TMPro;

public class MiniJeuManager : MonoBehaviour
{
    public static MiniJeuManager Instance;

    [Header("Interface UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    
    [Header("Paramètres du Jeu")]
    public GameObject prefabTache; 
    public RectTransform zoneDeSpawn; 
    public int nombreDeTachesSimultanees = 4; // On veut toujours 4 taches
    
    private int score = 0;
    private float tempsRestant = 15f; 
    private bool jeuTermine = false;

    private void Awake()
    {
        Instance = this; 
    }

    private void Start()
    {
        scoreText.text = "Score : 0";
        
        // AU LANCEMENT : On fait apparaître les 4 premières taches
        for (int i = 0; i < nombreDeTachesSimultanees; i++)
        {
            FaireApparaitreTache();
        }
    }

    private void Update()
    {
        if (jeuTermine) return;

        // Gestion du chronomètre uniquement
        tempsRestant -= Time.deltaTime;
        timerText.text = "Temps : " + Mathf.CeilToInt(tempsRestant).ToString() + "s";

        if (tempsRestant <= 0)
        {
            FinDuJeu();
        }
    }

    public void FaireApparaitreTache()
    {
        // Si le jeu est fini, on ne fait plus apparaître de nouvelles taches
        if (jeuTermine) return;

        GameObject nouvelleTache = Instantiate(prefabTache, zoneDeSpawn);
        RectTransform rectTache = nouvelleTache.GetComponent<RectTransform>();

        float randomX = Random.Range(zoneDeSpawn.rect.xMin, zoneDeSpawn.rect.xMax);
        float randomY = Random.Range(zoneDeSpawn.rect.yMin, zoneDeSpawn.rect.yMax);

        rectTache.anchoredPosition = new Vector2(randomX, randomY);
    }

    public void AjouterScore(int points)
    {
        if (jeuTermine) return;
        
        score += points;
        scoreText.text = "Score : " + score.ToString();

        // REMPLACEMENT IMMEDIAT : 
        // Puisqu'on vient d'en cliquer une, on en crée une nouvelle
        FaireApparaitreTache();
    }

    private void FinDuJeu()
    {
        jeuTermine = true;
        tempsRestant = 0;
        timerText.text = "Terminé !";
        
        // Optionnel : Détruire toutes les taches restantes à l'écran à la fin
        foreach (Transform child in zoneDeSpawn) {
            Destroy(child.gameObject);
        }

        Debug.Log("Score final : " + score);
    }
}
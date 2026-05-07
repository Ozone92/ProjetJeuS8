using UnityEngine;

public class NettoyageUIItem : MonoBehaviour
{
    // Cette fonction est "public" pour que le bouton puisse la déclencher
    public void NettoyerTache()
    {
        // Ajoute 1 point au score via le Manager
        MiniJeuManager.Instance.AjouterScore(1);
        
        // Détruit le bouton (la tache disparaît)
        Destroy(gameObject);
    }
}
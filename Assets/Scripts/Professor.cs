using UnityEngine;

public class Professor : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerStats playerStats;
    
    [SerializeField] private DialogData startDialog;
    [SerializeField] private DialogData happyDialog;
    [SerializeField] private DialogData unhappyDialog;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(startDialog.Play());
    }

    public void Interact()
    {
        var happyStat = playerStats.Get("happy");
        
        if (happyStat > 0)
        {
            StartCoroutine(happyDialog.Play());
        }
        else if (happyStat < 0)
        {
            StartCoroutine(unhappyDialog.Play());
        }
    }
}

using System;
using TMPro;
using UnityEngine;

public class ObjectifManager : MonoBehaviour
{
    [SerializeField]
    private GameObject objectifContainer;
    
    [SerializeField]
    private TMP_Text objectifText;

    [SerializeField] 
    private CycleManager cycleManager;

    private void Update()
    {
        objectifContainer.SetActive(!DialogData.InDialog);

        switch (cycleManager.CurrentCycle)
        {
            case 0:
                objectifText.text = "Discute avec les differents groupes et choisis en un (Deplacement [WASD], Interagir [E])";
                break;
            
            case 1:
                objectifText.text = "Va voir les autres groupes afin d'avoir des idees d'ingredients puis retourne vers ton groupe pour decider de quoi faire.";
                break;
            
            case 2:
                objectifText.text = "Discute avec ton groupe pour commencer le nettoyage.";
                break;
            
            case 3:
                objectifText.text = "Va voir les autres groupes afin de trouver comment gazeifier efficacement puis retourne vers ton groupe pour decider de quoi faire.";
                break;
        }
    }
}

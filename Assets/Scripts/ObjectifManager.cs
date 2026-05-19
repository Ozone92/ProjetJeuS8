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
                objectifText.text = "Discute avec les différents groupes et choisis lequel rejoindre (Déplacement [WASD], Interagir [E])";
                break;
            
            case 1:
                objectifText.text = "Va voir les autres groupes pour trouver des idées d'ingredients puis retourne vers ton groupe pour decider de quoi faire.";
                break;
            
            case 2:
                objectifText.text = "Discute avec ton groupe pour décider qui va s'occuper du nettoyage.";
                break;
            
            case 3:
                objectifText.text = "Va voir les autres groupes pour trouver comment gazeifier efficacement le soda puis retourne vers ton groupe pour decider de quoi faire.";
                break;
        }
    }
}

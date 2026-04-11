using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBoxHandler : MonoBehaviour
{
    public bool ChoiceMade { get; private set; } =  false;
    public string ChoiceIndex { get; private set; } = "";

    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private GameObject buttonContainer;

    public void fill(DialogData.Dialog dialog)
    {
        speakerText.text = dialog.speaker;
        dialogText.text = dialog.text;

        if (dialog.choices.Count == 0)
        {
            GameObject button = TMP_DefaultControls.CreateButton( new TMP_DefaultControls.Resources() );
            button.transform.SetParent(buttonContainer.transform);
            button.GetComponentInChildren<TMP_Text>().text = "Continuer";
            button.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                ChoiceMade = true;
            });
        }
        else
        {
            foreach (var dialogChoice in dialog.choices)
            {
                GameObject button = TMP_DefaultControls.CreateButton( new TMP_DefaultControls.Resources() );
                button.transform.SetParent(buttonContainer.transform);
                button.GetComponentInChildren<TMP_Text>().text = dialogChoice.text != "" ? dialogChoice.text : "Continuer";
                button.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    ChoiceMade = true;
                    ChoiceIndex = dialogChoice.idToGo;
                });
            }
        }
    }
}

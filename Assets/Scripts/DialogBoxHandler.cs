using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogBoxHandler : MonoBehaviour, DialogAction.IDialogActionMapActions
{
    public bool ChoiceMade { get; private set; } = false;
    public string ChoiceIndex { get; private set; } = "";

    private List<DialogData.Choice> choices = null;
    private PlayerStats playerStats = null;

    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private GameObject buttonContainer;

    public void fill(DialogData.Dialog dialog, PlayerStats stats)
    {
        this.choices = dialog.choices;
        this.playerStats = stats;
        
        speakerText.text = dialog.speaker;
        dialogText.text = dialog.text;

        if (dialog.choices == null || dialog.choices.Count == 0)
        {
            GameObject button = TMP_DefaultControls.CreateButton(new TMP_DefaultControls.Resources());
            button.transform.SetParent(buttonContainer.transform);
            button.transform.localEulerAngles = Vector3.zero;
            button.GetComponentInChildren<TMP_Text>().text = "Continuer";
            button.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                ChoiceMade = true;
                ChoiceIndex = "";
            });
        }
        else
        {
            int currentBox = 1;
            foreach (var dialogChoice in dialog.choices)
            {
                bool conditionsOk = true;
                foreach (var condition in dialogChoice.minimalCondition)
                {
                    if (stats.Get(condition.name) < condition.amount)
                    {
                        conditionsOk = false;
                        break;
                    }
                }

                if (!conditionsOk)
                {
                    continue;
                }

                GameObject button = TMP_DefaultControls.CreateButton(new TMP_DefaultControls.Resources());
                button.transform.SetParent(buttonContainer.transform);
                button.transform.localEulerAngles = Vector3.zero;
                button.GetComponentInChildren<TMP_Text>().text =
                    dialogChoice.text != "" ? ($"({currentBox}) {dialogChoice.text}") : "Continuer";
                button.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    foreach (var statToChange in dialogChoice.statsToChange)
                    {
                        stats.Add(statToChange.name, statToChange.amount);
                    }

                    ChoiceMade = true;
                    ChoiceIndex = dialogChoice.idToGo;
                });

                currentBox++;
            }
        }
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        if (!ChoiceMade)
        {
            if (choices is { Count: 1 })
            {
                DialogData.Choice selectedChoice = choices[0];

                foreach (var statToChange in selectedChoice.statsToChange)
                {
                    playerStats.Add(statToChange.name, statToChange.amount);
                }
            
                ChoiceMade = true;
                ChoiceIndex = selectedChoice.idToGo;
            }
            else if (choices is { Count: 0 })
            {
                ChoiceMade = true;
                ChoiceIndex = "";
            }
        }
    }

    public void OnChoice(InputAction.CallbackContext context)
    {
        int pressed = int.Parse(context.control.name);
        if (!ChoiceMade && choices != null && choices.Count >= pressed)
        {
            DialogData.Choice selectedChoice = choices[pressed - 1];

            foreach (var statToChange in selectedChoice.statsToChange)
            {
                playerStats.Add(statToChange.name, statToChange.amount);
            }
            
            ChoiceMade = true;
            ChoiceIndex = selectedChoice.idToGo;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LivesComputer : HackableObject
{
    [SerializeField] private int _requiredChips = 3;

    public override void DisplayIcon(bool display)
    {
        if (isIconDisplaying != display)
        {
            int currentChips = SceneData.Instance.Player.GetComponent<HealthSystem>().CollectedChips;

            Color textColor = currentChips >= _requiredChips ? Color.green : Color.red;

            ImgOnCanvas.transform.Find("RequiredChips").GetComponentInChildren<TextMeshProUGUI>(true).text = $"{currentChips}/{_requiredChips}";
            ImgOnCanvas.transform.Find("RequiredChips").GetComponentInChildren<TextMeshProUGUI>(true).color = textColor;
        }

        base.DisplayIcon(display);
    }

    protected override void OnLoaded_E()
    {
        int currentChips = SceneData.Instance.Player.GetComponent<HealthSystem>().CollectedChips;
        HealthSystem health = SceneData.Instance.Player.GetComponent<HealthSystem>();

        if (currentChips >= _requiredChips)
        {
            if (health.CurrentLives < 3)
            {
                health.RemoveChips(_requiredChips);
                health.AddLives(1);
            }
        }

        base.OnLoaded_E();

        isActive = true;
    }
}
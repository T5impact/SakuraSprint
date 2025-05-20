using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DevHandler : MonoBehaviour
{
    [SerializeField] HorseAttributes[] horseAttributes;
    [SerializeField] GameState gameState;
    [SerializeField] RaceFrontend raceVisuals;
    [SerializeField] GameObject devPanel;
    [SerializeField] TMP_Dropdown horseDropdown;
    [SerializeField] Slider strengthSlider;
    [SerializeField] TMP_Text strengthAmount;
    [SerializeField] Slider staminaSlider;
    [SerializeField] TMP_Text staminaAmount;

    private void Start()
    {
        devPanel.SetActive(false);
    }

    public void ToggleDevPanel()
    {
        devPanel.SetActive(!devPanel.activeSelf);

        if (devPanel.activeSelf) ShowStats();
    }

    public void ShowStats()
    {
        float strength = horseAttributes[horseDropdown.value].HorseStrength;
        float stamina = horseAttributes[horseDropdown.value].HorseStamina;

        strengthSlider.value = strength;
        strengthAmount.text = string.Format("{0:0.00}", horseAttributes[horseDropdown.value].HorseStrength);

        staminaSlider.value = stamina;
        staminaAmount.text = string.Format("{0:0.00}", horseAttributes[horseDropdown.value].HorseStamina);
    }

    public void SetStats()
    {
        horseAttributes[horseDropdown.value].HorseStrength = strengthSlider.value;
        strengthAmount.text = string.Format("{0:0.00}", horseAttributes[horseDropdown.value].HorseStrength);

        horseAttributes[horseDropdown.value].HorseStamina = staminaSlider.value;
        staminaAmount.text = string.Format("{0:0.00}", horseAttributes[horseDropdown.value].HorseStamina);
    }

    public void ResetRace()
    {
        raceVisuals.ResetRace();
    }

    public void ResetPoints()
    {
        gameState.points = 0;
        //raceVisuals.SaveCurrentGame();
        raceVisuals.ResetRace();
    }
}

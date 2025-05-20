using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Horse Attributes", menuName = "Horse Attributes")]
public class HorseAttributes : ScriptableObject
{
    [SerializeField] float horseStrength;
    [SerializeField] float horseStamina;

    public float horseWeight;

    public float HorseStrength { get => horseStrength; set => horseStrength = value; }
    public float HorseStamina { get => horseStamina; set => horseStamina = value; }
}

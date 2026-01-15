using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit_SO", menuName = "Scriptable Objects/Unit_SO")]
public class Unit_SO : ScriptableObject
{
    public string unitName;
    public float unitHP;
    public float unitSP;
    public float unitEXP;

    public Elements[] weakness;
    public Elements[] resistences;

    public List<Ability_SO> abilities;
}
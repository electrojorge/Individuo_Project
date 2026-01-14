using UnityEngine;

[CreateAssetMenu(fileName = "Ability_SO", menuName = "Scriptable Objects/Ability_SO")]
public class Ability_SO : ScriptableObject
{
    public string abilityName;
    public Elements abilityElement;
    public float damage;
}

public enum Elements
{
    Physical, //Físico
    Paranoia, //Paranoia
    Fear, //Miedo
    Guilt, //Culpa
    Obsession, //Obesesión
    Rage, //Ira
    Trauma //Trauma
}

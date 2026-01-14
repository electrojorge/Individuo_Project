using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combat", menuName = "Scriptable Objects/Enemy_SO")]
public class Enemy_SO : ScriptableObject
{
    public string enemyName;
    public float enemyHP;

    public Elements[] weakness;
    public Elements[] resistences;

    public List<Abilities> abilities;
}

public enum Elements
{
    Fire,
    Water,
    Air,
    Ice,
    Normal
}

[System.Serializable]
public class Abilities
{
    public string abilityName;
    public Elements abilityElement;
    public float damage;
}

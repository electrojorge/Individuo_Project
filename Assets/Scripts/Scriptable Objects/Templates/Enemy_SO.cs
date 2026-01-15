using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_SO", menuName = "Scriptable Objects/Enemy_SO")]
public class Enemy_SO : ScriptableObject
{
    public string enemyName;
    public float enemyHP;

    public Elements[] weakness;
    public Elements[] resistences;

    public List<Ability_SO> abilities;
}

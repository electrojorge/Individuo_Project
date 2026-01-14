using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combat", menuName = "ScriptableObjects/CombatScriptableObject")]
public class Combat_SO : ScriptableObject
{
    public string combatName;
    public List<Enemy_SO> enemiesInCombat;
    public GameObject BG;

}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combat_SO", menuName = "Scriptable Objects/Combat_SO")]
public class Combat_SO : ScriptableObject
{
    public string combatName;
    public List<Enemy_SO> enemiesInCombat;
    public GameObject BG;

}

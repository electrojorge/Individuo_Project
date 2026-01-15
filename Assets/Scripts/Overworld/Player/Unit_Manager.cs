using UnityEngine;
using System.Collections.Generic;

public class Unit_Manager : MonoBehaviour
{
    public Unit_SO unitData;
    [SerializeField]
    string unitName;
    [SerializeField]
    float unitHP;
    public void SetHp(float hpMod) => unitHP += hpMod;
    [SerializeField]
    float unitSP;
    public void SetSp(float spMod) => unitSP += spMod;
    [SerializeField]
    float unitEXP;
    public void SetExp(float expMod) => unitEXP += expMod;

    public List<Ability_SO> abilities;

    public void SetAbility(bool add, Ability_SO ability)
    {
        if (add)
        {
            abilities.Add(ability);
        }
        else if (abilities.Contains(ability))
        {
            abilities.Remove(ability);
        }
    }

    private void Start()
    {
        LoadData();
    }

    void LoadData()
    {
        unitName = unitData.unitName;
        unitHP = unitData.unitHP;
        unitSP = unitData.unitSP;
        unitEXP = unitData.unitEXP;
    }
}

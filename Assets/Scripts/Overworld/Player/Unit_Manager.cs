using NUnit.Framework;
using UnityEngine;

public class Unit_Manager : MonoBehaviour
{
    public Unit_SO unitData;

    string unitName;
    float unitHP;
    public void SetHp(float hpMod) => unitHP += hpMod;
    float unitPE;
    public void SetPe(float peMod) => unitPE += peMod;
    float unitEXP;
    public void SetExp(float expMod) => unitEXP += expMod;

    //List<Ability_SO> abilities;

    //public void SetAbility(bool add, Ability_SO ability)
    //{
    //    if (add)
    //    {
    //        abilities.Add(ability);
    //    }
    //    else if (abilities.Contains(ability))
    //    {
    //        abilities.Remove(ability);
    //    }
    //}

    private void Start()
    {
        LoadData();
    }

    void LoadData()
    {
        unitName = unitData.unitName;
        unitHP = unitData.unitHP;
        unitPE = unitData.unitSP;
        unitEXP = unitData.unitEXP;
    }
}

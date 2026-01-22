using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    public List<Unit> unitsList;
    public List<Unit> enemyList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="hp"></param>
    /// <param name="sp"></param>
    /// <param name="exp"></param>
    public void AddUnit(string name, int hp, int sp, float exp)
    {
        unitsList.Add(new Unit(name, hp, sp, exp));
    }
}

[System.Serializable]
public class Unit
{
    public GameObject unitPrefab;
    [Space(10)]
    [Header("RESOURCE STATS")]
    public string unitName;
    public int maxHP;
    public int currentHP;
    [Space(10)]
    public int maxSP;
    public int currentSP;

    [Header("BATTLE STATS")]
    public int physicalATK;
    public int magicalATK;
    public int DEF;

    [Header("EXP CONTROLLER")]
    [Space(10)]
    public float unitEXP;
    [SerializeField] float levelUpCap;
    [SerializeField] float levelUpThresold;

    public int lvl;
    public Unit(string name, int hp, int sp, float exp)
    {
        unitName = name;
        currentHP = hp;
        currentSP = sp;
        unitEXP = exp;
    }
    public void SetHp(int hpMod) => currentHP += hpMod;
    public void SetSp(int spMod) => currentSP += spMod;
    public void SetExp(float expMod)
    {
        unitEXP += expMod;
        if(unitEXP > levelUpCap)
        {
            unitEXP -= levelUpCap;
            lvl++;
            levelUpCap = levelUpCap * levelUpThresold;
            Debug.Log(unitName + " leveled up!");
        }
    } 
}

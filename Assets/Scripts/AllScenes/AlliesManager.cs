using System.Collections.Generic;
using UnityEngine;

public class AlliesManager : MonoBehaviour
{
    public List<Ally> allies;
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
    public void AddAlly(string name, int hp, int sp, float exp)
    {
        allies.Add(new Ally(name, hp, sp, exp));
    }
}

[System.Serializable]
public class Ally
{
    [Header("RESOURCE STATS")]
    public string allyName;
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
    public float allyEXP;
    [SerializeField] float levelUpCap;
    [SerializeField] float levelUpThresold;

    public int lvl;
    public Ally(string name, int hp, int sp, float exp)
    {
        allyName = name;
        currentHP = hp;
        currentSP = sp;
        allyEXP = exp;
    }
    public void SetHp(int hpMod) => currentHP += hpMod;
    public void SetSp(int spMod) => currentSP += spMod;
    public void SetExp(float expMod)
    {
        allyEXP += expMod;
        if(allyEXP > levelUpCap)
        {
            allyEXP -= levelUpCap;
            lvl++;
            levelUpCap = levelUpCap * levelUpThresold;
            Debug.Log(allyName + " leveled up!");
        }
    } 
}

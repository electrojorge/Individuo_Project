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
    public void AddAlly(string name, float hp, float sp, float exp)
    {
        allies.Add(new Ally(name, hp, sp, exp));
    }
}

[System.Serializable]
public class Ally
{
    public string allyName;
    public float allyHP;
    public float allySP;

    [Header("EXP CONTROLLER")]
    [Space(10)]
    public float allyEXP;
    [SerializeField] float levelUpCap;
    [SerializeField] float levelUpThresold;

    public int lvl;
    public Ally(string name, float hp, float sp, float exp)
    {
        allyName = name;
        allyHP = hp;
        allySP = sp;
        allyEXP = exp;
    }
    public void SetHp(float hpMod) => allyHP += hpMod;
    public void SetSp(float spMod) => allySP += spMod;
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

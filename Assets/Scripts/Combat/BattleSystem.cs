using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    START,
    PLAYER_TURN,
    ENEMY_TURN,
    WON,
    LOST
}

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem instance;

    UnitsManager UM; //video brackeys playerPrefab

    public BattleState state;

    public GameObject enemies; //video brackeys enemyPrefab

    public List<Transform> enemyBattlePositions;
    public List<Transform> playerBattlePositions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        UM = Game_Manager.instance.GetComponent<UnitsManager>();

        state = BattleState.START;
    }

    void SetupBattle()
    {
        
    }
}

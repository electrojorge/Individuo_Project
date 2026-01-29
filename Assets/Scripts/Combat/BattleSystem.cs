using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    START,
    PLAYER1_TURN,
    PLAYER2_TURN,
    PLAYER3_TURN,
    PLAYER4_TURN,
    ENEMY1_TURN,
    ENEMY2_TURN,
    ENEMY3_TURN,
    ENEMY4_TURN,
    ENEMY5_TURN,
    WON,
    LOST
}

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem instance;

    UnitsManager UM; //(no hacer caso) video brackeys playerPrefab

    public BattleState state;


    public List<Unit> playerUnits;
    public List<Unit> enemiesInCombat;


    Unit player1;
    Unit player2;
    Unit player3;
    Unit player4;

    Unit enemy1;
    Unit enemy2;
    Unit enemy3;
    Unit enemy4;
    Unit enemy5;

    public GameObject enemies; //(no hacer caso) video brackeys enemyPrefab

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
        SetupBattle();
    }

    void SetupBattle()
    {
        playerUnits = new List<Unit>(UM.unitsTeam);

        //switch (UM.unitsList.Count)
        //{
        //    case 0:
        //        Debug.LogError("No hay players");
        //        break;
        //    case 1:
        //        player1 = UM.unitsList[0];
        //        break;
        //    case 2:
        //        player1 = UM.unitsList[0];
        //        player2 = UM.unitsList[1];
        //        break;
        //    case 3:
        //        player1 = UM.unitsList[0];
        //        player2 = UM.unitsList[1];
        //        player3 = UM.unitsList[2];
        //        break;
        //    case 4:
        //        player1 = UM.unitsList[0];
        //        player2 = UM.unitsList[1];
        //        player3 = UM.unitsList[2];
        //        player4 = UM.unitsList[3];
        //        break;
        //}

        int enemiesNum = Random.Range(1, 6);

        for(int e = 0; e < enemiesNum; e++)
        {
            enemiesInCombat.Add(UM.enemyDex[Random.Range(0, UM.enemyDex.Count)]);
            Debug.Log("bombo");
        }
        //int randomEnemy1 = Random.Range(0, UM.enemyList.Count + 1);
        //int randomEnemy2 = Random.Range(0, UM.enemyList.Count + 1);
        //int randomEnemy3 = Random.Range(0, UM.enemyList.Count + 1);
        //int randomEnemy4 = Random.Range(0, UM.enemyList.Count + 1);
        //int randomEnemy5 = Random.Range(0, UM.enemyList.Count + 1);
        //switch (nemiesNum)
        //{
        //    case 1:
        //        enemy1 = UM.enemyList[randomEnemy1];
        //        break;
        //    case 2:
        //        enemy1 = UM.enemyList[randomEnemy1];
        //        enemy2 = UM.enemyList[randomEnemy2];
        //        break;
        //    case 3:
        //        enemy1 = UM.enemyList[randomEnemy1];
        //        enemy2 = UM.enemyList[randomEnemy2];
        //        enemy3 = UM.enemyList[randomEnemy3];
        //        break;
        //    case 4:
        //        enemy1 = UM.enemyList[randomEnemy1];
        //        enemy2 = UM.enemyList[randomEnemy2];
        //        enemy3 = UM.enemyList[randomEnemy3];
        //        enemy4 = UM.enemyList[randomEnemy4];
        //        break;
        //    case 5:
        //        enemy1 = UM.enemyList[randomEnemy1];
        //        enemy2 = UM.enemyList[randomEnemy2];
        //        enemy3 = UM.enemyList[randomEnemy3];
        //        enemy4 = UM.enemyList[randomEnemy4];
        //        enemy5 = UM.enemyList[randomEnemy5];
        //        break;
        //}

    }
}

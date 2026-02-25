using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    public List<Positions> enemies;

    public GameObject keyDrop;

    EnemyPatrol_Controller EPC;
    private void Awake()
    {
        EPC = EnemyPatrol_Controller.instance;
    }

    private void Start()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!Game_Manager.instance.savedIDs.Contains(i))
            {
                GameObject enemy = Instantiate(enemies[i].enemyPrefab, enemies[i].enemySpawn.position, Quaternion.identity);
                enemy.GetComponent<EnemyPatrol_Controller>().patrolPoints = enemies[i].patrolPoints;
                enemy.GetComponent<EnemyPatrol_Controller>().enemyID = i;
            }
            else
            {
                Instantiate(keyDrop);
            }
        }
    }
}

[System.Serializable]
public class Positions
{
    public GameObject enemyPrefab;
    public Transform enemySpawn;
    public Transform[] patrolPoints;
}

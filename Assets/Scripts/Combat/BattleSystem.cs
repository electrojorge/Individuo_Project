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

    UnitsManager UM;
    CombatHudManager CHM;

    public BattleState state;


    public List<Unit> playerUnits;
    public List<Unit> enemyUnits;

    Unit currentPlayer;
    Unit currentEnemy;

    Unit attackedPlayer;

    public GameObject attackButton;
    public GameObject healButton;

    [SerializeField] float waitTime;


    //Unit player1;
    //Unit player2;
    //Unit player3;
    //Unit player4;

    //Unit enemy1;
    //Unit enemy2;
    //Unit enemy3;
    //Unit enemy4;
    //Unit enemy5;

    //public GameObject enemies; //(no hacer caso) video brackeys enemyPrefab

    //public List<Transform> enemyBattlePositions;
    //public List<Transform> playerBattlePositions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        UM = Game_Manager.instance.GetComponent<UnitsManager>();
        CHM = GetComponent<CombatHudManager>();

        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        playerUnits = new List<Unit>(UM.unitsTeam); //pone los aliados

        int enemiesNum = Random.Range(1, 6);

        for(int e = 0; e < enemiesNum; e++) //pone a los enemigos
        {
            enemyUnits.Add(UM.enemyDex[Random.Range(0, UM.enemyDex.Count)]);
            Debug.Log("bombo");
        }

        yield return new WaitForSeconds(waitTime);

        state = BattleState.PLAYER_TURN;
        currentPlayer = playerUnits[0];
        currentEnemy = enemyUnits[0];
        PlayerTurn();
    }

    public IEnumerator PlayerAttack()
    {
        //bool isDead = ; //a futuro habra que cambiar phys atk, por un ataque bien calculado, de momento se queda asi para probar

        yield return new WaitForSeconds(waitTime);

        if (EnemyTakeDamage(currentPlayer.physicalATK))
        {
            //quitar tambien de BP
            
        }

        if (enemyUnits.Count == 0)
        {
            state = BattleState.WON;
            //EndBattle();
        }
        else if (currentPlayer == playerUnits[^1])
        {
            state = BattleState.ENEMY_TURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            currentPlayer = playerUnits[playerUnits.IndexOf(currentPlayer) + 1];
            PlayerTurn();
        }

        CHM.selectedEnemy = null;
    }

    IEnumerator EnemyTurn()
    {
        bool isDead = PlayerTakeDamage(currentEnemy.physicalATK);
        yield return new WaitForSeconds(waitTime);

        if (isDead)
        {
            playerUnits.Remove(attackedPlayer);
            //quitar tambien de BP
            if (playerUnits.Count == 0)
            {
                state = BattleState.LOST;
                //EndBattle();
            }
            else if (currentEnemy == enemyUnits[^1])
            {
                state = BattleState.PLAYER_TURN;
                PlayerTurn();
            }
            else
            {
                currentEnemy = enemyUnits[enemyUnits.IndexOf(currentEnemy) + 1];
                StartCoroutine(EnemyTurn());
            }

        }
        else
        {
            if (currentEnemy == enemyUnits[^1])
            {
                state = BattleState.PLAYER_TURN;
                PlayerTurn();
            }
            else
            {
                currentPlayer = playerUnits[playerUnits.IndexOf(currentPlayer) + 1];
                StartCoroutine(EnemyTurn());
            }
        }

        attackedPlayer = null;
    }

    void PlayerTurn()
    {
        attackButton.SetActive(true);
        healButton.SetActive(true);
    }

    void EnemyTakeDamage(int dmg)
    {
        CHM.selectedEnemy.currentHP -= dmg;
        if (CHM.selectedEnemy.currentHP <= 0)
            enemyUnits.Remove(CHM.selectedEnemy);
    }

    bool PlayerTakeDamage(int dmg)
    {
        int i = Random.Range(0, playerUnits.Count);
        playerUnits[i].currentHP -= dmg;
        attackedPlayer = playerUnits[i];

        if (attackedPlayer.currentHP <= 0)
            return true;
        else
            return false;
    }
}

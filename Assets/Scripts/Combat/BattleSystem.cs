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


    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();

    Unit currentPlayer;
    Unit currentEnemy;

    Unit attackedPlayer;

    public GameObject attackButton;
    public GameObject healButton;

    [SerializeField] float waitTime;

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
        Debug.Log("Empieza la batalla");
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        playerUnits = new List<Unit>(UM.unitsTeam); //pone los aliados

        for(int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].unitID = i + 1;
        }

        int enemiesNum = Random.Range(1, 6);
        enemyUnits = new List<Unit>();
        for (int e = 0; e < enemiesNum; e++) //pone a los enemigos
        {
            LoadUnitData(UM.GetEnemy(Random.Range(0, UM.enemyDex.Count)),e+1);
        }

        //for (int i = 0; i < enemyUnits.Count; i++)
        //{
        //    enemyUnits[i].unitID = i + 1;
        //}

        Debug.Log(playerUnits.Count + " aliados contra " + enemiesNum + " enemigos");

        yield return new WaitForSeconds(waitTime);

        state = BattleState.PLAYER_TURN;
        Debug.Log("Turno del jugador");
        currentPlayer = playerUnits[0];
        currentEnemy = enemyUnits[0];
        PlayerTurn();
    }
    void LoadUnitData(Unit unitToLoad,int unitID)
    {
        Unit newUnit = new Unit(unitToLoad.unitName,
            unitToLoad.currentHP,unitToLoad.currentSP,
            unitToLoad.unitEXP);
        newUnit.unitPrefab = unitToLoad.unitPrefab;
        newUnit.unitID = unitID;
        enemyUnits.Add(newUnit);
    }
    public IEnumerator PlayerAttack()
    {
        yield return new WaitForSeconds(waitTime);
        
        EnemyTakeDamage(currentPlayer.physicalATK);

        if (enemyUnits.Count == 0)
        {
            state = BattleState.WON;
            Debug.Log("Has ganado la batalla");
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
        Debug.Log("Turno de " + currentEnemy.unitName);

        yield return new WaitForSeconds(waitTime);

        bool isDead = PlayerTakeDamage(currentEnemy.physicalATK);

        if (isDead)
        {
            playerUnits.Remove(attackedPlayer);

            if (playerUnits.Count == 0)
            {
                state = BattleState.LOST;
                Debug.Log("Has perdido la batalla");
                attackedPlayer = null;
                yield break;
            }
        }

        if (currentEnemy == enemyUnits[^1])
        {
            state = BattleState.PLAYER_TURN;
            currentPlayer = playerUnits[0];
            PlayerTurn();
        }
        else
        {
            currentEnemy = enemyUnits[enemyUnits.IndexOf(currentEnemy) + 1];
            StartCoroutine(EnemyTurn());
        }

        attackedPlayer = null;
    }

    void PlayerTurn()
    {
        currentEnemy = enemyUnits[0];
        Debug.Log("Turno de: " + currentPlayer.unitName);
        attackButton.SetActive(true);
        healButton.SetActive(true);
    }

    void EnemyTakeDamage(int dmg)
    {
        CHM.selectedEnemy.currentHP -= dmg;
        Debug.Log("vida de: " + CHM.selectedEnemy.unitName + " ahora es: " + CHM.selectedEnemy.currentHP);
        if (CHM.selectedEnemy.currentHP <= 0)
            enemyUnits.Remove(CHM.selectedEnemy);
    }

    bool PlayerTakeDamage(int dmg)
    {
        int i = Random.Range(0, playerUnits.Count);
        playerUnits[i].currentHP -= dmg;
        attackedPlayer = playerUnits[i];
        Debug.Log("vida de: " + attackedPlayer.unitName + " ahora es: " + attackedPlayer.currentHP);

        if (attackedPlayer.currentHP <= 0)
            return true;
        else
            return false;
    }
}

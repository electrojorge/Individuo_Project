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
    BattlePositioner BP;

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
        BP = GetComponent<BattlePositioner>();

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

        Debug.Log(playerUnits.Count + " aliados contra " + enemiesNum + " enemigos");

        yield return new WaitForSeconds(waitTime);

        state = BattleState.PLAYER_TURN;
        Debug.Log("Turno del jugador");

        // Asignar currentPlayer usando NextCurrentPlayer para respetar la nueva función
        if (playerUnits != null && playerUnits.Count > 0)
            NextCurrentPlayer(playerUnits[0].unitID);

        // Asignar currentEnemy usando NextCurrentEnemy (consistente con la nueva función)
        if (enemyUnits != null && enemyUnits.Count > 0)
            NextCurrentEnemy(enemyUnits[0].unitID);

        PlayerTurn();
    }
    
    void NextCurrentPlayer(int index)
    {
        // Buscar en la lista el primer Unit cuyo unitID coincida con el índice dado.
        // Si no se encuentra, currentPlayer se deja en null y se registra una advertencia.
        currentPlayer = playerUnits.Find(u => u != null && u.unitID == index);
        if (currentPlayer == null)
        {
            Debug.LogWarning($"NextCurrentPlayer: no se encontró playerUnits con unitID == {index}");
        }
    }

    void NextCurrentEnemy(int index)
    {
        // Buscar en la lista el primer Unit cuyo unitID coincida con el índice dado.
        // Si no se encuentra, currentEnemy se deja en null y se registra una advertencia.
        currentEnemy = enemyUnits.Find(u => u != null && u.unitID == index);
        if (currentEnemy == null)
        {
            Debug.LogWarning($"NextCurrentEnemy: no se encontró enemyUnits con unitID == {index}");
        }
    }

    void LoadUnitData(Unit unitToLoad,int unitID)
    {
        // Crear nueva instancia usando el constructor existente y luego copiar el resto de campos
        Unit newUnit = new Unit(unitToLoad.unitName,
            unitToLoad.currentHP, unitToLoad.currentSP,
            unitToLoad.unitEXP);

        // Copiar referencias y estadísticas públicas
        newUnit.unitPrefab = unitToLoad.unitPrefab;
        newUnit.unitID = unitID;

        newUnit.maxHP = unitToLoad.maxHP;
        newUnit.currentHP = unitToLoad.currentHP;

        newUnit.maxSP = unitToLoad.maxSP;
        newUnit.currentSP = unitToLoad.currentSP;

        newUnit.physicalATK = unitToLoad.physicalATK;
        newUnit.magicalATK = unitToLoad.magicalATK;
        newUnit.DEF = unitToLoad.DEF;

        newUnit.lvl = unitToLoad.lvl;

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
            // Usar NextCurrentPlayer para decidir el siguiente currentPlayer según unitID
            NextCurrentPlayer(currentPlayer.unitID + 1);

            if (currentPlayer != null)
                PlayerTurn();
            else
                Debug.LogWarning("PlayerAttack: NextCurrentPlayer devolvió null al avanzar al siguiente jugador.");
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

            // Al volver al turno del jugador, seleccionar el primer jugador mediante NextCurrentPlayer
            if (playerUnits != null && playerUnits.Count > 0)
                NextCurrentPlayer(playerUnits[0].unitID);

            PlayerTurn();
        }
        else
        {
            // Avanzar al siguiente enemigo usando NextCurrentEnemy (mismo patrón que NextCurrentPlayer)
            NextCurrentEnemy(currentEnemy.unitID + 1);

            if (currentEnemy != null)
                StartCoroutine(EnemyTurn());
            else
                Debug.LogWarning("EnemyTurn: NextCurrentEnemy devolvió null al avanzar al siguiente enemigo.");
        }

        attackedPlayer = null;
    }

    void PlayerTurn()
    {
        // Seleccionar primer enemigo mediante NextCurrentEnemy para mantener consistencia
        if (enemyUnits != null && enemyUnits.Count > 0)
            NextCurrentEnemy(enemyUnits[0].unitID);

        if (currentPlayer != null)
            Debug.Log("Turno de: " + currentPlayer.unitName);
        else
            Debug.LogWarning("PlayerTurn: currentPlayer es null.");

        attackButton.SetActive(true);
        healButton.SetActive(true);
    }

    void EnemyTakeDamage(int dmg)
    {
        CHM.selectedEnemy.currentHP -= dmg;
        Debug.Log("vida de: " + CHM.selectedEnemy.unitName + " ahora es: " + CHM.selectedEnemy.currentHP);
        if (CHM.selectedEnemy.currentHP <= 0)
        {
            enemyUnits.Remove(CHM.selectedEnemy);
            Debug.Log("muelto");
            BP.enemiesContainer.transform.GetChild(CHM.selectedEnemy.unitID - 1).gameObject.SetActive(false);
        }
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

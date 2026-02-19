using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        // Buscamos al jugador que tenga el ID igual o más cercano (hacia arriba) al index
        Unit next = null;
        int lowestFoundID = int.MaxValue;

        foreach (Unit u in playerUnits)
        {
            if (u.unitID >= index && u.unitID < lowestFoundID)
            {
                next = u;
                lowestFoundID = u.unitID;
            }
        }
        currentPlayer = next;

        if (currentPlayer == null)
        {
            Debug.Log("No hay más jugadores vivos con ID >= " + index);
        }
    }

    void NextCurrentEnemy(int index)
    {
        // Buscamos al enemigo que tenga el ID igual o más cercano (hacia arriba) al index
        Unit next = null;
        int lowestFoundID = int.MaxValue;

        foreach (Unit u in enemyUnits)
        {
            if (u.unitID >= index && u.unitID < lowestFoundID)
            {
                next = u;
                lowestFoundID = u.unitID;
            }
        }
        currentEnemy = next;

        if (currentEnemy == null)
        {
            Debug.Log("No hay más enemigos vivos con ID >= " + index);
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
            StartCoroutine(EndBattle());
        }
        else
        {
            // Intentamos buscar al siguiente
            NextCurrentPlayer(currentPlayer.unitID + 1);

            if (currentPlayer != null)
            {
                // Si encontró a alguien, sigue el turno del jugador
                PlayerTurn();
            }
            else
            {
                // Si NextCurrentPlayer nos dejó el currentPlayer en null, es que ya no hay más
                state = BattleState.ENEMY_TURN;
                NextCurrentEnemy(1); // Buscamos al primer enemigo vivo (ID 1 o superior)
                StartCoroutine(EnemyTurn());
            }
        }
        CHM.selectedAlly = null;
        CHM.selectedEnemy = null;
    }

    public IEnumerator PlayerHeal()
    {
        yield return new WaitForSeconds(waitTime);

        PlayerGetsHealing(currentPlayer.magicalATK);

        NextCurrentPlayer(currentPlayer.unitID + 1);

        if (currentPlayer != null)
        {
            // Si encontró a alguien, sigue el turno del jugador
            PlayerTurn();
        }
        else
        {
            // Si NextCurrentPlayer nos dejó el currentPlayer en null, es que ya no hay más
            state = BattleState.ENEMY_TURN;
            NextCurrentEnemy(1); // Buscamos al primer enemigo vivo (ID 1 o superior)
            StartCoroutine(EnemyTurn());
        }

        CHM.selectedAlly = null;
        CHM.selectedEnemy = null;
    }

    IEnumerator EnemyTurn()
    {
        // Alineada con PlayerAttack: esperar, aplicar daño mediante función que gestiona la resta y la posible muerte,
        // luego comprobar si quedan jugadores y avanzar el puntero o cambiar turno.
        Debug.Log("Turno de " + currentEnemy.unitName);
        yield return new WaitForSeconds(waitTime);

        // Aplicar daño al jugador (la función ahora gestiona la resta y la eliminación)
        PlayerTakeDamage(currentEnemy.physicalATK);

        // Si ya no quedan jugadores -> perdido
        if (playerUnits.Count == 0)
        {
            state = BattleState.LOST;
            Debug.Log("Has perdido la batalla");
            //GameOver();
            yield break;
        }

        // Intentamos pasar al siguiente enemigo
        NextCurrentEnemy(currentEnemy.unitID + 1);

        if (currentEnemy != null)
        {
            StartCoroutine(EnemyTurn());
        }
        else
        {
            // No quedan más enemigos: volver a turno jugador
            state = BattleState.PLAYER_TURN;
            NextCurrentPlayer(1);
            PlayerTurn();
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

    IEnumerator EndBattle()
    {
        yield return new WaitForSeconds(waitTime);
        Game_Manager.instance.returningFromCombat = true;
        SceneManager.LoadScene("Hospital_Inside");
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

    void PlayerGetsHealing(int heal)
    {
        CHM.selectedAlly.currentHP += heal;
        if (CHM.selectedAlly.currentHP > CHM.selectedAlly.maxHP)
            CHM.selectedAlly.currentHP = CHM.selectedAlly.maxHP;
        Debug.Log("vida de: " + CHM.selectedAlly.unitName + " ahora es: " + CHM.selectedAlly.currentHP);
    }

    // Reestructurada para igualar la arquitectura de EnemyTakeDamage: esta función aplica el daño,
    // notifica y elimina al jugador si muere (sin devolver bool).
    void PlayerTakeDamage(int dmg)
    {
        if (playerUnits == null || playerUnits.Count == 0)
            return;

        int i = Random.Range(0, playerUnits.Count);
        playerUnits[i].currentHP -= dmg;
        attackedPlayer = playerUnits[i];
        Debug.Log("vida de: " + attackedPlayer.unitName + " ahora es: " + attackedPlayer.currentHP);

        if (attackedPlayer.currentHP <= 0)
        {
            // Eliminar jugador y log
            playerUnits.Remove(attackedPlayer);
            Debug.Log(attackedPlayer.unitName + " ha muerto");
            // Si hay UI o contenedores de jugador, aquí es donde deberías desactivarlos.
        }
    }
}

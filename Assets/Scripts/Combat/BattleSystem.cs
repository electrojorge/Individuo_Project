using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

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

    public bool isBoss;
    bool bossSecondTurn;

    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();

    Unit currentPlayer;
    Unit currentEnemy;

    Unit attackedPlayer;

    public GameObject attackButton;
    public GameObject healButton;

    [SerializeField] float waitTime;

    int cameraIndex = 0;

    private void Awake()
    {
        Floating_HealthBar FHB = GetComponentInChildren<Floating_HealthBar>();
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
        playerUnits = new List<Unit>(UM.unitsTeam); //establece los aliados

        for(int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].unitID = i + 1;
        }

        int enemiesNum = Random.Range(1, 6);
        enemiesNum = (isBoss) ? 1 : enemiesNum; // Si es un boss, solo hay un enemigo
        enemyUnits = new List<Unit>();
        for (int e = 0; e < enemiesNum; e++) //establece los enemigos
        {
            LoadUnitData(UM.GetEnemy(Random.Range(0, UM.enemyDex.Count)),e+1);
        }

        for(int i = 0; i < playerUnits.Count; i++)
        {
            if (playerUnits[i].currentHP <= 0)
            {
                playerUnits[i].currentHP = 1;
            }
        }

        for (int i = 0; i < playerUnits.Count; i++)
        {
            Floating_HealthBar bar = CHM.allyBars[i];

            playerUnits[i].healthBar = bar;

            bar.Init(
                playerUnits[i].currentHP,
                playerUnits[i].maxHP
            );
        }

        Debug.Log(playerUnits.Count + " aliados contra " + enemiesNum + " enemigos");

        yield return new WaitForSeconds(waitTime);

        //TurnOrderUI.instance.UpdateTurnOrder(playerUnits, enemyUnits);

        state = BattleState.PLAYER_TURN;
        Debug.Log("Turno del jugador");


        // Asigna al primer player del equipo
        if (playerUnits != null && playerUnits.Count > 0)
            NextCurrentPlayer(playerUnits[0].unitID);

        // Asigna el primer enemigo 
        if (enemyUnits != null && enemyUnits.Count > 0)
            NextCurrentEnemy(enemyUnits[0].unitID);

        PlayerTurn();
    }
    
    void NextCurrentPlayer(int index)
    {
        // Busca al siguiente jugador en la lista segun su ID
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
        //  Busca al siguiente enemigo en la lista segun su ID
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
        // Crear las unidades a partir de las estadisticas de UnitsManager
        Unit newUnit = new Unit(unitToLoad.unitName,
            unitToLoad.currentHP, unitToLoad.currentSP,
            unitToLoad.unitEXP);

        // Resto de stats
        newUnit.unitPrefab = unitToLoad.unitPrefab;
        newUnit.unitID = unitID;

        newUnit.currentHP = unitToLoad.currentHP;
        newUnit.maxHP = unitToLoad.maxHP != 0 ? unitToLoad.maxHP : unitToLoad.currentHP;

        newUnit.maxSP = unitToLoad.maxSP;
        newUnit.currentSP = unitToLoad.currentSP;

        newUnit.physicalATK = unitToLoad.physicalATK;
        newUnit.magicalATK = unitToLoad.magicalATK;
        newUnit.DEF = unitToLoad.DEF;

        newUnit.lvl = unitToLoad.lvl;

        enemyUnits.Add(newUnit);
    }

    public IEnumerator PlayerAttack() // Jugador ataca al enemigo seleccionado
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

        //TurnOrderUI.instance.RotateTurn();
    }

    public IEnumerator PlayerHeal() // Jugador cura al aliado seleccionado
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
        //TurnOrderUI.instance.RotateTurn();
    }

    IEnumerator EnemyTurn() // Turno de enemigo: ataca a un jugador aleatorio, luego pasa al siguiente enemigo o vuelve al jugador si no quedan más
    {
        Debug.Log("Turno de " + currentEnemy.unitName);
        yield return new WaitForSeconds(waitTime);

        // Aplicar daño al jugador
        PlayerTakeDamage(currentEnemy.physicalATK);

        // Si ya no quedan jugadores -> Game Over
        if (playerUnits.Count == 0)
        {
            state = BattleState.LOST;
            Debug.Log("Has perdido la batalla");
            //GameOver();
            yield break;
        }

        // Pasamos turno al siguiente enemigo o al jugador si no quedan más enemigos, o si es un boss, vuelve a atacar

        if (isBoss) // Lógica del segundo turno del boss
        {
            if (!bossSecondTurn)
            {
                // Si es el primer ataque del boss, activamos el bool y repetimos la corrutina
                bossSecondTurn = true;
                Debug.Log("¡El Boss ataca de nuevo!");
                StartCoroutine(EnemyTurn());
            }
            else
            {
                // Si ya es su segundo ataque, reseteamos el bool y pasamos al jugador
                bossSecondTurn = false;
                state = BattleState.PLAYER_TURN;
                NextCurrentPlayer(1); // Volvemos al primer aliado vivo
                PlayerTurn();
            }
        }
        else
        {
            // Lógica para enemigos normales (múltiples)
            NextCurrentEnemy(currentEnemy.unitID + 1);

            if (currentEnemy != null)
            {
                // Si hay otro enemigo en la lista, le toca a él
                StartCoroutine(EnemyTurn());
            }
            else
            {
                // Si no hay más enemigos, turno del jugador
                state = BattleState.PLAYER_TURN;
                NextCurrentPlayer(1); // Volvemos al primer aliado vivo
                PlayerTurn();
            }
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

        NextCamera();
        attackButton.SetActive(true);
        healButton.SetActive(true);
        Debug.Log("MOSTRAR BOTONES");
    }
    void NextCamera()
    {
        // 1. Verificación de seguridad: ¿Hay cámaras en la lista?
        if (BP.cameras == null || BP.cameras.Count == 0) return;

        // 2. Si el índice llegó al final, reseteamos TODO antes de usarlo
        if (cameraIndex >= BP.cameras.Count)
        {
            cameraIndex = 0; // Volvemos al inicio
            for (int i = 0; i < BP.cameras.Count; i++)
            {
                BP.cameras[i].Priority = 0;
            }
        }

        // 3. Ahora sí es seguro acceder porque ya validamos el índice
        BP.cameras[cameraIndex].Priority++;

        // 4. Incrementamos para el próximo turno
        cameraIndex++;
    }

    IEnumerator EndBattle() // Volver a la escena del hospital despues de ganar
    {
        yield return new WaitForSeconds(waitTime);
        Game_Manager.instance.returningFromCombat = true;
        SceneManager.LoadScene("Hospital_Inside");
    }

    void EnemyTakeDamage(int dmg) // funcion del enemigo recibe daño, si muere se elimina de la lista y se desactiva su prefab
    {
        CHM.selectedEnemy.currentHP -= dmg;
        Debug.Log("vida de: " + CHM.selectedEnemy.unitName + " ahora es: " + CHM.selectedEnemy.currentHP);
        if (CHM.selectedEnemy.currentHP <= 0)
        {
            enemyUnits.Remove(CHM.selectedEnemy);
            Debug.Log("muelto");
            BP.enemiesContainer.transform.GetChild(CHM.selectedEnemy.unitID - 1).gameObject.SetActive(false);
            //TurnOrderUI.instance.UpdateTurnOrder(playerUnits, enemyUnits);
        }
        if (CHM.selectedEnemy.healthBar != null)
        {
            CHM.selectedEnemy.healthBar.ShowNumberEvent(dmg, false);
        }
        if (CHM.selectedEnemy.healthBar != null)
        {
            Debug.Log("changing barra" + CHM.selectedEnemy.currentHP + "vida" + CHM.selectedEnemy.maxHP);
            CHM.selectedEnemy.healthBar.UpdateHealthBar(CHM.selectedEnemy.currentHP, CHM.selectedEnemy.maxHP);
        }
    }

    void PlayerGetsHealing(int heal) // funcion que cura al jugador seleccionado.
    {
        CHM.selectedAlly.currentHP += heal;
        if (CHM.selectedAlly.currentHP > CHM.selectedAlly.maxHP)
            CHM.selectedAlly.currentHP = CHM.selectedAlly.maxHP;
        Debug.Log("vida de: " + CHM.selectedAlly.unitName + " ahora es: " + CHM.selectedAlly.currentHP);
        if (CHM.selectedAlly.healthBar != null)
        {
            CHM.selectedAlly.healthBar.ShowNumberEvent(heal, true);
        }
        if (CHM.selectedAlly.healthBar != null)
        {
            CHM.selectedAlly.healthBar.UpdateHealthBar(CHM.selectedAlly.currentHP, CHM.selectedAlly.maxHP);
        }
    }

    void PlayerTakeDamage(int dmg)
    {
        if (playerUnits == null || playerUnits.Count == 0)
            return;

        // 1. Lógica de cálculo de daño
        int extraDmg = Random.Range(0, 10);
        int i = Random.Range(0, playerUnits.Count);
        dmg = (isBoss) ? dmg + extraDmg : dmg;

        // 2. Aplicar daño a la variable numérica
        playerUnits[i].currentHP -= dmg;
        attackedPlayer = playerUnits[i];

        Debug.Log("Vida de: " + attackedPlayer.unitName + " ahora es: " + attackedPlayer.currentHP);

        // 3. ACTUALIZAR UI (Esto es lo que faltaba o estaba fuera de lugar)
        // Lo hacemos antes de comprobar si muere para que el jugador vea la barra bajar a 0
        if (attackedPlayer.healthBar != null)
        {
            attackedPlayer.healthBar.ShowNumberEvent(dmg, false);
            attackedPlayer.healthBar.UpdateHealthBar(attackedPlayer.currentHP, attackedPlayer.maxHP);
        }

        // 4. Comprobar si la unidad ha muerto
        if (attackedPlayer.currentHP <= 0)
        {
            StartCoroutine(HandlePlayerDeath(attackedPlayer));
        }
    }

    // He separado la muerte en una pequeña función para que el código sea más limpio
    IEnumerator HandlePlayerDeath(Unit unit)
    {
        // Esperamos un momento para que el jugador vea la barra en 0 y el texto de daño
        yield return new WaitForSeconds(0.5f);

        // Buscar y eliminar la cámara de la lista de BattlePositioner
        Transform unitTransform = BP.playersContainer.transform.GetChild(unit.unitID - 1);
        CinemachineCamera unitCamera = unitTransform.GetComponentInChildren<CinemachineCamera>();

        if (unitCamera != null)
        {
            BP.cameras.Remove(unitCamera);
            Debug.Log("Cámara de " + unit.unitName + " eliminada.");
        }

        // Desactivar visualmente y eliminar de la lista de combate
        playerUnits.Remove(unit);
        unitTransform.gameObject.SetActive(false);

        if (unit.healthBar != null)
            unit.healthBar.gameObject.SetActive(false);

        // Resetear el índice de cámaras para evitar errores de "fuera de rango"
        cameraIndex = 0;

        Debug.Log(unit.unitName + " ha muerto.");
    }
}
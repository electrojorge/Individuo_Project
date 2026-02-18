using UnityEngine;
using UnityEngine.InputSystem;

public class CombatHudManager : MonoBehaviour
{
    BattleSystem BS;
    public Unit selectedEnemy;
    public Unit selectedAlly;

    //public static event System.Action<Unit> OnUnitSelected;

    private void Start()
    {
        BS = BattleSystem.instance;
        selectedEnemy = null;
        selectedAlly = null;
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("enemigo 1 seleccionado");
            selectedEnemy = BS.enemyUnits[0];
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("enemigo 2 seleccionado");
            selectedEnemy = BS.enemyUnits[1];
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Debug.Log("enemigo 3 seleccionado");
            selectedEnemy = BS.enemyUnits[2];
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            Debug.Log("enemigo 4 seleccionado");
            selectedEnemy = BS.enemyUnits[3];
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            Debug.Log("enemigo 5 seleccionado");
            selectedEnemy = BS.enemyUnits[4];
        }



        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Debug.Log("jugador 1 seleccionado");
            selectedAlly = BS.playerUnits[0];
        }
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            Debug.Log("jugador 2 seleccionado");
            selectedAlly = BS.playerUnits[1];
        }
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("jugador 3 seleccionado");
            selectedAlly = BS.playerUnits[2];
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("jugador 4 seleccionado");
            selectedAlly = BS.playerUnits[3];
        }
    }
    public void AttackButton()
    {
        if (selectedEnemy != null)
        {
            BS.attackButton.SetActive(false);
            BS.healButton.SetActive(false);
            StartCoroutine(BS.PlayerAttack());
            Debug.Log("BOMBASTICO");
        }
    }

    public void HealButton()
    {
        if(selectedAlly != null)
        {
            BS.attackButton.SetActive(false);
            BS.healButton.SetActive(false);
            StartCoroutine(BS.PlayerHeal());
        }
    }
}
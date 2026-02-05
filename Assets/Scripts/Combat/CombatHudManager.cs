using UnityEngine;
using UnityEngine.InputSystem;

public class CombatHudManager : MonoBehaviour
{
    BattleSystem BS;
    public Unit selectedEnemy;

    //public static event System.Action<Unit> OnUnitSelected;

    private void Start()
    {
        BS = BattleSystem.instance;
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            //OnUnitSelected?.Invoke(BS.enemyUnits[0]);
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
    }
    public void AttackButton()
    {
        if (selectedEnemy != null)
        {
            BS.attackButton.SetActive(false);
            BS.healButton.SetActive(false);
            StartCoroutine(BS.PlayerAttack());
        }
    }

    public void HealButton()
    {
        BS.attackButton.SetActive(false);
        BS.healButton.SetActive(false);
        //StartCoroutine(BS.PlayerHeal());
    }
}
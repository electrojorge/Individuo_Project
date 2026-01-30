using UnityEngine;
using UnityEngine.InputSystem;

public class CombatHudManager : MonoBehaviour
{
    BattleSystem BS;
    public Unit selectedEnemy;

    private void Start()
    {
        BS = BattleSystem.instance;
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            selectedEnemy = BS.enemyUnits[0];
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            selectedEnemy = BS.enemyUnits[1];
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            selectedEnemy = BS.enemyUnits[2];
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            selectedEnemy = BS.enemyUnits[3];
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
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
using UnityEngine;

public class SetBoss : MonoBehaviour
{
    UnitsManager UM;
    public Unit boss;
    private void OnEnable()
    {
        UM = Game_Manager.instance.GetComponent<UnitsManager>();
        UM.enemyDex.Clear();
        UM.enemyDex.Add(boss);
        gameObject.GetComponent<BattleSystem>().isBoss = true;
    }
}

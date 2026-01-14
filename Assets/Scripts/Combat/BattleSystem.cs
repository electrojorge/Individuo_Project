using UnityEngine;

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Won,
    Lost
}

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    private void Start()
    {
        state = BattleState.Start;
    }
}

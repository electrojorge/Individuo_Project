using UnityEngine;

public class TriggerMission1 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MissionManager.instance.EnemyAppears();
        }
        Destroy(gameObject);
    }
}

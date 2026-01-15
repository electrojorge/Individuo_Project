using UnityEngine;

public class BattlePositioner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject allyPrefab;
    public GameObject enemyPrefab;

    [Header("Cantidad")]
    public int allyCount = 3;
    public int enemyCount = 4;

    [Header("Posicionamiento")]
    public float sideDistance = 6f;   // Separación entre bandos (X)
    public float unitSpacing = 3f;    // Separación entre unidades (Z)
    public Vector3 battleCenter = Vector3.zero;

    void Start()
    {
        SpawnGroup(allyPrefab, allyCount, -sideDistance, Quaternion.Euler(0, 90, 0));
        SpawnGroup(enemyPrefab, enemyCount, sideDistance, Quaternion.Euler(0, -90, 0));
    }

    void SpawnGroup(GameObject prefab, int count, float xPos, Quaternion rotation)
    {
        if (count <= 0) return;

        float totalWidth = (count - 1) * unitSpacing;
        float startZ = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            float zPos = startZ + i * unitSpacing;

            Vector3 position = battleCenter + new Vector3(
                xPos,
                0f,
                zPos
            );

            Instantiate(prefab, position, rotation);
        }
    }
}

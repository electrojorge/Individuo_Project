using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BattlePositioner : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> allyPrefabs = new List<GameObject>();
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("Posicionamiento")]
    public float sideDistance = 6f;   // Separación entre bandos (X)
    public float unitSpacing = 3f;    // Separación entre unidades (Z)
    public Vector3 battleCenter = Vector3.zero;

    void Start()
    {
        SpawnGroup(allyPrefabs, sideDistance, Quaternion.Euler(0, 90, 0));
        SpawnGroup(enemyPrefabs, -sideDistance, Quaternion.Euler(0, -90, 0));
    }

    void SpawnGroup(List<GameObject> prefabs, float xPos, Quaternion rotation)
    {
        if (prefabs == null) return;

        // Filtrar nulos para calcular el ancho real y evitar errores al instanciar
        var validPrefabs = prefabs.Where(p => p != null).ToList();
        int count = validPrefabs.Count;
        if (count == 0) return;

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

            Instantiate(validPrefabs[i], position, rotation);
        }
    }
}

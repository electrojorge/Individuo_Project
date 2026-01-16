using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BattlePositioner : MonoBehaviour
{
    [Header("Prefabs")]
    // Los aliados ahora se obtienen desde UnitsManager.units (no hay lista pública de allyPrefabs)
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("Posicionamiento")]
    public float sideDistance = 6f;   // Separación entre bandos (X)
    public float unitSpacing = 3f;    // Separación entre unidades (Z)
    public Vector3 battleCenter = Vector3.zero;

    UnitsManager UM; // referencia al manager de unidades

    void Awake()
    {
        UM = Game_Manager.instance.GetComponent<UnitsManager>();
    }

    void Start()
    {
        // Aliados: usar la lista de Units del UnitsManager
        if (UM == null)
        {
            Debug.LogWarning("UnitsManager no encontrado en Game_Manager.instance. No se spawnearán aliados.");
        }
        else
        {
            SpawnGroup(UM.units, sideDistance, Quaternion.Euler(0, 90, 0));
        }

        // Enemigos: lista de prefabs pública (sigue igual)
        SpawnGroup(enemyPrefabs, -sideDistance, Quaternion.Euler(0, -90, 0));
    }

    // Sobrecarga para listas de GameObject (ej. enemigos)
    void SpawnGroup(List<GameObject> prefabs, float xPos, Quaternion rotation)
    {
        if (prefabs == null) return;

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

    // Sobrecarga para listas de Unit (aliados desde UnitsManager)
    void SpawnGroup(List<Unit> units, float xPos, Quaternion rotation)
    {
        if (units == null) return;

        // Extraer prefabs válidos desde cada Unit
        var validPrefabs = units
            .Where(u => u != null && u.unitPrefab != null)
            .Select(u => u.unitPrefab)
            .ToList();

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

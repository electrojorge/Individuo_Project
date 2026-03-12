using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor.Rendering;
using UnityEngine;

public class BattlePositioner : MonoBehaviour
{
    public Vector3 centerPos;
    public float distanceBetweenUnits;
    public float distanceBetweenTeams;

    UnitsManager UM;
    BattleSystem BS;

    public List<Unit> enemiesInCombat;
    public List<Unit> playersInCombat;

    public GameObject enemiesContainer;
    public GameObject playersContainer;

    public List<CinemachineCamera> cameras;

    private void Start()
    {
        UM = Game_Manager.instance.GetComponent<UnitsManager>();
        BS = GetComponent<BattleSystem>();
        // Enemigos en Z=+distanceBetweenTeams
        SetUnits(true, distanceBetweenTeams);
        // Aliados en Z=-distanceBetweenTeams
        SetUnits(false, -distanceBetweenTeams);

        for (int i = 0; i < playersContainer.transform.childCount; i++)
        {
            cameras.Add(playersContainer.transform.GetChild(i).transform.GetChild(0).GetComponent<CinemachineCamera>());
        }
    }

    void SetUnits(bool enemies, float zOffset = 0f)
    {
        List<Unit> unitsToPosition = enemies ? BS.enemyUnits : UM.unitsTeam;
        if (unitsToPosition == null || unitsToPosition.Count == 0) return;

        GameObject parentContainer = enemies ? enemiesContainer : playersContainer;
        Transform parentTransform = parentContainer != null ? parentContainer.transform : null;

        int count = unitsToPosition.Count;
        // ancho total ocupado por las unidades (N-1 espacios)
        float totalWidth = (count - 1) * distanceBetweenUnits;
        // posición X del extremo izquierdo
        float leftX = centerPos.x - totalWidth / 2f;
        float y = centerPos.y;
        float z = centerPos.z + zOffset;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(leftX + i * distanceBetweenUnits, y, z);
            GameObject prefab = unitsToPosition[i].unitPrefab;
            if (prefab != null)
            {
                GameObject inst = Instantiate(prefab, pos, Quaternion.identity, parentTransform);

                Floating_HealthBar hB = inst.GetComponentInChildren<Floating_HealthBar>();

                if (hB != null)
                {
                    unitsToPosition[i].healthBar = hB;
                    hB.Init(unitsToPosition[i].currentHP, unitsToPosition[i].maxHP);
                }
            }
        }
    }
}


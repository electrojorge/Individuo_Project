using System.Collections.Generic;
using UnityEngine;

public class BattlePositioner : MonoBehaviour
{
    public Vector3 centerPos;
    public float distanceBetweenUnits;


    public List<Unit> enemiesInCombat;
    public List<Unit> playersInCombat;

    private void Start()
    {
        SetUnits(true);
    }
    void SetUnits(bool enemies)
    {
        List<Unit> unitsToPosition = enemies ? enemiesInCombat : playersInCombat;

        //for(int i = 0; i < unitsToPosition.Count; i++)
        //{
        //    Instantiate(unitsToPosition[i].unitPrefab, centerPos + new Vector3(distanceBetweenUnits * 0.5f, 0, 0), Quaternion.identity);
        //}

        Vector3 firstPos = centerPos + new Vector3(distanceBetweenUnits * 0.5f, 0, 0);
        Vector3 secondPos = centerPos + new Vector3(distanceBetweenUnits, 0, 0);
        Vector3 thirdPos = centerPos + new Vector3(distanceBetweenUnits * 1.5f, 0, 0);
        Vector3 lastPos = centerPos + new Vector3(distanceBetweenUnits *2, 0, 0);

        switch (unitsToPosition.Count)
        {
            case 1:
                Instantiate(unitsToPosition[0].unitPrefab, centerPos, Quaternion.identity);
                break;
            case 2:
                Instantiate(unitsToPosition[0].unitPrefab, firstPos, Quaternion.identity);
                Instantiate(unitsToPosition[1].unitPrefab, -firstPos, Quaternion.identity);
                break;
            case 3:
                Instantiate(unitsToPosition[0].unitPrefab, centerPos, Quaternion.identity);
                Instantiate(unitsToPosition[1].unitPrefab, secondPos, Quaternion.identity);
                Instantiate(unitsToPosition[2].unitPrefab, -secondPos, Quaternion.identity);
                break;
            case 4:
                Instantiate(unitsToPosition[0].unitPrefab, firstPos, Quaternion.identity);
                Instantiate(unitsToPosition[1].unitPrefab, thirdPos, Quaternion.identity);
                Instantiate(unitsToPosition[2].unitPrefab, -firstPos, Quaternion.identity);
                Instantiate(unitsToPosition[3].unitPrefab, -thirdPos, Quaternion.identity);
                break;
            case 5:
                Instantiate(unitsToPosition[0].unitPrefab, centerPos, Quaternion.identity);
                Instantiate(unitsToPosition[1].unitPrefab, secondPos, Quaternion.identity);
                Instantiate(unitsToPosition[2].unitPrefab, lastPos, Quaternion.identity);
                Instantiate(unitsToPosition[3].unitPrefab, -secondPos, Quaternion.identity);
                Instantiate(unitsToPosition[4].unitPrefab, -lastPos, Quaternion.identity);
                break;
        }
    }
}

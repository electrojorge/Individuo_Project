using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyDropDatabase", menuName = "Game/KeyDropDatabase")]
public class KeyDropDatabase : ScriptableObject
{
    [System.Serializable]
    public class PrefabKeyPair
    {
        public GameObject enemyPrefab;
        public int keyID;
    }

    [Tooltip("Lista persistente de prefabs de enemigo y la key que sueltan.")]
    public List<PrefabKeyPair> entries = new List<PrefabKeyPair>();
}
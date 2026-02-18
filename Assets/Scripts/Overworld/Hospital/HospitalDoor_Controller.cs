using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HospitalDoor_Controller : MonoBehaviour
{
    // Referencia al asset persistente con la configuración prefab -> keyID
    [Tooltip("Asset con la lista persistente de prefabs y keyIDs.")]
    public KeyDropDatabase keyDropDatabase;

    // Llaves recogidas (persisten en el singleton)
    private static HashSet<int> collectedKeys = new HashSet<int>();

    // Singleton
    private static HospitalDoor_Controller instance;
    public static HospitalDoor_Controller Instance => instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    // NO MODIFICAR: funciones usadas por otros sistemas (se dejan exactamente igual)
    public void EnterHospital()
    {
        SceneManager.LoadScene(3);
        Debug.Log("Entering hospital");
    }
    public void ExitHospital()
    {
        SceneManager.LoadScene(2);
        Debug.Log("Exiting hospital");
    }

    // Añade la llave asociada al prefab del enemigo que se ha derrotado.
    // Debes pasar el prefab asset (no la instancia) que configuraste en el KeyDropDatabase.
    public static void CollectKeyFromPrefab(GameObject prefab)
    {
        if (prefab == null) return;

        var db = FindDatabase();
        if (db == null)
        {
            Debug.LogWarning("CollectKeyFromPrefab: no hay KeyDropDatabase asignado en HospitalDoor_Controller.");
            return;
        }

        foreach (var pair in db.entries)
        {
            if (pair == null || pair.enemyPrefab == null) continue;
            if (pair.enemyPrefab == prefab)
            {
                if (pair.keyID > 0 && !collectedKeys.Contains(pair.keyID))
                {
                    collectedKeys.Add(pair.keyID);
                    Debug.Log($"Llave {pair.keyID} recolectada (por prefab {prefab.name}).");
                }
                return;
            }
        }

        Debug.LogWarning($"CollectKeyFromPrefab: no se encontró keyID para el prefab {prefab.name} en la base de datos.");
    }

    // Añade la llave por su ID directamente
    public static void CollectKeyById(int keyId)
    {
        if (keyId <= 0) return;
        if (!collectedKeys.Contains(keyId))
        {
            collectedKeys.Add(keyId);
            Debug.Log($"Llave {keyId} recolectada por ID.");
        }
    }

    // Comprueba si se posee la llave
    public static bool HasKey(int keyId)
    {
        return keyId > 0 && collectedKeys.Contains(keyId);
    }

    public static IEnumerable<int> GetCollectedKeys()
    {
        return collectedKeys;
    }

    // Helpers
    private static HospitalDoor_Controller FindAnyInstance()
    {
        return instance;
    }

    private static KeyDropDatabase FindDatabase()
    {
        var inst = FindAnyInstance();
        if (inst != null && inst.keyDropDatabase != null) return inst.keyDropDatabase;
        return null;
    }
}
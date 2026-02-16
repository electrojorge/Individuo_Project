using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HospitalDoor_Controller : MonoBehaviour
{
    [System.Serializable]
    public class PrefabKeyPair
    {
        public GameObject enemyPrefab;
        public int keyID;
    }

    [Tooltip("Asocia cada prefab de enemigo con el ID de llave que suelta (0 = no suelta).")]
    public List<PrefabKeyPair> prefabKeyDrops = new List<PrefabKeyPair>();

    // LLAVES recogidas (compartido entre todas las instancias)
    private static HashSet<int> collectedKeys = new HashSet<int>();

    [Header("Door settings")]
    [Tooltip("Número de puerta. Si el jugador tiene la llave con este ID la puerta se abre.")]
    public int doorNumber = 0;

    [Tooltip("Animator opcional de la puerta. Si se asigna, se lanzará el trigger indicado al abrir.")]
    public Animator doorAnimator;

    [Tooltip("Nombre del trigger a usar en el Animator para abrir la puerta (si no hay Animator se ignorará).")]
    public string openTriggerName = "Open";

    [Tooltip("GameObject que representa la hoja/obstáculo de la puerta. Si no hay Animator, se desactivará al abrir.")]
    public GameObject doorObject;

    [Tooltip("Collider a desactivar cuando la puerta se abre (por ejemplo para pasar).")]
    public Collider doorCollider;

    // Estado local
    private bool isOpen = false;

    void Start()
    {
        // Sin tracking/detección: el sistema de llaves ahora espera recibir llamadas manuales
    }

    void Update()
    {
        // Sin tracking/detección en Update
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

    // --- Función pública para abrir la puerta si se tiene la llave ---
    public void OpenDoor()
    {
        if (isOpen)
        {
            Debug.Log("La puerta ya está abierta.");
            return;
        }

        if (doorNumber <= 0)
        {
            Debug.LogWarning("DoorNumber no está configurado o es 0. La puerta se abrirá de todos modos.");
            DoOpen();
            return;
        }

        if (HasKey(doorNumber))
        {
            Debug.Log($"Se tiene la llave {doorNumber} -> abriendo puerta.");
            DoOpen();
        }
        else
        {
            Debug.Log($"Puerta cerrada: necesitas la llave {doorNumber} para abrirla.");
        }
    }

    // Acciones concretas para "abrir" la puerta: Animator, desactivar objeto o collider.
    private void DoOpen()
    {
        isOpen = true;

        if (doorAnimator != null && !string.IsNullOrEmpty(openTriggerName))
        {
            doorAnimator.SetTrigger(openTriggerName);
        }
        else
        {
            if (doorObject != null)
                doorObject.SetActive(false);
        }

        if (doorCollider != null)
            doorCollider.enabled = false;
    }

                // --- API simple para recoger llaves (sin detección automática) ---

    // Añade la llave asociada al prefab proporcionado (busca por referencia al prefab).
    // Nota: debes pasar el prefab (asset) tal como lo asignaste en el Inspector en prefabKeyDrops,
    // por ejemplo desde el script del enemigo al morir: HospitalDoor_Controller.CollectKeyFromPrefab(myPrefab);
    public static void CollectKeyFromPrefab(GameObject prefab)
    {
        if (prefab == null) return;

        // Buscar en la lista de pares configurada en alguna instancia activa del script.
        // Buscamos una instancia del componente en escena para acceder a la lista configurada.
        var any = FindAnyInstance();
        if (any == null)
        {
            Debug.LogWarning("CollectKeyFromPrefab: no hay ninguna instancia de HospitalDoor_Controller en la escena para leer la configuración de prefabs.");
            return;
        }

        foreach (var pair in any.prefabKeyDrops)
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

        Debug.LogWarning($"CollectKeyFromPrefab: no se encontró keyID para el prefab {prefab.name}.");
    }

    // Añade la llave por su ID directamente (útil si tu enemigo sabe qué keyID suelta).
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

    // (Opcional) obtener llaves recogidas para UI/depuración
    public static IEnumerable<int> GetCollectedKeys()
    {
        return collectedKeys;
    }

    // Busca cualquier instancia activa del componente en la escena (o null si no existe).
    private static HospitalDoor_Controller FindAnyInstance()
    {
        return Object.FindObjectOfType<HospitalDoor_Controller>();
    }
}

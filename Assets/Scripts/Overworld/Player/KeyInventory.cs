using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class KeyEntry
{
    // Reemplazamos el Key_SO y el bool manual por la referencia al script KeyDrop
    public KeyDrop keyDrop;
}

public class KeyInventory : MonoBehaviour
{
    // Legacy (oculto): entradas antiguas, ahora referenciando al script KeyDrop.
    [SerializeField, HideInInspector]
    private List<KeyEntry> keyEntries = new List<KeyEntry>();

    // Nueva lista simple de IDs (serializada y usada a partir de ahora).
    [SerializeField]
    private List<int> obtainedKeyIDs = new List<int>();

    private HashSet<int> obtainedSet = new HashSet<int>();

    private void Awake()
    {
        MigrateIfNeeded();
        SyncFromList();
    }

    private void OnValidate()
    {
        MigrateIfNeeded();
        SyncFromList();
    }

    // Migra datos legacy (si existen) desde keyEntries -> obtainedKeyIDs.
    private void MigrateIfNeeded()
    {
        if (keyEntries == null || keyEntries.Count == 0) return;

        bool added = false;
        foreach (var e in keyEntries)
        {
            if (e == null) continue;
            if (e.keyDrop == null) continue; // Validamos que el script KeyDrop esté asignado

            // Obtenemos el bool directamente de KeyDrop
            if (!e.keyDrop.obtained) continue;

            // Obtenemos el int (ID) directamente de KeyDrop
            int id = e.keyDrop.keyID;

            if (!obtainedKeyIDs.Contains(id))
            {
                obtainedKeyIDs.Add(id);
                added = true;
            }
        }

        if (added)
        {
            Debug.Log("KeyInventory: migradas entradas legacy a lista de IDs desde KeyDrop.");
        }

        // Si quieres que se borre automáticamente la lista antigua tras migrar, descomenta la siguiente línea:
        // keyEntries.Clear();
    }

    private void SyncFromList()
    {
        obtainedSet = new HashSet<int>(obtainedKeyIDs ?? new List<int>());
    }

    // API pública: ahora solo por int
    public bool HasKey(int id)
    {
        return obtainedSet.Contains(id);
    }

    public void AddKey(int id)
    {
        if (id < 0)
        {
            Debug.LogWarning($"KeyInventory.AddKey recibió un id inválido: {id}");
            return;
        }

        if (obtainedSet.Add(id))
        {
            if (!obtainedKeyIDs.Contains(id))
                obtainedKeyIDs.Add(id);

            Debug.Log($"Llave {id} añadida al inventario.");
        }
    }

    public int[] GetKeys()
    {
        return obtainedSet.ToArray();
    }

    // Utilidad para UI / inspector
    public List<int> GetKeysList()
    {
        return new List<int>(obtainedKeyIDs);
    }
}
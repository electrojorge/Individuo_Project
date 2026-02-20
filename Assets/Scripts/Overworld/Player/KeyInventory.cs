using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyEntry
{
    public Key_SO key;
    public bool obtained;
}

public class KeyInventory : MonoBehaviour
{
    [SerializeField]
    private List<KeyEntry> keyEntries = new List<KeyEntry>();

    // Conjunto de consulta rápida basado en keyID
    private HashSet<int> obtainedSet = new HashSet<int>();

    private void Awake()
    {
        SyncFromEntries();
    }

    // Se ejecuta en el editor cuando se modifican valores en el Inspector
    private void OnValidate()
    {
        SyncFromEntries();
    }

    // Sincroniza el HashSet con las entradas editables
    public void SyncFromEntries()
    {
        obtainedSet = new HashSet<int>(
            keyEntries
                .Where(e => e != null && e.key != null && e.obtained)
                .Select(e => e.key.keyID)
        );
    }

    // Comprueba si el jugador tiene la llave por ID o por Key_SO
    public bool HasKey(int id)
    {
        return obtainedSet.Contains(id);
    }

    public bool HasKey(Key_SO key)
    {
        if (key == null) return false;
        return HasKey(key.keyID);
    }

    // Marca la llave como obtenida. Si no existe la entrada en la lista, se añade al conjunto
    // y se avisa para que añadas el Key_SO al Inspector (recomendado).
    public void AddKey(int id)
    {
        var entry = keyEntries.FirstOrDefault(e => e != null && e.key != null && e.key.keyID == id);
        if (entry != null)
        {
            if (!entry.obtained)
            {
                entry.obtained = true;
                obtainedSet.Add(id);
                Debug.Log($"KeyInventory: llave marcada como obtenida {id}");
            }
            return;
        }

        // Compatibilidad: añadimos al conjunto aunque no exista la entrada.
        if (obtainedSet.Add(id))
        {
            Debug.LogWarning($"KeyInventory: no existe Key_SO para la llave {id} en 'keyEntries'. Se ha marcado como obtenida en memoria. Añade el Key_SO en el Inspector para gestión completa.");
        }
    }

    public void AddKey(Key_SO key)
    {
        if (key == null)
        {
            Debug.LogWarning("KeyInventory.AddKey recibió null.");
            return;
        }
        AddKey(key.keyID);
    }

    // Marca la llave como no obtenida (si existe la entrada). Si no existe, se intenta eliminar del conjunto.
    public bool RemoveKey(int id)
    {
        var entry = keyEntries.FirstOrDefault(e => e != null && e.key != null && e.key.keyID == id);
        if (entry != null)
        {
            if (entry.obtained)
            {
                entry.obtained = false;
                obtainedSet.Remove(id);
                Debug.Log($"KeyInventory: llave marcada como no obtenida {id}");
                return true;
            }
            return false;
        }

        if (obtainedSet.Remove(id))
        {
            Debug.LogWarning($"KeyInventory: la llave {id} no tenía entrada en 'keyEntries' pero fue eliminada del conjunto en memoria.");
            return true;
        }

        Debug.LogWarning($"KeyInventory.RemoveKey: no existe la llave {id}.");
        return false;
    }

    // Devuelve los IDs de las llaves obtenidas
    public int[] GetKeys()
    {
        return obtainedSet.ToArray();
    }

    // Devuelve todas las entradas (útil para UI/editor)
    public KeyEntry[] GetAllEntries()
    {
        return keyEntries.ToArray();
    }
}
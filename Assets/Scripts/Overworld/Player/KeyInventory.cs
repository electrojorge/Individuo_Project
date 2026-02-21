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

    // Sincroniza el HashSet de llaves obtenidas a partir de la lista de entradas
    public void SyncFromEntries()
    {
        // Crea una nueva lista con las entradas que no son null, tienen una llave asignada y están marcadas como obtenidas
        obtainedSet = new HashSet<int>(
            keyEntries
                .Where(e => e != null && e.key != null && e.obtained) // Filtra solo las entradas válidas y obtenidas
                .Select(e => e.key.keyID) // Extrae el ID de la llave de cada entrada filtrada
        );
    }

    // Comprueba si el jugador tiene la llave por ID o por el scriptable object
    public bool HasKey(int id)
    {
        return obtainedSet.Contains(id);
    }

    public bool HasKey(Key_SO key)
    {
        if (key == null) return false;
        return HasKey(key.keyID);
    }

    // Añade una llave al jugador y la marca como obtenida
    public void AddKey(int id)
    {
        var entry = keyEntries.FirstOrDefault(e => e != null && e.key != null && e.key.keyID == id);
        if (entry != null)
        {
            if (!entry.obtained)
            {
                entry.obtained = true;
                obtainedSet.Add(id);
                Debug.Log($"Llave {id} marcada como obtenida");
            }
            return;
        }

        // Se añade la llave aunque no exista la entrada.
        if (obtainedSet.Add(id))
        {
            Debug.LogWarning($"No hay un Key_SO para la llave {id}");
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

    // Devuelve los IDs de las llaves obtenidas
    public int[] GetKeys()
    {
        return obtainedSet.ToArray();
    }

    // Devuelve todas las entradas. (Para el UI)
    public KeyEntry[] GetAllEntries()
    {
        return keyEntries.ToArray();
    }
}
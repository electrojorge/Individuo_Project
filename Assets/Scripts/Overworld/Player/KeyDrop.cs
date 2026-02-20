using UnityEngine;

public class KeyDrop : MonoBehaviour
{
    public KeyInventory KI;
    public int keyID;

    void Start()
    {
        keyID = GetComponent<Key_SO>().keyID;
    }
    void OnTriggerEnter(Collider other)
    {
        KI.AddKey(keyID);
    }
}

using UnityEngine;

public class KeyDrop : MonoBehaviour
{
    public KeyInventory KI;
    public int keyID;
    public bool obtained;

    void Start()
    {
        KI = Game_Manager.instance.GetComponent<KeyInventory>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        KI.AddKey(keyID);
        Destroy(this.gameObject);
    }
}

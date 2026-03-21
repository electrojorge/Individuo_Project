using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBoss : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene("BossFight");
    }
}

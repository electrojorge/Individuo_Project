using NUnit.Framework;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HospitalDoor_Controller : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
}

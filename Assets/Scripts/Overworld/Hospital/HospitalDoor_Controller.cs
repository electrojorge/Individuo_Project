using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HospitalDoor_Controller : MonoBehaviour
{
    public void EnterHospital()
    {
        SceneManager.LoadScene(4);
        Debug.Log("Entering hospital");
    }
    public void ExitHospital()
    {
        SceneManager.LoadScene(3);
        Debug.Log("Exiting hospital");
    }
}
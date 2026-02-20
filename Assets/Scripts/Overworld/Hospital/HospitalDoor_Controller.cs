using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HospitalDoor_Controller : MonoBehaviour
{
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
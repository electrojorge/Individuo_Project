using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HospitalDoor_Controller : MonoBehaviour
{
    // Llaves recogidas (persisten en el singleton)
    private static HashSet<int> collectedKeys = new HashSet<int>();

    // Singleton
    private static HospitalDoor_Controller instance;
    public static HospitalDoor_Controller Instance => instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
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
}
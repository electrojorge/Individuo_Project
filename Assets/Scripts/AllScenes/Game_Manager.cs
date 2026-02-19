using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager instance;

    public int? savedID;

    private Vector3 playerPos;
    public Vector3 PlayerPos  {get{return playerPos;} set{playerPos = value;}}

    public bool returningFromCombat;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Hospital_Inside" && returningFromCombat)
        {
            Debug.Log("volvemos al overworld");
            Player_Controller player = FindFirstObjectByType<Player_Controller>();
            player.GetRigidbody().position = playerPos;
            player.transform.position = playerPos;
            returningFromCombat = false;
        }
    }
}

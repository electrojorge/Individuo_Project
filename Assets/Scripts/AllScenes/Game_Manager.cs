using UnityEngine;
using System.Collections.Generic;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager instance;

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
  
}

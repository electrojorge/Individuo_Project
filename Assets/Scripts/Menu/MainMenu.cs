using System.Collections;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionsMenuPanel;

    public GameObject pointLights;
    public GameObject mainMenuPlane;

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        pointLights.SetActive(true);
        mainMenuPlane.SetActive(true);
    }

    public void StartGameButton()
    {
        SceneManager.LoadScene(3);
    }

    public void OptionsButton()
    {
        mainMenuPanel.SetActive(false);
        pointLights.SetActive(false);
        mainMenuPlane.SetActive(true);
        optionsMenuPanel.SetActive(true);
    }

    public void BackToMainMenuButton()
    {
        optionsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        pointLights.SetActive(true);
        mainMenuPlane.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}

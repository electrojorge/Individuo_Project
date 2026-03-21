using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject controllPauseMenu;

    private void Start()
    {
        pauseMenu.SetActive(false);
        controllPauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.isPressed)
        {
            ShowCursor();
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        HideCursor();
    }
     public void ExitToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void RestarGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(3);
    }

    public void ControllMenu()
    {
        pauseMenu.SetActive(false);
        controllPauseMenu.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        controllPauseMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

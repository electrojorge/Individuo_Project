using System.Collections;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject mainMenuPanel;
    public GameObject optionsMenuPanel;
    public GameObject loadingMenuPanel;

    public Slider loadingSlider;
    public TextMeshProUGUI loadingText;

    bool started = false;

    private void Start()
    {
        startScreen.SetActive(true);
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        loadingMenuPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame && !started)
        {
            started = true;
            StartCoroutine(ShowMainMenuDelay());
        }
    }

    IEnumerator ShowMainMenuDelay()
    {
        yield return new WaitForSeconds(1f);
        startScreen.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void StartGameButton()
    {
        StartCoroutine(StartGameDelay());
    }

    IEnumerator StartGameDelay()
    {
        mainMenuPanel.SetActive(false);
        loadingMenuPanel.SetActive(true);

        StartCoroutine(LoadingDots());

        float duration = 5f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            loadingSlider.value = Mathf.SmoothStep(0, 1, time / duration);
            yield return null;
        }

        loadingSlider.value = 1f;

        StopCoroutine("LoadingDots");
        loadingText.text = "Loading complete";
        SceneManager.LoadScene(1);
    }

    IEnumerator LoadingDots()
    {
        string baseText = "Loading game";
        int dots = 0;

        while (true)
        {
            dots = (dots + 1) % 4;
            loadingText.text = baseText + new string('.', dots);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void OptionsButton()
    {
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
    }

    public void BackToMainMenuButton()
    {
        optionsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}

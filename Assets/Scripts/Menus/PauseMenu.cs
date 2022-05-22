using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    private InputManager inputManager;
    public AudioMixer audioMixer;
    public GameObject pauseMenuUI;
    private void Start()
    {
        inputManager = InputManager.Instance;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
    private void Update()
    {
        if (inputManager.PlayerPausedGame())
        {
            if(!pauseMenuUI.activeInHierarchy)
            {
                Pause();
            }
        }
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
    }
}

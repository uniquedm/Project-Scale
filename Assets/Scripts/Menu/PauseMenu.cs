using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Boolean isPaused;
    public GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause(!isPaused);
        }
    }

    public void TogglePause(Boolean isPaused)
    {
        this.isPaused = isPaused;
        if (!isPaused) {
            // Lock the mouse cursor to the game screen.
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        Time.timeScale = isPaused ? 0.0f : 1.0f;
        pauseMenu.SetActive(isPaused);
    }

    public void RestartLevel()
    {
        TogglePause(false);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Boolean isPaused;
    public GameObject pauseMenu;
    public GameObject mainMenu;
    public List<GameObject> otherMenus;
    // Hotfix: For Workbench UI Issue with Pause Menu
    public GameObject workbenchCamera;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
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
        if (!isPaused && !workbenchCamera.activeSelf) {
            // Lock the mouse cursor to the game screen.
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        Time.timeScale = isPaused ? 0.0f : 1.0f;
        pauseMenu.SetActive(isPaused);
        mainMenu.SetActive(isPaused);
        if (otherMenus.Count > 0) {
            foreach (GameObject otherMenu in otherMenus)
            {
                otherMenu.SetActive(false);

            }
        }
    }

    public void RestartLevel()
    {
        TogglePause(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

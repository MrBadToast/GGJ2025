using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject visualGroup;
    [SerializeField] private KeyCode pauseKey;

    bool paused = false;

    private void Update()
    {
        if(Input.GetKeyDown(pauseKey))
        {
            if (GameplaySystem.Instance.GameState != GameplayState.Main)
                return;

            if(!paused)
            {
                paused = true;
                Time.timeScale = 0f;
                visualGroup.SetActive(true);
            }
            else
            {
                ResumeGame();
            }

        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        Time.timeScale = 1.0f;
        SceneLoader.Instance.LoadNewScene("MainGame");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        paused = false;
        visualGroup.SetActive(false);
    }
}

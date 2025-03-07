using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBehaivor : MonoBehaviour
{
    [SerializeField] private string mainGameScene;
    public void LoadMainScene()
    {
        SceneLoader.Instance.LoadNewScene(mainGameScene);
    }

    public void LoadFirstScene()
    {
        SceneLoader.Instance.LoadNewScene("Title");
    }

    public void Quit()
    {
        Application.Quit();
    }
}

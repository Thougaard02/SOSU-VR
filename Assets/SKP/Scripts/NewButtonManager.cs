using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewButtonManager : MonoBehaviour
{
    public NewVideoManager manager;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void ResumeButton()
    {
        //StartCoroutine(manager.StartPlayingFirstVideo());
        manager.StartPlayingFirstVideo();
    }

    public void FirstButton()
    {
        manager.FirstButtonPressed();
    }

    public void SecondButton()
    {
        manager.SecondButtonPressed();

    }
    public void ThirdButton()
    {
        manager.ThirdButtonPressed();

    }
    public void StartPlayScene()
    {
        SceneManager.LoadScene(1);
    }
}

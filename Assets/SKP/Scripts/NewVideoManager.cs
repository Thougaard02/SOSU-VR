using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.Video;

public class NewVideoManager : MonoBehaviour, OVRGrabber.IPickedUp
{
    public ClipHolder[] clips;
    public VideoPlayer videoPlayer;

    // Buttons that user can click on
    public Button[] btns;

    // Text on buttons
    public GameObject infomationTextBtn;

    public TextMeshProUGUI firstText;
    public TextMeshProUGUI secondText;
    public TextMeshProUGUI thirdText;

    // Notification that pops out when the wrong or correct answear was clicked
    public TextMeshProUGUI notificationText;
    public GameObject canvas;
    public GameObject environment;

    public GameObject Laser;

    private ClipHolder clipVideo;

    public GameObject objectiveText;

    //public TextMeshProUGUI debugText;


    private void Start()
    {
        Laser.SetActive(false);
        // If notification was active turn it off
        ShowNotficationText(false);
        ShowCanvas(false);
        OVRGrabber.PickUpManager.AddLisinger(this);
    }


    string getPath(string vidoeName)
    {
        string path = "";
        try
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                string rootPath = Application.persistentDataPath;
                path = Path.Combine(rootPath, vidoeName);
                //debugText.text = path;
            }
        }
        catch (Exception i)
        {
            //debugText.text = i.Message;
        }
        return path;
    }


    void WaitForVideosBreak()
    {
        ShowCanvas(true);
        ShowButtons(true);
        ShowMesh(false);
        SetText();
    }

    public void StartPlayingFirstVideo()
    {
        ShowCanvas(false);
        ShowMesh(false);
        infomationTextBtn.SetActive(false);
        ShowNotficationText(false);

        StartCoroutine(PlayFirstVideo());
    }

    IEnumerator PlayFirstVideo()
    {
        if (clipVideo.startClip != "")
        {
            videoPlayer.url = getPath(clipVideo.startClip);
            videoPlayer.Play();
            yield return new WaitForSeconds(clipVideo.startClipTime);
            videoPlayer.Stop();
        }

        WaitForVideosBreak();
    }

    public void BeforeVideoStarts()
    {
        ShowCanvas(true);
        ShowButtons(false);
        ShowNotficationText(false);
        infomationTextBtn.SetActive(true);
    }

    void ShowCanvas(bool _value)
    {
        canvas.SetActive(_value);
        Laser.SetActive(_value);
    }

    void ShowButtons(bool _value)
    {
        for (int i = 0; i < btns.Length; i++)
        {
            btns[i].gameObject.SetActive(_value);
        }
    }

    void ShowMesh(bool _value)
    {
        MeshRenderer[] renders = environment.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] skinnedMeshes = environment.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].enabled = _value;
        }
        for (int i = 0; i < skinnedMeshes.Length; i++)
        {
            skinnedMeshes[i].enabled = _value;
        }
        objectiveText.SetActive(_value);
    }

    // Set's button text according the the clip text, set in playing clip 
    public void SetText()
    {
        if (clipVideo != null)
        {
            infomationTextBtn.GetComponentInChildren<TextMeshProUGUI>().text = clipVideo.infomationText;
            firstText.text = clipVideo.firstText;
            secondText.text = clipVideo.secondText;
            thirdText.text = clipVideo.thirdText;
        }
    }

    public void FirstButtonPressed()
    {
        StartCoroutine(SetAfterButtonPressed(clipVideo.videoClips[0]));
    }

    public void SecondButtonPressed()
    {
        StartCoroutine(SetAfterButtonPressed(clipVideo.videoClips[1]));
    }

    public void ThirdButtonPressed()
    {
        StartCoroutine(SetAfterButtonPressed(clipVideo.videoClips[2]));
    }

    /// <summary>
    /// After button was pressed we take clip corresponding the the button and
    /// set default values accordingly
    /// </summary>
    /// <param name="_clip"></param>
    IEnumerator SetAfterButtonPressed(Clip _clip)
    {
        videoPlayer.Stop();
        videoPlayer.url = getPath(_clip.clip);

        // not sure if u should play video here ???
        videoPlayer.Play();
        ShowCanvas(false);
        ShowMesh(false);
        //yield return new WaitForSeconds((float)time);
        yield return new WaitForSeconds(_clip.clipTime);
        videoPlayer.Stop();
        ShowAnswearAfterTime(_clip);
    }

    void ShowAnswearAfterTime(Clip clip)
    {
        ShowCanvas(true);
        SetNotificationText(clip.DisplayText);

        if (clip.isCorrectAnswear)
        {
            // Change color of the text for green
            StartCoroutine(HideAnswearAndCanvas());
        }
        else
        {
            StartCoroutine(ShowCanvasAfterTime(2));
        }
        ShowNotficationText(true);
    }

    IEnumerator ShowCanvasAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        ShowCanvas(true);
        ShowButtons(true);
        ShowNotficationText(false);
    }

    void SetNotificationText(string _text)
    {
        ShowCanvas(true);
        ShowButtons(false);

        notificationText.text = _text;
    }

    void ShowNotficationText(bool _value)
    {
        notificationText.gameObject.SetActive(_value);
    }

    IEnumerator HideAnswearAndCanvas()
    {
        yield return new WaitForSeconds(3f);

        if (clipVideo.nextClipName != "")
        {
            SetNextClipHolder(clipVideo.nextClipName);

            SetText();
            BeforeVideoStarts();
        }
        else
        {
            ShowCanvas(false);
            ShowMesh(true);
        }
    }

    public void PickedUp(GameObject _gameObject)
    {
        if (_gameObject.GetComponent<NameOfObjects>().ObjectName != null)
        {
            SetNextClipHolder(_gameObject.GetComponent<NameOfObjects>().ObjectName);
            SetText();
            BeforeVideoStarts();
            ShowMesh(false);
            Laser.SetActive(true);
            ShowNotficationText(true);
        }
    }

    public void SetNextClipHolder(string _name)
    {
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].ClipName == _name)
            {
                clipVideo = clips[i];
            }
        }
    }
}
[System.Serializable]
public class ClipHolder
{
    public string ClipName;
    public Clip[] videoClips;
    public string startClip;
    public string infomationText;
    public string firstText;
    public string secondText;
    public string thirdText;

    // Play video for this amount of seconds
    public float startClipTime;

    public string nextClipName;
}
[System.Serializable]
public class Clip
{
    public string clip;
    public string DisplayText;
    public bool isCorrectAnswear;
    public float clipTime;
}
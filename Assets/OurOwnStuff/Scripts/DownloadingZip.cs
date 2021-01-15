using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using ICSharpCode.SharpZipLib.Zip;
using System;
using UnityEngine.UI;
using TMPro;

public class DownloadingZip : MonoBehaviour
{
    //string downloadLink = "https://cdn.discordapp.com/attachments/547746994987204610/798126397419421706/OurFiles.zip";
    string downloadLink = "https://cdn.discordapp.com/attachments/547746994987204610/798478519352688640/OurFilesZip.zip";
    UnityWebRequest unityWebRequest;

    public Slider slider;
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI progressText;
    public GameObject button;

    // Start is called before the first frame update
    void Start()
    {
        unityWebRequest = new UnityWebRequest(downloadLink, UnityWebRequest.kHttpVerbGET);
        DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
        unityWebRequest.downloadHandler = downloadHandler;

        StartCoroutine(Download(downloadLink, true));
    }



    void Update()
    {
        if (!unityWebRequest.downloadHandler.isDone)
        {
            slider.value = unityWebRequest.downloadProgress * 100;
            progressText.text = "Progress " + (unityWebRequest.downloadProgress * 100);
           // Debug.Log("Bytes " +  (unityWebRequest.));
        }
    }


    IEnumerator Download(string url, bool remove)
    {
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.downloadHandler.isDone)
        {
            byte[] data = unityWebRequest.downloadHandler.data;

            string docPath = Application.persistentDataPath;

            ZipFile.UnZip(docPath, data);

            try
            {
                if (remove)
                {
                    // delete zip file.
                    File.Delete(docPath);
                }
            }
            catch (Exception e)
            {
                print(e.Message);
            }
            finally
            {
                Finish();
            }

        }
    }

    void Finish()
    {
        mainText.text = "Dowloading færdig. Tryk på \"Start spil\" for at komme ind i spillet";
        button.SetActive(true);
        slider.gameObject.SetActive(false);
    }
}


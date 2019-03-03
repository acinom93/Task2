using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NeverSayNo : MonoBehaviour
{
    [SerializeField]
    private Text message = null;

    [SerializeField]
    private Text dateText = null;

    [SerializeField]
    private Text instruct = null;

    [SerializeField]
    private RawImage rawImage;

    public InputField usrMessage;

    
    void Awake()
    {
        instruct.text = "Type a message and hit post to display it! Just never say NO...";
        SavedInfoObj savedObj = Load();

        if (savedObj!=null && savedObj.msg != null && savedObj.msg.Length != 0)
        {
            ShowTexts(savedObj.msg, savedObj.date);
        }
        else
        {
            ShowImage();
        }
    }

    private static SavedInfoObj Load()
    {
        SavedInfoObj savedObj = null;
        try
        {
            String json = File.ReadAllText(Application.dataPath + "/save.txt");
            savedObj = JsonUtility.FromJson<SavedInfoObj>(json);
        }
        catch
        {
            Debug.LogError("Error occurred in read file");
        }
        return savedObj;
    }

    public void Mclick()
    {
        if (usrMessage.text.Length == 0)
        {
            instruct.text = "Please enter a message before hitting the Post button...";
        }
        else if ( HasNo(usrMessage.text.ToLower()) )
        {
            instruct.text = "OOPs! Don't use the word *No* in your message...";
            ShowImage();
        }
        else
        {
            instruct.text = "Type a message and hit post to display it! Just never say NO...";
            ShowTexts(usrMessage.text,GetDate());
        }

        Save();
        usrMessage.text = null;
        usrMessage.ActivateInputField();
    }

    private void ShowImage()
    {
        rawImage.enabled = true;
        message.text = "";
        dateText.text = "";
        StartCoroutine(DownloadImage("https://picsum.photos/100/100/?random"));
    }

    private void ShowTexts(String msg, String date)
    {
        rawImage.enabled = false;
        message.text = msg;
        dateText.text = date;
    }

    private void Save()
    {
        SavedInfoObj saveObj = new SavedInfoObj(message.text,dateText.text);
        string json = JsonUtility.ToJson(saveObj);
        File.WriteAllText(Application.dataPath + "/save.txt", json);
        SavedInfoObj savedObj = JsonUtility.FromJson<SavedInfoObj>(json);
    }

    private static bool HasNo(string str)
    {
        if (Regex.IsMatch(str, @"\bno\b"))
        {
            return true;
        }
        else
            return false;
    }

    private static string GetDate()
    {
        return DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    private class SavedInfoObj
    {
        public string msg;
        public string date;

        public SavedInfoObj(string str1, string str2)
        {
            msg = str1;
            date = str2;
        }
    }
}

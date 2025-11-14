using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GASSender : MonoBehaviour
{
    public string deployId; // Inspectorから設定可能に
    public string questionText; // Inspectorから設定する質問テキスト

    // 入力欄
    [SerializeField] InputField inputField;
    public void SendQuestion()
    {
        StartCoroutine(CallGoogleAppsScript());
    }

    public void Send60Question()
    {
        for (int i = 0; i < 60; i++)
        {
            StartCoroutine(CallGoogleAppsScript());
        }
    }


    IEnumerator CallGoogleAppsScript()
    {
        string url = $"https://script.google.com/macros/s/{deployId}/exec?question=" + UnityWebRequest.EscapeURL(inputField.text);
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("エラー: " + request.error);
        }
        else
        {
            Debug.Log("レスポンス: " + request.downloadHandler.text);
        }
    }

}

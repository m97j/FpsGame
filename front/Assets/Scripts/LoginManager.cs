using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public InputField id;
    public InputField password;
    public Text notify;

    private const string serverUrl = "https://your-backend.render.com/api";

    void Start()
    {
        notify.text = "";
    }

    public void OnLoginClicked()
    {
        if (!CheckInput(id.text, password.text)) return;
        StartCoroutine(Login(id.text, password.text));
    }

    public void OnSignupClicked()
    {
        if (!CheckInput(id.text, password.text)) return;
        StartCoroutine(Signup(id.text, password.text));
    }

    IEnumerator Login(string username, string password)
    {
        string json = JsonUtility.ToJson(new UserData { id = username, password = password });
        UnityWebRequest request = CreatePostRequest($"{serverUrl}/login", json);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                var response = JsonUtility.FromJson<ServerResponse>(request.downloadHandler.text);
                if (response.success)
                {
                    SceneManager.LoadScene("MainScene"); // ���� �� �̸����� ��ȯ
                }
                else
                {
                    notify.text = response.message;
                }
            }
            catch
            {
                notify.text = "���� ���� �Ľ� ������ �߻��߽��ϴ�.";
            }
        }
        else
        {
            notify.text = "���� ���� ����: " + request.error;
        }
    }

    IEnumerator Signup(string username, string password)
    {
        string json = JsonUtility.ToJson(new UserData { id = username, password = password });
        UnityWebRequest request = CreatePostRequest($"{serverUrl}/signup", json);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                var response = JsonUtility.FromJson<ServerResponse>(request.downloadHandler.text);
                notify.text = response.message;
            }
            catch
            {
                notify.text = "���� ���� �Ľ� ������ �߻��߽��ϴ�.";
            }
        }
        else
        {
            notify.text = "���� ���� ����: " + request.error;
        }
    }

    UnityWebRequest CreatePostRequest(string url, string json)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        return request;
    }

    bool CheckInput(string id, string pwd)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(pwd))
        {
            notify.text = "���̵� �Ǵ� �н����带 �Է����ּ���.";
            return false;
        }
        return true;
    }

    [System.Serializable]
    public class UserData
    {
        public string id;
        public string password;
    }

    [System.Serializable]
    public class ServerResponse
    {
        public bool success;
        public string message;
        // public string token; // ���� Ȯ��� (ex. JWT ��ū ��� ����)
    }
}

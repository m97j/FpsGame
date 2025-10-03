using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MapUIManager : MonoBehaviour
{
    public static MapUIManager Instance;  // �̱��� ���ٿ�

    [Header("Map Data")]
    public List<MapData> mapList;  // MapData���� �ν����Ϳ��� ���� ����

    [Header("UI References")]
    public GameObject gridPanel;
    public GameObject detailPanel;

    public Image detailImage;
    public TMP_Text detailTitle;
    public TMP_Text detailDescription;
    public Button playButton;
    public Button closeButton;

    private string selectedScene;

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // �ʱ�ȭ
        detailPanel.SetActive(false);
        playButton.onClick.AddListener(OnClickPlay);
        closeButton.onClick.AddListener(OnClickClose);
    }

    public void ShowMapDetail(int index)
    {
        if (index < 0 || index >= mapList.Count)
        {
            Debug.LogError("Invalid map index");
            return;
        }

        MapData data = mapList[index];
        detailImage.sprite = data.mapImage;
        detailTitle.text = data.mapTitle;
        detailDescription.text = data.mapDescription;
        selectedScene = data.sceneName;

        gridPanel.SetActive(false);
        detailPanel.SetActive(true);
    }

    private void OnClickPlay()
    {
        SceneManager.LoadScene(selectedScene);
    }

    private void OnClickClose()
    {
        detailPanel.SetActive(false);
        gridPanel.SetActive(true);
    }
}

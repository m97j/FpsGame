using UnityEngine;
using UnityEngine.UI;

public class MapItemUI : MonoBehaviour
{
    public int mapIndex; // �� ��ư�� �����ϴ� MapData�� �ε���
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // MapUIManager�� mapIndex �����Ͽ� �ش� MapData ǥ��
        MapUIManager.Instance.ShowMapDetail(mapIndex);
    }
}

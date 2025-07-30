using UnityEngine;
using UnityEngine.UI;

public class MapItemUI : MonoBehaviour
{
    public int mapIndex; // 각 버튼이 참조하는 MapData의 인덱스
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // MapUIManager에 mapIndex 전달하여 해당 MapData 표시
        MapUIManager.Instance.ShowMapDetail(mapIndex);
    }
}

using UnityEngine;

public class BehaviorTree : MonoBehaviour
{
    private BTNode root;

    // �ܺο��� ��Ʈ ��带 ���Թ���
    public void SetRoot(BTNode rootNode)
    {
        root = rootNode;
    }

    void Update()
    {
        root?.Tick();
    }
}

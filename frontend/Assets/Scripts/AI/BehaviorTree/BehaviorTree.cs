using UnityEngine;

public class BehaviorTree : MonoBehaviour
{
    private BTNode root;

    // 외부에서 루트 노드를 주입받음
    public void SetRoot(BTNode rootNode)
    {
        root = rootNode;
    }

    void Update()
    {
        root?.Tick();
    }
}

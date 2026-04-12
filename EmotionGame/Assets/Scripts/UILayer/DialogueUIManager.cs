using UnityEngine;

public class DialogueUIManager : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("DialogueUIManager: 已启动");
        
        // 检查自身的碰撞体
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            Debug.Log($"DialogueUIManager: 自身碰撞体类型: {myCollider.GetType().Name}, isTrigger: {myCollider.isTrigger}");
        }
        else
        {
            Debug.Log("DialogueUIManager: 警告 - 自身没有Collider2D组件");
        }
        
        // 检查Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Debug.Log($"DialogueUIManager: 自身Rigidbody2D存在，isKinematic: {rb.isKinematic}");
        }
        else
        {
            Debug.Log("DialogueUIManager: 自身没有Rigidbody2D组件");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"DialogueUIManager: 检测到碰撞，tag: {collision.tag}, 物体名称: {collision.gameObject.name}");
        
        // 检查碰撞体信息
        Collider2D otherCollider = collision;
        if (otherCollider != null)
        {
            Debug.Log($"DialogueUIManager: 碰撞体类型: {otherCollider.GetType().Name}, isTrigger: {otherCollider.isTrigger}");
        }
        
        // 检查游戏状态
        if (GameManager.Instance != null)
        {
            Debug.Log($"DialogueUIManager: 当前游戏状态: {GameManager.Instance.CurrentGameState}");
        }
        else
        {
            Debug.Log("DialogueUIManager: GameManager.Instance为null");
        }
        
        // 检查是否是对话碰撞体，且当前游戏状态是Episode2
        if (collision.tag == "Dialogue" && GameManager.Instance != null)
        {
            Debug.Log($"DialogueUIManager: 检测到Dialogue碰撞体");
            
            if (GameManager.Instance.CurrentGameState == GameManager.GameState.Episode2)
            {
                // 显示碰撞体
                collision.gameObject.SetActive(true);
                Debug.Log($"DialogueUIManager: 显示对话碰撞体: {collision.gameObject.name}");
            }
            else
            {
                Debug.Log($"DialogueUIManager: 游戏状态不是Episode2，当前状态: {GameManager.Instance.CurrentGameState}");
            }
        }
    }

    private void Update()
    {
        // 检查游戏状态
        if (GameManager.Instance != null)
        {
            // 每2秒输出一次游戏状态，方便监控
            if (Time.frameCount % 120 == 0)
            {
                Debug.Log($"DialogueUIManager: 当前游戏状态: {GameManager.Instance.CurrentGameState}");
            }
        }
        else
        {
            if (Time.frameCount % 120 == 0)
            {
                Debug.Log("DialogueUIManager: GameManager.Instance为null");
            }
        }
    }
}


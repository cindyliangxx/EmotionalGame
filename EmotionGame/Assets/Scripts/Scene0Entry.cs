using UnityEngine;

public class Scene0Entry : MonoBehaviour
{
    private void Start()
    {
        // 设置游戏状态为Scene0
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.Scene0);
            Debug.Log("场景0：设置游戏状态为Scene0");
        }
    }
}
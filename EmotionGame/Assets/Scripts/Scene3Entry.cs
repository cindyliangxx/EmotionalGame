using UnityEngine;

public class Scene3Entry : MonoBehaviour
{
    private void Start()
    {
        // 设置游戏状态为Episode3
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.Episode3);
            Debug.Log("Scene3: 游戏状态已设置为Episode3");
        }
    }
}
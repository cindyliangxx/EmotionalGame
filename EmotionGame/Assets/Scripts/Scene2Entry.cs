using UnityEngine;

public class Scene2Entry : MonoBehaviour
{
    private void Start()
    {
        // 设置游戏状态为Episode2
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.Episode2);
            Debug.Log("Scene2: 游戏状态已设置为Episode2");
        }
    }
}
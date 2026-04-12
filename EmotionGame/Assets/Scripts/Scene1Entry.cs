using UnityEngine;

public class Scene1Entry : MonoBehaviour
{
    private void Start()
    {
        // 设置游戏状态为Episode1
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.Episode1);
            Debug.Log("Scene1: 游戏状态已设置为Episode1");
        }
    }
}
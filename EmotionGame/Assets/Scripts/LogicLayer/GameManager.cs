using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action OnPlayerDeathEvent;
    public event Action<GameState> OnGameStateChanged;

    public enum GameState
    {
        Scene0,
        Episode1,
        Episode2,
        Episode3
    }

    public GameState CurrentGameState { get; private set; } = GameState.Episode1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPlayerDeath()
    {
        
        Debug.Log("GameManager: OnPlayerDeathEvent 事件监听器数量: " + (OnPlayerDeathEvent?.GetInvocationList().Length ?? 0));
        OnPlayerDeathEvent?.Invoke();
        Debug.Log("GameManager: OnPlayerDeathEvent 事件触发完成");
    }

    public void ChangeGameState(GameState newState)
    {
        if (CurrentGameState != newState)
        {
            CurrentGameState = newState;
            OnGameStateChanged?.Invoke(newState);
            Debug.Log("GameManager: 游戏状态已切换到: " + newState);
        }
    }
}

using System;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public event Action OnMoveLeft;
    public event Action OnMoveRight;
    public event Action OnJumpOrClimb;
    public event Action OnTryTakePhoto;
    public event Action OnTryMakePhoneCall;

    private void Start()
    {
        // 检查GameManager实例
        if (GameManager.Instance != null)
        {
            Debug.Log($"PlayerInputManager: 已启动，当前游戏状态: {GameManager.Instance.CurrentGameState}");
        }
        else
        {
            Debug.Log("PlayerInputManager: 警告 - 未找到GameManager实例");
        }
    }

    private void Update()
    {
        // 检查GameManager实例
        if (GameManager.Instance != null)
        {
            // 检查游戏状态，Episode1时只允许左键点击
            if (GameManager.Instance.CurrentGameState == GameManager.GameState.Episode1)
            {
                // Episode1只检测左键点击
                if (Input.GetMouseButtonDown(0))
                {
                    // 这里不触发OnTryTakePhoto，因为Episode1时不需要拍照
                    // 左键点击由PlayerColliderDetect处理
                }
            }
            else if (GameManager.Instance.CurrentGameState == GameManager.GameState.Episode2)
            {
                // Episode2只允许AWD移动
                DetectMovementInput();
                DetectJumpInput();
            }
            else
            {
                // 其他状态正常检测所有输入
                DetectMovementInput();
                DetectJumpInput();
                DetectMouseInput();
            }
        }
        else
        {
            // GameManager为null时，使用默认输入处理
            DetectMovementInput();
            DetectJumpInput();
            DetectMouseInput();
        }
    }

    private void DetectMovementInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            OnMoveLeft?.Invoke();
        }

        if (Input.GetKey(KeyCode.D))
        {
            OnMoveRight?.Invoke();
        }
    }

    private void DetectJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnJumpOrClimb?.Invoke();
        }
    }

    private void DetectMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnTryTakePhoto?.Invoke();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnTryMakePhoneCall?.Invoke();
        }
    }
}

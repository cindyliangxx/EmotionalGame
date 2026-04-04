using System;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public event Action OnTryMoveLeftDown;
    public event Action OnTryMoveLeftUp;
    public event Action OnTryMoveRightDown;
    public event Action OnTryMoveRightUp;
    public event Action OnTryJumpOrClimbDown;
    public event Action OnTryJumpOrClimbUp;
    public event Action OnTryConfirmOrSurrenderDown;
    public event Action OnTryConfirmOrSurrenderUp;
    public event Action OnTryTakePhotoDown;
    public event Action OnTryTakePhotoUp;

    private void Update()
    {
        DetectKeyboardInput();
        DetectMouseInput();
    }

    private void DetectKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnTryMoveLeftDown?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            OnTryMoveLeftUp?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            OnTryMoveRightDown?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            OnTryMoveRightUp?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            OnTryJumpOrClimbDown?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            OnTryJumpOrClimbUp?.Invoke();
        }
    }

    private void DetectMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnTryConfirmOrSurrenderDown?.Invoke();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnTryConfirmOrSurrenderUp?.Invoke();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnTryTakePhotoDown?.Invoke();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            OnTryTakePhotoUp?.Invoke();
        }
    }
}

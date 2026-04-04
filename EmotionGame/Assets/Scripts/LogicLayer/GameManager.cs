using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerInputManager playerInputManager;

    private bool isJumpingOrClimbing;
    private bool isConfirmingOrSurrendering;
    private bool isTakingPhoto;

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

        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        if (playerInputManager != null)
        {
            playerInputManager.OnTryMoveLeftDown += HandleTryMoveLeftDown;
            playerInputManager.OnTryMoveLeftUp += HandleTryMoveLeftUp;
            playerInputManager.OnTryMoveRightDown += HandleTryMoveRightDown;
            playerInputManager.OnTryMoveRightUp += HandleTryMoveRightUp;
            playerInputManager.OnTryJumpOrClimbDown += HandleTryJumpOrClimbDown;
            playerInputManager.OnTryConfirmOrSurrenderDown += HandleTryConfirmOrSurrenderDown;
            playerInputManager.OnTryTakePhotoDown += HandleTryTakePhotoDown;
        }
    }

    private void OnDisable()
    {
        if (playerInputManager != null)
        {
            playerInputManager.OnTryMoveLeftDown -= HandleTryMoveLeftDown;
            playerInputManager.OnTryMoveLeftUp -= HandleTryMoveLeftUp;
            playerInputManager.OnTryMoveRightDown -= HandleTryMoveRightDown;
            playerInputManager.OnTryMoveRightUp -= HandleTryMoveRightUp;
            playerInputManager.OnTryJumpOrClimbDown -= HandleTryJumpOrClimbDown;
            playerInputManager.OnTryConfirmOrSurrenderDown -= HandleTryConfirmOrSurrenderDown;
            playerInputManager.OnTryTakePhotoDown -= HandleTryTakePhotoDown;
        }
    }

    private void HandleTryMoveLeftDown()
    {
        Debug.Log("player按下A左移");
    }

    private void HandleTryMoveLeftUp()
    {
        Debug.Log("player松开A左移");
    }

    private void HandleTryMoveRightDown()
    {
        Debug.Log("player按下D右移");
    }

    private void HandleTryMoveRightUp()
    {
        Debug.Log("player松开D右移");
    }

    private void HandleTryJumpOrClimbDown()
    {
        if (isJumpingOrClimbing)
        {
            return;
        }

        isJumpingOrClimbing = true;
        Debug.Log("player按下W跳跃/攀爬");
        isJumpingOrClimbing = false;
    }

    private void HandleTryConfirmOrSurrenderDown()
    {
        if (isConfirmingOrSurrendering)
        {
            return;
        }

        isConfirmingOrSurrendering = true;
        Debug.Log("player按下LeftMouse确认/投降");
        isConfirmingOrSurrendering = false;
    }

    private void HandleTryTakePhotoDown()
    {
        if (isTakingPhoto)
        {
            return;
        }

        isTakingPhoto = true;
        Debug.Log("player按下RightMouse拍照");
        isTakingPhoto = false;
    }
}

using UnityEngine;

public class InputPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 移动速度
    public bool isJumping = false; // 是否正在跳跃
    public bool isTakingPhotos = false; // 是否正在拍照

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // 监听输入事件
        PlayerInputManager inputManager = GetComponent<PlayerInputManager>();
        if (inputManager != null)
        {
            inputManager.OnMoveLeft += MoveLeft;
            inputManager.OnMoveRight += MoveRight;
            inputManager.OnJumpOrClimb += JumpOrClimb;
        }
    }

    private void OnDisable()
    {
        // 取消事件监听
        PlayerInputManager inputManager = GetComponent<PlayerInputManager>();
        if (inputManager != null)
        {
            inputManager.OnMoveLeft -= MoveLeft;
            inputManager.OnMoveRight -= MoveRight;
            inputManager.OnJumpOrClimb -= JumpOrClimb;
        }
    }

    private void MoveLeft()
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }
    }

    private void MoveRight()
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
    }

    private void JumpOrClimb()
    {
        // 跳跃逻辑可以在这里实现
        // 由于在Episode2中我们禁用了重力，跳跃可能不需要
    }
}

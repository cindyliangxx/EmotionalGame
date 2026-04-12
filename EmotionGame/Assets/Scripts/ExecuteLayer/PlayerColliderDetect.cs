using System;
using System.Collections;
using UnityEngine;

public class PlayerColliderDetect : MonoBehaviour
{
    public static PlayerColliderDetect Instance { get; private set; }

    public float climbHeight = 2f;
    public float climbSpeed = 2f;
    public float photoSuccessDuration = 2f;
    public GameObject[] dialogueObjects; // 对话碰撞体所属的空物体数组
    public AudioSource scene2BGM; // Scene2背景音乐

    public event Action OnPhotoSuccess;
    public event Action OnEnterMineArea;
    public event Action OnMineHit;
    public event Action OnEnterDropArea;
    public event Action OnDropHit;
    public event Action OnEnterEnemyGunArea;
    public event Action OnEnterFriendGunArea;
    public event Action OnEpisode1Complete; // Episode1完成事件
    public event Action OnEnterDialogueArea; // 进入对话区域事件

    private bool isInPhotoArea;
    private Vector3 climbStartPosition;
    private float climbProgress;
    private int meetMineCount = -1;
    private int meetDropCount = -1;
    private int meetDialogueCount = -1; // 对话计数

    public int MeetMineCount => meetMineCount;
    public int MeetDropCount => meetDropCount;
    public int MeetDialogueCount => meetDialogueCount;
    public bool isInClimbArea { get; private set; }
    public bool isClimbing { get; private set; }

    private PlayerController playerController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        PlayerInputManager pim = FindObjectOfType<PlayerInputManager>();
        if (pim != null)
        {
            pim.OnJumpOrClimb += HandleJumpOrClimb;
            pim.OnTryTakePhoto += HandleTryTakePhoto;
        }
    }

    private void OnDisable()
    {
        PlayerInputManager pim = FindObjectOfType<PlayerInputManager>();
        if (pim != null)
        {
            pim.OnJumpOrClimb -= HandleJumpOrClimb;
            pim.OnTryTakePhoto -= HandleTryTakePhoto;
        }
    }

    private void Update()
    {
        // 检查游戏状态，Episode2时禁用重力
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameState == GameManager.GameState.Episode2)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0;
                // 保持y轴速度为0，防止掉下去
                rb.velocity = new Vector2(rb.velocity.x, 0);
                Debug.Log("PlayerColliderDetect: Episode2 - 禁用重力");
            }
            
            // 播放Scene2背景音乐
            if (scene2BGM != null && !scene2BGM.isPlaying)
            {
                scene2BGM.Play();
            }
        }
        else
        {
            // 停止Scene2背景音乐
            if (scene2BGM != null && scene2BGM.isPlaying)
            {
                scene2BGM.Stop();
            }
        }
        
        // 每帧检查游戏状态，确保重力设置正确
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.CurrentGameState == GameManager.GameState.Episode2)
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null && rb.gravityScale != 0)
                {
                    rb.gravityScale = 0;
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    Debug.Log("PlayerColliderDetect: 强制禁用重力");
                }
            }
        }
        
        HandleClimbing();
        HandleMovementInClimbArea();
        
        // 检测AD键输入，允许在爬墙时离开墙体
        if (isInClimbArea && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            // 离开爬墙区域
            isInClimbArea = false;
            isClimbing = false;
            hasClimbed = false;
            
            // 恢复重力
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 3f;
            }
            
            Debug.Log("PlayerColliderDetect: 按下AD键离开爬墙区域");
        }
        
        // 检测左键点击（Episode1）
        if (Input.GetMouseButtonDown(0) && GameManager.Instance != null && GameManager.Instance.CurrentGameState == GameManager.GameState.Episode1)
        {
            Debug.Log("PlayerColliderDetect: Episode1 检测到左键点击");
            OnEpisode1Complete?.Invoke();
        }
    }

    private void HandleJumpOrClimb()
    {
        // 检查游戏状态，如果是Episode1则不处理跳跃/攀爬
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameState == GameManager.GameState.Episode1)
        {
            return;
        }
        
        Debug.Log($"PlayerColliderDetect: 接收到跳跃/攀爬事件 - isInClimbArea: {isInClimbArea}, isClimbing: {isClimbing}");
        if (isInClimbArea && !isClimbing)
        {
            StartClimbing();
        }
        else if (!isInClimbArea)
        {
            Debug.Log("PlayerColliderDetect: 不在爬墙区域");
        }
        else if (isClimbing)
        {
            Debug.Log("PlayerColliderDetect: 已经在爬墙中");
        }
    }

    private void HandleTryTakePhoto()
    {
        // 检查游戏状态，如果是Episode1则不处理拍照
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameState == GameManager.GameState.Episode1)
        {
            return;
        }
        
        if (isInPhotoArea && playerController != null && !playerController.isTakingPhotos)
        {
            OnPhotoSuccess?.Invoke();
            Debug.Log("拍照成功");
            StartCoroutine(PhotoSuccessCoroutine());
        }
    }

    private void StartClimbing()
    {
        Debug.Log($"PlayerColliderDetect: 开始爬墙 - 起始位置: {transform.position}, 攀爬高度: {climbHeight}, 攀爬速度: {climbSpeed}");
        isClimbing = true;
        climbStartPosition = transform.position;
        climbProgress = 0f;
        
        // 禁用Rigidbody2D的重力和速度，确保竖直攀爬
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            Debug.Log("PlayerColliderDetect: 已禁用Rigidbody2D重力和速度");
        }
        
        if (playerController != null)
        {
            playerController.isJumping = true;
            Debug.Log("PlayerColliderDetect: 已设置playerController.isJumping = true");
        }
        else
        {
            Debug.Log("PlayerColliderDetect: 警告 - playerController为null");
        }
    }

    private void HandleClimbing()
    {
        if (isClimbing)
        {
            climbProgress += climbSpeed * Time.deltaTime;
            float currentHeight = Mathf.Min(climbProgress, climbHeight);
            
            // 确保是竖直攀爬，只修改y坐标
            Vector3 newPosition = transform.position;
            newPosition.y = climbStartPosition.y + currentHeight;
            transform.position = newPosition;
            
            Debug.Log($"PlayerColliderDetect: 正在爬墙 - 进度: {climbProgress}, 当前高度: {currentHeight}, 目标高度: {climbHeight}");
            Debug.Log($"PlayerColliderDetect: 起始位置: {climbStartPosition}, 当前位置: {newPosition}");
            Debug.Log($"PlayerColliderDetect: 位置变化 - x: {newPosition.x - climbStartPosition.x}, y: {newPosition.y - climbStartPosition.y}");

            if (climbProgress >= climbHeight)
            {
                Debug.Log("PlayerColliderDetect: 爬墙完成");
                isClimbing = false;
                hasClimbed = true; // 标记已经完成爬墙
                
                // 恢复重力，设置为3
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.gravityScale = 3;
                    Debug.Log("PlayerColliderDetect: 已恢复Rigidbody2D重力，设置为3");
                }
                
                if (playerController != null)
                {
                    playerController.isJumping = false;
                    Debug.Log("PlayerColliderDetect: 已设置playerController.isJumping = false");
                }
            }
        }
    }

    private bool hasClimbed = false; // 标记是否已经完成爬墙

    private void HandleMovementInClimbArea()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Debug.Log($"PlayerColliderDetect: HandleMovementInClimbArea - isInClimbArea: {isInClimbArea}, isClimbing: {isClimbing}, hasClimbed: {hasClimbed}, 当前重力: {rb.gravityScale}");
        }
        
        if (isInClimbArea && !isClimbing && !hasClimbed)
        {
            // 禁用重力，让玩家停在墙边
            if (rb != null)
            {
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;
                Debug.Log("PlayerColliderDetect: 在爬墙区域，禁用重力");
            }
        }
        else if (!isInClimbArea)
        {
            // 退出爬墙区域，直接将重力设置为3
            if (rb != null)
            {
                Debug.Log($"PlayerColliderDetect: 退出爬墙区域，当前重力: {rb.gravityScale}");
                rb.gravityScale = 3;
                Debug.Log("PlayerColliderDetect: 退出爬墙区域，重力设置为3");
            }
            
            // 重置爬墙标记
            hasClimbed = false;
            Debug.Log("PlayerColliderDetect: 重置爬墙标记");
        }
    }

    private IEnumerator PhotoSuccessCoroutine()
    {
        if (playerController != null)
        {
            playerController.isTakingPhotos = true;
        }
        yield return new WaitForSeconds(photoSuccessDuration);
        if (playerController != null)
        {
            playerController.isTakingPhotos = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        Debug.Log($"Player entered trigger: {tag}");

        if (tag == "PhotoArea")
        {
            isInPhotoArea = true;
        }
        else if (tag == "ClimbArea")
        {
            isInClimbArea = true;
            Debug.Log("PlayerColliderDetect: 进入爬墙区域");
        }
        else if (tag == "PreMine")
        {
            meetMineCount++;
            Debug.Log($"即将进入地雷区，当前地雷计数: {meetMineCount}");
            // 先触发事件，再隐藏物体
            OnEnterMineArea?.Invoke();
            // 延迟隐藏，确保事件处理完成
            StartCoroutine(HideObjectAfterDelay(collision.gameObject, 0.1f));
        }
        else if (tag == "Mine")
        {
            Debug.Log($"玩家碰到地雷！当前地雷计数: {meetMineCount}");
            // 触发地雷命中事件
            OnMineHit?.Invoke();
            // 广播玩家死亡到GM
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerDeath();
            }
        }
        else if (tag == "PreDrop")
        {
            meetDropCount++;
            Debug.Log($"即将进入掉落物区，当前掉落物计数: {meetDropCount}");
            // 先触发事件，再隐藏物体
            OnEnterDropArea?.Invoke();
            // 延迟隐藏，确保事件处理完成
            StartCoroutine(HideObjectAfterDelay(collision.gameObject, 0.1f));
        }
        else if (tag == "Drop")
            {
                Debug.Log($"玩家碰到掉落物！当前掉落物计数: {meetDropCount}");
                // 触发掉落物命中事件
                OnDropHit?.Invoke();
                // 广播玩家死亡到GM
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnPlayerDeath();
                }
            }
            else if (tag == "PreEnemyGun")
            {
                Debug.Log("即将进入敌方战火区");
                // 触发即将进入敌方战火事件
                OnEnterEnemyGunArea?.Invoke();
                // 延迟隐藏，确保事件处理完成
                StartCoroutine(HideObjectAfterDelay(collision.gameObject, 0.1f));
            }
            else if (tag == "PreFriendGun")
            {
                Debug.Log("即将进入友军战火区");
                // 触发即将进入友军战火事件
                OnEnterFriendGunArea?.Invoke();
                // 延迟隐藏，确保事件处理完成
                StartCoroutine(HideObjectAfterDelay(collision.gameObject, 0.1f));
            }
            else if (tag == "EnemyGun")
            {
                Debug.Log("玩家碰到敌方枪！");
                // 广播玩家死亡到GM
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnPlayerDeath();
                }
            }
            else if (tag == "FriendGun")
            {
                Debug.Log("玩家碰到友军枪！");
                // 通知GunController玩家进入了友军枪区域
                GunController gunController = FindObjectOfType<GunController>();
                if (gunController != null)
                {
                    // 确定是哪个友军枪，使用meetDropCount作为索引
                    gunController.OnPlayerEnterFriendGunArea(meetDropCount);
                }
            }
            // 新增：对话碰撞体检测
            else if (tag == "Dialogue")
            {
                // 检查游戏状态，只在Episode2处理
                if (GameManager.Instance != null && GameManager.Instance.CurrentGameState == GameManager.GameState.Episode2)
                {
                    meetDialogueCount++;
                    Debug.Log($"进入对话区域，当前对话计数: {meetDialogueCount}");
                    
                    // 触发进入对话区域事件
                    OnEnterDialogueArea?.Invoke();
                    
                    // 销毁对应索引的对话物体
                    if (dialogueObjects != null && meetDialogueCount < dialogueObjects.Length)
                    {
                        GameObject dialogueObject = dialogueObjects[meetDialogueCount];
                        if (dialogueObject != null)
                        {
                            Destroy(dialogueObject);
                            Debug.Log($"销毁对话物体: {dialogueObject.name}");
                        }
                    }
                }
            }
            // 新增：游戏状态检测碰撞体
            else if (tag == "GameStateDetector")
            {
                // 检查是否是Episode2末尾的状态检测
                if (GameManager.Instance != null && GameManager.Instance.CurrentGameState == GameManager.GameState.Episode2)
                {
                    Debug.Log("检测到游戏状态切换触发器，切换到Episode3");
                    GameManager.Instance.ChangeGameState(GameManager.GameState.Episode3);
                    // 切换到Scene3场景
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Scene3");
                    Debug.Log("切换到Scene3场景");
                }
            }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.tag;
        Debug.Log($"Player exited trigger: {tag}");

        if (tag == "PhotoArea")
        {
            isInPhotoArea = false;
        }
        else if (tag == "ClimbArea")
        {
            isInClimbArea = false;
            isClimbing = false;
            // 不要重置hasClimbed，让它在HandleMovementInClimbArea中处理
            Debug.Log("PlayerColliderDetect: 退出爬墙区域");
        }
        else if (tag == "FriendGun")
        {
            Debug.Log("玩家离开友军枪区域！");
            // 通知GunController玩家离开友军枪区域
            GunController gunController = FindObjectOfType<GunController>();
            if (gunController != null)
            {
                gunController.OnPlayerExitFriendGunArea();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.tag;
        Debug.Log($"Player staying in trigger: {tag}");
    }
    
    private IEnumerator HideObjectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            obj.SetActive(false);
            Debug.Log($"隐藏物体: {obj.name}");
        }
    }
    
    // 重置到初始状态
    public void ResetToInitialState()
    {
        Debug.Log("PlayerColliderDetect: 开始重置到初始状态");
        
        // 重置计数器
        meetMineCount = -1;
        meetDropCount = -1;
        
        Debug.Log("PlayerColliderDetect: 重置完成");
    }
}

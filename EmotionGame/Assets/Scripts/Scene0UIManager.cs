using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scene0UIManager : MonoBehaviour
{
    public Button startButton; // 开始按钮
    public GameObject rotateImage; // 手机图片
    public AudioSource telegramBGM; // 电报背景音乐

    public float rotationSpeed = 100f; // 旋转速度
    public float maxRotationAngle = 30f; // 最大旋转角度
    private float currentRotation = 0f; // 当前旋转角度
    private bool isRotatingRight = true; // 是否向右旋转

    private void Start()
    {
        // 初始化按钮点击事件
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }
        
        // 播放电报背景音乐
        if (telegramBGM != null)
        {
            telegramBGM.Play();
        }
    }

    private void Update()
    {
        // 处理手机左右旋转
        if (rotateImage != null)
        {
            if (isRotatingRight)
            {
                currentRotation += rotationSpeed * Time.deltaTime;
                if (currentRotation >= maxRotationAngle)
                {
                    isRotatingRight = false;
                }
            }
            else
            {
                currentRotation -= rotationSpeed * Time.deltaTime;
                if (currentRotation <= -maxRotationAngle)
                {
                    isRotatingRight = true;
                }
            }
            // 应用旋转
            rotateImage.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }
    }

    private void OnStartButtonClick()
    {
        Debug.Log("点击开始按钮，切换到场景1");
        // 切换到场景1
        SceneManager.LoadScene("Scene1");
    }
}

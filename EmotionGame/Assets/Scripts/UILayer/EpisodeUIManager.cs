using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EpisodeUIManager : MonoBehaviour
{
    public GameObject goldfishUI; // 金鱼正常形态UI
    public GameObject goldfishDeadUI; // 金鱼死亡形态UI
    public float fadeDuration = 2f; // 淡入/淡出持续时间
    public float deadDelay = 2f; // 显示死亡形态后的延迟时间
    public AudioSource enterSceneSound; // 进入场景时的音效
    public AudioSource clickSound; // 点击左键时的音效
    public AudioSource switchSound; // 切换金鱼画面时的音效

    private bool isProcessing = false;

    private void Start()
    {
        // 初始化UI状态
        if (goldfishUI != null)
        {
            goldfishUI.SetActive(true);
            SetAlpha(goldfishUI, 1f);
        }
        
        if (goldfishDeadUI != null)
        {
            goldfishDeadUI.SetActive(true);
            SetAlpha(goldfishDeadUI, 0f);
        }
        
        // 播放进入场景时的音效
        if (enterSceneSound != null)
        {
            enterSceneSound.Play();
        }
    }

    private void Update()
    {
        // 直接检测左键点击（Episode1）
        if (Input.GetMouseButtonDown(0) && GameManager.Instance != null && GameManager.Instance.CurrentGameState == GameManager.GameState.Episode1 && !isProcessing)
        {
            Debug.Log("EpisodeUIManager: 检测到左键点击");
            // 播放点击音效
            if (clickSound != null)
            {
                clickSound.Play();
            }
            isProcessing = true;
            StartCoroutine(SwitchGoldfishForm());
        }
    }

    private IEnumerator SwitchGoldfishForm()
    {
        // 1. 直接切换到第二张图片
        if (goldfishUI != null)
        {
            goldfishUI.SetActive(false);
        }
        
        if (goldfishDeadUI != null)
        {
            goldfishDeadUI.SetActive(true);
            SetAlpha(goldfishDeadUI, 1f);
        }
        
        // 播放切换音效
        if (switchSound != null)
        {
            switchSound.Play();
        }
        
        Debug.Log("图片切换完成");

        
        // 等待deadDelay秒
        yield return new WaitForSeconds(deadDelay);
        Debug.Log($"等待{deadDelay}秒完成");
        
        // 2. 第二张图片淡出
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;
            
            if (goldfishDeadUI != null)
            {
                SetAlpha(goldfishDeadUI, 1f - progress);
            }
            
            yield return null;
        }
        
        // 确保最终状态正确
        if (goldfishDeadUI != null)
        {
            SetAlpha(goldfishDeadUI, 0f);
        }
        
        Debug.Log("图片淡出完成");
        
        // 3. 切换到Scene2
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene2");
        Debug.Log("切换到Scene2场景");
    }

    // 设置GameObject的透明度
    private void SetAlpha(GameObject obj, float alpha)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }
}
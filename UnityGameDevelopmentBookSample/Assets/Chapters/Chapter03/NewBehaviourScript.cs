using UnityEngine;
/// <summary>
/// P37 MonoBehaviour生命周期-初始化和销毁
/// </summary>
public class NewBehaviourScript : MonoBehaviour
{
    public int a = 0;

    private void Awake()
    {
        Debug.Log($"Awake method called，frameCount={Time.frameCount}");
    }
    /// <summary>
    /// 在Awake之后，Start之前执行
    /// </summary>
    private void OnEnable()
    {
        Debug.Log($"OnEnable method called，frameCount={Time.frameCount}");
    }
#if UNITY_EDITOR
    /// <summary>
    /// 仅在编辑器下，当脚本被添加到GameObject上时，会触发【Reset】；或者在Inspector面板上点击Reset按钮时，也会触发【Reset】
    /// 运行状态下，不会触发【Reset】
    /// </summary>
    private void Reset()
    {
        Debug.Log($"Reset method called，frameCount={Time.frameCount}");
        a = 0;
    }
#endif
    /// <summary>
    /// Start是在Awake之后，下一帧的Update之前调用的,且只执行一次
    /// </summary>
    private void Start()
    {
        Debug.Log($"Start method called，frameCount={Time.frameCount}");
    }
    /// <summary>
    /// 在编辑器下，停止游戏会首先触发【OnApplicationQuit】
    /// </summary>
    private void OnApplicationQuit()
    {
        Debug.Log($"OnApplicationQuit method called，frameCount={Time.frameCount}");
    }
    /// <summary>
    /// 接着触发【OnDisable】
    /// </summary>
    private void OnDisable()
    {
        Debug.Log($"OnDisable method called，frameCount={Time.frameCount}");
    }
    /// <summary>
    /// 最后触发【OnDestroy】
    /// </summary>
    private void OnDestroy()
    {
        Debug.Log($"OnDestroy method called，frameCount={Time.frameCount}");
    }
}

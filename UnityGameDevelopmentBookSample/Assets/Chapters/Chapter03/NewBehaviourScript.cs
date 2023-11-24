using UnityEngine;
/// <summary>
/// P37 MonoBehaviour��������-��ʼ��������
/// </summary>
public class NewBehaviourScript : MonoBehaviour
{
    public int a = 0;

    private void Awake()
    {
        Debug.Log($"Awake method called��frameCount={Time.frameCount}");
    }
    /// <summary>
    /// ��Awake֮��Start֮ǰִ��
    /// </summary>
    private void OnEnable()
    {
        Debug.Log($"OnEnable method called��frameCount={Time.frameCount}");
    }
#if UNITY_EDITOR
    /// <summary>
    /// ���ڱ༭���£����ű�����ӵ�GameObject��ʱ���ᴥ����Reset����������Inspector����ϵ��Reset��ťʱ��Ҳ�ᴥ����Reset��
    /// ����״̬�£����ᴥ����Reset��
    /// </summary>
    private void Reset()
    {
        Debug.Log($"Reset method called��frameCount={Time.frameCount}");
        a = 0;
    }
#endif
    /// <summary>
    /// Start����Awake֮����һ֡��Update֮ǰ���õ�,��ִֻ��һ��
    /// </summary>
    private void Start()
    {
        Debug.Log($"Start method called��frameCount={Time.frameCount}");
    }
    /// <summary>
    /// �ڱ༭���£�ֹͣ��Ϸ�����ȴ�����OnApplicationQuit��
    /// </summary>
    private void OnApplicationQuit()
    {
        Debug.Log($"OnApplicationQuit method called��frameCount={Time.frameCount}");
    }
    /// <summary>
    /// ���Ŵ�����OnDisable��
    /// </summary>
    private void OnDisable()
    {
        Debug.Log($"OnDisable method called��frameCount={Time.frameCount}");
    }
    /// <summary>
    /// ��󴥷���OnDestroy��
    /// </summary>
    private void OnDestroy()
    {
        Debug.Log($"OnDestroy method called��frameCount={Time.frameCount}");
    }
}

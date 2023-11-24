using UnityEngine;

public class ScriptP41 : MonoBehaviour
{
    private void Awake()
    {
        //强制设置当前帧率为25
        Application.targetFrameRate = 25;
    }

    private void FixedUpdate()
    {
        Debug.Log($"frameCount={Time.frameCount} time={Time.time}");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Update time={Time.time}");
    }
}

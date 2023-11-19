using UnityEngine;
using System.Diagnostics;

public class ScriptP30 : MonoBehaviour
{
    Transform t;

    // Start is called before the first frame update
    void Start()
    {
        //create a cube and assign to t
        t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
    }

    // Update is called once per frame
    void Update()
    {
        // press spacebar and set a random position and rotation to t
        if (Input.GetKeyDown(KeyCode.Space))
        {
            long executionTime = TimeMeasurement.MeasureExecutionTimeMicroseconds(() =>
            {
                // 执行需要计算耗时的函数
                SetRandomTransform1(t);//有意思的是，第二次执行，耗时极大减少！
            });
            UnityEngine.Debug.Log("函数执行耗时: " + executionTime + " 微秒");
        }
        // press enter and set a random position and rotation to t in the sametime
        if (Input.GetKeyDown(KeyCode.Return))
        {
            long executionTime = TimeMeasurement.MeasureExecutionTimeMicroseconds(() =>
            {
                // 执行需要计算耗时的函数
                SetRandomTransform2(t);//第一次执行时，耗时会比上面的少一些；第二次执行就不明显了。
            });
            UnityEngine.Debug.Log("函数执行耗时: " + executionTime + " 微秒");
        }       
    }

    //封装上面的space按键所触发的功能代码
    void SetRandomTransform1(Transform t)
    {
        //循环执行100次
        for (int i = 0; i < 100; i++)
        {
            var position = GetRandomPosition();
            var rotation = GetRandomRotation();
            t.position = position;
            t.rotation = rotation;
        }
    }

    //封装上面回车按键所触发的功能代码
    void SetRandomTransform2(Transform t)
    {
        for (int i = 0; i < 100; i++)
        {
            var position = GetRandomPosition();
            var rotation = GetRandomRotation();
            t.SetPositionAndRotation(position, rotation);
        }
    }

    //获取随机位置的方法
    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
    }

    //获取随机旋转的方法
    Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    }
}

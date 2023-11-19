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
                // ִ����Ҫ�����ʱ�ĺ���
                SetRandomTransform1(t);//����˼���ǣ��ڶ���ִ�У���ʱ������٣�
            });
            UnityEngine.Debug.Log("����ִ�к�ʱ: " + executionTime + " ΢��");
        }
        // press enter and set a random position and rotation to t in the sametime
        if (Input.GetKeyDown(KeyCode.Return))
        {
            long executionTime = TimeMeasurement.MeasureExecutionTimeMicroseconds(() =>
            {
                // ִ����Ҫ�����ʱ�ĺ���
                SetRandomTransform2(t);//��һ��ִ��ʱ����ʱ����������һЩ���ڶ���ִ�оͲ������ˡ�
            });
            UnityEngine.Debug.Log("����ִ�к�ʱ: " + executionTime + " ΢��");
        }       
    }

    //��װ�����space�����������Ĺ��ܴ���
    void SetRandomTransform1(Transform t)
    {
        //ѭ��ִ��100��
        for (int i = 0; i < 100; i++)
        {
            var position = GetRandomPosition();
            var rotation = GetRandomRotation();
            t.position = position;
            t.rotation = rotation;
        }
    }

    //��װ����س������������Ĺ��ܴ���
    void SetRandomTransform2(Transform t)
    {
        for (int i = 0; i < 100; i++)
        {
            var position = GetRandomPosition();
            var rotation = GetRandomRotation();
            t.SetPositionAndRotation(position, rotation);
        }
    }

    //��ȡ���λ�õķ���
    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
    }

    //��ȡ�����ת�ķ���
    Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    }
}

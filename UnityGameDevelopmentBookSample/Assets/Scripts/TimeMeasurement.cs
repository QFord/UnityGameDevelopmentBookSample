using System.Diagnostics;
/// <summary>
/// �������㺯��ִ�е�ʱ��
/// </summary>
public static class TimeMeasurement
{
    public static long MeasureExecutionTimeMicroseconds(System.Action action)
    {
        //���action������
        //var name = action.Method.Name;
        //UnityEngine.Debug.Log("����ĺ�����"+name);//��������Ǳ��������ɵģ�������д�Ĳ�һ��������û��̫�����塣
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        action.Invoke(); // ������Ҫ�����ʱ�ĺ���

        stopwatch.Stop();
        // ��ȡ��΢��Ϊ��λ��ʱ��
        long microseconds = stopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
        return microseconds;
    }
}
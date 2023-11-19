using System.Diagnostics;
/// <summary>
/// 用来计算函数执行的时长
/// </summary>
public static class TimeMeasurement
{
    public static long MeasureExecutionTimeMicroseconds(System.Action action)
    {
        //输出action的名称
        //var name = action.Method.Name;
        //UnityEngine.Debug.Log("计算的函数："+name);//这个名称是编译器生成的，跟代码写的不一样，所以没有太大意义。
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        action.Invoke(); // 调用需要计算耗时的函数

        stopwatch.Stop();
        // 获取以微秒为单位的时间
        long microseconds = stopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
        return microseconds;
    }
}
using System;
using System.Threading;
public sealed class SingletonObj
{
    private static object _lockObj = new object();
    private static SingletonObj? _singletonObj;
    private static int _objectCounter = 0;
    public static SingletonObj GetSingletonInstance
    {
        get
        {

            if (_singletonObj == null)
            {
                lock (_lockObj)
                {
                    if (_singletonObj == null)
                    {
                        _singletonObj = new SingletonObj();
                    }
                }
            }
            return _singletonObj;
        }
    }
    public int InstanceCounter { get { return _objectCounter; } }

    #region constructor
    private SingletonObj()
    {
        _objectCounter++;
    }
    #endregion
}

class Client
{
    static void Main()
    {
        //InitializerB();
        //InitializerA();
        Parallel.Invoke(() => InitializerA(), () => InitializerB());
    }

    private static void InitializerA()
    {
        SingletonObj objA = SingletonObj.GetSingletonInstance;
        Console.WriteLine($"IntializerA called...instance count:{objA.InstanceCounter}");
    }
    private static void InitializerB()
    {
        //Thread.Sleep(100);
        SingletonObj objB = SingletonObj.GetSingletonInstance;
        Console.WriteLine($"IntializerB called...instance count:{objB.InstanceCounter}");
    }
}

using System;
using System.Threading;
public sealed class SingletonObj
{
    private static Lazy<SingletonObj> _singletonObj = new Lazy<SingletonObj>(() => new SingletonObj());
    private static int _objectCounter = 0;
    public static SingletonObj GetSingletonInstance
    {
        get
        {
            return _singletonObj.Value;
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

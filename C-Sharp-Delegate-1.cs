using System;

class DownloadCompletedEventArgs : EventArgs
{
    //private long _bytesReceived;
    //private double _duration;
    //private bool _success;

    public long BytesReceived { get; }
    public TimeSpan Duration { get; }
    public bool Success { get; }
    public DownloadCompletedEventArgs(long bytesReceived, TimeSpan duration, bool success)
    {
        this.BytesReceived = bytesReceived;
        this.Duration = duration;
        this.Success = success;
    }
}

// publisher of event
class FileDownloadHandler
{
    // private delegate string x_del(string name); // delegate declaration

    // private readonly EventHandler<DownloadCompletedEventArgs?> _downloadCompletedEvent; // eventhandler delegate declaration

    public event EventHandler<DownloadCompletedEventArgs>? DownloadCompletedEvent; // event declaration

    protected virtual void OnDownLoadComplete(DownloadCompletedEventArgs e)
    {
        DownloadCompletedEvent?.Invoke(this, e);
    }

    public void RaiseDownloadCompleteEvent(long bytesReceived, TimeSpan duration, bool success)
    {
        this.OnDownLoadComplete(new DownloadCompletedEventArgs(bytesReceived, duration, success));
    }
}

// Subscribing to the event 
class FileDownloadClient
{

    static void Main()
    {
        FileDownloadHandler handler = new FileDownloadHandler();

        handler.DownloadCompletedEvent += OnDownLoadCompleteHandler; // registering the event

        // client code to download a file

        Console.WriteLine("Download started...");
        Console.WriteLine("Download In progress...");
        Console.WriteLine("Download Completed...");

        handler.RaiseDownloadCompleteEvent(2000, TimeSpan.FromSeconds(5.5), true);
    }

    private static void OnDownLoadCompleteHandler(object? sender, DownloadCompletedEventArgs e)
    {
        if (e.Success)
        {
            Console.WriteLine($"Download Completed.Total {e.BytesReceived} bytes downloaded in {e.Duration} seconds");
            return;
        }
        Console.WriteLine("DownLoad Failed");
    }
}
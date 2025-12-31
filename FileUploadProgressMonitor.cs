/*
 * File Upload Progress + Completed Events
 * Simulate a file upload system that raises two events
 * UploadProgressChanged
 * UploadCompleted
 */
//using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

class UploadProgressChangedEventArgs : EventArgs
{
    public int ProgressPercentage { get; }

    public UploadProgressChangedEventArgs(int progressPercentage)
    {
        this.ProgressPercentage = progressPercentage;
    }
}

class UploadCompletedEventArgs : EventArgs
{
    public bool Success { get; }
    public long BytesUploaded { get; }
    public TimeSpan Duration { get; }

    public UploadCompletedEventArgs(bool success, long bytesUploaded, TimeSpan duration)
    {
        this.Success = success;
        this.BytesUploaded = bytesUploaded;
        this.Duration = duration;
    }
}

class UploadFailedEventArgs : EventArgs
{
    public string ErrorMessage { get; }
    public UploadFailedEventArgs(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }
}

class FileUploader
{
    public event EventHandler<UploadProgressChangedEventArgs>? UploadProgressChanged;
    public event EventHandler<UploadCompletedEventArgs>? UploadCompleted;
    public event EventHandler<UploadFailedEventArgs>? UploadFailed;

    protected virtual void RaiseUploadProgressChangedEvent(UploadProgressChangedEventArgs eventArgs)
    {
        UploadProgressChanged?.Invoke(this, eventArgs);
    }

    protected virtual void RaiseUploadCompletedEvent(UploadCompletedEventArgs eventArgs)
    {
        UploadCompleted?.Invoke(this, eventArgs);
    }

    protected virtual void RaiseUploadFailedEvent(UploadFailedEventArgs eventArgs)
    {
        UploadFailed?.Invoke(this, eventArgs);
    }

    public void CheckUploadProgress(int progressPercentage, long bytesUploaded, TimeSpan duration, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (progressPercentage > 0 && progressPercentage < 100)
        {
            this.RaiseUploadProgressChangedEvent(
                new UploadProgressChangedEventArgs(progressPercentage)
            );
        }
        if (progressPercentage == 100)
        {
            this.RaiseUploadCompletedEvent(
                new UploadCompletedEventArgs(true, bytesUploaded, duration)
            );
        }
    }

    public void CheckForFileUploadFailure(string errorMessage)
    {
        this.RaiseUploadFailedEvent(
            new UploadFailedEventArgs(errorMessage)
        );
    }
}

class FileUploadViewer
{
    static async Task Main()
    {
        FileUploader fileUploader = new FileUploader();

        fileUploader.UploadProgressChanged += OnUploadProgressChanged;
        fileUploader.UploadCompleted += OnUploadCompleted;
        fileUploader.UploadFailed += OnUploadFailure;

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        long fileSize = 2560;
        long bytesRemaining = fileSize;
        long totalBytesUploaded = 0;
        double chunkSize = 250;
        Stopwatch w = new Stopwatch();

        // Fire and Forget scenario
        //_ = Task.Run(async() =>
        //{
        //    await Task.Delay(3000);
        //    cancellationTokenSource.Cancel();
        //});

        try
        {
            w.Start();
            Console.WriteLine("File Upload Started...");
            while (bytesRemaining > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int interval = 1000;

                await Task.Delay(interval, cancellationToken);
                chunkSize = bytesRemaining >= chunkSize ? chunkSize : bytesRemaining;

                totalBytesUploaded += (long)chunkSize;
                int percentage = (int)Math.Ceiling((double)((totalBytesUploaded / (double)fileSize) * 100));
                fileUploader.CheckUploadProgress(percentage, totalBytesUploaded, TimeSpan.FromMilliseconds(w.ElapsedMilliseconds), cancellationToken);

                bytesRemaining -= (long)chunkSize;
            }
            w.Stop();
        }
        catch(OperationCanceledException)
        {
            fileUploader.CheckForFileUploadFailure($"File upload is cancelled by the user");
        }
        catch (Exception ex)
        {
            fileUploader.CheckForFileUploadFailure(ex.Message);
            w.Stop();
        }
    }

    public static void OnUploadProgressChanged(object? sender, UploadProgressChangedEventArgs e)
    {
        Console.WriteLine($"File Upload progress {e.ProgressPercentage}%...");
    }

    public static void OnUploadCompleted(object? sender, UploadCompletedEventArgs e)
    {
        if (e.Success)
        {
            Console.WriteLine($"File Upload Completed. Took {e.Duration}s to upload {e.BytesUploaded} bytes...");
            return;
        }
        Console.WriteLine($"File upload did not complete");
    }
    public static void OnUploadFailure(object? sender, UploadFailedEventArgs e)
    {
        Console.WriteLine($"File upload failure:{e.ErrorMessage}");
    }
}
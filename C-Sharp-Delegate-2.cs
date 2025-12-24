/*
 * Create a program that simulates a temperature sensor.
 * When the temperature crosses a threshold (e.g., 75°C), the sensor should raise a TemperatureExceeded event.
 */
// custom eventargs 
public class TemperatureExceededEventArgs : EventArgs
{
    public double CurrentTemperature { get; }
    public double Threshold { get; }
    public DateTime Timestamp { get; }

    public TemperatureExceededEventArgs(double currentTemperature, double threshold, DateTime timestamp)
    {
        this.CurrentTemperature = currentTemperature;
        this.Threshold = threshold;
        this.Timestamp = timestamp;
    }
}

// event publisher
public class TemperatureSensor
{
    public event EventHandler<TemperatureExceededEventArgs>? TemperatureExceeded;

    protected virtual void RaiseTemperatureExceededEvent(TemperatureExceededEventArgs e)
    {
        TemperatureExceeded?.Invoke(this, e);
    }
    public void CheckTemperature(double temperature, double threshold)
    {
        if (temperature > threshold)
        {
            RaiseTemperatureExceededEvent(
                new TemperatureExceededEventArgs(temperature, threshold, DateTime.Now)
            );
        }
    }
}

// event subscriber
public class MyApp
{
    static void Main()
    {
        TemperatureSensor sensor = new TemperatureSensor();

        sensor.TemperatureExceeded += OnTemperatureExceed; // added the event handler

        var rand = new Random();
        int iterationCount = 5;

        while (iterationCount > 0) 
        {
            int randTemp = rand.Next(60, 120);
            sensor.CheckTemperature(temperature: randTemp, threshold: 75);
            iterationCount--;
        }
    }

    public static void OnTemperatureExceed(object? sender, TemperatureExceededEventArgs eventArgs)
    {
        Console.WriteLine($"Temperature has exceeded to {eventArgs.CurrentTemperature}F beyond the threshold of {eventArgs.Threshold}F");
    }
}
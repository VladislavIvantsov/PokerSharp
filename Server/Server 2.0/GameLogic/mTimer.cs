using System.Timers;

class mTimer
{
    public Timer sTimer;
    public bool Interruption = false;

    public mTimer(int _Seconds)
    {
        sTimer = new Timer();
        sTimer.AutoReset = false;
        sTimer.Interval = _Seconds * 1000;
    }

    public void Start()
    {
        Interruption = false;
        sTimer.Start();
    }

    public void Stop()
    {
        Interruption = true;
        sTimer.Stop();
    }
}


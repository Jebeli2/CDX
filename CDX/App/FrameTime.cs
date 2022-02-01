namespace CDX.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FrameTime
    {
        private TimeSpan elapsedTime;
        private TimeSpan totalTime;

        public FrameTime()
        {
            elapsedTime = TimeSpan.Zero;
            totalTime = TimeSpan.Zero;
        }
        public FrameTime(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            this.elapsedTime = elapsedTime;
            this.totalTime = totalTime;
        }
        public TimeSpan ElapsedTime => elapsedTime;
        public TimeSpan TotalTime => totalTime;
        public bool IsRunningSlowly { get; set; }

        public void Add(TimeSpan time)
        {
            elapsedTime += time;
            totalTime += time;
        }

        public void Add(long ticks)
        {
            Add(TimeSpan.FromTicks(ticks));
        }

        public void AddTotal(TimeSpan time)
        {
            totalTime += time;
        }

        public void SetElapsedTime(TimeSpan elapsed)
        {
            elapsedTime = elapsed;
        }

        public void ResetElapsedTime() { elapsedTime = TimeSpan.Zero; }
    }
}

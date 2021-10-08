using System;

namespace Utilities.TickSystem
{
    public class TickTimer
    {
        private int duration;
        private TickSystem tickSystem;
        public enum TickTiming { Early, Middle, Late }
        private TickTiming timing;
        private Action action;
        private int loopAmount;

        /// <summary>
        /// The total duration of the TickTimer in seconds.
        /// </summary>
        public float TimeDuration { get => duration * tickSystem.TickLength; }
        /// <summary>
        /// The time remaining until the TickTimer fires in seconds.
        /// </summary>
        public float TimeRemaining { get => (duration - timer) * tickSystem.TickLength; }

        private int timer;

        /// <summary>
        /// A timer that fires an action after a given amount of ticks.
        /// </summary>
        /// <param name="duration"> The amount of ticks before the timer will be fired after its creation. </param>
        /// <param name="tickSystem"> The TickSystem that the Timer should listen to. </param>
        /// <param name="timing"> Should the timer fire on OnTickEarly, OnTick or OnTickLate. </param>
        /// <param name="action"></param>
        /// <param name="loopAmount"> The amount of times the timer should repeat itself after its initial activation. </param>
        public TickTimer(int duration, TickSystem tickSystem, TickTiming timing, Action action, int loopAmount = 0)
        {
            this.duration = duration;
            this.tickSystem = tickSystem;
            this.timing = timing;
            this.action = action;
            this.loopAmount = loopAmount;

            switch (timing)
            {
                case TickTiming.Early:
                    tickSystem.OnTickEarly.AddListener(Update);
                    break;
                case TickTiming.Middle:
                    tickSystem.OnTick.AddListener(Update);
                    break;
                case TickTiming.Late:
                    tickSystem.OnTickLate.AddListener(Update);
                    break;
            }
        }
        /// <summary>
        /// A timer that fires an action after a given amount of ticks.
        /// </summary>
        /// <param name="duration"> The amount of ticks before the timer will be fired after its creation. </param>
        /// <param name="key"> The key to the TickSystem that the Timer should listen to. </param>
        /// <param name="timing"> Should the timer fire on OnTickEarly, OnTick or OnTickLate. </param>
        /// <param name="action"></param>
        /// <param name="loopAmount"> The amount of times the timer should repeat itself after its initial activation. </param>
        public TickTimer(int duration, string key, TickTiming timing, Action action, int loopAmount = 0)
        {
            this.duration = duration;
            tickSystem = TickSystem.Instances[key];
            this.timing = timing;
            this.action = action;
            this.loopAmount = loopAmount;

            switch (timing)
            {
                case TickTiming.Early:
                    tickSystem.OnTickEarly.AddListener(Update);
                    break;
                case TickTiming.Middle:
                    tickSystem.OnTick.AddListener(Update);
                    break;
                case TickTiming.Late:
                    tickSystem.OnTickLate.AddListener(Update);
                    break;
            }
        }

        private void Update()
        {
            timer++;

            if (timer >= duration)
            {
                action();
                if (loopAmount != 0)
                {
                    loopAmount--;
                    timer = 0;
                }
                else
                {
                    switch (timing)
                    {
                        case TickTiming.Early:
                            tickSystem.OnTickEarly.RemoveListener(Update);
                            break;
                        case TickTiming.Middle:
                            tickSystem.OnTick.RemoveListener(Update);
                            break;
                        case TickTiming.Late:
                            tickSystem.OnTickLate.RemoveListener(Update);
                            break;
                    }
                }
            }
        }
    }
}
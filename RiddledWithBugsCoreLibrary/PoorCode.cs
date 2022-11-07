
using System.Timers;

namespace RiddledWithBugsCoreLibrary
{
    public class PoorCode   
    {
        public System.Timers.Timer UnDisposedTimer;

        private int i, j, k, l = 0;//poor names - will SAST notice?

        public PoorCode()
        {
            i = j + k + l++;//shocking code!

            UnDisposedTimer = new System.Timers.Timer //will SAST notice if this is not destroyed, and we shoudl use IDisposable?
            {
                Interval = i * 60000              
            };

            UnDisposedTimer.Elapsed += UnDisposedTimer_Elapsed;
            UnDisposedTimer.Start();
            
        }

        private static void UnDisposedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new Exception("Shouldn't use Exception - will this be picked up by SAST?");
        }

        internal double TimerDuration()
        {
            return UnDisposedTimer.Interval;
        }

        internal void Stop()
        {
            UnDisposedTimer.Stop();
        }
    }
}

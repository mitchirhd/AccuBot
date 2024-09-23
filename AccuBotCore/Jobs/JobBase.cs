namespace AccuBotCore.Jobs
{
    public abstract class JobBase
    {
        private bool _interrupted = false;

        protected int IdleIntervall = 250;

        public bool IsStopped = true;

        public virtual void Start()
        {
            Task.Run(Idle);
            IsStopped = false;
        }

        public virtual void Stop()
        {
            _interrupted = true;
        }

        protected async void Idle()
        {
            _interrupted = false;
            while (!_interrupted)
            {
                await OnExecute();
                Thread.Sleep(IdleIntervall);
            }
            IsStopped = true;
        }

        public abstract Task OnExecute();
    }
}

namespace Fraktal.DesignPatterns
{
    public abstract class PipelineStep<T> : IPipelineStep<T>
    {
        private bool hasCancelled = false;

        public abstract T Process(T input);
    

        public bool HasCancelled()
        {
            return hasCancelled;
        }

        public void SetCancelled(bool value)
        {
            hasCancelled = value;
        }
    }
}
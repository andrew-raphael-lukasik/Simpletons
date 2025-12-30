namespace Simpleton
{
    public abstract partial class SimpletonState
    {

        public SimpletonStateTransition[] transitions;
        public bool completed;
        public float timeStart;
        public float timeExpectedEnd;
        //public string label;

        public abstract void OnEnter(SimpletonState previous, float time);
        public abstract void OnExit(SimpletonState next);
        public abstract void Tick(float time);

    }
}

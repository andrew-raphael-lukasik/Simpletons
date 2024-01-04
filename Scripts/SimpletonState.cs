namespace Simpleton
{
    public abstract partial class SimpletonState
    {
        
        public string label = null;
        public SimpletonStateTransition[] transitions = null;
        public bool completed = false;
        public float timeStart = 0;
        public float timeExpectedEnd = 0;

        public abstract void OnEnter ( SimpletonState previous , float time );
        public abstract void OnExit ( SimpletonState next );
        public abstract void Tick ( float time );

    }
}

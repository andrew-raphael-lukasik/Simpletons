namespace Simpleton
{
    public class SimpletonStateTransition
    {

        public string label;
        public Predicate predicate;
        public SimpletonState destination;

        public SimpletonStateTransition(
            Predicate predicate,
            SimpletonState destination,
            string label = null
        )
        {
            this.predicate = predicate;
            this.destination = destination;
            this.label = label;
        }

        public delegate bool Predicate(SimpletonState state, float time);

    }
}

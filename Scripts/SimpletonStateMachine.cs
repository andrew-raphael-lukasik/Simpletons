using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Simpleton
{
    public class SimpletonStateMachine
    {
        
        protected SimpletonState _initial = null;
        public SimpletonState Initial => _initial;

        protected SimpletonState _current = null;
        public SimpletonState Current => _current;

        #if DEBUG
        [System.NonSerialized] public RingBuffer<string> DebugLogs = new (128);
        #endif

        public SimpletonStateMachine () {
            for( int i=0 ; i<DebugLogs.Capacity ; i++ )
				DebugLogs.Buffer[i] = $"i:{i}";
        }
        public SimpletonStateMachine ( SimpletonState initial_state )
        {
            Assert.IsNotNull( initial_state , $"{nameof(initial_state)} is null" );
            _initial = initial_state;
            Reset();
            ConstructorAssertions();
        }

        public void ConstructorAssertions ()
        {
            Assert.IsNotNull( _initial , $"{nameof(_initial)} is null" );
            Assert.IsNotNull( _current , $"{nameof(_current)} state is null" );
            Assert.IsNotNull( _current.transitions , $"\"{_current.GetType().Name}\" {nameof(_current)}.{nameof(_current.transitions)} is null" );
            
            #if UNITY_ASSERTIONS
            FindAllStates();
            #endif
        }

        public void Tick ( float time )
        {
            // tick current state:
            _current.Tick( time:time );

            // go through transitions:
            int numTransitions = _current.transitions.Length;
            for( int i=0 ; i<numTransitions ; i++ )
            {
                var transition = _current.transitions[i];
                Assert.IsNotNull( transition.predicate , $"{nameof(transition)}.{nameof(transition.predicate)} is null" );

                if( transition.predicate(state:_current,time:time) )
                {
                    #if DEBUG
                    DebugLogs.Push($"\ttransition '{transition.label}' triggered");
                    #endif
                    // << transition triggered! >>

                    // on exit:
                    _current.OnExit( next:transition.destination );

                    // swap states:
                    var previousState = _current;
                    _current = transition.destination;

                    // on enter:
                    _current.completed = false;
                    _current.timeStart = time;
                    _current.timeExpectedEnd = time + 10f;//< a reasonable default you may want to override
                    _current.OnEnter( previous:previousState , time:time );

                    #if DEBUG
                    //DebugLogs.Push($"\tchanged state: <{previousState?.GetType().Name}> -> <{_current?.GetType().Name}>");
                    DebugLogs.Push($"<{_current?.GetType().Name}>");
                    #endif
                    
                    return;
                }
            }
        }

        public void Reset ()
        {
            _current = _initial;

            #if DEBUG
            DebugLogs.Push("<b>- RESET -</b>");
            DebugLogs.Push($"<{_current?.GetType().Name}>");
            #endif
        }

        public HashSet<SimpletonState> FindAllStates ()
        {
            Assert.IsNotNull( _initial , $"{nameof(_initial)} state is null!" );
            return FindAllStates( _initial );
        }
        public HashSet<SimpletonState> FindAllStates ( SimpletonState start )
        {
            Assert.IsNotNull( start , $"{GetType().Name}.{nameof(FindAllStates)}(): starting state cannot be null" );

            var set = new HashSet<SimpletonState>();
            set.Add( start );
            
            var search = new Queue<SimpletonState>();
            search.Enqueue( start );
            
            while( search.Count!=0 )
            {
                SimpletonState next = search.Dequeue();

                Assert.IsNotNull( next.transitions , $"{GetType().Name}.{nameof(FindAllStates)}(): {nameof(next.transitions)} field for \"{next.GetType().Name}\" state is null. Make sure transition array is assigned to the state! Inspect code where this state is defined." );

                foreach( SimpletonStateTransition transition in next.transitions )
                {
                    Assert.IsNotNull( transition.destination , $"{GetType().Name}.{nameof(FindAllStates)}(): transition.destination for \"{next.GetType().Name}\" state is null" );

                    if( set.Add( transition.destination ) )
                        search.Enqueue( transition.destination );
                }
            }
            
            return set;
        }

    }
}

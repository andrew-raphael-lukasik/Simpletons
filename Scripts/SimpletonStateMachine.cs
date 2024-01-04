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
		public RingBuffer<string> DebugLogs = new RingBuffer<string>(40);
		#endif

		// [System.Obsolete("don't",true)]
		public SimpletonStateMachine () {}
		public SimpletonStateMachine ( SimpletonState initial_state )
		{
			Assert.IsNotNull( initial_state , $"{nameof(initial_state)} is null" );

			this._initial = initial_state;
			this.Reset();

			ConstructorAssertions();
		}

		public void ConstructorAssertions ()
		{
			Assert.IsNotNull( _current , $"{nameof(_current)} state is null" );
			Assert.IsNotNull( _current.transitions , $"\"{_current.label}\" {nameof(_current)}.{nameof(_current.transitions)} is null" );
			#if UNITY_ASSERTIONS
			FindAllStates();
			#endif
		}

		public void Tick ( float time )
		{
			// tick current state:
			_current.Tick( time:time );

			// go through transitions:
			for( int i=0 ; i<_current.transitions.Length ; i++ )
			{
				var transition = _current.transitions[i];
				Assert.IsNotNull( transition.predicate , $"{nameof(transition)}.{nameof(transition.predicate)} is null" );

				if( transition.predicate(state:_current,time:time) )
				{
					#if DEBUG
					// DebugLogs.Push($"transition {transition.label}");
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
					// DebugLogs.Push($"changed state: {previousState.label} -> {_current.label}");
					DebugLogs.Push(_current.label);
					#endif
					
					return;
				}
			}
		}

		public void Reset ()
		{
			_current = _initial;

			#if DEBUG
			DebugLogs.Push("<b>RESET</b>");
			DebugLogs.Push(_current.label);
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

				Assert.IsNotNull( next.transitions , $"{GetType().Name}.{nameof(FindAllStates)}(): {nameof(next.transitions)} field for \"{next.label}\" state is null" );

				foreach( SimpletonStateTransition transition in next.transitions )
				{
					Assert.IsNotNull( transition.destination , $"{GetType().Name}.{nameof(FindAllStates)}(): transition.destination for \"{next.label}\" state is null" );

					if( set.Add( transition.destination ) )
						search.Enqueue( transition.destination );
				}
			}
			
			return set;
		}

	}
}

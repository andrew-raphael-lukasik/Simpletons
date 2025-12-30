using UnityEngine;
using Simpleton;

namespace Simpleton.Samples.Crosswalk
{
    [System.Serializable]// this + [SerializeField] will allow SimpletonStateMachinePropertyDrawer to draw a `Inspect` button in Inspector
    public class PedestrianAI : SimpletonStateMachine
    {

        PedestrianController _owner;

        public PedestrianAI(PedestrianController owner )
        {
            _owner = owner;

            // create states:
            var start   = new PedestrianIdleState(_owner);
            var move    = new PedestrianMoveState(_owner);
            var stop    = new PedestrianStopState(_owner);

            // create transitions:
            SimpletonStateTransition start_to_move, move_to_move, move_to_stop, stop_to_move;
            start_to_move = new SimpletonStateTransition(
                predicate:      (state, time) => true,
                destination:    move,
                label:          nameof(start_to_move)
            );
            move_to_move = new SimpletonStateTransition(
                predicate:      (state, time) => state.completed,
                destination:    move,
                label:          nameof(move_to_move)
            );
            move_to_stop = new SimpletonStateTransition(
                predicate:      (state, time) => stop.triggered,
                destination:    stop,
                label:          nameof(move_to_stop)
            );
            stop_to_move = new SimpletonStateTransition(
                predicate:      (state, time) => state.completed,
                destination:    move,
                label:          nameof(stop_to_move)
            );

            // assign transitions to states:
            start.transitions = new SimpletonStateTransition[]{
                start_to_move
            };
            move.transitions = new SimpletonStateTransition[]{
                move_to_stop,
                move_to_move,
            };
            stop.transitions = new SimpletonStateTransition[]{
                stop_to_move
            };

            // assign initial state
            _initial = start;
            Reset();
            ConstructorAssertions();
        }

    }
}

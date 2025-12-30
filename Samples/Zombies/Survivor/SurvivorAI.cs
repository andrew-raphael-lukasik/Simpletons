using UnityEngine;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    [System.Serializable]// this + [SerializeField] will allow SimpletonStateMachinePropertyDrawer to draw a `Inspect` button in Inspector
    public class SurvivorAI : SimpletonStateMachine
    {

        SurvivorController _owner;

        public SurvivorAI(SurvivorController owner)
        {
            _owner = owner;

            // create states:
            var start       = new SurvivorIdleState(_owner);
            var idle        = new SurvivorIdleState(_owner);
            var wander      = new SurvivorWanderState(_owner);
            var flee        = new SurvivorFleeState(_owner);
            var dead        = new SurvivorDeadState(_owner);
            var afterlife   = new SurvivorReanimationState(_owner);

            // create transitions:
            SimpletonStateTransition start_to_idle, idle_to_wander, wander_to_idle, any_to_flee, flee_to_idle, any_to_dead, dead_to_afterlife;
            start_to_idle = new SimpletonStateTransition(
                predicate:      (state, time) => true,
                destination:    idle,
                label:          nameof(start_to_idle)
            );
            idle_to_wander = new SimpletonStateTransition(
                predicate:      (state, time) => Random.Range(1,6)==1,// 20% chance
                destination:    wander,
                label:          nameof(idle_to_wander)
            );
            wander_to_idle = new SimpletonStateTransition(
                predicate:      (state, time) => state.completed || time>state.timeExpectedEnd,
                destination:    idle,
                label:          nameof(wander_to_idle)
            );
            any_to_flee = new SimpletonStateTransition(
                predicate:      (state, time) => _owner.zombiesAround.Count!=0,
                destination:    flee,
                label:          nameof(any_to_flee)
            );
            flee_to_idle = new SimpletonStateTransition(
                predicate:      (state, time) => _owner.zombiesAround.Count==0,
                destination:    idle,
                label:          nameof(flee_to_idle)
            );
            any_to_dead = new SimpletonStateTransition(
                predicate:      (state, time) => _owner.Health<=0,
                destination:    dead,
                label:          nameof(any_to_dead)
            );
            dead_to_afterlife = new SimpletonStateTransition(
                predicate:      (state, time) => state.completed,
                destination:    afterlife,
                label:          nameof(dead_to_afterlife)
            );

            // assign transitions to states:
            start.transitions = new SimpletonStateTransition[]{
                any_to_dead,
                start_to_idle
            };
            idle.transitions = new SimpletonStateTransition[]{
                any_to_dead,
                idle_to_wander,
                any_to_flee,
            };
            wander.transitions = new SimpletonStateTransition[]{
                any_to_dead,
                wander_to_idle,
                any_to_flee,
            };
            flee.transitions = new SimpletonStateTransition[]{
                any_to_dead,
                flee_to_idle
            };
            dead.transitions = new SimpletonStateTransition[]{
                dead_to_afterlife
            };
            afterlife.transitions = new SimpletonStateTransition[]{};

            // assign initial state
            _initial = start;
            Reset();
            ConstructorAssertions();
        }

    }
}

using UnityEngine;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    [System.Serializable]// this + [SerializeField] will allow SimpletonStateMachinePropertyDrawer to draw a `Inspect` button in Inspector
    public class WalkerAI : SimpletonStateMachine
    {

        WalkerController _owner;

        public WalkerAI(WalkerController owner)
        {
            _owner = owner;

            // create states:
            var start   = new WalkerIdleState(_owner);
            var idle    = new WalkerIdleState(_owner);
            var wander  = new WalkerWanderState(_owner);
            var agro    = new WalkerAgroState(_owner);
            var attack  = new WalkerAttackState(_owner);
            var dead    = new WalkerDeadState(_owner);

            // create transitions:
            SimpletonStateTransition start_to_idle, idle_to_wander, wander_to_idle, any_to_agro, agro_to_idle, agro_to_attack, attack_to_agro, any_to_dead;
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
                predicate:      (state, time) => state.completed || time>state.timeExpectedEnd || _owner.navMeshAgent.remainingDistance<1f,
                destination:    idle,
                label:          nameof(wander_to_idle)
            );
            any_to_agro = new SimpletonStateTransition(
                predicate:      (state, time) => _owner.survivorsAround.Count!=0,
                destination:    agro,
                label:          nameof(any_to_agro)
            );
            agro_to_idle = new SimpletonStateTransition(
                predicate:      (state, time) => state.completed || _owner.survivorsAround.Count==0,
                destination:    idle,
                label:          nameof(agro_to_idle)
            );
            any_to_dead = new SimpletonStateTransition(
                predicate:      (state, time) => _owner.Health<=0,
                destination:    idle,
                label:          nameof(any_to_dead)
            );
            agro_to_attack = new SimpletonStateTransition(
                predicate:      (state, time) => ((WalkerAgroState)state).victimToFollow!=null && Vector3.Distance(_owner.transform.position,((WalkerAgroState)state).victimToFollow.transform.position)<_owner.meleeAttackRange,
                destination:    attack,
                label:          nameof(agro_to_attack)
            );
            attack_to_agro = new SimpletonStateTransition(
                predicate:      (state, time) => state.completed,
                destination:    agro,
                label:          nameof(attack_to_agro)
            );

            // assign transitions to states:
            start.transitions = new SimpletonStateTransition[]{
                start_to_idle
            };
            idle.transitions = new SimpletonStateTransition[]{
                any_to_dead,
                idle_to_wander,
                any_to_agro,
            };
            wander.transitions = new SimpletonStateTransition[]{
                any_to_dead,
                wander_to_idle,
                any_to_agro,
            };
            agro.transitions = new SimpletonStateTransition[]{
                any_to_dead,
                agro_to_attack,
                agro_to_idle
            };
            attack.transitions = new SimpletonStateTransition[]{
                any_to_dead,
                attack_to_agro
            };
            dead.transitions = new SimpletonStateTransition[]{};

            // assign initial state
            _initial = start;
            Reset();
            ConstructorAssertions();
        }

    }
}

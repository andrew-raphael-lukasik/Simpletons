using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    public class SurvivorWanderState : SimpletonState
    {

        SurvivorController _owner;

        public SurvivorWanderState(SurvivorController owner)
        {
            _owner = owner;
        }

        public override void OnEnter(SimpletonState previous, float time)
        {
            // behave
            if (NavMesh.SamplePosition(_owner.transform.position + new Vector3(Random.Range(-10f,10f), 0, Random.Range(-10f, 10f)), out var sample, 10f, ~0))
            {
                _owner.navMeshAgent.SetDestination(sample.position);
                _owner.navMeshAgent.isStopped = false;

                // set execution time limit
                timeExpectedEnd = time + 5f;
            }
            else
            {
                _owner.navMeshAgent.isStopped = true;

                // set execution time limit
                timeExpectedEnd = time + 1f;
            }
            // todo: play animation, etc.
        }

        public override void OnExit(SimpletonState next)
        {
            
        }

        public override void Tick(float time)
        {
            completed |= _owner.navMeshAgent.remainingDistance<1f;
        }

    }
}

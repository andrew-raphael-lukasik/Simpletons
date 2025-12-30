using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Crosswalk
{
    public class PedestrianIdleState : SimpletonState
    {

        PedestrianController _owner;

        public PedestrianIdleState(PedestrianController owner)
        {
            _owner = owner;
        }

        public override void OnEnter(SimpletonState previous, float time)
        {
            // set state time limit
            timeExpectedEnd = time + Random.Range(1f, 5f);

            // behave
            _owner.navMeshAgent.isStopped = true;
            // todo: play idle animation, etc
        }

        public override void OnExit(SimpletonState next)
        {
            
        }

        public override void Tick(float time)
        {
            completed |= time>timeExpectedEnd;
        }

    }
}

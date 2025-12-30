using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Crosswalk
{
    public class PedestrianMoveState : SimpletonState
    {

        PedestrianController _owner;

        public PedestrianMoveState(PedestrianController owner)
        {
            _owner = owner;
        }

        public override void OnEnter(SimpletonState previous, float time)
        {
            // behave
            Transform waypoint = _owner.waypoints[_owner.waypointIndex];
            if (NavMesh.SamplePosition(waypoint.position, out var sample, 5f, ~0))
            {
                _owner.navMeshAgent.SetDestination(sample.position);
                _owner.navMeshAgent.isStopped = false;

                // set execution time limit
                timeExpectedEnd = time + 120f;
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
            // switch to next waypoint:
            _owner.waypointIndex = (_owner.waypointIndex+1)%_owner.waypoints.Length;
        }

        public override void Tick(float time)
        {
            completed |= _owner.navMeshAgent.remainingDistance<1f;
        }

    }
}

using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    public class SurvivorFleeState : SimpletonState
    {

        public float remainingDistance;
        float _nextPathUpdateTime;
        SurvivorController _owner;

        public SurvivorFleeState(SurvivorController owner)
        {
            _owner = owner;
        }

        public override void OnEnter(SimpletonState previous, float time)
        {
            // set execution time limit
            timeExpectedEnd = time + 10f;
            _nextPathUpdateTime = time + 0.7f;
        }

        public override void OnExit(SimpletonState next)
        {
            
        }

        public override void Tick(float time)
        {
            // follow target
            remainingDistance = _owner.navMeshAgent.remainingDistance;
            if (remainingDistance<1f) completed = true;

            if (time>_nextPathUpdateTime)
            {
                if (_owner.zombiesAround.Count!=0)
                {
                    Vector3 pos = _owner.transform.position;
                    NavMeshAgent nearest = null;
                    float nearestDist = float.MaxValue;
                    foreach (var next in _owner.zombiesAround)
                    {
                        float nextDist = Vector3.Distance(pos, next.transform.position);
                        if (nextDist<nearestDist)
                        {
                            nearest = next;
                            nearestDist = nextDist;
                        }
                    }
                    _owner.navMeshAgent.SetDestination( pos + Vector3.Normalize(pos-nearest.transform.position)*6f);
                    _owner.navMeshAgent.isStopped = false;
                    _nextPathUpdateTime = time + 0.7f;
                }
                else completed = true;
            }
        }

    }
}

using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    public class WalkerAgroState : SimpletonState
    {

        public bool readyToAttack;
        public SurvivorController victimToFollow;

        float _nextPathUpdateTime;
        WalkerController _owner;

        public WalkerAgroState(WalkerController owner)
        {
            _owner = owner;
        }

        public override void OnEnter(SimpletonState previous , float time)
        {
            // set execution time limit
            timeExpectedEnd = time + 60f;
            _nextPathUpdateTime = time + 0.7f;

            //behave:
            _owner.navMeshAgent.speed *= 2;
            // todo: play audio etc.
        }

        public override void OnExit(SimpletonState next)
        {
            if (next is WalkerAttackState)
            {
                var attack = next as WalkerAttackState;
                attack.attackTarget = victimToFollow;
            }

            //undo:
            _owner.navMeshAgent.speed /= 2;
            victimToFollow = null;
            readyToAttack = false;
        }

        public override void Tick(float time)
        {
            // exit when there are no victims around:
            if (_owner.survivorsAround.Count==0)
            {
                completed = true;
                return;
            }
            
            // follow nearest victim
            if (time>_nextPathUpdateTime)
            {
                // find nearest victim:
                Vector3 pos = _owner.transform.position;
                float nearestDist = float.MaxValue;
                foreach (var next in _owner.survivorsAround)
                {
                    float nextDist = Vector3.Distance( pos , next.transform.position);
                    if (nextDist<nearestDist)
                    {
                        victimToFollow = next;
                        nearestDist = nextDist;
                    }
                }

                _owner.navMeshAgent.SetDestination( victimToFollow.transform.position);
                _owner.navMeshAgent.isStopped = false;
                _nextPathUpdateTime = time + 0.333f;
            }
        }

    }
}

using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    public class SurvivorIdleState : SimpletonState
    {

        SurvivorController _owner;

        public SurvivorIdleState(SurvivorController owner)
        {
            _owner = owner;
        }

        public override void OnEnter(SimpletonState previous, float time)
        {
            // set state time limit
            timeExpectedEnd = time + Random.Range(3f, 20f);

            //behave
            _owner.navMeshAgent.isStopped = true;
            // todo: play idle animation, etc
        }

        public override void OnExit(SimpletonState next)
        {
            
        }

        public override void Tick(float time)
        {
            
        }

    }
}

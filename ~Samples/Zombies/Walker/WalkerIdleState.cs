using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    public class WalkerIdleState : SimpletonState
    {
        
        WalkerController _owner;

        public WalkerIdleState ( WalkerController owner )
        {
            _owner = owner;
        }

        public override void OnEnter ( SimpletonState previous , float time )
        {
            // set state time limit
            timeExpectedEnd = time + Random.Range( 3f , 20f );

            // behave
            _owner.navMeshAgent.isStopped = true;
            // todo: play idle animation, etc
        }

        public override void OnExit ( SimpletonState next )
        {
            
        }

        public override void Tick ( float time )
        {
            
        }

    }
}

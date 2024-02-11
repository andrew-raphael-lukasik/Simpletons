using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    public class WalkerDeadState : SimpletonState
    {
        
        WalkerController _owner;

        public WalkerDeadState ( WalkerController owner )
        {
            _owner = owner;
        }
        
        public override void OnEnter ( SimpletonState previous , float time )
        {
            // set execution time limit
            timeExpectedEnd = float.MaxValue;
            
            // make it dead:
            _owner.enabled = false;
            _owner.navMeshAgent.enabled = false;

            // todo: start dead animation, play death sound etc.
            _owner.transform.localScale = new (2,0.1f,2);// temporary death animation :V
        }

        public override void OnExit ( SimpletonState next )
        {
            // undo:
            _owner.transform.localScale = new (1,1,1);
        }

        public override void Tick ( float time )
        {
            
        }

    }
}

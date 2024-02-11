using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    public class SurvivorDeadState : SimpletonState
    {
        
        SurvivorController _owner;

        public SurvivorDeadState ( SurvivorController owner )
        {
            _owner = owner;
        }
        
        public override void OnEnter ( SimpletonState previous , float time )
        {
            // set execution time limit
            timeExpectedEnd = time + 5f;
            
            // make it dead:
            _owner.enabled = false;
            _owner.navMeshAgent.enabled = false;
            
            // todo: start dead animation, play death sound etc.
            _owner.transform.localScale = new (2,0.1f,2);// temporary death animation :V
        }

        public override void OnExit ( SimpletonState next )
        {
            
        }

        public override void Tick ( float time )
        {
            completed |= time>timeExpectedEnd;
        }

    }
}

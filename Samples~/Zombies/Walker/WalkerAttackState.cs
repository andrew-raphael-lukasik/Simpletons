using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    public class WalkerAttackState : SimpletonState
    {
        
        public SurvivorController attackTarget;
        
        WalkerController _owner;

        public WalkerAttackState ( WalkerController owner )
        {
            _owner = owner;
        }
        
        public override void OnEnter ( SimpletonState previous , float time )
        {
            // set execution time limit
            timeExpectedEnd = time + 0.5f;

            // apply damage:
            if( attackTarget!=null )
            {
                attackTarget.Health -= 60;

                // remove dead victim from list:
                if( attackTarget.Health<=0 )
                {
                    _owner.survivorsAround.Remove( attackTarget );
                }
            }

            // todo: play attack animation, audio etc.

            // mark state as completed when animation ends:
            // completed = true;
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

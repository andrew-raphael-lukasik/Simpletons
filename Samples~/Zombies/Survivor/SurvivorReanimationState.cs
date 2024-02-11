using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    public class SurvivorReanimationState : SimpletonState
    {
        
        SurvivorController _owner;

        public SurvivorReanimationState ( SurvivorController owner )
        {
            _owner = owner;
        }
        
        public override void OnEnter ( SimpletonState previous , float time )
        {
            // set execution time limit
            timeExpectedEnd = time + 3f;
        }

        public override void OnExit ( SimpletonState next )
        {
            
        }

        public override void Tick ( float time )
        {
            if( time>timeExpectedEnd )
            {
                _owner.gameObject.SetActive( false );
                GameObject.Instantiate( _owner.zombieFormPrefab , _owner.transform.position , Quaternion.Euler(0,Random.Range(0,360),0) );
                Object.Destroy( _owner );
            }
        }

    }
}

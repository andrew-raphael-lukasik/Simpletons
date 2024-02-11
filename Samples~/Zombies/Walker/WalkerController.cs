using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    [SelectionBase]
    public class WalkerController : MonoBehaviour
    {
        public int Health = 100;

        public NavMeshAgent navMeshAgent;
        public List<SurvivorController> survivorsAround = new ();
        public float meleeAttackRange = 1.5f;

        [SerializeField] WalkerAI _brain;

        void OnEnable ()
        {
            _brain = new WalkerAI( owner:this );
            const float tickRate = 1f/3f;
            InvokeRepeating( nameof(Tick) , Random.Range(0,tickRate) , tickRate );
        }

        void OnTriggerEnter ( Collider other )
        {
            if( !other.isTrigger && other.TryGetComponent<SurvivorController>(out var comp) && comp.enabled )
            {
                survivorsAround.Add( comp );
            }
        }

        void OnTriggerExit ( Collider other )
        {
            if( other.TryGetComponent<SurvivorController>(out var comp) )
            {
                survivorsAround.Remove( comp );
            }
        }

        void Tick ()
        {
            _brain.Tick( time:Time.time );
        }

        #if UNITY_EDITOR
        void OnDrawGizmos ()
        {
            if( !Application.isPlaying ) return;

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.yellow;
            UnityEditor.Handles.Label( transform.position , $"<{_brain?.Current?.GetType().Name}>\nHealth: {Health}%" , style );

            if( survivorsAround.Count!=0 )
            {
                Vector3 pos = transform.position + new Vector3(0,2,0);
                Gizmos.color = Color.red;
                foreach( var next in survivorsAround )
                {
                    Gizmos.DrawLine( pos , next.transform.position );
                }
            }
        }
        #endif

    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Zombies
{
    [SelectionBase]
    public class SurvivorController : MonoBehaviour
    {
        public int Health = 100;
        public NavMeshAgent navMeshAgent;
        public List<NavMeshAgent> zombiesAround = new ();
        public GameObject zombieFormPrefab;

        [SerializeField] SurvivorAI _brain;

        void OnEnable ()
        {
            _brain = new SurvivorAI( owner:this );
            const float tickRate = 1f/3f;
            InvokeRepeating( nameof(Tick) , Random.Range(0,tickRate) , tickRate );
        }

        void OnTriggerEnter ( Collider other )
        {
            if( !other.isTrigger && other.TryGetComponent<WalkerController>(out var comp) && comp.enabled )
            {
                zombiesAround.Add( comp.navMeshAgent );
            }
        }

        void OnTriggerExit ( Collider other )
        {
            if( other.TryGetComponent<WalkerController>(out var comp) )
            {
                zombiesAround.Remove( comp.navMeshAgent );
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

            if( zombiesAround.Count!=0 )
            {
                Vector3 pos = transform.position + new Vector3(0,2,0);
                Gizmos.color = Color.red;
                foreach( var next in zombiesAround )
                {
                    Gizmos.DrawLine( pos , next.transform.position );
                }
            }
        }
        #endif
        
    }
}

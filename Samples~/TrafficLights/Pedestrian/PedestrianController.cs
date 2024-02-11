using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Crosswalk
{
    [SelectionBase]
    public class PedestrianController : MonoBehaviour
    {
        public NavMeshAgent navMeshAgent;
        public float meleeAttackRange = 1.5f;
        public Transform[] waypoints = new Transform[0];
        public int waypointIndex = -1;

        [SerializeField] PedestrianAI _brain;
        public event System.Action<CrosswalkLights> onCrosswalkLightsReached;
        
        void OnEnable ()
        {
            _brain = new PedestrianAI( owner:this );
            if( waypointIndex==-1 ) waypointIndex = Random.Range( 0 , waypoints.Length );
            const float tickRate = 1f/3f;
            InvokeRepeating( nameof(Tick) , Random.Range(0,tickRate) , tickRate );
        }

        void OnTriggerEnter ( Collider other )
        {
            if( other.TryGetComponent<CrosswalkLights>(out var comp) && comp.enabled )
            {
                onCrosswalkLightsReached(comp);
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
            UnityEditor.Handles.Label( transform.position , $"<{_brain?.Current?.GetType().Name}>" , style );
        }
        #endif

    }
}

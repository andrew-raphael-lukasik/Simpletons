using UnityEngine;
using UnityEngine.AI;
using Simpleton;

namespace Simpleton.Samples.Crosswalk
{
    public class PedestrianStopState : SimpletonState
    {

        PedestrianController _owner;
        public bool triggered;
        CrosswalkLights _crosswalkLightsReached;

        public PedestrianStopState(PedestrianController owner)
        {
            _owner = owner;
            _owner.onCrosswalkLightsReached += OnCrosswalkLightsReached;
        }

        ~PedestrianStopState ()
        {
            _owner.onCrosswalkLightsReached -= OnCrosswalkLightsReached;
        }

        public override void OnEnter(SimpletonState previous, float time)
        {
            // "consume" trigger:
            triggered = false;

            // set state time limit
            timeExpectedEnd = time + 300f;

            // behave
            _owner.navMeshAgent.isStopped = true;
            // todo: play stop animation, etc
        }

        public override void OnExit(SimpletonState next)
        {
            // reset:
            _crosswalkLightsReached = null;
        }

        public override void Tick(float time)
        {
            completed |= _crosswalkLightsReached.state==CrosswalkLights.EState.GO;
        }

        void OnCrosswalkLightsReached(CrosswalkLights comp)
        {
            if (comp.state!=CrosswalkLights.EState.GO)
            {
                triggered = true;
                _crosswalkLightsReached = comp;
            }
        }

    }
}

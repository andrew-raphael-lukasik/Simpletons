using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Simpleton.Samples.Crosswalk
{
    public class CrosswalkLights : MonoBehaviour
    {

        [SerializeField] EState _state = EState.STOP;
        public EState state => _state;

        [SerializeField] Text _text;

        IEnumerator Start()
        {
            var switchTime = new WaitForSeconds(5f);
            while (true)
            {
                yield return switchTime;
                
                if (_state==EState.STOP)
                {
                    _state = EState.GO;
                    _text.text = nameof(EState.GO);
                    _text.color = Color.green;
                }
                else if (_state==EState.GO)
                {
                    _state = EState.STOP;
                    _text.text = nameof(EState.STOP);
                    _text.color = Color.red;
                }
            }
        }

        public enum EState : byte
        {
            STOP = 0 ,
            GO = 1
        }

    }
}

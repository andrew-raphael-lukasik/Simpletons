using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simpleton
{
    public class RingBuffer <T> : IEnumerable, IEnumerable<T>
    {
        
        public readonly T[] Buffer;
        public readonly int Capacity;
        public int Fill;
        
        /// <summary> Current index </summary>
        /// <remarks> (not next one) </remarks>
        int Index;

        [System.Obsolete("Must set capacity",true)]
        public RingBuffer () {}
        public RingBuffer ( int capacity )
        {
            Buffer = new T[ capacity ];
            Capacity = capacity;
            Index = 0;
            Fill = 0;
        }
        
        public void Push ( T value )
        {
            Buffer[ Index++ ] = value;
            if( Index==Capacity ) Index = 0;
            Fill = Mathf.Min( Fill+1 , Capacity );
        }

        public T Peek () => Buffer[Index];

        IEnumerator IEnumerable.GetEnumerator ()
        {
            int startingIndex = Index - Fill;
            if( startingIndex<0 ) startingIndex += Fill;
            int count = 0;
            for( int i=startingIndex ; i<Capacity && count<Fill ; i++, count++)
            {
                yield return Buffer[i>=0?i:Capacity+i];
            }
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator ()
        {
            int startingIndex = Index - Fill;
            if( startingIndex<0 ) startingIndex += Fill;
            int count = 0;
            for( int i=startingIndex ; i<Capacity && count<Fill ; i++, count++)
            {
                yield return Buffer[i>=0?i:Capacity+i];
            }
        }

        public T[] ToArray ()
        {
            if( Capacity==0 ) return new T[0];
            T[] output = new T[ Capacity ];
            int o = 0;
            int startingIndex = Index - Fill;
            if( startingIndex<0 ) startingIndex += Fill;
            int count = 0;
            for( int i=startingIndex ; i<Capacity && count<Fill ; i++, count++)
            {
                output[o++] = Buffer[i>=0?i:Capacity+i];
            }
            return output;
        }

    }
}

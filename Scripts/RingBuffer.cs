using System.Collections;
using System.Collections.Generic;

namespace Simpleton
{
	[System.Serializable]
	public class RingBuffer <T> : IEnumerable, IEnumerable<T>
	{

		readonly T[] _array;
		readonly int Length;
	
		/// <summary> Current index </summary>
		/// <remarks> (not next one) </remarks>
		int _index;

		[System.Obsolete("don't",true)]
		public RingBuffer () {}
		public RingBuffer ( int capacity )
		{
			this._array = new T[ capacity ];
			this.Length = capacity;
			this._index = 0;
		}
	
		public void Push ( T value )
		{
			_array[ _index++ ] = value;
			if( _index==Length ) _index = 0;
		}

		public T Peek () => _array[_index];

		IEnumerator IEnumerable.GetEnumerator ()
		{
			for( int i=_index ; i<Length ; i++ ) yield return _array[i];
			for( int i=0 ; i<_index ; i++ ) yield return _array[i];
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator ()
		{
			for( int i=_index ; i<Length ; i++ ) yield return _array[i];
			for( int i=0 ; i<_index ; i++ ) yield return _array[i];
		}

		public T[] AsArray () => _array;

	}
}

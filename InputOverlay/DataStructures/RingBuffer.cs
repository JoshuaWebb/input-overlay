using System;
using System.Collections;
using System.Collections.Generic;

namespace JoshuaWebb.DataStructures
{
   public class RingBuffer<T> : IEnumerable,
                                IEnumerable<T>
   {
      #region Private member variables

      private T[] _buffer;
      private int _start;
      private int _count;

      #endregion

      #region Private properties

      private int Start
      {
         get
         {
            return _start;
         }
         set
         {
            _start = value;
         }
      }

      #endregion

      #region Public properties

      public T[] Buffer
      {
         get
         {
            return _buffer;
         }
      }

      public int Count
      {
         get
         {
            return _count;
         }
         private set
         {
            _count = value;
         }
      }

      public bool Empty
      {
         get
         {
            return Count == 0;
         }
      }

      public bool Full
      {
         get
         {
            return Count == Buffer.Length;
         }
      }

      public int Capacity
      {
         get
         {
            return Buffer.Length;
         }
      }

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes the RingBuffer as if each element of the array was 
      /// Enqueued into a RingBuffer with a capacity equal to the size of the
      /// array.
      /// </summary>
      public RingBuffer(T[] array)
         : this(array, array.Length) { }

      /// <summary>
      /// Initializes the RingBuffer as if each element of the array was 
      /// Enqueued into a RingBuffer with the given capacity.
      /// </summary>
      public RingBuffer(T[] array, int capacity)
      {
         if (array.Length == 0 || capacity < 1)
         {
            throw new ArgumentOutOfRangeException("Size can't be less than 1");
         }

         var numToCopy = Math.Min(array.Length, capacity);
         _buffer = new T[capacity];
         Array.Copy(array, array.Length - numToCopy,
                    _buffer, 0, numToCopy);

         _count = numToCopy;
         _start = 0;
      }

      /// <summary>
      /// Copy constructor (shallow coppy)
      /// </summary>
      public RingBuffer(RingBuffer<T> other)
      {
         _buffer = new T[other.Capacity];
         other.copyAsManyAsPossibleTo(_buffer, 0);

         _count = other.Count;
         _start = other.Start;
      }

      public RingBuffer(int size)
      {
         if (size < 1)
         {
            throw new ArgumentOutOfRangeException("Size can't be less than 1");
         }

         _buffer = new T[size];
         _count = 0;
         _start = 0;
      }

      #endregion

      #region Private helper functions

      private int wrapIndex(int index)
      {
         // Use longs to avoid overflow in the intermediate calculations.
         return (int)((index + Buffer.LongLength) % Buffer.LongLength);
      }

      private void copyAsManyAsPossibleTo(T[] array, int startDest)
      {
         // First determine how many items there are to the left of the Start 
         // (i.e. the newest items). Then determine how many to copy from the
         // right (if any), then copy from the right into the start of the
         // target, then copy the left next to the right in the target.
         //
         // This process covers every (possible) combination of RingBuffer size
         // Array size, Count, and whether or not the items wrap around.

         int dSize = array.Length - startDest;

         // This may be 0.
         int numOnLeftSide = Math.Min(wrapIndex(Start + Count), dSize);

         // The amount is dictated by the number we are going to copy from
         // the left, and the smaller of the amount of space in the target
         // and the amount of items we have left. 
         // This may be 0.
         int numToCopyFromRightSide = Math.Min(Count, dSize) - numOnLeftSide;

         // Count backwards from the right edge, rather than forwards from the
         // start, so that we get the rightmost elements.
         int startRight = Buffer.Length - numToCopyFromRightSide;

         // Copy the rightmost elements to the start of the new array (if any).
         Array.Copy(Buffer, startRight,
                     array, startDest, numToCopyFromRightSide);

         // Then copy the leftmost elements next to them (if any).
         Array.Copy(Buffer, wrapIndex(Start + Count) - numOnLeftSide,
                     array, startDest + numToCopyFromRightSide, numOnLeftSide);
      }

      #endregion

      #region Projections

      public T[] ToArray()
      {
         T[] array = new T[Count];
         copyAsManyAsPossibleTo(array, 0);

         return array;
      }

      public void CopyTo(T[] array, int index)
      {
         if (ReferenceEquals(array, null))
         {
            throw new ArgumentNullException();
         }

         if (index < 0)
         {
            throw new ArgumentOutOfRangeException();
         }

         // Check if we have space to copy all elements.
         if (array.Length - index < Count)
         {
            throw new ArgumentException("Not enough space in target array");
         }

         copyAsManyAsPossibleTo(array, index);
      }

      #endregion

      #region Mutators

      public void Clear()
      {
         Count = 0;
      }

      public void Enqueue(T item)
      {
         if (Full)
         {
            // Replace the start item.
            Buffer[Start] = item;
            Start = wrapIndex(Start + 1);
         }
         else
         {
            // Place after the currently last item.
            Buffer[wrapIndex(Start + Count)] = item;
            Count++;
         }
      }

      public T Dequeue()
      {
         if (Empty)
         {
            throw new InvalidOperationException("Buffer is empty");
         }

         T value = Buffer[Start];
         Start = wrapIndex(Start + 1);
         Count--;

         return value;
      }

      public void Resize(int newSize)
      {
         if (newSize < 1)
         {
            throw new ArgumentOutOfRangeException("Size can't be less than 1");
         }

         // Resizing to current size.
         if (newSize == Buffer.Length)
         {
            // Smooth resize fella.
            return;
         }

         // For simplicity, copy to temporary buffer and replace, rather than
         // attempting to resize in place.
         T[] temp = new T[newSize];
         copyAsManyAsPossibleTo(temp, 0);

         // If Shrinking.
         if (newSize < Buffer.Length)
         {
            // If we have more items than the new buffer can fit.
            if (Count > newSize)
            {
               Count = newSize;
            }
         }

         Start = 0;
         _buffer = temp;
      }

      #endregion

      #region Enumeration

      /// <summary>
      /// FIFO order
      /// </summary>
      public IEnumerable<T> Forwards()
      {
         int currIdx = Start;
         int count = 0;
         while (count < Count)
         {
            yield return Buffer[currIdx];
            currIdx = wrapIndex(currIdx + 1);
            count++;
         }
      }

      /// <summary>
      /// LIFO order
      /// </summary>
      public IEnumerable<T> Backwards()
      {
         int currIdx = wrapIndex(Start + Count - 1);
         int count = 0;
         while (count < Count)
         {
            yield return Buffer[currIdx];
            currIdx = wrapIndex(currIdx - 1);
            count++;
         }
      }

      public IEnumerator<T> GetEnumerator()
      {
         return Forwards().GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion
   }
}

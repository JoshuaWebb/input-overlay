using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joshua.Webb.Util
{
   public class OrderedSet<T> : IEnumerable,
                                IEnumerable<T>
                              
   {
      #region Private member variables

      // The list maintains the order.
      private readonly LinkedList<T> _list;

      // The dictionary maintains unique-ness.
      //
      // NOTE:
      //   Key = The replacement.
      // Value = The node in the list.
      private readonly IDictionary<T, LinkedListNode<T>> _dict;

      #endregion

      #region Private properties

      private LinkedList<T> List
      {
         get
         {
            return _list;
         }
      }
      private IDictionary<T, LinkedListNode<T>> Dict
      {
         get
         {
            return _dict;
         }
      }

      #endregion

      #region Constructors

      public OrderedSet()
      {
         _list = new LinkedList<T>();
         _dict = new Dictionary<T, LinkedListNode<T>>();
      }

      #endregion

      #region Accessors

      private LinkedListNode<T> nodeFor(T item)
      {
         LinkedListNode<T> node;
         if (Dict.TryGetValue(item, out node))
         {
            return node;
         }
         else
         {
            return null;
         }
      }

      public bool Contains(T item)
      {
         LinkedListNode<T> ignored;
         return Dict.TryGetValue(item, out ignored);
      }

      #endregion Accessors

      #region Mutators

      public bool Add(T item)
      {
         if (Contains(item))
         {
            return false;
         }
         else
         {
            var node = List.AddLast(item);
            Dict.Add(item, node);

            return true;
         }
      }

      public void Remove(T item)
      {
         LinkedListNode<T> nodeToRemove;
         if (Dict.TryGetValue(item, out nodeToRemove))
         {
            Dict.Remove(item);
            List.Remove(nodeToRemove);
         }
      }

      public bool Replace(T itemToReplace, T replacement)
      {
         // Item already exists in set.
         if(Contains(replacement))
         {
            return false;
         }

         LinkedListNode<T> nodeToReplace = nodeFor(itemToReplace);

         // Item to replace isn't in set.
         if (nodeToReplace == null)
         {
            return false;
         }

         replaceNode(nodeToReplace, replacement);
         return true;
      }

      // Perform the actual replacement
      private void replaceNode(LinkedListNode<T> nodeToReplace, T replacement)
      {
         // Grab the old value out of the node.
         T old = nodeToReplace.Value;

         // Keep the node in the list in it's current position, but change
         // the value that the node contains.
         nodeToReplace.Value = replacement;

         // Remove the old key from the dictionary because the set no longer
         // contains this value.
         Dict.Remove(old);

         // Add the node back in using the new key for the new replacement.
         Dict.Add(replacement, nodeToReplace);
      }

      #endregion

      #region IEnumerable

      public IEnumerator<T> GetEnumerator()
      {
         return List.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return List.GetEnumerator();
      }

      #endregion
   }
}

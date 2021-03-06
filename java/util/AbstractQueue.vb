Imports System.Collections.Generic

'
' * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' 

'
' *
' *
' *
' *
' *
' * Written by Doug Lea with assistance from members of JCP JSR-166
' * Expert Group and released to the public domain, as explained at
' * http://creativecommons.org/publicdomain/zero/1.0/
' 

Namespace java.util

	''' <summary>
	''' This class provides skeletal implementations of some <seealso cref="Queue"/>
	''' operations. The implementations in this class are appropriate when
	''' the base implementation does <em>not</em> allow <tt>null</tt>
	''' elements.  Methods <seealso cref="#add add"/>, <seealso cref="#remove remove"/>, and
	''' <seealso cref="#element element"/> are based on <seealso cref="#offer offer"/>, {@link
	''' #poll poll}, and <seealso cref="#peek peek"/>, respectively, but throw
	''' exceptions instead of indicating failure via <tt>false</tt> or
	''' <tt>null</tt> returns.
	''' 
	''' <p>A <tt>Queue</tt> implementation that extends this class must
	''' minimally define a method <seealso cref="Queue#offer"/> which does not permit
	''' insertion of <tt>null</tt> elements, along with methods {@link
	''' Queue#peek}, <seealso cref="Queue#poll"/>, <seealso cref="Collection#size"/>, and
	''' <seealso cref="Collection#iterator"/>.  Typically, additional methods will be
	''' overridden as well.  If these requirements cannot be met, consider
	''' instead subclassing <seealso cref="AbstractCollection"/>.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <E> the type of elements held in this collection </param>
	Public MustInherit Class AbstractQueue(Of E)
		Inherits AbstractCollection(Of E)
		Implements Queue(Of E)

		''' <summary>
		''' Constructor for use by subclasses.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Inserts the specified element into this queue if it is possible to do so
		''' immediately without violating capacity restrictions, returning
		''' <tt>true</tt> upon success and throwing an <tt>IllegalStateException</tt>
		''' if no space is currently available.
		''' 
		''' <p>This implementation returns <tt>true</tt> if <tt>offer</tt> succeeds,
		''' else throws an <tt>IllegalStateException</tt>.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <returns> <tt>true</tt> (as specified by <seealso cref="Collection#add"/>) </returns>
		''' <exception cref="IllegalStateException"> if the element cannot be added at this
		'''         time due to capacity restrictions </exception>
		''' <exception cref="ClassCastException"> if the class of the specified element
		'''         prevents it from being added to this queue </exception>
		''' <exception cref="NullPointerException"> if the specified element is null and
		'''         this queue does not permit null elements </exception>
		''' <exception cref="IllegalArgumentException"> if some property of this element
		'''         prevents it from being added to this queue </exception>
		Public Overridable Function add(  e As E) As Boolean
			If offer(e) Then
				Return True
			Else
				Throw New IllegalStateException("Queue full")
			End If
		End Function

		''' <summary>
		''' Retrieves and removes the head of this queue.  This method differs
		''' from <seealso cref="#poll poll"/> only in that it throws an exception if this
		''' queue is empty.
		''' 
		''' <p>This implementation returns the result of <tt>poll</tt>
		''' unless the queue is empty.
		''' </summary>
		''' <returns> the head of this queue </returns>
		''' <exception cref="NoSuchElementException"> if this queue is empty </exception>
		Public Overridable Function remove() As E Implements Queue(Of E).remove
			Dim x As E = poll()
			If x IsNot Nothing Then
				Return x
			Else
				Throw New NoSuchElementException
			End If
		End Function

		''' <summary>
		''' Retrieves, but does not remove, the head of this queue.  This method
		''' differs from <seealso cref="#peek peek"/> only in that it throws an exception if
		''' this queue is empty.
		''' 
		''' <p>This implementation returns the result of <tt>peek</tt>
		''' unless the queue is empty.
		''' </summary>
		''' <returns> the head of this queue </returns>
		''' <exception cref="NoSuchElementException"> if this queue is empty </exception>
		Public Overridable Function element() As E Implements Queue(Of E).element
			Dim x As E = peek()
			If x IsNot Nothing Then
				Return x
			Else
				Throw New NoSuchElementException
			End If
		End Function

		''' <summary>
		''' Removes all of the elements from this queue.
		''' The queue will be empty after this call returns.
		''' 
		''' <p>This implementation repeatedly invokes <seealso cref="#poll poll"/> until it
		''' returns <tt>null</tt>.
		''' </summary>
		Public Overridable Sub clear()
			Do While poll() IsNot Nothing

			Loop
		End Sub

		''' <summary>
		''' Adds all of the elements in the specified collection to this
		''' queue.  Attempts to addAll of a queue to itself result in
		''' <tt>IllegalArgumentException</tt>. Further, the behavior of
		''' this operation is undefined if the specified collection is
		''' modified while the operation is in progress.
		''' 
		''' <p>This implementation iterates over the specified collection,
		''' and adds each element returned by the iterator to this
		''' queue, in turn.  A runtime exception encountered while
		''' trying to add an element (including, in particular, a
		''' <tt>null</tt> element) may result in only some of the elements
		''' having been successfully added when the associated exception is
		''' thrown.
		''' </summary>
		''' <param name="c"> collection containing elements to be added to this queue </param>
		''' <returns> <tt>true</tt> if this queue changed as a result of the call </returns>
		''' <exception cref="ClassCastException"> if the class of an element of the specified
		'''         collection prevents it from being added to this queue </exception>
		''' <exception cref="NullPointerException"> if the specified collection contains a
		'''         null element and this queue does not permit null elements,
		'''         or if the specified collection is null </exception>
		''' <exception cref="IllegalArgumentException"> if some property of an element of the
		'''         specified collection prevents it from being added to this
		'''         queue, or if the specified collection is this queue </exception>
		''' <exception cref="IllegalStateException"> if not all the elements can be added at
		'''         this time due to insertion restrictions </exception>
		''' <seealso cref= #add(Object) </seealso>
		Public Overridable Function addAll(Of T1 As E)(  c As Collection(Of T1)) As Boolean
			If c Is Nothing Then Throw New NullPointerException
			If c Is Me Then Throw New IllegalArgumentException
			Dim modified As Boolean = False
			For Each e As E In c
				If add(e) Then modified = True
			Next e
			Return modified
		End Function

	End Class

End Namespace
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1994, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util

	''' <summary>
	''' This class represents an observable object, or "data"
	''' in the model-view paradigm. It can be subclassed to represent an
	''' object that the application wants to have observed.
	''' <p>
	''' An observable object can have one or more observers. An observer
	''' may be any object that implements interface <tt>Observer</tt>. After an
	''' observable instance changes, an application calling the
	''' <code>Observable</code>'s <code>notifyObservers</code> method
	''' causes all of its observers to be notified of the change by a call
	''' to their <code>update</code> method.
	''' <p>
	''' The order in which notifications will be delivered is unspecified.
	''' The default implementation provided in the Observable class will
	''' notify Observers in the order in which they registered interest, but
	''' subclasses may change this order, use no guaranteed order, deliver
	''' notifications on separate threads, or may guarantee that their
	''' subclass follows this order, as they choose.
	''' <p>
	''' Note that this notification mechanism has nothing to do with threads
	''' and is completely separate from the <tt>wait</tt> and <tt>notify</tt>
	''' mechanism of class <tt>Object</tt>.
	''' <p>
	''' When an observable object is newly created, its set of observers is
	''' empty. Two observers are considered the same if and only if the
	''' <tt>equals</tt> method returns true for them.
	''' 
	''' @author  Chris Warth </summary>
	''' <seealso cref=     java.util.Observable#notifyObservers() </seealso>
	''' <seealso cref=     java.util.Observable#notifyObservers(java.lang.Object) </seealso>
	''' <seealso cref=     java.util.Observer </seealso>
	''' <seealso cref=     java.util.Observer#update(java.util.Observable, java.lang.Object)
	''' @since   JDK1.0 </seealso>
	Public Class Observable
		Private changed As Boolean = False
		Private obs As Vector(Of Observer)

		''' <summary>
		''' Construct an Observable with zero Observers. </summary>

		Public Sub New()
			obs = New Vector(Of )
		End Sub

		''' <summary>
		''' Adds an observer to the set of observers for this object, provided
		''' that it is not the same as some observer already in the set.
		''' The order in which notifications will be delivered to multiple
		''' observers is not specified. See the class comment.
		''' </summary>
		''' <param name="o">   an observer to be added. </param>
		''' <exception cref="NullPointerException">   if the parameter o is null. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addObserver(  o As Observer)
			If o Is Nothing Then Throw New NullPointerException
			If Not obs.contains(o) Then obs.addElement(o)
		End Sub

		''' <summary>
		''' Deletes an observer from the set of observers of this object.
		''' Passing <CODE>null</CODE> to this method will have no effect. </summary>
		''' <param name="o">   the observer to be deleted. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub deleteObserver(  o As Observer)
			obs.removeElement(o)
		End Sub

		''' <summary>
		''' If this object has changed, as indicated by the
		''' <code>hasChanged</code> method, then notify all of its observers
		''' and then call the <code>clearChanged</code> method to
		''' indicate that this object has no longer changed.
		''' <p>
		''' Each observer has its <code>update</code> method called with two
		''' arguments: this observable object and <code>null</code>. In other
		''' words, this method is equivalent to:
		''' <blockquote><tt>
		''' notifyObservers(null)</tt></blockquote>
		''' </summary>
		''' <seealso cref=     java.util.Observable#clearChanged() </seealso>
		''' <seealso cref=     java.util.Observable#hasChanged() </seealso>
		''' <seealso cref=     java.util.Observer#update(java.util.Observable, java.lang.Object) </seealso>
		Public Overridable Sub notifyObservers()
			notifyObservers(Nothing)
		End Sub

		''' <summary>
		''' If this object has changed, as indicated by the
		''' <code>hasChanged</code> method, then notify all of its observers
		''' and then call the <code>clearChanged</code> method to indicate
		''' that this object has no longer changed.
		''' <p>
		''' Each observer has its <code>update</code> method called with two
		''' arguments: this observable object and the <code>arg</code> argument.
		''' </summary>
		''' <param name="arg">   any object. </param>
		''' <seealso cref=     java.util.Observable#clearChanged() </seealso>
		''' <seealso cref=     java.util.Observable#hasChanged() </seealso>
		''' <seealso cref=     java.util.Observer#update(java.util.Observable, java.lang.Object) </seealso>
		Public Overridable Sub notifyObservers(  arg As Object)
	'        
	'         * a temporary array buffer, used as a snapshot of the state of
	'         * current Observers.
	'         
			Dim arrLocal As Object()

			SyncLock Me
	'             We don't want the Observer doing callbacks into
	'             * arbitrary code while holding its own Monitor.
	'             * The code where we extract each Observable from
	'             * the Vector and store the state of the Observer
	'             * needs synchronization, but notifying observers
	'             * does not (should not).  The worst result of any
	'             * potential race-condition here is that:
	'             * 1) a newly-added Observer will miss a
	'             *   notification in progress
	'             * 2) a recently unregistered Observer will be
	'             *   wrongly notified when it doesn't care
	'             
				If Not changed Then Return
				arrLocal = obs.ToArray()
				clearChanged()
			End SyncLock

			For i As Integer = arrLocal.Length-1 To 0 Step -1
				CType(arrLocal(i), Observer).update(Me, arg)
			Next i
		End Sub

		''' <summary>
		''' Clears the observer list so that this object no longer has any observers.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub deleteObservers()
			obs.removeAllElements()
		End Sub

		''' <summary>
		''' Marks this <tt>Observable</tt> object as having been changed; the
		''' <tt>hasChanged</tt> method will now return <tt>true</tt>.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub setChanged()
			changed = True
		End Sub

		''' <summary>
		''' Indicates that this object has no longer changed, or that it has
		''' already notified all of its observers of its most recent change,
		''' so that the <tt>hasChanged</tt> method will now return <tt>false</tt>.
		''' This method is called automatically by the
		''' <code>notifyObservers</code> methods.
		''' </summary>
		''' <seealso cref=     java.util.Observable#notifyObservers() </seealso>
		''' <seealso cref=     java.util.Observable#notifyObservers(java.lang.Object) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub clearChanged()
			changed = False
		End Sub

		''' <summary>
		''' Tests if this object has changed.
		''' </summary>
		''' <returns>  <code>true</code> if and only if the <code>setChanged</code>
		'''          method has been called more recently than the
		'''          <code>clearChanged</code> method on this object;
		'''          <code>false</code> otherwise. </returns>
		''' <seealso cref=     java.util.Observable#clearChanged() </seealso>
		''' <seealso cref=     java.util.Observable#setChanged() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function hasChanged() As Boolean
			Return changed
		End Function

		''' <summary>
		''' Returns the number of observers of this <tt>Observable</tt> object.
		''' </summary>
		''' <returns>  the number of observers of this object. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function countObservers() As Integer
			Return obs.size()
		End Function
	End Class

End Namespace
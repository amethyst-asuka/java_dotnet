Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans.beancontext





	''' <summary>
	''' <p>
	''' This is a general support class to provide support for implementing the
	''' BeanContextChild protocol.
	''' 
	''' This class may either be directly subclassed, or encapsulated and delegated
	''' to in order to implement this interface for a given component.
	''' </p>
	''' 
	''' @author      Laurence P. G. Cable
	''' @since       1.2
	''' </summary>
	''' <seealso cref= java.beans.beancontext.BeanContext </seealso>
	''' <seealso cref= java.beans.beancontext.BeanContextServices </seealso>
	''' <seealso cref= java.beans.beancontext.BeanContextChild </seealso>

	<Serializable> _
	Public Class BeanContextChildSupport
		Implements BeanContextChild, BeanContextServicesListener

		Friend Const serialVersionUID As Long = 6328947014421475877L

		''' <summary>
		''' construct a BeanContextChildSupport where this class has been
		''' subclassed in order to implement the JavaBean component itself.
		''' </summary>

		Public Sub New()
			MyBase.New()

			beanContextChildPeer = Me

			pcSupport = New java.beans.PropertyChangeSupport(beanContextChildPeer)
			vcSupport = New java.beans.VetoableChangeSupport(beanContextChildPeer)
		End Sub

		''' <summary>
		''' construct a BeanContextChildSupport where the JavaBean component
		''' itself implements BeanContextChild, and encapsulates this, delegating
		''' that interface to this implementation </summary>
		''' <param name="bcc"> the underlying bean context child </param>

		Public Sub New(  bcc As BeanContextChild)
			MyBase.New()

			beanContextChildPeer = If(bcc IsNot Nothing, bcc, Me)

			pcSupport = New java.beans.PropertyChangeSupport(beanContextChildPeer)
			vcSupport = New java.beans.VetoableChangeSupport(beanContextChildPeer)
		End Sub

		''' <summary>
		''' Sets the <code>BeanContext</code> for
		''' this <code>BeanContextChildSupport</code>. </summary>
		''' <param name="bc"> the new value to be assigned to the <code>BeanContext</code>
		''' property </param>
		''' <exception cref="PropertyVetoException"> if the change is rejected </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property beanContext As BeanContext
			Set(  bc As BeanContext)
				If bc Is beanContext Then Return
    
				Dim oldValue As BeanContext = beanContext
				Dim newValue As BeanContext = bc
    
				If Not rejectedSetBCOnce Then
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					If rejectedSetBCOnce = (Not validatePendingSetBeanContext(bc)) Then
						Throw New java.beans.PropertyVetoException("setBeanContext() change rejected:", New java.beans.PropertyChangeEvent(beanContextChildPeer, "beanContext", oldValue, newValue)
					   )
					End If
    
					Try
						fireVetoableChange("beanContext", oldValue, newValue)
					Catch pve As java.beans.PropertyVetoException
						rejectedSetBCOnce = True
    
						Throw pve ' re-throw
					End Try
				End If
    
				If beanContext IsNot Nothing Then releaseBeanContextResources()
    
				beanContext = newValue
				rejectedSetBCOnce = False
    
				firePropertyChange("beanContext", oldValue, newValue)
    
				If beanContext IsNot Nothing Then initializeBeanContextResources()
			End Set
			Get
				Return beanContext
			End Get
		End Property


		''' <summary>
		''' Add a PropertyChangeListener for a specific property.
		''' The same listener object may be added more than once.  For each
		''' property,  the listener will be invoked the number of times it was added
		''' for that property.
		''' If <code>name</code> or <code>pcl</code> is null, no exception is thrown
		''' and no action is taken.
		''' </summary>
		''' <param name="name"> The name of the property to listen on </param>
		''' <param name="pcl"> The <code>PropertyChangeListener</code> to be added </param>
		Public Overridable Sub addPropertyChangeListener(  name As String,   pcl As java.beans.PropertyChangeListener) Implements BeanContextChild.addPropertyChangeListener
			pcSupport.addPropertyChangeListener(name, pcl)
		End Sub

		''' <summary>
		''' Remove a PropertyChangeListener for a specific property.
		''' If <code>pcl</code> was added more than once to the same event
		''' source for the specified property, it will be notified one less time
		''' after being removed.
		''' If <code>name</code> is null, no exception is thrown
		''' and no action is taken.
		''' If <code>pcl</code> is null, or was never added for the specified
		''' property, no exception is thrown and no action is taken.
		''' </summary>
		''' <param name="name"> The name of the property that was listened on </param>
		''' <param name="pcl"> The PropertyChangeListener to be removed </param>
		Public Overridable Sub removePropertyChangeListener(  name As String,   pcl As java.beans.PropertyChangeListener) Implements BeanContextChild.removePropertyChangeListener
			pcSupport.removePropertyChangeListener(name, pcl)
		End Sub

		''' <summary>
		''' Add a VetoableChangeListener for a specific property.
		''' The same listener object may be added more than once.  For each
		''' property,  the listener will be invoked the number of times it was added
		''' for that property.
		''' If <code>name</code> or <code>vcl</code> is null, no exception is thrown
		''' and no action is taken.
		''' </summary>
		''' <param name="name"> The name of the property to listen on </param>
		''' <param name="vcl"> The <code>VetoableChangeListener</code> to be added </param>
		Public Overridable Sub addVetoableChangeListener(  name As String,   vcl As java.beans.VetoableChangeListener) Implements BeanContextChild.addVetoableChangeListener
			vcSupport.addVetoableChangeListener(name, vcl)
		End Sub

		''' <summary>
		''' Removes a <code>VetoableChangeListener</code>.
		''' If <code>pcl</code> was added more than once to the same event
		''' source for the specified property, it will be notified one less time
		''' after being removed.
		''' If <code>name</code> is null, no exception is thrown
		''' and no action is taken.
		''' If <code>vcl</code> is null, or was never added for the specified
		''' property, no exception is thrown and no action is taken.
		''' </summary>
		''' <param name="name"> The name of the property that was listened on </param>
		''' <param name="vcl"> The <code>VetoableChangeListener</code> to be removed </param>
		Public Overridable Sub removeVetoableChangeListener(  name As String,   vcl As java.beans.VetoableChangeListener) Implements BeanContextChild.removeVetoableChangeListener
			vcSupport.removeVetoableChangeListener(name, vcl)
		End Sub

		''' <summary>
		''' A service provided by the nesting BeanContext has been revoked.
		''' 
		''' Subclasses may override this method in order to implement their own
		''' behaviors. </summary>
		''' <param name="bcsre"> The <code>BeanContextServiceRevokedEvent</code> fired as a
		''' result of a service being revoked </param>
		Public Overridable Sub serviceRevoked(  bcsre As BeanContextServiceRevokedEvent)
		End Sub

		''' <summary>
		''' A new service is available from the nesting BeanContext.
		''' 
		''' Subclasses may override this method in order to implement their own
		''' behaviors </summary>
		''' <param name="bcsae"> The BeanContextServiceAvailableEvent fired as a
		''' result of a service becoming available
		'''  </param>
		Public Overridable Sub serviceAvailable(  bcsae As BeanContextServiceAvailableEvent)
		End Sub

		''' <summary>
		''' Gets the <tt>BeanContextChild</tt> associated with this
		''' <tt>BeanContextChildSupport</tt>.
		''' </summary>
		''' <returns> the <tt>BeanContextChild</tt> peer of this class </returns>
		Public Overridable Property beanContextChildPeer As BeanContextChild
			Get
				Return beanContextChildPeer
			End Get
		End Property

		''' <summary>
		''' Reports whether or not this class is a delegate of another.
		''' </summary>
		''' <returns> true if this class is a delegate of another </returns>
		Public Overridable Property delegated As Boolean
			Get
				Return Not Me.Equals(beanContextChildPeer)
			End Get
		End Property

		''' <summary>
		''' Report a bound property update to any registered listeners. No event is
		''' fired if old and new are equal and non-null. </summary>
		''' <param name="name"> The programmatic name of the property that was changed </param>
		''' <param name="oldValue">  The old value of the property </param>
		''' <param name="newValue">  The new value of the property </param>
		Public Overridable Sub firePropertyChange(  name As String,   oldValue As Object,   newValue As Object)
			pcSupport.firePropertyChange(name, oldValue, newValue)
		End Sub

		''' <summary>
		''' Report a vetoable property update to any registered listeners.
		''' If anyone vetos the change, then fire a new event
		''' reverting everyone to the old value and then rethrow
		''' the PropertyVetoException. <P>
		''' 
		''' No event is fired if old and new are equal and non-null.
		''' <P> </summary>
		''' <param name="name"> The programmatic name of the property that is about to
		''' change
		''' </param>
		''' <param name="oldValue"> The old value of the property </param>
		''' <param name="newValue"> - The new value of the property
		''' </param>
		''' <exception cref="PropertyVetoException"> if the recipient wishes the property
		''' change to be rolled back. </exception>
		Public Overridable Sub fireVetoableChange(  name As String,   oldValue As Object,   newValue As Object)
			vcSupport.fireVetoableChange(name, oldValue, newValue)
		End Sub

		''' <summary>
		''' Called from setBeanContext to validate (or otherwise) the
		''' pending change in the nesting BeanContext property value.
		''' Returning false will cause setBeanContext to throw
		''' PropertyVetoException. </summary>
		''' <param name="newValue"> the new value that has been requested for
		'''  the BeanContext property </param>
		''' <returns> <code>true</code> if the change operation is to be vetoed </returns>
		Public Overridable Function validatePendingSetBeanContext(  newValue As BeanContext) As Boolean
			Return True
		End Function

		''' <summary>
		''' This method may be overridden by subclasses to provide their own
		''' release behaviors. When invoked any resources held by this instance
		''' obtained from its current BeanContext property should be released
		''' since the object is no longer nested within that BeanContext.
		''' </summary>

		Protected Friend Overridable Sub releaseBeanContextResources()
			' do nothing
		End Sub

		''' <summary>
		''' This method may be overridden by subclasses to provide their own
		''' initialization behaviors. When invoked any resources required by the
		''' BeanContextChild should be obtained from the current BeanContext.
		''' </summary>

		Protected Friend Overridable Sub initializeBeanContextResources()
			' do nothing
		End Sub

		''' <summary>
		''' Write the persistence state of the object.
		''' </summary>

		Private Sub writeObject(  oos As java.io.ObjectOutputStream)

	'        
	'         * don't serialize if we are delegated and the delegator is not also
	'         * serializable.
	'         

			If (Not Equals(beanContextChildPeer)) AndAlso Not(TypeOf beanContextChildPeer Is java.io.Serializable) Then
				Throw New java.io.IOException("BeanContextChildSupport beanContextChildPeer not Serializable")

			Else
				oos.defaultWriteObject()
			End If

		End Sub


		''' <summary>
		''' Restore a persistent object, must wait for subsequent setBeanContext()
		''' to fully restore any resources obtained from the new nesting
		''' BeanContext
		''' </summary>

		Private Sub readObject(  ois As java.io.ObjectInputStream)
			ois.defaultReadObject()
		End Sub

	'    
	'     * fields
	'     

		''' <summary>
		''' The <code>BeanContext</code> in which
		''' this <code>BeanContextChild</code> is nested.
		''' </summary>
		Public beanContextChildPeer As BeanContextChild

	   ''' <summary>
	   ''' The <tt>PropertyChangeSupport</tt> associated with this
	   ''' <tt>BeanContextChildSupport</tt>.
	   ''' </summary>
		Protected Friend pcSupport As java.beans.PropertyChangeSupport

	   ''' <summary>
	   ''' The <tt>VetoableChangeSupport</tt> associated with this
	   ''' <tt>BeanContextChildSupport</tt>.
	   ''' </summary>
		Protected Friend vcSupport As java.beans.VetoableChangeSupport

		''' <summary>
		''' The bean context.
		''' </summary>
		<NonSerialized> _
		Protected Friend beanContext As BeanContext

	   ''' <summary>
	   ''' A flag indicating that there has been
	   ''' at least one <code>PropertyChangeVetoException</code>
	   ''' thrown for the attempted setBeanContext operation.
	   ''' </summary>
		<NonSerialized> _
		Protected Friend rejectedSetBCOnce As Boolean

	End Class

End Namespace
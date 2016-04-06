Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This helper class provides a utility implementation of the
	''' java.beans.beancontext.BeanContext interface.
	''' <p>
	''' Since this class directly implements the BeanContext interface, the class
	''' can, and is intended to be used either by subclassing this implementation,
	''' or via ad-hoc delegation of an instance of this class from another.
	''' </p>
	''' 
	''' @author Laurence P. G. Cable
	''' @since 1.2
	''' </summary>
	<Serializable> _
	Public Class BeanContextSupport
		Inherits BeanContextChildSupport
		Implements BeanContext, java.beans.PropertyChangeListener, java.beans.VetoableChangeListener

		' Fix for bug 4282900 to pass JCK regression test
		Friend Shadows Const serialVersionUID As Long = -4879613978649577204L

		''' 
		''' <summary>
		''' Construct a BeanContextSupport instance
		''' 
		''' </summary>
		''' <param name="peer">      The peer <tt>BeanContext</tt> we are
		'''                  supplying an implementation for,
		'''                  or <tt>null</tt>
		'''                  if this object is its own peer </param>
		''' <param name="lcle">      The current Locale for this BeanContext. If
		'''                  <tt>lcle</tt> is <tt>null</tt>, the default locale
		'''                  is assigned to the <tt>BeanContext</tt> instance. </param>
		''' <param name="dTime">     The initial state,
		'''                  <tt>true</tt> if in design mode,
		'''                  <tt>false</tt> if runtime. </param>
		''' <param name="visible">   The initial visibility. </param>
		''' <seealso cref= java.util.Locale#getDefault() </seealso>
		''' <seealso cref= java.util.Locale#setDefault(java.util.Locale) </seealso>
		Public Sub New(  peer As BeanContext,   lcle As java.util.Locale,   dTime As Boolean,   visible As Boolean)
			MyBase.New(peer)

			locale = If(lcle IsNot Nothing, lcle, java.util.Locale.default)
			designTime = dTime
			okToUseGui_Renamed = visible

			initialize()
		End Sub

		''' <summary>
		''' Create an instance using the specified Locale and design mode.
		''' </summary>
		''' <param name="peer">      The peer <tt>BeanContext</tt> we
		'''                  are supplying an implementation for,
		'''                  or <tt>null</tt> if this object is its own peer </param>
		''' <param name="lcle">      The current Locale for this <tt>BeanContext</tt>. If
		'''                  <tt>lcle</tt> is <tt>null</tt>, the default locale
		'''                  is assigned to the <tt>BeanContext</tt> instance. </param>
		''' <param name="dtime">     The initial state, <tt>true</tt>
		'''                  if in design mode,
		'''                  <tt>false</tt> if runtime. </param>
		''' <seealso cref= java.util.Locale#getDefault() </seealso>
		''' <seealso cref= java.util.Locale#setDefault(java.util.Locale) </seealso>
		Public Sub New(  peer As BeanContext,   lcle As java.util.Locale,   dtime As Boolean)
			Me.New(peer, lcle, dtime, True)
		End Sub

		''' <summary>
		''' Create an instance using the specified locale
		''' </summary>
		''' <param name="peer">      The peer BeanContext we are
		'''                  supplying an implementation for,
		'''                  or <tt>null</tt> if this object
		'''                  is its own peer </param>
		''' <param name="lcle">      The current Locale for this
		'''                  <tt>BeanContext</tt>. If
		'''                  <tt>lcle</tt> is <tt>null</tt>,
		'''                  the default locale
		'''                  is assigned to the <tt>BeanContext</tt>
		'''                  instance. </param>
		''' <seealso cref= java.util.Locale#getDefault() </seealso>
		''' <seealso cref= java.util.Locale#setDefault(java.util.Locale) </seealso>
		Public Sub New(  peer As BeanContext,   lcle As java.util.Locale)
			Me.New(peer, lcle, False, True)
		End Sub

		''' <summary>
		''' Create an instance using with a default locale
		''' </summary>
		''' <param name="peer">      The peer <tt>BeanContext</tt> we are
		'''                  supplying an implementation for,
		'''                  or <tt>null</tt> if this object
		'''                  is its own peer </param>
		Public Sub New(  peer As BeanContext)
			Me.New(peer, Nothing, False, True)
		End Sub

		''' <summary>
		''' Create an instance that is not a delegate of another object
		''' </summary>

		Public Sub New()
			Me.New(Nothing, Nothing, False, True)
		End Sub

		''' <summary>
		''' Gets the instance of <tt>BeanContext</tt> that
		''' this object is providing the implementation for. </summary>
		''' <returns> the BeanContext instance </returns>
		Public Overridable Property beanContextPeer As BeanContext
			Get
				Return CType(beanContextChildPeer, BeanContext)
			End Get
		End Property

		''' <summary>
		''' <p>
		''' The instantiateChild method is a convenience hook
		''' in BeanContext to simplify
		''' the task of instantiating a Bean, nested,
		''' into a <tt>BeanContext</tt>.
		''' </p>
		''' <p>
		''' The semantics of the beanName parameter are defined by java.beans.Beans.instantiate.
		''' </p>
		''' </summary>
		''' <param name="beanName"> the name of the Bean to instantiate within this BeanContext </param>
		''' <exception cref="IOException"> if there is an I/O error when the bean is being deserialized </exception>
		''' <exception cref="ClassNotFoundException"> if the class
		''' identified by the beanName parameter is not found </exception>
		''' <returns> the new object </returns>
		Public Overridable Function instantiateChild(  beanName As String) As Object Implements BeanContext.instantiateChild
			Dim bc As BeanContext = beanContextPeer

			Return java.beans.Beans.instantiate(bc.GetType().classLoader, beanName, bc)
		End Function

		''' <summary>
		''' Gets the number of children currently nested in
		''' this BeanContext.
		''' </summary>
		''' <returns> number of children </returns>
		Public Overridable Function size() As Integer
			SyncLock children
				Return children.Count
			End SyncLock
		End Function

		''' <summary>
		''' Reports whether or not this
		''' <tt>BeanContext</tt> is empty.
		''' A <tt>BeanContext</tt> is considered
		''' empty when it contains zero
		''' nested children. </summary>
		''' <returns> if there are not children </returns>
		Public Overridable Property empty As Boolean
			Get
				SyncLock children
					Return children.Count = 0
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Determines whether or not the specified object
		''' is currently a child of this <tt>BeanContext</tt>. </summary>
		''' <param name="o"> the Object in question </param>
		''' <returns> if this object is a child </returns>
		Public Overridable Function contains(  o As Object) As Boolean
			SyncLock children
				Return children.ContainsKey(o)
			End SyncLock
		End Function

		''' <summary>
		''' Determines whether or not the specified object
		''' is currently a child of this <tt>BeanContext</tt>. </summary>
		''' <param name="o"> the Object in question </param>
		''' <returns> if this object is a child </returns>
		Public Overridable Function containsKey(  o As Object) As Boolean
			SyncLock children
				Return children.ContainsKey(o)
			End SyncLock
		End Function

		''' <summary>
		''' Gets all JavaBean or <tt>BeanContext</tt> instances
		''' currently nested in this <tt>BeanContext</tt>. </summary>
		''' <returns> an <tt>Iterator</tt> of the nested children </returns>
		Public Overridable Function [iterator]() As IEnumerator
			SyncLock children
				Return New BCSIterator(children.Keys.GetEnumerator())
			End SyncLock
		End Function

		''' <summary>
		''' Gets all JavaBean or <tt>BeanContext</tt>
		''' instances currently nested in this BeanContext.
		''' </summary>
		Public Overridable Function toArray() As Object()
			SyncLock children
				Return children.Keys.ToArray()
			End SyncLock
		End Function

		''' <summary>
		''' Gets an array containing all children of
		''' this <tt>BeanContext</tt> that match
		''' the types contained in arry. </summary>
		''' <param name="arry"> The array of object
		''' types that are of interest. </param>
		''' <returns> an array of children </returns>
		Public Overridable Function toArray(  arry As Object()) As Object()
			SyncLock children
				Return children.Keys.ToArray(arry)
			End SyncLock
		End Function


		''' <summary>
		'''********************************************************************* </summary>

		''' <summary>
		''' protected final subclass that encapsulates an iterator but implements
		''' a noop remove() method.
		''' </summary>

		Protected Friend NotInheritable Class BCSIterator
			Implements IEnumerator

			Friend Sub New(  i As IEnumerator)
				MyBase.New()
				src = i
			End Sub

			Public Function hasNext() As Boolean
				Return src.hasNext()
			End Function
			Public Function [next]() As Object
				Return src.next()
			End Function
			Public Sub remove() ' do nothing
			End Sub

			Private src As IEnumerator
		End Class

		''' <summary>
		'''********************************************************************* </summary>

	'    
	'     * protected nested class containing per child information, an instance
	'     * of which is associated with each child in the "children" hashtable.
	'     * subclasses can extend this class to include their own per-child state.
	'     *
	'     * Note that this 'value' is serialized with the corresponding child 'key'
	'     * when the BeanContextSupport is serialized.
	'     

		<Serializable> _
		Protected Friend Class BCSChild
			Private ReadOnly outerInstance As BeanContextSupport


		Private Const serialVersionUID As Long = -5815286101609939109L

			Friend Sub New(  outerInstance As BeanContextSupport,   bcc As Object,   peer As Object)
					Me.outerInstance = outerInstance
				MyBase.New()

				child = bcc
				proxyPeer = peer
			End Sub

			Friend Overridable Property child As Object
				Get
					Return child
				End Get
			End Property

			Friend Overridable Property removePending As Boolean
				Set(  v As Boolean)
					removePending = v
				End Set
				Get
					Return removePending
				End Get
			End Property


			Friend Overridable Property proxyPeer As Boolean
				Get
					Return proxyPeer IsNot Nothing
				End Get
			End Property

			Friend Overridable Property proxyPeer As Object
				Get
					Return proxyPeer
				End Get
			End Property
	'        
	'         * fields
	'         


			Private child As Object
			Private proxyPeer As Object

			<NonSerialized> _
			Private removePending As Boolean
		End Class

		''' <summary>
		''' <p>
		''' Subclasses can override this method to insert their own subclass
		''' of Child without having to override add() or the other Collection
		''' methods that add children to the set.
		''' </p> </summary>
		''' <param name="targetChild"> the child to create the Child on behalf of </param> </param>
		''' <param name="peer">        the peer if the tragetChild and the peer are related by an implementation of BeanContextProxy     * <returns> Subtype-specific subclass of Child without overriding collection methods </returns>

		Protected Friend Overridable Function createBCSChild(  targetChild As Object,   peer As Object) As BCSChild
			Return New BCSChild(Me, targetChild, peer)
		End Function

		''' <summary>
		'''********************************************************************* </summary>

		''' <summary>
		''' Adds/nests a child within this <tt>BeanContext</tt>.
		''' <p>
		''' Invoked as a side effect of java.beans.Beans.instantiate().
		''' If the child object is not valid for adding then this method
		''' throws an IllegalStateException.
		''' </p>
		''' 
		''' </summary>
		''' <param name="targetChild"> The child objects to nest
		''' within this <tt>BeanContext</tt> </param>
		''' <returns> true if the child was added successfully. </returns>
		''' <seealso cref= #validatePendingAdd </seealso>
		Public Overridable Function add(  targetChild As Object) As Boolean

			If targetChild Is Nothing Then Throw New IllegalArgumentException

			' The specification requires that we do nothing if the child
			' is already nested herein.

			If children.ContainsKey(targetChild) Then ' test before locking Return False

			SyncLock BeanContext.globalHierarchyLock
				If children.ContainsKey(targetChild) Then ' check again Return False

				If Not validatePendingAdd(targetChild) Then Throw New IllegalStateException


				' The specification requires that we invoke setBeanContext() on the
				' newly added child if it implements the java.beans.beancontext.BeanContextChild interface

				Dim cbcc As BeanContextChild = getChildBeanContextChild(targetChild)
				Dim bccp As BeanContextChild = Nothing

				SyncLock targetChild

					If TypeOf targetChild Is BeanContextProxy Then
						bccp = CType(targetChild, BeanContextProxy).beanContextProxy

						If bccp Is Nothing Then Throw New NullPointerException("BeanContextPeer.getBeanContextProxy()")
					End If

					Dim bcsc As BCSChild = createBCSChild(targetChild, bccp)
					Dim pbcsc As BCSChild = Nothing

					SyncLock children
						children(targetChild) = bcsc

						If bccp IsNot Nothing Then
								pbcsc = createBCSChild(bccp, targetChild)
								children(bccp) = pbcsc
						End If
					End SyncLock

					If cbcc IsNot Nothing Then
						SyncLock cbcc
						Try
							cbcc.beanContext = beanContextPeer
						Catch pve As java.beans.PropertyVetoException

							SyncLock children
								children.Remove(targetChild)

								If bccp IsNot Nothing Then children.Remove(bccp)
							End SyncLock

							Throw New IllegalStateException
						End Try

						cbcc.addPropertyChangeListener("beanContext", childPCL)
						cbcc.addVetoableChangeListener("beanContext", childVCL)
						End SyncLock
					End If

					Dim v As java.beans.Visibility = getChildVisibility(targetChild)

					If v IsNot Nothing Then
						If okToUseGui_Renamed Then
							v.okToUseGui()
						Else
							v.dontUseGui()
						End If
					End If

					If getChildSerializable(targetChild) IsNot Nothing Then serializable += 1

					childJustAddedHook(targetChild, bcsc)

					If bccp IsNot Nothing Then
						v = getChildVisibility(bccp)

						If v IsNot Nothing Then
							If okToUseGui_Renamed Then
								v.okToUseGui()
							Else
								v.dontUseGui()
							End If
						End If

						If getChildSerializable(bccp) IsNot Nothing Then serializable += 1

						childJustAddedHook(bccp, pbcsc)
					End If


				End SyncLock

				' The specification requires that we fire a notification of the change

				fireChildrenAdded(New BeanContextMembershipEvent(beanContextPeer,If(bccp Is Nothing, New Object() { targetChild }, New Object){ targetChild, bccp }))

			End SyncLock

			Return True
		End Function

		''' <summary>
		''' Removes a child from this BeanContext.  If the child object is not
		''' for adding then this method throws an IllegalStateException. </summary>
		''' <param name="targetChild"> The child objects to remove </param>
		''' <seealso cref= #validatePendingRemove </seealso>
		Public Overridable Function remove(  targetChild As Object) As Boolean
			Return remove(targetChild, True)
		End Function

		''' <summary>
		''' internal remove used when removal caused by
		''' unexpected <tt>setBeanContext</tt> or
		''' by <tt>remove()</tt> invocation. </summary>
		''' <param name="targetChild"> the JavaBean, BeanContext, or Object to be removed </param>
		''' <param name="callChildSetBC"> used to indicate that
		''' the child should be notified that it is no
		''' longer nested in this <tt>BeanContext</tt>. </param>
		''' <returns> whether or not was present before being removed </returns>
		Protected Friend Overridable Function remove(  targetChild As Object,   callChildSetBC As Boolean) As Boolean

			If targetChild Is Nothing Then Throw New IllegalArgumentException

			SyncLock BeanContext.globalHierarchyLock
				If Not containsKey(targetChild) Then Return False

				If Not validatePendingRemove(targetChild) Then Throw New IllegalStateException

				Dim bcsc As BCSChild = CType(children(targetChild), BCSChild)
				Dim pbcsc As BCSChild = Nothing
				Dim peer As Object = Nothing

				' we are required to notify the child that it is no longer nested here if
				' it implements java.beans.beancontext.BeanContextChild

				SyncLock targetChild
					If callChildSetBC Then
						Dim cbcc As BeanContextChild = getChildBeanContextChild(targetChild)
						If cbcc IsNot Nothing Then
							SyncLock cbcc
							cbcc.removePropertyChangeListener("beanContext", childPCL)
							cbcc.removeVetoableChangeListener("beanContext", childVCL)

							Try
								cbcc.beanContext = Nothing
							Catch pve1 As java.beans.PropertyVetoException
								cbcc.addPropertyChangeListener("beanContext", childPCL)
								cbcc.addVetoableChangeListener("beanContext", childVCL)
								Throw New IllegalStateException
							End Try

							End SyncLock
						End If
					End If

					SyncLock children
						children.Remove(targetChild)

						If bcsc.proxyPeer Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							pbcsc = CType(children(peer = bcsc.proxyPeer), BCSChild)
							children.Remove(peer)
						End If
					End SyncLock

					If getChildSerializable(targetChild) IsNot Nothing Then serializable -= 1

					childJustRemovedHook(targetChild, bcsc)

					If peer IsNot Nothing Then
						If getChildSerializable(peer) IsNot Nothing Then serializable -= 1

						childJustRemovedHook(peer, pbcsc)
					End If
				End SyncLock

				fireChildrenRemoved(New BeanContextMembershipEvent(beanContextPeer,If(peer Is Nothing, New Object() { targetChild }, New Object){ targetChild, peer }))

			End SyncLock

			Return True
		End Function

		''' <summary>
		''' Tests to see if all objects in the
		''' specified <tt>Collection</tt> are children of
		''' this <tt>BeanContext</tt>. </summary>
		''' <param name="c"> the specified <tt>Collection</tt>
		''' </param>
		''' <returns> <tt>true</tt> if all objects
		''' in the collection are children of
		''' this <tt>BeanContext</tt>, false if not. </returns>
		Public Overridable Function containsAll(  c As ICollection) As Boolean
			SyncLock children
				Dim i As IEnumerator = c.GetEnumerator()
				Do While i.hasNext()
					If Not contains(i.next()) Then Return False
				Loop

				Return True
			End SyncLock
		End Function

		''' <summary>
		''' add Collection to set of Children (Unsupported)
		''' implementations must synchronized on the hierarchy lock and "children" protected field </summary>
		''' <exception cref="UnsupportedOperationException"> thrown unconditionally by this implementation </exception>
		''' <returns> this implementation unconditionally throws {@code UnsupportedOperationException} </returns>
		Public Overridable Function addAll(  c As ICollection) As Boolean
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' remove all specified children (Unsupported)
		''' implementations must synchronized on the hierarchy lock and "children" protected field </summary>
		''' <exception cref="UnsupportedOperationException"> thrown unconditionally by this implementation </exception>
		''' <returns> this implementation unconditionally throws {@code UnsupportedOperationException}
		'''  </returns>
		Public Overridable Function removeAll(  c As ICollection) As Boolean
			Throw New UnsupportedOperationException
		End Function


		''' <summary>
		''' retain only specified children (Unsupported)
		''' implementations must synchronized on the hierarchy lock and "children" protected field </summary>
		''' <exception cref="UnsupportedOperationException"> thrown unconditionally by this implementation </exception>
		''' <returns> this implementation unconditionally throws {@code UnsupportedOperationException} </returns>
		Public Overridable Function retainAll(  c As ICollection) As Boolean
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' clear the children (Unsupported)
		''' implementations must synchronized on the hierarchy lock and "children" protected field </summary>
		''' <exception cref="UnsupportedOperationException"> thrown unconditionally by this implementation </exception>
		Public Overridable Sub clear()
			Throw New UnsupportedOperationException
		End Sub

		''' <summary>
		''' Adds a BeanContextMembershipListener
		''' </summary>
		''' <param name="bcml"> the BeanContextMembershipListener to add </param>
		''' <exception cref="NullPointerException"> if the argument is null </exception>

		Public Overridable Sub addBeanContextMembershipListener(  bcml As BeanContextMembershipListener) Implements BeanContext.addBeanContextMembershipListener
			If bcml Is Nothing Then Throw New NullPointerException("listener")

			SyncLock bcmListeners
				If bcmListeners.Contains(bcml) Then
					Return
				Else
					bcmListeners.Add(bcml)
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Removes a BeanContextMembershipListener
		''' </summary>
		''' <param name="bcml"> the BeanContextMembershipListener to remove </param>
		''' <exception cref="NullPointerException"> if the argument is null </exception>

		Public Overridable Sub removeBeanContextMembershipListener(  bcml As BeanContextMembershipListener) Implements BeanContext.removeBeanContextMembershipListener
			If bcml Is Nothing Then Throw New NullPointerException("listener")

			SyncLock bcmListeners
				If Not bcmListeners.Contains(bcml) Then
					Return
				Else
					bcmListeners.Remove(bcml)
				End If
			End SyncLock
		End Sub

		''' <param name="name"> the name of the resource requested. </param>
		''' <param name="bcc">  the child object making the request.
		''' </param>
		''' <returns>  the requested resource as an InputStream </returns>
		''' <exception cref="NullPointerException"> if the argument is null </exception>

		Public Overridable Function getResourceAsStream(  name As String,   bcc As BeanContextChild) As java.io.InputStream Implements BeanContext.getResourceAsStream
			If name Is Nothing Then Throw New NullPointerException("name")
			If bcc Is Nothing Then Throw New NullPointerException("bcc")

			If containsKey(bcc) Then
				Dim cl As  ClassLoader = bcc.GetType().classLoader

				Return If(cl IsNot Nothing, cl.getResourceAsStream(name), ClassLoader.getSystemResourceAsStream(name))
			Else
				Throw New IllegalArgumentException("Not a valid child")
			End If
		End Function

		''' <param name="name"> the name of the resource requested. </param>
		''' <param name="bcc">  the child object making the request.
		''' </param>
		''' <returns> the requested resource as an InputStream </returns>

		Public Overridable Function getResource(  name As String,   bcc As BeanContextChild) As java.net.URL Implements BeanContext.getResource
			If name Is Nothing Then Throw New NullPointerException("name")
			If bcc Is Nothing Then Throw New NullPointerException("bcc")

			If containsKey(bcc) Then
				Dim cl As  ClassLoader = bcc.GetType().classLoader

				Return If(cl IsNot Nothing, cl.getResource(name), ClassLoader.getSystemResource(name))
			Else
				Throw New IllegalArgumentException("Not a valid child")
			End If
		End Function

		''' <summary>
		''' Sets the new design time value for this <tt>BeanContext</tt>. </summary>
		''' <param name="dTime"> the new designTime value </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property designTime Implements DesignMode.setDesignTime As Boolean
			Set(  dTime As Boolean)
				If designTime <> dTime Then
					designTime = dTime
    
					firePropertyChange("designMode", Convert.ToBoolean((Not dTime)), Convert.ToBoolean(dTime))
				End If
			End Set
			Get
				Return designTime
			End Get
		End Property



		''' <summary>
		''' Sets the locale of this BeanContext. </summary>
		''' <param name="newLocale"> the new locale. This method call will have
		'''        no effect if newLocale is <CODE>null</CODE>. </param>
		''' <exception cref="PropertyVetoException"> if the new value is rejected </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property locale As java.util.Locale
			Set(  newLocale As java.util.Locale)
    
				If (locale IsNot Nothing AndAlso (Not locale.Equals(newLocale))) AndAlso newLocale IsNot Nothing Then
					Dim old As java.util.Locale = locale
    
					fireVetoableChange("locale", old, newLocale) ' throws
    
					locale = newLocale
    
					firePropertyChange("locale", old, newLocale)
				End If
			End Set
			Get
				Return locale
			End Get
		End Property


		''' <summary>
		''' <p>
		''' This method is typically called from the environment in order to determine
		''' if the implementor "needs" a GUI.
		''' </p>
		''' <p>
		''' The algorithm used herein tests the BeanContextPeer, and its current children
		''' to determine if they are either Containers, Components, or if they implement
		''' Visibility and return needsGui() == true.
		''' </p> </summary>
		''' <returns> <tt>true</tt> if the implementor needs a GUI </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function needsGui() As Boolean Implements Visibility.needsGui
			Dim bc As BeanContext = beanContextPeer

			If bc IsNot Me Then
				If TypeOf bc Is java.beans.Visibility Then Return CType(bc, java.beans.Visibility).needsGui()

				If TypeOf bc Is java.awt.Container OrElse TypeOf bc Is java.awt.Component Then Return True
			End If

			SyncLock children
				Dim i As IEnumerator = children.Keys.GetEnumerator()
				Do While i.hasNext()
					Dim c As Object = i.next()

					Try
							Return CType(c, java.beans.Visibility).needsGui()
						Catch cce As  ClassCastException
							' do nothing ...
						End Try

						If TypeOf c Is java.awt.Container OrElse TypeOf c Is java.awt.Component Then Return True
				Loop
			End SyncLock

			Return False
		End Function

		''' <summary>
		''' notify this instance that it may no longer render a GUI.
		''' </summary>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub dontUseGui() Implements Visibility.dontUseGui
			If okToUseGui_Renamed Then
				okToUseGui_Renamed = False

				' lets also tell the Children that can that they may not use their GUI's
				SyncLock children
					Dim i As IEnumerator = children.Keys.GetEnumerator()
					Do While i.hasNext()
						Dim v As java.beans.Visibility = getChildVisibility(i.next())

						If v IsNot Nothing Then v.dontUseGui()
					Loop
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Notify this instance that it may now render a GUI
		''' </summary>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub okToUseGui() Implements Visibility.okToUseGui
			If Not okToUseGui_Renamed Then
				okToUseGui_Renamed = True

				' lets also tell the Children that can that they may use their GUI's
				SyncLock children
					Dim i As IEnumerator = children.Keys.GetEnumerator()
					Do While i.hasNext()
						Dim v As java.beans.Visibility = getChildVisibility(i.next())

						If v IsNot Nothing Then v.okToUseGui()
					Loop
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Used to determine if the <tt>BeanContext</tt>
		''' child is avoiding using its GUI. </summary>
		''' <returns> is this instance avoiding using its GUI? </returns>
		''' <seealso cref= Visibility </seealso>
		Public Overridable Function avoidingGui() As Boolean Implements Visibility.avoidingGui
			Return (Not okToUseGui_Renamed) AndAlso needsGui()
		End Function

		''' <summary>
		''' Is this <tt>BeanContext</tt> in the
		''' process of being serialized? </summary>
		''' <returns> if this <tt>BeanContext</tt> is
		''' currently being serialized </returns>
		Public Overridable Property serializing As Boolean
			Get
				Return serializing
			End Get
		End Property

		''' <summary>
		''' Returns an iterator of all children
		''' of this <tt>BeanContext</tt>. </summary>
		''' <returns> an iterator for all the current BCSChild values </returns>
		Protected Friend Overridable Function bcsChildren() As IEnumerator
			SyncLock children
				Return children.Values.GetEnumerator()
			End SyncLock
		End Function

		''' <summary>
		''' called by writeObject after defaultWriteObject() but prior to
		''' serialization of currently serializable children.
		''' 
		''' This method may be overridden by subclasses to perform custom
		''' serialization of their state prior to this superclass serializing
		''' the children.
		''' 
		''' This method should not however be used by subclasses to replace their
		''' own implementation (if any) of writeObject(). </summary>
		''' <param name="oos"> the {@code ObjectOutputStream} to use during serialization </param>
		''' <exception cref="IOException"> if serialization failed </exception>

		Protected Friend Overridable Sub bcsPreSerializationHook(  oos As java.io.ObjectOutputStream)
		End Sub

		''' <summary>
		''' called by readObject after defaultReadObject() but prior to
		''' deserialization of any children.
		''' 
		''' This method may be overridden by subclasses to perform custom
		''' deserialization of their state prior to this superclass deserializing
		''' the children.
		''' 
		''' This method should not however be used by subclasses to replace their
		''' own implementation (if any) of readObject(). </summary>
		''' <param name="ois"> the {@code ObjectInputStream} to use during deserialization </param>
		''' <exception cref="IOException"> if deserialization failed </exception>
		''' <exception cref="ClassNotFoundException"> if needed classes are not found </exception>

		Protected Friend Overridable Sub bcsPreDeserializationHook(  ois As java.io.ObjectInputStream)
		End Sub

		''' <summary>
		''' Called by readObject with the newly deserialized child and BCSChild. </summary>
		''' <param name="child"> the newly deserialized child </param>
		''' <param name="bcsc"> the newly deserialized BCSChild </param>
		Protected Friend Overridable Sub childDeserializedHook(  child As Object,   bcsc As BCSChild)
			SyncLock children
				children(child) = bcsc
			End SyncLock
		End Sub

		''' <summary>
		''' Used by writeObject to serialize a Collection. </summary>
		''' <param name="oos"> the <tt>ObjectOutputStream</tt>
		''' to use during serialization </param>
		''' <param name="coll"> the <tt>Collection</tt> to serialize </param>
		''' <exception cref="IOException"> if serialization failed </exception>
		Protected Friend Sub serialize(  oos As java.io.ObjectOutputStream,   coll As ICollection)
			Dim count As Integer = 0
			Dim objects As Object() = coll.ToArray()

			For i As Integer = 0 To objects.Length - 1
				If TypeOf objects(i) Is java.io.Serializable Then
					count += 1
				Else
					objects(i) = Nothing
				End If
			Next i

			oos.writeInt(count) ' number of subsequent objects

			Dim i As Integer = 0
			Do While count > 0
				Dim o As Object = objects(i)

				If o IsNot Nothing Then
					oos.writeObject(o)
					count -= 1
				End If
				i += 1
			Loop
		End Sub

		''' <summary>
		''' used by readObject to deserialize a collection. </summary>
		''' <param name="ois"> the ObjectInputStream to use </param>
		''' <param name="coll"> the Collection </param>
		''' <exception cref="IOException"> if deserialization failed </exception>
		''' <exception cref="ClassNotFoundException"> if needed classes are not found </exception>
		Protected Friend Sub deserialize(  ois As java.io.ObjectInputStream,   coll As ICollection)
			Dim count As Integer = 0

			count = ois.readInt()

			Dim tempVar As Boolean = count > 0
			count -= 1
			Do While tempVar
				coll.Add(ois.readObject())
				tempVar = count > 0
				count -= 1
			Loop
		End Sub

		''' <summary>
		''' Used to serialize all children of
		''' this <tt>BeanContext</tt>. </summary>
		''' <param name="oos"> the <tt>ObjectOutputStream</tt>
		''' to use during serialization </param>
		''' <exception cref="IOException"> if serialization failed </exception>
		Public Sub writeChildren(  oos As java.io.ObjectOutputStream)
			If serializable <= 0 Then Return

			Dim prev As Boolean = serializing

			serializing = True

			Dim count As Integer = 0

			SyncLock children
				Dim i As IEnumerator = children.GetEnumerator()

				Do While i.hasNext() AndAlso count < serializable
					Dim entry As DictionaryEntry = CType(i.next(), DictionaryEntry)

					If TypeOf entry.Key Is java.io.Serializable Then
						Try
							oos.writeObject(entry.Key) ' child
							oos.writeObject(entry.Value) ' BCSChild
						Catch ioe As java.io.IOException
							serializing = prev
							Throw ioe
						End Try
						count += 1
					End If
				Loop
			End SyncLock

			serializing = prev

			If count <> serializable Then Throw New java.io.IOException("wrote different number of children than expected")

		End Sub

		''' <summary>
		''' Serialize the BeanContextSupport, if this instance has a distinct
		''' peer (that is this object is acting as a delegate for another) then
		''' the children of this instance are not serialized here due to a
		''' 'chicken and egg' problem that occurs on deserialization of the
		''' children at the same time as this instance.
		''' 
		''' Therefore in situations where there is a distinct peer to this instance
		''' it should always call writeObject() followed by writeChildren() and
		''' readObject() followed by readChildren().
		''' </summary>
		''' <param name="oos"> the ObjectOutputStream </param>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub writeObject(  oos As java.io.ObjectOutputStream)
			serializing = True

			SyncLock BeanContext.globalHierarchyLock
				Try
					oos.defaultWriteObject() ' serialize the BeanContextSupport object

					bcsPreSerializationHook(oos)

					If serializable > 0 AndAlso Me.Equals(beanContextPeer) Then writeChildren(oos)

					serialize(oos, CType(bcmListeners, ICollection))
				Finally
					serializing = False
				End Try
			End SyncLock
		End Sub

		''' <summary>
		''' When an instance of this class is used as a delegate for the
		''' implementation of the BeanContext protocols (and its subprotocols)
		''' there exists a 'chicken and egg' problem during deserialization </summary>
		''' <param name="ois"> the ObjectInputStream to use </param>
		''' <exception cref="IOException"> if deserialization failed </exception>
		''' <exception cref="ClassNotFoundException"> if needed classes are not found </exception>

		Public Sub readChildren(  ois As java.io.ObjectInputStream)
			Dim count As Integer = serializable

			Dim tempVar As Boolean = count > 0
			count -= 1
			Do While tempVar
				Dim child As Object = Nothing
				Dim bscc As BeanContextSupport.BCSChild = Nothing

				Try
					child = ois.readObject()
					bscc = CType(ois.readObject(), BeanContextSupport.BCSChild)
				Catch ioe As java.io.IOException
					tempVar = count > 0
				count -= 1
					Continue Do
				Catch cnfe As  ClassNotFoundException
					tempVar = count > 0
				count -= 1
					Continue Do
				End Try


				SyncLock child
					Dim bcc As BeanContextChild = Nothing

					Try
						bcc = CType(child, BeanContextChild)
					Catch cce As  ClassCastException
						' do nothing;
					End Try

					If bcc IsNot Nothing Then
						Try
							bcc.beanContext = beanContextPeer

						   bcc.addPropertyChangeListener("beanContext", childPCL)
						   bcc.addVetoableChangeListener("beanContext", childVCL)

						Catch pve As java.beans.PropertyVetoException
							tempVar = count > 0
				count -= 1
							Continue Do
						End Try
					End If

					childDeserializedHook(child, bscc)
				End SyncLock
				tempVar = count > 0
				count -= 1
			Loop
		End Sub

		''' <summary>
		''' deserialize contents ... if this instance has a distinct peer the
		''' children are *not* serialized here, the peer's readObject() must call
		''' readChildren() after deserializing this instance.
		''' </summary>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub readObject(  ois As java.io.ObjectInputStream)

			SyncLock BeanContext.globalHierarchyLock
				ois.defaultReadObject()

				initialize()

				bcsPreDeserializationHook(ois)

				If serializable > 0 AndAlso Me.Equals(beanContextPeer) Then readChildren(ois)

'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				deserialize(ois, bcmListeners = New ArrayList(1))
			End SyncLock
		End Sub

		''' <summary>
		''' subclasses may envelope to monitor veto child property changes.
		''' </summary>

		Public Overridable Sub vetoableChange(  pce As java.beans.PropertyChangeEvent)
			Dim propertyName As String = pce.propertyName
			Dim source As Object = pce.source

			SyncLock children
				If "beanContext".Equals(propertyName) AndAlso containsKey(source) AndAlso (Not beanContextPeer.Equals(pce.newValue)) Then
					If Not validatePendingRemove(source) Then
						Throw New java.beans.PropertyVetoException("current BeanContext vetoes setBeanContext()", pce)
					Else
						CType(children(source), BCSChild).removePending = True
					End If
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' subclasses may envelope to monitor child property changes.
		''' </summary>

		Public Overridable Sub propertyChange(  pce As java.beans.PropertyChangeEvent)
			Dim propertyName As String = pce.propertyName
			Dim source As Object = pce.source

			SyncLock children
				If "beanContext".Equals(propertyName) AndAlso containsKey(source) AndAlso CType(children(source), BCSChild).removePending Then
					Dim bc As BeanContext = beanContextPeer

					If bc.Equals(pce.oldValue) AndAlso (Not bc.Equals(pce.newValue)) Then
						remove(source, False)
					Else
						CType(children(source), BCSChild).removePending = False
					End If
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' <p>
		''' Subclasses of this class may override, or envelope, this method to
		''' add validation behavior for the BeanContext to examine child objects
		''' immediately prior to their being added to the BeanContext.
		''' </p>
		''' </summary>
		''' <param name="targetChild"> the child to create the Child on behalf of </param>
		''' <returns> true iff the child may be added to this BeanContext, otherwise false. </returns>

		Protected Friend Overridable Function validatePendingAdd(  targetChild As Object) As Boolean
			Return True
		End Function

		''' <summary>
		''' <p>
		''' Subclasses of this class may override, or envelope, this method to
		''' add validation behavior for the BeanContext to examine child objects
		''' immediately prior to their being removed from the BeanContext.
		''' </p>
		''' </summary>
		''' <param name="targetChild"> the child to create the Child on behalf of </param>
		''' <returns> true iff the child may be removed from this BeanContext, otherwise false. </returns>

		Protected Friend Overridable Function validatePendingRemove(  targetChild As Object) As Boolean
			Return True
		End Function

		''' <summary>
		''' subclasses may override this method to simply extend add() semantics
		''' after the child has been added and before the event notification has
		''' occurred. The method is called with the child synchronized. </summary>
		''' <param name="child"> the child </param>
		''' <param name="bcsc"> the BCSChild </param>

		Protected Friend Overridable Sub childJustAddedHook(  child As Object,   bcsc As BCSChild)
		End Sub

		''' <summary>
		''' subclasses may override this method to simply extend remove() semantics
		''' after the child has been removed and before the event notification has
		''' occurred. The method is called with the child synchronized. </summary>
		''' <param name="child"> the child </param>
		''' <param name="bcsc"> the BCSChild </param>

		Protected Friend Overridable Sub childJustRemovedHook(  child As Object,   bcsc As BCSChild)
		End Sub

		''' <summary>
		''' Gets the Component (if any) associated with the specified child. </summary>
		''' <param name="child"> the specified child </param>
		''' <returns> the Component (if any) associated with the specified child. </returns>
		Protected Friend Shared Function getChildVisibility(  child As Object) As java.beans.Visibility
			Try
				Return CType(child, java.beans.Visibility)
			Catch cce As  ClassCastException
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Gets the Serializable (if any) associated with the specified Child </summary>
		''' <param name="child"> the specified child </param>
		''' <returns> the Serializable (if any) associated with the specified Child </returns>
		Protected Friend Shared Function getChildSerializable(  child As Object) As java.io.Serializable
			Try
				Return CType(child, java.io.Serializable)
			Catch cce As  ClassCastException
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Gets the PropertyChangeListener
		''' (if any) of the specified child </summary>
		''' <param name="child"> the specified child </param>
		''' <returns> the PropertyChangeListener (if any) of the specified child </returns>
		Protected Friend Shared Function getChildPropertyChangeListener(  child As Object) As java.beans.PropertyChangeListener
			Try
				Return CType(child, java.beans.PropertyChangeListener)
			Catch cce As  ClassCastException
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Gets the VetoableChangeListener
		''' (if any) of the specified child </summary>
		''' <param name="child"> the specified child </param>
		''' <returns> the VetoableChangeListener (if any) of the specified child </returns>
		Protected Friend Shared Function getChildVetoableChangeListener(  child As Object) As java.beans.VetoableChangeListener
			Try
				Return CType(child, java.beans.VetoableChangeListener)
			Catch cce As  ClassCastException
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Gets the BeanContextMembershipListener
		''' (if any) of the specified child </summary>
		''' <param name="child"> the specified child </param>
		''' <returns> the BeanContextMembershipListener (if any) of the specified child </returns>
		Protected Friend Shared Function getChildBeanContextMembershipListener(  child As Object) As BeanContextMembershipListener
			Try
				Return CType(child, BeanContextMembershipListener)
			Catch cce As  ClassCastException
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Gets the BeanContextChild (if any) of the specified child </summary>
		''' <param name="child"> the specified child </param>
		''' <returns>  the BeanContextChild (if any) of the specified child </returns>
		''' <exception cref="IllegalArgumentException"> if child implements both BeanContextChild and BeanContextProxy </exception>
		Protected Friend Shared Function getChildBeanContextChild(  child As Object) As BeanContextChild
			Try
				Dim bcc As BeanContextChild = CType(child, BeanContextChild)

				If TypeOf child Is BeanContextChild AndAlso TypeOf child Is BeanContextProxy Then
					Throw New IllegalArgumentException("child cannot implement both BeanContextChild and BeanContextProxy")
				Else
					Return bcc
				End If
			Catch cce As  ClassCastException
				Try
					Return CType(child, BeanContextProxy).beanContextProxy
				Catch cce1 As  ClassCastException
					Return Nothing
				End Try
			End Try
		End Function

		''' <summary>
		''' Fire a BeanContextshipEvent on the BeanContextMembershipListener interface </summary>
		''' <param name="bcme"> the event to fire </param>

		Protected Friend Sub fireChildrenAdded(  bcme As BeanContextMembershipEvent)
			Dim copy As Object()

			SyncLock bcmListeners
				copy = bcmListeners.ToArray()
			End SyncLock

			For i As Integer = 0 To copy.Length - 1
				CType(copy(i), BeanContextMembershipListener).childrenAdded(bcme)
			Next i
		End Sub

		''' <summary>
		''' Fire a BeanContextshipEvent on the BeanContextMembershipListener interface </summary>
		''' <param name="bcme"> the event to fire </param>

		Protected Friend Sub fireChildrenRemoved(  bcme As BeanContextMembershipEvent)
			Dim copy As Object()

			SyncLock bcmListeners
				copy = bcmListeners.ToArray()
			End SyncLock

			For i As Integer = 0 To copy.Length - 1
				CType(copy(i), BeanContextMembershipListener).childrenRemoved(bcme)
			Next i
		End Sub

		''' <summary>
		''' protected method called from constructor and readObject to initialize
		''' transient state of BeanContextSupport instance.
		''' 
		''' This class uses this method to instantiate inner class listeners used
		''' to monitor PropertyChange and VetoableChange events on children.
		''' 
		''' subclasses may envelope this method to add their own initialization
		''' behavior
		''' </summary>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub initialize()
			children = New Hashtable(serializable + 1)
			bcmListeners = New ArrayList(1)

			childPCL = New PropertyChangeListenerAnonymousInnerClassHelper

			childVCL = New VetoableChangeListenerAnonymousInnerClassHelper
		End Sub

		Private Class PropertyChangeListenerAnonymousInnerClassHelper
			Implements java.beans.PropertyChangeListener

		'            
		'             * this adaptor is used by the BeanContextSupport class to forward
		'             * property changes from a child to the BeanContext, avoiding
		'             * accidential serialization of the BeanContext by a badly
		'             * behaved Serializable child.
		'             

			Public Overridable Sub propertyChange(  pce As java.beans.PropertyChangeEvent)
				outerInstance.propertyChange(pce)
			End Sub
		End Class

		Private Class VetoableChangeListenerAnonymousInnerClassHelper
			Implements java.beans.VetoableChangeListener

		'            
		'             * this adaptor is used by the BeanContextSupport class to forward
		'             * vetoable changes from a child to the BeanContext, avoiding
		'             * accidential serialization of the BeanContext by a badly
		'             * behaved Serializable child.
		'             

			Public Overridable Sub vetoableChange(  pce As java.beans.PropertyChangeEvent)
				outerInstance.vetoableChange(pce)
			End Sub
		End Class

		''' <summary>
		''' Gets a copy of the this BeanContext's children. </summary>
		''' <returns> a copy of the current nested children </returns>
		Protected Friend Function copyChildren() As Object()
			SyncLock children
				Return children.Keys.ToArray()
			End SyncLock
		End Function

		''' <summary>
		''' Tests to see if two class objects,
		''' or their names are equal. </summary>
		''' <param name="first"> the first object </param>
		''' <param name="second"> the second object </param>
		''' <returns> true if equal, false if not </returns>
		Protected Friend Shared Function classEquals(  first As [Class],   second As [Class]) As Boolean
			Return first.Equals(second) OrElse first.name.Equals(second.name)
		End Function


	'    
	'     * fields
	'     


		''' <summary>
		''' all accesses to the <code> protected HashMap children </code> field
		''' shall be synchronized on that object.
		''' </summary>
		<NonSerialized> _
		Protected Friend children As Hashtable

		Private serializable As Integer = 0 ' children serializable

		''' <summary>
		''' all accesses to the <code> protected ArrayList bcmListeners </code> field
		''' shall be synchronized on that object.
		''' </summary>
		<NonSerialized> _
		Protected Friend bcmListeners As ArrayList

		'

		''' <summary>
		''' The current locale of this BeanContext.
		''' </summary>
		Protected Friend locale As java.util.Locale

		''' <summary>
		''' A <tt>boolean</tt> indicating if this
		''' instance may now render a GUI.
		''' </summary>
		Protected Friend okToUseGui_Renamed As Boolean


		''' <summary>
		''' A <tt>boolean</tt> indicating whether or not
		''' this object is currently in design time mode.
		''' </summary>
		Protected Friend designTime As Boolean

	'    
	'     * transient
	'     

		<NonSerialized> _
		Private childPCL As java.beans.PropertyChangeListener

		<NonSerialized> _
		Private childVCL As java.beans.VetoableChangeListener

		<NonSerialized> _
		Private serializing As Boolean
	End Class

End Namespace
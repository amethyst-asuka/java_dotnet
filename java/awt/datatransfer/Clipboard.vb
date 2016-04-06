Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.datatransfer





	''' <summary>
	''' A class that implements a mechanism to transfer data using
	''' cut/copy/paste operations.
	''' <p>
	''' <seealso cref="FlavorListener"/>s may be registered on an instance of the
	''' Clipboard class to be notified about changes to the set of
	''' <seealso cref="DataFlavor"/>s available on this clipboard (see
	''' <seealso cref="#addFlavorListener"/>).
	''' </summary>
	''' <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
	''' <seealso cref= java.awt.Toolkit#getSystemSelection
	''' 
	''' @author      Amy Fowler
	''' @author      Alexander Gerasimov </seealso>
	Public Class Clipboard

        Protected Friend owner As ClipboardOwner
        Protected Friend contents As Transferable

		''' <summary>
		''' An aggregate of flavor listeners registered on this local clipboard.
		''' 
		''' @since 1.5
		''' </summary>
		Private flavorListeners As sun.awt.EventListenerAggregate

		''' <summary>
		''' A set of <code>DataFlavor</code>s that is available on
		''' this local clipboard. It is used for tracking changes
		''' of <code>DataFlavor</code>s available on this clipboard.
		''' 
		''' @since 1.5
		''' </summary>
		Private currentDataFlavors As java.util.Set(Of DataFlavor)

		''' <summary>
		''' Creates a clipboard object.
		''' </summary>
		''' <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
		Public Sub New(  name As String)
			Me.name = name
		End Sub

        ''' <summary>
        ''' Returns the name of this clipboard object.
        ''' </summary>
        ''' <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
        Public Overridable ReadOnly Property name As String

        ''' <summary>
        ''' Sets the current contents of the clipboard to the specified
        ''' transferable object and registers the specified clipboard owner
        ''' as the owner of the new contents.
        ''' <p>
        ''' If there is an existing owner different from the argument
        ''' <code>owner</code>, that owner is notified that it no longer
        ''' holds ownership of the clipboard contents via an invocation
        ''' of <code>ClipboardOwner.lostOwnership()</code> on that owner.
        ''' An implementation of <code>setContents()</code> is free not
        ''' to invoke <code>lostOwnership()</code> directly from this method.
        ''' For example, <code>lostOwnership()</code> may be invoked later on
        ''' a different thread. The same applies to <code>FlavorListener</code>s
        ''' registered on this clipboard.
        ''' <p>
        ''' The method throws <code>IllegalStateException</code> if the clipboard
        ''' is currently unavailable. For example, on some platforms, the system
        ''' clipboard is unavailable while it is accessed by another application.
        ''' </summary>
        ''' <param name="contents"> the transferable object representing the
        '''                 clipboard content </param>
        ''' <param name="owner"> the object which owns the clipboard content </param>
        ''' <exception cref="IllegalStateException"> if the clipboard is currently unavailable </exception>
        ''' <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub setContents(  contents As Transferable,   owner As ClipboardOwner)
			Dim oldOwner As ClipboardOwner = Me.owner
			Dim oldContents As Transferable = Me.contents

			Me.owner = owner
			Me.contents = contents

            If oldOwner IsNot Nothing AndAlso oldOwner IsNot owner Then java.awt.EventQueue.invokeLater(New RunnableAnonymousInnerClassHelper)
            fireFlavorsChanged()
		End Sub

		Private Class RunnableAnonymousInnerClassHelper
			Implements Runnable

			Public Overridable Sub run() Implements Runnable.run
				oldOwner.lostOwnership(Clipboard.this, oldContents)
			End Sub
		End Class

		''' <summary>
		''' Returns a transferable object representing the current contents
		''' of the clipboard.  If the clipboard currently has no contents,
		''' it returns <code>null</code>. The parameter Object requestor is
		''' not currently used.  The method throws
		''' <code>IllegalStateException</code> if the clipboard is currently
		''' unavailable.  For example, on some platforms, the system clipboard is
		''' unavailable while it is accessed by another application.
		''' </summary>
		''' <param name="requestor"> the object requesting the clip data  (not used) </param>
		''' <returns> the current transferable object on the clipboard </returns>
		''' <exception cref="IllegalStateException"> if the clipboard is currently unavailable </exception>
		''' <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getContents(  requestor As Object) As Transferable
			Return contents
		End Function


		''' <summary>
		''' Returns an array of <code>DataFlavor</code>s in which the current
		''' contents of this clipboard can be provided. If there are no
		''' <code>DataFlavor</code>s available, this method returns a zero-length
		''' array.
		''' </summary>
		''' <returns> an array of <code>DataFlavor</code>s in which the current
		'''         contents of this clipboard can be provided
		''' </returns>
		''' <exception cref="IllegalStateException"> if this clipboard is currently unavailable
		''' 
		''' @since 1.5 </exception>
		Public Overridable Property availableDataFlavors As DataFlavor()
			Get
				Dim cntnts As Transferable = getContents(Nothing)
				If cntnts Is Nothing Then Return New DataFlavor(){}
				Return cntnts.transferDataFlavors
			End Get
		End Property

		''' <summary>
		''' Returns whether or not the current contents of this clipboard can be
		''' provided in the specified <code>DataFlavor</code>.
		''' </summary>
		''' <param name="flavor"> the requested <code>DataFlavor</code> for the contents
		''' </param>
		''' <returns> <code>true</code> if the current contents of this clipboard
		'''         can be provided in the specified <code>DataFlavor</code>;
		'''         <code>false</code> otherwise
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>flavor</code> is <code>null</code> </exception>
		''' <exception cref="IllegalStateException"> if this clipboard is currently unavailable
		''' 
		''' @since 1.5 </exception>
		Public Overridable Function isDataFlavorAvailable(  flavor As DataFlavor) As Boolean
			If flavor Is Nothing Then Throw New NullPointerException("flavor")

			Dim cntnts As Transferable = getContents(Nothing)
			If cntnts Is Nothing Then Return False
			Return cntnts.isDataFlavorSupported(flavor)
		End Function

		''' <summary>
		''' Returns an object representing the current contents of this clipboard
		''' in the specified <code>DataFlavor</code>.
		''' The class of the object returned is defined by the representation
		''' class of <code>flavor</code>.
		''' </summary>
		''' <param name="flavor"> the requested <code>DataFlavor</code> for the contents
		''' </param>
		''' <returns> an object representing the current contents of this clipboard
		'''         in the specified <code>DataFlavor</code>
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>flavor</code> is <code>null</code> </exception>
		''' <exception cref="IllegalStateException"> if this clipboard is currently unavailable </exception>
		''' <exception cref="UnsupportedFlavorException"> if the requested <code>DataFlavor</code>
		'''         is not available </exception>
		''' <exception cref="IOException"> if the data in the requested <code>DataFlavor</code>
		'''         can not be retrieved
		''' </exception>
		''' <seealso cref= DataFlavor#getRepresentationClass
		''' 
		''' @since 1.5 </seealso>
		Public Overridable Function getData(  flavor As DataFlavor) As Object
			If flavor Is Nothing Then Throw New NullPointerException("flavor")

			Dim cntnts As Transferable = getContents(Nothing)
			If cntnts Is Nothing Then Throw New UnsupportedFlavorException(flavor)
			Return cntnts.getTransferData(flavor)
		End Function


		''' <summary>
		''' Registers the specified <code>FlavorListener</code> to receive
		''' <code>FlavorEvent</code>s from this clipboard.
		''' If <code>listener</code> is <code>null</code>, no exception
		''' is thrown and no action is performed.
		''' </summary>
		''' <param name="listener"> the listener to be added
		''' </param>
		''' <seealso cref= #removeFlavorListener </seealso>
		''' <seealso cref= #getFlavorListeners </seealso>
		''' <seealso cref= FlavorListener </seealso>
		''' <seealso cref= FlavorEvent
		''' @since 1.5 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addFlavorListener(  listener As FlavorListener)
			If listener Is Nothing Then Return
			If flavorListeners Is Nothing Then
				currentDataFlavors = availableDataFlavorSet
				flavorListeners = New sun.awt.EventListenerAggregate(GetType(FlavorListener))
			End If
			flavorListeners.add(listener)
		End Sub

		''' <summary>
		''' Removes the specified <code>FlavorListener</code> so that it no longer
		''' receives <code>FlavorEvent</code>s from this <code>Clipboard</code>.
		''' This method performs no function, nor does it throw an exception, if
		''' the listener specified by the argument was not previously added to this
		''' <code>Clipboard</code>.
		''' If <code>listener</code> is <code>null</code>, no exception
		''' is thrown and no action is performed.
		''' </summary>
		''' <param name="listener"> the listener to be removed
		''' </param>
		''' <seealso cref= #addFlavorListener </seealso>
		''' <seealso cref= #getFlavorListeners </seealso>
		''' <seealso cref= FlavorListener </seealso>
		''' <seealso cref= FlavorEvent
		''' @since 1.5 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeFlavorListener(  listener As FlavorListener)
			If listener Is Nothing OrElse flavorListeners Is Nothing Then Return
			flavorListeners.remove(listener)
		End Sub

        ''' <summary>
        ''' Returns an array of all the <code>FlavorListener</code>s currently
        ''' registered on this <code>Clipboard</code>.
        ''' </summary>
        ''' <returns> all of this clipboard's <code>FlavorListener</code>s or an empty
        '''         array if no listeners are currently registered </returns>
        ''' <seealso cref= #addFlavorListener </seealso>
        ''' <seealso cref= #removeFlavorListener </seealso>
        ''' <seealso cref= FlavorListener </seealso>
        ''' <seealso cref= FlavorEvent
        ''' @since 1.5 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable ReadOnly Property flavorListeners As FlavorListener()
            Get
                Return If(flavorListeners Is Nothing, New FlavorListener() {}, CType(flavorListeners.listenersCopy, FlavorListener()))
            End Get
        End Property

        ''' <summary>
        ''' Checks change of the <code>DataFlavor</code>s and, if necessary,
        ''' notifies all listeners that have registered interest for notification
        ''' on <code>FlavorEvent</code>s.
        ''' 
        ''' @since 1.5
        ''' </summary>
        Private Sub fireFlavorsChanged()
			If flavorListeners Is Nothing Then Return
			Dim prevDataFlavors As java.util.Set(Of DataFlavor) = currentDataFlavors
			currentDataFlavors = availableDataFlavorSet
			If prevDataFlavors.Equals(currentDataFlavors) Then Return
			Dim flavorListenerArray As FlavorListener() = CType(flavorListeners.listenersInternal, FlavorListener())
			For i As Integer = 0 To flavorListenerArray.Length - 1
				Dim listener As FlavorListener = flavorListenerArray(i)
                java.awt.EventQueue.invokeLater(New RunnableAnonymousInnerClassHelper2)
            Next i
		End Sub

		Private Class RunnableAnonymousInnerClassHelper2
			Implements Runnable

			Public Overridable Sub run() Implements Runnable.run
				listener.flavorsChanged(New FlavorEvent(Clipboard.this))
			End Sub
		End Class

		''' <summary>
		''' Returns a set of <code>DataFlavor</code>s currently available
		''' on this clipboard.
		''' </summary>
		''' <returns> a set of <code>DataFlavor</code>s currently available
		'''         on this clipboard
		''' 
		''' @since 1.5 </returns>
		Private Property availableDataFlavorSet As java.util.Set(Of DataFlavor)
			Get
				Dim [set] As java.util.Set(Of DataFlavor) = New HashSet(Of DataFlavor)
				Dim contents_Renamed As Transferable = getContents(Nothing)
				If contents_Renamed IsNot Nothing Then
					Dim flavors As DataFlavor() = contents_Renamed.transferDataFlavors
					If flavors IsNot Nothing Then [set].addAll(java.util.Arrays.asList(flavors))
				End If
				Return [set]
			End Get
		End Property
	End Class

End Namespace
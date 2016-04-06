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
	''' A <code>BeanContextMembershipEvent</code> encapsulates
	''' the list of children added to, or removed from,
	''' the membership of a particular <code>BeanContext</code>.
	''' An instance of this event is fired whenever a successful
	''' add(), remove(), retainAll(), removeAll(), or clear() is
	''' invoked on a given <code>BeanContext</code> instance.
	''' Objects interested in receiving events of this type must
	''' implement the <code>BeanContextMembershipListener</code>
	''' interface, and must register their intent via the
	''' <code>BeanContext</code>'s
	''' <code>addBeanContextMembershipListener(BeanContextMembershipListener bcml)
	''' </code> method.
	''' 
	''' @author      Laurence P. G. Cable
	''' @since       1.2 </summary>
	''' <seealso cref=         java.beans.beancontext.BeanContext </seealso>
	''' <seealso cref=         java.beans.beancontext.BeanContextEvent </seealso>
	''' <seealso cref=         java.beans.beancontext.BeanContextMembershipListener </seealso>
	Public Class BeanContextMembershipEvent
		Inherits java.beans.beancontext.BeanContextEvent

		Private Const serialVersionUID As Long = 3499346510334590959L

		''' <summary>
		''' Contruct a BeanContextMembershipEvent
		''' </summary>
		''' <param name="bc">        The BeanContext source </param>
		''' <param name="changes">   The Children affected </param>
		''' <exception cref="NullPointerException"> if <CODE>changes</CODE> is <CODE>null</CODE> </exception>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(  bc As java.beans.beancontext.BeanContext,   changes As ICollection)
			MyBase.New(bc)

			If changes Is Nothing Then Throw New NullPointerException("BeanContextMembershipEvent constructor:  changes is null.")

			children = changes
		End Sub

		''' <summary>
		''' Contruct a BeanContextMembershipEvent
		''' </summary>
		''' <param name="bc">        The BeanContext source </param>
		''' <param name="changes">   The Children effected </param>
		''' <exception cref="NullPointerException"> if changes associated with this
		'''                  event are null. </exception>

		Public Sub New(  bc As java.beans.beancontext.BeanContext,   changes As Object())
			MyBase.New(bc)

			If changes Is Nothing Then Throw New NullPointerException("BeanContextMembershipEvent:  changes is null.")

			children = java.util.Arrays.asList(changes)
		End Sub

		''' <summary>
		''' Gets the number of children affected by the notification. </summary>
		''' <returns> the number of children affected by the notification </returns>
		Public Overridable Function size() As Integer
			Return children.Count
		End Function

		''' <summary>
		''' Is the child specified affected by the event? </summary>
		''' <returns> <code>true</code> if affected, <code>false</code>
		''' if not </returns>
		''' <param name="child"> the object to check for being affected </param>
		Public Overridable Function contains(  child As Object) As Boolean
			Return children.Contains(child)
		End Function

		''' <summary>
		''' Gets the array of children affected by this event. </summary>
		''' <returns> the array of children affected </returns>
		Public Overridable Function toArray() As Object()
			Return children.ToArray()
		End Function

		''' <summary>
		''' Gets the array of children affected by this event. </summary>
		''' <returns> the array of children effected </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function [iterator]() As IEnumerator
	'    
	'     * fields
	'     
	   ''' <summary>
	   ''' The list of children affected by this
	   ''' event notification.
	   ''' </summary>
			Return children.GetEnumerator()
		End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend children As ICollection
	End Class

End Namespace
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management




	''' <summary>
	''' This class implements of the <seealso cref="javax.management.NotificationFilter NotificationFilter"/>
	''' interface for the <seealso cref="javax.management.AttributeChangeNotification attribute change notification"/>.
	''' The filtering is performed on the name of the observed attribute.
	''' <P>
	''' It manages a list of enabled attribute names.
	''' A method allows users to enable/disable as many attribute names as required.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class AttributeChangeNotificationFilter
		Implements NotificationFilter

		' Serial version 
		Private Const serialVersionUID As Long = -6347317584796410029L

		''' <summary>
		''' @serial <seealso cref="Vector"/> that contains the enabled attribute names.
		'''         The default value is an empty vector.
		''' </summary>
		Private enabledAttributes As New List(Of String)


		''' <summary>
		''' Invoked before sending the specified notification to the listener.
		''' <BR>This filter compares the attribute name of the specified attribute change notification
		''' with each enabled attribute name.
		''' If the attribute name equals one of the enabled attribute names,
		''' the notification must be sent to the listener and this method returns <CODE>true</CODE>.
		''' </summary>
		''' <param name="notification"> The attribute change notification to be sent. </param>
		''' <returns> <CODE>true</CODE> if the notification has to be sent to the listener, <CODE>false</CODE> otherwise. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function isNotificationEnabled(ByVal ___notification As Notification) As Boolean Implements NotificationFilter.isNotificationEnabled

			Dim type As String = ___notification.type

			If (type Is Nothing) OrElse (type.Equals(AttributeChangeNotification.ATTRIBUTE_CHANGE) = False) OrElse (Not(TypeOf ___notification Is AttributeChangeNotification)) Then Return False

			Dim attributeName As String = CType(___notification, AttributeChangeNotification).attributeName
			Return enabledAttributes.Contains(attributeName)
		End Function

		''' <summary>
		''' Enables all the attribute change notifications the attribute name of which equals
		''' the specified name to be sent to the listener.
		''' <BR>If the specified name is already in the list of enabled attribute names,
		''' this method has no effect.
		''' </summary>
		''' <param name="name"> The attribute name. </param>
		''' <exception cref="java.lang.IllegalArgumentException"> The attribute name parameter is null. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub enableAttribute(ByVal name As String)

			If name Is Nothing Then Throw New System.ArgumentException("The name cannot be null.")
			If Not enabledAttributes.Contains(name) Then enabledAttributes.Add(name)
		End Sub

		''' <summary>
		''' Disables all the attribute change notifications the attribute name of which equals
		''' the specified attribute name to be sent to the listener.
		''' <BR>If the specified name is not in the list of enabled attribute names,
		''' this method has no effect.
		''' </summary>
		''' <param name="name"> The attribute name. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub disableAttribute(ByVal name As String)
			enabledAttributes.Remove(name)
		End Sub

		''' <summary>
		''' Disables all the attribute names.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub disableAllAttributes()
			enabledAttributes.Clear()
		End Sub

		''' <summary>
		''' Gets all the enabled attribute names for this filter.
		''' </summary>
		''' <returns> The list containing all the enabled attribute names. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property enabledAttributes As List(Of String)
			Get
				Return enabledAttributes
			End Get
		End Property

	End Class

End Namespace
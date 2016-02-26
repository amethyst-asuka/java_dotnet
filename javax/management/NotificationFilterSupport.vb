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
	''' Provides an implementation of the <seealso cref="javax.management.NotificationFilter"/> interface.
	''' The filtering is performed on the notification type attribute.
	''' <P>
	''' Manages a list of enabled notification types.
	''' A method allows users to enable/disable as many notification types as required.
	''' <P>
	''' Then, before sending a notification to a listener registered with a filter,
	''' the notification broadcaster compares this notification type with all notification types
	''' enabled by the filter. The notification will be sent to the listener
	''' only if its filter enables this notification type.
	''' <P>
	''' Example:
	''' <BLOCKQUOTE>
	''' <PRE>
	''' NotificationFilterSupport myFilter = new NotificationFilterSupport();
	''' myFilter.enableType("my_example.my_type");
	''' myBroadcaster.addListener(myListener, myFilter, null);
	''' </PRE>
	''' </BLOCKQUOTE>
	''' The listener <CODE>myListener</CODE> will only receive notifications the type of which equals/starts with "my_example.my_type".
	''' </summary>
	''' <seealso cref= javax.management.NotificationBroadcaster#addNotificationListener
	''' 
	''' @since 1.5 </seealso>
	Public Class NotificationFilterSupport
		Implements NotificationFilter

		' Serial version 
		Private Const serialVersionUID As Long = 6579080007561786969L

		''' <summary>
		''' @serial <seealso cref="Vector"/> that contains the enabled notification types.
		'''         The default value is an empty vector.
		''' </summary>
		Private enabledTypes As IList(Of String) = New List(Of String)


		''' <summary>
		''' Invoked before sending the specified notification to the listener.
		''' <BR>This filter compares the type of the specified notification with each enabled type.
		''' If the notification type matches one of the enabled types,
		''' the notification should be sent to the listener and this method returns <CODE>true</CODE>.
		''' </summary>
		''' <param name="notification"> The notification to be sent. </param>
		''' <returns> <CODE>true</CODE> if the notification should be sent to the listener, <CODE>false</CODE> otherwise. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function isNotificationEnabled(ByVal ___notification As Notification) As Boolean Implements NotificationFilter.isNotificationEnabled

			Dim type As String = ___notification.type

			If type Is Nothing Then Return False
			Try
				For Each prefix As String In enabledTypes
					If type.StartsWith(prefix) Then Return True
				Next prefix
			Catch e As java.lang.NullPointerException
				' Should never occurs...
				Return False
			End Try
			Return False
		End Function

		''' <summary>
		''' Enables all the notifications the type of which starts with the specified prefix
		''' to be sent to the listener.
		''' <BR>If the specified prefix is already in the list of enabled notification types,
		''' this method has no effect.
		''' <P>
		''' Example:
		''' <BLOCKQUOTE>
		''' <PRE>
		''' // Enables all notifications the type of which starts with "my_example" to be sent.
		''' myFilter.enableType("my_example");
		''' // Enables all notifications the type of which is "my_example.my_type" to be sent.
		''' myFilter.enableType("my_example.my_type");
		''' </PRE>
		''' </BLOCKQUOTE>
		''' 
		''' Note that:
		''' <BLOCKQUOTE><CODE>
		''' myFilter.enableType("my_example.*");
		''' </CODE></BLOCKQUOTE>
		''' will no match any notification type.
		''' </summary>
		''' <param name="prefix"> The prefix. </param>
		''' <exception cref="java.lang.IllegalArgumentException"> The prefix parameter is null. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub enableType(ByVal prefix As String)

			If prefix Is Nothing Then Throw New System.ArgumentException("The prefix cannot be null.")
			If Not enabledTypes.Contains(prefix) Then enabledTypes.Add(prefix)
		End Sub

		''' <summary>
		''' Removes the given prefix from the prefix list.
		''' <BR>If the specified prefix is not in the list of enabled notification types,
		''' this method has no effect.
		''' </summary>
		''' <param name="prefix"> The prefix. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub disableType(ByVal prefix As String)
			enabledTypes.Remove(prefix)
		End Sub

		''' <summary>
		''' Disables all notification types.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub disableAllTypes()
			enabledTypes.Clear()
		End Sub


		''' <summary>
		''' Gets all the enabled notification types for this filter.
		''' </summary>
		''' <returns> The list containing all the enabled notification types. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property enabledTypes As List(Of String)
			Get
				Return CType(enabledTypes, List(Of String))
			End Get
		End Property

	End Class

End Namespace
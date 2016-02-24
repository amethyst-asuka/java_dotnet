'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt


	Friend MustInherit Class AttributeValue
		Private Shared ReadOnly log As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.AttributeValue")
		Private ReadOnly value As Integer
		Private ReadOnly names As String()

		Protected Friend Sub New(ByVal value As Integer, ByVal names As String())
			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then log.finest("value = " & value & ", names = " & names)

			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then
				If (value < 0) OrElse (names Is Nothing) OrElse (value >= names.Length) Then log.finer("Assertion failed")
			End If
			Me.value = value
			Me.names = names
		End Sub
		' This hashCode is used by the sun.awt implementation as an array
		' index.
		Public Overrides Function GetHashCode() As Integer
			Return value
		End Function
		Public Overrides Function ToString() As String
			Return names(value)
		End Function
	End Class

End Namespace
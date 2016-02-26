Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing


	''' <summary>
	''' ImageObserver for DebugGraphics, used for images only.
	'''  
	''' @author Dave Karlton
	''' </summary>
	Friend Class DebugGraphicsObserver
		Implements ImageObserver

		Friend lastInfo As Integer

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Function allBitsPresent() As Boolean
			Return (lastInfo And ImageObserver.ALLBITS) <> 0
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Function imageHasProblem() As Boolean
			Return ((lastInfo And ImageObserver.ERROR) <> 0 OrElse (lastInfo And ImageObserver.ABORT) <> 0)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function imageUpdate(ByVal img As Image, ByVal infoflags As Integer, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As Boolean
			lastInfo = infoflags
			Return True
		End Function
	End Class

End Namespace
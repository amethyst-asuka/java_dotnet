'
' * Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.net


	Friend Class SocketSecrets

		' accessed by reflection from jdk.net.Sockets 

		' obj must be a Socket or ServerSocket 

		Private Shared Sub setOption(Of T)(  obj As Object,   name As SocketOption(Of T),   value As T)
			Dim impl As SocketImpl

			If TypeOf obj Is Socket Then
				impl = CType(obj, Socket).impl
			ElseIf TypeOf obj Is ServerSocket Then
				impl = CType(obj, ServerSocket).impl
			Else
				Throw New IllegalArgumentException
			End If
			impl.optionion(name, value)
		End Sub

		Private Shared Function getOption(Of T)(  obj As Object,   name As SocketOption(Of T)) As T
			Dim impl As SocketImpl

			If TypeOf obj Is Socket Then
				impl = CType(obj, Socket).impl
			ElseIf TypeOf obj Is ServerSocket Then
				impl = CType(obj, ServerSocket).impl
			Else
				Throw New IllegalArgumentException
			End If
			Return impl.getOption(name)
		End Function

		Private Shared Sub setOption(Of T)(  s As DatagramSocket,   name As SocketOption(Of T),   value As T)
			s.impl.optionion(name, value)
		End Sub

		Private Shared Function getOption(Of T)(  s As DatagramSocket,   name As SocketOption(Of T)) As T
			Return s.impl.getOption(name)
		End Function

	End Class

End Namespace
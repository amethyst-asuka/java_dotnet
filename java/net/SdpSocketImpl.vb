'
' * Copyright (c) 2010, Oracle and/or its affiliates. All rights reserved.
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



	''' <summary>
	''' SocketImpl that supports the SDP protocol
	''' </summary>
	Friend Class SdpSocketImpl
		Inherits PlainSocketImpl

		Friend Sub New()
		End Sub

		Protected Friend Overrides Sub create(  stream As Boolean)
			If Not stream Then Throw New UnsupportedOperationException("Must be a stream socket")
			fd = sun.net.sdp.SdpSupport.createSocket()
			If socket_Renamed IsNot Nothing Then socket_Renamed.createdted()
			If serverSocket_Renamed IsNot Nothing Then serverSocket_Renamed.createdted()
		End Sub
	End Class

End Namespace
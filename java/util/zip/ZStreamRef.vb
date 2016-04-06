'
' * Copyright (c) 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.zip

	''' <summary>
	''' A reference to the native zlib's z_stream structure.
	''' </summary>

	Friend Class ZStreamRef

		Private address_Renamed As Long
		Friend Sub New(  address As Long)
			Me.address_Renamed = address
		End Sub

		Friend Overridable Function address() As Long
			Return address_Renamed
		End Function

		Friend Overridable Sub clear()
			address_Renamed = 0
		End Sub
	End Class

End Namespace
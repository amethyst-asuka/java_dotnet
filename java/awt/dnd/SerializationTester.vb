'
' * Copyright (c) 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.dnd


	''' <summary>
	''' Tests if an object can truly be serialized by serializing it to a null
	''' OutputStream.
	''' 
	''' @since 1.4
	''' </summary>
	Friend NotInheritable Class SerializationTester
		Private Shared stream As java.io.ObjectOutputStream
		Shared Sub New()
			Try
                stream = New java.io.ObjectOutputStream(New OutputStreamAnonymousInnerClassHelper)
            Catch cannotHappen As java.io.IOException
			End Try
		End Sub

		Private Class OutputStreamAnonymousInnerClassHelper
			Inherits java.io.OutputStream

			Public Overrides Sub write(  b As Integer)
			End Sub
		End Class

		Friend Shared Function test(  obj As Object) As Boolean
			If Not(TypeOf obj Is java.io.Serializable) Then Return False

			Try
				stream.writeObject(obj)
			Catch e As java.io.IOException
				Return False
			Finally
				' Fix for 4503661.
				' Reset the stream so that it doesn't keep a reference to the
				' written object.
				Try
					stream.reset()
				Catch e As java.io.IOException
					' Ignore the exception.
				End Try
			End Try
			Return True
		End Function

		Private Sub New()
		End Sub
	End Class

End Namespace
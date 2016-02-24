'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.reflect

	''' <summary>
	''' Thrown when {@link java.lang.reflect.Executable#getParameters the
	''' java.lang.reflect package} attempts to read method parameters from
	''' a class file and determines that one or more parameters are
	''' malformed.
	''' 
	''' <p>The following is a list of conditions under which this exception
	''' can be thrown:
	''' <ul>
	''' <li> The number of parameters (parameter_count) is wrong for the method
	''' <li> A constant pool index is out of bounds.
	''' <li> A constant pool index does not refer to a UTF-8 entry
	''' <li> A parameter's name is "", or contains an illegal character
	''' <li> The flags field contains an illegal flag (something other than
	'''     FINAL, SYNTHETIC, or MANDATED)
	''' </ul>
	''' 
	''' See <seealso cref="java.lang.reflect.Executable#getParameters"/> for more
	''' information.
	''' </summary>
	''' <seealso cref= java.lang.reflect.Executable#getParameters
	''' @since 1.8 </seealso>
	Public Class MalformedParametersException
		Inherits RuntimeException

		''' <summary>
		''' Version for serialization.
		''' </summary>
		Private Shadows Const serialVersionUID As Long = 20130919L

		''' <summary>
		''' Create a {@code MalformedParametersException} with an empty
		''' reason.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Create a {@code MalformedParametersException}.
		''' </summary>
		''' <param name="reason"> The reason for the exception. </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(reason)
		End Sub
	End Class

End Namespace
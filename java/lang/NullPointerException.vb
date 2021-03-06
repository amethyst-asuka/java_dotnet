'
' * Copyright (c) 1994, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang

	''' <summary>
	''' Thrown when an application attempts to use {@code null} in a
	''' case where an object is required. These include:
	''' <ul>
	''' <li>Calling the instance method of a {@code null} object.
	''' <li>Accessing or modifying the field of a {@code null} object.
	''' <li>Taking the length of {@code null} as if it were an array.
	''' <li>Accessing or modifying the slots of {@code null} as if it
	'''     were an array.
	''' <li>Throwing {@code null} as if it were a {@code Throwable}
	'''     value.
	''' </ul>
	''' <p>
	''' Applications should throw instances of this class to indicate
	''' other illegal uses of the {@code null} object.
	''' 
	''' {@code NullPointerException} objects may be constructed by the
	''' virtual machine as if {@link Throwable#Throwable(String,
	''' Throwable, boolean, boolean) suppression were disabled and/or the
	''' stack trace was not writable}.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class NullPointerException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = 5162710183389028792L

		''' <summary>
		''' Constructs a {@code NullPointerException} with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a {@code NullPointerException} with the specified
		''' detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
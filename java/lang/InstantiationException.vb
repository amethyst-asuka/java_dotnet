'
' * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when an application tries to create an instance of a class
	''' using the {@code newInstance} method in class
	''' {@code Class}, but the specified class object cannot be
	''' instantiated.  The instantiation can fail for a variety of
	''' reasons including but not limited to:
	''' 
	''' <ul>
	''' <li> the class object represents an abstract class, an interface,
	'''      an array class, a primitive type, or {@code void}
	''' <li> the class has no nullary constructor
	''' </ul>
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     java.lang.Class#newInstance()
	''' @since   JDK1.0 </seealso>
	Public Class InstantiationException
		Inherits ReflectiveOperationException

		Private Shadows Const serialVersionUID As Long = -8441929162975509110L

		''' <summary>
		''' Constructs an {@code InstantiationException} with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an {@code InstantiationException} with the
		''' specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
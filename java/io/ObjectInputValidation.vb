'
' * Copyright (c) 1996, 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io

	''' <summary>
	''' Callback interface to allow validation of objects within a graph.
	''' Allows an object to be called when a complete graph of objects has
	''' been deserialized.
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     ObjectInputStream </seealso>
	''' <seealso cref=     ObjectInputStream#registerValidation(java.io.ObjectInputValidation, int)
	''' @since   JDK1.1 </seealso>
	Public Interface ObjectInputValidation
		''' <summary>
		''' Validates the object.
		''' </summary>
		''' <exception cref="InvalidObjectException"> If the object cannot validate itself. </exception>
		Sub validateObject()
	End Interface

End Namespace
'
' * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' The {@code Void} class is an uninstantiable placeholder class to hold a
	''' reference to the {@code Class} object representing the Java keyword
	''' void.
	''' 
	''' @author  unascribed
	''' @since   JDK1.1
	''' </summary>
	Public NotInheritable Class Void

		''' <summary>
		''' The {@code Class} object representing the pseudo-type corresponding to
		''' the keyword {@code void}.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly TYPE As  [Class] = CType(Class.getPrimitiveClass("void"), [Class])

	'    
	'     * The  Sub  class cannot be instantiated.
	'     
		Private Sub New()
		End Sub
	End Class

End Namespace
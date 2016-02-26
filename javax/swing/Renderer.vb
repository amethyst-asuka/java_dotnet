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
	''' Defines the requirements for an object responsible for
	''' "rendering" (displaying) a value.
	''' 
	''' @author Arnaud Weber
	''' </summary>
	Public Interface Renderer
		''' <summary>
		''' Specifies the value to display and whether or not the
		''' value should be portrayed as "currently selected".
		''' </summary>
		''' <param name="aValue">      an Object object </param>
		''' <param name="isSelected">  a boolean </param>
		Sub setValue(ByVal aValue As Object, ByVal isSelected As Boolean)
		''' <summary>
		''' Returns the component used to render the value.
		''' </summary>
		''' <returns> the Component responsible for displaying the value </returns>
		ReadOnly Property component As java.awt.Component
	End Interface

End Namespace
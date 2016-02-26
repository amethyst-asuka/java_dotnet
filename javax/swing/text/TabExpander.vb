'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text


	''' <summary>
	''' Simple interface to allow for different types of
	''' implementations of tab expansion.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Interface TabExpander

		''' <summary>
		''' Returns the next tab stop position given a reference
		''' position.  Values are expressed in points.
		''' </summary>
		''' <param name="x"> the position in points &gt;= 0 </param>
		''' <param name="tabOffset"> the position within the text stream
		'''   that the tab occurred at &gt;= 0. </param>
		''' <returns> the next tab stop &gt;= 0 </returns>
		Function nextTabStop(ByVal x As Single, ByVal tabOffset As Integer) As Single

	End Interface

End Namespace
'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.table

	''' <summary>
	''' TableStringConverter is used to convert objects from the model into
	''' strings.  This is useful in filtering and searching when the model returns
	''' objects that do not have meaningful <code>toString</code> implementations.
	''' 
	''' @since 1.6
	''' </summary>
	Public MustInherit Class TableStringConverter
		''' <summary>
		''' Returns the string representation of the value at the specified
		''' location.
		''' </summary>
		''' <param name="model"> the <code>TableModel</code> to fetch the value from </param>
		''' <param name="row"> the row the string is being requested for </param>
		''' <param name="column"> the column the string is being requested for </param>
		''' <returns> the string representation.  This should never return null. </returns>
		''' <exception cref="NullPointerException"> if <code>model</code> is null </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the arguments are outside the
		'''         bounds of the model </exception>
		Public MustOverride Function ToString(ByVal model As TableModel, ByVal row As Integer, ByVal column As Integer) As String
	End Class

End Namespace
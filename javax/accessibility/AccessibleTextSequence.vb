'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.accessibility


	''' <summary>
	''' <P>The AccessibleTextSequence provides information about
	''' a contiguous sequence of text.
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleText </seealso>
	''' <seealso cref= AccessibleAttributeSequence
	''' 
	''' @author       Lynn Monsanto </seealso>

	''' <summary>
	''' This class collects together key details of a span of text.  It
	''' is used by implementors of the class <code>AccessibleExtendedText</code> in
	''' order to return the requested triplet of a <code>String</code>, and the
	''' start and end indicies/offsets into a larger body of text that the
	''' <code>String</code> comes from.
	''' </summary>
	''' <seealso cref= javax.accessibility.AccessibleExtendedText </seealso>
	Public Class AccessibleTextSequence

		''' <summary>
		''' The start index of the text sequence </summary>
		Public startIndex As Integer

		''' <summary>
		''' The end index of the text sequence </summary>
		Public endIndex As Integer

		''' <summary>
		''' The text </summary>
		Public text As String

		''' <summary>
		''' Constructs an <code>AccessibleTextSequence</code> with the given
		''' parameters.
		''' </summary>
		''' <param name="start"> the beginning index of the span of text </param>
		''' <param name="end"> the ending index of the span of text </param>
		''' <param name="txt"> the <code>String</code> shared by this text span
		''' 
		''' @since 1.6 </param>
		Public Sub New(ByVal start As Integer, ByVal [end] As Integer, ByVal txt As String)
			startIndex = start
			endIndex = [end]
			text = txt
		End Sub
	End Class

End Namespace
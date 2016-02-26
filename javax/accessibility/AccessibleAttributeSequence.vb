'
' * Copyright (c) 2003, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' <P>The AccessibleAttributeSequence provides information about
	''' a contiguous sequence of text attributes
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleText </seealso>
	''' <seealso cref= AccessibleTextSequence
	''' 
	''' @author       Lynn Monsanto </seealso>

	''' <summary>
	''' This class collects together the span of text that share the same
	''' contiguous set of attributes, along with that set of attributes.  It
	''' is used by implementors of the class <code>AccessibleContext</code> in
	''' order to generate <code>ACCESSIBLE_TEXT_ATTRIBUTES_CHANGED</code> events.
	''' </summary>
	''' <seealso cref= javax.accessibility.AccessibleContext </seealso>
	''' <seealso cref= javax.accessibility.AccessibleContext#ACCESSIBLE_TEXT_ATTRIBUTES_CHANGED </seealso>
	Public Class AccessibleAttributeSequence
		''' <summary>
		''' The start index of the text sequence </summary>
		Public startIndex As Integer

		''' <summary>
		''' The end index of the text sequence </summary>
		Public endIndex As Integer

		''' <summary>
		''' The text attributes </summary>
		Public attributes As javax.swing.text.AttributeSet

		''' <summary>
		''' Constructs an <code>AccessibleAttributeSequence</code> with the given
		''' parameters.
		''' </summary>
		''' <param name="start"> the beginning index of the span of text </param>
		''' <param name="end"> the ending index of the span of text </param>
		''' <param name="attr"> the <code>AttributeSet</code> shared by this text span
		''' 
		''' @since 1.6 </param>
		Public Sub New(ByVal start As Integer, ByVal [end] As Integer, ByVal attr As javax.swing.text.AttributeSet)
			startIndex = start
			endIndex = [end]
			attributes = attr
		End Sub

	End Class

End Namespace
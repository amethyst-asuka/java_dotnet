Imports System.Collections.Generic

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

Namespace java.awt.im


	''' <summary>
	''' An InputMethodHighlight is used to describe the highlight
	''' attributes of text being composed.
	''' The description can be at two levels:
	''' at the abstract level it specifies the conversion state and whether the
	''' text is selected; at the concrete level it specifies style attributes used
	''' to render the highlight.
	''' An InputMethodHighlight must provide the description at the
	''' abstract level; it may or may not provide the description at the concrete
	''' level.
	''' If no concrete style is provided, a renderer should use
	''' <seealso cref="java.awt.Toolkit#mapInputMethodHighlight"/> to map to a concrete style.
	''' <p>
	''' The abstract description consists of three fields: <code>selected</code>,
	''' <code>state</code>, and <code>variation</code>.
	''' <code>selected</code> indicates whether the text range is the one that the
	''' input method is currently working on, for example, the segment for which
	''' conversion candidates are currently shown in a menu.
	''' <code>state</code> represents the conversion state. State values are defined
	''' by the input method framework and should be distinguished in all
	''' mappings from abstract to concrete styles. Currently defined state values
	''' are raw (unconverted) and converted.
	''' These state values are recommended for use before and after the
	''' main conversion step of text composition, say, before and after kana-&gt;kanji
	''' or pinyin-&gt;hanzi conversion.
	''' The <code>variation</code> field allows input methods to express additional
	''' information about the conversion results.
	''' <p>
	''' 
	''' InputMethodHighlight instances are typically used as attribute values
	''' returned from AttributedCharacterIterator for the INPUT_METHOD_HIGHLIGHT
	''' attribute. They may be wrapped into <seealso cref="java.text.Annotation Annotation"/>
	''' instances to indicate separate text segments.
	''' </summary>
	''' <seealso cref= java.text.AttributedCharacterIterator
	''' @since 1.2 </seealso>

	Public Class InputMethodHighlight

		''' <summary>
		''' Constant for the raw text state.
		''' </summary>
		Public Const RAW_TEXT As Integer = 0

		''' <summary>
		''' Constant for the converted text state.
		''' </summary>
		Public Const CONVERTED_TEXT As Integer = 1


		''' <summary>
		''' Constant for the default highlight for unselected raw text.
		''' </summary>
		Public Shared ReadOnly UNSELECTED_RAW_TEXT_HIGHLIGHT As New InputMethodHighlight(False, RAW_TEXT)

		''' <summary>
		''' Constant for the default highlight for selected raw text.
		''' </summary>
		Public Shared ReadOnly SELECTED_RAW_TEXT_HIGHLIGHT As New InputMethodHighlight(True, RAW_TEXT)

		''' <summary>
		''' Constant for the default highlight for unselected converted text.
		''' </summary>
		Public Shared ReadOnly UNSELECTED_CONVERTED_TEXT_HIGHLIGHT As New InputMethodHighlight(False, CONVERTED_TEXT)

		''' <summary>
		''' Constant for the default highlight for selected converted text.
		''' </summary>
		Public Shared ReadOnly SELECTED_CONVERTED_TEXT_HIGHLIGHT As New InputMethodHighlight(True, CONVERTED_TEXT)


		''' <summary>
		''' Constructs an input method highlight record.
		''' The variation is set to 0, the style to null. </summary>
		''' <param name="selected"> Whether the text range is selected </param>
		''' <param name="state"> The conversion state for the text range - RAW_TEXT or CONVERTED_TEXT </param>
		''' <seealso cref= InputMethodHighlight#RAW_TEXT </seealso>
		''' <seealso cref= InputMethodHighlight#CONVERTED_TEXT </seealso>
		''' <exception cref="IllegalArgumentException"> if a state other than RAW_TEXT or CONVERTED_TEXT is given </exception>
		Public Sub New(  selected As Boolean,   state As Integer)
			Me.New(selected, state, 0, Nothing)
		End Sub

		''' <summary>
		''' Constructs an input method highlight record.
		''' The style is set to null. </summary>
		''' <param name="selected"> Whether the text range is selected </param>
		''' <param name="state"> The conversion state for the text range - RAW_TEXT or CONVERTED_TEXT </param>
		''' <param name="variation"> The style variation for the text range </param>
		''' <seealso cref= InputMethodHighlight#RAW_TEXT </seealso>
		''' <seealso cref= InputMethodHighlight#CONVERTED_TEXT </seealso>
		''' <exception cref="IllegalArgumentException"> if a state other than RAW_TEXT or CONVERTED_TEXT is given </exception>
		Public Sub New(  selected As Boolean,   state As Integer,   variation As Integer)
			Me.New(selected, state, variation, Nothing)
		End Sub

		''' <summary>
		''' Constructs an input method highlight record.
		''' The style attributes map provided must be unmodifiable. </summary>
		''' <param name="selected"> whether the text range is selected </param>
		''' <param name="state"> the conversion state for the text range - RAW_TEXT or CONVERTED_TEXT </param>
		''' <param name="variation"> the variation for the text range </param>
		''' <param name="style"> the rendering style attributes for the text range, or null </param>
		''' <seealso cref= InputMethodHighlight#RAW_TEXT </seealso>
		''' <seealso cref= InputMethodHighlight#CONVERTED_TEXT </seealso>
		''' <exception cref="IllegalArgumentException"> if a state other than RAW_TEXT or CONVERTED_TEXT is given
		''' @since 1.3 </exception>
		Public Sub New(Of T1)(  selected As Boolean,   state As Integer,   variation As Integer,   style As IDictionary(Of T1))
			Me.selected = selected
			If Not(state = RAW_TEXT OrElse state = CONVERTED_TEXT) Then Throw New IllegalArgumentException("unknown input method highlight state")
			Me.state = state
			Me.variation = variation
			Me.style = style
		End Sub

		''' <summary>
		''' Returns whether the text range is selected.
		''' </summary>
		Public Overridable Property selected As Boolean
			Get
				Return selected
			End Get
		End Property

		''' <summary>
		''' Returns the conversion state of the text range. </summary>
		''' <returns> The conversion state for the text range - RAW_TEXT or CONVERTED_TEXT. </returns>
		''' <seealso cref= InputMethodHighlight#RAW_TEXT </seealso>
		''' <seealso cref= InputMethodHighlight#CONVERTED_TEXT </seealso>
		Public Overridable Property state As Integer
			Get
				Return state
			End Get
		End Property

		''' <summary>
		''' Returns the variation of the text range.
		''' </summary>
		Public Overridable Property variation As Integer
			Get
				Return variation
			End Get
		End Property

		''' <summary>
		''' Returns the rendering style attributes for the text range, or null.
		''' @since 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property style As IDictionary(Of java.awt.font.TextAttribute, ?)
			Get
				Return style
			End Get
		End Property

		Private selected As Boolean
		Private state As Integer
		Private variation As Integer
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private style As IDictionary(Of java.awt.font.TextAttribute, ?)

	End Class

End Namespace
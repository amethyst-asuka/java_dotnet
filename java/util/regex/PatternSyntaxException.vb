'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.regex



	''' <summary>
	''' Unchecked exception thrown to indicate a syntax error in a
	''' regular-expression pattern.
	''' 
	''' @author  unascribed
	''' @since 1.4
	''' @spec JSR-51
	''' </summary>

	Public Class PatternSyntaxException
		Inherits IllegalArgumentException

		Private Shadows Const serialVersionUID As Long = -3864639126226059218L

		Private ReadOnly desc As String
		Private ReadOnly pattern_Renamed As String
		Private ReadOnly index As Integer

		''' <summary>
		''' Constructs a new instance of this class.
		''' </summary>
		''' <param name="desc">
		'''         A description of the error
		''' </param>
		''' <param name="regex">
		'''         The erroneous pattern
		''' </param>
		''' <param name="index">
		'''         The approximate index in the pattern of the error,
		'''         or <tt>-1</tt> if the index is not known </param>
		Public Sub New(ByVal desc As String, ByVal regex As String, ByVal index As Integer)
			Me.desc = desc
			Me.pattern_Renamed = regex
			Me.index = index
		End Sub

		''' <summary>
		''' Retrieves the error index.
		''' </summary>
		''' <returns>  The approximate index in the pattern of the error,
		'''         or <tt>-1</tt> if the index is not known </returns>
		Public Overridable Property index As Integer
			Get
				Return index
			End Get
		End Property

		''' <summary>
		''' Retrieves the description of the error.
		''' </summary>
		''' <returns>  The description of the error </returns>
		Public Overridable Property description As String
			Get
				Return desc
			End Get
		End Property

		''' <summary>
		''' Retrieves the erroneous regular-expression pattern.
		''' </summary>
		''' <returns>  The erroneous pattern </returns>
		Public Overridable Property pattern As String
			Get
				Return pattern_Renamed
			End Get
		End Property

		Private Shared ReadOnly nl As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("line.separator"))

		''' <summary>
		''' Returns a multi-line string containing the description of the syntax
		''' error and its index, the erroneous regular-expression pattern, and a
		''' visual indication of the error index within the pattern.
		''' </summary>
		''' <returns>  The full detail message </returns>
		Public  Overrides ReadOnly Property  message As String
			Get
				Dim sb As New StringBuffer
				sb.append(desc)
				If index >= 0 Then
					sb.append(" near index ")
					sb.append(index)
				End If
				sb.append(nl)
				sb.append(pattern_Renamed)
				If index >= 0 Then
					sb.append(nl)
					For i As Integer = 0 To index - 1
						sb.append(" "c)
					Next i
					sb.append("^"c)
				End If
				Return sb.ToString()
			End Get
		End Property

	End Class

End Namespace
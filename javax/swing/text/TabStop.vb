Imports System

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
	''' This class encapsulates a single tab stop (basically as tab stops
	''' are thought of by RTF). A tab stop is at a specified distance from the
	''' left margin, aligns text in a specified way, and has a specified leader.
	''' TabStops are immutable, and usually contained in TabSets.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' </summary>
	<Serializable> _
	Public Class TabStop

		''' <summary>
		''' Character following tab is positioned at location. </summary>
		Public Const ALIGN_LEFT As Integer = 0
		''' <summary>
		''' Characters following tab are positioned such that all following
		''' characters up to next tab/newline end at location. 
		''' </summary>
		Public Const ALIGN_RIGHT As Integer = 1
		''' <summary>
		''' Characters following tab are positioned such that all following
		''' characters up to next tab/newline are centered around the tabs
		''' location. 
		''' </summary>
		Public Const ALIGN_CENTER As Integer = 2
		''' <summary>
		''' Characters following tab are aligned such that next
		''' decimal/tab/newline is at the tab location, very similar to
		''' RIGHT_TAB, just includes decimal as additional character to look for.
		''' </summary>
		Public Const ALIGN_DECIMAL As Integer = 4
		Public Const ALIGN_BAR As Integer = 5

	'     Bar tabs (whatever they are) are actually a separate kind of tab
	'       in the RTF spec. However, being a bar tab and having alignment
	'       properties are mutually exclusive, so the reader treats barness
	'       as being a kind of alignment. 

		Public Const LEAD_NONE As Integer = 0
		Public Const LEAD_DOTS As Integer = 1
		Public Const LEAD_HYPHENS As Integer = 2
		Public Const LEAD_UNDERLINE As Integer = 3
		Public Const LEAD_THICKLINE As Integer = 4
		Public Const LEAD_EQUALS As Integer = 5

		''' <summary>
		''' Tab type. </summary>
		Private alignment As Integer
		''' <summary>
		''' Location, from the left margin, that tab is at. </summary>
		Private position As Single
		Private leader As Integer

		''' <summary>
		''' Creates a tab at position <code>pos</code> with a default alignment
		''' and default leader.
		''' </summary>
		Public Sub New(ByVal pos As Single)
			Me.New(pos, ALIGN_LEFT, LEAD_NONE)
		End Sub

		''' <summary>
		''' Creates a tab with the specified position <code>pos</code>,
		''' alignment <code>align</code> and leader <code>leader</code>.
		''' </summary>
		Public Sub New(ByVal pos As Single, ByVal align As Integer, ByVal leader As Integer)
			alignment = align
			Me.leader = leader
			position = pos
		End Sub

		''' <summary>
		''' Returns the position, as a float, of the tab. </summary>
		''' <returns> the position of the tab </returns>
		Public Overridable Property position As Single
			Get
				Return position
			End Get
		End Property

		''' <summary>
		''' Returns the alignment, as an integer, of the tab. </summary>
		''' <returns> the alignment of the tab </returns>
		Public Overridable Property alignment As Integer
			Get
				Return alignment
			End Get
		End Property

		''' <summary>
		''' Returns the leader of the tab. </summary>
		''' <returns> the leader of the tab </returns>
		Public Overridable Property leader As Integer
			Get
				Return leader
			End Get
		End Property

		''' <summary>
		''' Returns true if the tabs are equal. </summary>
		''' <returns> true if the tabs are equal, otherwise false </returns>
		Public Overrides Function Equals(ByVal other As Object) As Boolean
			If other Is Me Then Return True
			If TypeOf other Is TabStop Then
				Dim o As TabStop = CType(other, TabStop)
				Return ((alignment = o.alignment) AndAlso (leader = o.leader) AndAlso (position = o.position)) ' TODO: epsilon
			End If
			Return False
		End Function

		''' <summary>
		''' Returns the hashCode for the object.  This must be defined
		''' here to ensure 100% pure.
		''' </summary>
		''' <returns> the hashCode for the object </returns>
		Public Overrides Function GetHashCode() As Integer
			Return alignment Xor leader Xor Math.Round(position)
		End Function

		' This is for debugging; perhaps it should be removed before release 
		Public Overrides Function ToString() As String
			Dim buf As String
			Select Case alignment
			  Case Else
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			  Case ALIGN_LEFT
				buf = ""
			  Case ALIGN_RIGHT
				buf = "right "
			  Case ALIGN_CENTER
				buf = "center "
			  Case ALIGN_DECIMAL
				buf = "decimal "
			  Case ALIGN_BAR
				buf = "bar "
			End Select
			buf = buf & "tab @" & Convert.ToString(position)
			If leader <> LEAD_NONE Then buf = buf & " (w/leaders)"
			Return buf
		End Function
	End Class

End Namespace
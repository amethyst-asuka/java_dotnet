Imports javax.swing

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

Namespace javax.swing.event


	''' <summary>
	''' An event that characterizes a change in selection. The change is limited to a
	''' a single inclusive interval. The selection of at least one index within the
	''' range will have changed. A decent {@code ListSelectionModel} implementation
	''' will keep the range as small as possible. {@code ListSelectionListeners} will
	''' generally query the source of the event for the new selected status of each
	''' potentially changed row.
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
	''' @author Hans Muller
	''' @author Ray Ryan </summary>
	''' <seealso cref= ListSelectionModel </seealso>
	Public Class ListSelectionEvent
		Inherits java.util.EventObject

		Private firstIndex As Integer
		Private lastIndex As Integer
		Private isAdjusting As Boolean

		''' <summary>
		''' Represents a change in selection status between {@code firstIndex} and
		''' {@code lastIndex}, inclusive. {@code firstIndex} is less than or equal to
		''' {@code lastIndex}. The selection of at least one index within the range will
		''' have changed.
		''' </summary>
		''' <param name="firstIndex"> the first index in the range, &lt;= lastIndex </param>
		''' <param name="lastIndex"> the last index in the range, &gt;= firstIndex </param>
		''' <param name="isAdjusting"> whether or not this is one in a series of
		'''        multiple events, where changes are still being made </param>
		Public Sub New(ByVal source As Object, ByVal firstIndex As Integer, ByVal lastIndex As Integer, ByVal isAdjusting As Boolean)
			MyBase.New(source)
			Me.firstIndex = firstIndex
			Me.lastIndex = lastIndex
			Me.isAdjusting = isAdjusting
		End Sub

		''' <summary>
		''' Returns the index of the first row whose selection may have changed.
		''' {@code getFirstIndex() &lt;= getLastIndex()}
		''' </summary>
		''' <returns> the first row whose selection value may have changed,
		'''         where zero is the first row </returns>
		Public Overridable Property firstIndex As Integer
			Get
				Return firstIndex
			End Get
		End Property

		''' <summary>
		''' Returns the index of the last row whose selection may have changed.
		''' {@code getLastIndex() &gt;= getFirstIndex()}
		''' </summary>
		''' <returns> the last row whose selection value may have changed,
		'''         where zero is the first row </returns>
		Public Overridable Property lastIndex As Integer
			Get
				Return lastIndex
			End Get
		End Property

		''' <summary>
		''' Returns whether or not this is one in a series of multiple events,
		''' where changes are still being made. See the documentation for
		''' <seealso cref="javax.swing.ListSelectionModel#setValueIsAdjusting"/> for
		''' more details on how this is used.
		''' </summary>
		''' <returns> {@code true} if this is one in a series of multiple events,
		'''         where changes are still being made </returns>
		Public Overridable Property valueIsAdjusting As Boolean
			Get
				Return isAdjusting
			End Get
		End Property

		''' <summary>
		''' Returns a {@code String} that displays and identifies this
		''' object's properties.
		''' </summary>
		''' <returns> a String representation of this object </returns>
		Public Overrides Function ToString() As String
			Dim properties As String = " source=" & source & " firstIndex= " & firstIndex & " lastIndex= " & lastIndex & " isAdjusting= " & isAdjusting & " "
			Return Me.GetType().name & "[" & properties & "]"
		End Function
	End Class

End Namespace
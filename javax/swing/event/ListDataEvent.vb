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

Namespace javax.swing.event



	''' <summary>
	''' Defines an event that encapsulates changes to a list.
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
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class ListDataEvent
		Inherits java.util.EventObject

		''' <summary>
		''' Identifies one or more changes in the lists contents. </summary>
		Public Const CONTENTS_CHANGED As Integer = 0
		''' <summary>
		''' Identifies the addition of one or more contiguous items to the list </summary>
		Public Const INTERVAL_ADDED As Integer = 1
		''' <summary>
		''' Identifies the removal of one or more contiguous items from the list </summary>
		Public Const INTERVAL_REMOVED As Integer = 2

		Private type As Integer
		Private index0 As Integer
		Private index1 As Integer

		''' <summary>
		''' Returns the event type. The possible values are:
		''' <ul>
		''' <li> <seealso cref="#CONTENTS_CHANGED"/>
		''' <li> <seealso cref="#INTERVAL_ADDED"/>
		''' <li> <seealso cref="#INTERVAL_REMOVED"/>
		''' </ul>
		''' </summary>
		''' <returns> an int representing the type value </returns>
		Public Overridable Property type As Integer
			Get
				Return type
			End Get
		End Property

		''' <summary>
		''' Returns the lower index of the range. For a single
		''' element, this value is the same as that returned by <seealso cref="#getIndex1"/>.
		''' 
		''' </summary>
		''' <returns> an int representing the lower index value </returns>
		Public Overridable Property index0 As Integer
			Get
				Return index0
			End Get
		End Property
		''' <summary>
		''' Returns the upper index of the range. For a single
		''' element, this value is the same as that returned by <seealso cref="#getIndex0"/>.
		''' </summary>
		''' <returns> an int representing the upper index value </returns>
		Public Overridable Property index1 As Integer
			Get
				Return index1
			End Get
		End Property

		''' <summary>
		''' Constructs a ListDataEvent object. If index0 is &gt;
		''' index1, index0 and index1 will be swapped such that
		''' index0 will always be &lt;= index1.
		''' </summary>
		''' <param name="source">  the source Object (typically <code>this</code>) </param>
		''' <param name="type">    an int specifying <seealso cref="#CONTENTS_CHANGED"/>,
		'''                <seealso cref="#INTERVAL_ADDED"/>, or <seealso cref="#INTERVAL_REMOVED"/> </param>
		''' <param name="index0">  one end of the new interval </param>
		''' <param name="index1">  the other end of the new interval </param>
		Public Sub New(ByVal source As Object, ByVal type As Integer, ByVal index0 As Integer, ByVal index1 As Integer)
			MyBase.New(source)
			Me.type = type
			Me.index0 = Math.Min(index0, index1)
			Me.index1 = Math.Max(index0, index1)
		End Sub

		''' <summary>
		''' Returns a string representation of this ListDataEvent. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' 
		''' @since 1.4 </summary>
		''' <returns>  a string representation of this ListDataEvent. </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[type=" & type & ",index0=" & index0 & ",index1=" & index1 & "]"
		End Function
	End Class

End Namespace
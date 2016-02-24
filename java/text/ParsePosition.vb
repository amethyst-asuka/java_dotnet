'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

'
' * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
' *
' *   The original version of this source code and documentation is copyrighted
' * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
' * materials are provided under terms of a License Agreement between Taligent
' * and Sun. This technology is protected by multiple US and International
' * patents. This notice and attribution to Taligent may not be removed.
' *   Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.text


	''' <summary>
	''' <code>ParsePosition</code> is a simple class used by <code>Format</code>
	''' and its subclasses to keep track of the current position during parsing.
	''' The <code>parseObject</code> method in the various <code>Format</code>
	''' classes requires a <code>ParsePosition</code> object as an argument.
	''' 
	''' <p>
	''' By design, as you parse through a string with different formats,
	''' you can use the same <code>ParsePosition</code>, since the index parameter
	''' records the current position.
	''' 
	''' @author      Mark Davis </summary>
	''' <seealso cref=         java.text.Format </seealso>

	Public Class ParsePosition

		''' <summary>
		''' Input: the place you start parsing.
		''' <br>Output: position where the parse stopped.
		''' This is designed to be used serially,
		''' with each call setting index up for the next one.
		''' </summary>
		Friend index As Integer = 0
		Friend errorIndex As Integer = -1

		''' <summary>
		''' Retrieve the current parse position.  On input to a parse method, this
		''' is the index of the character at which parsing will begin; on output, it
		''' is the index of the character following the last character parsed.
		''' </summary>
		''' <returns> the current parse position </returns>
		Public Overridable Property index As Integer
			Get
				Return index
			End Get
			Set(ByVal index As Integer)
				Me.index = index
			End Set
		End Property


		''' <summary>
		''' Create a new ParsePosition with the given initial index.
		''' </summary>
		''' <param name="index"> initial index </param>
		Public Sub New(ByVal index As Integer)
			Me.index = index
		End Sub
		''' <summary>
		''' Set the index at which a parse error occurred.  Formatters
		''' should set this before returning an error code from their
		''' parseObject method.  The default value is -1 if this is not set.
		''' </summary>
		''' <param name="ei"> the index at which an error occurred
		''' @since 1.2 </param>
		Public Overridable Property errorIndex As Integer
			Set(ByVal ei As Integer)
				errorIndex = ei
			End Set
			Get
				Return errorIndex
			End Get
		End Property


		''' <summary>
		''' Overrides equals
		''' </summary>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Nothing Then Return False
			If Not(TypeOf obj Is ParsePosition) Then Return False
			Dim other As ParsePosition = CType(obj, ParsePosition)
			Return (index = other.index AndAlso errorIndex = other.errorIndex)
		End Function

		''' <summary>
		''' Returns a hash code for this ParsePosition. </summary>
		''' <returns> a hash code value for this object </returns>
		Public Overrides Function GetHashCode() As Integer
			Return (errorIndex << 16) Or index
		End Function

		''' <summary>
		''' Return a string representation of this ParsePosition. </summary>
		''' <returns>  a string representation of this object </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[index=" & index & ",errorIndex=" & errorIndex + AscW("]"c)
		End Function
	End Class

End Namespace
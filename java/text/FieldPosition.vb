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
' * (C) Copyright Taligent, Inc. 1996 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - All Rights Reserved
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
	''' <code>FieldPosition</code> is a simple class used by <code>Format</code>
	''' and its subclasses to identify fields in formatted output. Fields can
	''' be identified in two ways:
	''' <ul>
	'''  <li>By an integer constant, whose names typically end with
	'''      <code>_FIELD</code>. The constants are defined in the various
	'''      subclasses of <code>Format</code>.
	'''  <li>By a <code>Format.Field</code> constant, see <code>ERA_FIELD</code>
	'''      and its friends in <code>DateFormat</code> for an example.
	''' </ul>
	''' <p>
	''' <code>FieldPosition</code> keeps track of the position of the
	''' field within the formatted output with two indices: the index
	''' of the first character of the field and the index of the last
	''' character of the field.
	''' 
	''' <p>
	''' One version of the <code>format</code> method in the various
	''' <code>Format</code> classes requires a <code>FieldPosition</code>
	''' object as an argument. You use this <code>format</code> method
	''' to perform partial formatting or to get information about the
	''' formatted output (such as the position of a field).
	''' 
	''' <p>
	''' If you are interested in the positions of all attributes in the
	''' formatted string use the <code>Format</code> method
	''' <code>formatToCharacterIterator</code>.
	''' 
	''' @author      Mark Davis </summary>
	''' <seealso cref=         java.text.Format </seealso>
	Public Class FieldPosition

		''' <summary>
		''' Input: Desired field to determine start and end offsets for.
		''' The meaning depends on the subclass of Format.
		''' </summary>
		Friend field As Integer = 0

		''' <summary>
		''' Output: End offset of field in text.
		''' If the field does not occur in the text, 0 is returned.
		''' </summary>
		Friend endIndex As Integer = 0

		''' <summary>
		''' Output: Start offset of field in text.
		''' If the field does not occur in the text, 0 is returned.
		''' </summary>
		Friend beginIndex As Integer = 0

		''' <summary>
		''' Desired field this FieldPosition is for.
		''' </summary>
		Private attribute As Format.Field

		''' <summary>
		''' Creates a FieldPosition object for the given field.  Fields are
		''' identified by constants, whose names typically end with _FIELD,
		''' in the various subclasses of Format.
		''' </summary>
		''' <param name="field"> the field identifier </param>
		''' <seealso cref= java.text.NumberFormat#INTEGER_FIELD </seealso>
		''' <seealso cref= java.text.NumberFormat#FRACTION_FIELD </seealso>
		''' <seealso cref= java.text.DateFormat#YEAR_FIELD </seealso>
		''' <seealso cref= java.text.DateFormat#MONTH_FIELD </seealso>
		Public Sub New(  field As Integer)
			Me.field = field
		End Sub

		''' <summary>
		''' Creates a FieldPosition object for the given field constant. Fields are
		''' identified by constants defined in the various <code>Format</code>
		''' subclasses. This is equivalent to calling
		''' <code>new FieldPosition(attribute, -1)</code>.
		''' </summary>
		''' <param name="attribute"> Format.Field constant identifying a field
		''' @since 1.4 </param>
		Public Sub New(  attribute As Format.Field)
			Me.New(attribute, -1)
		End Sub

		''' <summary>
		''' Creates a <code>FieldPosition</code> object for the given field.
		''' The field is identified by an attribute constant from one of the
		''' <code>Field</code> subclasses as well as an integer field ID
		''' defined by the <code>Format</code> subclasses. <code>Format</code>
		''' subclasses that are aware of <code>Field</code> should give precedence
		''' to <code>attribute</code> and ignore <code>fieldID</code> if
		''' <code>attribute</code> is not null. However, older <code>Format</code>
		''' subclasses may not be aware of <code>Field</code> and rely on
		''' <code>fieldID</code>. If the field has no corresponding integer
		''' constant, <code>fieldID</code> should be -1.
		''' </summary>
		''' <param name="attribute"> Format.Field constant identifying a field </param>
		''' <param name="fieldID"> integer constant identifying a field
		''' @since 1.4 </param>
		Public Sub New(  attribute As Format.Field,   fieldID As Integer)
			Me.attribute = attribute
			Me.field = fieldID
		End Sub

		''' <summary>
		''' Returns the field identifier as an attribute constant
		''' from one of the <code>Field</code> subclasses. May return null if
		''' the field is specified only by an integer field ID.
		''' </summary>
		''' <returns> Identifier for the field
		''' @since 1.4 </returns>
		Public Overridable Property fieldAttribute As Format.Field
			Get
				Return attribute
			End Get
		End Property

		''' <summary>
		''' Retrieves the field identifier.
		''' </summary>
		''' <returns> the field identifier </returns>
		Public Overridable Property field As Integer
			Get
				Return field
			End Get
		End Property

		''' <summary>
		''' Retrieves the index of the first character in the requested field.
		''' </summary>
		''' <returns> the begin index </returns>
		Public Overridable Property beginIndex As Integer
			Get
				Return beginIndex
			End Get
			Set(  bi As Integer)
				beginIndex = bi
			End Set
		End Property

		''' <summary>
		''' Retrieves the index of the character following the last character in the
		''' requested field.
		''' </summary>
		''' <returns> the end index </returns>
		Public Overridable Property endIndex As Integer
			Get
				Return endIndex
			End Get
			Set(  ei As Integer)
				endIndex = ei
			End Set
		End Property



		''' <summary>
		''' Returns a <code>Format.FieldDelegate</code> instance that is associated
		''' with the FieldPosition. When the delegate is notified of the same
		''' field the FieldPosition is associated with, the begin/end will be
		''' adjusted.
		''' </summary>
		Friend Overridable Property fieldDelegate As Format.FieldDelegate
			Get
				Return New [Delegate](Me)
			End Get
		End Property

		''' <summary>
		''' Overrides equals
		''' </summary>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If obj Is Nothing Then Return False
			If Not(TypeOf obj Is FieldPosition) Then Return False
			Dim other As FieldPosition = CType(obj, FieldPosition)
			If attribute Is Nothing Then
				If other.attribute IsNot Nothing Then Return False
			ElseIf Not attribute.Equals(other.attribute) Then
				Return False
			End If
			Return (beginIndex = other.beginIndex AndAlso endIndex = other.endIndex AndAlso field = other.field)
		End Function

		''' <summary>
		''' Returns a hash code for this FieldPosition. </summary>
		''' <returns> a hash code value for this object </returns>
		Public Overrides Function GetHashCode() As Integer
			Return (field << 24) Or (beginIndex << 16) Or endIndex
		End Function

		''' <summary>
		''' Return a string representation of this FieldPosition. </summary>
		''' <returns>  a string representation of this object </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[field=" & field & ",attribute=" & attribute & ",beginIndex=" & beginIndex & ",endIndex=" & endIndex + AscW("]"c)
		End Function


		''' <summary>
		''' Return true if the receiver wants a <code>Format.Field</code> value and
		''' <code>attribute</code> is equal to it.
		''' </summary>
		Private Function matchesField(  attribute As Format.Field) As Boolean
			If Me.attribute IsNot Nothing Then Return Me.attribute.Equals(attribute)
			Return False
		End Function

		''' <summary>
		''' Return true if the receiver wants a <code>Format.Field</code> value and
		''' <code>attribute</code> is equal to it, or true if the receiver
		''' represents an inteter constant and <code>field</code> equals it.
		''' </summary>
		Private Function matchesField(  attribute As Format.Field,   field As Integer) As Boolean
			If Me.attribute IsNot Nothing Then Return Me.attribute.Equals(attribute)
			Return (field = Me.field)
		End Function


		''' <summary>
		''' An implementation of FieldDelegate that will adjust the begin/end
		''' of the FieldPosition if the arguments match the field of
		''' the FieldPosition.
		''' </summary>
		Private Class [Delegate]
			Implements Format.FieldDelegate

			Private ReadOnly outerInstance As FieldPosition

			Public Sub New(  outerInstance As FieldPosition)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Indicates whether the field has been  encountered before. If this
			''' is true, and <code>formatted</code> is invoked, the begin/end
			''' are not updated.
			''' </summary>
			Private encounteredField As Boolean

			Public Overridable Sub formatted(  attr As Format.Field,   value As Object,   start As Integer,   [end] As Integer,   buffer As StringBuffer) Implements Format.FieldDelegate.formatted
				If (Not encounteredField) AndAlso outerInstance.matchesField(attr) Then
					outerInstance.beginIndex = start
					outerInstance.endIndex = [end]
					encounteredField = (start <> [end])
				End If
			End Sub

			Public Overridable Sub formatted(  fieldID As Integer,   attr As Format.Field,   value As Object,   start As Integer,   [end] As Integer,   buffer As StringBuffer) Implements Format.FieldDelegate.formatted
				If (Not encounteredField) AndAlso outerInstance.matchesField(attr, fieldID) Then
					outerInstance.beginIndex = start
					outerInstance.endIndex = [end]
					encounteredField = (start <> [end])
				End If
			End Sub
		End Class
	End Class

End Namespace
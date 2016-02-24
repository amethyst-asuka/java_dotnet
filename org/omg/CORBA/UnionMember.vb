'
' * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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
' * File: ./org/omg/CORBA/UnionMember.java
' * From: ./ir.idl
' * Date: Fri Aug 28 16:03:31 1998
' *   By: idltojava Java IDL 1.2 Aug 11 1998 02:00:18
' 

Namespace org.omg.CORBA

	''' <summary>
	''' A description in the Interface Repository of a member of an IDL union.
	''' </summary>
	Public NotInheritable Class UnionMember
		Implements org.omg.CORBA.portable.IDLEntity

		'  instance variables

		''' <summary>
		''' The name of the union member described by this
		''' <code>UnionMember</code> object.
		''' @serial
		''' </summary>
		Public name As String

		''' <summary>
		''' The label of the union member described by this
		''' <code>UnionMember</code> object.
		''' @serial
		''' </summary>
		Public label As org.omg.CORBA.Any

		''' <summary>
		''' The type of the union member described by this
		''' <code>UnionMember</code> object.
		''' @serial
		''' </summary>
		Public type As org.omg.CORBA.TypeCode

		''' <summary>
		''' The typedef that represents the IDL type of the union member described by this
		''' <code>UnionMember</code> object.
		''' @serial
		''' </summary>
		Public type_def As org.omg.CORBA.IDLType

		'  constructors

		''' <summary>
		''' Constructs a new <code>UnionMember</code> object with its fields initialized
		''' to null.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new <code>UnionMember</code> object with its fields initialized
		''' to the given values.
		''' </summary>
		''' <param name="__name"> a <code>String</code> object with the name of this
		'''        <code>UnionMember</code> object </param>
		''' <param name="__label"> an <code>Any</code> object with the label of this
		'''        <code>UnionMember</code> object </param>
		''' <param name="__type"> a <code>TypeCode</code> object describing the type of this
		'''        <code>UnionMember</code> object </param>
		''' <param name="__type_def"> an <code>IDLType</code> object that represents the
		'''        IDL type of this <code>UnionMember</code> object </param>
		Public Sub New(ByVal __name As String, ByVal __label As org.omg.CORBA.Any, ByVal __type As org.omg.CORBA.TypeCode, ByVal __type_def As org.omg.CORBA.IDLType)
			name = __name
			label = __label
			type = __type
			type_def = __type_def
		End Sub
	End Class

End Namespace
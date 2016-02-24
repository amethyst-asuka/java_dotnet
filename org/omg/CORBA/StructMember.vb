'
' * Copyright (c) 1997, 2001, Oracle and/or its affiliates. All rights reserved.
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
' * File: ./org/omg/CORBA/StructMember.java
' * From: ./ir.idl
' * Date: Fri Aug 28 16:03:31 1998
' *   By: idltojava Java IDL 1.2 Aug 11 1998 02:00:18
' 

Namespace org.omg.CORBA

	''' <summary>
	''' Describes a member of an IDL <code>struct</code> in the
	''' Interface Repository, including
	''' the  name of the <code>struct</code> member, the type of
	''' the <code>struct</code> member, and
	''' the typedef that represents the IDL type of the
	''' <code>struct</code> member
	''' described the <code>struct</code> member object.
	''' </summary>
	Public NotInheritable Class StructMember
		Implements org.omg.CORBA.portable.IDLEntity

		'  instance variables

		''' <summary>
		''' The name of the struct member described by
		''' this <code>StructMember</code> object.
		''' @serial
		''' </summary>
		Public name As String

		''' <summary>
		''' The type of the struct member described by
		''' this <code>StructMember</code> object.
		''' @serial
		''' </summary>
		Public type As org.omg.CORBA.TypeCode

		''' <summary>
		''' The typedef that represents the IDL type of the struct member described by
		''' this <code>StructMember</code> object.
		''' @serial
		''' </summary>
		Public type_def As org.omg.CORBA.IDLType
		'  constructors

		''' <summary>
		''' Constructs a default <code>StructMember</code> object.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a <code>StructMember</code> object initialized with the
		''' given values. </summary>
		''' <param name="__name"> a <code>String</code> object with the name of the struct
		'''        member </param>
		''' <param name="__type"> a <code>TypeCode</code> object describing the type of the struct
		'''        member </param>
		''' <param name="__type_def"> an <code>IDLType</code> object representing the IDL type
		'''        of the struct member </param>
		Public Sub New(ByVal __name As String, ByVal __type As org.omg.CORBA.TypeCode, ByVal __type_def As org.omg.CORBA.IDLType)
			name = __name
			type = __type
			type_def = __type_def
		End Sub
	End Class

End Namespace
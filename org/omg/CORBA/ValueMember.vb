'
' * Copyright (c) 1998, 2000, Oracle and/or its affiliates. All rights reserved.
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
' * File: ./org/omg/CORBA/ValueMember.java
' * From: ./ir.idl
' * Date: Fri Aug 28 16:03:31 1998
' *   By: idltojava Java IDL 1.2 Aug 11 1998 02:00:18
' 

Namespace org.omg.CORBA

	''' <summary>
	''' A description in the Interface Repository of
	''' a member of a <code>value</code> object.
	''' </summary>
	Public NotInheritable Class ValueMember
		Implements org.omg.CORBA.portable.IDLEntity

		'  instance variables

		''' <summary>
		''' The name of the <code>value</code> member described by this
		''' <code>ValueMember</code> object.
		''' @serial
		''' </summary>
		Public name As String

		''' <summary>
		''' The repository ID of the <code>value</code> member described by
		''' this <code>ValueMember</code> object;
		''' @serial
		''' </summary>
		Public id As String

		''' <summary>
		''' The repository ID of the <code>value</code> in which this member
		''' is defined.
		''' @serial
		''' </summary>
		Public defined_in As String

		''' <summary>
		''' The version of the <code>value</code> in which this member is defined.
		''' @serial
		''' </summary>
		Public version As String

		''' <summary>
		''' The type of of this <code>value</code> member.
		''' @serial
		''' </summary>
		Public type As org.omg.CORBA.TypeCode

		''' <summary>
		''' The typedef that represents the IDL type of the <code>value</code>
		''' member described by this <code>ValueMember</code> object.
		''' @serial
		''' </summary>
		Public type_def As org.omg.CORBA.IDLType

		''' <summary>
		''' The type of access (public, private) for the <code>value</code>
		''' member described by this <code>ValueMember</code> object.
		''' @serial
		''' </summary>
		Public access As Short
		'  constructors

		''' <summary>
		''' Constructs a default <code>ValueMember</code> object.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a <code>ValueMember</code> object initialized with
		''' the given values.
		''' </summary>
		''' <param name="__name"> The name of the <code>value</code> member described by this
		''' <code>ValueMember</code> object. </param>
		''' <param name="__id"> The repository ID of the <code>value</code> member described by
		''' this <code>ValueMember</code> object; </param>
		''' <param name="__defined_in"> The repository ID of the <code>value</code> in which this member
		''' is defined. </param>
		''' <param name="__version"> The version of the <code>value</code> in which this member is defined. </param>
		''' <param name="__type"> The type of of this <code>value</code> member. </param>
		''' <param name="__type_def"> The typedef that represents the IDL type of the <code>value</code>
		''' member described by this <code>ValueMember</code> object. </param>
		''' <param name="__access"> The type of access (public, private) for the <code>value</code>
		''' member described by this <code>ValueMember</code> object. </param>
		Public Sub New(ByVal __name As String, ByVal __id As String, ByVal __defined_in As String, ByVal __version As String, ByVal __type As org.omg.CORBA.TypeCode, ByVal __type_def As org.omg.CORBA.IDLType, ByVal __access As Short)
			name = __name
			id = __id
			defined_in = __defined_in
			version = __version
			type = __type
			type_def = __type_def
			access = __access
		End Sub
	End Class

End Namespace
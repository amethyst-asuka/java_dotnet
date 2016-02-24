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
' * File: ./org/omg/CORBA/DefinitionKind.java
' * From: ./ir.idl
' * Date: Fri Aug 28 16:03:31 1998
' *   By: idltojava Java IDL 1.2 Aug 11 1998 02:00:18
' 

Namespace org.omg.CORBA

	''' <summary>
	''' The class that provides the constants used to identify the type of an
	''' Interface Repository object.  This class contains two kinds of constants,
	''' those that are an <code>int</code> and those that are an instance of the class
	''' <code>DefinitionKind</code>.  This class provides the method
	''' <code>from_int</code>, which given one
	''' of the <code>int</code> constants, creates the corresponding
	''' <code>DefinitionKind</code> instance.  It also provides the method
	''' <code>value</code>, which returns the <code>int</code> constant that
	''' is the value for a <code>DefinitionKind</code> instance.
	''' </summary>
	''' <seealso cref= IRObject </seealso>

	Public Class DefinitionKind
		Implements org.omg.CORBA.portable.IDLEntity

	''' <summary>
	''' The constant that indicates that an Interface Repository object
	''' does not have a definition kind.
	''' </summary>
			Public Const _dk_none As Integer = 0, _dk_all As Integer = 1, _dk_Attribute As Integer = 2, _dk_Constant As Integer = 3, _dk_Exception As Integer = 4, _dk_Interface As Integer = 5, _dk_Module As Integer = 6, _dk_Operation As Integer = 7, _dk_Typedef As Integer = 8, _dk_Alias As Integer = 9, _dk_Struct As Integer = 10, _dk_Union As Integer = 11, _dk_Enum As Integer = 12, _dk_Primitive As Integer = 13, _dk_String As Integer = 14, _dk_Sequence As Integer = 15, _dk_Array As Integer = 16, _dk_Repository As Integer = 17, _dk_Wstring As Integer = 18, _dk_Fixed As Integer = 19, _dk_Value As Integer = 20, _dk_ValueBox As Integer = 21, _dk_ValueMember As Integer = 22, _dk_Native As Integer = 23, _dk_AbstractInterface As Integer = 24
	''' <summary>
	''' The constant that indicates that the type of an Interface Repository object
	''' may be any type.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is an
	''' attribute.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' constant.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is an
	''' exception.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is an
	''' interface.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' module.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is an
	''' operation.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' Typedef.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is an
	''' Alias.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' Struct.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' Union.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is an
	''' Enum.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' Primitive.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' String.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' Sequence.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is an
	''' Array.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' Repository.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' Wstring.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is of type
	''' Fixed.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' Value.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' ValueBox.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is a
	''' ValueMember.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object is of type
	''' Native.
	''' </summary>
	''' <summary>
	''' The constant that indicates that an Interface Repository object
	''' is representing an abstract interface.
	''' </summary>

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object has no definition kind.
	''' </summary>

		Public Shared ReadOnly dk_none As New DefinitionKind(_dk_none)

		 ''' <summary>
		 ''' The wildcard <code>DefinitionKind</code> constant, useful
		 ''' in all occasions where any
		 ''' <code>DefinitionKind</code> is appropriate. The Container's
		 ''' <code>contents</code> method
		 ''' makes use of this constant to return all contained definitions of any kind.
		 ''' </summary>

		Public Shared ReadOnly dk_all As New DefinitionKind(_dk_all)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is an Attribute.
	''' </summary>

		Public Shared ReadOnly dk_Attribute As New DefinitionKind(_dk_Attribute)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a constant.
	''' </summary>

		Public Shared ReadOnly dk_Constant As New DefinitionKind(_dk_Constant)


	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is an Exception.
	''' </summary>

		Public Shared ReadOnly dk_Exception As New DefinitionKind(_dk_Exception)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is an Interface.
	''' </summary>

		Public Shared ReadOnly dk_Interface As New DefinitionKind(_dk_Interface)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a Module.
	''' </summary>

		Public Shared ReadOnly dk_Module As New DefinitionKind(_dk_Module)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is an Operation.
	''' </summary>

		Public Shared ReadOnly dk_Operation As New DefinitionKind(_dk_Operation)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a Typedef.
	''' </summary>

		Public Shared ReadOnly dk_Typedef As New DefinitionKind(_dk_Typedef)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is an Alias.
	''' </summary>

		Public Shared ReadOnly dk_Alias As New DefinitionKind(_dk_Alias)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a Struct.
	''' </summary>

		Public Shared ReadOnly dk_Struct As New DefinitionKind(_dk_Struct)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a Union.
	''' </summary>

		Public Shared ReadOnly dk_Union As New DefinitionKind(_dk_Union)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is an Enum.
	''' </summary>

		Public Shared ReadOnly dk_Enum As New DefinitionKind(_dk_Enum)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a Primitive.
	''' </summary>

		Public Shared ReadOnly dk_Primitive As New DefinitionKind(_dk_Primitive)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a String.
	''' </summary>

		Public Shared ReadOnly dk_String As New DefinitionKind(_dk_String)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a Sequence.
	''' </summary>

		Public Shared ReadOnly dk_Sequence As New DefinitionKind(_dk_Sequence)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is an Array.
	''' </summary>

		Public Shared ReadOnly dk_Array As New DefinitionKind(_dk_Array)


	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a Repository.
	''' </summary>

		Public Shared ReadOnly dk_Repository As New DefinitionKind(_dk_Repository)


	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a Wstring.
	''' </summary>

		Public Shared ReadOnly dk_Wstring As New DefinitionKind(_dk_Wstring)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a Fixed value.
	''' </summary>

		Public Shared ReadOnly dk_Fixed As New DefinitionKind(_dk_Fixed)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a Value.
	''' </summary>

		Public Shared ReadOnly dk_Value As New DefinitionKind(_dk_Value)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a ValueBox.
	''' </summary>

		Public Shared ReadOnly dk_ValueBox As New DefinitionKind(_dk_ValueBox)

	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a ValueMember.
	''' </summary>

		Public Shared ReadOnly dk_ValueMember As New DefinitionKind(_dk_ValueMember)


	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object is a Native value.
	''' </summary>

		Public Shared ReadOnly dk_Native As New DefinitionKind(_dk_Native)


	''' <summary>
	''' The static instance of <code>DefinitionKind</code> indicating that an
	''' Interface Repository object represents an abstract interface.
	''' </summary>
		Public Shared ReadOnly dk_AbstractInterface As New DefinitionKind(_dk_AbstractInterface)


		 ''' <summary>
		 ''' Returns the <code>int</code> constant identifying the type of an IR object. </summary>
		 ''' <returns> the <code>int</code> constant from the class
		 ''' <code>DefinitionKind</code> that is the value of this
		 ''' <code>DefinitionKind</code> instance </returns>

		Public Overridable Function value() As Integer
			Return _value
		End Function


		 ''' <summary>
		 ''' Creates a <code>DefinitionKind</code> instance corresponding to the given code
		 ''' . </summary>
		 ''' <param name="i"> one of the <code>int</code> constants from the class
		 ''' <code>DefinitionKind</code> </param>
		 ''' <returns> the <code>DefinitionKind</code> instance corresponding
		 '''         to the given code </returns>
		 ''' <exception cref="org.omg.CORBA.BAD_PARAM"> if the given parameter is not
		 ''' one
		 '''         of the <code>int</code> constants from the class
		 '''         <code>DefinitionKind</code> </exception>

		Public Shared Function from_int(ByVal i As Integer) As DefinitionKind
			Select Case i
			Case _dk_none
				Return dk_none
			Case _dk_all
				Return dk_all
			Case _dk_Attribute
				Return dk_Attribute
			Case _dk_Constant
				Return dk_Constant
			Case _dk_Exception
				Return dk_Exception
			Case _dk_Interface
				Return dk_Interface
			Case _dk_Module
				Return dk_Module
			Case _dk_Operation
				Return dk_Operation
			Case _dk_Typedef
				Return dk_Typedef
			Case _dk_Alias
				Return dk_Alias
			Case _dk_Struct
				Return dk_Struct
			Case _dk_Union
				Return dk_Union
			Case _dk_Enum
				Return dk_Enum
			Case _dk_Primitive
				Return dk_Primitive
			Case _dk_String
				Return dk_String
			Case _dk_Sequence
				Return dk_Sequence
			Case _dk_Array
				Return dk_Array
			Case _dk_Repository
				Return dk_Repository
			Case _dk_Wstring
				Return dk_Wstring
			Case _dk_Fixed
				Return dk_Fixed
			Case _dk_Value
				Return dk_Value
			Case _dk_ValueBox
				Return dk_ValueBox
			Case _dk_ValueMember
				Return dk_ValueMember
			Case _dk_Native
				Return dk_Native
			Case Else
				Throw New org.omg.CORBA.BAD_PARAM
			End Select
		End Function

	   ''' <summary>
	   ''' Constructs a <code>DefinitionKind</code> object with its <code>_value</code>
	   ''' field initialized with the given value. </summary>
	   ''' <param name="_value"> one of the <code>int</code> constants defined in the
	   '''                   class <code>DefinitionKind</code> </param>

		Protected Friend Sub New(ByVal _value As Integer)
			Me._value = _value
		End Sub

		 ''' <summary>
		 ''' The field that holds a value for a <code>DefinitionKind</code> object.
		 ''' @serial
		 ''' </summary>

		Private _value As Integer
	End Class

End Namespace
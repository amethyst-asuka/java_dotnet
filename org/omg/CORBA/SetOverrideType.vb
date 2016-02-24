'
' * Copyright (c) 1998, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA

	''' <summary>
	''' The mapping of a CORBA <code>enum</code> tagging
	''' <code>SET_OVERRIDE</code> and <code>ADD_OVERRIDE</code>, which
	''' indicate whether policies should replace the
	''' existing policies of an <code>Object</code> or be added to them.
	''' <P>
	''' The method <seealso cref="org.omg.CORBA.Object#_set_policy_override"/> takes
	''' either <code>SetOverrideType.SET_OVERRIDE</code> or
	''' <code>SetOverrideType.ADD_OVERRIDE</code> as its second argument.
	''' The method <code>_set_policy_override</code>
	''' creates a new <code>Object</code> initialized with the
	''' <code>Policy</code> objects supplied as the first argument.  If the
	''' second argument is <code>ADD_OVERRIDE</code>, the new policies
	''' are added to those of the <code>Object</code> instance that is
	''' calling the <code>_set_policy_override</code> method.  If
	''' <code>SET_OVERRIDE</code> is given instead, the existing policies
	''' are replaced with the given ones.
	''' 
	''' @author OMG
	''' @since   JDK1.2
	''' </summary>

	Public Class SetOverrideType
		Implements org.omg.CORBA.portable.IDLEntity

		''' <summary>
		''' The <code>int</code> constant for the enum value SET_OVERRIDE.
		''' </summary>
		Public Const _SET_OVERRIDE As Integer = 0

		''' <summary>
		''' The <code>int</code> constant for the enum value ADD_OVERRIDE.
		''' </summary>
		Public Const _ADD_OVERRIDE As Integer = 1

		''' <summary>
		''' The <code>SetOverrideType</code> constant for the enum value SET_OVERRIDE.
		''' </summary>
		Public Shared ReadOnly SET_OVERRIDE As New SetOverrideType(_SET_OVERRIDE)

		''' <summary>
		''' The <code>SetOverrideType</code> constant for the enum value ADD_OVERRIDE.
		''' </summary>
		Public Shared ReadOnly ADD_OVERRIDE As New SetOverrideType(_ADD_OVERRIDE)

		''' <summary>
		''' Retrieves the value of this <code>SetOverrideType</code> instance.
		''' </summary>
		''' <returns>  the <code>int</code> for this <code>SetOverrideType</code> instance. </returns>
		Public Overridable Function value() As Integer
			Return _value
		End Function

		''' <summary>
		''' Converts the given <code>int</code> to the corresponding
		''' <code>SetOverrideType</code> instance.
		''' </summary>
		''' <param name="i"> the <code>int</code> to convert; must be either
		'''         <code>SetOverrideType._SET_OVERRIDE</code> or
		'''         <code>SetOverrideType._ADD_OVERRIDE</code> </param>
		''' <returns>  the <code>SetOverrideType</code> instance whose value
		'''       matches the given <code>int</code> </returns>
		''' <exception cref="BAD_PARAM">  if the given <code>int</code> does not
		'''       match the value of
		'''       any <code>SetOverrideType</code> instance </exception>
		Public Shared Function from_int(ByVal i As Integer) As SetOverrideType
			Select Case i
			Case _SET_OVERRIDE
				Return SET_OVERRIDE
			Case _ADD_OVERRIDE
				Return ADD_OVERRIDE
			Case Else
				Throw New org.omg.CORBA.BAD_PARAM
			End Select
		End Function

		''' <summary>
		''' Constructs a <code>SetOverrideType</code> instance from an
		''' <code>int</code>. </summary>
		''' <param name="_value"> must be either <code>SET_OVERRIDE</code> or
		'''        <code>ADD_OVERRIDE</code> </param>
		Protected Friend Sub New(ByVal _value As Integer)
			Me._value = _value
		End Sub

		''' <summary>
		''' The field containing the value for this <code>SetOverrideType</code>
		''' object.
		''' 
		''' </summary>
		Private _value As Integer
	End Class

End Namespace
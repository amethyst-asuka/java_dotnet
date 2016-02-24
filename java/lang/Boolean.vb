Imports System

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang

	''' <summary>
	''' The Boolean class wraps a value of the primitive type
	''' {@code boolean} in an object. An object of type
	''' {@code Boolean} contains a single field whose type is
	''' {@code boolean}.
	''' <p>
	''' In addition, this class provides many methods for
	''' converting a {@code boolean} to a {@code String} and a
	''' {@code String} to a {@code boolean}, as well as other
	''' constants and methods useful when dealing with a
	''' {@code boolean}.
	''' 
	''' @author  Arthur van Hoff
	''' @since   JDK1.0
	''' </summary>
	<Serializable> _
	Public NotInheritable Class Boolean?
		Implements Comparable(Of Boolean?)

		''' <summary>
		''' The {@code Boolean} object corresponding to the primitive
		''' value {@code true}.
		''' </summary>
		Public Shared ReadOnly [TRUE] As Boolean? = New Boolean?(True)

		''' <summary>
		''' The {@code Boolean} object corresponding to the primitive
		''' value {@code false}.
		''' </summary>
		Public Shared ReadOnly [FALSE] As Boolean? = New Boolean?(False)

        ''' <summary>
        ''' The Class object representing the primitive type boolean.
        ''' 
        ''' @since   JDK1.1
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Shared ReadOnly TYPE As [Class] = CType([Class].getPrimitiveClass("boolean"), [Class])

        ''' <summary>
        ''' The value of the Boolean.
        ''' 
        ''' @serial
        ''' </summary>
        Private ReadOnly value As Boolean

		''' <summary>
		''' use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		Private Const serialVersionUID As Long = -3665804199014368530L

        ''' <summary>
        ''' Allocates a {@code Boolean} object representing the
        ''' {@code value} argument.
        ''' 
        ''' <p><b>Note: It is rarely appropriate to use this constructor.
        ''' Unless a <i>new</i> instance is required, the static factory
        ''' <seealso cref="#valueOf(boolean)"/> is generally a better choice. It is
        ''' likely to yield significantly better space and time performance.</b>
        ''' </summary>
        ''' <param name="value">   the value of the {@code Boolean}. </param>
        Sub New(ByVal value As Boolean)
            Me.value = value
        End Sub

        ''' <summary>
        ''' Allocates a {@code Boolean} object representing the value
        ''' {@code true} if the string argument is not {@code null}
        ''' and is equal, ignoring case, to the string {@code "true"}.
        ''' Otherwise, allocate a {@code Boolean} object representing the
        ''' value {@code false}. Examples:<p>
        ''' {@code new Boolean("True")} produces a {@code Boolean} object
        ''' that represents {@code true}.<br>
        ''' {@code new Boolean("yes")} produces a {@code Boolean} object
        ''' that represents {@code false}.
        ''' </summary>
        ''' <param name="s">   the string to be converted to a {@code Boolean}. </param>
        Sub New(ByVal s As String)
            Me.New(parseBoolean(s))
        End Sub

        ''' <summary>
        ''' Parses the string argument as a boolean.  The {@code boolean}
        ''' returned represents the value {@code true} if the string argument
        ''' is not {@code null} and is equal, ignoring case, to the string
        ''' {@code "true"}. <p>
        ''' Example: {@code Boolean.parseBoolean("True")} returns {@code true}.<br>
        ''' Example: {@code Boolean.parseBoolean("yes")} returns {@code false}.
        ''' </summary>
        ''' <param name="s">   the {@code String} containing the boolean
        '''                 representation to be parsed </param>
        ''' <returns>     the boolean represented by the string argument
        ''' @since 1.5 </returns>
        Public Shared Function parseBoolean(ByVal s As String) As Boolean
			Return ((s IsNot Nothing) AndAlso s.equalsIgnoreCase("true"))
		End Function

		''' <summary>
		''' Returns the value of this {@code Boolean} object as a boolean
		''' primitive.
		''' </summary>
		''' <returns>  the primitive {@code boolean} value of this object. </returns>
		Public Function booleanValue() As Boolean
			Return value
		End Function

		''' <summary>
		''' Returns a {@code Boolean} instance representing the specified
		''' {@code boolean} value.  If the specified {@code boolean} value
		''' is {@code true}, this method returns {@code Boolean.TRUE};
		''' if it is {@code false}, this method returns {@code Boolean.FALSE}.
		''' If a new {@code Boolean} instance is not required, this method
		''' should generally be used in preference to the constructor
		''' <seealso cref="#Boolean(boolean)"/>, as this method is likely to yield
		''' significantly better space and time performance.
		''' </summary>
		''' <param name="b"> a boolean value. </param>
		''' <returns> a {@code Boolean} instance representing {@code b}.
		''' @since  1.4 </returns>
		Public Shared Function valueOf(ByVal b As Boolean) As Boolean?
			Return (If(b, [TRUE], [FALSE]))
		End Function

		''' <summary>
		''' Returns a {@code Boolean} with a value represented by the
		''' specified string.  The {@code Boolean} returned represents a
		''' true value if the string argument is not {@code null}
		''' and is equal, ignoring case, to the string {@code "true"}.
		''' </summary>
		''' <param name="s">   a string. </param>
		''' <returns>  the {@code Boolean} value represented by the string. </returns>
		Public Shared Function valueOf(ByVal s As String) As Boolean?
			Return If(parseBoolean(s), [TRUE], [FALSE])
		End Function

		''' <summary>
		''' Returns a {@code String} object representing the specified
		''' boolean.  If the specified boolean is {@code true}, then
		''' the string {@code "true"} will be returned, otherwise the
		''' string {@code "false"} will be returned.
		''' </summary>
		''' <param name="b"> the boolean to be converted </param>
		''' <returns> the string representation of the specified {@code boolean}
		''' @since 1.4 </returns>
		Public Shared Function ToString(ByVal b As Boolean) As String
			Return If(b, "true", "false")
		End Function

		''' <summary>
		''' Returns a {@code String} object representing this Boolean's
		''' value.  If this object represents the value {@code true},
		''' a string equal to {@code "true"} is returned. Otherwise, a
		''' string equal to {@code "false"} is returned.
		''' </summary>
		''' <returns>  a string representation of this object. </returns>
		Public Overrides Function ToString() As String
			Return If(value, "true", "false")
		End Function

		''' <summary>
		''' Returns a hash code for this {@code Boolean} object.
		''' </summary>
		''' <returns>  the integer {@code 1231} if this object represents
		''' {@code true}; returns the integer {@code 1237} if this
		''' object represents {@code false}. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return Boolean.hashCode(value)
		End Function

		''' <summary>
		''' Returns a hash code for a {@code boolean} value; compatible with
		''' {@code Boolean.hashCode()}.
		''' </summary>
		''' <param name="value"> the value to hash </param>
		''' <returns> a hash code value for a {@code boolean} value.
		''' @since 1.8 </returns>
		Public Shared Function GetHashCode(ByVal value As Boolean) As Integer
			Return If(value, 1231, 1237)
		End Function

	   ''' <summary>
	   ''' Returns {@code true} if and only if the argument is not
	   ''' {@code null} and is a {@code Boolean} object that
	   ''' represents the same {@code boolean} value as this object.
	   ''' </summary>
	   ''' <param name="obj">   the object to compare with. </param>
	   ''' <returns>  {@code true} if the Boolean objects represent the
	   '''          same value; {@code false} otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If TypeOf obj Is Boolean? Then Return value = CBool(obj)
			Return False
		End Function

		''' <summary>
		''' Returns {@code true} if and only if the system property
		''' named by the argument exists and is equal to the string
		''' {@code "true"}. (Beginning with version 1.0.2 of the
		''' Java<small><sup>TM</sup></small> platform, the test of
		''' this string is case insensitive.) A system property is accessible
		''' through {@code getProperty}, a method defined by the
		''' {@code System} class.
		''' <p>
		''' If there is no property with the specified name, or if the specified
		''' name is empty or null, then {@code false} is returned.
		''' </summary>
		''' <param name="name">   the system property name. </param>
		''' <returns>  the {@code boolean} value of the system property. </returns>
		''' <exception cref="SecurityException"> for the same reasons as
		'''          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
		''' <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
		''' <seealso cref=     java.lang.System#getProperty(java.lang.String, java.lang.String) </seealso>
		Public Shared Function getBoolean(ByVal name As String) As Boolean
			Dim result As Boolean = False
			Try
				result = parseBoolean(System.getProperty(name))
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch IllegalArgumentException Or NullPointerException e
			End Try
			Return result
		End Function

		''' <summary>
		''' Compares this {@code Boolean} instance with another.
		''' </summary>
		''' <param name="b"> the {@code Boolean} instance to be compared </param>
		''' <returns>  zero if this object represents the same boolean value as the
		'''          argument; a positive value if this object represents true
		'''          and the argument represents false; and a negative value if
		'''          this object represents false and the argument represents true </returns>
		''' <exception cref="NullPointerException"> if the argument is {@code null} </exception>
		''' <seealso cref=     Comparable
		''' @since  1.5 </seealso>
		Public Function compareTo(ByVal b As Boolean?) As Integer
			Return compare(Me.value, b.value)
		End Function

		''' <summary>
		''' Compares two {@code boolean} values.
		''' The value returned is identical to what would be returned by:
		''' <pre>
		'''    Boolean.valueOf(x).compareTo(Boolean.valueOf(y))
		''' </pre>
		''' </summary>
		''' <param name="x"> the first {@code boolean} to compare </param>
		''' <param name="y"> the second {@code boolean} to compare </param>
		''' <returns> the value {@code 0} if {@code x == y};
		'''         a value less than {@code 0} if {@code !x && y}; and
		'''         a value greater than {@code 0} if {@code x && !y}
		''' @since 1.7 </returns>
		Public Shared Function compare(ByVal x As Boolean, ByVal y As Boolean) As Integer
			Return If(x = y, 0, (If(x, 1, -1)))
		End Function

		''' <summary>
		''' Returns the result of applying the logical AND operator to the
		''' specified {@code boolean} operands.
		''' </summary>
		''' <param name="a"> the first operand </param>
		''' <param name="b"> the second operand </param>
		''' <returns> the logical AND of {@code a} and {@code b} </returns>
		''' <seealso cref= java.util.function.BinaryOperator
		''' @since 1.8 </seealso>
		Public Shared Function logicalAnd(ByVal a As Boolean, ByVal b As Boolean) As Boolean
			Return a AndAlso b
		End Function

		''' <summary>
		''' Returns the result of applying the logical OR operator to the
		''' specified {@code boolean} operands.
		''' </summary>
		''' <param name="a"> the first operand </param>
		''' <param name="b"> the second operand </param>
		''' <returns> the logical OR of {@code a} and {@code b} </returns>
		''' <seealso cref= java.util.function.BinaryOperator
		''' @since 1.8 </seealso>
		Public Shared Function logicalOr(ByVal a As Boolean, ByVal b As Boolean) As Boolean
			Return a OrElse b
		End Function

		''' <summary>
		''' Returns the result of applying the logical XOR operator to the
		''' specified {@code boolean} operands.
		''' </summary>
		''' <param name="a"> the first operand </param>
		''' <param name="b"> the second operand </param>
		''' <returns>  the logical XOR of {@code a} and {@code b} </returns>
		''' <seealso cref= java.util.function.BinaryOperator
		''' @since 1.8 </seealso>
		Public Shared Function logicalXor(ByVal a As Boolean, ByVal b As Boolean) As Boolean
			Return a Xor b
		End Function
	End Class

End Namespace
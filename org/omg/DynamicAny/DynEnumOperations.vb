Namespace org.omg.DynamicAny


	''' <summary>
	''' org/omg/DynamicAny/DynEnumOperations.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/DynamicAny/DynamicAny.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' DynEnum objects support the manipulation of IDL enumerated values.
	''' The current position of a DynEnum is always -1.
	''' </summary>
	Public Interface DynEnumOperations
		Inherits org.omg.DynamicAny.DynAnyOperations

	  ''' <summary>
	  ''' Returns the value of the DynEnum as an IDL identifier.
	  ''' </summary>
	  Function get_as_string() As String

	  ''' <summary>
	  ''' Sets the value of the DynEnum to the enumerated value whose IDL identifier is passed in the value parameter.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> If value contains a string that is not a valid IDL identifier
	  '''            for the corresponding enumerated type </exception>
	  Sub set_as_string(ByVal value As String)

	  ''' <summary>
	  ''' Returns the value of the DynEnum as the enumerated value's ordinal value.
	  ''' Enumerators have ordinal values 0 to n-1, as they appear from left to right
	  ''' in the corresponding IDL definition.
	  ''' </summary>
	  Function get_as_ulong() As Integer

	  ''' <summary>
	  ''' Sets the value of the DynEnum as the enumerated value's ordinal value.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> If value contains a value that is outside the range of ordinal values
	  '''            for the corresponding enumerated type </exception>
	  Sub set_as_ulong(ByVal value As Integer)
	End Interface ' interface DynEnumOperations

End Namespace
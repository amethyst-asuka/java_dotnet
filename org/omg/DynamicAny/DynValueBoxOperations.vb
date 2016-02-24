Namespace org.omg.DynamicAny


	''' <summary>
	''' org/omg/DynamicAny/DynValueBoxOperations.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/DynamicAny/DynamicAny.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' DynValueBox objects support the manipulation of IDL boxed value types.
	''' The DynValueBox interface can represent both null and non-null value types.
	''' For a DynValueBox representing a non-null value type, the DynValueBox has a single component
	''' of the boxed type. A DynValueBox representing a null value type has no components
	''' and a current position of -1.
	''' </summary>
	Public Interface DynValueBoxOperations
		Inherits org.omg.DynamicAny.DynValueCommonOperations

	  ''' <summary>
	  ''' Returns the boxed value as an Any.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this object represents a null value box type </exception>
	  Function get_boxed_value() As org.omg.CORBA.Any

	  ''' <summary>
	  ''' Replaces the boxed value with the specified value.
	  ''' If the DynBoxedValue represents a null valuetype, it is converted to a non-null value.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if this object represents a non-null value box type and the type
	  '''            of the parameter is not matching the current boxed value type. </exception>
	  Sub set_boxed_value(ByVal boxed As org.omg.CORBA.Any)

	  ''' <summary>
	  ''' Returns the boxed value as a DynAny.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this object represents a null value box type </exception>
	  Function get_boxed_value_as_dyn_any() As org.omg.DynamicAny.DynAny

	  ''' <summary>
	  ''' Replaces the boxed value with the value contained in the parameter.
	  ''' If the DynBoxedValue represents a null valuetype, it is converted to a non-null value.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if this object represents a non-null value box type and the type
	  '''            of the parameter is not matching the current boxed value type. </exception>
	  Sub set_boxed_value_as_dyn_any(ByVal boxed As org.omg.DynamicAny.DynAny)
	End Interface ' interface DynValueBoxOperations

End Namespace
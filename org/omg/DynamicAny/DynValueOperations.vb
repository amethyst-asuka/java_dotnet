Namespace org.omg.DynamicAny


	''' <summary>
	''' org/omg/DynamicAny/DynValueOperations.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/DynamicAny/DynamicAny.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' DynValue objects support the manipulation of IDL non-boxed value types.
	''' The DynValue interface can represent both null and non-null value types.
	''' For a DynValue representing a non-null value type, the DynValue's components comprise
	''' the public and private members of the value type, including those inherited from concrete base value types,
	''' in the order of definition. A DynValue representing a null value type has no components
	''' and a current position of -1.
	''' <P>Warning: Indiscriminantly changing the contents of private value type members can cause the value type
	''' implementation to break by violating internal constraints. Access to private members is provided to support
	''' such activities as ORB bridging and debugging and should not be used to arbitrarily violate
	''' the encapsulation of the value type. 
	''' </summary>
	Public Interface DynValueOperations
		Inherits org.omg.DynamicAny.DynValueCommonOperations

	  ''' <summary>
	  ''' Returns the name of the member at the current position.
	  ''' This operation may return an empty string since the TypeCode of the value being
	  ''' manipulated may not contain the names of members.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the DynValue represents a null value type. </exception>
	  ''' <exception cref="InvalidValue"> if the current position does not indicate a member </exception>
	  Function current_member_name() As String

	  ''' <summary>
	  ''' Returns the TCKind associated with the member at the current position.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the DynValue represents a null value type. </exception>
	  ''' <exception cref="InvalidValue"> if the current position does not indicate a member </exception>
	  Function current_member_kind() As org.omg.CORBA.TCKind

	  ''' <summary>
	  ''' Returns a sequence of NameValuePairs describing the name and the value of each member
	  ''' in the value type.
	  ''' The sequence contains members in the same order as the declaration order of members
	  ''' as indicated by the DynValue's TypeCode. The current position is not affected.
	  ''' The member names in the returned sequence will be empty strings if the DynValue's TypeCode
	  ''' does not contain member names.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this object represents a null value type </exception>
	  Function get_members() As org.omg.DynamicAny.NameValuePair()

	  ''' <summary>
	  ''' Initializes the value type's members from a sequence of NameValuePairs.
	  ''' The operation sets the current position to zero if the passed sequences has non-zero length. Otherwise,
	  ''' if an empty sequence is passed, the current position is set to -1.
	  ''' A null value type can be initialized to a non-null value type using this method.
	  ''' <P>Members must appear in the NameValuePairs in the order in which they appear in the IDL specification
	  ''' of the value type as indicated by the DynValue's TypeCode or they must be empty strings.
	  ''' The operation makes no attempt to assign member values based on member names.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the member names supplied in the passed sequence do not match the
	  '''            corresponding member name in the DynValue's TypeCode and they are not empty strings </exception>
	  ''' <exception cref="InvalidValue"> if the passed sequence has a number of elements that disagrees
	  '''            with the number of members as indicated by the DynValue's TypeCode </exception>
	  Sub set_members(ByVal value As org.omg.DynamicAny.NameValuePair())

	  ''' <summary>
	  ''' Returns a sequence of NameDynAnyPairs describing the name and the value of each member
	  ''' in the value type.
	  ''' The sequence contains members in the same order as the declaration order of members
	  ''' as indicated by the DynValue's TypeCode. The current position is not affected.
	  ''' The member names in the returned sequence will be empty strings if the DynValue's TypeCode
	  ''' does not contain member names.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this object represents a null value type </exception>
	  Function get_members_as_dyn_any() As org.omg.DynamicAny.NameDynAnyPair()

	  ''' <summary>
	  ''' Initializes the value type's members from a sequence of NameDynAnyPairs.
	  ''' The operation sets the current position to zero if the passed sequences has non-zero length. Otherwise,
	  ''' if an empty sequence is passed, the current position is set to -1.
	  ''' A null value type can be initialized to a non-null value type using this method.
	  ''' <P>Members must appear in the NameDynAnyPairs in the order in which they appear in the IDL specification
	  ''' of the value type as indicated by the DynValue's TypeCode or they must be empty strings.
	  ''' The operation makes no attempt to assign member values based on member names.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the member names supplied in the passed sequence do not match the
	  '''            corresponding member name in the DynValue's TypeCode and they are not empty strings </exception>
	  ''' <exception cref="InvalidValue"> if the passed sequence has a number of elements that disagrees
	  '''            with the number of members as indicated by the DynValue's TypeCode </exception>
	  Sub set_members_as_dyn_any(ByVal value As org.omg.DynamicAny.NameDynAnyPair())
	End Interface ' interface DynValueOperations

End Namespace
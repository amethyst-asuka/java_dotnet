Namespace org.omg.DynamicAny


	''' <summary>
	''' org/omg/DynamicAny/DynUnionOperations.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/DynamicAny/DynamicAny.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' DynUnion objects support the manipulation of IDL unions.
	''' A union can have only two valid current positions:
	''' <UL>
	''' <LI>zero, which denotes the discriminator
	''' <LI>one, which denotes the active member
	''' </UL>
	''' The component_count value for a union depends on the current discriminator:
	''' it is 2 for a union whose discriminator indicates a named member, and 1 otherwise.
	''' </summary>
	Public Interface DynUnionOperations
		Inherits org.omg.DynamicAny.DynAnyOperations

	  ''' <summary>
	  ''' Returns the current discriminator value.
	  ''' </summary>
	  Function get_discriminator() As org.omg.DynamicAny.DynAny

	  ''' <summary>
	  ''' Sets the discriminator of the DynUnion to the specified value.
	  ''' Setting the discriminator to a value that is consistent with the currently active union member
	  ''' does not affect the currently active member. Setting the discriminator to a value that is inconsistent
	  ''' with the currently active member deactivates the member and activates the member that is consistent
	  ''' with the new discriminator value (if there is a member for that value) by initializing the member
	  ''' to its default value.
	  ''' Setting the discriminator of a union sets the current position to 0 if the discriminator value
	  ''' indicates a non-existent union member (has_no_active_member returns true in this case).
	  ''' Otherwise, if the discriminator value indicates a named union member, the current position is set to 1
	  ''' (has_no_active_member returns false and component_count returns 2 in this case).
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the TypeCode of the parameter is not equivalent to the TypeCode
	  '''            of the union's discriminator </exception>
	  Sub set_discriminator(ByVal d As org.omg.DynamicAny.DynAny)

	  ''' <summary>
	  ''' Sets the discriminator to a value that is consistent with the value of the default case of a union.
	  ''' It sets the current position to zero and causes component_count to return 2.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the union does not have an explicit default case </exception>
	  Sub set_to_default_member()

	  ''' <summary>
	  ''' Sets the discriminator to a value that does not correspond to any of the unions case labels.
	  ''' It sets the current position to zero and causes component_count to return 1.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the union has an explicit default case or if it uses the entire range
	  '''            of discriminator values for explicit case labels </exception>
	  Sub set_to_no_active_member()

	  ''' <summary>
	  ''' Returns true if the union has no active member, that is, the unions value consists solely
	  ''' of its discriminator because the discriminator has a value that is not listed as an explicit case label.
	  ''' Calling this operation on a union that has a default case returns false.
	  ''' Calling this operation on a union that uses the entire range of discriminator values
	  ''' for explicit case labels returns false.
	  ''' </summary>
	  Function has_no_active_member() As Boolean

	  ''' <summary>
	  ''' Returns the TCKind value of the discriminators TypeCode.
	  ''' </summary>
	  Function discriminator_kind() As org.omg.CORBA.TCKind

	  ''' <summary>
	  ''' Returns the TCKind value of the currently active members TypeCode. 
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if the union does not have a currently active member </exception>
	  Function member_kind() As org.omg.CORBA.TCKind

	  ''' <summary>
	  ''' Returns the currently active member. Note that the returned reference remains valid only
	  ''' for as long as the currently active member does not change. Using the returned reference
	  ''' beyond the life time of the currently active member raises OBJECT_NOT_EXIST. 
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if the union has no active member </exception>
	  Function member() As org.omg.DynamicAny.DynAny

	  ''' <summary>
	  ''' Returns the name of the currently active member. If the unions TypeCode does not contain
	  ''' a member name for the currently active member, the operation returns an empty string.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if the union has no active member </exception>
	  Function member_name() As String
	End Interface ' interface DynUnionOperations

End Namespace
Namespace org.omg.DynamicAny


	''' <summary>
	''' org/omg/DynamicAny/DynValueCommonOperations.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/DynamicAny/DynamicAny.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' DynValueCommon provides operations supported by both the DynValue and DynValueBox interfaces.
	''' </summary>
	Public Interface DynValueCommonOperations
		Inherits org.omg.DynamicAny.DynAnyOperations

	  ''' <summary>
	  ''' Returns true if the DynValueCommon represents a null value type.
	  ''' </summary>
	  Function is_null() As Boolean

	  ''' <summary>
	  ''' Changes the representation of a DynValueCommon to a null value type.
	  ''' </summary>
	  Sub set_to_null()

	  ''' <summary>
	  ''' Replaces a null value type with a newly constructed value. Its components are initialized
	  ''' to default values as in DynAnyFactory.create_dyn_any_from_type_code.
	  ''' If the DynValueCommon represents a non-null value type, then this operation has no effect. 
	  ''' </summary>
	  Sub set_to_value()
	End Interface ' interface DynValueCommonOperations

End Namespace
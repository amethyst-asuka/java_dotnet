Imports System

'
' * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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
Namespace org.omg.PortableServer


	''' <summary>
	''' Defines the native <code>Servant</code> type. In Java, the
	''' <code>Servant</code> type is mapped to the Java
	''' <code>org.omg.PortableServer.Servant</code> class.
	''' It serves as the base class for all POA servant
	''' implementations and provides a number of methods that may
	''' be invoked by the application programmer, as well as methods
	''' which are invoked by the POA itself and may be overridden by
	''' the user to control aspects of servant behavior.
	''' Based on IDL to Java spec. (CORBA V2.3.1) ptc/00-01-08.pdf.
	''' </summary>

	Public MustInherit Class Servant

		<NonSerialized> _
		Private _delegate As org.omg.PortableServer.portable.Delegate = Nothing
		''' <summary>
		''' Gets the ORB vendor-specific implementation of
		''' <code>PortableServer::Servant</code>. </summary>
		''' <returns> <code>_delegate</code> the ORB vendor-specific
		''' implementation of <code>PortableServer::Servant</code>. </returns>
		Public Function _get_delegate() As org.omg.PortableServer.portable.Delegate
			If _delegate Is Nothing Then Throw New org.omg.CORBA.BAD_INV_ORDER("The Servant has not been associated with an ORB instance")
			Return _delegate
		End Function

		''' <summary>
		''' Supports the Java ORB portability
		''' interfaces by providing a method for classes that support
		''' ORB portability through delegation to set their delegate. </summary>
		''' <param name="delegate"> ORB vendor-specific implementation of
		'''                 the <code>PortableServer::Servant</code>. </param>
		Public Sub _set_delegate(ByVal [delegate] As org.omg.PortableServer.portable.Delegate)
			_delegate = [delegate]
		End Sub

		''' <summary>
		''' Allows the servant to obtain the object reference for
		''' the target CORBA object it is incarnating for that request. </summary>
		''' <returns> <code>this_object</code> <code>Object</code> reference
		''' associated with the request. </returns>
		Public Function _this_object() As org.omg.CORBA.Object
			Return _get_delegate().this_object(Me)
		End Function

		''' <summary>
		''' Allows the servant to obtain the object reference for
		''' the target CORBA Object it is incarnating for that request. </summary>
		''' <param name="orb"> ORB with which the servant is associated. </param>
		''' <returns> <code>_this_object</code> reference associated with the request. </returns>
		Public Function _this_object(ByVal orb As org.omg.CORBA.ORB) As org.omg.CORBA.Object
			Try
				CType(orb, org.omg.CORBA_2_3.ORB).set_delegate(Me)
			Catch e As ClassCastException
				Throw New org.omg.CORBA.BAD_PARAM("POA Servant requires an instance of org.omg.CORBA_2_3.ORB")
			End Try
			Return _this_object()
		End Function

		''' <summary>
		''' Returns the instance of the ORB
		''' currently associated with the <code>Servant</code> (convenience method). </summary>
		''' <returns> <code>orb</code> the instance of the ORB currently
		''' associated with the <code>Servant</code>. </returns>
		Public Function _orb() As org.omg.CORBA.ORB
			Return _get_delegate().orb(Me)
		End Function

		''' <summary>
		''' Allows easy execution of common methods, equivalent to
		''' <code>PortableServer::Current:get_POA</code>. </summary>
		''' <returns> <code>poa</code> POA associated with the servant. </returns>
		Public Function _poa() As POA
			Return _get_delegate().poa(Me)
		End Function

		''' <summary>
		''' Allows easy execution of
		''' common methods, equivalent
		''' to calling <code>PortableServer::Current::get_object_id</code>. </summary>
		''' <returns> <code>object_id</code> the <code>Object</code> ID associated
		''' with this servant. </returns>
		Public Function _object_id() As SByte()
			Return _get_delegate().object_id(Me)
		End Function

		''' <summary>
		''' Returns the
		''' root POA from the ORB instance associated with the servant.
		''' Subclasses may override this method to return a different POA. </summary>
		''' <returns> <code>default_POA</code> the POA associated with the
		''' <code>Servant</code>. </returns>
		Public Overridable Function _default_POA() As POA
			Return _get_delegate().default_POA(Me)
		End Function

		''' <summary>
		''' Checks to see if the specified <code>repository_id</code> is present
		''' on the list returned by <code>_all_interfaces()</code> or is the
		''' <code>repository_id</code> for the generic CORBA Object. </summary>
		''' <param name="repository_id"> the <code>repository_id</code>
		'''          to be checked in the repository list or against the id
		'''          of generic CORBA objects. </param>
		''' <returns> <code>is_a</code> boolean indicating whether the specified
		'''          <code>repository_id</code> is
		'''         in the repository list or is same as a generic CORBA
		'''         object. </returns>
		Public Overridable Function _is_a(ByVal repository_id As String) As Boolean
			Return _get_delegate().is_a(Me, repository_id)
		End Function

		''' <summary>
		''' Checks for the existence of an
		''' <code>Object</code>.
		''' The <code>Servant</code> provides a default implementation of
		''' <code>_non_existent()</code> that can be overridden by derived servants. </summary>
		''' <returns> <code>non_existent</code> <code>true</code> if that object does
		'''           not exist,  <code>false</code> otherwise. </returns>
		Public Overridable Function _non_existent() As Boolean
			Return _get_delegate().non_existent(Me)
		End Function

		' Ken and Simon will ask about editorial changes
		' needed in IDL to Java mapping to the following
		' signature.
		''' <summary>
		''' Returns an object in the Interface Repository
		''' which provides type information that may be useful to a program.
		''' <code>Servant</code> provides a default implementation of
		''' <code>_get_interface()</code>
		''' that can be overridden by derived servants if the default
		''' behavior is not adequate. </summary>
		''' <returns> <code>get_interface</code> type information that corresponds to this servant. </returns>
	'    
	'    public org.omg.CORBA.Object _get_interface() {
	'        return _get_delegate().get_interface(this);
	'    }
	'    

		' _get_interface_def() replaces the _get_interface() method

		''' <summary>
		''' Returns an <code>InterfaceDef</code> object as a
		''' <code>CORBA::Object</code> that defines the runtime type of the
		''' <code>CORBA::Object</code> implemented by the <code>Servant</code>.
		''' The invoker of <code>_get_interface_def</code>
		''' must narrow the result to an <code>InterfaceDef</code> in order
		''' to use it.
		''' <P>This default implementation of <code>_get_interface_def()</code>
		''' can be overridden
		''' by derived servants if the default behavior is not adequate.
		''' As defined in the CORBA 2.3.1 specification, section 11.3.1, the
		''' default behavior of <code>_get_interface_def()</code> is to use
		''' the most derived
		''' interface of a static servant or the most derived interface retrieved
		''' from a dynamic servant to obtain the <code>InterfaceDef</code>.
		''' This behavior must
		''' be supported by the <code>Delegate</code> that implements the
		''' <code>Servant</code>. </summary>
		''' <returns> <code>get_interface_def</code> an <code>InterfaceDef</code>
		''' object as a
		''' <code>CORBA::Object</code> that defines the runtime type of the
		''' <code>CORBA::Object</code> implemented by the <code>Servant</code>. </returns>
		Public Overridable Function _get_interface_def() As org.omg.CORBA.Object
			' First try to call the delegate implementation class's
			' "Object get_interface_def(..)" method (will work for ORBs
			' whose delegates implement this method).
			' Else call the delegate implementation class's
			' "InterfaceDef get_interface(..)" method using reflection
			' (will work for ORBs that were built using an older version
			' of the Delegate interface with a get_interface method
			' but not a get_interface_def method).

			Dim [delegate] As org.omg.PortableServer.portable.Delegate = _get_delegate()
			Try
				' If the ORB's delegate class does not implement
				' "Object get_interface_def(..)", this will throw
				' an AbstractMethodError.
				Return [delegate].get_interface_def(Me)
			Catch aex As AbstractMethodError
				' Call "InterfaceDef get_interface(..)" method using reflection.
				Try
					Dim argc As Type() = { GetType(org.omg.PortableServer.Servant) }
					Dim meth As System.Reflection.MethodInfo = [delegate].GetType().GetMethod("get_interface", argc)
					Dim argx As Object() = { Me }
					Return CType(meth.invoke([delegate], argx), org.omg.CORBA.Object)
				Catch exs As java.lang.reflect.InvocationTargetException
					Dim t As Exception = exs.targetException
					If TypeOf t Is Exception Then
						Throw CType(t, [Error])
					ElseIf TypeOf t Is Exception Then
						Throw CType(t, Exception)
					Else
						Throw New org.omg.CORBA.NO_IMPLEMENT
					End If
				Catch rex As Exception
					Throw rex
				Catch exr As Exception
					Throw New org.omg.CORBA.NO_IMPLEMENT
				End Try
			End Try
		End Function

		' methods for which the user must provide an
		' implementation
		''' <summary>
		''' Used by the ORB to obtain complete type
		''' information from the servant. </summary>
		''' <param name="poa"> POA with which the servant is associated. </param>
		''' <param name="objectId"> is the id corresponding to the object
		'''         associated with this servant. </param>
		''' <returns> list of type information for the object. </returns>
		Public MustOverride Function _all_interfaces(ByVal poa As POA, ByVal objectId As SByte()) As String()
	End Class

End Namespace
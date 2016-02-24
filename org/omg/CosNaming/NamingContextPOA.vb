Imports Microsoft.VisualBasic
Imports System.Collections

Namespace org.omg.CosNaming


	''' <summary>
	''' org/omg/CosNaming/NamingContextPOA.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/CosNaming/nameservice.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' A naming context is an object that contains a set of name bindings in 
	''' which each name is unique. Different names can be bound to an object 
	''' in the same or different contexts at the same time. <p>
	''' 
	''' See <a href="http://www.omg.org/technology/documents/formal/naming_service.htm">
	''' CORBA COS 
	''' Naming Specification.</a>
	''' </summary>
	Public MustInherit Class NamingContextPOA
		Inherits org.omg.PortableServer.Servant
		Implements org.omg.CosNaming.NamingContextOperations, org.omg.CORBA.portable.InvokeHandler

			Public MustOverride Function _invoke(ByVal method As String, ByVal input As InputStream, ByVal handler As ResponseHandler) As OutputStream
			Public MustOverride Sub destroy()
			Public MustOverride Function bind_new_context(ByVal n As org.omg.CosNaming.NameComponent()) As org.omg.CosNaming.NamingContext
			Public MustOverride Function new_context() As org.omg.CosNaming.NamingContext
			Public MustOverride Sub list(ByVal how_many As Integer, ByVal bl As org.omg.CosNaming.BindingListHolder, ByVal bi As org.omg.CosNaming.BindingIteratorHolder)
			Public MustOverride Sub unbind(ByVal n As org.omg.CosNaming.NameComponent())
			Public MustOverride Function resolve(ByVal n As org.omg.CosNaming.NameComponent()) As org.omg.CORBA.Object
			Public MustOverride Sub rebind_context(ByVal n As org.omg.CosNaming.NameComponent(), ByVal nc As org.omg.CosNaming.NamingContext)
			Public MustOverride Sub rebind(ByVal n As org.omg.CosNaming.NameComponent(), ByVal obj As org.omg.CORBA.Object)
			Public MustOverride Sub bind_context(ByVal n As org.omg.CosNaming.NameComponent(), ByVal nc As org.omg.CosNaming.NamingContext)
			Public MustOverride Sub bind(ByVal n As org.omg.CosNaming.NameComponent(), ByVal obj As org.omg.CORBA.Object)

	  ' Constructors

	  Private Shared _methods As New Hashtable
	  Shared Sub New()
		_methods("bind") = New Integer?(0)
		_methods("bind_context") = New Integer?(1)
		_methods("rebind") = New Integer?(2)
		_methods("rebind_context") = New Integer?(3)
		_methods("resolve") = New Integer?(4)
		_methods("unbind") = New Integer?(5)
		_methods("list") = New Integer?(6)
		_methods("new_context") = New Integer?(7)
		_methods("bind_new_context") = New Integer?(8)
		_methods("destroy") = New Integer?(9)
	  End Sub

	  Public Overridable Function _invoke(ByVal $method As String, ByVal [in] As org.omg.CORBA.portable.InputStream, ByVal $rh As org.omg.CORBA.portable.ResponseHandler) As org.omg.CORBA.portable.OutputStream
		Dim out As org.omg.CORBA.portable.OutputStream = Nothing
		Dim __method As Integer? = CInt(Fix(_methods($method)))
		If __method Is Nothing Then Throw New org.omg.CORBA.BAD_OPERATION(0, org.omg.CORBA.CompletionStatus.COMPLETED_MAYBE)

		Select Case __method

	  ''' <summary>
	  ''' Creates a binding of a name and an object in the naming context.
	  ''' Naming contexts that are bound using bind do not participate in name
	  ''' resolution when compound names are passed to be resolved. 
	  ''' </summary>
	  ''' <param name="n"> Name of the object <p>
	  ''' </param>
	  ''' <param name="obj"> The Object to bind with the given name<p>
	  ''' </param>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.NotFound"> Indicates
	  ''' the name does not identify a binding.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.CannotProceed"> 
	  ''' Indicates that the implementation has given up for some reason.
	  ''' The client, however, may be able to continue the operation
	  ''' at the returned naming context.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.InvalidName"> 
	  ''' Indicates that the name is invalid. <p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.AlreadyBound"> 
	  ''' Indicates an object is already bound to the specified name.<p>  </exception>
		   Case 0 ' CosNaming/NamingContext/bind
			 Try
			   Dim n As org.omg.CosNaming.NameComponent() = org.omg.CosNaming.NameHelper.read([in])
			   Dim obj As org.omg.CORBA.Object = org.omg.CORBA.ObjectHelper.read([in])
			   Me.bind(n, obj)
			   out = $rh.createReply()
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.NotFound
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.NotFoundHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.CannotProceed
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.InvalidName
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.AlreadyBound
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.AlreadyBoundHelper.write(out, $ex)
			 End Try
			 Exit Select


	  ''' <summary>
	  ''' Names an object that is a naming context. Naming contexts that
	  ''' are bound using bind_context() participate in name resolution 
	  ''' when compound names are passed to be resolved.
	  ''' </summary>
	  ''' <param name="n"> Name of the object <p>
	  ''' </param>
	  ''' <param name="nc"> NamingContect object to bind with the given name <p>
	  ''' </param>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.NotFound"> Indicates the name does not identify a binding.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.CannotProceed"> Indicates that the implementation has
	  ''' given up for some reason. The client, however, may be able to 
	  ''' continue the operation at the returned naming context.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.InvalidName"> Indicates that the name is invalid. <p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.AlreadyBound"> Indicates an object is already 
	  ''' bound to the specified name.<p> </exception>
		   Case 1 ' CosNaming/NamingContext/bind_context
			 Try
			   Dim n As org.omg.CosNaming.NameComponent() = org.omg.CosNaming.NameHelper.read([in])
			   Dim nc As org.omg.CosNaming.NamingContext = org.omg.CosNaming.NamingContextHelper.read([in])
			   Me.bind_context(n, nc)
			   out = $rh.createReply()
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.NotFound
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.NotFoundHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.CannotProceed
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.InvalidName
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.AlreadyBound
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.AlreadyBoundHelper.write(out, $ex)
			 End Try
			 Exit Select


	  ''' <summary>
	  ''' Creates a binding of a name and an object in the naming context
	  ''' even if the name is already bound in the context. Naming contexts 
	  ''' that are bound using rebind do not participate in name resolution 
	  ''' when compound names are passed to be resolved.
	  ''' </summary>
	  ''' <param name="n"> Name of the object <p>
	  ''' </param>
	  ''' <param name="obj"> The Object to rebind with the given name <p>
	  ''' </param>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.NotFound"> Indicates the name does not identify a binding.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.CannotProceed"> Indicates that the implementation has
	  ''' given up for some reason. The client, however, may be able to 
	  ''' continue the operation at the returned naming context.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.InvalidName"> Indicates that the name is invalid. <p> </exception>
		   Case 2 ' CosNaming/NamingContext/rebind
			 Try
			   Dim n As org.omg.CosNaming.NameComponent() = org.omg.CosNaming.NameHelper.read([in])
			   Dim obj As org.omg.CORBA.Object = org.omg.CORBA.ObjectHelper.read([in])
			   Me.rebind(n, obj)
			   out = $rh.createReply()
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.NotFound
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.NotFoundHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.CannotProceed
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.InvalidName
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.write(out, $ex)
			 End Try
			 Exit Select


	  ''' <summary>
	  ''' Creates a binding of a name and a naming context in the naming
	  ''' context even if the name is already bound in the context. Naming 
	  ''' contexts that are bound using rebind_context() participate in name 
	  ''' resolution when compound names are passed to be resolved.
	  ''' </summary>
	  ''' <param name="n"> Name of the object <p>
	  ''' </param>
	  ''' <param name="nc"> NamingContect object to rebind with the given name <p>
	  ''' </param>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.NotFound"> Indicates the name does not identify a binding.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.CannotProceed"> Indicates that the implementation has
	  ''' given up for some reason. The client, however, may be able to 
	  ''' continue the operation at the returned naming context.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.InvalidName"> Indicates that the name is invalid. <p> </exception>
		   Case 3 ' CosNaming/NamingContext/rebind_context
			 Try
			   Dim n As org.omg.CosNaming.NameComponent() = org.omg.CosNaming.NameHelper.read([in])
			   Dim nc As org.omg.CosNaming.NamingContext = org.omg.CosNaming.NamingContextHelper.read([in])
			   Me.rebind_context(n, nc)
			   out = $rh.createReply()
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.NotFound
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.NotFoundHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.CannotProceed
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.InvalidName
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.write(out, $ex)
			 End Try
			 Exit Select


	  ''' <summary>
	  ''' The resolve operation is the process of retrieving an object
	  ''' bound to a name in a given context. The given name must exactly 
	  ''' match the bound name. The naming service does not return the type 
	  ''' of the object. Clients are responsible for "narrowing" the object 
	  ''' to the appropriate type. That is, clients typically cast the returned 
	  ''' object from Object to a more specialized interface.
	  ''' </summary>
	  ''' <param name="n"> Name of the object <p>
	  ''' </param>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.NotFound"> Indicates the name does not identify a binding.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.CannotProceed"> Indicates that the implementation has
	  ''' given up for some reason. The client, however, may be able to 
	  ''' continue the operation at the returned naming context.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.InvalidName"> Indicates that the name is invalid. <p> </exception>
		   Case 4 ' CosNaming/NamingContext/resolve
			 Try
			   Dim n As org.omg.CosNaming.NameComponent() = org.omg.CosNaming.NameHelper.read([in])
			   Dim $result As org.omg.CORBA.Object = Nothing
			   $result = Me.resolve(n)
			   out = $rh.createReply()
			   org.omg.CORBA.ObjectHelper.write(out, $result)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.NotFound
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.NotFoundHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.CannotProceed
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.InvalidName
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.write(out, $ex)
			 End Try
			 Exit Select


	  ''' <summary>
	  ''' The unbind operation removes a name binding from a context.
	  ''' </summary>
	  ''' <param name="n"> Name of the object <p>
	  ''' </param>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.NotFound"> Indicates the name does not identify a binding.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.CannotProceed"> Indicates that the implementation has
	  ''' given up for some reason. The client, however, may be able to 
	  ''' continue the operation at the returned naming context.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.InvalidName"> Indicates that the name is invalid. <p> </exception>
		   Case 5 ' CosNaming/NamingContext/unbind
			 Try
			   Dim n As org.omg.CosNaming.NameComponent() = org.omg.CosNaming.NameHelper.read([in])
			   Me.unbind(n)
			   out = $rh.createReply()
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.NotFound
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.NotFoundHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.CannotProceed
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.InvalidName
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.write(out, $ex)
			 End Try
			 Exit Select


	  ''' <summary>
	  ''' The list operation allows a client to iterate through a set of
	  ''' bindings in a naming context. <p>
	  ''' 
	  ''' The list operation returns at most the requested number of
	  ''' bindings in BindingList bl. 
	  ''' <ul>
	  ''' <li>If the naming context contains additional 
	  ''' bindings, the list operation returns a BindingIterator with the 
	  ''' additional bindings. 
	  ''' <li>If the naming context does not contain additional 
	  ''' bindings, the binding iterator is a nil object reference.
	  ''' </ul>
	  ''' </summary>
	  ''' <param name="how_many"> the maximum number of bindings to return <p>
	  ''' </param>
	  ''' <param name="bl"> the returned list of bindings <p>
	  ''' </param>
	  ''' <param name="bi"> the returned binding iterator <p> </param>
		   Case 6 ' CosNaming/NamingContext/list
			 Dim how_many As Integer = [in].read_ulong()
			 Dim bl As New org.omg.CosNaming.BindingListHolder
			 Dim bi As New org.omg.CosNaming.BindingIteratorHolder
			 Me.list(how_many, bl, bi)
			 out = $rh.createReply()
			 org.omg.CosNaming.BindingListHelper.write(out, bl.value)
			 org.omg.CosNaming.BindingIteratorHelper.write(out, bi.value)
			 Exit Select


	  ''' <summary>
	  ''' This operation returns a naming context implemented by the same
	  ''' naming server as the context on which the operation was invoked. 
	  ''' The new context is not bound to any name.
	  ''' </summary>
		   Case 7 ' CosNaming/NamingContext/new_context
			 Dim $result As org.omg.CosNaming.NamingContext = Nothing
			 $result = Me.new_context()
			 out = $rh.createReply()
			 org.omg.CosNaming.NamingContextHelper.write(out, $result)
			 Exit Select


	  ''' <summary>
	  ''' This operation creates a new context and binds it to the name
	  ''' supplied as an argument. The newly-created context is implemented 
	  ''' by the same naming server as the context in which it was bound (that 
	  ''' is, the naming server that implements the context denoted by the 
	  ''' name argument excluding the last component).
	  ''' </summary>
	  ''' <param name="n"> Name of the object <p>
	  ''' </param>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.NotFound"> Indicates the name does not identify a binding.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.AlreadyBound"> Indicates an object is already 
	  ''' bound to the specified name.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.CannotProceed"> Indicates that the implementation has
	  ''' given up for some reason. The client, however, may be able to 
	  ''' continue the operation at the returned naming context.<p>
	  ''' </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.InvalidName"> Indicates that the name is invalid. <p> </exception>
		   Case 8 ' CosNaming/NamingContext/bind_new_context
			 Try
			   Dim n As org.omg.CosNaming.NameComponent() = org.omg.CosNaming.NameHelper.read([in])
			   Dim $result As org.omg.CosNaming.NamingContext = Nothing
			   $result = Me.bind_new_context(n)
			   out = $rh.createReply()
			   org.omg.CosNaming.NamingContextHelper.write(out, $result)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.NotFound
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.NotFoundHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.AlreadyBound
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.AlreadyBoundHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.CannotProceed
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.write(out, $ex)
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.InvalidName
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.write(out, $ex)
			 End Try
			 Exit Select


	  ''' <summary>
	  ''' The destroy operation deletes a naming context. If the naming 
	  ''' context contains bindings, the NotEmpty exception is raised.
	  ''' </summary>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.NotEmpty"> Indicates that the Naming Context contains bindings. </exception>
		   Case 9 ' CosNaming/NamingContext/destroy
			 Try
			   Me.destroy()
			   out = $rh.createReply()
			 Catch $ex As org.omg.CosNaming.NamingContextPackage.NotEmpty
			   out = $rh.createExceptionReply()
			   org.omg.CosNaming.NamingContextPackage.NotEmptyHelper.write(out, $ex)
			 End Try
			 Exit Select

		   Case Else
			 Throw New org.omg.CORBA.BAD_OPERATION(0, org.omg.CORBA.CompletionStatus.COMPLETED_MAYBE)
		End Select

		Return out
	  End Function ' _invoke

	  ' Type-specific CORBA::Object operations
	  Private Shared __ids As String() = { "IDL:omg.org/CosNaming/NamingContext:1.0"}

	  Public Overridable Function _all_interfaces(ByVal poa As org.omg.PortableServer.POA, ByVal objectId As SByte()) As String()
		Return CType(__ids.clone(), String())
	  End Function

	  Public Overridable Function _this() As NamingContext
		Return NamingContextHelper.narrow(MyBase._this_object())
	  End Function

	  Public Overridable Function _this(ByVal orb As org.omg.CORBA.ORB) As NamingContext
		Return NamingContextHelper.narrow(MyBase._this_object(orb))
	  End Function


	End Class ' class NamingContextPOA

End Namespace
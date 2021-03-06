Namespace org.omg.CosNaming


	''' <summary>
	''' org/omg/CosNaming/NamingContextOperations.java .
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
	Public Interface NamingContextOperations

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
	  Sub bind(ByVal n As org.omg.CosNaming.NameComponent(), ByVal obj As org.omg.CORBA.Object)

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
	  Sub bind_context(ByVal n As org.omg.CosNaming.NameComponent(), ByVal nc As org.omg.CosNaming.NamingContext)

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
	  Sub rebind(ByVal n As org.omg.CosNaming.NameComponent(), ByVal obj As org.omg.CORBA.Object)

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
	  Sub rebind_context(ByVal n As org.omg.CosNaming.NameComponent(), ByVal nc As org.omg.CosNaming.NamingContext)

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
	  Function resolve(ByVal n As org.omg.CosNaming.NameComponent()) As org.omg.CORBA.Object

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
	  Sub unbind(ByVal n As org.omg.CosNaming.NameComponent())

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
	  Sub list(ByVal how_many As Integer, ByVal bl As org.omg.CosNaming.BindingListHolder, ByVal bi As org.omg.CosNaming.BindingIteratorHolder)

	  ''' <summary>
	  ''' This operation returns a naming context implemented by the same
	  ''' naming server as the context on which the operation was invoked. 
	  ''' The new context is not bound to any name.
	  ''' </summary>
	  Function new_context() As org.omg.CosNaming.NamingContext

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
	  Function bind_new_context(ByVal n As org.omg.CosNaming.NameComponent()) As org.omg.CosNaming.NamingContext

	  ''' <summary>
	  ''' The destroy operation deletes a naming context. If the naming 
	  ''' context contains bindings, the NotEmpty exception is raised.
	  ''' </summary>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.NotEmpty"> Indicates that the Naming Context contains bindings. </exception>
	  Sub destroy()
	End Interface ' interface NamingContextOperations

End Namespace
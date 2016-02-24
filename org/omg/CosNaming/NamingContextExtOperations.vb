Namespace org.omg.CosNaming


	''' <summary>
	''' org/omg/CosNaming/NamingContextExtOperations.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/CosNaming/nameservice.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' <code>NamingContextExt</code> is the extension of <code>NamingContext</code>
	''' which
	''' contains a set of name bindings in which each name is unique and is
	''' part of Interoperable Naming Service.
	''' Different names can be bound to an object in the same or different
	''' contexts at the same time. Using <tt>NamingContextExt</tt>, you can use
	''' URL-based names to bind and resolve. <p>
	''' 
	''' See <a href="http://www.omg.org/technology/documents/formal/naming_service.htm">
	''' CORBA COS 
	''' Naming Specification.</a>
	''' </summary>
	Public Interface NamingContextExtOperations
		Inherits org.omg.CosNaming.NamingContextOperations

	  ''' <summary>
	  ''' This operation creates a stringified name from the array of Name
	  ''' components.
	  ''' </summary>
	  ''' <param name="n"> Name of the object <p>
	  ''' </param>
	  ''' <exception cref="org.omg.CosNaming.NamingContextExtPackage.InvalidName">
	  ''' Indicates the name does not identify a binding.<p>
	  '''  </exception>
	  Function to_string(ByVal n As org.omg.CosNaming.NameComponent()) As String

	  ''' <summary>
	  ''' This operation  converts a Stringified Name into an  equivalent array
	  ''' of Name Components. 
	  ''' </summary>
	  ''' <param name="sn"> Stringified Name of the object <p>
	  ''' </param>
	  ''' <exception cref="org.omg.CosNaming.NamingContextExtPackage.InvalidName">
	  ''' Indicates the name does not identify a binding.<p>
	  '''  </exception>
	  Function to_name(ByVal sn As String) As org.omg.CosNaming.NameComponent()

	  ''' <summary>
	  ''' This operation creates a URL based "iiopname://" format name
	  ''' from the Stringified Name of the object.
	  ''' </summary>
	  ''' <param name="addr"> internet based address of the host machine where  Name Service is running <p> </param>
	  ''' <param name="sn"> Stringified Name of the object <p>
	  ''' </param>
	  ''' <exception cref="org.omg.CosNaming.NamingContextExtPackage.InvalidName">
	  ''' Indicates the name does not identify a binding.<p> </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.InvalidAddress">
	  ''' Indicates the internet based address of the host machine is 
	  ''' incorrect <p>
	  '''  </exception>
	  Function to_url(ByVal addr As String, ByVal sn As String) As String

	  ''' <summary>
	  ''' This operation resolves the Stringified name into the object
	  ''' reference. 
	  ''' </summary>
	  ''' <param name="sn"> Stringified Name of the object <p>
	  ''' </param>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.NotFound">
	  ''' Indicates there is no object reference for the given name. <p> </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextPackage.CannotProceed">
	  ''' Indicates that the given compound name is incorrect <p> </exception>
	  ''' <exception cref="org.omg.CosNaming.NamingContextExtPackage.InvalidName">
	  ''' Indicates the name does not identify a binding.<p>
	  '''  </exception>
	  Function resolve_str(ByVal sn As String) As org.omg.CORBA.Object
	End Interface ' interface NamingContextExtOperations

End Namespace
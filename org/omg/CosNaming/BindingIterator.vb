Namespace org.omg.CosNaming


	''' <summary>
	''' org/omg/CosNaming/BindingIterator.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/CosNaming/nameservice.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' The BindingIterator interface allows a client to iterate through
	''' the bindings using the next_one or next_n operations.
	''' 
	''' The bindings iterator is obtained by using the <tt>list</tt>
	''' method on the <tt>NamingContext</tt>. </summary>
	''' <seealso cref= org.omg.CosNaming.NamingContext#list </seealso>
	Public Interface BindingIterator
		Inherits BindingIteratorOperations, org.omg.CORBA.Object, org.omg.CORBA.portable.IDLEntity

	End Interface ' interface BindingIterator

End Namespace
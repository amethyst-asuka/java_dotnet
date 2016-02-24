Namespace org.omg.CosNaming


	''' <summary>
	''' org/omg/CosNaming/BindingIteratorOperations.java .
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
	Public Interface BindingIteratorOperations

	  ''' <summary>
	  ''' This operation returns the next binding. If there are no more
	  ''' bindings, false is returned.
	  ''' </summary>
	  ''' <param name="b"> the returned binding </param>
	  Function next_one(ByVal b As org.omg.CosNaming.BindingHolder) As Boolean

	  ''' <summary>
	  ''' This operation returns at most the requested number of bindings.
	  ''' </summary>
	  ''' <param name="how_many"> the maximum number of bindings tro return <p>
	  ''' </param>
	  ''' <param name="bl"> the returned bindings </param>
	  Function next_n(ByVal how_many As Integer, ByVal bl As org.omg.CosNaming.BindingListHolder) As Boolean

	  ''' <summary>
	  ''' This operation destroys the iterator.
	  ''' </summary>
	  Sub destroy()
	End Interface ' interface BindingIteratorOperations

End Namespace
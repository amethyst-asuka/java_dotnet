Imports Microsoft.VisualBasic
Imports System.Collections

Namespace org.omg.CosNaming


	''' <summary>
	''' org/omg/CosNaming/BindingIteratorPOA.java .
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
	Public MustInherit Class BindingIteratorPOA
		Inherits org.omg.PortableServer.Servant
		Implements org.omg.CosNaming.BindingIteratorOperations, org.omg.CORBA.portable.InvokeHandler

			Public MustOverride Function _invoke(ByVal method As String, ByVal input As InputStream, ByVal handler As ResponseHandler) As OutputStream
			Public MustOverride Sub destroy()
			Public MustOverride Function next_n(ByVal how_many As Integer, ByVal bl As org.omg.CosNaming.BindingListHolder) As Boolean
			Public MustOverride Function next_one(ByVal b As org.omg.CosNaming.BindingHolder) As Boolean

	  ' Constructors

	  Private Shared _methods As New Hashtable
	  Shared Sub New()
		_methods("next_one") = New Integer?(0)
		_methods("next_n") = New Integer?(1)
		_methods("destroy") = New Integer?(2)
	  End Sub

	  Public Overridable Function _invoke(ByVal $method As String, ByVal [in] As org.omg.CORBA.portable.InputStream, ByVal $rh As org.omg.CORBA.portable.ResponseHandler) As org.omg.CORBA.portable.OutputStream
		Dim out As org.omg.CORBA.portable.OutputStream = Nothing
		Dim __method As Integer? = CInt(Fix(_methods($method)))
		If __method Is Nothing Then Throw New org.omg.CORBA.BAD_OPERATION(0, org.omg.CORBA.CompletionStatus.COMPLETED_MAYBE)

		Select Case __method

	  ''' <summary>
	  ''' This operation returns the next binding. If there are no more
	  ''' bindings, false is returned.
	  ''' </summary>
	  ''' <param name="b"> the returned binding </param>
		   Case 0 ' CosNaming/BindingIterator/next_one
			 Dim b As New org.omg.CosNaming.BindingHolder
			 Dim $result As Boolean = False
			 $result = Me.next_one(b)
			 out = $rh.createReply()
			 out.write_boolean($result)
			 org.omg.CosNaming.BindingHelper.write(out, b.value)
			 Exit Select


	  ''' <summary>
	  ''' This operation returns at most the requested number of bindings.
	  ''' </summary>
	  ''' <param name="how_many"> the maximum number of bindings tro return <p>
	  ''' </param>
	  ''' <param name="bl"> the returned bindings </param>
		   Case 1 ' CosNaming/BindingIterator/next_n
			 Dim how_many As Integer = [in].read_ulong()
			 Dim bl As New org.omg.CosNaming.BindingListHolder
			 Dim $result As Boolean = False
			 $result = Me.next_n(how_many, bl)
			 out = $rh.createReply()
			 out.write_boolean($result)
			 org.omg.CosNaming.BindingListHelper.write(out, bl.value)
			 Exit Select


	  ''' <summary>
	  ''' This operation destroys the iterator.
	  ''' </summary>
		   Case 2 ' CosNaming/BindingIterator/destroy
			 Me.destroy()
			 out = $rh.createReply()
			 Exit Select

		   Case Else
			 Throw New org.omg.CORBA.BAD_OPERATION(0, org.omg.CORBA.CompletionStatus.COMPLETED_MAYBE)
		End Select

		Return out
	  End Function ' _invoke

	  ' Type-specific CORBA::Object operations
	  Private Shared __ids As String() = { "IDL:omg.org/CosNaming/BindingIterator:1.0"}

	  Public Overridable Function _all_interfaces(ByVal poa As org.omg.PortableServer.POA, ByVal objectId As SByte()) As String()
		Return CType(__ids.clone(), String())
	  End Function

	  Public Overridable Function _this() As BindingIterator
		Return BindingIteratorHelper.narrow(MyBase._this_object())
	  End Function

	  Public Overridable Function _this(ByVal orb As org.omg.CORBA.ORB) As BindingIterator
		Return BindingIteratorHelper.narrow(MyBase._this_object(orb))
	  End Function


	End Class ' class BindingIteratorPOA

End Namespace
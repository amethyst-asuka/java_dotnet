'
' * Copyright (c) 1999, 2001, Oracle and/or its affiliates. All rights reserved.
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
Namespace org.omg.CORBA


	''' <summary>
	''' The Stub for <tt>Policy</tt>.  For more information on
	''' Stub files, see <a href="doc-files/generatedfiles.html#stub">
	''' "Generated Files: Stubs"</a>.<P>
	''' org/omg/CORBA/_PolicyStub.java
	''' Generated by the IDL-to-Java compiler (portable), version "3.0"
	''' from ../../../../../src/share/classes/org/omg/PortableServer/corba.idl.
	''' Saturday, July 17, 1999 12:26:20 AM PDT
	''' </summary>

	Public Class _PolicyStub
		Inherits org.omg.CORBA.portable.ObjectImpl
		Implements org.omg.CORBA.Policy

	  ''' <summary>
	  ''' Constructors
	  ''' NOTE:  If the default constructor is used, the
	  '''        object is useless until _set_delegate (...)
	  '''        is called.
	  ''' </summary>
	  Public Sub New()
		MyBase.New()
	  End Sub

	  ''' <summary>
	  ''' Constructs a <code>_PolicyStub</code> object initialized
	  ''' with the given <code>Delegate</code> object.
	  ''' </summary>
	  ''' <param name="delegate"> a Delegate Object </param>
	  Public Sub New(ByVal [delegate] As org.omg.CORBA.portable.Delegate)
		MyBase.New()
		_set_delegate([delegate])
	  End Sub


	  ''' <summary>
	  ''' Returns the constant value that corresponds to the
	  ''' type of the policy object.  The values of
	  ''' the polivy objectys are allocated by the OMG.
	  ''' New values for PolicyType should be obtained from the OMG by
	  ''' sending mail to request@omg.org.  In general the constant
	  ''' values that are allocated are defined in conjunction with
	  ''' the definition of the corresponding policy object. </summary>
	  ''' <returns> the constant value that corresponds to the type of
	  ''' the policy object. </returns>
	  Public Overridable Function policy_type() As Integer
		Dim _in As org.omg.CORBA.portable.InputStream = Nothing
		Try
		   Dim _out As org.omg.CORBA.portable.OutputStream = _request("_get_policy_type", True)
		   _in = _invoke(_out)
		   Dim __result As Integer = org.omg.CORBA.PolicyTypeHelper.read(_in)
		   Return __result
		Catch _ex As org.omg.CORBA.portable.ApplicationException
		   _in = _ex.inputStream
		   Dim _id As String = _ex.id
		   Throw New org.omg.CORBA.MARSHAL(_id)
		Catch _rm As org.omg.CORBA.portable.RemarshalException
		   Return policy_type()
		Finally
			_releaseReply(_in)
		End Try
	  End Function ' policy_type


	  ''' <summary>
	  ''' Copies the policy object. The copy does not retain any
	  ''' relationships that the policy had with any domain or object. </summary>
	  ''' <returns> the copy of the policy object. </returns>
	  Public Overridable Function copy() As org.omg.CORBA.Policy
		Dim _in As org.omg.CORBA.portable.InputStream = Nothing
		Try
		   Dim _out As org.omg.CORBA.portable.OutputStream = _request("copy", True)
		   _in = _invoke(_out)
		   Dim __result As org.omg.CORBA.Policy = org.omg.CORBA.PolicyHelper.read(_in)
		   Return __result
		Catch _ex As org.omg.CORBA.portable.ApplicationException
		   _in = _ex.inputStream
		   Dim _id As String = _ex.id
		   Throw New org.omg.CORBA.MARSHAL(_id)
		Catch _rm As org.omg.CORBA.portable.RemarshalException
		   Return copy()
		Finally
			_releaseReply(_in)
		End Try
	  End Function ' copy


	  ''' <summary>
	  ''' Destroys the policy object.  It is the responsibility of
	  ''' the policy object to determine whether it can be destroyed.
	  ''' </summary>
	  Public Overridable Sub destroy()
		Dim _in As org.omg.CORBA.portable.InputStream = Nothing
		Try
		   Dim _out As org.omg.CORBA.portable.OutputStream = _request("destroy", True)
		   _in = _invoke(_out)
		Catch _ex As org.omg.CORBA.portable.ApplicationException
		   _in = _ex.inputStream
		   Dim _id As String = _ex.id
		   Throw New org.omg.CORBA.MARSHAL(_id)
		Catch _rm As org.omg.CORBA.portable.RemarshalException
		   destroy()
		Finally
			_releaseReply(_in)
		End Try
	  End Sub ' destroy

	  ' Type-specific CORBA::Object operations
	  Private Shared __ids As String() = { "IDL:omg.org/CORBA/Policy:1.0"}

	  Public Overrides Function _ids() As String()
		Return CType(__ids.clone(), String())
	  End Function

	  Private Sub readObject(ByVal s As java.io.ObjectInputStream)
		 Try
		   Dim str As String = s.readUTF()
		   Dim obj As org.omg.CORBA.Object = org.omg.CORBA.ORB.init().string_to_object(str)
		   Dim [delegate] As org.omg.CORBA.portable.Delegate = CType(obj, org.omg.CORBA.portable.ObjectImpl)._get_delegate()
		   _set_delegate([delegate])
		 Catch e As java.io.IOException
		 End Try
	  End Sub

	  Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
		 Try
		   Dim str As String = org.omg.CORBA.ORB.init().object_to_string(Me)
		   s.writeUTF(str)
		 Catch e As java.io.IOException
		 End Try
	  End Sub
	End Class ' class _PolicyStub

End Namespace
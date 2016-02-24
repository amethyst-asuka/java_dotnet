'
' * Copyright (c) 1998, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' Associates  a name with a value that is an
	''' attribute of an IDL struct, and is used in the <tt>DynStruct</tt> APIs.
	''' </summary>

	Public NotInheritable Class NameValuePair
		Implements org.omg.CORBA.portable.IDLEntity

		''' <summary>
		''' The name to be associated with a value by this <code>NameValuePair</code> object.
		''' </summary>
		Public id As String

		''' <summary>
		''' The value to be associated with a name by this <code>NameValuePair</code> object.
		''' </summary>
		Public value As org.omg.CORBA.Any

		''' <summary>
		''' Constructs an empty <code>NameValuePair</code> object.
		''' To associate a name with a value after using this constructor, the fields
		''' of this object have to be accessed individually.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a <code>NameValuePair</code> object that associates
		''' the given name with the given <code>org.omg.CORBA.Any</code> object. </summary>
		''' <param name="__id"> the name to be associated with the given <code>Any</code> object </param>
		''' <param name="__value"> the <code>Any</code> object to be associated with the given name </param>
		Public Sub New(ByVal __id As String, ByVal __value As org.omg.CORBA.Any)
			id = __id
			value = __value
		End Sub
	End Class

End Namespace
'
' * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
Namespace org.omg.CORBA.portable


	''' <summary>
	''' The base class for the Holder classess of all complex
	''' IDL types. The ORB treats all generated Holders as Streamable to invoke
	''' the methods for marshalling and unmarshalling.
	''' 
	''' @since   JDK1.2
	''' </summary>

	Public Interface Streamable
		''' <summary>
		''' Reads data from <code>istream</code> and initalizes the
		''' <code>value</code> field of the Holder with the unmarshalled data.
		''' </summary>
		''' <param name="istream">   the InputStream that represents the CDR data from the wire. </param>
		Sub _read(ByVal istream As InputStream)
		''' <summary>
		''' Marshals to <code>ostream</code> the value in the
		''' <code>value</code> field of the Holder.
		''' </summary>
		''' <param name="ostream">   the CDR OutputStream </param>
		Sub _write(ByVal ostream As OutputStream)

		''' <summary>
		''' Retrieves the <code>TypeCode</code> object corresponding to the value
		''' in the <code>value</code> field of the Holder.
		''' </summary>
		''' <returns>    the <code>TypeCode</code> object for the value held in the holder </returns>
		Function _type() As org.omg.CORBA.TypeCode
	End Interface

End Namespace
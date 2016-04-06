Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.sql

	''' <summary>
	''' The mapping in the Java programming language of an SQL <code>REF</code>
	''' value, which is a reference to an SQL structured type value in the database.
	''' <P>
	''' SQL <code>REF</code> values are stored in a table that contains
	''' instances of a referenceable SQL structured type, and each <code>REF</code>
	''' value is a unique identifier for one instance in that table.
	''' An SQL <code>REF</code> value may be used in place of the
	''' SQL structured type it references, either as a column value in a
	''' table or an attribute value in a structured type.
	''' <P>
	''' Because an SQL <code>REF</code> value is a logical pointer to an
	''' SQL structured type, a <code>Ref</code> object is by default also a logical
	''' pointer. Thus, retrieving an SQL <code>REF</code> value as
	''' a <code>Ref</code> object does not materialize
	''' the attributes of the structured type on the client.
	''' <P>
	''' A <code>Ref</code> object can be stored in the database using the
	''' <code>PreparedStatement.setRef</code> method.
	''' <p>
	''' All methods on the <code>Ref</code> interface must be fully implemented if the
	''' JDBC driver supports the data type.
	''' </summary>
	''' <seealso cref= Struct
	''' @since 1.2 </seealso>
	Public Interface Ref

		''' <summary>
		''' Retrieves the fully-qualified SQL name of the SQL structured type that
		''' this <code>Ref</code> object references.
		''' </summary>
		''' <returns> the fully-qualified SQL name of the referenced SQL structured type </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		ReadOnly Property baseTypeName As String

		''' <summary>
		''' Retrieves the referenced object and maps it to a Java type
		''' using the given type map.
		''' </summary>
		''' <param name="map"> a <code>java.util.Map</code> object that contains
		'''        the mapping to use (the fully-qualified name of the SQL
		'''        structured type being referenced and the class object for
		'''        <code>SQLData</code> implementation to which the SQL
		'''        structured type will be mapped) </param>
		''' <returns>  a Java <code>Object</code> that is the custom mapping for
		'''          the SQL structured type to which this <code>Ref</code>
		'''          object refers </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.4 </exception>
		''' <seealso cref= #setObject </seealso>
		Function getObject(  map As IDictionary(Of String, [Class])) As Object


		''' <summary>
		''' Retrieves the SQL structured type instance referenced by
		''' this <code>Ref</code> object.  If the connection's type map has an entry
		''' for the structured type, the instance will be custom mapped to
		''' the Java class indicated in the type map.  Otherwise, the
		''' structured type instance will be mapped to a <code>Struct</code> object.
		''' </summary>
		''' <returns>  a Java <code>Object</code> that is the mapping for
		'''          the SQL structured type to which this <code>Ref</code>
		'''          object refers </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.4 </exception>
		''' <seealso cref= #setObject </seealso>
		Property [object] As Object


	End Interface

End Namespace
Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.script

	''' <summary>
	''' The interface whose implementing classes are used to connect Script Engines
	''' with objects, such as scoped Bindings, in hosting applications.  Each scope is a set
	''' of named attributes whose values can be set and retrieved using the
	''' <code>ScriptContext</code> methods. ScriptContexts also expose Readers and Writers
	''' that can be used by the ScriptEngines for input and output.
	''' 
	''' @author Mike Grogan
	''' @since 1.6
	''' </summary>
	Public Interface ScriptContext


		''' <summary>
		''' EngineScope attributes are visible during the lifetime of a single
		''' <code>ScriptEngine</code> and a set of attributes is maintained for each
		''' engine.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int ENGINE_SCOPE = 100;

		''' <summary>
		''' GlobalScope attributes are visible to all engines created by same ScriptEngineFactory.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int GLOBAL_SCOPE = 200;


		''' <summary>
		''' Associates a <code>Bindings</code> instance with a particular scope in this
		''' <code>ScriptContext</code>.  Calls to the <code>getAttribute</code> and
		''' <code>setAttribute</code> methods must map to the <code>get</code> and
		''' <code>put</code> methods of the <code>Bindings</code> for the specified scope.
		''' </summary>
		''' <param name="bindings"> The <code>Bindings</code> to associate with the given scope </param>
		''' <param name="scope"> The scope
		''' </param>
		''' <exception cref="IllegalArgumentException"> If no <code>Bindings</code> is defined for the
		''' specified scope value in ScriptContexts of this type. </exception>
		''' <exception cref="NullPointerException"> if value of scope is <code>ENGINE_SCOPE</code> and
		''' the specified <code>Bindings</code> is null.
		'''  </exception>
		Sub setBindings(ByVal bindings As Bindings, ByVal scope As Integer)

		''' <summary>
		''' Gets the <code>Bindings</code>  associated with the given scope in this
		''' <code>ScriptContext</code>.
		''' </summary>
		''' <returns> The associated <code>Bindings</code>.  Returns <code>null</code> if it has not
		''' been set.
		''' </returns>
		''' <param name="scope"> The scope </param>
		''' <exception cref="IllegalArgumentException"> If no <code>Bindings</code> is defined for the
		''' specified scope value in <code>ScriptContext</code> of this type. </exception>
		Function getBindings(ByVal scope As Integer) As Bindings

		''' <summary>
		''' Sets the value of an attribute in a given scope.
		''' </summary>
		''' <param name="name"> The name of the attribute to set </param>
		''' <param name="value"> The value of the attribute </param>
		''' <param name="scope"> The scope in which to set the attribute
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''         if the name is empty or if the scope is invalid. </exception>
		''' <exception cref="NullPointerException"> if the name is null. </exception>
		Sub setAttribute(ByVal name As String, ByVal value As Object, ByVal scope As Integer)

		''' <summary>
		''' Gets the value of an attribute in a given scope.
		''' </summary>
		''' <param name="name"> The name of the attribute to retrieve. </param>
		''' <param name="scope"> The scope in which to retrieve the attribute. </param>
		''' <returns> The value of the attribute. Returns <code>null</code> is the name
		''' does not exist in the given scope.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if the name is empty or if the value of scope is invalid. </exception>
		''' <exception cref="NullPointerException"> if the name is null. </exception>
		Function getAttribute(ByVal name As String, ByVal scope As Integer) As Object

		''' <summary>
		''' Remove an attribute in a given scope.
		''' </summary>
		''' <param name="name"> The name of the attribute to remove </param>
		''' <param name="scope"> The scope in which to remove the attribute
		''' </param>
		''' <returns> The removed value. </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if the name is empty or if the scope is invalid. </exception>
		''' <exception cref="NullPointerException"> if the name is null. </exception>
		Function removeAttribute(ByVal name As String, ByVal scope As Integer) As Object

		''' <summary>
		''' Retrieves the value of the attribute with the given name in
		''' the scope occurring earliest in the search order.  The order
		''' is determined by the numeric value of the scope parameter (lowest
		''' scope values first.)
		''' </summary>
		''' <param name="name"> The name of the the attribute to retrieve. </param>
		''' <returns> The value of the attribute in the lowest scope for
		''' which an attribute with the given name is defined.  Returns
		''' null if no attribute with the name exists in any scope. </returns>
		''' <exception cref="NullPointerException"> if the name is null. </exception>
		''' <exception cref="IllegalArgumentException"> if the name is empty. </exception>
		Function getAttribute(ByVal name As String) As Object


		''' <summary>
		''' Get the lowest scope in which an attribute is defined. </summary>
		''' <param name="name"> Name of the attribute
		''' . </param>
		''' <returns> The lowest scope.  Returns -1 if no attribute with the given
		''' name is defined in any scope. </returns>
		''' <exception cref="NullPointerException"> if name is null. </exception>
		''' <exception cref="IllegalArgumentException"> if name is empty. </exception>
		Function getAttributesScope(ByVal name As String) As Integer

		''' <summary>
		''' Returns the <code>Writer</code> for scripts to use when displaying output.
		''' </summary>
		''' <returns> The <code>Writer</code>. </returns>
		Property writer As java.io.Writer


		''' <summary>
		''' Returns the <code>Writer</code> used to display error output.
		''' </summary>
		''' <returns> The <code>Writer</code> </returns>
		Property errorWriter As java.io.Writer




		''' <summary>
		''' Returns a <code>Reader</code> to be used by the script to read
		''' input.
		''' </summary>
		''' <returns> The <code>Reader</code>. </returns>
		Property reader As java.io.Reader



		''' <summary>
		''' Returns immutable <code>List</code> of all the valid values for
		''' scope in the ScriptContext.
		''' </summary>
		''' <returns> list of scope values </returns>
		ReadOnly Property scopes As IList(Of Integer?)
	End Interface

End Namespace
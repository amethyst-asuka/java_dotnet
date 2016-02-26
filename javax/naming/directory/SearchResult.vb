'
' * Copyright (c) 1999, 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.directory


	''' <summary>
	''' This class represents an item in the NamingEnumeration returned as a
	''' result of the DirContext.search() methods.
	''' <p>
	''' A SearchResult instance is not synchronized against concurrent
	''' multithreaded access. Multiple threads trying to access and modify
	''' a single SearchResult instance should lock the object.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= DirContext#search
	''' @since 1.3 </seealso>

	Public Class SearchResult
		Inherits javax.naming.Binding

		''' <summary>
		''' Contains the attributes returned with the object.
		''' @serial
		''' </summary>
		Private attrs As Attributes

		''' <summary>
		''' Constructs a search result using the result's name, its bound object, and
		''' its attributes.
		''' <p>
		''' <tt>getClassName()</tt> will return the class name of <tt>obj</tt>
		''' (or null if <tt>obj</tt> is null) unless the class name has been
		''' explicitly set using <tt>setClassName()</tt>.
		''' </summary>
		''' <param name="name"> The non-null name of the search item. It is relative
		'''             to the <em>target context</em> of the search (which is
		''' named by the first parameter of the <code>search()</code> method)
		''' </param>
		''' <param name="obj">  The object bound to name. Can be null. </param>
		''' <param name="attrs"> The attributes that were requested to be returned with
		''' this search item. Cannot be null. </param>
		''' <seealso cref= javax.naming.NameClassPair#setClassName </seealso>
		''' <seealso cref= javax.naming.NameClassPair#getClassName </seealso>
		Public Sub New(ByVal name As String, ByVal obj As Object, ByVal attrs As Attributes)
			MyBase.New(name, obj)
			Me.attrs = attrs
		End Sub

		''' <summary>
		''' Constructs a search result using the result's name, its bound object, and
		''' its attributes, and whether the name is relative.
		''' <p>
		''' <tt>getClassName()</tt> will return the class name of <tt>obj</tt>
		''' (or null if <tt>obj</tt> is null) unless the class name has been
		''' explicitly set using <tt>setClassName()</tt>
		''' </summary>
		''' <param name="name"> The non-null name of the search item. </param>
		''' <param name="obj">  The object bound to name. Can be null. </param>
		''' <param name="attrs"> The attributes that were requested to be returned with
		''' this search item. Cannot be null. </param>
		''' <param name="isRelative"> true if <code>name</code> is relative
		'''         to the target context of the search (which is named by
		'''         the first parameter of the <code>search()</code> method);
		'''         false if <code>name</code> is a URL string. </param>
		''' <seealso cref= javax.naming.NameClassPair#setClassName </seealso>
		''' <seealso cref= javax.naming.NameClassPair#getClassName </seealso>
		Public Sub New(ByVal name As String, ByVal obj As Object, ByVal attrs As Attributes, ByVal isRelative As Boolean)
			MyBase.New(name, obj, isRelative)
			Me.attrs = attrs
		End Sub

		''' <summary>
		''' Constructs a search result using the result's name, its class name,
		''' its bound object, and its attributes.
		''' </summary>
		''' <param name="name"> The non-null name of the search item. It is relative
		'''             to the <em>target context</em> of the search (which is
		''' named by the first parameter of the <code>search()</code> method)
		''' </param>
		''' <param name="className">       The possibly null class name of the object
		'''         bound to <tt>name</tt>. If null, the class name of <tt>obj</tt> is
		'''         returned by <tt>getClassName()</tt>. If <tt>obj</tt> is also null,
		'''         <tt>getClassName()</tt> will return null. </param>
		''' <param name="obj">  The object bound to name. Can be null. </param>
		''' <param name="attrs"> The attributes that were requested to be returned with
		''' this search item. Cannot be null. </param>
		''' <seealso cref= javax.naming.NameClassPair#setClassName </seealso>
		''' <seealso cref= javax.naming.NameClassPair#getClassName </seealso>
		Public Sub New(ByVal name As String, ByVal className As String, ByVal obj As Object, ByVal attrs As Attributes)
			MyBase.New(name, className, obj)
			Me.attrs = attrs
		End Sub

		''' <summary>
		''' Constructs a search result using the result's name, its class name,
		''' its bound object, its attributes, and whether the name is relative.
		''' </summary>
		''' <param name="name"> The non-null name of the search item. </param>
		''' <param name="className">       The possibly null class name of the object
		'''         bound to <tt>name</tt>. If null, the class name of <tt>obj</tt> is
		'''         returned by <tt>getClassName()</tt>. If <tt>obj</tt> is also null,
		'''         <tt>getClassName()</tt> will return null. </param>
		''' <param name="obj">  The object bound to name. Can be null. </param>
		''' <param name="attrs"> The attributes that were requested to be returned with
		''' this search item. Cannot be null. </param>
		''' <param name="isRelative"> true if <code>name</code> is relative
		'''         to the target context of the search (which is named by
		'''         the first parameter of the <code>search()</code> method);
		'''         false if <code>name</code> is a URL string. </param>
		''' <seealso cref= javax.naming.NameClassPair#setClassName </seealso>
		''' <seealso cref= javax.naming.NameClassPair#getClassName </seealso>
		Public Sub New(ByVal name As String, ByVal className As String, ByVal obj As Object, ByVal attrs As Attributes, ByVal isRelative As Boolean)
			MyBase.New(name, className, obj, isRelative)
			Me.attrs = attrs
		End Sub

		''' <summary>
		''' Retrieves the attributes in this search result.
		''' </summary>
		''' <returns> The non-null attributes in this search result. Can be empty. </returns>
		''' <seealso cref= #setAttributes </seealso>
		Public Overridable Property attributes As Attributes
			Get
				Return attrs
			End Get
			Set(ByVal attrs As Attributes)
				Me.attrs = attrs
				' ??? check for null?
			End Set
		End Property




		''' <summary>
		''' Generates the string representation of this SearchResult.
		''' The string representation consists of the string representation
		''' of the binding and the string representation of
		''' this search result's attributes, separated by ':'.
		''' The contents of this string is useful
		''' for debugging and is not meant to be interpreted programmatically.
		''' </summary>
		''' <returns> The string representation of this SearchResult. Cannot be null. </returns>
		Public Overrides Function ToString() As String
			Return MyBase.ToString() & ":" & attributes
		End Function

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -9158063327699723172L
	End Class

End Namespace
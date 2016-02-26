'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming

	''' <summary>
	''' This class represents a name-to-object binding found in a context.
	''' <p>
	''' A context consists of name-to-object bindings.
	''' The Binding class represents such a binding.  It consists
	''' of a name and an object. The <code>Context.listBindings()</code>
	''' method returns an enumeration of Binding.
	''' <p>
	''' Use subclassing for naming systems that generate contents of
	''' a binding dynamically.
	''' <p>
	''' A Binding instance is not synchronized against concurrent access by multiple
	''' threads. Threads that need to access a Binding concurrently should
	''' synchronize amongst themselves and provide the necessary locking.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Class Binding
		Inherits NameClassPair

		''' <summary>
		''' Contains this binding's object.
		''' It is initialized by the constructor and can be updated using
		''' <tt>setObject</tt>.
		''' @serial </summary>
		''' <seealso cref= #getObject </seealso>
		''' <seealso cref= #setObject </seealso>
		Private boundObj As Object

		''' <summary>
		''' Constructs an instance of a Binding given its name and object.
		''' <p>
		''' <tt>getClassName()</tt> will return
		''' the class name of <tt>obj</tt> (or null if <tt>obj</tt> is null)
		''' unless the class name has been explicitly set using <tt>setClassName()</tt>
		''' </summary>
		''' <param name="name">    The non-null name of the object. It is relative
		'''             to the <em>target context</em> (which is
		''' named by the first parameter of the <code>listBindings()</code> method) </param>
		''' <param name="obj">     The possibly null object bound to name. </param>
		''' <seealso cref= NameClassPair#setClassName </seealso>
		Public Sub New(ByVal name As String, ByVal obj As Object)
			MyBase.New(name, Nothing)
			Me.boundObj = obj
		End Sub

		''' <summary>
		''' Constructs an instance of a Binding given its name, object, and whether
		''' the name is relative.
		''' <p>
		''' <tt>getClassName()</tt> will return the class name of <tt>obj</tt>
		''' (or null if <tt>obj</tt> is null) unless the class name has been
		''' explicitly set using <tt>setClassName()</tt>
		''' </summary>
		''' <param name="name">    The non-null string name of the object. </param>
		''' <param name="obj">     The possibly null object bound to name. </param>
		''' <param name="isRelative"> true if <code>name</code> is a name relative
		'''         to the target context (which is named by
		'''         the first parameter of the <code>listBindings()</code> method);
		'''         false if <code>name</code> is a URL string. </param>
		''' <seealso cref= NameClassPair#isRelative </seealso>
		''' <seealso cref= NameClassPair#setRelative </seealso>
		''' <seealso cref= NameClassPair#setClassName </seealso>
		Public Sub New(ByVal name As String, ByVal obj As Object, ByVal isRelative As Boolean)
			MyBase.New(name, Nothing, isRelative)
			Me.boundObj = obj
		End Sub

		''' <summary>
		''' Constructs an instance of a Binding given its name, class name, and object.
		''' </summary>
		''' <param name="name">    The non-null name of the object. It is relative
		'''             to the <em>target context</em> (which is
		''' named by the first parameter of the <code>listBindings()</code> method) </param>
		''' <param name="className">       The possibly null class name of the object
		'''         bound to <tt>name</tt>. If null, the class name of <tt>obj</tt> is
		'''         returned by <tt>getClassName()</tt>. If <tt>obj</tt> is also
		'''         null, <tt>getClassName()</tt> will return null. </param>
		''' <param name="obj">     The possibly null object bound to name. </param>
		''' <seealso cref= NameClassPair#setClassName </seealso>
		Public Sub New(ByVal name As String, ByVal className As String, ByVal obj As Object)
			MyBase.New(name, className)
			Me.boundObj = obj
		End Sub

		''' <summary>
		''' Constructs an instance of a Binding given its
		''' name, class name, object, and whether the name is relative.
		''' </summary>
		''' <param name="name">    The non-null string name of the object. </param>
		''' <param name="className">       The possibly null class name of the object
		'''         bound to <tt>name</tt>. If null, the class name of <tt>obj</tt> is
		'''         returned by <tt>getClassName()</tt>. If <tt>obj</tt> is also
		'''         null, <tt>getClassName()</tt> will return null. </param>
		''' <param name="obj">     The possibly null object bound to name. </param>
		''' <param name="isRelative"> true if <code>name</code> is a name relative
		'''         to the target context (which is named by
		'''         the first parameter of the <code>listBindings()</code> method);
		'''         false if <code>name</code> is a URL string. </param>
		''' <seealso cref= NameClassPair#isRelative </seealso>
		''' <seealso cref= NameClassPair#setRelative </seealso>
		''' <seealso cref= NameClassPair#setClassName </seealso>
		Public Sub New(ByVal name As String, ByVal className As String, ByVal obj As Object, ByVal isRelative As Boolean)
			MyBase.New(name, className, isRelative)
			Me.boundObj = obj
		End Sub

		''' <summary>
		''' Retrieves the class name of the object bound to the name of this binding.
		''' If the class name has been set explicitly, return it.
		''' Otherwise, if this binding contains a non-null object,
		''' that object's class name is used. Otherwise, null is returned.
		''' </summary>
		''' <returns> A possibly null string containing class name of object bound. </returns>
		Public Property Overrides className As String
			Get
				Dim cname As String = MyBase.className
				If cname IsNot Nothing Then Return cname
				If boundObj IsNot Nothing Then
					Return boundObj.GetType().name
				Else
					Return Nothing
				End If
			End Get
		End Property

		''' <summary>
		''' Retrieves the object bound to the name of this binding.
		''' </summary>
		''' <returns> The object bound; null if this binding does not contain an object. </returns>
		''' <seealso cref= #setObject </seealso>

		Public Overridable Property [object] As Object
			Get
				Return boundObj
			End Get
			Set(ByVal obj As Object)
				boundObj = obj
			End Set
		End Property


		''' <summary>
		''' Generates the string representation of this binding.
		''' The string representation consists of the string representation
		''' of the name/class pair and the string representation of
		''' this binding's object, separated by ':'.
		''' The contents of this string is useful
		''' for debugging and is not meant to be interpreted programmatically.
		''' </summary>
		''' <returns> The non-null string representation of this binding. </returns>

		Public Overrides Function ToString() As String
			Return MyBase.ToString() & ":" & [object]
		End Function

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 8839217842691845890L
	End Class

End Namespace
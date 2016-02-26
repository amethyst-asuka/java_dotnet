Imports System

'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' This class represents the object name and class name pair of a binding
	''' found in a context.
	''' <p>
	''' A context consists of name-to-object bindings.
	''' The NameClassPair class represents the name and the
	''' class of the bound object. It consists
	''' of a name and a string representing the
	''' package-qualified class name.
	''' <p>
	''' Use subclassing for naming systems that generate contents of
	''' a name/class pair dynamically.
	''' <p>
	''' A NameClassPair instance is not synchronized against concurrent
	''' access by multiple threads. Threads that need to access a NameClassPair
	''' concurrently should synchronize amongst themselves and provide
	''' the necessary locking.
	''' 
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= Context#list
	''' @since 1.3 </seealso>

	' 
	'  * <p>
	'  * The serialized form of a NameClassPair object consists of the name (a
	'  * String), class name (a String), and isRelative flag (a boolean).
	'  

	<Serializable> _
	Public Class NameClassPair
		''' <summary>
		''' Contains the name of this NameClassPair.
		''' It is initialized by the constructor and can be updated using
		''' <tt>setName()</tt>.
		''' @serial </summary>
		''' <seealso cref= #getName </seealso>
		''' <seealso cref= #setName </seealso>
		Private name As String

		''' <summary>
		''' Contains the class name contained in this NameClassPair.
		''' It is initialized by the constructor and can be updated using
		''' <tt>setClassName()</tt>.
		''' @serial </summary>
		''' <seealso cref= #getClassName </seealso>
		''' <seealso cref= #setClassName </seealso>
		Private className As String

		''' <summary>
		''' Contains the full name of this NameClassPair within its
		''' own namespace.
		''' It is initialized using <tt>setNameInNamespace()</tt>
		''' @serial </summary>
		''' <seealso cref= #getNameInNamespace </seealso>
		''' <seealso cref= #setNameInNamespace </seealso>
		Private fullName As String = Nothing


		''' <summary>
		''' Records whether the name of this <tt>NameClassPair</tt>
		''' is relative to the target context.
		''' It is initialized by the constructor and can be updated using
		''' <tt>setRelative()</tt>.
		''' @serial </summary>
		''' <seealso cref= #isRelative </seealso>
		''' <seealso cref= #setRelative </seealso>
		''' <seealso cref= #getName </seealso>
		''' <seealso cref= #setName </seealso>
		Private isRel As Boolean = True

		''' <summary>
		''' Constructs an instance of a NameClassPair given its
		''' name and class name.
		''' </summary>
		''' <param name="name">    The non-null name of the object. It is relative
		'''                  to the <em>target context</em> (which is
		''' named by the first parameter of the <code>list()</code> method) </param>
		''' <param name="className">       The possibly null class name of the object
		'''          bound to name. It is null if the object bound is null. </param>
		''' <seealso cref= #getClassName </seealso>
		''' <seealso cref= #setClassName </seealso>
		''' <seealso cref= #getName </seealso>
		''' <seealso cref= #setName </seealso>
		Public Sub New(ByVal name As String, ByVal className As String)
			Me.name = name
			Me.className = className
		End Sub

		''' <summary>
		''' Constructs an instance of a NameClassPair given its
		''' name, class name, and whether it is relative to the listing context.
		''' </summary>
		''' <param name="name">    The non-null name of the object. </param>
		''' <param name="className">       The possibly null class name of the object
		'''  bound to name.  It is null if the object bound is null. </param>
		''' <param name="isRelative"> true if <code>name</code> is a name relative
		'''          to the target context (which is named by the first parameter
		'''          of the <code>list()</code> method); false if <code>name</code>
		'''          is a URL string. </param>
		''' <seealso cref= #getClassName </seealso>
		''' <seealso cref= #setClassName </seealso>
		''' <seealso cref= #getName </seealso>
		''' <seealso cref= #setName </seealso>
		''' <seealso cref= #isRelative </seealso>
		''' <seealso cref= #setRelative </seealso>
		Public Sub New(ByVal name As String, ByVal className As String, ByVal isRelative As Boolean)
			Me.name = name
			Me.className = className
			Me.isRel = isRelative
		End Sub

		''' <summary>
		''' Retrieves the class name of the object bound to the name of this binding.
		''' If a reference or some other indirect information is bound,
		''' retrieves the class name of the eventual object that
		''' will be returned by <tt>Binding.getObject()</tt>.
		''' </summary>
		''' <returns>  The possibly null class name of object bound.
		'''          It is null if the object bound is null. </returns>
		''' <seealso cref= Binding#getObject </seealso>
		''' <seealso cref= Binding#getClassName </seealso>
		''' <seealso cref= #setClassName </seealso>
		Public Overridable Property className As String
			Get
				Return className
			End Get
			Set(ByVal name As String)
					Me.className = name
			End Set
		End Property

		''' <summary>
		''' Retrieves the name of this binding.
		''' If <tt>isRelative()</tt> is true, this name is relative to the
		''' target context (which is named by the first parameter of the
		''' <tt>list()</tt>).
		''' If <tt>isRelative()</tt> is false, this name is a URL string.
		''' </summary>
		''' <returns>  The non-null name of this binding. </returns>
		''' <seealso cref= #isRelative </seealso>
		''' <seealso cref= #setName </seealso>
		Public Overridable Property name As String
			Get
				Return name
			End Get
			Set(ByVal name As String)
				Me.name = name
			End Set
		End Property


		''' <summary>
		''' Determines whether the name of this binding is
		''' relative to the target context (which is named by
		''' the first parameter of the <code>list()</code> method).
		''' </summary>
		''' <returns> true if the name of this binding is relative to the
		'''          target context;
		'''          false if the name of this binding is a URL string. </returns>
		''' <seealso cref= #setRelative </seealso>
		''' <seealso cref= #getName </seealso>
		Public Overridable Property relative As Boolean
			Get
				Return isRel
			End Get
			Set(ByVal r As Boolean)
				isRel = r
			End Set
		End Property


		''' <summary>
		''' Retrieves the full name of this binding.
		''' The full name is the absolute name of this binding within
		''' its own namespace. See <seealso cref="Context#getNameInNamespace()"/>.
		''' <p>
		''' 
		''' In naming systems for which the notion of full name does not
		''' apply to this binding an <tt>UnsupportedOperationException</tt>
		''' is thrown.
		''' This exception is also thrown when a service provider written before
		''' the introduction of the method is in use.
		''' <p>
		''' The string returned by this method is not a JNDI composite name and
		''' should not be passed directly to context methods.
		''' </summary>
		''' <returns> The full name of this binding. </returns>
		''' <exception cref="UnsupportedOperationException"> if the notion of full name
		'''         does not apply to this binding in the naming system.
		''' @since 1.5 </exception>
		''' <seealso cref= #setNameInNamespace </seealso>
		''' <seealso cref= #getName </seealso>
		Public Overridable Property nameInNamespace As String
			Get
				If fullName Is Nothing Then Throw New System.NotSupportedException
				Return fullName
			End Get
			Set(ByVal fullName As String)
				Me.fullName = fullName
			End Set
		End Property


		''' <summary>
		''' Generates the string representation of this name/class pair.
		''' The string representation consists of the name and class name separated
		''' by a colon (':').
		''' The contents of this string is useful
		''' for debugging and is not meant to be interpreted programmatically.
		''' </summary>
		''' <returns> The string representation of this name/class pair. </returns>
		Public Overrides Function ToString() As String
			Return (If(relative, "", "(not relative)")) + name & ": " & className
		End Function


		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 5620776610160863339L
	End Class

End Namespace
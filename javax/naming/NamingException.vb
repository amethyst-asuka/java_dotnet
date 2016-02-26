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
	''' This is the superclass of all exceptions thrown by
	''' operations in the Context and DirContext interfaces.
	''' The nature of the failure is described by the name of the subclass.
	''' This exception captures the information pinpointing where the operation
	''' failed, such as where resolution last proceeded to.
	''' <ul>
	''' <li> Resolved Name. Portion of name that has been resolved.
	''' <li> Resolved Object. Object to which resolution of name proceeded.
	''' <li> Remaining Name. Portion of name that has not been resolved.
	''' <li> Explanation. Detail explaining why name resolution failed.
	''' <li> Root Exception. The exception that caused this naming exception
	'''                     to be thrown.
	''' </ul>
	''' null is an acceptable value for any of these fields. When null,
	''' it means that no such information has been recorded for that field.
	''' <p>
	''' A NamingException instance is not synchronized against concurrent
	''' multithreaded access. Multiple threads trying to access and modify
	''' a single NamingException instance should lock the object.
	''' <p>
	''' This exception has been retrofitted to conform to
	''' the general purpose exception-chaining mechanism.  The
	''' <i>root exception</i> (or <i>root cause</i>) is the same object as the
	''' <i>cause</i> returned by the <seealso cref="Throwable#getCause()"/> method.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>


	Public Class NamingException
		Inherits Exception

		''' <summary>
		''' Contains the part of the name that has been successfully resolved.
		''' It is a composite name and can be null.
		''' This field is initialized by the constructors.
		''' You should access and manipulate this field
		''' through its get and set methods.
		''' @serial </summary>
		''' <seealso cref= #getResolvedName </seealso>
		''' <seealso cref= #setResolvedName </seealso>
		Protected Friend _resolvedName As Name
		''' <summary>
		''' Contains the object to which resolution of the part of the name was
		''' successful. Can be null.
		''' This field is initialized by the constructors.
		''' You should access and manipulate this field
		''' through its get and set methods.
		''' @serial </summary>
		''' <seealso cref= #getResolvedObj </seealso>
		''' <seealso cref= #setResolvedObj </seealso>
		Protected Friend _resolvedObj As Object
		''' <summary>
		''' Contains the remaining name that has not been resolved yet.
		''' It is a composite name and can be null.
		''' This field is initialized by the constructors.
		''' You should access and manipulate this field
		''' through its get, set, "append" methods.
		''' @serial </summary>
		''' <seealso cref= #getRemainingName </seealso>
		''' <seealso cref= #setRemainingName </seealso>
		''' <seealso cref= #appendRemainingName </seealso>
		''' <seealso cref= #appendRemainingComponent </seealso>
		Protected Friend _remainingName As Name

		''' <summary>
		''' Contains the original exception that caused this NamingException to
		''' be thrown. This field is set if there is additional
		''' information that could be obtained from the original
		''' exception, or if the original exception could not be
		''' mapped to a subclass of NamingException.
		''' Can be null.
		''' <p>
		''' This field predates the general-purpose exception chaining facility.
		''' The <seealso cref="#initCause(Throwable)"/> and <seealso cref="#getCause()"/> methods
		''' are now the preferred means of accessing this information.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getRootCause </seealso>
		''' <seealso cref= #setRootCause(Throwable) </seealso>
		''' <seealso cref= #initCause(Throwable) </seealso>
		''' <seealso cref= #getCause </seealso>
		Protected Friend _rootException As Exception = Nothing

		''' <summary>
		''' Constructs a new NamingException with an explanation.
		''' All unspecified fields are set to null.
		''' </summary>
		''' <param name="explanation">     A possibly null string containing
		'''                          additional detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
				remainingName = Nothing
				resolvedName = remainingName
			resolvedObj = Nothing
		End Sub

		''' <summary>
		''' Constructs a new NamingException.
		''' All fields are set to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
				remainingName = Nothing
				resolvedName = remainingName
			resolvedObj = Nothing
		End Sub

		''' <summary>
		''' Retrieves the leading portion of the name that was resolved
		''' successfully.
		''' </summary>
		''' <returns> The part of the name that was resolved successfully.
		'''          It is a composite name. It can be null, which means
		'''          the resolved name field has not been set. </returns>
		''' <seealso cref= #getResolvedObj </seealso>
		''' <seealso cref= #setResolvedName </seealso>
		Public Overridable Property resolvedName As Name
			Get
				Return _resolvedName
			End Get
			Set(ByVal name As Name)
				If name IsNot Nothing Then
					_resolvedName = CType(name.clone(), Name)
				Else
					_resolvedName = Nothing
				End If
			End Set
		End Property

		''' <summary>
		''' Retrieves the remaining unresolved portion of the name. </summary>
		''' <returns> The part of the name that has not been resolved.
		'''          It is a composite name. It can be null, which means
		'''          the remaining name field has not been set. </returns>
		''' <seealso cref= #setRemainingName </seealso>
		''' <seealso cref= #appendRemainingName </seealso>
		''' <seealso cref= #appendRemainingComponent </seealso>
		Public Overridable Property remainingName As Name
			Get
				Return _remainingName
			End Get
			Set(ByVal name As Name)
				If name IsNot Nothing Then
					_remainingName = CType(name.clone(), Name)
				Else
					_remainingName = Nothing
				End If
			End Set
		End Property

		''' <summary>
		''' Retrieves the object to which resolution was successful.
		''' This is the object to which the resolved name is bound.
		''' </summary>
		''' <returns> The possibly null object that was resolved so far.
		'''  null means that the resolved object field has not been set. </returns>
		''' <seealso cref= #getResolvedName </seealso>
		''' <seealso cref= #setResolvedObj </seealso>
		Public Overridable Property resolvedObj As Object
			Get
				Return _resolvedObj
			End Get
			Set(ByVal obj As Object)
				_resolvedObj = obj
			End Set
		End Property

		''' <summary>
		''' Retrieves the explanation associated with this exception.
		''' </summary>
		''' <returns> The possibly null detail string explaining more
		'''         about this exception. If null, it means there is no
		'''         detail message for this exception.
		''' </returns>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
	ReadOnly	Public Overridable Property explanation As String
			Get
				Return message
			End Get
		End Property




		''' <summary>
		''' Add name as the last component in remaining name. </summary>
		''' <param name="name"> The component to add.
		'''         If name is null, this method does not do anything. </param>
		''' <seealso cref= #setRemainingName </seealso>
		''' <seealso cref= #getRemainingName </seealso>
		''' <seealso cref= #appendRemainingName </seealso>
		Public Overridable Sub appendRemainingComponent(ByVal name As String)
			If name IsNot Nothing Then
				Try
					If remainingName Is Nothing Then remainingName = New CompositeName
					remainingName.add(name)
				Catch e As NamingException
					Throw New System.ArgumentException(e.ToString())
				End Try
			End If
		End Sub

		''' <summary>
		''' Add components from 'name' as the last components in
		''' remaining name.
		''' <p>
		''' <tt>name</tt> is a composite name. If the intent is to append
		''' a compound name, you should "stringify" the compound name
		''' then invoke the overloaded form that accepts a String parameter.
		''' <p>
		''' Subsequent changes to <code>name</code> does not
		''' affect the remaining name field in this NamingException and vice versa. </summary>
		''' <param name="name"> The possibly null name containing ordered components to add.
		'''                 If name is null, this method does not do anything. </param>
		''' <seealso cref= #setRemainingName </seealso>
		''' <seealso cref= #getRemainingName </seealso>
		''' <seealso cref= #appendRemainingComponent </seealso>
		Public Overridable Sub appendRemainingName(ByVal name As Name)
			If name Is Nothing Then Return
			If remainingName IsNot Nothing Then
				Try
					remainingName.addAll(name)
				Catch e As NamingException
					Throw New System.ArgumentException(e.ToString())
				End Try
			Else
				remainingName = CType(name.clone(), Name)
			End If
		End Sub

		''' <summary>
		''' Retrieves the root cause of this NamingException, if any.
		''' The root cause of a naming exception is used when the service provider
		''' wants to indicate to the caller a non-naming related exception
		''' but at the same time wants to use the NamingException structure
		''' to indicate how far the naming operation proceeded.
		''' <p>
		''' This method predates the general-purpose exception chaining facility.
		''' The <seealso cref="#getCause()"/> method is now the preferred means of obtaining
		''' this information.
		''' </summary>
		''' <returns> The possibly null exception that caused this naming
		'''    exception. If null, it means no root cause has been
		'''    set for this naming exception. </returns>
		''' <seealso cref= #setRootCause </seealso>
		''' <seealso cref= #rootException </seealso>
		''' <seealso cref= #getCause </seealso>
		Public Overridable Property rootCause As Exception
			Get
				Return _rootException
			End Get
			Set(ByVal e As Exception)
				If e IsNot Me Then _rootException = e
			End Set
		End Property


		''' <summary>
		''' Returns the cause of this exception.  The cause is the
		''' throwable that caused this naming exception to be thrown.
		''' Returns <code>null</code> if the cause is nonexistent or
		''' unknown.
		''' </summary>
		''' <returns>  the cause of this exception, or <code>null</code> if the
		'''          cause is nonexistent or unknown. </returns>
		''' <seealso cref= #initCause(Throwable)
		''' @since 1.4 </seealso>
	ReadOnly	Public Overridable Property cause As Exception
			Get
				Return rootCause
			End Get
		End Property

		''' <summary>
		''' Initializes the cause of this exception to the specified value.
		''' The cause is the throwable that caused this naming exception to be
		''' thrown.
		''' <p>
		''' This method may be called at most once.
		''' </summary>
		''' <param name="cause">   the cause, which is saved for later retrieval by
		'''         the <seealso cref="#getCause()"/> method.  A <tt>null</tt> value
		'''         indicates that the cause is nonexistent or unknown. </param>
		''' <returns> a reference to this <code>NamingException</code> instance. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>cause</code> is this
		'''         exception.  (A throwable cannot be its own cause.) </exception>
		''' <exception cref="IllegalStateException"> if this method has already
		'''         been called on this exception. </exception>
		''' <seealso cref= #getCause
		''' @since 1.4 </seealso>
		Public Overridable Function initCause(ByVal cause As Exception) As Exception
			MyBase.initCause(cause)
			rootCause = cause
			Return Me
		End Function

		''' <summary>
		''' Generates the string representation of this exception.
		''' The string representation consists of this exception's class name,
		''' its detailed message, and if it has a root cause, the string
		''' representation of the root cause exception, followed by
		''' the remaining name (if it is not null).
		''' This string is used for debugging and not meant to be interpreted
		''' programmatically.
		''' </summary>
		''' <returns> The non-null string containing the string representation
		''' of this exception. </returns>
		Public Overrides Function ToString() As String
			Dim answer As String = MyBase.ToString()

			If rootException IsNot Nothing Then answer &= " [Root exception is " & rootException & "]"
			If remainingName IsNot Nothing Then answer &= "; remaining name '" & remainingName & "'"
			Return answer
		End Function

		''' <summary>
		''' Generates the string representation in more detail.
		''' This string representation consists of the information returned
		''' by the toString() that takes no parameters, plus the string
		''' representation of the resolved object (if it is not null).
		''' This string is used for debugging and not meant to be interpreted
		''' programmatically.
		''' </summary>
		''' <param name="detail"> If true, include details about the resolved object
		'''                 in addition to the other information. </param>
		''' <returns> The non-null string containing the string representation. </returns>
		Public Overrides Function ToString(ByVal detail As Boolean) As String
			If (Not detail) OrElse resolvedObj Is Nothing Then
				Return ToString()
			Else
				Return (ToString() & "; resolved object " & resolvedObj)
			End If
		End Function

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -1299181962103167177L
	End Class

End Namespace
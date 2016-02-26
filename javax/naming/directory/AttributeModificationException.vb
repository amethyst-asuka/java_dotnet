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
	''' This exception is thrown when an attempt is
	''' made to add, or remove, or modify an attribute, its identifier,
	''' or its values that conflicts with the attribute's (schema) definition
	''' or the attribute's state.
	''' It is thrown in response to DirContext.modifyAttributes().
	''' It contains a list of modifications that have not been performed, in the
	''' order that they were supplied to modifyAttributes().
	''' If the list is null, none of the modifications were performed successfully.
	''' <p>
	''' An AttributeModificationException instance is not synchronized
	''' against concurrent multithreaded access. Multiple threads trying
	''' to access and modify a single AttributeModification instance
	''' should lock the object.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= DirContext#modifyAttributes
	''' @since 1.3 </seealso>

	'
	'  *<p>
	'  * The serialized form of an AttributeModificationException object
	'  * consists of the serialized fields of its NamingException
	'  * superclass, followed by an array of ModificationItem objects.
	'  *
	'


	Public Class AttributeModificationException
		Inherits javax.naming.NamingException

		''' <summary>
		''' Contains the possibly null list of unexecuted modifications.
		''' @serial
		''' </summary>
		Private unexecs As ModificationItem() = Nothing

		''' <summary>
		''' Constructs a new instance of AttributeModificationException using
		''' an explanation. All other fields are set to null.
		''' </summary>
		''' <param name="explanation">     Possibly null additional detail about this exception.
		''' If null, this exception has no detail message.
		''' </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of AttributeModificationException.
		''' All fields are set to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Sets the unexecuted modification list to be e.
		''' Items in the list must appear in the same order in which they were
		''' originally supplied in DirContext.modifyAttributes().
		''' The first item in the list is the first one that was not executed.
		''' If this list is null, none of the operations originally submitted
		''' to modifyAttributes() were executed.
		''' </summary>
		''' <param name="e">        The possibly null list of unexecuted modifications. </param>
		''' <seealso cref= #getUnexecutedModifications </seealso>
		Public Overridable Property unexecutedModifications As ModificationItem()
			Set(ByVal e As ModificationItem())
				unexecs = e
			End Set
			Get
				Return unexecs
			End Get
		End Property


		''' <summary>
		''' The string representation of this exception consists of
		''' information about where the error occurred, and
		''' the first unexecuted modification.
		''' This string is meant for debugging and not mean to be interpreted
		''' programmatically. </summary>
		''' <returns> The non-null string representation of this exception. </returns>
		Public Overrides Function ToString() As String
			Dim orig As String = MyBase.ToString()
			If unexecs IsNot Nothing Then orig += ("First unexecuted modification: " & unexecs(0).ToString())
			Return orig
		End Function

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 8060676069678710186L
	End Class

End Namespace
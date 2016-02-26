Imports System

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

Namespace javax.naming.directory

	''' <summary>
	''' This class represents a modification item.
	''' It consists of a modification code and an attribute on which to operate.
	''' <p>
	''' A ModificationItem instance is not synchronized against concurrent
	''' multithreaded access. Multiple threads trying to access and modify
	''' a single ModificationItem instance should lock the object.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	'
	'  *<p>
	'  * The serialized form of a ModificationItem object consists of the
	'  * modification op (and int) and the corresponding Attribute.
	'

	<Serializable> _
	Public Class ModificationItem
		''' <summary>
		''' Contains an integer identify the modification
		''' to be performed.
		''' @serial
		''' </summary>
		Private mod_op As Integer
		''' <summary>
		''' Contains the attribute identifying
		''' the attribute and/or its value to be applied for the modification.
		''' @serial
		''' </summary>
		Private attr As Attribute

		''' <summary>
		''' Creates a new instance of ModificationItem. </summary>
		''' <param name="mod_op"> Modification to apply.  It must be one of:
		'''         DirContext.ADD_ATTRIBUTE
		'''         DirContext.REPLACE_ATTRIBUTE
		'''         DirContext.REMOVE_ATTRIBUTE </param>
		''' <param name="attr">     The non-null attribute to use for modification. </param>
		''' <exception cref="IllegalArgumentException"> If attr is null, or if mod_op is
		'''         not one of the ones specified above. </exception>
		Public Sub New(ByVal mod_op As Integer, ByVal attr As Attribute)
			Select Case mod_op
			Case DirContext.ADD_ATTRIBUTE, DirContext.REPLACE_ATTRIBUTE, DirContext.REMOVE_ATTRIBUTE
				If attr Is Nothing Then Throw New System.ArgumentException("Must specify non-null attribute for modification")

				Me.mod_op = mod_op
				Me.attr = attr

			Case Else
				Throw New System.ArgumentException("Invalid modification code " & mod_op)
			End Select
		End Sub

		''' <summary>
		''' Retrieves the modification code of this modification item. </summary>
		''' <returns> The modification code.  It is one of:
		'''         DirContext.ADD_ATTRIBUTE
		'''         DirContext.REPLACE_ATTRIBUTE
		'''         DirContext.REMOVE_ATTRIBUTE </returns>
		Public Overridable Property modificationOp As Integer
			Get
				Return mod_op
			End Get
		End Property

		''' <summary>
		''' Retrieves the attribute associated with this modification item. </summary>
		''' <returns> The non-null attribute to use for the modification. </returns>
		Public Overridable Property attribute As Attribute
			Get
				Return attr
			End Get
		End Property

		''' <summary>
		''' Generates the string representation of this modification item,
		''' which consists of the modification operation and its related attribute.
		''' The string representation is meant for debugging and not to be
		''' interpreted programmatically.
		''' </summary>
		''' <returns> The non-null string representation of this modification item. </returns>
		Public Overrides Function ToString() As String
			Select Case mod_op
			Case DirContext.ADD_ATTRIBUTE
				Return ("Add attribute: " & attr.ToString())

			Case DirContext.REPLACE_ATTRIBUTE
				Return ("Replace attribute: " & attr.ToString())

			Case DirContext.REMOVE_ATTRIBUTE
				Return ("Remove attribute: " & attr.ToString())
			End Select
			Return "" ' should never happen
		End Function

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 7573258562534746850L
	End Class

End Namespace
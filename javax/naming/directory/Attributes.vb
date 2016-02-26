Imports System

'
' * Copyright (c) 1999, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' This interface represents a collection of attributes.
	''' <p>
	''' In a directory, named objects can have associated with them
	''' attributes.  The Attributes interface represents a collection of attributes.
	''' For example, you can request from the directory the attributes
	''' associated with an object.  Those attributes are returned in
	''' an object that implements the Attributes interface.
	''' <p>
	''' Attributes in an object that implements the  Attributes interface are
	''' unordered. The object can have zero or more attributes.
	''' Attributes is either case-sensitive or case-insensitive (case-ignore).
	''' This property is determined at the time the Attributes object is
	''' created. (see BasicAttributes constructor for example).
	''' In a case-insensitive Attributes, the case of its attribute identifiers
	''' is ignored when searching for an attribute, or adding attributes.
	''' In a case-sensitive Attributes, the case is significant.
	''' <p>
	''' Note that updates to Attributes (such as adding or removing an attribute)
	''' do not affect the corresponding representation in the directory.
	''' Updates to the directory can only be effected
	''' using operations in the DirContext interface.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= DirContext#getAttributes </seealso>
	''' <seealso cref= DirContext#modifyAttributes </seealso>
	''' <seealso cref= DirContext#bind </seealso>
	''' <seealso cref= DirContext#rebind </seealso>
	''' <seealso cref= DirContext#createSubcontext </seealso>
	''' <seealso cref= DirContext#search </seealso>
	''' <seealso cref= BasicAttributes
	''' @since 1.3 </seealso>

	Public Interface Attributes
		Inherits ICloneable, java.io.Serializable

		''' <summary>
		''' Determines whether the attribute set ignores the case of
		''' attribute identifiers when retrieving or adding attributes. </summary>
		''' <returns> true if case is ignored; false otherwise. </returns>
		ReadOnly Property caseIgnored As Boolean

		''' <summary>
		''' Retrieves the number of attributes in the attribute set.
		''' </summary>
		''' <returns> The nonnegative number of attributes in this attribute set. </returns>
		Function size() As Integer

		''' <summary>
		''' Retrieves the attribute with the given attribute id from the
		''' attribute set.
		''' </summary>
		''' <param name="attrID"> The non-null id of the attribute to retrieve.
		'''           If this attribute set ignores the character
		'''           case of its attribute ids, the case of attrID
		'''           is ignored. </param>
		''' <returns> The attribute identified by attrID; null if not found. </returns>
		''' <seealso cref= #put </seealso>
		''' <seealso cref= #remove </seealso>
		Function [get](ByVal attrID As String) As Attribute

		''' <summary>
		''' Retrieves an enumeration of the attributes in the attribute set.
		''' The effects of updates to this attribute set on this enumeration
		''' are undefined.
		''' </summary>
		''' <returns> A non-null enumeration of the attributes in this attribute set.
		'''         Each element of the enumeration is of class <tt>Attribute</tt>.
		'''         If attribute set has zero attributes, an empty enumeration
		'''         is returned. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property all As javax.naming.NamingEnumeration(Of ? As Attribute)

		''' <summary>
		''' Retrieves an enumeration of the ids of the attributes in the
		''' attribute set.
		''' The effects of updates to this attribute set on this enumeration
		''' are undefined.
		''' </summary>
		''' <returns> A non-null enumeration of the attributes' ids in
		'''         this attribute set. Each element of the enumeration is
		'''         of class String.
		'''         If attribute set has zero attributes, an empty enumeration
		'''         is returned. </returns>
		ReadOnly Property iDs As javax.naming.NamingEnumeration(Of String)

		''' <summary>
		''' Adds a new attribute to the attribute set.
		''' </summary>
		''' <param name="attrID">   non-null The id of the attribute to add.
		'''           If the attribute set ignores the character
		'''           case of its attribute ids, the case of attrID
		'''           is ignored. </param>
		''' <param name="val">      The possibly null value of the attribute to add.
		'''                 If null, the attribute does not have any values. </param>
		''' <returns> The Attribute with attrID that was previous in this attribute set;
		'''         null if no such attribute existed. </returns>
		''' <seealso cref= #remove </seealso>
		Function put(ByVal attrID As String, ByVal val As Object) As Attribute

		''' <summary>
		''' Adds a new attribute to the attribute set.
		''' </summary>
		''' <param name="attr">     The non-null attribute to add.
		'''                 If the attribute set ignores the character
		'''                 case of its attribute ids, the case of
		'''                 attr's identifier is ignored. </param>
		''' <returns> The Attribute with the same ID as attr that was previous
		'''         in this attribute set;
		'''         null if no such attribute existed. </returns>
		''' <seealso cref= #remove </seealso>
		Function put(ByVal attr As Attribute) As Attribute

		''' <summary>
		''' Removes the attribute with the attribute id 'attrID' from
		''' the attribute set. If the attribute does not exist, ignore.
		''' </summary>
		''' <param name="attrID">   The non-null id of the attribute to remove.
		'''                 If the attribute set ignores the character
		'''                 case of its attribute ids, the case of
		'''                 attrID is ignored. </param>
		''' <returns> The Attribute with the same ID as attrID that was previous
		'''         in the attribute set;
		'''         null if no such attribute existed. </returns>
		Function remove(ByVal attrID As String) As Attribute

		''' <summary>
		''' Makes a copy of the attribute set.
		''' The new set contains the same attributes as the original set:
		''' the attributes are not themselves cloned.
		''' Changes to the copy will not affect the original and vice versa.
		''' </summary>
		''' <returns> A non-null copy of this attribute set. </returns>
		Function clone() As Object

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		' static final long serialVersionUID = -7247874645443605347L;
	End Interface

End Namespace
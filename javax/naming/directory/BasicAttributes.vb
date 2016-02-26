Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' This class provides a basic implementation
	''' of the Attributes interface.
	''' <p>
	''' BasicAttributes is either case-sensitive or case-insensitive (case-ignore).
	''' This property is determined at the time the BasicAttributes constructor
	''' is called.
	''' In a case-insensitive BasicAttributes, the case of its attribute identifiers
	''' is ignored when searching for an attribute, or adding attributes.
	''' In a case-sensitive BasicAttributes, the case is significant.
	''' <p>
	''' When the BasicAttributes class needs to create an Attribute, it
	''' uses BasicAttribute. There is no other dependency on BasicAttribute.
	''' <p>
	''' Note that updates to BasicAttributes (such as adding or removing an attribute)
	''' does not affect the corresponding representation in the directory.
	''' Updates to the directory can only be effected
	''' using operations in the DirContext interface.
	''' <p>
	''' A BasicAttributes instance is not synchronized against concurrent
	''' multithreaded access. Multiple threads trying to access and modify
	''' a single BasicAttributes instance should lock the object.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= DirContext#getAttributes </seealso>
	''' <seealso cref= DirContext#modifyAttributes </seealso>
	''' <seealso cref= DirContext#bind </seealso>
	''' <seealso cref= DirContext#rebind </seealso>
	''' <seealso cref= DirContext#createSubcontext </seealso>
	''' <seealso cref= DirContext#search
	''' @since 1.3 </seealso>

	Public Class BasicAttributes
		Implements Attributes

		''' <summary>
		''' Indicates whether case of attribute ids is ignored.
		''' @serial
		''' </summary>
		Private ignoreCase As Boolean = False

		' The 'key' in attrs is stored in the 'right case'.
		' If ignoreCase is true, key is aways lowercase.
		' If ignoreCase is false, key is stored as supplied by put().
		' %%% Not declared "private" due to bug 4064984.
		<NonSerialized> _
		Friend attrs As New Dictionary(Of String, Attribute)(11)

		''' <summary>
		''' Constructs a new instance of Attributes.
		''' The character case of attribute identifiers
		''' is significant when subsequently retrieving or adding attributes.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new instance of Attributes.
		''' If <code>ignoreCase</code> is true, the character case of attribute
		''' identifiers is ignored; otherwise the case is significant. </summary>
		''' <param name="ignoreCase"> true means this attribute set will ignore
		'''                   the case of its attribute identifiers
		'''                   when retrieving or adding attributes;
		'''                   false means case is respected. </param>
		Public Sub New(ByVal ignoreCase As Boolean)
			Me.ignoreCase = ignoreCase
		End Sub

		''' <summary>
		''' Constructs a new instance of Attributes with one attribute.
		''' The attribute specified by attrID and val are added to the newly
		''' created attribute.
		''' The character case of attribute identifiers
		''' is significant when subsequently retrieving or adding attributes. </summary>
		''' <param name="attrID">   non-null The id of the attribute to add. </param>
		''' <param name="val"> The value of the attribute to add. If null, a null
		'''        value is added to the attribute. </param>
		Public Sub New(ByVal attrID As String, ByVal val As Object)
			Me.New()
			Me.put(New BasicAttribute(attrID, val))
		End Sub

		''' <summary>
		''' Constructs a new instance of Attributes with one attribute.
		''' The attribute specified by attrID and val are added to the newly
		''' created attribute.
		''' If <code>ignoreCase</code> is true, the character case of attribute
		''' identifiers is ignored; otherwise the case is significant. </summary>
		''' <param name="attrID">   non-null The id of the attribute to add.
		'''           If this attribute set ignores the character
		'''           case of its attribute ids, the case of attrID
		'''           is ignored. </param>
		''' <param name="val"> The value of the attribute to add. If null, a null
		'''        value is added to the attribute. </param>
		''' <param name="ignoreCase"> true means this attribute set will ignore
		'''                   the case of its attribute identifiers
		'''                   when retrieving or adding attributes;
		'''                   false means case is respected. </param>
		Public Sub New(ByVal attrID As String, ByVal val As Object, ByVal ignoreCase As Boolean)
			Me.New(ignoreCase)
			Me.put(New BasicAttribute(attrID, val))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function clone() As Object Implements Attributes.clone
			Dim attrset As BasicAttributes
			Try
				attrset = CType(MyBase.clone(), BasicAttributes)
			Catch e As CloneNotSupportedException
				attrset = New BasicAttributes(ignoreCase)
			End Try
			attrset.attrs = CType(attrs.clone(), Dictionary(Of String, Attribute))
			Return attrset
		End Function

		Public Overridable Property caseIgnored As Boolean Implements Attributes.isCaseIgnored
			Get
				Return ignoreCase
			End Get
		End Property

		Public Overridable Function size() As Integer Implements Attributes.size
			Return attrs.Count
		End Function

		Public Overridable Function [get](ByVal attrID As String) As Attribute Implements Attributes.get
			Dim attr As Attribute = attrs(If(ignoreCase, attrID.ToLower(java.util.Locale.ENGLISH), attrID))
			Return (attr)
		End Function

		Public Overridable Property all As javax.naming.NamingEnumeration(Of Attribute) Implements Attributes.getAll
			Get
				Return New AttrEnumImpl(Me)
			End Get
		End Property

		Public Overridable Property iDs As javax.naming.NamingEnumeration(Of String) Implements Attributes.getIDs
			Get
				Return New IDEnumImpl(Me)
			End Get
		End Property

		Public Overridable Function put(ByVal attrID As String, ByVal val As Object) As Attribute Implements Attributes.put
			Return Me.put(New BasicAttribute(attrID, val))
		End Function

		Public Overridable Function put(ByVal attr As Attribute) As Attribute Implements Attributes.put
			Dim id As String = attr.iD
			If ignoreCase Then id = id.ToLower(java.util.Locale.ENGLISH)
				attrs(id) = attr
				Return attrs(id)
		End Function

		Public Overridable Function remove(ByVal attrID As String) As Attribute Implements Attributes.remove
			Dim id As String = (If(ignoreCase, attrID.ToLower(java.util.Locale.ENGLISH), attrID))
			Return attrs.Remove(id)
		End Function

		''' <summary>
		''' Generates the string representation of this attribute set.
		''' The string consists of each attribute identifier and the contents
		''' of each attribute. The contents of this string is useful
		''' for debugging and is not meant to be interpreted programmatically.
		''' </summary>
		''' <returns> A non-null string listing the contents of this attribute set. </returns>
		Public Overrides Function ToString() As String
			If attrs.Count = 0 Then
				Return ("No attributes")
			Else
				Return attrs.ToString()
			End If
		End Function

		''' <summary>
		''' Determines whether this <tt>BasicAttributes</tt> is equal to another
		''' <tt>Attributes</tt>
		''' Two <tt>Attributes</tt> are equal if they are both instances of
		''' <tt>Attributes</tt>,
		''' treat the case of attribute IDs the same way, and contain the
		''' same attributes. Each <tt>Attribute</tt> in this <tt>BasicAttributes</tt>
		''' is checked for equality using <tt>Object.equals()</tt>, which may have
		''' be overridden by implementations of <tt>Attribute</tt>).
		''' If a subclass overrides <tt>equals()</tt>,
		''' it should override <tt>hashCode()</tt>
		''' as well so that two <tt>Attributes</tt> instances that are equal
		''' have the same hash code. </summary>
		''' <param name="obj"> the possibly null object to compare against.
		''' </param>
		''' <returns> true If obj is equal to this BasicAttributes. </returns>
		''' <seealso cref= #hashCode </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If (obj IsNot Nothing) AndAlso (TypeOf obj Is Attributes) Then
				Dim target As Attributes = CType(obj, Attributes)

				' Check case first
				If ignoreCase <> target.caseIgnored Then Return False

				If size() = target.size() Then
					Dim their, mine As Attribute
					Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim theirs As javax.naming.NamingEnumeration(Of ?) = target.all
						Do While theirs.hasMore()
							their = CType(theirs.next(), Attribute)
							mine = [get](their.iD)
							If Not their.Equals(mine) Then Return False
						Loop
					Catch e As javax.naming.NamingException
						Return False
					End Try
					Return True
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Calculates the hash code of this BasicAttributes.
		''' <p>
		''' The hash code is computed by adding the hash code of
		''' the attributes of this object. If this BasicAttributes
		''' ignores case of its attribute IDs, one is added to the hash code.
		''' If a subclass overrides <tt>hashCode()</tt>,
		''' it should override <tt>equals()</tt>
		''' as well so that two <tt>Attributes</tt> instances that are equal
		''' have the same hash code.
		''' </summary>
		''' <returns> an int representing the hash code of this BasicAttributes instance. </returns>
		''' <seealso cref= #equals </seealso>
		Public Overrides Function GetHashCode() As Integer
			Dim hash As Integer = (If(ignoreCase, 1, 0))
			Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim ___all As javax.naming.NamingEnumeration(Of ?) = all
				Do While ___all.hasMore()
					hash += ___all.next().GetHashCode()
				Loop
			Catch e As javax.naming.NamingException
			End Try
			Return hash
		End Function

		''' <summary>
		''' Overridden to avoid exposing implementation details.
		''' @serialData Default field (ignoreCase flag -- a boolean), followed by
		''' the number of attributes in the set
		''' (an int), and then the individual Attribute objects.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject() ' write out the ignoreCase flag
			s.writeInt(attrs.Count)
			Dim attrEnum As System.Collections.IEnumerator(Of Attribute) = attrs.Values.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While attrEnum.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				s.writeObject(attrEnum.nextElement())
			Loop
		End Sub

		''' <summary>
		''' Overridden to avoid exposing implementation details.
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject() ' read in the ignoreCase flag
			Dim n As Integer = s.readInt() ' number of attributes
			attrs = If(n >= 1, New Dictionary(Of String, Attribute)(n * 2), New Dictionary(Of String, Attribute)(2)) ' can't have initial size of 0 (grrr...)
			n -= 1
			Do While n >= 0
				put(CType(s.readObject(), Attribute))
				n -= 1
			Loop
		End Sub


	Friend Class AttrEnumImpl
		Implements javax.naming.NamingEnumeration(Of Attribute)

			Private ReadOnly outerInstance As BasicAttributes


		Friend elements As System.Collections.IEnumerator(Of Attribute)

		Public Sub New(ByVal outerInstance As BasicAttributes)
				Me.outerInstance = outerInstance
			Me.elements = outerInstance.attrs.Values.GetEnumerator()
		End Sub

		Public Overridable Function hasMoreElements() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return elements.hasMoreElements()
		End Function

		Public Overridable Function nextElement() As Attribute
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return elements.nextElement()
		End Function

		Public Overridable Function hasMore() As Boolean
			Return hasMoreElements()
		End Function

		Public Overridable Function [next]() As Attribute
			Return nextElement()
		End Function

		Public Overridable Sub close()
			elements = Nothing
		End Sub
	End Class

	Friend Class IDEnumImpl
		Implements javax.naming.NamingEnumeration(Of String)

			Private ReadOnly outerInstance As BasicAttributes


		Friend elements As System.Collections.IEnumerator(Of Attribute)

		Public Sub New(ByVal outerInstance As BasicAttributes)
				Me.outerInstance = outerInstance
			' Walking through the elements, rather than the keys, gives
			' us attribute IDs that have not been converted to lowercase.
			Me.elements = outerInstance.attrs.Values.GetEnumerator()
		End Sub

		Public Overridable Function hasMoreElements() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return elements.hasMoreElements()
		End Function

		Public Overridable Function nextElement() As String
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim attr As Attribute = elements.nextElement()
			Return attr.iD
		End Function

		Public Overridable Function hasMore() As Boolean
			Return hasMoreElements()
		End Function

		Public Overridable Function [next]() As String
			Return nextElement()
		End Function

		Public Overridable Sub close()
			elements = Nothing
		End Sub
	End Class

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability.
		''' </summary>
		Private Const serialVersionUID As Long = 4980164073184639448L
	End Class

End Namespace
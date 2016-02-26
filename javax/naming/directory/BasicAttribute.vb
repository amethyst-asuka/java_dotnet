Imports System
Imports System.Collections.Generic
Imports System.Text

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
	''' This class provides a basic implementation of the <tt>Attribute</tt> interface.
	''' <p>
	''' This implementation does not support the schema methods
	''' <tt>getAttributeDefinition()</tt> and <tt>getAttributeSyntaxDefinition()</tt>.
	''' They simply throw <tt>OperationNotSupportedException</tt>.
	''' Subclasses of <tt>BasicAttribute</tt> should override these methods if they
	''' support them.
	''' <p>
	''' The <tt>BasicAttribute</tt> class by default uses <tt>Object.equals()</tt> to
	''' determine equality of attribute values when testing for equality or
	''' when searching for values, <em>except</em> when the value is an array.
	''' For an array, each element of the array is checked using <tt>Object.equals()</tt>.
	''' Subclasses of <tt>BasicAttribute</tt> can make use of schema information
	''' when doing similar equality checks by overriding methods
	''' in which such use of schema is meaningful.
	''' Similarly, the <tt>BasicAttribute</tt> class by default returns the values passed to its
	''' constructor and/or manipulated using the add/remove methods.
	''' Subclasses of <tt>BasicAttribute</tt> can override <tt>get()</tt> and <tt>getAll()</tt>
	''' to get the values dynamically from the directory (or implement
	''' the <tt>Attribute</tt> interface directly instead of subclassing <tt>BasicAttribute</tt>).
	''' <p>
	''' Note that updates to <tt>BasicAttribute</tt> (such as adding or removing a value)
	''' does not affect the corresponding representation of the attribute
	''' in the directory.  Updates to the directory can only be effected
	''' using operations in the <tt>DirContext</tt> interface.
	''' <p>
	''' A <tt>BasicAttribute</tt> instance is not synchronized against concurrent
	''' multithreaded access. Multiple threads trying to access and modify a
	''' <tt>BasicAttribute</tt> should lock the object.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>
	Public Class BasicAttribute
		Implements Attribute

		''' <summary>
		''' Holds the attribute's id. It is initialized by the public constructor and
		''' cannot be null unless methods in BasicAttribute that use attrID
		''' have been overridden.
		''' @serial
		''' </summary>
		Protected Friend attrID As String

		''' <summary>
		''' Holds the attribute's values. Initialized by public constructors.
		''' Cannot be null unless methods in BasicAttribute that use
		''' values have been overridden.
		''' </summary>
		<NonSerialized> _
		Protected Friend values As List(Of Object)

		''' <summary>
		''' A flag for recording whether this attribute's values are ordered.
		''' @serial
		''' </summary>
		Protected Friend ordered As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function clone() As Object Implements Attribute.clone
			Dim attr As BasicAttribute
			Try
				attr = CType(MyBase.clone(), BasicAttribute)
			Catch e As CloneNotSupportedException
				attr = New BasicAttribute(attrID, ordered)
			End Try
			attr.values = CType(values.clone(), List(Of Object))
			Return attr
		End Function

		''' <summary>
		''' Determines whether obj is equal to this attribute.
		''' Two attributes are equal if their attribute-ids, syntaxes
		''' and values are equal.
		''' If the attribute values are unordered, the order that the values were added
		''' are irrelevant. If the attribute values are ordered, then the
		''' order the values must match.
		''' If obj is null or not an Attribute, false is returned.
		''' <p>
		''' By default <tt>Object.equals()</tt> is used when comparing the attribute
		''' id and its values except when a value is an array. For an array,
		''' each element of the array is checked using <tt>Object.equals()</tt>.
		''' A subclass may override this to make
		''' use of schema syntax information and matching rules,
		''' which define what it means for two attributes to be equal.
		''' How and whether a subclass makes
		''' use of the schema information is determined by the subclass.
		''' If a subclass overrides <tt>equals()</tt>, it should also override
		''' <tt>hashCode()</tt>
		''' such that two attributes that are equal have the same hash code.
		''' </summary>
		''' <param name="obj">      The possibly null object to check. </param>
		''' <returns> true if obj is equal to this attribute; false otherwise. </returns>
		''' <seealso cref= #hashCode </seealso>
		''' <seealso cref= #contains </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If (obj IsNot Nothing) AndAlso (TypeOf obj Is Attribute) Then
				Dim target As Attribute = CType(obj, Attribute)

				' Check order first
				If ordered <> target.ordered Then Return False
				Dim len As Integer
				len=size()
				If attrID.Equals(target.iD) AndAlso len = target.size() Then
					Try
						If ordered Then
							' Go through both list of values
							For i As Integer = 0 To len - 1
								If Not valueEquals([get](i), target.get(i)) Then Return False
							Next i
						Else
							' order is not relevant; check for existence
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Dim theirs As System.Collections.IEnumerator(Of ?) = target.all
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							Do While theirs.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								If find(theirs.nextElement()) < 0 Then Return False
							Loop
						End If
					Catch e As javax.naming.NamingException
						Return False
					End Try
					Return True
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Calculates the hash code of this attribute.
		''' <p>
		''' The hash code is computed by adding the hash code of
		''' the attribute's id and that of all of its values except for
		''' values that are arrays.
		''' For an array, the hash code of each element of the array is summed.
		''' If a subclass overrides <tt>hashCode()</tt>, it should override
		''' <tt>equals()</tt>
		''' as well so that two attributes that are equal have the same hash code.
		''' </summary>
		''' <returns> an int representing the hash code of this attribute. </returns>
		''' <seealso cref= #equals </seealso>
		Public Overrides Function GetHashCode() As Integer
			Dim hash As Integer = attrID.GetHashCode()
			Dim num As Integer = values.Count
			Dim val As Object
			For i As Integer = 0 To num - 1
				val = values(i)
				If val IsNot Nothing Then
					If val.GetType().IsArray Then
						Dim it As Object
						Dim len As Integer = Array.getLength(val)
						For j As Integer = 0 To len - 1
							it = Array.get(val, j)
							If it IsNot Nothing Then hash += it.GetHashCode()
						Next j
					Else
						hash += val.GetHashCode()
					End If
				End If
			Next i
			Return hash
		End Function

		''' <summary>
		''' Generates the string representation of this attribute.
		''' The string consists of the attribute's id and its values.
		''' This string is meant for debugging and not meant to be
		''' interpreted programmatically. </summary>
		''' <returns> The non-null string representation of this attribute. </returns>
		Public Overrides Function ToString() As String
			Dim answer As New StringBuilder(attrID & ": ")
			If values.Count = 0 Then
				answer.Append("No values")
			Else
				Dim start As Boolean = True
				Dim e As System.Collections.IEnumerator(Of Object) = values.elements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While e.hasMoreElements()
					If Not start Then answer.Append(", ")
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					answer.Append(e.nextElement())
					start = False
				Loop
			End If
			Return answer.ToString()
		End Function

		''' <summary>
		''' Constructs a new instance of an unordered attribute with no value.
		''' </summary>
		''' <param name="id"> The attribute's id. It cannot be null. </param>
		Public Sub New(ByVal id As String)
			Me.New(id, False)
		End Sub

		''' <summary>
		''' Constructs a new instance of an unordered attribute with a single value.
		''' </summary>
		''' <param name="id"> The attribute's id. It cannot be null. </param>
		''' <param name="value"> The attribute's value. If null, a null
		'''        value is added to the attribute. </param>
		Public Sub New(ByVal id As String, ByVal value As Object)
			Me.New(id, value, False)
		End Sub

		''' <summary>
		''' Constructs a new instance of a possibly ordered attribute with no value.
		''' </summary>
		''' <param name="id"> The attribute's id. It cannot be null. </param>
		''' <param name="ordered"> true means the attribute's values will be ordered;
		''' false otherwise. </param>
		Public Sub New(ByVal id As String, ByVal ordered As Boolean)
			attrID = id
			values = New List(Of )
			Me.ordered = ordered
		End Sub

		''' <summary>
		''' Constructs a new instance of a possibly ordered attribute with a
		''' single value.
		''' </summary>
		''' <param name="id"> The attribute's id. It cannot be null. </param>
		''' <param name="value"> The attribute's value. If null, a null
		'''        value is added to the attribute. </param>
		''' <param name="ordered"> true means the attribute's values will be ordered;
		''' false otherwise. </param>
		Public Sub New(ByVal id As String, ByVal value As Object, ByVal ordered As Boolean)
			Me.New(id, ordered)
			values.Add(value)
		End Sub

		''' <summary>
		''' Retrieves an enumeration of this attribute's values.
		''' <p>
		''' By default, the values returned are those passed to the
		''' constructor and/or manipulated using the add/replace/remove methods.
		''' A subclass may override this to retrieve the values dynamically
		''' from the directory.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property all As javax.naming.NamingEnumeration(Of ?) Implements Attribute.getAll
			Get
			  Return New ValuesEnumImpl(Me)
			End Get
		End Property

		''' <summary>
		''' Retrieves one of this attribute's values.
		''' <p>
		''' By default, the value returned is one of those passed to the
		''' constructor and/or manipulated using the add/replace/remove methods.
		''' A subclass may override this to retrieve the value dynamically
		''' from the directory.
		''' </summary>
		Public Overridable Function [get]() As Object Implements Attribute.get
			If values.Count = 0 Then
				Throw New java.util.NoSuchElementException("Attribute " & iD & " has no value")
			Else
				Return values(0)
			End If
		End Function

		Public Overridable Function size() As Integer Implements Attribute.size
		  Return values.Count
		End Function

		Public Overridable Property iD As String Implements Attribute.getID
			Get
				Return attrID
			End Get
		End Property

		''' <summary>
		''' Determines whether a value is in this attribute.
		''' <p>
		''' By default,
		''' <tt>Object.equals()</tt> is used when comparing <tt>attrVal</tt>
		''' with this attribute's values except when <tt>attrVal</tt> is an array.
		''' For an array, each element of the array is checked using
		''' <tt>Object.equals()</tt>.
		''' A subclass may use schema information to determine equality.
		''' </summary>
		Public Overridable Function contains(ByVal attrVal As Object) As Boolean Implements Attribute.contains
			Return (find(attrVal) >= 0)
		End Function

		' For finding first element that has a null in JDK1.1 Vector.
		' In the Java 2 platform, can just replace this with Vector.indexOf(target);
		Private Function find(ByVal target As Object) As Integer
			Dim cl As Type
			If target Is Nothing Then
				Dim ct As Integer = values.Count
				For i As Integer = 0 To ct - 1
					If values(i) Is Nothing Then Return i
				Next i
			Else
				cl=target.GetType()
				If cl.IsArray Then
					Dim ct As Integer = values.Count
					Dim it As Object
					For i As Integer = 0 To ct - 1
						it = values(i)
						If it IsNot Nothing AndAlso cl Is it.GetType() AndAlso arrayEquals(target, it) Then Return i
					Next i
				Else
					Return values.IndexOf(target, 0)
				End If
				End If
			Return -1 ' not found
		End Function

		''' <summary>
		''' Determines whether two attribute values are equal.
		''' Use arrayEquals for arrays and <tt>Object.equals()</tt> otherwise.
		''' </summary>
		Private Shared Function valueEquals(ByVal obj1 As Object, ByVal obj2 As Object) As Boolean
			If obj1 Is obj2 Then Return True ' object references are equal
			If obj1 Is Nothing Then Return False ' obj2 was not false
			If obj1.GetType().IsArray AndAlso obj2.GetType().IsArray Then Return arrayEquals(obj1, obj2)
			Return (obj1.Equals(obj2))
		End Function

		''' <summary>
		''' Determines whether two arrays are equal by comparing each of their
		''' elements using <tt>Object.equals()</tt>.
		''' </summary>
		Private Shared Function arrayEquals(ByVal a1 As Object, ByVal a2 As Object) As Boolean
			Dim len As Integer
			len = Array.getLength(a1)
			If len <> Array.getLength(a2) Then Return False

			For j As Integer = 0 To len - 1
				Dim i1 As Object = Array.get(a1, j)
				Dim i2 As Object = Array.get(a2, j)
				If i1 Is Nothing OrElse i2 Is Nothing Then
					If i1 IsNot i2 Then Return False
				ElseIf Not i1.Equals(i2) Then
					Return False
				End If
			Next j
			Return True
		End Function

		''' <summary>
		''' Adds a new value to this attribute.
		''' <p>
		''' By default, <tt>Object.equals()</tt> is used when comparing <tt>attrVal</tt>
		''' with this attribute's values except when <tt>attrVal</tt> is an array.
		''' For an array, each element of the array is checked using
		''' <tt>Object.equals()</tt>.
		''' A subclass may use schema information to determine equality.
		''' </summary>
		Public Overridable Function add(ByVal attrVal As Object) As Boolean Implements Attribute.add
			If ordered OrElse (find(attrVal) < 0) Then
				values.Add(attrVal)
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Removes a specified value from this attribute.
		''' <p>
		''' By default, <tt>Object.equals()</tt> is used when comparing <tt>attrVal</tt>
		''' with this attribute's values except when <tt>attrVal</tt> is an array.
		''' For an array, each element of the array is checked using
		''' <tt>Object.equals()</tt>.
		''' A subclass may use schema information to determine equality.
		''' </summary>
		Public Overridable Function remove(ByVal attrval As Object) As Boolean Implements Attribute.remove
			' For the Java 2 platform, can just use "return removeElement(attrval);"
			' Need to do the following to handle null case

			Dim i As Integer = find(attrval)
			If i >= 0 Then
				values.RemoveAt(i)
				Return True
			End If
			Return False
		End Function

		Public Overridable Sub clear() Implements Attribute.clear
			values.Capacity = 0
		End Sub

	'  ---- ordering methods

		Public Overridable Property ordered As Boolean Implements Attribute.isOrdered
			Get
				Return ordered
			End Get
		End Property

		Public Overridable Function [get](ByVal ix As Integer) As Object Implements Attribute.get
			Return values(ix)
		End Function

		Public Overridable Function remove(ByVal ix As Integer) As Object Implements Attribute.remove
			Dim answer As Object = values(ix)
			values.RemoveAt(ix)
			Return answer
		End Function

		Public Overridable Sub add(ByVal ix As Integer, ByVal attrVal As Object) Implements Attribute.add
			If (Not ordered) AndAlso contains(attrVal) Then Throw New IllegalStateException("Cannot add duplicate to unordered attribute")
			values.Insert(ix, attrVal)
		End Sub

		Public Overridable Function [set](ByVal ix As Integer, ByVal attrVal As Object) As Object Implements Attribute.set
			If (Not ordered) AndAlso contains(attrVal) Then Throw New IllegalStateException("Cannot add duplicate to unordered attribute")

			Dim answer As Object = values(ix)
			values(ix) = attrVal
			Return answer
		End Function

	' ----------------- Schema methods

		''' <summary>
		''' Retrieves the syntax definition associated with this attribute.
		''' <p>
		''' This method by default throws OperationNotSupportedException. A subclass
		''' should override this method if it supports schema.
		''' </summary>
		Public Overridable Property attributeSyntaxDefinition As DirContext Implements Attribute.getAttributeSyntaxDefinition
			Get
					Throw New javax.naming.OperationNotSupportedException("attribute syntax")
			End Get
		End Property

		''' <summary>
		''' Retrieves this attribute's schema definition.
		''' <p>
		''' This method by default throws OperationNotSupportedException. A subclass
		''' should override this method if it supports schema.
		''' </summary>
		Public Overridable Property attributeDefinition As DirContext Implements Attribute.getAttributeDefinition
			Get
				Throw New javax.naming.OperationNotSupportedException("attribute definition")
			End Get
		End Property


	'  ---- serialization methods

		''' <summary>
		''' Overridden to avoid exposing implementation details
		''' @serialData Default field (the attribute ID -- a String),
		''' followed by the number of values (an int), and the
		''' individual values.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject() ' write out the attrID
			s.writeInt(values.Count)
			For i As Integer = 0 To values.Count - 1
				s.writeObject(values(i))
			Next i
		End Sub

		''' <summary>
		''' Overridden to avoid exposing implementation details.
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject() ' read in the attrID
			Dim n As Integer = s.readInt() ' number of values
			values = New List(Of )(n)
			n -= 1
			Do While n >= 0
				values.Add(s.readObject())
				n -= 1
			Loop
		End Sub


		Friend Class ValuesEnumImpl
			Implements javax.naming.NamingEnumeration(Of Object)

			Private ReadOnly outerInstance As BasicAttribute

			Friend list As System.Collections.IEnumerator(Of Object)

			Friend Sub New(ByVal outerInstance As BasicAttribute)
					Me.outerInstance = outerInstance
				list = outerInstance.values.elements()
			End Sub

			Public Overridable Function hasMoreElements() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return list.hasMoreElements()
			End Function

			Public Overridable Function nextElement() As Object
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return (list.nextElement())
			End Function

			Public Overridable Function [next]() As Object
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return list.nextElement()
			End Function

			Public Overridable Function hasMore() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return list.hasMoreElements()
			End Function

			Public Overridable Sub close()
				list = Nothing
			End Sub
		End Class

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability.
		''' </summary>
		Private Const serialVersionUID As Long = 6743528196119291326L
	End Class

End Namespace
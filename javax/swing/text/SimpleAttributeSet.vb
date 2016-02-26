Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text


	''' <summary>
	''' A straightforward implementation of MutableAttributeSet using a
	''' hash table.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Tim Prinzing
	''' </summary>
	<Serializable> _
	Public Class SimpleAttributeSet
		Implements MutableAttributeSet, ICloneable

		Private Const serialVersionUID As Long = -6631553454711782652L

		''' <summary>
		''' An empty attribute set.
		''' </summary>
		Public Shared ReadOnly EMPTY As AttributeSet = New EmptyAttributeSet

		<NonSerialized> _
		Private table As New java.util.LinkedHashMap(Of Object, Object)(3)

		''' <summary>
		''' Creates a new attribute set.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new attribute set based on a supplied set of attributes.
		''' </summary>
		''' <param name="source"> the set of attributes </param>
		Public Sub New(ByVal source As AttributeSet)
			addAttributes(source)
		End Sub

		''' <summary>
		''' Checks whether the set of attributes is empty.
		''' </summary>
		''' <returns> true if the set is empty else false </returns>
		Public Overridable Property empty As Boolean Implements AttributeSet.isEmpty
			Get
				Return table.empty
			End Get
		End Property

		''' <summary>
		''' Gets a count of the number of attributes.
		''' </summary>
		''' <returns> the count </returns>
		Public Overridable Property attributeCount As Integer Implements AttributeSet.getAttributeCount
			Get
				Return table.size()
			End Get
		End Property

		''' <summary>
		''' Tells whether a given attribute is defined.
		''' </summary>
		''' <param name="attrName"> the attribute name </param>
		''' <returns> true if the attribute is defined </returns>
		Public Overridable Function isDefined(ByVal attrName As Object) As Boolean Implements AttributeSet.isDefined
			Return table.containsKey(attrName)
		End Function

		''' <summary>
		''' Compares two attribute sets.
		''' </summary>
		''' <param name="attr"> the second attribute set </param>
		''' <returns> true if the sets are equal, false otherwise </returns>
		Public Overridable Function isEqual(ByVal attr As AttributeSet) As Boolean Implements AttributeSet.isEqual
			Return ((attributeCount = attr.attributeCount) AndAlso containsAttributes(attr))
		End Function

		''' <summary>
		''' Makes a copy of the attributes.
		''' </summary>
		''' <returns> the copy </returns>
		Public Overridable Function copyAttributes() As AttributeSet Implements AttributeSet.copyAttributes
			Return CType(clone(), AttributeSet)
		End Function

		''' <summary>
		''' Gets the names of the attributes in the set.
		''' </summary>
		''' <returns> the names as an <code>Enumeration</code> </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property attributeNames As System.Collections.IEnumerator(Of ?) Implements AttributeSet.getAttributeNames
			Get
				Return java.util.Collections.enumeration(table.Keys)
			End Get
		End Property

		''' <summary>
		''' Gets the value of an attribute.
		''' </summary>
		''' <param name="name"> the attribute name </param>
		''' <returns> the value </returns>
		Public Overridable Function getAttribute(ByVal name As Object) As Object Implements AttributeSet.getAttribute
			Dim value As Object = table.get(name)
			If value Is Nothing Then
				Dim parent As AttributeSet = resolveParent
				If parent IsNot Nothing Then value = parent.getAttribute(name)
			End If
			Return value
		End Function

		''' <summary>
		''' Checks whether the attribute list contains a
		''' specified attribute name/value pair.
		''' </summary>
		''' <param name="name"> the name </param>
		''' <param name="value"> the value </param>
		''' <returns> true if the name/value pair is in the list </returns>
		Public Overridable Function containsAttribute(ByVal name As Object, ByVal value As Object) As Boolean Implements AttributeSet.containsAttribute
			Return value.Equals(getAttribute(name))
		End Function

		''' <summary>
		''' Checks whether the attribute list contains all the
		''' specified name/value pairs.
		''' </summary>
		''' <param name="attributes"> the attribute list </param>
		''' <returns> true if the list contains all the name/value pairs </returns>
		Public Overridable Function containsAttributes(ByVal attributes As AttributeSet) As Boolean Implements AttributeSet.containsAttributes
			Dim result As Boolean = True

			Dim names As System.Collections.IEnumerator = attributes.attributeNames
			Do While result AndAlso names.hasMoreElements()
				Dim name As Object = names.nextElement()
				result = attributes.getAttribute(name).Equals(getAttribute(name))
			Loop

			Return result
		End Function

		''' <summary>
		''' Adds an attribute to the list.
		''' </summary>
		''' <param name="name"> the attribute name </param>
		''' <param name="value"> the attribute value </param>
		Public Overridable Sub addAttribute(ByVal name As Object, ByVal value As Object) Implements MutableAttributeSet.addAttribute
			table.put(name, value)
		End Sub

		''' <summary>
		''' Adds a set of attributes to the list.
		''' </summary>
		''' <param name="attributes"> the set of attributes to add </param>
		Public Overridable Sub addAttributes(ByVal attributes As AttributeSet) Implements MutableAttributeSet.addAttributes
			Dim names As System.Collections.IEnumerator = attributes.attributeNames
			Do While names.hasMoreElements()
				Dim name As Object = names.nextElement()
				addAttribute(name, attributes.getAttribute(name))
			Loop
		End Sub

		''' <summary>
		''' Removes an attribute from the list.
		''' </summary>
		''' <param name="name"> the attribute name </param>
		Public Overridable Sub removeAttribute(ByVal name As Object) Implements MutableAttributeSet.removeAttribute
			table.remove(name)
		End Sub

		''' <summary>
		''' Removes a set of attributes from the list.
		''' </summary>
		''' <param name="names"> the set of names to remove </param>
		Public Overridable Sub removeAttributes(Of T1)(ByVal names As System.Collections.IEnumerator(Of T1)) Implements MutableAttributeSet.removeAttributes
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While names.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				removeAttribute(names.nextElement())
			Loop
		End Sub

		''' <summary>
		''' Removes a set of attributes from the list.
		''' </summary>
		''' <param name="attributes"> the set of attributes to remove </param>
		Public Overridable Sub removeAttributes(ByVal attributes As AttributeSet) Implements MutableAttributeSet.removeAttributes
			If attributes Is Me Then
				table.clear()
			Else
				Dim names As System.Collections.IEnumerator = attributes.attributeNames
				Do While names.hasMoreElements()
					Dim name As Object = names.nextElement()
					Dim value As Object = attributes.getAttribute(name)
					If value.Equals(getAttribute(name)) Then removeAttribute(name)
				Loop
			End If
		End Sub

		''' <summary>
		''' Gets the resolving parent.  This is the set
		''' of attributes to resolve through if an attribute
		''' isn't defined locally.  This is null if there
		''' are no other sets of attributes to resolve
		''' through.
		''' </summary>
		''' <returns> the parent </returns>
		Public Overridable Property resolveParent As AttributeSet Implements AttributeSet.getResolveParent
			Get
				Return CType(table.get(StyleConstants.ResolveAttribute), AttributeSet)
			End Get
			Set(ByVal parent As AttributeSet)
				addAttribute(StyleConstants.ResolveAttribute, parent)
			End Set
		End Property


		' --- Object methods ---------------------------------

		''' <summary>
		''' Clones a set of attributes.
		''' </summary>
		''' <returns> the new set of attributes </returns>
		Public Overridable Function clone() As Object
			Dim attr As SimpleAttributeSet
			Try
				attr = CType(MyBase.clone(), SimpleAttributeSet)
				attr.table = CType(table.clone(), java.util.LinkedHashMap)
			Catch cnse As CloneNotSupportedException
				attr = Nothing
			End Try
			Return attr
		End Function

		''' <summary>
		''' Returns a hashcode for this set of attributes. </summary>
		''' <returns>     a hashcode value for this set of attributes. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return table.GetHashCode()
		End Function

		''' <summary>
		''' Compares this object to the specified object.
		''' The result is <code>true</code> if the object is an equivalent
		''' set of attributes. </summary>
		''' <param name="obj">   the object to compare this attribute set with </param>
		''' <returns>    <code>true</code> if the objects are equal;
		'''            <code>false</code> otherwise </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is AttributeSet Then
				Dim attrs As AttributeSet = CType(obj, AttributeSet)
				Return isEqual(attrs)
			End If
			Return False
		End Function

		''' <summary>
		''' Converts the attribute set to a String.
		''' </summary>
		''' <returns> the string </returns>
		Public Overrides Function ToString() As String
			Dim s As String = ""
			Dim names As System.Collections.IEnumerator = attributeNames
			Do While names.hasMoreElements()
				Dim key As Object = names.nextElement()
				Dim value As Object = getAttribute(key)
				If TypeOf value Is AttributeSet Then
					' don't go recursive
					s = s + key & "=**AttributeSet** "
				Else
					s = s + key & "=" & value & " "
				End If
			Loop
			Return s
		End Function

		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			StyleContext.writeAttributeSet(s, Me)
		End Sub

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			table = New java.util.LinkedHashMap(Of )(3)
			StyleContext.readAttributeSet(s, Me)
		End Sub

		''' <summary>
		''' An AttributeSet that is always empty.
		''' </summary>
		<Serializable> _
		Friend Class EmptyAttributeSet
			Implements AttributeSet

			Friend Const serialVersionUID As Long = -8714803568785904228L

			Public Overridable Property attributeCount As Integer Implements AttributeSet.getAttributeCount
				Get
					Return 0
				End Get
			End Property
			Public Overridable Function isDefined(ByVal attrName As Object) As Boolean Implements AttributeSet.isDefined
				Return False
			End Function
			Public Overridable Function isEqual(ByVal attr As AttributeSet) As Boolean Implements AttributeSet.isEqual
				Return (attr.attributeCount = 0)
			End Function
			Public Overridable Function copyAttributes() As AttributeSet Implements AttributeSet.copyAttributes
				Return Me
			End Function
			Public Overridable Function getAttribute(ByVal key As Object) As Object Implements AttributeSet.getAttribute
				Return Nothing
			End Function
			Public Overridable Property attributeNames As System.Collections.IEnumerator
				Get
					Return java.util.Collections.emptyEnumeration()
				End Get
			End Property
			Public Overridable Function containsAttribute(ByVal name As Object, ByVal value As Object) As Boolean Implements AttributeSet.containsAttribute
				Return False
			End Function
			Public Overridable Function containsAttributes(ByVal attributes As AttributeSet) As Boolean Implements AttributeSet.containsAttributes
				Return (attributes.attributeCount = 0)
			End Function
			Public Overridable Property resolveParent As AttributeSet Implements AttributeSet.getResolveParent
				Get
					Return Nothing
				End Get
			End Property
			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If Me Is obj Then Return True
				Return ((TypeOf obj Is AttributeSet) AndAlso (CType(obj, AttributeSet).attributeCount = 0))
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return 0
			End Function
		End Class
	End Class

End Namespace
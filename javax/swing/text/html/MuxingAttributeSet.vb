Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports javax.swing.text

'
' * Copyright (c) 2001, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html


	''' <summary>
	''' An implementation of <code>AttributeSet</code> that can multiplex
	''' across a set of <code>AttributeSet</code>s.
	''' 
	''' </summary>
	<Serializable> _
	Friend Class MuxingAttributeSet
		Implements AttributeSet

		''' <summary>
		''' Creates a <code>MuxingAttributeSet</code> with the passed in
		''' attributes.
		''' </summary>
		Public Sub New(ByVal attrs As AttributeSet())
			Me.attrs = attrs
		End Sub

		''' <summary>
		''' Creates an empty <code>MuxingAttributeSet</code>. This is intended for
		''' use by subclasses only, and it is also intended that subclasses will
		''' set the constituent <code>AttributeSet</code>s before invoking any
		''' of the <code>AttributeSet</code> methods.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Directly sets the <code>AttributeSet</code>s that comprise this
		''' <code>MuxingAttributeSet</code>.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Property attributes As AttributeSet()
			Set(ByVal attrs As AttributeSet())
				Me.attrs = attrs
			End Set
			Get
				Return attrs
			End Get
		End Property


		''' <summary>
		''' Inserts <code>as</code> at <code>index</code>. This assumes
		''' the value of <code>index</code> is between 0 and attrs.length,
		''' inclusive.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub insertAttributeSetAt(ByVal [as] As AttributeSet, ByVal index As Integer)
			Dim numAttrs As Integer = attrs.Length
			Dim newAttrs As AttributeSet() = New AttributeSet(numAttrs){}
			If index < numAttrs Then
				If index > 0 Then
					Array.Copy(attrs, 0, newAttrs, 0, index)
					Array.Copy(attrs, index, newAttrs, index + 1, numAttrs - index)
				Else
					Array.Copy(attrs, 0, newAttrs, 1, numAttrs)
				End If
			Else
				Array.Copy(attrs, 0, newAttrs, 0, numAttrs)
			End If
			newAttrs(index) = [as]
			attrs = newAttrs
		End Sub

		''' <summary>
		''' Removes the AttributeSet at <code>index</code>. This assumes
		''' the value of <code>index</code> is greater than or equal to 0,
		''' and less than attrs.length.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub removeAttributeSetAt(ByVal index As Integer)
			Dim numAttrs As Integer = attrs.Length
			Dim newAttrs As AttributeSet() = New AttributeSet(numAttrs - 2){}
			If numAttrs > 0 Then
				If index = 0 Then
					' FIRST
					Array.Copy(attrs, 1, newAttrs, 0, numAttrs - 1)
				ElseIf index < (numAttrs - 1) Then
					' MIDDLE
					Array.Copy(attrs, 0, newAttrs, 0, index)
					Array.Copy(attrs, index + 1, newAttrs, index, numAttrs - index - 1)
				Else
					' END
					Array.Copy(attrs, 0, newAttrs, 0, numAttrs - 1)
				End If
			End If
			attrs = newAttrs
		End Sub

		'  --- AttributeSet methods ----------------------------

		''' <summary>
		''' Gets the number of attributes that are defined.
		''' </summary>
		''' <returns> the number of attributes </returns>
		''' <seealso cref= AttributeSet#getAttributeCount </seealso>
		Public Overridable Property attributeCount As Integer Implements AttributeSet.getAttributeCount
			Get
				Dim [as] As AttributeSet() = attributes
				Dim n As Integer = 0
				For i As Integer = 0 To [as].Length - 1
					n += [as](i).attributeCount
				Next i
				Return n
			End Get
		End Property

		''' <summary>
		''' Checks whether a given attribute is defined.
		''' This will convert the key over to CSS if the
		''' key is a StyleConstants key that has a CSS
		''' mapping.
		''' </summary>
		''' <param name="key"> the attribute key </param>
		''' <returns> true if the attribute is defined </returns>
		''' <seealso cref= AttributeSet#isDefined </seealso>
		Public Overridable Function isDefined(ByVal key As Object) As Boolean Implements AttributeSet.isDefined
			Dim [as] As AttributeSet() = attributes
			For i As Integer = 0 To [as].Length - 1
				If [as](i).isDefined(key) Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Checks whether two attribute sets are equal.
		''' </summary>
		''' <param name="attr"> the attribute set to check against </param>
		''' <returns> true if the same </returns>
		''' <seealso cref= AttributeSet#isEqual </seealso>
		Public Overridable Function isEqual(ByVal attr As AttributeSet) As Boolean Implements AttributeSet.isEqual
			Return ((attributeCount = attr.attributeCount) AndAlso containsAttributes(attr))
		End Function

		''' <summary>
		''' Copies a set of attributes.
		''' </summary>
		''' <returns> the copy </returns>
		''' <seealso cref= AttributeSet#copyAttributes </seealso>
		Public Overridable Function copyAttributes() As AttributeSet Implements AttributeSet.copyAttributes
			Dim [as] As AttributeSet() = attributes
			Dim a As MutableAttributeSet = New SimpleAttributeSet
			Dim n As Integer = 0
			For i As Integer = [as].Length - 1 To 0 Step -1
				a.addAttributes([as](i))
			Next i
			Return a
		End Function

		''' <summary>
		''' Gets the value of an attribute.  If the requested
		''' attribute is a StyleConstants attribute that has
		''' a CSS mapping, the request will be converted.
		''' </summary>
		''' <param name="key"> the attribute name </param>
		''' <returns> the attribute value </returns>
		''' <seealso cref= AttributeSet#getAttribute </seealso>
		Public Overridable Function getAttribute(ByVal key As Object) As Object Implements AttributeSet.getAttribute
			Dim [as] As AttributeSet() = attributes
			Dim n As Integer = [as].Length
			For i As Integer = 0 To n - 1
				Dim o As Object = [as](i).getAttribute(key)
				If o IsNot Nothing Then Return o
			Next i
			Return Nothing
		End Function

		''' <summary>
		''' Gets the names of all attributes.
		''' </summary>
		''' <returns> the attribute names </returns>
		''' <seealso cref= AttributeSet#getAttributeNames </seealso>
		Public Overridable Property attributeNames As System.Collections.IEnumerator
			Get
				Return New MuxingAttributeNameEnumeration(Me)
			End Get
		End Property

		''' <summary>
		''' Checks whether a given attribute name/value is defined.
		''' </summary>
		''' <param name="name"> the attribute name </param>
		''' <param name="value"> the attribute value </param>
		''' <returns> true if the name/value is defined </returns>
		''' <seealso cref= AttributeSet#containsAttribute </seealso>
		Public Overridable Function containsAttribute(ByVal name As Object, ByVal value As Object) As Boolean Implements AttributeSet.containsAttribute
			Return value.Equals(getAttribute(name))
		End Function

		''' <summary>
		''' Checks whether the attribute set contains all of
		''' the given attributes.
		''' </summary>
		''' <param name="attrs"> the attributes to check </param>
		''' <returns> true if the element contains all the attributes </returns>
		''' <seealso cref= AttributeSet#containsAttributes </seealso>
		Public Overridable Function containsAttributes(ByVal attrs As AttributeSet) As Boolean Implements AttributeSet.containsAttributes
			Dim result As Boolean = True

			Dim names As System.Collections.IEnumerator = attrs.attributeNames
			Do While result AndAlso names.hasMoreElements()
				Dim name As Object = names.nextElement()
				result = attrs.getAttribute(name).Equals(getAttribute(name))
			Loop

			Return result
		End Function

		''' <summary>
		''' Returns null, subclasses may wish to do something more
		''' intelligent with this.
		''' </summary>
		Public Overridable Property resolveParent As AttributeSet Implements AttributeSet.getResolveParent
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' The <code>AttributeSet</code>s that make up the resulting
		''' <code>AttributeSet</code>.
		''' </summary>
		Private attrs As AttributeSet()


		''' <summary>
		''' An Enumeration of the Attribute names in a MuxingAttributeSet.
		''' This may return the same name more than once.
		''' </summary>
		Private Class MuxingAttributeNameEnumeration
			Implements System.Collections.IEnumerator

			Private ReadOnly outerInstance As MuxingAttributeSet


			Friend Sub New(ByVal outerInstance As MuxingAttributeSet)
					Me.outerInstance = outerInstance
				updateEnum()
			End Sub

			Public Overridable Function hasMoreElements() As Boolean
				If currentEnum Is Nothing Then Return False
				Return currentEnum.hasMoreElements()
			End Function

			Public Overridable Function nextElement() As Object
				If currentEnum Is Nothing Then Throw New NoSuchElementException("No more names")
				Dim retObject As Object = currentEnum.nextElement()
				If Not currentEnum.hasMoreElements() Then updateEnum()
				Return retObject
			End Function

			Friend Overridable Sub updateEnum()
				Dim [as] As AttributeSet() = outerInstance.attributes
				currentEnum = Nothing
				Do While currentEnum Is Nothing AndAlso attrIndex < [as].Length
					currentEnum = [as](attrIndex).attributeNames
					attrIndex += 1
					If Not currentEnum.hasMoreElements() Then currentEnum = Nothing
				Loop
			End Sub


			''' <summary>
			''' Index into attrs the current Enumeration came from. </summary>
			Private attrIndex As Integer
			''' <summary>
			''' Enumeration from attrs. </summary>
			Private currentEnum As System.Collections.IEnumerator
		End Class
	End Class

End Namespace
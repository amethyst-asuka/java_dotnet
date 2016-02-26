Imports System
Imports System.Collections

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print.attribute


	''' <summary>
	''' Class HashAttributeSet provides an <code>AttributeSet</code>
	''' implementation with characteristics of a hash map.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public Class HashAttributeSet
		Implements AttributeSet

		Private Const serialVersionUID As Long = 5311560590283707917L

		''' <summary>
		''' The interface of which all members of this attribute set must be an
		''' instance. It is assumed to be interface <seealso cref="Attribute Attribute"/>
		''' or a subinterface thereof.
		''' @serial
		''' </summary>
		Private myInterface As Type

	'    
	'     * A HashMap used by the implementation.
	'     * The serialised form doesn't include this instance variable.
	'     
		<NonSerialized> _
		Private attrMap As New Hashtable

		''' <summary>
		''' Write the instance to a stream (ie serialize the object)
		''' 
		''' @serialData
		''' The serialized form of an attribute set explicitly writes the
		''' number of attributes in the set, and each of the attributes.
		''' This does not guarantee equality of serialized forms since
		''' the order in which the attributes are written is not defined.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)

			s.defaultWriteObject()
			Dim attrs As Attribute() = ToArray()
			s.writeInt(attrs.Length)
			For i As Integer = 0 To attrs.Length - 1
				s.writeObject(attrs(i))
			Next i
		End Sub

		''' <summary>
		''' Reconstitute an instance from a stream that is, deserialize it).
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)

			s.defaultReadObject()
			attrMap = New Hashtable
			Dim count As Integer = s.readInt()
			Dim attr As Attribute
			For i As Integer = 0 To count - 1
				attr = CType(s.readObject(), Attribute)
				add(attr)
			Next i
		End Sub

		''' <summary>
		''' Construct a new, empty attribute set.
		''' </summary>
		Public Sub New()
			Me.New(GetType(Attribute))
		End Sub

		''' <summary>
		''' Construct a new attribute set,
		''' initially populated with the given attribute.
		''' </summary>
		''' <param name="attribute">  Attribute value to add to the set.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>attribute</CODE> is null. </exception>
		Public Sub New(ByVal attribute As Attribute)
			Me.New(attribute, GetType(Attribute))
		End Sub

		''' <summary>
		''' Construct a new attribute set,
		''' initially populated with the values from the
		''' given array. The new attribute set is populated by
		''' adding the elements of <CODE>attributes</CODE> array to the set in
		''' sequence, starting at index 0. Thus, later array elements may replace
		''' earlier array elements if the array contains duplicate attribute
		''' values or attribute categories.
		''' </summary>
		''' <param name="attributes">  Array of attribute values to add to the set.
		'''                    If null, an empty attribute set is constructed.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if any element of
		'''     <CODE>attributes</CODE> is null. </exception>
		Public Sub New(ByVal attributes As Attribute())
			Me.New(attributes, GetType(Attribute))
		End Sub

		''' <summary>
		''' Construct a new attribute set,
		''' initially populated with the values from the  given set.
		''' </summary>
		''' <param name="attributes"> Set of attributes from which to initialise this set.
		'''                 If null, an empty attribute set is constructed.
		'''  </param>
		Public Sub New(ByVal attributes As AttributeSet)
			Me.New(attributes, GetType(Attribute))
		End Sub

		''' <summary>
		''' Construct a new, empty attribute set, where the members of
		''' the attribute set are restricted to the given interface.
		''' </summary>
		''' <param name="interfaceName">  The interface of which all members of this
		'''                     attribute set must be an instance. It is assumed to
		'''                     be interface <seealso cref="Attribute Attribute"/> or a
		'''                     subinterface thereof. </param>
		''' <exception cref="NullPointerException"> if interfaceName is null. </exception>
		Protected Friend Sub New(ByVal interfaceName As Type)
			If interfaceName Is Nothing Then Throw New NullPointerException("null interface")
			myInterface = interfaceName
		End Sub

		''' <summary>
		''' Construct a new attribute set, initially populated with the given
		''' attribute, where the members of the attribute set are restricted to the
		''' given interface.
		''' </summary>
		''' <param name="attribute">      Attribute value to add to the set. </param>
		''' <param name="interfaceName">  The interface of which all members of this
		'''                    attribute set must be an instance. It is assumed to
		'''                    be interface <seealso cref="Attribute Attribute"/> or a
		'''                    subinterface thereof.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>attribute</CODE> is null. </exception>
		''' <exception cref="NullPointerException"> if interfaceName is null. </exception>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if <CODE>attribute</CODE> is not an
		'''     instance of <CODE>interfaceName</CODE>. </exception>
		Protected Friend Sub New(ByVal attribute As Attribute, ByVal interfaceName As Type)
			If interfaceName Is Nothing Then Throw New NullPointerException("null interface")
			myInterface = interfaceName
			add(attribute)
		End Sub

		''' <summary>
		''' Construct a new attribute set, where the members of the attribute
		''' set are restricted to the given interface.
		''' The new attribute set is populated
		''' by adding the elements of <CODE>attributes</CODE> array to the set in
		''' sequence, starting at index 0. Thus, later array elements may replace
		''' earlier array elements if the array contains duplicate attribute
		''' values or attribute categories.
		''' </summary>
		''' <param name="attributes"> Array of attribute values to add to the set. If
		'''                    null, an empty attribute set is constructed. </param>
		''' <param name="interfaceName">  The interface of which all members of this
		'''                    attribute set must be an instance. It is assumed to
		'''                    be interface <seealso cref="Attribute Attribute"/> or a
		'''                    subinterface thereof.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if any element of
		''' <CODE>attributes</CODE> is null. </exception>
		''' <exception cref="NullPointerException"> if interfaceName is null. </exception>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if any element of
		''' <CODE>attributes</CODE> is not an instance of
		''' <CODE>interfaceName</CODE>. </exception>
		Protected Friend Sub New(ByVal attributes As Attribute(), ByVal interfaceName As Type)
			If interfaceName Is Nothing Then Throw New NullPointerException("null interface")
			myInterface = interfaceName
			Dim n As Integer = If(attributes Is Nothing, 0, attributes.Length)
			For i As Integer = 0 To n - 1
				add(attributes(i))
			Next i
		End Sub

		''' <summary>
		''' Construct a new attribute set, initially populated with the
		''' values from the  given set where the members of the attribute
		''' set are restricted to the given interface.
		''' </summary>
		''' <param name="attributes"> set of attribute values to initialise the set. If
		'''                    null, an empty attribute set is constructed. </param>
		''' <param name="interfaceName">  The interface of which all members of this
		'''                    attribute set must be an instance. It is assumed to
		'''                    be interface <seealso cref="Attribute Attribute"/> or a
		'''                    subinterface thereof.
		''' </param>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if any element of
		''' <CODE>attributes</CODE> is not an instance of
		''' <CODE>interfaceName</CODE>. </exception>
		Protected Friend Sub New(ByVal attributes As AttributeSet, ByVal interfaceName As Type)
		  myInterface = interfaceName
		  If attributes IsNot Nothing Then
			Dim attribArray As Attribute() = attributes.ToArray()
			Dim n As Integer = If(attribArray Is Nothing, 0, attribArray.Length)
			For i As Integer = 0 To n - 1
			  add(attribArray(i))
			Next i
		  End If
		End Sub

		''' <summary>
		''' Returns the attribute value which this attribute set contains in the
		''' given attribute category. Returns <tt>null</tt> if this attribute set
		''' does not contain any attribute value in the given attribute category.
		''' </summary>
		''' <param name="category">  Attribute category whose associated attribute value
		'''                   is to be returned. It must be a
		'''                   <seealso cref="java.lang.Class Class"/>
		'''                   that implements interface {@link Attribute
		'''                   Attribute}.
		''' </param>
		''' <returns>  The attribute value in the given attribute category contained
		'''          in this attribute set, or <tt>null</tt> if this attribute set
		'''          does not contain any attribute value in the given attribute
		'''          category.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if the <CODE>category</CODE> is null. </exception>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if the <CODE>category</CODE> is not a
		'''     <seealso cref="java.lang.Class Class"/> that implements interface {@link
		'''     Attribute Attribute}. </exception>
		Public Overridable Function [get](ByVal category As Type) As Attribute Implements AttributeSet.get
			Return CType(attrMap(AttributeSetUtilities.verifyAttributeCategory(category, GetType(Attribute))), Attribute)
		End Function

		''' <summary>
		''' Adds the specified attribute to this attribute set if it is not
		''' already present, first removing any existing in the same
		''' attribute category as the specified attribute value.
		''' </summary>
		''' <param name="attribute">  Attribute value to be added to this attribute set.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set changed as a result of the
		'''          call, i.e., the given attribute value was not already a
		'''          member of this attribute set.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''    (unchecked exception) Thrown if the <CODE>attribute</CODE> is null. </exception>
		''' <exception cref="UnmodifiableSetException">
		'''    (unchecked exception) Thrown if this attribute set does not support
		'''     the <CODE>add()</CODE> operation. </exception>
		Public Overridable Function add(ByVal attribute As Attribute) As Boolean Implements AttributeSet.add
				attrMap(attribute.category) = AttributeSetUtilities.verifyAttributeValue(attribute, myInterface)
				Dim oldAttribute As Object = attrMap(attribute.category)
			Return ((Not attribute.Equals(oldAttribute)))
		End Function

		''' <summary>
		''' Removes any attribute for this category from this attribute set if
		''' present. If <CODE>category</CODE> is null, then
		''' <CODE>remove()</CODE> does nothing and returns <tt>false</tt>.
		''' </summary>
		''' <param name="category"> Attribute category to be removed from this
		'''                  attribute set.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set changed as a result of the
		'''         call, i.e., the given attribute category had been a member of
		'''         this attribute set.
		''' </returns>
		''' <exception cref="UnmodifiableSetException">
		'''     (unchecked exception) Thrown if this attribute set does not
		'''     support the <CODE>remove()</CODE> operation. </exception>
		Public Overridable Function remove(ByVal category As Type) As Boolean Implements AttributeSet.remove
			Return category IsNot Nothing AndAlso AttributeSetUtilities.verifyAttributeCategory(category, GetType(Attribute)) IsNot Nothing AndAlso attrMap.Remove(category) IsNot Nothing
		End Function

		''' <summary>
		''' Removes the specified attribute from this attribute set if
		''' present. If <CODE>attribute</CODE> is null, then
		''' <CODE>remove()</CODE> does nothing and returns <tt>false</tt>.
		''' </summary>
		''' <param name="attribute"> Attribute value to be removed from this attribute set.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set changed as a result of the
		'''         call, i.e., the given attribute value had been a member of
		'''         this attribute set.
		''' </returns>
		''' <exception cref="UnmodifiableSetException">
		'''     (unchecked exception) Thrown if this attribute set does not
		'''     support the <CODE>remove()</CODE> operation. </exception>
		Public Overridable Function remove(ByVal attribute As Attribute) As Boolean Implements AttributeSet.remove
			Return attribute IsNot Nothing AndAlso attrMap.Remove(attribute.category) IsNot Nothing
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if this attribute set contains an
		''' attribute for the specified category.
		''' </summary>
		''' <param name="category"> whose presence in this attribute set is
		'''            to be tested.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set contains an attribute
		'''         value for the specified category. </returns>
		Public Overridable Function containsKey(ByVal category As Type) As Boolean Implements AttributeSet.containsKey
			Return category IsNot Nothing AndAlso AttributeSetUtilities.verifyAttributeCategory(category, GetType(Attribute)) IsNot Nothing AndAlso attrMap(category) IsNot Nothing
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if this attribute set contains the given
		''' attribute.
		''' </summary>
		''' <param name="attribute">  value whose presence in this attribute set is
		'''            to be tested.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set contains the given
		'''      attribute    value. </returns>
		Public Overridable Function containsValue(ByVal attribute As Attribute) As Boolean Implements AttributeSet.containsValue
			Return attribute IsNot Nothing AndAlso TypeOf attribute Is Attribute AndAlso attribute.Equals(attrMap(CType(attribute, Attribute).category))
		End Function

		''' <summary>
		''' Adds all of the elements in the specified set to this attribute.
		''' The outcome is the same as if the
		''' <seealso cref="#add(Attribute) add(Attribute)"/>
		''' operation had been applied to this attribute set successively with
		''' each element from the specified set.
		''' The behavior of the <CODE>addAll(AttributeSet)</CODE>
		''' operation is unspecified if the specified set is modified while
		''' the operation is in progress.
		''' <P>
		''' If the <CODE>addAll(AttributeSet)</CODE> operation throws an exception,
		''' the effect on this attribute set's state is implementation dependent;
		''' elements from the specified set before the point of the exception may
		''' or may not have been added to this attribute set.
		''' </summary>
		''' <param name="attributes">  whose elements are to be added to this attribute
		'''            set.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set changed as a result of the
		'''          call.
		''' </returns>
		''' <exception cref="UnmodifiableSetException">
		'''    (Unchecked exception) Thrown if this attribute set does not
		'''     support the <tt>addAll(AttributeSet)</tt> method. </exception>
		''' <exception cref="NullPointerException">
		'''     (Unchecked exception) Thrown if some element in the specified
		'''     set is null, or the set is null.
		''' </exception>
		''' <seealso cref= #add(Attribute) </seealso>
		Public Overridable Function addAll(ByVal attributes As AttributeSet) As Boolean Implements AttributeSet.addAll

			Dim attrs As Attribute() = attributes.ToArray()
			Dim result As Boolean = False
			For i As Integer = 0 To attrs.Length - 1
				Dim newValue As Attribute = AttributeSetUtilities.verifyAttributeValue(attrs(i), myInterface)
					attrMap(newValue.category) = newValue
					Dim oldValue As Object = attrMap(newValue.category)
				result = ((Not newValue.Equals(oldValue))) OrElse result
			Next i
			Return result
		End Function

		''' <summary>
		''' Returns the number of attributes in this attribute set. If this
		''' attribute set contains more than <tt>Integer.MAX_VALUE</tt> elements,
		''' returns  <tt>Integer.MAX_VALUE</tt>.
		''' </summary>
		''' <returns>  The number of attributes in this attribute set. </returns>
		Public Overridable Function size() As Integer Implements AttributeSet.size
			Return attrMap.Count
		End Function

		''' 
		''' <returns> the Attributes contained in this set as an array, zero length
		''' if the AttributeSet is empty. </returns>
		Public Overridable Function toArray() As Attribute() Implements AttributeSet.toArray
			Dim attrs As Attribute() = New Attribute(size() - 1){}
			attrMap.Values.ToArray(attrs)
			Return attrs
		End Function


		''' <summary>
		''' Removes all attributes from this attribute set.
		''' </summary>
		''' <exception cref="UnmodifiableSetException">
		'''   (unchecked exception) Thrown if this attribute set does not support
		'''     the <CODE>clear()</CODE> operation. </exception>
		Public Overridable Sub clear() Implements AttributeSet.clear
			attrMap.Clear()
		End Sub

	   ''' <summary>
	   ''' Returns true if this attribute set contains no attributes.
	   ''' </summary>
	   ''' <returns> true if this attribute set contains no attributes. </returns>
		Public Overridable Property empty As Boolean Implements AttributeSet.isEmpty
			Get
				Return attrMap.Count = 0
			End Get
		End Property

		''' <summary>
		''' Compares the specified object with this attribute set for equality.
		''' Returns <tt>true</tt> if the given object is also an attribute set and
		''' the two attribute sets contain the same attribute category-attribute
		''' value mappings. This ensures that the
		''' <tt>equals()</tt> method works properly across different
		''' implementations of the AttributeSet interface.
		''' </summary>
		''' <param name="object"> to be compared for equality with this attribute set.
		''' </param>
		''' <returns>  <tt>true</tt> if the specified object is equal to this
		'''       attribute   set. </returns>

		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			If [object] Is Nothing OrElse Not(TypeOf [object] Is AttributeSet) Then Return False

			Dim aset As AttributeSet = CType([object], AttributeSet)
			If aset.size() <> size() Then Return False

			Dim attrs As Attribute() = ToArray()
			For i As Integer = 0 To attrs.Length - 1
				If Not aset.containsValue(attrs(i)) Then Return False
			Next i
			Return True
		End Function

		''' <summary>
		''' Returns the hash code value for this attribute set.
		''' The hash code of an attribute set is defined to be the sum
		''' of the hash codes of each entry in the AttributeSet.
		''' This ensures that <tt>t1.equals(t2)</tt> implies that
		''' <tt>t1.hashCode()==t2.hashCode()</tt> for any two attribute sets
		''' <tt>t1</tt> and <tt>t2</tt>, as required by the general contract of
		''' <seealso cref="java.lang.Object#hashCode() Object.hashCode()"/>.
		''' </summary>
		''' <returns>  The hash code value for this attribute set. </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim hcode As Integer = 0
			Dim attrs As Attribute() = ToArray()
			For i As Integer = 0 To attrs.Length - 1
				hcode += attrs(i).GetHashCode()
			Next i
			Return hcode
		End Function

	End Class

End Namespace
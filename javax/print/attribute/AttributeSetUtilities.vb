Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' Class AttributeSetUtilities provides static methods for manipulating
	''' AttributeSets.
	''' <ul>
	''' <li>Methods for creating unmodifiable and synchronized views of attribute
	''' sets.
	''' <li>operations useful for building
	''' implementations of interface <seealso cref="AttributeSet AttributeSet"/>
	''' </ul>
	''' <P>
	''' An <B>unmodifiable view</B> <I>U</I> of an AttributeSet <I>S</I> provides a
	''' client with "read-only" access to <I>S</I>. Query operations on <I>U</I>
	''' "read through" to <I>S</I>; thus, changes in <I>S</I> are reflected in
	''' <I>U</I>. However, any attempt to modify <I>U</I>,
	'''  results in an UnmodifiableSetException.
	''' The unmodifiable view object <I>U</I> will be serializable if the
	''' attribute set object <I>S</I> is serializable.
	''' <P>
	''' A <B>synchronized view</B> <I>V</I> of an attribute set <I>S</I> provides a
	''' client with synchronized (multiple thread safe) access to <I>S</I>. Each
	''' operation of <I>V</I> is synchronized using <I>V</I> itself as the lock
	''' object and then merely invokes the corresponding operation of <I>S</I>. In
	''' order to guarantee mutually exclusive access, it is critical that all
	''' access to <I>S</I> is accomplished through <I>V</I>. The synchronized view
	''' object <I>V</I> will be serializable if the attribute set object <I>S</I>
	''' is serializable.
	''' <P>
	''' As mentioned in the package description of javax.print, a null reference
	''' parameter to methods is
	''' incorrect unless explicitly documented on the method as having a meaningful
	''' interpretation.  Usage to the contrary is incorrect coding and may result in
	''' a run time exception either immediately
	''' or at some later time. IllegalArgumentException and NullPointerException
	''' are examples of typical and acceptable run time exceptions for such cases.
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class AttributeSetUtilities

	'     Suppress default constructor, ensuring non-instantiability.
	'     
		Private Sub New()
		End Sub

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class UnmodifiableAttributeSet
			Implements AttributeSet

			Private attrset As AttributeSet

	'         Unmodifiable view of the underlying attribute set.
	'         
			Public Sub New(ByVal attributeSet As AttributeSet)

				attrset = attributeSet
			End Sub

			Public Overridable Function [get](ByVal key As Type) As Attribute Implements AttributeSet.get
				Return attrset.get(key)
			End Function

			Public Overridable Function add(ByVal attribute As Attribute) As Boolean Implements AttributeSet.add
				Throw New UnmodifiableSetException
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Function remove(ByVal category As Type) As Boolean Implements AttributeSet.remove
				Throw New UnmodifiableSetException
			End Function

			Public Overridable Function remove(ByVal attribute As Attribute) As Boolean Implements AttributeSet.remove
				Throw New UnmodifiableSetException
			End Function

			Public Overridable Function containsKey(ByVal category As Type) As Boolean Implements AttributeSet.containsKey
				Return attrset.containsKey(category)
			End Function

			Public Overridable Function containsValue(ByVal attribute As Attribute) As Boolean Implements AttributeSet.containsValue
				Return attrset.containsValue(attribute)
			End Function

			Public Overridable Function addAll(ByVal attributes As AttributeSet) As Boolean Implements AttributeSet.addAll
				Throw New UnmodifiableSetException
			End Function

			Public Overridable Function size() As Integer Implements AttributeSet.size
				Return attrset.size()
			End Function

			Public Overridable Function toArray() As Attribute() Implements AttributeSet.toArray
				Return attrset.ToArray()
			End Function

			Public Overridable Sub clear() Implements AttributeSet.clear
				Throw New UnmodifiableSetException
			End Sub

			Public Overridable Property empty As Boolean Implements AttributeSet.isEmpty
				Get
					Return attrset.empty
				End Get
			End Property

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return attrset.Equals(o)
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return attrset.GetHashCode()
			End Function

		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class UnmodifiableDocAttributeSet
			Inherits UnmodifiableAttributeSet
			Implements DocAttributeSet

			Public Sub New(ByVal attributeSet As DocAttributeSet)

				MyBase.New(attributeSet)
			End Sub
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class UnmodifiablePrintRequestAttributeSet
			Inherits UnmodifiableAttributeSet
			Implements PrintRequestAttributeSet

			Public Sub New(ByVal attributeSet As PrintRequestAttributeSet)

				MyBase.New(attributeSet)
			End Sub
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class UnmodifiablePrintJobAttributeSet
			Inherits UnmodifiableAttributeSet
			Implements PrintJobAttributeSet

			Public Sub New(ByVal attributeSet As PrintJobAttributeSet)

				MyBase.New(attributeSet)
			End Sub
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class UnmodifiablePrintServiceAttributeSet
			Inherits UnmodifiableAttributeSet
			Implements PrintServiceAttributeSet

			Public Sub New(ByVal attributeSet As PrintServiceAttributeSet)

				MyBase.New(attributeSet)
			End Sub
		End Class

		''' <summary>
		''' Creates an unmodifiable view of the given attribute set.
		''' </summary>
		''' <param name="attributeSet">  Underlying attribute set.
		''' </param>
		''' <returns>  Unmodifiable view of <CODE>attributeSet</CODE>.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     Thrown if <CODE>attributeSet</CODE> is null. Null is never a </exception>
		Public Shared Function unmodifiableView(ByVal attributeSet As AttributeSet) As AttributeSet
			If attributeSet Is Nothing Then Throw New NullPointerException

			Return New UnmodifiableAttributeSet(attributeSet)
		End Function

		''' <summary>
		''' Creates an unmodifiable view of the given doc attribute set.
		''' </summary>
		''' <param name="attributeSet">  Underlying doc attribute set.
		''' </param>
		''' <returns>  Unmodifiable view of <CODE>attributeSet</CODE>.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     Thrown if <CODE>attributeSet</CODE> is null. </exception>
		Public Shared Function unmodifiableView(ByVal attributeSet As DocAttributeSet) As DocAttributeSet
			If attributeSet Is Nothing Then Throw New NullPointerException
			Return New UnmodifiableDocAttributeSet(attributeSet)
		End Function

		''' <summary>
		''' Creates an unmodifiable view of the given print request attribute set.
		''' </summary>
		''' <param name="attributeSet">  Underlying print request attribute set.
		''' </param>
		''' <returns>  Unmodifiable view of <CODE>attributeSet</CODE>.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     Thrown if <CODE>attributeSet</CODE> is null. </exception>
		Public Shared Function unmodifiableView(ByVal attributeSet As PrintRequestAttributeSet) As PrintRequestAttributeSet
			If attributeSet Is Nothing Then Throw New NullPointerException
			Return New UnmodifiablePrintRequestAttributeSet(attributeSet)
		End Function

		''' <summary>
		''' Creates an unmodifiable view of the given print job attribute set.
		''' </summary>
		''' <param name="attributeSet">  Underlying print job attribute set.
		''' </param>
		''' <returns>  Unmodifiable view of <CODE>attributeSet</CODE>.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     Thrown if <CODE>attributeSet</CODE> is null. </exception>
		Public Shared Function unmodifiableView(ByVal attributeSet As PrintJobAttributeSet) As PrintJobAttributeSet
			If attributeSet Is Nothing Then Throw New NullPointerException
			Return New UnmodifiablePrintJobAttributeSet(attributeSet)
		End Function

		''' <summary>
		''' Creates an unmodifiable view of the given print service attribute set.
		''' </summary>
		''' <param name="attributeSet">  Underlying print service attribute set.
		''' </param>
		''' <returns>  Unmodifiable view of <CODE>attributeSet</CODE>.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     Thrown if <CODE>attributeSet</CODE> is null. </exception>
		Public Shared Function unmodifiableView(ByVal attributeSet As PrintServiceAttributeSet) As PrintServiceAttributeSet
			If attributeSet Is Nothing Then Throw New NullPointerException
			Return New UnmodifiablePrintServiceAttributeSet(attributeSet)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SynchronizedAttributeSet
			Implements AttributeSet

			Private attrset As AttributeSet

			Public Sub New(ByVal attributeSet As AttributeSet)
				attrset = attributeSet
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Function [get](ByVal category As Type) As Attribute Implements AttributeSet.get
				Return attrset.get(category)
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Function add(ByVal attribute As Attribute) As Boolean Implements AttributeSet.add
				Return attrset.add(attribute)
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Function remove(ByVal category As Type) As Boolean Implements AttributeSet.remove
				Return attrset.remove(category)
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Function remove(ByVal attribute As Attribute) As Boolean Implements AttributeSet.remove
				Return attrset.remove(attribute)
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Function containsKey(ByVal category As Type) As Boolean Implements AttributeSet.containsKey
				Return attrset.containsKey(category)
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Function containsValue(ByVal attribute As Attribute) As Boolean Implements AttributeSet.containsValue
				Return attrset.containsValue(attribute)
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Function addAll(ByVal attributes As AttributeSet) As Boolean Implements AttributeSet.addAll
				Return attrset.addAll(attributes)
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Function size() As Integer Implements AttributeSet.size
				Return attrset.size()
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Function toArray() As Attribute() Implements AttributeSet.toArray
				Return attrset.ToArray()
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Sub clear() Implements AttributeSet.clear
				attrset.clear()
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Property empty As Boolean Implements AttributeSet.isEmpty
				Get
					Return attrset.empty
				End Get
			End Property

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return attrset.Equals(o)
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overrides Function GetHashCode() As Integer
				Return attrset.GetHashCode()
			End Function
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SynchronizedDocAttributeSet
			Inherits SynchronizedAttributeSet
			Implements DocAttributeSet

			Public Sub New(ByVal attributeSet As DocAttributeSet)
				MyBase.New(attributeSet)
			End Sub
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SynchronizedPrintRequestAttributeSet
			Inherits SynchronizedAttributeSet
			Implements PrintRequestAttributeSet

			Public Sub New(ByVal attributeSet As PrintRequestAttributeSet)
				MyBase.New(attributeSet)
			End Sub
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SynchronizedPrintJobAttributeSet
			Inherits SynchronizedAttributeSet
			Implements PrintJobAttributeSet

			Public Sub New(ByVal attributeSet As PrintJobAttributeSet)
				MyBase.New(attributeSet)
			End Sub
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SynchronizedPrintServiceAttributeSet
			Inherits SynchronizedAttributeSet
			Implements PrintServiceAttributeSet

			Public Sub New(ByVal attributeSet As PrintServiceAttributeSet)
				MyBase.New(attributeSet)
			End Sub
		End Class

		''' <summary>
		''' Creates a synchronized view of the given attribute set.
		''' </summary>
		''' <param name="attributeSet">  Underlying attribute set.
		''' </param>
		''' <returns>  Synchronized view of <CODE>attributeSet</CODE>.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     Thrown if <CODE>attributeSet</CODE> is null. </exception>
		Public Shared Function synchronizedView(ByVal attributeSet As AttributeSet) As AttributeSet
			If attributeSet Is Nothing Then Throw New NullPointerException
			Return New SynchronizedAttributeSet(attributeSet)
		End Function

		''' <summary>
		''' Creates a synchronized view of the given doc attribute set.
		''' </summary>
		''' <param name="attributeSet">  Underlying doc attribute set.
		''' </param>
		''' <returns>  Synchronized view of <CODE>attributeSet</CODE>.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     Thrown if <CODE>attributeSet</CODE> is null. </exception>
		Public Shared Function synchronizedView(ByVal attributeSet As DocAttributeSet) As DocAttributeSet
			If attributeSet Is Nothing Then Throw New NullPointerException
			Return New SynchronizedDocAttributeSet(attributeSet)
		End Function

		''' <summary>
		''' Creates a synchronized view of the given print request attribute set.
		''' </summary>
		''' <param name="attributeSet">  Underlying print request attribute set.
		''' </param>
		''' <returns>  Synchronized view of <CODE>attributeSet</CODE>.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     Thrown if <CODE>attributeSet</CODE> is null. </exception>
		Public Shared Function synchronizedView(ByVal attributeSet As PrintRequestAttributeSet) As PrintRequestAttributeSet
			If attributeSet Is Nothing Then Throw New NullPointerException
			Return New SynchronizedPrintRequestAttributeSet(attributeSet)
		End Function

		''' <summary>
		''' Creates a synchronized view of the given print job attribute set.
		''' </summary>
		''' <param name="attributeSet">  Underlying print job attribute set.
		''' </param>
		''' <returns>  Synchronized view of <CODE>attributeSet</CODE>.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     Thrown if <CODE>attributeSet</CODE> is null. </exception>
		Public Shared Function synchronizedView(ByVal attributeSet As PrintJobAttributeSet) As PrintJobAttributeSet
			If attributeSet Is Nothing Then Throw New NullPointerException
			Return New SynchronizedPrintJobAttributeSet(attributeSet)
		End Function

		''' <summary>
		''' Creates a synchronized view of the given print service attribute set.
		''' </summary>
		''' <param name="attributeSet">  Underlying print service attribute set.
		''' </param>
		''' <returns>  Synchronized view of <CODE>attributeSet</CODE>. </returns>
		Public Shared Function synchronizedView(ByVal attributeSet As PrintServiceAttributeSet) As PrintServiceAttributeSet
			If attributeSet Is Nothing Then Throw New NullPointerException
			Return New SynchronizedPrintServiceAttributeSet(attributeSet)
		End Function


		''' <summary>
		''' Verify that the given object is a <seealso cref="java.lang.Class Class"/> that
		''' implements the given interface, which is assumed to be interface {@link
		''' Attribute Attribute} or a subinterface thereof.
		''' </summary>
		''' <param name="object">     Object to test. </param>
		''' <param name="interfaceName">  Interface the object must implement.
		''' </param>
		''' <returns>  If <CODE>object</CODE> is a <seealso cref="java.lang.Class Class"/>
		'''          that implements <CODE>interfaceName</CODE>,
		'''          <CODE>object</CODE> is returned downcast to type {@link
		'''          java.lang.Class Class}; otherwise an exception is thrown.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>object</CODE> is null. </exception>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if <CODE>object</CODE> is not a
		'''     <seealso cref="java.lang.Class Class"/> that implements
		'''     <CODE>interfaceName</CODE>. </exception>
		Public Shared Function verifyAttributeCategory(ByVal [object] As Object, ByVal interfaceName As Type) As Type

			Dim result As Type = CType([object], [Class])
			If interfaceName.IsAssignableFrom(result) Then
				Return result
			Else
				Throw New ClassCastException
			End If
		End Function

		''' <summary>
		''' Verify that the given object is an instance of the given interface, which
		''' is assumed to be interface <seealso cref="Attribute Attribute"/> or a subinterface
		''' thereof.
		''' </summary>
		''' <param name="object">     Object to test. </param>
		''' <param name="interfaceName">  Interface of which the object must be an instance.
		''' </param>
		''' <returns>  If <CODE>object</CODE> is an instance of
		'''          <CODE>interfaceName</CODE>, <CODE>object</CODE> is returned
		'''          downcast to type <seealso cref="Attribute Attribute"/>; otherwise an
		'''          exception is thrown.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>object</CODE> is null. </exception>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if <CODE>object</CODE> is not an
		'''     instance of <CODE>interfaceName</CODE>. </exception>
		Public Shared Function verifyAttributeValue(ByVal [object] As Object, ByVal interfaceName As Type) As Attribute

			If [object] Is Nothing Then
				Throw New NullPointerException
			ElseIf interfaceName.IsInstanceOfType([object]) Then
				Return CType([object], Attribute)
			Else
				Throw New ClassCastException
			End If
		End Function

		''' <summary>
		''' Verify that the given attribute category object is equal to the
		''' category of the given attribute value object. If so, this method
		''' returns doing nothing. If not, this method throws an exception.
		''' </summary>
		''' <param name="category">   Attribute category to test. </param>
		''' <param name="attribute">  Attribute value to test.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if the <CODE>category</CODE> is
		'''     null or if the <CODE>attribute</CODE> is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if the <CODE>category</CODE> is not
		'''     equal to the category of the <CODE>attribute</CODE>. </exception>
		Public Shared Sub verifyCategoryForValue(ByVal category As Type, ByVal attribute As Attribute)

			If Not category.Equals(attribute.category) Then Throw New System.ArgumentException
		End Sub
	End Class

End Namespace
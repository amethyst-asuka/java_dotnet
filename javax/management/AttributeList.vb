Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management


	''' <summary>
	''' <p>Represents a list of values for attributes of an MBean.  See the
	''' <seealso cref="MBeanServerConnection#getAttributes getAttributes"/> and
	''' <seealso cref="MBeanServerConnection#setAttributes setAttributes"/> methods of
	''' <seealso cref="MBeanServer"/> and <seealso cref="MBeanServerConnection"/>.</p>
	''' 
	''' <p id="type-safe">For compatibility reasons, it is possible, though
	''' highly discouraged, to add objects to an {@code AttributeList} that are
	''' not instances of {@code Attribute}.  However, an {@code AttributeList}
	''' can be made <em>type-safe</em>, which means that an attempt to add
	''' an object that is not an {@code Attribute} will produce an {@code
	''' IllegalArgumentException}.  An {@code AttributeList} becomes type-safe
	''' when the method <seealso cref="#asList()"/> is called on it.</p>
	''' 
	''' @since 1.5
	''' </summary>
	' We cannot extend ArrayList<Attribute> because our legacy
	'   add(Attribute) method would then override add(E) in ArrayList<E>,
	'   and our return value is void whereas ArrayList.add(E)'s is boolean.
	'   Likewise for set(int,Attribute).  Grrr.  We cannot use covariance
	'   to override the most important methods and have them return
	'   Attribute, either, because that would break subclasses that
	'   override those methods in turn (using the original return type
	'   of Object).  Finally, we cannot implement Iterable<Attribute>
	'   so you could write
	'       for (Attribute a : attributeList)
	'   because ArrayList<> implements Iterable<> and the same class cannot
	'   implement two versions of a generic interface.  Instead we provide
	'   the asList() method so you can write
	'       for (Attribute a : attributeList.asList())
	'
	Public Class AttributeList
		Inherits List(Of Object)

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private typeSafe As Boolean
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private tainted As Boolean

		' Serial version 
		Private Const serialVersionUID As Long = -4077085769279709076L

		''' <summary>
		''' Constructs an empty <CODE>AttributeList</CODE>.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an empty <CODE>AttributeList</CODE> with
		''' the initial capacity specified.
		''' </summary>
		''' <param name="initialCapacity"> the initial capacity of the
		''' <code>AttributeList</code>, as specified by {@link
		''' ArrayList#ArrayList(int)}. </param>
		Public Sub New(ByVal initialCapacity As Integer)
			MyBase.New(initialCapacity)
		End Sub

		''' <summary>
		''' Constructs an <CODE>AttributeList</CODE> containing the
		''' elements of the <CODE>AttributeList</CODE> specified, in the
		''' order in which they are returned by the
		''' <CODE>AttributeList</CODE>'s iterator.  The
		''' <CODE>AttributeList</CODE> instance has an initial capacity of
		''' 110% of the size of the <CODE>AttributeList</CODE> specified.
		''' </summary>
		''' <param name="list"> the <code>AttributeList</code> that defines the initial
		''' contents of the new <code>AttributeList</code>.
		''' </param>
		''' <seealso cref= ArrayList#ArrayList(java.util.Collection) </seealso>
		Public Sub New(ByVal list As AttributeList)
			MyBase.New(list)
		End Sub

		''' <summary>
		''' Constructs an {@code AttributeList} containing the elements of the
		''' {@code List} specified, in the order in which they are returned by
		''' the {@code List}'s iterator.
		''' </summary>
		''' <param name="list"> the {@code List} that defines the initial contents of
		''' the new {@code AttributeList}.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the {@code list} parameter
		''' is {@code null} or if the {@code list} parameter contains any
		''' non-Attribute objects.
		''' </exception>
		''' <seealso cref= ArrayList#ArrayList(java.util.Collection)
		''' 
		''' @since 1.6 </seealso>
		Public Sub New(ByVal list As IList(Of Attribute))
			' Check for null parameter
			'
			If list Is Nothing Then Throw New System.ArgumentException("Null parameter")

			' Check for non-Attribute objects
			'
			adding(list)

			' Build the List<Attribute>
			'
			MyBase.addAll(list)
		End Sub

		''' <summary>
		''' Return a view of this list as a {@code List<Attribute>}.
		''' Changes to the returned value are reflected by changes
		''' to the original {@code AttributeList} and vice versa.
		''' </summary>
		''' <returns> a {@code List<Attribute>} whose contents
		''' reflect the contents of this {@code AttributeList}.
		''' 
		''' <p>If this method has ever been called on a given
		''' {@code AttributeList} instance, a subsequent attempt to add
		''' an object to that instance which is not an {@code Attribute}
		''' will fail with a {@code IllegalArgumentException}. For compatibility
		''' reasons, an {@code AttributeList} on which this method has never
		''' been called does allow objects other than {@code Attribute}s to
		''' be added.</p>
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if this {@code AttributeList} contains
		''' an element that is not an {@code Attribute}.
		''' 
		''' @since 1.6 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function asList() As IList(Of Attribute)
			typeSafe = True
			If tainted Then adding(CType(Me, ICollection(Of ?))) ' will throw IllegalArgumentException
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return CType(CType(Me, IList(Of ?)), IList(Of Attribute))
		End Function

		''' <summary>
		''' Adds the {@code Attribute} specified as the last element of the list.
		''' </summary>
		''' <param name="object">  The attribute to be added. </param>
		Public Overridable Sub add(ByVal [object] As Attribute)
			MyBase.add([object])
		End Sub

		''' <summary>
		''' Inserts the attribute specified as an element at the position specified.
		''' Elements with an index greater than or equal to the current position are
		''' shifted up. If the index is out of range {@literal (index < 0 || index >
		''' size())} a RuntimeOperationsException should be raised, wrapping the
		''' java.lang.IndexOutOfBoundsException thrown.
		''' </summary>
		''' <param name="object">  The <CODE>Attribute</CODE> object to be inserted. </param>
		''' <param name="index"> The position in the list where the new {@code Attribute}
		''' object is to be inserted. </param>
		Public Overridable Sub add(ByVal index As Integer, ByVal [object] As Attribute)
			Try
				MyBase.add(index, [object])
			Catch e As System.IndexOutOfRangeException
				Throw New RuntimeOperationsException(e, "The specified index is out of range")
			End Try
		End Sub

		''' <summary>
		''' Sets the element at the position specified to be the attribute specified.
		''' The previous element at that position is discarded. If the index is
		''' out of range {@literal (index < 0 || index > size())} a RuntimeOperationsException
		''' should be raised, wrapping the java.lang.IndexOutOfBoundsException thrown.
		''' </summary>
		''' <param name="object">  The value to which the attribute element should be set. </param>
		''' <param name="index">  The position specified. </param>
		Public Overridable Sub [set](ByVal index As Integer, ByVal [object] As Attribute)
			Try
				MyBase.set(index, [object])
			Catch e As System.IndexOutOfRangeException
				Throw New RuntimeOperationsException(e, "The specified index is out of range")
			End Try
		End Sub

		''' <summary>
		''' Appends all the elements in the <CODE>AttributeList</CODE> specified to
		''' the end of the list, in the order in which they are returned by the
		''' Iterator of the <CODE>AttributeList</CODE> specified.
		''' </summary>
		''' <param name="list">  Elements to be inserted into the list.
		''' </param>
		''' <returns> true if this list changed as a result of the call.
		''' </returns>
		''' <seealso cref= ArrayList#addAll(java.util.Collection) </seealso>
		Public Overridable Function addAll(ByVal list As AttributeList) As Boolean
			Return (MyBase.addAll(list))
		End Function

		''' <summary>
		''' Inserts all of the elements in the <CODE>AttributeList</CODE> specified
		''' into this list, starting at the specified position, in the order in which
		''' they are returned by the Iterator of the {@code AttributeList} specified.
		''' If the index is out of range {@literal (index < 0 || index > size())} a
		''' RuntimeOperationsException should be raised, wrapping the
		''' java.lang.IndexOutOfBoundsException thrown.
		''' </summary>
		''' <param name="list">  Elements to be inserted into the list. </param>
		''' <param name="index">  Position at which to insert the first element from the
		''' <CODE>AttributeList</CODE> specified.
		''' </param>
		''' <returns> true if this list changed as a result of the call.
		''' </returns>
		''' <seealso cref= ArrayList#addAll(int, java.util.Collection) </seealso>
		Public Overridable Function addAll(ByVal index As Integer, ByVal list As AttributeList) As Boolean
			Try
				Return MyBase.addAll(index, list)
			Catch e As System.IndexOutOfRangeException
				Throw New RuntimeOperationsException(e, "The specified index is out of range")
			End Try
		End Function

	'    
	'     * Override all of the methods from ArrayList<Object> that might add
	'     * a non-Attribute to the List, and disallow that if asList has ever
	'     * been called on this instance.
	'     

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="IllegalArgumentException"> if this {@code AttributeList} is
		''' <a href="#type-safe">type-safe</a> and {@code element} is not an
		''' {@code Attribute}. </exception>
		Public Overrides Function add(ByVal element As Object) As Boolean
			adding(element)
			Return MyBase.add(element)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="IllegalArgumentException"> if this {@code AttributeList} is
		''' <a href="#type-safe">type-safe</a> and {@code element} is not an
		''' {@code Attribute}. </exception>
		Public Overrides Sub add(ByVal index As Integer, ByVal element As Object)
			adding(element)
			MyBase.add(index, element)
		End Sub

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="IllegalArgumentException"> if this {@code AttributeList} is
		''' <a href="#type-safe">type-safe</a> and {@code c} contains an
		''' element that is not an {@code Attribute}. </exception>
		Public Overrides Function addAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			adding(c)
			Return MyBase.addAll(c)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="IllegalArgumentException"> if this {@code AttributeList} is
		''' <a href="#type-safe">type-safe</a> and {@code c} contains an
		''' element that is not an {@code Attribute}. </exception>
		Public Overrides Function addAll(Of T1)(ByVal index As Integer, ByVal c As ICollection(Of T1)) As Boolean
			adding(c)
			Return MyBase.addAll(index, c)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="IllegalArgumentException"> if this {@code AttributeList} is
		''' <a href="#type-safe">type-safe</a> and {@code element} is not an
		''' {@code Attribute}. </exception>
		Public Overrides Function [set](ByVal index As Integer, ByVal element As Object) As Object
			adding(element)
			Return MyBase.set(index, element)
		End Function

		Private Sub adding(ByVal x As Object)
			If x Is Nothing OrElse TypeOf x Is Attribute Then Return
			If typeSafe Then
				Throw New System.ArgumentException("Not an Attribute: " & x)
			Else
				tainted = True
			End If
		End Sub

		Private Sub adding(Of T1)(ByVal c As ICollection(Of T1))
			For Each x As Object In c
				adding(x)
			Next x
		End Sub
	End Class

End Namespace
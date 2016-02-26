Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.relation


	''' <summary>
	''' A RoleUnresolvedList represents a list of RoleUnresolved objects,
	''' representing roles not retrieved from a relation due to a problem
	''' encountered when trying to access (read or write) the roles.
	''' 
	''' @since 1.5
	''' </summary>
	' We cannot extend ArrayList<RoleUnresolved> because our legacy
	'   add(RoleUnresolved) method would then override add(E) in ArrayList<E>,
	'   and our return value is void whereas ArrayList.add(E)'s is boolean.
	'   Likewise for set(int,RoleUnresolved).  Grrr.  We cannot use covariance
	'   to override the most important methods and have them return
	'   RoleUnresolved, either, because that would break subclasses that
	'   override those methods in turn (using the original return type
	'   of Object).  Finally, we cannot implement Iterable<RoleUnresolved>
	'   so you could write
	'       for (RoleUnresolved r : roleUnresolvedList)
	'   because ArrayList<> implements Iterable<> and the same class cannot
	'   implement two versions of a generic interface.  Instead we provide
	'   the asList() method so you can write
	'       for (RoleUnresolved r : roleUnresolvedList.asList())
	'
	Public Class RoleUnresolvedList
		Inherits List(Of Object)

		<NonSerialized> _
		Private typeSafe As Boolean
		<NonSerialized> _
		Private tainted As Boolean

		' Serial version 
		Private Const serialVersionUID As Long = 4054902803091433324L

		'
		' Constructors
		'

		''' <summary>
		''' Constructs an empty RoleUnresolvedList.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an empty RoleUnresolvedList with the initial capacity
		''' specified.
		''' </summary>
		''' <param name="initialCapacity">  initial capacity </param>
		Public Sub New(ByVal initialCapacity As Integer)
			MyBase.New(initialCapacity)
		End Sub

		''' <summary>
		''' Constructs a {@code RoleUnresolvedList} containing the elements of the
		''' {@code List} specified, in the order in which they are returned by
		''' the {@code List}'s iterator. The {@code RoleUnresolvedList} instance has
		''' an initial capacity of 110% of the size of the {@code List}
		''' specified.
		''' </summary>
		''' <param name="list"> the {@code List} that defines the initial contents of
		''' the new {@code RoleUnresolvedList}.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the {@code list} parameter
		''' is {@code null} or if the {@code list} parameter contains any
		''' non-RoleUnresolved objects.
		''' </exception>
		''' <seealso cref= ArrayList#ArrayList(java.util.Collection) </seealso>
		Public Sub New(ByVal list As IList(Of RoleUnresolved))
			' Check for null parameter
			'
			If list Is Nothing Then Throw New System.ArgumentException("Null parameter")

			' Check for non-RoleUnresolved objects
			'
			checkTypeSafe(list)

			' Build the List<RoleUnresolved>
			'
			MyBase.addAll(list)
		End Sub

		''' <summary>
		''' Return a view of this list as a {@code List<RoleUnresolved>}.
		''' Changes to the returned value are reflected by changes
		''' to the original {@code RoleUnresolvedList} and vice versa.
		''' </summary>
		''' <returns> a {@code List<RoleUnresolved>} whose contents
		''' reflect the contents of this {@code RoleUnresolvedList}.
		''' 
		''' <p>If this method has ever been called on a given
		''' {@code RoleUnresolvedList} instance, a subsequent attempt to add
		''' an object to that instance which is not a {@code RoleUnresolved}
		''' will fail with an {@code IllegalArgumentException}. For compatibility
		''' reasons, a {@code RoleUnresolvedList} on which this method has never
		''' been called does allow objects other than {@code RoleUnresolved}s to
		''' be added.</p>
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if this {@code RoleUnresolvedList}
		''' contains an element that is not a {@code RoleUnresolved}.
		''' 
		''' @since 1.6 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function asList() As IList(Of RoleUnresolved)
			If Not typeSafe Then
				If tainted Then checkTypeSafe(Me)
				typeSafe = True
			End If
			Return com.sun.jmx.mbeanserver.Util.cast(Me)
		End Function

		'
		' Accessors
		'

		''' <summary>
		''' Adds the RoleUnresolved specified as the last element of the list.
		''' </summary>
		''' <param name="role"> - the unresolved role to be added.
		''' </param>
		''' <exception cref="IllegalArgumentException">  if the unresolved role is null. </exception>
		Public Overridable Sub add(ByVal ___role As RoleUnresolved)

			If ___role Is Nothing Then
				Dim excMsg As String = "Invalid parameter"
				Throw New System.ArgumentException(excMsg)
			End If
			MyBase.add(___role)
		End Sub

		''' <summary>
		''' Inserts the unresolved role specified as an element at the position
		''' specified.
		''' Elements with an index greater than or equal to the current position are
		''' shifted up.
		''' </summary>
		''' <param name="index"> - The position in the list where the new
		''' RoleUnresolved object is to be inserted. </param>
		''' <param name="role"> - The RoleUnresolved object to be inserted.
		''' </param>
		''' <exception cref="IllegalArgumentException">  if the unresolved role is null. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		''' (<code>index &lt; 0 || index &gt; size()</code>). </exception>
		Public Overridable Sub add(ByVal index As Integer, ByVal ___role As RoleUnresolved)

			If ___role Is Nothing Then
				Dim excMsg As String = "Invalid parameter"
				Throw New System.ArgumentException(excMsg)
			End If

			MyBase.add(index, ___role)
		End Sub

		''' <summary>
		''' Sets the element at the position specified to be the unresolved role
		''' specified.
		''' The previous element at that position is discarded.
		''' </summary>
		''' <param name="index"> - The position specified. </param>
		''' <param name="role"> - The value to which the unresolved role element
		''' should be set.
		''' </param>
		''' <exception cref="IllegalArgumentException">   if the unresolved role is null. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		''' (<code>index &lt; 0 || index &gt;= size()</code>). </exception>
		 Public Overridable Sub [set](ByVal index As Integer, ByVal ___role As RoleUnresolved)

			If ___role Is Nothing Then
				Dim excMsg As String = "Invalid parameter"
				Throw New System.ArgumentException(excMsg)
			End If

			MyBase.set(index, ___role)
		 End Sub

		''' <summary>
		''' Appends all the elements in the RoleUnresolvedList specified to the end
		''' of the list, in the order in which they are returned by the Iterator of
		''' the RoleUnresolvedList specified.
		''' </summary>
		''' <param name="roleList"> - Elements to be inserted into the list
		''' (can be null).
		''' </param>
		''' <returns> true if this list changed as a result of the call.
		''' </returns>
		''' <exception cref="IndexOutOfBoundsException">  if accessing with an index
		''' outside of the list. </exception>
		Public Overridable Function addAll(ByVal roleList As RoleUnresolvedList) As Boolean

			If roleList Is Nothing Then Return True

			Return (MyBase.addAll(roleList))
		End Function

		''' <summary>
		''' Inserts all of the elements in the RoleUnresolvedList specified into
		''' this list, starting at the specified position, in the order in which
		''' they are returned by the Iterator of the RoleUnresolvedList specified.
		''' </summary>
		''' <param name="index"> - Position at which to insert the first element from the
		''' RoleUnresolvedList specified. </param>
		''' <param name="roleList"> - Elements to be inserted into the list.
		''' </param>
		''' <returns> true if this list changed as a result of the call.
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if the role is null. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		''' (<code>index &lt; 0 || index &gt; size()</code>). </exception>
		Public Overridable Function addAll(ByVal index As Integer, ByVal roleList As RoleUnresolvedList) As Boolean

			If roleList Is Nothing Then
				Dim excMsg As String = "Invalid parameter"
				Throw New System.ArgumentException(excMsg)
			End If

			Return (MyBase.addAll(index, roleList))
		End Function

	'    
	'     * Override all of the methods from ArrayList<Object> that might add
	'     * a non-RoleUnresolved to the List, and disallow that if asList has
	'     * ever been called on this instance.
	'     

		Public Overrides Function add(ByVal o As Object) As Boolean
			If Not tainted Then tainted = isTainted(o)
			If typeSafe Then checkTypeSafe(o)
			Return MyBase.add(o)
		End Function

		Public Overrides Sub add(ByVal index As Integer, ByVal element As Object)
			If Not tainted Then tainted = isTainted(element)
			If typeSafe Then checkTypeSafe(element)
			MyBase.add(index, element)
		End Sub

		Public Overrides Function addAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			If Not tainted Then tainted = isTainted(c)
			If typeSafe Then checkTypeSafe(c)
			Return MyBase.addAll(c)
		End Function

		Public Overrides Function addAll(Of T1)(ByVal index As Integer, ByVal c As ICollection(Of T1)) As Boolean
			If Not tainted Then tainted = isTainted(c)
			If typeSafe Then checkTypeSafe(c)
			Return MyBase.addAll(index, c)
		End Function

		Public Overrides Function [set](ByVal index As Integer, ByVal element As Object) As Object
			If Not tainted Then tainted = isTainted(element)
			If typeSafe Then checkTypeSafe(element)
			Return MyBase.set(index, element)
		End Function

		''' <summary>
		''' IllegalArgumentException if o is a non-RoleUnresolved object.
		''' </summary>
		Private Shared Sub checkTypeSafe(ByVal o As Object)
			Try
				o = CType(o, RoleUnresolved)
			Catch e As ClassCastException
				Throw New System.ArgumentException(e)
			End Try
		End Sub

		''' <summary>
		''' IllegalArgumentException if c contains any non-RoleUnresolved objects.
		''' </summary>
		Private Shared Sub checkTypeSafe(Of T1)(ByVal c As ICollection(Of T1))
			Try
				Dim r As RoleUnresolved
				For Each o As Object In c
					r = CType(o, RoleUnresolved)
				Next o
			Catch e As ClassCastException
				Throw New System.ArgumentException(e)
			End Try
		End Sub

		''' <summary>
		''' Returns true if o is a non-RoleUnresolved object.
		''' </summary>
		Private Shared Function isTainted(ByVal o As Object) As Boolean
			Try
				checkTypeSafe(o)
			Catch e As System.ArgumentException
				Return True
			End Try
			Return False
		End Function

		''' <summary>
		''' Returns true if c contains any non-RoleUnresolved objects.
		''' </summary>
		Private Shared Function isTainted(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			Try
				checkTypeSafe(c)
			Catch e As System.ArgumentException
				Return True
			End Try
			Return False
		End Function
	End Class

End Namespace
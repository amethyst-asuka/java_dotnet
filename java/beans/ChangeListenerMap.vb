Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.beans


	''' <summary>
	''' This is an abstract class that provides base functionality
	''' for the <seealso cref="PropertyChangeSupport PropertyChangeSupport"/> class
	''' and the <seealso cref="VetoableChangeSupport VetoableChangeSupport"/> class.
	''' </summary>
	''' <seealso cref= PropertyChangeListenerMap </seealso>
	''' <seealso cref= VetoableChangeListenerMap
	''' 
	''' @author Sergey A. Malenkov </seealso>
	Friend MustInherit Class ChangeListenerMap(Of L As java.util.EventListener)
		Private map As IDictionary(Of String, L())

		''' <summary>
		''' Creates an array of listeners.
		''' This method can be optimized by using
		''' the same instance of the empty array
		''' when {@code length} is equal to {@code 0}.
		''' </summary>
		''' <param name="length">  the array length </param>
		''' <returns>        an array with specified length </returns>
		Protected Friend MustOverride Function newArray(  length As Integer) As L()

		''' <summary>
		''' Creates a proxy listener for the specified property.
		''' </summary>
		''' <param name="name">      the name of the property to listen on </param>
		''' <param name="listener">  the listener to process events </param>
		''' <returns>          a proxy listener </returns>
		Protected Friend MustOverride Function newProxy(  name As String,   listener As L) As L

		''' <summary>
		''' Adds a listener to the list of listeners for the specified property.
		''' This listener is called as many times as it was added.
		''' </summary>
		''' <param name="name">      the name of the property to listen on </param>
		''' <param name="listener">  the listener to process events </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Sub add(  name As String,   listener As L)
			If Me.map Is Nothing Then Me.map = New Dictionary(Of )
			Dim array As L() = Me.map(name)
			Dim size As Integer = If(array IsNot Nothing, array.Length, 0)

			Dim clone As L() = newArray(size + 1)
			clone(size) = listener
			If array IsNot Nothing Then Array.Copy(array, 0, clone, 0, size)
			Me.map(name) = clone
		End Sub

		''' <summary>
		''' Removes a listener from the list of listeners for the specified property.
		''' If the listener was added more than once to the same event source,
		''' this listener will be notified one less time after being removed.
		''' </summary>
		''' <param name="name">      the name of the property to listen on </param>
		''' <param name="listener">  the listener to process events </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Sub remove(  name As String,   listener As L)
			If Me.map IsNot Nothing Then
				Dim array As L() = Me.map(name)
				If array IsNot Nothing Then
					For i As Integer = 0 To array.Length - 1
						If listener.Equals(array(i)) Then
							Dim size As Integer = array.Length - 1
							If size > 0 Then
								Dim clone As L() = newArray(size)
								Array.Copy(array, 0, clone, 0, i)
								Array.Copy(array, i + 1, clone, i, size - i)
								Me.map(name) = clone
							Else
								Me.map.Remove(name)
								If Me.map.Count = 0 Then Me.map = Nothing
							End If
							Exit For
						End If
					Next i
				End If
			End If
		End Sub

		''' <summary>
		''' Returns the list of listeners for the specified property.
		''' </summary>
		''' <param name="name">  the name of the property </param>
		''' <returns>      the corresponding list of listeners </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Function [get](  name As String) As L()
			Return If(Me.map IsNot Nothing, Me.map(name), Nothing)
		End Function

		''' <summary>
		''' Sets new list of listeners for the specified property.
		''' </summary>
		''' <param name="name">       the name of the property </param>
		''' <param name="listeners">  new list of listeners </param>
		Public Sub [set](  name As String,   listeners As L())
			If listeners IsNot Nothing Then
				If Me.map Is Nothing Then Me.map = New Dictionary(Of )
				Me.map(name) = listeners
			ElseIf Me.map IsNot Nothing Then
				Me.map.Remove(name)
				If Me.map.Count = 0 Then Me.map = Nothing
			End If
		End Sub

		''' <summary>
		''' Returns all listeners in the map.
		''' </summary>
		''' <returns> an array of all listeners </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property listeners As L()
			Get
				If Me.map Is Nothing Then Return newArray(0)
				Dim list As IList(Of L) = New List(Of L)
    
				Dim listeners_Renamed As L() = Me.map(Nothing)
				If listeners_Renamed IsNot Nothing Then
					For Each listener As L In listeners_Renamed
						list.Add(listener)
					Next listener
				End If
				For Each entry As KeyValuePair(Of String, L()) In Me.map
					Dim name As String = entry.Key
					If name IsNot Nothing Then
						For Each listener As L In entry.Value
							list.Add(newProxy(name, listener))
						Next listener
					End If
				Next entry
				Return list.ToArray(newArray(list.Count))
			End Get
		End Property

		''' <summary>
		''' Returns listeners that have been associated with the named property.
		''' </summary>
		''' <param name="name">  the name of the property </param>
		''' <returns> an array of listeners for the named property </returns>
		Public Function getListeners(  name As String) As L()
			If name IsNot Nothing Then
				Dim listeners_Renamed As L() = [get](name)
				If listeners_Renamed IsNot Nothing Then Return listeners_Renamed.clone()
			End If
			Return newArray(0)
		End Function

		''' <summary>
		''' Indicates whether the map contains
		''' at least one listener to be notified.
		''' </summary>
		''' <param name="name">  the name of the property </param>
		''' <returns>      {@code true} if at least one listener exists or
		'''              {@code false} otherwise </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Function hasListeners(  name As String) As Boolean
			If Me.map Is Nothing Then Return False
			Dim array As L() = Me.map(Nothing)
			Return (array IsNot Nothing) OrElse ((name IsNot Nothing) AndAlso (Nothing IsNot Me.map(name)))
		End Function

		''' <summary>
		''' Returns a set of entries from the map.
		''' Each entry is a pair consisted of the property name
		''' and the corresponding list of listeners.
		''' </summary>
		''' <returns> a set of entries from the map </returns>
		Public Property entries As java.util.Set(Of KeyValuePair(Of String, L()))
			Get
	'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
				Return If(Me.map IsNot Nothing, Me.map.entrySet(), java.util.Collections.emptySet(Of KeyValuePair(Of String, L()))())
			End Get
		End Property

		''' <summary>
		''' Extracts a real listener from the proxy listener.
		''' It is necessary because default proxy class is not serializable.
		''' </summary>
		''' <returns> a real listener </returns>
		Public MustOverride Function extract(  listener As L) As L
	End Class

End Namespace
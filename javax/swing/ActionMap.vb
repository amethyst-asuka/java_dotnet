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
Namespace javax.swing


	''' <summary>
	''' <code>ActionMap</code> provides mappings from
	''' <code>Object</code>s
	''' (called <em>keys</em> or <em><code>Action</code> names</em>)
	''' to <code>Action</code>s.
	''' An <code>ActionMap</code> is usually used with an <code>InputMap</code>
	''' to locate a particular action
	''' when a key is pressed. As with <code>InputMap</code>,
	''' an <code>ActionMap</code> can have a parent
	''' that is searched for keys not defined in the <code>ActionMap</code>.
	''' <p>As with <code>InputMap</code> if you create a cycle, eg:
	''' <pre>
	'''   ActionMap am = new ActionMap();
	'''   ActionMap bm = new ActionMap():
	'''   am.setParent(bm);
	'''   bm.setParent(am);
	''' </pre>
	''' some of the methods will cause a StackOverflowError to be thrown.
	''' </summary>
	''' <seealso cref= InputMap
	''' 
	''' @author Scott Violet
	''' @since 1.3 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class ActionMap
		''' <summary>
		''' Handles the mapping between Action name and Action. </summary>
		<NonSerialized> _
		Private arrayTable As ArrayTable
		''' <summary>
		''' Parent that handles any bindings we don't contain. </summary>
		Private parent As ActionMap


		''' <summary>
		''' Creates an <code>ActionMap</code> with no parent and no mappings.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Sets this <code>ActionMap</code>'s parent.
		''' </summary>
		''' <param name="map">  the <code>ActionMap</code> that is the parent of this one </param>
		Public Overridable Property parent As ActionMap
			Set(ByVal map As ActionMap)
				Me.parent = map
			End Set
			Get
				Return parent
			End Get
		End Property


		''' <summary>
		''' Adds a binding for <code>key</code> to <code>action</code>.
		''' If <code>action</code> is null, this removes the current binding
		''' for <code>key</code>.
		''' <p>In most instances, <code>key</code> will be
		''' <code>action.getValue(NAME)</code>.
		''' </summary>
		Public Overridable Sub put(ByVal key As Object, ByVal action As Action)
			If key Is Nothing Then Return
			If action Is Nothing Then
				remove(key)
			Else
				If arrayTable Is Nothing Then arrayTable = New ArrayTable
				arrayTable.put(key, action)
			End If
		End Sub

		''' <summary>
		''' Returns the binding for <code>key</code>, messaging the
		''' parent <code>ActionMap</code> if the binding is not locally defined.
		''' </summary>
		Public Overridable Function [get](ByVal key As Object) As Action
			Dim value As Action = If(arrayTable Is Nothing, Nothing, CType(arrayTable.get(key), Action))

			If value Is Nothing Then
				Dim ___parent As ActionMap = parent

				If ___parent IsNot Nothing Then Return ___parent.get(key)
			End If
			Return value
		End Function

		''' <summary>
		''' Removes the binding for <code>key</code> from this <code>ActionMap</code>.
		''' </summary>
		Public Overridable Sub remove(ByVal key As Object)
			If arrayTable IsNot Nothing Then arrayTable.remove(key)
		End Sub

		''' <summary>
		''' Removes all the mappings from this <code>ActionMap</code>.
		''' </summary>
		Public Overridable Sub clear()
			If arrayTable IsNot Nothing Then arrayTable.clear()
		End Sub

		''' <summary>
		''' Returns the <code>Action</code> names that are bound in this <code>ActionMap</code>.
		''' </summary>
		Public Overridable Function keys() As Object()
			If arrayTable Is Nothing Then Return Nothing
			Return arrayTable.getKeys(Nothing)
		End Function

		''' <summary>
		''' Returns the number of bindings in this {@code ActionMap}.
		''' </summary>
		''' <returns> the number of bindings in this {@code ActionMap} </returns>
		Public Overridable Function size() As Integer
			If arrayTable Is Nothing Then Return 0
			Return arrayTable.size()
		End Function

		''' <summary>
		''' Returns an array of the keys defined in this <code>ActionMap</code> and
		''' its parent. This method differs from <code>keys()</code> in that
		''' this method includes the keys defined in the parent.
		''' </summary>
		Public Overridable Function allKeys() As Object()
			Dim count As Integer = size()
			Dim ___parent As ActionMap = parent

			If count = 0 Then
				If ___parent IsNot Nothing Then Return ___parent.allKeys()
				Return keys()
			End If
			If ___parent Is Nothing Then Return keys()
			Dim keys As Object() = keys()
			Dim pKeys As Object() = ___parent.allKeys()

			If pKeys Is Nothing Then Return keys
			If keys Is Nothing Then Return pKeys

			Dim keyMap As New Dictionary(Of Object, Object)
			Dim counter As Integer

			For counter = keys.Length - 1 To 0 Step -1
				keyMap(keys(counter)) = keys(counter)
			Next counter
			For counter = pKeys.Length - 1 To 0 Step -1
				keyMap(pKeys(counter)) = pKeys(counter)
			Next counter
			Return keyMap.Keys.ToArray()
		End Function

		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()

			ArrayTable.writeArrayTable(s, arrayTable)
		End Sub

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			For counter As Integer = s.readInt() - 1 To 0 Step -1
				put(s.readObject(), CType(s.readObject(), Action))
			Next counter
		End Sub
	End Class

End Namespace
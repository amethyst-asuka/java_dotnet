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
	''' <code>InputMap</code> provides a binding between an input event
	''' (currently only <code>KeyStroke</code>s are used)
	''' and an <code>Object</code>. <code>InputMap</code>s
	''' are usually used with an <code>ActionMap</code>,
	''' to determine an <code>Action</code> to perform
	''' when a key is pressed.
	''' An <code>InputMap</code> can have a parent
	''' that is searched for bindings not defined in the <code>InputMap</code>.
	''' <p>As with <code>ActionMap</code> if you create a cycle, eg:
	''' <pre>
	'''   InputMap am = new InputMap();
	'''   InputMap bm = new InputMap():
	'''   am.setParent(bm);
	'''   bm.setParent(am);
	''' </pre>
	''' some of the methods will cause a StackOverflowError to be thrown.
	''' 
	''' @author Scott Violet
	''' @since 1.3
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class InputMap
		''' <summary>
		''' Handles the mapping between KeyStroke and Action name. </summary>
		<NonSerialized> _
		Private ___arrayTable As ArrayTable
		''' <summary>
		''' Parent that handles any bindings we don't contain. </summary>
		Private parent As InputMap


		''' <summary>
		''' Creates an <code>InputMap</code> with no parent and no mappings.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Sets this <code>InputMap</code>'s parent.
		''' </summary>
		''' <param name="map">  the <code>InputMap</code> that is the parent of this one </param>
		Public Overridable Property parent As InputMap
			Set(ByVal map As InputMap)
				Me.parent = map
			End Set
			Get
				Return parent
			End Get
		End Property


		''' <summary>
		''' Adds a binding for <code>keyStroke</code> to <code>actionMapKey</code>.
		''' If <code>actionMapKey</code> is null, this removes the current binding
		''' for <code>keyStroke</code>.
		''' </summary>
		Public Overridable Sub put(ByVal ___keyStroke As KeyStroke, ByVal actionMapKey As Object)
			If ___keyStroke Is Nothing Then Return
			If actionMapKey Is Nothing Then
				remove(___keyStroke)
			Else
				If ___arrayTable Is Nothing Then ___arrayTable = New ArrayTable
				___arrayTable.put(___keyStroke, actionMapKey)
			End If
		End Sub

		''' <summary>
		''' Returns the binding for <code>keyStroke</code>, messaging the
		''' parent <code>InputMap</code> if the binding is not locally defined.
		''' </summary>
		Public Overridable Function [get](ByVal ___keyStroke As KeyStroke) As Object
			If ___arrayTable Is Nothing Then
				Dim ___parent As InputMap = parent

				If ___parent IsNot Nothing Then Return ___parent.get(___keyStroke)
				Return Nothing
			End If
			Dim value As Object = ___arrayTable.get(___keyStroke)

			If value Is Nothing Then
				Dim ___parent As InputMap = parent

				If ___parent IsNot Nothing Then Return ___parent.get(___keyStroke)
			End If
			Return value
		End Function

		''' <summary>
		''' Removes the binding for <code>key</code> from this
		''' <code>InputMap</code>.
		''' </summary>
		Public Overridable Sub remove(ByVal key As KeyStroke)
			If ___arrayTable IsNot Nothing Then ___arrayTable.remove(key)
		End Sub

		''' <summary>
		''' Removes all the mappings from this <code>InputMap</code>.
		''' </summary>
		Public Overridable Sub clear()
			If ___arrayTable IsNot Nothing Then ___arrayTable.clear()
		End Sub

		''' <summary>
		''' Returns the <code>KeyStroke</code>s that are bound in this <code>InputMap</code>.
		''' </summary>
		Public Overridable Function keys() As KeyStroke()
			If ___arrayTable Is Nothing Then Return Nothing
			Dim ___keys As KeyStroke() = New KeyStroke(___arrayTable.size() - 1){}
			___arrayTable.getKeys(___keys)
			Return ___keys
		End Function

		''' <summary>
		''' Returns the number of <code>KeyStroke</code> bindings.
		''' </summary>
		Public Overridable Function size() As Integer
			If ___arrayTable Is Nothing Then Return 0
			Return ___arrayTable.size()
		End Function

		''' <summary>
		''' Returns an array of the <code>KeyStroke</code>s defined in this
		''' <code>InputMap</code> and its parent. This differs from <code>keys()</code> in that
		''' this method includes the keys defined in the parent.
		''' </summary>
		Public Overridable Function allKeys() As KeyStroke()
			Dim count As Integer = size()
			Dim ___parent As InputMap = parent

			If count = 0 Then
				If ___parent IsNot Nothing Then Return ___parent.allKeys()
				Return keys()
			End If
			If ___parent Is Nothing Then Return keys()
			Dim keys As KeyStroke() = keys()
			Dim pKeys As KeyStroke() = ___parent.allKeys()

			If pKeys Is Nothing Then Return keys
			If keys Is Nothing Then Return pKeys

			Dim keyMap As New Dictionary(Of KeyStroke, KeyStroke)
			Dim counter As Integer

			For counter = keys.Length - 1 To 0 Step -1
				keyMap(keys(counter)) = keys(counter)
			Next counter
			For counter = pKeys.Length - 1 To 0 Step -1
				keyMap(pKeys(counter)) = pKeys(counter)
			Next counter

			Dim ___allKeys As KeyStroke() = New KeyStroke(keyMap.Count - 1){}

			Return keyMap.Keys.ToArray(___allKeys)
		End Function

		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()

			ArrayTable.writeArrayTable(s, ___arrayTable)
		End Sub

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			For counter As Integer = s.readInt() - 1 To 0 Step -1
				put(CType(s.readObject(), KeyStroke), s.readObject())
			Next counter
		End Sub
	End Class

End Namespace
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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




	''' 
	''' <summary>
	''' @author Hans Muller
	''' </summary>
	Friend Class MultiUIDefaults
		Inherits UIDefaults

		Private tables As UIDefaults()

		Public Sub New(ByVal defaults As UIDefaults())
			MyBase.New()
			tables = defaults
		End Sub

		Public Sub New()
			MyBase.New()
			tables = New UIDefaults(){}
		End Sub

		Public Overrides Function [get](ByVal key As Object) As Object
			Dim value As Object = MyBase.get(key)
			If value IsNot Nothing Then Return value

			For Each table As UIDefaults In tables
				value = If(table IsNot Nothing, table(key), Nothing)
				If value IsNot Nothing Then Return value
			Next table

			Return Nothing
		End Function

		Public Overrides Function [get](ByVal key As Object, ByVal l As java.util.Locale) As Object
			Dim value As Object = MyBase.get(key,l)
			If value IsNot Nothing Then Return value

			For Each table As UIDefaults In tables
				value = If(table IsNot Nothing, table.get(key,l), Nothing)
				If value IsNot Nothing Then Return value
			Next table

			Return Nothing
		End Function

		Public Overrides Function size() As Integer
			Return entrySet().size()
		End Function

		Public Property Overrides empty As Boolean
			Get
				Return size() = 0
			End Get
		End Property

		Public Overrides Function keys() As System.Collections.IEnumerator(Of Object)
			Return New MultiUIDefaultsEnumerator(MultiUIDefaultsEnumerator.Type.KEYS, entrySet())
		End Function

		Public Overrides Function elements() As System.Collections.IEnumerator(Of Object)
			Return New MultiUIDefaultsEnumerator(MultiUIDefaultsEnumerator.Type.ELEMENTS, entrySet())
		End Function

		Public Overrides Function entrySet() As java.util.Set(Of KeyValuePair(Of Object, Object))
			Dim [set] As java.util.Set(Of KeyValuePair(Of Object, Object)) = New HashSet(Of KeyValuePair(Of Object, Object))
			For i As Integer = tables.Length - 1 To 0 Step -1
				If tables(i) IsNot Nothing Then [set].addAll(tables(i).entrySet())
			Next i
			[set].addAll(MyBase.entrySet())
			Return [set]
		End Function

		Protected Friend Overrides Sub getUIError(ByVal msg As String)
			If tables.Length > 0 Then
				tables(0).getUIError(msg)
			Else
				MyBase.getUIError(msg)
			End If
		End Sub

		Private Class MultiUIDefaultsEnumerator
			Implements System.Collections.IEnumerator(Of Object)

			Public Enum Type
				KEYS
				ELEMENTS
			End Enum
			Private [iterator] As IEnumerator(Of KeyValuePair(Of Object, Object))
			Private type As Type

			Friend Sub New(ByVal type As Type, ByVal entries As java.util.Set(Of KeyValuePair(Of Object, Object)))
				Me.type = type
				Me.iterator = entries.GetEnumerator()
			End Sub

			Public Overridable Function hasMoreElements() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return [iterator].hasNext()
			End Function

			Public Overridable Function nextElement() As Object
				Select Case type
					Case Type.KEYS
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Return [iterator].next().key
					Case Type.ELEMENTS
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Return [iterator].next().value
					Case Else
						Return Nothing
				End Select
			End Function
		End Class

		Public Overrides Function remove(ByVal key As Object) As Object
			Dim value As Object = Nothing
			For i As Integer = tables.Length - 1 To 0 Step -1
				If tables(i) IsNot Nothing Then
					Dim v As Object = tables(i).Remove(key)
					If v IsNot Nothing Then value = v
				End If
			Next i
			Dim v As Object = MyBase.remove(key)
			If v IsNot Nothing Then value = v

			Return value
		End Function

		Public Overrides Sub clear()
			MyBase.clear()
			For Each table As UIDefaults In tables
				If table IsNot Nothing Then table.Clear()
			Next table
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function ToString() As String
			Dim buf As New StringBuilder
			buf.Append("{")
			Dim keys As System.Collections.IEnumerator = keys()
			Do While keys.hasMoreElements()
				Dim key As Object = keys.nextElement()
				buf.Append(key & "=" & [get](key) & ", ")
			Loop
			Dim length As Integer = buf.Length
			If length > 1 Then buf.Remove(length-2, length)
			buf.Append("}")
			Return buf.ToString()
		End Function
	End Class

End Namespace
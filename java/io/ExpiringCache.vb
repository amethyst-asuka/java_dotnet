Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 2002, 2011, Oracle and/or its affiliates. All rights reserved.
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

'
' 

Namespace java.io


	Friend Class ExpiringCache
		Private millisUntilExpiration As Long
		Private map As IDictionary(Of String, Entry)
		' Clear out old entries every few queries
		Private queryCount As Integer
		Private queryOverflow As Integer = 300
		Private MAX_ENTRIES As Integer = 200

		Friend Class Entry
			Private timestamp_Renamed As Long
			Private val_Renamed As String

			Friend Sub New(ByVal timestamp As Long, ByVal val As String)
				Me.timestamp_Renamed = timestamp
				Me.val_Renamed = val
			End Sub

			Friend Overridable Function timestamp() As Long
				Return timestamp_Renamed
			End Function
			Friend Overridable Property timestamp As Long
				Set(ByVal timestamp As Long)
					Me.timestamp_Renamed = timestamp
				End Set
			End Property

			Friend Overridable Function val() As String
				Return val_Renamed
			End Function
			Friend Overridable Property val As String
				Set(ByVal val As String)
					Me.val_Renamed = val
				End Set
			End Property
		End Class

		Friend Sub New()
			Me.New(30000)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Sub New(ByVal millisUntilExpiration As Long)
			Me.millisUntilExpiration = millisUntilExpiration
			map = New LinkedHashMapAnonymousInnerClassHelper(Of K, V)
		End Sub

		Private Class LinkedHashMapAnonymousInnerClassHelper(Of K, V)
			Inherits java.util.LinkedHashMap(Of K, V)

			Protected Friend Overridable Function removeEldestEntry(ByVal eldest As KeyValuePair(Of String, Entry)) As Boolean
			  Return size() > outerInstance.MAX_ENTRIES
			End Function
		End Class

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Function [get](ByVal key As String) As String
			queryCount += 1
			If queryCount >= queryOverflow Then cleanup()
			Dim entry As Entry = entryFor(key)
			If entry IsNot Nothing Then Return entry.val()
			Return Nothing
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Sub put(ByVal key As String, ByVal val As String)
			queryCount += 1
			If queryCount >= queryOverflow Then cleanup()
			Dim entry As Entry = entryFor(key)
			If entry IsNot Nothing Then
				entry.timestamp = System.currentTimeMillis()
				entry.val = val
			Else
				map(key) = New Entry(System.currentTimeMillis(), val)
			End If
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Sub clear()
			map.Clear()
		End Sub

		Private Function entryFor(ByVal key As String) As Entry
			Dim entry As Entry = map(key)
			If entry IsNot Nothing Then
				Dim delta As Long = System.currentTimeMillis() - entry.timestamp()
				If delta < 0 OrElse delta >= millisUntilExpiration Then
					map.Remove(key)
					entry = Nothing
				End If
			End If
			Return entry
		End Function

		Private Sub cleanup()
			Dim keySet As IDictionary(Of String, Entry).KeyCollection = map.Keys
			' Avoid ConcurrentModificationExceptions
			Dim keys As String() = New String(keySet.size() - 1){}
			Dim i As Integer = 0
			For Each key As String In keySet
				keys(i) = key
				i += 1
			Next key
			For j As Integer = 0 To keys.Length - 1
				entryFor(keys(j))
			Next j
			queryCount = 0
		End Sub
	End Class

End Namespace
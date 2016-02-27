'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Hash table based mapping, which uses weak references to store keys
	''' and reference-equality in place of object-equality to compare them.
	''' An entry will automatically be removed when its key is no longer
	''' in ordinary use.  Both null values and the null key are supported.
	''' This class does not require additional synchronization.
	''' A thread-safety is provided by a fragile combination
	''' of synchronized blocks and volatile fields.
	''' Be very careful during editing!
	''' </summary>
	''' <seealso cref= java.util.IdentityHashMap </seealso>
	''' <seealso cref= java.util.WeakHashMap </seealso>
	Friend MustInherit Class WeakIdentityMap(Of T)

		Private Shared ReadOnly MAXIMUM_CAPACITY As Integer = 1 << 30 ' it MUST be a power of two
		Private Shared ReadOnly NULL As New Object ' special object for null key

		Private ReadOnly queue As New ReferenceQueue(Of Object)

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private table As Entry(Of T)() = newTable(1<<3) ' table's length MUST be a power of two
		Private threshold As Integer = 6 ' the next size value at which to resize
		Private size As Integer = 0 ' the number of key-value mappings

		Public Overridable Function [get](ByVal key As Object) As T
			removeStaleEntries()
			If key Is Nothing Then key = NULL
			Dim hash As Integer = key.GetHashCode()
			Dim table As Entry(Of T)() = Me.table
			' unsynchronized search improves performance
			' the null value does not mean that there are no needed entry
			Dim index_Renamed As Integer = getIndex(table, hash)
			Dim entry As Entry(Of T) = table(index_Renamed)
			Do While entry IsNot Nothing
				If entry.isMatched(key, hash) Then Return entry.value
				entry = entry.next
			Loop
			SyncLock NULL
				' synchronized search improves stability
				' we must create and add new value if there are no needed entry
				index_Renamed = getIndex(Me.table, hash)
				entry = Me.table(index_Renamed)
				Do While entry IsNot Nothing
					If entry.isMatched(key, hash) Then Return entry.value
					entry = entry.next
				Loop
				Dim value As T = create(key)
				Me.table(index_Renamed) = New Entry(Of T)(key, hash, value, Me.queue, Me.table(index_Renamed))
				Me.size += 1
				If Me.size >= Me.threshold Then
					If Me.table.Length = MAXIMUM_CAPACITY Then
						Me.threshold =  java.lang.[Integer].Max_Value
					Else
						removeStaleEntries()
						table = newTable(Me.table.Length * 2)
						transfer(Me.table, table)
						' If ignoring null elements and processing ref queue caused massive
						' shrinkage, then restore old table.  This should be rare, but avoids
						' unbounded expansion of garbage-filled tables.
						If Me.size >= Me.threshold \ 2 Then
							Me.table = table
							Me.threshold *= 2
						Else
							transfer(table, Me.table)
						End If
					End If
				End If
				Return value
			End SyncLock
		End Function

		Protected Friend MustOverride Function create(ByVal key As Object) As T

		Private Sub removeStaleEntries()
			Dim ref As Object = Me.queue.poll()
			If ref IsNot Nothing Then
				SyncLock NULL
					Do
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim entry As Entry(Of T) = CType(ref, Entry(Of T))
						Dim index_Renamed As Integer = getIndex(Me.table, entry.hash)

						Dim prev As Entry(Of T) = Me.table(index_Renamed)
						Dim current As Entry(Of T) = prev
						Do While current IsNot Nothing
							Dim [next] As Entry(Of T) = current.next
							If current Is entry Then
								If prev Is entry Then
									Me.table(index_Renamed) = [next]
								Else
									prev.next = [next]
								End If
								entry.value = Nothing ' Help GC
								entry.next = Nothing ' Help GC
								Me.size -= 1
								Exit Do
							End If
							prev = current
							current = [next]
						Loop
						ref = Me.queue.poll()
					Loop While ref IsNot Nothing
				End SyncLock
			End If
		End Sub

		Private Sub transfer(ByVal oldTable As Entry(Of T)(), ByVal newTable As Entry(Of T)())
			For i As Integer = 0 To oldTable.Length - 1
				Dim entry As Entry(Of T) = oldTable(i)
				oldTable(i) = Nothing
				Do While entry IsNot Nothing
					Dim [next] As Entry(Of T) = entry.next
					Dim key As Object = entry.get()
					If key Is Nothing Then
						entry.value = Nothing ' Help GC
						entry.next = Nothing ' Help GC
						Me.size -= 1
					Else
						Dim index_Renamed As Integer = getIndex(newTable, entry.hash)
						entry.next = newTable(index_Renamed)
						newTable(index_Renamed) = entry
					End If
					entry = [next]
				Loop
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Function newTable(ByVal length As Integer) As Entry(Of T)()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return CType(New Entry(Of ?)(length - 1){}, Entry(Of T)())
		End Function

		Private Shared Function getIndex(Of T1)(ByVal table As Entry(Of T1)(), ByVal hash As Integer) As Integer
			Return hash And (table.Length - 1)
		End Function

		Private Class Entry(Of T)
			Inherits WeakReference(Of Object)

			Private ReadOnly hash As Integer
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private value As T
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private [next] As Entry(Of T)

			Friend Sub New(ByVal key As Object, ByVal hash As Integer, ByVal value As T, ByVal queue As ReferenceQueue(Of Object), ByVal [next] As Entry(Of T))
				MyBase.New(key, queue)
				Me.hash = hash
				Me.value = value
				Me.next = [next]
			End Sub

			Friend Overridable Function isMatched(ByVal key As Object, ByVal hash As Integer) As Boolean
				Return (Me.hash = hash) AndAlso (key Is get())
			End Function
		End Class
	End Class

End Namespace
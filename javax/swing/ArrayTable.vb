Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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


	'
	' * Private storage mechanism for Action key-value pairs.
	' * In most cases this will be an array of alternating
	' * key-value pairs.  As it grows larger it is scaled
	' * up to a Hashtable.
	' * <p>
	' * This does no synchronization, if you need thread safety synchronize on
	' * another object before calling this.
	' *
	' * @author Georges Saab
	' * @author Scott Violet
	' 
	Friend Class ArrayTable
		Implements ICloneable

		' Our field for storage
		Private table As Object = Nothing
		Private Const ARRAY_BOUNDARY As Integer = 8


		''' <summary>
		''' Writes the passed in ArrayTable to the passed in ObjectOutputStream.
		''' The data is saved as an integer indicating how many key/value
		''' pairs are being archived, followed by the the key/value pairs. If
		''' <code>table</code> is null, 0 will be written to <code>s</code>.
		''' <p>
		''' This is a convenience method that ActionMap/InputMap and
		''' AbstractAction use to avoid having the same code in each class.
		''' </summary>
		Shared Sub writeArrayTable(ByVal s As java.io.ObjectOutputStream, ByVal table As ArrayTable)
			Dim ___keys As Object()

			___keys = table.getKeys(Nothing)
			If table Is Nothing OrElse ___keys Is Nothing Then
				s.writeInt(0)
			Else
				' Determine how many keys have Serializable values, when
				' done all non-null values in keys identify the Serializable
				' values.
				Dim validCount As Integer = 0

				For counter As Integer = 0 To ___keys.Length - 1
					Dim key As Object = ___keys(counter)

					' include in Serialization when both keys and values are Serializable 
					If (TypeOf key Is java.io.Serializable AndAlso TypeOf table.get(key) Is java.io.Serializable) OrElse (TypeOf key Is ClientPropertyKey AndAlso CType(key, ClientPropertyKey).reportValueNotSerializable) Then
							 ' include these only so that we get the appropriate exception below 

						validCount += 1
					Else
						___keys(counter) = Nothing
					End If
				Next counter
				' Write ou the Serializable key/value pairs.
				s.writeInt(validCount)
				If validCount > 0 Then
					For Each key As Object In ___keys
						If key IsNot Nothing Then
							s.writeObject(key)
							s.writeObject(table.get(key))
							validCount -= 1
							If validCount = 0 Then Exit For
						End If
					Next key
				End If
			End If
		End Sub


	'    
	'     * Put the key-value pair into storage
	'     
		Public Overridable Sub put(ByVal key As Object, ByVal value As Object)
			If table Is Nothing Then
				table = New Object() {key, value}
			Else
				Dim size As Integer = size()
				If size < ARRAY_BOUNDARY Then ' We are an array
					If containsKey(key) Then
						Dim tmp As Object() = CType(table, Object())
						For i As Integer = 0 To tmp.Length-2 Step 2
							If tmp(i).Equals(key) Then
								tmp(i+1)=value
								Exit For
							End If
						Next i
					Else
						Dim ___array As Object() = CType(table, Object())
						Dim i As Integer = ___array.Length
						Dim tmp As Object() = New Object(i+2 - 1){}
						Array.Copy(___array, 0, tmp, 0, i)

						tmp(i) = key
						tmp(i+1) = value
						table = tmp
					End If ' We are a hashtable
				Else
					If (size=ARRAY_BOUNDARY) AndAlso array Then grow()
					CType(table, Dictionary(Of Object, Object))(key) = value
				End If
			End If
		End Sub

	'    
	'     * Gets the value for key
	'     
		Public Overridable Function [get](ByVal key As Object) As Object
			Dim value As Object = Nothing
			If table IsNot Nothing Then
				If array Then
					Dim ___array As Object() = CType(table, Object())
					For i As Integer = 0 To ___array.Length-2 Step 2
						If ___array(i).Equals(key) Then
							value = ___array(i+1)
							Exit For
						End If
					Next i
				Else
					value = CType(table, Hashtable)(key)
				End If
			End If
			Return value
		End Function

	'    
	'     * Returns the number of pairs in storage
	'     
		Public Overridable Function size() As Integer
			Dim ___size As Integer
			If table Is Nothing Then Return 0
			If array Then
				___size = CType(table, Object()).Length\2
			Else
				___size = CType(table, Hashtable).Count
			End If
			Return ___size
		End Function

	'    
	'     * Returns true if we have a value for the key
	'     
		Public Overridable Function containsKey(ByVal key As Object) As Boolean
			Dim contains As Boolean = False
			If table IsNot Nothing Then
				If array Then
					Dim ___array As Object() = CType(table, Object())
					For i As Integer = 0 To ___array.Length-2 Step 2
						If ___array(i).Equals(key) Then
							contains = True
							Exit For
						End If
					Next i
				Else
					contains = CType(table, Hashtable).ContainsKey(key)
				End If
			End If
			Return contains
		End Function

	'    
	'     * Removes the key and its value
	'     * Returns the value for the pair removed
	'     
		Public Overridable Function remove(ByVal key As Object) As Object
			Dim value As Object = Nothing
			If key Is Nothing Then Return Nothing
			If table IsNot Nothing Then
				If array Then
					' Is key on the list?
					Dim index As Integer = -1
					Dim ___array As Object() = CType(table, Object())
					For i As Integer = ___array.Length-2 To 0 Step -2
						If ___array(i).Equals(key) Then
							index = i
							value = ___array(i+1)
							Exit For
						End If
					Next i

					' If so,  remove it
					If index <> -1 Then
						Dim tmp As Object() = New Object(___array.Length-2 - 1){}
						' Copy the list up to index
						Array.Copy(___array, 0, tmp, 0, index)
						' Copy from two past the index, up to
						' the end of tmp (which is two elements
						' shorter than the old list)
						If index < tmp.Length Then Array.Copy(___array, index+2, tmp, index, tmp.Length - index)
						' set the listener array to the new array or null
						table = If(tmp.Length = 0, Nothing, tmp)
					End If
				Else
					value = CType(table, Hashtable).Remove(key)
				End If
				If size()=ARRAY_BOUNDARY - 1 AndAlso (Not array) Then shrink()
			End If
			Return value
		End Function

		''' <summary>
		''' Removes all the mappings.
		''' </summary>
		Public Overridable Sub clear()
			table = Nothing
		End Sub

	'    
	'     * Returns a clone of the <code>ArrayTable</code>.
	'     
		Public Overridable Function clone() As Object
			Dim newArrayTable As New ArrayTable
			If array Then
				Dim ___array As Object() = CType(table, Object())
				For i As Integer = 0 To ___array.Length-2 Step 2
					newArrayTable.put(___array(i), ___array(i+1))
				Next i
			Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim tmp As Dictionary(Of ?, ?) = CType(table, Hashtable)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim ___keys As System.Collections.IEnumerator(Of ?) = tmp.Keys.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While ___keys.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim o As Object = ___keys.nextElement()
					newArrayTable.put(o,tmp(o))
				Loop
			End If
			Return newArrayTable
		End Function

		''' <summary>
		''' Returns the keys of the table, or <code>null</code> if there
		''' are currently no bindings. </summary>
		''' <param name="keys">  array of keys </param>
		''' <returns> an array of bindings </returns>
		Public Overridable Function getKeys(ByVal keys As Object()) As Object()
			If table Is Nothing Then Return Nothing
			If array Then
				Dim ___array As Object() = CType(table, Object())
				If keys Is Nothing Then keys = New Object(___array.Length \ 2 - 1){}
				Dim i As Integer = 0
				Dim index As Integer = 0
				Do While i < ___array.Length-1
					keys(index) = ___array(i)
					i+=2
					index += 1
				Loop
			Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim tmp As Dictionary(Of ?, ?) = CType(table, Hashtable)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim enum_ As System.Collections.IEnumerator(Of ?) = tmp.Keys.GetEnumerator()
				Dim counter As Integer = tmp.Count
				If keys Is Nothing Then keys = New Object(counter - 1){}
				Do While counter > 0
					counter -= 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					keys(counter) = enum_.nextElement()
				Loop
			End If
			Return keys
		End Function

	'    
	'     * Returns true if the current storage mechanism is
	'     * an array of alternating key-value pairs.
	'     
		Private Property array As Boolean
			Get
				Return (TypeOf table Is Object())
			End Get
		End Property

	'    
	'     * Grows the storage from an array to a hashtable.
	'     
		Private Sub grow()
			Dim ___array As Object() = CType(table, Object())
			Dim tmp As New Dictionary(Of Object, Object)(___array.Length\2)
			For i As Integer = 0 To ___array.Length - 1 Step 2
				tmp(___array(i)) = ___array(i+1)
			Next i
			table = tmp
		End Sub

	'    
	'     * Shrinks the storage from a hashtable to an array.
	'     
		Private Sub shrink()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tmp As Dictionary(Of ?, ?) = CType(table, Hashtable)
			Dim ___array As Object() = New Object(tmp.Count*2 - 1){}
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim ___keys As System.Collections.IEnumerator(Of ?) = tmp.Keys.GetEnumerator()
			Dim j As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While ___keys.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim o As Object = ___keys.nextElement()
				___array(j) = o
				___array(j+1) = tmp(o)
				j+=2
			Loop
			table = ___array
		End Sub
	End Class

End Namespace
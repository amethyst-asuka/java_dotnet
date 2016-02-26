Imports System.Collections
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

Namespace javax.accessibility


	''' <summary>
	''' Class AccessibleRelationSet determines a component's relation set.  The
	''' relation set of a component is a set of AccessibleRelation objects that
	''' describe the component's relationships with other components.
	''' </summary>
	''' <seealso cref= AccessibleRelation
	''' 
	''' @author      Lynn Monsanto
	''' @since 1.3 </seealso>
	Public Class AccessibleRelationSet

		''' <summary>
		''' Each entry in the Vector represents an AccessibleRelation. </summary>
		''' <seealso cref= #add </seealso>
		''' <seealso cref= #addAll </seealso>
		''' <seealso cref= #remove </seealso>
		''' <seealso cref= #contains </seealso>
		''' <seealso cref= #get </seealso>
		''' <seealso cref= #size </seealso>
		''' <seealso cref= #toArray </seealso>
		''' <seealso cref= #clear </seealso>
		Protected Friend relations As List(Of AccessibleRelation) = Nothing

		''' <summary>
		''' Creates a new empty relation set.
		''' </summary>
		Public Sub New()
			relations = Nothing
		End Sub

		''' <summary>
		''' Creates a new relation with the initial set of relations contained in
		''' the array of relations passed in.  Duplicate entries are ignored.
		''' </summary>
		''' <param name="relations"> an array of AccessibleRelation describing the
		''' relation set. </param>
		Public Sub New(ByVal relations As AccessibleRelation())
			If relations.Length <> 0 Then
				Me.relations = New ArrayList(relations.Length)
				For i As Integer = 0 To relations.Length - 1
					add(relations(i))
				Next i
			End If
		End Sub

		''' <summary>
		''' Adds a new relation to the current relation set.  If the relation
		''' is already in the relation set, the target(s) of the specified
		''' relation is merged with the target(s) of the existing relation.
		''' Otherwise,  the new relation is added to the relation set.
		''' </summary>
		''' <param name="relation"> the relation to add to the relation set </param>
		''' <returns> true if relation is added to the relation set; false if the
		''' relation set is unchanged </returns>
		Public Overridable Function add(ByVal relation As AccessibleRelation) As Boolean
			If relations Is Nothing Then relations = New ArrayList

			' Merge the relation targets if the key exists
			Dim existingRelation As AccessibleRelation = [get](relation.key)
			If existingRelation Is Nothing Then
				relations.Add(relation)
				Return True
			Else
				Dim existingTarget As Object() = existingRelation.target
				Dim newTarget As Object() = relation.target
				Dim mergedLength As Integer = existingTarget.Length + newTarget.Length
				Dim mergedTarget As Object() = New Object(mergedLength - 1){}
				For i As Integer = 0 To existingTarget.Length - 1
					mergedTarget(i) = existingTarget(i)
				Next i
				Dim i As Integer = existingTarget.Length
				Dim j As Integer = 0
				Do While i < mergedLength
					mergedTarget(i) = newTarget(j)
					i += 1
					j += 1
				Loop
				existingRelation.target = mergedTarget
			End If
			Return True
		End Function

		''' <summary>
		''' Adds all of the relations to the existing relation set.  Duplicate
		''' entries are ignored.
		''' </summary>
		''' <param name="relations">  AccessibleRelation array describing the relation set. </param>
		Public Overridable Sub addAll(ByVal relations As AccessibleRelation())
			If relations.Length <> 0 Then
				If Me.relations Is Nothing Then Me.relations = New ArrayList(relations.Length)
				For i As Integer = 0 To relations.Length - 1
					add(relations(i))
				Next i
			End If
		End Sub

		''' <summary>
		''' Removes a relation from the current relation set.  If the relation
		''' is not in the set, the relation set will be unchanged and the
		''' return value will be false.  If the relation is in the relation
		''' set, it will be removed from the set and the return value will be
		''' true.
		''' </summary>
		''' <param name="relation"> the relation to remove from the relation set </param>
		''' <returns> true if the relation is in the relation set; false if the
		''' relation set is unchanged </returns>
		Public Overridable Function remove(ByVal relation As AccessibleRelation) As Boolean
			If relations Is Nothing Then
				Return False
			Else
				Return relations.Remove(relation)
			End If
		End Function

		''' <summary>
		''' Removes all the relations from the current relation set.
		''' </summary>
		Public Overridable Sub clear()
			If relations IsNot Nothing Then relations.Clear()
		End Sub

		''' <summary>
		''' Returns the number of relations in the relation set. </summary>
		''' <returns> the number of relations in the relation set </returns>
		Public Overridable Function size() As Integer
			If relations Is Nothing Then
				Return 0
			Else
				Return relations.Count
			End If
		End Function

		''' <summary>
		''' Returns whether the relation set contains a relation
		''' that matches the specified key. </summary>
		''' <param name="key"> the AccessibleRelation key </param>
		''' <returns> true if the relation is in the relation set; otherwise false </returns>
		Public Overridable Function contains(ByVal key As String) As Boolean
			Return [get](key) IsNot Nothing
		End Function

		''' <summary>
		''' Returns the relation that matches the specified key. </summary>
		''' <param name="key"> the AccessibleRelation key </param>
		''' <returns> the relation, if one exists, that matches the specified key.
		''' Otherwise, null is returned. </returns>
		Public Overridable Function [get](ByVal key As String) As AccessibleRelation
			If relations Is Nothing Then
				Return Nothing
			Else
				Dim len As Integer = relations.Count
				For i As Integer = 0 To len - 1
					Dim relation As AccessibleRelation = CType(relations(i), AccessibleRelation)
					If relation IsNot Nothing AndAlso relation.key.Equals(key) Then Return relation
				Next i
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Returns the current relation set as an array of AccessibleRelation </summary>
		''' <returns> AccessibleRelation array contacting the current relation. </returns>
		Public Overridable Function toArray() As AccessibleRelation()
			If relations Is Nothing Then
				Return New AccessibleRelation(){}
			Else
				Dim relationArray As AccessibleRelation() = New AccessibleRelation(relations.Count - 1){}
				For i As Integer = 0 To relationArray.Length - 1
					relationArray(i) = CType(relations(i), AccessibleRelation)
				Next i
				Return relationArray
			End If
		End Function

		''' <summary>
		''' Creates a localized String representing all the relations in the set
		''' using the default locale.
		''' </summary>
		''' <returns> comma separated localized String </returns>
		''' <seealso cref= AccessibleBundle#toDisplayString </seealso>
		Public Overrides Function ToString() As String
			Dim ret As String = ""
			If (relations IsNot Nothing) AndAlso (relations.Count > 0) Then
				ret = CType(relations(0), AccessibleRelation).toDisplayString()
				For i As Integer = 1 To relations.Count - 1
					ret = ret & "," & CType(relations(i), AccessibleRelation).toDisplayString()
				Next i
			End If
			Return ret
		End Function
	End Class

End Namespace
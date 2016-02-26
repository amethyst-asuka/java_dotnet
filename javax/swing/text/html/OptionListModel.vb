Imports System
Imports System.Collections
Imports javax.swing
Imports javax.swing.event

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html



	''' <summary>
	''' This class extends DefaultListModel, and also implements
	''' the ListSelectionModel interface, allowing for it to store state
	''' relevant to a SELECT form element which is implemented as a List.
	''' If SELECT has a size attribute whose value is greater than 1,
	''' or if allows multiple selection then a JList is used to
	''' represent it and the OptionListModel is used as its model.
	''' It also stores the initial state of the JList, to ensure an
	''' accurate reset, if the user requests a reset of the form.
	''' 
	'''  @author Sunita Mani
	''' </summary>

	<Serializable> _
	Friend Class OptionListModel(Of E)
		Inherits DefaultListModel(Of E)
		Implements ListSelectionModel


		Private Const MIN As Integer = -1
		Private Shared ReadOnly MAX As Integer = Integer.MAX_VALUE
		Private selectionMode As Integer = SINGLE_SELECTION
		Private minIndex As Integer = MAX
		Private maxIndex As Integer = MIN
		Private anchorIndex As Integer = -1
		Private leadIndex As Integer = -1
		Private firstChangedIndex As Integer = MAX
		Private lastChangedIndex As Integer = MIN
		Private isAdjusting As Boolean = False
		Private value As New BitArray(32)
		Private initialValue As New BitArray(32)
		Protected Friend listenerList As New EventListenerList

		Protected Friend leadAnchorNotificationEnabled As Boolean = True

		Public Overridable Property minSelectionIndex As Integer Implements ListSelectionModel.getMinSelectionIndex
			Get
				Return If(selectionEmpty, -1, minIndex)
			End Get
		End Property

		Public Overridable Property maxSelectionIndex As Integer Implements ListSelectionModel.getMaxSelectionIndex
			Get
				Return maxIndex
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getValueIsAdjusting() As Boolean Implements ListSelectionModel.getValueIsAdjusting 'JavaToDotNetTempPropertyGetvalueIsAdjusting
		Public Overridable Property valueIsAdjusting As Boolean Implements ListSelectionModel.getValueIsAdjusting
			Get
				Return isAdjusting
			End Get
			Set(ByVal isAdjusting As Boolean)
		End Property

		Public Overridable Property selectionMode As Integer Implements ListSelectionModel.getSelectionMode
			Get
				Return selectionMode
			End Get
			Set(ByVal selectionMode As Integer)
				Select Case selectionMode
				Case SINGLE_SELECTION, SINGLE_INTERVAL_SELECTION, MULTIPLE_INTERVAL_SELECTION
					Me.selectionMode = selectionMode
				Case Else
					Throw New System.ArgumentException("invalid selectionMode")
				End Select
			End Set
		End Property


		Public Overridable Function isSelectedIndex(ByVal index As Integer) As Boolean Implements ListSelectionModel.isSelectedIndex
			Return If((index < minIndex) OrElse (index > maxIndex), False, value.Get(index))
		End Function

		Public Overridable Property selectionEmpty As Boolean Implements ListSelectionModel.isSelectionEmpty
			Get
				Return (minIndex > maxIndex)
			End Get
		End Property

		Public Overridable Sub addListSelectionListener(ByVal l As ListSelectionListener) Implements ListSelectionModel.addListSelectionListener
			listenerList.add(GetType(ListSelectionListener), l)
		End Sub

		Public Overridable Sub removeListSelectionListener(ByVal l As ListSelectionListener) Implements ListSelectionModel.removeListSelectionListener
			listenerList.remove(GetType(ListSelectionListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>ListSelectionListener</code>s added
		''' to this OptionListModel with addListSelectionListener().
		''' </summary>
		''' <returns> all of the <code>ListSelectionListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property listSelectionListeners As ListSelectionListener()
			Get
				Return listenerList.getListeners(GetType(ListSelectionListener))
			End Get
		End Property

		''' <summary>
		''' Notify listeners that we are beginning or ending a
		''' series of value changes
		''' </summary>
		Protected Friend Overridable Sub fireValueChanged(ByVal isAdjusting As Boolean)
			fireValueChanged(minSelectionIndex, maxSelectionIndex, isAdjusting)
		End Sub


		''' <summary>
		''' Notify ListSelectionListeners that the value of the selection,
		''' in the closed interval firstIndex,lastIndex, has changed.
		''' </summary>
		Protected Friend Overridable Sub fireValueChanged(ByVal firstIndex As Integer, ByVal lastIndex As Integer)
			fireValueChanged(firstIndex, lastIndex, valueIsAdjusting)
		End Sub

		''' <param name="firstIndex"> The first index in the interval. </param>
		''' <param name="lastIndex"> The last index in the interval. </param>
		''' <param name="isAdjusting"> True if this is the final change in a series of them. </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireValueChanged(ByVal firstIndex As Integer, ByVal lastIndex As Integer, ByVal isAdjusting As Boolean)
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As ListSelectionEvent = Nothing

			For i As Integer = ___listeners.Length - 2 To 0 Step -2
				If ___listeners(i) Is GetType(ListSelectionListener) Then
					If e Is Nothing Then e = New ListSelectionEvent(Me, firstIndex, lastIndex, isAdjusting)
					CType(___listeners(i+1), ListSelectionListener).valueChanged(e)
				End If
			Next i
		End Sub

		Private Sub fireValueChanged()
			If lastChangedIndex = MIN Then Return
	'         Change the values before sending the event to the
	'         * listeners in case the event causes a listener to make
	'         * another change to the selection.
	'         
			Dim oldFirstChangedIndex As Integer = firstChangedIndex
			Dim oldLastChangedIndex As Integer = lastChangedIndex
			firstChangedIndex = MAX
			lastChangedIndex = MIN
			fireValueChanged(oldFirstChangedIndex, oldLastChangedIndex)
		End Sub


		' Update first and last change indices
		Private Sub markAsDirty(ByVal r As Integer)
			firstChangedIndex = Math.Min(firstChangedIndex, r)
			lastChangedIndex = Math.Max(lastChangedIndex, r)
		End Sub

		' Set the state at this index and update all relevant state.
		Private Sub [set](ByVal r As Integer)
			If value.Get(r) Then Return
			value.Set(r, True)
			Dim [option] As [Option] = CType([get](r), [Option])
			[option].selection = True
			markAsDirty(r)

			' Update minimum and maximum indices
			minIndex = Math.Min(minIndex, r)
			maxIndex = Math.Max(maxIndex, r)
		End Sub

		' Clear the state at this index and update all relevant state.
		Private Sub clear(ByVal r As Integer)
			If Not value.Get(r) Then Return
			value.Set(r, False)
			Dim [option] As [Option] = CType([get](r), [Option])
			[option].selection = False
			markAsDirty(r)

			' Update minimum and maximum indices
	'        
	'           If (r > minIndex) the minimum has not changed.
	'           The case (r < minIndex) is not possible because r'th value was set.
	'           We only need to check for the case when lowest entry has been cleared,
	'           and in this case we need to search for the first value set above it.
	'        
			If r = minIndex Then
				For minIndex = minIndex + 1 To maxIndex
					If value.Get(minIndex) Then Exit For
				Next minIndex
			End If
	'        
	'           If (r < maxIndex) the maximum has not changed.
	'           The case (r > maxIndex) is not possible because r'th value was set.
	'           We only need to check for the case when highest entry has been cleared,
	'           and in this case we need to search for the first value set below it.
	'        
			If r = maxIndex Then
				maxIndex = maxIndex - 1
				Do While minIndex <= maxIndex
					If value.Get(maxIndex) Then Exit Do
					maxIndex -= 1
				Loop
			End If
	'         Performance note: This method is called from inside a loop in
	'           changeSelection() but we will only iterate in the loops
	'           above on the basis of one iteration per deselected cell - in total.
	'           Ie. the next time this method is called the work of the previous
	'           deselection will not be repeated.
	'
	'           We also don't need to worry about the case when the min and max
	'           values are in their unassigned states. This cannot happen because
	'           this method's initial check ensures that the selection was not empty
	'           and therefore that the minIndex and maxIndex had 'real' values.
	'
	'           If we have cleared the whole selection, set the minIndex and maxIndex
	'           to their cannonical values so that the next set command always works
	'           just by using Math.min and Math.max.
	'        
			If selectionEmpty Then
				minIndex = MAX
				maxIndex = MIN
			End If
		End Sub

		''' <summary>
		''' Sets the value of the leadAnchorNotificationEnabled flag. </summary>
		''' <seealso cref=             #isLeadAnchorNotificationEnabled() </seealso>
		Public Overridable Property leadAnchorNotificationEnabled As Boolean
			Set(ByVal flag As Boolean)
				leadAnchorNotificationEnabled = flag
			End Set
			Get
				Return leadAnchorNotificationEnabled
			End Get
		End Property


		Private Sub updateLeadAnchorIndices(ByVal anchorIndex As Integer, ByVal leadIndex As Integer)
			If leadAnchorNotificationEnabled Then
				If Me.anchorIndex <> anchorIndex Then
					If Me.anchorIndex <> -1 Then ' The unassigned state. markAsDirty(Me.anchorIndex)
					markAsDirty(anchorIndex)
				End If

				If Me.leadIndex <> leadIndex Then
					If Me.leadIndex <> -1 Then ' The unassigned state. markAsDirty(Me.leadIndex)
					markAsDirty(leadIndex)
				End If
			End If
			Me.anchorIndex = anchorIndex
			Me.leadIndex = leadIndex
		End Sub

		Private Function contains(ByVal a As Integer, ByVal b As Integer, ByVal i As Integer) As Boolean
			Return (i >= a) AndAlso (i <= b)
		End Function

		Private Sub changeSelection(ByVal clearMin As Integer, ByVal clearMax As Integer, ByVal setMin As Integer, ByVal setMax As Integer, ByVal clearFirst As Boolean)
			For i As Integer = Math.Min(minMin, clearMin) To Math.Max(setMax, clearMax)

				Dim shouldClear As Boolean = contains(clearMin, clearMax, i)
				Dim shouldSet As Boolean = contains(minMin, setMax, i)

				If shouldSet AndAlso shouldClear Then
					If clearFirst Then
						shouldClear = False
					Else
						shouldSet = False
					End If
				End If

				If shouldSet Then [set](i)
				If shouldClear Then clear(i)
			Next i
			fireValueChanged()
		End Sub

	'      Change the selection with the effect of first clearing the values
	'    *   in the inclusive range [clearMin, clearMax] then setting the values
	'    *   in the inclusive range [setMin, setMax]. Do this in one pass so
	'    *   that no values are cleared if they would later be set.
	'    
		Private Sub changeSelection(ByVal clearMin As Integer, ByVal clearMax As Integer, ByVal setMin As Integer, ByVal setMax As Integer)
			changeSelection(clearMin, clearMax, minMin, setMax, True)
		End Sub

		Public Overridable Sub clearSelection() Implements ListSelectionModel.clearSelection
			removeSelectionInterval(minIndex, maxIndex)
		End Sub

		Public Overridable Sub setSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer) Implements ListSelectionModel.setSelectionInterval
			If index0 = -1 OrElse index1 = -1 Then Return

			If selectionMode = SINGLE_SELECTION Then index0 = index1

			updateLeadAnchorIndices(index0, index1)

			Dim clearMin As Integer = minIndex
			Dim clearMax As Integer = maxIndex
			Dim minMin As Integer = Math.Min(index0, index1)
			Dim maxMax As Integer = Math.Max(index0, index1)
			changeSelection(clearMin, clearMax, minMin, setMax)
		End Sub

		Public Overridable Sub addSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer) Implements ListSelectionModel.addSelectionInterval
			If index0 = -1 OrElse index1 = -1 Then Return

			If selectionMode <> MULTIPLE_INTERVAL_SELECTION Then
				selectionIntervalval(index0, index1)
				Return
			End If

			updateLeadAnchorIndices(index0, index1)

			Dim clearMin As Integer = MAX
			Dim clearMax As Integer = MIN
			Dim minMin As Integer = Math.Min(index0, index1)
			Dim maxMax As Integer = Math.Max(index0, index1)
			changeSelection(clearMin, clearMax, minMin, setMax)
		End Sub


		Public Overridable Sub removeSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer) Implements ListSelectionModel.removeSelectionInterval
			If index0 = -1 OrElse index1 = -1 Then Return

			updateLeadAnchorIndices(index0, index1)

			Dim clearMin As Integer = Math.Min(index0, index1)
			Dim clearMax As Integer = Math.Max(index0, index1)
			Dim minMin As Integer = MAX
			Dim maxMax As Integer = MIN
			changeSelection(clearMin, clearMax, minMin, setMax)
		End Sub

		Private Sub setState(ByVal index As Integer, ByVal state As Boolean)
			If state Then
				[set](index)
			Else
				clear(index)
			End If
		End Sub

		''' <summary>
		''' Insert length indices beginning before/after index. If the value
		''' at index is itself selected, set all of the newly inserted
		''' items, otherwise leave them unselected. This method is typically
		''' called to sync the selection model with a corresponding change
		''' in the data model.
		''' </summary>
		Public Overridable Sub insertIndexInterval(ByVal index As Integer, ByVal length As Integer, ByVal before As Boolean) Implements ListSelectionModel.insertIndexInterval
	'         The first new index will appear at insMinIndex and the last
	'         * one will appear at insMaxIndex
	'         
			Dim insMinIndex As Integer = If(before, index, index + 1)
			Dim insMaxIndex As Integer = (insMinIndex + length) - 1

	'         Right shift the entire bitset by length, beginning with
	'         * index-1 if before is true, index+1 if it's false (i.e. with
	'         * insMinIndex).
	'         
			For i As Integer = maxIndex To insMinIndex Step -1
				stateate(i + length, value.Get(i))
			Next i

	'         Initialize the newly inserted indices.
	'         
			Dim insertedValuesues As Boolean = value.Get(index)
			For i As Integer = insMinIndex To insMaxIndex
				stateate(i, setInsertedValues)
			Next i
		End Sub


		''' <summary>
		''' Remove the indices in the interval index0,index1 (inclusive) from
		''' the selection model.  This is typically called to sync the selection
		''' model width a corresponding change in the data model.  Note
		''' that (as always) index0 can be greater than index1.
		''' </summary>
		Public Overridable Sub removeIndexInterval(ByVal index0 As Integer, ByVal index1 As Integer) Implements ListSelectionModel.removeIndexInterval
			Dim rmMinIndex As Integer = Math.Min(index0, index1)
			Dim rmMaxIndex As Integer = Math.Max(index0, index1)
			Dim gapLength As Integer = (rmMaxIndex - rmMinIndex) + 1

	'         Shift the entire bitset to the left to close the index0, index1
	'         * gap.
	'         
			For i As Integer = rmMinIndex To maxIndex
				stateate(i, value.Get(i + gapLength))
			Next i
		End Sub


			If isAdjusting <> Me.isAdjusting Then
				Me.isAdjusting = isAdjusting
				Me.fireValueChanged(isAdjusting)
			End If
		End Sub


		Public Overrides Function ToString() As String
			Dim s As String = (If(valueIsAdjusting, "~", "=")) + value.ToString()
			Return Me.GetType().name & " " & Convert.ToString(GetHashCode()) & " " & s
		End Function

		''' <summary>
		''' Returns a clone of the receiver with the same selection.
		''' <code>listenerLists</code> are not duplicated.
		''' </summary>
		''' <returns> a clone of the receiver </returns>
		''' <exception cref="CloneNotSupportedException"> if the receiver does not
		'''    both (a) implement the <code>Cloneable</code> interface
		'''    and (b) define a <code>clone</code> method </exception>
		Public Overridable Function clone() As Object
			Dim ___clone As OptionListModel = CType(MyBase.clone(), OptionListModel)
			___clone.value = CType(value.clone(), BitArray)
			___clone.listenerList = New EventListenerList
			Return ___clone
		End Function

		Public Overridable Property anchorSelectionIndex As Integer Implements ListSelectionModel.getAnchorSelectionIndex
			Get
				Return anchorIndex
			End Get
			Set(ByVal anchorIndex As Integer)
				Me.anchorIndex = anchorIndex
			End Set
		End Property

		Public Overridable Property leadSelectionIndex As Integer Implements ListSelectionModel.getLeadSelectionIndex
			Get
				Return leadIndex
			End Get
			Set(ByVal leadIndex As Integer)
				Dim anchorIndex As Integer = Me.anchorIndex
				If selectionMode = SINGLE_SELECTION Then anchorIndex = leadIndex
    
				Dim oldMin As Integer = Math.Min(Me.anchorIndex, Me.leadIndex)
				Dim oldMax As Integer = Math.Max(Me.anchorIndex, Me.leadIndex)
				Dim newMin As Integer = Math.Min(anchorIndex, leadIndex)
				Dim newMax As Integer = Math.Max(anchorIndex, leadIndex)
				If value.Get(Me.anchorIndex) Then
					changeSelection(oldMin, oldMax, newMin, newMax)
				Else
					changeSelection(newMin, newMax, oldMin, oldMax, False)
				End If
				Me.anchorIndex = anchorIndex
				Me.leadIndex = leadIndex
			End Set
		End Property




		''' <summary>
		''' This method is responsible for storing the state
		''' of the initial selection.  If the selectionMode
		''' is the default, i.e allowing only for SINGLE_SELECTION,
		''' then the very last OPTION that has the selected
		''' attribute set wins.
		''' </summary>
		Public Overridable Property initialSelection As Integer
			Set(ByVal i As Integer)
				If initialValue.Get(i) Then Return
				If selectionMode = SINGLE_SELECTION Then initialValue = initialValue.And(New BitArray)
				initialValue.Set(i, True)
			End Set
			Get
				Return initialValue
			End Get
		End Property

	End Class

End Namespace
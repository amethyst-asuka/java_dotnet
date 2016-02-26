Imports System
Imports System.Collections
Imports javax.swing.event

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Default data model for list selections.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Philip Milne
	''' @author Hans Muller </summary>
	''' <seealso cref= ListSelectionModel </seealso>

	<Serializable> _
	Public Class DefaultListSelectionModel
		Implements ListSelectionModel, ICloneable

		Private Const MIN As Integer = -1
		Private Shared ReadOnly MAX As Integer = Integer.MAX_VALUE
		Private selectionMode As Integer = MULTIPLE_INTERVAL_SELECTION
		Private minIndex As Integer = MAX
		Private maxIndex As Integer = MIN
		Private anchorIndex As Integer = -1
		Private leadIndex As Integer = -1
		Private firstAdjustedIndex As Integer = MAX
		Private lastAdjustedIndex As Integer = MIN
		Private isAdjusting As Boolean = False

		Private firstChangedIndex As Integer = MAX
		Private lastChangedIndex As Integer = MIN

		Private value As New BitArray(32)
		Protected Friend listenerList As New EventListenerList

		Protected Friend leadAnchorNotificationEnabled As Boolean = True

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Property minSelectionIndex As Integer Implements ListSelectionModel.getMinSelectionIndex
			Get
				Return If(selectionEmpty, -1, minIndex)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Property maxSelectionIndex As Integer Implements ListSelectionModel.getMaxSelectionIndex
			Get
				Return maxIndex
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc} </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getValueIsAdjusting() As Boolean Implements ListSelectionModel.getValueIsAdjusting 'JavaToDotNetTempPropertyGetvalueIsAdjusting
		Public Overridable Property valueIsAdjusting As Boolean Implements ListSelectionModel.getValueIsAdjusting
			Get
				Return isAdjusting
			End Get
			Set(ByVal isAdjusting As Boolean)
		End Property

		''' <summary>
		''' {@inheritDoc} </summary>
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


		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Function isSelectedIndex(ByVal index As Integer) As Boolean Implements ListSelectionModel.isSelectedIndex
			Return If((index < minIndex) OrElse (index > maxIndex), False, value.Get(index))
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Property selectionEmpty As Boolean Implements ListSelectionModel.isSelectionEmpty
			Get
				Return (minIndex > maxIndex)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Sub addListSelectionListener(ByVal l As ListSelectionListener) Implements ListSelectionModel.addListSelectionListener
			listenerList.add(GetType(ListSelectionListener), l)
		End Sub

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Sub removeListSelectionListener(ByVal l As ListSelectionListener) Implements ListSelectionModel.removeListSelectionListener
			listenerList.remove(GetType(ListSelectionListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the list selection listeners
		''' registered on this <code>DefaultListSelectionModel</code>.
		''' </summary>
		''' <returns> all of this model's <code>ListSelectionListener</code>s
		'''         or an empty
		'''         array if no list selection listeners are currently registered
		''' </returns>
		''' <seealso cref= #addListSelectionListener </seealso>
		''' <seealso cref= #removeListSelectionListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property listSelectionListeners As ListSelectionListener()
			Get
				Return listenerList.getListeners(GetType(ListSelectionListener))
			End Get
		End Property

		''' <summary>
		''' Notifies listeners that we have ended a series of adjustments.
		''' </summary>
		Protected Friend Overridable Sub fireValueChanged(ByVal isAdjusting As Boolean)
			If lastChangedIndex = MIN Then Return
	'         Change the values before sending the event to the
	'         * listeners in case the event causes a listener to make
	'         * another change to the selection.
	'         
			Dim oldFirstChangedIndex As Integer = firstChangedIndex
			Dim oldLastChangedIndex As Integer = lastChangedIndex
			firstChangedIndex = MAX
			lastChangedIndex = MIN
			fireValueChanged(oldFirstChangedIndex, oldLastChangedIndex, isAdjusting)
		End Sub


		''' <summary>
		''' Notifies <code>ListSelectionListeners</code> that the value
		''' of the selection, in the closed interval <code>firstIndex</code>,
		''' <code>lastIndex</code>, has changed.
		''' </summary>
		Protected Friend Overridable Sub fireValueChanged(ByVal firstIndex As Integer, ByVal lastIndex As Integer)
			fireValueChanged(firstIndex, lastIndex, valueIsAdjusting)
		End Sub

		''' <param name="firstIndex"> the first index in the interval </param>
		''' <param name="lastIndex"> the last index in the interval </param>
		''' <param name="isAdjusting"> true if this is the final change in a series of
		'''          adjustments </param>
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
			If lastAdjustedIndex = MIN Then Return
	'         If getValueAdjusting() is true, (eg. during a drag opereration)
	'         * record the bounds of the changes so that, when the drag finishes (and
	'         * setValueAdjusting(false) is called) we can post a single event
	'         * with bounds covering all of these individual adjustments.
	'         
			If valueIsAdjusting Then
				firstChangedIndex = Math.Min(firstChangedIndex, firstAdjustedIndex)
				lastChangedIndex = Math.Max(lastChangedIndex, lastAdjustedIndex)
			End If
	'         Change the values before sending the event to the
	'         * listeners in case the event causes a listener to make
	'         * another change to the selection.
	'         
			Dim oldFirstAdjustedIndex As Integer = firstAdjustedIndex
			Dim oldLastAdjustedIndex As Integer = lastAdjustedIndex
			firstAdjustedIndex = MAX
			lastAdjustedIndex = MIN

			fireValueChanged(oldFirstAdjustedIndex, oldLastAdjustedIndex)
		End Sub

		''' <summary>
		''' Returns an array of all the objects currently registered as
		''' <code><em>Foo</em>Listener</code>s
		''' upon this model.
		''' <code><em>Foo</em>Listener</code>s
		''' are registered using the <code>add<em>Foo</em>Listener</code> method.
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a <code>DefaultListSelectionModel</code>
		''' instance <code>m</code>
		''' for its list selection listeners
		''' with the following code:
		''' 
		''' <pre>ListSelectionListener[] lsls = (ListSelectionListener[])(m.getListeners(ListSelectionListener.class));</pre>
		''' 
		''' If no such listeners exist,
		''' this method returns an empty array.
		''' </summary>
		''' <param name="listenerType">  the type of listeners requested;
		'''          this parameter should specify an interface
		'''          that descends from <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s
		'''          on this model,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code> doesn't
		'''          specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getListSelectionListeners
		''' 
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function

		' Updates first and last change indices
		Private Sub markAsDirty(ByVal r As Integer)
			If r = -1 Then Return

			firstAdjustedIndex = Math.Min(firstAdjustedIndex, r)
			lastAdjustedIndex = Math.Max(lastAdjustedIndex, r)
		End Sub

		' Sets the state at this index and update all relevant state.
		Private Sub [set](ByVal r As Integer)
			If value.Get(r) Then Return
			value.Set(r, True)
			markAsDirty(r)

			' Update minimum and maximum indices
			minIndex = Math.Min(minIndex, r)
			maxIndex = Math.Max(maxIndex, r)
		End Sub

		' Clears the state at this index and update all relevant state.
		Private Sub clear(ByVal r As Integer)
			If Not value.Get(r) Then Return
			value.Set(r, False)
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
					markAsDirty(Me.anchorIndex)
					markAsDirty(anchorIndex)
				End If

				If Me.leadIndex <> leadIndex Then
					markAsDirty(Me.leadIndex)
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

	   ''' <summary>
	   ''' Change the selection with the effect of first clearing the values
	   ''' in the inclusive range [clearMin, clearMax] then setting the values
	   ''' in the inclusive range [setMin, setMax]. Do this in one pass so
	   ''' that no values are cleared if they would later be set.
	   ''' </summary>
		Private Sub changeSelection(ByVal clearMin As Integer, ByVal clearMax As Integer, ByVal setMin As Integer, ByVal setMax As Integer)
			changeSelection(clearMin, clearMax, minMin, setMax, True)
		End Sub

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Sub clearSelection() Implements ListSelectionModel.clearSelection
			removeSelectionIntervalImpl(minIndex, maxIndex, False)
		End Sub

		''' <summary>
		''' Changes the selection to be between {@code index0} and {@code index1}
		''' inclusive. {@code index0} doesn't have to be less than or equal to
		''' {@code index1}.
		''' <p>
		''' In {@code SINGLE_SELECTION} selection mode, only the second index
		''' is used.
		''' <p>
		''' If this represents a change to the current selection, then each
		''' {@code ListSelectionListener} is notified of the change.
		''' <p>
		''' If either index is {@code -1}, this method does nothing and returns
		''' without exception. Otherwise, if either index is less than {@code -1},
		''' an {@code IndexOutOfBoundsException} is thrown.
		''' </summary>
		''' <param name="index0"> one end of the interval. </param>
		''' <param name="index1"> other end of the interval </param>
		''' <exception cref="IndexOutOfBoundsException"> if either index is less than {@code -1}
		'''         (and neither index is {@code -1}) </exception>
		''' <seealso cref= #addListSelectionListener </seealso>
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

		''' <summary>
		''' Changes the selection to be the set union of the current selection
		''' and the indices between {@code index0} and {@code index1} inclusive.
		''' <p>
		''' In {@code SINGLE_SELECTION} selection mode, this is equivalent
		''' to calling {@code setSelectionInterval}, and only the second index
		''' is used. In {@code SINGLE_INTERVAL_SELECTION} selection mode, this
		''' method behaves like {@code setSelectionInterval}, unless the given
		''' interval is immediately adjacent to or overlaps the existing selection,
		''' and can therefore be used to grow it.
		''' <p>
		''' If this represents a change to the current selection, then each
		''' {@code ListSelectionListener} is notified of the change. Note that
		''' {@code index0} doesn't have to be less than or equal to {@code index1}.
		''' <p>
		''' If either index is {@code -1}, this method does nothing and returns
		''' without exception. Otherwise, if either index is less than {@code -1},
		''' an {@code IndexOutOfBoundsException} is thrown.
		''' </summary>
		''' <param name="index0"> one end of the interval. </param>
		''' <param name="index1"> other end of the interval </param>
		''' <exception cref="IndexOutOfBoundsException"> if either index is less than {@code -1}
		'''         (and neither index is {@code -1}) </exception>
		''' <seealso cref= #addListSelectionListener </seealso>
		''' <seealso cref= #setSelectionInterval </seealso>
		Public Overridable Sub addSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer) Implements ListSelectionModel.addSelectionInterval
			If index0 = -1 OrElse index1 = -1 Then Return

			' If we only allow a single selection, channel through
			' setSelectionInterval() to enforce the rule.
			If selectionMode = SINGLE_SELECTION Then
				selectionIntervalval(index0, index1)
				Return
			End If

			updateLeadAnchorIndices(index0, index1)

			Dim clearMin As Integer = MAX
			Dim clearMax As Integer = MIN
			Dim minMin As Integer = Math.Min(index0, index1)
			Dim maxMax As Integer = Math.Max(index0, index1)

			' If we only allow a single interval and this would result
			' in multiple intervals, then set the selection to be just
			' the new range.
			If selectionMode = SINGLE_INTERVAL_SELECTION AndAlso (maxMax < minIndex - 1 OrElse setMin > maxIndex + 1) Then

				selectionIntervalval(index0, index1)
				Return
			End If

			changeSelection(clearMin, clearMax, minMin, setMax)
		End Sub


		''' <summary>
		''' Changes the selection to be the set difference of the current selection
		''' and the indices between {@code index0} and {@code index1} inclusive.
		''' {@code index0} doesn't have to be less than or equal to {@code index1}.
		''' <p>
		''' In {@code SINGLE_INTERVAL_SELECTION} selection mode, if the removal
		''' would produce two disjoint selections, the removal is extended through
		''' the greater end of the selection. For example, if the selection is
		''' {@code 0-10} and you supply indices {@code 5,6} (in any order) the
		''' resulting selection is {@code 0-4}.
		''' <p>
		''' If this represents a change to the current selection, then each
		''' {@code ListSelectionListener} is notified of the change.
		''' <p>
		''' If either index is {@code -1}, this method does nothing and returns
		''' without exception. Otherwise, if either index is less than {@code -1},
		''' an {@code IndexOutOfBoundsException} is thrown.
		''' </summary>
		''' <param name="index0"> one end of the interval </param>
		''' <param name="index1"> other end of the interval </param>
		''' <exception cref="IndexOutOfBoundsException"> if either index is less than {@code -1}
		'''         (and neither index is {@code -1}) </exception>
		''' <seealso cref= #addListSelectionListener </seealso>
		Public Overridable Sub removeSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer) Implements ListSelectionModel.removeSelectionInterval
			removeSelectionIntervalImpl(index0, index1, True)
		End Sub

		' private implementation allowing the selection interval
		' to be removed without affecting the lead and anchor
		Private Sub removeSelectionIntervalImpl(ByVal index0 As Integer, ByVal index1 As Integer, ByVal changeLeadAnchor As Boolean)

			If index0 = -1 OrElse index1 = -1 Then Return

			If changeLeadAnchor Then updateLeadAnchorIndices(index0, index1)

			Dim clearMin As Integer = Math.Min(index0, index1)
			Dim clearMax As Integer = Math.Max(index0, index1)
			Dim minMin As Integer = MAX
			Dim maxMax As Integer = MIN

			' If the removal would produce to two disjoint selections in a mode
			' that only allows one, extend the removal to the end of the selection.
			If selectionMode <> MULTIPLE_INTERVAL_SELECTION AndAlso clearMin > minIndex AndAlso clearMax < maxIndex Then clearMax = maxIndex

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
		''' at index is itself selected and the selection mode is not
		''' SINGLE_SELECTION, set all of the newly inserted items as selected.
		''' Otherwise leave them unselected. This method is typically
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
			Dim insertedValuesues As Boolean = (If(selectionMode = SINGLE_SELECTION, False, value.Get(index)))
			For i As Integer = insMinIndex To insMaxIndex
				stateate(i, setInsertedValues)
			Next i

			Dim leadIndex As Integer = Me.leadIndex
			If leadIndex > index OrElse (before AndAlso leadIndex = index) Then leadIndex = Me.leadIndex + length
			Dim anchorIndex As Integer = Me.anchorIndex
			If anchorIndex > index OrElse (before AndAlso anchorIndex = index) Then anchorIndex = Me.anchorIndex + length
			If leadIndex <> Me.leadIndex OrElse anchorIndex <> Me.anchorIndex Then updateLeadAnchorIndices(anchorIndex, leadIndex)

			fireValueChanged()
		End Sub


		''' <summary>
		''' Remove the indices in the interval index0,index1 (inclusive) from
		''' the selection model.  This is typically called to sync the selection
		''' model width a corresponding change in the data model.  Note
		''' that (as always) index0 need not be &lt;= index1.
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

			Dim leadIndex As Integer = Me.leadIndex
			If leadIndex = 0 AndAlso rmMinIndex = 0 Then
				' do nothing
			ElseIf leadIndex > rmMaxIndex Then
				leadIndex = Me.leadIndex - gapLength
			ElseIf leadIndex >= rmMinIndex Then
				leadIndex = rmMinIndex - 1
			End If

			Dim anchorIndex As Integer = Me.anchorIndex
			If anchorIndex = 0 AndAlso rmMinIndex = 0 Then
				' do nothing
			ElseIf anchorIndex > rmMaxIndex Then
				anchorIndex = Me.anchorIndex - gapLength
			ElseIf anchorIndex >= rmMinIndex Then
				anchorIndex = rmMinIndex - 1
			End If

			If leadIndex <> Me.leadIndex OrElse anchorIndex <> Me.anchorIndex Then updateLeadAnchorIndices(anchorIndex, leadIndex)

			fireValueChanged()
		End Sub


			If isAdjusting <> Me.isAdjusting Then
				Me.isAdjusting = isAdjusting
				Me.fireValueChanged(isAdjusting)
			End If
		End Sub


		''' <summary>
		''' Returns a string that displays and identifies this
		''' object's properties.
		''' </summary>
		''' <returns> a <code>String</code> representation of this object </returns>
		Public Overrides Function ToString() As String
			Dim s As String = (If(valueIsAdjusting, "~", "=")) + value.ToString()
			Return Me.GetType().name & " " & Convert.ToString(GetHashCode()) & " " & s
		End Function

		''' <summary>
		''' Returns a clone of this selection model with the same selection.
		''' <code>listenerLists</code> are not duplicated.
		''' </summary>
		''' <exception cref="CloneNotSupportedException"> if the selection model does not
		'''    both (a) implement the Cloneable interface and (b) define a
		'''    <code>clone</code> method. </exception>
		Public Overridable Function clone() As Object
			Dim ___clone As DefaultListSelectionModel = CType(MyBase.clone(), DefaultListSelectionModel)
			___clone.value = CType(value.clone(), BitArray)
			___clone.listenerList = New EventListenerList
			Return ___clone
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property anchorSelectionIndex As Integer Implements ListSelectionModel.getAnchorSelectionIndex
			Get
				Return anchorIndex
			End Get
			Set(ByVal anchorIndex As Integer)
				updateLeadAnchorIndices(anchorIndex, Me.leadIndex)
				fireValueChanged()
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc} </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getLeadSelectionIndex() As Integer Implements ListSelectionModel.getLeadSelectionIndex 'JavaToDotNetTempPropertyGetleadSelectionIndex
		Public Overridable Property leadSelectionIndex As Integer Implements ListSelectionModel.getLeadSelectionIndex
			Get
				Return leadIndex
			End Get
			Set(ByVal leadIndex As Integer)
		End Property


		''' <summary>
		''' Set the lead selection index, leaving all selection values unchanged.
		''' If leadAnchorNotificationEnabled is true, send a notification covering
		''' the old and new lead cells.
		''' </summary>
		''' <param name="leadIndex"> the new lead selection index
		''' </param>
		''' <seealso cref= #setAnchorSelectionIndex </seealso>
		''' <seealso cref= #setLeadSelectionIndex </seealso>
		''' <seealso cref= #getLeadSelectionIndex
		''' 
		''' @since 1.5 </seealso>
		Public Overridable Sub moveLeadSelectionIndex(ByVal leadIndex As Integer)
			' disallow a -1 lead unless the anchor is already -1
			If leadIndex = -1 Then
				If Me.anchorIndex <> -1 Then Return

	' PENDING(shannonh) - The following check is nice, to be consistent with
	'                       setLeadSelectionIndex. However, it is not absolutely
	'                       necessary: One could work around it by setting the anchor
	'                       to something valid, modifying the lead, and then moving
	'                       the anchor back to -1. For this reason, there's no sense
	'                       in adding it at this time, as that would require
	'                       updating the spec and officially committing to it.
	'
	'        // otherwise, don't do anything if the anchor is -1
	'        } else if (this.anchorIndex == -1) {
	'            return;
	'

			End If

			updateLeadAnchorIndices(Me.anchorIndex, leadIndex)
			fireValueChanged()
		End Sub

			Dim anchorIndex As Integer = Me.anchorIndex

			' only allow a -1 lead if the anchor is already -1
			If leadIndex = -1 Then
				If anchorIndex = -1 Then
					updateLeadAnchorIndices(anchorIndex, leadIndex)
					fireValueChanged()
				End If

				Return
			' otherwise, don't do anything if the anchor is -1
			ElseIf anchorIndex = -1 Then
				Return
			End If

			If Me.leadIndex = -1 Then Me.leadIndex = leadIndex

			Dim shouldSelect As Boolean = value.Get(Me.anchorIndex)

			If selectionMode = SINGLE_SELECTION Then
				anchorIndex = leadIndex
				shouldSelect = True
			End If

			Dim oldMin As Integer = Math.Min(Me.anchorIndex, Me.leadIndex)
			Dim oldMax As Integer = Math.Max(Me.anchorIndex, Me.leadIndex)
			Dim newMin As Integer = Math.Min(anchorIndex, leadIndex)
			Dim newMax As Integer = Math.Max(anchorIndex, leadIndex)

			updateLeadAnchorIndices(anchorIndex, leadIndex)

			If shouldSelect Then
				changeSelection(oldMin, oldMax, newMin, newMax)
			Else
				changeSelection(newMin, newMax, oldMin, oldMax, False)
			End If
		End Sub
	End Class

End Namespace
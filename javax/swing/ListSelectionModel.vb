Imports javax.swing.event

'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' This interface represents the current state of the
	''' selection for any of the components that display a
	''' list of values with stable indices.  The selection is
	''' modeled as a set of intervals, each interval represents
	''' a contiguous range of selected list elements.
	''' The methods for modifying the set of selected intervals
	''' all take a pair of indices, index0 and index1, that represent
	''' a closed interval, i.e. the interval includes both index0 and
	''' index1.
	''' 
	''' @author Hans Muller
	''' @author Philip Milne </summary>
	''' <seealso cref= DefaultListSelectionModel </seealso>

	Public Interface ListSelectionModel
		''' <summary>
		''' A value for the selectionMode property: select one list index
		''' at a time.
		''' </summary>
		''' <seealso cref= #setSelectionMode </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int SINGLE_SELECTION = 0;

		''' <summary>
		''' A value for the selectionMode property: select one contiguous
		''' range of indices at a time.
		''' </summary>
		''' <seealso cref= #setSelectionMode </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int SINGLE_INTERVAL_SELECTION = 1;

		''' <summary>
		''' A value for the selectionMode property: select one or more
		''' contiguous ranges of indices at a time.
		''' </summary>
		''' <seealso cref= #setSelectionMode </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int MULTIPLE_INTERVAL_SELECTION = 2;


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
		''' </summary>
		''' <param name="index0"> one end of the interval. </param>
		''' <param name="index1"> other end of the interval </param>
		''' <seealso cref= #addListSelectionListener </seealso>
		Sub setSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)


		''' <summary>
		''' Changes the selection to be the set union of the current selection
		''' and the indices between {@code index0} and {@code index1} inclusive.
		''' {@code index0} doesn't have to be less than or equal to {@code index1}.
		''' <p>
		''' In {@code SINGLE_SELECTION} selection mode, this is equivalent
		''' to calling {@code setSelectionInterval}, and only the second index
		''' is used. In {@code SINGLE_INTERVAL_SELECTION} selection mode, this
		''' method behaves like {@code setSelectionInterval}, unless the given
		''' interval is immediately adjacent to or overlaps the existing selection,
		''' and can therefore be used to grow the selection.
		''' <p>
		''' If this represents a change to the current selection, then each
		''' {@code ListSelectionListener} is notified of the change.
		''' </summary>
		''' <param name="index0"> one end of the interval. </param>
		''' <param name="index1"> other end of the interval </param>
		''' <seealso cref= #addListSelectionListener </seealso>
		''' <seealso cref= #setSelectionInterval </seealso>
		Sub addSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)


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
		''' </summary>
		''' <param name="index0"> one end of the interval. </param>
		''' <param name="index1"> other end of the interval </param>
		''' <seealso cref= #addListSelectionListener </seealso>
		Sub removeSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)


		''' <summary>
		''' Returns the first selected index or -1 if the selection is empty.
		''' </summary>
		ReadOnly Property minSelectionIndex As Integer


		''' <summary>
		''' Returns the last selected index or -1 if the selection is empty.
		''' </summary>
		ReadOnly Property maxSelectionIndex As Integer


		''' <summary>
		''' Returns true if the specified index is selected.
		''' </summary>
		Function isSelectedIndex(ByVal index As Integer) As Boolean


		''' <summary>
		''' Return the first index argument from the most recent call to
		''' setSelectionInterval(), addSelectionInterval() or removeSelectionInterval().
		''' The most recent index0 is considered the "anchor" and the most recent
		''' index1 is considered the "lead".  Some interfaces display these
		''' indices specially, e.g. Windows95 displays the lead index with a
		''' dotted yellow outline.
		''' </summary>
		''' <seealso cref= #getLeadSelectionIndex </seealso>
		''' <seealso cref= #setSelectionInterval </seealso>
		''' <seealso cref= #addSelectionInterval </seealso>
		Property anchorSelectionIndex As Integer




		''' <summary>
		''' Return the second index argument from the most recent call to
		''' setSelectionInterval(), addSelectionInterval() or removeSelectionInterval().
		''' </summary>
		''' <seealso cref= #getAnchorSelectionIndex </seealso>
		''' <seealso cref= #setSelectionInterval </seealso>
		''' <seealso cref= #addSelectionInterval </seealso>
		Property leadSelectionIndex As Integer


		''' <summary>
		''' Change the selection to the empty set.  If this represents
		''' a change to the current selection then notify each ListSelectionListener.
		''' </summary>
		''' <seealso cref= #addListSelectionListener </seealso>
		Sub clearSelection()

		''' <summary>
		''' Returns true if no indices are selected.
		''' </summary>
		ReadOnly Property selectionEmpty As Boolean

		''' <summary>
		''' Insert length indices beginning before/after index.  This is typically
		''' called to sync the selection model with a corresponding change
		''' in the data model.
		''' </summary>
		Sub insertIndexInterval(ByVal index As Integer, ByVal length As Integer, ByVal before As Boolean)

		''' <summary>
		''' Remove the indices in the interval index0,index1 (inclusive) from
		''' the selection model.  This is typically called to sync the selection
		''' model width a corresponding change in the data model.
		''' </summary>
		Sub removeIndexInterval(ByVal index0 As Integer, ByVal index1 As Integer)

		''' <summary>
		''' Sets the {@code valueIsAdjusting} property, which indicates whether
		''' or not upcoming selection changes should be considered part of a single
		''' change. The value of this property is used to initialize the
		''' {@code valueIsAdjusting} property of the {@code ListSelectionEvent}s that
		''' are generated.
		''' <p>
		''' For example, if the selection is being updated in response to a user
		''' drag, this property can be set to {@code true} when the drag is initiated
		''' and set to {@code false} when the drag is finished. During the drag,
		''' listeners receive events with a {@code valueIsAdjusting} property
		''' set to {@code true}. At the end of the drag, when the change is
		''' finalized, listeners receive an event with the value set to {@code false}.
		''' Listeners can use this pattern if they wish to update only when a change
		''' has been finalized.
		''' <p>
		''' Setting this property to {@code true} begins a series of changes that
		''' is to be considered part of a single change. When the property is changed
		''' back to {@code false}, an event is sent out characterizing the entire
		''' selection change (if there was one), with the event's
		''' {@code valueIsAdjusting} property set to {@code false}.
		''' </summary>
		''' <param name="valueIsAdjusting"> the new value of the property </param>
		''' <seealso cref= #getValueIsAdjusting </seealso>
		''' <seealso cref= javax.swing.event.ListSelectionEvent#getValueIsAdjusting </seealso>
		Property valueIsAdjusting As Boolean


		''' <summary>
		''' Sets the selection mode. The following list describes the accepted
		''' selection modes:
		''' <ul>
		''' <li>{@code ListSelectionModel.SINGLE_SELECTION} -
		'''   Only one list index can be selected at a time. In this mode,
		'''   {@code setSelectionInterval} and {@code addSelectionInterval} are
		'''   equivalent, both replacing the current selection with the index
		'''   represented by the second argument (the "lead").
		''' <li>{@code ListSelectionModel.SINGLE_INTERVAL_SELECTION} -
		'''   Only one contiguous interval can be selected at a time.
		'''   In this mode, {@code addSelectionInterval} behaves like
		'''   {@code setSelectionInterval} (replacing the current selection),
		'''   unless the given interval is immediately adjacent to or overlaps
		'''   the existing selection, and can therefore be used to grow it.
		''' <li>{@code ListSelectionModel.MULTIPLE_INTERVAL_SELECTION} -
		'''   In this mode, there's no restriction on what can be selected.
		''' </ul>
		''' </summary>
		''' <seealso cref= #getSelectionMode </seealso>
		''' <exception cref="IllegalArgumentException"> if the selection mode isn't
		'''         one of those allowed </exception>
		Property selectionMode As Integer


		''' <summary>
		''' Add a listener to the list that's notified each time a change
		''' to the selection occurs.
		''' </summary>
		''' <param name="x"> the ListSelectionListener </param>
		''' <seealso cref= #removeListSelectionListener </seealso>
		''' <seealso cref= #setSelectionInterval </seealso>
		''' <seealso cref= #addSelectionInterval </seealso>
		''' <seealso cref= #removeSelectionInterval </seealso>
		''' <seealso cref= #clearSelection </seealso>
		''' <seealso cref= #insertIndexInterval </seealso>
		''' <seealso cref= #removeIndexInterval </seealso>
		Sub addListSelectionListener(ByVal x As ListSelectionListener)

		''' <summary>
		''' Remove a listener from the list that's notified each time a
		''' change to the selection occurs.
		''' </summary>
		''' <param name="x"> the ListSelectionListener </param>
		''' <seealso cref= #addListSelectionListener </seealso>
		Sub removeListSelectionListener(ByVal x As ListSelectionListener)
	End Interface

End Namespace
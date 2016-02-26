Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
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

Namespace javax.swing.tree


	''' <summary>
	''' Default implementation of TreeSelectionModel.  Listeners are notified
	''' whenever
	''' the paths in the selection change, not the rows. In order
	''' to be able to track row changes you may wish to become a listener
	''' for expansion events on the tree and test for changes from there.
	''' <p>resetRowSelection is called from any of the methods that update
	''' the selected paths. If you subclass any of these methods to
	''' filter what is allowed to be selected, be sure and message
	''' <code>resetRowSelection</code> if you do not message super.
	''' 
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= javax.swing.JTree
	''' 
	''' @author Scott Violet </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class DefaultTreeSelectionModel
		Implements ICloneable, TreeSelectionModel

		''' <summary>
		''' Property name for selectionMode. </summary>
		Public Const SELECTION_MODE_PROPERTY As String = "selectionMode"

		''' <summary>
		''' Used to messaged registered listeners. </summary>
		Protected Friend changeSupport As SwingPropertyChangeSupport

		''' <summary>
		''' Paths that are currently selected.  Will be null if nothing is
		''' currently selected. 
		''' </summary>
		Protected Friend selection As TreePath()

		''' <summary>
		''' Event listener list. </summary>
		Protected Friend listenerList As New EventListenerList

		''' <summary>
		''' Provides a row for a given path. </summary>
		<NonSerialized> _
		Protected Friend rowMapper As RowMapper

		''' <summary>
		''' Handles maintaining the list selection model. The RowMapper is used
		''' to map from a TreePath to a row, and the value is then placed here. 
		''' </summary>
		Protected Friend listSelectionModel As javax.swing.DefaultListSelectionModel

		''' <summary>
		''' Mode for the selection, will be either SINGLE_TREE_SELECTION,
		''' CONTIGUOUS_TREE_SELECTION or DISCONTIGUOUS_TREE_SELECTION.
		''' </summary>
		Protected Friend selectionMode As Integer

		''' <summary>
		''' Last path that was added. </summary>
		Protected Friend leadPath As TreePath
		''' <summary>
		''' Index of the lead path in selection. </summary>
		Protected Friend leadIndex As Integer
		''' <summary>
		''' Lead row. </summary>
		Protected Friend leadRow As Integer

		''' <summary>
		''' Used to make sure the paths are unique, will contain all the paths
		''' in <code>selection</code>.
		''' </summary>
		Private uniquePaths As Dictionary(Of TreePath, Boolean?)
		Private lastPaths As Dictionary(Of TreePath, Boolean?)
		Private tempPaths As TreePath()


		''' <summary>
		''' Creates a new instance of DefaultTreeSelectionModel that is
		''' empty, with a selection mode of DISCONTIGUOUS_TREE_SELECTION.
		''' </summary>
		Public Sub New()
			listSelectionModel = New javax.swing.DefaultListSelectionModel
			selectionMode = DISCONTIGUOUS_TREE_SELECTION
				leadRow = -1
				leadIndex = leadRow
			uniquePaths = New Dictionary(Of TreePath, Boolean?)
			lastPaths = New Dictionary(Of TreePath, Boolean?)
			tempPaths = New TreePath(0){}
		End Sub

		''' <summary>
		''' Sets the RowMapper instance. This instance is used to determine
		''' the row for a particular TreePath.
		''' </summary>
		Public Overridable Property rowMapper Implements TreeSelectionModel.setRowMapper As RowMapper
			Set(ByVal newMapper As RowMapper)
				rowMapper = newMapper
				resetRowSelection()
			End Set
			Get
				Return rowMapper
			End Get
		End Property


		''' <summary>
		''' Sets the selection model, which must be one of SINGLE_TREE_SELECTION,
		''' CONTIGUOUS_TREE_SELECTION or DISCONTIGUOUS_TREE_SELECTION. If mode
		''' is not one of the defined value,
		''' <code>DISCONTIGUOUS_TREE_SELECTION</code> is assumed.
		''' <p>This may change the selection if the current selection is not valid
		''' for the new mode. For example, if three TreePaths are
		''' selected when the mode is changed to <code>SINGLE_TREE_SELECTION</code>,
		''' only one TreePath will remain selected. It is up to the particular
		''' implementation to decide what TreePath remains selected.
		''' <p>
		''' Setting the mode to something other than the defined types will
		''' result in the mode becoming <code>DISCONTIGUOUS_TREE_SELECTION</code>.
		''' </summary>
		Public Overridable Property selectionMode Implements TreeSelectionModel.setSelectionMode As Integer
			Set(ByVal mode As Integer)
				Dim oldMode As Integer = selectionMode
    
				selectionMode = mode
				If selectionMode <> TreeSelectionModel.SINGLE_TREE_SELECTION AndAlso selectionMode <> TreeSelectionModel.CONTIGUOUS_TREE_SELECTION AndAlso selectionMode <> TreeSelectionModel.DISCONTIGUOUS_TREE_SELECTION Then selectionMode = TreeSelectionModel.DISCONTIGUOUS_TREE_SELECTION
				If oldMode <> selectionMode AndAlso changeSupport IsNot Nothing Then changeSupport.firePropertyChange(SELECTION_MODE_PROPERTY, Convert.ToInt32(oldMode), Convert.ToInt32(selectionMode))
			End Set
			Get
				Return selectionMode
			End Get
		End Property


		''' <summary>
		''' Sets the selection to path. If this represents a change, then
		''' the TreeSelectionListeners are notified. If <code>path</code> is
		''' null, this has the same effect as invoking <code>clearSelection</code>.
		''' </summary>
		''' <param name="path"> new path to select </param>
		Public Overridable Property selectionPath Implements TreeSelectionModel.setSelectionPath As TreePath
			Set(ByVal path As TreePath)
				If path Is Nothing Then
					selectionPaths = Nothing
				Else
					Dim newPaths As TreePath() = New TreePath(0){}
    
					newPaths(0) = path
					selectionPaths = newPaths
				End If
			End Set
			Get
				If selection IsNot Nothing AndAlso selection.Length > 0 Then Return selection(0)
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Sets the selection. Whether the supplied paths are taken as the
		''' new selection depends upon the selection mode. If the supplied
		''' array is {@code null}, or empty, the selection is cleared. If
		''' the selection mode is {@code SINGLE_TREE_SELECTION}, only the
		''' first path in {@code pPaths} is used. If the selection
		''' mode is {@code CONTIGUOUS_TREE_SELECTION} and the supplied paths
		''' are not contiguous, then only the first path in {@code pPaths} is
		''' used. If the selection mode is
		''' {@code DISCONTIGUOUS_TREE_SELECTION}, then all paths are used.
		''' <p>
		''' All {@code null} paths in {@code pPaths} are ignored.
		''' <p>
		''' If this represents a change, all registered {@code
		''' TreeSelectionListener}s are notified.
		''' <p>
		''' The lead path is set to the last unique path.
		''' <p>
		''' The paths returned from {@code getSelectionPaths} are in the same
		''' order as those supplied to this method.
		''' </summary>
		''' <param name="pPaths"> the new selection </param>
		Public Overridable Property selectionPaths Implements TreeSelectionModel.setSelectionPaths As TreePath()
			Set(ByVal pPaths As TreePath())
				Dim newCount, newCounter, oldCount, oldCounter As Integer
				Dim paths As TreePath() = pPaths
    
				If paths Is Nothing Then
					newCount = 0
				Else
					newCount = paths.Length
				End If
				If selection Is Nothing Then
					oldCount = 0
				Else
					oldCount = selection.Length
				End If
				If (newCount + oldCount) <> 0 Then
					If selectionMode = TreeSelectionModel.SINGLE_TREE_SELECTION Then
		'                 If single selection and more than one path, only allow
		'                   first. 
						If newCount > 1 Then
							paths = New TreePath(0){}
							paths(0) = pPaths(0)
							newCount = 1
						End If
					ElseIf selectionMode = TreeSelectionModel.CONTIGUOUS_TREE_SELECTION Then
		'                 If contiguous selection and paths aren't contiguous,
		'                   only select the first path item. 
						If newCount > 0 AndAlso (Not arePathsContiguous(paths)) Then
							paths = New TreePath(0){}
							paths(0) = pPaths(0)
							newCount = 1
						End If
					End If
    
					Dim beginLeadPath As TreePath = leadPath
					Dim cPaths As New List(Of PathPlaceHolder)(newCount + oldCount)
					Dim newSelectionAsList As IList(Of TreePath) = New List(Of TreePath)(newCount)
    
					lastPaths.Clear()
					leadPath = Nothing
					' Find the paths that are new. 
					For newCounter = 0 To newCount - 1
						Dim path As TreePath = paths(newCounter)
						If path IsNot Nothing AndAlso lastPaths(path) Is Nothing Then
							lastPaths(path) = Boolean.TRUE
							If uniquePaths(path) Is Nothing Then cPaths.Add(New PathPlaceHolder(path, True))
							leadPath = path
							newSelectionAsList.Add(path)
						End If
					Next newCounter
    
					Dim newSelection As TreePath() = newSelectionAsList.ToArray()
    
					' Get the paths that were selected but no longer selected. 
					For oldCounter = 0 To oldCount - 1
						If selection(oldCounter) IsNot Nothing AndAlso lastPaths(selection(oldCounter)) Is Nothing Then cPaths.Add(New PathPlaceHolder(selection(oldCounter), False))
					Next oldCounter
    
					selection = newSelection
    
					Dim tempHT As Dictionary(Of TreePath, Boolean?) = uniquePaths
    
					uniquePaths = lastPaths
					lastPaths = tempHT
					lastPaths.Clear()
    
					' No reason to do this now, but will still call it.
					insureUniqueness()
    
					updateLeadIndex()
    
					resetRowSelection()
					' Notify of the change. 
					If cPaths.Count > 0 Then notifyPathChange(cPaths, beginLeadPath)
				End If
			End Set
			Get
				If selection IsNot Nothing Then
					Dim pathSize As Integer = selection.Length
					Dim result As TreePath() = New TreePath(pathSize - 1){}
    
					Array.Copy(selection, 0, result, 0, pathSize)
					Return result
				End If
				Return New TreePath(){}
			End Get
		End Property

		''' <summary>
		''' Adds path to the current selection. If path is not currently
		''' in the selection the TreeSelectionListeners are notified. This has
		''' no effect if <code>path</code> is null.
		''' </summary>
		''' <param name="path"> the new path to add to the current selection </param>
		Public Overridable Sub addSelectionPath(ByVal path As TreePath) Implements TreeSelectionModel.addSelectionPath
			If path IsNot Nothing Then
				Dim toAdd As TreePath() = New TreePath(0){}

				toAdd(0) = path
				addSelectionPaths(toAdd)
			End If
		End Sub

		''' <summary>
		''' Adds paths to the current selection. If any of the paths in
		''' paths are not currently in the selection the TreeSelectionListeners
		''' are notified. This has
		''' no effect if <code>paths</code> is null.
		''' <p>The lead path is set to the last element in <code>paths</code>.
		''' <p>If the selection mode is <code>CONTIGUOUS_TREE_SELECTION</code>,
		''' and adding the new paths would make the selection discontiguous.
		''' Then two things can result: if the TreePaths in <code>paths</code>
		''' are contiguous, then the selection becomes these TreePaths,
		''' otherwise the TreePaths aren't contiguous and the selection becomes
		''' the first TreePath in <code>paths</code>.
		''' </summary>
		''' <param name="paths"> the new path to add to the current selection </param>
		Public Overridable Sub addSelectionPaths(ByVal paths As TreePath()) Implements TreeSelectionModel.addSelectionPaths
			Dim newPathLength As Integer = (If(paths Is Nothing, 0, paths.Length))

			If newPathLength > 0 Then
				If selectionMode = TreeSelectionModel.SINGLE_TREE_SELECTION Then
					selectionPaths = paths
				ElseIf selectionMode = TreeSelectionModel.CONTIGUOUS_TREE_SELECTION AndAlso (Not canPathsBeAdded(paths)) Then
					If arePathsContiguous(paths) Then
						selectionPaths = paths
					Else
						Dim newPaths As TreePath() = New TreePath(0){}

						newPaths(0) = paths(0)
						selectionPaths = newPaths
					End If
				Else
					Dim counter, validCount As Integer
					Dim oldCount As Integer
					Dim beginLeadPath As TreePath = leadPath
					Dim cPaths As List(Of PathPlaceHolder) = Nothing

					If selection Is Nothing Then
						oldCount = 0
					Else
						oldCount = selection.Length
					End If
	'                 Determine the paths that aren't currently in the
	'                   selection. 
					lastPaths.Clear()
					counter = 0
					validCount = 0
					Do While counter < newPathLength
						If paths(counter) IsNot Nothing Then
							If uniquePaths(paths(counter)) Is Nothing Then
								validCount += 1
								If cPaths Is Nothing Then cPaths = New List(Of PathPlaceHolder)
								cPaths.Add(New PathPlaceHolder(paths(counter), True))
								uniquePaths(paths(counter)) = Boolean.TRUE
								lastPaths(paths(counter)) = Boolean.TRUE
							End If
							leadPath = paths(counter)
						End If
						counter += 1
					Loop

					If leadPath Is Nothing Then leadPath = beginLeadPath

					If validCount > 0 Then
						Dim newSelection As TreePath() = New TreePath(oldCount + validCount - 1){}

						' And build the new selection. 
						If oldCount > 0 Then Array.Copy(selection, 0, newSelection, 0, oldCount)
						If validCount <> paths.Length Then
	'                         Some of the paths in paths are already in
	'                           the selection. 
							Dim newPaths As System.Collections.IEnumerator(Of TreePath) = lastPaths.Keys.GetEnumerator()

							counter = oldCount
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							Do While newPaths.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								newSelection(counter) = newPaths.nextElement()
								counter += 1
							Loop
						Else
							Array.Copy(paths, 0, newSelection, oldCount, validCount)
						End If

						selection = newSelection

						insureUniqueness()

						updateLeadIndex()

						resetRowSelection()

						notifyPathChange(cPaths, beginLeadPath)
					Else
						leadPath = beginLeadPath
					End If
					lastPaths.Clear()
				End If
			End If
		End Sub

		''' <summary>
		''' Removes path from the selection. If path is in the selection
		''' The TreeSelectionListeners are notified. This has no effect if
		''' <code>path</code> is null.
		''' </summary>
		''' <param name="path"> the path to remove from the selection </param>
		Public Overridable Sub removeSelectionPath(ByVal path As TreePath) Implements TreeSelectionModel.removeSelectionPath
			If path IsNot Nothing Then
				Dim rPath As TreePath() = New TreePath(0){}

				rPath(0) = path
				removeSelectionPaths(rPath)
			End If
		End Sub

		''' <summary>
		''' Removes paths from the selection.  If any of the paths in paths
		''' are in the selection the TreeSelectionListeners are notified.
		''' This has no effect if <code>paths</code> is null.
		''' </summary>
		''' <param name="paths"> the paths to remove from the selection </param>
		Public Overridable Sub removeSelectionPaths(ByVal paths As TreePath()) Implements TreeSelectionModel.removeSelectionPaths
			If paths IsNot Nothing AndAlso selection IsNot Nothing AndAlso paths.Length > 0 Then
				If Not canPathsBeRemoved(paths) Then
					' Could probably do something more interesting here! 
					clearSelection()
				Else
					Dim pathsToRemove As List(Of PathPlaceHolder) = Nothing

					' Find the paths that can be removed. 
					For removeCounter As Integer = paths.Length - 1 To 0 Step -1
						If paths(removeCounter) IsNot Nothing Then
							If uniquePaths(paths(removeCounter)) IsNot Nothing Then
								If pathsToRemove Is Nothing Then pathsToRemove = New List(Of PathPlaceHolder)(paths.Length)
								uniquePaths.Remove(paths(removeCounter))
								pathsToRemove.Add(New PathPlaceHolder(paths(removeCounter), False))
							End If
						End If
					Next removeCounter
					If pathsToRemove IsNot Nothing Then
						Dim removeCount As Integer = pathsToRemove.Count
						Dim beginLeadPath As TreePath = leadPath

						If removeCount = selection.Length Then
							selection = Nothing
						Else
							Dim pEnum As System.Collections.IEnumerator(Of TreePath) = uniquePaths.Keys.GetEnumerator()
							Dim validCount As Integer = 0

							selection = New TreePath(selection.Length - removeCount - 1){}
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							Do While pEnum.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								selection(validCount) = pEnum.nextElement()
								validCount += 1
							Loop
						End If
						If leadPath IsNot Nothing AndAlso uniquePaths(leadPath) Is Nothing Then
							If selection IsNot Nothing Then
								leadPath = selection(selection.Length - 1)
							Else
								leadPath = Nothing
							End If
						ElseIf selection IsNot Nothing Then
							leadPath = selection(selection.Length - 1)
						Else
							leadPath = Nothing
						End If
						updateLeadIndex()

						resetRowSelection()

						notifyPathChange(pathsToRemove, beginLeadPath)
					End If
				End If
			End If
		End Sub



		''' <summary>
		''' Returns the number of paths that are selected.
		''' </summary>
		Public Overridable Property selectionCount As Integer Implements TreeSelectionModel.getSelectionCount
			Get
				Return If(selection Is Nothing, 0, selection.Length)
			End Get
		End Property

		''' <summary>
		''' Returns true if the path, <code>path</code>,
		''' is in the current selection.
		''' </summary>
		Public Overridable Function isPathSelected(ByVal path As TreePath) As Boolean Implements TreeSelectionModel.isPathSelected
			Return If(path IsNot Nothing, (uniquePaths(path) IsNot Nothing), False)
		End Function

		''' <summary>
		''' Returns true if the selection is currently empty.
		''' </summary>
		Public Overridable Property selectionEmpty As Boolean Implements TreeSelectionModel.isSelectionEmpty
			Get
				Return (selection Is Nothing OrElse selection.Length = 0)
			End Get
		End Property

		''' <summary>
		''' Empties the current selection.  If this represents a change in the
		''' current selection, the selection listeners are notified.
		''' </summary>
		Public Overridable Sub clearSelection() Implements TreeSelectionModel.clearSelection
			If selection IsNot Nothing AndAlso selection.Length > 0 Then
				Dim selSize As Integer = selection.Length
				Dim newness As Boolean() = New Boolean(selSize - 1){}

				For counter As Integer = 0 To selSize - 1
					newness(counter) = False
				Next counter

				Dim [event] As New TreeSelectionEvent(Me, selection, newness, leadPath, Nothing)

				leadPath = Nothing
					leadRow = -1
					leadIndex = leadRow
				uniquePaths.Clear()
				selection = Nothing
				resetRowSelection()
				fireValueChanged([event])
			End If
		End Sub

		''' <summary>
		''' Adds x to the list of listeners that are notified each time the
		''' set of selected TreePaths changes.
		''' </summary>
		''' <param name="x"> the new listener to be added </param>
		Public Overridable Sub addTreeSelectionListener(ByVal x As TreeSelectionListener) Implements TreeSelectionModel.addTreeSelectionListener
			listenerList.add(GetType(TreeSelectionListener), x)
		End Sub

		''' <summary>
		''' Removes x from the list of listeners that are notified each time
		''' the set of selected TreePaths changes.
		''' </summary>
		''' <param name="x"> the listener to remove </param>
		Public Overridable Sub removeTreeSelectionListener(ByVal x As TreeSelectionListener) Implements TreeSelectionModel.removeTreeSelectionListener
			listenerList.remove(GetType(TreeSelectionListener), x)
		End Sub

		''' <summary>
		''' Returns an array of all the tree selection listeners
		''' registered on this model.
		''' </summary>
		''' <returns> all of this model's <code>TreeSelectionListener</code>s
		'''         or an empty
		'''         array if no tree selection listeners are currently registered
		''' </returns>
		''' <seealso cref= #addTreeSelectionListener </seealso>
		''' <seealso cref= #removeTreeSelectionListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property treeSelectionListeners As TreeSelectionListener()
			Get
				Return listenerList.getListeners(GetType(TreeSelectionListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that are registered for
		''' tree selection events on this object. </summary>
		''' <seealso cref= #addTreeSelectionListener </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireValueChanged(ByVal e As TreeSelectionEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' TreeSelectionEvent e = null;
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TreeSelectionListener) Then CType(___listeners(i+1), TreeSelectionListener).valueChanged(e)
			Next i
		End Sub

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this model.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' 
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal,
		''' such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' <code>DefaultTreeSelectionModel</code> <code>m</code>
		''' for its tree selection listeners with the following code:
		''' 
		''' <pre>TreeSelectionListener[] tsls = (TreeSelectionListener[])(m.getListeners(TreeSelectionListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this component,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getTreeSelectionListeners </seealso>
		''' <seealso cref= #getPropertyChangeListeners
		''' 
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function

		''' <summary>
		''' Returns the selection in terms of rows. There is not
		''' necessarily a one-to-one mapping between the {@code TreePath}s
		''' returned from {@code getSelectionPaths} and this method. In
		''' particular, if a {@code TreePath} is not viewable (the {@code
		''' RowMapper} returns {@code -1} for the row corresponding to the
		''' {@code TreePath}), then the corresponding row is not included
		''' in the returned array. For example, if the selection consists
		''' of two paths, {@code A} and {@code B}, with {@code A} at row
		''' {@code 10}, and {@code B} not currently viewable, then this method
		''' returns an array with the single entry {@code 10}.
		''' </summary>
		''' <returns> the selection in terms of rows </returns>
		Public Overridable Property selectionRows As Integer() Implements TreeSelectionModel.getSelectionRows
			Get
				' This is currently rather expensive.  Needs
				' to be better support from ListSelectionModel to speed this up.
				If rowMapper IsNot Nothing AndAlso selection IsNot Nothing AndAlso selection.Length > 0 Then
					Dim rows As Integer() = rowMapper.getRowsForPaths(selection)
    
					If rows IsNot Nothing Then
						Dim invisCount As Integer = 0
    
						For counter As Integer = rows.Length - 1 To 0 Step -1
							If rows(counter) = -1 Then invisCount += 1
						Next counter
						If invisCount > 0 Then
							If invisCount = rows.Length Then
								rows = Nothing
							Else
								Dim tempRows As Integer() = New Integer(rows.Length - invisCount - 1){}
    
								Dim counter As Integer = rows.Length - 1
								Dim visCounter As Integer = 0
								Do While counter >= 0
									If rows(counter) <> -1 Then
										tempRows(visCounter) = rows(counter)
										visCounter += 1
									End If
									counter -= 1
								Loop
								rows = tempRows
							End If
						End If
					End If
					Return rows
				End If
				Return New Integer(){}
			End Get
		End Property

		''' <summary>
		''' Returns the smallest value obtained from the RowMapper for the
		''' current set of selected TreePaths. If nothing is selected,
		''' or there is no RowMapper, this will return -1.
		''' </summary>
		Public Overridable Property minSelectionRow As Integer Implements TreeSelectionModel.getMinSelectionRow
			Get
				Return listSelectionModel.minSelectionIndex
			End Get
		End Property

		''' <summary>
		''' Returns the largest value obtained from the RowMapper for the
		''' current set of selected TreePaths. If nothing is selected,
		''' or there is no RowMapper, this will return -1.
		''' </summary>
		Public Overridable Property maxSelectionRow As Integer Implements TreeSelectionModel.getMaxSelectionRow
			Get
				Return listSelectionModel.maxSelectionIndex
			End Get
		End Property

		''' <summary>
		''' Returns true if the row identified by <code>row</code> is selected.
		''' </summary>
		Public Overridable Function isRowSelected(ByVal row As Integer) As Boolean Implements TreeSelectionModel.isRowSelected
			Return listSelectionModel.isSelectedIndex(row)
		End Function

		''' <summary>
		''' Updates this object's mapping from TreePath to rows. This should
		''' be invoked when the mapping from TreePaths to integers has changed
		''' (for example, a node has been expanded).
		''' <p>You do not normally have to call this, JTree and its associated
		''' Listeners will invoke this for you. If you are implementing your own
		''' View class, then you will have to invoke this.
		''' <p>This will invoke <code>insureRowContinuity</code> to make sure
		''' the currently selected TreePaths are still valid based on the
		''' selection mode.
		''' </summary>
		Public Overridable Sub resetRowSelection() Implements TreeSelectionModel.resetRowSelection
			listSelectionModel.clearSelection()
			If selection IsNot Nothing AndAlso rowMapper IsNot Nothing Then
				Dim aRow As Integer
				Dim validCount As Integer = 0
				Dim rows As Integer() = rowMapper.getRowsForPaths(selection)

				Dim counter As Integer = 0
				Dim maxCounter As Integer = selection.Length
				Do While counter < maxCounter
					aRow = rows(counter)
					If aRow <> -1 Then listSelectionModel.addSelectionInterval(aRow, aRow)
					counter += 1
				Loop
				If leadIndex <> -1 AndAlso rows IsNot Nothing Then
					leadRow = rows(leadIndex)
				ElseIf leadPath IsNot Nothing Then
					' Lead selection path doesn't have to be in the selection.
					tempPaths(0) = leadPath
					rows = rowMapper.getRowsForPaths(tempPaths)
					leadRow = If(rows IsNot Nothing, rows(0), -1)
				Else
					leadRow = -1
				End If
				insureRowContinuity()

			Else
				leadRow = -1
			End If
		End Sub

		''' <summary>
		''' Returns the lead selection index. That is the last index that was
		''' added.
		''' </summary>
		Public Overridable Property leadSelectionRow As Integer Implements TreeSelectionModel.getLeadSelectionRow
			Get
				Return leadRow
			End Get
		End Property

		''' <summary>
		''' Returns the last path that was added. This may differ from the
		''' leadSelectionPath property maintained by the JTree.
		''' </summary>
		Public Overridable Property leadSelectionPath As TreePath Implements TreeSelectionModel.getLeadSelectionPath
			Get
				Return leadPath
			End Get
		End Property

		''' <summary>
		''' Adds a PropertyChangeListener to the listener list.
		''' The listener is registered for all properties.
		''' <p>
		''' A PropertyChangeEvent will get fired when the selection mode
		''' changes.
		''' </summary>
		''' <param name="listener">  the PropertyChangeListener to be added </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener) Implements TreeSelectionModel.addPropertyChangeListener
			If changeSupport Is Nothing Then changeSupport = New SwingPropertyChangeSupport(Me)
			changeSupport.addPropertyChangeListener(listener)
		End Sub

		''' <summary>
		''' Removes a PropertyChangeListener from the listener list.
		''' This removes a PropertyChangeListener that was registered
		''' for all properties.
		''' </summary>
		''' <param name="listener">  the PropertyChangeListener to be removed </param>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener) Implements TreeSelectionModel.removePropertyChangeListener
			If changeSupport Is Nothing Then Return
			changeSupport.removePropertyChangeListener(listener)
		End Sub

		''' <summary>
		''' Returns an array of all the property change listeners
		''' registered on this <code>DefaultTreeSelectionModel</code>.
		''' </summary>
		''' <returns> all of this model's <code>PropertyChangeListener</code>s
		'''         or an empty
		'''         array if no property change listeners are currently registered
		''' </returns>
		''' <seealso cref= #addPropertyChangeListener </seealso>
		''' <seealso cref= #removePropertyChangeListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property propertyChangeListeners As java.beans.PropertyChangeListener()
			Get
				If changeSupport Is Nothing Then Return New java.beans.PropertyChangeListener(){}
				Return changeSupport.propertyChangeListeners
			End Get
		End Property

		''' <summary>
		''' Makes sure the currently selected <code>TreePath</code>s are valid
		''' for the current selection mode.
		''' If the selection mode is <code>CONTIGUOUS_TREE_SELECTION</code>
		''' and a <code>RowMapper</code> exists, this will make sure all
		''' the rows are contiguous, that is, when sorted all the rows are
		''' in order with no gaps.
		''' If the selection isn't contiguous, the selection is
		''' reset to contain the first set, when sorted, of contiguous rows.
		''' <p>
		''' If the selection mode is <code>SINGLE_TREE_SELECTION</code> and
		''' more than one TreePath is selected, the selection is reset to
		''' contain the first path currently selected.
		''' </summary>
		Protected Friend Overridable Sub insureRowContinuity()
			If selectionMode = TreeSelectionModel.CONTIGUOUS_TREE_SELECTION AndAlso selection IsNot Nothing AndAlso rowMapper IsNot Nothing Then
				Dim lModel As javax.swing.DefaultListSelectionModel = listSelectionModel
				Dim min As Integer = lModel.minSelectionIndex

				If min <> -1 Then
					Dim counter As Integer = min
					Dim maxCounter As Integer = lModel.maxSelectionIndex
					Do While counter <= maxCounter
						If Not lModel.isSelectedIndex(counter) Then
							If counter = min Then
								clearSelection()
							Else
								Dim newSel As TreePath() = New TreePath(counter - min - 1){}
								Dim selectionIndex As Integer() = rowMapper.getRowsForPaths(selection)
								' find the actual selection pathes corresponded to the
								' rows of the new selection
								For i As Integer = 0 To selectionIndex.Length - 1
									If selectionIndex(i)<counter Then newSel(selectionIndex(i)-min) = selection(i)
								Next i
								selectionPaths = newSel
								Exit Do
							End If
						End If
						counter += 1
					Loop
				End If
			ElseIf selectionMode = TreeSelectionModel.SINGLE_TREE_SELECTION AndAlso selection IsNot Nothing AndAlso selection.Length > 1 Then
				selectionPath = selection(0)
			End If
		End Sub

		''' <summary>
		''' Returns true if the paths are contiguous,
		''' or this object has no RowMapper.
		''' </summary>
		Protected Friend Overridable Function arePathsContiguous(ByVal paths As TreePath()) As Boolean
			If rowMapper Is Nothing OrElse paths.Length < 2 Then
				Return True
			Else
				Dim bitSet As New BitArray(32)
				Dim anIndex, counter, min As Integer
				Dim pathCount As Integer = paths.Length
				Dim validCount As Integer = 0
				Dim tempPath As TreePath() = New TreePath(0){}

				tempPath(0) = paths(0)
				min = rowMapper.getRowsForPaths(tempPath)(0)
				For counter = 0 To pathCount - 1
					If paths(counter) IsNot Nothing Then
						tempPath(0) = paths(counter)
						Dim rows As Integer() = rowMapper.getRowsForPaths(tempPath)
						If rows Is Nothing Then Return False
						anIndex = rows(0)
						If anIndex = -1 OrElse anIndex < (min - pathCount) OrElse anIndex > (min + pathCount) Then Return False
						If anIndex < min Then min = anIndex
						If Not bitSet.Get(anIndex) Then
							bitSet.Set(anIndex, True)
							validCount += 1
						End If
					End If
				Next counter
				Dim maxCounter As Integer = validCount + min

				For counter = min To maxCounter - 1
					If Not bitSet.Get(counter) Then Return False
				Next counter
			End If
			Return True
		End Function

		''' <summary>
		''' Used to test if a particular set of <code>TreePath</code>s can
		''' be added. This will return true if <code>paths</code> is null (or
		''' empty), or this object has no RowMapper, or nothing is currently selected,
		''' or the selection mode is <code>DISCONTIGUOUS_TREE_SELECTION</code>, or
		''' adding the paths to the current selection still results in a
		''' contiguous set of <code>TreePath</code>s.
		''' </summary>
		Protected Friend Overridable Function canPathsBeAdded(ByVal paths As TreePath()) As Boolean
			If paths Is Nothing OrElse paths.Length = 0 OrElse rowMapper Is Nothing OrElse selection Is Nothing OrElse selectionMode = TreeSelectionModel.DISCONTIGUOUS_TREE_SELECTION Then
				Return True
			Else
				Dim bitSet As New BitArray
				Dim lModel As javax.swing.DefaultListSelectionModel = listSelectionModel
				Dim anIndex As Integer
				Dim counter As Integer
				Dim min As Integer = lModel.minSelectionIndex
				Dim max As Integer = lModel.maxSelectionIndex
				Dim tempPath As TreePath() = New TreePath(0){}

				If min <> -1 Then
					For counter = min To max
						If lModel.isSelectedIndex(counter) Then bitSet.Set(counter, True)
					Next counter
				Else
					tempPath(0) = paths(0)
						max = rowMapper.getRowsForPaths(tempPath)(0)
						min = max
				End If
				For counter = paths.Length - 1 To 0 Step -1
					If paths(counter) IsNot Nothing Then
						tempPath(0) = paths(counter)
						Dim rows As Integer() = rowMapper.getRowsForPaths(tempPath)
						If rows Is Nothing Then Return False
						anIndex = rows(0)
						min = Math.Min(anIndex, min)
						max = Math.Max(anIndex, max)
						If anIndex = -1 Then Return False
						bitSet.Set(anIndex, True)
					End If
				Next counter
				For counter = min To max
					If Not bitSet.Get(counter) Then Return False
				Next counter
			End If
			Return True
		End Function

		''' <summary>
		''' Returns true if the paths can be removed without breaking the
		''' continuity of the model.
		''' This is rather expensive.
		''' </summary>
		Protected Friend Overridable Function canPathsBeRemoved(ByVal paths As TreePath()) As Boolean
			If rowMapper Is Nothing OrElse selection Is Nothing OrElse selectionMode = TreeSelectionModel.DISCONTIGUOUS_TREE_SELECTION Then
				Return True
			Else
				Dim bitSet As New BitArray
				Dim counter As Integer
				Dim pathCount As Integer = paths.Length
				Dim anIndex As Integer
				Dim min As Integer = -1
				Dim validCount As Integer = 0
				Dim tempPath As TreePath() = New TreePath(0){}
				Dim rows As Integer()

				' Determine the rows for the removed entries. 
				lastPaths.Clear()
				For counter = 0 To pathCount - 1
					If paths(counter) IsNot Nothing Then lastPaths(paths(counter)) = Boolean.TRUE
				Next counter
				For counter = selection.Length - 1 To 0 Step -1
					If lastPaths(selection(counter)) Is Nothing Then
						tempPath(0) = selection(counter)
						rows = rowMapper.getRowsForPaths(tempPath)
						If rows IsNot Nothing AndAlso rows(0) <> -1 AndAlso (Not bitSet.Get(rows(0))) Then
							validCount += 1
							If min = -1 Then
								min = rows(0)
							Else
								min = Math.Min(min, rows(0))
							End If
							bitSet.Set(rows(0), True)
						End If
					End If
				Next counter
				lastPaths.Clear()
				' Make sure they are contiguous. 
				If validCount > 1 Then
					For counter = min + validCount - 1 To min Step -1
						If Not bitSet.Get(counter) Then Return False
					Next counter
				End If
			End If
			Return True
		End Function

		''' <summary>
		''' Notifies listeners of a change in path. changePaths should contain
		''' instances of PathPlaceHolder.
		''' </summary>
		''' @deprecated As of JDK version 1.7 
		<Obsolete("As of JDK version 1.7")> _
		Protected Friend Overridable Sub notifyPathChange(Of T1)(ByVal changedPaths As List(Of T1), ByVal oldLeadSelection As TreePath)
			Dim cPathCount As Integer = changedPaths.Count
			Dim newness As Boolean() = New Boolean(cPathCount - 1){}
			Dim paths As TreePath() = New TreePath(cPathCount - 1){}
			Dim placeholder As PathPlaceHolder

			For counter As Integer = 0 To cPathCount - 1
				placeholder = CType(changedPaths(counter), PathPlaceHolder)
				newness(counter) = placeholder.isNew
				paths(counter) = placeholder.path
			Next counter

			Dim [event] As New TreeSelectionEvent(Me, paths, newness, oldLeadSelection, leadPath)

			fireValueChanged([event])
		End Sub

		''' <summary>
		''' Updates the leadIndex instance variable.
		''' </summary>
		Protected Friend Overridable Sub updateLeadIndex()
			If leadPath IsNot Nothing Then
				If selection Is Nothing Then
					leadPath = Nothing
						leadRow = -1
						leadIndex = leadRow
				Else
						leadIndex = -1
						leadRow = leadIndex
					For counter As Integer = selection.Length - 1 To 0 Step -1
						' Can use == here since we know leadPath came from
						' selection
						If selection(counter) Is leadPath Then
							leadIndex = counter
							Exit For
						End If
					Next counter
				End If
			Else
				leadIndex = -1
			End If
		End Sub

		''' <summary>
		''' This method is obsolete and its implementation is now a noop.  It's
		''' still called by setSelectionPaths and addSelectionPaths, but only
		''' for backwards compatibility.
		''' </summary>
		Protected Friend Overridable Sub insureUniqueness()
		End Sub


		''' <summary>
		''' Returns a string that displays and identifies this
		''' object's properties.
		''' </summary>
		''' <returns> a String representation of this object </returns>
		Public Overrides Function ToString() As String
			Dim selCount As Integer = selectionCount
			Dim retBuffer As New StringBuilder
			Dim rows As Integer()

			If rowMapper IsNot Nothing Then
				rows = rowMapper.getRowsForPaths(selection)
			Else
				rows = Nothing
			End If
			retBuffer.Append(Me.GetType().name & " " & GetHashCode() & " [ ")
			For counter As Integer = 0 To selCount - 1
				If rows IsNot Nothing Then
					retBuffer.Append(selection(counter).ToString() & "@" & Convert.ToString(rows(counter)) & " ")
				Else
					retBuffer.Append(selection(counter).ToString() & " ")
				End If
			Next counter
			retBuffer.Append("]")
			Return retBuffer.ToString()
		End Function

		''' <summary>
		''' Returns a clone of this object with the same selection.
		''' This method does not duplicate
		''' selection listeners and property listeners.
		''' </summary>
		''' <exception cref="CloneNotSupportedException"> never thrown by instances of
		'''                                       this class </exception>
		Public Overridable Function clone() As Object
			Dim ___clone As DefaultTreeSelectionModel = CType(MyBase.clone(), DefaultTreeSelectionModel)

			___clone.changeSupport = Nothing
			If selection IsNot Nothing Then
				Dim selLength As Integer = selection.Length

				___clone.selection = New TreePath(selLength - 1){}
				Array.Copy(selection, 0, ___clone.selection, 0, selLength)
			End If
			___clone.listenerList = New EventListenerList
			___clone.listSelectionModel = CType(listSelectionModel.clone(), javax.swing.DefaultListSelectionModel)
			___clone.uniquePaths = New Dictionary(Of TreePath, Boolean?)
			___clone.lastPaths = New Dictionary(Of TreePath, Boolean?)
			___clone.tempPaths = New TreePath(0){}
			Return ___clone
		End Function

		' Serialization support.
		Private Sub writeObject(ByVal s As ObjectOutputStream)
			Dim tValues As Object()

			s.defaultWriteObject()
			' Save the rowMapper, if it implements Serializable
			If rowMapper IsNot Nothing AndAlso TypeOf rowMapper Is Serializable Then
				tValues = New Object(1){}
				tValues(0) = "rowMapper"
				tValues(1) = rowMapper
			Else
				tValues = New Object(){}
			End If
			s.writeObject(tValues)
		End Sub


		Private Sub readObject(ByVal s As ObjectInputStream)
			Dim tValues As Object()

			s.defaultReadObject()

			tValues = CType(s.readObject(), Object())

			If tValues.Length > 0 AndAlso tValues(0).Equals("rowMapper") Then rowMapper = CType(tValues(1), RowMapper)
		End Sub
	End Class

	''' <summary>
	''' Holds a path and whether or not it is new.
	''' </summary>
	Friend Class PathPlaceHolder
		Protected Friend isNew As Boolean
		Protected Friend path As TreePath

		Friend Sub New(ByVal path As TreePath, ByVal isNew As Boolean)
			Me.path = path
			Me.isNew = isNew
		End Sub
	End Class

End Namespace
Imports System

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

Namespace javax.swing.event


	''' <summary>
	''' An event that characterizes a change in the current
	''' selection.  The change is based on any number of paths.
	''' TreeSelectionListeners will generally query the source of
	''' the event for the new selected status of each potentially
	''' changed row.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= TreeSelectionListener </seealso>
	''' <seealso cref= javax.swing.tree.TreeSelectionModel
	''' 
	''' @author Scott Violet </seealso>
	Public Class TreeSelectionEvent
		Inherits java.util.EventObject

		''' <summary>
		''' Paths this event represents. </summary>
		Protected Friend paths As javax.swing.tree.TreePath()
		''' <summary>
		''' For each path identifies if that path is in fact new. </summary>
		Protected Friend areNew As Boolean()
		''' <summary>
		''' leadSelectionPath before the paths changed, may be null. </summary>
		Protected Friend oldLeadSelectionPath As javax.swing.tree.TreePath
		''' <summary>
		''' leadSelectionPath after the paths changed, may be null. </summary>
		Protected Friend newLeadSelectionPath As javax.swing.tree.TreePath

		''' <summary>
		''' Represents a change in the selection of a TreeSelectionModel.
		''' paths identifies the paths that have been either added or
		''' removed from the selection.
		''' </summary>
		''' <param name="source"> source of event </param>
		''' <param name="paths"> the paths that have changed in the selection </param>
		Public Sub New(ByVal source As Object, ByVal paths As javax.swing.tree.TreePath(), ByVal areNew As Boolean(), ByVal oldLeadSelectionPath As javax.swing.tree.TreePath, ByVal newLeadSelectionPath As javax.swing.tree.TreePath)
			MyBase.New(source)
			Me.paths = paths
			Me.areNew = areNew
			Me.oldLeadSelectionPath = oldLeadSelectionPath
			Me.newLeadSelectionPath = newLeadSelectionPath
		End Sub

		''' <summary>
		''' Represents a change in the selection of a TreeSelectionModel.
		''' path identifies the path that have been either added or
		''' removed from the selection.
		''' </summary>
		''' <param name="source"> source of event </param>
		''' <param name="path"> the path that has changed in the selection </param>
		''' <param name="isNew"> whether or not the path is new to the selection, false
		''' means path was removed from the selection. </param>
		Public Sub New(ByVal source As Object, ByVal path As javax.swing.tree.TreePath, ByVal isNew As Boolean, ByVal oldLeadSelectionPath As javax.swing.tree.TreePath, ByVal newLeadSelectionPath As javax.swing.tree.TreePath)
			MyBase.New(source)
			paths = New javax.swing.tree.TreePath(0){}
			paths(0) = path
			areNew = New Boolean(0){}
			areNew(0) = isNew
			Me.oldLeadSelectionPath = oldLeadSelectionPath
			Me.newLeadSelectionPath = newLeadSelectionPath
		End Sub

		''' <summary>
		''' Returns the paths that have been added or removed from the
		''' selection.
		''' </summary>
		Public Overridable Property paths As javax.swing.tree.TreePath()
			Get
				Dim numPaths As Integer
				Dim retPaths As javax.swing.tree.TreePath()
    
				numPaths = paths.Length
				retPaths = New javax.swing.tree.TreePath(numPaths - 1){}
				Array.Copy(paths, 0, retPaths, 0, numPaths)
				Return retPaths
			End Get
		End Property

		''' <summary>
		''' Returns the first path element.
		''' </summary>
		Public Overridable Property path As javax.swing.tree.TreePath
			Get
				Return paths(0)
			End Get
		End Property

		''' <summary>
		''' Returns whether the path identified by {@code getPath} was
		''' added to the selection.  A return value of {@code true}
		''' indicates the path identified by {@code getPath} was added to
		''' the selection. A return value of {@code false} indicates {@code
		''' getPath} was selected, but is no longer selected.
		''' </summary>
		''' <returns> {@code true} if {@code getPath} was added to the selection,
		'''         {@code false} otherwise </returns>
		Public Overridable Property addedPath As Boolean
			Get
				Return areNew(0)
			End Get
		End Property

		''' <summary>
		''' Returns whether the specified path was added to the selection.
		''' A return value of {@code true} indicates the path identified by
		''' {@code path} was added to the selection. A return value of
		''' {@code false} indicates {@code path} is no longer selected. This method
		''' is only valid for the paths returned from {@code getPaths()}; invoking
		''' with a path not included in {@code getPaths()} throws an
		''' {@code IllegalArgumentException}.
		''' </summary>
		''' <param name="path"> the path to test </param>
		''' <returns> {@code true} if {@code path} was added to the selection,
		'''         {@code false} otherwise </returns>
		''' <exception cref="IllegalArgumentException"> if {@code path} is not contained
		'''         in {@code getPaths} </exception>
		''' <seealso cref= #getPaths </seealso>
		Public Overridable Function isAddedPath(ByVal path As javax.swing.tree.TreePath) As Boolean
			For counter As Integer = paths.Length - 1 To 0 Step -1
				If paths(counter).Equals(path) Then Return areNew(counter)
			Next counter
			Throw New System.ArgumentException("path is not a path identified by the TreeSelectionEvent")
		End Function

		''' <summary>
		''' Returns whether the path at {@code getPaths()[index]} was added
		''' to the selection.  A return value of {@code true} indicates the
		''' path was added to the selection. A return value of {@code false}
		''' indicates the path is no longer selected.
		''' </summary>
		''' <param name="index"> the index of the path to test </param>
		''' <returns> {@code true} if the path was added to the selection,
		'''         {@code false} otherwise </returns>
		''' <exception cref="IllegalArgumentException"> if index is outside the range of
		'''         {@code getPaths} </exception>
		''' <seealso cref= #getPaths
		''' 
		''' @since 1.3 </seealso>
		Public Overridable Function isAddedPath(ByVal index As Integer) As Boolean
			If paths Is Nothing OrElse index < 0 OrElse index >= paths.Length Then Throw New System.ArgumentException("index is beyond range of added paths identified by TreeSelectionEvent")
			Return areNew(index)
		End Function

		''' <summary>
		''' Returns the path that was previously the lead path.
		''' </summary>
		Public Overridable Property oldLeadSelectionPath As javax.swing.tree.TreePath
			Get
				Return oldLeadSelectionPath
			End Get
		End Property

		''' <summary>
		''' Returns the current lead path.
		''' </summary>
		Public Overridable Property newLeadSelectionPath As javax.swing.tree.TreePath
			Get
				Return newLeadSelectionPath
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the receiver, but with the source being newSource.
		''' </summary>
		Public Overridable Function cloneWithSource(ByVal newSource As Object) As Object
		  ' Fix for IE bug - crashing
		  Return New TreeSelectionEvent(newSource, paths,areNew, oldLeadSelectionPath, newLeadSelectionPath)
		End Function
	End Class

End Namespace
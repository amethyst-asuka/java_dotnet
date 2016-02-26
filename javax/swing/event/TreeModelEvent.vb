Imports System
Imports System.Text

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
	''' Encapsulates information describing changes to a tree model, and
	''' used to notify tree model listeners of the change.
	''' For more information and examples see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/events/treemodellistener.html">How to Write a Tree Model Listener</a>,
	''' a section in <em>The Java Tutorial.</em>
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
	''' @author Rob Davis
	''' @author Ray Ryan
	''' @author Scott Violet
	''' </summary>
	Public Class TreeModelEvent
		Inherits java.util.EventObject

		''' <summary>
		''' Path to the parent of the nodes that have changed. </summary>
		Protected Friend path As javax.swing.tree.TreePath
		''' <summary>
		''' Indices identifying the position of where the children were. </summary>
		Protected Friend childIndices As Integer()
		''' <summary>
		''' Children that have been removed. </summary>
		Protected Friend children As Object()

		''' <summary>
		''' Used to create an event when nodes have been changed, inserted, or
		''' removed, identifying the path to the parent of the modified items as
		''' an array of Objects. All of the modified objects are siblings which are
		''' direct descendents (not grandchildren) of the specified parent.
		''' The positions at which the inserts, deletes, or changes occurred are
		''' specified by an array of <code>int</code>. The indexes in that array
		''' must be in order, from lowest to highest.
		''' <p>
		''' For changes, the indexes in the model correspond exactly to the indexes
		''' of items currently displayed in the UI. As a result, it is not really
		''' critical if the indexes are not in their exact order. But after multiple
		''' inserts or deletes, the items currently in the UI no longer correspond
		''' to the items in the model. It is therefore critical to specify the
		''' indexes properly for inserts and deletes.
		''' <p>
		''' For inserts, the indexes represent the <i>final</i> state of the tree,
		''' after the inserts have occurred. Since the indexes must be specified in
		''' order, the most natural processing methodology is to do the inserts
		''' starting at the lowest index and working towards the highest. Accumulate
		''' a Vector of <code>Integer</code> objects that specify the
		''' insert-locations as you go, then convert the Vector to an
		''' array of <code>int</code> to create the event. When the postition-index
		''' equals zero, the node is inserted at the beginning of the list. When the
		''' position index equals the size of the list, the node is "inserted" at
		''' (appended to) the end of the list.
		''' <p>
		''' For deletes, the indexes represent the <i>initial</i> state of the tree,
		''' before the deletes have occurred. Since the indexes must be specified in
		''' order, the most natural processing methodology is to use a delete-counter.
		''' Start by initializing the counter to zero and start work through the
		''' list from lowest to highest. Every time you do a delete, add the current
		''' value of the delete-counter to the index-position where the delete occurred,
		''' and append the result to a Vector of delete-locations, using
		''' <code>addElement()</code>. Then increment the delete-counter. The index
		''' positions stored in the Vector therefore reflect the effects of all previous
		''' deletes, so they represent each object's position in the initial tree.
		''' (You could also start at the highest index and working back towards the
		''' lowest, accumulating a Vector of delete-locations as you go using the
		''' <code>insertElementAt(Integer, 0)</code>.) However you produce the Vector
		''' of initial-positions, you then need to convert the Vector of <code>Integer</code>
		''' objects to an array of <code>int</code> to create the event.
		''' <p>
		''' <b>Notes:</b><ul style="list-style-type:none">
		''' <li>Like the <code>insertNodeInto</code> method in the
		'''    <code>DefaultTreeModel</code> class, <code>insertElementAt</code>
		'''    appends to the <code>Vector</code> when the index matches the size
		'''    of the vector. So you can use <code>insertElementAt(Integer, 0)</code>
		'''    even when the vector is empty.</li>
		''' <li>To create a node changed event for the root node, specify the parent
		'''     and the child indices as <code>null</code>.</li>
		''' </ul>
		''' </summary>
		''' <param name="source"> the Object responsible for generating the event (typically
		'''               the creator of the event object passes <code>this</code>
		'''               for its value) </param>
		''' <param name="path">   an array of Object identifying the path to the
		'''               parent of the modified item(s), where the first element
		'''               of the array is the Object stored at the root node and
		'''               the last element is the Object stored at the parent node </param>
		''' <param name="childIndices"> an array of <code>int</code> that specifies the
		'''               index values of the removed items. The indices must be
		'''               in sorted order, from lowest to highest </param>
		''' <param name="children"> an array of Object containing the inserted, removed, or
		'''                 changed objects </param>
		''' <seealso cref= TreePath </seealso>
		Public Sub New(ByVal source As Object, ByVal path As Object(), ByVal childIndices As Integer(), ByVal children As Object())
			Me.New(source,If(path Is Nothing, Nothing, New javax.swing.tree.TreePath(path)), childIndices, children)
		End Sub

		''' <summary>
		''' Used to create an event when nodes have been changed, inserted, or
		''' removed, identifying the path to the parent of the modified items as
		''' a TreePath object. For more information on how to specify the indexes
		''' and objects, see
		''' <code>TreeModelEvent(Object,Object[],int[],Object[])</code>.
		''' </summary>
		''' <param name="source"> the Object responsible for generating the event (typically
		'''               the creator of the event object passes <code>this</code>
		'''               for its value) </param>
		''' <param name="path">   a TreePath object that identifies the path to the
		'''               parent of the modified item(s) </param>
		''' <param name="childIndices"> an array of <code>int</code> that specifies the
		'''               index values of the modified items </param>
		''' <param name="children"> an array of Object containing the inserted, removed, or
		'''                 changed objects
		''' </param>
		''' <seealso cref= #TreeModelEvent(Object,Object[],int[],Object[]) </seealso>
		Public Sub New(ByVal source As Object, ByVal path As javax.swing.tree.TreePath, ByVal childIndices As Integer(), ByVal children As Object())
			MyBase.New(source)
			Me.path = path
			Me.childIndices = childIndices
			Me.children = children
		End Sub

		''' <summary>
		''' Used to create an event when the node structure has changed in some way,
		''' identifying the path to the root of a modified subtree as an array of
		''' Objects. A structure change event might involve nodes swapping position,
		''' for example, or it might encapsulate multiple inserts and deletes in the
		''' subtree stemming from the node, where the changes may have taken place at
		''' different levels of the subtree.
		''' <blockquote>
		'''   <b>Note:</b><br>
		'''   JTree collapses all nodes under the specified node, so that only its
		'''   immediate children are visible.
		''' </blockquote>
		''' </summary>
		''' <param name="source"> the Object responsible for generating the event (typically
		'''               the creator of the event object passes <code>this</code>
		'''               for its value) </param>
		''' <param name="path">   an array of Object identifying the path to the root of the
		'''               modified subtree, where the first element of the array is
		'''               the object stored at the root node and the last element
		'''               is the object stored at the changed node </param>
		''' <seealso cref= TreePath </seealso>
		Public Sub New(ByVal source As Object, ByVal path As Object())
			Me.New(source,If(path Is Nothing, Nothing, New javax.swing.tree.TreePath(path)))
		End Sub

		''' <summary>
		''' Used to create an event when the node structure has changed in some way,
		''' identifying the path to the root of the modified subtree as a TreePath
		''' object. For more information on this event specification, see
		''' <code>TreeModelEvent(Object,Object[])</code>.
		''' </summary>
		''' <param name="source"> the Object responsible for generating the event (typically
		'''               the creator of the event object passes <code>this</code>
		'''               for its value) </param>
		''' <param name="path">   a TreePath object that identifies the path to the
		'''               change. In the DefaultTreeModel,
		'''               this object contains an array of user-data objects,
		'''               but a subclass of TreePath could use some totally
		'''               different mechanism -- for example, a node ID number
		''' </param>
		''' <seealso cref= #TreeModelEvent(Object,Object[]) </seealso>
		Public Sub New(ByVal source As Object, ByVal path As javax.swing.tree.TreePath)
			MyBase.New(source)
			Me.path = path
			Me.childIndices = New Integer(){}
		End Sub

		''' <summary>
		''' For all events, except treeStructureChanged,
		''' returns the parent of the changed nodes.
		''' For treeStructureChanged events, returns the ancestor of the
		''' structure that has changed. This and
		''' <code>getChildIndices</code> are used to get a list of the effected
		''' nodes.
		''' <p>
		''' The one exception to this is a treeNodesChanged event that is to
		''' identify the root, in which case this will return the root
		''' and <code>getChildIndices</code> will return null.
		''' </summary>
		''' <returns> the TreePath used in identifying the changed nodes. </returns>
		''' <seealso cref= TreePath#getLastPathComponent </seealso>
		Public Overridable Property treePath As javax.swing.tree.TreePath
			Get
				Return path
			End Get
		End Property

		''' <summary>
		''' Convenience method to get the array of objects from the TreePath
		''' instance that this event wraps.
		''' </summary>
		''' <returns> an array of Objects, where the first Object is the one
		'''         stored at the root and the last object is the one
		'''         stored at the node identified by the path </returns>
		Public Overridable Property path As Object()
			Get
				If path IsNot Nothing Then Return path.path
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the objects that are children of the node identified by
		''' <code>getPath</code> at the locations specified by
		''' <code>getChildIndices</code>. If this is a removal event the
		''' returned objects are no longer children of the parent node.
		''' </summary>
		''' <returns> an array of Object containing the children specified by
		'''         the event </returns>
		''' <seealso cref= #getPath </seealso>
		''' <seealso cref= #getChildIndices </seealso>
		Public Overridable Property children As Object()
			Get
				If children IsNot Nothing Then
					Dim cCount As Integer = children.Length
					Dim retChildren As Object() = New Object(cCount - 1){}
    
					Array.Copy(children, 0, retChildren, 0, cCount)
					Return retChildren
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the values of the child indexes. If this is a removal event
		''' the indexes point to locations in the initial list where items
		''' were removed. If it is an insert, the indices point to locations
		''' in the final list where the items were added. For node changes,
		''' the indices point to the locations of the modified nodes.
		''' </summary>
		''' <returns> an array of <code>int</code> containing index locations for
		'''         the children specified by the event </returns>
		Public Overridable Property childIndices As Integer()
			Get
				If childIndices IsNot Nothing Then
					Dim cCount As Integer = childIndices.Length
					Dim retArray As Integer() = New Integer(cCount - 1){}
    
					Array.Copy(childIndices, 0, retArray, 0, cCount)
					Return retArray
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns a string that displays and identifies this object's
		''' properties.
		''' </summary>
		''' <returns> a String representation of this object </returns>
		Public Overrides Function ToString() As String
			Dim retBuffer As New StringBuilder

			retBuffer.Append(Me.GetType().name & " " & Convert.ToString(GetHashCode()))
			If path IsNot Nothing Then retBuffer.Append(" path " & path)
			If childIndices IsNot Nothing Then
				retBuffer.Append(" indices [ ")
				For counter As Integer = 0 To childIndices.Length - 1
					retBuffer.Append(Convert.ToString(childIndices(counter)) & " ")
				Next counter
				retBuffer.Append("]")
			End If
			If children IsNot Nothing Then
				retBuffer.Append(" children [ ")
				For counter As Integer = 0 To children.Length - 1
					retBuffer.Append(children(counter) & " ")
				Next counter
				retBuffer.Append("]")
			End If
			Return retBuffer.ToString()
		End Function
	End Class

End Namespace
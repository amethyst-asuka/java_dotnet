Imports System
Imports System.Collections

'
' * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.spi


	''' <summary>
	''' A node in a directed graph.  In addition to an arbitrary
	''' <code>Object</code> containing user data associated with the node,
	''' each node maintains a <code>Set</code>s of nodes which are pointed
	''' to by the current node (available from <code>getOutNodes</code>).
	''' The in-degree of the node (that is, number of nodes that point to
	''' the current node) may be queried.
	''' 
	''' </summary>
	<Serializable> _
	Friend Class DigraphNode
		Implements ICloneable

		''' <summary>
		''' The data associated with this node. </summary>
		Protected Friend data As Object

		''' <summary>
		''' A <code>Set</code> of neighboring nodes pointed to by this
		''' node.
		''' </summary>
		Protected Friend outNodes As java.util.Set = New HashSet

		''' <summary>
		''' The in-degree of the node. </summary>
		Protected Friend inDegree As Integer = 0

		''' <summary>
		''' A <code>Set</code> of neighboring nodes that point to this
		''' node.
		''' </summary>
		Private inNodes As java.util.Set = New HashSet

		Public Sub New(ByVal data As Object)
			Me.data = data
		End Sub

		''' <summary>
		''' Returns the <code>Object</code> referenced by this node. </summary>
		Public Overridable Property data As Object
			Get
				Return data
			End Get
		End Property

		''' <summary>
		''' Returns an <code>Iterator</code> containing the nodes pointed
		''' to by this node.
		''' </summary>
		Public Overridable Property outNodes As IEnumerator
			Get
				Return outNodes.GetEnumerator()
			End Get
		End Property

		''' <summary>
		''' Adds a directed edge to the graph.  The outNodes list of this
		''' node is updated and the in-degree of the other node is incremented.
		''' </summary>
		''' <param name="node"> a <code>DigraphNode</code>.
		''' </param>
		''' <returns> <code>true</code> if the node was not previously the
		''' target of an edge. </returns>
		Public Overridable Function addEdge(ByVal node As DigraphNode) As Boolean
			If outNodes.contains(node) Then Return False

			outNodes.add(node)
			node.inNodes.add(Me)
			node.incrementInDegree()
			Return True
		End Function

		''' <summary>
		''' Returns <code>true</code> if an edge exists between this node
		''' and the given node.
		''' </summary>
		''' <param name="node"> a <code>DigraphNode</code>.
		''' </param>
		''' <returns> <code>true</code> if the node is the target of an edge. </returns>
		Public Overridable Function hasEdge(ByVal node As DigraphNode) As Boolean
			Return outNodes.contains(node)
		End Function

		''' <summary>
		''' Removes a directed edge from the graph.  The outNodes list of this
		''' node is updated and the in-degree of the other node is decremented.
		''' </summary>
		''' <returns> <code>true</code> if the node was previously the target
		''' of an edge. </returns>
		Public Overridable Function removeEdge(ByVal node As DigraphNode) As Boolean
			If Not outNodes.contains(node) Then Return False

			outNodes.remove(node)
			node.inNodes.remove(Me)
			node.decrementInDegree()
			Return True
		End Function

		''' <summary>
		''' Removes this node from the graph, updating neighboring nodes
		''' appropriately.
		''' </summary>
		Public Overridable Sub dispose()
			Dim inNodesArray As Object() = inNodes.ToArray()
			For i As Integer = 0 To inNodesArray.Length - 1
				Dim node As DigraphNode = CType(inNodesArray(i), DigraphNode)
				node.removeEdge(Me)
			Next i

			Dim outNodesArray As Object() = outNodes.ToArray()
			For i As Integer = 0 To outNodesArray.Length - 1
				Dim node As DigraphNode = CType(outNodesArray(i), DigraphNode)
				removeEdge(node)
			Next i
		End Sub

		''' <summary>
		''' Returns the in-degree of this node. </summary>
		Public Overridable Property inDegree As Integer
			Get
				Return inDegree
			End Get
		End Property

		''' <summary>
		''' Increments the in-degree of this node. </summary>
		Private Sub incrementInDegree()
			inDegree += 1
		End Sub

		''' <summary>
		''' Decrements the in-degree of this node. </summary>
		Private Sub decrementInDegree()
			inDegree -= 1
		End Sub
	End Class

End Namespace
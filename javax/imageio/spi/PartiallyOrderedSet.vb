Imports Microsoft.VisualBasic
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
	''' A set of <code>Object</code>s with pairwise orderings between them.
	''' The <code>iterator</code> method provides the elements in
	''' topologically sorted order.  Elements participating in a cycle
	''' are not returned.
	''' 
	''' Unlike the <code>SortedSet</code> and <code>SortedMap</code>
	''' interfaces, which require their elements to implement the
	''' <code>Comparable</code> interface, this class receives ordering
	''' information via its <code>setOrdering</code> and
	''' <code>unsetPreference</code> methods.  This difference is due to
	''' the fact that the relevant ordering between elements is unlikely to
	''' be inherent in the elements themselves; rather, it is set
	''' dynamically accoring to application policy.  For example, in a
	''' service provider registry situation, an application might allow the
	''' user to set a preference order for service provider objects
	''' supplied by a trusted vendor over those supplied by another.
	''' 
	''' </summary>
	Friend Class PartiallyOrderedSet
		Inherits java.util.AbstractSet

		' The topological sort (roughly) follows the algorithm described in
		' Horowitz and Sahni, _Fundamentals of Data Structures_ (1976),
		' p. 315.

		' Maps Objects to DigraphNodes that contain them
		Private poNodes As IDictionary = New Hashtable

		' The set of Objects
		Private nodes As IDictionary.KeyCollection = poNodes.Keys

		''' <summary>
		''' Constructs a <code>PartiallyOrderedSet</code>.
		''' </summary>
		Public Sub New()
		End Sub

		Public Overridable Function size() As Integer
			Return nodes.size()
		End Function

		Public Overridable Function contains(ByVal o As Object) As Boolean
			Return nodes.contains(o)
		End Function

		''' <summary>
		''' Returns an iterator over the elements contained in this
		''' collection, with an ordering that respects the orderings set
		''' by the <code>setOrdering</code> method.
		''' </summary>
		Public Overridable Function [iterator]() As IEnumerator
			Return New PartialOrderIterator(poNodes.Values.GetEnumerator())
		End Function

		''' <summary>
		''' Adds an <code>Object</code> to this
		''' <code>PartiallyOrderedSet</code>.
		''' </summary>
		Public Overridable Function add(ByVal o As Object) As Boolean
			If nodes.contains(o) Then Return False

			Dim node As New DigraphNode(o)
			poNodes(o) = node
			Return True
		End Function

		''' <summary>
		''' Removes an <code>Object</code> from this
		''' <code>PartiallyOrderedSet</code>.
		''' </summary>
		Public Overridable Function remove(ByVal o As Object) As Boolean
			Dim node As DigraphNode = CType(poNodes(o), DigraphNode)
			If node Is Nothing Then Return False

			poNodes.Remove(o)
			node.Dispose()
			Return True
		End Function

		Public Overridable Sub clear()
			poNodes.Clear()
		End Sub

		''' <summary>
		''' Sets an ordering between two nodes.  When an iterator is
		''' requested, the first node will appear earlier in the
		''' sequence than the second node.  If a prior ordering existed
		''' between the nodes in the opposite order, it is removed.
		''' </summary>
		''' <returns> <code>true</code> if no prior ordering existed
		''' between the nodes, <code>false</code>otherwise. </returns>
		Public Overridable Function setOrdering(ByVal first As Object, ByVal second As Object) As Boolean
			Dim firstPONode As DigraphNode = CType(poNodes(first), DigraphNode)
			Dim secondPONode As DigraphNode = CType(poNodes(second), DigraphNode)

			secondPONode.removeEdge(firstPONode)
			Return firstPONode.addEdge(secondPONode)
		End Function

		''' <summary>
		''' Removes any ordering between two nodes.
		''' </summary>
		''' <returns> true if a prior prefence existed between the nodes. </returns>
		Public Overridable Function unsetOrdering(ByVal first As Object, ByVal second As Object) As Boolean
			Dim firstPONode As DigraphNode = CType(poNodes(first), DigraphNode)
			Dim secondPONode As DigraphNode = CType(poNodes(second), DigraphNode)

			Return firstPONode.removeEdge(secondPONode) OrElse secondPONode.removeEdge(firstPONode)
		End Function

		''' <summary>
		''' Returns <code>true</code> if an ordering exists between two
		''' nodes.
		''' </summary>
		Public Overridable Function hasOrdering(ByVal preferred As Object, ByVal other As Object) As Boolean
			Dim preferredPONode As DigraphNode = CType(poNodes(preferred), DigraphNode)
			Dim otherPONode As DigraphNode = CType(poNodes(other), DigraphNode)

			Return preferredPONode.hasEdge(otherPONode)
		End Function
	End Class

	Friend Class PartialOrderIterator
		Implements IEnumerator

		Friend zeroList As New LinkedList
		Friend inDegrees As IDictionary = New Hashtable ' DigraphNode -> Integer

		Public Sub New(ByVal iter As IEnumerator)
			' Initialize scratch in-degree values, zero list
			Do While iter.hasNext()
				Dim node As DigraphNode = CType(iter.next(), DigraphNode)
				Dim inDegree As Integer = node.inDegree
				inDegrees(node) = New Integer?(inDegree)

				' Add nodes with zero in-degree to the zero list
				If inDegree = 0 Then zeroList.AddLast(node)
			Loop
		End Sub

		Public Overridable Function hasNext() As Boolean
			Return zeroList.Count > 0
		End Function

		Public Overridable Function [next]() As Object
			Dim first As DigraphNode = CType(zeroList.RemoveFirst(), DigraphNode)

			' For each out node of the output node, decrement its in-degree
			Dim outNodes As IEnumerator = first.outNodes
			Do While outNodes.hasNext()
				Dim node As DigraphNode = CType(outNodes.next(), DigraphNode)
				Dim inDegree As Integer = CInt(Fix(inDegrees(node))) - 1
				inDegrees(node) = New Integer?(inDegree)

				' If the in-degree has fallen to 0, place the node on the list
				If inDegree = 0 Then zeroList.AddLast(node)
			Loop

			Return first.data
		End Function

		Public Overridable Sub remove()
			Throw New System.NotSupportedException
		End Sub
	End Class

End Namespace
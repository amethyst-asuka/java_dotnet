Imports System
Imports System.Diagnostics
Imports System.Collections.Generic

'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.util.stream


	''' <summary>
	''' Factory methods for constructing implementations of <seealso cref="Node"/> and
	''' <seealso cref="Node.Builder"/> and their primitive specializations.  Fork/Join tasks
	''' for collecting output from a <seealso cref="PipelineHelper"/> to a <seealso cref="Node"/> and
	''' flattening <seealso cref="Node"/>s.
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class Nodes

		Private Sub New()
			Throw New [Error]("no instances")
		End Sub

		''' <summary>
		''' The maximum size of an array that can be allocated.
		''' </summary>
		Friend Shared ReadOnly MAX_ARRAY_SIZE As Long =  java.lang.[Integer].MAX_VALUE - 8

		' IllegalArgumentException messages
		Friend Const BAD_SIZE As String = "Stream size exceeds max array size"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared ReadOnly EMPTY_NODE As Node = New EmptyNode.OfRef
		Private Shared ReadOnly EMPTY_INT_NODE As Node.OfInt = New EmptyNode.OfInt
		Private Shared ReadOnly EMPTY_LONG_NODE As Node.OfLong = New EmptyNode.OfLong
		Private Shared ReadOnly EMPTY_DOUBLE_NODE As Node.OfDouble = New EmptyNode.OfDouble

		' General shape-based node creation methods

		''' <summary>
		''' Produces an empty node whose count is zero, has no children and no content.
		''' </summary>
		''' @param <T> the type of elements of the created node </param>
		''' <param name="shape"> the shape of the node to be created </param>
		''' <returns> an empty node. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function emptyNode(Of T)(  shape As StreamShape) As Node(Of T)
			Select Case shape
				Case StreamShape.REFERENCE
					Return CType(EMPTY_NODE, Node(Of T))
				Case StreamShape.INT_VALUE
					Return CType(EMPTY_INT_NODE, Node(Of T))
				Case StreamShape.LONG_VALUE
					Return CType(EMPTY_LONG_NODE, Node(Of T))
				Case StreamShape.DOUBLE_VALUE
					Return CType(EMPTY_DOUBLE_NODE, Node(Of T))
				Case Else
					Throw New IllegalStateException("Unknown shape " & shape)
			End Select
		End Function

		''' <summary>
		''' Produces a concatenated <seealso cref="Node"/> that has two or more children.
		''' <p>The count of the concatenated node is equal to the sum of the count
		''' of each child. Traversal of the concatenated node traverses the content
		''' of each child in encounter order of the list of children. Splitting a
		''' spliterator obtained from the concatenated node preserves the encounter
		''' order of the list of children.
		''' 
		''' <p>The result may be a concatenated node, the input sole node if the size
		''' of the list is 1, or an empty node.
		''' </summary>
		''' @param <T> the type of elements of the concatenated node </param>
		''' <param name="shape"> the shape of the concatenated node to be created </param>
		''' <param name="left"> the left input node </param>
		''' <param name="right"> the right input node </param>
		''' <returns> a {@code Node} covering the elements of the input nodes </returns>
		''' <exception cref="IllegalStateException"> if all <seealso cref="Node"/> elements of the list
		''' are an not instance of type supported by this factory. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function conc(Of T)(  shape As StreamShape,   left As Node(Of T),   right As Node(Of T)) As Node(Of T)
			Select Case shape
				Case StreamShape.REFERENCE
					Return New ConcNode(Of )(left, right)
				Case StreamShape.INT_VALUE
					Return CType(New ConcNode.OfInt(CType(left, Node.OfInt), CType(right, Node.OfInt)), Node(Of T))
				Case StreamShape.LONG_VALUE
					Return CType(New ConcNode.OfLong(CType(left, Node.OfLong), CType(right, Node.OfLong)), Node(Of T))
				Case StreamShape.DOUBLE_VALUE
					Return CType(New ConcNode.OfDouble(CType(left, Node.OfDouble), CType(right, Node.OfDouble)), Node(Of T))
				Case Else
					Throw New IllegalStateException("Unknown shape " & shape)
			End Select
		End Function

		' Reference-based node methods

		''' <summary>
		''' Produces a <seealso cref="Node"/> describing an array.
		''' 
		''' <p>The node will hold a reference to the array and will not make a copy.
		''' </summary>
		''' @param <T> the type of elements held by the node </param>
		''' <param name="array"> the array </param>
		''' <returns> a node holding an array </returns>
		Friend Shared Function node(Of T)(  array As T()) As Node(Of T)
			Return New ArrayNode(Of )(array)
		End Function

		''' <summary>
		''' Produces a <seealso cref="Node"/> describing a <seealso cref="Collection"/>.
		''' <p>
		''' The node will hold a reference to the collection and will not make a copy.
		''' </summary>
		''' @param <T> the type of elements held by the node </param>
		''' <param name="c"> the collection </param>
		''' <returns> a node holding a collection </returns>
		Friend Shared Function node(Of T)(  c As ICollection(Of T)) As Node(Of T)
			Return New CollectionNode(Of )(c)
		End Function

		''' <summary>
		''' Produces a <seealso cref="Node.Builder"/>.
		''' </summary>
		''' <param name="exactSizeIfKnown"> -1 if a variable size builder is requested,
		''' otherwise the exact capacity desired.  A fixed capacity builder will
		''' fail if the wrong number of elements are added to the builder. </param>
		''' <param name="generator"> the array factory </param>
		''' @param <T> the type of elements of the node builder </param>
		''' <returns> a {@code Node.Builder} </returns>
		Friend Shared Function builder(Of T)(  exactSizeIfKnown As Long,   generator As java.util.function.IntFunction(Of T())) As Node.Builder(Of T)
			Return If(exactSizeIfKnown >= 0 AndAlso exactSizeIfKnown < MAX_ARRAY_SIZE, New FixedNodeBuilder(Of )(exactSizeIfKnown, generator), builder())
		End Function

		''' <summary>
		''' Produces a variable size @{link Node.Builder}.
		''' </summary>
		''' @param <T> the type of elements of the node builder </param>
		''' <returns> a {@code Node.Builder} </returns>
		Friend Shared Function builder(Of T)() As Node.Builder(Of T)
			Return New SpinedNodeBuilder(Of )
		End Function

		' Int nodes

		''' <summary>
		''' Produces a <seealso cref="Node.OfInt"/> describing an int[] array.
		''' 
		''' <p>The node will hold a reference to the array and will not make a copy.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <returns> a node holding an array </returns>
		Friend Shared Function node(  array As Integer()) As Node.OfInt
			Return New IntArrayNode(array)
		End Function

		''' <summary>
		''' Produces a <seealso cref="Node.Builder.OfInt"/>.
		''' </summary>
		''' <param name="exactSizeIfKnown"> -1 if a variable size builder is requested,
		''' otherwise the exact capacity desired.  A fixed capacity builder will
		''' fail if the wrong number of elements are added to the builder. </param>
		''' <returns> a {@code Node.Builder.OfInt} </returns>
		Friend Shared Function intBuilder(  exactSizeIfKnown As Long) As Node.Builder.OfInt
			Return If(exactSizeIfKnown >= 0 AndAlso exactSizeIfKnown < MAX_ARRAY_SIZE, New IntFixedNodeBuilder(exactSizeIfKnown), intBuilder())
		End Function

		''' <summary>
		''' Produces a variable size @{link Node.Builder.OfInt}.
		''' </summary>
		''' <returns> a {@code Node.Builder.OfInt} </returns>
		Friend Shared Function intBuilder() As Node.Builder.OfInt
			Return New IntSpinedNodeBuilder
		End Function

		' Long nodes

		''' <summary>
		''' Produces a <seealso cref="Node.OfLong"/> describing a long[] array.
		''' <p>
		''' The node will hold a reference to the array and will not make a copy.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <returns> a node holding an array </returns>
		Friend Shared Function node(  array As Long()) As Node.OfLong
			Return New LongArrayNode(array)
		End Function

		''' <summary>
		''' Produces a <seealso cref="Node.Builder.OfLong"/>.
		''' </summary>
		''' <param name="exactSizeIfKnown"> -1 if a variable size builder is requested,
		''' otherwise the exact capacity desired.  A fixed capacity builder will
		''' fail if the wrong number of elements are added to the builder. </param>
		''' <returns> a {@code Node.Builder.OfLong} </returns>
		Friend Shared Function longBuilder(  exactSizeIfKnown As Long) As Node.Builder.OfLong
			Return If(exactSizeIfKnown >= 0 AndAlso exactSizeIfKnown < MAX_ARRAY_SIZE, New LongFixedNodeBuilder(exactSizeIfKnown), longBuilder())
		End Function

		''' <summary>
		''' Produces a variable size @{link Node.Builder.OfLong}.
		''' </summary>
		''' <returns> a {@code Node.Builder.OfLong} </returns>
		Friend Shared Function longBuilder() As Node.Builder.OfLong
			Return New LongSpinedNodeBuilder
		End Function

		' Double nodes

		''' <summary>
		''' Produces a <seealso cref="Node.OfDouble"/> describing a double[] array.
		''' 
		''' <p>The node will hold a reference to the array and will not make a copy.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <returns> a node holding an array </returns>
		Friend Shared Function node(  array As Double()) As Node.OfDouble
			Return New DoubleArrayNode(array)
		End Function

		''' <summary>
		''' Produces a <seealso cref="Node.Builder.OfDouble"/>.
		''' </summary>
		''' <param name="exactSizeIfKnown"> -1 if a variable size builder is requested,
		''' otherwise the exact capacity desired.  A fixed capacity builder will
		''' fail if the wrong number of elements are added to the builder. </param>
		''' <returns> a {@code Node.Builder.OfDouble} </returns>
		Friend Shared Function doubleBuilder(  exactSizeIfKnown As Long) As Node.Builder.OfDouble
			Return If(exactSizeIfKnown >= 0 AndAlso exactSizeIfKnown < MAX_ARRAY_SIZE, New DoubleFixedNodeBuilder(exactSizeIfKnown), doubleBuilder())
		End Function

		''' <summary>
		''' Produces a variable size @{link Node.Builder.OfDouble}.
		''' </summary>
		''' <returns> a {@code Node.Builder.OfDouble} </returns>
		Friend Shared Function doubleBuilder() As Node.Builder.OfDouble
			Return New DoubleSpinedNodeBuilder
		End Function

		' Parallel evaluation of pipelines to nodes

		''' <summary>
		''' Collect, in parallel, elements output from a pipeline and describe those
		''' elements with a <seealso cref="Node"/>.
		''' 
		''' @implSpec
		''' If the exact size of the output from the pipeline is known and the source
		''' <seealso cref="Spliterator"/> has the <seealso cref="Spliterator#SUBSIZED"/> characteristic,
		''' then a flat <seealso cref="Node"/> will be returned whose content is an array,
		''' since the size is known the array can be constructed in advance and
		''' output elements can be placed into the array concurrently by leaf
		''' tasks at the correct offsets.  If the exact size is not known, output
		''' elements are collected into a conc-node whose shape mirrors that
		''' of the computation. This conc-node can then be flattened in
		''' parallel to produce a flat {@code Node} if desired.
		''' </summary>
		''' <param name="helper"> the pipeline helper describing the pipeline </param>
		''' <param name="flattenTree"> whether a conc node should be flattened into a node
		'''                    describing an array before returning </param>
		''' <param name="generator"> the array generator </param>
		''' <returns> a <seealso cref="Node"/> describing the output elements </returns>
		Public Shared Function collect(Of P_IN, P_OUT)(  helper As PipelineHelper(Of P_OUT),   spliterator As java.util.Spliterator(Of P_IN),   flattenTree As Boolean,   generator As java.util.function.IntFunction(Of P_OUT())) As Node(Of P_OUT)
			Dim size As Long = helper.exactOutputSizeIfKnown(spliterator)
			If size >= 0 AndAlso spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED) Then
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Dim array As P_OUT() = generator.apply(CInt(size))
				CType(New SizedCollectorTask.OfRef(Of )(spliterator, helper, array), SizedCollectorTask.OfRef(Of )).invoke()
				Return node(array)
			Else
				Dim node As Node(Of P_OUT) = (New CollectorTask.OfRef(Of P_OUT)(helper, generator, spliterator)).invoke()
				Return If(flattenTree, flatten(node, generator), node)
			End If
		End Function

		''' <summary>
		''' Collect, in parallel, elements output from an int-valued pipeline and
		''' describe those elements with a <seealso cref="Node.OfInt"/>.
		''' 
		''' @implSpec
		''' If the exact size of the output from the pipeline is known and the source
		''' <seealso cref="Spliterator"/> has the <seealso cref="Spliterator#SUBSIZED"/> characteristic,
		''' then a flat <seealso cref="Node"/> will be returned whose content is an array,
		''' since the size is known the array can be constructed in advance and
		''' output elements can be placed into the array concurrently by leaf
		''' tasks at the correct offsets.  If the exact size is not known, output
		''' elements are collected into a conc-node whose shape mirrors that
		''' of the computation. This conc-node can then be flattened in
		''' parallel to produce a flat {@code Node.OfInt} if desired.
		''' </summary>
		''' @param <P_IN> the type of elements from the source Spliterator </param>
		''' <param name="helper"> the pipeline helper describing the pipeline </param>
		''' <param name="flattenTree"> whether a conc node should be flattened into a node
		'''                    describing an array before returning </param>
		''' <returns> a <seealso cref="Node.OfInt"/> describing the output elements </returns>
		Public Shared Function collectInt(Of P_IN)(  helper As PipelineHelper(Of Integer?),   spliterator As java.util.Spliterator(Of P_IN),   flattenTree As Boolean) As Node.OfInt
			Dim size As Long = helper.exactOutputSizeIfKnown(spliterator)
			If size >= 0 AndAlso spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED) Then
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Dim array As Integer() = New Integer(CInt(size) - 1){}
				CType(New SizedCollectorTask.OfInt(Of )(spliterator, helper, array), SizedCollectorTask.OfInt(Of )).invoke()
				Return node(array)
			Else
				Dim node As Node.OfInt = (New CollectorTask.OfInt(Of )(helper, spliterator)).invoke()
				Return If(flattenTree, flattenInt(node), node)
			End If
		End Function

		''' <summary>
		''' Collect, in parallel, elements output from a long-valued pipeline and
		''' describe those elements with a <seealso cref="Node.OfLong"/>.
		''' 
		''' @implSpec
		''' If the exact size of the output from the pipeline is known and the source
		''' <seealso cref="Spliterator"/> has the <seealso cref="Spliterator#SUBSIZED"/> characteristic,
		''' then a flat <seealso cref="Node"/> will be returned whose content is an array,
		''' since the size is known the array can be constructed in advance and
		''' output elements can be placed into the array concurrently by leaf
		''' tasks at the correct offsets.  If the exact size is not known, output
		''' elements are collected into a conc-node whose shape mirrors that
		''' of the computation. This conc-node can then be flattened in
		''' parallel to produce a flat {@code Node.OfLong} if desired.
		''' </summary>
		''' @param <P_IN> the type of elements from the source Spliterator </param>
		''' <param name="helper"> the pipeline helper describing the pipeline </param>
		''' <param name="flattenTree"> whether a conc node should be flattened into a node
		'''                    describing an array before returning </param>
		''' <returns> a <seealso cref="Node.OfLong"/> describing the output elements </returns>
		Public Shared Function collectLong(Of P_IN)(  helper As PipelineHelper(Of Long?),   spliterator As java.util.Spliterator(Of P_IN),   flattenTree As Boolean) As Node.OfLong
			Dim size As Long = helper.exactOutputSizeIfKnown(spliterator)
			If size >= 0 AndAlso spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED) Then
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Dim array As Long() = New Long(CInt(size) - 1){}
				CType(New SizedCollectorTask.OfLong(Of )(spliterator, helper, array), SizedCollectorTask.OfLong(Of )).invoke()
				Return node(array)
			Else
				Dim node As Node.OfLong = (New CollectorTask.OfLong(Of )(helper, spliterator)).invoke()
				Return If(flattenTree, flattenLong(node), node)
			End If
		End Function

		''' <summary>
		''' Collect, in parallel, elements output from n double-valued pipeline and
		''' describe those elements with a <seealso cref="Node.OfDouble"/>.
		''' 
		''' @implSpec
		''' If the exact size of the output from the pipeline is known and the source
		''' <seealso cref="Spliterator"/> has the <seealso cref="Spliterator#SUBSIZED"/> characteristic,
		''' then a flat <seealso cref="Node"/> will be returned whose content is an array,
		''' since the size is known the array can be constructed in advance and
		''' output elements can be placed into the array concurrently by leaf
		''' tasks at the correct offsets.  If the exact size is not known, output
		''' elements are collected into a conc-node whose shape mirrors that
		''' of the computation. This conc-node can then be flattened in
		''' parallel to produce a flat {@code Node.OfDouble} if desired.
		''' </summary>
		''' @param <P_IN> the type of elements from the source Spliterator </param>
		''' <param name="helper"> the pipeline helper describing the pipeline </param>
		''' <param name="flattenTree"> whether a conc node should be flattened into a node
		'''                    describing an array before returning </param>
		''' <returns> a <seealso cref="Node.OfDouble"/> describing the output elements </returns>
		Public Shared Function collectDouble(Of P_IN)(  helper As PipelineHelper(Of Double?),   spliterator As java.util.Spliterator(Of P_IN),   flattenTree As Boolean) As Node.OfDouble
			Dim size As Long = helper.exactOutputSizeIfKnown(spliterator)
			If size >= 0 AndAlso spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED) Then
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Dim array As Double() = New Double(CInt(size) - 1){}
				CType(New SizedCollectorTask.OfDouble(Of )(spliterator, helper, array), SizedCollectorTask.OfDouble(Of )).invoke()
				Return node(array)
			Else
				Dim node As Node.OfDouble = (New CollectorTask.OfDouble(Of )(helper, spliterator)).invoke()
				Return If(flattenTree, flattenDouble(node), node)
			End If
		End Function

		' Parallel flattening of nodes

		''' <summary>
		''' Flatten, in parallel, a <seealso cref="Node"/>.  A flattened node is one that has
		''' no children.  If the node is already flat, it is simply returned.
		''' 
		''' @implSpec
		''' If a new node is to be created, the generator is used to create an array
		''' whose length is <seealso cref="Node#count()"/>.  Then the node tree is traversed
		''' and leaf node elements are placed in the array concurrently by leaf tasks
		''' at the correct offsets.
		''' </summary>
		''' @param <T> type of elements contained by the node </param>
		''' <param name="node"> the node to flatten </param>
		''' <param name="generator"> the array factory used to create array instances </param>
		''' <returns> a flat {@code Node} </returns>
		Public Shared Function flatten(Of T)(  node As Node(Of T),   generator As java.util.function.IntFunction(Of T())) As Node(Of T)
			If node.childCount > 0 Then
				Dim size As Long = node.count()
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Dim array As T() = generator.apply(CInt(size))
				CType(New ToArrayTask.OfRef(Of )(node, array, 0), ToArrayTask.OfRef(Of )).invoke()
				Return node(array)
			Else
				Return node
			End If
		End Function

		''' <summary>
		''' Flatten, in parallel, a <seealso cref="Node.OfInt"/>.  A flattened node is one that
		''' has no children.  If the node is already flat, it is simply returned.
		''' 
		''' @implSpec
		''' If a new node is to be created, a new int[] array is created whose length
		''' is <seealso cref="Node#count()"/>.  Then the node tree is traversed and leaf node
		''' elements are placed in the array concurrently by leaf tasks at the
		''' correct offsets.
		''' </summary>
		''' <param name="node"> the node to flatten </param>
		''' <returns> a flat {@code Node.OfInt} </returns>
		Public Shared Function flattenInt(  node As Node.OfInt) As Node.OfInt
			If node.childCount > 0 Then
				Dim size As Long = node.count()
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Dim array As Integer() = New Integer(CInt(size) - 1){}
				CType(New ToArrayTask.OfInt(node, array, 0), ToArrayTask.OfInt).invoke()
				Return node(array)
			Else
				Return node
			End If
		End Function

		''' <summary>
		''' Flatten, in parallel, a <seealso cref="Node.OfLong"/>.  A flattened node is one that
		''' has no children.  If the node is already flat, it is simply returned.
		''' 
		''' @implSpec
		''' If a new node is to be created, a new long[] array is created whose length
		''' is <seealso cref="Node#count()"/>.  Then the node tree is traversed and leaf node
		''' elements are placed in the array concurrently by leaf tasks at the
		''' correct offsets.
		''' </summary>
		''' <param name="node"> the node to flatten </param>
		''' <returns> a flat {@code Node.OfLong} </returns>
		Public Shared Function flattenLong(  node As Node.OfLong) As Node.OfLong
			If node.childCount > 0 Then
				Dim size As Long = node.count()
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Dim array As Long() = New Long(CInt(size) - 1){}
				CType(New ToArrayTask.OfLong(node, array, 0), ToArrayTask.OfLong).invoke()
				Return node(array)
			Else
				Return node
			End If
		End Function

		''' <summary>
		''' Flatten, in parallel, a <seealso cref="Node.OfDouble"/>.  A flattened node is one that
		''' has no children.  If the node is already flat, it is simply returned.
		''' 
		''' @implSpec
		''' If a new node is to be created, a new double[] array is created whose length
		''' is <seealso cref="Node#count()"/>.  Then the node tree is traversed and leaf node
		''' elements are placed in the array concurrently by leaf tasks at the
		''' correct offsets.
		''' </summary>
		''' <param name="node"> the node to flatten </param>
		''' <returns> a flat {@code Node.OfDouble} </returns>
		Public Shared Function flattenDouble(  node As Node.OfDouble) As Node.OfDouble
			If node.childCount > 0 Then
				Dim size As Long = node.count()
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Dim array As Double() = New Double(CInt(size) - 1){}
				CType(New ToArrayTask.OfDouble(node, array, 0), ToArrayTask.OfDouble).invoke()
				Return node(array)
			Else
				Return node
			End If
		End Function

		' Implementations

		Private MustInherit Class EmptyNode(Of T, T_ARR, T_CONS)
			Implements Node(Of T)

			Friend Sub New()
			End Sub

			Public Overrides Function asArray(  generator As java.util.function.IntFunction(Of T())) As T() Implements Node(Of T).asArray
				Return generator.apply(0)
			End Function

			Public Overridable Sub copyInto(  array As T_ARR,   offset As Integer) Implements Node(Of T).copyInto
			End Sub

			Public Overrides Function count() As Long Implements Node(Of T).count
				Return 0
			End Function

			Public Overridable Sub forEach(  consumer As T_CONS) Implements Node(Of T).forEach
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Private Class OfRef(Of T)
				Inherits EmptyNode(Of T, T(), java.util.function.Consumer(Of JavaToDotNetGenericWildcard))

				Private Sub New()
					MyBase.New()
				End Sub

				Public Overrides Function spliterator() As java.util.Spliterator(Of T)
					Return java.util.Spliterators.emptySpliterator()
				End Function
			End Class

			Private NotInheritable Class OfInt
				Inherits EmptyNode(Of Integer?, int(), java.util.function.IntConsumer)
				Implements Node.OfInt

				Friend Sub New() ' Avoid creation of special accessor
				End Sub

				Public Overrides Function spliterator() As java.util.Spliterator.OfInt
					Return java.util.Spliterators.emptyIntSpliterator()
				End Function

				Public Overrides Function asPrimitiveArray() As Integer()
					Return EMPTY_INT_ARRAY
				End Function
			End Class

			Private NotInheritable Class OfLong
				Inherits EmptyNode(Of Long?, long(), java.util.function.LongConsumer)
				Implements Node.OfLong

				Friend Sub New() ' Avoid creation of special accessor
				End Sub

				Public Overrides Function spliterator() As java.util.Spliterator.OfLong
					Return java.util.Spliterators.emptyLongSpliterator()
				End Function

				Public Overrides Function asPrimitiveArray() As Long()
					Return EMPTY_LONG_ARRAY
				End Function
			End Class

			Private NotInheritable Class OfDouble
				Inherits EmptyNode(Of Double?, double(), java.util.function.DoubleConsumer)
				Implements Node.OfDouble

				Friend Sub New() ' Avoid creation of special accessor
				End Sub

				Public Overrides Function spliterator() As java.util.Spliterator.OfDouble
					Return java.util.Spliterators.emptyDoubleSpliterator()
				End Function

				Public Overrides Function asPrimitiveArray() As Double()
					Return EMPTY_DOUBLE_ARRAY
				End Function
			End Class
		End Class

		''' <summary>
		''' Node class for a reference array </summary>
		Private Class ArrayNode(Of T)
			Implements Node(Of T)

			Friend ReadOnly array As T()
			Friend curSize As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Sub New(  size As Long,   generator As java.util.function.IntFunction(Of T()))
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Me.array = generator.apply(CInt(size))
				Me.curSize = 0
			End Sub

			Friend Sub New(  array As T())
				Me.array = array
				Me.curSize = array.Length
			End Sub

			' Node

			Public Overrides Function spliterator() As java.util.Spliterator(Of T) Implements Node(Of T).spliterator
				Return java.util.Arrays.spliterator(array, 0, curSize)
			End Function

			Public Overrides Sub copyInto(  dest As T(),   destOffset As Integer) Implements Node(Of T).copyInto
				Array.Copy(array, 0, dest, destOffset, curSize)
			End Sub

			Public Overrides Function asArray(  generator As java.util.function.IntFunction(Of T())) As T() Implements Node(Of T).asArray
				If array.Length = curSize Then
					Return array
				Else
					Throw New IllegalStateException
				End If
			End Function

			Public Overrides Function count() As Long Implements Node(Of T).count
				Return curSize
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(  consumer As java.util.function.Consumer(Of T1)) Implements Node(Of T).forEach
				For i As Integer = 0 To curSize - 1
					consumer.accept(array(i))
				Next i
			End Sub

			'

			Public Overrides Function ToString() As String
				Return String.Format("ArrayNode[{0:D}][{1}]", array.Length - curSize, java.util.Arrays.ToString(array))
			End Function
		End Class

		''' <summary>
		''' Node class for a Collection </summary>
		Private NotInheritable Class CollectionNode(Of T)
			Implements Node(Of T)

			Private ReadOnly c As ICollection(Of T)

			Friend Sub New(  c As ICollection(Of T))
				Me.c = c
			End Sub

			' Node

			Public Overrides Function spliterator() As java.util.Spliterator(Of T) Implements Node(Of T).spliterator
				Return c.stream().spliterator()
			End Function

			Public Overrides Sub copyInto(  array As T(),   offset As Integer) Implements Node(Of T).copyInto
				For Each t As T In c
					array(offset) = t
					offset += 1
				Next t
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Function asArray(  generator As java.util.function.IntFunction(Of T())) As T() Implements Node(Of T).asArray
				Return c.ToArray(generator.apply(c.Count))
			End Function

			Public Overrides Function count() As Long Implements Node(Of T).count
				Return c.Count
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(  consumer As java.util.function.Consumer(Of T1)) Implements Node(Of T).forEach
				c.forEach(consumer)
			End Sub

			'

			Public Overrides Function ToString() As String
				Return String.Format("CollectionNode[{0:D}][{1}]", c.Count, c)
			End Function
		End Class

		''' <summary>
		''' Node class for an internal node with two or more children
		''' </summary>
		Private MustInherit Class AbstractConcNode(Of T, T_NODE As Node(Of T))
			Implements Node(Of T)

			Protected Friend ReadOnly left As T_NODE
			Protected Friend ReadOnly right As T_NODE
			Private ReadOnly size As Long

			Friend Sub New(  left As T_NODE,   right As T_NODE)
				Me.left = left
				Me.right = right
				' The Node count will be required when the Node spliterator is
				' obtained and it is cheaper to aggressively calculate bottom up
				' as the tree is built rather than later on from the top down
				' traversing the tree
				Me.size = left.count() + right.count()
			End Sub

			Public  Overrides ReadOnly Property  childCount As Integer Implements Node(Of T).getChildCount
				Get
					Return 2
				End Get
			End Property

			Public Overrides Function getChild(  i As Integer) As T_NODE Implements Node(Of T).getChild
				If i = 0 Then Return left
				If i = 1 Then Return right
				Throw New IndexOutOfBoundsException
			End Function

			Public Overrides Function count() As Long Implements Node(Of T).count
				Return size
			End Function
		End Class

		Friend NotInheritable Class ConcNode(Of T)
			Inherits AbstractConcNode(Of T, Node(Of T))
			Implements Node(Of T)

			Friend Sub New(  left As Node(Of T),   right As Node(Of T))
				MyBase.New(left, right)
			End Sub

			Public Overrides Function spliterator() As java.util.Spliterator(Of T) Implements Node(Of T).spliterator
				Return New Nodes.InternalNodeSpliterator.OfRef(Of )(Me)
			End Function

			Public Overrides Sub copyInto(  array As T(),   offset As Integer) Implements Node(Of T).copyInto
				java.util.Objects.requireNonNull(array)
				left.copyInto(array, offset)
				' Cast to int is safe since it is the callers responsibility to
				' ensure that there is sufficient room in the array
				right.copyInto(array, offset + CInt(left.count()))
			End Sub

			Public Overrides Function asArray(  generator As java.util.function.IntFunction(Of T())) As T() Implements Node(Of T).asArray
				Dim size As Long = count()
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Dim array As T() = generator.apply(CInt(size))
				copyInto(array, 0)
				Return array
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(  consumer As java.util.function.Consumer(Of T1)) Implements Node(Of T).forEach
				left.forEach(consumer)
				right.forEach(consumer)
			End Sub

			Public Overrides Function truncate(  [from] As Long,   [to] As Long,   generator As java.util.function.IntFunction(Of T())) As Node(Of T) Implements Node(Of T).truncate
				If [from] = 0 AndAlso [to] = count() Then Return Me
				Dim leftCount As Long = left.count()
				If [from] >= leftCount Then
					Return right.truncate([from] - leftCount, [to] - leftCount, generator)
				ElseIf [to] <= leftCount Then
					Return left.truncate([from], [to], generator)
				Else
					Return Nodes.conc(shape, left.truncate([from], leftCount, generator), right.truncate(0, [to] - leftCount, generator))
				End If
			End Function

			Public Overrides Function ToString() As String
				If count() < 32 Then
					Return String.Format("ConcNode[{0}.{1}]", left, right)
				Else
					Return String.Format("ConcNode[size={0:D}]", count())
				End If
			End Function

			Private MustInherit Class OfPrimitive(Of E, T_CONS, T_ARR, T_SPLITR As java.util.Spliterator.OfPrimitive(Of E, T_CONS, T_SPLITR), T_NODE As Node.OfPrimitive(Of E, T_CONS, T_ARR, T_SPLITR, T_NODE))
				Inherits AbstractConcNode(Of E, T_NODE)
				Implements Node.OfPrimitive(Of E, T_CONS, T_ARR, T_SPLITR, T_NODE)

				Friend Sub New(  left As T_NODE,   right As T_NODE)
					MyBase.New(left, right)
				End Sub

				Public Overrides Sub forEach(  consumer As T_CONS)
					outerInstance.left.forEach(consumer)
					outerInstance.right.forEach(consumer)
				End Sub

				Public Overrides Sub copyInto(  array As T_ARR,   offset As Integer)
					outerInstance.left.copyInto(array, offset)
					' Cast to int is safe since it is the callers responsibility to
					' ensure that there is sufficient room in the array
					outerInstance.right.copyInto(array, offset + CInt(outerInstance.left.count()))
				End Sub

				Public Overrides Function asPrimitiveArray() As T_ARR
					Dim size As Long = outerInstance.count()
					If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
					Dim array As T_ARR = newArray(CInt(size))
					copyInto(array, 0)
					Return array
				End Function

				Public Overrides Function ToString() As String
					If outerInstance.count() < 32 Then
						Return String.Format("{0}[{1}.{2}]", Me.GetType().name, outerInstance.left, outerInstance.right)
					Else
						Return String.Format("{0}[size={1:D}]", Me.GetType().name, outerInstance.count())
					End If
				End Function
			End Class

			Friend NotInheritable Class OfInt
				Inherits ConcNode.OfPrimitive(Of Integer?, java.util.function.IntConsumer, int(), java.util.Spliterator.OfInt, Node.OfInt)
				Implements Node.OfInt

				Friend Sub New(  left As Node.OfInt,   right As Node.OfInt)
					MyBase.New(left, right)
				End Sub

				Public Overrides Function spliterator() As java.util.Spliterator.OfInt
					Return New InternalNodeSpliterator.OfInt(Me)
				End Function
			End Class

			Friend NotInheritable Class OfLong
				Inherits ConcNode.OfPrimitive(Of Long?, java.util.function.LongConsumer, long(), java.util.Spliterator.OfLong, Node.OfLong)
				Implements Node.OfLong

				Friend Sub New(  left As Node.OfLong,   right As Node.OfLong)
					MyBase.New(left, right)
				End Sub

				Public Overrides Function spliterator() As java.util.Spliterator.OfLong
					Return New InternalNodeSpliterator.OfLong(Me)
				End Function
			End Class

			Friend NotInheritable Class OfDouble
				Inherits ConcNode.OfPrimitive(Of Double?, java.util.function.DoubleConsumer, double(), java.util.Spliterator.OfDouble, Node.OfDouble)
				Implements Node.OfDouble

				Friend Sub New(  left As Node.OfDouble,   right As Node.OfDouble)
					MyBase.New(left, right)
				End Sub

				Public Overrides Function spliterator() As java.util.Spliterator.OfDouble
					Return New InternalNodeSpliterator.OfDouble(Me)
				End Function
			End Class
		End Class

		''' <summary>
		''' Abstract class for spliterator for all internal node classes </summary>
		Private MustInherit Class InternalNodeSpliterator(Of T, S As java.util.Spliterator(Of T), N As Node(Of T))
			Implements java.util.Spliterator(Of T)

			' Node we are pointing to
			' null if full traversal has occurred
			Friend curNode As N

			' next child of curNode to consume
			Friend curChildIndex As Integer

			' The spliterator of the curNode if that node is last and has no children.
			' This spliterator will be delegated to for splitting and traversing.
			' null if curNode has children
			Friend lastNodeSpliterator As S

			' spliterator used while traversing with tryAdvance
			' null if no partial traversal has occurred
			Friend tryAdvanceSpliterator As S

			' node stack used when traversing to search and find leaf nodes
			' null if no partial traversal has occurred
			Friend tryAdvanceStack As java.util.Deque(Of N)

			Friend Sub New(  curNode As N)
				Me.curNode = curNode
			End Sub

			''' <summary>
			''' Initiate a stack containing, in left-to-right order, the child nodes
			''' covered by this spliterator
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Protected Friend Function initStack() As java.util.Deque(Of N)
				' Bias size to the case where leaf nodes are close to this node
				' 8 is the minimum initial capacity for the ArrayDeque implementation
				Dim stack As java.util.Deque(Of N) = New java.util.ArrayDeque(Of N)(8)
				For i As Integer = curNode.childCount - 1 To curChildIndex Step -1
					stack.addFirst(CType(curNode.getChild(i), N))
				Next i
				Return stack
			End Function

			''' <summary>
			''' Depth first search, in left-to-right order, of the node tree, using
			''' an explicit stack, to find the next non-empty leaf node.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Protected Friend Function findNextLeafNode(  stack As java.util.Deque(Of N)) As N
				Dim n As N = Nothing
				n = stack.pollFirst()
				Do While n IsNot Nothing
					If n.childCount = 0 Then
						If n.count() > 0 Then Return n
					Else
						For i As Integer = n.childCount - 1 To 0 Step -1
							stack.addFirst(CType(n.getChild(i), N))
						Next i
					End If
					n = stack.pollFirst()
				Loop

				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Protected Friend Function initTryAdvance() As Boolean
				If curNode Is Nothing Then Return False

				If tryAdvanceSpliterator Is Nothing Then
					If lastNodeSpliterator Is Nothing Then
						' Initiate the node stack
						tryAdvanceStack = initStack()
						Dim leaf As N = findNextLeafNode(tryAdvanceStack)
						If leaf IsNot Nothing Then
							tryAdvanceSpliterator = CType(leaf.spliterator(), S)
						Else
							' A non-empty leaf node was not found
							' No elements to traverse
							curNode = Nothing
							Return False
						End If
					Else
						tryAdvanceSpliterator = lastNodeSpliterator
					End If
				End If
				Return True
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Function trySplit() As S
				If curNode Is Nothing OrElse tryAdvanceSpliterator IsNot Nothing Then
					Return Nothing ' Cannot split if fully or partially traversed
				ElseIf lastNodeSpliterator IsNot Nothing Then
					Return CType(lastNodeSpliterator.trySplit(), S)
				ElseIf curChildIndex < curNode.childCount - 1 Then
						Dim tempVar As Integer = curChildIndex
				End If
						curChildIndex += 1
						Return CType(curNode.getChild(tempVar).spliterator(), S)
				Else
					curNode = CType(curNode.getChild(curChildIndex), N)
					If curNode.childCount = 0 Then
						lastNodeSpliterator = CType(curNode.spliterator(), S)
						Return CType(lastNodeSpliterator.trySplit(), S)
					Else
						curChildIndex = 0
							Dim tempVar2 As Integer = curChildIndex
							curChildIndex += 1
							Return CType(curNode.getChild(tempVar2).spliterator(), S)
					End If
				End If
			End Function

			Public Overrides Function estimateSize() As Long
				If curNode Is Nothing Then Return 0

				' Will not reflect the effects of partial traversal.
				' This is compliant with the specification
				If lastNodeSpliterator IsNot Nothing Then
					Return lastNodeSpliterator.estimateSize()
				Else
					Dim size As Long = 0
					For i As Integer = curChildIndex To curNode.childCount - 1
						size += curNode.getChild(i).count()
					Next i
					Return size
				End If
			End Function

			Public Overrides Function characteristics() As Integer
				Return java.util.Spliterator.SIZED
			End Function

			Private NotInheritable Class OfRef(Of T)
				Inherits InternalNodeSpliterator(Of T, java.util.Spliterator(Of T), Node(Of T))

				Friend Sub New(  curNode As Node(Of T))
					MyBase.New(curNode)
				End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overrides Function tryAdvance(Of T1)(  consumer As java.util.function.Consumer(Of T1)) As Boolean
					If Not initTryAdvance() Then Return False

					Dim hasNext As Boolean = tryAdvanceSpliterator.tryAdvance(consumer)
					If Not hasNext Then
						If lastNodeSpliterator Is Nothing Then
							' Advance to the spliterator of the next non-empty leaf node
							Dim leaf As Node(Of T) = findNextLeafNode(tryAdvanceStack)
							If leaf IsNot Nothing Then
								tryAdvanceSpliterator = leaf.spliterator()
								' Since the node is not-empty the spliterator can be advanced
								Return tryAdvanceSpliterator.tryAdvance(consumer)
							End If
						End If
						' No more elements to traverse
						curNode = Nothing
					End If
					Return hasNext
				End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overrides Sub forEachRemaining(Of T1)(  consumer As java.util.function.Consumer(Of T1))
					If curNode Is Nothing Then Return

					If tryAdvanceSpliterator Is Nothing Then
						If lastNodeSpliterator Is Nothing Then
							Dim stack As java.util.Deque(Of Node(Of T)) = initStack()
							Dim leaf As Node(Of T)
							leaf = findNextLeafNode(stack)
							Do While leaf IsNot Nothing
								leaf.forEach(consumer)
								leaf = findNextLeafNode(stack)
							Loop
							curNode = Nothing
						Else
							lastNodeSpliterator.forEachRemaining(consumer)
						End If
					Else
						Do While tryAdvance(consumer)
						Loop
					End If
				End Sub
			End Class

			Private MustInherit Class OfPrimitive(Of T, T_CONS, T_ARR, T_SPLITR As java.util.Spliterator.OfPrimitive(Of T, T_CONS, T_SPLITR), N As Node.OfPrimitive(Of T, T_CONS, T_ARR, T_SPLITR, N))
				Inherits InternalNodeSpliterator(Of T, T_SPLITR, N)
				Implements java.util.Spliterator.OfPrimitive(Of T, T_CONS, T_SPLITR)

				Friend Sub New(  cur As N)
					MyBase.New(cur)
				End Sub

				Public Overrides Function tryAdvance(  consumer As T_CONS) As Boolean
					If Not outerInstance.initTryAdvance() Then Return False

					Dim hasNext As Boolean = outerInstance.tryAdvanceSpliterator.tryAdvance(consumer)
					If Not hasNext Then
						If outerInstance.lastNodeSpliterator Is Nothing Then
							' Advance to the spliterator of the next non-empty leaf node
							Dim leaf As N = outerInstance.findNextLeafNode(outerInstance.tryAdvanceStack)
							If leaf IsNot Nothing Then
								outerInstance.tryAdvanceSpliterator = leaf.spliterator()
								' Since the node is not-empty the spliterator can be advanced
								Return outerInstance.tryAdvanceSpliterator.tryAdvance(consumer)
							End If
						End If
						' No more elements to traverse
						outerInstance.curNode = Nothing
					End If
					Return hasNext
				End Function

				Public Overrides Sub forEachRemaining(  consumer As T_CONS)
					If outerInstance.curNode Is Nothing Then Return

					If outerInstance.tryAdvanceSpliterator Is Nothing Then
						If outerInstance.lastNodeSpliterator Is Nothing Then
							Dim stack As java.util.Deque(Of N) = outerInstance.initStack()
							Dim leaf As N
							leaf = outerInstance.findNextLeafNode(stack)
							Do While leaf IsNot Nothing
								leaf.forEach(consumer)
								leaf = outerInstance.findNextLeafNode(stack)
							Loop
							outerInstance.curNode = Nothing
						Else
							outerInstance.lastNodeSpliterator.forEachRemaining(consumer)
						End If
					Else
						Do While tryAdvance(consumer)
						Loop
					End If
				End Sub
			End Class

			Private NotInheritable Class OfInt
				Inherits OfPrimitive(Of Integer?, java.util.function.IntConsumer, int(), java.util.Spliterator.OfInt, Node.OfInt)
				Implements java.util.Spliterator.OfInt

				Friend Sub New(  cur As Node.OfInt)
					MyBase.New(cur)
				End Sub
			End Class

			Private NotInheritable Class OfLong
				Inherits OfPrimitive(Of Long?, java.util.function.LongConsumer, long(), java.util.Spliterator.OfLong, Node.OfLong)
				Implements java.util.Spliterator.OfLong

				Friend Sub New(  cur As Node.OfLong)
					MyBase.New(cur)
				End Sub
			End Class

			Private NotInheritable Class OfDouble
				Inherits OfPrimitive(Of Double?, java.util.function.DoubleConsumer, double(), java.util.Spliterator.OfDouble, Node.OfDouble)
				Implements java.util.Spliterator.OfDouble

				Friend Sub New(  cur As Node.OfDouble)
					MyBase.New(cur)
				End Sub
			End Class
		End Class

		''' <summary>
		''' Fixed-sized builder class for reference nodes
		''' </summary>
		Private NotInheritable Class FixedNodeBuilder(Of T)
			Inherits ArrayNode(Of T)
			Implements Node.Builder(Of T)

			Friend Sub New(  size As Long,   generator As java.util.function.IntFunction(Of T()))
				MyBase.New(size, generator)
				Debug.Assert(size < MAX_ARRAY_SIZE)
			End Sub

			Public Overrides Function build() As Node(Of T)
				If curSize < array.length Then Throw New IllegalStateException(String.Format("Current size {0:D} is less than fixed size {1:D}", curSize, array.length))
				Return Me
			End Function

			Public Overrides Sub begin(  size As Long)
				If size <> array.length Then Throw New IllegalStateException(String.Format("Begin size {0:D} is not equal to fixed size {1:D}", size, array.length))
				curSize = 0
			End Sub

			Public Overrides Sub accept(  t As T)
				If curSize < array.length Then
					array(curSize) = t
					curSize += 1
				Else
					Throw New IllegalStateException(String.Format("Accept exceeded fixed size of {0:D}", array.length))
				End If
			End Sub

			Public Overrides Sub [end]()
				If curSize < array.length Then Throw New IllegalStateException(String.Format("End size {0:D} is less than fixed size {1:D}", curSize, array.length))
			End Sub

			Public Overrides Function ToString() As String
				Return String.Format("FixedNodeBuilder[{0:D}][{1}]", array.length - curSize, java.util.Arrays.ToString(array))
			End Function
		End Class

		''' <summary>
		''' Variable-sized builder class for reference nodes
		''' </summary>
		Private NotInheritable Class SpinedNodeBuilder(Of T)
			Inherits SpinedBuffer(Of T)
			Implements Node(Of T), Node.Builder(Of T)

			Private building As Boolean = False

			Friend Sub New() ' Avoid creation of special accessor
			End Sub

			Public Overrides Function spliterator() As java.util.Spliterator(Of T) Implements Node(Of T).spliterator
				Debug.Assert((Not building), "during building")
				Return MyBase.spliterator()
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(  consumer As java.util.function.Consumer(Of T1)) Implements Node(Of T).forEach
				Debug.Assert((Not building), "during building")
				MyBase.forEach(consumer)
			End Sub

			'
			Public Overrides Sub begin(  size As Long)
				Debug.Assert((Not building), "was already building")
				building = True
				clear()
				ensureCapacity(size)
			End Sub

			Public Overrides Sub accept(  t As T)
				Debug.Assert(building, "not building")
				MyBase.accept(t)
			End Sub

			Public Overrides Sub [end]()
				Debug.Assert(building, "was not building")
				building = False
				' @@@ check begin(size) and size
			End Sub

			Public Overrides Sub copyInto(  array As T(),   offset As Integer) Implements Node(Of T).copyInto
				Debug.Assert((Not building), "during building")
				MyBase.copyInto(array, offset)
			End Sub

			Public Overrides Function asArray(  arrayFactory As java.util.function.IntFunction(Of T())) As T() Implements Node(Of T).asArray
				Debug.Assert((Not building), "during building")
				Return MyBase.asArray(arrayFactory)
			End Function

			Public Overrides Function build() As Node(Of T) Implements Node(Of T).build
				Debug.Assert((Not building), "during building")
				Return Me
			End Function
		End Class

		'

		Private Shared ReadOnly EMPTY_INT_ARRAY As Integer() = New Integer(){}
		Private Shared ReadOnly EMPTY_LONG_ARRAY As Long() = New Long(){}
		Private Shared ReadOnly EMPTY_DOUBLE_ARRAY As Double() = New Double(){}

		Private Class IntArrayNode
			Implements Node.OfInt

			Friend ReadOnly array As Integer()
			Friend curSize As Integer

			Friend Sub New(  size As Long)
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Me.array = New Integer(CInt(size) - 1){}
				Me.curSize = 0
			End Sub

			Friend Sub New(  array As Integer())
				Me.array = array
				Me.curSize = array.Length
			End Sub

			' Node

			Public Overrides Function spliterator() As java.util.Spliterator.OfInt
				Return java.util.Arrays.spliterator(array, 0, curSize)
			End Function

			Public Overrides Function asPrimitiveArray() As Integer()
				If array.Length = curSize Then
					Return array
				Else
					Return java.util.Arrays.copyOf(array, curSize)
				End If
			End Function

			Public Overrides Sub copyInto(  dest As Integer(),   destOffset As Integer)
				Array.Copy(array, 0, dest, destOffset, curSize)
			End Sub

			Public Overrides Function count() As Long
				Return curSize
			End Function

			Public Overrides Sub forEach(  consumer As java.util.function.IntConsumer)
				For i As Integer = 0 To curSize - 1
					consumer.accept(array(i))
				Next i
			End Sub

			Public Overrides Function ToString() As String
				Return String.Format("IntArrayNode[{0:D}][{1}]", array.Length - curSize, java.util.Arrays.ToString(array))
			End Function
		End Class

		Private Class LongArrayNode
			Implements Node.OfLong

			Friend ReadOnly array As Long()
			Friend curSize As Integer

			Friend Sub New(  size As Long)
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Me.array = New Long(CInt(size) - 1){}
				Me.curSize = 0
			End Sub

			Friend Sub New(  array As Long())
				Me.array = array
				Me.curSize = array.Length
			End Sub

			Public Overrides Function spliterator() As java.util.Spliterator.OfLong
				Return java.util.Arrays.spliterator(array, 0, curSize)
			End Function

			Public Overrides Function asPrimitiveArray() As Long()
				If array.Length = curSize Then
					Return array
				Else
					Return java.util.Arrays.copyOf(array, curSize)
				End If
			End Function

			Public Overrides Sub copyInto(  dest As Long(),   destOffset As Integer)
				Array.Copy(array, 0, dest, destOffset, curSize)
			End Sub

			Public Overrides Function count() As Long
				Return curSize
			End Function

			Public Overrides Sub forEach(  consumer As java.util.function.LongConsumer)
				For i As Integer = 0 To curSize - 1
					consumer.accept(array(i))
				Next i
			End Sub

			Public Overrides Function ToString() As String
				Return String.Format("LongArrayNode[{0:D}][{1}]", array.Length - curSize, java.util.Arrays.ToString(array))
			End Function
		End Class

		Private Class DoubleArrayNode
			Implements Node.OfDouble

			Friend ReadOnly array As Double()
			Friend curSize As Integer

			Friend Sub New(  size As Long)
				If size >= MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(BAD_SIZE)
				Me.array = New Double(CInt(size) - 1){}
				Me.curSize = 0
			End Sub

			Friend Sub New(  array As Double())
				Me.array = array
				Me.curSize = array.Length
			End Sub

			Public Overrides Function spliterator() As java.util.Spliterator.OfDouble
				Return java.util.Arrays.spliterator(array, 0, curSize)
			End Function

			Public Overrides Function asPrimitiveArray() As Double()
				If array.Length = curSize Then
					Return array
				Else
					Return java.util.Arrays.copyOf(array, curSize)
				End If
			End Function

			Public Overrides Sub copyInto(  dest As Double(),   destOffset As Integer)
				Array.Copy(array, 0, dest, destOffset, curSize)
			End Sub

			Public Overrides Function count() As Long
				Return curSize
			End Function

			Public Overrides Sub forEach(  consumer As java.util.function.DoubleConsumer)
				For i As Integer = 0 To curSize - 1
					consumer.accept(array(i))
				Next i
			End Sub

			Public Overrides Function ToString() As String
				Return String.Format("DoubleArrayNode[{0:D}][{1}]", array.Length - curSize, java.util.Arrays.ToString(array))
			End Function
		End Class

		Private NotInheritable Class IntFixedNodeBuilder
			Inherits IntArrayNode
			Implements Node.Builder.OfInt

			Friend Sub New(  size As Long)
				MyBase.New(size)
				Debug.Assert(size < MAX_ARRAY_SIZE)
			End Sub

			Public Overrides Function build() As Node.OfInt
				If curSize < array.Length Then Throw New IllegalStateException(String.Format("Current size {0:D} is less than fixed size {1:D}", curSize, array.Length))

				Return Me
			End Function

			Public Overrides Sub begin(  size As Long)
				If size <> array.Length Then Throw New IllegalStateException(String.Format("Begin size {0:D} is not equal to fixed size {1:D}", size, array.Length))

				curSize = 0
			End Sub

			Public Overrides Sub accept(  i As Integer)
				If curSize < array.Length Then
					array(curSize) = i
					curSize += 1
				Else
					Throw New IllegalStateException(String.Format("Accept exceeded fixed size of {0:D}", array.Length))
				End If
			End Sub

			Public Overrides Sub [end]()
				If curSize < array.Length Then Throw New IllegalStateException(String.Format("End size {0:D} is less than fixed size {1:D}", curSize, array.Length))
			End Sub

			Public Overrides Function ToString() As String
				Return String.Format("IntFixedNodeBuilder[{0:D}][{1}]", array.Length - curSize, java.util.Arrays.ToString(array))
			End Function
		End Class

		Private NotInheritable Class LongFixedNodeBuilder
			Inherits LongArrayNode
			Implements Node.Builder.OfLong

			Friend Sub New(  size As Long)
				MyBase.New(size)
				Debug.Assert(size < MAX_ARRAY_SIZE)
			End Sub

			Public Overrides Function build() As Node.OfLong
				If curSize < array.Length Then Throw New IllegalStateException(String.Format("Current size {0:D} is less than fixed size {1:D}", curSize, array.Length))

				Return Me
			End Function

			Public Overrides Sub begin(  size As Long)
				If size <> array.Length Then Throw New IllegalStateException(String.Format("Begin size {0:D} is not equal to fixed size {1:D}", size, array.Length))

				curSize = 0
			End Sub

			Public Overrides Sub accept(  i As Long)
				If curSize < array.Length Then
					array(curSize) = i
					curSize += 1
				Else
					Throw New IllegalStateException(String.Format("Accept exceeded fixed size of {0:D}", array.Length))
				End If
			End Sub

			Public Overrides Sub [end]()
				If curSize < array.Length Then Throw New IllegalStateException(String.Format("End size {0:D} is less than fixed size {1:D}", curSize, array.Length))
			End Sub

			Public Overrides Function ToString() As String
				Return String.Format("LongFixedNodeBuilder[{0:D}][{1}]", array.Length - curSize, java.util.Arrays.ToString(array))
			End Function
		End Class

		Private NotInheritable Class DoubleFixedNodeBuilder
			Inherits DoubleArrayNode
			Implements Node.Builder.OfDouble

			Friend Sub New(  size As Long)
				MyBase.New(size)
				Debug.Assert(size < MAX_ARRAY_SIZE)
			End Sub

			Public Overrides Function build() As Node.OfDouble
				If curSize < array.Length Then Throw New IllegalStateException(String.Format("Current size {0:D} is less than fixed size {1:D}", curSize, array.Length))

				Return Me
			End Function

			Public Overrides Sub begin(  size As Long)
				If size <> array.Length Then Throw New IllegalStateException(String.Format("Begin size {0:D} is not equal to fixed size {1:D}", size, array.Length))

				curSize = 0
			End Sub

			Public Overrides Sub accept(  i As Double)
				If curSize < array.Length Then
					array(curSize) = i
					curSize += 1
				Else
					Throw New IllegalStateException(String.Format("Accept exceeded fixed size of {0:D}", array.Length))
				End If
			End Sub

			Public Overrides Sub [end]()
				If curSize < array.Length Then Throw New IllegalStateException(String.Format("End size {0:D} is less than fixed size {1:D}", curSize, array.Length))
			End Sub

			Public Overrides Function ToString() As String
				Return String.Format("DoubleFixedNodeBuilder[{0:D}][{1}]", array.Length - curSize, java.util.Arrays.ToString(array))
			End Function
		End Class

		Private NotInheritable Class IntSpinedNodeBuilder
			Inherits SpinedBuffer.OfInt
			Implements Node.OfInt, Node.Builder.OfInt

			Private building As Boolean = False

			Friend Sub New() ' Avoid creation of special accessor
			End Sub

			Public Overrides Function spliterator() As java.util.Spliterator.OfInt
				Debug.Assert((Not building), "during building")
				Return MyBase.spliterator()
			End Function

			Public Overrides Sub forEach(  consumer As java.util.function.IntConsumer)
				Debug.Assert((Not building), "during building")
				MyBase.forEach(consumer)
			End Sub

			'
			Public Overrides Sub begin(  size As Long)
				Debug.Assert((Not building), "was already building")
				building = True
				clear()
				ensureCapacity(size)
			End Sub

			Public Overrides Sub accept(  i As Integer)
				Debug.Assert(building, "not building")
				MyBase.accept(i)
			End Sub

			Public Overrides Sub [end]()
				Debug.Assert(building, "was not building")
				building = False
				' @@@ check begin(size) and size
			End Sub

			Public Overrides Sub copyInto(  array As Integer(),   offset As Integer)
				Debug.Assert((Not building), "during building")
				MyBase.copyInto(array, offset)
			End Sub

			Public Overrides Function asPrimitiveArray() As Integer()
				Debug.Assert((Not building), "during building")
				Return MyBase.asPrimitiveArray()
			End Function

			Public Overrides Function build() As Node.OfInt
				Debug.Assert((Not building), "during building")
				Return Me
			End Function
		End Class

		Private NotInheritable Class LongSpinedNodeBuilder
			Inherits SpinedBuffer.OfLong
			Implements Node.OfLong, Node.Builder.OfLong

			Private building As Boolean = False

			Friend Sub New() ' Avoid creation of special accessor
			End Sub

			Public Overrides Function spliterator() As java.util.Spliterator.OfLong
				Debug.Assert((Not building), "during building")
				Return MyBase.spliterator()
			End Function

			Public Overrides Sub forEach(  consumer As java.util.function.LongConsumer)
				Debug.Assert((Not building), "during building")
				MyBase.forEach(consumer)
			End Sub

			'
			Public Overrides Sub begin(  size As Long)
				Debug.Assert((Not building), "was already building")
				building = True
				clear()
				ensureCapacity(size)
			End Sub

			Public Overrides Sub accept(  i As Long)
				Debug.Assert(building, "not building")
				MyBase.accept(i)
			End Sub

			Public Overrides Sub [end]()
				Debug.Assert(building, "was not building")
				building = False
				' @@@ check begin(size) and size
			End Sub

			Public Overrides Sub copyInto(  array As Long(),   offset As Integer)
				Debug.Assert((Not building), "during building")
				MyBase.copyInto(array, offset)
			End Sub

			Public Overrides Function asPrimitiveArray() As Long()
				Debug.Assert((Not building), "during building")
				Return MyBase.asPrimitiveArray()
			End Function

			Public Overrides Function build() As Node.OfLong
				Debug.Assert((Not building), "during building")
				Return Me
			End Function
		End Class

		Private NotInheritable Class DoubleSpinedNodeBuilder
			Inherits SpinedBuffer.OfDouble
			Implements Node.OfDouble, Node.Builder.OfDouble

			Private building As Boolean = False

			Friend Sub New() ' Avoid creation of special accessor
			End Sub

			Public Overrides Function spliterator() As java.util.Spliterator.OfDouble
				Debug.Assert((Not building), "during building")
				Return MyBase.spliterator()
			End Function

			Public Overrides Sub forEach(  consumer As java.util.function.DoubleConsumer)
				Debug.Assert((Not building), "during building")
				MyBase.forEach(consumer)
			End Sub

			'
			Public Overrides Sub begin(  size As Long)
				Debug.Assert((Not building), "was already building")
				building = True
				clear()
				ensureCapacity(size)
			End Sub

			Public Overrides Sub accept(  i As Double)
				Debug.Assert(building, "not building")
				MyBase.accept(i)
			End Sub

			Public Overrides Sub [end]()
				Debug.Assert(building, "was not building")
				building = False
				' @@@ check begin(size) and size
			End Sub

			Public Overrides Sub copyInto(  array As Double(),   offset As Integer)
				Debug.Assert((Not building), "during building")
				MyBase.copyInto(array, offset)
			End Sub

			Public Overrides Function asPrimitiveArray() As Double()
				Debug.Assert((Not building), "during building")
				Return MyBase.asPrimitiveArray()
			End Function

			Public Overrides Function build() As Node.OfDouble
				Debug.Assert((Not building), "during building")
				Return Me
			End Function
		End Class

	'    
	'     * This and subclasses are not intended to be serializable
	'     
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private MustInherit Class SizedCollectorTask(Of P_IN, P_OUT, T_SINK As Sink(Of P_OUT), K As SizedCollectorTask(Of P_IN, P_OUT, T_SINK, K))
			Inherits java.util.concurrent.CountedCompleter(Of Void)
			Implements Sink(Of P_OUT)

			Protected Friend ReadOnly spliterator As java.util.Spliterator(Of P_IN)
			Protected Friend ReadOnly helper As PipelineHelper(Of P_OUT)
			Protected Friend ReadOnly targetSize As Long
			Protected Friend offset As Long
			Protected Friend length As Long
			' For Sink implementation
			Protected Friend index, fence As Integer

			Friend Sub New(  spliterator As java.util.Spliterator(Of P_IN),   helper As PipelineHelper(Of P_OUT),   arrayLength As Integer)
				Debug.Assert(spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED))
				Me.spliterator = spliterator
				Me.helper = helper
				Me.targetSize = AbstractTask.suggestTargetSize(spliterator.estimateSize())
				Me.offset = 0
				Me.length = arrayLength
			End Sub

			Friend Sub New(  parent As K,   spliterator As java.util.Spliterator(Of P_IN),   offset As Long,   length As Long,   arrayLength As Integer)
				MyBase.New(parent)
				Debug.Assert(spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED))
				Me.spliterator = spliterator
				Me.helper = parent.helper
				Me.targetSize = parent.targetSize
				Me.offset = offset
				Me.length = length

				If offset < 0 OrElse length < 0 OrElse (offset + length - 1 >= arrayLength) Then Throw New IllegalArgumentException(String.Format("offset and length interval [{0:D}, {1:D} + {2:D}) is not within array size interval [0, {3:D})", offset, offset, length, arrayLength))
			End Sub

			Public Overrides Sub compute()
				Dim task As SizedCollectorTask(Of P_IN, P_OUT, T_SINK, K) = Me
				Dim rightSplit As java.util.Spliterator(Of P_IN) = spliterator, leftSplit As java.util.Spliterator(Of P_IN)
				leftSplit = rightSplit.trySplit()
				Do While rightSplit.estimateSize() > task.targetSize AndAlso leftSplit IsNot Nothing
					task.pendingCount = 1
					Dim leftSplitSize As Long = leftSplit.estimateSize()
					task.makeChild(leftSplit, task.offset, leftSplitSize).fork()
					task = task.makeChild(rightSplit, task.offset + leftSplitSize, task.length - leftSplitSize)
					leftSplit = rightSplit.trySplit()
				Loop

				Debug.Assert(task.offset + task.length < MAX_ARRAY_SIZE)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim sink As T_SINK = CType(task, T_SINK)
				task.helper.wrapAndCopyInto(sink, rightSplit)
				task.propagateCompletion()
			End Sub

			Friend MustOverride Function makeChild(  spliterator As java.util.Spliterator(Of P_IN),   offset As Long,   size As Long) As K

			Public Overrides Sub begin(  size As Long) Implements Sink(Of P_OUT).begin
				If size > length Then Throw New IllegalStateException("size passed to Sink.begin exceeds array length")
				' Casts to int are safe since absolute size is verified to be within
				' bounds when the root concrete SizedCollectorTask is constructed
				' with the shared array
				index = CInt(offset)
				fence = index + CInt(length)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend NotInheritable Class OfRef(Of P_IN, P_OUT)
				Inherits SizedCollectorTask(Of P_IN, P_OUT, Sink(Of P_OUT), OfRef(Of P_IN, P_OUT))
				Implements Sink(Of P_OUT)

				Private ReadOnly array As P_OUT()

				Friend Sub New(  spliterator As java.util.Spliterator(Of P_IN),   helper As PipelineHelper(Of P_OUT),   array As P_OUT())
					MyBase.New(spliterator, helper, array.Length)
					Me.array = array
				End Sub

				Friend Sub New(  parent As OfRef(Of P_IN, P_OUT),   spliterator As java.util.Spliterator(Of P_IN),   offset As Long,   length As Long)
					MyBase.New(parent, spliterator, offset, length, parent.array.length)
					Me.array = parent.array
				End Sub

				Friend Overrides Function makeChild(  spliterator As java.util.Spliterator(Of P_IN),   offset As Long,   size As Long) As OfRef(Of P_IN, P_OUT)
					Return New OfRef(Of )(Me, spliterator, offset, size)
				End Function

				Public Overrides Sub accept(  value As P_OUT)
					If index >= fence Then Throw New IndexOutOfBoundsException(Convert.ToString(index))
					array(index) = value
					index += 1
				End Sub
			End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend NotInheritable Class OfInt(Of P_IN)
				Inherits SizedCollectorTask(Of P_IN, Integer?, Sink.OfInt, OfInt(Of P_IN))
				Implements Sink.OfInt

				Private ReadOnly array As Integer()

				Friend Sub New(  spliterator As java.util.Spliterator(Of P_IN),   helper As PipelineHelper(Of Integer?),   array As Integer())
					MyBase.New(spliterator, helper, array.Length)
					Me.array = array
				End Sub

				Friend Sub New(  parent As SizedCollectorTask.OfInt(Of P_IN),   spliterator As java.util.Spliterator(Of P_IN),   offset As Long,   length As Long)
					MyBase.New(parent, spliterator, offset, length, parent.array.length)
					Me.array = parent.array
				End Sub

				Friend Overrides Function makeChild(  spliterator As java.util.Spliterator(Of P_IN),   offset As Long,   size As Long) As SizedCollectorTask.OfInt(Of P_IN)
					Return New SizedCollectorTask.OfInt(Of )(Me, spliterator, offset, size)
				End Function

				Public Overrides Sub accept(  value As Integer)
					If index >= fence Then Throw New IndexOutOfBoundsException(Convert.ToString(index))
					array(index) = value
					index += 1
				End Sub
			End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend NotInheritable Class OfLong(Of P_IN)
				Inherits SizedCollectorTask(Of P_IN, Long?, Sink.OfLong, OfLong(Of P_IN))
				Implements Sink.OfLong

				Private ReadOnly array As Long()

				Friend Sub New(  spliterator As java.util.Spliterator(Of P_IN),   helper As PipelineHelper(Of Long?),   array As Long())
					MyBase.New(spliterator, helper, array.Length)
					Me.array = array
				End Sub

				Friend Sub New(  parent As SizedCollectorTask.OfLong(Of P_IN),   spliterator As java.util.Spliterator(Of P_IN),   offset As Long,   length As Long)
					MyBase.New(parent, spliterator, offset, length, parent.array.length)
					Me.array = parent.array
				End Sub

				Friend Overrides Function makeChild(  spliterator As java.util.Spliterator(Of P_IN),   offset As Long,   size As Long) As SizedCollectorTask.OfLong(Of P_IN)
					Return New SizedCollectorTask.OfLong(Of )(Me, spliterator, offset, size)
				End Function

				Public Overrides Sub accept(  value As Long)
					If index >= fence Then Throw New IndexOutOfBoundsException(Convert.ToString(index))
					array(index) = value
					index += 1
				End Sub
			End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend NotInheritable Class OfDouble(Of P_IN)
				Inherits SizedCollectorTask(Of P_IN, Double?, Sink.OfDouble, OfDouble(Of P_IN))
				Implements Sink.OfDouble

				Private ReadOnly array As Double()

				Friend Sub New(  spliterator As java.util.Spliterator(Of P_IN),   helper As PipelineHelper(Of Double?),   array As Double())
					MyBase.New(spliterator, helper, array.Length)
					Me.array = array
				End Sub

				Friend Sub New(  parent As SizedCollectorTask.OfDouble(Of P_IN),   spliterator As java.util.Spliterator(Of P_IN),   offset As Long,   length As Long)
					MyBase.New(parent, spliterator, offset, length, parent.array.length)
					Me.array = parent.array
				End Sub

				Friend Overrides Function makeChild(  spliterator As java.util.Spliterator(Of P_IN),   offset As Long,   size As Long) As SizedCollectorTask.OfDouble(Of P_IN)
					Return New SizedCollectorTask.OfDouble(Of )(Me, spliterator, offset, size)
				End Function

				Public Overrides Sub accept(  value As Double)
					If index >= fence Then Throw New IndexOutOfBoundsException(Convert.ToString(index))
					array(index) = value
					index += 1
				End Sub
			End Class
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private MustInherit Class ToArrayTask(Of T, T_NODE As Node(Of T), K As ToArrayTask(Of T, T_NODE, K))
			Inherits java.util.concurrent.CountedCompleter(Of Void)

			Protected Friend ReadOnly node As T_NODE
			Protected Friend ReadOnly offset As Integer

			Friend Sub New(  node As T_NODE,   offset As Integer)
				Me.node = node
				Me.offset = offset
			End Sub

			Friend Sub New(  parent As K,   node As T_NODE,   offset As Integer)
				MyBase.New(parent)
				Me.node = node
				Me.offset = offset
			End Sub

			Friend MustOverride Sub copyNodeToArray()

			Friend MustOverride Function makeChild(  childIndex As Integer,   offset As Integer) As K

			Public Overrides Sub compute()
				Dim task As ToArrayTask(Of T, T_NODE, K) = Me
				Do
					If task.node.childCount = 0 Then
						task.copyNodeToArray()
						task.propagateCompletion()
						Return
					Else
						task.pendingCount = task.node.childCount - 1

						Dim size As Integer = 0
						Dim i As Integer = 0
						Do While i < task.node.childCount - 1
							Dim leftTask As K = task.makeChild(i, task.offset + size)
							size += leftTask.node.count()
							leftTask.fork()
							i += 1
						Loop
						task = task.makeChild(i, task.offset + size)
					End If
				Loop
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private NotInheritable Class OfRef(Of T)
				Inherits ToArrayTask(Of T, Node(Of T), OfRef(Of T))

				Private ReadOnly array As T()

				Private Sub New(  node As Node(Of T),   array As T(),   offset As Integer)
					MyBase.New(node, offset)
					Me.array = array
				End Sub

				Private Sub New(  parent As OfRef(Of T),   node As Node(Of T),   offset As Integer)
					MyBase.New(parent, node, offset)
					Me.array = parent.array
				End Sub

				Friend Overrides Function makeChild(  childIndex As Integer,   offset As Integer) As OfRef(Of T)
					Return New OfRef(Of )(Me, node.getChild(childIndex), offset)
				End Function

				Friend Overrides Sub copyNodeToArray()
					node.copyInto(array, offset)
				End Sub
			End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private Class OfPrimitive(Of T, T_CONS, T_ARR, T_SPLITR As java.util.Spliterator.OfPrimitive(Of T, T_CONS, T_SPLITR), T_NODE As Node.OfPrimitive(Of T, T_CONS, T_ARR, T_SPLITR, T_NODE))
				Inherits ToArrayTask(Of T, T_NODE, OfPrimitive(Of T, T_CONS, T_ARR, T_SPLITR, T_NODE))

				Private ReadOnly array As T_ARR

				Private Sub New(  node As T_NODE,   array As T_ARR,   offset As Integer)
					MyBase.New(node, offset)
					Me.array = array
				End Sub

				Private Sub New(  parent As OfPrimitive(Of T, T_CONS, T_ARR, T_SPLITR, T_NODE),   node As T_NODE,   offset As Integer)
					MyBase.New(parent, node, offset)
					Me.array = parent.array
				End Sub

				Friend Overrides Function makeChild(  childIndex As Integer,   offset As Integer) As OfPrimitive(Of T, T_CONS, T_ARR, T_SPLITR, T_NODE)
					Return New OfPrimitive(Of )(Me, outerInstance.node.getChild(childIndex), offset)
				End Function

				Friend Overrides Sub copyNodeToArray()
					outerInstance.node.copyInto(array, outerInstance.offset)
				End Sub
			End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private NotInheritable Class OfInt
				Inherits OfPrimitive(Of Integer?, java.util.function.IntConsumer, int(), java.util.Spliterator.OfInt, Node.OfInt)

				Private Sub New(  node As Node.OfInt,   array As Integer(),   offset As Integer)
					MyBase.New(node, array, offset)
				End Sub
			End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private NotInheritable Class OfLong
				Inherits OfPrimitive(Of Long?, java.util.function.LongConsumer, long(), java.util.Spliterator.OfLong, Node.OfLong)

				Private Sub New(  node As Node.OfLong,   array As Long(),   offset As Integer)
					MyBase.New(node, array, offset)
				End Sub
			End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private NotInheritable Class OfDouble
				Inherits OfPrimitive(Of Double?, java.util.function.DoubleConsumer, double(), java.util.Spliterator.OfDouble, Node.OfDouble)

				Private Sub New(  node As Node.OfDouble,   array As Double(),   offset As Integer)
					MyBase.New(node, array, offset)
				End Sub
			End Class
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Class CollectorTask(Of P_IN, P_OUT, T_NODE As Node(Of P_OUT), T_BUILDER As Node.Builder(Of P_OUT))
			Inherits AbstractTask(Of P_IN, P_OUT, T_NODE, CollectorTask(Of P_IN, P_OUT, T_NODE, T_BUILDER))

			Protected Friend ReadOnly helper As PipelineHelper(Of P_OUT)
			Protected Friend ReadOnly builderFactory As java.util.function.LongFunction(Of T_BUILDER)
			Protected Friend ReadOnly concFactory As java.util.function.BinaryOperator(Of T_NODE)

			Friend Sub New(  helper As PipelineHelper(Of P_OUT),   spliterator As java.util.Spliterator(Of P_IN),   builderFactory As java.util.function.LongFunction(Of T_BUILDER),   concFactory As java.util.function.BinaryOperator(Of T_NODE))
				MyBase.New(helper, spliterator)
				Me.helper = helper
				Me.builderFactory = builderFactory
				Me.concFactory = concFactory
			End Sub

			Friend Sub New(  parent As CollectorTask(Of P_IN, P_OUT, T_NODE, T_BUILDER),   spliterator As java.util.Spliterator(Of P_IN))
				MyBase.New(parent, spliterator)
				helper = parent.helper
				builderFactory = parent.builderFactory
				concFactory = parent.concFactory
			End Sub

			Protected Friend Overrides Function makeChild(  spliterator As java.util.Spliterator(Of P_IN)) As CollectorTask(Of P_IN, P_OUT, T_NODE, T_BUILDER)
				Return New CollectorTask(Of )(Me, spliterator)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Protected Friend Overrides Function doLeaf() As T_NODE
				Dim builder As T_BUILDER = builderFactory.apply(helper.exactOutputSizeIfKnown(spliterator))
				Return CType(helper.wrapAndCopyInto(builder, spliterator).build(), T_NODE)
			End Function

			Public Overrides Sub onCompletion(Of T1)(  caller As java.util.concurrent.CountedCompleter(Of T1))
				If Not leaf Then localResult = concFactory.apply(leftChild.localResult, rightChild.localResult)
				MyBase.onCompletion(caller)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private NotInheritable Class OfRef(Of P_IN, P_OUT)
				Inherits CollectorTask(Of P_IN, P_OUT, Node(Of P_OUT), Node.Builder(Of P_OUT))

				Friend Sub New(  helper As PipelineHelper(Of P_OUT),   generator As java.util.function.IntFunction(Of P_OUT()),   spliterator As java.util.Spliterator(Of P_IN))
					MyBase.New(helper, spliterator, s -> builder(s, generator), ConcNode::New)
				End Sub
			End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private NotInheritable Class OfInt(Of P_IN)
				Inherits CollectorTask(Of P_IN, Integer?, Node.OfInt, Node.Builder.OfInt)

				Friend Sub New(  helper As PipelineHelper(Of Integer?),   spliterator As java.util.Spliterator(Of P_IN))
					MyBase.New(helper, spliterator, Nodes::intBuilder, ConcNode.OfInt::New)
				End Sub
			End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private NotInheritable Class OfLong(Of P_IN)
				Inherits CollectorTask(Of P_IN, Long?, Node.OfLong, Node.Builder.OfLong)

				Friend Sub New(  helper As PipelineHelper(Of Long?),   spliterator As java.util.Spliterator(Of P_IN))
					MyBase.New(helper, spliterator, Nodes::longBuilder, ConcNode.OfLong::New)
				End Sub
			End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private NotInheritable Class OfDouble(Of P_IN)
				Inherits CollectorTask(Of P_IN, Double?, Node.OfDouble, Node.Builder.OfDouble)

				Friend Sub New(  helper As PipelineHelper(Of Double?),   spliterator As java.util.Spliterator(Of P_IN))
					MyBase.New(helper, spliterator, Nodes::doubleBuilder, ConcNode.OfDouble::New)
				End Sub
			End Class
		End Class
	End Class

End Namespace
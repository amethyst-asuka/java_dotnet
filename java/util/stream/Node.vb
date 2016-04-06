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
	''' An immutable container for describing an ordered sequence of elements of some
	''' type {@code T}.
	''' 
	''' <p>A {@code Node} contains a fixed number of elements, which can be accessed
	''' via the <seealso cref="#count"/>, <seealso cref="#spliterator"/>, <seealso cref="#forEach"/>,
	''' <seealso cref="#asArray"/>, or <seealso cref="#copyInto"/> methods.  A {@code Node} may have zero
	''' or more child {@code Node}s; if it has no children (accessed via
	''' <seealso cref="#getChildCount"/> and <seealso cref="#getChild(int)"/>, it is considered <em>flat
	''' </em> or a <em>leaf</em>; if it has children, it is considered an
	''' <em>internal</em> node.  The size of an internal node is the sum of sizes of
	''' its children.
	''' 
	''' @apiNote
	''' <p>A {@code Node} typically does not store the elements directly, but instead
	''' mediates access to one or more existing (effectively immutable) data
	''' structures such as a {@code Collection}, array, or a set of other
	''' {@code Node}s.  Commonly {@code Node}s are formed into a tree whose shape
	''' corresponds to the computation tree that produced the elements that are
	''' contained in the leaf nodes.  The use of {@code Node} within the stream
	''' framework is largely to avoid copying data unnecessarily during parallel
	''' operations.
	''' </summary>
	''' @param <T> the type of elements.
	''' @since 1.8 </param>
	Friend Interface Node(Of T)

		''' <summary>
		''' Returns a <seealso cref="Spliterator"/> describing the elements contained in this
		''' {@code Node}.
		''' </summary>
		''' <returns> a {@code Spliterator} describing the elements contained in this
		'''         {@code Node} </returns>
		Function spliterator() As java.util.Spliterator(Of T)

		''' <summary>
		''' Traverses the elements of this node, and invoke the provided
		''' {@code Consumer} with each element.  Elements are provided in encounter
		''' order if the source for the {@code Node} has a defined encounter order.
		''' </summary>
		''' <param name="consumer"> a {@code Consumer} that is to be invoked with each
		'''        element in this {@code Node} </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Sub forEach(Of T1)(  consumer As java.util.function.Consumer(Of T1))

		''' <summary>
		''' Returns the number of child nodes of this node.
		''' 
		''' @implSpec The default implementation returns zero.
		''' </summary>
		''' <returns> the number of child nodes </returns>
		ReadOnly Property default childCount As Integer
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return 0;

		''' <summary>
		''' Retrieves the child {@code Node} at a given index.
		''' 
		''' @implSpec The default implementation always throws
		''' {@code IndexOutOfBoundsException}.
		''' </summary>
		''' <param name="i"> the index to the child node </param>
		''' <returns> the child node </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the index is less than 0 or greater
		'''         than or equal to the number of child nodes </exception>
		default Function getChild(  i As Integer) As Node(Of T)
			throw Function IndexOutOfBoundsException() As New

		''' <summary>
		''' Return a node describing a subsequence of the elements of this node,
		''' starting at the given inclusive start offset and ending at the given
		''' exclusive end offset.
		''' </summary>
		''' <param name="from"> The (inclusive) starting offset of elements to include, must
		'''             be in range 0..count(). </param>
		''' <param name="to"> The (exclusive) end offset of elements to include, must be
		'''           in range 0..count(). </param>
		''' <param name="generator"> A function to be used to create a new array, if needed,
		'''                  for reference nodes. </param>
		''' <returns> the truncated node </returns>
		default Function truncate(  [from] As Long,   [to] As Long,   generator As java.util.function.IntFunction(Of T())) As Node(Of T)
			Sub [New](from == 0 && to == count()    As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				Return Me;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			java.util.Spliterator(Of T) spliterator = spliterator();
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long size = to - from;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Node.Builder(Of T) nodeBuilder = Nodes.builder(size, generator);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			nodeBuilder.begin(size);
			Sub [New](Integer i = 0; i < from && spliterator.tryAdvance(e -> { }); i +=   1 As )
			Sub [New](Integer i = 0; (i < size) && spliterator.tryAdvance(nodeBuilder); i +=   1 As )
			Sub [New]()
			Function nodeBuilder.build() As [Return]

		''' <summary>
		''' Provides an array view of the contents of this node.
		''' 
		''' <p>Depending on the underlying implementation, this may return a
		''' reference to an internal array rather than a copy.  Since the returned
		''' array may be shared, the returned array should not be modified.  The
		''' {@code generator} function may be consulted to create the array if a new
		''' array needs to be created.
		''' </summary>
		''' <param name="generator"> a factory function which takes an integer parameter and
		'''        returns a new, empty array of that size and of the appropriate
		'''        array type </param>
		''' <returns> an array containing the contents of this {@code Node} </returns>
		Function asArray(  generator As java.util.function.IntFunction(Of T())) As T()

		''' <summary>
		''' Copies the content of this {@code Node} into an array, starting at a
		''' given offset into the array.  It is the caller's responsibility to ensure
		''' there is sufficient room in the array, otherwise unspecified behaviour
		''' will occur if the array length is less than the number of elements
		''' contained in this node.
		''' </summary>
		''' <param name="array"> the array into which to copy the contents of this
		'''       {@code Node} </param>
		''' <param name="offset"> the starting offset within the array </param>
		''' <exception cref="IndexOutOfBoundsException"> if copying would cause access of data
		'''         outside array bounds </exception>
		''' <exception cref="NullPointerException"> if {@code array} is {@code null} </exception>
		Sub copyInto(  array As T(),   offset As Integer)

		''' <summary>
		''' Gets the {@code StreamShape} associated with this {@code Node}.
		''' 
		''' @implSpec The default in {@code Node} returns
		''' {@code StreamShape.REFERENCE}
		''' </summary>
		''' <returns> the stream shape associated with this node </returns>
		ReadOnly Property default shape As StreamShape
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return StreamShape.REFERENCE;

		''' <summary>
		''' Returns the number of elements contained in this node.
		''' </summary>
		''' <returns> the number of elements contained in this node </returns>
		Function count() As Long

		''' <summary>
		''' A mutable builder for a {@code Node} that implements <seealso cref="Sink"/>, which
		''' builds a flat node containing the elements that have been pushed to it.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		interface Builder(Of T) extends Sink(Of T)
	'	{
	'
	'		''' <summary>
	'		''' Builds the node.  Should be called after all elements have been
	'		''' pushed and signalled with an invocation of <seealso cref="Sink#end()"/>.
	'		''' </summary>
	'		''' <returns> the resulting {@code Node} </returns>
	'		Node<T> build();
	'
	'		''' <summary>
	'		''' Specialized @{code Node.Builder} for int elements
	'		''' </summary>
	'		interface OfInt extends Node.Builder<java.lang.Integer>, Sink.OfInt
	'		{
	'			@Override Node.OfInt build();
	'		}
	'
	'		''' <summary>
	'		''' Specialized @{code Node.Builder} for long elements
	'		''' </summary>
	'		interface OfLong extends Node.Builder<java.lang.Long>, Sink.OfLong
	'		{
	'			@Override Node.OfLong build();
	'		}
	'
	'		''' <summary>
	'		''' Specialized @{code Node.Builder} for double elements
	'		''' </summary>
	'		interface OfDouble extends Node.Builder<java.lang.Double>, Sink.OfDouble
	'		{
	'			@Override Node.OfDouble build();
	'		}
	'	}

'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public interface OfPrimitive(Of T, T_CONS, T_ARR, T_SPLITR As java.util.Spliterator.OfPrimitive(Of T, T_CONS, T_SPLITR), T_NODE As OfPrimitive(Of T, T_CONS, T_ARR, T_SPLITR, T_NODE)) extends Node(Of T)
	'	{
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' </summary>
	'		''' <returns> a <seealso cref="Spliterator.OfPrimitive"/> describing the elements of
	'		'''         this node </returns>
	'		@Override T_SPLITR spliterator();
	'
	'		''' <summary>
	'		''' Traverses the elements of this node, and invoke the provided
	'		''' {@code action} with each element.
	'		''' </summary>
	'		''' <param name="action"> a consumer that is to be invoked with each
	'		'''        element in this {@code Node.OfPrimitive} </param>
	'		@SuppressWarnings("overloads")  Sub  forEach(T_CONS action);
	'
	'		@Override default T_NODE getChild(int i)
	'		{
	'			throw New IndexOutOfBoundsException();
	'		}
	'
	'		T_NODE truncate(long from, long to, IntFunction<T[]> generator);
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' 
	'		''' @implSpec the default implementation invokes the generator to create
	'		''' an instance of a boxed primitive array with a length of
	'		''' <seealso cref="#count()"/> and then invokes <seealso cref="#copyInto(T[], int)"/> with
	'		''' that array at an offset of 0.
	'		''' </summary>
	'		@Override default T[] asArray(IntFunction<T[]> generator)
	'		{
	'			if (java.util.stream.Tripwire.ENABLED)
	'				java.util.stream.Tripwire.trip(getClass(), "{0} calling Node.OfPrimitive.asArray");
	'
	'			long size = count();
	'			if (size >= Nodes.MAX_ARRAY_SIZE)
	'				throw New IllegalArgumentException(Nodes.BAD_SIZE);
	'			T[] boxed = generator.apply((int) count());
	'			copyInto(boxed, 0);
	'			Return boxed;
	'		}
	'
	'		''' <summary>
	'		''' Views this node as a primitive array.
	'		''' 
	'		''' <p>Depending on the underlying implementation this may return a
	'		''' reference to an internal array rather than a copy.  It is the callers
	'		''' responsibility to decide if either this node or the array is utilized
	'		''' as the primary reference for the data.</p>
	'		''' </summary>
	'		''' <returns> an array containing the contents of this {@code Node} </returns>
	'		T_ARR asPrimitiveArray();
	'
	'		''' <summary>
	'		''' Creates a new primitive array.
	'		''' </summary>
	'		''' <param name="count"> the length of the primitive array. </param>
	'		''' <returns> the new primitive array. </returns>
	'		T_ARR newArray(int count);
	'
	'		''' <summary>
	'		''' Copies the content of this {@code Node} into a primitive array,
	'		''' starting at a given offset into the array.  It is the caller's
	'		''' responsibility to ensure there is sufficient room in the array.
	'		''' </summary>
	'		''' <param name="array"> the array into which to copy the contents of this
	'		'''              {@code Node} </param>
	'		''' <param name="offset"> the starting offset within the array </param>
	'		''' <exception cref="IndexOutOfBoundsException"> if copying would cause access of
	'		'''         data outside array bounds </exception>
	'		''' <exception cref="NullPointerException"> if {@code array} is {@code null} </exception>
	'		void copyInto(T_ARR array, int offset);
	'	}

		''' <summary>
		''' Specialized {@code Node} for int elements
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		interface OfInt extends OfPrimitive(Of java.lang.Integer, java.util.function.IntConsumer, int(), java.util.Spliterator.OfInt, OfInt)
	'	{
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' </summary>
	'		''' <param name="consumer"> a {@code Consumer} that is to be invoked with each
	'		'''        element in this {@code Node}.  If this is an
	'		'''        {@code IntConsumer}, it is cast to {@code IntConsumer} so the
	'		'''        elements may be processed without boxing. </param>
	'		@Override default  Sub  forEach(Consumer<? MyBase java.lang.Integer> consumer)
	'		{
	'			if (consumer instanceof IntConsumer)
	'			{
	'				forEach((IntConsumer) consumer);
	'			}
	'			else
	'			{
	'				if (Tripwire.ENABLED)
	'					Tripwire.trip(getClass(), "{0} calling Node.OfInt.forEachRemaining(Consumer)");
	'				spliterator().forEachRemaining(consumer);
	'			}
	'		}
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' 
	'		''' @implSpec the default implementation invokes <seealso cref="#asPrimitiveArray()"/> to
	'		''' obtain an int[] array then and copies the elements from that int[]
	'		''' array into the boxed Integer[] array.  This is not efficient and it
	'		''' is recommended to invoke <seealso cref="#copyInto(Object, int)"/>.
	'		''' </summary>
	'		@Override default  Sub  copyInto(java.lang.Integer[] boxed, int offset)
	'		{
	'			if (Tripwire.ENABLED)
	'				Tripwire.trip(getClass(), "{0} calling Node.OfInt.copyInto(Integer[], int)");
	'
	'			int[] array = asPrimitiveArray();
	'			for (int i = 0; i < array.length; i += 1)
	'			{
	'				boxed[offset + i] = array[i];
	'			}
	'		}
	'
	'		@Override default Node.OfInt truncate(long from, long to, IntFunction<java.lang.Integer[]> generator)
	'		{
	'			if (from == 0 && to == count())
	'				Return Me;
	'			long size = to - from;
	'			Spliterator.OfInt spliterator = spliterator();
	'			Node.Builder.OfInt nodeBuilder = Nodes.intBuilder(size);
	'			nodeBuilder.begin(size);
	'			for (int i = 0; i < from && spliterator.tryAdvance((IntConsumer) e -> { }); i += 1)
	'			{
	'			}
	'			for (int i = 0; (i < size) && spliterator.tryAdvance((IntConsumer) nodeBuilder); i += 1)
	'			{
	'			}
	'			nodeBuilder.end();
	'			Return nodeBuilder.build();
	'		}
	'
	'		@Override default int[] newArray(int count)
	'		{
	'			Return New int[count];
	'		}
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' @implSpec The default in {@code Node.OfInt} returns
	'		''' {@code StreamShape.INT_VALUE}
	'		''' </summary>
	'		default StreamShape getShape()
	'		{
	'			Return StreamShape.INT_VALUE;
	'		}
	'	}

		''' <summary>
		''' Specialized {@code Node} for long elements
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		interface OfLong extends OfPrimitive(Of java.lang.Long, java.util.function.LongConsumer, long(), java.util.Spliterator.OfLong, OfLong)
	'	{
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' </summary>
	'		''' <param name="consumer"> A {@code Consumer} that is to be invoked with each
	'		'''        element in this {@code Node}.  If this is an
	'		'''        {@code LongConsumer}, it is cast to {@code LongConsumer} so
	'		'''        the elements may be processed without boxing. </param>
	'		@Override default  Sub  forEach(Consumer<? MyBase java.lang.Long> consumer)
	'		{
	'			if (consumer instanceof LongConsumer)
	'			{
	'				forEach((LongConsumer) consumer);
	'			}
	'			else
	'			{
	'				if (Tripwire.ENABLED)
	'					Tripwire.trip(getClass(), "{0} calling Node.OfLong.forEachRemaining(Consumer)");
	'				spliterator().forEachRemaining(consumer);
	'			}
	'		}
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' 
	'		''' @implSpec the default implementation invokes <seealso cref="#asPrimitiveArray()"/>
	'		''' to obtain a long[] array then and copies the elements from that
	'		''' long[] array into the boxed Long[] array.  This is not efficient and
	'		''' it is recommended to invoke <seealso cref="#copyInto(Object, int)"/>.
	'		''' </summary>
	'		@Override default  Sub  copyInto(java.lang.Long[] boxed, int offset)
	'		{
	'			if (Tripwire.ENABLED)
	'				Tripwire.trip(getClass(), "{0} calling Node.OfInt.copyInto(Long[], int)");
	'
	'			long[] array = asPrimitiveArray();
	'			for (int i = 0; i < array.length; i += 1)
	'			{
	'				boxed[offset + i] = array[i];
	'			}
	'		}
	'
	'		@Override default Node.OfLong truncate(long from, long to, IntFunction<java.lang.Long[]> generator)
	'		{
	'			if (from == 0 && to == count())
	'				Return Me;
	'			long size = to - from;
	'			Spliterator.OfLong spliterator = spliterator();
	'			Node.Builder.OfLong nodeBuilder = Nodes.longBuilder(size);
	'			nodeBuilder.begin(size);
	'			for (int i = 0; i < from && spliterator.tryAdvance((LongConsumer) e -> { }); i += 1)
	'			{
	'			}
	'			for (int i = 0; (i < size) && spliterator.tryAdvance((LongConsumer) nodeBuilder); i += 1)
	'			{
	'			}
	'			nodeBuilder.end();
	'			Return nodeBuilder.build();
	'		}
	'
	'		@Override default long[] newArray(int count)
	'		{
	'			Return New long[count];
	'		}
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' @implSpec The default in {@code Node.OfLong} returns
	'		''' {@code StreamShape.LONG_VALUE}
	'		''' </summary>
	'		default StreamShape getShape()
	'		{
	'			Return StreamShape.LONG_VALUE;
	'		}
	'	}

		''' <summary>
		''' Specialized {@code Node} for double elements
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		interface OfDouble extends OfPrimitive(Of java.lang.Double, java.util.function.DoubleConsumer, double(), java.util.Spliterator.OfDouble, OfDouble)
	'	{
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' </summary>
	'		''' <param name="consumer"> A {@code Consumer} that is to be invoked with each
	'		'''        element in this {@code Node}.  If this is an
	'		'''        {@code DoubleConsumer}, it is cast to {@code DoubleConsumer}
	'		'''        so the elements may be processed without boxing. </param>
	'		@Override default  Sub  forEach(Consumer<? MyBase java.lang.Double> consumer)
	'		{
	'			if (consumer instanceof DoubleConsumer)
	'			{
	'				forEach((DoubleConsumer) consumer);
	'			}
	'			else
	'			{
	'				if (Tripwire.ENABLED)
	'					Tripwire.trip(getClass(), "{0} calling Node.OfLong.forEachRemaining(Consumer)");
	'				spliterator().forEachRemaining(consumer);
	'			}
	'		}
	'
	'		'
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' 
	'		''' @implSpec the default implementation invokes <seealso cref="#asPrimitiveArray()"/>
	'		''' to obtain a double[] array then and copies the elements from that
	'		''' double[] array into the boxed Double[] array.  This is not efficient
	'		''' and it is recommended to invoke <seealso cref="#copyInto(Object, int)"/>.
	'		''' </summary>
	'		@Override default  Sub  copyInto(java.lang.Double[] boxed, int offset)
	'		{
	'			if (Tripwire.ENABLED)
	'				Tripwire.trip(getClass(), "{0} calling Node.OfDouble.copyInto(Double[], int)");
	'
	'			double[] array = asPrimitiveArray();
	'			for (int i = 0; i < array.length; i += 1)
	'			{
	'				boxed[offset + i] = array[i];
	'			}
	'		}
	'
	'		@Override default Node.OfDouble truncate(long from, long to, IntFunction<java.lang.Double[]> generator)
	'		{
	'			if (from == 0 && to == count())
	'				Return Me;
	'			long size = to - from;
	'			Spliterator.OfDouble spliterator = spliterator();
	'			Node.Builder.OfDouble nodeBuilder = Nodes.doubleBuilder(size);
	'			nodeBuilder.begin(size);
	'			for (int i = 0; i < from && spliterator.tryAdvance((DoubleConsumer) e -> { }); i += 1)
	'			{
	'			}
	'			for (int i = 0; (i < size) && spliterator.tryAdvance((DoubleConsumer) nodeBuilder); i += 1)
	'			{
	'			}
	'			nodeBuilder.end();
	'			Return nodeBuilder.build();
	'		}
	'
	'		@Override default double[] newArray(int count)
	'		{
	'			Return New double[count];
	'		}
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' 
	'		''' @implSpec The default in {@code Node.OfDouble} returns
	'		''' {@code StreamShape.DOUBLE_VALUE}
	'		''' </summary>
	'		default StreamShape getShape()
	'		{
	'			Return StreamShape.DOUBLE_VALUE;
	'		}
	'	}
	End Interface

End Namespace
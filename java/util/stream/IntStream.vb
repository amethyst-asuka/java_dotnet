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
	''' A sequence of primitive int-valued elements supporting sequential and parallel
	''' aggregate operations.  This is the {@code int} primitive specialization of
	''' <seealso cref="Stream"/>.
	''' 
	''' <p>The following example illustrates an aggregate operation using
	''' <seealso cref="Stream"/> and <seealso cref="IntStream"/>, computing the sum of the weights of the
	''' red widgets:
	''' 
	''' <pre>{@code
	'''     int sum = widgets.stream()
	'''                      .filter(w -> w.getColor() == RED)
	'''                      .mapToInt(w -> w.getWeight())
	'''                      .sum();
	''' }</pre>
	''' 
	''' See the class documentation for <seealso cref="Stream"/> and the package documentation
	''' for <a href="package-summary.html">java.util.stream</a> for additional
	''' specification of streams, stream operations, stream pipelines, and
	''' parallelism.
	''' 
	''' @since 1.8 </summary>
	''' <seealso cref= Stream </seealso>
	''' <seealso cref= <a href="package-summary.html">java.util.stream</a> </seealso>
	Public Interface IntStream
		Inherits BaseStream(Of Integer?, IntStream)

		''' <summary>
		''' Returns a stream consisting of the elements of this stream that match
		''' the given predicate.
		''' 
		''' <p>This is an <a href="package-summary.html#StreamOps">intermediate
		''' operation</a>.
		''' </summary>
		''' <param name="predicate"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''                  <a href="package-summary.html#Statelessness">stateless</a>
		'''                  predicate to apply to each element to determine if it
		'''                  should be included </param>
		''' <returns> the new stream </returns>
		Function filter(ByVal predicate As java.util.function.IntPredicate) As IntStream

		''' <summary>
		''' Returns a stream consisting of the results of applying the given
		''' function to the elements of this stream.
		''' 
		''' <p>This is an <a href="package-summary.html#StreamOps">intermediate
		''' operation</a>.
		''' </summary>
		''' <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''               <a href="package-summary.html#Statelessness">stateless</a>
		'''               function to apply to each element </param>
		''' <returns> the new stream </returns>
		Function map(ByVal mapper As java.util.function.IntUnaryOperator) As IntStream

		''' <summary>
		''' Returns an object-valued {@code Stream} consisting of the results of
		''' applying the given function to the elements of this stream.
		''' 
		''' <p>This is an <a href="package-summary.html#StreamOps">
		'''     intermediate operation</a>.
		''' </summary>
		''' @param <U> the element type of the new stream </param>
		''' <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''               <a href="package-summary.html#Statelessness">stateless</a>
		'''               function to apply to each element </param>
		''' <returns> the new stream </returns>
		 Function mapToObj(Of U, T1 As U)(ByVal mapper As java.util.function.IntFunction(Of T1)) As Stream(Of U)

		''' <summary>
		''' Returns a {@code LongStream} consisting of the results of applying the
		''' given function to the elements of this stream.
		''' 
		''' <p>This is an <a href="package-summary.html#StreamOps">intermediate
		''' operation</a>.
		''' </summary>
		''' <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''               <a href="package-summary.html#Statelessness">stateless</a>
		'''               function to apply to each element </param>
		''' <returns> the new stream </returns>
		Function mapToLong(ByVal mapper As java.util.function.IntToLongFunction) As LongStream

		''' <summary>
		''' Returns a {@code DoubleStream} consisting of the results of applying the
		''' given function to the elements of this stream.
		''' 
		''' <p>This is an <a href="package-summary.html#StreamOps">intermediate
		''' operation</a>.
		''' </summary>
		''' <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''               <a href="package-summary.html#Statelessness">stateless</a>
		'''               function to apply to each element </param>
		''' <returns> the new stream </returns>
		Function mapToDouble(ByVal mapper As java.util.function.IntToDoubleFunction) As DoubleStream

		''' <summary>
		''' Returns a stream consisting of the results of replacing each element of
		''' this stream with the contents of a mapped stream produced by applying
		''' the provided mapping function to each element.  Each mapped stream is
		''' <seealso cref="java.util.stream.BaseStream#close() closed"/> after its contents
		''' have been placed into this stream.  (If a mapped stream is {@code null}
		''' an empty stream is used, instead.)
		''' 
		''' <p>This is an <a href="package-summary.html#StreamOps">intermediate
		''' operation</a>.
		''' </summary>
		''' <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''               <a href="package-summary.html#Statelessness">stateless</a>
		'''               function to apply to each element which produces an
		'''               {@code IntStream} of new values </param>
		''' <returns> the new stream </returns>
		''' <seealso cref= Stream#flatMap(Function) </seealso>
		Function flatMap(Of T1 As IntStream)(ByVal mapper As java.util.function.IntFunction(Of T1)) As IntStream

		''' <summary>
		''' Returns a stream consisting of the distinct elements of this stream.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">stateful
		''' intermediate operation</a>.
		''' </summary>
		''' <returns> the new stream </returns>
		Function distinct() As IntStream

		''' <summary>
		''' Returns a stream consisting of the elements of this stream in sorted
		''' order.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">stateful
		''' intermediate operation</a>.
		''' </summary>
		''' <returns> the new stream </returns>
		Function sorted() As IntStream

		''' <summary>
		''' Returns a stream consisting of the elements of this stream, additionally
		''' performing the provided action on each element as elements are consumed
		''' from the resulting stream.
		''' 
		''' <p>This is an <a href="package-summary.html#StreamOps">intermediate
		''' operation</a>.
		''' 
		''' <p>For parallel stream pipelines, the action may be called at
		''' whatever time and in whatever thread the element is made available by the
		''' upstream operation.  If the action modifies shared state,
		''' it is responsible for providing the required synchronization.
		''' 
		''' @apiNote This method exists mainly to support debugging, where you want
		''' to see the elements as they flow past a certain point in a pipeline:
		''' <pre>{@code
		'''     IntStream.of(1, 2, 3, 4)
		'''         .filter(e -> e > 2)
		'''         .peek(e -> System.out.println("Filtered value: " + e))
		'''         .map(e -> e * e)
		'''         .peek(e -> System.out.println("Mapped value: " + e))
		'''         .sum();
		''' }</pre>
		''' </summary>
		''' <param name="action"> a <a href="package-summary.html#NonInterference">
		'''               non-interfering</a> action to perform on the elements as
		'''               they are consumed from the stream </param>
		''' <returns> the new stream </returns>
		Function peek(ByVal action As java.util.function.IntConsumer) As IntStream

		''' <summary>
		''' Returns a stream consisting of the elements of this stream, truncated
		''' to be no longer than {@code maxSize} in length.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">short-circuiting
		''' stateful intermediate operation</a>.
		''' 
		''' @apiNote
		''' While {@code limit()} is generally a cheap operation on sequential
		''' stream pipelines, it can be quite expensive on ordered parallel pipelines,
		''' especially for large values of {@code maxSize}, since {@code limit(n)}
		''' is constrained to return not just any <em>n</em> elements, but the
		''' <em>first n</em> elements in the encounter order.  Using an unordered
		''' stream source (such as <seealso cref="#generate(IntSupplier)"/>) or removing the
		''' ordering constraint with <seealso cref="#unordered()"/> may result in significant
		''' speedups of {@code limit()} in parallel pipelines, if the semantics of
		''' your situation permit.  If consistency with encounter order is required,
		''' and you are experiencing poor performance or memory utilization with
		''' {@code limit()} in parallel pipelines, switching to sequential execution
		''' with <seealso cref="#sequential()"/> may improve performance.
		''' </summary>
		''' <param name="maxSize"> the number of elements the stream should be limited to </param>
		''' <returns> the new stream </returns>
		''' <exception cref="IllegalArgumentException"> if {@code maxSize} is negative </exception>
		Function limit(ByVal maxSize As Long) As IntStream

		''' <summary>
		''' Returns a stream consisting of the remaining elements of this stream
		''' after discarding the first {@code n} elements of the stream.
		''' If this stream contains fewer than {@code n} elements then an
		''' empty stream will be returned.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">stateful
		''' intermediate operation</a>.
		''' 
		''' @apiNote
		''' While {@code skip()} is generally a cheap operation on sequential
		''' stream pipelines, it can be quite expensive on ordered parallel pipelines,
		''' especially for large values of {@code n}, since {@code skip(n)}
		''' is constrained to skip not just any <em>n</em> elements, but the
		''' <em>first n</em> elements in the encounter order.  Using an unordered
		''' stream source (such as <seealso cref="#generate(IntSupplier)"/>) or removing the
		''' ordering constraint with <seealso cref="#unordered()"/> may result in significant
		''' speedups of {@code skip()} in parallel pipelines, if the semantics of
		''' your situation permit.  If consistency with encounter order is required,
		''' and you are experiencing poor performance or memory utilization with
		''' {@code skip()} in parallel pipelines, switching to sequential execution
		''' with <seealso cref="#sequential()"/> may improve performance.
		''' </summary>
		''' <param name="n"> the number of leading elements to skip </param>
		''' <returns> the new stream </returns>
		''' <exception cref="IllegalArgumentException"> if {@code n} is negative </exception>
		Function skip(ByVal n As Long) As IntStream

		''' <summary>
		''' Performs an action for each element of this stream.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' 
		''' <p>For parallel stream pipelines, this operation does <em>not</em>
		''' guarantee to respect the encounter order of the stream, as doing so
		''' would sacrifice the benefit of parallelism.  For any given element, the
		''' action may be performed at whatever time and in whatever thread the
		''' library chooses.  If the action accesses shared state, it is
		''' responsible for providing the required synchronization.
		''' </summary>
		''' <param name="action"> a <a href="package-summary.html#NonInterference">
		'''               non-interfering</a> action to perform on the elements </param>
		Sub forEach(ByVal action As java.util.function.IntConsumer)

		''' <summary>
		''' Performs an action for each element of this stream, guaranteeing that
		''' each element is processed in encounter order for streams that have a
		''' defined encounter order.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' </summary>
		''' <param name="action"> a <a href="package-summary.html#NonInterference">
		'''               non-interfering</a> action to perform on the elements </param>
		''' <seealso cref= #forEach(IntConsumer) </seealso>
		Sub forEachOrdered(ByVal action As java.util.function.IntConsumer)

		''' <summary>
		''' Returns an array containing the elements of this stream.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' </summary>
		''' <returns> an array containing the elements of this stream </returns>
		Function toArray() As Integer()

		''' <summary>
		''' Performs a <a href="package-summary.html#Reduction">reduction</a> on the
		''' elements of this stream, using the provided identity value and an
		''' <a href="package-summary.html#Associativity">associative</a>
		''' accumulation function, and returns the reduced value.  This is equivalent
		''' to:
		''' <pre>{@code
		'''     int result = identity;
		'''     for (int element : this stream)
		'''         result = accumulator.applyAsInt(result, element)
		'''     return result;
		''' }</pre>
		''' 
		''' but is not constrained to execute sequentially.
		''' 
		''' <p>The {@code identity} value must be an identity for the accumulator
		''' function. This means that for all {@code x},
		''' {@code accumulator.apply(identity, x)} is equal to {@code x}.
		''' The {@code accumulator} function must be an
		''' <a href="package-summary.html#Associativity">associative</a> function.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' 
		''' @apiNote Sum, min, max, and average are all special cases of reduction.
		''' Summing a stream of numbers can be expressed as:
		''' 
		''' <pre>{@code
		'''     int sum = integers.reduce(0, (a, b) -> a+b);
		''' }</pre>
		''' 
		''' or more compactly:
		''' 
		''' <pre>{@code
		'''     int sum = integers.reduce(0, Integer::sum);
		''' }</pre>
		''' 
		''' <p>While this may seem a more roundabout way to perform an aggregation
		''' compared to simply mutating a running total in a loop, reduction
		''' operations parallelize more gracefully, without needing additional
		''' synchronization and with greatly reduced risk of data races.
		''' </summary>
		''' <param name="identity"> the identity value for the accumulating function </param>
		''' <param name="op"> an <a href="package-summary.html#Associativity">associative</a>,
		'''           <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''           <a href="package-summary.html#Statelessness">stateless</a>
		'''           function for combining two values </param>
		''' <returns> the result of the reduction </returns>
		''' <seealso cref= #sum() </seealso>
		''' <seealso cref= #min() </seealso>
		''' <seealso cref= #max() </seealso>
		''' <seealso cref= #average() </seealso>
		Function reduce(ByVal identity As Integer, ByVal op As java.util.function.IntBinaryOperator) As Integer

		''' <summary>
		''' Performs a <a href="package-summary.html#Reduction">reduction</a> on the
		''' elements of this stream, using an
		''' <a href="package-summary.html#Associativity">associative</a> accumulation
		''' function, and returns an {@code OptionalInt} describing the reduced value,
		''' if any. This is equivalent to:
		''' <pre>{@code
		'''     boolean foundAny = false;
		'''     int result = null;
		'''     for (int element : this stream) {
		'''         if (!foundAny) {
		'''             foundAny = true;
		'''             result = element;
		'''         }
		'''         else
		'''             result = accumulator.applyAsInt(result, element);
		'''     }
		'''     return foundAny ? OptionalInt.of(result) : OptionalInt.empty();
		''' }</pre>
		''' 
		''' but is not constrained to execute sequentially.
		''' 
		''' <p>The {@code accumulator} function must be an
		''' <a href="package-summary.html#Associativity">associative</a> function.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' </summary>
		''' <param name="op"> an <a href="package-summary.html#Associativity">associative</a>,
		'''           <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''           <a href="package-summary.html#Statelessness">stateless</a>
		'''           function for combining two values </param>
		''' <returns> the result of the reduction </returns>
		''' <seealso cref= #reduce(int, IntBinaryOperator) </seealso>
		Function reduce(ByVal op As java.util.function.IntBinaryOperator) As java.util.OptionalInt

		''' <summary>
		''' Performs a <a href="package-summary.html#MutableReduction">mutable
		''' reduction</a> operation on the elements of this stream.  A mutable
		''' reduction is one in which the reduced value is a mutable result container,
		''' such as an {@code ArrayList}, and elements are incorporated by updating
		''' the state of the result rather than by replacing the result.  This
		''' produces a result equivalent to:
		''' <pre>{@code
		'''     R result = supplier.get();
		'''     for (int element : this stream)
		'''         accumulator.accept(result, element);
		'''     return result;
		''' }</pre>
		''' 
		''' <p>Like <seealso cref="#reduce(int, IntBinaryOperator)"/>, {@code collect} operations
		''' can be parallelized without requiring additional synchronization.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' </summary>
		''' @param <R> type of the result </param>
		''' <param name="supplier"> a function that creates a new result container. For a
		'''                 parallel execution, this function may be called
		'''                 multiple times and must return a fresh value each time. </param>
		''' <param name="accumulator"> an <a href="package-summary.html#Associativity">associative</a>,
		'''                    <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''                    <a href="package-summary.html#Statelessness">stateless</a>
		'''                    function for incorporating an additional element into a result </param>
		''' <param name="combiner"> an <a href="package-summary.html#Associativity">associative</a>,
		'''                    <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''                    <a href="package-summary.html#Statelessness">stateless</a>
		'''                    function for combining two values, which must be
		'''                    compatible with the accumulator function </param>
		''' <returns> the result of the reduction </returns>
		''' <seealso cref= Stream#collect(Supplier, BiConsumer, BiConsumer) </seealso>
		 Function collect(Of R)(ByVal supplier As java.util.function.Supplier(Of R), ByVal accumulator As java.util.function.ObjIntConsumer(Of R), ByVal combiner As java.util.function.BiConsumer(Of R, R)) As R

		''' <summary>
		''' Returns the sum of elements in this stream.  This is a special case
		''' of a <a href="package-summary.html#Reduction">reduction</a>
		''' and is equivalent to:
		''' <pre>{@code
		'''     return reduce(0, Integer::sum);
		''' }</pre>
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' </summary>
		''' <returns> the sum of elements in this stream </returns>
		Function sum() As Integer

		''' <summary>
		''' Returns an {@code OptionalInt} describing the minimum element of this
		''' stream, or an empty optional if this stream is empty.  This is a special
		''' case of a <a href="package-summary.html#Reduction">reduction</a>
		''' and is equivalent to:
		''' <pre>{@code
		'''     return reduce(Integer::min);
		''' }</pre>
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal operation</a>.
		''' </summary>
		''' <returns> an {@code OptionalInt} containing the minimum element of this
		''' stream, or an empty {@code OptionalInt} if the stream is empty </returns>
		Function min() As java.util.OptionalInt

		''' <summary>
		''' Returns an {@code OptionalInt} describing the maximum element of this
		''' stream, or an empty optional if this stream is empty.  This is a special
		''' case of a <a href="package-summary.html#Reduction">reduction</a>
		''' and is equivalent to:
		''' <pre>{@code
		'''     return reduce(Integer::max);
		''' }</pre>
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' </summary>
		''' <returns> an {@code OptionalInt} containing the maximum element of this
		''' stream, or an empty {@code OptionalInt} if the stream is empty </returns>
		Function max() As java.util.OptionalInt

		''' <summary>
		''' Returns the count of elements in this stream.  This is a special case of
		''' a <a href="package-summary.html#Reduction">reduction</a> and is
		''' equivalent to:
		''' <pre>{@code
		'''     return mapToLong(e -> 1L).sum();
		''' }</pre>
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal operation</a>.
		''' </summary>
		''' <returns> the count of elements in this stream </returns>
		Function count() As Long

		''' <summary>
		''' Returns an {@code OptionalDouble} describing the arithmetic mean of elements of
		''' this stream, or an empty optional if this stream is empty.  This is a
		''' special case of a
		''' <a href="package-summary.html#Reduction">reduction</a>.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' </summary>
		''' <returns> an {@code OptionalDouble} containing the average element of this
		''' stream, or an empty optional if the stream is empty </returns>
		Function average() As java.util.OptionalDouble

		''' <summary>
		''' Returns an {@code IntSummaryStatistics} describing various
		''' summary data about the elements of this stream.  This is a special
		''' case of a <a href="package-summary.html#Reduction">reduction</a>.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' </summary>
		''' <returns> an {@code IntSummaryStatistics} describing various summary data
		''' about the elements of this stream </returns>
		Function summaryStatistics() As java.util.IntSummaryStatistics

		''' <summary>
		''' Returns whether any elements of this stream match the provided
		''' predicate.  May not evaluate the predicate on all elements if not
		''' necessary for determining the result.  If the stream is empty then
		''' {@code false} is returned and the predicate is not evaluated.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">short-circuiting
		''' terminal operation</a>.
		''' 
		''' @apiNote
		''' This method evaluates the <em>existential quantification</em> of the
		''' predicate over the elements of the stream (for some x P(x)).
		''' </summary>
		''' <param name="predicate"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''                  <a href="package-summary.html#Statelessness">stateless</a>
		'''                  predicate to apply to elements of this stream </param>
		''' <returns> {@code true} if any elements of the stream match the provided
		''' predicate, otherwise {@code false} </returns>
		Function anyMatch(ByVal predicate As java.util.function.IntPredicate) As Boolean

		''' <summary>
		''' Returns whether all elements of this stream match the provided predicate.
		''' May not evaluate the predicate on all elements if not necessary for
		''' determining the result.  If the stream is empty then {@code true} is
		''' returned and the predicate is not evaluated.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">short-circuiting
		''' terminal operation</a>.
		''' 
		''' @apiNote
		''' This method evaluates the <em>universal quantification</em> of the
		''' predicate over the elements of the stream (for all x P(x)).  If the
		''' stream is empty, the quantification is said to be <em>vacuously
		''' satisfied</em> and is always {@code true} (regardless of P(x)).
		''' </summary>
		''' <param name="predicate"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''                  <a href="package-summary.html#Statelessness">stateless</a>
		'''                  predicate to apply to elements of this stream </param>
		''' <returns> {@code true} if either all elements of the stream match the
		''' provided predicate or the stream is empty, otherwise {@code false} </returns>
		Function allMatch(ByVal predicate As java.util.function.IntPredicate) As Boolean

		''' <summary>
		''' Returns whether no elements of this stream match the provided predicate.
		''' May not evaluate the predicate on all elements if not necessary for
		''' determining the result.  If the stream is empty then {@code true} is
		''' returned and the predicate is not evaluated.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">short-circuiting
		''' terminal operation</a>.
		''' 
		''' @apiNote
		''' This method evaluates the <em>universal quantification</em> of the
		''' negated predicate over the elements of the stream (for all x ~P(x)).  If
		''' the stream is empty, the quantification is said to be vacuously satisfied
		''' and is always {@code true}, regardless of P(x).
		''' </summary>
		''' <param name="predicate"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''                  <a href="package-summary.html#Statelessness">stateless</a>
		'''                  predicate to apply to elements of this stream </param>
		''' <returns> {@code true} if either no elements of the stream match the
		''' provided predicate or the stream is empty, otherwise {@code false} </returns>
		Function noneMatch(ByVal predicate As java.util.function.IntPredicate) As Boolean

		''' <summary>
		''' Returns an <seealso cref="OptionalInt"/> describing the first element of this
		''' stream, or an empty {@code OptionalInt} if the stream is empty.  If the
		''' stream has no encounter order, then any element may be returned.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">short-circuiting
		''' terminal operation</a>.
		''' </summary>
		''' <returns> an {@code OptionalInt} describing the first element of this stream,
		''' or an empty {@code OptionalInt} if the stream is empty </returns>
		Function findFirst() As java.util.OptionalInt

		''' <summary>
		''' Returns an <seealso cref="OptionalInt"/> describing some element of the stream, or
		''' an empty {@code OptionalInt} if the stream is empty.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">short-circuiting
		''' terminal operation</a>.
		''' 
		''' <p>The behavior of this operation is explicitly nondeterministic; it is
		''' free to select any element in the stream.  This is to allow for maximal
		''' performance in parallel operations; the cost is that multiple invocations
		''' on the same source may not return the same result.  (If a stable result
		''' is desired, use <seealso cref="#findFirst()"/> instead.)
		''' </summary>
		''' <returns> an {@code OptionalInt} describing some element of this stream, or
		''' an empty {@code OptionalInt} if the stream is empty </returns>
		''' <seealso cref= #findFirst() </seealso>
		Function findAny() As java.util.OptionalInt

		''' <summary>
		''' Returns a {@code LongStream} consisting of the elements of this stream,
		''' converted to {@code long}.
		''' 
		''' <p>This is an <a href="package-summary.html#StreamOps">intermediate
		''' operation</a>.
		''' </summary>
		''' <returns> a {@code LongStream} consisting of the elements of this stream,
		''' converted to {@code long} </returns>
		Function asLongStream() As LongStream

		''' <summary>
		''' Returns a {@code DoubleStream} consisting of the elements of this stream,
		''' converted to {@code double}.
		''' 
		''' <p>This is an <a href="package-summary.html#StreamOps">intermediate
		''' operation</a>.
		''' </summary>
		''' <returns> a {@code DoubleStream} consisting of the elements of this stream,
		''' converted to {@code double} </returns>
		Function asDoubleStream() As DoubleStream

		''' <summary>
		''' Returns a {@code Stream} consisting of the elements of this stream,
		''' each boxed to an {@code Integer}.
		''' 
		''' <p>This is an <a href="package-summary.html#StreamOps">intermediate
		''' operation</a>.
		''' </summary>
		''' <returns> a {@code Stream} consistent of the elements of this stream,
		''' each boxed to an {@code Integer} </returns>
		Function boxed() As Stream(Of Integer?)

		Overrides Function sequential() As IntStream

		Overrides Function parallel() As IntStream

		Overrides Function [iterator]() As java.util.PrimitiveIterator.OfInt

		Overrides Function spliterator() As java.util.Spliterator.OfInt

		' Static factories

		''' <summary>
		''' Returns a builder for an {@code IntStream}.
		''' </summary>
		''' <returns> a stream builder </returns>
		Shared Function builder() As Builder
			Return Function Streams.IntStreamBuilderImpl() As New

		''' <summary>
		''' Returns an empty sequential {@code IntStream}.
		''' </summary>
		''' <returns> an empty sequential stream </returns>
		Shared Function empty() As IntStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return StreamSupport.intStream(java.util.Spliterators.emptyIntSpliterator(), False);

		''' <summary>
		''' Returns a sequential {@code IntStream} containing a single element.
		''' </summary>
		''' <param name="t"> the single element </param>
		''' <returns> a singleton sequential stream </returns>
		Shared Function [of](ByVal t As Integer) As IntStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return StreamSupport.intStream(New Streams.IntStreamBuilderImpl(t), False);

		''' <summary>
		''' Returns a sequential ordered stream whose elements are the specified values.
		''' </summary>
		''' <param name="values"> the elements of the new stream </param>
		''' <returns> the new stream </returns>
		Shared Function [of](ParamArray ByVal values As Integer()) As IntStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return java.util.Arrays.stream(values);

		''' <summary>
		''' Returns an infinite sequential ordered {@code IntStream} produced by iterative
		''' application of a function {@code f} to an initial element {@code seed},
		''' producing a {@code Stream} consisting of {@code seed}, {@code f(seed)},
		''' {@code f(f(seed))}, etc.
		''' 
		''' <p>The first element (position {@code 0}) in the {@code IntStream} will be
		''' the provided {@code seed}.  For {@code n > 0}, the element at position
		''' {@code n}, will be the result of applying the function {@code f} to the
		''' element at position {@code n - 1}.
		''' </summary>
		''' <param name="seed"> the initial element </param>
		''' <param name="f"> a function to be applied to to the previous element to produce
		'''          a new element </param>
		''' <returns> A new sequential {@code IntStream} </returns>
		Shared Function iterate(ByVal seed As Integer, ByVal f As java.util.function.IntUnaryOperator) As IntStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(f);
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			final java.util.PrimitiveIterator.OfInt iterator = New java.util.PrimitiveIterator.OfInt()
	'		{
	'			int t = seed;
	'
	'			@Override public boolean hasNext()
	'			{
	'				Return True;
	'			}
	'
	'			@Override public int nextInt()
	'			{
	'				int v = t;
	'				t = f.applyAsInt(t);
	'				Return v;
	'			}
	'		};
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return StreamSupport.intStream(java.util.Spliterators.spliteratorUnknownSize(iterator, java.util.Spliterator.ORDERED | java.util.Spliterator.IMMUTABLE | java.util.Spliterator.NONNULL), False);

		''' <summary>
		''' Returns an infinite sequential unordered stream where each element is
		''' generated by the provided {@code IntSupplier}.  This is suitable for
		''' generating constant streams, streams of random elements, etc.
		''' </summary>
		''' <param name="s"> the {@code IntSupplier} for generated elements </param>
		''' <returns> a new infinite sequential unordered {@code IntStream} </returns>
		Shared Function generate(ByVal s As java.util.function.IntSupplier) As IntStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(s);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return StreamSupport.intStream(New StreamSpliterators.InfiniteSupplyingSpliterator.OfInt(Long.MAX_VALUE, s), False);

		''' <summary>
		''' Returns a sequential ordered {@code IntStream} from {@code startInclusive}
		''' (inclusive) to {@code endExclusive} (exclusive) by an incremental step of
		''' {@code 1}.
		''' 
		''' @apiNote
		''' <p>An equivalent sequence of increasing values can be produced
		''' sequentially using a {@code for} loop as follows:
		''' <pre>{@code
		'''     for (int i = startInclusive; i < endExclusive ; i++) { ... }
		''' }</pre>
		''' </summary>
		''' <param name="startInclusive"> the (inclusive) initial value </param>
		''' <param name="endExclusive"> the exclusive upper bound </param>
		''' <returns> a sequential {@code IntStream} for the range of {@code int}
		'''         elements </returns>
		Shared Function range(ByVal startInclusive As Integer, ByVal endExclusive As Integer) As IntStream
			Sub [New](startInclusive >= ByVal endExclusive As )
				Function empty() As [Return]
			Else
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				Return StreamSupport.intStream(New Streams.RangeIntSpliterator(startInclusive, endExclusive, False), False);
			End If

		''' <summary>
		''' Returns a sequential ordered {@code IntStream} from {@code startInclusive}
		''' (inclusive) to {@code endInclusive} (inclusive) by an incremental step of
		''' {@code 1}.
		''' 
		''' @apiNote
		''' <p>An equivalent sequence of increasing values can be produced
		''' sequentially using a {@code for} loop as follows:
		''' <pre>{@code
		'''     for (int i = startInclusive; i <= endInclusive ; i++) { ... }
		''' }</pre>
		''' </summary>
		''' <param name="startInclusive"> the (inclusive) initial value </param>
		''' <param name="endInclusive"> the inclusive upper bound </param>
		''' <returns> a sequential {@code IntStream} for the range of {@code int}
		'''         elements </returns>
		Shared Function rangeClosed(ByVal startInclusive As Integer, ByVal endInclusive As Integer) As IntStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			if (startInclusive > endInclusive)
				Function empty() As [Return]
			Else
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				Return StreamSupport.intStream(New Streams.RangeIntSpliterator(startInclusive, endInclusive, True), False);
			End If

		''' <summary>
		''' Creates a lazily concatenated stream whose elements are all the
		''' elements of the first stream followed by all the elements of the
		''' second stream.  The resulting stream is ordered if both
		''' of the input streams are ordered, and parallel if either of the input
		''' streams is parallel.  When the resulting stream is closed, the close
		''' handlers for both input streams are invoked.
		''' 
		''' @implNote
		''' Use caution when constructing streams from repeated concatenation.
		''' Accessing an element of a deeply concatenated stream can result in deep
		''' call chains, or even {@code StackOverflowException}.
		''' </summary>
		''' <param name="a"> the first stream </param>
		''' <param name="b"> the second stream </param>
		''' <returns> the concatenation of the two input streams </returns>
		Shared Function concat(ByVal a As IntStream, ByVal b As IntStream) As IntStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(a);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(b);

'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			java.util.Spliterator.OfInt split = New Streams.ConcatSpliterator.OfInt(a.spliterator(), b.spliterator());
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			IntStream stream = StreamSupport.intStream(split, a.isParallel() || b.isParallel());
			Function stream.onClose(Streams.composedClose(a, b) ByVal  As ) As [Return]

		''' <summary>
		''' A mutable builder for an {@code IntStream}.
		''' 
		''' <p>A stream builder has a lifecycle, which starts in a building
		''' phase, during which elements can be added, and then transitions to a built
		''' phase, after which elements may not be added.  The built phase
		''' begins when the <seealso cref="#build()"/> method is called, which creates an
		''' ordered stream whose elements are the elements that were added to the
		''' stream builder, in the order they were added.
		''' </summary>
		''' <seealso cref= IntStream#builder()
		''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public interface Builder extends java.util.function.IntConsumer
	'	{
	'
	'		''' <summary>
	'		''' Adds an element to the stream being built.
	'		''' </summary>
	'		''' <exception cref="IllegalStateException"> if the builder has already transitioned
	'		''' to the built state </exception>
	'		@Override  Sub  accept(int t);
	'
	'		''' <summary>
	'		''' Adds an element to the stream being built.
	'		''' 
	'		''' @implSpec
	'		''' The default implementation behaves as if:
	'		''' <pre>{@code
	'		'''     accept(t)
	'		'''     return this;
	'		''' }</pre>
	'		''' </summary>
	'		''' <param name="t"> the element to add </param>
	'		''' <returns> {@code this} builder </returns>
	'		''' <exception cref="IllegalStateException"> if the builder has already transitioned
	'		''' to the built state </exception>
	'		default Builder add(int t)
	'		{
	'			accept(t);
	'			Return Me;
	'		}
	'
	'		''' <summary>
	'		''' Builds the stream, transitioning this builder to the built state.
	'		''' An {@code IllegalStateException} is thrown if there are further
	'		''' attempts to operate on the builder after it has entered the built
	'		''' state.
	'		''' </summary>
	'		''' <returns> the built stream </returns>
	'		''' <exception cref="IllegalStateException"> if the builder has already transitioned to
	'		''' the built state </exception>
	'		IntStream build();
	'	}
	End Interface

End Namespace
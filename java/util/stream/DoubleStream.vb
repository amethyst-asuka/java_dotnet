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
	''' A sequence of primitive double-valued elements supporting sequential and parallel
	''' aggregate operations.  This is the {@code double} primitive specialization of
	''' <seealso cref="Stream"/>.
	''' 
	''' <p>The following example illustrates an aggregate operation using
	''' <seealso cref="Stream"/> and <seealso cref="DoubleStream"/>, computing the sum of the weights of the
	''' red widgets:
	''' 
	''' <pre>{@code
	'''     double sum = widgets.stream()
	'''                         .filter(w -> w.getColor() == RED)
	'''                         .mapToDouble(w -> w.getWeight())
	'''                         .sum();
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
	Public Interface DoubleStream
		Inherits BaseStream(Of Double?, DoubleStream)

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
		Function filter(  predicate As java.util.function.DoublePredicate) As DoubleStream

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
		Function map(  mapper As java.util.function.DoubleUnaryOperator) As DoubleStream

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
		 Function mapToObj(Of U, T1 As U)(  mapper As java.util.function.DoubleFunction(Of T1)) As Stream(Of U)

		''' <summary>
		''' Returns an {@code IntStream} consisting of the results of applying the
		''' given function to the elements of this stream.
		''' 
		''' <p>This is an <a href="package-summary.html#StreamOps">intermediate
		''' operation</a>.
		''' </summary>
		''' <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		'''               <a href="package-summary.html#Statelessness">stateless</a>
		'''               function to apply to each element </param>
		''' <returns> the new stream </returns>
		Function mapToInt(  mapper As java.util.function.DoubleToIntFunction) As IntStream

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
		Function mapToLong(  mapper As java.util.function.DoubleToLongFunction) As LongStream

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
		'''               function to apply to each element which produces a
		'''               {@code DoubleStream} of new values </param>
		''' <returns> the new stream </returns>
		''' <seealso cref= Stream#flatMap(Function) </seealso>
		Function flatMap(Of T1 As DoubleStream)(  mapper As java.util.function.DoubleFunction(Of T1)) As DoubleStream

		''' <summary>
		''' Returns a stream consisting of the distinct elements of this stream. The
		''' elements are compared for equality according to
		''' <seealso cref="java.lang.Double#compare(double, double)"/>.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">stateful
		''' intermediate operation</a>.
		''' </summary>
		''' <returns> the result stream </returns>
		Function distinct() As DoubleStream

		''' <summary>
		''' Returns a stream consisting of the elements of this stream in sorted
		''' order. The elements are compared for equality according to
		''' <seealso cref="java.lang.Double#compare(double, double)"/>.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">stateful
		''' intermediate operation</a>.
		''' </summary>
		''' <returns> the result stream </returns>
		Function sorted() As DoubleStream

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
		'''     DoubleStream.of(1, 2, 3, 4)
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
		Function peek(  action As java.util.function.DoubleConsumer) As DoubleStream

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
		''' stream source (such as <seealso cref="#generate(DoubleSupplier)"/>) or removing the
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
		Function limit(  maxSize As Long) As DoubleStream

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
		''' stream source (such as <seealso cref="#generate(DoubleSupplier)"/>) or removing the
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
		Function skip(  n As Long) As DoubleStream

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
		Sub forEach(  action As java.util.function.DoubleConsumer)

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
		''' <seealso cref= #forEach(DoubleConsumer) </seealso>
		Sub forEachOrdered(  action As java.util.function.DoubleConsumer)

		''' <summary>
		''' Returns an array containing the elements of this stream.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' </summary>
		''' <returns> an array containing the elements of this stream </returns>
		Function toArray() As Double()

		''' <summary>
		''' Performs a <a href="package-summary.html#Reduction">reduction</a> on the
		''' elements of this stream, using the provided identity value and an
		''' <a href="package-summary.html#Associativity">associative</a>
		''' accumulation function, and returns the reduced value.  This is equivalent
		''' to:
		''' <pre>{@code
		'''     double result = identity;
		'''     for (double element : this stream)
		'''         result = accumulator.applyAsDouble(result, element)
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
		'''     double sum = numbers.reduce(0, (a, b) -> a+b);
		''' }</pre>
		''' 
		''' or more compactly:
		''' 
		''' <pre>{@code
		'''     double sum = numbers.reduce(0, Double::sum);
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
		Function reduce(  identity As Double,   op As java.util.function.DoubleBinaryOperator) As Double

		''' <summary>
		''' Performs a <a href="package-summary.html#Reduction">reduction</a> on the
		''' elements of this stream, using an
		''' <a href="package-summary.html#Associativity">associative</a> accumulation
		''' function, and returns an {@code OptionalDouble} describing the reduced
		''' value, if any. This is equivalent to:
		''' <pre>{@code
		'''     boolean foundAny = false;
		'''     double result = null;
		'''     for (double element : this stream) {
		'''         if (!foundAny) {
		'''             foundAny = true;
		'''             result = element;
		'''         }
		'''         else
		'''             result = accumulator.applyAsDouble(result, element);
		'''     }
		'''     return foundAny ? OptionalDouble.of(result) : OptionalDouble.empty();
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
		''' <seealso cref= #reduce(double, DoubleBinaryOperator) </seealso>
		Function reduce(  op As java.util.function.DoubleBinaryOperator) As java.util.OptionalDouble

		''' <summary>
		''' Performs a <a href="package-summary.html#MutableReduction">mutable
		''' reduction</a> operation on the elements of this stream.  A mutable
		''' reduction is one in which the reduced value is a mutable result container,
		''' such as an {@code ArrayList}, and elements are incorporated by updating
		''' the state of the result rather than by replacing the result.  This
		''' produces a result equivalent to:
		''' <pre>{@code
		'''     R result = supplier.get();
		'''     for (double element : this stream)
		'''         accumulator.accept(result, element);
		'''     return result;
		''' }</pre>
		''' 
		''' <p>Like <seealso cref="#reduce(double, DoubleBinaryOperator)"/>, {@code collect}
		''' operations can be parallelized without requiring additional
		''' synchronization.
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
		 Function collect(Of R)(  supplier As java.util.function.Supplier(Of R),   accumulator As java.util.function.ObjDoubleConsumer(Of R),   combiner As java.util.function.BiConsumer(Of R, R)) As R

		''' <summary>
		''' Returns the sum of elements in this stream.
		''' 
		''' Summation is a special case of a <a
		''' href="package-summary.html#Reduction">reduction</a>. If
		''' floating-point summation were exact, this method would be
		''' equivalent to:
		''' 
		''' <pre>{@code
		'''     return reduce(0, Double::sum);
		''' }</pre>
		''' 
		''' However, since floating-point summation is not exact, the above
		''' code is not necessarily equivalent to the summation computation
		''' done by this method.
		''' 
		''' <p>If any stream element is a NaN or the sum is at any point a NaN
		''' then the sum will be NaN.
		''' 
		''' The value of a floating-point sum is a function both
		''' of the input values as well as the order of addition
		''' operations. The order of addition operations of this method is
		''' intentionally not defined to allow for implementation
		''' flexibility to improve the speed and accuracy of the computed
		''' result.
		''' 
		''' In particular, this method may be implemented using compensated
		''' summation or other technique to reduce the error bound in the
		''' numerical sum compared to a simple summation of {@code double}
		''' values.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' 
		''' @apiNote Elements sorted by increasing absolute magnitude tend
		''' to yield more accurate results.
		''' </summary>
		''' <returns> the sum of elements in this stream </returns>
		Function sum() As Double

		''' <summary>
		''' Returns an {@code OptionalDouble} describing the minimum element of this
		''' stream, or an empty OptionalDouble if this stream is empty.  The minimum
		''' element will be {@code java.lang.[Double].NaN} if any stream element was NaN. Unlike
		''' the numerical comparison operators, this method considers negative zero
		''' to be strictly smaller than positive zero. This is a special case of a
		''' <a href="package-summary.html#Reduction">reduction</a> and is
		''' equivalent to:
		''' <pre>{@code
		'''     return reduce(Double::min);
		''' }</pre>
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' </summary>
		''' <returns> an {@code OptionalDouble} containing the minimum element of this
		''' stream, or an empty optional if the stream is empty </returns>
		Function min() As java.util.OptionalDouble

		''' <summary>
		''' Returns an {@code OptionalDouble} describing the maximum element of this
		''' stream, or an empty OptionalDouble if this stream is empty.  The maximum
		''' element will be {@code java.lang.[Double].NaN} if any stream element was NaN. Unlike
		''' the numerical comparison operators, this method considers negative zero
		''' to be strictly smaller than positive zero. This is a
		''' special case of a
		''' <a href="package-summary.html#Reduction">reduction</a> and is
		''' equivalent to:
		''' <pre>{@code
		'''     return reduce(Double::max);
		''' }</pre>
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' </summary>
		''' <returns> an {@code OptionalDouble} containing the maximum element of this
		''' stream, or an empty optional if the stream is empty </returns>
		Function max() As java.util.OptionalDouble

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
		''' Returns an {@code OptionalDouble} describing the arithmetic
		''' mean of elements of this stream, or an empty optional if this
		''' stream is empty.
		''' 
		''' If any recorded value is a NaN or the sum is at any point a NaN
		''' then the average will be NaN.
		''' 
		''' <p>The average returned can vary depending upon the order in
		''' which values are recorded.
		''' 
		''' This method may be implemented using compensated summation or
		''' other technique to reduce the error bound in the {@link #sum
		''' numerical sum} used to compute the average.
		''' 
		'''  <p>The average is a special case of a <a
		'''  href="package-summary.html#Reduction">reduction</a>.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' 
		''' @apiNote Elements sorted by increasing absolute magnitude tend
		''' to yield more accurate results.
		''' </summary>
		''' <returns> an {@code OptionalDouble} containing the average element of this
		''' stream, or an empty optional if the stream is empty </returns>
		Function average() As java.util.OptionalDouble

		''' <summary>
		''' Returns a {@code DoubleSummaryStatistics} describing various summary data
		''' about the elements of this stream.  This is a special
		''' case of a <a href="package-summary.html#Reduction">reduction</a>.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">terminal
		''' operation</a>.
		''' </summary>
		''' <returns> a {@code DoubleSummaryStatistics} describing various summary data
		''' about the elements of this stream </returns>
		Function summaryStatistics() As java.util.DoubleSummaryStatistics

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
		Function anyMatch(  predicate As java.util.function.DoublePredicate) As Boolean

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
		Function allMatch(  predicate As java.util.function.DoublePredicate) As Boolean

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
		Function noneMatch(  predicate As java.util.function.DoublePredicate) As Boolean

		''' <summary>
		''' Returns an <seealso cref="OptionalDouble"/> describing the first element of this
		''' stream, or an empty {@code OptionalDouble} if the stream is empty.  If
		''' the stream has no encounter order, then any element may be returned.
		''' 
		''' <p>This is a <a href="package-summary.html#StreamOps">short-circuiting
		''' terminal operation</a>.
		''' </summary>
		''' <returns> an {@code OptionalDouble} describing the first element of this
		''' stream, or an empty {@code OptionalDouble} if the stream is empty </returns>
		Function findFirst() As java.util.OptionalDouble

		''' <summary>
		''' Returns an <seealso cref="OptionalDouble"/> describing some element of the stream,
		''' or an empty {@code OptionalDouble} if the stream is empty.
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
		''' <returns> an {@code OptionalDouble} describing some element of this stream,
		''' or an empty {@code OptionalDouble} if the stream is empty </returns>
		''' <seealso cref= #findFirst() </seealso>
		Function findAny() As java.util.OptionalDouble

		''' <summary>
		''' Returns a {@code Stream} consisting of the elements of this stream,
		''' boxed to {@code Double}.
		''' 
		''' <p>This is an <a href="package-summary.html#StreamOps">intermediate
		''' operation</a>.
		''' </summary>
		''' <returns> a {@code Stream} consistent of the elements of this stream,
		''' each boxed to a {@code Double} </returns>
		Function boxed() As Stream(Of Double?)

		Overrides Function sequential() As DoubleStream

		Overrides Function parallel() As DoubleStream

		Overrides Function [iterator]() As java.util.PrimitiveIterator.OfDouble

		Overrides Function spliterator() As java.util.Spliterator.OfDouble


		' Static factories

		''' <summary>
		''' Returns a builder for a {@code DoubleStream}.
		''' </summary>
		''' <returns> a stream builder </returns>
		Shared Function builder() As Builder
			Return Function Streams.DoubleStreamBuilderImpl() As New

		''' <summary>
		''' Returns an empty sequential {@code DoubleStream}.
		''' </summary>
		''' <returns> an empty sequential stream </returns>
		Shared Function empty() As DoubleStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return StreamSupport.doubleStream(java.util.Spliterators.emptyDoubleSpliterator(), False);

		''' <summary>
		''' Returns a sequential {@code DoubleStream} containing a single element.
		''' </summary>
		''' <param name="t"> the single element </param>
		''' <returns> a singleton sequential stream </returns>
		Shared Function [of](  t As Double) As DoubleStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return StreamSupport.doubleStream(New Streams.DoubleStreamBuilderImpl(t), False);

		''' <summary>
		''' Returns a sequential ordered stream whose elements are the specified values.
		''' </summary>
		''' <param name="values"> the elements of the new stream </param>
		''' <returns> the new stream </returns>
		Shared Function [of](ParamArray   values As Double()) As DoubleStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return java.util.Arrays.stream(values);

		''' <summary>
		''' Returns an infinite sequential ordered {@code DoubleStream} produced by iterative
		''' application of a function {@code f} to an initial element {@code seed},
		''' producing a {@code Stream} consisting of {@code seed}, {@code f(seed)},
		''' {@code f(f(seed))}, etc.
		''' 
		''' <p>The first element (position {@code 0}) in the {@code DoubleStream}
		''' will be the provided {@code seed}.  For {@code n > 0}, the element at
		''' position {@code n}, will be the result of applying the function {@code f}
		'''  to the element at position {@code n - 1}.
		''' </summary>
		''' <param name="seed"> the initial element </param>
		''' <param name="f"> a function to be applied to to the previous element to produce
		'''          a new element </param>
		''' <returns> a new sequential {@code DoubleStream} </returns>
		Shared Function iterate(  seed As Double,   f As java.util.function.DoubleUnaryOperator) As DoubleStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(f);
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			final java.util.PrimitiveIterator.OfDouble iterator = New java.util.PrimitiveIterator.OfDouble()
	'		{
	'			double t = seed;
	'
	'			@Override public boolean hasNext()
	'			{
	'				Return True;
	'			}
	'
	'			@Override public double nextDouble()
	'			{
	'				double v = t;
	'				t = f.applyAsDouble(t);
	'				Return v;
	'			}
	'		};
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return StreamSupport.doubleStream(java.util.Spliterators.spliteratorUnknownSize(iterator, java.util.Spliterator.ORDERED | java.util.Spliterator.IMMUTABLE | java.util.Spliterator.NONNULL), False);

		''' <summary>
		''' Returns an infinite sequential unordered stream where each element is
		''' generated by the provided {@code DoubleSupplier}.  This is suitable for
		''' generating constant streams, streams of random elements, etc.
		''' </summary>
		''' <param name="s"> the {@code DoubleSupplier} for generated elements </param>
		''' <returns> a new infinite sequential unordered {@code DoubleStream} </returns>
		Shared Function generate(  s As java.util.function.DoubleSupplier) As DoubleStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(s);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return StreamSupport.doubleStream(New StreamSpliterators.InfiniteSupplyingSpliterator.OfDouble(Long.MAX_VALUE, s), False);

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
		Shared Function concat(  a As DoubleStream,   b As DoubleStream) As DoubleStream
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(a);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(b);

'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			java.util.Spliterator.OfDouble split = New Streams.ConcatSpliterator.OfDouble(a.spliterator(), b.spliterator());
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			DoubleStream stream = StreamSupport.doubleStream(split, a.isParallel() || b.isParallel());
			Function stream.onClose(Streams.composedClose(a, b)    As ) As [Return]

		''' <summary>
		''' A mutable builder for a {@code DoubleStream}.
		''' 
		''' <p>A stream builder has a lifecycle, which starts in a building
		''' phase, during which elements can be added, and then transitions to a built
		''' phase, after which elements may not be added.  The built phase
		''' begins when the <seealso cref="#build()"/> method is called, which creates an
		''' ordered stream whose elements are the elements that were added to the
		''' stream builder, in the order they were added.
		''' </summary>
		''' <seealso cref= DoubleStream#builder()
		''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public interface Builder extends java.util.function.DoubleConsumer
	'	{
	'
	'		''' <summary>
	'		''' Adds an element to the stream being built.
	'		''' </summary>
	'		''' <exception cref="IllegalStateException"> if the builder has already transitioned
	'		''' to the built state </exception>
	'		@Override  Sub  accept(double t);
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
	'		default Builder add(double t)
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
	'		''' <exception cref="IllegalStateException"> if the builder has already transitioned
	'		''' to the built state </exception>
	'		DoubleStream build();
	'	}
	End Interface

End Namespace
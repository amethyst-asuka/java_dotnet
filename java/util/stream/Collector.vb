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
	''' A <a href="package-summary.html#Reduction">mutable reduction operation</a> that
	''' accumulates input elements into a mutable result container, optionally transforming
	''' the accumulated result into a final representation after all input elements
	''' have been processed.  Reduction operations can be performed either sequentially
	''' or in parallel.
	''' 
	''' <p>Examples of mutable reduction operations include:
	''' accumulating elements into a {@code Collection}; concatenating
	''' strings using a {@code StringBuilder}; computing summary information about
	''' elements such as sum, min, max, or average; computing "pivot table" summaries
	''' such as "maximum valued transaction by seller", etc.  The class <seealso cref="Collectors"/>
	''' provides implementations of many common mutable reductions.
	''' 
	''' <p>A {@code Collector} is specified by four functions that work together to
	''' accumulate entries into a mutable result container, and optionally perform
	''' a final transform on the result.  They are: <ul>
	'''     <li>creation of a new result container (<seealso cref="#supplier()"/>)</li>
	'''     <li>incorporating a new data element into a result container (<seealso cref="#accumulator()"/>)</li>
	'''     <li>combining two result containers into one (<seealso cref="#combiner()"/>)</li>
	'''     <li>performing an optional final transform on the container (<seealso cref="#finisher()"/>)</li>
	''' </ul>
	''' 
	''' <p>Collectors also have a set of characteristics, such as
	''' <seealso cref="Characteristics#CONCURRENT"/>, that provide hints that can be used by a
	''' reduction implementation to provide better performance.
	''' 
	''' <p>A sequential implementation of a reduction using a collector would
	''' create a single result container using the supplier function, and invoke the
	''' accumulator function once for each input element.  A parallel implementation
	''' would partition the input, create a result container for each partition,
	''' accumulate the contents of each partition into a subresult for that partition,
	''' and then use the combiner function to merge the subresults into a combined
	''' result.
	''' 
	''' <p>To ensure that sequential and parallel executions produce equivalent
	''' results, the collector functions must satisfy an <em>identity</em> and an
	''' <a href="package-summary.html#Associativity">associativity</a> constraints.
	''' 
	''' <p>The identity constraint says that for any partially accumulated result,
	''' combining it with an empty result container must produce an equivalent
	''' result.  That is, for a partially accumulated result {@code a} that is the
	''' result of any series of accumulator and combiner invocations, {@code a} must
	''' be equivalent to {@code combiner.apply(a, supplier.get())}.
	''' 
	''' <p>The associativity constraint says that splitting the computation must
	''' produce an equivalent result.  That is, for any input elements {@code t1}
	''' and {@code t2}, the results {@code r1} and {@code r2} in the computation
	''' below must be equivalent:
	''' <pre>{@code
	'''     A a1 = supplier.get();
	'''     accumulator.accept(a1, t1);
	'''     accumulator.accept(a1, t2);
	'''     R r1 = finisher.apply(a1);  // result without splitting
	''' 
	'''     A a2 = supplier.get();
	'''     accumulator.accept(a2, t1);
	'''     A a3 = supplier.get();
	'''     accumulator.accept(a3, t2);
	'''     R r2 = finisher.apply(combiner.apply(a2, a3));  // result with splitting
	''' } </pre>
	''' 
	''' <p>For collectors that do not have the {@code UNORDERED} characteristic,
	''' two accumulated results {@code a1} and {@code a2} are equivalent if
	''' {@code finisher.apply(a1).equals(finisher.apply(a2))}.  For unordered
	''' collectors, equivalence is relaxed to allow for non-equality related to
	''' differences in order.  (For example, an unordered collector that accumulated
	''' elements to a {@code List} would consider two lists equivalent if they
	''' contained the same elements, ignoring order.)
	''' 
	''' <p>Libraries that implement reduction based on {@code Collector}, such as
	''' <seealso cref="Stream#collect(Collector)"/>, must adhere to the following constraints:
	''' <ul>
	'''     <li>The first argument passed to the accumulator function, both
	'''     arguments passed to the combiner function, and the argument passed to the
	'''     finisher function must be the result of a previous invocation of the
	'''     result supplier, accumulator, or combiner functions.</li>
	'''     <li>The implementation should not do anything with the result of any of
	'''     the result supplier, accumulator, or combiner functions other than to
	'''     pass them again to the accumulator, combiner, or finisher functions,
	'''     or return them to the caller of the reduction operation.</li>
	'''     <li>If a result is passed to the combiner or finisher
	'''     function, and the same object is not returned from that function, it is
	'''     never used again.</li>
	'''     <li>Once a result is passed to the combiner or finisher function, it
	'''     is never passed to the accumulator function again.</li>
	'''     <li>For non-concurrent collectors, any result returned from the result
	'''     supplier, accumulator, or combiner functions must be serially
	'''     thread-confined.  This enables collection to occur in parallel without
	'''     the {@code Collector} needing to implement any additional synchronization.
	'''     The reduction implementation must manage that the input is properly
	'''     partitioned, that partitions are processed in isolation, and combining
	'''     happens only after accumulation is complete.</li>
	'''     <li>For concurrent collectors, an implementation is free to (but not
	'''     required to) implement reduction concurrently.  A concurrent reduction
	'''     is one where the accumulator function is called concurrently from
	'''     multiple threads, using the same concurrently-modifiable result container,
	'''     rather than keeping the result isolated during accumulation.
	'''     A concurrent reduction should only be applied if the collector has the
	'''     <seealso cref="Characteristics#UNORDERED"/> characteristics or if the
	'''     originating data is unordered.</li>
	''' </ul>
	''' 
	''' <p>In addition to the predefined implementations in <seealso cref="Collectors"/>, the
	''' static factory methods <seealso cref="#of(Supplier, BiConsumer, BinaryOperator, Characteristics...)"/>
	''' can be used to construct collectors.  For example, you could create a collector
	''' that accumulates widgets into a {@code TreeSet} with:
	''' 
	''' <pre>{@code
	'''     Collector<Widget, ?, TreeSet<Widget>> intoSet =
	'''         Collector.of(TreeSet::new, TreeSet::add,
	'''                      (left, right) -> { left.addAll(right); return left; });
	''' }</pre>
	''' 
	''' (This behavior is also implemented by the predefined collector
	''' <seealso cref="Collectors#toCollection(Supplier)"/>).
	''' 
	''' @apiNote
	''' Performing a reduction operation with a {@code Collector} should produce a
	''' result equivalent to:
	''' <pre>{@code
	'''     R container = collector.supplier().get();
	'''     for (T t : data)
	'''         collector.accumulator().accept(container, t);
	'''     return collector.finisher().apply(container);
	''' }</pre>
	''' 
	''' <p>However, the library is free to partition the input, perform the reduction
	''' on the partitions, and then use the combiner function to combine the partial
	''' results to achieve a parallel reduction.  (Depending on the specific reduction
	''' operation, this may perform better or worse, depending on the relative cost
	''' of the accumulator and combiner functions.)
	''' 
	''' <p>Collectors are designed to be <em>composed</em>; many of the methods
	''' in <seealso cref="Collectors"/> are functions that take a collector and produce
	''' a new collector.  For example, given the following collector that computes
	''' the sum of the salaries of a stream of employees:
	''' 
	''' <pre>{@code
	'''     Collector<Employee, ?, Integer> summingSalaries
	'''         = Collectors.summingInt(Employee::getSalary))
	''' }</pre>
	''' 
	''' If we wanted to create a collector to tabulate the sum of salaries by
	''' department, we could reuse the "sum of salaries" logic using
	''' <seealso cref="Collectors#groupingBy(Function, Collector)"/>:
	''' 
	''' <pre>{@code
	'''     Collector<Employee, ?, Map<Department, Integer>> summingSalariesByDept
	'''         = Collectors.groupingBy(Employee::getDepartment, summingSalaries);
	''' }</pre>
	''' </summary>
	''' <seealso cref= Stream#collect(Collector) </seealso>
	''' <seealso cref= Collectors
	''' </seealso>
	''' @param <T> the type of input elements to the reduction operation </param>
	''' @param <A> the mutable accumulation type of the reduction operation (often
	'''            hidden as an implementation detail) </param>
	''' @param <R> the result type of the reduction operation
	''' @since 1.8 </param>
	Public Interface Collector(Of T, A, R)
		''' <summary>
		''' A function that creates and returns a new mutable result container.
		''' </summary>
		''' <returns> a function which returns a new, mutable result container </returns>
		Function supplier() As java.util.function.Supplier(Of A)

		''' <summary>
		''' A function that folds a value into a mutable result container.
		''' </summary>
		''' <returns> a function which folds a value into a mutable result container </returns>
		Function accumulator() As java.util.function.BiConsumer(Of A, T)

		''' <summary>
		''' A function that accepts two partial results and merges them.  The
		''' combiner function may fold state from one argument into the other and
		''' return that, or may return a new result container.
		''' </summary>
		''' <returns> a function which combines two partial results into a combined
		''' result </returns>
		Function combiner() As java.util.function.BinaryOperator(Of A)

		''' <summary>
		''' Perform the final transformation from the intermediate accumulation type
		''' {@code A} to the final result type {@code R}.
		''' 
		''' <p>If the characteristic {@code IDENTITY_TRANSFORM} is
		''' set, this function may be presumed to be an identity transform with an
		''' unchecked cast from {@code A} to {@code R}.
		''' </summary>
		''' <returns> a function which transforms the intermediate result to the final
		''' result </returns>
		Function finisher() As java.util.function.Function(Of A, R)

		''' <summary>
		''' Returns a {@code Set} of {@code Collector.Characteristics} indicating
		''' the characteristics of this Collector.  This set should be immutable.
		''' </summary>
		''' <returns> an immutable set of collector characteristics </returns>
		Function characteristics() As java.util.Set(Of Characteristics)

		''' <summary>
		''' Returns a new {@code Collector} described by the given {@code supplier},
		''' {@code accumulator}, and {@code combiner} functions.  The resulting
		''' {@code Collector} has the {@code Collector.Characteristics.IDENTITY_FINISH}
		''' characteristic.
		''' </summary>
		''' <param name="supplier"> The supplier function for the new collector </param>
		''' <param name="accumulator"> The accumulator function for the new collector </param>
		''' <param name="combiner"> The combiner function for the new collector </param>
		''' <param name="characteristics"> The collector characteristics for the new
		'''                        collector </param>
		''' @param <T> The type of input elements for the new collector </param>
		''' @param <R> The type of intermediate accumulation result, and final result,
		'''           for the new collector </param>
		''' <exception cref="NullPointerException"> if any argument is null </exception>
		''' <returns> the new {@code Collector} </returns>
		Shared Function [of](Of T, R)(  supplier As java.util.function.Supplier(Of R),   accumulator As java.util.function.BiConsumer(Of R, T),   combiner As java.util.function.BinaryOperator(Of R), ParamArray   characteristics As Characteristics()) As Collector(Of T, R, R)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(supplier);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(accumulator);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(combiner);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(characteristics);
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			java.util.Set(Of Characteristics) cs = (characteristics.length == 0) ? Collectors.CH_ID : java.util.Collections.unmodifiableSet(java.util.EnumSet.of(Collector.Characteristics.IDENTITY_FINISH, characteristics));
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return New Collectors.CollectorImpl<>(supplier, accumulator, combiner, cs);

		''' <summary>
		''' Returns a new {@code Collector} described by the given {@code supplier},
		''' {@code accumulator}, {@code combiner}, and {@code finisher} functions.
		''' </summary>
		''' <param name="supplier"> The supplier function for the new collector </param>
		''' <param name="accumulator"> The accumulator function for the new collector </param>
		''' <param name="combiner"> The combiner function for the new collector </param>
		''' <param name="finisher"> The finisher function for the new collector </param>
		''' <param name="characteristics"> The collector characteristics for the new
		'''                        collector </param>
		''' @param <T> The type of input elements for the new collector </param>
		''' @param <A> The intermediate accumulation type of the new collector </param>
		''' @param <R> The final result type of the new collector </param>
		''' <exception cref="NullPointerException"> if any argument is null </exception>
		''' <returns> the new {@code Collector} </returns>
		Shared Function [of](Of T, A, R)(  supplier As java.util.function.Supplier(Of A),   accumulator As java.util.function.BiConsumer(Of A, T),   combiner As java.util.function.BinaryOperator(Of A),   finisher As java.util.function.Function(Of A, R), ParamArray   characteristics As Characteristics()) As Collector(Of T, A, R)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(supplier);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(accumulator);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(combiner);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(finisher);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(characteristics);
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			java.util.Set(Of Characteristics) cs = Collectors.CH_NOID;
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			if (characteristics.length > 0)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				cs = java.util.EnumSet.noneOf(Characteristics.class);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				java.util.Collections.addAll(cs, characteristics);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				cs = java.util.Collections.unmodifiableSet(cs);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return New Collectors.CollectorImpl<>(supplier, accumulator, combiner, finisher, cs);

		''' <summary>
		''' Characteristics indicating properties of a {@code Collector}, which can
		''' be used to optimize reduction implementations.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		enum Characteristics
	'	{
	'		''' <summary>
	'		''' Indicates that this collector is <em>concurrent</em>, meaning that
	'		''' the result container can support the accumulator function being
	'		''' called concurrently with the same result container from multiple
	'		''' threads.
	'		''' 
	'		''' <p>If a {@code CONCURRENT} collector is not also {@code UNORDERED},
	'		''' then it should only be evaluated concurrently if applied to an
	'		''' unordered data source.
	'		''' </summary>
	'		CONCURRENT,
	'
	'		''' <summary>
	'		''' Indicates that the collection operation does not commit to preserving
	'		''' the encounter order of input elements.  (This might be true if the
	'		''' result container has no intrinsic order, such as a <seealso cref="Set"/>.)
	'		''' </summary>
	'		UNORDERED,
	'
	'		''' <summary>
	'		''' Indicates that the finisher function is the identity function and
	'		''' can be elided.  If set, it must be the case that an unchecked cast
	'		''' from A to R will succeed.
	'		''' </summary>
	'		IDENTITY_FINISH
	'	}
	End Interface

End Namespace
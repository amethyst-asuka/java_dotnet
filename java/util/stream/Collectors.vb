Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Concurrent

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
	''' Implementations of <seealso cref="Collector"/> that implement various useful reduction
	''' operations, such as accumulating elements into collections, summarizing
	''' elements according to various criteria, etc.
	''' 
	''' <p>The following are examples of using the predefined collectors to perform
	''' common mutable reduction tasks:
	''' 
	''' <pre>{@code
	'''     // Accumulate names into a List
	'''     List<String> list = people.stream().map(Person::getName).collect(Collectors.toList());
	''' 
	'''     // Accumulate names into a TreeSet
	'''     Set<String> set = people.stream().map(Person::getName).collect(Collectors.toCollection(TreeSet::new));
	''' 
	'''     // Convert elements to strings and concatenate them, separated by commas
	'''     String joined = things.stream()
	'''                           .map(Object::toString)
	'''                           .collect(Collectors.joining(", "));
	''' 
	'''     // Compute sum of salaries of employee
	'''     int total = employees.stream()
	'''                          .collect(Collectors.summingInt(Employee::getSalary)));
	''' 
	'''     // Group employees by department
	'''     Map<Department, List<Employee>> byDept
	'''         = employees.stream()
	'''                    .collect(Collectors.groupingBy(Employee::getDepartment));
	''' 
	'''     // Compute sum of salaries by department
	'''     Map<Department, Integer> totalByDept
	'''         = employees.stream()
	'''                    .collect(Collectors.groupingBy(Employee::getDepartment,
	'''                                                   Collectors.summingInt(Employee::getSalary)));
	''' 
	'''     // Partition students into passing and failing
	'''     Map<Boolean, List<Student>> passingFailing =
	'''         students.stream()
	'''                 .collect(Collectors.partitioningBy(s -> s.getGrade() >= PASS_THRESHOLD));
	''' 
	''' }</pre>
	''' 
	''' @since 1.8
	''' </summary>
	Public NotInheritable Class Collectors

		Friend Shared ReadOnly CH_CONCURRENT_ID As java.util.Set(Of Collector.Characteristics) = java.util.Collections.unmodifiableSet(java.util.EnumSet.of(Collector.Characteristics.CONCURRENT, Collector.Characteristics.UNORDERED, Collector.Characteristics.IDENTITY_FINISH))
		Friend Shared ReadOnly CH_CONCURRENT_NOID As java.util.Set(Of Collector.Characteristics) = java.util.Collections.unmodifiableSet(java.util.EnumSet.of(Collector.Characteristics.CONCURRENT, Collector.Characteristics.UNORDERED))
		Friend Shared ReadOnly CH_ID As java.util.Set(Of Collector.Characteristics) = java.util.Collections.unmodifiableSet(java.util.EnumSet.of(Collector.Characteristics.IDENTITY_FINISH))
		Friend Shared ReadOnly CH_UNORDERED_ID As java.util.Set(Of Collector.Characteristics) = java.util.Collections.unmodifiableSet(java.util.EnumSet.of(Collector.Characteristics.UNORDERED, Collector.Characteristics.IDENTITY_FINISH))
		Friend Shared ReadOnly CH_NOID As java.util.Set(Of Collector.Characteristics) = java.util.Collections.emptySet()

		Private Sub New()
		End Sub

		''' <summary>
		''' Returns a merge function, suitable for use in
		''' <seealso cref="Map#merge(Object, Object, BiFunction) Map.merge()"/> or
		''' <seealso cref="#toMap(Function, Function, BinaryOperator) toMap()"/>, which always
		''' throws {@code IllegalStateException}.  This can be used to enforce the
		''' assumption that the elements being collected are distinct.
		''' </summary>
		''' @param <T> the type of input arguments to the merge function </param>
		''' <returns> a merge function which always throw {@code IllegalStateException} </returns>
		Private Shared Function throwingMerger(Of T)() As java.util.function.BinaryOperator(Of T)
			Return (u,v) ->
				Throw New IllegalStateException(String.Format("Duplicate key {0}", u))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared Function castingIdentity(Of I, R)() As java.util.function.Function(Of I, R)
			Return i -> (R) i
		End Function

		''' <summary>
		''' Simple implementation class for {@code Collector}.
		''' </summary>
		''' @param <T> the type of elements to be collected </param>
		''' @param <R> the type of the result </param>
		Friend Class CollectorImpl(Of T, A, R)
			Implements Collector(Of T, A, R)

			Private ReadOnly supplier_Renamed As java.util.function.Supplier(Of A)
			Private ReadOnly accumulator_Renamed As java.util.function.BiConsumer(Of A, T)
			Private ReadOnly combiner_Renamed As java.util.function.BinaryOperator(Of A)
			Private ReadOnly finisher_Renamed As java.util.function.Function(Of A, R)
			Private ReadOnly characteristics_Renamed As java.util.Set(Of Characteristics)

			Friend Sub New(ByVal supplier As java.util.function.Supplier(Of A), ByVal accumulator As java.util.function.BiConsumer(Of A, T), ByVal combiner As java.util.function.BinaryOperator(Of A), ByVal finisher As java.util.function.Function(Of A, R), ByVal characteristics As java.util.Set(Of Characteristics))
				Me.supplier_Renamed = supplier
				Me.accumulator_Renamed = accumulator
				Me.combiner_Renamed = combiner
				Me.finisher_Renamed = finisher
				Me.characteristics_Renamed = characteristics
			End Sub

			Friend Sub New(ByVal supplier As java.util.function.Supplier(Of A), ByVal accumulator As java.util.function.BiConsumer(Of A, T), ByVal combiner As java.util.function.BinaryOperator(Of A), ByVal characteristics As java.util.Set(Of Characteristics))
				Me.New(supplier, accumulator, combiner, castingIdentity(), characteristics)
			End Sub

			Public Overrides Function accumulator() As java.util.function.BiConsumer(Of A, T) Implements Collector(Of T, A, R).accumulator
				Return accumulator_Renamed
			End Function

			Public Overrides Function supplier() As java.util.function.Supplier(Of A) Implements Collector(Of T, A, R).supplier
				Return supplier_Renamed
			End Function

			Public Overrides Function combiner() As java.util.function.BinaryOperator(Of A) Implements Collector(Of T, A, R).combiner
				Return combiner_Renamed
			End Function

			Public Overrides Function finisher() As java.util.function.Function(Of A, R) Implements Collector(Of T, A, R).finisher
				Return finisher_Renamed
			End Function

			Public Overrides Function characteristics() As java.util.Set(Of Characteristics) Implements Collector(Of T, A, R).characteristics
				Return characteristics_Renamed
			End Function
		End Class

		''' <summary>
		''' Returns a {@code Collector} that accumulates the input elements into a
		''' new {@code Collection}, in encounter order.  The {@code Collection} is
		''' created by the provided factory.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <C> the type of the resulting {@code Collection} </param>
		''' <param name="collectionFactory"> a {@code Supplier} which returns a new, empty
		''' {@code Collection} of the appropriate type </param>
		''' <returns> a {@code Collector} which collects all the input elements into a
		''' {@code Collection}, in encounter order </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function toCollection(Of T, C As ICollection(Of T))(ByVal collectionFactory As java.util.function.Supplier(Of C)) As Collector(Of T, ?, C)
			Return New CollectorImpl(Of )(collectionFactory, ICollection(Of T)::add, (r1, r2) -> { r1.addAll(r2); Return r1; }, CH_ID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} that accumulates the input elements into a
		''' new {@code List}. There are no guarantees on the type, mutability,
		''' serializability, or thread-safety of the {@code List} returned; if more
		''' control over the returned {@code List} is required, use <seealso cref="#toCollection(Supplier)"/>.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <returns> a {@code Collector} which collects all the input elements into a
		''' {@code List}, in encounter order </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function toList(Of T)() As Collector(Of T, ?, IList(Of T))
			Return New CollectorImpl(Of )(CType(ArrayList, java.util.function.Supplier(Of IList(Of T)))::New, IList::add, (left, right) -> { left.addAll(right); Return left; }, CH_ID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} that accumulates the input elements into a
		''' new {@code Set}. There are no guarantees on the type, mutability,
		''' serializability, or thread-safety of the {@code Set} returned; if more
		''' control over the returned {@code Set} is required, use
		''' <seealso cref="#toCollection(Supplier)"/>.
		''' 
		''' <p>This is an <seealso cref="Collector.Characteristics#UNORDERED unordered"/>
		''' Collector.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <returns> a {@code Collector} which collects all the input elements into a
		''' {@code Set} </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function toSet(Of T)() As Collector(Of T, ?, java.util.Set(Of T))
			Return New CollectorImpl(Of )(CType(HashSet, java.util.function.Supplier(Of java.util.Set(Of T)))::New, java.util.Set::add, (left, right) -> { left.addAll(right); Return left; }, CH_UNORDERED_ID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} that concatenates the input elements into a
		''' {@code String}, in encounter order.
		''' </summary>
		''' <returns> a {@code Collector} that concatenates the input elements into a
		''' {@code String}, in encounter order </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function joining() As Collector(Of CharSequence, ?, String)
			Return New CollectorImpl(Of CharSequence, StringBuilder, String)(StringBuilder::New, StringBuilder::append, (r1, r2) -> { r1.append(r2); Return r1; }, StringBuilder::toString, CH_NOID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} that concatenates the input elements,
		''' separated by the specified delimiter, in encounter order.
		''' </summary>
		''' <param name="delimiter"> the delimiter to be used between each element </param>
		''' <returns> A {@code Collector} which concatenates CharSequence elements,
		''' separated by the specified delimiter, in encounter order </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function joining(ByVal delimiter As CharSequence) As Collector(Of CharSequence, ?, String)
			Return joining(delimiter, "", "")
		End Function

		''' <summary>
		''' Returns a {@code Collector} that concatenates the input elements,
		''' separated by the specified delimiter, with the specified prefix and
		''' suffix, in encounter order.
		''' </summary>
		''' <param name="delimiter"> the delimiter to be used between each element </param>
		''' <param name="prefix"> the sequence of characters to be used at the beginning
		'''                of the joined result </param>
		''' <param name="suffix"> the sequence of characters to be used at the end
		'''                of the joined result </param>
		''' <returns> A {@code Collector} which concatenates CharSequence elements,
		''' separated by the specified delimiter, in encounter order </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function joining(ByVal delimiter As CharSequence, ByVal prefix As CharSequence, ByVal suffix As CharSequence) As Collector(Of CharSequence, ?, String)
			Return New CollectorImpl(Of )(() -> New java.util.StringJoiner(delimiter, prefix, suffix), java.util.StringJoiner::add, java.util.StringJoiner::merge, java.util.StringJoiner::toString, CH_NOID)
		End Function

		''' <summary>
		''' {@code BinaryOperator<Map>} that merges the contents of its right
		''' argument into its left argument, using the provided merge function to
		''' handle duplicate keys.
		''' </summary>
		''' @param <K> type of the map keys </param>
		''' @param <V> type of the map values </param>
		''' @param <M> type of the map </param>
		''' <param name="mergeFunction"> A merge function suitable for
		''' <seealso cref="Map#merge(Object, Object, BiFunction) Map.merge()"/> </param>
		''' <returns> a merge function for two maps </returns>
		Private Shared Function mapMerger(Of K, V, M As IDictionary(Of K, V))(ByVal mergeFunction As java.util.function.BinaryOperator(Of V)) As java.util.function.BinaryOperator(Of M)
			Return (m1, m2) ->
				For Each e As KeyValuePair(Of K, V) In m2.entrySet()
					m1.merge(e.Key, e.Value, mergeFunction)
				Next e
				Return m1
		End Function

		''' <summary>
		''' Adapts a {@code Collector} accepting elements of type {@code U} to one
		''' accepting elements of type {@code T} by applying a mapping function to
		''' each input element before accumulation.
		''' 
		''' @apiNote
		''' The {@code mapping()} collectors are most useful when used in a
		''' multi-level reduction, such as downstream of a {@code groupingBy} or
		''' {@code partitioningBy}.  For example, given a stream of
		''' {@code Person}, to accumulate the set of last names in each city:
		''' <pre>{@code
		'''     Map<City, Set<String>> lastNamesByCity
		'''         = people.stream().collect(groupingBy(Person::getCity,
		'''                                              mapping(Person::getLastName, toSet())));
		''' }</pre>
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <U> type of elements accepted by downstream collector </param>
		''' @param <A> intermediate accumulation type of the downstream collector </param>
		''' @param <R> result type of collector </param>
		''' <param name="mapper"> a function to be applied to the input elements </param>
		''' <param name="downstream"> a collector which will accept mapped values </param>
		''' <returns> a collector which applies the mapping function to the input
		''' elements and provides the mapped results to the downstream collector </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function mapping(Of T, U, A, R, T1 As U, T2)(ByVal mapper As java.util.function.Function(Of T1), ByVal downstream As Collector(Of T2)) As Collector(Of T, ?, R)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim downstreamAccumulator As java.util.function.BiConsumer(Of A, ?) = downstream.accumulator()
			Return New CollectorImpl(Of )(downstream.supplier(), (r, t) -> downstreamAccumulator.accept(r, mapper.apply(t)), downstream.combiner(), downstream.finisher(), downstream.characteristics())
		End Function

		''' <summary>
		''' Adapts a {@code Collector} to perform an additional finishing
		''' transformation.  For example, one could adapt the <seealso cref="#toList()"/>
		''' collector to always produce an immutable list with:
		''' <pre>{@code
		'''     List<String> people
		'''         = people.stream().collect(collectingAndThen(toList(), Collections::unmodifiableList));
		''' }</pre>
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <A> intermediate accumulation type of the downstream collector </param>
		''' @param <R> result type of the downstream collector </param>
		''' @param <RR> result type of the resulting collector </param>
		''' <param name="downstream"> a collector </param>
		''' <param name="finisher"> a function to be applied to the final result of the downstream collector </param>
		''' <returns> a collector which performs the action of the downstream collector,
		''' followed by an additional finishing step </returns>
		Public Shared Function collectingAndThen(Of T, A, R, RR)(ByVal downstream As Collector(Of T, A, R), ByVal finisher As java.util.function.Function(Of R, RR)) As Collector(Of T, A, RR)
			Dim characteristics As java.util.Set(Of Collector.Characteristics) = downstream.characteristics()
			If characteristics.contains(Collector.Characteristics.IDENTITY_FINISH) Then
				If characteristics.size() = 1 Then
					characteristics = Collectors.CH_NOID
				Else
					characteristics = java.util.EnumSet.copyOf(characteristics)
					characteristics.remove(Collector.Characteristics.IDENTITY_FINISH)
					characteristics = java.util.Collections.unmodifiableSet(characteristics)
				End If
			End If
			Return New CollectorImpl(Of )(downstream.supplier(), downstream.accumulator(), downstream.combiner(), downstream.finisher().andThen(finisher), characteristics)
		End Function

		''' <summary>
		''' Returns a {@code Collector} accepting elements of type {@code T} that
		''' counts the number of input elements.  If no elements are present, the
		''' result is 0.
		''' 
		''' @implSpec
		''' This produces a result equivalent to:
		''' <pre>{@code
		'''     reducing(0L, e -> 1L, Long::sum)
		''' }</pre>
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <returns> a {@code Collector} that counts the input elements </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function counting(Of T)() As Collector(Of T, ?, Long?)
			Return reducing(0L, e -> 1L, Long?::sum)
		End Function

		''' <summary>
		''' Returns a {@code Collector} that produces the minimal element according
		''' to a given {@code Comparator}, described as an {@code Optional<T>}.
		''' 
		''' @implSpec
		''' This produces a result equivalent to:
		''' <pre>{@code
		'''     reducing(BinaryOperator.minBy(comparator))
		''' }</pre>
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <param name="comparator"> a {@code Comparator} for comparing elements </param>
		''' <returns> a {@code Collector} that produces the minimal value </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function minBy(Of T, T1)(ByVal comparator As IComparer(Of T1)) As Collector(Of T, ?, java.util.Optional(Of T))
			Return reducing(java.util.function.BinaryOperator.minBy(comparator))
		End Function

		''' <summary>
		''' Returns a {@code Collector} that produces the maximal element according
		''' to a given {@code Comparator}, described as an {@code Optional<T>}.
		''' 
		''' @implSpec
		''' This produces a result equivalent to:
		''' <pre>{@code
		'''     reducing(BinaryOperator.maxBy(comparator))
		''' }</pre>
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <param name="comparator"> a {@code Comparator} for comparing elements </param>
		''' <returns> a {@code Collector} that produces the maximal value </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function maxBy(Of T, T1)(ByVal comparator As IComparer(Of T1)) As Collector(Of T, ?, java.util.Optional(Of T))
			Return reducing(java.util.function.BinaryOperator.maxBy(comparator))
		End Function

		''' <summary>
		''' Returns a {@code Collector} that produces the sum of a integer-valued
		''' function applied to the input elements.  If no elements are present,
		''' the result is 0.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <param name="mapper"> a function extracting the property to be summed </param>
		''' <returns> a {@code Collector} that produces the sum of a derived property </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function summingInt(Of T, T1)(ByVal mapper As java.util.function.ToIntFunction(Of T1)) As Collector(Of T, ?, Integer?)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return New CollectorImpl(Of )(() -> New Integer(0){}, (a, t) -> { a(0) += mapper.applyAsInt(t); }, (a, b) -> { a(0) += b(0); Return a; }, a -> a(0), CH_NOID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} that produces the sum of a long-valued
		''' function applied to the input elements.  If no elements are present,
		''' the result is 0.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <param name="mapper"> a function extracting the property to be summed </param>
		''' <returns> a {@code Collector} that produces the sum of a derived property </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function summingLong(Of T, T1)(ByVal mapper As java.util.function.ToLongFunction(Of T1)) As Collector(Of T, ?, Long?)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return New CollectorImpl(Of )(() -> New Long(0){}, (a, t) -> { a(0) += mapper.applyAsLong(t); }, (a, b) -> { a(0) += b(0); Return a; }, a -> a(0), CH_NOID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} that produces the sum of a double-valued
		''' function applied to the input elements.  If no elements are present,
		''' the result is 0.
		''' 
		''' <p>The sum returned can vary depending upon the order in which
		''' values are recorded, due to accumulated rounding error in
		''' addition of values of differing magnitudes. Values sorted by increasing
		''' absolute magnitude tend to yield more accurate results.  If any recorded
		''' value is a {@code NaN} or the sum is at any point a {@code NaN} then the
		''' sum will be {@code NaN}.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <param name="mapper"> a function extracting the property to be summed </param>
		''' <returns> a {@code Collector} that produces the sum of a derived property </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function summingDouble(Of T, T1)(ByVal mapper As java.util.function.ToDoubleFunction(Of T1)) As Collector(Of T, ?, Double?)
	'        
	'         * In the arrays allocated for the collect operation, index 0
	'         * holds the high-order bits of the running sum, index 1 holds
	'         * the low-order bits of the sum computed via compensated
	'         * summation, and index 2 holds the simple sum used to compute
	'         * the proper result if the stream contains infinite values of
	'         * the same sign.
	'         
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return New CollectorImpl(Of )(() -> New Double(2){}, (a, t) -> { sumWithCompensation(a, mapper.applyAsDouble(t)); a(2) += mapper.applyAsDouble(t);}, (a, b) -> { sumWithCompensation(a, b(0)); a(2) += b(2); Return sumWithCompensation(a, b(1)); }, a -> computeFinalSum(a), CH_NOID)
		End Function

		''' <summary>
		''' Incorporate a new double value using Kahan summation /
		''' compensation summation.
		''' 
		''' High-order bits of the sum are in intermediateSum[0], low-order
		''' bits of the sum are in intermediateSum[1], any additional
		''' elements are application-specific.
		''' </summary>
		''' <param name="intermediateSum"> the high-order and low-order words of the intermediate sum </param>
		''' <param name="value"> the name value to be included in the running sum </param>
		Friend Shared Function sumWithCompensation(ByVal intermediateSum As Double(), ByVal value As Double) As Double()
			Dim tmp As Double = value - intermediateSum(1)
			Dim sum As Double = intermediateSum(0)
			Dim velvel As Double = sum + tmp ' Little wolf of rounding error
			intermediateSum(1) = (velvel - sum) - tmp
			intermediateSum(0) = velvel
			Return intermediateSum
		End Function

		''' <summary>
		''' If the compensated sum is spuriously NaN from accumulating one
		''' or more same-signed infinite values, return the
		''' correctly-signed infinity stored in the simple sum.
		''' </summary>
		Friend Shared Function computeFinalSum(ByVal summands As Double()) As Double
			' Better error bounds to add both terms as the final sum
			Dim tmp As Double = summands(0) + summands(1)
			Dim simpleSum As Double = summands(summands.Length - 1)
			If Double.IsNaN(tmp) AndAlso Double.IsInfinity(simpleSum) Then
				Return simpleSum
			Else
				Return tmp
			End If
		End Function

		''' <summary>
		''' Returns a {@code Collector} that produces the arithmetic mean of an integer-valued
		''' function applied to the input elements.  If no elements are present,
		''' the result is 0.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <param name="mapper"> a function extracting the property to be summed </param>
		''' <returns> a {@code Collector} that produces the sum of a derived property </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function averagingInt(Of T, T1)(ByVal mapper As java.util.function.ToIntFunction(Of T1)) As Collector(Of T, ?, Double?)
			Return New CollectorImpl(Of )(() -> New Long(1){}, (a, t) -> { a(0) += mapper.applyAsInt(t); a(1)++; }, (a, b) -> { a(0) += b(0); a(1) += b(1); Return a; },If(a -> (a(1) == 0), 0.0R, CDbl(a(0)) / a(1)), CH_NOID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} that produces the arithmetic mean of a long-valued
		''' function applied to the input elements.  If no elements are present,
		''' the result is 0.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <param name="mapper"> a function extracting the property to be summed </param>
		''' <returns> a {@code Collector} that produces the sum of a derived property </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function averagingLong(Of T, T1)(ByVal mapper As java.util.function.ToLongFunction(Of T1)) As Collector(Of T, ?, Double?)
			Return New CollectorImpl(Of )(() -> New Long(1){}, (a, t) -> { a(0) += mapper.applyAsLong(t); a(1)++; }, (a, b) -> { a(0) += b(0); a(1) += b(1); Return a; },If(a -> (a(1) == 0), 0.0R, CDbl(a(0)) / a(1)), CH_NOID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} that produces the arithmetic mean of a double-valued
		''' function applied to the input elements.  If no elements are present,
		''' the result is 0.
		''' 
		''' <p>The average returned can vary depending upon the order in which
		''' values are recorded, due to accumulated rounding error in
		''' addition of values of differing magnitudes. Values sorted by increasing
		''' absolute magnitude tend to yield more accurate results.  If any recorded
		''' value is a {@code NaN} or the sum is at any point a {@code NaN} then the
		''' average will be {@code NaN}.
		''' 
		''' @implNote The {@code double} format can represent all
		''' consecutive integers in the range -2<sup>53</sup> to
		''' 2<sup>53</sup>. If the pipeline has more than 2<sup>53</sup>
		''' values, the divisor in the average computation will saturate at
		''' 2<sup>53</sup>, leading to additional numerical errors.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <param name="mapper"> a function extracting the property to be summed </param>
		''' <returns> a {@code Collector} that produces the sum of a derived property </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function averagingDouble(Of T, T1)(ByVal mapper As java.util.function.ToDoubleFunction(Of T1)) As Collector(Of T, ?, Double?)
	'        
	'         * In the arrays allocated for the collect operation, index 0
	'         * holds the high-order bits of the running sum, index 1 holds
	'         * the low-order bits of the sum computed via compensated
	'         * summation, and index 2 holds the number of values seen.
	'         
			Return New CollectorImpl(Of )(() -> New Double(3){}, (a, t) -> { sumWithCompensation(a, mapper.applyAsDouble(t)); a(2)++; a(3)+= mapper.applyAsDouble(t);}, (a, b) -> { sumWithCompensation(a, b(0)); sumWithCompensation(a, b(1)); a(2) += b(2); a(3) += b(3); Return a; },If(a -> (a(2) == 0), 0.0R, (computeFinalSum(a) / a(2))), CH_NOID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} which performs a reduction of its
		''' input elements under a specified {@code BinaryOperator} using the
		''' provided identity.
		''' 
		''' @apiNote
		''' The {@code reducing()} collectors are most useful when used in a
		''' multi-level reduction, downstream of {@code groupingBy} or
		''' {@code partitioningBy}.  To perform a simple reduction on a stream,
		''' use <seealso cref="Stream#reduce(Object, BinaryOperator)"/>} instead.
		''' </summary>
		''' @param <T> element type for the input and output of the reduction </param>
		''' <param name="identity"> the identity value for the reduction (also, the value
		'''                 that is returned when there are no input elements) </param>
		''' <param name="op"> a {@code BinaryOperator<T>} used to reduce the input elements </param>
		''' <returns> a {@code Collector} which implements the reduction operation
		''' </returns>
		''' <seealso cref= #reducing(BinaryOperator) </seealso>
		''' <seealso cref= #reducing(Object, Function, BinaryOperator) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function reducing(Of T)(ByVal identity As T, ByVal op As java.util.function.BinaryOperator(Of T)) As Collector(Of T, ?, T)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return New CollectorImpl(Of )(boxSupplier(identity), (a, t) -> { a(0) = op.apply(a(0), t); }, (a, b) -> { a(0) = op.apply(a(0), b(0)); Return a; }, a -> a(0), CH_NOID)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared Function boxSupplier(Of T)(ByVal identity As T) As java.util.function.Supplier(Of T())
			Return () -> (T()) New Object() { identity }
		End Function

		''' <summary>
		''' Returns a {@code Collector} which performs a reduction of its
		''' input elements under a specified {@code BinaryOperator}.  The result
		''' is described as an {@code Optional<T>}.
		''' 
		''' @apiNote
		''' The {@code reducing()} collectors are most useful when used in a
		''' multi-level reduction, downstream of {@code groupingBy} or
		''' {@code partitioningBy}.  To perform a simple reduction on a stream,
		''' use <seealso cref="Stream#reduce(BinaryOperator)"/> instead.
		''' 
		''' <p>For example, given a stream of {@code Person}, to calculate tallest
		''' person in each city:
		''' <pre>{@code
		'''     Comparator<Person> byHeight = Comparator.comparing(Person::getHeight);
		'''     Map<City, Person> tallestByCity
		'''         = people.stream().collect(groupingBy(Person::getCity, reducing(BinaryOperator.maxBy(byHeight))));
		''' }</pre>
		''' </summary>
		''' @param <T> element type for the input and output of the reduction </param>
		''' <param name="op"> a {@code BinaryOperator<T>} used to reduce the input elements </param>
		''' <returns> a {@code Collector} which implements the reduction operation
		''' </returns>
		''' <seealso cref= #reducing(Object, BinaryOperator) </seealso>
		''' <seealso cref= #reducing(Object, Function, BinaryOperator) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function reducing(Of T)(ByVal op As java.util.function.BinaryOperator(Of T)) As Collector(Of T, ?, java.util.Optional(Of T))
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class OptionalBox implements java.util.function.Consumer(Of T)
	'		{
	'			T value = Nothing;
	'			boolean present = False;
	'
	'			@Override public void accept(T t)
	'			{
	'				if (present)
	'				{
	'					value = op.apply(value, t);
	'				}
	'				else
	'				{
	'					value = t;
	'					present = True;
	'				}
	'			}
	'		}

			Return New CollectorImpl(Of T, OptionalBox, java.util.Optional(Of T))(OptionalBox::New, OptionalBox::accept, (a, b) -> { if(b.present) a.accept(b.value); Return a; }, a -> java.util.Optional.ofNullable(a.value), CH_NOID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} which performs a reduction of its
		''' input elements under a specified mapping function and
		''' {@code BinaryOperator}. This is a generalization of
		''' <seealso cref="#reducing(Object, BinaryOperator)"/> which allows a transformation
		''' of the elements before reduction.
		''' 
		''' @apiNote
		''' The {@code reducing()} collectors are most useful when used in a
		''' multi-level reduction, downstream of {@code groupingBy} or
		''' {@code partitioningBy}.  To perform a simple map-reduce on a stream,
		''' use <seealso cref="Stream#map(Function)"/> and <seealso cref="Stream#reduce(Object, BinaryOperator)"/>
		''' instead.
		''' 
		''' <p>For example, given a stream of {@code Person}, to calculate the longest
		''' last name of residents in each city:
		''' <pre>{@code
		'''     Comparator<String> byLength = Comparator.comparing(String::length);
		'''     Map<City, String> longestLastNameByCity
		'''         = people.stream().collect(groupingBy(Person::getCity,
		'''                                              reducing(Person::getLastName, BinaryOperator.maxBy(byLength))));
		''' }</pre>
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <U> the type of the mapped values </param>
		''' <param name="identity"> the identity value for the reduction (also, the value
		'''                 that is returned when there are no input elements) </param>
		''' <param name="mapper"> a mapping function to apply to each input value </param>
		''' <param name="op"> a {@code BinaryOperator<U>} used to reduce the mapped values </param>
		''' <returns> a {@code Collector} implementing the map-reduce operation
		''' </returns>
		''' <seealso cref= #reducing(Object, BinaryOperator) </seealso>
		''' <seealso cref= #reducing(BinaryOperator) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function reducing(Of T, U, T1 As U)(ByVal identity As U, ByVal mapper As java.util.function.Function(Of T1), ByVal op As java.util.function.BinaryOperator(Of U)) As Collector(Of T, ?, U)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return New CollectorImpl(Of )(boxSupplier(identity), (a, t) -> { a(0) = op.apply(a(0), mapper.apply(t)); }, (a, b) -> { a(0) = op.apply(a(0), b(0)); Return a; }, a -> a(0), CH_NOID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} implementing a "group by" operation on
		''' input elements of type {@code T}, grouping elements according to a
		''' classification function, and returning the results in a {@code Map}.
		''' 
		''' <p>The classification function maps elements to some key type {@code K}.
		''' The collector produces a {@code Map<K, List<T>>} whose keys are the
		''' values resulting from applying the classification function to the input
		''' elements, and whose corresponding values are {@code List}s containing the
		''' input elements which map to the associated key under the classification
		''' function.
		''' 
		''' <p>There are no guarantees on the type, mutability, serializability, or
		''' thread-safety of the {@code Map} or {@code List} objects returned.
		''' @implSpec
		''' This produces a result similar to:
		''' <pre>{@code
		'''     groupingBy(classifier, toList());
		''' }</pre>
		''' 
		''' @implNote
		''' The returned {@code Collector} is not concurrent.  For parallel stream
		''' pipelines, the {@code combiner} function operates by merging the keys
		''' from one map into another, which can be an expensive operation.  If
		''' preservation of the order in which elements appear in the resulting {@code Map}
		''' collector is not required, using <seealso cref="#groupingByConcurrent(Function)"/>
		''' may offer better parallel performance.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <K> the type of the keys </param>
		''' <param name="classifier"> the classifier function mapping input elements to keys </param>
		''' <returns> a {@code Collector} implementing the group-by operation
		''' </returns>
		''' <seealso cref= #groupingBy(Function, Collector) </seealso>
		''' <seealso cref= #groupingBy(Function, Supplier, Collector) </seealso>
		''' <seealso cref= #groupingByConcurrent(Function) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function groupingBy(Of T, K, T1 As K)(ByVal classifier As java.util.function.Function(Of T1)) As Collector(Of T, ?, IDictionary(Of K, IList(Of T)))
			Return groupingBy(classifier, toList())
		End Function

		''' <summary>
		''' Returns a {@code Collector} implementing a cascaded "group by" operation
		''' on input elements of type {@code T}, grouping elements according to a
		''' classification function, and then performing a reduction operation on
		''' the values associated with a given key using the specified downstream
		''' {@code Collector}.
		''' 
		''' <p>The classification function maps elements to some key type {@code K}.
		''' The downstream collector operates on elements of type {@code T} and
		''' produces a result of type {@code D}. The resulting collector produces a
		''' {@code Map<K, D>}.
		''' 
		''' <p>There are no guarantees on the type, mutability,
		''' serializability, or thread-safety of the {@code Map} returned.
		''' 
		''' <p>For example, to compute the set of last names of people in each city:
		''' <pre>{@code
		'''     Map<City, Set<String>> namesByCity
		'''         = people.stream().collect(groupingBy(Person::getCity,
		'''                                              mapping(Person::getLastName, toSet())));
		''' }</pre>
		''' 
		''' @implNote
		''' The returned {@code Collector} is not concurrent.  For parallel stream
		''' pipelines, the {@code combiner} function operates by merging the keys
		''' from one map into another, which can be an expensive operation.  If
		''' preservation of the order in which elements are presented to the downstream
		''' collector is not required, using <seealso cref="#groupingByConcurrent(Function, Collector)"/>
		''' may offer better parallel performance.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <K> the type of the keys </param>
		''' @param <A> the intermediate accumulation type of the downstream collector </param>
		''' @param <D> the result type of the downstream reduction </param>
		''' <param name="classifier"> a classifier function mapping input elements to keys </param>
		''' <param name="downstream"> a {@code Collector} implementing the downstream reduction </param>
		''' <returns> a {@code Collector} implementing the cascaded group-by operation </returns>
		''' <seealso cref= #groupingBy(Function)
		''' </seealso>
		''' <seealso cref= #groupingBy(Function, Supplier, Collector) </seealso>
		''' <seealso cref= #groupingByConcurrent(Function, Collector) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function groupingBy(Of T, K, A, D, T1 As K, T2)(ByVal classifier As java.util.function.Function(Of T1), ByVal downstream As Collector(Of T2)) As Collector(Of T, ?, IDictionary(Of K, D))
			Return groupingBy(classifier, Hashtable::New, downstream)
		End Function

		''' <summary>
		''' Returns a {@code Collector} implementing a cascaded "group by" operation
		''' on input elements of type {@code T}, grouping elements according to a
		''' classification function, and then performing a reduction operation on
		''' the values associated with a given key using the specified downstream
		''' {@code Collector}.  The {@code Map} produced by the Collector is created
		''' with the supplied factory function.
		''' 
		''' <p>The classification function maps elements to some key type {@code K}.
		''' The downstream collector operates on elements of type {@code T} and
		''' produces a result of type {@code D}. The resulting collector produces a
		''' {@code Map<K, D>}.
		''' 
		''' <p>For example, to compute the set of last names of people in each city,
		''' where the city names are sorted:
		''' <pre>{@code
		'''     Map<City, Set<String>> namesByCity
		'''         = people.stream().collect(groupingBy(Person::getCity, TreeMap::new,
		'''                                              mapping(Person::getLastName, toSet())));
		''' }</pre>
		''' 
		''' @implNote
		''' The returned {@code Collector} is not concurrent.  For parallel stream
		''' pipelines, the {@code combiner} function operates by merging the keys
		''' from one map into another, which can be an expensive operation.  If
		''' preservation of the order in which elements are presented to the downstream
		''' collector is not required, using <seealso cref="#groupingByConcurrent(Function, Supplier, Collector)"/>
		''' may offer better parallel performance.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <K> the type of the keys </param>
		''' @param <A> the intermediate accumulation type of the downstream collector </param>
		''' @param <D> the result type of the downstream reduction </param>
		''' @param <M> the type of the resulting {@code Map} </param>
		''' <param name="classifier"> a classifier function mapping input elements to keys </param>
		''' <param name="downstream"> a {@code Collector} implementing the downstream reduction </param>
		''' <param name="mapFactory"> a function which, when called, produces a new empty
		'''                   {@code Map} of the desired type </param>
		''' <returns> a {@code Collector} implementing the cascaded group-by operation
		''' </returns>
		''' <seealso cref= #groupingBy(Function, Collector) </seealso>
		''' <seealso cref= #groupingBy(Function) </seealso>
		''' <seealso cref= #groupingByConcurrent(Function, Supplier, Collector) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function groupingBy(Of T, K, D, A, M As IDictionary(Of K, D), T1 As K, T2)(ByVal classifier As java.util.function.Function(Of T1), ByVal mapFactory As java.util.function.Supplier(Of M), ByVal downstream As Collector(Of T2)) As Collector(Of T, ?, M)
			Dim downstreamSupplier As java.util.function.Supplier(Of A) = downstream.supplier()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim downstreamAccumulator As java.util.function.BiConsumer(Of A, ?) = downstream.accumulator()
			Dim accumulator As java.util.function.BiConsumer(Of IDictionary(Of K, A), T) = (m, t) ->
				Dim key As K = java.util.Objects.requireNonNull(classifier.apply(t), "element cannot be mapped to a null key")
				Dim container As A = m.computeIfAbsent(key, k -> downstreamSupplier.get())
				downstreamAccumulator.accept(container, t)
			Dim merger As java.util.function.BinaryOperator(Of IDictionary(Of K, A)) = Collectors.mapMerger(Of K, A, IDictionary(Of K, A))(downstream.combiner())
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim mangledFactory As java.util.function.Supplier(Of IDictionary(Of K, A)) = CType(mapFactory, java.util.function.Supplier(Of IDictionary(Of K, A)))

			If downstream.characteristics().contains(Collector.Characteristics.IDENTITY_FINISH) Then
				Return New CollectorImpl(Of )(mangledFactory, accumulator, merger, CH_ID)
			Else
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim downstreamFinisher As java.util.function.Function(Of A, A) = CType(downstream.finisher(), java.util.function.Function(Of A, A))
				Dim finisher As java.util.function.Function(Of IDictionary(Of K, A), M) = intermediate ->
					intermediate.replaceAll((k, v) -> downstreamFinisher.apply(v))
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim castResult As M = CType(intermediate, M)
					Return castResult
				Return New CollectorImpl(Of )(mangledFactory, accumulator, merger, finisher, CH_NOID)
			End If
		End Function

		''' <summary>
		''' Returns a concurrent {@code Collector} implementing a "group by"
		''' operation on input elements of type {@code T}, grouping elements
		''' according to a classification function.
		''' 
		''' <p>This is a <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/> and
		''' <seealso cref="Collector.Characteristics#UNORDERED unordered"/> Collector.
		''' 
		''' <p>The classification function maps elements to some key type {@code K}.
		''' The collector produces a {@code ConcurrentMap<K, List<T>>} whose keys are the
		''' values resulting from applying the classification function to the input
		''' elements, and whose corresponding values are {@code List}s containing the
		''' input elements which map to the associated key under the classification
		''' function.
		''' 
		''' <p>There are no guarantees on the type, mutability, or serializability
		''' of the {@code Map} or {@code List} objects returned, or of the
		''' thread-safety of the {@code List} objects returned.
		''' @implSpec
		''' This produces a result similar to:
		''' <pre>{@code
		'''     groupingByConcurrent(classifier, toList());
		''' }</pre>
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <K> the type of the keys </param>
		''' <param name="classifier"> a classifier function mapping input elements to keys </param>
		''' <returns> a concurrent, unordered {@code Collector} implementing the group-by operation
		''' </returns>
		''' <seealso cref= #groupingBy(Function) </seealso>
		''' <seealso cref= #groupingByConcurrent(Function, Collector) </seealso>
		''' <seealso cref= #groupingByConcurrent(Function, Supplier, Collector) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function groupingByConcurrent(Of T, K, T1 As K)(ByVal classifier As java.util.function.Function(Of T1)) As Collector(Of T, ?, java.util.concurrent.ConcurrentMap(Of K, IList(Of T)))
			Return groupingByConcurrent(classifier, ConcurrentDictionary::New, toList())
		End Function

		''' <summary>
		''' Returns a concurrent {@code Collector} implementing a cascaded "group by"
		''' operation on input elements of type {@code T}, grouping elements
		''' according to a classification function, and then performing a reduction
		''' operation on the values associated with a given key using the specified
		''' downstream {@code Collector}.
		''' 
		''' <p>This is a <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/> and
		''' <seealso cref="Collector.Characteristics#UNORDERED unordered"/> Collector.
		''' 
		''' <p>The classification function maps elements to some key type {@code K}.
		''' The downstream collector operates on elements of type {@code T} and
		''' produces a result of type {@code D}. The resulting collector produces a
		''' {@code Map<K, D>}.
		''' 
		''' <p>For example, to compute the set of last names of people in each city,
		''' where the city names are sorted:
		''' <pre>{@code
		'''     ConcurrentMap<City, Set<String>> namesByCity
		'''         = people.stream().collect(groupingByConcurrent(Person::getCity,
		'''                                                        mapping(Person::getLastName, toSet())));
		''' }</pre>
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <K> the type of the keys </param>
		''' @param <A> the intermediate accumulation type of the downstream collector </param>
		''' @param <D> the result type of the downstream reduction </param>
		''' <param name="classifier"> a classifier function mapping input elements to keys </param>
		''' <param name="downstream"> a {@code Collector} implementing the downstream reduction </param>
		''' <returns> a concurrent, unordered {@code Collector} implementing the cascaded group-by operation
		''' </returns>
		''' <seealso cref= #groupingBy(Function, Collector) </seealso>
		''' <seealso cref= #groupingByConcurrent(Function) </seealso>
		''' <seealso cref= #groupingByConcurrent(Function, Supplier, Collector) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function groupingByConcurrent(Of T, K, A, D, T1 As K, T2)(ByVal classifier As java.util.function.Function(Of T1), ByVal downstream As Collector(Of T2)) As Collector(Of T, ?, java.util.concurrent.ConcurrentMap(Of K, D))
			Return groupingByConcurrent(classifier, ConcurrentDictionary::New, downstream)
		End Function

		''' <summary>
		''' Returns a concurrent {@code Collector} implementing a cascaded "group by"
		''' operation on input elements of type {@code T}, grouping elements
		''' according to a classification function, and then performing a reduction
		''' operation on the values associated with a given key using the specified
		''' downstream {@code Collector}.  The {@code ConcurrentMap} produced by the
		''' Collector is created with the supplied factory function.
		''' 
		''' <p>This is a <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/> and
		''' <seealso cref="Collector.Characteristics#UNORDERED unordered"/> Collector.
		''' 
		''' <p>The classification function maps elements to some key type {@code K}.
		''' The downstream collector operates on elements of type {@code T} and
		''' produces a result of type {@code D}. The resulting collector produces a
		''' {@code Map<K, D>}.
		''' 
		''' <p>For example, to compute the set of last names of people in each city,
		''' where the city names are sorted:
		''' <pre>{@code
		'''     ConcurrentMap<City, Set<String>> namesByCity
		'''         = people.stream().collect(groupingBy(Person::getCity, ConcurrentSkipListMap::new,
		'''                                              mapping(Person::getLastName, toSet())));
		''' }</pre>
		''' 
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <K> the type of the keys </param>
		''' @param <A> the intermediate accumulation type of the downstream collector </param>
		''' @param <D> the result type of the downstream reduction </param>
		''' @param <M> the type of the resulting {@code ConcurrentMap} </param>
		''' <param name="classifier"> a classifier function mapping input elements to keys </param>
		''' <param name="downstream"> a {@code Collector} implementing the downstream reduction </param>
		''' <param name="mapFactory"> a function which, when called, produces a new empty
		'''                   {@code ConcurrentMap} of the desired type </param>
		''' <returns> a concurrent, unordered {@code Collector} implementing the cascaded group-by operation
		''' </returns>
		''' <seealso cref= #groupingByConcurrent(Function) </seealso>
		''' <seealso cref= #groupingByConcurrent(Function, Collector) </seealso>
		''' <seealso cref= #groupingBy(Function, Supplier, Collector) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function groupingByConcurrent(Of T, K, A, D, M As java.util.concurrent.ConcurrentMap(Of K, D), T1 As K, T2)(ByVal classifier As java.util.function.Function(Of T1), ByVal mapFactory As java.util.function.Supplier(Of M), ByVal downstream As Collector(Of T2)) As Collector(Of T, ?, M)
			Dim downstreamSupplier As java.util.function.Supplier(Of A) = downstream.supplier()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim downstreamAccumulator As java.util.function.BiConsumer(Of A, ?) = downstream.accumulator()
			Dim merger As java.util.function.BinaryOperator(Of java.util.concurrent.ConcurrentMap(Of K, A)) = Collectors.mapMerger(Of K, A, java.util.concurrent.ConcurrentMap(Of K, A))(downstream.combiner())
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim mangledFactory As java.util.function.Supplier(Of java.util.concurrent.ConcurrentMap(Of K, A)) = CType(mapFactory, java.util.function.Supplier(Of java.util.concurrent.ConcurrentMap(Of K, A)))
			Dim accumulator As java.util.function.BiConsumer(Of java.util.concurrent.ConcurrentMap(Of K, A), T)
			If downstream.characteristics().contains(Collector.Characteristics.CONCURRENT) Then
				accumulator = (m, t) ->
					Dim key As K = java.util.Objects.requireNonNull(classifier.apply(t), "element cannot be mapped to a null key")
					Dim resultContainer As A = m.computeIfAbsent(key, k -> downstreamSupplier.get())
					downstreamAccumulator.accept(resultContainer, t)
			Else
				accumulator = (m, t) ->
					Dim key As K = java.util.Objects.requireNonNull(classifier.apply(t), "element cannot be mapped to a null key")
					Dim resultContainer As A = m.computeIfAbsent(key, k -> downstreamSupplier.get())
					SyncLock resultContainer
						downstreamAccumulator.accept(resultContainer, t)
					End SyncLock
			End If

			If downstream.characteristics().contains(Collector.Characteristics.IDENTITY_FINISH) Then
				Return New CollectorImpl(Of )(mangledFactory, accumulator, merger, CH_CONCURRENT_ID)
			Else
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim downstreamFinisher As java.util.function.Function(Of A, A) = CType(downstream.finisher(), java.util.function.Function(Of A, A))
				Dim finisher As java.util.function.Function(Of java.util.concurrent.ConcurrentMap(Of K, A), M) = intermediate ->
					intermediate.replaceAll((k, v) -> downstreamFinisher.apply(v))
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim castResult As M = CType(intermediate, M)
					Return castResult
				Return New CollectorImpl(Of )(mangledFactory, accumulator, merger, finisher, CH_CONCURRENT_NOID)
			End If
		End Function

		''' <summary>
		''' Returns a {@code Collector} which partitions the input elements according
		''' to a {@code Predicate}, and organizes them into a
		''' {@code Map<Boolean, List<T>>}.
		''' 
		''' There are no guarantees on the type, mutability,
		''' serializability, or thread-safety of the {@code Map} returned.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <param name="predicate"> a predicate used for classifying input elements </param>
		''' <returns> a {@code Collector} implementing the partitioning operation
		''' </returns>
		''' <seealso cref= #partitioningBy(Predicate, Collector) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function partitioningBy(Of T, T1)(ByVal predicate As java.util.function.Predicate(Of T1)) As Collector(Of T, ?, IDictionary(Of Boolean?, IList(Of T)))
			Return partitioningBy(predicate, toList())
		End Function

		''' <summary>
		''' Returns a {@code Collector} which partitions the input elements according
		''' to a {@code Predicate}, reduces the values in each partition according to
		''' another {@code Collector}, and organizes them into a
		''' {@code Map<Boolean, D>} whose values are the result of the downstream
		''' reduction.
		''' 
		''' <p>There are no guarantees on the type, mutability,
		''' serializability, or thread-safety of the {@code Map} returned.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <A> the intermediate accumulation type of the downstream collector </param>
		''' @param <D> the result type of the downstream reduction </param>
		''' <param name="predicate"> a predicate used for classifying input elements </param>
		''' <param name="downstream"> a {@code Collector} implementing the downstream
		'''                   reduction </param>
		''' <returns> a {@code Collector} implementing the cascaded partitioning
		'''         operation
		''' </returns>
		''' <seealso cref= #partitioningBy(Predicate) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function partitioningBy(Of T, D, A, T1, T2)(ByVal predicate As java.util.function.Predicate(Of T1), ByVal downstream As Collector(Of T2)) As Collector(Of T, ?, IDictionary(Of Boolean?, D))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim downstreamAccumulator As java.util.function.BiConsumer(Of A, ?) = downstream.accumulator()
			Dim accumulator As java.util.function.BiConsumer(Of Partition(Of A), T) = (result, t) -> downstreamAccumulator.accept(If(predicate.test(t), result.forTrue, result.forFalse), t)
			Dim op As java.util.function.BinaryOperator(Of A) = downstream.combiner()
			Dim merger As java.util.function.BinaryOperator(Of Partition(Of A)) = (left, right) -> New Partition(Of Partition(Of A))(op.apply(left.forTrue, right.forTrue), op.apply(left.forFalse, right.forFalse))
			Dim supplier As java.util.function.Supplier(Of Partition(Of A)) = () -> New Partition(Of Partition(Of A))(downstream.supplier().get(), downstream.supplier().get())
			If downstream.characteristics().contains(Collector.Characteristics.IDENTITY_FINISH) Then
				Return New CollectorImpl(Of )(supplier, accumulator, merger, CH_ID)
			Else
				Dim finisher As java.util.function.Function(Of Partition(Of A), IDictionary(Of Boolean?, D)) = par -> New Partition(Of Partition(Of A), IDictionary(Of Boolean?, D))(downstream.finisher().apply(par.forTrue), downstream.finisher().apply(par.forFalse))
				Return New CollectorImpl(Of )(supplier, accumulator, merger, finisher, CH_NOID)
			End If
		End Function

		''' <summary>
		''' Returns a {@code Collector} that accumulates elements into a
		''' {@code Map} whose keys and values are the result of applying the provided
		''' mapping functions to the input elements.
		''' 
		''' <p>If the mapped keys contains duplicates (according to
		''' <seealso cref="Object#equals(Object)"/>), an {@code IllegalStateException} is
		''' thrown when the collection operation is performed.  If the mapped keys
		''' may have duplicates, use <seealso cref="#toMap(Function, Function, BinaryOperator)"/>
		''' instead.
		''' 
		''' @apiNote
		''' It is common for either the key or the value to be the input elements.
		''' In this case, the utility method
		''' <seealso cref="java.util.function.Function#identity()"/> may be helpful.
		''' For example, the following produces a {@code Map} mapping
		''' students to their grade point average:
		''' <pre>{@code
		'''     Map<Student, Double> studentToGPA
		'''         students.stream().collect(toMap(Functions.identity(),
		'''                                         student -> computeGPA(student)));
		''' }</pre>
		''' And the following produces a {@code Map} mapping a unique identifier to
		''' students:
		''' <pre>{@code
		'''     Map<String, Student> studentIdToStudent
		'''         students.stream().collect(toMap(Student::getId,
		'''                                         Functions.identity());
		''' }</pre>
		''' 
		''' @implNote
		''' The returned {@code Collector} is not concurrent.  For parallel stream
		''' pipelines, the {@code combiner} function operates by merging the keys
		''' from one map into another, which can be an expensive operation.  If it is
		''' not required that results are inserted into the {@code Map} in encounter
		''' order, using <seealso cref="#toConcurrentMap(Function, Function)"/>
		''' may offer better parallel performance.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <K> the output type of the key mapping function </param>
		''' @param <U> the output type of the value mapping function </param>
		''' <param name="keyMapper"> a mapping function to produce keys </param>
		''' <param name="valueMapper"> a mapping function to produce values </param>
		''' <returns> a {@code Collector} which collects elements into a {@code Map}
		''' whose keys and values are the result of applying mapping functions to
		''' the input elements
		''' </returns>
		''' <seealso cref= #toMap(Function, Function, BinaryOperator) </seealso>
		''' <seealso cref= #toMap(Function, Function, BinaryOperator, Supplier) </seealso>
		''' <seealso cref= #toConcurrentMap(Function, Function) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function toMap(Of T, K, U, T1 As K, T2 As U)(ByVal keyMapper As java.util.function.Function(Of T1), ByVal valueMapper As java.util.function.Function(Of T2)) As Collector(Of T, ?, IDictionary(Of K, U))
			Return toMap(keyMapper, valueMapper, throwingMerger(), Hashtable::New)
		End Function

		''' <summary>
		''' Returns a {@code Collector} that accumulates elements into a
		''' {@code Map} whose keys and values are the result of applying the provided
		''' mapping functions to the input elements.
		''' 
		''' <p>If the mapped
		''' keys contains duplicates (according to <seealso cref="Object#equals(Object)"/>),
		''' the value mapping function is applied to each equal element, and the
		''' results are merged using the provided merging function.
		''' 
		''' @apiNote
		''' There are multiple ways to deal with collisions between multiple elements
		''' mapping to the same key.  The other forms of {@code toMap} simply use
		''' a merge function that throws unconditionally, but you can easily write
		''' more flexible merge policies.  For example, if you have a stream
		''' of {@code Person}, and you want to produce a "phone book" mapping name to
		''' address, but it is possible that two persons have the same name, you can
		''' do as follows to gracefully deals with these collisions, and produce a
		''' {@code Map} mapping names to a concatenated list of addresses:
		''' <pre>{@code
		'''     Map<String, String> phoneBook
		'''         people.stream().collect(toMap(Person::getName,
		'''                                       Person::getAddress,
		'''                                       (s, a) -> s + ", " + a));
		''' }</pre>
		''' 
		''' @implNote
		''' The returned {@code Collector} is not concurrent.  For parallel stream
		''' pipelines, the {@code combiner} function operates by merging the keys
		''' from one map into another, which can be an expensive operation.  If it is
		''' not required that results are merged into the {@code Map} in encounter
		''' order, using <seealso cref="#toConcurrentMap(Function, Function, BinaryOperator)"/>
		''' may offer better parallel performance.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <K> the output type of the key mapping function </param>
		''' @param <U> the output type of the value mapping function </param>
		''' <param name="keyMapper"> a mapping function to produce keys </param>
		''' <param name="valueMapper"> a mapping function to produce values </param>
		''' <param name="mergeFunction"> a merge function, used to resolve collisions between
		'''                      values associated with the same key, as supplied
		'''                      to <seealso cref="Map#merge(Object, Object, BiFunction)"/> </param>
		''' <returns> a {@code Collector} which collects elements into a {@code Map}
		''' whose keys are the result of applying a key mapping function to the input
		''' elements, and whose values are the result of applying a value mapping
		''' function to all input elements equal to the key and combining them
		''' using the merge function
		''' </returns>
		''' <seealso cref= #toMap(Function, Function) </seealso>
		''' <seealso cref= #toMap(Function, Function, BinaryOperator, Supplier) </seealso>
		''' <seealso cref= #toConcurrentMap(Function, Function, BinaryOperator) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function toMap(Of T, K, U, T1 As K, T2 As U)(ByVal keyMapper As java.util.function.Function(Of T1), ByVal valueMapper As java.util.function.Function(Of T2), ByVal mergeFunction As java.util.function.BinaryOperator(Of U)) As Collector(Of T, ?, IDictionary(Of K, U))
			Return toMap(keyMapper, valueMapper, mergeFunction, Hashtable::New)
		End Function

		''' <summary>
		''' Returns a {@code Collector} that accumulates elements into a
		''' {@code Map} whose keys and values are the result of applying the provided
		''' mapping functions to the input elements.
		''' 
		''' <p>If the mapped
		''' keys contains duplicates (according to <seealso cref="Object#equals(Object)"/>),
		''' the value mapping function is applied to each equal element, and the
		''' results are merged using the provided merging function.  The {@code Map}
		''' is created by a provided supplier function.
		''' 
		''' @implNote
		''' The returned {@code Collector} is not concurrent.  For parallel stream
		''' pipelines, the {@code combiner} function operates by merging the keys
		''' from one map into another, which can be an expensive operation.  If it is
		''' not required that results are merged into the {@code Map} in encounter
		''' order, using <seealso cref="#toConcurrentMap(Function, Function, BinaryOperator, Supplier)"/>
		''' may offer better parallel performance.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <K> the output type of the key mapping function </param>
		''' @param <U> the output type of the value mapping function </param>
		''' @param <M> the type of the resulting {@code Map} </param>
		''' <param name="keyMapper"> a mapping function to produce keys </param>
		''' <param name="valueMapper"> a mapping function to produce values </param>
		''' <param name="mergeFunction"> a merge function, used to resolve collisions between
		'''                      values associated with the same key, as supplied
		'''                      to <seealso cref="Map#merge(Object, Object, BiFunction)"/> </param>
		''' <param name="mapSupplier"> a function which returns a new, empty {@code Map} into
		'''                    which the results will be inserted </param>
		''' <returns> a {@code Collector} which collects elements into a {@code Map}
		''' whose keys are the result of applying a key mapping function to the input
		''' elements, and whose values are the result of applying a value mapping
		''' function to all input elements equal to the key and combining them
		''' using the merge function
		''' </returns>
		''' <seealso cref= #toMap(Function, Function) </seealso>
		''' <seealso cref= #toMap(Function, Function, BinaryOperator) </seealso>
		''' <seealso cref= #toConcurrentMap(Function, Function, BinaryOperator, Supplier) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function toMap(Of T, K, U, M As IDictionary(Of K, U), T1 As K, T2 As U)(ByVal keyMapper As java.util.function.Function(Of T1), ByVal valueMapper As java.util.function.Function(Of T2), ByVal mergeFunction As java.util.function.BinaryOperator(Of U), ByVal mapSupplier As java.util.function.Supplier(Of M)) As Collector(Of T, ?, M)
			Dim accumulator As java.util.function.BiConsumer(Of M, T) = (map, element) -> map.merge(keyMapper.apply(element), valueMapper.apply(element), mergeFunction)
			Return New CollectorImpl(Of )(mapSupplier, accumulator, mapMerger(mergeFunction), CH_ID)
		End Function

		''' <summary>
		''' Returns a concurrent {@code Collector} that accumulates elements into a
		''' {@code ConcurrentMap} whose keys and values are the result of applying
		''' the provided mapping functions to the input elements.
		''' 
		''' <p>If the mapped keys contains duplicates (according to
		''' <seealso cref="Object#equals(Object)"/>), an {@code IllegalStateException} is
		''' thrown when the collection operation is performed.  If the mapped keys
		''' may have duplicates, use
		''' <seealso cref="#toConcurrentMap(Function, Function, BinaryOperator)"/> instead.
		''' 
		''' @apiNote
		''' It is common for either the key or the value to be the input elements.
		''' In this case, the utility method
		''' <seealso cref="java.util.function.Function#identity()"/> may be helpful.
		''' For example, the following produces a {@code Map} mapping
		''' students to their grade point average:
		''' <pre>{@code
		'''     Map<Student, Double> studentToGPA
		'''         students.stream().collect(toMap(Functions.identity(),
		'''                                         student -> computeGPA(student)));
		''' }</pre>
		''' And the following produces a {@code Map} mapping a unique identifier to
		''' students:
		''' <pre>{@code
		'''     Map<String, Student> studentIdToStudent
		'''         students.stream().collect(toConcurrentMap(Student::getId,
		'''                                                   Functions.identity());
		''' }</pre>
		''' 
		''' <p>This is a <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/> and
		''' <seealso cref="Collector.Characteristics#UNORDERED unordered"/> Collector.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <K> the output type of the key mapping function </param>
		''' @param <U> the output type of the value mapping function </param>
		''' <param name="keyMapper"> the mapping function to produce keys </param>
		''' <param name="valueMapper"> the mapping function to produce values </param>
		''' <returns> a concurrent, unordered {@code Collector} which collects elements into a
		''' {@code ConcurrentMap} whose keys are the result of applying a key mapping
		''' function to the input elements, and whose values are the result of
		''' applying a value mapping function to the input elements
		''' </returns>
		''' <seealso cref= #toMap(Function, Function) </seealso>
		''' <seealso cref= #toConcurrentMap(Function, Function, BinaryOperator) </seealso>
		''' <seealso cref= #toConcurrentMap(Function, Function, BinaryOperator, Supplier) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function toConcurrentMap(Of T, K, U, T1 As K, T2 As U)(ByVal keyMapper As java.util.function.Function(Of T1), ByVal valueMapper As java.util.function.Function(Of T2)) As Collector(Of T, ?, java.util.concurrent.ConcurrentMap(Of K, U))
			Return toConcurrentMap(keyMapper, valueMapper, throwingMerger(), ConcurrentDictionary::New)
		End Function

		''' <summary>
		''' Returns a concurrent {@code Collector} that accumulates elements into a
		''' {@code ConcurrentMap} whose keys and values are the result of applying
		''' the provided mapping functions to the input elements.
		''' 
		''' <p>If the mapped keys contains duplicates (according to <seealso cref="Object#equals(Object)"/>),
		''' the value mapping function is applied to each equal element, and the
		''' results are merged using the provided merging function.
		''' 
		''' @apiNote
		''' There are multiple ways to deal with collisions between multiple elements
		''' mapping to the same key.  The other forms of {@code toConcurrentMap} simply use
		''' a merge function that throws unconditionally, but you can easily write
		''' more flexible merge policies.  For example, if you have a stream
		''' of {@code Person}, and you want to produce a "phone book" mapping name to
		''' address, but it is possible that two persons have the same name, you can
		''' do as follows to gracefully deals with these collisions, and produce a
		''' {@code Map} mapping names to a concatenated list of addresses:
		''' <pre>{@code
		'''     Map<String, String> phoneBook
		'''         people.stream().collect(toConcurrentMap(Person::getName,
		'''                                                 Person::getAddress,
		'''                                                 (s, a) -> s + ", " + a));
		''' }</pre>
		''' 
		''' <p>This is a <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/> and
		''' <seealso cref="Collector.Characteristics#UNORDERED unordered"/> Collector.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <K> the output type of the key mapping function </param>
		''' @param <U> the output type of the value mapping function </param>
		''' <param name="keyMapper"> a mapping function to produce keys </param>
		''' <param name="valueMapper"> a mapping function to produce values </param>
		''' <param name="mergeFunction"> a merge function, used to resolve collisions between
		'''                      values associated with the same key, as supplied
		'''                      to <seealso cref="Map#merge(Object, Object, BiFunction)"/> </param>
		''' <returns> a concurrent, unordered {@code Collector} which collects elements into a
		''' {@code ConcurrentMap} whose keys are the result of applying a key mapping
		''' function to the input elements, and whose values are the result of
		''' applying a value mapping function to all input elements equal to the key
		''' and combining them using the merge function
		''' </returns>
		''' <seealso cref= #toConcurrentMap(Function, Function) </seealso>
		''' <seealso cref= #toConcurrentMap(Function, Function, BinaryOperator, Supplier) </seealso>
		''' <seealso cref= #toMap(Function, Function, BinaryOperator) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function toConcurrentMap(Of T, K, U, T1 As K, T2 As U)(ByVal keyMapper As java.util.function.Function(Of T1), ByVal valueMapper As java.util.function.Function(Of T2), ByVal mergeFunction As java.util.function.BinaryOperator(Of U)) As Collector(Of T, ?, java.util.concurrent.ConcurrentMap(Of K, U))
			Return toConcurrentMap(keyMapper, valueMapper, mergeFunction, ConcurrentDictionary::New)
		End Function

		''' <summary>
		''' Returns a concurrent {@code Collector} that accumulates elements into a
		''' {@code ConcurrentMap} whose keys and values are the result of applying
		''' the provided mapping functions to the input elements.
		''' 
		''' <p>If the mapped keys contains duplicates (according to <seealso cref="Object#equals(Object)"/>),
		''' the value mapping function is applied to each equal element, and the
		''' results are merged using the provided merging function.  The
		''' {@code ConcurrentMap} is created by a provided supplier function.
		''' 
		''' <p>This is a <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/> and
		''' <seealso cref="Collector.Characteristics#UNORDERED unordered"/> Collector.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <K> the output type of the key mapping function </param>
		''' @param <U> the output type of the value mapping function </param>
		''' @param <M> the type of the resulting {@code ConcurrentMap} </param>
		''' <param name="keyMapper"> a mapping function to produce keys </param>
		''' <param name="valueMapper"> a mapping function to produce values </param>
		''' <param name="mergeFunction"> a merge function, used to resolve collisions between
		'''                      values associated with the same key, as supplied
		'''                      to <seealso cref="Map#merge(Object, Object, BiFunction)"/> </param>
		''' <param name="mapSupplier"> a function which returns a new, empty {@code Map} into
		'''                    which the results will be inserted </param>
		''' <returns> a concurrent, unordered {@code Collector} which collects elements into a
		''' {@code ConcurrentMap} whose keys are the result of applying a key mapping
		''' function to the input elements, and whose values are the result of
		''' applying a value mapping function to all input elements equal to the key
		''' and combining them using the merge function
		''' </returns>
		''' <seealso cref= #toConcurrentMap(Function, Function) </seealso>
		''' <seealso cref= #toConcurrentMap(Function, Function, BinaryOperator) </seealso>
		''' <seealso cref= #toMap(Function, Function, BinaryOperator, Supplier) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function toConcurrentMap(Of T, K, U, M As java.util.concurrent.ConcurrentMap(Of K, U), T1 As K, T2 As U)(ByVal keyMapper As java.util.function.Function(Of T1), ByVal valueMapper As java.util.function.Function(Of T2), ByVal mergeFunction As java.util.function.BinaryOperator(Of U), ByVal mapSupplier As java.util.function.Supplier(Of M)) As Collector(Of T, ?, M)
			Dim accumulator As java.util.function.BiConsumer(Of M, T) = (map, element) -> map.merge(keyMapper.apply(element), valueMapper.apply(element), mergeFunction)
			Return New CollectorImpl(Of )(mapSupplier, accumulator, mapMerger(mergeFunction), CH_CONCURRENT_ID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} which applies an {@code int}-producing
		''' mapping function to each input element, and returns summary statistics
		''' for the resulting values.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <param name="mapper"> a mapping function to apply to each element </param>
		''' <returns> a {@code Collector} implementing the summary-statistics reduction
		''' </returns>
		''' <seealso cref= #summarizingDouble(ToDoubleFunction) </seealso>
		''' <seealso cref= #summarizingLong(ToLongFunction) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function summarizingInt(Of T, T1)(ByVal mapper As java.util.function.ToIntFunction(Of T1)) As Collector(Of T, ?, java.util.IntSummaryStatistics)
			Return New CollectorImpl(Of T, java.util.IntSummaryStatistics, java.util.IntSummaryStatistics)(java.util.IntSummaryStatistics::New, (r, t) -> r.accept(mapper.applyAsInt(t)), (l, r) -> { l.combine(r); Return l; }, CH_ID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} which applies an {@code long}-producing
		''' mapping function to each input element, and returns summary statistics
		''' for the resulting values.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <param name="mapper"> the mapping function to apply to each element </param>
		''' <returns> a {@code Collector} implementing the summary-statistics reduction
		''' </returns>
		''' <seealso cref= #summarizingDouble(ToDoubleFunction) </seealso>
		''' <seealso cref= #summarizingInt(ToIntFunction) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function summarizingLong(Of T, T1)(ByVal mapper As java.util.function.ToLongFunction(Of T1)) As Collector(Of T, ?, java.util.LongSummaryStatistics)
			Return New CollectorImpl(Of T, java.util.LongSummaryStatistics, java.util.LongSummaryStatistics)(java.util.LongSummaryStatistics::New, (r, t) -> r.accept(mapper.applyAsLong(t)), (l, r) -> { l.combine(r); Return l; }, CH_ID)
		End Function

		''' <summary>
		''' Returns a {@code Collector} which applies an {@code double}-producing
		''' mapping function to each input element, and returns summary statistics
		''' for the resulting values.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' <param name="mapper"> a mapping function to apply to each element </param>
		''' <returns> a {@code Collector} implementing the summary-statistics reduction
		''' </returns>
		''' <seealso cref= #summarizingLong(ToLongFunction) </seealso>
		''' <seealso cref= #summarizingInt(ToIntFunction) </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function summarizingDouble(Of T, T1)(ByVal mapper As java.util.function.ToDoubleFunction(Of T1)) As Collector(Of T, ?, java.util.DoubleSummaryStatistics)
			Return New CollectorImpl(Of T, java.util.DoubleSummaryStatistics, java.util.DoubleSummaryStatistics)(java.util.DoubleSummaryStatistics::New, (r, t) -> r.accept(mapper.applyAsDouble(t)), (l, r) -> { l.combine(r); Return l; }, CH_ID)
		End Function

		''' <summary>
		''' Implementation class used by partitioningBy.
		''' </summary>
		Private NotInheritable Class Partition(Of T)
			Inherits java.util.AbstractMap(Of Boolean?, T)
			Implements IDictionary(Of Boolean?, T)

			Friend ReadOnly forTrue As T
			Friend ReadOnly forFalse As T

			Friend Sub New(ByVal forTrue As T, ByVal forFalse As T)
				Me.forTrue = forTrue
				Me.forFalse = forFalse
			End Sub

			Public Overrides Function entrySet() As java.util.Set(Of KeyValuePair(Of Boolean?, T))
				Return New AbstractSetAnonymousInnerClassHelper(Of E)
			End Function

			Private Class AbstractSetAnonymousInnerClassHelper(Of E)
				Inherits java.util.AbstractSet(Of E)

				Public Overrides Function [iterator]() As IEnumerator(Of KeyValuePair(Of Boolean?, T))
					Dim falseEntry As KeyValuePair(Of Boolean?, T) = New SimpleImmutableEntry(Of Boolean?, T)(False, outerInstance.forFalse)
					Dim trueEntry As KeyValuePair(Of Boolean?, T) = New SimpleImmutableEntry(Of Boolean?, T)(True, outerInstance.forTrue)
					Return java.util.Arrays.asList(falseEntry, trueEntry).GetEnumerator()
				End Function

				Public Overrides Function size() As Integer
					Return 2
				End Function
			End Class
		End Class
	End Class

End Namespace
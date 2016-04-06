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
	''' Low-level utility methods for creating and manipulating streams.
	''' 
	''' <p>This class is mostly for library writers presenting stream views
	''' of data structures; most static stream methods intended for end users are in
	''' the various {@code Stream} classes.
	''' 
	''' @since 1.8
	''' </summary>
	Public NotInheritable Class StreamSupport

		' Suppresses default constructor, ensuring non-instantiability.
		Private Sub New()
		End Sub

		''' <summary>
		''' Creates a new sequential or parallel {@code Stream} from a
		''' {@code Spliterator}.
		''' 
		''' <p>The spliterator is only traversed, split, or queried for estimated
		''' size after the terminal operation of the stream pipeline commences.
		''' 
		''' <p>It is strongly recommended the spliterator report a characteristic of
		''' {@code IMMUTABLE} or {@code CONCURRENT}, or be
		''' <a href="../Spliterator.html#binding">late-binding</a>.  Otherwise,
		''' <seealso cref="#stream(java.util.function.Supplier, int, boolean)"/> should be used
		''' to reduce the scope of potential interference with the source.  See
		''' <a href="package-summary.html#NonInterference">Non-Interference</a> for
		''' more details.
		''' </summary>
		''' @param <T> the type of stream elements </param>
		''' <param name="spliterator"> a {@code Spliterator} describing the stream elements </param>
		''' <param name="parallel"> if {@code true} then the returned stream is a parallel
		'''        stream; if {@code false} the returned stream is a sequential
		'''        stream. </param>
		''' <returns> a new sequential or parallel {@code Stream} </returns>
		Public Shared Function stream(Of T)(  spliterator As java.util.Spliterator(Of T),   parallel As Boolean) As Stream(Of T)
			java.util.Objects.requireNonNull(spliterator)
			Return New ReferencePipeline.Head(Of )(spliterator, StreamOpFlag.fromCharacteristics(spliterator), parallel)
		End Function

		''' <summary>
		''' Creates a new sequential or parallel {@code Stream} from a
		''' {@code Supplier} of {@code Spliterator}.
		''' 
		''' <p>The <seealso cref="Supplier#get()"/> method will be invoked on the supplier no
		''' more than once, and only after the terminal operation of the stream pipeline
		''' commences.
		''' 
		''' <p>For spliterators that report a characteristic of {@code IMMUTABLE}
		''' or {@code CONCURRENT}, or that are
		''' <a href="../Spliterator.html#binding">late-binding</a>, it is likely
		''' more efficient to use <seealso cref="#stream(java.util.Spliterator, boolean)"/>
		''' instead.
		''' <p>The use of a {@code Supplier} in this form provides a level of
		''' indirection that reduces the scope of potential interference with the
		''' source.  Since the supplier is only invoked after the terminal operation
		''' commences, any modifications to the source up to the start of the
		''' terminal operation are reflected in the stream result.  See
		''' <a href="package-summary.html#NonInterference">Non-Interference</a> for
		''' more details.
		''' </summary>
		''' @param <T> the type of stream elements </param>
		''' <param name="supplier"> a {@code Supplier} of a {@code Spliterator} </param>
		''' <param name="characteristics"> Spliterator characteristics of the supplied
		'''        {@code Spliterator}.  The characteristics must be equal to
		'''        {@code supplier.get().characteristics()}, otherwise undefined
		'''        behavior may occur when terminal operation commences. </param>
		''' <param name="parallel"> if {@code true} then the returned stream is a parallel
		'''        stream; if {@code false} the returned stream is a sequential
		'''        stream. </param>
		''' <returns> a new sequential or parallel {@code Stream} </returns>
		''' <seealso cref= #stream(java.util.Spliterator, boolean) </seealso>
		Public Shared Function stream(Of T, T1 As java.util.Spliterator(Of T)(  supplier As java.util.function.Supplier(Of T1),   characteristics As Integer,   parallel As Boolean) As Stream(Of T)
			java.util.Objects.requireNonNull(supplier)
			Return New ReferencePipeline.Head(Of )(supplier, StreamOpFlag.fromCharacteristics(characteristics), parallel)
		End Function

		''' <summary>
		''' Creates a new sequential or parallel {@code IntStream} from a
		''' {@code Spliterator.OfInt}.
		''' 
		''' <p>The spliterator is only traversed, split, or queried for estimated size
		''' after the terminal operation of the stream pipeline commences.
		''' 
		''' <p>It is strongly recommended the spliterator report a characteristic of
		''' {@code IMMUTABLE} or {@code CONCURRENT}, or be
		''' <a href="../Spliterator.html#binding">late-binding</a>.  Otherwise,
		''' <seealso cref="#intStream(java.util.function.Supplier, int, boolean)"/> should be
		''' used to reduce the scope of potential interference with the source.  See
		''' <a href="package-summary.html#NonInterference">Non-Interference</a> for
		''' more details.
		''' </summary>
		''' <param name="spliterator"> a {@code Spliterator.OfInt} describing the stream elements </param>
		''' <param name="parallel"> if {@code true} then the returned stream is a parallel
		'''        stream; if {@code false} the returned stream is a sequential
		'''        stream. </param>
		''' <returns> a new sequential or parallel {@code IntStream} </returns>
		Public Shared Function intStream(  spliterator As java.util.Spliterator.OfInt,   parallel As Boolean) As IntStream
			Return New IntPipeline.Head(Of )(spliterator, StreamOpFlag.fromCharacteristics(spliterator), parallel)
		End Function

		''' <summary>
		''' Creates a new sequential or parallel {@code IntStream} from a
		''' {@code Supplier} of {@code Spliterator.OfInt}.
		''' 
		''' <p>The <seealso cref="Supplier#get()"/> method will be invoked on the supplier no
		''' more than once, and only after the terminal operation of the stream pipeline
		''' commences.
		''' 
		''' <p>For spliterators that report a characteristic of {@code IMMUTABLE}
		''' or {@code CONCURRENT}, or that are
		''' <a href="../Spliterator.html#binding">late-binding</a>, it is likely
		''' more efficient to use <seealso cref="#intStream(java.util.Spliterator.OfInt, boolean)"/>
		''' instead.
		''' <p>The use of a {@code Supplier} in this form provides a level of
		''' indirection that reduces the scope of potential interference with the
		''' source.  Since the supplier is only invoked after the terminal operation
		''' commences, any modifications to the source up to the start of the
		''' terminal operation are reflected in the stream result.  See
		''' <a href="package-summary.html#NonInterference">Non-Interference</a> for
		''' more details.
		''' </summary>
		''' <param name="supplier"> a {@code Supplier} of a {@code Spliterator.OfInt} </param>
		''' <param name="characteristics"> Spliterator characteristics of the supplied
		'''        {@code Spliterator.OfInt}.  The characteristics must be equal to
		'''        {@code supplier.get().characteristics()}, otherwise undefined
		'''        behavior may occur when terminal operation commences. </param>
		''' <param name="parallel"> if {@code true} then the returned stream is a parallel
		'''        stream; if {@code false} the returned stream is a sequential
		'''        stream. </param>
		''' <returns> a new sequential or parallel {@code IntStream} </returns>
		''' <seealso cref= #intStream(java.util.Spliterator.OfInt, boolean) </seealso>
		Public Shared Function intStream(Of T1 As java.util.Spliterator.OfInt)(  supplier As java.util.function.Supplier(Of T1),   characteristics As Integer,   parallel As Boolean) As IntStream
			Return New IntPipeline.Head(Of )(supplier, StreamOpFlag.fromCharacteristics(characteristics), parallel)
		End Function

		''' <summary>
		''' Creates a new sequential or parallel {@code LongStream} from a
		''' {@code Spliterator.OfLong}.
		''' 
		''' <p>The spliterator is only traversed, split, or queried for estimated
		''' size after the terminal operation of the stream pipeline commences.
		''' 
		''' <p>It is strongly recommended the spliterator report a characteristic of
		''' {@code IMMUTABLE} or {@code CONCURRENT}, or be
		''' <a href="../Spliterator.html#binding">late-binding</a>.  Otherwise,
		''' <seealso cref="#longStream(java.util.function.Supplier, int, boolean)"/> should be
		''' used to reduce the scope of potential interference with the source.  See
		''' <a href="package-summary.html#NonInterference">Non-Interference</a> for
		''' more details.
		''' </summary>
		''' <param name="spliterator"> a {@code Spliterator.OfLong} describing the stream elements </param>
		''' <param name="parallel"> if {@code true} then the returned stream is a parallel
		'''        stream; if {@code false} the returned stream is a sequential
		'''        stream. </param>
		''' <returns> a new sequential or parallel {@code LongStream} </returns>
		Public Shared Function longStream(  spliterator As java.util.Spliterator.OfLong,   parallel As Boolean) As LongStream
			Return New LongPipeline.Head(Of )(spliterator, StreamOpFlag.fromCharacteristics(spliterator), parallel)
		End Function

		''' <summary>
		''' Creates a new sequential or parallel {@code LongStream} from a
		''' {@code Supplier} of {@code Spliterator.OfLong}.
		''' 
		''' <p>The <seealso cref="Supplier#get()"/> method will be invoked on the supplier no
		''' more than once, and only after the terminal operation of the stream pipeline
		''' commences.
		''' 
		''' <p>For spliterators that report a characteristic of {@code IMMUTABLE}
		''' or {@code CONCURRENT}, or that are
		''' <a href="../Spliterator.html#binding">late-binding</a>, it is likely
		''' more efficient to use <seealso cref="#longStream(java.util.Spliterator.OfLong, boolean)"/>
		''' instead.
		''' <p>The use of a {@code Supplier} in this form provides a level of
		''' indirection that reduces the scope of potential interference with the
		''' source.  Since the supplier is only invoked after the terminal operation
		''' commences, any modifications to the source up to the start of the
		''' terminal operation are reflected in the stream result.  See
		''' <a href="package-summary.html#NonInterference">Non-Interference</a> for
		''' more details.
		''' </summary>
		''' <param name="supplier"> a {@code Supplier} of a {@code Spliterator.OfLong} </param>
		''' <param name="characteristics"> Spliterator characteristics of the supplied
		'''        {@code Spliterator.OfLong}.  The characteristics must be equal to
		'''        {@code supplier.get().characteristics()}, otherwise undefined
		'''        behavior may occur when terminal operation commences. </param>
		''' <param name="parallel"> if {@code true} then the returned stream is a parallel
		'''        stream; if {@code false} the returned stream is a sequential
		'''        stream. </param>
		''' <returns> a new sequential or parallel {@code LongStream} </returns>
		''' <seealso cref= #longStream(java.util.Spliterator.OfLong, boolean) </seealso>
		Public Shared Function longStream(Of T1 As java.util.Spliterator.OfLong)(  supplier As java.util.function.Supplier(Of T1),   characteristics As Integer,   parallel As Boolean) As LongStream
			Return New LongPipeline.Head(Of )(supplier, StreamOpFlag.fromCharacteristics(characteristics), parallel)
		End Function

		''' <summary>
		''' Creates a new sequential or parallel {@code DoubleStream} from a
		''' {@code Spliterator.OfDouble}.
		''' 
		''' <p>The spliterator is only traversed, split, or queried for estimated size
		''' after the terminal operation of the stream pipeline commences.
		''' 
		''' <p>It is strongly recommended the spliterator report a characteristic of
		''' {@code IMMUTABLE} or {@code CONCURRENT}, or be
		''' <a href="../Spliterator.html#binding">late-binding</a>.  Otherwise,
		''' <seealso cref="#doubleStream(java.util.function.Supplier, int, boolean)"/> should
		''' be used to reduce the scope of potential interference with the source.  See
		''' <a href="package-summary.html#NonInterference">Non-Interference</a> for
		''' more details.
		''' </summary>
		''' <param name="spliterator"> A {@code Spliterator.OfDouble} describing the stream elements </param>
		''' <param name="parallel"> if {@code true} then the returned stream is a parallel
		'''        stream; if {@code false} the returned stream is a sequential
		'''        stream. </param>
		''' <returns> a new sequential or parallel {@code DoubleStream} </returns>
		Public Shared Function doubleStream(  spliterator As java.util.Spliterator.OfDouble,   parallel As Boolean) As DoubleStream
			Return New DoublePipeline.Head(Of )(spliterator, StreamOpFlag.fromCharacteristics(spliterator), parallel)
		End Function

		''' <summary>
		''' Creates a new sequential or parallel {@code DoubleStream} from a
		''' {@code Supplier} of {@code Spliterator.OfDouble}.
		''' 
		''' <p>The <seealso cref="Supplier#get()"/> method will be invoked on the supplier no
		''' more than once, and only after the terminal operation of the stream pipeline
		''' commences.
		''' 
		''' <p>For spliterators that report a characteristic of {@code IMMUTABLE}
		''' or {@code CONCURRENT}, or that are
		''' <a href="../Spliterator.html#binding">late-binding</a>, it is likely
		''' more efficient to use <seealso cref="#doubleStream(java.util.Spliterator.OfDouble, boolean)"/>
		''' instead.
		''' <p>The use of a {@code Supplier} in this form provides a level of
		''' indirection that reduces the scope of potential interference with the
		''' source.  Since the supplier is only invoked after the terminal operation
		''' commences, any modifications to the source up to the start of the
		''' terminal operation are reflected in the stream result.  See
		''' <a href="package-summary.html#NonInterference">Non-Interference</a> for
		''' more details.
		''' </summary>
		''' <param name="supplier"> A {@code Supplier} of a {@code Spliterator.OfDouble} </param>
		''' <param name="characteristics"> Spliterator characteristics of the supplied
		'''        {@code Spliterator.OfDouble}.  The characteristics must be equal to
		'''        {@code supplier.get().characteristics()}, otherwise undefined
		'''        behavior may occur when terminal operation commences. </param>
		''' <param name="parallel"> if {@code true} then the returned stream is a parallel
		'''        stream; if {@code false} the returned stream is a sequential
		'''        stream. </param>
		''' <returns> a new sequential or parallel {@code DoubleStream} </returns>
		''' <seealso cref= #doubleStream(java.util.Spliterator.OfDouble, boolean) </seealso>
		Public Shared Function doubleStream(Of T1 As java.util.Spliterator.OfDouble)(  supplier As java.util.function.Supplier(Of T1),   characteristics As Integer,   parallel As Boolean) As DoubleStream
			Return New DoublePipeline.Head(Of )(supplier, StreamOpFlag.fromCharacteristics(characteristics), parallel)
		End Function
	End Class

End Namespace
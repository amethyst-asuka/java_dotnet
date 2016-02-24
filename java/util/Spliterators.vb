'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.util


	''' <summary>
	''' Static classes and methods for operating on or creating instances of
	''' <seealso cref="Spliterator"/> and its primitive specializations
	''' <seealso cref="Spliterator.OfInt"/>, <seealso cref="Spliterator.OfLong"/>, and
	''' <seealso cref="Spliterator.OfDouble"/>.
	''' </summary>
	''' <seealso cref= Spliterator
	''' @since 1.8 </seealso>
	Public NotInheritable Class Spliterators

		' Suppresses default constructor, ensuring non-instantiability.
		Private Sub New()
		End Sub

		' Empty spliterators

		''' <summary>
		''' Creates an empty {@code Spliterator}
		''' 
		''' <p>The empty spliterator reports <seealso cref="Spliterator#SIZED"/> and
		''' <seealso cref="Spliterator#SUBSIZED"/>.  Calls to
		''' <seealso cref="java.util.Spliterator#trySplit()"/> always return {@code null}.
		''' </summary>
		''' @param <T> Type of elements </param>
		''' <returns> An empty spliterator </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function emptySpliterator(Of T)() As Spliterator(Of T)
			Return CType(EMPTY_SPLITERATOR, Spliterator(Of T))
		End Function

		Private Shared ReadOnly EMPTY_SPLITERATOR As Spliterator(Of Object) = New EmptySpliterator.OfRef(Of Object)

		''' <summary>
		''' Creates an empty {@code Spliterator.OfInt}
		''' 
		''' <p>The empty spliterator reports <seealso cref="Spliterator#SIZED"/> and
		''' <seealso cref="Spliterator#SUBSIZED"/>.  Calls to
		''' <seealso cref="java.util.Spliterator#trySplit()"/> always return {@code null}.
		''' </summary>
		''' <returns> An empty spliterator </returns>
		Public Shared Function emptyIntSpliterator() As Spliterator.OfInt
			Return EMPTY_INT_SPLITERATOR
		End Function

		Private Shared ReadOnly EMPTY_INT_SPLITERATOR As Spliterator.OfInt = New EmptySpliterator.OfInt

		''' <summary>
		''' Creates an empty {@code Spliterator.OfLong}
		''' 
		''' <p>The empty spliterator reports <seealso cref="Spliterator#SIZED"/> and
		''' <seealso cref="Spliterator#SUBSIZED"/>.  Calls to
		''' <seealso cref="java.util.Spliterator#trySplit()"/> always return {@code null}.
		''' </summary>
		''' <returns> An empty spliterator </returns>
		Public Shared Function emptyLongSpliterator() As Spliterator.OfLong
			Return EMPTY_LONG_SPLITERATOR
		End Function

		Private Shared ReadOnly EMPTY_LONG_SPLITERATOR As Spliterator.OfLong = New EmptySpliterator.OfLong

		''' <summary>
		''' Creates an empty {@code Spliterator.OfDouble}
		''' 
		''' <p>The empty spliterator reports <seealso cref="Spliterator#SIZED"/> and
		''' <seealso cref="Spliterator#SUBSIZED"/>.  Calls to
		''' <seealso cref="java.util.Spliterator#trySplit()"/> always return {@code null}.
		''' </summary>
		''' <returns> An empty spliterator </returns>
		Public Shared Function emptyDoubleSpliterator() As Spliterator.OfDouble
			Return EMPTY_DOUBLE_SPLITERATOR
		End Function

		Private Shared ReadOnly EMPTY_DOUBLE_SPLITERATOR As Spliterator.OfDouble = New EmptySpliterator.OfDouble

		' Array-based spliterators

		''' <summary>
		''' Creates a {@code Spliterator} covering the elements of a given array,
		''' using a customized set of spliterator characteristics.
		''' 
		''' <p>This method is provided as an implementation convenience for
		''' Spliterators which store portions of their elements in arrays, and need
		''' fine control over Spliterator characteristics.  Most other situations in
		''' which a Spliterator for an array is needed should use
		''' <seealso cref="Arrays#spliterator(Object[])"/>.
		''' 
		''' <p>The returned spliterator always reports the characteristics
		''' {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		''' characteristics for the spliterator to report; it is common to
		''' additionally specify {@code IMMUTABLE} and {@code ORDERED}.
		''' </summary>
		''' @param <T> Type of elements </param>
		''' <param name="array"> The array, assumed to be unmodified during use </param>
		''' <param name="additionalCharacteristics"> Additional spliterator characteristics
		'''        of this spliterator's source or elements beyond {@code SIZED} and
		'''        {@code SUBSIZED} which are are always reported </param>
		''' <returns> A spliterator for an array </returns>
		''' <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		''' <seealso cref= Arrays#spliterator(Object[]) </seealso>
		Public Shared Function spliterator(Of T)(ByVal array As Object(), ByVal additionalCharacteristics As Integer) As Spliterator(Of T)
			Return New ArraySpliterator(Of )(Objects.requireNonNull(array), additionalCharacteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator} covering a range of elements of a given
		''' array, using a customized set of spliterator characteristics.
		''' 
		''' <p>This method is provided as an implementation convenience for
		''' Spliterators which store portions of their elements in arrays, and need
		''' fine control over Spliterator characteristics.  Most other situations in
		''' which a Spliterator for an array is needed should use
		''' <seealso cref="Arrays#spliterator(Object[])"/>.
		''' 
		''' <p>The returned spliterator always reports the characteristics
		''' {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		''' characteristics for the spliterator to report; it is common to
		''' additionally specify {@code IMMUTABLE} and {@code ORDERED}.
		''' </summary>
		''' @param <T> Type of elements </param>
		''' <param name="array"> The array, assumed to be unmodified during use </param>
		''' <param name="fromIndex"> The least index (inclusive) to cover </param>
		''' <param name="toIndex"> One past the greatest index to cover </param>
		''' <param name="additionalCharacteristics"> Additional spliterator characteristics
		'''        of this spliterator's source or elements beyond {@code SIZED} and
		'''        {@code SUBSIZED} which are are always reported </param>
		''' <returns> A spliterator for an array </returns>
		''' <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex} is negative,
		'''         {@code toIndex} is less than {@code fromIndex}, or
		'''         {@code toIndex} is greater than the array size </exception>
		''' <seealso cref= Arrays#spliterator(Object[], int, int) </seealso>
		Public Shared Function spliterator(Of T)(ByVal array As Object(), ByVal fromIndex As Integer, ByVal toIndex As Integer, ByVal additionalCharacteristics As Integer) As Spliterator(Of T)
			checkFromToBounds(Objects.requireNonNull(array).length, fromIndex, toIndex)
			Return New ArraySpliterator(Of )(array, fromIndex, toIndex, additionalCharacteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator.OfInt} covering the elements of a given array,
		''' using a customized set of spliterator characteristics.
		''' 
		''' <p>This method is provided as an implementation convenience for
		''' Spliterators which store portions of their elements in arrays, and need
		''' fine control over Spliterator characteristics.  Most other situations in
		''' which a Spliterator for an array is needed should use
		''' <seealso cref="Arrays#spliterator(int[])"/>.
		''' 
		''' <p>The returned spliterator always reports the characteristics
		''' {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		''' characteristics for the spliterator to report; it is common to
		''' additionally specify {@code IMMUTABLE} and {@code ORDERED}.
		''' </summary>
		''' <param name="array"> The array, assumed to be unmodified during use </param>
		''' <param name="additionalCharacteristics"> Additional spliterator characteristics
		'''        of this spliterator's source or elements beyond {@code SIZED} and
		'''        {@code SUBSIZED} which are are always reported </param>
		''' <returns> A spliterator for an array </returns>
		''' <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		''' <seealso cref= Arrays#spliterator(int[]) </seealso>
		Public Shared Function spliterator(ByVal array As Integer(), ByVal additionalCharacteristics As Integer) As Spliterator.OfInt
			Return New IntArraySpliterator(Objects.requireNonNull(array), additionalCharacteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator.OfInt} covering a range of elements of a
		''' given array, using a customized set of spliterator characteristics.
		''' 
		''' <p>This method is provided as an implementation convenience for
		''' Spliterators which store portions of their elements in arrays, and need
		''' fine control over Spliterator characteristics.  Most other situations in
		''' which a Spliterator for an array is needed should use
		''' <seealso cref="Arrays#spliterator(int[], int, int)"/>.
		''' 
		''' <p>The returned spliterator always reports the characteristics
		''' {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		''' characteristics for the spliterator to report; it is common to
		''' additionally specify {@code IMMUTABLE} and {@code ORDERED}.
		''' </summary>
		''' <param name="array"> The array, assumed to be unmodified during use </param>
		''' <param name="fromIndex"> The least index (inclusive) to cover </param>
		''' <param name="toIndex"> One past the greatest index to cover </param>
		''' <param name="additionalCharacteristics"> Additional spliterator characteristics
		'''        of this spliterator's source or elements beyond {@code SIZED} and
		'''        {@code SUBSIZED} which are are always reported </param>
		''' <returns> A spliterator for an array </returns>
		''' <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex} is negative,
		'''         {@code toIndex} is less than {@code fromIndex}, or
		'''         {@code toIndex} is greater than the array size </exception>
		''' <seealso cref= Arrays#spliterator(int[], int, int) </seealso>
		Public Shared Function spliterator(ByVal array As Integer(), ByVal fromIndex As Integer, ByVal toIndex As Integer, ByVal additionalCharacteristics As Integer) As Spliterator.OfInt
			checkFromToBounds(Objects.requireNonNull(array).length, fromIndex, toIndex)
			Return New IntArraySpliterator(array, fromIndex, toIndex, additionalCharacteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator.OfLong} covering the elements of a given array,
		''' using a customized set of spliterator characteristics.
		''' 
		''' <p>This method is provided as an implementation convenience for
		''' Spliterators which store portions of their elements in arrays, and need
		''' fine control over Spliterator characteristics.  Most other situations in
		''' which a Spliterator for an array is needed should use
		''' <seealso cref="Arrays#spliterator(long[])"/>.
		''' 
		''' <p>The returned spliterator always reports the characteristics
		''' {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		''' characteristics for the spliterator to report; it is common to
		''' additionally specify {@code IMMUTABLE} and {@code ORDERED}.
		''' </summary>
		''' <param name="array"> The array, assumed to be unmodified during use </param>
		''' <param name="additionalCharacteristics"> Additional spliterator characteristics
		'''        of this spliterator's source or elements beyond {@code SIZED} and
		'''        {@code SUBSIZED} which are are always reported </param>
		''' <returns> A spliterator for an array </returns>
		''' <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		''' <seealso cref= Arrays#spliterator(long[]) </seealso>
		Public Shared Function spliterator(ByVal array As Long(), ByVal additionalCharacteristics As Integer) As Spliterator.OfLong
			Return New LongArraySpliterator(Objects.requireNonNull(array), additionalCharacteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator.OfLong} covering a range of elements of a
		''' given array, using a customized set of spliterator characteristics.
		''' 
		''' <p>This method is provided as an implementation convenience for
		''' Spliterators which store portions of their elements in arrays, and need
		''' fine control over Spliterator characteristics.  Most other situations in
		''' which a Spliterator for an array is needed should use
		''' <seealso cref="Arrays#spliterator(long[], int, int)"/>.
		''' 
		''' <p>The returned spliterator always reports the characteristics
		''' {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		''' characteristics for the spliterator to report.  (For example, if it is
		''' known the array will not be further modified, specify {@code IMMUTABLE};
		''' if the array data is considered to have an an encounter order, specify
		''' {@code ORDERED}).  The method <seealso cref="Arrays#spliterator(long[], int, int)"/> can
		''' often be used instead, which returns a spliterator that reports
		''' {@code SIZED}, {@code SUBSIZED}, {@code IMMUTABLE}, and {@code ORDERED}.
		''' </summary>
		''' <param name="array"> The array, assumed to be unmodified during use </param>
		''' <param name="fromIndex"> The least index (inclusive) to cover </param>
		''' <param name="toIndex"> One past the greatest index to cover </param>
		''' <param name="additionalCharacteristics"> Additional spliterator characteristics
		'''        of this spliterator's source or elements beyond {@code SIZED} and
		'''        {@code SUBSIZED} which are are always reported </param>
		''' <returns> A spliterator for an array </returns>
		''' <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex} is negative,
		'''         {@code toIndex} is less than {@code fromIndex}, or
		'''         {@code toIndex} is greater than the array size </exception>
		''' <seealso cref= Arrays#spliterator(long[], int, int) </seealso>
		Public Shared Function spliterator(ByVal array As Long(), ByVal fromIndex As Integer, ByVal toIndex As Integer, ByVal additionalCharacteristics As Integer) As Spliterator.OfLong
			checkFromToBounds(Objects.requireNonNull(array).length, fromIndex, toIndex)
			Return New LongArraySpliterator(array, fromIndex, toIndex, additionalCharacteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator.OfDouble} covering the elements of a given array,
		''' using a customized set of spliterator characteristics.
		''' 
		''' <p>This method is provided as an implementation convenience for
		''' Spliterators which store portions of their elements in arrays, and need
		''' fine control over Spliterator characteristics.  Most other situations in
		''' which a Spliterator for an array is needed should use
		''' <seealso cref="Arrays#spliterator(double[])"/>.
		''' 
		''' <p>The returned spliterator always reports the characteristics
		''' {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		''' characteristics for the spliterator to report; it is common to
		''' additionally specify {@code IMMUTABLE} and {@code ORDERED}.
		''' </summary>
		''' <param name="array"> The array, assumed to be unmodified during use </param>
		''' <param name="additionalCharacteristics"> Additional spliterator characteristics
		'''        of this spliterator's source or elements beyond {@code SIZED} and
		'''        {@code SUBSIZED} which are are always reported </param>
		''' <returns> A spliterator for an array </returns>
		''' <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		''' <seealso cref= Arrays#spliterator(double[]) </seealso>
		Public Shared Function spliterator(ByVal array As Double(), ByVal additionalCharacteristics As Integer) As Spliterator.OfDouble
			Return New DoubleArraySpliterator(Objects.requireNonNull(array), additionalCharacteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator.OfDouble} covering a range of elements of a
		''' given array, using a customized set of spliterator characteristics.
		''' 
		''' <p>This method is provided as an implementation convenience for
		''' Spliterators which store portions of their elements in arrays, and need
		''' fine control over Spliterator characteristics.  Most other situations in
		''' which a Spliterator for an array is needed should use
		''' <seealso cref="Arrays#spliterator(double[], int, int)"/>.
		''' 
		''' <p>The returned spliterator always reports the characteristics
		''' {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		''' characteristics for the spliterator to report.  (For example, if it is
		''' known the array will not be further modified, specify {@code IMMUTABLE};
		''' if the array data is considered to have an an encounter order, specify
		''' {@code ORDERED}).  The method <seealso cref="Arrays#spliterator(long[], int, int)"/> can
		''' often be used instead, which returns a spliterator that reports
		''' {@code SIZED}, {@code SUBSIZED}, {@code IMMUTABLE}, and {@code ORDERED}.
		''' </summary>
		''' <param name="array"> The array, assumed to be unmodified during use </param>
		''' <param name="fromIndex"> The least index (inclusive) to cover </param>
		''' <param name="toIndex"> One past the greatest index to cover </param>
		''' <param name="additionalCharacteristics"> Additional spliterator characteristics
		'''        of this spliterator's source or elements beyond {@code SIZED} and
		'''        {@code SUBSIZED} which are are always reported </param>
		''' <returns> A spliterator for an array </returns>
		''' <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex} is negative,
		'''         {@code toIndex} is less than {@code fromIndex}, or
		'''         {@code toIndex} is greater than the array size </exception>
		''' <seealso cref= Arrays#spliterator(double[], int, int) </seealso>
		Public Shared Function spliterator(ByVal array As Double(), ByVal fromIndex As Integer, ByVal toIndex As Integer, ByVal additionalCharacteristics As Integer) As Spliterator.OfDouble
			checkFromToBounds(Objects.requireNonNull(array).length, fromIndex, toIndex)
			Return New DoubleArraySpliterator(array, fromIndex, toIndex, additionalCharacteristics)
		End Function

		''' <summary>
		''' Validate inclusive start index and exclusive end index against the length
		''' of an array. </summary>
		''' <param name="arrayLength"> The length of the array </param>
		''' <param name="origin"> The inclusive start index </param>
		''' <param name="fence"> The exclusive end index </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the start index is greater than
		''' the end index, if the start index is negative, or the end index is
		''' greater than the array length </exception>
		Private Shared Sub checkFromToBounds(ByVal arrayLength As Integer, ByVal origin As Integer, ByVal fence As Integer)
			If origin > fence Then Throw New ArrayIndexOutOfBoundsException("origin(" & origin & ") > fence(" & fence & ")")
			If origin < 0 Then Throw New ArrayIndexOutOfBoundsException(origin)
			If fence > arrayLength Then Throw New ArrayIndexOutOfBoundsException(fence)
		End Sub

		' Iterator-based spliterators

		''' <summary>
		''' Creates a {@code Spliterator} using the given collection's
		''' <seealso cref="java.util.Collection#iterator()"/> as the source of elements, and
		''' reporting its <seealso cref="java.util.Collection#size()"/> as its initial size.
		''' 
		''' <p>The spliterator is
		''' <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		''' the <em>fail-fast</em> properties of the collection's iterator, and
		''' implements {@code trySplit} to permit limited parallelism.
		''' </summary>
		''' @param <T> Type of elements </param>
		''' <param name="c"> The collection </param>
		''' <param name="characteristics"> Characteristics of this spliterator's source or
		'''        elements.  The characteristics {@code SIZED} and {@code SUBSIZED}
		'''        are additionally reported unless {@code CONCURRENT} is supplied. </param>
		''' <returns> A spliterator from an iterator </returns>
		''' <exception cref="NullPointerException"> if the given collection is {@code null} </exception>
		Public Shared Function spliterator(Of T, T1 As T)(ByVal c As Collection(Of T1), ByVal characteristics As Integer) As Spliterator(Of T)
			Return New IteratorSpliterator(Of )(Objects.requireNonNull(c), characteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator} using a given {@code Iterator}
		''' as the source of elements, and with a given initially reported size.
		''' 
		''' <p>The spliterator is not
		''' <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		''' the <em>fail-fast</em> properties of the iterator, and implements
		''' {@code trySplit} to permit limited parallelism.
		''' 
		''' <p>Traversal of elements should be accomplished through the spliterator.
		''' The behaviour of splitting and traversal is undefined if the iterator is
		''' operated on after the spliterator is returned, or the initially reported
		''' size is not equal to the actual number of elements in the source.
		''' </summary>
		''' @param <T> Type of elements </param>
		''' <param name="iterator"> The iterator for the source </param>
		''' <param name="size"> The number of elements in the source, to be reported as
		'''        initial {@code estimateSize} </param>
		''' <param name="characteristics"> Characteristics of this spliterator's source or
		'''        elements.  The characteristics {@code SIZED} and {@code SUBSIZED}
		'''        are additionally reported unless {@code CONCURRENT} is supplied. </param>
		''' <returns> A spliterator from an iterator </returns>
		''' <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		Public Shared Function spliterator(Of T, T1 As T)(ByVal [iterator] As [Iterator](Of T1), ByVal size As Long, ByVal characteristics As Integer) As Spliterator(Of T)
			Return New IteratorSpliterator(Of )(Objects.requireNonNull([iterator]), size, characteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator} using a given {@code Iterator}
		''' as the source of elements, with no initial size estimate.
		''' 
		''' <p>The spliterator is not
		''' <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		''' the <em>fail-fast</em> properties of the iterator, and implements
		''' {@code trySplit} to permit limited parallelism.
		''' 
		''' <p>Traversal of elements should be accomplished through the spliterator.
		''' The behaviour of splitting and traversal is undefined if the iterator is
		''' operated on after the spliterator is returned.
		''' </summary>
		''' @param <T> Type of elements </param>
		''' <param name="iterator"> The iterator for the source </param>
		''' <param name="characteristics"> Characteristics of this spliterator's source
		'''        or elements ({@code SIZED} and {@code SUBSIZED}, if supplied, are
		'''        ignored and are not reported.) </param>
		''' <returns> A spliterator from an iterator </returns>
		''' <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		Public Shared Function spliteratorUnknownSize(Of T, T1 As T)(ByVal [iterator] As [Iterator](Of T1), ByVal characteristics As Integer) As Spliterator(Of T)
			Return New IteratorSpliterator(Of )(Objects.requireNonNull([iterator]), characteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator.OfInt} using a given
		''' {@code IntStream.IntIterator} as the source of elements, and with a given
		''' initially reported size.
		''' 
		''' <p>The spliterator is not
		''' <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		''' the <em>fail-fast</em> properties of the iterator, and implements
		''' {@code trySplit} to permit limited parallelism.
		''' 
		''' <p>Traversal of elements should be accomplished through the spliterator.
		''' The behaviour of splitting and traversal is undefined if the iterator is
		''' operated on after the spliterator is returned, or the initially reported
		''' size is not equal to the actual number of elements in the source.
		''' </summary>
		''' <param name="iterator"> The iterator for the source </param>
		''' <param name="size"> The number of elements in the source, to be reported as
		'''        initial {@code estimateSize}. </param>
		''' <param name="characteristics"> Characteristics of this spliterator's source or
		'''        elements.  The characteristics {@code SIZED} and {@code SUBSIZED}
		'''        are additionally reported unless {@code CONCURRENT} is supplied. </param>
		''' <returns> A spliterator from an iterator </returns>
		''' <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		Public Shared Function spliterator(ByVal [iterator] As PrimitiveIterator.OfInt, ByVal size As Long, ByVal characteristics As Integer) As Spliterator.OfInt
			Return New IntIteratorSpliterator(Objects.requireNonNull([iterator]), size, characteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator.OfInt} using a given
		''' {@code IntStream.IntIterator} as the source of elements, with no initial
		''' size estimate.
		''' 
		''' <p>The spliterator is not
		''' <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		''' the <em>fail-fast</em> properties of the iterator, and implements
		''' {@code trySplit} to permit limited parallelism.
		''' 
		''' <p>Traversal of elements should be accomplished through the spliterator.
		''' The behaviour of splitting and traversal is undefined if the iterator is
		''' operated on after the spliterator is returned.
		''' </summary>
		''' <param name="iterator"> The iterator for the source </param>
		''' <param name="characteristics"> Characteristics of this spliterator's source
		'''        or elements ({@code SIZED} and {@code SUBSIZED}, if supplied, are
		'''        ignored and are not reported.) </param>
		''' <returns> A spliterator from an iterator </returns>
		''' <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		Public Shared Function spliteratorUnknownSize(ByVal [iterator] As PrimitiveIterator.OfInt, ByVal characteristics As Integer) As Spliterator.OfInt
			Return New IntIteratorSpliterator(Objects.requireNonNull([iterator]), characteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator.OfLong} using a given
		''' {@code LongStream.LongIterator} as the source of elements, and with a
		''' given initially reported size.
		''' 
		''' <p>The spliterator is not
		''' <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		''' the <em>fail-fast</em> properties of the iterator, and implements
		''' {@code trySplit} to permit limited parallelism.
		''' 
		''' <p>Traversal of elements should be accomplished through the spliterator.
		''' The behaviour of splitting and traversal is undefined if the iterator is
		''' operated on after the spliterator is returned, or the initially reported
		''' size is not equal to the actual number of elements in the source.
		''' </summary>
		''' <param name="iterator"> The iterator for the source </param>
		''' <param name="size"> The number of elements in the source, to be reported as
		'''        initial {@code estimateSize}. </param>
		''' <param name="characteristics"> Characteristics of this spliterator's source or
		'''        elements.  The characteristics {@code SIZED} and {@code SUBSIZED}
		'''        are additionally reported unless {@code CONCURRENT} is supplied. </param>
		''' <returns> A spliterator from an iterator </returns>
		''' <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		Public Shared Function spliterator(ByVal [iterator] As PrimitiveIterator.OfLong, ByVal size As Long, ByVal characteristics As Integer) As Spliterator.OfLong
			Return New LongIteratorSpliterator(Objects.requireNonNull([iterator]), size, characteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator.OfLong} using a given
		''' {@code LongStream.LongIterator} as the source of elements, with no
		''' initial size estimate.
		''' 
		''' <p>The spliterator is not
		''' <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		''' the <em>fail-fast</em> properties of the iterator, and implements
		''' {@code trySplit} to permit limited parallelism.
		''' 
		''' <p>Traversal of elements should be accomplished through the spliterator.
		''' The behaviour of splitting and traversal is undefined if the iterator is
		''' operated on after the spliterator is returned.
		''' </summary>
		''' <param name="iterator"> The iterator for the source </param>
		''' <param name="characteristics"> Characteristics of this spliterator's source
		'''        or elements ({@code SIZED} and {@code SUBSIZED}, if supplied, are
		'''        ignored and are not reported.) </param>
		''' <returns> A spliterator from an iterator </returns>
		''' <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		Public Shared Function spliteratorUnknownSize(ByVal [iterator] As PrimitiveIterator.OfLong, ByVal characteristics As Integer) As Spliterator.OfLong
			Return New LongIteratorSpliterator(Objects.requireNonNull([iterator]), characteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator.OfDouble} using a given
		''' {@code DoubleStream.DoubleIterator} as the source of elements, and with a
		''' given initially reported size.
		''' 
		''' <p>The spliterator is not
		''' <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		''' the <em>fail-fast</em> properties of the iterator, and implements
		''' {@code trySplit} to permit limited parallelism.
		''' 
		''' <p>Traversal of elements should be accomplished through the spliterator.
		''' The behaviour of splitting and traversal is undefined if the iterator is
		''' operated on after the spliterator is returned, or the initially reported
		''' size is not equal to the actual number of elements in the source.
		''' </summary>
		''' <param name="iterator"> The iterator for the source </param>
		''' <param name="size"> The number of elements in the source, to be reported as
		'''        initial {@code estimateSize} </param>
		''' <param name="characteristics"> Characteristics of this spliterator's source or
		'''        elements.  The characteristics {@code SIZED} and {@code SUBSIZED}
		'''        are additionally reported unless {@code CONCURRENT} is supplied. </param>
		''' <returns> A spliterator from an iterator </returns>
		''' <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		Public Shared Function spliterator(ByVal [iterator] As PrimitiveIterator.OfDouble, ByVal size As Long, ByVal characteristics As Integer) As Spliterator.OfDouble
			Return New DoubleIteratorSpliterator(Objects.requireNonNull([iterator]), size, characteristics)
		End Function

		''' <summary>
		''' Creates a {@code Spliterator.OfDouble} using a given
		''' {@code DoubleStream.DoubleIterator} as the source of elements, with no
		''' initial size estimate.
		''' 
		''' <p>The spliterator is not
		''' <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		''' the <em>fail-fast</em> properties of the iterator, and implements
		''' {@code trySplit} to permit limited parallelism.
		''' 
		''' <p>Traversal of elements should be accomplished through the spliterator.
		''' The behaviour of splitting and traversal is undefined if the iterator is
		''' operated on after the spliterator is returned.
		''' </summary>
		''' <param name="iterator"> The iterator for the source </param>
		''' <param name="characteristics"> Characteristics of this spliterator's source
		'''        or elements ({@code SIZED} and {@code SUBSIZED}, if supplied, are
		'''        ignored and are not reported.) </param>
		''' <returns> A spliterator from an iterator </returns>
		''' <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		Public Shared Function spliteratorUnknownSize(ByVal [iterator] As PrimitiveIterator.OfDouble, ByVal characteristics As Integer) As Spliterator.OfDouble
			Return New DoubleIteratorSpliterator(Objects.requireNonNull([iterator]), characteristics)
		End Function

		' Iterators from Spliterators

		''' <summary>
		''' Creates an {@code Iterator} from a {@code Spliterator}.
		''' 
		''' <p>Traversal of elements should be accomplished through the iterator.
		''' The behaviour of traversal is undefined if the spliterator is operated
		''' after the iterator is returned.
		''' </summary>
		''' @param <T> Type of elements </param>
		''' <param name="spliterator"> The spliterator </param>
		''' <returns> An iterator </returns>
		''' <exception cref="NullPointerException"> if the given spliterator is {@code null} </exception>
		Public Shared Function [iterator](Of T, T1 As T)(ByVal spliterator As Spliterator(Of T1)) As [Iterator](Of T)
			Objects.requireNonNull(spliterator)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class Adapter implements Iterator(Of T), java.util.function.Consumer(Of T)
	'		{
	'			boolean valueReady = False;
	'			T nextElement;
	'
	'			@Override public void accept(T t)
	'			{
	'				valueReady = True;
	'				nextElement = t;
	'			}
	'
	'			@Override public boolean hasNext()
	'			{
	'				if (!valueReady)
	'					spliterator.tryAdvance(Me);
	'				Return valueReady;
	'			}
	'
	'			@Override public T next()
	'			{
	'				if (!valueReady && !hasNext())
	'					throw New NoSuchElementException();
	'				else
	'				{
	'					valueReady = False;
	'					Return nextElement;
	'				}
	'			}
	'		}

			Return New Adapter
		End Function

		''' <summary>
		''' Creates an {@code PrimitiveIterator.OfInt} from a
		''' {@code Spliterator.OfInt}.
		''' 
		''' <p>Traversal of elements should be accomplished through the iterator.
		''' The behaviour of traversal is undefined if the spliterator is operated
		''' after the iterator is returned.
		''' </summary>
		''' <param name="spliterator"> The spliterator </param>
		''' <returns> An iterator </returns>
		''' <exception cref="NullPointerException"> if the given spliterator is {@code null} </exception>
		Public Shared Function [iterator](ByVal spliterator As Spliterator.OfInt) As PrimitiveIterator.OfInt
			Objects.requireNonNull(spliterator)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class Adapter implements PrimitiveIterator.OfInt, java.util.function.IntConsumer
	'		{
	'			boolean valueReady = False;
	'			int nextElement;
	'
	'			@Override public void accept(int t)
	'			{
	'				valueReady = True;
	'				nextElement = t;
	'			}
	'
	'			@Override public boolean hasNext()
	'			{
	'				if (!valueReady)
	'					spliterator.tryAdvance(Me);
	'				Return valueReady;
	'			}
	'
	'			@Override public int nextInt()
	'			{
	'				if (!valueReady && !hasNext())
	'					throw New NoSuchElementException();
	'				else
	'				{
	'					valueReady = False;
	'					Return nextElement;
	'				}
	'			}
	'		}

			Return New Adapter
		End Function

		''' <summary>
		''' Creates an {@code PrimitiveIterator.OfLong} from a
		''' {@code Spliterator.OfLong}.
		''' 
		''' <p>Traversal of elements should be accomplished through the iterator.
		''' The behaviour of traversal is undefined if the spliterator is operated
		''' after the iterator is returned.
		''' </summary>
		''' <param name="spliterator"> The spliterator </param>
		''' <returns> An iterator </returns>
		''' <exception cref="NullPointerException"> if the given spliterator is {@code null} </exception>
		Public Shared Function [iterator](ByVal spliterator As Spliterator.OfLong) As PrimitiveIterator.OfLong
			Objects.requireNonNull(spliterator)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class Adapter implements PrimitiveIterator.OfLong, java.util.function.LongConsumer
	'		{
	'			boolean valueReady = False;
	'			long nextElement;
	'
	'			@Override public void accept(long t)
	'			{
	'				valueReady = True;
	'				nextElement = t;
	'			}
	'
	'			@Override public boolean hasNext()
	'			{
	'				if (!valueReady)
	'					spliterator.tryAdvance(Me);
	'				Return valueReady;
	'			}
	'
	'			@Override public long nextLong()
	'			{
	'				if (!valueReady && !hasNext())
	'					throw New NoSuchElementException();
	'				else
	'				{
	'					valueReady = False;
	'					Return nextElement;
	'				}
	'			}
	'		}

			Return New Adapter
		End Function

		''' <summary>
		''' Creates an {@code PrimitiveIterator.OfDouble} from a
		''' {@code Spliterator.OfDouble}.
		''' 
		''' <p>Traversal of elements should be accomplished through the iterator.
		''' The behaviour of traversal is undefined if the spliterator is operated
		''' after the iterator is returned.
		''' </summary>
		''' <param name="spliterator"> The spliterator </param>
		''' <returns> An iterator </returns>
		''' <exception cref="NullPointerException"> if the given spliterator is {@code null} </exception>
		Public Shared Function [iterator](ByVal spliterator As Spliterator.OfDouble) As PrimitiveIterator.OfDouble
			Objects.requireNonNull(spliterator)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class Adapter implements PrimitiveIterator.OfDouble, java.util.function.DoubleConsumer
	'		{
	'			boolean valueReady = False;
	'			double nextElement;
	'
	'			@Override public void accept(double t)
	'			{
	'				valueReady = True;
	'				nextElement = t;
	'			}
	'
	'			@Override public boolean hasNext()
	'			{
	'				if (!valueReady)
	'					spliterator.tryAdvance(Me);
	'				Return valueReady;
	'			}
	'
	'			@Override public double nextDouble()
	'			{
	'				if (!valueReady && !hasNext())
	'					throw New NoSuchElementException();
	'				else
	'				{
	'					valueReady = False;
	'					Return nextElement;
	'				}
	'			}
	'		}

			Return New Adapter
		End Function

		' Implementations

		Private MustInherit Class EmptySpliterator(Of T, S As Spliterator(Of T), C)

			Friend Sub New()
			End Sub

			Public Overridable Function trySplit() As S
				Return Nothing
			End Function

			Public Overridable Function tryAdvance(ByVal consumer As C) As Boolean
				Objects.requireNonNull(consumer)
				Return False
			End Function

			Public Overridable Sub forEachRemaining(ByVal consumer As C)
				Objects.requireNonNull(consumer)
			End Sub

			Public Overridable Function estimateSize() As Long
				Return 0
			End Function

			Public Overridable Function characteristics() As Integer
				Return Spliterator.SIZED Or Spliterator.SUBSIZED
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Private NotInheritable Class OfRef(Of T)
				Inherits EmptySpliterator(Of T, Spliterator(Of T), java.util.function.Consumer(Of JavaToDotNetGenericWildcard))
				Implements Spliterator(Of T)

				Friend Sub New()
				End Sub
			End Class

			Private NotInheritable Class OfInt
				Inherits EmptySpliterator(Of Integer?, Spliterator.OfInt, java.util.function.IntConsumer)
				Implements Spliterator.OfInt

				Friend Sub New()
				End Sub
			End Class

			Private NotInheritable Class OfLong
				Inherits EmptySpliterator(Of Long?, Spliterator.OfLong, java.util.function.LongConsumer)
				Implements Spliterator.OfLong

				Friend Sub New()
				End Sub
			End Class

			Private NotInheritable Class OfDouble
				Inherits EmptySpliterator(Of Double?, Spliterator.OfDouble, java.util.function.DoubleConsumer)
				Implements Spliterator.OfDouble

				Friend Sub New()
				End Sub
			End Class
		End Class

		' Array-based spliterators

		''' <summary>
		''' A Spliterator designed for use by sources that traverse and split
		''' elements maintained in an unmodifiable {@code Object[]} array.
		''' </summary>
		Friend NotInheritable Class ArraySpliterator(Of T)
			Implements Spliterator(Of T)

			''' <summary>
			''' The array, explicitly typed as Object[]. Unlike in some other
			''' classes (see for example CR 6260652), we do not need to
			''' screen arguments to ensure they are exactly of type Object[]
			''' so long as no methods write into the array or serialize it,
			''' which we ensure here by defining this class as final.
			''' </summary>
			Private ReadOnly array As Object()
			Private index As Integer ' current index, modified on advance/split
			Private ReadOnly fence As Integer ' one past last index
			Private ReadOnly characteristics_Renamed As Integer

			''' <summary>
			''' Creates a spliterator covering all of the given array. </summary>
			''' <param name="array"> the array, assumed to be unmodified during use </param>
			''' <param name="additionalCharacteristics"> Additional spliterator characteristics
			''' of this spliterator's source or elements beyond {@code SIZED} and
			''' {@code SUBSIZED} which are are always reported </param>
			Public Sub New(ByVal array As Object(), ByVal additionalCharacteristics As Integer)
				Me.New(array, 0, array.Length, additionalCharacteristics)
			End Sub

			''' <summary>
			''' Creates a spliterator covering the given array and range </summary>
			''' <param name="array"> the array, assumed to be unmodified during use </param>
			''' <param name="origin"> the least index (inclusive) to cover </param>
			''' <param name="fence"> one past the greatest index to cover </param>
			''' <param name="additionalCharacteristics"> Additional spliterator characteristics
			''' of this spliterator's source or elements beyond {@code SIZED} and
			''' {@code SUBSIZED} which are are always reported </param>
			Public Sub New(ByVal array As Object(), ByVal origin As Integer, ByVal fence As Integer, ByVal additionalCharacteristics As Integer)
				Me.array = array
				Me.index = origin
				Me.fence = fence
				Me.characteristics_Renamed = additionalCharacteristics Or Spliterator.SIZED Or Spliterator.SUBSIZED
			End Sub

			Public Overrides Function trySplit() As Spliterator(Of T) Implements Spliterator(Of T).trySplit
				Dim lo As Integer = index, mid As Integer = CInt(CUInt((lo + fence)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New ArraySpliterator(Of )(array, lo, index = mid, characteristics_Renamed))
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of T).forEachRemaining
				Dim a As Object() ' hoist accesses and checks from loop
				Dim i, hi As Integer
				If action Is Nothing Then Throw New NullPointerException
				a = array
				hi = fence
				i = index
				index = hi
				If a .length >= hi AndAlso i >= 0 AndAlso i < index Then
					Do
						action.accept(CType(a(i), T))
						i += 1
					Loop While i < hi
				End If
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of T).tryAdvance
				If action Is Nothing Then Throw New NullPointerException
				If index >= 0 AndAlso index < fence Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim e As T = CType(array(index), T)
					index += 1
					action.accept(e)
					Return True
				End If
				Return False
			End Function

			Public Overrides Function estimateSize() As Long Implements Spliterator(Of T).estimateSize
				Return CLng(fence - index)
			End Function
			Public Overrides Function characteristics() As Integer Implements Spliterator(Of T).characteristics
				Return characteristics_Renamed
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overrides Function getComparator() As Comparator(Of ?) Implements Spliterator(Of T).getComparator
				If hasCharacteristics(Spliterator.SORTED) Then Return Nothing
				Throw New IllegalStateException
			End Function
		End Class

		''' <summary>
		''' A Spliterator.OfInt designed for use by sources that traverse and split
		''' elements maintained in an unmodifiable {@code int[]} array.
		''' </summary>
		Friend NotInheritable Class IntArraySpliterator
			Implements Spliterator.OfInt

			Private ReadOnly array As Integer()
			Private index As Integer ' current index, modified on advance/split
			Private ReadOnly fence As Integer ' one past last index
			Private ReadOnly characteristics_Renamed As Integer

			''' <summary>
			''' Creates a spliterator covering all of the given array. </summary>
			''' <param name="array"> the array, assumed to be unmodified during use </param>
			''' <param name="additionalCharacteristics"> Additional spliterator characteristics
			'''        of this spliterator's source or elements beyond {@code SIZED} and
			'''        {@code SUBSIZED} which are are always reported </param>
			Public Sub New(ByVal array As Integer(), ByVal additionalCharacteristics As Integer)
				Me.New(array, 0, array.Length, additionalCharacteristics)
			End Sub

			''' <summary>
			''' Creates a spliterator covering the given array and range </summary>
			''' <param name="array"> the array, assumed to be unmodified during use </param>
			''' <param name="origin"> the least index (inclusive) to cover </param>
			''' <param name="fence"> one past the greatest index to cover </param>
			''' <param name="additionalCharacteristics"> Additional spliterator characteristics
			'''        of this spliterator's source or elements beyond {@code SIZED} and
			'''        {@code SUBSIZED} which are are always reported </param>
			Public Sub New(ByVal array As Integer(), ByVal origin As Integer, ByVal fence As Integer, ByVal additionalCharacteristics As Integer)
				Me.array = array
				Me.index = origin
				Me.fence = fence
				Me.characteristics_Renamed = additionalCharacteristics Or Spliterator.SIZED Or Spliterator.SUBSIZED
			End Sub

			Public Overrides Function trySplit() As OfInt
				Dim lo As Integer = index, mid As Integer = CInt(CUInt((lo + fence)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New IntArraySpliterator(array, lo, index = mid, characteristics_Renamed))
			End Function

			Public Overrides Sub forEachRemaining(ByVal action As java.util.function.IntConsumer)
				Dim a As Integer() ' hoist accesses and checks from loop
				Dim i, hi As Integer
				If action Is Nothing Then Throw New NullPointerException
				a = array
				hi = fence
				i = index
				index = hi
				If a .length >= hi AndAlso i >= 0 AndAlso i < index Then
					Do
						action.accept(a(i))
						i += 1
					Loop While i < hi
				End If
			End Sub

			Public Overrides Function tryAdvance(ByVal action As java.util.function.IntConsumer) As Boolean
				If action Is Nothing Then Throw New NullPointerException
				If index >= 0 AndAlso index < fence Then
					action.accept(array(index))
					index += 1
					Return True
				End If
				Return False
			End Function

			Public Overrides Function estimateSize() As Long
				Return CLng(fence - index)
			End Function
			Public Overrides Function characteristics() As Integer
				Return characteristics_Renamed
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Property Overrides comparator As Comparator(Of ?)
				Get
					If hasCharacteristics(Spliterator.SORTED) Then Return Nothing
					Throw New IllegalStateException
				End Get
			End Property
		End Class

		''' <summary>
		''' A Spliterator.OfLong designed for use by sources that traverse and split
		''' elements maintained in an unmodifiable {@code int[]} array.
		''' </summary>
		Friend NotInheritable Class LongArraySpliterator
			Implements Spliterator.OfLong

			Private ReadOnly array As Long()
			Private index As Integer ' current index, modified on advance/split
			Private ReadOnly fence As Integer ' one past last index
			Private ReadOnly characteristics_Renamed As Integer

			''' <summary>
			''' Creates a spliterator covering all of the given array. </summary>
			''' <param name="array"> the array, assumed to be unmodified during use </param>
			''' <param name="additionalCharacteristics"> Additional spliterator characteristics
			'''        of this spliterator's source or elements beyond {@code SIZED} and
			'''        {@code SUBSIZED} which are are always reported </param>
			Public Sub New(ByVal array As Long(), ByVal additionalCharacteristics As Integer)
				Me.New(array, 0, array.Length, additionalCharacteristics)
			End Sub

			''' <summary>
			''' Creates a spliterator covering the given array and range </summary>
			''' <param name="array"> the array, assumed to be unmodified during use </param>
			''' <param name="origin"> the least index (inclusive) to cover </param>
			''' <param name="fence"> one past the greatest index to cover </param>
			''' <param name="additionalCharacteristics"> Additional spliterator characteristics
			'''        of this spliterator's source or elements beyond {@code SIZED} and
			'''        {@code SUBSIZED} which are are always reported </param>
			Public Sub New(ByVal array As Long(), ByVal origin As Integer, ByVal fence As Integer, ByVal additionalCharacteristics As Integer)
				Me.array = array
				Me.index = origin
				Me.fence = fence
				Me.characteristics_Renamed = additionalCharacteristics Or Spliterator.SIZED Or Spliterator.SUBSIZED
			End Sub

			Public Overrides Function trySplit() As OfLong
				Dim lo As Integer = index, mid As Integer = CInt(CUInt((lo + fence)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New LongArraySpliterator(array, lo, index = mid, characteristics_Renamed))
			End Function

			Public Overrides Sub forEachRemaining(ByVal action As java.util.function.LongConsumer)
				Dim a As Long() ' hoist accesses and checks from loop
				Dim i, hi As Integer
				If action Is Nothing Then Throw New NullPointerException
				a = array
				hi = fence
				i = index
				index = hi
				If a .length >= hi AndAlso i >= 0 AndAlso i < index Then
					Do
						action.accept(a(i))
						i += 1
					Loop While i < hi
				End If
			End Sub

			Public Overrides Function tryAdvance(ByVal action As java.util.function.LongConsumer) As Boolean
				If action Is Nothing Then Throw New NullPointerException
				If index >= 0 AndAlso index < fence Then
					action.accept(array(index))
					index += 1
					Return True
				End If
				Return False
			End Function

			Public Overrides Function estimateSize() As Long
				Return CLng(fence - index)
			End Function
			Public Overrides Function characteristics() As Integer
				Return characteristics_Renamed
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Property Overrides comparator As Comparator(Of ?)
				Get
					If hasCharacteristics(Spliterator.SORTED) Then Return Nothing
					Throw New IllegalStateException
				End Get
			End Property
		End Class

		''' <summary>
		''' A Spliterator.OfDouble designed for use by sources that traverse and split
		''' elements maintained in an unmodifiable {@code int[]} array.
		''' </summary>
		Friend NotInheritable Class DoubleArraySpliterator
			Implements Spliterator.OfDouble

			Private ReadOnly array As Double()
			Private index As Integer ' current index, modified on advance/split
			Private ReadOnly fence As Integer ' one past last index
			Private ReadOnly characteristics_Renamed As Integer

			''' <summary>
			''' Creates a spliterator covering all of the given array. </summary>
			''' <param name="array"> the array, assumed to be unmodified during use </param>
			''' <param name="additionalCharacteristics"> Additional spliterator characteristics
			'''        of this spliterator's source or elements beyond {@code SIZED} and
			'''        {@code SUBSIZED} which are are always reported </param>
			Public Sub New(ByVal array As Double(), ByVal additionalCharacteristics As Integer)
				Me.New(array, 0, array.Length, additionalCharacteristics)
			End Sub

			''' <summary>
			''' Creates a spliterator covering the given array and range </summary>
			''' <param name="array"> the array, assumed to be unmodified during use </param>
			''' <param name="origin"> the least index (inclusive) to cover </param>
			''' <param name="fence"> one past the greatest index to cover </param>
			''' <param name="additionalCharacteristics"> Additional spliterator characteristics
			'''        of this spliterator's source or elements beyond {@code SIZED} and
			'''        {@code SUBSIZED} which are are always reported </param>
			Public Sub New(ByVal array As Double(), ByVal origin As Integer, ByVal fence As Integer, ByVal additionalCharacteristics As Integer)
				Me.array = array
				Me.index = origin
				Me.fence = fence
				Me.characteristics_Renamed = additionalCharacteristics Or Spliterator.SIZED Or Spliterator.SUBSIZED
			End Sub

			Public Overrides Function trySplit() As OfDouble
				Dim lo As Integer = index, mid As Integer = CInt(CUInt((lo + fence)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New DoubleArraySpliterator(array, lo, index = mid, characteristics_Renamed))
			End Function

			Public Overrides Sub forEachRemaining(ByVal action As java.util.function.DoubleConsumer)
				Dim a As Double() ' hoist accesses and checks from loop
				Dim i, hi As Integer
				If action Is Nothing Then Throw New NullPointerException
				a = array
				hi = fence
				i = index
				index = hi
				If a .length >= hi AndAlso i >= 0 AndAlso i < index Then
					Do
						action.accept(a(i))
						i += 1
					Loop While i < hi
				End If
			End Sub

			Public Overrides Function tryAdvance(ByVal action As java.util.function.DoubleConsumer) As Boolean
				If action Is Nothing Then Throw New NullPointerException
				If index >= 0 AndAlso index < fence Then
					action.accept(array(index))
					index += 1
					Return True
				End If
				Return False
			End Function

			Public Overrides Function estimateSize() As Long
				Return CLng(fence - index)
			End Function
			Public Overrides Function characteristics() As Integer
				Return characteristics_Renamed
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Property Overrides comparator As Comparator(Of ?)
				Get
					If hasCharacteristics(Spliterator.SORTED) Then Return Nothing
					Throw New IllegalStateException
				End Get
			End Property
		End Class

		'

		''' <summary>
		''' An abstract {@code Spliterator} that implements {@code trySplit} to
		''' permit limited parallelism.
		''' 
		''' <p>An extending class need only
		''' implement <seealso cref="#tryAdvance(java.util.function.Consumer) tryAdvance"/>.
		''' The extending class should override
		''' <seealso cref="#forEachRemaining(java.util.function.Consumer) forEach"/> if it can
		''' provide a more performant implementation.
		''' 
		''' @apiNote
		''' This class is a useful aid for creating a spliterator when it is not
		''' possible or difficult to efficiently partition elements in a manner
		''' allowing balanced parallel computation.
		''' 
		''' <p>An alternative to using this class, that also permits limited
		''' parallelism, is to create a spliterator from an iterator
		''' (see <seealso cref="#spliterator(Iterator, long, int)"/>.  Depending on the
		''' circumstances using an iterator may be easier or more convenient than
		''' extending this class, such as when there is already an iterator
		''' available to use.
		''' </summary>
		''' <seealso cref= #spliterator(Iterator, long, int)
		''' @since 1.8 </seealso>
		Public MustInherit Class AbstractSpliterator(Of T)
			Implements Spliterator(Of T)

			Friend Shared ReadOnly BATCH_UNIT As Integer = 1 << 10 ' batch array size increment
			Friend Shared ReadOnly MAX_BATCH As Integer = 1 << 25 ' max batch array size;
			Private ReadOnly characteristics_Renamed As Integer
			Private est As Long ' size estimate
			Private batch As Integer ' batch size for splits

			''' <summary>
			''' Creates a spliterator reporting the given estimated size and
			''' additionalCharacteristics.
			''' </summary>
			''' <param name="est"> the estimated size of this spliterator if known, otherwise
			'''        {@code Long.MAX_VALUE}. </param>
			''' <param name="additionalCharacteristics"> properties of this spliterator's
			'''        source or elements.  If {@code SIZED} is reported then this
			'''        spliterator will additionally report {@code SUBSIZED}. </param>
			Protected Friend Sub New(ByVal est As Long, ByVal additionalCharacteristics As Integer)
				Me.est = est
				Me.characteristics_Renamed = If((additionalCharacteristics And Spliterator.SIZED) <> 0, additionalCharacteristics Or Spliterator.SUBSIZED, additionalCharacteristics)
			End Sub

			Friend NotInheritable Class HoldingConsumer(Of T)
				Implements java.util.function.Consumer(Of T)

				Friend value As Object

				Public Overrides Sub accept(ByVal value As T)
					Me.value = value
				End Sub
			End Class

			''' <summary>
			''' {@inheritDoc}
			''' 
			''' This implementation permits limited parallelism.
			''' </summary>
			Public Overrides Function trySplit() As Spliterator(Of T) Implements Spliterator(Of T).trySplit
	'            
	'             * Split into arrays of arithmetically increasing batch
	'             * sizes.  This will only improve parallel performance if
	'             * per-element Consumer actions are more costly than
	'             * transferring them into an array.  The use of an
	'             * arithmetic progression in split sizes provides overhead
	'             * vs parallelism bounds that do not particularly favor or
	'             * penalize cases of lightweight vs heavyweight element
	'             * operations, across combinations of #elements vs #cores,
	'             * whether or not either are known.  We generate
	'             * O(sqrt(#elements)) splits, allowing O(sqrt(#cores))
	'             * potential speedup.
	'             
				Dim holder As New HoldingConsumer(Of T)
				Dim s As Long = est
				If s > 1 AndAlso tryAdvance(holder) Then
					Dim n As Integer = batch + BATCH_UNIT
					If n > s Then n = CInt(s)
					If n > MAX_BATCH Then n = MAX_BATCH
					Dim a As Object() = New Object(n - 1){}
					Dim j As Integer = 0
					Do
						a(j) = holder.value
						j += 1
					Loop While j < n AndAlso tryAdvance(holder)
					batch = j
					If est <> Long.MaxValue Then est -= j
					Return New ArraySpliterator(Of )(a, 0, j, characteristics())
				End If
				Return Nothing
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' 
			''' @implSpec
			''' This implementation returns the estimated size as reported when
			''' created and, if the estimate size is known, decreases in size when
			''' split.
			''' </summary>
			Public Overrides Function estimateSize() As Long Implements Spliterator(Of T).estimateSize
				Return est
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' 
			''' @implSpec
			''' This implementation returns the characteristics as reported when
			''' created.
			''' </summary>
			Public Overrides Function characteristics() As Integer Implements Spliterator(Of T).characteristics
				Return characteristics_Renamed
			End Function
		End Class

		''' <summary>
		''' An abstract {@code Spliterator.OfInt} that implements {@code trySplit} to
		''' permit limited parallelism.
		''' 
		''' <p>To implement a spliterator an extending class need only
		''' implement <seealso cref="#tryAdvance(java.util.function.IntConsumer)"/>
		''' tryAdvance}.  The extending class should override
		''' <seealso cref="#forEachRemaining(java.util.function.IntConsumer)"/> forEach} if it
		''' can provide a more performant implementation.
		''' 
		''' @apiNote
		''' This class is a useful aid for creating a spliterator when it is not
		''' possible or difficult to efficiently partition elements in a manner
		''' allowing balanced parallel computation.
		''' 
		''' <p>An alternative to using this class, that also permits limited
		''' parallelism, is to create a spliterator from an iterator
		''' (see <seealso cref="#spliterator(java.util.PrimitiveIterator.OfInt, long, int)"/>.
		''' Depending on the circumstances using an iterator may be easier or more
		''' convenient than extending this class. For example, if there is already an
		''' iterator available to use then there is no need to extend this class.
		''' </summary>
		''' <seealso cref= #spliterator(java.util.PrimitiveIterator.OfInt, long, int)
		''' @since 1.8 </seealso>
		Public MustInherit Class AbstractIntSpliterator
			Implements Spliterator.OfInt

			Friend Shared ReadOnly MAX_BATCH As Integer = AbstractSpliterator.MAX_BATCH
			Friend Shared ReadOnly BATCH_UNIT As Integer = AbstractSpliterator.BATCH_UNIT
			Private ReadOnly characteristics_Renamed As Integer
			Private est As Long ' size estimate
			Private batch As Integer ' batch size for splits

			''' <summary>
			''' Creates a spliterator reporting the given estimated size and
			''' characteristics.
			''' </summary>
			''' <param name="est"> the estimated size of this spliterator if known, otherwise
			'''        {@code Long.MAX_VALUE}. </param>
			''' <param name="additionalCharacteristics"> properties of this spliterator's
			'''        source or elements.  If {@code SIZED} is reported then this
			'''        spliterator will additionally report {@code SUBSIZED}. </param>
			Protected Friend Sub New(ByVal est As Long, ByVal additionalCharacteristics As Integer)
				Me.est = est
				Me.characteristics_Renamed = If((additionalCharacteristics And Spliterator.SIZED) <> 0, additionalCharacteristics Or Spliterator.SUBSIZED, additionalCharacteristics)
			End Sub

			Friend NotInheritable Class HoldingIntConsumer
				Implements java.util.function.IntConsumer

				Friend value As Integer

				Public Overrides Sub accept(ByVal value As Integer)
					Me.value = value
				End Sub
			End Class

			''' <summary>
			''' {@inheritDoc}
			''' 
			''' This implementation permits limited parallelism.
			''' </summary>
			Public Overrides Function trySplit() As Spliterator.OfInt
				Dim holder As New HoldingIntConsumer
				Dim s As Long = est
				If s > 1 AndAlso tryAdvance(holder) Then
					Dim n As Integer = batch + BATCH_UNIT
					If n > s Then n = CInt(s)
					If n > MAX_BATCH Then n = MAX_BATCH
					Dim a As Integer() = New Integer(n - 1){}
					Dim j As Integer = 0
					Do
						a(j) = holder.value
						j += 1
					Loop While j < n AndAlso tryAdvance(holder)
					batch = j
					If est <> Long.MaxValue Then est -= j
					Return New IntArraySpliterator(a, 0, j, characteristics())
				End If
				Return Nothing
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' 
			''' @implSpec
			''' This implementation returns the estimated size as reported when
			''' created and, if the estimate size is known, decreases in size when
			''' split.
			''' </summary>
			Public Overrides Function estimateSize() As Long
				Return est
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' 
			''' @implSpec
			''' This implementation returns the characteristics as reported when
			''' created.
			''' </summary>
			Public Overrides Function characteristics() As Integer
				Return characteristics_Renamed
			End Function
		End Class

		''' <summary>
		''' An abstract {@code Spliterator.OfLong} that implements {@code trySplit}
		''' to permit limited parallelism.
		''' 
		''' <p>To implement a spliterator an extending class need only
		''' implement <seealso cref="#tryAdvance(java.util.function.LongConsumer)"/>
		''' tryAdvance}.  The extending class should override
		''' <seealso cref="#forEachRemaining(java.util.function.LongConsumer)"/> forEach} if it
		''' can provide a more performant implementation.
		''' 
		''' @apiNote
		''' This class is a useful aid for creating a spliterator when it is not
		''' possible or difficult to efficiently partition elements in a manner
		''' allowing balanced parallel computation.
		''' 
		''' <p>An alternative to using this class, that also permits limited
		''' parallelism, is to create a spliterator from an iterator
		''' (see <seealso cref="#spliterator(java.util.PrimitiveIterator.OfLong, long, int)"/>.
		''' Depending on the circumstances using an iterator may be easier or more
		''' convenient than extending this class. For example, if there is already an
		''' iterator available to use then there is no need to extend this class.
		''' </summary>
		''' <seealso cref= #spliterator(java.util.PrimitiveIterator.OfLong, long, int)
		''' @since 1.8 </seealso>
		Public MustInherit Class AbstractLongSpliterator
			Implements Spliterator.OfLong

			Friend Shared ReadOnly MAX_BATCH As Integer = AbstractSpliterator.MAX_BATCH
			Friend Shared ReadOnly BATCH_UNIT As Integer = AbstractSpliterator.BATCH_UNIT
			Private ReadOnly characteristics_Renamed As Integer
			Private est As Long ' size estimate
			Private batch As Integer ' batch size for splits

			''' <summary>
			''' Creates a spliterator reporting the given estimated size and
			''' characteristics.
			''' </summary>
			''' <param name="est"> the estimated size of this spliterator if known, otherwise
			'''        {@code Long.MAX_VALUE}. </param>
			''' <param name="additionalCharacteristics"> properties of this spliterator's
			'''        source or elements.  If {@code SIZED} is reported then this
			'''        spliterator will additionally report {@code SUBSIZED}. </param>
			Protected Friend Sub New(ByVal est As Long, ByVal additionalCharacteristics As Integer)
				Me.est = est
				Me.characteristics_Renamed = If((additionalCharacteristics And Spliterator.SIZED) <> 0, additionalCharacteristics Or Spliterator.SUBSIZED, additionalCharacteristics)
			End Sub

			Friend NotInheritable Class HoldingLongConsumer
				Implements java.util.function.LongConsumer

				Friend value As Long

				Public Overrides Sub accept(ByVal value As Long)
					Me.value = value
				End Sub
			End Class

			''' <summary>
			''' {@inheritDoc}
			''' 
			''' This implementation permits limited parallelism.
			''' </summary>
			Public Overrides Function trySplit() As Spliterator.OfLong
				Dim holder As New HoldingLongConsumer
				Dim s As Long = est
				If s > 1 AndAlso tryAdvance(holder) Then
					Dim n As Integer = batch + BATCH_UNIT
					If n > s Then n = CInt(s)
					If n > MAX_BATCH Then n = MAX_BATCH
					Dim a As Long() = New Long(n - 1){}
					Dim j As Integer = 0
					Do
						a(j) = holder.value
						j += 1
					Loop While j < n AndAlso tryAdvance(holder)
					batch = j
					If est <> Long.MaxValue Then est -= j
					Return New LongArraySpliterator(a, 0, j, characteristics())
				End If
				Return Nothing
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' 
			''' @implSpec
			''' This implementation returns the estimated size as reported when
			''' created and, if the estimate size is known, decreases in size when
			''' split.
			''' </summary>
			Public Overrides Function estimateSize() As Long
				Return est
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' 
			''' @implSpec
			''' This implementation returns the characteristics as reported when
			''' created.
			''' </summary>
			Public Overrides Function characteristics() As Integer
				Return characteristics_Renamed
			End Function
		End Class

		''' <summary>
		''' An abstract {@code Spliterator.OfDouble} that implements
		''' {@code trySplit} to permit limited parallelism.
		''' 
		''' <p>To implement a spliterator an extending class need only
		''' implement <seealso cref="#tryAdvance(java.util.function.DoubleConsumer)"/>
		''' tryAdvance}.  The extending class should override
		''' <seealso cref="#forEachRemaining(java.util.function.DoubleConsumer)"/> forEach} if
		''' it can provide a more performant implementation.
		''' 
		''' @apiNote
		''' This class is a useful aid for creating a spliterator when it is not
		''' possible or difficult to efficiently partition elements in a manner
		''' allowing balanced parallel computation.
		''' 
		''' <p>An alternative to using this class, that also permits limited
		''' parallelism, is to create a spliterator from an iterator
		''' (see <seealso cref="#spliterator(java.util.PrimitiveIterator.OfDouble, long, int)"/>.
		''' Depending on the circumstances using an iterator may be easier or more
		''' convenient than extending this class. For example, if there is already an
		''' iterator available to use then there is no need to extend this class.
		''' </summary>
		''' <seealso cref= #spliterator(java.util.PrimitiveIterator.OfDouble, long, int)
		''' @since 1.8 </seealso>
		Public MustInherit Class AbstractDoubleSpliterator
			Implements Spliterator.OfDouble

			Friend Shared ReadOnly MAX_BATCH As Integer = AbstractSpliterator.MAX_BATCH
			Friend Shared ReadOnly BATCH_UNIT As Integer = AbstractSpliterator.BATCH_UNIT
			Private ReadOnly characteristics_Renamed As Integer
			Private est As Long ' size estimate
			Private batch As Integer ' batch size for splits

			''' <summary>
			''' Creates a spliterator reporting the given estimated size and
			''' characteristics.
			''' </summary>
			''' <param name="est"> the estimated size of this spliterator if known, otherwise
			'''        {@code Long.MAX_VALUE}. </param>
			''' <param name="additionalCharacteristics"> properties of this spliterator's
			'''        source or elements.  If {@code SIZED} is reported then this
			'''        spliterator will additionally report {@code SUBSIZED}. </param>
			Protected Friend Sub New(ByVal est As Long, ByVal additionalCharacteristics As Integer)
				Me.est = est
				Me.characteristics_Renamed = If((additionalCharacteristics And Spliterator.SIZED) <> 0, additionalCharacteristics Or Spliterator.SUBSIZED, additionalCharacteristics)
			End Sub

			Friend NotInheritable Class HoldingDoubleConsumer
				Implements java.util.function.DoubleConsumer

				Friend value As Double

				Public Overrides Sub accept(ByVal value As Double)
					Me.value = value
				End Sub
			End Class

			''' <summary>
			''' {@inheritDoc}
			''' 
			''' This implementation permits limited parallelism.
			''' </summary>
			Public Overrides Function trySplit() As Spliterator.OfDouble
				Dim holder As New HoldingDoubleConsumer
				Dim s As Long = est
				If s > 1 AndAlso tryAdvance(holder) Then
					Dim n As Integer = batch + BATCH_UNIT
					If n > s Then n = CInt(s)
					If n > MAX_BATCH Then n = MAX_BATCH
					Dim a As Double() = New Double(n - 1){}
					Dim j As Integer = 0
					Do
						a(j) = holder.value
						j += 1
					Loop While j < n AndAlso tryAdvance(holder)
					batch = j
					If est <> Long.MaxValue Then est -= j
					Return New DoubleArraySpliterator(a, 0, j, characteristics())
				End If
				Return Nothing
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' 
			''' @implSpec
			''' This implementation returns the estimated size as reported when
			''' created and, if the estimate size is known, decreases in size when
			''' split.
			''' </summary>
			Public Overrides Function estimateSize() As Long
				Return est
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' 
			''' @implSpec
			''' This implementation returns the characteristics as reported when
			''' created.
			''' </summary>
			Public Overrides Function characteristics() As Integer
				Return characteristics_Renamed
			End Function
		End Class

		' Iterator-based Spliterators

		''' <summary>
		''' A Spliterator using a given Iterator for element
		''' operations. The spliterator implements {@code trySplit} to
		''' permit limited parallelism.
		''' </summary>
		Friend Class IteratorSpliterator(Of T)
			Implements Spliterator(Of T)

			Friend Shared ReadOnly BATCH_UNIT As Integer = 1 << 10 ' batch array size increment
			Friend Shared ReadOnly MAX_BATCH As Integer = 1 << 25 ' max batch array size;
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private ReadOnly collection As Collection(Of ? As T) ' null OK
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private it As [Iterator](Of ? As T)
			Private ReadOnly characteristics_Renamed As Integer
			Private est As Long ' size estimate
			Private batch As Integer ' batch size for splits

			''' <summary>
			''' Creates a spliterator using the given given
			''' collection's {@link java.util.Collection#iterator()) for traversal,
			''' and reporting its {@link java.util.Collection#size()) as its initial
			''' size.
			''' </summary>
			''' <param name="c"> the collection </param>
			''' <param name="characteristics"> properties of this spliterator's
			'''        source or elements. </param>
			Public Sub New(Of T1 As T)(ByVal collection As Collection(Of T1), ByVal characteristics As Integer)
				Me.collection = collection
				Me.it = Nothing
				Me.characteristics_Renamed = If((characteristics And Spliterator.CONCURRENT) = 0, characteristics Or Spliterator.SIZED Or Spliterator.SUBSIZED, characteristics)
			End Sub

			''' <summary>
			''' Creates a spliterator using the given iterator
			''' for traversal, and reporting the given initial size
			''' and characteristics.
			''' </summary>
			''' <param name="iterator"> the iterator for the source </param>
			''' <param name="size"> the number of elements in the source </param>
			''' <param name="characteristics"> properties of this spliterator's
			''' source or elements. </param>
			Public Sub New(Of T1 As T)(ByVal [iterator] As [Iterator](Of T1), ByVal size As Long, ByVal characteristics As Integer)
				Me.collection = Nothing
				Me.it = [iterator]
				Me.est = size
				Me.characteristics_Renamed = If((characteristics And Spliterator.CONCURRENT) = 0, characteristics Or Spliterator.SIZED Or Spliterator.SUBSIZED, characteristics)
			End Sub

			''' <summary>
			''' Creates a spliterator using the given iterator
			''' for traversal, and reporting the given initial size
			''' and characteristics.
			''' </summary>
			''' <param name="iterator"> the iterator for the source </param>
			''' <param name="characteristics"> properties of this spliterator's
			''' source or elements. </param>
			Public Sub New(Of T1 As T)(ByVal [iterator] As [Iterator](Of T1), ByVal characteristics As Integer)
				Me.collection = Nothing
				Me.it = [iterator]
				Me.est = Long.MaxValue
				Me.characteristics_Renamed = characteristics And Not(Spliterator.SIZED Or Spliterator.SUBSIZED)
			End Sub

			Public Overrides Function trySplit() As Spliterator(Of T) Implements Spliterator(Of T).trySplit
	'            
	'             * Split into arrays of arithmetically increasing batch
	'             * sizes.  This will only improve parallel performance if
	'             * per-element Consumer actions are more costly than
	'             * transferring them into an array.  The use of an
	'             * arithmetic progression in split sizes provides overhead
	'             * vs parallelism bounds that do not particularly favor or
	'             * penalize cases of lightweight vs heavyweight element
	'             * operations, across combinations of #elements vs #cores,
	'             * whether or not either are known.  We generate
	'             * O(sqrt(#elements)) splits, allowing O(sqrt(#cores))
	'             * potential speedup.
	'             
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim i As [Iterator](Of ? As T)
				Dim s As Long
				i = it
				If i Is Nothing Then
						it = collection.GetEnumerator()
						i = it
						est = CLng(collection.size())
						s = est
				Else
					s = est
				End If
				If s > 1 AndAlso i.hasNext() Then
					Dim n As Integer = batch + BATCH_UNIT
					If n > s Then n = CInt(s)
					If n > MAX_BATCH Then n = MAX_BATCH
					Dim a As Object() = New Object(n - 1){}
					Dim j As Integer = 0
					Do
						a(j) = i.next()
						j += 1
					Loop While j < n AndAlso i.hasNext()
					batch = j
					If est <> Long.MaxValue Then est -= j
					Return New ArraySpliterator(Of )(a, 0, j, characteristics_Renamed)
				End If
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of T).forEachRemaining
				If action Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim i As [Iterator](Of ? As T)
				i = it
				If i Is Nothing Then
						it = collection.GetEnumerator()
						i = it
					est = CLng(collection.size())
				End If
				i.forEachRemaining(action)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of T).tryAdvance
				If action Is Nothing Then Throw New NullPointerException
				If it Is Nothing Then
					it = collection.GetEnumerator()
					est = CLng(collection.size())
				End If
				If it.hasNext() Then
					action.accept(it.next())
					Return True
				End If
				Return False
			End Function

			Public Overrides Function estimateSize() As Long Implements Spliterator(Of T).estimateSize
				If it Is Nothing Then
					it = collection.GetEnumerator()
						est = CLng(collection.size())
						Return est
				End If
				Return est
			End Function

			Public Overrides Function characteristics() As Integer Implements Spliterator(Of T).characteristics
				Return characteristics_Renamed
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overrides Function getComparator() As Comparator(Of ?) Implements Spliterator(Of T).getComparator
				If hasCharacteristics(Spliterator.SORTED) Then Return Nothing
				Throw New IllegalStateException
			End Function
		End Class

		''' <summary>
		''' A Spliterator.OfInt using a given IntStream.IntIterator for element
		''' operations. The spliterator implements {@code trySplit} to
		''' permit limited parallelism.
		''' </summary>
		Friend NotInheritable Class IntIteratorSpliterator
			Implements Spliterator.OfInt

			Friend Shared ReadOnly BATCH_UNIT As Integer = IteratorSpliterator.BATCH_UNIT
			Friend Shared ReadOnly MAX_BATCH As Integer = IteratorSpliterator.MAX_BATCH
			Private it As PrimitiveIterator.OfInt
			Private ReadOnly characteristics_Renamed As Integer
			Private est As Long ' size estimate
			Private batch As Integer ' batch size for splits

			''' <summary>
			''' Creates a spliterator using the given iterator
			''' for traversal, and reporting the given initial size
			''' and characteristics.
			''' </summary>
			''' <param name="iterator"> the iterator for the source </param>
			''' <param name="size"> the number of elements in the source </param>
			''' <param name="characteristics"> properties of this spliterator's
			''' source or elements. </param>
			Public Sub New(ByVal [iterator] As PrimitiveIterator.OfInt, ByVal size As Long, ByVal characteristics As Integer)
				Me.it = [iterator]
				Me.est = size
				Me.characteristics_Renamed = If((characteristics And Spliterator.CONCURRENT) = 0, characteristics Or Spliterator.SIZED Or Spliterator.SUBSIZED, characteristics)
			End Sub

			''' <summary>
			''' Creates a spliterator using the given iterator for a
			''' source of unknown size, reporting the given
			''' characteristics.
			''' </summary>
			''' <param name="iterator"> the iterator for the source </param>
			''' <param name="characteristics"> properties of this spliterator's
			''' source or elements. </param>
			Public Sub New(ByVal [iterator] As PrimitiveIterator.OfInt, ByVal characteristics As Integer)
				Me.it = [iterator]
				Me.est = Long.MaxValue
				Me.characteristics_Renamed = characteristics And Not(Spliterator.SIZED Or Spliterator.SUBSIZED)
			End Sub

			Public Overrides Function trySplit() As OfInt
				Dim i As PrimitiveIterator.OfInt = it
				Dim s As Long = est
				If s > 1 AndAlso i.hasNext() Then
					Dim n As Integer = batch + BATCH_UNIT
					If n > s Then n = CInt(s)
					If n > MAX_BATCH Then n = MAX_BATCH
					Dim a As Integer() = New Integer(n - 1){}
					Dim j As Integer = 0
					Do
						a(j) = i.Next()
						j += 1
					Loop While j < n AndAlso i.hasNext()
					batch = j
					If est <> Long.MaxValue Then est -= j
					Return New IntArraySpliterator(a, 0, j, characteristics_Renamed)
				End If
				Return Nothing
			End Function

			Public Overrides Sub forEachRemaining(ByVal action As java.util.function.IntConsumer)
				If action Is Nothing Then Throw New NullPointerException
				it.forEachRemaining(action)
			End Sub

			Public Overrides Function tryAdvance(ByVal action As java.util.function.IntConsumer) As Boolean
				If action Is Nothing Then Throw New NullPointerException
				If it.hasNext() Then
					action.accept(it.Next())
					Return True
				End If
				Return False
			End Function

			Public Overrides Function estimateSize() As Long
				Return est
			End Function

			Public Overrides Function characteristics() As Integer
				Return characteristics_Renamed
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Property Overrides comparator As Comparator(Of ?)
				Get
					If hasCharacteristics(Spliterator.SORTED) Then Return Nothing
					Throw New IllegalStateException
				End Get
			End Property
		End Class

		Friend NotInheritable Class LongIteratorSpliterator
			Implements Spliterator.OfLong

			Friend Shared ReadOnly BATCH_UNIT As Integer = IteratorSpliterator.BATCH_UNIT
			Friend Shared ReadOnly MAX_BATCH As Integer = IteratorSpliterator.MAX_BATCH
			Private it As PrimitiveIterator.OfLong
			Private ReadOnly characteristics_Renamed As Integer
			Private est As Long ' size estimate
			Private batch As Integer ' batch size for splits

			''' <summary>
			''' Creates a spliterator using the given iterator
			''' for traversal, and reporting the given initial size
			''' and characteristics.
			''' </summary>
			''' <param name="iterator"> the iterator for the source </param>
			''' <param name="size"> the number of elements in the source </param>
			''' <param name="characteristics"> properties of this spliterator's
			''' source or elements. </param>
			Public Sub New(ByVal [iterator] As PrimitiveIterator.OfLong, ByVal size As Long, ByVal characteristics As Integer)
				Me.it = [iterator]
				Me.est = size
				Me.characteristics_Renamed = If((characteristics And Spliterator.CONCURRENT) = 0, characteristics Or Spliterator.SIZED Or Spliterator.SUBSIZED, characteristics)
			End Sub

			''' <summary>
			''' Creates a spliterator using the given iterator for a
			''' source of unknown size, reporting the given
			''' characteristics.
			''' </summary>
			''' <param name="iterator"> the iterator for the source </param>
			''' <param name="characteristics"> properties of this spliterator's
			''' source or elements. </param>
			Public Sub New(ByVal [iterator] As PrimitiveIterator.OfLong, ByVal characteristics As Integer)
				Me.it = [iterator]
				Me.est = Long.MaxValue
				Me.characteristics_Renamed = characteristics And Not(Spliterator.SIZED Or Spliterator.SUBSIZED)
			End Sub

			Public Overrides Function trySplit() As OfLong
				Dim i As PrimitiveIterator.OfLong = it
				Dim s As Long = est
				If s > 1 AndAlso i.hasNext() Then
					Dim n As Integer = batch + BATCH_UNIT
					If n > s Then n = CInt(s)
					If n > MAX_BATCH Then n = MAX_BATCH
					Dim a As Long() = New Long(n - 1){}
					Dim j As Integer = 0
					Do
						a(j) = i.nextLong()
						j += 1
					Loop While j < n AndAlso i.hasNext()
					batch = j
					If est <> Long.MaxValue Then est -= j
					Return New LongArraySpliterator(a, 0, j, characteristics_Renamed)
				End If
				Return Nothing
			End Function

			Public Overrides Sub forEachRemaining(ByVal action As java.util.function.LongConsumer)
				If action Is Nothing Then Throw New NullPointerException
				it.forEachRemaining(action)
			End Sub

			Public Overrides Function tryAdvance(ByVal action As java.util.function.LongConsumer) As Boolean
				If action Is Nothing Then Throw New NullPointerException
				If it.hasNext() Then
					action.accept(it.nextLong())
					Return True
				End If
				Return False
			End Function

			Public Overrides Function estimateSize() As Long
				Return est
			End Function

			Public Overrides Function characteristics() As Integer
				Return characteristics_Renamed
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Property Overrides comparator As Comparator(Of ?)
				Get
					If hasCharacteristics(Spliterator.SORTED) Then Return Nothing
					Throw New IllegalStateException
				End Get
			End Property
		End Class

		Friend NotInheritable Class DoubleIteratorSpliterator
			Implements Spliterator.OfDouble

			Friend Shared ReadOnly BATCH_UNIT As Integer = IteratorSpliterator.BATCH_UNIT
			Friend Shared ReadOnly MAX_BATCH As Integer = IteratorSpliterator.MAX_BATCH
			Private it As PrimitiveIterator.OfDouble
			Private ReadOnly characteristics_Renamed As Integer
			Private est As Long ' size estimate
			Private batch As Integer ' batch size for splits

			''' <summary>
			''' Creates a spliterator using the given iterator
			''' for traversal, and reporting the given initial size
			''' and characteristics.
			''' </summary>
			''' <param name="iterator"> the iterator for the source </param>
			''' <param name="size"> the number of elements in the source </param>
			''' <param name="characteristics"> properties of this spliterator's
			''' source or elements. </param>
			Public Sub New(ByVal [iterator] As PrimitiveIterator.OfDouble, ByVal size As Long, ByVal characteristics As Integer)
				Me.it = [iterator]
				Me.est = size
				Me.characteristics_Renamed = If((characteristics And Spliterator.CONCURRENT) = 0, characteristics Or Spliterator.SIZED Or Spliterator.SUBSIZED, characteristics)
			End Sub

			''' <summary>
			''' Creates a spliterator using the given iterator for a
			''' source of unknown size, reporting the given
			''' characteristics.
			''' </summary>
			''' <param name="iterator"> the iterator for the source </param>
			''' <param name="characteristics"> properties of this spliterator's
			''' source or elements. </param>
			Public Sub New(ByVal [iterator] As PrimitiveIterator.OfDouble, ByVal characteristics As Integer)
				Me.it = [iterator]
				Me.est = Long.MaxValue
				Me.characteristics_Renamed = characteristics And Not(Spliterator.SIZED Or Spliterator.SUBSIZED)
			End Sub

			Public Overrides Function trySplit() As OfDouble
				Dim i As PrimitiveIterator.OfDouble = it
				Dim s As Long = est
				If s > 1 AndAlso i.hasNext() Then
					Dim n As Integer = batch + BATCH_UNIT
					If n > s Then n = CInt(s)
					If n > MAX_BATCH Then n = MAX_BATCH
					Dim a As Double() = New Double(n - 1){}
					Dim j As Integer = 0
					Do
						a(j) = i.NextDouble()
						j += 1
					Loop While j < n AndAlso i.hasNext()
					batch = j
					If est <> Long.MaxValue Then est -= j
					Return New DoubleArraySpliterator(a, 0, j, characteristics_Renamed)
				End If
				Return Nothing
			End Function

			Public Overrides Sub forEachRemaining(ByVal action As java.util.function.DoubleConsumer)
				If action Is Nothing Then Throw New NullPointerException
				it.forEachRemaining(action)
			End Sub

			Public Overrides Function tryAdvance(ByVal action As java.util.function.DoubleConsumer) As Boolean
				If action Is Nothing Then Throw New NullPointerException
				If it.hasNext() Then
					action.accept(it.NextDouble())
					Return True
				End If
				Return False
			End Function

			Public Overrides Function estimateSize() As Long
				Return est
			End Function

			Public Overrides Function characteristics() As Integer
				Return characteristics_Renamed
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Property Overrides comparator As Comparator(Of ?)
				Get
					If hasCharacteristics(Spliterator.SORTED) Then Return Nothing
					Throw New IllegalStateException
				End Get
			End Property
		End Class
	End Class

End Namespace
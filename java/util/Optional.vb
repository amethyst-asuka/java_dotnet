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
Namespace java.util


	''' <summary>
	''' A container object which may or may not contain a non-null value.
	''' If a value is present, {@code isPresent()} will return {@code true} and
	''' {@code get()} will return the value.
	''' 
	''' <p>Additional methods that depend on the presence or absence of a contained
	''' value are provided, such as <seealso cref="#orElse(java.lang.Object) orElse()"/>
	''' (return a default value if value not present) and
	''' <seealso cref="#ifPresent(java.util.function.Consumer) ifPresent()"/> (execute a block
	''' of code if the value is present).
	''' 
	''' <p>This is a <a href="../lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code Optional} may have unpredictable results and should be avoided.
	''' 
	''' @since 1.8
	''' </summary>
	Public NotInheritable Class [Optional](Of T)
		''' <summary>
		''' Common instance for {@code empty()}.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared ReadOnly EMPTY_Renamed As New [Optional](Of ?)()

		''' <summary>
		''' If non-null, the value; if null, indicates no value is present
		''' </summary>
		Private ReadOnly value As T

		''' <summary>
		''' Constructs an empty instance.
		''' 
		''' @implNote Generally only one empty instance, <seealso cref="Optional#EMPTY"/>,
		''' should exist per VM.
		''' </summary>
		Private Sub New()
			Me.value = Nothing
		End Sub

		''' <summary>
		''' Returns an empty {@code Optional} instance.  No value is present for this
		''' Optional.
		''' 
		''' @apiNote Though it may be tempting to do so, avoid testing if an object
		''' is empty by comparing with {@code ==} against instances returned by
		''' {@code Option.empty()}. There is no guarantee that it is a singleton.
		''' Instead, use <seealso cref="#isPresent()"/>.
		''' </summary>
		''' @param <T> Type of the non-existent value </param>
		''' <returns> an empty {@code Optional} </returns>
		Public Shared Function empty(Of T)() As [Optional](Of T)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim t As [Optional](Of T) = CType(EMPTY_Renamed, Optional(Of T))
			Return t
		End Function

		''' <summary>
		''' Constructs an instance with the value present.
		''' </summary>
		''' <param name="value"> the non-null value to be present </param>
		''' <exception cref="NullPointerException"> if value is null </exception>
		Private Sub New(ByVal value As T)
			Me.value = Objects.requireNonNull(value)
		End Sub

		''' <summary>
		''' Returns an {@code Optional} with the specified present non-null value.
		''' </summary>
		''' @param <T> the class of the value </param>
		''' <param name="value"> the value to be present, which must be non-null </param>
		''' <returns> an {@code Optional} with the value present </returns>
		''' <exception cref="NullPointerException"> if value is null </exception>
		Public Shared Function [of](Of T)(ByVal value As T) As [Optional](Of T)
			Return New OptionalCType(value, [Of])
		End Function

		''' <summary>
		''' Returns an {@code Optional} describing the specified value, if non-null,
		''' otherwise returns an empty {@code Optional}.
		''' </summary>
		''' @param <T> the class of the value </param>
		''' <param name="value"> the possibly-null value to describe </param>
		''' <returns> an {@code Optional} with a present value if the specified value
		''' is non-null, otherwise an empty {@code Optional} </returns>
		Public Shared Function ofNullable(Of T)(ByVal value As T) As [Optional](Of T)
			Return If(value Is Nothing, empty(), [of](value))
		End Function

		''' <summary>
		''' If a value is present in this {@code Optional}, returns the value,
		''' otherwise throws {@code NoSuchElementException}.
		''' </summary>
		''' <returns> the non-null value held by this {@code Optional} </returns>
		''' <exception cref="NoSuchElementException"> if there is no value present
		''' </exception>
		''' <seealso cref= Optional#isPresent() </seealso>
		Public Function [get]() As T
			If value Is Nothing Then Throw New NoSuchElementException("No value present")
			Return value
		End Function

		''' <summary>
		''' Return {@code true} if there is a value present, otherwise {@code false}.
		''' </summary>
		''' <returns> {@code true} if there is a value present, otherwise {@code false} </returns>
		Public Property present As Boolean
			Get
				Return value IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' If a value is present, invoke the specified consumer with the value,
		''' otherwise do nothing.
		''' </summary>
		''' <param name="consumer"> block to be executed if a value is present </param>
		''' <exception cref="NullPointerException"> if value is present and {@code consumer} is
		''' null </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub ifPresent(Of T1)(ByVal consumer As java.util.function.Consumer(Of T1))
			If value IsNot Nothing Then consumer.accept(value)
		End Sub

		''' <summary>
		''' If a value is present, and the value matches the given predicate,
		''' return an {@code Optional} describing the value, otherwise return an
		''' empty {@code Optional}.
		''' </summary>
		''' <param name="predicate"> a predicate to apply to the value, if present </param>
		''' <returns> an {@code Optional} describing the value of this {@code Optional}
		''' if a value is present and the value matches the given predicate,
		''' otherwise an empty {@code Optional} </returns>
		''' <exception cref="NullPointerException"> if the predicate is null </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Function filter(Of T1)(ByVal predicate As java.util.function.Predicate(Of T1)) As [Optional](Of T)
			Objects.requireNonNull(predicate)
			If Not present Then
				Return Me
			Else
				Return If(predicate.test(value), Me, empty())
			End If
		End Function

		''' <summary>
		''' If a value is present, apply the provided mapping function to it,
		''' and if the result is non-null, return an {@code Optional} describing the
		''' result.  Otherwise return an empty {@code Optional}.
		''' 
		''' @apiNote This method supports post-processing on optional values, without
		''' the need to explicitly check for a return status.  For example, the
		''' following code traverses a stream of file names, selects one that has
		''' not yet been processed, and then opens that file, returning an
		''' {@code Optional<FileInputStream>}:
		''' 
		''' <pre>{@code
		'''     Optional<FileInputStream> fis =
		'''         names.stream().filter(name -> !isProcessedYet(name))
		'''                       .findFirst()
		'''                       .map(name -> new FileInputStream(name));
		''' }</pre>
		''' 
		''' Here, {@code findFirst} returns an {@code Optional<String>}, and then
		''' {@code map} returns an {@code Optional<FileInputStream>} for the desired
		''' file if one exists.
		''' </summary>
		''' @param <U> The type of the result of the mapping function </param>
		''' <param name="mapper"> a mapping function to apply to the value, if present </param>
		''' <returns> an {@code Optional} describing the result of applying a mapping
		''' function to the value of this {@code Optional}, if a value is present,
		''' otherwise an empty {@code Optional} </returns>
		''' <exception cref="NullPointerException"> if the mapping function is null </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Function map(Of U, T1 As U)(ByVal mapper As java.util.function.Function(Of T1)) As [Optional](Of U)
			Objects.requireNonNull(mapper)
			If Not present Then
				Return empty()
			Else
				Return Optional.ofNullable(mapper.apply(value))
			End If
		End Function

		''' <summary>
		''' If a value is present, apply the provided {@code Optional}-bearing
		''' mapping function to it, return that result, otherwise return an empty
		''' {@code Optional}.  This method is similar to <seealso cref="#map(Function)"/>,
		''' but the provided mapper is one whose result is already an {@code Optional},
		''' and if invoked, {@code flatMap} does not wrap it with an additional
		''' {@code Optional}.
		''' </summary>
		''' @param <U> The type parameter to the {@code Optional} returned by </param>
		''' <param name="mapper"> a mapping function to apply to the value, if present
		'''           the mapping function </param>
		''' <returns> the result of applying an {@code Optional}-bearing mapping
		''' function to the value of this {@code Optional}, if a value is present,
		''' otherwise an empty {@code Optional} </returns>
		''' <exception cref="NullPointerException"> if the mapping function is null or returns
		''' a null result </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Function flatMap(Of U, T1)(ByVal mapper As java.util.function.Function(Of T1)) As [Optional](Of U)
			Objects.requireNonNull(mapper)
			If Not present Then
				Return empty()
			Else
				Return Objects.requireNonNull(mapper.apply(value))
			End If
		End Function

		''' <summary>
		''' Return the value if present, otherwise return {@code other}.
		''' </summary>
		''' <param name="other"> the value to be returned if there is no value present, may
		''' be null </param>
		''' <returns> the value, if present, otherwise {@code other} </returns>
		Public Function [orElse](ByVal other As T) As T
			Return If(value IsNot Nothing, value, other)
		End Function

		''' <summary>
		''' Return the value if present, otherwise invoke {@code other} and return
		''' the result of that invocation.
		''' </summary>
		''' <param name="other"> a {@code Supplier} whose result is returned if no value
		''' is present </param>
		''' <returns> the value if present otherwise the result of {@code other.get()} </returns>
		''' <exception cref="NullPointerException"> if value is not present and {@code other} is
		''' null </exception>
		Public Function orElseGet(Of T1 As T)(ByVal other As java.util.function.Supplier(Of T1)) As T
			Return If(value IsNot Nothing, value, other.get())
		End Function

		''' <summary>
		''' Return the contained value, if present, otherwise throw an exception
		''' to be created by the provided supplier.
		''' 
		''' @apiNote A method reference to the exception constructor with an empty
		''' argument list can be used as the supplier. For example,
		''' {@code IllegalStateException::new}
		''' </summary>
		''' @param <X> Type of the exception to be thrown </param>
		''' <param name="exceptionSupplier"> The supplier which will return the exception to
		''' be thrown </param>
		''' <returns> the present value </returns>
		''' <exception cref="X"> if there is no value present </exception>
		''' <exception cref="NullPointerException"> if no value is present and
		''' {@code exceptionSupplier} is null </exception>
		Public Function orElseThrow(Of X As Throwable, T1 As X)(ByVal exceptionSupplier As java.util.function.Supplier(Of T1)) As T
			If value IsNot Nothing Then
				Return value
			Else
				Throw exceptionSupplier.get()
			End If
		End Function

		''' <summary>
		''' Indicates whether some other object is "equal to" this Optional. The
		''' other object is considered equal if:
		''' <ul>
		''' <li>it is also an {@code Optional} and;
		''' <li>both instances have no value present or;
		''' <li>the present values are "equal to" each other via {@code equals()}.
		''' </ul>
		''' </summary>
		''' <param name="obj"> an object to be tested for equality </param>
		''' <returns> {code true} if the other object is "equal to" this object
		''' otherwise {@code false} </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True

			If Not(TypeOf obj Is Optional) Then Return False

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim other As [Optional](Of ?) = CType(obj, Optional(Of ?))
			Return Objects.Equals(value, other.value)
		End Function

		''' <summary>
		''' Returns the hash code value of the present value, if any, or 0 (zero) if
		''' no value is present.
		''' </summary>
		''' <returns> hash code value of the present value or 0 if no value is present </returns>
		Public Overrides Function GetHashCode() As Integer
			Return Objects.hashCode(value)
		End Function

		''' <summary>
		''' Returns a non-empty string representation of this Optional suitable for
		''' debugging. The exact presentation format is unspecified and may vary
		''' between implementations and versions.
		''' 
		''' @implSpec If a value is present the result must include its string
		''' representation in the result. Empty and present Optionals must be
		''' unambiguously differentiable.
		''' </summary>
		''' <returns> the string representation of this instance </returns>
		Public Overrides Function ToString() As String
			Return If(value IsNot Nothing, String.Format("Optional[{0}]", value), "Optional.empty")
		End Function
	End Class

End Namespace
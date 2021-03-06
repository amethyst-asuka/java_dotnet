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
	''' A container object which may or may not contain a {@code double} value.
	''' If a value is present, {@code isPresent()} will return {@code true} and
	''' {@code getAsDouble()} will return the value.
	''' 
	''' <p>Additional methods that depend on the presence or absence of a contained
	''' value are provided, such as <seealso cref="#orElse(double) orElse()"/>
	''' (return a default value if value not present) and
	''' <seealso cref="#ifPresent(java.util.function.DoubleConsumer) ifPresent()"/> (execute a block
	''' of code if the value is present).
	''' 
	''' <p>This is a <a href="../lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code OptionalDouble} may have unpredictable results and should be avoided.
	''' 
	''' @since 1.8
	''' </summary>
	Public NotInheritable Class OptionalDouble
		''' <summary>
		''' Common instance for {@code empty()}.
		''' </summary>
		Private Shared ReadOnly EMPTY_Renamed As New OptionalDouble

		''' <summary>
		''' If true then the value is present, otherwise indicates no value is present
		''' </summary>
		Private ReadOnly isPresent_Renamed As Boolean
		Private ReadOnly value As Double

		''' <summary>
		''' Construct an empty instance.
		''' 
		''' @implNote generally only one empty instance, <seealso cref="OptionalDouble#EMPTY"/>,
		''' should exist per VM.
		''' </summary>
		Private Sub New()
			Me.isPresent_Renamed = False
			Me.value = java.lang.[Double].NaN
		End Sub

		''' <summary>
		''' Returns an empty {@code OptionalDouble} instance.  No value is present for this
		''' OptionalDouble.
		''' 
		''' @apiNote Though it may be tempting to do so, avoid testing if an object
		''' is empty by comparing with {@code ==} against instances returned by
		''' {@code Option.empty()}. There is no guarantee that it is a singleton.
		''' Instead, use <seealso cref="#isPresent()"/>.
		''' </summary>
		'''  <returns> an empty {@code OptionalDouble}. </returns>
		Public Shared Function empty() As OptionalDouble
			Return EMPTY_Renamed
		End Function

		''' <summary>
		''' Construct an instance with the value present.
		''' </summary>
		''' <param name="value"> the double value to be present. </param>
		Private Sub New(  value As Double)
			Me.isPresent_Renamed = True
			Me.value = value
		End Sub

		''' <summary>
		''' Return an {@code OptionalDouble} with the specified value present.
		''' </summary>
		''' <param name="value"> the value to be present </param>
		''' <returns> an {@code OptionalDouble} with the value present </returns>
		Public Shared Function [of](  value As Double) As OptionalDouble
			Return New OptionalDouble(value)
		End Function

		''' <summary>
		''' If a value is present in this {@code OptionalDouble}, returns the value,
		''' otherwise throws {@code NoSuchElementException}.
		''' </summary>
		''' <returns> the value held by this {@code OptionalDouble} </returns>
		''' <exception cref="NoSuchElementException"> if there is no value present
		''' </exception>
		''' <seealso cref= OptionalDouble#isPresent() </seealso>
		Public Property asDouble As Double
			Get
				If Not isPresent_Renamed Then Throw New NoSuchElementException("No value present")
				Return value
			End Get
		End Property

		''' <summary>
		''' Return {@code true} if there is a value present, otherwise {@code false}.
		''' </summary>
		''' <returns> {@code true} if there is a value present, otherwise {@code false} </returns>
		Public Property present As Boolean
			Get
				Return isPresent_Renamed
			End Get
		End Property

		''' <summary>
		''' Have the specified consumer accept the value if a value is present,
		''' otherwise do nothing.
		''' </summary>
		''' <param name="consumer"> block to be executed if a value is present </param>
		''' <exception cref="NullPointerException"> if value is present and {@code consumer} is
		''' null </exception>
		Public Sub ifPresent(  consumer As java.util.function.DoubleConsumer)
			If isPresent_Renamed Then consumer.accept(value)
		End Sub

		''' <summary>
		''' Return the value if present, otherwise return {@code other}.
		''' </summary>
		''' <param name="other"> the value to be returned if there is no value present </param>
		''' <returns> the value, if present, otherwise {@code other} </returns>
		Public Function [orElse](  other As Double) As Double
			Return If(isPresent_Renamed, value, other)
		End Function

		''' <summary>
		''' Return the value if present, otherwise invoke {@code other} and return
		''' the result of that invocation.
		''' </summary>
		''' <param name="other"> a {@code DoubleSupplier} whose result is returned if no value
		''' is present </param>
		''' <returns> the value if present otherwise the result of {@code other.getAsDouble()} </returns>
		''' <exception cref="NullPointerException"> if value is not present and {@code other} is
		''' null </exception>
		Public Function orElseGet(  other As java.util.function.DoubleSupplier) As Double
			Return If(isPresent_Renamed, value, other.asDouble)
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
		Public Function orElseThrow(Of X As Throwable)(  exceptionSupplier As java.util.function.Supplier(Of X)) As Double
			If isPresent_Renamed Then
				Return value
			Else
				Throw exceptionSupplier.get()
			End If
		End Function

		''' <summary>
		''' Indicates whether some other object is "equal to" this OptionalDouble. The
		''' other object is considered equal if:
		''' <ul>
		''' <li>it is also an {@code OptionalDouble} and;
		''' <li>both instances have no value present or;
		''' <li>the present values are "equal to" each other via {@code java.lang.[Double].compare() == 0}.
		''' </ul>
		''' </summary>
		''' <param name="obj"> an object to be tested for equality </param>
		''' <returns> {code true} if the other object is "equal to" this object
		''' otherwise {@code false} </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Me Is obj Then Return True

			If Not(TypeOf obj Is OptionalDouble) Then Return False

			Dim other As OptionalDouble = CType(obj, OptionalDouble)
			Return If(isPresent_Renamed AndAlso other.isPresent_Renamed, java.lang.[Double].Compare(value, other.value) = 0, isPresent_Renamed = other.isPresent_Renamed)
		End Function

		''' <summary>
		''' Returns the hash code value of the present value, if any, or 0 (zero) if
		''' no value is present.
		''' </summary>
		''' <returns> hash code value of the present value or 0 if no value is present </returns>
		Public Overrides Function GetHashCode() As Integer
			Return If(isPresent_Renamed, java.lang.[Double].hashCode(value), 0)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' Returns a non-empty string representation of this object suitable for
		''' debugging. The exact presentation format is unspecified and may vary
		''' between implementations and versions.
		''' 
		''' @implSpec If a value is present the result must include its string
		''' representation in the result. Empty and present instances must be
		''' unambiguously differentiable.
		''' </summary>
		''' <returns> the string representation of this instance </returns>
		Public Overrides Function ToString() As String
			Return If(isPresent_Renamed, String.Format("OptionalDouble[{0}]", value), "OptionalDouble.empty")
		End Function
	End Class

End Namespace
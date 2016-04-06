Imports System

'
' * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class consists of {@code static} utility methods for operating
	''' on objects.  These utilities include {@code null}-safe or {@code
	''' null}-tolerant methods for computing the hash code of an object,
	''' returning a string for an object, and comparing two objects.
	''' 
	''' @since 1.7
	''' </summary>
	Public NotInheritable Class Objects
		Private Sub New()
			Throw New AssertionError("No java.util.Objects instances for you!")
		End Sub

		''' <summary>
		''' Returns {@code true} if the arguments are equal to each other
		''' and {@code false} otherwise.
		''' Consequently, if both arguments are {@code null}, {@code true}
		''' is returned and if exactly one argument is {@code null}, {@code
		''' false} is returned.  Otherwise, equality is determined by using
		''' the <seealso cref="Object#equals equals"/> method of the first
		''' argument.
		''' </summary>
		''' <param name="a"> an object </param>
		''' <param name="b"> an object to be compared with {@code a} for equality </param>
		''' <returns> {@code true} if the arguments are equal to each other
		''' and {@code false} otherwise </returns>
		''' <seealso cref= Object#equals(Object) </seealso>
		Public Shared Function Equals(  a As Object,   b As Object) As Boolean
			Return (a Is b) OrElse (a IsNot Nothing AndAlso a.Equals(b))
		End Function

	   ''' <summary>
	   ''' Returns {@code true} if the arguments are deeply equal to each other
	   ''' and {@code false} otherwise.
	   ''' 
	   ''' Two {@code null} values are deeply equal.  If both arguments are
	   ''' arrays, the algorithm in {@link Arrays#deepEquals(Object[],
	   ''' Object[]) Arrays.deepEquals} is used to determine equality.
	   ''' Otherwise, equality is determined by using the {@link
	   ''' Object#equals equals} method of the first argument.
	   ''' </summary>
	   ''' <param name="a"> an object </param>
	   ''' <param name="b"> an object to be compared with {@code a} for deep equality </param>
	   ''' <returns> {@code true} if the arguments are deeply equal to each other
	   ''' and {@code false} otherwise </returns>
	   ''' <seealso cref= Arrays#deepEquals(Object[], Object[]) </seealso>
	   ''' <seealso cref= Objects#equals(Object, Object) </seealso>
		Public Shared Function deepEquals(  a As Object,   b As Object) As Boolean
			If a Is b Then
				Return True
			ElseIf a Is Nothing OrElse b Is Nothing Then
				Return False
			Else
				Return Arrays.deepEquals0(a, b)
			End If
		End Function

		''' <summary>
		''' Returns the hash code of a non-{@code null} argument and 0 for
		''' a {@code null} argument.
		''' </summary>
		''' <param name="o"> an object </param>
		''' <returns> the hash code of a non-{@code null} argument and 0 for
		''' a {@code null} argument </returns>
		''' <seealso cref= Object#hashCode </seealso>
		Public Shared Function GetHashCode(  o As Object) As Integer
			Return If(o IsNot Nothing, o.GetHashCode(), 0)
		End Function

	   ''' <summary>
	   ''' Generates a hash code for a sequence of input values. The hash
	   ''' code is generated as if all the input values were placed into an
	   ''' array, and that array were hashed by calling {@link
	   ''' Arrays#hashCode(Object[])}.
	   ''' 
	   ''' <p>This method is useful for implementing {@link
	   ''' Object#hashCode()} on objects containing multiple fields. For
	   ''' example, if an object that has three fields, {@code x}, {@code
	   ''' y}, and {@code z}, one could write:
	   ''' 
	   ''' <blockquote><pre>
	   ''' &#064;Override public int hashCode() {
	   '''     return Objects.hash(x, y, z);
	   ''' }
	   ''' </pre></blockquote>
	   ''' 
	   ''' <b>Warning: When a single object reference is supplied, the returned
	   ''' value does not equal the hash code of that object reference.</b> This
	   ''' value can be computed by calling <seealso cref="#hashCode(Object)"/>.
	   ''' </summary>
	   ''' <param name="values"> the values to be hashed </param>
	   ''' <returns> a hash value of the sequence of input values </returns>
	   ''' <seealso cref= Arrays#hashCode(Object[]) </seealso>
	   ''' <seealso cref= List#hashCode </seealso>
		Public Shared Function hash(ParamArray   values As Object()) As Integer
			Return Arrays.hashCode(values)
		End Function

		''' <summary>
		''' Returns the result of calling {@code toString} for a non-{@code
		''' null} argument and {@code "null"} for a {@code null} argument.
		''' </summary>
		''' <param name="o"> an object </param>
		''' <returns> the result of calling {@code toString} for a non-{@code
		''' null} argument and {@code "null"} for a {@code null} argument </returns>
		''' <seealso cref= Object#toString </seealso>
		''' <seealso cref= String#valueOf(Object) </seealso>
		Public Shared Function ToString(  o As Object) As String
			Return Convert.ToString(o)
		End Function

		''' <summary>
		''' Returns the result of calling {@code toString} on the first
		''' argument if the first argument is not {@code null} and returns
		''' the second argument otherwise.
		''' </summary>
		''' <param name="o"> an object </param>
		''' <param name="nullDefault"> string to return if the first argument is
		'''        {@code null} </param>
		''' <returns> the result of calling {@code toString} on the first
		''' argument if it is not {@code null} and the second argument
		''' otherwise. </returns>
		''' <seealso cref= Objects#toString(Object) </seealso>
		Public Shared Function ToString(  o As Object,   nullDefault As String) As String
			Return If(o IsNot Nothing, o.ToString(), nullDefault)
		End Function

		''' <summary>
		''' Returns 0 if the arguments are identical and {@code
		''' c.compare(a, b)} otherwise.
		''' Consequently, if both arguments are {@code null} 0
		''' is returned.
		''' 
		''' <p>Note that if one of the arguments is {@code null}, a {@code
		''' NullPointerException} may or may not be thrown depending on
		''' what ordering policy, if any, the <seealso cref="Comparator Comparator"/>
		''' chooses to have for {@code null} values.
		''' </summary>
		''' @param <T> the type of the objects being compared </param>
		''' <param name="a"> an object </param>
		''' <param name="b"> an object to be compared with {@code a} </param>
		''' <param name="c"> the {@code Comparator} to compare the first two arguments </param>
		''' <returns> 0 if the arguments are identical and {@code
		''' c.compare(a, b)} otherwise. </returns>
		''' <seealso cref= Comparable </seealso>
		''' <seealso cref= Comparator </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Function compare(Of T, T1)(  a As T,   b As T,   c As Comparator(Of T1)) As Integer
			Return If(a Is b, 0, c.Compare(a, b))
		End Function

		''' <summary>
		''' Checks that the specified object reference is not {@code null}. This
		''' method is designed primarily for doing parameter validation in methods
		''' and constructors, as demonstrated below:
		''' <blockquote><pre>
		''' public Foo(Bar bar) {
		'''     this.bar = Objects.requireNonNull(bar);
		''' }
		''' </pre></blockquote>
		''' </summary>
		''' <param name="obj"> the object reference to check for nullity </param>
		''' @param <T> the type of the reference </param>
		''' <returns> {@code obj} if not {@code null} </returns>
		''' <exception cref="NullPointerException"> if {@code obj} is {@code null} </exception>
		Public Shared Function requireNonNull(Of T)(  obj As T) As T
			If obj Is Nothing Then Throw New NullPointerException
			Return obj
		End Function

		''' <summary>
		''' Checks that the specified object reference is not {@code null} and
		''' throws a customized <seealso cref="NullPointerException"/> if it is. This method
		''' is designed primarily for doing parameter validation in methods and
		''' constructors with multiple parameters, as demonstrated below:
		''' <blockquote><pre>
		''' public Foo(Bar bar, Baz baz) {
		'''     this.bar = Objects.requireNonNull(bar, "bar must not be null");
		'''     this.baz = Objects.requireNonNull(baz, "baz must not be null");
		''' }
		''' </pre></blockquote>
		''' </summary>
		''' <param name="obj">     the object reference to check for nullity </param>
		''' <param name="message"> detail message to be used in the event that a {@code
		'''                NullPointerException} is thrown </param>
		''' @param <T> the type of the reference </param>
		''' <returns> {@code obj} if not {@code null} </returns>
		''' <exception cref="NullPointerException"> if {@code obj} is {@code null} </exception>
		Public Shared Function requireNonNull(Of T)(  obj As T,   message As String) As T
			If obj Is Nothing Then Throw New NullPointerException(message)
			Return obj
		End Function

		''' <summary>
		''' Returns {@code true} if the provided reference is {@code null} otherwise
		''' returns {@code false}.
		''' 
		''' @apiNote This method exists to be used as a
		''' <seealso cref="java.util.function.Predicate"/>, {@code filter(Objects::isNull)}
		''' </summary>
		''' <param name="obj"> a reference to be checked against {@code null} </param>
		''' <returns> {@code true} if the provided reference is {@code null} otherwise
		''' {@code false}
		''' </returns>
		''' <seealso cref= java.util.function.Predicate
		''' @since 1.8 </seealso>
		Public Shared Function isNull(  obj As Object) As Boolean
			Return obj Is Nothing
		End Function

		''' <summary>
		''' Returns {@code true} if the provided reference is non-{@code null}
		''' otherwise returns {@code false}.
		''' 
		''' @apiNote This method exists to be used as a
		''' <seealso cref="java.util.function.Predicate"/>, {@code filter(Objects::nonNull)}
		''' </summary>
		''' <param name="obj"> a reference to be checked against {@code null} </param>
		''' <returns> {@code true} if the provided reference is non-{@code null}
		''' otherwise {@code false}
		''' </returns>
		''' <seealso cref= java.util.function.Predicate
		''' @since 1.8 </seealso>
		Public Shared Function nonNull(  obj As Object) As Boolean
			Return obj IsNot Nothing
		End Function

		''' <summary>
		''' Checks that the specified object reference is not {@code null} and
		''' throws a customized <seealso cref="NullPointerException"/> if it is.
		''' 
		''' <p>Unlike the method <seealso cref="#requireNonNull(Object, String)"/>,
		''' this method allows creation of the message to be deferred until
		''' after the null check is made. While this may confer a
		''' performance advantage in the non-null case, when deciding to
		''' call this method care should be taken that the costs of
		''' creating the message supplier are less than the cost of just
		''' creating the string message directly.
		''' </summary>
		''' <param name="obj">     the object reference to check for nullity </param>
		''' <param name="messageSupplier"> supplier of the detail message to be
		''' used in the event that a {@code NullPointerException} is thrown </param>
		''' @param <T> the type of the reference </param>
		''' <returns> {@code obj} if not {@code null} </returns>
		''' <exception cref="NullPointerException"> if {@code obj} is {@code null}
		''' @since 1.8 </exception>
		Public Shared Function requireNonNull(Of T)(  obj As T,   messageSupplier As java.util.function.Supplier(Of String)) As T
			If obj Is Nothing Then Throw New NullPointerException(messageSupplier.get())
			Return obj
		End Function
	End Class

End Namespace
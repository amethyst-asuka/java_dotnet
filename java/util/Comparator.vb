'
' * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' A comparison function, which imposes a <i>total ordering</i> on some
	''' collection of objects.  Comparators can be passed to a sort method (such
	''' as <seealso cref="Collections#sort(List,Comparator) Collections.sort"/> or {@link
	''' Arrays#sort(Object[],Comparator) Arrays.sort}) to allow precise control
	''' over the sort order.  Comparators can also be used to control the order of
	''' certain data structures (such as <seealso cref="SortedSet sorted sets"/> or {@link
	''' SortedMap sorted maps}), or to provide an ordering for collections of
	''' objects that don't have a <seealso cref="Comparable natural ordering"/>.<p>
	''' 
	''' The ordering imposed by a comparator <tt>c</tt> on a set of elements
	''' <tt>S</tt> is said to be <i>consistent with equals</i> if and only if
	''' <tt>c.compare(e1, e2)==0</tt> has the same boolean value as
	''' <tt>e1.equals(e2)</tt> for every <tt>e1</tt> and <tt>e2</tt> in
	''' <tt>S</tt>.<p>
	''' 
	''' Caution should be exercised when using a comparator capable of imposing an
	''' ordering inconsistent with equals to order a sorted set (or sorted map).
	''' Suppose a sorted set (or sorted map) with an explicit comparator <tt>c</tt>
	''' is used with elements (or keys) drawn from a set <tt>S</tt>.  If the
	''' ordering imposed by <tt>c</tt> on <tt>S</tt> is inconsistent with equals,
	''' the sorted set (or sorted map) will behave "strangely."  In particular the
	''' sorted set (or sorted map) will violate the general contract for set (or
	''' map), which is defined in terms of <tt>equals</tt>.<p>
	''' 
	''' For example, suppose one adds two elements {@code a} and {@code b} such that
	''' {@code (a.equals(b) && c.compare(a, b) != 0)}
	''' to an empty {@code TreeSet} with comparator {@code c}.
	''' The second {@code add} operation will return
	''' true (and the size of the tree set will increase) because {@code a} and
	''' {@code b} are not equivalent from the tree set's perspective, even though
	''' this is contrary to the specification of the
	''' <seealso cref="Set#add Set.add"/> method.<p>
	''' 
	''' Note: It is generally a good idea for comparators to also implement
	''' <tt>java.io.Serializable</tt>, as they may be used as ordering methods in
	''' serializable data structures (like <seealso cref="TreeSet"/>, <seealso cref="TreeMap"/>).  In
	''' order for the data structure to serialize successfully, the comparator (if
	''' provided) must implement <tt>Serializable</tt>.<p>
	''' 
	''' For the mathematically inclined, the <i>relation</i> that defines the
	''' <i>imposed ordering</i> that a given comparator <tt>c</tt> imposes on a
	''' given set of objects <tt>S</tt> is:<pre>
	'''       {(x, y) such that c.compare(x, y) &lt;= 0}.
	''' </pre> The <i>quotient</i> for this total order is:<pre>
	'''       {(x, y) such that c.compare(x, y) == 0}.
	''' </pre>
	''' 
	''' It follows immediately from the contract for <tt>compare</tt> that the
	''' quotient is an <i>equivalence relation</i> on <tt>S</tt>, and that the
	''' imposed ordering is a <i>total order</i> on <tt>S</tt>.  When we say that
	''' the ordering imposed by <tt>c</tt> on <tt>S</tt> is <i>consistent with
	''' equals</i>, we mean that the quotient for the ordering is the equivalence
	''' relation defined by the objects' {@link Object#equals(Object)
	''' equals(Object)} method(s):<pre>
	'''     {(x, y) such that x.equals(y)}. </pre>
	''' 
	''' <p>Unlike {@code Comparable}, a comparator may optionally permit
	''' comparison of null arguments, while maintaining the requirements for
	''' an equivalence relation.
	''' 
	''' <p>This interface is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' </summary>
	''' @param <T> the type of objects that may be compared by this comparator
	''' 
	''' @author  Josh Bloch
	''' @author  Neal Gafter </param>
	''' <seealso cref= Comparable </seealso>
	''' <seealso cref= java.io.Serializable
	''' @since 1.2 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface Comparator(Of T)
		''' <summary>
		''' Compares its two arguments for order.  Returns a negative integer,
		''' zero, or a positive integer as the first argument is less than, equal
		''' to, or greater than the second.<p>
		''' 
		''' In the foregoing description, the notation
		''' <tt>sgn(</tt><i>expression</i><tt>)</tt> designates the mathematical
		''' <i>signum</i> function, which is defined to return one of <tt>-1</tt>,
		''' <tt>0</tt>, or <tt>1</tt> according to whether the value of
		''' <i>expression</i> is negative, zero or positive.<p>
		''' 
		''' The implementor must ensure that <tt>sgn(compare(x, y)) ==
		''' -sgn(compare(y, x))</tt> for all <tt>x</tt> and <tt>y</tt>.  (This
		''' implies that <tt>compare(x, y)</tt> must throw an exception if and only
		''' if <tt>compare(y, x)</tt> throws an exception.)<p>
		''' 
		''' The implementor must also ensure that the relation is transitive:
		''' <tt>((compare(x, y)&gt;0) &amp;&amp; (compare(y, z)&gt;0))</tt> implies
		''' <tt>compare(x, z)&gt;0</tt>.<p>
		''' 
		''' Finally, the implementor must ensure that <tt>compare(x, y)==0</tt>
		''' implies that <tt>sgn(compare(x, z))==sgn(compare(y, z))</tt> for all
		''' <tt>z</tt>.<p>
		''' 
		''' It is generally the case, but <i>not</i> strictly required that
		''' <tt>(compare(x, y)==0) == (x.equals(y))</tt>.  Generally speaking,
		''' any comparator that violates this condition should clearly indicate
		''' this fact.  The recommended language is "Note: this comparator
		''' imposes orderings that are inconsistent with equals."
		''' </summary>
		''' <param name="o1"> the first object to be compared. </param>
		''' <param name="o2"> the second object to be compared. </param>
		''' <returns> a negative integer, zero, or a positive integer as the
		'''         first argument is less than, equal to, or greater than the
		'''         second. </returns>
		''' <exception cref="NullPointerException"> if an argument is null and this
		'''         comparator does not permit null arguments </exception>
		''' <exception cref="ClassCastException"> if the arguments' types prevent them from
		'''         being compared by this comparator. </exception>
		Function compare(  o1 As T,   o2 As T) As Integer

		''' <summary>
		''' Indicates whether some other object is &quot;equal to&quot; this
		''' comparator.  This method must obey the general contract of
		''' <seealso cref="Object#equals(Object)"/>.  Additionally, this method can return
		''' <tt>true</tt> <i>only</i> if the specified object is also a comparator
		''' and it imposes the same ordering as this comparator.  Thus,
		''' <code>comp1.equals(comp2)</code> implies that <tt>sgn(comp1.compare(o1,
		''' o2))==sgn(comp2.compare(o1, o2))</tt> for every object reference
		''' <tt>o1</tt> and <tt>o2</tt>.<p>
		''' 
		''' Note that it is <i>always</i> safe <i>not</i> to override
		''' <tt>Object.equals(Object)</tt>.  However, overriding this method may,
		''' in some cases, improve performance by allowing programs to determine
		''' that two distinct comparators impose the same order.
		''' </summary>
		''' <param name="obj">   the reference object with which to compare. </param>
		''' <returns>  <code>true</code> only if the specified object is also
		'''          a comparator and it imposes the same ordering as this
		'''          comparator. </returns>
		''' <seealso cref= Object#equals(Object) </seealso>
		''' <seealso cref= Object#hashCode() </seealso>
		Function Equals(  obj As Object) As Boolean

		''' <summary>
		''' Returns a comparator that imposes the reverse ordering of this
		''' comparator.
		''' </summary>
		''' <returns> a comparator that imposes the reverse ordering of this
		'''         comparator.
		''' @since 1.8 </returns>
		default Function reversed() As Comparator(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return Collections.reverseOrder(Me);

		''' <summary>
		''' Returns a lexicographic-order comparator with another comparator.
		''' If this {@code Comparator} considers two elements equal, i.e.
		''' {@code compare(a, b) == 0}, {@code other} is used to determine the order.
		''' 
		''' <p>The returned comparator is serializable if the specified comparator
		''' is also serializable.
		''' 
		''' @apiNote
		''' For example, to sort a collection of {@code String} based on the length
		''' and then case-insensitive natural ordering, the comparator can be
		''' composed using following code,
		''' 
		''' <pre>{@code
		'''     Comparator<String> cmp = Comparator.comparingInt(String::length)
		'''             .thenComparing(String.CASE_INSENSITIVE_ORDER);
		''' }</pre>
		''' </summary>
		''' <param name="other"> the other comparator to be used when this comparator
		'''         compares two objects that are equal. </param>
		''' <returns> a lexicographic-order comparator composed of this and then the
		'''         other comparator </returns>
		''' <exception cref="NullPointerException"> if the argument is null.
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Function thenComparing(Of T1)(  other As Comparator(Of T1)) As Comparator(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Objects.requireNonNull(other);
			Sub [New](Comparator &   java.io.Serializable As (Of T))
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				int res = compare(c1, c2);
				Sub [New](res !=   0 As )

		''' <summary>
		''' Returns a lexicographic-order comparator with a function that
		''' extracts a key to be compared with the given {@code Comparator}.
		''' 
		''' @implSpec This default implementation behaves as if {@code
		'''           thenComparing(comparing(keyExtractor, cmp))}.
		''' </summary>
		''' @param  <U>  the type of the sort key </param>
		''' <param name="keyExtractor"> the function used to extract the sort key </param>
		''' <param name="keyComparator"> the {@code Comparator} used to compare the sort key </param>
		''' <returns> a lexicographic-order comparator composed of this comparator
		'''         and then comparing on the key extracted by the keyExtractor function </returns>
		''' <exception cref="NullPointerException"> if either argument is null. </exception>
		''' <seealso cref= #comparing(Function, Comparator) </seealso>
		''' <seealso cref= #thenComparing(Comparator)
		''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Function Comparator(  T As [Of]) As default(Of U)
			Function thenComparing(comparing(keyExtractor, keyComparator)    As ) As [Return]

		''' <summary>
		''' Returns a lexicographic-order comparator with a function that
		''' extracts a {@code Comparable} sort key.
		''' 
		''' @implSpec This default implementation behaves as if {@code
		'''           thenComparing(comparing(keyExtractor))}.
		''' </summary>
		''' @param  <U>  the type of the <seealso cref="Comparable"/> sort key </param>
		''' <param name="keyExtractor"> the function used to extract the {@link
		'''         Comparable} sort key </param>
		''' <returns> a lexicographic-order comparator composed of this and then the
		'''         <seealso cref="Comparable"/> sort key. </returns>
		''' <exception cref="NullPointerException"> if the argument is null. </exception>
		''' <seealso cref= #comparing(Function) </seealso>
		''' <seealso cref= #thenComparing(Comparator)
		''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function Comparator(  T As [Of]) As default(Of U As Comparable(Of ?))
			Function thenComparing(comparing(keyExtractor)    As ) As [Return]

		''' <summary>
		''' Returns a lexicographic-order comparator with a function that
		''' extracts a {@code int} sort key.
		''' 
		''' @implSpec This default implementation behaves as if {@code
		'''           thenComparing(comparingInt(keyExtractor))}.
		''' </summary>
		''' <param name="keyExtractor"> the function used to extract the integer sort key </param>
		''' <returns> a lexicographic-order comparator composed of this and then the
		'''         {@code int} sort key </returns>
		''' <exception cref="NullPointerException"> if the argument is null. </exception>
		''' <seealso cref= #comparingInt(ToIntFunction) </seealso>
		''' <seealso cref= #thenComparing(Comparator)
		''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Function thenComparingInt(Of T1)(  keyExtractor As java.util.function.ToIntFunction(Of T1)) As Comparator(Of T)
			Function thenComparing(comparingInt(keyExtractor)    As ) As [Return]

		''' <summary>
		''' Returns a lexicographic-order comparator with a function that
		''' extracts a {@code long} sort key.
		''' 
		''' @implSpec This default implementation behaves as if {@code
		'''           thenComparing(comparingLong(keyExtractor))}.
		''' </summary>
		''' <param name="keyExtractor"> the function used to extract the long sort key </param>
		''' <returns> a lexicographic-order comparator composed of this and then the
		'''         {@code long} sort key </returns>
		''' <exception cref="NullPointerException"> if the argument is null. </exception>
		''' <seealso cref= #comparingLong(ToLongFunction) </seealso>
		''' <seealso cref= #thenComparing(Comparator)
		''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Function thenComparingLong(Of T1)(  keyExtractor As java.util.function.ToLongFunction(Of T1)) As Comparator(Of T)
			Function thenComparing(comparingLong(keyExtractor)    As ) As [Return]

		''' <summary>
		''' Returns a lexicographic-order comparator with a function that
		''' extracts a {@code double} sort key.
		''' 
		''' @implSpec This default implementation behaves as if {@code
		'''           thenComparing(comparingDouble(keyExtractor))}.
		''' </summary>
		''' <param name="keyExtractor"> the function used to extract the double sort key </param>
		''' <returns> a lexicographic-order comparator composed of this and then the
		'''         {@code double} sort key </returns>
		''' <exception cref="NullPointerException"> if the argument is null. </exception>
		''' <seealso cref= #comparingDouble(ToDoubleFunction) </seealso>
		''' <seealso cref= #thenComparing(Comparator)
		''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Function thenComparingDouble(Of T1)(  keyExtractor As java.util.function.ToDoubleFunction(Of T1)) As Comparator(Of T)
			Function thenComparing(comparingDouble(keyExtractor)    As ) As [Return]

		''' <summary>
		''' Returns a comparator that imposes the reverse of the <em>natural
		''' ordering</em>.
		''' 
		''' <p>The returned comparator is serializable and throws {@link
		''' NullPointerException} when comparing {@code null}.
		''' </summary>
		''' @param  <T> the <seealso cref="Comparable"/> type of element to be compared </param>
		''' <returns> a comparator that imposes the reverse of the <i>natural
		'''         ordering</i> on {@code Comparable} objects. </returns>
		''' <seealso cref= Comparable
		''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Shared Function reverseOrder(Of T As Comparable(Of ?))() As Comparator(Of T)
			Function Collections.reverseOrder() As [Return]

		''' <summary>
		''' Returns a comparator that compares <seealso cref="Comparable"/> objects in natural
		''' order.
		''' 
		''' <p>The returned comparator is serializable and throws {@link
		''' NullPointerException} when comparing {@code null}.
		''' </summary>
		''' @param  <T> the <seealso cref="Comparable"/> type of element to be compared </param>
		''' <returns> a comparator that imposes the <i>natural ordering</i> on {@code
		'''         Comparable} objects. </returns>
		''' <seealso cref= Comparable
		''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Shared Function naturalOrder(Of T As Comparable(Of ?))() As Comparator(Of T)
			Sub [New](   As Comparator(Of T))

		''' <summary>
		''' Returns a null-friendly comparator that considers {@code null} to be
		''' less than non-null. When both are {@code null}, they are considered
		''' equal. If both are non-null, the specified {@code Comparator} is used
		''' to determine the order. If the specified comparator is {@code null},
		''' then the returned comparator considers all non-null values to be equal.
		''' 
		''' <p>The returned comparator is serializable if the specified comparator
		''' is serializable.
		''' </summary>
		''' @param  <T> the type of the elements to be compared </param>
		''' <param name="comparator"> a {@code Comparator} for comparing non-null values </param>
		''' <returns> a comparator that considers {@code null} to be less than
		'''         non-null, and compares non-null objects with the supplied
		'''         {@code Comparator}.
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Shared Function nullsFirst(Of T, T1)(  comparator As Comparator(Of T1)) As Comparator(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return New java.util.Comparators.NullComparator<>(True, comparator);

		''' <summary>
		''' Returns a null-friendly comparator that considers {@code null} to be
		''' greater than non-null. When both are {@code null}, they are considered
		''' equal. If both are non-null, the specified {@code Comparator} is used
		''' to determine the order. If the specified comparator is {@code null},
		''' then the returned comparator considers all non-null values to be equal.
		''' 
		''' <p>The returned comparator is serializable if the specified comparator
		''' is serializable.
		''' </summary>
		''' @param  <T> the type of the elements to be compared </param>
		''' <param name="comparator"> a {@code Comparator} for comparing non-null values </param>
		''' <returns> a comparator that considers {@code null} to be greater than
		'''         non-null, and compares non-null objects with the supplied
		'''         {@code Comparator}.
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Shared Function nullsLast(Of T, T1)(  comparator As Comparator(Of T1)) As Comparator(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return New java.util.Comparators.NullComparator<>(False, comparator);

		''' <summary>
		''' Accepts a function that extracts a sort key from a type {@code T}, and
		''' returns a {@code Comparator<T>} that compares by that sort key using
		''' the specified <seealso cref="Comparator"/>.
		'''  
		''' <p>The returned comparator is serializable if the specified function
		''' and comparator are both serializable.
		''' 
		''' @apiNote
		''' For example, to obtain a {@code Comparator} that compares {@code
		''' Person} objects by their last name ignoring case differences,
		''' 
		''' <pre>{@code
		'''     Comparator<Person> cmp = Comparator.comparing(
		'''             Person::getLastName,
		'''             String.CASE_INSENSITIVE_ORDER);
		''' }</pre>
		''' </summary>
		''' @param  <T> the type of element to be compared </param>
		''' @param  <U> the type of the sort key </param>
		''' <param name="keyExtractor"> the function used to extract the sort key </param>
		''' <param name="keyComparator"> the {@code Comparator} used to compare the sort key </param>
		''' <returns> a comparator that compares by an extracted key using the
		'''         specified {@code Comparator} </returns>
		''' <exception cref="NullPointerException"> if either argument is null
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Shared Function comparing(Of T, U, T1 As U, T2)(  keyExtractor As java.util.function.Function(Of T1),   keyComparator As Comparator(Of T2)) As Comparator(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Objects.requireNonNull(keyExtractor);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Objects.requireNonNull(keyComparator);
			Sub [New](Comparator &   java.io.Serializable As (Of T))

		''' <summary>
		''' Accepts a function that extracts a {@link java.lang.Comparable
		''' Comparable} sort key from a type {@code T}, and returns a {@code
		''' Comparator<T>} that compares by that sort key.
		''' 
		''' <p>The returned comparator is serializable if the specified function
		''' is also serializable.
		''' 
		''' @apiNote
		''' For example, to obtain a {@code Comparator} that compares {@code
		''' Person} objects by their last name,
		''' 
		''' <pre>{@code
		'''     Comparator<Person> byLastName = Comparator.comparing(Person::getLastName);
		''' }</pre>
		''' </summary>
		''' @param  <T> the type of element to be compared </param>
		''' @param  <U> the type of the {@code Comparable} sort key </param>
		''' <param name="keyExtractor"> the function used to extract the {@link
		'''         Comparable} sort key </param>
		''' <returns> a comparator that compares by an extracted key </returns>
		''' <exception cref="NullPointerException"> if the argument is null
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Shared Function comparing(Of T, U As Comparable(Of ?), T1 As U)(  keyExtractor As java.util.function.Function(Of T1)) As Comparator(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Objects.requireNonNull(keyExtractor);
			Sub [New](Comparator &   java.io.Serializable As (Of T))

		''' <summary>
		''' Accepts a function that extracts an {@code int} sort key from a type
		''' {@code T}, and returns a {@code Comparator<T>} that compares by that
		''' sort key.
		''' 
		''' <p>The returned comparator is serializable if the specified function
		''' is also serializable.
		''' </summary>
		''' @param  <T> the type of element to be compared </param>
		''' <param name="keyExtractor"> the function used to extract the integer sort key </param>
		''' <returns> a comparator that compares by an extracted key </returns>
		''' <seealso cref= #comparing(Function) </seealso>
		''' <exception cref="NullPointerException"> if the argument is null
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Shared Function comparingInt(Of T, T1)(  keyExtractor As java.util.function.ToIntFunction(Of T1)) As Comparator(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Objects.requireNonNull(keyExtractor);
			Sub [New](Comparator &   java.io.Serializable As (Of T))

		''' <summary>
		''' Accepts a function that extracts a {@code long} sort key from a type
		''' {@code T}, and returns a {@code Comparator<T>} that compares by that
		''' sort key.
		''' 
		''' <p>The returned comparator is serializable if the specified function is
		''' also serializable.
		''' </summary>
		''' @param  <T> the type of element to be compared </param>
		''' <param name="keyExtractor"> the function used to extract the long sort key </param>
		''' <returns> a comparator that compares by an extracted key </returns>
		''' <seealso cref= #comparing(Function) </seealso>
		''' <exception cref="NullPointerException"> if the argument is null
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Shared Function comparingLong(Of T, T1)(  keyExtractor As java.util.function.ToLongFunction(Of T1)) As Comparator(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Objects.requireNonNull(keyExtractor);
			Sub [New](Comparator &   java.io.Serializable As (Of T))

		''' <summary>
		''' Accepts a function that extracts a {@code double} sort key from a type
		''' {@code T}, and returns a {@code Comparator<T>} that compares by that
		''' sort key.
		''' 
		''' <p>The returned comparator is serializable if the specified function
		''' is also serializable.
		''' </summary>
		''' @param  <T> the type of element to be compared </param>
		''' <param name="keyExtractor"> the function used to extract the double sort key </param>
		''' <returns> a comparator that compares by an extracted key </returns>
		''' <seealso cref= #comparing(Function) </seealso>
		''' <exception cref="NullPointerException"> if the argument is null
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Shared Function comparingDouble(Of T, T1)(  keyExtractor As java.util.function.ToDoubleFunction(Of T1)) As Comparator(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Objects.requireNonNull(keyExtractor);
			Sub [New](Comparator &   java.io.Serializable As (Of T))
	End Interface

End Namespace
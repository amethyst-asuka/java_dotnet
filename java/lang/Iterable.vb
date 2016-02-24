Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.lang


	''' <summary>
	''' Implementing this interface allows an object to be the target of
	''' the "for-each loop" statement. See
	''' <strong>
	''' <a href="{@docRoot}/../technotes/guides/language/foreach.html">For-each Loop</a>
	''' </strong>
	''' </summary>
	''' @param <T> the type of elements returned by the iterator
	''' 
	''' @since 1.5
	''' @jls 14.14.2 The enhanced for statement </param>
	Public Interface Iterable(Of T)
		''' <summary>
		''' Returns an iterator over elements of type {@code T}.
		''' </summary>
		''' <returns> an Iterator. </returns>
		Function [iterator]() As IEnumerator(Of T)

		''' <summary>
		''' Performs the given action for each element of the {@code Iterable}
		''' until all elements have been processed or the action throws an
		''' exception.  Unless otherwise specified by the implementing class,
		''' actions are performed in the order of iteration (if an iteration order
		''' is specified).  Exceptions thrown by the action are relayed to the
		''' caller.
		''' 
		''' @implSpec
		''' <p>The default implementation behaves as if:
		''' <pre>{@code
		'''     for (T t : this)
		'''         action.accept(t);
		''' }</pre>
		''' </summary>
		''' <param name="action"> The action to be performed for each element </param>
		''' <exception cref="NullPointerException"> if the specified action is null
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(action);
			Sub [New](T t : ByVal [Me] As )
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				action.accept(t);

		''' <summary>
		''' Creates a <seealso cref="Spliterator"/> over the elements described by this
		''' {@code Iterable}.
		''' 
		''' @implSpec
		''' The default implementation creates an
		''' <em><a href="Spliterator.html#binding">early-binding</a></em>
		''' spliterator from the iterable's {@code Iterator}.  The spliterator
		''' inherits the <em>fail-fast</em> properties of the iterable's iterator.
		''' 
		''' @implNote
		''' The default implementation should usually be overridden.  The
		''' spliterator returned by the default implementation has poor splitting
		''' capabilities, is unsized, and does not report any spliterator
		''' characteristics. Implementing classes can nearly always provide a
		''' better implementation.
		''' </summary>
		''' <returns> a {@code Spliterator} over the elements described by this
		''' {@code Iterable}.
		''' @since 1.8 </returns>
		default Function spliterator() As java.util.Spliterator(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return java.util.Spliterators.spliteratorUnknownSize(iterator(), 0);
	End Interface

End Namespace
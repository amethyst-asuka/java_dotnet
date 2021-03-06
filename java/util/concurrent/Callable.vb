'
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

'
' *
' *
' *
' *
' *
' * Written by Doug Lea with assistance from members of JCP JSR-166
' * Expert Group and released to the public domain, as explained at
' * http://creativecommons.org/publicdomain/zero/1.0/
' 

Namespace java.util.concurrent

	''' <summary>
	''' A task that returns a result and may throw an exception.
	''' Implementors define a single method with no arguments called
	''' {@code call}.
	''' 
	''' <p>The {@code Callable} interface is similar to {@link
	''' java.lang.Runnable}, in that both are designed for classes whose
	''' instances are potentially executed by another thread.  A
	''' {@code Runnable}, however, does not return a result and cannot
	''' throw a checked exception.
	''' 
	''' <p>The <seealso cref="Executors"/> class contains utility methods to
	''' convert from other common forms to {@code Callable} classes.
	''' </summary>
	''' <seealso cref= Executor
	''' @since 1.5
	''' @author Doug Lea </seealso>
	''' @param <V> the result type of method {@code call} </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface Callable(Of V)
		''' <summary>
		''' Computes a result, or throws an exception if unable to do so.
		''' </summary>
		''' <returns> computed result </returns>
		''' <exception cref="Exception"> if unable to compute a result </exception>
		Function [call]() As V
	End Interface

End Namespace
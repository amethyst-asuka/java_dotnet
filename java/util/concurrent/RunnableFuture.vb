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
	''' A <seealso cref="Future"/> that is <seealso cref="Runnable"/>. Successful execution of
	''' the {@code run} method causes completion of the {@code Future}
	''' and allows access to its results. </summary>
	''' <seealso cref= FutureTask </seealso>
	''' <seealso cref= Executor
	''' @since 1.6
	''' @author Doug Lea </seealso>
	''' @param <V> The result type returned by this Future's {@code get} method </param>
	Public Interface RunnableFuture(Of V)
		Inherits Runnable, Future(Of V)

		''' <summary>
		''' Sets this Future to the result of its computation
		''' unless it has been cancelled.
		''' </summary>
		Sub run()
	End Interface

End Namespace
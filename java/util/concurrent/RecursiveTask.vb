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
	''' A recursive result-bearing <seealso cref="ForkJoinTask"/>.
	''' 
	''' <p>For a classic example, here is a task computing Fibonacci numbers:
	''' 
	'''  <pre> {@code
	''' class Fibonacci extends RecursiveTask<Integer> {
	'''   final int n;
	'''   Fibonacci(int n) { this.n = n; }
	'''   Integer compute() {
	'''     if (n <= 1)
	'''       return n;
	'''     Fibonacci f1 = new Fibonacci(n - 1);
	'''     f1.fork();
	'''     Fibonacci f2 = new Fibonacci(n - 2);
	'''     return f2.compute() + f1.join();
	'''   }
	''' }}</pre>
	''' 
	''' However, besides being a dumb way to compute Fibonacci functions
	''' (there is a simple fast linear algorithm that you'd use in
	''' practice), this is likely to perform poorly because the smallest
	''' subtasks are too small to be worthwhile splitting up. Instead, as
	''' is the case for nearly all fork/join applications, you'd pick some
	''' minimum granularity size (for example 10 here) for which you always
	''' sequentially solve rather than subdividing.
	''' 
	''' @since 1.7
	''' @author Doug Lea
	''' </summary>
	Public MustInherit Class RecursiveTask(Of V)
		Inherits ForkJoinTask(Of V)

		Private Const serialVersionUID As Long = 5232453952276485270L

		''' <summary>
		''' The result of the computation.
		''' </summary>
		Friend result As V

		''' <summary>
		''' The main computation performed by this task. </summary>
		''' <returns> the result of the computation </returns>
		Protected Friend MustOverride Function compute() As V

		Public Property rawResult As V
			Get
				Return result
			End Get
			Set(ByVal value As V)
				result = value
			End Set
		End Property


		''' <summary>
		''' Implements execution conventions for RecursiveTask.
		''' </summary>
		Protected Friend Function exec() As Boolean
			result = compute()
			Return True
		End Function

	End Class

End Namespace
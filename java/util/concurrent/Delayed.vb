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
	''' A mix-in style interface for marking objects that should be
	''' acted upon after a given delay.
	''' 
	''' <p>An implementation of this interface must define a
	''' {@code compareTo} method that provides an ordering consistent with
	''' its {@code getDelay} method.
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	Public Interface Delayed
		Inherits Comparable(Of Delayed)

		''' <summary>
		''' Returns the remaining delay associated with this object, in the
		''' given time unit.
		''' </summary>
		''' <param name="unit"> the time unit </param>
		''' <returns> the remaining delay; zero or negative values indicate
		''' that the delay has already elapsed </returns>
		Function getDelay(ByVal unit As TimeUnit) As Long
	End Interface

End Namespace
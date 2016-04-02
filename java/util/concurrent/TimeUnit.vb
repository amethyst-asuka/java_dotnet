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
	''' A {@code TimeUnit} represents time durations at a given unit of
	''' granularity and provides utility methods to convert across units,
	''' and to perform timing and delay operations in these units.  A
	''' {@code TimeUnit} does not maintain time information, but only
	''' helps organize and use time representations that may be maintained
	''' separately across various contexts.  A nanosecond is defined as one
	''' thousandth of a microsecond, a microsecond as one thousandth of a
	''' millisecond, a millisecond as one thousandth of a second, a minute
	''' as sixty seconds, an hour as sixty minutes, and a day as twenty four
	''' hours.
	''' 
	''' <p>A {@code TimeUnit} is mainly used to inform time-based methods
	''' how a given timing parameter should be interpreted. For example,
	''' the following code will timeout in 50 milliseconds if the {@link
	''' java.util.concurrent.locks.Lock lock} is not available:
	''' 
	'''  <pre> {@code
	''' Lock lock = ...;
	''' if (lock.tryLock(50L, TimeUnit.MILLISECONDS)) ...}</pre>
	''' 
	''' while this code will timeout in 50 seconds:
	'''  <pre> {@code
	''' Lock lock = ...;
	''' if (lock.tryLock(50L, TimeUnit.SECONDS)) ...}</pre>
	''' 
	''' Note however, that there is no guarantee that a particular timeout
	''' implementation will be able to notice the passage of time at the
	''' same granularity as the given {@code TimeUnit}.
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	Public Enum TimeUnit
		''' <summary>
		''' Time unit representing one thousandth of a microsecond
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		NANOSECONDS
			[public] long toNanos(long d) { Return d
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMicros(long d)
	'		{
	'			Return d/(C1/C0);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMillis(long d)
	'		{
	'			Return d/(C2/C0);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toSeconds(long d)
	'		{
	'			Return d/(C3/C0);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMinutes(long d)
	'		{
	'			Return d/(C4/C0);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toHours(long d)
	'		{
	'			Return d/(C5/C0);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toDays(long d)
	'		{
	'			Return d/(C6/C0);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long convert(long d, TimeUnit u)
	'		{
	'			Return u.toNanos(d);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			int excessNanos(long d, long m)
	'		{
	'			Return (int)(d - (m*C2));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

		''' <summary>
		''' Time unit representing one thousandth of a millisecond
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		MICROSECONDS
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toNanos(long d)
	'		{
	'			Return x(d, C1/C0, MAX/(C1/C0));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMicros(long d)
	'		{
	'			Return d;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMillis(long d)
	'		{
	'			Return d/(C2/C1);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toSeconds(long d)
	'		{
	'			Return d/(C3/C1);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMinutes(long d)
	'		{
	'			Return d/(C4/C1);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toHours(long d)
	'		{
	'			Return d/(C5/C1);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toDays(long d)
	'		{
	'			Return d/(C6/C1);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long convert(long d, TimeUnit u)
	'		{
	'			Return u.toMicros(d);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			int excessNanos(long d, long m)
	'		{
	'			Return (int)((d*C1) - (m*C2));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

		''' <summary>
		''' Time unit representing one thousandth of a second
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		MILLISECONDS
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toNanos(long d)
	'		{
	'			Return x(d, C2/C0, MAX/(C2/C0));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMicros(long d)
	'		{
	'			Return x(d, C2/C1, MAX/(C2/C1));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMillis(long d)
	'		{
	'			Return d;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toSeconds(long d)
	'		{
	'			Return d/(C3/C2);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMinutes(long d)
	'		{
	'			Return d/(C4/C2);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toHours(long d)
	'		{
	'			Return d/(C5/C2);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toDays(long d)
	'		{
	'			Return d/(C6/C2);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long convert(long d, TimeUnit u)
	'		{
	'			Return u.toMillis(d);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			int excessNanos(long d, long m)
	'		{
	'			Return 0;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

		''' <summary>
		''' Time unit representing one second
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		SECONDS
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toNanos(long d)
	'		{
	'			Return x(d, C3/C0, MAX/(C3/C0));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMicros(long d)
	'		{
	'			Return x(d, C3/C1, MAX/(C3/C1));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMillis(long d)
	'		{
	'			Return x(d, C3/C2, MAX/(C3/C2));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toSeconds(long d)
	'		{
	'			Return d;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMinutes(long d)
	'		{
	'			Return d/(C4/C3);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toHours(long d)
	'		{
	'			Return d/(C5/C3);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toDays(long d)
	'		{
	'			Return d/(C6/C3);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long convert(long d, TimeUnit u)
	'		{
	'			Return u.toSeconds(d);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			int excessNanos(long d, long m)
	'		{
	'			Return 0;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

		''' <summary>
		''' Time unit representing sixty seconds
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		MINUTES
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toNanos(long d)
	'		{
	'			Return x(d, C4/C0, MAX/(C4/C0));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMicros(long d)
	'		{
	'			Return x(d, C4/C1, MAX/(C4/C1));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMillis(long d)
	'		{
	'			Return x(d, C4/C2, MAX/(C4/C2));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toSeconds(long d)
	'		{
	'			Return x(d, C4/C3, MAX/(C4/C3));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMinutes(long d)
	'		{
	'			Return d;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toHours(long d)
	'		{
	'			Return d/(C5/C4);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toDays(long d)
	'		{
	'			Return d/(C6/C4);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long convert(long d, TimeUnit u)
	'		{
	'			Return u.toMinutes(d);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			int excessNanos(long d, long m)
	'		{
	'			Return 0;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

		''' <summary>
		''' Time unit representing sixty minutes
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		HOURS
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toNanos(long d)
	'		{
	'			Return x(d, C5/C0, MAX/(C5/C0));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMicros(long d)
	'		{
	'			Return x(d, C5/C1, MAX/(C5/C1));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMillis(long d)
	'		{
	'			Return x(d, C5/C2, MAX/(C5/C2));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toSeconds(long d)
	'		{
	'			Return x(d, C5/C3, MAX/(C5/C3));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMinutes(long d)
	'		{
	'			Return x(d, C5/C4, MAX/(C5/C4));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toHours(long d)
	'		{
	'			Return d;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toDays(long d)
	'		{
	'			Return d/(C6/C5);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long convert(long d, TimeUnit u)
	'		{
	'			Return u.toHours(d);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			int excessNanos(long d, long m)
	'		{
	'			Return 0;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

		''' <summary>
		''' Time unit representing twenty four hours
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		DAYS
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toNanos(long d)
	'		{
	'			Return x(d, C6/C0, MAX/(C6/C0));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMicros(long d)
	'		{
	'			Return x(d, C6/C1, MAX/(C6/C1));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMillis(long d)
	'		{
	'			Return x(d, C6/C2, MAX/(C6/C2));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toSeconds(long d)
	'		{
	'			Return x(d, C6/C3, MAX/(C6/C3));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toMinutes(long d)
	'		{
	'			Return x(d, C6/C4, MAX/(C6/C4));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toHours(long d)
	'		{
	'			Return x(d, C6/C5, MAX/(C6/C5));
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long toDays(long d)
	'		{
	'			Return d;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public long convert(long d, TimeUnit u)
	'		{
	'			Return u.toDays(d);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			int excessNanos(long d, long m)
	'		{
	'			Return 0;
	'		}

		' Handy constants for conversion methods
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final long C0 = 1L;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final long C1 = C0 * 1000L;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final long C2 = C1 * 1000L;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final long C3 = C2 * 1000L;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final long C4 = C3 * 60L;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final long C5 = C4 * 60L;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final long C6 = C5 * 24L;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final long MAX = java.lang.[Long].MAX_VALUE;

		''' <summary>
		''' Scale d by m, checking for overflow.
		''' This has a short name to make above code more readable.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		static long x(long d, long m, long over)
	'	{
	'		if (d > over)
	'			Return java.lang.[Long].MAX_VALUE;
	'		if (d < -over)
	'			Return java.lang.[Long].MIN_VALUE;
	'		Return d * m;
	'	}

		' To maintain full signature compatibility with 1.5, and to improve the
		' clarity of the generated javadoc (see 6287639: Abstract methods in
		' enum classes should not be listed as abstract), method convert
		' etc. are not declared abstract but otherwise act as abstract methods.

		''' <summary>
		''' Converts the given time duration in the given unit to this unit.
		''' Conversions from finer to coarser granularities truncate, so
		''' lose precision. For example, converting {@code 999} milliseconds
		''' to seconds results in {@code 0}. Conversions from coarser to
		''' finer granularities with arguments that would numerically
		''' overflow saturate to {@code java.lang.[Long].MIN_VALUE} if negative or
		''' {@code java.lang.[Long].MAX_VALUE} if positive.
		''' 
		''' <p>For example, to convert 10 minutes to milliseconds, use:
		''' {@code TimeUnit.MILLISECONDS.convert(10L, TimeUnit.MINUTES)}
		''' </summary>
		''' <param name="sourceDuration"> the time duration in the given {@code sourceUnit} </param>
		''' <param name="sourceUnit"> the unit of the {@code sourceDuration} argument </param>
		''' <returns> the converted duration in this unit,
		''' or {@code java.lang.[Long].MIN_VALUE} if conversion would negatively
		''' overflow, or {@code java.lang.[Long].MAX_VALUE} if it would positively overflow. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public long convert(long sourceDuration, TimeUnit sourceUnit)
	'	{
	'		throw New AbstractMethodError();
	'	}

		''' <summary>
		''' Equivalent to
		''' <seealso cref="#convert(long, TimeUnit) NANOSECONDS.convert(duration, this)"/>. </summary>
		''' <param name="duration"> the duration </param>
		''' <returns> the converted duration,
		''' or {@code java.lang.[Long].MIN_VALUE} if conversion would negatively
		''' overflow, or {@code java.lang.[Long].MAX_VALUE} if it would positively overflow. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public long toNanos(long duration)
	'	{
	'		throw New AbstractMethodError();
	'	}

		''' <summary>
		''' Equivalent to
		''' <seealso cref="#convert(long, TimeUnit) MICROSECONDS.convert(duration, this)"/>. </summary>
		''' <param name="duration"> the duration </param>
		''' <returns> the converted duration,
		''' or {@code java.lang.[Long].MIN_VALUE} if conversion would negatively
		''' overflow, or {@code java.lang.[Long].MAX_VALUE} if it would positively overflow. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public long toMicros(long duration)
	'	{
	'		throw New AbstractMethodError();
	'	}

		''' <summary>
		''' Equivalent to
		''' <seealso cref="#convert(long, TimeUnit) MILLISECONDS.convert(duration, this)"/>. </summary>
		''' <param name="duration"> the duration </param>
		''' <returns> the converted duration,
		''' or {@code java.lang.[Long].MIN_VALUE} if conversion would negatively
		''' overflow, or {@code java.lang.[Long].MAX_VALUE} if it would positively overflow. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public long toMillis(long duration)
	'	{
	'		throw New AbstractMethodError();
	'	}

		''' <summary>
		''' Equivalent to
		''' <seealso cref="#convert(long, TimeUnit) SECONDS.convert(duration, this)"/>. </summary>
		''' <param name="duration"> the duration </param>
		''' <returns> the converted duration,
		''' or {@code java.lang.[Long].MIN_VALUE} if conversion would negatively
		''' overflow, or {@code java.lang.[Long].MAX_VALUE} if it would positively overflow. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public long toSeconds(long duration)
	'	{
	'		throw New AbstractMethodError();
	'	}

		''' <summary>
		''' Equivalent to
		''' <seealso cref="#convert(long, TimeUnit) MINUTES.convert(duration, this)"/>. </summary>
		''' <param name="duration"> the duration </param>
		''' <returns> the converted duration,
		''' or {@code java.lang.[Long].MIN_VALUE} if conversion would negatively
		''' overflow, or {@code java.lang.[Long].MAX_VALUE} if it would positively overflow.
		''' @since 1.6 </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public long toMinutes(long duration)
	'	{
	'		throw New AbstractMethodError();
	'	}

		''' <summary>
		''' Equivalent to
		''' <seealso cref="#convert(long, TimeUnit) HOURS.convert(duration, this)"/>. </summary>
		''' <param name="duration"> the duration </param>
		''' <returns> the converted duration,
		''' or {@code java.lang.[Long].MIN_VALUE} if conversion would negatively
		''' overflow, or {@code java.lang.[Long].MAX_VALUE} if it would positively overflow.
		''' @since 1.6 </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public long toHours(long duration)
	'	{
	'		throw New AbstractMethodError();
	'	}

		''' <summary>
		''' Equivalent to
		''' <seealso cref="#convert(long, TimeUnit) DAYS.convert(duration, this)"/>. </summary>
		''' <param name="duration"> the duration </param>
		''' <returns> the converted duration
		''' @since 1.6 </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public long toDays(long duration)
	'	{
	'		throw New AbstractMethodError();
	'	}

		''' <summary>
		''' Utility to compute the excess-nanosecond argument to wait,
		''' sleep, join. </summary>
		''' <param name="d"> the duration </param>
		''' <param name="m"> the number of milliseconds </param>
		''' <returns> the number of nanoseconds </returns>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		abstract int excessNanos(long d, long m);

		''' <summary>
		''' Performs a timed <seealso cref="Object#wait(long, int) Object.wait"/>
		''' using this time unit.
		''' This is a convenience method that converts timeout arguments
		''' into the form required by the {@code Object.wait} method.
		''' 
		''' <p>For example, you could implement a blocking {@code poll}
		''' method (see <seealso cref="BlockingQueue#poll BlockingQueue.poll"/>)
		''' using:
		''' 
		'''  <pre> {@code
		''' Public  Object poll(long timeout, TimeUnit unit)
		'''     throws InterruptedException {
		'''   while (empty) {
		'''     unit.timedWait(this, timeout);
		'''     ...
		'''   }
		''' }}</pre>
		''' </summary>
		''' <param name="obj"> the object to wait on </param>
		''' <param name="timeout"> the maximum time to wait. If less than
		''' or equal to zero, do not wait at all. </param>
		''' <exception cref="InterruptedException"> if interrupted while waiting </exception>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public  Sub  timedWait(Object obj, long timeout) throws InterruptedException
	'	{
	'		if (timeout > 0)
	'		{
	'			long ms = toMillis(timeout);
	'			int ns = excessNanos(timeout, ms);
	'			obj.wait(ms, ns);
	'		}
	'	}

		''' <summary>
		''' Performs a timed <seealso cref="Thread#join(long, int) Thread.join"/>
		''' using this time unit.
		''' This is a convenience method that converts time arguments into the
		''' form required by the {@code Thread.join} method.
		''' </summary>
		''' <param name="thread"> the thread to wait for </param>
		''' <param name="timeout"> the maximum time to wait. If less than
		''' or equal to zero, do not wait at all. </param>
		''' <exception cref="InterruptedException"> if interrupted while waiting </exception>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public  Sub  timedJoin(Thread thread, long timeout) throws InterruptedException
	'	{
	'		if (timeout > 0)
	'		{
	'			long ms = toMillis(timeout);
	'			int ns = excessNanos(timeout, ms);
	'			thread.join(ms, ns);
	'		}
	'	}

		''' <summary>
		''' Performs a <seealso cref="Thread#sleep(long, int) Thread.sleep"/> using
		''' this time unit.
		''' This is a convenience method that converts time arguments into the
		''' form required by the {@code Thread.sleep} method.
		''' </summary>
		''' <param name="timeout"> the minimum time to sleep. If less than
		''' or equal to zero, do not sleep at all. </param>
		''' <exception cref="InterruptedException"> if interrupted while sleeping </exception>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public  Sub  sleep(long timeout) throws InterruptedException
	'	{
	'		if (timeout > 0)
	'		{
	'			long ms = toMillis(timeout);
	'			int ns = excessNanos(timeout, ms);
	'			Thread.sleep(ms, ns);
	'		}
	'	}


End Namespace
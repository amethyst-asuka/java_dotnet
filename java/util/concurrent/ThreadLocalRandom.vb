Imports Microsoft.VisualBasic
Imports System
Imports System.Threading

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
	''' A random number generator isolated to the current thread.  Like the
	''' global <seealso cref="java.util.Random"/> generator used by the {@link
	''' java.lang.Math} [Class], a {@code ThreadLocalRandom} is initialized
	''' with an internally generated seed that may not otherwise be
	''' modified. When applicable, use of {@code ThreadLocalRandom} rather
	''' than shared {@code Random} objects in concurrent programs will
	''' typically encounter much less overhead and contention.  Use of
	''' {@code ThreadLocalRandom} is particularly appropriate when multiple
	''' tasks (for example, each a <seealso cref="ForkJoinTask"/>) use random numbers
	''' in parallel in thread pools.
	''' 
	''' <p>Usages of this class should typically be of the form:
	''' {@code ThreadLocalRandom.current().nextX(...)} (where
	''' {@code X} is {@code Int}, {@code Long}, etc).
	''' When all usages are of this form, it is never possible to
	''' accidently share a {@code ThreadLocalRandom} across multiple threads.
	''' 
	''' <p>This class also provides additional commonly used bounded random
	''' generation methods.
	''' 
	''' <p>Instances of {@code ThreadLocalRandom} are not cryptographically
	''' secure.  Consider instead using <seealso cref="java.security.SecureRandom"/>
	''' in security-sensitive applications. Additionally,
	''' default-constructed instances do not use a cryptographically random
	''' seed unless the <seealso cref="System#getProperty system property"/>
	''' {@code java.util.secureRandomSeed} is set to {@code true}.
	''' 
	''' @since 1.7
	''' @author Doug Lea
	''' </summary>
	Public Class ThreadLocalRandom
		Inherits Random

	'    
	'     * This class implements the java.util.Random API (and subclasses
	'     * Random) using a single static instance that accesses random
	'     * number state held in class Thread (primarily, field
	'     * threadLocalRandomSeed). In doing so, it also provides a home
	'     * for managing package-private utilities that rely on exactly the
	'     * same state as needed to maintain the ThreadLocalRandom
	'     * instances. We leverage the need for an initialization flag
	'     * field to also use it as a "probe" -- a self-adjusting thread
	'     * hash used for contention avoidance, as well as a secondary
	'     * simpler (xorShift) random seed that is conservatively used to
	'     * avoid otherwise surprising users by hijacking the
	'     * ThreadLocalRandom sequence.  The dual use is a marriage of
	'     * convenience, but is a simple and efficient way of reducing
	'     * application-level overhead and footprint of most concurrent
	'     * programs.
	'     *
	'     * Even though this class subclasses java.util.Random, it uses the
	'     * same basic algorithm as java.util.SplittableRandom.  (See its
	'     * internal documentation for explanations, which are not repeated
	'     * here.)  Because ThreadLocalRandoms are not splittable
	'     * though, we use only a single 64bit gamma.
	'     *
	'     * Because this class is in a different package than class Thread,
	'     * field access methods use Unsafe to bypass access control rules.
	'     * To conform to the requirements of the Random superclass
	'     * constructor, the common static ThreadLocalRandom maintains an
	'     * "initialized" field for the sake of rejecting user calls to
	'     * setSeed while still allowing a call from constructor.  Note
	'     * that serialization is completely unnecessary because there is
	'     * only a static singleton.  But we generate a serial form
	'     * containing "rnd" and "initialized" fields to ensure
	'     * compatibility across versions.
	'     *
	'     * Implementations of non-core methods are mostly the same as in
	'     * SplittableRandom, that were in part derived from a previous
	'     * version of this class.
	'     *
	'     * The nextLocalGaussian ThreadLocal supports the very rarely used
	'     * nextGaussian method by providing a holder for the second of a
	'     * pair of them. As is true for the base class version of this
	'     * method, this time/space tradeoff is probably never worthwhile,
	'     * but we provide identical statistical properties.
	'     

		''' <summary>
		''' Generates per-thread initialization/probe field </summary>
		Private Shared ReadOnly probeGenerator As New java.util.concurrent.atomic.AtomicInteger

		''' <summary>
		''' The next seed for default constructors.
		''' </summary>
		Private Shared ReadOnly seeder As New java.util.concurrent.atomic.AtomicLong(initialSeed())

		Private Shared Function initialSeed() As Long
			Dim pp As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("java.util.secureRandomSeed"))
			If pp IsNot Nothing AndAlso pp.equalsIgnoreCase("true") Then
				Dim seedBytes As SByte() = java.security.SecureRandom.getSeed(8)
				Dim s As Long = CLng(seedBytes(0)) And &HffL
				For i As Integer = 1 To 7
					s = (s << 8) Or (CLng(seedBytes(i)) And &HffL)
				Next i
				Return s
			End If
			Return (mix64(System.currentTimeMillis()) Xor mix64(System.nanoTime()))
		End Function

		''' <summary>
		''' The seed increment
		''' </summary>
		Private Const GAMMA As Long = &H9e3779b97f4a7c15L

		''' <summary>
		''' The increment for generating probe values
		''' </summary>
		Private Const PROBE_INCREMENT As Integer = &H9e3779b9L

		''' <summary>
		''' The increment of seeder per new instance
		''' </summary>
		Private Const SEEDER_INCREMENT As Long = &Hbb67ae8584caa73bL

		' Constants from SplittableRandom
		Private Shared ReadOnly DOUBLE_UNIT As Double = &H1.0p-53 ' 1.0  / (1L << 53)
		Private Shared ReadOnly FLOAT_UNIT As Single = &H1.0p-24f ' 1.0f / (1 << 24)

		''' <summary>
		''' Rarely-used holder for the second of a pair of Gaussians </summary>
		Private Shared ReadOnly nextLocalGaussian As New ThreadLocal(Of Double?)

		Private Shared Function mix64(  z As Long) As Long
			z = (z Xor (CLng(CULng(z) >> 33))) * &Hff51afd7ed558ccdL
			z = (z Xor (CLng(CULng(z) >> 33))) * &Hc4ceb9fe1a85ec53L
			Return z Xor (CLng(CULng(z) >> 33))
		End Function

		Private Shared Function mix32(  z As Long) As Integer
			z = (z Xor (CLng(CULng(z) >> 33))) * &Hff51afd7ed558ccdL
			Return CInt(CInt(CUInt(((z Xor (CLng(CULng(z) >> 33))) * &Hc4ceb9fe1a85ec53L)) >> 32))
		End Function

		''' <summary>
		''' Field used only during singleton initialization.
		''' True when constructor completes.
		''' </summary>
		Friend initialized As Boolean

		''' <summary>
		''' Constructor used only for static singleton </summary>
		Private Sub New()
			initialized = True ' false during super() call
		End Sub

		''' <summary>
		''' The common ThreadLocalRandom </summary>
		Friend Shared ReadOnly instance As New ThreadLocalRandom

		''' <summary>
		''' Initialize Thread fields for the current thread.  Called only
		''' when Thread.threadLocalRandomProbe is zero, indicating that a
		''' thread local seed value needs to be generated. Note that even
		''' though the initialization is purely thread-local, we need to
		''' rely on (static) atomic generators to initialize the values.
		''' </summary>
		Friend Shared Sub localInit()
			Dim p As Integer = probeGenerator.addAndGet(PROBE_INCREMENT)
			Dim probe_Renamed As Integer = If(p = 0, 1, p) ' skip 0
			Dim seed_Renamed As Long = mix64(seeder.getAndAdd(SEEDER_INCREMENT))
			Dim t As Thread = Thread.CurrentThread
			UNSAFE.putLong(t, SEED, seed_Renamed)
			UNSAFE.putInt(t, PROBE, probe_Renamed)
		End Sub

		''' <summary>
		''' Returns the current thread's {@code ThreadLocalRandom}.
		''' </summary>
		''' <returns> the current thread's {@code ThreadLocalRandom} </returns>
		Public Shared Function current() As ThreadLocalRandom
			If UNSAFE.getInt(Thread.CurrentThread, PROBE) = 0 Then localInit()
			Return instance
		End Function

		''' <summary>
		''' Throws {@code UnsupportedOperationException}.  Setting seeds in
		''' this generator is not supported.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> always </exception>
		Public Overrides Property seed As Long
			Set(  seed As Long)
				' only allow call from super() constructor
				If initialized Then Throw New UnsupportedOperationException
			End Set
		End Property

		Friend Function nextSeed() As Long
			Dim t As Thread ' read and update per-thread seed
			Dim r As Long
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			UNSAFE.putLong(t = Thread.CurrentThread, SEED, r = UNSAFE.getLong(t, SEED) + GAMMA)
			Return r
		End Function

		' We must define this, but never use it.
		Protected Friend Overrides Function [next](  bits As Integer) As Integer
			Return CInt(CInt(CUInt(mix64(nextSeed())) >> (64 - bits)))
		End Function

		' IllegalArgumentException messages
		Friend Shadows Const BadBound As String = "bound must be positive"
		Friend Shadows Const BadRange As String = "bound must be greater than origin"
		Friend Shadows Const BadSize As String = "size must be non-negative"

		''' <summary>
		''' The form of nextLong used by LongStream Spliterators.  If
		''' origin is greater than bound, acts as unbounded form of
		''' nextLong, else as bounded form.
		''' </summary>
		''' <param name="origin"> the least value, unless greater than bound </param>
		''' <param name="bound"> the upper bound (exclusive), must not equal origin </param>
		''' <returns> a pseudorandom value </returns>
		Friend NotOverridable Overrides Function internalNextLong(  origin As Long,   bound As Long) As Long
			Dim r As Long = mix64(nextSeed())
			If origin < bound Then
				Dim n As Long = bound - origin, m As Long = n - 1
				If (n And m) = 0L Then ' power of two
					r = (r And m) + origin
				ElseIf n > 0L Then ' reject over-represented candidates
					Dim u As Long = CLng(CULng(r) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While u + m - (r = u Mod n) < 0L ' retry -  rejection check -  ensure nonnegative

						u = CInt(CUInt(mix64(nextSeed())) >> 1)
					Loop
					r += origin
				Else ' range not representable as long
					Do While r < origin OrElse r >= bound
						r = mix64(nextSeed())
					Loop
				End If
			End If
			Return r
		End Function

		''' <summary>
		''' The form of nextInt used by IntStream Spliterators.
		''' Exactly the same as long version, except for types.
		''' </summary>
		''' <param name="origin"> the least value, unless greater than bound </param>
		''' <param name="bound"> the upper bound (exclusive), must not equal origin </param>
		''' <returns> a pseudorandom value </returns>
		Friend NotOverridable Overrides Function internalNextInt(  origin As Integer,   bound As Integer) As Integer
			Dim r As Integer = mix32(nextSeed())
			If origin < bound Then
				Dim n As Integer = bound - origin, m As Integer = n - 1
				If (n And m) = 0 Then
					r = (r And m) + origin
				ElseIf n > 0 Then
					Dim u As Integer = CInt(CUInt(r) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While u + m - (r = u Mod n) < 0

						u = CInt(CUInt(mix32(nextSeed())) >> 1)
					Loop
					r += origin
				Else
					Do While r < origin OrElse r >= bound
						r = mix32(nextSeed())
					Loop
				End If
			End If
			Return r
		End Function

		''' <summary>
		''' The form of nextDouble used by DoubleStream Spliterators.
		''' </summary>
		''' <param name="origin"> the least value, unless greater than bound </param>
		''' <param name="bound"> the upper bound (exclusive), must not equal origin </param>
		''' <returns> a pseudorandom value </returns>
		Friend NotOverridable Overrides Function internalNextDouble(  origin As Double,   bound As Double) As Double
			Dim r As Double = (CInt(CUInt(nextLong()) >> 11)) * DOUBLE_UNIT
			If origin < bound Then
				r = r * (bound - origin) + origin
				If r >= bound Then ' correct for rounding r = java.lang.[Double].longBitsToDouble(Double.doubleToLongBits(bound) - 1)
			End If
			Return r
		End Function

		''' <summary>
		''' Returns a pseudorandom {@code int} value.
		''' </summary>
		''' <returns> a pseudorandom {@code int} value </returns>
		Public Overrides Function nextInt() As Integer
			Return mix32(nextSeed())
		End Function

		''' <summary>
		''' Returns a pseudorandom {@code int} value between zero (inclusive)
		''' and the specified bound (exclusive).
		''' </summary>
		''' <param name="bound"> the upper bound (exclusive).  Must be positive. </param>
		''' <returns> a pseudorandom {@code int} value between zero
		'''         (inclusive) and the bound (exclusive) </returns>
		''' <exception cref="IllegalArgumentException"> if {@code bound} is not positive </exception>
		Public Overrides Function nextInt(  bound As Integer) As Integer
			If bound <= 0 Then Throw New IllegalArgumentException(BadBound)
			Dim r As Integer = mix32(nextSeed())
			Dim m As Integer = bound - 1
			If (bound And m) = 0 Then ' power of two
				r = r And m
			Else ' reject over-represented candidates
				Dim u As Integer = CInt(CUInt(r) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While u + m - (r = u Mod bound) < 0

					u = CInt(CUInt(mix32(nextSeed())) >> 1)
				Loop
			End If
			Return r
		End Function

		''' <summary>
		''' Returns a pseudorandom {@code int} value between the specified
		''' origin (inclusive) and the specified bound (exclusive).
		''' </summary>
		''' <param name="origin"> the least value returned </param>
		''' <param name="bound"> the upper bound (exclusive) </param>
		''' <returns> a pseudorandom {@code int} value between the origin
		'''         (inclusive) and the bound (exclusive) </returns>
		''' <exception cref="IllegalArgumentException"> if {@code origin} is greater than
		'''         or equal to {@code bound} </exception>
		Public Overridable Function nextInt(  origin As Integer,   bound As Integer) As Integer
			If origin >= bound Then Throw New IllegalArgumentException(BadRange)
			Return internalNextInt(origin, bound)
		End Function

		''' <summary>
		''' Returns a pseudorandom {@code long} value.
		''' </summary>
		''' <returns> a pseudorandom {@code long} value </returns>
		Public Overrides Function nextLong() As Long
			Return mix64(nextSeed())
		End Function

		''' <summary>
		''' Returns a pseudorandom {@code long} value between zero (inclusive)
		''' and the specified bound (exclusive).
		''' </summary>
		''' <param name="bound"> the upper bound (exclusive).  Must be positive. </param>
		''' <returns> a pseudorandom {@code long} value between zero
		'''         (inclusive) and the bound (exclusive) </returns>
		''' <exception cref="IllegalArgumentException"> if {@code bound} is not positive </exception>
		Public Overridable Function nextLong(  bound As Long) As Long
			If bound <= 0 Then Throw New IllegalArgumentException(BadBound)
			Dim r As Long = mix64(nextSeed())
			Dim m As Long = bound - 1
			If (bound And m) = 0L Then ' power of two
				r = r And m
			Else ' reject over-represented candidates
				Dim u As Long = CLng(CULng(r) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While u + m - (r = u Mod bound) < 0L

					u = CInt(CUInt(mix64(nextSeed())) >> 1)
				Loop
			End If
			Return r
		End Function

		''' <summary>
		''' Returns a pseudorandom {@code long} value between the specified
		''' origin (inclusive) and the specified bound (exclusive).
		''' </summary>
		''' <param name="origin"> the least value returned </param>
		''' <param name="bound"> the upper bound (exclusive) </param>
		''' <returns> a pseudorandom {@code long} value between the origin
		'''         (inclusive) and the bound (exclusive) </returns>
		''' <exception cref="IllegalArgumentException"> if {@code origin} is greater than
		'''         or equal to {@code bound} </exception>
		Public Overridable Function nextLong(  origin As Long,   bound As Long) As Long
			If origin >= bound Then Throw New IllegalArgumentException(BadRange)
			Return internalNextLong(origin, bound)
		End Function

		''' <summary>
		''' Returns a pseudorandom {@code double} value between zero
		''' (inclusive) and one (exclusive).
		''' </summary>
		''' <returns> a pseudorandom {@code double} value between zero
		'''         (inclusive) and one (exclusive) </returns>
		Public Overrides Function nextDouble() As Double
			Return (CInt(CUInt(mix64(nextSeed())) >> 11)) * DOUBLE_UNIT
		End Function

		''' <summary>
		''' Returns a pseudorandom {@code double} value between 0.0
		''' (inclusive) and the specified bound (exclusive).
		''' </summary>
		''' <param name="bound"> the upper bound (exclusive).  Must be positive. </param>
		''' <returns> a pseudorandom {@code double} value between zero
		'''         (inclusive) and the bound (exclusive) </returns>
		''' <exception cref="IllegalArgumentException"> if {@code bound} is not positive </exception>
		Public Overridable Function nextDouble(  bound As Double) As Double
			If Not(bound > 0.0) Then Throw New IllegalArgumentException(BadBound)
			Dim result As Double = (CInt(CUInt(mix64(nextSeed())) >> 11)) * DOUBLE_UNIT * bound
			Return If(result < bound, result, java.lang.[Double].longBitsToDouble(Double.doubleToLongBits(bound) - 1)) ' correct for rounding
		End Function

		''' <summary>
		''' Returns a pseudorandom {@code double} value between the specified
		''' origin (inclusive) and bound (exclusive).
		''' </summary>
		''' <param name="origin"> the least value returned </param>
		''' <param name="bound"> the upper bound (exclusive) </param>
		''' <returns> a pseudorandom {@code double} value between the origin
		'''         (inclusive) and the bound (exclusive) </returns>
		''' <exception cref="IllegalArgumentException"> if {@code origin} is greater than
		'''         or equal to {@code bound} </exception>
		Public Overridable Function nextDouble(  origin As Double,   bound As Double) As Double
			If Not(origin < bound) Then Throw New IllegalArgumentException(BadRange)
			Return internalNextDouble(origin, bound)
		End Function

		''' <summary>
		''' Returns a pseudorandom {@code boolean} value.
		''' </summary>
		''' <returns> a pseudorandom {@code boolean} value </returns>
		Public Overrides Function nextBoolean() As Boolean
			Return mix32(nextSeed()) < 0
		End Function

		''' <summary>
		''' Returns a pseudorandom {@code float} value between zero
		''' (inclusive) and one (exclusive).
		''' </summary>
		''' <returns> a pseudorandom {@code float} value between zero
		'''         (inclusive) and one (exclusive) </returns>
		Public Overrides Function nextFloat() As Single
			Return (CInt(CUInt(mix32(nextSeed())) >> 8)) * FLOAT_UNIT
		End Function

		Public Overrides Function nextGaussian() As Double
			' Use nextLocalGaussian instead of nextGaussian field
			Dim d As Double? = nextLocalGaussian.get()
			If d IsNot Nothing Then
				nextLocalGaussian.set(Nothing)
				Return d
			End If
			Dim v1, v2, s As Double
			Do
				v1 = 2 * nextDouble() - 1 ' between -1 and 1
				v2 = 2 * nextDouble() - 1 ' between -1 and 1
				s = v1 * v1 + v2 * v2
			Loop While s >= 1 OrElse s = 0
			Dim multiplier As Double = System.Math.Sqrt(-2 * System.Math.Log(s)/s)
			nextLocalGaussian.set(New Double?(v2 * multiplier))
			Return v1 * multiplier
		End Function

		' stream methods, coded in a way intended to better isolate for
		' maintenance purposes the small differences across forms.

		''' <summary>
		''' Returns a stream producing the given {@code streamSize} number of
		''' pseudorandom {@code int} values.
		''' </summary>
		''' <param name="streamSize"> the number of values to generate </param>
		''' <returns> a stream of pseudorandom {@code int} values </returns>
		''' <exception cref="IllegalArgumentException"> if {@code streamSize} is
		'''         less than zero
		''' @since 1.8 </exception>
		Public Overrides Function ints(  streamSize As Long) As java.util.stream.IntStream
			If streamSize < 0L Then Throw New IllegalArgumentException(BadSize)
			Return java.util.stream.StreamSupport.intStream(New RandomIntsSpliterator(0L, streamSize,  java.lang.[Integer].Max_Value, 0), False)
		End Function

		''' <summary>
		''' Returns an effectively unlimited stream of pseudorandom {@code int}
		''' values.
		''' 
		''' @implNote This method is implemented to be equivalent to {@code
		''' ints(Long.MAX_VALUE)}.
		''' </summary>
		''' <returns> a stream of pseudorandom {@code int} values
		''' @since 1.8 </returns>
		Public Overrides Function ints() As java.util.stream.IntStream
			Return java.util.stream.StreamSupport.intStream(New RandomIntsSpliterator(0L, java.lang.[Long].Max_Value,  java.lang.[Integer].Max_Value, 0), False)
		End Function

		''' <summary>
		''' Returns a stream producing the given {@code streamSize} number
		''' of pseudorandom {@code int} values, each conforming to the given
		''' origin (inclusive) and bound (exclusive).
		''' </summary>
		''' <param name="streamSize"> the number of values to generate </param>
		''' <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		''' <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		''' <returns> a stream of pseudorandom {@code int} values,
		'''         each with the given origin (inclusive) and bound (exclusive) </returns>
		''' <exception cref="IllegalArgumentException"> if {@code streamSize} is
		'''         less than zero, or {@code randomNumberOrigin}
		'''         is greater than or equal to {@code randomNumberBound}
		''' @since 1.8 </exception>
		Public Overrides Function ints(  streamSize As Long,   randomNumberOrigin As Integer,   randomNumberBound As Integer) As java.util.stream.IntStream
			If streamSize < 0L Then Throw New IllegalArgumentException(BadSize)
			If randomNumberOrigin >= randomNumberBound Then Throw New IllegalArgumentException(BadRange)
			Return java.util.stream.StreamSupport.intStream(New RandomIntsSpliterator(0L, streamSize, randomNumberOrigin, randomNumberBound), False)
		End Function

		''' <summary>
		''' Returns an effectively unlimited stream of pseudorandom {@code
		''' int} values, each conforming to the given origin (inclusive) and bound
		''' (exclusive).
		''' 
		''' @implNote This method is implemented to be equivalent to {@code
		''' ints(Long.MAX_VALUE, randomNumberOrigin, randomNumberBound)}.
		''' </summary>
		''' <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		''' <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		''' <returns> a stream of pseudorandom {@code int} values,
		'''         each with the given origin (inclusive) and bound (exclusive) </returns>
		''' <exception cref="IllegalArgumentException"> if {@code randomNumberOrigin}
		'''         is greater than or equal to {@code randomNumberBound}
		''' @since 1.8 </exception>
		Public Overrides Function ints(  randomNumberOrigin As Integer,   randomNumberBound As Integer) As java.util.stream.IntStream
			If randomNumberOrigin >= randomNumberBound Then Throw New IllegalArgumentException(BadRange)
			Return java.util.stream.StreamSupport.intStream(New RandomIntsSpliterator(0L, java.lang.[Long].Max_Value, randomNumberOrigin, randomNumberBound), False)
		End Function

		''' <summary>
		''' Returns a stream producing the given {@code streamSize} number of
		''' pseudorandom {@code long} values.
		''' </summary>
		''' <param name="streamSize"> the number of values to generate </param>
		''' <returns> a stream of pseudorandom {@code long} values </returns>
		''' <exception cref="IllegalArgumentException"> if {@code streamSize} is
		'''         less than zero
		''' @since 1.8 </exception>
		Public Overrides Function longs(  streamSize As Long) As java.util.stream.LongStream
			If streamSize < 0L Then Throw New IllegalArgumentException(BadSize)
			Return java.util.stream.StreamSupport.longStream(New RandomLongsSpliterator(0L, streamSize, java.lang.[Long].Max_Value, 0L), False)
		End Function

		''' <summary>
		''' Returns an effectively unlimited stream of pseudorandom {@code long}
		''' values.
		''' 
		''' @implNote This method is implemented to be equivalent to {@code
		''' longs(Long.MAX_VALUE)}.
		''' </summary>
		''' <returns> a stream of pseudorandom {@code long} values
		''' @since 1.8 </returns>
		Public Overrides Function longs() As java.util.stream.LongStream
			Return java.util.stream.StreamSupport.longStream(New RandomLongsSpliterator(0L, java.lang.[Long].Max_Value, java.lang.[Long].Max_Value, 0L), False)
		End Function

		''' <summary>
		''' Returns a stream producing the given {@code streamSize} number of
		''' pseudorandom {@code long}, each conforming to the given origin
		''' (inclusive) and bound (exclusive).
		''' </summary>
		''' <param name="streamSize"> the number of values to generate </param>
		''' <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		''' <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		''' <returns> a stream of pseudorandom {@code long} values,
		'''         each with the given origin (inclusive) and bound (exclusive) </returns>
		''' <exception cref="IllegalArgumentException"> if {@code streamSize} is
		'''         less than zero, or {@code randomNumberOrigin}
		'''         is greater than or equal to {@code randomNumberBound}
		''' @since 1.8 </exception>
		Public Overrides Function longs(  streamSize As Long,   randomNumberOrigin As Long,   randomNumberBound As Long) As java.util.stream.LongStream
			If streamSize < 0L Then Throw New IllegalArgumentException(BadSize)
			If randomNumberOrigin >= randomNumberBound Then Throw New IllegalArgumentException(BadRange)
			Return java.util.stream.StreamSupport.longStream(New RandomLongsSpliterator(0L, streamSize, randomNumberOrigin, randomNumberBound), False)
		End Function

		''' <summary>
		''' Returns an effectively unlimited stream of pseudorandom {@code
		''' long} values, each conforming to the given origin (inclusive) and bound
		''' (exclusive).
		''' 
		''' @implNote This method is implemented to be equivalent to {@code
		''' longs(Long.MAX_VALUE, randomNumberOrigin, randomNumberBound)}.
		''' </summary>
		''' <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		''' <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		''' <returns> a stream of pseudorandom {@code long} values,
		'''         each with the given origin (inclusive) and bound (exclusive) </returns>
		''' <exception cref="IllegalArgumentException"> if {@code randomNumberOrigin}
		'''         is greater than or equal to {@code randomNumberBound}
		''' @since 1.8 </exception>
		Public Overrides Function longs(  randomNumberOrigin As Long,   randomNumberBound As Long) As java.util.stream.LongStream
			If randomNumberOrigin >= randomNumberBound Then Throw New IllegalArgumentException(BadRange)
			Return java.util.stream.StreamSupport.longStream(New RandomLongsSpliterator(0L, java.lang.[Long].Max_Value, randomNumberOrigin, randomNumberBound), False)
		End Function

		''' <summary>
		''' Returns a stream producing the given {@code streamSize} number of
		''' pseudorandom {@code double} values, each between zero
		''' (inclusive) and one (exclusive).
		''' </summary>
		''' <param name="streamSize"> the number of values to generate </param>
		''' <returns> a stream of {@code double} values </returns>
		''' <exception cref="IllegalArgumentException"> if {@code streamSize} is
		'''         less than zero
		''' @since 1.8 </exception>
		Public Overrides Function doubles(  streamSize As Long) As java.util.stream.DoubleStream
			If streamSize < 0L Then Throw New IllegalArgumentException(BadSize)
			Return java.util.stream.StreamSupport.doubleStream(New RandomDoublesSpliterator(0L, streamSize, java.lang.[Double].Max_Value, 0.0), False)
		End Function

		''' <summary>
		''' Returns an effectively unlimited stream of pseudorandom {@code
		''' double} values, each between zero (inclusive) and one
		''' (exclusive).
		''' 
		''' @implNote This method is implemented to be equivalent to {@code
		''' doubles(Long.MAX_VALUE)}.
		''' </summary>
		''' <returns> a stream of pseudorandom {@code double} values
		''' @since 1.8 </returns>
		Public Overrides Function doubles() As java.util.stream.DoubleStream
			Return java.util.stream.StreamSupport.doubleStream(New RandomDoublesSpliterator(0L, java.lang.[Long].Max_Value, java.lang.[Double].Max_Value, 0.0), False)
		End Function

		''' <summary>
		''' Returns a stream producing the given {@code streamSize} number of
		''' pseudorandom {@code double} values, each conforming to the given origin
		''' (inclusive) and bound (exclusive).
		''' </summary>
		''' <param name="streamSize"> the number of values to generate </param>
		''' <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		''' <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		''' <returns> a stream of pseudorandom {@code double} values,
		'''         each with the given origin (inclusive) and bound (exclusive) </returns>
		''' <exception cref="IllegalArgumentException"> if {@code streamSize} is
		'''         less than zero </exception>
		''' <exception cref="IllegalArgumentException"> if {@code randomNumberOrigin}
		'''         is greater than or equal to {@code randomNumberBound}
		''' @since 1.8 </exception>
		Public Overrides Function doubles(  streamSize As Long,   randomNumberOrigin As Double,   randomNumberBound As Double) As java.util.stream.DoubleStream
			If streamSize < 0L Then Throw New IllegalArgumentException(BadSize)
			If Not(randomNumberOrigin < randomNumberBound) Then Throw New IllegalArgumentException(BadRange)
			Return java.util.stream.StreamSupport.doubleStream(New RandomDoublesSpliterator(0L, streamSize, randomNumberOrigin, randomNumberBound), False)
		End Function

		''' <summary>
		''' Returns an effectively unlimited stream of pseudorandom {@code
		''' double} values, each conforming to the given origin (inclusive) and bound
		''' (exclusive).
		''' 
		''' @implNote This method is implemented to be equivalent to {@code
		''' doubles(Long.MAX_VALUE, randomNumberOrigin, randomNumberBound)}.
		''' </summary>
		''' <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		''' <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		''' <returns> a stream of pseudorandom {@code double} values,
		'''         each with the given origin (inclusive) and bound (exclusive) </returns>
		''' <exception cref="IllegalArgumentException"> if {@code randomNumberOrigin}
		'''         is greater than or equal to {@code randomNumberBound}
		''' @since 1.8 </exception>
		Public Overrides Function doubles(  randomNumberOrigin As Double,   randomNumberBound As Double) As java.util.stream.DoubleStream
			If Not(randomNumberOrigin < randomNumberBound) Then Throw New IllegalArgumentException(BadRange)
			Return java.util.stream.StreamSupport.doubleStream(New RandomDoublesSpliterator(0L, java.lang.[Long].Max_Value, randomNumberOrigin, randomNumberBound), False)
		End Function

		''' <summary>
		''' Spliterator for int streams.  We multiplex the four int
		''' versions into one class by treating a bound less than origin as
		''' unbounded, and also by treating "infinite" as equivalent to
		''' java.lang.[Long].MAX_VALUE. For splits, it uses the standard divide-by-two
		''' approach. The long and double versions of this class are
		''' identical except for types.
		''' </summary>
		Friend NotInheritable Class RandomIntsSpliterator
			Implements java.util.Spliterator.OfInt

			Friend index As Long
			Friend ReadOnly fence As Long
			Friend ReadOnly origin As Integer
			Friend ReadOnly bound As Integer
			Friend Sub New(  index As Long,   fence As Long,   origin As Integer,   bound As Integer)
				Me.index = index
				Me.fence = fence
				Me.origin = origin
				Me.bound = bound
			End Sub

			Public Function trySplit() As RandomIntsSpliterator
				Dim i As Long = index, m As Long = CInt(CUInt((i + fence)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(m <= i, Nothing, New RandomIntsSpliterator(i, index = m, origin, bound))
			End Function

			Public Function estimateSize() As Long
				Return fence - index
			End Function

			Public Function characteristics() As Integer
				Return (java.util.Spliterator.SIZED Or java.util.Spliterator.SUBSIZED Or java.util.Spliterator.NONNULL Or java.util.Spliterator.IMMUTABLE)
			End Function

			Public Function tryAdvance(  consumer As java.util.function.IntConsumer) As Boolean
				If consumer Is Nothing Then Throw New NullPointerException
				Dim i As Long = index, f As Long = fence
				If i < f Then
					consumer.accept(ThreadLocalRandom.current().internalNextInt(origin, bound))
					index = i + 1
					Return True
				End If
				Return False
			End Function

			Public Sub forEachRemaining(  consumer As java.util.function.IntConsumer)
				If consumer Is Nothing Then Throw New NullPointerException
				Dim i As Long = index, f As Long = fence
				If i < f Then
					index = f
					Dim o As Integer = origin, b As Integer = bound
					Dim rng As ThreadLocalRandom = ThreadLocalRandom.current()
					Do
						consumer.accept(rng.internalNextInt(o, b))
						i += 1
					Loop While i < f
				End If
			End Sub
		End Class

		''' <summary>
		''' Spliterator for long streams.
		''' </summary>
		Friend NotInheritable Class RandomLongsSpliterator
			Implements java.util.Spliterator.OfLong

			Friend index As Long
			Friend ReadOnly fence As Long
			Friend ReadOnly origin As Long
			Friend ReadOnly bound As Long
			Friend Sub New(  index As Long,   fence As Long,   origin As Long,   bound As Long)
				Me.index = index
				Me.fence = fence
				Me.origin = origin
				Me.bound = bound
			End Sub

			Public Function trySplit() As RandomLongsSpliterator
				Dim i As Long = index, m As Long = CInt(CUInt((i + fence)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(m <= i, Nothing, New RandomLongsSpliterator(i, index = m, origin, bound))
			End Function

			Public Function estimateSize() As Long
				Return fence - index
			End Function

			Public Function characteristics() As Integer
				Return (java.util.Spliterator.SIZED Or java.util.Spliterator.SUBSIZED Or java.util.Spliterator.NONNULL Or java.util.Spliterator.IMMUTABLE)
			End Function

			Public Function tryAdvance(  consumer As java.util.function.LongConsumer) As Boolean
				If consumer Is Nothing Then Throw New NullPointerException
				Dim i As Long = index, f As Long = fence
				If i < f Then
					consumer.accept(ThreadLocalRandom.current().internalNextLong(origin, bound))
					index = i + 1
					Return True
				End If
				Return False
			End Function

			Public Sub forEachRemaining(  consumer As java.util.function.LongConsumer)
				If consumer Is Nothing Then Throw New NullPointerException
				Dim i As Long = index, f As Long = fence
				If i < f Then
					index = f
					Dim o As Long = origin, b As Long = bound
					Dim rng As ThreadLocalRandom = ThreadLocalRandom.current()
					Do
						consumer.accept(rng.internalNextLong(o, b))
						i += 1
					Loop While i < f
				End If
			End Sub

		End Class

		''' <summary>
		''' Spliterator for double streams.
		''' </summary>
		Friend NotInheritable Class RandomDoublesSpliterator
			Implements java.util.Spliterator.OfDouble

			Friend index As Long
			Friend ReadOnly fence As Long
			Friend ReadOnly origin As Double
			Friend ReadOnly bound As Double
			Friend Sub New(  index As Long,   fence As Long,   origin As Double,   bound As Double)
				Me.index = index
				Me.fence = fence
				Me.origin = origin
				Me.bound = bound
			End Sub

			Public Function trySplit() As RandomDoublesSpliterator
				Dim i As Long = index, m As Long = CInt(CUInt((i + fence)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(m <= i, Nothing, New RandomDoublesSpliterator(i, index = m, origin, bound))
			End Function

			Public Function estimateSize() As Long
				Return fence - index
			End Function

			Public Function characteristics() As Integer
				Return (java.util.Spliterator.SIZED Or java.util.Spliterator.SUBSIZED Or java.util.Spliterator.NONNULL Or java.util.Spliterator.IMMUTABLE)
			End Function

			Public Function tryAdvance(  consumer As java.util.function.DoubleConsumer) As Boolean
				If consumer Is Nothing Then Throw New NullPointerException
				Dim i As Long = index, f As Long = fence
				If i < f Then
					consumer.accept(ThreadLocalRandom.current().internalNextDouble(origin, bound))
					index = i + 1
					Return True
				End If
				Return False
			End Function

			Public Sub forEachRemaining(  consumer As java.util.function.DoubleConsumer)
				If consumer Is Nothing Then Throw New NullPointerException
				Dim i As Long = index, f As Long = fence
				If i < f Then
					index = f
					Dim o As Double = origin, b As Double = bound
					Dim rng As ThreadLocalRandom = ThreadLocalRandom.current()
					Do
						consumer.accept(rng.internalNextDouble(o, b))
						i += 1
					Loop While i < f
				End If
			End Sub
		End Class


		' Within-package utilities

	'    
	'     * Descriptions of the usages of the methods below can be found in
	'     * the classes that use them. Briefly, a thread's "probe" value is
	'     * a non-zero hash code that (probably) does not collide with
	'     * other existing threads with respect to any power of two
	'     * collision space. When it does collide, it is pseudo-randomly
	'     * adjusted (using a Marsaglia XorShift). The nextSecondarySeed
	'     * method is used in the same contexts as ThreadLocalRandom, but
	'     * only for transient usages such as random adaptive spin/block
	'     * sequences for which a cheap RNG suffices and for which it could
	'     * in principle disrupt user-visible statistical properties of the
	'     * main ThreadLocalRandom if we were to use it.
	'     *
	'     * Note: Because of package-protection issues, versions of some
	'     * these methods also appear in some subpackage classes.
	'     

		''' <summary>
		''' Returns the probe value for the current thread without forcing
		''' initialization. Note that invoking ThreadLocalRandom.current()
		''' can be used to force initialization on zero return.
		''' </summary>
		FriendShared ReadOnly Propertyprobe As Integer
			Get
				Return UNSAFE.getInt(Thread.CurrentThread, PROBE)
			End Get
		End Property

		''' <summary>
		''' Pseudo-randomly advances and records the given probe value for the
		''' given thread.
		''' </summary>
		Friend Shared Function advanceProbe(  probe As Integer) As Integer
			probe = probe Xor probe << 13 ' xorshift
			probe = probe Xor CInt(CUInt(probe) >> 17)
			probe = probe Xor probe << 5
			UNSAFE.putInt(Thread.CurrentThread, ThreadLocalRandom.PROBE, probe)
			Return probe
		End Function

		''' <summary>
		''' Returns the pseudo-randomly initialized or updated secondary seed.
		''' </summary>
		Friend Shared Function nextSecondarySeed() As Integer
			Dim r As Integer
			Dim t As Thread = Thread.CurrentThread
			r = UNSAFE.getInt(t, SECONDARY)
			If r <> 0 Then
				r = r Xor r << 13 ' xorshift
				r = r Xor CInt(CUInt(r) >> 17)
				r = r Xor r << 5
			Else
				localInit()
				r = CInt(Fix(UNSAFE.getLong(t, SEED)))
				If r = 0 Then r = 1 ' avoid zero
			End If
			UNSAFE.putInt(t, SECONDARY, r)
			Return r
		End Function

		' Serialization support

		Private Shadows Const serialVersionUID As Long = -5851777807851030925L

		''' <summary>
		''' @serialField rnd long
		'''              seed for random computations
		''' @serialField initialized boolean
		'''              always true
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("rnd", GetType(Long)), New java.io.ObjectStreamField("initialized", GetType(Boolean)) }

		''' <summary>
		''' Saves the {@code ThreadLocalRandom} to a stream (that is, serializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
		Private Sub writeObject(  s As java.io.ObjectOutputStream)

			Dim fields As java.io.ObjectOutputStream.PutField = s.putFields()
			fields.put("rnd", UNSAFE.getLong(Thread.CurrentThread, SEED))
			fields.put("initialized", True)
			s.writeFields()
		End Sub

		''' <summary>
		''' Returns the <seealso cref="#current() current"/> thread's {@code ThreadLocalRandom}. </summary>
		''' <returns> the <seealso cref="#current() current"/> thread's {@code ThreadLocalRandom} </returns>
		Private Function readResolve() As Object
			Return current()
		End Function

		' Unsafe mechanics
		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly SEED As Long
		Private Shared ReadOnly PROBE As Long
		Private Shared ReadOnly SECONDARY As Long
		Shared Sub New()
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim tk As  [Class] = GetType(Thread)
				SEED = UNSAFE.objectFieldOffset(tk.getDeclaredField("threadLocalRandomSeed"))
				PROBE = UNSAFE.objectFieldOffset(tk.getDeclaredField("threadLocalRandomProbe"))
				SECONDARY = UNSAFE.objectFieldOffset(tk.getDeclaredField("threadLocalRandomSecondarySeed"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub
	End Class

End Namespace
Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' An instance of this class is used to generate a stream of
	''' pseudorandom numbers. The class uses a 48-bit seed, which is
	''' modified using a linear congruential formula. (See Donald Knuth,
	''' <i>The Art of Computer Programming, Volume 2</i>, Section 3.2.1.)
	''' <p>
	''' If two instances of {@code Random} are created with the same
	''' seed, and the same sequence of method calls is made for each, they
	''' will generate and return identical sequences of numbers. In order to
	''' guarantee this property, particular algorithms are specified for the
	''' class {@code Random}. Java implementations must use all the algorithms
	''' shown here for the class {@code Random}, for the sake of absolute
	''' portability of Java code. However, subclasses of class {@code Random}
	''' are permitted to use other algorithms, so long as they adhere to the
	''' general contracts for all the methods.
	''' <p>
	''' The algorithms implemented by class {@code Random} use a
	''' {@code protected} utility method that on each invocation can supply
	''' up to 32 pseudorandomly generated bits.
	''' <p>
	''' Many applications will find the method <seealso cref="Math#random"/> simpler to use.
	''' 
	''' <p>Instances of {@code java.util.Random} are threadsafe.
	''' However, the concurrent use of the same {@code java.util.Random}
	''' instance across threads may encounter contention and consequent
	''' poor performance. Consider instead using
	''' <seealso cref="java.util.concurrent.ThreadLocalRandom"/> in multithreaded
	''' designs.
	''' 
	''' <p>Instances of {@code java.util.Random} are not cryptographically
	''' secure.  Consider instead using <seealso cref="java.security.SecureRandom"/> to
	''' get a cryptographically secure pseudo-random number generator for use
	''' by security-sensitive applications.
	''' 
	''' @author  Frank Yellin
	''' @since   1.0
	''' </summary>
	<Serializable> _
	Public Class Random
		''' <summary>
		''' use serialVersionUID from JDK 1.1 for interoperability </summary>
		Friend Const serialVersionUID As Long = 3905348978240129619L

		''' <summary>
		''' The internal state associated with this pseudorandom number generator.
		''' (The specs for the methods in this class describe the ongoing
		''' computation of this value.)
		''' </summary>
		Private ReadOnly seed As java.util.concurrent.atomic.AtomicLong

		Private Const multiplier As Long = &H5DEECE66RL
		Private Const addend As Long = &HBL
		Private Shared ReadOnly mask As Long = (1L << 48) - 1

		Private Shared ReadOnly DOUBLE_UNIT As Double = &H1.0p-53 ' 1.0 / (1L << 53)

		' IllegalArgumentException messages
		Friend Const BadBound As String = "bound must be positive"
		Friend Const BadRange As String = "bound must be greater than origin"
		Friend Const BadSize As String = "size must be non-negative"

		''' <summary>
		''' Creates a new random number generator. This constructor sets
		''' the seed of the random number generator to a value very likely
		''' to be distinct from any other invocation of this constructor.
		''' </summary>
		Public Sub New()
			Me.New(seedUniquifier() Xor System.nanoTime())
		End Sub

		Private Shared Function seedUniquifier() As Long
			' L'Ecuyer, "Tables of Linear Congruential Generators of
			' Different Sizes and Good Lattice Structure", 1999
			Do
				Dim current As Long = seedUniquifier_Renamed.get()
				Dim [next] As Long = current * 181783497276652981L
				If seedUniquifier_Renamed.compareAndSet(current, [next]) Then Return [next]
			Loop
		End Function

		Private Shared ReadOnly seedUniquifier_Renamed As New java.util.concurrent.atomic.AtomicLong(8682522807148012L)

		''' <summary>
		''' Creates a new random number generator using a single {@code long} seed.
		''' The seed is the initial value of the internal state of the pseudorandom
		''' number generator which is maintained by method <seealso cref="#next"/>.
		''' 
		''' <p>The invocation {@code new Random(seed)} is equivalent to:
		'''  <pre> {@code
		''' Random rnd = new Random();
		''' rnd.setSeed(seed);}</pre>
		''' </summary>
		''' <param name="seed"> the initial seed </param>
		''' <seealso cref=   #setSeed(long) </seealso>
		Public Sub New(ByVal seed As Long)
			If Me.GetType() = GetType(Random) Then
				Me.seed = New java.util.concurrent.atomic.AtomicLong(initialScramble(seed))
			Else
				' subclass might have overriden setSeed
				Me.seed = New java.util.concurrent.atomic.AtomicLong
				seed = seed
			End If
		End Sub

		Private Shared Function initialScramble(ByVal seed As Long) As Long
			Return (seed Xor multiplier) And mask
		End Function

		''' <summary>
		''' Sets the seed of this random number generator using a single
		''' {@code long} seed. The general contract of {@code setSeed} is
		''' that it alters the state of this random number generator object
		''' so as to be in exactly the same state as if it had just been
		''' created with the argument {@code seed} as a seed. The method
		''' {@code setSeed} is implemented by class {@code Random} by
		''' atomically updating the seed to
		'''  <pre>{@code (seed ^ 0x5DEECE66DL) & ((1L << 48) - 1)}</pre>
		''' and clearing the {@code haveNextNextGaussian} flag used by {@link
		''' #nextGaussian}.
		''' 
		''' <p>The implementation of {@code setSeed} by class {@code Random}
		''' happens to use only 48 bits of the given seed. In general, however,
		''' an overriding method may use all 64 bits of the {@code long}
		''' argument as a seed value.
		''' </summary>
		''' <param name="seed"> the initial seed </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property seed As Long
			Set(ByVal seed As Long)
				Me.seed.set(initialScramble(seed))
				haveNextNextGaussian = False
			End Set
		End Property

		''' <summary>
		''' Generates the next pseudorandom number. Subclasses should
		''' override this, as this is used by all other methods.
		''' 
		''' <p>The general contract of {@code next} is that it returns an
		''' {@code int} value and if the argument {@code bits} is between
		''' {@code 1} and {@code 32} (inclusive), then that many low-order
		''' bits of the returned value will be (approximately) independently
		''' chosen bit values, each of which is (approximately) equally
		''' likely to be {@code 0} or {@code 1}. The method {@code next} is
		''' implemented by class {@code Random} by atomically updating the seed to
		'''  <pre>{@code (seed * 0x5DEECE66DL + 0xBL) & ((1L << 48) - 1)}</pre>
		''' and returning
		'''  <pre>{@code (int)(seed >>> (48 - bits))}.</pre>
		''' 
		''' This is a linear congruential pseudorandom number generator, as
		''' defined by D. H. Lehmer and described by Donald E. Knuth in
		''' <i>The Art of Computer Programming,</i> Volume 3:
		''' <i>Seminumerical Algorithms</i>, section 3.2.1.
		''' </summary>
		''' <param name="bits"> random bits </param>
		''' <returns> the next pseudorandom value from this random number
		'''         generator's sequence
		''' @since  1.1 </returns>
		Protected Friend Overridable Function [next](ByVal bits_Renamed As Integer) As Integer
			Dim oldseed, nextseed As Long
			Dim seed_Renamed As java.util.concurrent.atomic.AtomicLong = Me.seed
			Do
				oldseed = seed_Renamed.get()
				nextseed = (oldseed * multiplier + addend) And mask
			Loop While Not seed_Renamed.compareAndSet(oldseed, nextseed)
			Return CInt(CLng(CULng(nextseed) >> (48 - bits_Renamed)))
		End Function

		''' <summary>
		''' Generates random bytes and places them into a user-supplied
		''' byte array.  The number of random bytes produced is equal to
		''' the length of the byte array.
		''' 
		''' <p>The method {@code nextBytes} is implemented by class {@code Random}
		''' as if by:
		'''  <pre> {@code
		''' public  Sub  nextBytes(byte[] bytes) {
		'''   for (int i = 0; i < bytes.length; )
		'''     for (int rnd = nextInt(), n = System.Math.min(bytes.length - i, 4);
		'''          n-- > 0; rnd >>= 8)
		'''       bytes[i++] = (byte)rnd;
		''' }}</pre>
		''' </summary>
		''' <param name="bytes"> the byte array to fill with random bytes </param>
		''' <exception cref="NullPointerException"> if the byte array is null
		''' @since  1.1 </exception>
		Public Overridable Sub nextBytes(ByVal bytes As SByte())
			Dim i As Integer = 0
			Dim len As Integer = bytes.Length
			Do While i < len
				Dim rnd As Integer = nextInt()
				Dim n As Integer = System.Math.Min(len - i,  java.lang.[Integer].SIZE/Byte.SIZE)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While n -= 1 > 0
					bytes(i) = CByte(rnd)
					rnd >>= java.lang.[Byte].SIZE
				Loop
			Loop
					i += 1
		End Sub

		''' <summary>
		''' The form of nextLong used by LongStream Spliterators.  If
		''' origin is greater than bound, acts as unbounded form of
		''' nextLong, else as bounded form.
		''' </summary>
		''' <param name="origin"> the least value, unless greater than bound </param>
		''' <param name="bound"> the upper bound (exclusive), must not equal origin </param>
		''' <returns> a pseudorandom value </returns>
		Friend Function internalNextLong(ByVal origin As Long, ByVal bound As Long) As Long
			Dim r As Long = nextLong()
			If origin < bound Then
				Dim n As Long = bound - origin, m As Long = n - 1
				If (n And m) = 0L Then ' power of two
					r = (r And m) + origin
				ElseIf n > 0L Then ' reject over-represented candidates
					Dim u As Long = CLng(CULng(r) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While u + m - (r = u Mod n) < 0L ' retry -  rejection check -  ensure nonnegative

						u = CInt(CUInt(nextLong()) >> 1)
					Loop
					r += origin
				Else ' range not representable as long
					Do While r < origin OrElse r >= bound
						r = nextLong()
					Loop
				End If
			End If
			Return r
		End Function

		''' <summary>
		''' The form of nextInt used by IntStream Spliterators.
		''' For the unbounded case: uses nextInt().
		''' For the bounded case with representable range: uses nextInt(int bound)
		''' For the bounded case with unrepresentable range: uses nextInt()
		''' </summary>
		''' <param name="origin"> the least value, unless greater than bound </param>
		''' <param name="bound"> the upper bound (exclusive), must not equal origin </param>
		''' <returns> a pseudorandom value </returns>
		Friend Function internalNextInt(ByVal origin As Integer, ByVal bound As Integer) As Integer
			If origin < bound Then
				Dim n As Integer = bound - origin
				If n > 0 Then
					Return nextInt(n) + origin
				Else ' range not representable as int
					Dim r As Integer
					Do
						r = nextInt()
					Loop While r < origin OrElse r >= bound
					Return r
				End If
			Else
				Return nextInt()
			End If
		End Function

		''' <summary>
		''' The form of nextDouble used by DoubleStream Spliterators.
		''' </summary>
		''' <param name="origin"> the least value, unless greater than bound </param>
		''' <param name="bound"> the upper bound (exclusive), must not equal origin </param>
		''' <returns> a pseudorandom value </returns>
		Friend Function internalNextDouble(ByVal origin As Double, ByVal bound As Double) As Double
			Dim r As Double = nextDouble()
			If origin < bound Then
				r = r * (bound - origin) + origin
				If r >= bound Then ' correct for rounding r = java.lang.[Double].longBitsToDouble(Double.doubleToLongBits(bound) - 1)
			End If
			Return r
		End Function

		''' <summary>
		''' Returns the next pseudorandom, uniformly distributed {@code int}
		''' value from this random number generator's sequence. The general
		''' contract of {@code nextInt} is that one {@code int} value is
		''' pseudorandomly generated and returned. All 2<sup>32</sup> possible
		''' {@code int} values are produced with (approximately) equal probability.
		''' 
		''' <p>The method {@code nextInt} is implemented by class {@code Random}
		''' as if by:
		'''  <pre> {@code
		''' public int nextInt() {
		'''   return next(32);
		''' }}</pre>
		''' </summary>
		''' <returns> the next pseudorandom, uniformly distributed {@code int}
		'''         value from this random number generator's sequence </returns>
		Public Overridable Function nextInt() As Integer
			Return [next](32)
		End Function

		''' <summary>
		''' Returns a pseudorandom, uniformly distributed {@code int} value
		''' between 0 (inclusive) and the specified value (exclusive), drawn from
		''' this random number generator's sequence.  The general contract of
		''' {@code nextInt} is that one {@code int} value in the specified range
		''' is pseudorandomly generated and returned.  All {@code bound} possible
		''' {@code int} values are produced with (approximately) equal
		''' probability.  The method {@code nextInt(int bound)} is implemented by
		''' class {@code Random} as if by:
		'''  <pre> {@code
		''' public int nextInt(int bound) {
		'''   if (bound <= 0)
		'''     throw new IllegalArgumentException("bound must be positive");
		''' 
		'''   if ((bound & -bound) == bound)  // i.e., bound is a power of 2
		'''     return (int)((bound * (long)next(31)) >> 31);
		''' 
		'''   int bits, val;
		'''   do {
		'''       bits = next(31);
		'''       val = bits % bound;
		'''   } while (bits - val + (bound-1) < 0);
		'''   return val;
		''' }}</pre>
		''' 
		''' <p>The hedge "approximately" is used in the foregoing description only
		''' because the next method is only approximately an unbiased source of
		''' independently chosen bits.  If it were a perfect source of randomly
		''' chosen bits, then the algorithm shown would choose {@code int}
		''' values from the stated range with perfect uniformity.
		''' <p>
		''' The algorithm is slightly tricky.  It rejects values that would result
		''' in an uneven distribution (due to the fact that 2^31 is not divisible
		''' by n). The probability of a value being rejected depends on n.  The
		''' worst case is n=2^30+1, for which the probability of a reject is 1/2,
		''' and the expected number of iterations before the loop terminates is 2.
		''' <p>
		''' The algorithm treats the case where n is a power of two specially: it
		''' returns the correct number of high-order bits from the underlying
		''' pseudo-random number generator.  In the absence of special treatment,
		''' the correct number of <i>low-order</i> bits would be returned.  Linear
		''' congruential pseudo-random number generators such as the one
		''' implemented by this class are known to have short periods in the
		''' sequence of values of their low-order bits.  Thus, this special case
		''' greatly increases the length of the sequence of values returned by
		''' successive calls to this method if n is a small power of two.
		''' </summary>
		''' <param name="bound"> the upper bound (exclusive).  Must be positive. </param>
		''' <returns> the next pseudorandom, uniformly distributed {@code int}
		'''         value between zero (inclusive) and {@code bound} (exclusive)
		'''         from this random number generator's sequence </returns>
		''' <exception cref="IllegalArgumentException"> if bound is not positive
		''' @since 1.2 </exception>
		Public Overridable Function nextInt(ByVal bound As Integer) As Integer
			If bound <= 0 Then Throw New IllegalArgumentException(BadBound)

			Dim r As Integer = [next](31)
			Dim m As Integer = bound - 1
			If (bound And m) = 0 Then ' i.e., bound is a power of 2
				r = CInt(Fix((bound * CLng(r)) >> 31))
			Else
				Dim u As Integer = r
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While u - (r = u Mod bound) + m < 0

					u = [next](31)
				Loop
			End If
			Return r
		End Function

		''' <summary>
		''' Returns the next pseudorandom, uniformly distributed {@code long}
		''' value from this random number generator's sequence. The general
		''' contract of {@code nextLong} is that one {@code long} value is
		''' pseudorandomly generated and returned.
		''' 
		''' <p>The method {@code nextLong} is implemented by class {@code Random}
		''' as if by:
		'''  <pre> {@code
		''' public long nextLong() {
		'''   return ((long)next(32) << 32) + next(32);
		''' }}</pre>
		''' 
		''' Because class {@code Random} uses a seed with only 48 bits,
		''' this algorithm will not return all possible {@code long} values.
		''' </summary>
		''' <returns> the next pseudorandom, uniformly distributed {@code long}
		'''         value from this random number generator's sequence </returns>
		Public Overridable Function nextLong() As Long
			' it's okay that the bottom word remains signed.
			Return (CLng([next](32)) << 32) + [next](32)
		End Function

		''' <summary>
		''' Returns the next pseudorandom, uniformly distributed
		''' {@code boolean} value from this random number generator's
		''' sequence. The general contract of {@code nextBoolean} is that one
		''' {@code boolean} value is pseudorandomly generated and returned.  The
		''' values {@code true} and {@code false} are produced with
		''' (approximately) equal probability.
		''' 
		''' <p>The method {@code nextBoolean} is implemented by class {@code Random}
		''' as if by:
		'''  <pre> {@code
		''' public boolean nextBoolean() {
		'''   return next(1) != 0;
		''' }}</pre>
		''' </summary>
		''' <returns> the next pseudorandom, uniformly distributed
		'''         {@code boolean} value from this random number generator's
		'''         sequence
		''' @since 1.2 </returns>
		Public Overridable Function nextBoolean() As Boolean
			Return [next](1) <> 0
		End Function

		''' <summary>
		''' Returns the next pseudorandom, uniformly distributed {@code float}
		''' value between {@code 0.0} and {@code 1.0} from this random
		''' number generator's sequence.
		''' 
		''' <p>The general contract of {@code nextFloat} is that one
		''' {@code float} value, chosen (approximately) uniformly from the
		''' range {@code 0.0f} (inclusive) to {@code 1.0f} (exclusive), is
		''' pseudorandomly generated and returned. All 2<sup>24</sup> possible
		''' {@code float} values of the form <i>m&nbsp;x&nbsp;</i>2<sup>-24</sup>,
		''' where <i>m</i> is a positive integer less than 2<sup>24</sup>, are
		''' produced with (approximately) equal probability.
		''' 
		''' <p>The method {@code nextFloat} is implemented by class {@code Random}
		''' as if by:
		'''  <pre> {@code
		''' public float nextFloat() {
		'''   return next(24) / ((float)(1 << 24));
		''' }}</pre>
		''' 
		''' <p>The hedge "approximately" is used in the foregoing description only
		''' because the next method is only approximately an unbiased source of
		''' independently chosen bits. If it were a perfect source of randomly
		''' chosen bits, then the algorithm shown would choose {@code float}
		''' values from the stated range with perfect uniformity.<p>
		''' [In early versions of Java, the result was incorrectly calculated as:
		'''  <pre> {@code
		'''   return next(30) / ((float)(1 << 30));}</pre>
		''' This might seem to be equivalent, if not better, but in fact it
		''' introduced a slight nonuniformity because of the bias in the rounding
		''' of floating-point numbers: it was slightly more likely that the
		''' low-order bit of the significand would be 0 than that it would be 1.]
		''' </summary>
		''' <returns> the next pseudorandom, uniformly distributed {@code float}
		'''         value between {@code 0.0} and {@code 1.0} from this
		'''         random number generator's sequence </returns>
		Public Overridable Function nextFloat() As Single
			Return [next](24) / (CSng(1 << 24))
		End Function

		''' <summary>
		''' Returns the next pseudorandom, uniformly distributed
		''' {@code double} value between {@code 0.0} and
		''' {@code 1.0} from this random number generator's sequence.
		''' 
		''' <p>The general contract of {@code nextDouble} is that one
		''' {@code double} value, chosen (approximately) uniformly from the
		''' range {@code 0.0d} (inclusive) to {@code 1.0d} (exclusive), is
		''' pseudorandomly generated and returned.
		''' 
		''' <p>The method {@code nextDouble} is implemented by class {@code Random}
		''' as if by:
		'''  <pre> {@code
		''' public double nextDouble() {
		'''   return (((long)next(26) << 27) + next(27))
		'''     / (double)(1L << 53);
		''' }}</pre>
		''' 
		''' <p>The hedge "approximately" is used in the foregoing description only
		''' because the {@code next} method is only approximately an unbiased
		''' source of independently chosen bits. If it were a perfect source of
		''' randomly chosen bits, then the algorithm shown would choose
		''' {@code double} values from the stated range with perfect uniformity.
		''' <p>[In early versions of Java, the result was incorrectly calculated as:
		'''  <pre> {@code
		'''   return (((long)next(27) << 27) + next(27))
		'''     / (double)(1L << 54);}</pre>
		''' This might seem to be equivalent, if not better, but in fact it
		''' introduced a large nonuniformity because of the bias in the rounding
		''' of floating-point numbers: it was three times as likely that the
		''' low-order bit of the significand would be 0 than that it would be 1!
		''' This nonuniformity probably doesn't matter much in practice, but we
		''' strive for perfection.]
		''' </summary>
		''' <returns> the next pseudorandom, uniformly distributed {@code double}
		'''         value between {@code 0.0} and {@code 1.0} from this
		'''         random number generator's sequence </returns>
		''' <seealso cref= Math#random </seealso>
		Public Overridable Function nextDouble() As Double
			Return ((CLng([next](26)) << 27) + [next](27)) * DOUBLE_UNIT
		End Function

		Private nextNextGaussian As Double
		Private haveNextNextGaussian As Boolean = False

		''' <summary>
		''' Returns the next pseudorandom, Gaussian ("normally") distributed
		''' {@code double} value with mean {@code 0.0} and standard
		''' deviation {@code 1.0} from this random number generator's sequence.
		''' <p>
		''' The general contract of {@code nextGaussian} is that one
		''' {@code double} value, chosen from (approximately) the usual
		''' normal distribution with mean {@code 0.0} and standard deviation
		''' {@code 1.0}, is pseudorandomly generated and returned.
		''' 
		''' <p>The method {@code nextGaussian} is implemented by class
		''' {@code Random} as if by a threadsafe version of the following:
		'''  <pre> {@code
		''' private double nextNextGaussian;
		''' private boolean haveNextNextGaussian = false;
		''' 
		''' public double nextGaussian() {
		'''   if (haveNextNextGaussian) {
		'''     haveNextNextGaussian = false;
		'''     return nextNextGaussian;
		'''   } else {
		'''     double v1, v2, s;
		'''     do {
		'''       v1 = 2 * nextDouble() - 1;   // between -1.0 and 1.0
		'''       v2 = 2 * nextDouble() - 1;   // between -1.0 and 1.0
		'''       s = v1 * v1 + v2 * v2;
		'''     } while (s >= 1 || s == 0);
		'''     double multiplier = StrictMath.sqrt(-2 * StrictMath.log(s)/s);
		'''     nextNextGaussian = v2 * multiplier;
		'''     haveNextNextGaussian = true;
		'''     return v1 * multiplier;
		'''   }
		''' }}</pre>
		''' This uses the <i>polar method</i> of G. E. P. Box, M. E. Muller, and
		''' G. Marsaglia, as described by Donald E. Knuth in <i>The Art of
		''' Computer Programming</i>, Volume 3: <i>Seminumerical Algorithms</i>,
		''' section 3.4.1, subsection C, algorithm P. Note that it generates two
		''' independent values at the cost of only one call to {@code StrictMath.log}
		''' and one call to {@code StrictMath.sqrt}.
		''' </summary>
		''' <returns> the next pseudorandom, Gaussian ("normally") distributed
		'''         {@code double} value with mean {@code 0.0} and
		'''         standard deviation {@code 1.0} from this random number
		'''         generator's sequence </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function nextGaussian() As Double
			' See Knuth, ACP, Section 3.4.1 Algorithm C.
			If haveNextNextGaussian Then
				haveNextNextGaussian = False
				Return nextNextGaussian
			Else
				Dim v1, v2, s As Double
				Do
					v1 = 2 * nextDouble() - 1 ' between -1 and 1
					v2 = 2 * nextDouble() - 1 ' between -1 and 1
					s = v1 * v1 + v2 * v2
				Loop While s >= 1 OrElse s = 0
				Dim multiplier As Double = System.Math.Sqrt(-2 * System.Math.Log(s)/s)
				nextNextGaussian = v2 * multiplier
				haveNextNextGaussian = True
				Return v1 * multiplier
			End If
		End Function

		' stream methods, coded in a way intended to better isolate for
		' maintenance purposes the small differences across forms.

		''' <summary>
		''' Returns a stream producing the given {@code streamSize} number of
		''' pseudorandom {@code int} values.
		''' 
		''' <p>A pseudorandom {@code int} value is generated as if it's the result of
		''' calling the method <seealso cref="#nextInt()"/>.
		''' </summary>
		''' <param name="streamSize"> the number of values to generate </param>
		''' <returns> a stream of pseudorandom {@code int} values </returns>
		''' <exception cref="IllegalArgumentException"> if {@code streamSize} is
		'''         less than zero
		''' @since 1.8 </exception>
		Public Overridable Function ints(ByVal streamSize As Long) As java.util.stream.IntStream
			If streamSize < 0L Then Throw New IllegalArgumentException(BadSize)
			Return java.util.stream.StreamSupport.intStream(New RandomIntsSpliterator(Me, 0L, streamSize,  java.lang.[Integer].Max_Value, 0), False)
		End Function

		''' <summary>
		''' Returns an effectively unlimited stream of pseudorandom {@code int}
		''' values.
		''' 
		''' <p>A pseudorandom {@code int} value is generated as if it's the result of
		''' calling the method <seealso cref="#nextInt()"/>.
		''' 
		''' @implNote This method is implemented to be equivalent to {@code
		''' ints(Long.MAX_VALUE)}.
		''' </summary>
		''' <returns> a stream of pseudorandom {@code int} values
		''' @since 1.8 </returns>
		Public Overridable Function ints() As java.util.stream.IntStream
			Return java.util.stream.StreamSupport.intStream(New RandomIntsSpliterator(Me, 0L, java.lang.[Long].Max_Value,  java.lang.[Integer].Max_Value, 0), False)
		End Function

		''' <summary>
		''' Returns a stream producing the given {@code streamSize} number
		''' of pseudorandom {@code int} values, each conforming to the given
		''' origin (inclusive) and bound (exclusive).
		''' 
		''' <p>A pseudorandom {@code int} value is generated as if it's the result of
		''' calling the following method with the origin and bound:
		''' <pre> {@code
		''' int nextInt(int origin, int bound) {
		'''   int n = bound - origin;
		'''   if (n > 0) {
		'''     return nextInt(n) + origin;
		'''   }
		'''   else {  // range not representable as int
		'''     int r;
		'''     do {
		'''       r = nextInt();
		'''     } while (r < origin || r >= bound);
		'''     return r;
		'''   }
		''' }}</pre>
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
		Public Overridable Function ints(ByVal streamSize As Long, ByVal randomNumberOrigin As Integer, ByVal randomNumberBound As Integer) As java.util.stream.IntStream
			If streamSize < 0L Then Throw New IllegalArgumentException(BadSize)
			If randomNumberOrigin >= randomNumberBound Then Throw New IllegalArgumentException(BadRange)
			Return java.util.stream.StreamSupport.intStream(New RandomIntsSpliterator(Me, 0L, streamSize, randomNumberOrigin, randomNumberBound), False)
		End Function

		''' <summary>
		''' Returns an effectively unlimited stream of pseudorandom {@code
		''' int} values, each conforming to the given origin (inclusive) and bound
		''' (exclusive).
		''' 
		''' <p>A pseudorandom {@code int} value is generated as if it's the result of
		''' calling the following method with the origin and bound:
		''' <pre> {@code
		''' int nextInt(int origin, int bound) {
		'''   int n = bound - origin;
		'''   if (n > 0) {
		'''     return nextInt(n) + origin;
		'''   }
		'''   else {  // range not representable as int
		'''     int r;
		'''     do {
		'''       r = nextInt();
		'''     } while (r < origin || r >= bound);
		'''     return r;
		'''   }
		''' }}</pre>
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
		Public Overridable Function ints(ByVal randomNumberOrigin As Integer, ByVal randomNumberBound As Integer) As java.util.stream.IntStream
			If randomNumberOrigin >= randomNumberBound Then Throw New IllegalArgumentException(BadRange)
			Return java.util.stream.StreamSupport.intStream(New RandomIntsSpliterator(Me, 0L, java.lang.[Long].Max_Value, randomNumberOrigin, randomNumberBound), False)
		End Function

		''' <summary>
		''' Returns a stream producing the given {@code streamSize} number of
		''' pseudorandom {@code long} values.
		''' 
		''' <p>A pseudorandom {@code long} value is generated as if it's the result
		''' of calling the method <seealso cref="#nextLong()"/>.
		''' </summary>
		''' <param name="streamSize"> the number of values to generate </param>
		''' <returns> a stream of pseudorandom {@code long} values </returns>
		''' <exception cref="IllegalArgumentException"> if {@code streamSize} is
		'''         less than zero
		''' @since 1.8 </exception>
		Public Overridable Function longs(ByVal streamSize As Long) As java.util.stream.LongStream
			If streamSize < 0L Then Throw New IllegalArgumentException(BadSize)
			Return java.util.stream.StreamSupport.longStream(New RandomLongsSpliterator(Me, 0L, streamSize, java.lang.[Long].Max_Value, 0L), False)
		End Function

		''' <summary>
		''' Returns an effectively unlimited stream of pseudorandom {@code long}
		''' values.
		''' 
		''' <p>A pseudorandom {@code long} value is generated as if it's the result
		''' of calling the method <seealso cref="#nextLong()"/>.
		''' 
		''' @implNote This method is implemented to be equivalent to {@code
		''' longs(Long.MAX_VALUE)}.
		''' </summary>
		''' <returns> a stream of pseudorandom {@code long} values
		''' @since 1.8 </returns>
		Public Overridable Function longs() As java.util.stream.LongStream
			Return java.util.stream.StreamSupport.longStream(New RandomLongsSpliterator(Me, 0L, java.lang.[Long].Max_Value, java.lang.[Long].Max_Value, 0L), False)
		End Function

		''' <summary>
		''' Returns a stream producing the given {@code streamSize} number of
		''' pseudorandom {@code long}, each conforming to the given origin
		''' (inclusive) and bound (exclusive).
		''' 
		''' <p>A pseudorandom {@code long} value is generated as if it's the result
		''' of calling the following method with the origin and bound:
		''' <pre> {@code
		''' long nextLong(long origin, long bound) {
		'''   long r = nextLong();
		'''   long n = bound - origin, m = n - 1;
		'''   if ((n & m) == 0L)  // power of two
		'''     r = (r & m) + origin;
		'''   else if (n > 0L) {  // reject over-represented candidates
		'''     for (long u = r >>> 1;            // ensure nonnegative
		'''          u + m - (r = u % n) < 0L;    // rejection check
		'''          u = nextLong() >>> 1) // retry
		'''         ;
		'''     r += origin;
		'''   }
		'''   else {              // range not representable as long
		'''     while (r < origin || r >= bound)
		'''       r = nextLong();
		'''   }
		'''   return r;
		''' }}</pre>
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
		Public Overridable Function longs(ByVal streamSize As Long, ByVal randomNumberOrigin As Long, ByVal randomNumberBound As Long) As java.util.stream.LongStream
			If streamSize < 0L Then Throw New IllegalArgumentException(BadSize)
			If randomNumberOrigin >= randomNumberBound Then Throw New IllegalArgumentException(BadRange)
			Return java.util.stream.StreamSupport.longStream(New RandomLongsSpliterator(Me, 0L, streamSize, randomNumberOrigin, randomNumberBound), False)
		End Function

		''' <summary>
		''' Returns an effectively unlimited stream of pseudorandom {@code
		''' long} values, each conforming to the given origin (inclusive) and bound
		''' (exclusive).
		''' 
		''' <p>A pseudorandom {@code long} value is generated as if it's the result
		''' of calling the following method with the origin and bound:
		''' <pre> {@code
		''' long nextLong(long origin, long bound) {
		'''   long r = nextLong();
		'''   long n = bound - origin, m = n - 1;
		'''   if ((n & m) == 0L)  // power of two
		'''     r = (r & m) + origin;
		'''   else if (n > 0L) {  // reject over-represented candidates
		'''     for (long u = r >>> 1;            // ensure nonnegative
		'''          u + m - (r = u % n) < 0L;    // rejection check
		'''          u = nextLong() >>> 1) // retry
		'''         ;
		'''     r += origin;
		'''   }
		'''   else {              // range not representable as long
		'''     while (r < origin || r >= bound)
		'''       r = nextLong();
		'''   }
		'''   return r;
		''' }}</pre>
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
		Public Overridable Function longs(ByVal randomNumberOrigin As Long, ByVal randomNumberBound As Long) As java.util.stream.LongStream
			If randomNumberOrigin >= randomNumberBound Then Throw New IllegalArgumentException(BadRange)
			Return java.util.stream.StreamSupport.longStream(New RandomLongsSpliterator(Me, 0L, java.lang.[Long].Max_Value, randomNumberOrigin, randomNumberBound), False)
		End Function

		''' <summary>
		''' Returns a stream producing the given {@code streamSize} number of
		''' pseudorandom {@code double} values, each between zero
		''' (inclusive) and one (exclusive).
		''' 
		''' <p>A pseudorandom {@code double} value is generated as if it's the result
		''' of calling the method <seealso cref="#nextDouble()"/>.
		''' </summary>
		''' <param name="streamSize"> the number of values to generate </param>
		''' <returns> a stream of {@code double} values </returns>
		''' <exception cref="IllegalArgumentException"> if {@code streamSize} is
		'''         less than zero
		''' @since 1.8 </exception>
		Public Overridable Function doubles(ByVal streamSize As Long) As java.util.stream.DoubleStream
			If streamSize < 0L Then Throw New IllegalArgumentException(BadSize)
			Return java.util.stream.StreamSupport.doubleStream(New RandomDoublesSpliterator(Me, 0L, streamSize, java.lang.[Double].Max_Value, 0.0), False)
		End Function

		''' <summary>
		''' Returns an effectively unlimited stream of pseudorandom {@code
		''' double} values, each between zero (inclusive) and one
		''' (exclusive).
		''' 
		''' <p>A pseudorandom {@code double} value is generated as if it's the result
		''' of calling the method <seealso cref="#nextDouble()"/>.
		''' 
		''' @implNote This method is implemented to be equivalent to {@code
		''' doubles(Long.MAX_VALUE)}.
		''' </summary>
		''' <returns> a stream of pseudorandom {@code double} values
		''' @since 1.8 </returns>
		Public Overridable Function doubles() As java.util.stream.DoubleStream
			Return java.util.stream.StreamSupport.doubleStream(New RandomDoublesSpliterator(Me, 0L, java.lang.[Long].Max_Value, java.lang.[Double].Max_Value, 0.0), False)
		End Function

		''' <summary>
		''' Returns a stream producing the given {@code streamSize} number of
		''' pseudorandom {@code double} values, each conforming to the given origin
		''' (inclusive) and bound (exclusive).
		''' 
		''' <p>A pseudorandom {@code double} value is generated as if it's the result
		''' of calling the following method with the origin and bound:
		''' <pre> {@code
		''' double nextDouble(double origin, double bound) {
		'''   double r = nextDouble();
		'''   r = r * (bound - origin) + origin;
		'''   if (r >= bound) // correct for rounding
		'''     r = System.Math.nextDown(bound);
		'''   return r;
		''' }}</pre>
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
		Public Overridable Function doubles(ByVal streamSize As Long, ByVal randomNumberOrigin As Double, ByVal randomNumberBound As Double) As java.util.stream.DoubleStream
			If streamSize < 0L Then Throw New IllegalArgumentException(BadSize)
			If Not(randomNumberOrigin < randomNumberBound) Then Throw New IllegalArgumentException(BadRange)
			Return java.util.stream.StreamSupport.doubleStream(New RandomDoublesSpliterator(Me, 0L, streamSize, randomNumberOrigin, randomNumberBound), False)
		End Function

		''' <summary>
		''' Returns an effectively unlimited stream of pseudorandom {@code
		''' double} values, each conforming to the given origin (inclusive) and bound
		''' (exclusive).
		''' 
		''' <p>A pseudorandom {@code double} value is generated as if it's the result
		''' of calling the following method with the origin and bound:
		''' <pre> {@code
		''' double nextDouble(double origin, double bound) {
		'''   double r = nextDouble();
		'''   r = r * (bound - origin) + origin;
		'''   if (r >= bound) // correct for rounding
		'''     r = System.Math.nextDown(bound);
		'''   return r;
		''' }}</pre>
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
		Public Overridable Function doubles(ByVal randomNumberOrigin As Double, ByVal randomNumberBound As Double) As java.util.stream.DoubleStream
			If Not(randomNumberOrigin < randomNumberBound) Then Throw New IllegalArgumentException(BadRange)
			Return java.util.stream.StreamSupport.doubleStream(New RandomDoublesSpliterator(Me, 0L, java.lang.[Long].Max_Value, randomNumberOrigin, randomNumberBound), False)
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
			Implements Spliterator.OfInt

			Friend ReadOnly rng As Random
			Friend index As Long
			Friend ReadOnly fence As Long
			Friend ReadOnly origin As Integer
			Friend ReadOnly bound As Integer
			Friend Sub New(ByVal rng As Random, ByVal index As Long, ByVal fence As Long, ByVal origin As Integer, ByVal bound As Integer)
				Me.rng = rng
				Me.index = index
				Me.fence = fence
				Me.origin = origin
				Me.bound = bound
			End Sub

			Public Function trySplit() As RandomIntsSpliterator
				Dim i As Long = index, m As Long = CInt(CUInt((i + fence)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(m <= i, Nothing, New RandomIntsSpliterator(rng, i, index = m, origin, bound))
			End Function

			Public Function estimateSize() As Long
				Return fence - index
			End Function

			Public Function characteristics() As Integer
				Return (Spliterator.SIZED Or Spliterator.SUBSIZED Or Spliterator.NONNULL Or Spliterator.IMMUTABLE)
			End Function

			Public Function tryAdvance(ByVal consumer As java.util.function.IntConsumer) As Boolean
				If consumer Is Nothing Then Throw New NullPointerException
				Dim i As Long = index, f As Long = fence
				If i < f Then
					consumer.accept(rng.internalNextInt(origin, bound))
					index = i + 1
					Return True
				End If
				Return False
			End Function

			Public Sub forEachRemaining(ByVal consumer As java.util.function.IntConsumer)
				If consumer Is Nothing Then Throw New NullPointerException
				Dim i As Long = index, f As Long = fence
				If i < f Then
					index = f
					Dim r As Random = rng
					Dim o As Integer = origin, b As Integer = bound
					Do
						consumer.accept(r.internalNextInt(o, b))
						i += 1
					Loop While i < f
				End If
			End Sub
		End Class

		''' <summary>
		''' Spliterator for long streams.
		''' </summary>
		Friend NotInheritable Class RandomLongsSpliterator
			Implements Spliterator.OfLong

			Friend ReadOnly rng As Random
			Friend index As Long
			Friend ReadOnly fence As Long
			Friend ReadOnly origin As Long
			Friend ReadOnly bound As Long
			Friend Sub New(ByVal rng As Random, ByVal index As Long, ByVal fence As Long, ByVal origin As Long, ByVal bound As Long)
				Me.rng = rng
				Me.index = index
				Me.fence = fence
				Me.origin = origin
				Me.bound = bound
			End Sub

			Public Function trySplit() As RandomLongsSpliterator
				Dim i As Long = index, m As Long = CInt(CUInt((i + fence)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(m <= i, Nothing, New RandomLongsSpliterator(rng, i, index = m, origin, bound))
			End Function

			Public Function estimateSize() As Long
				Return fence - index
			End Function

			Public Function characteristics() As Integer
				Return (Spliterator.SIZED Or Spliterator.SUBSIZED Or Spliterator.NONNULL Or Spliterator.IMMUTABLE)
			End Function

			Public Function tryAdvance(ByVal consumer As java.util.function.LongConsumer) As Boolean
				If consumer Is Nothing Then Throw New NullPointerException
				Dim i As Long = index, f As Long = fence
				If i < f Then
					consumer.accept(rng.internalNextLong(origin, bound))
					index = i + 1
					Return True
				End If
				Return False
			End Function

			Public Sub forEachRemaining(ByVal consumer As java.util.function.LongConsumer)
				If consumer Is Nothing Then Throw New NullPointerException
				Dim i As Long = index, f As Long = fence
				If i < f Then
					index = f
					Dim r As Random = rng
					Dim o As Long = origin, b As Long = bound
					Do
						consumer.accept(r.internalNextLong(o, b))
						i += 1
					Loop While i < f
				End If
			End Sub

		End Class

		''' <summary>
		''' Spliterator for double streams.
		''' </summary>
		Friend NotInheritable Class RandomDoublesSpliterator
			Implements Spliterator.OfDouble

			Friend ReadOnly rng As Random
			Friend index As Long
			Friend ReadOnly fence As Long
			Friend ReadOnly origin As Double
			Friend ReadOnly bound As Double
			Friend Sub New(ByVal rng As Random, ByVal index As Long, ByVal fence As Long, ByVal origin As Double, ByVal bound As Double)
				Me.rng = rng
				Me.index = index
				Me.fence = fence
				Me.origin = origin
				Me.bound = bound
			End Sub

			Public Function trySplit() As RandomDoublesSpliterator
				Dim i As Long = index, m As Long = CInt(CUInt((i + fence)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(m <= i, Nothing, New RandomDoublesSpliterator(rng, i, index = m, origin, bound))
			End Function

			Public Function estimateSize() As Long
				Return fence - index
			End Function

			Public Function characteristics() As Integer
				Return (Spliterator.SIZED Or Spliterator.SUBSIZED Or Spliterator.NONNULL Or Spliterator.IMMUTABLE)
			End Function

			Public Function tryAdvance(ByVal consumer As java.util.function.DoubleConsumer) As Boolean
				If consumer Is Nothing Then Throw New NullPointerException
				Dim i As Long = index, f As Long = fence
				If i < f Then
					consumer.accept(rng.internalNextDouble(origin, bound))
					index = i + 1
					Return True
				End If
				Return False
			End Function

			Public Sub forEachRemaining(ByVal consumer As java.util.function.DoubleConsumer)
				If consumer Is Nothing Then Throw New NullPointerException
				Dim i As Long = index, f As Long = fence
				If i < f Then
					index = f
					Dim r As Random = rng
					Dim o As Double = origin, b As Double = bound
					Do
						consumer.accept(r.internalNextDouble(o, b))
						i += 1
					Loop While i < f
				End If
			End Sub
		End Class

		''' <summary>
		''' Serializable fields for Random.
		''' 
		''' @serialField    seed long
		'''              seed for random computations
		''' @serialField    nextNextGaussian double
		'''              next Gaussian to be returned
		''' @serialField      haveNextNextGaussian boolean
		'''              nextNextGaussian is valid
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As ObjectStreamField() = { New ObjectStreamField("seed", java.lang.[Long].TYPE), New ObjectStreamField("nextNextGaussian", java.lang.[Double].TYPE), New ObjectStreamField("haveNextNextGaussian",  java.lang.[Boolean].TYPE) }

		''' <summary>
		''' Reconstitute the {@code Random} instance from a stream (that is,
		''' deserialize it).
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)

			Dim fields As ObjectInputStream.GetField = s.readFields()

			' The seed is read in as {@code long} for
			' historical reasons, but it is converted to an AtomicLong.
			Dim seedVal As Long = fields.get("seed", -1L)
			If seedVal < 0 Then Throw New java.io.StreamCorruptedException("Random: invalid seed")
			resetSeed(seedVal)
			nextNextGaussian = fields.get("nextNextGaussian", 0.0)
			haveNextNextGaussian = fields.get("haveNextNextGaussian", False)
		End Sub

		''' <summary>
		''' Save the {@code Random} instance to a stream.
		''' </summary>
		SyncLock private  Sub  writeObject ObjectOutputStream s
			Dim IOException As throws

			' set the values of the Serializable fields
			Dim fields As ObjectOutputStream.PutField = s.putFields()

			' The seed is serialized as a long for historical reasons.
			fields.put("seed", seed.get())
			fields.put("nextNextGaussian", nextNextGaussian)
			fields.put("haveNextNextGaussian", haveNextNextGaussian)

			' save them
			s.writeFields()
		End SyncLock

		' Support for resetting seed while deserializing
		private static final sun.misc.Unsafe unsafe = sun.misc.Unsafe.unsafe
		private static final Long seedOffset
		static Random()
			Try
				seedOffset = unsafe.objectFieldOffset(GetType(Random).getDeclaredField("seed"))
			Catch ex As Exception
				Throw New [Error](ex)
			End Try
		private  Sub  resetSeed(Long seedVal)
			unsafe.putObjectVolatile(Me, seedOffset, New java.util.concurrent.atomic.AtomicLong(seedVal))
	End Class

End Namespace
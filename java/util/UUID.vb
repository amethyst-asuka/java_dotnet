Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics

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

Namespace java.util


	''' <summary>
	''' A class that represents an immutable universally unique identifier (UUID).
	''' A UUID represents a 128-bit value.
	''' 
	''' <p> There exist different variants of these global identifiers.  The methods
	''' of this class are for manipulating the Leach-Salz variant, although the
	''' constructors allow the creation of any variant of UUID (described below).
	''' 
	''' <p> The layout of a variant 2 (Leach-Salz) UUID is as follows:
	''' 
	''' The most significant long consists of the following unsigned fields:
	''' <pre>
	''' 0xFFFFFFFF00000000 time_low
	''' 0x00000000FFFF0000 time_mid
	''' 0x000000000000F000 version
	''' 0x0000000000000FFF time_hi
	''' </pre>
	''' The least significant long consists of the following unsigned fields:
	''' <pre>
	''' 0xC000000000000000 variant
	''' 0x3FFF000000000000 clock_seq
	''' 0x0000FFFFFFFFFFFF node
	''' </pre>
	''' 
	''' <p> The variant field contains a value which identifies the layout of the
	''' {@code UUID}.  The bit layout described above is valid only for a {@code
	''' UUID} with a variant value of 2, which indicates the Leach-Salz variant.
	''' 
	''' <p> The version field holds a value that describes the type of this {@code
	''' UUID}.  There are four different basic types of UUIDs: time-based, DCE
	''' security, name-based, and randomly generated UUIDs.  These types have a
	''' version value of 1, 2, 3 and 4, respectively.
	''' 
	''' <p> For more information including algorithms used to create {@code UUID}s,
	''' see <a href="http://www.ietf.org/rfc/rfc4122.txt"> <i>RFC&nbsp;4122: A
	''' Universally Unique IDentifier (UUID) URN Namespace</i></a>, section 4.2
	''' &quot;Algorithms for Creating a Time-Based UUID&quot;.
	''' 
	''' @since   1.5
	''' </summary>
	<Serializable> _
	Public NotInheritable Class UUID
		Implements Comparable(Of UUID)

		''' <summary>
		''' Explicit serialVersionUID for interoperability.
		''' </summary>
		Private Const serialVersionUID As Long = -4856846361193249489L

	'    
	'     * The most significant 64 bits of this UUID.
	'     *
	'     * @serial
	'     
		Private ReadOnly mostSigBits As Long

	'    
	'     * The least significant 64 bits of this UUID.
	'     *
	'     * @serial
	'     
		Private ReadOnly leastSigBits As Long

	'    
	'     * The random number generator used by this class to create random
	'     * based UUIDs. In a holder class to defer initialization until needed.
	'     
		Private Class Holder
			Friend Shared ReadOnly numberGenerator As New SecureRandom
		End Class

		' Constructors and Factories

	'    
	'     * Private constructor which uses a byte array to construct the new UUID.
	'     
		Private Sub New(ByVal data As SByte())
			Dim msb As Long = 0
			Dim lsb As Long = 0
			Debug.Assert(data.Length = 16, "data must be 16 bytes in length")
			For i As Integer = 0 To 7
				msb = (msb << 8) Or (data(i) And &Hff)
			Next i
			For i As Integer = 8 To 15
				lsb = (lsb << 8) Or (data(i) And &Hff)
			Next i
			Me.mostSigBits = msb
			Me.leastSigBits = lsb
		End Sub

		''' <summary>
		''' Constructs a new {@code UUID} using the specified data.  {@code
		''' mostSigBits} is used for the most significant 64 bits of the {@code
		''' UUID} and {@code leastSigBits} becomes the least significant 64 bits of
		''' the {@code UUID}.
		''' </summary>
		''' <param name="mostSigBits">
		'''         The most significant bits of the {@code UUID}
		''' </param>
		''' <param name="leastSigBits">
		'''         The least significant bits of the {@code UUID} </param>
		Public Sub New(ByVal mostSigBits As Long, ByVal leastSigBits As Long)
			Me.mostSigBits = mostSigBits
			Me.leastSigBits = leastSigBits
		End Sub

		''' <summary>
		''' Static factory to retrieve a type 4 (pseudo randomly generated) UUID.
		''' 
		''' The {@code UUID} is generated using a cryptographically strong pseudo
		''' random number generator.
		''' </summary>
		''' <returns>  A randomly generated {@code UUID} </returns>
		Public Shared Function randomUUID() As UUID
			Dim ng As SecureRandom = Holder.numberGenerator

			Dim randomBytes As SByte() = New SByte(15){}
			ng.nextBytes(randomBytes)
			randomBytes(6) = randomBytes(6) And &Hf ' clear version
			randomBytes(6) = randomBytes(6) Or &H40 ' set to version 4
			randomBytes(8) = randomBytes(8) And &H3f ' clear variant
			randomBytes(8) = randomBytes(8) Or &H80 ' set to IETF variant
			Return New UUID(randomBytes)
		End Function

		''' <summary>
		''' Static factory to retrieve a type 3 (name based) {@code UUID} based on
		''' the specified byte array.
		''' </summary>
		''' <param name="name">
		'''         A byte array to be used to construct a {@code UUID}
		''' </param>
		''' <returns>  A {@code UUID} generated from the specified array </returns>
		Public Shared Function nameUUIDFromBytes(ByVal name As SByte()) As UUID
			Dim md As MessageDigest
			Try
				md = MessageDigest.getInstance("MD5")
			Catch nsae As NoSuchAlgorithmException
				Throw New InternalError("MD5 not supported", nsae)
			End Try
			Dim md5Bytes As SByte() = md.digest(name)
			md5Bytes(6) = md5Bytes(6) And &Hf ' clear version
			md5Bytes(6) = md5Bytes(6) Or &H30 ' set to version 3
			md5Bytes(8) = md5Bytes(8) And &H3f ' clear variant
			md5Bytes(8) = md5Bytes(8) Or &H80 ' set to IETF variant
			Return New UUID(md5Bytes)
		End Function

		''' <summary>
		''' Creates a {@code UUID} from the string standard representation as
		''' described in the <seealso cref="#toString"/> method.
		''' </summary>
		''' <param name="name">
		'''         A string that specifies a {@code UUID}
		''' </param>
		''' <returns>  A {@code UUID} with the specified value
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If name does not conform to the string representation as
		'''          described in <seealso cref="#toString"/>
		'''  </exception>
		Public Shared Function fromString(ByVal name As String) As UUID
			Dim components As String() = name.Split("-")
			If components.Length <> 5 Then Throw New IllegalArgumentException("Invalid UUID string: " & name)
			For i As Integer = 0 To 4
				components(i) = "0x" & components(i)
			Next i

			Dim mostSigBits As Long = Long.decode(components(0))
			mostSigBits <<= 16
			mostSigBits = mostSigBits Or Long.decode(components(1))
			mostSigBits <<= 16
			mostSigBits = mostSigBits Or Long.decode(components(2))

			Dim leastSigBits As Long = Long.decode(components(3))
			leastSigBits <<= 48
			leastSigBits = leastSigBits Or Long.decode(components(4))

			Return New UUID(mostSigBits, leastSigBits)
		End Function

		' Field Accessor Methods

		''' <summary>
		''' Returns the least significant 64 bits of this UUID's 128 bit value.
		''' </summary>
		''' <returns>  The least significant 64 bits of this UUID's 128 bit value </returns>
		Public Property leastSignificantBits As Long
			Get
				Return leastSigBits
			End Get
		End Property

		''' <summary>
		''' Returns the most significant 64 bits of this UUID's 128 bit value.
		''' </summary>
		''' <returns>  The most significant 64 bits of this UUID's 128 bit value </returns>
		Public Property mostSignificantBits As Long
			Get
				Return mostSigBits
			End Get
		End Property

		''' <summary>
		''' The version number associated with this {@code UUID}.  The version
		''' number describes how this {@code UUID} was generated.
		''' 
		''' The version number has the following meaning:
		''' <ul>
		''' <li>1    Time-based UUID
		''' <li>2    DCE security UUID
		''' <li>3    Name-based UUID
		''' <li>4    Randomly generated UUID
		''' </ul>
		''' </summary>
		''' <returns>  The version number of this {@code UUID} </returns>
		Public Function version() As Integer
			' Version is bits masked by 0x000000000000F000 in MS long
			Return CInt(Fix((mostSigBits >> 12) And &Hf))
		End Function

		''' <summary>
		''' The variant number associated with this {@code UUID}.  The variant
		''' number describes the layout of the {@code UUID}.
		''' 
		''' The variant number has the following meaning:
		''' <ul>
		''' <li>0    Reserved for NCS backward compatibility
		''' <li>2    <a href="http://www.ietf.org/rfc/rfc4122.txt">IETF&nbsp;RFC&nbsp;4122</a>
		''' (Leach-Salz), used by this class
		''' <li>6    Reserved, Microsoft Corporation backward compatibility
		''' <li>7    Reserved for future definition
		''' </ul>
		''' </summary>
		''' <returns>  The variant number of this {@code UUID} </returns>
		Public Function [variant]() As Integer
			' This field is composed of a varying number of bits.
			' 0    -    -    Reserved for NCS backward compatibility
			' 1    0    -    The IETF aka Leach-Salz variant (used by this [Class])
			' 1    1    0    Reserved, Microsoft backward compatibility
			' 1    1    1    Reserved for future definition.
			Return CInt(Fix((CLng(CULng(leastSigBits) >> (64 - (CLng(CULng(leastSigBits) >> 62))))) And (leastSigBits >> 63)))
		End Function

		''' <summary>
		''' The timestamp value associated with this UUID.
		''' 
		''' <p> The 60 bit timestamp value is constructed from the time_low,
		''' time_mid, and time_hi fields of this {@code UUID}.  The resulting
		''' timestamp is measured in 100-nanosecond units since midnight,
		''' October 15, 1582 UTC.
		''' 
		''' <p> The timestamp value is only meaningful in a time-based UUID, which
		''' has version type 1.  If this {@code UUID} is not a time-based UUID then
		''' this method throws UnsupportedOperationException.
		''' </summary>
		''' <exception cref="UnsupportedOperationException">
		'''         If this UUID is not a version 1 UUID </exception>
		''' <returns> The timestamp of this {@code UUID}. </returns>
		Public Function timestamp() As Long
			If version() <> 1 Then Throw New UnsupportedOperationException("Not a time-based UUID")

			Return (mostSigBits And &HFFFL) << 48 Or ((mostSigBits >> 16) And &HFFFFL) << 32 Or CLng(CULng(mostSigBits) >> 32)
		End Function

		''' <summary>
		''' The clock sequence value associated with this UUID.
		''' 
		''' <p> The 14 bit clock sequence value is constructed from the clock
		''' sequence field of this UUID.  The clock sequence field is used to
		''' guarantee temporal uniqueness in a time-based UUID.
		''' 
		''' <p> The {@code clockSequence} value is only meaningful in a time-based
		''' UUID, which has version type 1.  If this UUID is not a time-based UUID
		''' then this method throws UnsupportedOperationException.
		''' </summary>
		''' <returns>  The clock sequence of this {@code UUID}
		''' </returns>
		''' <exception cref="UnsupportedOperationException">
		'''          If this UUID is not a version 1 UUID </exception>
		Public Function clockSequence() As Integer
			If version() <> 1 Then Throw New UnsupportedOperationException("Not a time-based UUID")

			Return CInt(CInt(CUInt((leastSigBits And &H3FFF000000000000L)) >> 48))
		End Function

		''' <summary>
		''' The node value associated with this UUID.
		''' 
		''' <p> The 48 bit node value is constructed from the node field of this
		''' UUID.  This field is intended to hold the IEEE 802 address of the machine
		''' that generated this UUID to guarantee spatial uniqueness.
		''' 
		''' <p> The node value is only meaningful in a time-based UUID, which has
		''' version type 1.  If this UUID is not a time-based UUID then this method
		''' throws UnsupportedOperationException.
		''' </summary>
		''' <returns>  The node value of this {@code UUID}
		''' </returns>
		''' <exception cref="UnsupportedOperationException">
		'''          If this UUID is not a version 1 UUID </exception>
		Public Function node() As Long
			If version() <> 1 Then Throw New UnsupportedOperationException("Not a time-based UUID")

			Return leastSigBits And &HFFFFFFFFFFFFL
		End Function

		' Object Inherited Methods

		''' <summary>
		''' Returns a {@code String} object representing this {@code UUID}.
		''' 
		''' <p> The UUID string representation is as described by this BNF:
		''' <blockquote><pre>
		''' {@code
		''' UUID                   = <time_low> "-" <time_mid> "-"
		'''                          <time_high_and_version> "-"
		'''                          <variant_and_sequence> "-"
		'''                          <node>
		''' time_low               = 4*<hexOctet>
		''' time_mid               = 2*<hexOctet>
		''' time_high_and_version  = 2*<hexOctet>
		''' variant_and_sequence   = 2*<hexOctet>
		''' node                   = 6*<hexOctet>
		''' hexOctet               = <hexDigit><hexDigit>
		''' hexDigit               =
		'''       "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
		'''       | "a" | "b" | "c" | "d" | "e" | "f"
		'''       | "A" | "B" | "C" | "D" | "E" | "F"
		''' }</pre></blockquote>
		''' </summary>
		''' <returns>  A string representation of this {@code UUID} </returns>
		Public Overrides Function ToString() As String
			Return (digits(mostSigBits >> 32, 8) & "-" & digits(mostSigBits >> 16, 4) & "-" & digits(mostSigBits, 4) & "-" & digits(leastSigBits >> 48, 4) & "-" & digits(leastSigBits, 12))
		End Function

		''' <summary>
		''' Returns val represented by the specified number of hex digits. </summary>
		Private Shared Function digits(ByVal val As Long, ByVal digits_Renamed As Integer) As String
			Dim hi As Long = 1L << (digits_Renamed * 4)
			Return Long.toHexString(hi Or (val And (hi - 1))).Substring(1)
		End Function

		''' <summary>
		''' Returns a hash code for this {@code UUID}.
		''' </summary>
		''' <returns>  A hash code value for this {@code UUID} </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim hilo As Long = mostSigBits Xor leastSigBits
			Return (CInt(Fix(hilo >> 32))) Xor CInt(hilo)
		End Function

		''' <summary>
		''' Compares this object to the specified object.  The result is {@code
		''' true} if and only if the argument is not {@code null}, is a {@code UUID}
		''' object, has the same variant, and contains the same value, bit for bit,
		''' as this {@code UUID}.
		''' </summary>
		''' <param name="obj">
		'''         The object to be compared
		''' </param>
		''' <returns>  {@code true} if the objects are the same; {@code false}
		'''          otherwise </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If (Nothing Is obj) OrElse (obj.GetType() IsNot GetType(UUID)) Then Return False
			Dim id As UUID = CType(obj, UUID)
			Return (mostSigBits = id.mostSigBits AndAlso leastSigBits = id.leastSigBits)
		End Function

		' Comparison Operations

		''' <summary>
		''' Compares this UUID with the specified UUID.
		''' 
		''' <p> The first of two UUIDs is greater than the second if the most
		''' significant field in which the UUIDs differ is greater for the first
		''' UUID.
		''' </summary>
		''' <param name="val">
		'''         {@code UUID} to which this {@code UUID} is to be compared
		''' </param>
		''' <returns>  -1, 0 or 1 as this {@code UUID} is less than, equal to, or
		'''          greater than {@code val}
		'''  </returns>
		Public Function compareTo(ByVal val As UUID) As Integer Implements Comparable(Of UUID).compareTo
			' The ordering is intentionally set up so that the UUIDs
			' can simply be numerically compared as two numbers
			Return (If(Me.mostSigBits < val.mostSigBits, -1, (If(Me.mostSigBits > val.mostSigBits, 1, (If(Me.leastSigBits < val.leastSigBits, -1, (If(Me.leastSigBits > val.leastSigBits, 1, 0))))))))
		End Function
	End Class

End Namespace
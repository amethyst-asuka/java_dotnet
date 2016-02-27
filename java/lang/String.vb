Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The {@code String} class represents character strings. All
	''' string literals in Java programs, such as {@code "abc"}, are
	''' implemented as instances of this class.
	''' <p>
	''' Strings are constant; their values cannot be changed after they
	''' are created. String buffers support mutable strings.
	''' Because String objects are immutable they can be shared. For example:
	''' <blockquote><pre>
	'''     String str = "abc";
	''' </pre></blockquote><p>
	''' is equivalent to:
	''' <blockquote><pre>
	'''     char data[] = {'a', 'b', 'c'};
	'''     String str = new String(data);
	''' </pre></blockquote><p>
	''' Here are some more examples of how strings can be used:
	''' <blockquote><pre>
	'''     System.out.println("abc");
	'''     String cde = "cde";
	'''     System.out.println("abc" + cde);
	'''     String c = "abc".substring(2,3);
	'''     String d = cde.substring(1, 2);
	''' </pre></blockquote>
	''' <p>
	''' The class {@code String} includes methods for examining
	''' individual characters of the sequence, for comparing strings, for
	''' searching strings, for extracting substrings, and for creating a
	''' copy of a string with all characters translated to uppercase or to
	''' lowercase. Case mapping is based on the Unicode Standard version
	''' specified by the <seealso cref="java.lang.Character Character"/> class.
	''' <p>
	''' The Java language provides special support for the string
	''' concatenation operator (&nbsp;+&nbsp;), and for conversion of
	''' other objects to strings. String concatenation is implemented
	''' through the {@code StringBuilder}(or {@code StringBuffer})
	''' class and its {@code append} method.
	''' String conversions are implemented through the method
	''' {@code toString}, defined by {@code Object} and
	''' inherited by all classes in Java. For additional information on
	''' string concatenation and conversion, see Gosling, Joy, and Steele,
	''' <i>The Java Language Specification</i>.
	''' 
	''' <p> Unless otherwise noted, passing a <tt>null</tt> argument to a constructor
	''' or method in this class will cause a <seealso cref="NullPointerException"/> to be
	''' thrown.
	''' 
	''' <p>A {@code String} represents a string in the UTF-16 format
	''' in which <em>supplementary characters</em> are represented by <em>surrogate
	''' pairs</em> (see the section <a href="Character.html#unicode">Unicode
	''' Character Representations</a> in the {@code Character} class for
	''' more information).
	''' Index values refer to {@code char} code units, so a supplementary
	''' character uses two positions in a {@code String}.
	''' <p>The {@code String} class provides methods for dealing with
	''' Unicode code points (i.e., characters), in addition to those for
	''' dealing with Unicode code units (i.e., {@code char} values).
	''' 
	''' @author  Lee Boynton
	''' @author  Arthur van Hoff
	''' @author  Martin Buchholz
	''' @author  Ulf Zibis </summary>
	''' <seealso cref=     java.lang.Object#toString() </seealso>
	''' <seealso cref=     java.lang.StringBuffer </seealso>
	''' <seealso cref=     java.lang.StringBuilder </seealso>
	''' <seealso cref=     java.nio.charset.Charset
	''' @since   JDK1.0 </seealso>

	<Serializable> _
	Public NotInheritable Class [String]
		Implements Comparable(Of String), CharSequence

        ''' <summary>
        ''' The value is used for character storage. </summary>
        Private ReadOnly value As Char()

        ''' <summary>
        ''' Cache the hash code for the string </summary>
        Private hash As Integer ' Default to 0

		''' <summary>
		''' use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		Private Const serialVersionUID As Long = -6849794470754667710L

		''' <summary>
		''' Class String is special cased within the Serialization Stream Protocol.
		''' 
		''' A String instance is written into an ObjectOutputStream according to
		''' <a href="{@docRoot}/../platform/serialization/spec/output.html">
		''' Object Serialization Specification, Section 6.2, "Stream Elements"</a>
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = New java.io.ObjectStreamField(){}

		''' <summary>
		''' Initializes a newly created {@code String} object so that it represents
		''' an empty character sequence.  Note that use of this constructor is
		''' unnecessary since Strings are immutable.
		''' </summary>
		Public Sub New()
			Me.value = "".value
		End Sub

		''' <summary>
		''' Initializes a newly created {@code String} object so that it represents
		''' the same sequence of characters as the argument; in other words, the
		''' newly created string is a copy of the argument string. Unless an
		''' explicit copy of {@code original} is needed, use of this constructor is
		''' unnecessary since Strings are immutable.
		''' </summary>
		''' <param name="original">
		'''         A {@code String} </param>
		Public Sub New(ByVal original As String)
			Me.value = original.value
			Me.hash = original.hash
		End Sub

		''' <summary>
		''' Allocates a new {@code String} so that it represents the sequence of
		''' characters currently contained in the character array argument. The
		''' contents of the character array are copied; subsequent modification of
		''' the character array does not affect the newly created string.
		''' </summary>
		''' <param name="value">
		'''         The initial value of the string </param>
		Public Sub New(ByVal value As Char())
			Me.value = java.util.Arrays.copyOf(value, value.Length)
		End Sub

        ''' <summary>
        ''' Allocates a new {@code String} that contains characters from a subarray
        ''' of the character array argument. The {@code offset} argument is the
        ''' index of the first character of the subarray and the {@code count}
        ''' argument specifies the length of the subarray. The contents of the
        ''' subarray are copied; subsequent modification of the character array does
        ''' not affect the newly created string.
        ''' </summary>
        ''' <param name="value">
        '''         Array that is the source of characters
        ''' </param>
        ''' <param name="offset">
        '''         The initial offset
        ''' </param>
        ''' <param name="count">
        '''         The length
        ''' </param>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the {@code offset} and {@code count} arguments index
        '''          characters outside the bounds of the {@code value} array </exception>
        'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Sub New(value As Char(), offset As Integer, count As Integer)
            If offset < 0 Then Throw New StringIndexOutOfBoundsException(offset)
            If count <= 0 Then
                If count < 0 Then Throw New StringIndexOutOfBoundsException(count)
                If offset <= value.Length Then
                    Me.value = "".value
                    Return
                End If
            End If
            ' Note: offset or count might be near -1>>>1.
            If offset > value.Length - count Then Throw New StringIndexOutOfBoundsException(offset + count)
            Me.value = java.util.Arrays.copyOfRange(value, offset, offset + count)
        End Sub
        ''' <summary>
        ''' Allocates a new {@code String} that contains characters from a subarray
        ''' of the <a href="Character.html#unicode">Unicode code point</a> array
        ''' argument.  The {@code offset} argument is the index of the first code
        ''' point of the subarray and the {@code count} argument specifies the
        ''' length of the subarray.  The contents of the subarray are converted to
        ''' {@code char}s; subsequent modification of the {@code int} array does not
        ''' affect the newly created string.
        ''' </summary>
        ''' <param name="codePoints">
        '''         Array that is the source of Unicode code points
        ''' </param>
        ''' <param name="offset">
        '''         The initial offset
        ''' </param>
        ''' <param name="count">
        '''         The length
        ''' </param>
        ''' <exception cref="IllegalArgumentException">
        '''          If any invalid Unicode code point is found in {@code
        '''          codePoints}
        ''' </exception>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the {@code offset} and {@code count} arguments index
        '''          characters outside the bounds of the {@code codePoints} array
        ''' 
        ''' @since  1.5 </exception>
        Sub New(codePoints As Integer(), offset As Integer, count As Integer)
            If offset < 0 Then Throw New StringIndexOutOfBoundsException(offset)
            If count <= 0 Then
                If count < 0 Then Throw New StringIndexOutOfBoundsException(count)
                If offset <= codePoints.Length Then
                    Me.value = "".value
                    Return
                End If
            End If
            ' Note: offset or count might be near -1>>>1.
            If offset > codePoints.Length - count Then Throw New StringIndexOutOfBoundsException(offset + count)

            Dim [end] As Integer = offset + count

            ' Pass 1: Compute precise size of char[]
            Dim n As Integer = count
            For i As Integer = offset To [end] - 1
                Dim c As Integer = codePoints(i)
                If Character.isBmpCodePoint(c) Then
                    Continue For
                ElseIf Character.isValidCodePoint(c) Then
                    n += 1
                Else
                    Throw New IllegalArgumentException(Convert.ToString(c))
                End If
            Next i

            ' Pass 2: Allocate and fill in char[]
            Dim v As Char() = New Char(n - 1) {}

            Dim i As Integer = offset
            Dim j As Integer = 0
            Do While i < [end]
                Dim c As Integer = codePoints(i)
                If Character.isBmpCodePoint(c) Then
                    v(j) = ChrW(c)
                Else
                    Character.toSurrogates(c, v, j)
                    j += 1
                End If
                i += 1
                j += 1
            Loop

            Me.value = v
        End Sub
        ''' <summary>
        ''' Allocates a new {@code String} constructed from a subarray of an array
        ''' of 8-bit integer values.
        ''' 
        ''' <p> The {@code offset} argument is the index of the first byte of the
        ''' subarray, and the {@code count} argument specifies the length of the
        ''' subarray.
        ''' 
        ''' <p> Each {@code byte} in the subarray is converted to a {@code char} as
        ''' specified in the method above.
        ''' </summary>
        ''' @deprecated This method does not properly convert bytes into characters.
        ''' As of JDK&nbsp;1.1, the preferred way to do this is via the
        ''' {@code String} constructors that take a {@link
        ''' java.nio.charset.Charset}, charset name, or that use the platform's
        ''' default charset.
        ''' 
        ''' <param name="ascii">
        '''         The bytes to be converted to characters
        ''' </param>
        ''' <param name="hibyte">
        '''         The top 8 bits of each 16-bit Unicode code unit
        ''' </param>
        ''' <param name="offset">
        '''         The initial offset </param>
        ''' <param name="count">
        '''         The length
        ''' </param>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the {@code offset} or {@code count} argument is invalid
        ''' </exception>
        ''' <seealso cref=  #String(byte[], int) </seealso>
        ''' <seealso cref=  #String(byte[], int, int, java.lang.String) </seealso>
        ''' <seealso cref=  #String(byte[], int, int, java.nio.charset.Charset) </seealso>
        ''' <seealso cref=  #String(byte[], int, int) </seealso>
        ''' <seealso cref=  #String(byte[], java.lang.String) </seealso>
        ''' <seealso cref=  #String(byte[], java.nio.charset.Charset) </seealso>
        ''' <seealso cref=  #String(byte[]) </seealso>
        <Obsolete("This method does not properly convert bytes into characters.")>
        Sub New(ascii() As SByte, hibyte As Integer, offset As Integer, count As Integer)
            checkBounds(ascii, offset, count)
            Dim value As Char() = New Char(count - 1) {}

            If hibyte = 0 Then
                Dim i As Integer = count
                'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                Do While i -= 1 > 0
					value(i) = CChar(ascii(i + offset) And &HFF)
                Loop
            Else
                hibyte <<= 8
                Dim i As Integer = count
                'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                Do While i -= 1 > 0
					value(i) = CChar(hibyte Or (ascii(i + offset) And &HFF))
                Loop
            End If
            Me.value = value
        End Sub
        ''' <summary>
        ''' Allocates a new {@code String} containing characters constructed from
        ''' an array of 8-bit integer values. Each character <i>c</i>in the
        ''' resulting string is constructed from the corresponding component
        ''' <i>b</i> in the byte array such that:
        ''' 
        ''' <blockquote><pre>
        '''     <b><i>c</i></b> == (char)(((hibyte &amp; 0xff) &lt;&lt; 8)
        '''                         | (<b><i>b</i></b> &amp; 0xff))
        ''' </pre></blockquote>
        ''' </summary>
        ''' @deprecated  This method does not properly convert bytes into
        ''' characters.  As of JDK&nbsp;1.1, the preferred way to do this is via the
        ''' {@code String} constructors that take a {@link
        ''' java.nio.charset.Charset}, charset name, or that use the platform's
        ''' default charset.
        ''' 
        ''' <param name="ascii">
        '''         The bytes to be converted to characters
        ''' </param>
        ''' <param name="hibyte">
        '''         The top 8 bits of each 16-bit Unicode code unit
        ''' </param>
        ''' <seealso cref=  #String(byte[], int, int, java.lang.String) </seealso>
        ''' <seealso cref=  #String(byte[], int, int, java.nio.charset.Charset) </seealso>
        ''' <seealso cref=  #String(byte[], int, int) </seealso>
        ''' <seealso cref=  #String(byte[], java.lang.String) </seealso>
        ''' <seealso cref=  #String(byte[], java.nio.charset.Charset) </seealso>
        ''' <seealso cref=  #String(byte[]) </seealso>
        <Obsolete(" This method does not properly convert bytes into")>
        Sub New(ascii() As SByte, hibyte As Integer)
            Me.New(ascii, hibyte, 0, ascii.Length)
        End Sub
        '     Common private utility method used to bounds check the byte array
        '     * and requested offset & length values used by the String(byte[],..)
        '     * constructors.
        '     
        Private Shared Sub checkBounds(bytes As SByte(), offset As Integer, length As Integer)
            If length < 0 Then Throw New StringIndexOutOfBoundsException(length)
            If offset < 0 Then Throw New StringIndexOutOfBoundsException(offset)
            If offset > bytes.Length - length Then Throw New StringIndexOutOfBoundsException(offset + length)
        End Sub
        ''' <summary>
        ''' Constructs a new {@code String} by decoding the specified subarray of
        ''' bytes using the specified charset.  The length of the new {@code String}
        ''' is a function of the charset, and hence may not be equal to the length
        ''' of the subarray.
        ''' 
        ''' <p> The behavior of this constructor when the given bytes are not valid
        ''' in the given charset is unspecified.  The {@link
        ''' java.nio.charset.CharsetDecoder} class should be used when more control
        ''' over the decoding process is required.
        ''' </summary>
        ''' <param name="bytes">
        '''         The bytes to be decoded into characters
        ''' </param>
        ''' <param name="offset">
        '''         The index of the first byte to decode
        ''' </param>
        ''' <param name="length">
        '''         The number of bytes to decode
        ''' </param>
        ''' <param name="charsetName">
        '''         The name of a supported {@link java.nio.charset.Charset
        '''         charset}
        ''' </param>
        ''' <exception cref="UnsupportedEncodingException">
        '''          If the named charset is not supported
        ''' </exception>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the {@code offset} and {@code length} arguments index
        '''          characters outside the bounds of the {@code bytes} array
        ''' 
        ''' @since  JDK1.1 </exception>
        Sub New(bytes() As SByte(), offset As Integer, length As Integer, charsetName As String)
            Try
                If charsetName Is Nothing Then Throw New NullPointerException("charsetName")
                checkBounds(bytes, offset, length)
                Me.value = StringCoding.decode(charsetName, bytes, offset, length)
            Catch ex As Exception
                Throw New java.io.UnsupportedEncodingException(ex)
            End Try
        End Sub
        ''' <summary>
        ''' Constructs a new {@code String} by decoding the specified subarray of
        ''' bytes using the specified <seealso cref="java.nio.charset.Charset charset"/>.
        ''' The length of the new {@code String} is a function of the charset, and
        ''' hence may not be equal to the length of the subarray.
        ''' 
        ''' <p> This method always replaces malformed-input and unmappable-character
        ''' sequences with this charset's default replacement string.  The {@link
        ''' java.nio.charset.CharsetDecoder} class should be used when more control
        ''' over the decoding process is required.
        ''' </summary>
        ''' <param name="bytes">
        '''         The bytes to be decoded into characters
        ''' </param>
        ''' <param name="offset">
        '''         The index of the first byte to decode
        ''' </param>
        ''' <param name="length">
        '''         The number of bytes to decode
        ''' </param>
        ''' <param name="charset">
        '''         The <seealso cref="java.nio.charset.Charset charset"/> to be used to
        '''         decode the {@code bytes}
        ''' </param>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the {@code offset} and {@code length} arguments index
        '''          characters outside the bounds of the {@code bytes} array
        ''' 
        ''' @since  1.6 </exception>
        Sub New(bytes() As SByte, offset As Integer, length As Integer, charset As java.nio.charset.Charset)
            If charset Is Nothing Then Throw New NullPointerException("charset")
            checkBounds(bytes, offset, length)
            Me.value = StringCoding.decode(charset, bytes, offset, length)
        End Sub
        ''' <summary>
        ''' Constructs a new {@code String} by decoding the specified array of bytes
        ''' using the specified <seealso cref="java.nio.charset.Charset charset"/>.  The
        ''' length of the new {@code String} is a function of the charset, and hence
        ''' may not be equal to the length of the byte array.
        ''' 
        ''' <p> The behavior of this constructor when the given bytes are not valid
        ''' in the given charset is unspecified.  The {@link
        ''' java.nio.charset.CharsetDecoder} class should be used when more control
        ''' over the decoding process is required.
        ''' </summary>
        ''' <param name="bytes">
        '''         The bytes to be decoded into characters
        ''' </param>
        ''' <param name="charsetName">
        '''         The name of a supported {@link java.nio.charset.Charset
        '''         charset}
        ''' </param>
        ''' <exception cref="UnsupportedEncodingException">
        '''          If the named charset is not supported
        ''' 
        ''' @since  JDK1.1 </exception>
        Sub New(bytes() As SByte, charsetName As String) 'throws java.io.UnsupportedEncodingException
            Me.New(bytes, 0, bytes.Length, charsetName)
        End Sub
        ''' <summary>
        ''' Constructs a new {@code String} by decoding the specified array of
        ''' bytes using the specified <seealso cref="java.nio.charset.Charset charset"/>.
        ''' The length of the new {@code String} is a function of the charset, and
        ''' hence may not be equal to the length of the byte array.
        ''' 
        ''' <p> This method always replaces malformed-input and unmappable-character
        ''' sequences with this charset's default replacement string.  The {@link
        ''' java.nio.charset.CharsetDecoder} class should be used when more control
        ''' over the decoding process is required.
        ''' </summary>
        ''' <param name="bytes">
        '''         The bytes to be decoded into characters
        ''' </param>
        ''' <param name="charset">
        '''         The <seealso cref="java.nio.charset.Charset charset"/> to be used to
        '''         decode the {@code bytes}
        ''' 
        ''' @since  1.6 </param>
        Sub New(bytes() As SByte, charset As java.nio.charset.Charset)
            Me.New(bytes, 0, bytes.Length, charset)
        End Sub
        ''' <summary>
        ''' Constructs a new {@code String} by decoding the specified subarray of
        ''' bytes using the platform's default charset.  The length of the new
        ''' {@code String} is a function of the charset, and hence may not be equal
        ''' to the length of the subarray.
        ''' 
        ''' <p> The behavior of this constructor when the given bytes are not valid
        ''' in the default charset is unspecified.  The {@link
        ''' java.nio.charset.CharsetDecoder} class should be used when more control
        ''' over the decoding process is required.
        ''' </summary>
        ''' <param name="bytes">
        '''         The bytes to be decoded into characters
        ''' </param>
        ''' <param name="offset">
        '''         The index of the first byte to decode
        ''' </param>
        ''' <param name="length">
        '''         The number of bytes to decode
        ''' </param>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the {@code offset} and the {@code length} arguments index
        '''          characters outside the bounds of the {@code bytes} array
        ''' 
        ''' @since  JDK1.1 </exception>
        Sub New(bytes() As SByte, offset As Integer, length As Integer)
            checkBounds(bytes, offset, length)
            Me.value = StringCoding.decode(bytes, offset, length)
        End Sub
        ''' <summary>
        ''' Constructs a new {@code String} by decoding the specified array of bytes
        ''' using the platform's default charset.  The length of the new {@code
        ''' String} is a function of the charset, and hence may not be equal to the
        ''' length of the byte array.
        ''' 
        ''' <p> The behavior of this constructor when the given bytes are not valid
        ''' in the default charset is unspecified.  The {@link
        ''' java.nio.charset.CharsetDecoder} class should be used when more control
        ''' over the decoding process is required.
        ''' </summary>
        ''' <param name="bytes">
        '''         The bytes to be decoded into characters
        ''' 
        ''' @since  JDK1.1 </param>
        Sub New(bytes() As SByte)
            Me.New(bytes, 0, bytes.Length)
        End Sub
        ''' <summary>
        ''' Allocates a new string that contains the sequence of characters
        ''' currently contained in the string buffer argument. The contents of the
        ''' string buffer are copied; subsequent modification of the string buffer
        ''' does not affect the newly created string.
        ''' </summary>
        ''' <param name="buffer">
        '''         A {@code StringBuffer} </param>
        Sub New(buffer As StringBuffer)
            SyncLock buffer
                Me.value = java.util.Arrays.copyOf(buffer.value, buffer.length())
            End SyncLock
        End Sub
        ''' <summary>
        ''' Allocates a new string that contains the sequence of characters
        ''' currently contained in the string builder argument. The contents of the
        ''' string builder are copied; subsequent modification of the string builder
        ''' does not affect the newly created string.
        ''' 
        ''' <p> This constructor is provided to ease migration to {@code
        ''' StringBuilder}. Obtaining a string from a string builder via the {@code
        ''' toString} method is likely to run faster and is generally preferred.
        ''' </summary>
        ''' <param name="builder">
        '''          A {@code StringBuilder}
        ''' 
        ''' @since  1.5 </param>
        Sub New(buffer As StringBuilder)
            Me.value = java.util.Arrays.copyOf(builder.value, builder.length())
        End Sub
        '    
        '    * Package private constructor which shares value array for speed.
        '    * this constructor is always expected to be called with share==true.
        '    * a separate constructor is needed because we already have a public
        '    * String(char[]) constructor that makes a copy of the given char[].
        '    
        Sub New(value As Char(), share As Boolean)
            ' assert share : "unshared not supported";
            Me.value = value
        End Sub
        ''' <summary>
        ''' Returns the length of this string.
        ''' The length is equal to the number of <a href="Character.html#unicode">Unicode
        ''' code units</a> in the string.
        ''' </summary>
        ''' <returns>  the length of the sequence of characters represented by this
        '''          object. </returns>
        Public Function length() As Integer
            Return value.Length
        End Function
        ''' <summary>
        ''' Returns {@code true} if, and only if, <seealso cref="#length()"/> is {@code 0}.
        ''' </summary>
        ''' <returns> {@code true} if <seealso cref="#length()"/> is {@code 0}, otherwise
        ''' {@code false}
        ''' 
        ''' @since 1.6 </returns>
        Public Function empty() As Boolean
            Return value.Length = 0
        End Function
        ''' <summary>
        ''' Returns the {@code char} value at the
        ''' specified index. An index ranges from {@code 0} to
        ''' {@code length() - 1}. The first {@code char} value of the sequence
        ''' is at index {@code 0}, the next at index {@code 1},
        ''' and so on, as for array indexing.
        ''' 
        ''' <p>If the {@code char} value specified by the index is a
        ''' <a href="Character.html#unicode">surrogate</a>, the surrogate
        ''' value is returned.
        ''' </summary>
        ''' <param name="index">   the index of the {@code char} value. </param>
        ''' <returns>     the {@code char} value at the specified index of this string.
        '''             The first {@code char} value is at index {@code 0}. </returns>
        ''' <exception cref="IndexOutOfBoundsException">  if the {@code index}
        '''             argument is negative or not less than the length of this
        '''             string. </exception>
        Public Function charAt(index As Integer) As Char
            If (index < 0) OrElse (index >= value.Length) Then Throw New StringIndexOutOfBoundsException(index)
            Return value(index)
        End Function
        ''' <summary>
        ''' Returns the character (Unicode code point) at the specified
        ''' index. The index refers to {@code char} values
        ''' (Unicode code units) and ranges from {@code 0} to
        ''' <seealso cref="#length()"/>{@code  - 1}.
        ''' 
        ''' <p> If the {@code char} value specified at the given index
        ''' is in the high-surrogate range, the following index is less
        ''' than the length of this {@code String}, and the
        ''' {@code char} value at the following index is in the
        ''' low-surrogate range, then the supplementary code point
        ''' corresponding to this surrogate pair is returned. Otherwise,
        ''' the {@code char} value at the given index is returned.
        ''' </summary>
        ''' <param name="index"> the index to the {@code char} values </param>
        ''' <returns>     the code point value of the character at the
        '''             {@code index} </returns>
        ''' <exception cref="IndexOutOfBoundsException">  if the {@code index}
        '''             argument is negative or not less than the length of this
        '''             string.
        ''' @since      1.5 </exception>
        Public Function codePointAt(index As Integer) As Integer
            If (index < 0) OrElse (index >= value.Length) Then Throw New StringIndexOutOfBoundsException(index)
            Return Character.codePointAtImpl(value, index, value.Length)
        End Function
        ''' <summary>
        ''' Returns the character (Unicode code point) before the specified
        ''' index. The index refers to {@code char} values
        ''' (Unicode code units) and ranges from {@code 1} to {@link
        ''' CharSequence#length() length}.
        ''' 
        ''' <p> If the {@code char} value at {@code (index - 1)}
        ''' is in the low-surrogate range, {@code (index - 2)} is not
        ''' negative, and the {@code char} value at {@code (index -
        ''' 2)} is in the high-surrogate range, then the
        ''' supplementary code point value of the surrogate pair is
        ''' returned. If the {@code char} value at {@code index -
        ''' 1} is an unpaired low-surrogate or a high-surrogate, the
        ''' surrogate value is returned.
        ''' </summary>
        ''' <param name="index"> the index following the code point that should be returned </param>
        ''' <returns>    the Unicode code point value before the given index. </returns>
        ''' <exception cref="IndexOutOfBoundsException"> if the {@code index}
        '''            argument is less than 1 or greater than the length
        '''            of this string.
        ''' @since     1.5 </exception>
        Public Function codePointBefore(index As Integer) As Integer
            Dim i As Integer = index - 1
            If (i < 0) OrElse (i >= value.Length) Then Throw New StringIndexOutOfBoundsException(index)
            Return Character.codePointBeforeImpl(value, index, 0)
        End Function
        ''' <summary>
        ''' Returns the number of Unicode code points in the specified text
        ''' range of this {@code String}. The text range begins at the
        ''' specified {@code beginIndex} and extends to the
        ''' {@code char} at index {@code endIndex - 1}. Thus the
        ''' length (in {@code char}s) of the text range is
        ''' {@code endIndex-beginIndex}. Unpaired surrogates within
        ''' the text range count as one code point each.
        ''' </summary>
        ''' <param name="beginIndex"> the index to the first {@code char} of
        ''' the text range. </param>
        ''' <param name="endIndex"> the index after the last {@code char} of
        ''' the text range. </param>
        ''' <returns> the number of Unicode code points in the specified text
        ''' range </returns>
        ''' <exception cref="IndexOutOfBoundsException"> if the
        ''' {@code beginIndex} is negative, or {@code endIndex}
        ''' is larger than the length of this {@code String}, or
        ''' {@code beginIndex} is larger than {@code endIndex}.
        ''' @since  1.5 </exception>
        Public Function codePointCount(beginIndex As Integer, endIndex As Integer) As Integer
            If beginIndex < 0 OrElse endIndex > value.Length OrElse beginIndex > endIndex Then Throw New IndexOutOfBoundsException
            Return Character.codePointCountImpl(value, beginIndex, endIndex - beginIndex)
        End Function
        ''' <summary>
        ''' Returns the index within this {@code String} that is
        ''' offset from the given {@code index} by
        ''' {@code codePointOffset} code points. Unpaired surrogates
        ''' within the text range given by {@code index} and
        ''' {@code codePointOffset} count as one code point each.
        ''' </summary>
        ''' <param name="index"> the index to be offset </param>
        ''' <param name="codePointOffset"> the offset in code points </param>
        ''' <returns> the index within this {@code String} </returns>
        ''' <exception cref="IndexOutOfBoundsException"> if {@code index}
        '''   is negative or larger then the length of this
        '''   {@code String}, or if {@code codePointOffset} is positive
        '''   and the substring starting with {@code index} has fewer
        '''   than {@code codePointOffset} code points,
        '''   or if {@code codePointOffset} is negative and the substring
        '''   before {@code index} has fewer than the absolute value
        '''   of {@code codePointOffset} code points.
        ''' @since 1.5 </exception>
        Public Function offsetByCodePoints(index As Integer, codePointOffset As Integer) As Integer
            If index < 0 OrElse index > value.Length Then Throw New IndexOutOfBoundsException
            Return Character.offsetByCodePointsImpl(value, 0, value.Length, index, codePointOffset)
        End Function
        ''' <summary>
        ''' Copy characters from this string into dst starting at dstBegin.
        ''' This method doesn't perform any range checking.
        ''' </summary>
        Sub getChars(dst() As Char, dstBegin As Integer)
            Array.Copy(value, 0, dst, dstBegin, value.Length)
        End Sub
        ''' <summary>
        ''' Copies characters from this string into the destination character
        ''' array.
        ''' <p>
        ''' The first character to be copied is at index {@code srcBegin};
        ''' the last character to be copied is at index {@code srcEnd-1}
        ''' (thus the total number of characters to be copied is
        ''' {@code srcEnd-srcBegin}). The characters are copied into the
        ''' subarray of {@code dst} starting at index {@code dstBegin}
        ''' and ending at index:
        ''' <blockquote><pre>
        '''     dstBegin + (srcEnd-srcBegin) - 1
        ''' </pre></blockquote>
        ''' </summary>
        ''' <param name="srcBegin">   index of the first character in the string
        '''                        to copy. </param>
        ''' <param name="srcEnd">     index after the last character in the string
        '''                        to copy. </param>
        ''' <param name="dst">        the destination array. </param>
        ''' <param name="dstBegin">   the start offset in the destination array. </param>
        ''' <exception cref="IndexOutOfBoundsException"> If any of the following
        '''            is true:
        '''            <ul><li>{@code srcBegin} is negative.
        '''            <li>{@code srcBegin} is greater than {@code srcEnd}
        '''            <li>{@code srcEnd} is greater than the length of this
        '''                string
        '''            <li>{@code dstBegin} is negative
        '''            <li>{@code dstBegin+(srcEnd-srcBegin)} is larger than
        '''                {@code dst.length}</ul> </exception>
        Public Sub getChars(Integer srcBegin, Integer srcEnd, Char dst() , Integer dstBegin)
			If srcBegin < 0 Then Throw New StringIndexOutOfBoundsException(srcBegin)
            If srcEnd > value.Length Then Throw New StringIndexOutOfBoundsException(srcEnd)
            If srcBegin > srcEnd Then Throw New StringIndexOutOfBoundsException(srcEnd - srcBegin)
            Array.Copy(value, srcBegin, dst, dstBegin, srcEnd - srcBegin)
        End Sub
        ''' <summary>
        ''' Copies characters from this string into the destination byte array. Each
        ''' byte receives the 8 low-order bits of the corresponding character. The
        ''' eight high-order bits of each character are not copied and do not
        ''' participate in the transfer in any way.
        ''' 
        ''' <p> The first character to be copied is at index {@code srcBegin}; the
        ''' last character to be copied is at index {@code srcEnd-1}.  The total
        ''' number of characters to be copied is {@code srcEnd-srcBegin}. The
        ''' characters, converted to bytes, are copied into the subarray of {@code
        ''' dst} starting at index {@code dstBegin} and ending at index:
        ''' 
        ''' <blockquote><pre>
        '''     dstBegin + (srcEnd-srcBegin) - 1
        ''' </pre></blockquote>
        ''' </summary>
        ''' @deprecated  This method does not properly convert characters into
        ''' bytes.  As of JDK&nbsp;1.1, the preferred way to do this is via the
        ''' <seealso cref="#getBytes()"/> method, which uses the platform's default charset.
        ''' 
        ''' <param name="srcBegin">
        '''         Index of the first character in the string to copy
        ''' </param>
        ''' <param name="srcEnd">
        '''         Index after the last character in the string to copy
        ''' </param>
        ''' <param name="dst">
        '''         The destination array
        ''' </param>
        ''' <param name="dstBegin">
        '''         The start offset in the destination array
        ''' </param>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If any of the following is true:
        '''          <ul>
        '''            <li> {@code srcBegin} is negative
        '''            <li> {@code srcBegin} is greater than {@code srcEnd}
        '''            <li> {@code srcEnd} is greater than the length of this String
        '''            <li> {@code dstBegin} is negative
        '''            <li> {@code dstBegin+(srcEnd-srcBegin)} is larger than {@code
        '''                 dst.length}
        '''          </ul> </exception>
        <Obsolete(" This method does not properly convert characters into")>
        Public Sub getBytes(Integer srcBegin, Integer srcEnd, SByte dst() , Integer dstBegin)
			If srcBegin < 0 Then Throw New StringIndexOutOfBoundsException(srcBegin)
            If srcEnd > value.Length Then Throw New StringIndexOutOfBoundsException(srcEnd)
            If srcBegin > srcEnd Then Throw New StringIndexOutOfBoundsException(srcEnd - srcBegin)
            java.util.Objects.requireNonNull(dst)

            Dim j As Integer = dstBegin
            Dim n As Integer = srcEnd
            Dim i As Integer = srcBegin
            Dim val As Char() = value ' avoid getfield opcode

            Do While i < n
                dst(j) = AscW(val(i))
                i += 1
                j += 1
            Loop
        End Sub
        ''' <summary>
        ''' Encodes this {@code String} into a sequence of bytes using the named
        ''' charset, storing the result into a new byte array.
        ''' 
        ''' <p> The behavior of this method when this string cannot be encoded in
        ''' the given charset is unspecified.  The {@link
        ''' java.nio.charset.CharsetEncoder} class should be used when more control
        ''' over the encoding process is required.
        ''' </summary>
        ''' <param name="charsetName">
        '''         The name of a supported {@link java.nio.charset.Charset
        '''         charset}
        ''' </param>
        ''' <returns>  The resultant byte array
        ''' </returns>
        ''' <exception cref="UnsupportedEncodingException">
        '''          If the named charset is not supported
        ''' 
        ''' @since  JDK1.1 </exception>
        Public SByte() getBytes(String charsetName) throws java.io.UnsupportedEncodingException
			If charsetName Is Nothing Then Throw New NullPointerException
			Return StringCoding.encode(charsetName, value, 0, value.Length)

		''' <summary>
		''' Encodes this {@code String} into a sequence of bytes using the given
		''' <seealso cref="java.nio.charset.Charset charset"/>, storing the result into a
		''' new byte array.
		''' 
		''' <p> This method always replaces malformed-input and unmappable-character
		''' sequences with this charset's default replacement byte array.  The
		''' <seealso cref="java.nio.charset.CharsetEncoder"/> class should be used when more
		''' control over the encoding process is required.
		''' </summary>
		''' <param name="charset">
		'''         The <seealso cref="java.nio.charset.Charset"/> to be used to encode
		'''         the {@code String}
		''' </param>
		''' <returns>  The resultant byte array
		''' 
		''' @since  1.6 </returns>
		public SByte() getBytes(java.nio.charset.Charset charset)
			If charset Is Nothing Then Throw New NullPointerException
			Return StringCoding.encode(charset, value, 0, value.Length)

        ''' <summary>
        ''' Encodes this {@code String} into a sequence of bytes using the
        ''' platform's default charset, storing the result into a new byte array.
        ''' 
        ''' <p> The behavior of this method when this string cannot be encoded in
        ''' the default charset is unspecified.  The {@link
        ''' java.nio.charset.CharsetEncoder} class should be used when more control
        ''' over the encoding process is required.
        ''' </summary>
        ''' <returns>  The resultant byte array
        ''' 
        ''' @since      JDK1.1 </returns>
        Public Function bytes() As SByte()
            Return StringCoding.encode(value, 0, value.Length)
        End Function
        ''' <summary>
        ''' Compares this string to the specified object.  The result is {@code
        ''' true} if and only if the argument is not {@code null} and is a {@code
        ''' String} object that represents the same sequence of characters as this
        ''' object.
        ''' </summary>
        ''' <param name="anObject">
        '''         The object to compare this {@code String} against
        ''' </param>
        ''' <returns>  {@code true} if the given object represents a {@code String}
        '''          equivalent to this string, {@code false} otherwise
        ''' </returns>
        ''' <seealso cref=  #compareTo(String) </seealso>
        ''' <seealso cref=  #equalsIgnoreCase(String) </seealso>
        Public Function Equals(Object anObject) As Boolean
            If Me Is anObject Then Return True
            If TypeOf anObject Is String Then
                Dim anotherString As String = CStr(anObject)
                Dim n As Integer = value.Length
                If n = anotherString.value.Length Then
                    Dim v1 As Char() = value
                    Dim v2 As Char() = anotherString.value
                    Dim i As Integer = 0
                    Dim tempVar As Boolean = n <> 0
                    n -= 1
                    Do While tempVar
                        If v1(i) <> v2(i) Then Return False
                        i += 1
                        tempVar = n <> 0
                        n -= 1
                    Loop
                    Return True
                End If
            End If
            Return False
        End Function
        ''' <summary>
        ''' Compares this string to the specified {@code StringBuffer}.  The result
        ''' is {@code true} if and only if this {@code String} represents the same
        ''' sequence of characters as the specified {@code StringBuffer}. This method
        ''' synchronizes on the {@code StringBuffer}.
        ''' </summary>
        ''' <param name="sb">
        '''         The {@code StringBuffer} to compare this {@code String} against
        ''' </param>
        ''' <returns>  {@code true} if this {@code String} represents the same
        '''          sequence of characters as the specified {@code StringBuffer},
        '''          {@code false} otherwise
        ''' 
        ''' @since  1.4 </returns>
        Public Boolean contentEquals(StringBuffer sb)
			Return contentEquals(CType(sb, CharSequence))

		private Boolean nonSyncContentEquals(AbstractStringBuilder sb)
			Dim v1 As Char() = value
			Dim v2 As Char() = sb.value
			Dim n As Integer = v1.Length
			If n <> sb.length() Then Return False
			For i As Integer = 0 To n - 1
				If v1(i) <> v2(i) Then Return False
			Next i
			Return True

		''' <summary>
		''' Compares this string to the specified {@code CharSequence}.  The
		''' result is {@code true} if and only if this {@code String} represents the
		''' same sequence of char values as the specified sequence. Note that if the
		''' {@code CharSequence} is a {@code StringBuffer} then the method
		''' synchronizes on it.
		''' </summary>
		''' <param name="cs">
		'''         The sequence to compare this {@code String} against
		''' </param>
		''' <returns>  {@code true} if this {@code String} represents the same
		'''          sequence of char values as the specified sequence, {@code
		'''          false} otherwise
		''' 
		''' @since  1.5 </returns>
		public Boolean contentEquals(CharSequence cs)
			' Argument is a StringBuffer, StringBuilder
			If TypeOf cs Is AbstractStringBuilder Then
				If TypeOf cs Is StringBuffer Then
					SyncLock cs
					   Return nonSyncContentEquals(CType(cs, AbstractStringBuilder))
					End SyncLock
				Else
					Return nonSyncContentEquals(CType(cs, AbstractStringBuilder))
				End If
			End If
			' Argument is a String
			If TypeOf cs Is String Then Return Equals(cs)
			' Argument is a generic CharSequence
			Dim v1 As Char() = value
			Dim n As Integer = v1.Length
			If n <> cs.length() Then Return False
			For i As Integer = 0 To n - 1
				If v1(i) <> cs.Chars(i) Then Return False
			Next i
			Return True

		''' <summary>
		''' Compares this {@code String} to another {@code String}, ignoring case
		''' considerations.  Two strings are considered equal ignoring case if they
		''' are of the same length and corresponding characters in the two strings
		''' are equal ignoring case.
		''' 
		''' <p> Two characters {@code c1} and {@code c2} are considered the same
		''' ignoring case if at least one of the following is true:
		''' <ul>
		'''   <li> The two characters are the same (as compared by the
		'''        {@code ==} operator)
		'''   <li> Applying the method {@link
		'''        java.lang.Character#toUpperCase(char)} to each character
		'''        produces the same result
		'''   <li> Applying the method {@link
		'''        java.lang.Character#toLowerCase(char)} to each character
		'''        produces the same result
		''' </ul>
		''' </summary>
		''' <param name="anotherString">
		'''         The {@code String} to compare this {@code String} against
		''' </param>
		''' <returns>  {@code true} if the argument is not {@code null} and it
		'''          represents an equivalent {@code String} ignoring case; {@code
		'''          false} otherwise
		''' </returns>
		''' <seealso cref=  #equals(Object) </seealso>
		public Boolean equalsIgnoreCase(String anotherString)
			Return If(Me Is anotherString, True, (anotherString IsNot Nothing) AndAlso (anotherString.value.length = value.Length) AndAlso regionMatches(True, 0, anotherString, 0, value.Length))

		''' <summary>
		''' Compares two strings lexicographically.
		''' The comparison is based on the Unicode value of each character in
		''' the strings. The character sequence represented by this
		''' {@code String} object is compared lexicographically to the
		''' character sequence represented by the argument string. The result is
		''' a negative integer if this {@code String} object
		''' lexicographically precedes the argument string. The result is a
		''' positive integer if this {@code String} object lexicographically
		''' follows the argument string. The result is zero if the strings
		''' are equal; {@code compareTo} returns {@code 0} exactly when
		''' the <seealso cref="#equals(Object)"/> method would return {@code true}.
		''' <p>
		''' This is the definition of lexicographic ordering. If two strings are
		''' different, then either they have different characters at some index
		''' that is a valid index for both strings, or their lengths are different,
		''' or both. If they have different characters at one or more index
		''' positions, let <i>k</i> be the smallest such index; then the string
		''' whose character at position <i>k</i> has the smaller value, as
		''' determined by using the &lt; operator, lexicographically precedes the
		''' other string. In this case, {@code compareTo} returns the
		''' difference of the two character values at position {@code k} in
		''' the two string -- that is, the value:
		''' <blockquote><pre>
		''' this.charAt(k)-anotherString.charAt(k)
		''' </pre></blockquote>
		''' If there is no index position at which they differ, then the shorter
		''' string lexicographically precedes the longer string. In this case,
		''' {@code compareTo} returns the difference of the lengths of the
		''' strings -- that is, the value:
		''' <blockquote><pre>
		''' this.length()-anotherString.length()
		''' </pre></blockquote>
		''' </summary>
		''' <param name="anotherString">   the {@code String} to be compared. </param>
		''' <returns>  the value {@code 0} if the argument string is equal to
		'''          this string; a value less than {@code 0} if this string
		'''          is lexicographically less than the string argument; and a
		'''          value greater than {@code 0} if this string is
		'''          lexicographically greater than the string argument. </returns>
		public Integer compareTo(String anotherString)
			Dim len1 As Integer = value.Length
			Dim len2 As Integer = anotherString.value.length
			Dim lim As Integer = System.Math.Min(len1, len2)
			Dim v1 As Char() = value
			Dim v2 As Char() = anotherString.value

			Dim k As Integer = 0
			Do While k < lim
				Dim c1 As Char = v1(k)
				Dim c2 As Char = v2(k)
				If c1 <> c2 Then Return AscW(c1) - AscW(c2)
				k += 1
			Loop
			Return len1 - len2

		''' <summary>
		''' A Comparator that orders {@code String} objects as by
		''' {@code compareToIgnoreCase}. This comparator is serializable.
		''' <p>
		''' Note that this Comparator does <em>not</em> take locale into account,
		''' and will result in an unsatisfactory ordering for certain locales.
		''' The java.text package provides <em>Collators</em> to allow
		''' locale-sensitive ordering.
		''' </summary>
		''' <seealso cref=     java.text.Collator#compare(String, String)
		''' @since   1.2 </seealso>
		public static final IComparer(Of String) CASE_INSENSITIVE_ORDER = New CaseInsensitiveComparator
		private static class CaseInsensitiveComparator implements IComparer(Of String), java.io.Serializable
			' use serialVersionUID from JDK 1.2.2 for interoperability
			private static final Long serialVersionUID = 8575799808933029326L

			public Integer compare(String s1, String s2)
				Dim n1 As Integer = s1.length()
				Dim n2 As Integer = s2.length()
				Dim min As Integer = System.Math.Min(n1, n2)
				For i As Integer = 0 To min - 1
					Dim c1 As Char = s1.Chars(i)
					Dim c2 As Char = s2.Chars(i)
					If c1 <> c2 Then
						c1 = Char.ToUpper(c1)
						c2 = Char.ToUpper(c2)
						If c1 <> c2 Then
							c1 = Char.ToLower(c1)
							c2 = Char.ToLower(c2)
							If c1 <> c2 Then Return AscW(c1) - AscW(c2)
						End If
					End If
				Next i
				Return n1 - n2

			''' <summary>
			''' Replaces the de-serialized object. </summary>
			private Object readResolve()
				Return CASE_INSENSITIVE_ORDER

		''' <summary>
		''' Compares two strings lexicographically, ignoring case
		''' differences. This method returns an integer whose sign is that of
		''' calling {@code compareTo} with normalized versions of the strings
		''' where case differences have been eliminated by calling
		''' {@code Character.toLowerCase(Character.toUpperCase(character))} on
		''' each character.
		''' <p>
		''' Note that this method does <em>not</em> take locale into account,
		''' and will result in an unsatisfactory ordering for certain locales.
		''' The java.text package provides <em>collators</em> to allow
		''' locale-sensitive ordering.
		''' </summary>
		''' <param name="str">   the {@code String} to be compared. </param>
		''' <returns>  a negative integer, zero, or a positive integer as the
		'''          specified String is greater than, equal to, or less
		'''          than this String, ignoring case considerations. </returns>
		''' <seealso cref=     java.text.Collator#compare(String, String)
		''' @since   1.2 </seealso>
		public Integer compareToIgnoreCase(String str)
			Return CASE_INSENSITIVE_ORDER.Compare(Me, str)

		''' <summary>
		''' Tests if two string regions are equal.
		''' <p>
		''' A substring of this {@code String} object is compared to a substring
		''' of the argument other. The result is true if these substrings
		''' represent identical character sequences. The substring of this
		''' {@code String} object to be compared begins at index {@code toffset}
		''' and has length {@code len}. The substring of other to be compared
		''' begins at index {@code ooffset} and has length {@code len}. The
		''' result is {@code false} if and only if at least one of the following
		''' is true:
		''' <ul><li>{@code toffset} is negative.
		''' <li>{@code ooffset} is negative.
		''' <li>{@code toffset+len} is greater than the length of this
		''' {@code String} object.
		''' <li>{@code ooffset+len} is greater than the length of the other
		''' argument.
		''' <li>There is some nonnegative integer <i>k</i> less than {@code len}
		''' such that:
		''' {@code this.charAt(toffset + }<i>k</i>{@code ) != other.charAt(ooffset + }
		''' <i>k</i>{@code )}
		''' </ul>
		''' </summary>
		''' <param name="toffset">   the starting offset of the subregion in this string. </param>
		''' <param name="other">     the string argument. </param>
		''' <param name="ooffset">   the starting offset of the subregion in the string
		'''                    argument. </param>
		''' <param name="len">       the number of characters to compare. </param>
		''' <returns>  {@code true} if the specified subregion of this string
		'''          exactly matches the specified subregion of the string argument;
		'''          {@code false} otherwise. </returns>
		public Boolean regionMatches(Integer toffset, String other, Integer ooffset, Integer len)
			Dim ta As Char() = value
			Dim [to] As Integer = toffset
			Dim pa As Char() = other.value
			Dim po As Integer = ooffset
			' Note: toffset, ooffset, or len might be near -1>>>1.
			If (ooffset < 0) OrElse (toffset < 0) OrElse (toffset > (Long)value.Length - len) OrElse (ooffset > (Long)other.value.length - len) Then Return False
			Dim tempVar2 As Boolean = len > 0
			len -= 1
			Do While tempVar2
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim tempVar3 As Boolean = ta([to]++) <> pa(po++)
				If tempVar3 Then Return False
				tempVar2 = len > 0
				len -= 1
			Loop
			Return True

		''' <summary>
		''' Tests if two string regions are equal.
		''' <p>
		''' A substring of this {@code String} object is compared to a substring
		''' of the argument {@code other}. The result is {@code true} if these
		''' substrings represent character sequences that are the same, ignoring
		''' case if and only if {@code ignoreCase} is true. The substring of
		''' this {@code String} object to be compared begins at index
		''' {@code toffset} and has length {@code len}. The substring of
		''' {@code other} to be compared begins at index {@code ooffset} and
		''' has length {@code len}. The result is {@code false} if and only if
		''' at least one of the following is true:
		''' <ul><li>{@code toffset} is negative.
		''' <li>{@code ooffset} is negative.
		''' <li>{@code toffset+len} is greater than the length of this
		''' {@code String} object.
		''' <li>{@code ooffset+len} is greater than the length of the other
		''' argument.
		''' <li>{@code ignoreCase} is {@code false} and there is some nonnegative
		''' integer <i>k</i> less than {@code len} such that:
		''' <blockquote><pre>
		''' this.charAt(toffset+k) != other.charAt(ooffset+k)
		''' </pre></blockquote>
		''' <li>{@code ignoreCase} is {@code true} and there is some nonnegative
		''' integer <i>k</i> less than {@code len} such that:
		''' <blockquote><pre>
		''' Character.toLowerCase(this.charAt(toffset+k)) !=
		''' Character.toLowerCase(other.charAt(ooffset+k))
		''' </pre></blockquote>
		''' and:
		''' <blockquote><pre>
		''' Character.toUpperCase(this.charAt(toffset+k)) !=
		'''         Character.toUpperCase(other.charAt(ooffset+k))
		''' </pre></blockquote>
		''' </ul>
		''' </summary>
		''' <param name="ignoreCase">   if {@code true}, ignore case when comparing
		'''                       characters. </param>
		''' <param name="toffset">      the starting offset of the subregion in this
		'''                       string. </param>
		''' <param name="other">        the string argument. </param>
		''' <param name="ooffset">      the starting offset of the subregion in the string
		'''                       argument. </param>
		''' <param name="len">          the number of characters to compare. </param>
		''' <returns>  {@code true} if the specified subregion of this string
		'''          matches the specified subregion of the string argument;
		'''          {@code false} otherwise. Whether the matching is exact
		'''          or case insensitive depends on the {@code ignoreCase}
		'''          argument. </returns>
		public Boolean regionMatches(Boolean ignoreCase, Integer toffset, String other, Integer ooffset, Integer len)
			Dim ta As Char() = value
			Dim [to] As Integer = toffset
			Dim pa As Char() = other.value
			Dim po As Integer = ooffset
			' Note: toffset, ooffset, or len might be near -1>>>1.
			If (ooffset < 0) OrElse (toffset < 0) OrElse (toffset > (Long)value.Length - len) OrElse (ooffset > (Long)other.value.length - len) Then Return False
			Dim tempVar4 As Boolean = len > 0
			len -= 1
			Do While tempVar4
				Dim c1 As Char = ta([to])
				[to] += 1
				Dim c2 As Char = pa(po)
				po += 1
				If c1 = c2 Then
					tempVar4 = len > 0
				len -= 1
					Continue Do
				End If
				If ignoreCase Then
					' If characters don't match but case may be ignored,
					' try converting both characters to uppercase.
					' If the results match, then the comparison scan should
					' continue.
					Dim u1 As Char = Char.ToUpper(c1)
					Dim u2 As Char = Char.ToUpper(c2)
					If u1 = u2 Then
						tempVar4 = len > 0
				len -= 1
						Continue Do
					End If
					' Unfortunately, conversion to uppercase does not work properly
					' for the Georgian alphabet, which has strange rules about case
					' conversion.  So we need to make one last check before
					' exiting.
					If Char.ToLower(u1) = Char.ToLower(u2) Then
						tempVar4 = len > 0
				len -= 1
						Continue Do
					End If
				End If
				Return False
				tempVar4 = len > 0
				len -= 1
			Loop
			Return True

		''' <summary>
		''' Tests if the substring of this string beginning at the
		''' specified index starts with the specified prefix.
		''' </summary>
		''' <param name="prefix">    the prefix. </param>
		''' <param name="toffset">   where to begin looking in this string. </param>
		''' <returns>  {@code true} if the character sequence represented by the
		'''          argument is a prefix of the substring of this object starting
		'''          at index {@code toffset}; {@code false} otherwise.
		'''          The result is {@code false} if {@code toffset} is
		'''          negative or greater than the length of this
		'''          {@code String} object; otherwise the result is the same
		'''          as the result of the expression
		'''          <pre>
		'''          this.substring(toffset).startsWith(prefix)
		'''          </pre> </returns>
		public Boolean StartsWith(String prefix, Integer toffset)
			Dim ta As Char() = value
			Dim [to] As Integer = toffset
			Dim pa As Char() = prefix.value
			Dim po As Integer = 0
			Dim pc As Integer = prefix.value.length
			' Note: toffset might be near -1>>>1.
			If (toffset < 0) OrElse (toffset > value.Length - pc) Then Return False
			pc -= 1
			Do While pc >= 0
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim tempVar5 As Boolean = ta([to]++) <> pa(po++)
				If tempVar5 Then Return False
				pc -= 1
			Loop
			Return True

		''' <summary>
		''' Tests if this string starts with the specified prefix.
		''' </summary>
		''' <param name="prefix">   the prefix. </param>
		''' <returns>  {@code true} if the character sequence represented by the
		'''          argument is a prefix of the character sequence represented by
		'''          this string; {@code false} otherwise.
		'''          Note also that {@code true} will be returned if the
		'''          argument is an empty string or is equal to this
		'''          {@code String} object as determined by the
		'''          <seealso cref="#equals(Object)"/> method.
		''' @since   1. 0 </returns>
		public Boolean StartsWith(String prefix)
			Return StartsWith(prefix, 0)

		''' <summary>
		''' Tests if this string ends with the specified suffix.
		''' </summary>
		''' <param name="suffix">   the suffix. </param>
		''' <returns>  {@code true} if the character sequence represented by the
		'''          argument is a suffix of the character sequence represented by
		'''          this object; {@code false} otherwise. Note that the
		'''          result will be {@code true} if the argument is the
		'''          empty string or is equal to this {@code String} object
		'''          as determined by the <seealso cref="#equals(Object)"/> method. </returns>
		public Boolean EndsWith(String suffix)
			Return StartsWith(suffix, value.Length - suffix.value.length)

		''' <summary>
		''' Returns a hash code for this string. The hash code for a
		''' {@code String} object is computed as
		''' <blockquote><pre>
		''' s[0]*31^(n-1) + s[1]*31^(n-2) + ... + s[n-1]
		''' </pre></blockquote>
		''' using {@code int} arithmetic, where {@code s[i]} is the
		''' <i>i</i>th character of the string, {@code n} is the length of
		''' the string, and {@code ^} indicates exponentiation.
		''' (The hash value of the empty string is zero.)
		''' </summary>
		''' <returns>  a hash code value for this object. </returns>
		public Integer GetHashCode()
			Dim h As Integer = hash
			If h = 0 AndAlso value.Length > 0 Then
				Dim val As Char() = value

				For i As Integer = 0 To value.Length - 1
					h = 31 * h + AscW(val(i))
				Next i
				hash = h
			End If
			Return h

		''' <summary>
		''' Returns the index within this string of the first occurrence of
		''' the specified character. If a character with value
		''' {@code ch} occurs in the character sequence represented by
		''' this {@code String} object, then the index (in Unicode
		''' code units) of the first such occurrence is returned. For
		''' values of {@code ch} in the range from 0 to 0xFFFF
		''' (inclusive), this is the smallest value <i>k</i> such that:
		''' <blockquote><pre>
		''' this.charAt(<i>k</i>) == ch
		''' </pre></blockquote>
		''' is true. For other values of {@code ch}, it is the
		''' smallest value <i>k</i> such that:
		''' <blockquote><pre>
		''' this.codePointAt(<i>k</i>) == ch
		''' </pre></blockquote>
		''' is true. In either case, if no such character occurs in this
		''' string, then {@code -1} is returned.
		''' </summary>
		''' <param name="ch">   a character (Unicode code point). </param>
		''' <returns>  the index of the first occurrence of the character in the
		'''          character sequence represented by this object, or
		'''          {@code -1} if the character does not occur. </returns>
		public Integer IndexOf(Integer ch)
			Return IndexOf(ch, 0)

		''' <summary>
		''' Returns the index within this string of the first occurrence of the
		''' specified character, starting the search at the specified index.
		''' <p>
		''' If a character with value {@code ch} occurs in the
		''' character sequence represented by this {@code String}
		''' object at an index no smaller than {@code fromIndex}, then
		''' the index of the first such occurrence is returned. For values
		''' of {@code ch} in the range from 0 to 0xFFFF (inclusive),
		''' this is the smallest value <i>k</i> such that:
		''' <blockquote><pre>
		''' (this.charAt(<i>k</i>) == ch) {@code &&} (<i>k</i> &gt;= fromIndex)
		''' </pre></blockquote>
		''' is true. For other values of {@code ch}, it is the
		''' smallest value <i>k</i> such that:
		''' <blockquote><pre>
		''' (this.codePointAt(<i>k</i>) == ch) {@code &&} (<i>k</i> &gt;= fromIndex)
		''' </pre></blockquote>
		''' is true. In either case, if no such character occurs in this
		''' string at or after position {@code fromIndex}, then
		''' {@code -1} is returned.
		''' 
		''' <p>
		''' There is no restriction on the value of {@code fromIndex}. If it
		''' is negative, it has the same effect as if it were zero: this entire
		''' string may be searched. If it is greater than the length of this
		''' string, it has the same effect as if it were equal to the length of
		''' this string: {@code -1} is returned.
		''' 
		''' <p>All indices are specified in {@code char} values
		''' (Unicode code units).
		''' </summary>
		''' <param name="ch">          a character (Unicode code point). </param>
		''' <param name="fromIndex">   the index to start the search from. </param>
		''' <returns>  the index of the first occurrence of the character in the
		'''          character sequence represented by this object that is greater
		'''          than or equal to {@code fromIndex}, or {@code -1}
		'''          if the character does not occur. </returns>
		public Integer IndexOf(Integer ch, Integer fromIndex)
			Dim max As Integer = value.Length
			If fromIndex < 0 Then
				fromIndex = 0
			ElseIf fromIndex >= max Then
				' Note: fromIndex might be near -1>>>1.
				Return -1
			End If

			If ch < Character.MIN_SUPPLEMENTARY_CODE_POINT Then
				' handle most cases here (ch is a BMP code point or a
				' negative value (invalid code point))
				Dim value As Char() = Me.value
				For i As Integer = fromIndex To max - 1
					If value(i) = ch Then Return i
				Next i
				Return -1
			Else
				Return indexOfSupplementary(ch, fromIndex)
			End If

		''' <summary>
		''' Handles (rare) calls of indexOf with a supplementary character.
		''' </summary>
		private Integer indexOfSupplementary(Integer ch, Integer fromIndex)
			If Character.isValidCodePoint(ch) Then
				Dim value As Char() = Me.value
				Dim hi As Char = Character.highSurrogate(ch)
				Dim lo As Char = Character.lowSurrogate(ch)
				Dim max As Integer = value.Length - 1
				For i As Integer = fromIndex To max - 1
					If value(i) = hi AndAlso value(i + 1) = lo Then Return i
				Next i
			End If
			Return -1

		''' <summary>
		''' Returns the index within this string of the last occurrence of
		''' the specified character. For values of {@code ch} in the
		''' range from 0 to 0xFFFF (inclusive), the index (in Unicode code
		''' units) returned is the largest value <i>k</i> such that:
		''' <blockquote><pre>
		''' this.charAt(<i>k</i>) == ch
		''' </pre></blockquote>
		''' is true. For other values of {@code ch}, it is the
		''' largest value <i>k</i> such that:
		''' <blockquote><pre>
		''' this.codePointAt(<i>k</i>) == ch
		''' </pre></blockquote>
		''' is true.  In either case, if no such character occurs in this
		''' string, then {@code -1} is returned.  The
		''' {@code String} is searched backwards starting at the last
		''' character.
		''' </summary>
		''' <param name="ch">   a character (Unicode code point). </param>
		''' <returns>  the index of the last occurrence of the character in the
		'''          character sequence represented by this object, or
		'''          {@code -1} if the character does not occur. </returns>
		public Integer LastIndexOf(Integer ch)
			Return LastIndexOf(ch, value.Length - 1)

		''' <summary>
		''' Returns the index within this string of the last occurrence of
		''' the specified character, searching backward starting at the
		''' specified index. For values of {@code ch} in the range
		''' from 0 to 0xFFFF (inclusive), the index returned is the largest
		''' value <i>k</i> such that:
		''' <blockquote><pre>
		''' (this.charAt(<i>k</i>) == ch) {@code &&} (<i>k</i> &lt;= fromIndex)
		''' </pre></blockquote>
		''' is true. For other values of {@code ch}, it is the
		''' largest value <i>k</i> such that:
		''' <blockquote><pre>
		''' (this.codePointAt(<i>k</i>) == ch) {@code &&} (<i>k</i> &lt;= fromIndex)
		''' </pre></blockquote>
		''' is true. In either case, if no such character occurs in this
		''' string at or before position {@code fromIndex}, then
		''' {@code -1} is returned.
		''' 
		''' <p>All indices are specified in {@code char} values
		''' (Unicode code units).
		''' </summary>
		''' <param name="ch">          a character (Unicode code point). </param>
		''' <param name="fromIndex">   the index to start the search from. There is no
		'''          restriction on the value of {@code fromIndex}. If it is
		'''          greater than or equal to the length of this string, it has
		'''          the same effect as if it were equal to one less than the
		'''          length of this string: this entire string may be searched.
		'''          If it is negative, it has the same effect as if it were -1:
		'''          -1 is returned. </param>
		''' <returns>  the index of the last occurrence of the character in the
		'''          character sequence represented by this object that is less
		'''          than or equal to {@code fromIndex}, or {@code -1}
		'''          if the character does not occur before that point. </returns>
		public Integer LastIndexOf(Integer ch, Integer fromIndex)
			If ch < Character.MIN_SUPPLEMENTARY_CODE_POINT Then
				' handle most cases here (ch is a BMP code point or a
				' negative value (invalid code point))
				Dim value As Char() = Me.value
				Dim i As Integer = System.Math.Min(fromIndex, value.Length - 1)
				Do While i >= 0
					If value(i) = ch Then Return i
					i -= 1
				Loop
				Return -1
			Else
				Return lastIndexOfSupplementary(ch, fromIndex)
			End If

		''' <summary>
		''' Handles (rare) calls of lastIndexOf with a supplementary character.
		''' </summary>
		private Integer lastIndexOfSupplementary(Integer ch, Integer fromIndex)
			If Character.isValidCodePoint(ch) Then
				Dim value As Char() = Me.value
				Dim hi As Char = Character.highSurrogate(ch)
				Dim lo As Char = Character.lowSurrogate(ch)
				Dim i As Integer = System.Math.Min(fromIndex, value.Length - 2)
				Do While i >= 0
					If value(i) = hi AndAlso value(i + 1) = lo Then Return i
					i -= 1
				Loop
			End If
			Return -1

		''' <summary>
		''' Returns the index within this string of the first occurrence of the
		''' specified substring.
		''' 
		''' <p>The returned index is the smallest value <i>k</i> for which:
		''' <blockquote><pre>
		''' this.startsWith(str, <i>k</i>)
		''' </pre></blockquote>
		''' If no such value of <i>k</i> exists, then {@code -1} is returned.
		''' </summary>
		''' <param name="str">   the substring to search for. </param>
		''' <returns>  the index of the first occurrence of the specified substring,
		'''          or {@code -1} if there is no such occurrence. </returns>
		public Integer IndexOf(String str)
			Return IndexOf(str, 0)

		''' <summary>
		''' Returns the index within this string of the first occurrence of the
		''' specified substring, starting at the specified index.
		''' 
		''' <p>The returned index is the smallest value <i>k</i> for which:
		''' <blockquote><pre>
		''' <i>k</i> &gt;= fromIndex {@code &&} this.startsWith(str, <i>k</i>)
		''' </pre></blockquote>
		''' If no such value of <i>k</i> exists, then {@code -1} is returned.
		''' </summary>
		''' <param name="str">         the substring to search for. </param>
		''' <param name="fromIndex">   the index from which to start the search. </param>
		''' <returns>  the index of the first occurrence of the specified substring,
		'''          starting at the specified index,
		'''          or {@code -1} if there is no such occurrence. </returns>
		public Integer IndexOf(String str, Integer fromIndex)
			Return IndexOf(value, 0, value.Length, str.value, 0, str.value.length, fromIndex)

		''' <summary>
		''' Code shared by String and AbstractStringBuilder to do searches. The
		''' source is the character array being searched, and the target
		''' is the string being searched for.
		''' </summary>
		''' <param name="source">       the characters being searched. </param>
		''' <param name="sourceOffset"> offset of the source string. </param>
		''' <param name="sourceCount">  count of the source string. </param>
		''' <param name="target">       the characters being searched for. </param>
		''' <param name="fromIndex">    the index to begin searching from. </param>
		static Integer IndexOf(Char() source, Integer sourceOffset, Integer sourceCount, String target, Integer fromIndex)
			Return IndexOf(source, sourceOffset, sourceCount, target.value, 0, target.value.length, fromIndex)

		''' <summary>
		''' Code shared by String and StringBuffer to do searches. The
		''' source is the character array being searched, and the target
		''' is the string being searched for.
		''' </summary>
		''' <param name="source">       the characters being searched. </param>
		''' <param name="sourceOffset"> offset of the source string. </param>
		''' <param name="sourceCount">  count of the source string. </param>
		''' <param name="target">       the characters being searched for. </param>
		''' <param name="targetOffset"> offset of the target string. </param>
		''' <param name="targetCount">  count of the target string. </param>
		''' <param name="fromIndex">    the index to begin searching from. </param>
		static Integer IndexOf(Char() source, Integer sourceOffset, Integer sourceCount, Char() target, Integer targetOffset, Integer targetCount, Integer fromIndex)
			If fromIndex >= sourceCount Then Return (If(targetCount = 0, sourceCount, -1))
			If fromIndex < 0 Then fromIndex = 0
			If targetCount = 0 Then Return fromIndex

			Dim first As Char = target(targetOffset)
			Dim max As Integer = sourceOffset + (sourceCount - targetCount)

			For i As Integer = sourceOffset + fromIndex To max
				' Look for first character. 
				If source(i) <> first Then
					i += 1
					Do While i <= max AndAlso source(i) <> first

						i += 1
					Loop
				End If

				' Found first character, now look at the rest of v2 
				If i <= max Then
					Dim j As Integer = i + 1
					Dim [end] As Integer = j + targetCount - 1
					Dim k As Integer = targetOffset + 1
					Do While j < [end] AndAlso source(j) = target(k)

						j += 1
						k += 1
					Loop

					If j = [end] Then Return i - sourceOffset
				End If
			Next i
			Return -1

		''' <summary>
		''' Returns the index within this string of the last occurrence of the
		''' specified substring.  The last occurrence of the empty string ""
		''' is considered to occur at the index value {@code this.length()}.
		''' 
		''' <p>The returned index is the largest value <i>k</i> for which:
		''' <blockquote><pre>
		''' this.startsWith(str, <i>k</i>)
		''' </pre></blockquote>
		''' If no such value of <i>k</i> exists, then {@code -1} is returned.
		''' </summary>
		''' <param name="str">   the substring to search for. </param>
		''' <returns>  the index of the last occurrence of the specified substring,
		'''          or {@code -1} if there is no such occurrence. </returns>
		public Integer LastIndexOf(String str)
			Return LastIndexOf(str, value.Length)

		''' <summary>
		''' Returns the index within this string of the last occurrence of the
		''' specified substring, searching backward starting at the specified index.
		''' 
		''' <p>The returned index is the largest value <i>k</i> for which:
		''' <blockquote><pre>
		''' <i>k</i> {@code <=} fromIndex {@code &&} this.startsWith(str, <i>k</i>)
		''' </pre></blockquote>
		''' If no such value of <i>k</i> exists, then {@code -1} is returned.
		''' </summary>
		''' <param name="str">         the substring to search for. </param>
		''' <param name="fromIndex">   the index to start the search from. </param>
		''' <returns>  the index of the last occurrence of the specified substring,
		'''          searching backward from the specified index,
		'''          or {@code -1} if there is no such occurrence. </returns>
		public Integer LastIndexOf(String str, Integer fromIndex)
			Return LastIndexOf(value, 0, value.Length, str.value, 0, str.value.length, fromIndex)

		''' <summary>
		''' Code shared by String and AbstractStringBuilder to do searches. The
		''' source is the character array being searched, and the target
		''' is the string being searched for.
		''' </summary>
		''' <param name="source">       the characters being searched. </param>
		''' <param name="sourceOffset"> offset of the source string. </param>
		''' <param name="sourceCount">  count of the source string. </param>
		''' <param name="target">       the characters being searched for. </param>
		''' <param name="fromIndex">    the index to begin searching from. </param>
		static Integer LastIndexOf(Char() source, Integer sourceOffset, Integer sourceCount, String target, Integer fromIndex)
			Return LastIndexOf(source, sourceOffset, sourceCount, target.value, 0, target.value.length, fromIndex)

		''' <summary>
		''' Code shared by String and StringBuffer to do searches. The
		''' source is the character array being searched, and the target
		''' is the string being searched for.
		''' </summary>
		''' <param name="source">       the characters being searched. </param>
		''' <param name="sourceOffset"> offset of the source string. </param>
		''' <param name="sourceCount">  count of the source string. </param>
		''' <param name="target">       the characters being searched for. </param>
		''' <param name="targetOffset"> offset of the target string. </param>
		''' <param name="targetCount">  count of the target string. </param>
		''' <param name="fromIndex">    the index to begin searching from. </param>
		static Integer LastIndexOf(Char() source, Integer sourceOffset, Integer sourceCount, Char() target, Integer targetOffset, Integer targetCount, Integer fromIndex)
	'        
	'         * Check arguments; return immediately where possible. For
	'         * consistency, don't check for null str.
	'         
			Dim rightIndex As Integer = sourceCount - targetCount
			If fromIndex < 0 Then Return -1
			If fromIndex > rightIndex Then fromIndex = rightIndex
			' Empty string always matches. 
			If targetCount = 0 Then Return fromIndex

			Dim strLastIndex As Integer = targetOffset + targetCount - 1
			Dim strLastChar As Char = target(strLastIndex)
			Dim min As Integer = sourceOffset + targetCount - 1
			Dim i As Integer = min + fromIndex

		startSearchForLastChar:
			Do
				Do While i >= min AndAlso source(i) <> strLastChar
					i -= 1
				Loop
				If i < min Then Return -1
				Dim j As Integer = i - 1
				Dim start As Integer = j - (targetCount - 1)
				Dim k As Integer = strLastIndex - 1

				Do While j > start
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Dim tempVar6 As Boolean = source(j--) <> target(k--)
					If tempVar6 Then
						i -= 1
						GoTo startSearchForLastChar
					End If
				Loop
				Return start - sourceOffset + 1
			Loop

		''' <summary>
		''' Returns a string that is a substring of this string. The
		''' substring begins with the character at the specified index and
		''' extends to the end of this string. <p>
		''' Examples:
		''' <blockquote><pre>
		''' "unhappy".substring(2) returns "happy"
		''' "Harbison".substring(3) returns "bison"
		''' "emptiness".substring(9) returns "" (an empty string)
		''' </pre></blockquote>
		''' </summary>
		''' <param name="beginIndex">   the beginning index, inclusive. </param>
		''' <returns>     the specified substring. </returns>
		''' <exception cref="IndexOutOfBoundsException">  if
		'''             {@code beginIndex} is negative or larger than the
		'''             length of this {@code String} object. </exception>
		public String Substring(Integer beginIndex)
			If beginIndex < 0 Then Throw New StringIndexOutOfBoundsException(beginIndex)
			Dim subLen As Integer = value.Length - beginIndex
			If subLen < 0 Then Throw New StringIndexOutOfBoundsException(subLen)
			Return If(beginIndex = 0, Me, New String(value, beginIndex, subLen))

		''' <summary>
		''' Returns a string that is a substring of this string. The
		''' substring begins at the specified {@code beginIndex} and
		''' extends to the character at index {@code endIndex - 1}.
		''' Thus the length of the substring is {@code endIndex-beginIndex}.
		''' <p>
		''' Examples:
		''' <blockquote><pre>
		''' "hamburger".substring(4, 8) returns "urge"
		''' "smiles".substring(1, 5) returns "mile"
		''' </pre></blockquote>
		''' </summary>
		''' <param name="beginIndex">   the beginning index, inclusive. </param>
		''' <param name="endIndex">     the ending index, exclusive. </param>
		''' <returns>     the specified substring. </returns>
		''' <exception cref="IndexOutOfBoundsException">  if the
		'''             {@code beginIndex} is negative, or
		'''             {@code endIndex} is larger than the length of
		'''             this {@code String} object, or
		'''             {@code beginIndex} is larger than
		'''             {@code endIndex}. </exception>
		public String Substring(Integer beginIndex, Integer endIndex)
			If beginIndex < 0 Then Throw New StringIndexOutOfBoundsException(beginIndex)
			If endIndex > value.Length Then Throw New StringIndexOutOfBoundsException(endIndex)
			Dim subLen As Integer = endIndex - beginIndex
			If subLen < 0 Then Throw New StringIndexOutOfBoundsException(subLen)
			Return If((beginIndex = 0) AndAlso (endIndex = value.Length), Me, New String(value, beginIndex, subLen))

		''' <summary>
		''' Returns a character sequence that is a subsequence of this sequence.
		''' 
		''' <p> An invocation of this method of the form
		''' 
		''' <blockquote><pre>
		''' str.subSequence(begin,&nbsp;end)</pre></blockquote>
		''' 
		''' behaves in exactly the same way as the invocation
		''' 
		''' <blockquote><pre>
		''' str.substring(begin,&nbsp;end)</pre></blockquote>
		''' 
		''' @apiNote
		''' This method is defined so that the {@code String} class can implement
		''' the <seealso cref="CharSequence"/> interface.
		''' </summary>
		''' <param name="beginIndex">   the begin index, inclusive. </param>
		''' <param name="endIndex">     the end index, exclusive. </param>
		''' <returns>  the specified subsequence.
		''' </returns>
		''' <exception cref="IndexOutOfBoundsException">
		'''          if {@code beginIndex} or {@code endIndex} is negative,
		'''          if {@code endIndex} is greater than {@code length()},
		'''          or if {@code beginIndex} is greater than {@code endIndex}
		''' 
		''' @since 1.4
		''' @spec JSR-51 </exception>
		public CharSequence subSequence(Integer beginIndex, Integer endIndex)
			Return Me.Substring(beginIndex, endIndex - beginIndex)

		''' <summary>
		''' Concatenates the specified string to the end of this string.
		''' <p>
		''' If the length of the argument string is {@code 0}, then this
		''' {@code String} object is returned. Otherwise, a
		''' {@code String} object is returned that represents a character
		''' sequence that is the concatenation of the character sequence
		''' represented by this {@code String} object and the character
		''' sequence represented by the argument string.<p>
		''' Examples:
		''' <blockquote><pre>
		''' "cares".concat("s") returns "caress"
		''' "to".concat("get").concat("her") returns "together"
		''' </pre></blockquote>
		''' </summary>
		''' <param name="str">   the {@code String} that is concatenated to the end
		'''                of this {@code String}. </param>
		''' <returns>  a string that represents the concatenation of this object's
		'''          characters followed by the string argument's characters. </returns>
		public String concat(String str)
			Dim otherLen As Integer = str.length()
			If otherLen = 0 Then Return Me
			Dim len As Integer = value.Length
			Dim buf As Char() = java.util.Arrays.copyOf(value, len + otherLen)
			str.getChars(buf, len)
			Return New String(buf, True)

		''' <summary>
		''' Returns a string resulting from replacing all occurrences of
		''' {@code oldChar} in this string with {@code newChar}.
		''' <p>
		''' If the character {@code oldChar} does not occur in the
		''' character sequence represented by this {@code String} object,
		''' then a reference to this {@code String} object is returned.
		''' Otherwise, a {@code String} object is returned that
		''' represents a character sequence identical to the character sequence
		''' represented by this {@code String} object, except that every
		''' occurrence of {@code oldChar} is replaced by an occurrence
		''' of {@code newChar}.
		''' <p>
		''' Examples:
		''' <blockquote><pre>
		''' "mesquite in your cellar".replace('e', 'o')
		'''         returns "mosquito in your collar"
		''' "the war of baronets".replace('r', 'y')
		'''         returns "the way of bayonets"
		''' "sparring with a purple porpoise".replace('p', 't')
		'''         returns "starring with a turtle tortoise"
		''' "JonL".replace('q', 'x') returns "JonL" (no change)
		''' </pre></blockquote>
		''' </summary>
		''' <param name="oldChar">   the old character. </param>
		''' <param name="newChar">   the new character. </param>
		''' <returns>  a string derived from this string by replacing every
		'''          occurrence of {@code oldChar} with {@code newChar}. </returns>
		public String replace(Char oldChar, Char newChar)
			If oldChar <> newChar Then
				Dim len As Integer = value.Length
				Dim i As Integer = -1
				Dim val As Char() = value ' avoid getfield opcode

				i += 1
				Do While i < len
					If val(i) = oldChar Then Exit Do
					i += 1
				Loop
				If i < len Then
					Dim buf As Char() = New Char(len - 1){}
					For j As Integer = 0 To i - 1
						buf(j) = val(j)
					Next j
					Do While i < len
						Dim c As Char = val(i)
						buf(i) = If(c = oldChar, newChar, c)
						i += 1
					Loop
					Return New String(buf, True)
				End If
			End If
			Return Me

		''' <summary>
		''' Tells whether or not this string matches the given <a
		''' href="../util/regex/Pattern.html#sum">regular expression</a>.
		''' 
		''' <p> An invocation of this method of the form
		''' <i>str</i>{@code .matches(}<i>regex</i>{@code )} yields exactly the
		''' same result as the expression
		''' 
		''' <blockquote>
		''' <seealso cref="java.util.regex.Pattern"/>.{@link java.util.regex.Pattern#matches(String,CharSequence)
		''' matches(<i>regex</i>, <i>str</i>)}
		''' </blockquote>
		''' </summary>
		''' <param name="regex">
		'''          the regular expression to which this string is to be matched
		''' </param>
		''' <returns>  {@code true} if, and only if, this string matches the
		'''          given regular expression
		''' </returns>
		''' <exception cref="PatternSyntaxException">
		'''          if the regular expression's syntax is invalid
		''' </exception>
		''' <seealso cref= java.util.regex.Pattern
		''' 
		''' @since 1.4
		''' @spec JSR-51 </seealso>
		public Boolean matches(String regex)
			Return java.util.regex.Pattern.matches(regex, Me)

		''' <summary>
		''' Returns true if and only if this string contains the specified
		''' sequence of char values.
		''' </summary>
		''' <param name="s"> the sequence to search for </param>
		''' <returns> true if this string contains {@code s}, false otherwise
		''' @since 1.5 </returns>
		public Boolean contains(CharSequence s)
			Return IndexOf(s.ToString()) > -1

		''' <summary>
		''' Replaces the first substring of this string that matches the given <a
		''' href="../util/regex/Pattern.html#sum">regular expression</a> with the
		''' given replacement.
		''' 
		''' <p> An invocation of this method of the form
		''' <i>str</i>{@code .replaceFirst(}<i>regex</i>{@code ,} <i>repl</i>{@code )}
		''' yields exactly the same result as the expression
		''' 
		''' <blockquote>
		''' <code>
		''' <seealso cref="java.util.regex.Pattern"/>.{@link
		''' java.util.regex.Pattern#compile compile}(<i>regex</i>).{@link
		''' java.util.regex.Pattern#matcher(java.lang.CharSequence) matcher}(<i>str</i>).{@link
		''' java.util.regex.Matcher#replaceFirst replaceFirst}(<i>repl</i>)
		''' </code>
		''' </blockquote>
		''' 
		''' <p>
		''' Note that backslashes ({@code \}) and dollar signs ({@code $}) in the
		''' replacement string may cause the results to be different than if it were
		''' being treated as a literal replacement string; see
		''' <seealso cref="java.util.regex.Matcher#replaceFirst"/>.
		''' Use <seealso cref="java.util.regex.Matcher#quoteReplacement"/> to suppress the special
		''' meaning of these characters, if desired.
		''' </summary>
		''' <param name="regex">
		'''          the regular expression to which this string is to be matched </param>
		''' <param name="replacement">
		'''          the string to be substituted for the first match
		''' </param>
		''' <returns>  The resulting {@code String}
		''' </returns>
		''' <exception cref="PatternSyntaxException">
		'''          if the regular expression's syntax is invalid
		''' </exception>
		''' <seealso cref= java.util.regex.Pattern
		''' 
		''' @since 1.4
		''' @spec JSR-51 </seealso>
		public String replaceFirst(String regex, String replacement)
			Return java.util.regex.Pattern.compile(regex).matcher(Me).replaceFirst(replacement)

		''' <summary>
		''' Replaces each substring of this string that matches the given <a
		''' href="../util/regex/Pattern.html#sum">regular expression</a> with the
		''' given replacement.
		''' 
		''' <p> An invocation of this method of the form
		''' <i>str</i>{@code .replaceAll(}<i>regex</i>{@code ,} <i>repl</i>{@code )}
		''' yields exactly the same result as the expression
		''' 
		''' <blockquote>
		''' <code>
		''' <seealso cref="java.util.regex.Pattern"/>.{@link
		''' java.util.regex.Pattern#compile compile}(<i>regex</i>).{@link
		''' java.util.regex.Pattern#matcher(java.lang.CharSequence) matcher}(<i>str</i>).{@link
		''' java.util.regex.Matcher#replaceAll replaceAll}(<i>repl</i>)
		''' </code>
		''' </blockquote>
		''' 
		''' <p>
		''' Note that backslashes ({@code \}) and dollar signs ({@code $}) in the
		''' replacement string may cause the results to be different than if it were
		''' being treated as a literal replacement string; see
		''' <seealso cref="java.util.regex.Matcher#replaceAll Matcher.replaceAll"/>.
		''' Use <seealso cref="java.util.regex.Matcher#quoteReplacement"/> to suppress the special
		''' meaning of these characters, if desired.
		''' </summary>
		''' <param name="regex">
		'''          the regular expression to which this string is to be matched </param>
		''' <param name="replacement">
		'''          the string to be substituted for each match
		''' </param>
		''' <returns>  The resulting {@code String}
		''' </returns>
		''' <exception cref="PatternSyntaxException">
		'''          if the regular expression's syntax is invalid
		''' </exception>
		''' <seealso cref= java.util.regex.Pattern
		''' 
		''' @since 1.4
		''' @spec JSR-51 </seealso>
		public String replaceAll(String regex, String replacement)
			Return java.util.regex.Pattern.compile(regex).matcher(Me).replaceAll(replacement)

		''' <summary>
		''' Replaces each substring of this string that matches the literal target
		''' sequence with the specified literal replacement sequence. The
		''' replacement proceeds from the beginning of the string to the end, for
		''' example, replacing "aa" with "b" in the string "aaa" will result in
		''' "ba" rather than "ab".
		''' </summary>
		''' <param name="target"> The sequence of char values to be replaced </param>
		''' <param name="replacement"> The replacement sequence of char values </param>
		''' <returns>  The resulting string
		''' @since 1.5 </returns>
		public String replace(CharSequence target, CharSequence replacement)
			Return java.util.regex.Pattern.compile(target.ToString(), java.util.regex.Pattern.LITERAL).matcher(Me).replaceAll(java.util.regex.Matcher.quoteReplacement(replacement.ToString()))

		''' <summary>
		''' Splits this string around matches of the given
		''' <a href="../util/regex/Pattern.html#sum">regular expression</a>.
		''' 
		''' <p> The array returned by this method contains each substring of this
		''' string that is terminated by another substring that matches the given
		''' expression or is terminated by the end of the string.  The substrings in
		''' the array are in the order in which they occur in this string.  If the
		''' expression does not match any part of the input then the resulting array
		''' has just one element, namely this string.
		''' 
		''' <p> When there is a positive-width match at the beginning of this
		''' string then an empty leading substring is included at the beginning
		''' of the resulting array. A zero-width match at the beginning however
		''' never produces such empty leading substring.
		''' 
		''' <p> The {@code limit} parameter controls the number of times the
		''' pattern is applied and therefore affects the length of the resulting
		''' array.  If the limit <i>n</i> is greater than zero then the pattern
		''' will be applied at most <i>n</i>&nbsp;-&nbsp;1 times, the array's
		''' length will be no greater than <i>n</i>, and the array's last entry
		''' will contain all input beyond the last matched delimiter.  If <i>n</i>
		''' is non-positive then the pattern will be applied as many times as
		''' possible and the array can have any length.  If <i>n</i> is zero then
		''' the pattern will be applied as many times as possible, the array can
		''' have any length, and trailing empty strings will be discarded.
		''' 
		''' <p> The string {@code "boo:and:foo"}, for example, yields the
		''' following results with these parameters:
		''' 
		''' <blockquote><table cellpadding=1 cellspacing=0 summary="Split example showing regex, limit, and result">
		''' <tr>
		'''     <th>Regex</th>
		'''     <th>Limit</th>
		'''     <th>Result</th>
		''' </tr>
		''' <tr><td align=center>:</td>
		'''     <td align=center>2</td>
		'''     <td>{@code { "boo", "and:foo" }}</td></tr>
		''' <tr><td align=center>:</td>
		'''     <td align=center>5</td>
		'''     <td>{@code { "boo", "and", "foo" }}</td></tr>
		''' <tr><td align=center>:</td>
		'''     <td align=center>-2</td>
		'''     <td>{@code { "boo", "and", "foo" }}</td></tr>
		''' <tr><td align=center>o</td>
		'''     <td align=center>5</td>
		'''     <td>{@code { "b", "", ":and:f", "", "" }}</td></tr>
		''' <tr><td align=center>o</td>
		'''     <td align=center>-2</td>
		'''     <td>{@code { "b", "", ":and:f", "", "" }}</td></tr>
		''' <tr><td align=center>o</td>
		'''     <td align=center>0</td>
		'''     <td>{@code { "b", "", ":and:f" }}</td></tr>
		''' </table></blockquote>
		''' 
		''' <p> An invocation of this method of the form
		''' <i>str.</i>{@code split(}<i>regex</i>{@code ,}&nbsp;<i>n</i>{@code )}
		''' yields the same result as the expression
		''' 
		''' <blockquote>
		''' <code>
		''' <seealso cref="java.util.regex.Pattern"/>.{@link
		''' java.util.regex.Pattern#compile compile}(<i>regex</i>).{@link
		''' java.util.regex.Pattern#split(java.lang.CharSequence,int) split}(<i>str</i>,&nbsp;<i>n</i>)
		''' </code>
		''' </blockquote>
		''' 
		''' </summary>
		''' <param name="regex">
		'''         the delimiting regular expression
		''' </param>
		''' <param name="limit">
		'''         the result threshold, as described above
		''' </param>
		''' <returns>  the array of strings computed by splitting this string
		'''          around matches of the given regular expression
		''' </returns>
		''' <exception cref="PatternSyntaxException">
		'''          if the regular expression's syntax is invalid
		''' </exception>
		''' <seealso cref= java.util.regex.Pattern
		''' 
		''' @since 1.4
		''' @spec JSR-51 </seealso>
		public String() Split(String regex, Integer limit)
	'         fastpath if the regex is a
	'         (1)one-char String and this character is not one of the
	'            RegEx's meta characters ".$|()[{^?*+\\", or
	'         (2)two-char String and the first char is the backslash and
	'            the second is not the ascii digit or ascii letter.
	'         
			Dim ch As Char = 0
			ch = regex.Chars(0)
			ch = regex.Chars(1)
			If ((regex.value.length = 1 AndAlso ".$|()[{^?*+\".indexOfch = -1) OrElse (regex.length() = 2 AndAlso regex.Chars(0) = "\"c AndAlso ((AscW(ch) -AscW("0"c)) Or (AscW("9"c)-AscW(ch))) < 0 AndAlso ((AscW(ch)-AscW("a"c)) Or (AscW("z"c)-AscW(ch))) < 0 AndAlso ((AscW(ch)-AscW("A"c)) Or (AscW("Z"c)-AscW(ch))) < 0)) AndAlso (ch < Character.MIN_HIGH_SURROGATE OrElse ch > Character.MAX_LOW_SURROGATE) Then
				Dim [off] As Integer = 0
				Dim [next] As Integer = 0
				Dim limited As Boolean = limit > 0
				Dim list As New List(Of String)
				[next] = IndexOf(ch, [off])
				Do While [next] <> -1
					If (Not limited) OrElse list.Count < limit - 1 Then
						list.Add(Substring([off], [next]))
						[off] = [next] + 1 ' last one
					Else
						'assert (list.size() == limit - 1);
						list.Add(Substring([off], value.Length))
						[off] = value.Length
						Exit Do
					End If
					[next] = IndexOf(ch, [off])
				Loop
				' If no match was found, return this
				If [off] = 0 Then Return New String(){Me}

				' Add remaining segment
				If (Not limited) OrElse list.Count < limit Then list.Add(Substring([off], value.Length))

				' Construct result
				Dim resultSize As Integer = list.Count
				If limit = 0 Then
					Do While resultSize > 0 AndAlso list(resultSize - 1).length() = 0
						resultSize -= 1
					Loop
				End If
				Dim result As String() = New String(resultSize - 1){}
				Return list.subList(0, resultSize).ToArray(result)
			End If
			Return java.util.regex.Pattern.compile(regex).Split(Me, limit)

		''' <summary>
		''' Splits this string around matches of the given <a
		''' href="../util/regex/Pattern.html#sum">regular expression</a>.
		''' 
		''' <p> This method works as if by invoking the two-argument {@link
		''' #split(String, int) split} method with the given expression and a limit
		''' argument of zero.  Trailing empty strings are therefore not included in
		''' the resulting array.
		''' 
		''' <p> The string {@code "boo:and:foo"}, for example, yields the following
		''' results with these expressions:
		''' 
		''' <blockquote><table cellpadding=1 cellspacing=0 summary="Split examples showing regex and result">
		''' <tr>
		'''  <th>Regex</th>
		'''  <th>Result</th>
		''' </tr>
		''' <tr><td align=center>:</td>
		'''     <td>{@code { "boo", "and", "foo" }}</td></tr>
		''' <tr><td align=center>o</td>
		'''     <td>{@code { "b", "", ":and:f" }}</td></tr>
		''' </table></blockquote>
		''' 
		''' </summary>
		''' <param name="regex">
		'''         the delimiting regular expression
		''' </param>
		''' <returns>  the array of strings computed by splitting this string
		'''          around matches of the given regular expression
		''' </returns>
		''' <exception cref="PatternSyntaxException">
		'''          if the regular expression's syntax is invalid
		''' </exception>
		''' <seealso cref= java.util.regex.Pattern
		''' 
		''' @since 1.4
		''' @spec JSR-51 </seealso>
		public String() Split(String regex)
			Return Split(regex, 0)

		''' <summary>
		''' Returns a new String composed of copies of the
		''' {@code CharSequence elements} joined together with a copy of
		''' the specified {@code delimiter}.
		''' 
		''' <blockquote>For example,
		''' <pre>{@code
		'''     String message = String.join("-", "Java", "is", "cool");
		'''     // message returned is: "Java-is-cool"
		''' }</pre></blockquote>
		''' 
		''' Note that if an element is null, then {@code "null"} is added.
		''' </summary>
		''' <param name="delimiter"> the delimiter that separates each element </param>
		''' <param name="elements"> the elements to join together.
		''' </param>
		''' <returns> a new {@code String} that is composed of the {@code elements}
		'''         separated by the {@code delimiter}
		''' </returns>
		''' <exception cref="NullPointerException"> If {@code delimiter} or {@code elements}
		'''         is {@code null}
		''' </exception>
		''' <seealso cref= java.util.StringJoiner
		''' @since 1.8 </seealso>
		public static String join(CharSequence delimiter, CharSequence... elements)
			java.util.Objects.requireNonNull(delimiter)
			java.util.Objects.requireNonNull(elements)
			' Number of elements not likely worth Arrays.stream overhead.
			Dim joiner As New java.util.StringJoiner(delimiter)
			For Each cs As CharSequence In elements
				joiner.add(cs)
			Next cs
			Return joiner.ToString()

		''' <summary>
		''' Returns a new {@code String} composed of copies of the
		''' {@code CharSequence elements} joined together with a copy of the
		''' specified {@code delimiter}.
		''' 
		''' <blockquote>For example,
		''' <pre>{@code
		'''     List<String> strings = new LinkedList<>();
		'''     strings.add("Java");strings.add("is");
		'''     strings.add("cool");
		'''     String message = String.join(" ", strings);
		'''     //message returned is: "Java is cool"
		''' 
		'''     Set<String> strings = new LinkedHashSet<>();
		'''     strings.add("Java"); strings.add("is");
		'''     strings.add("very"); strings.add("cool");
		'''     String message = String.join("-", strings);
		'''     //message returned is: "Java-is-very-cool"
		''' }</pre></blockquote>
		''' 
		''' Note that if an individual element is {@code null}, then {@code "null"} is added.
		''' </summary>
		''' <param name="delimiter"> a sequence of characters that is used to separate each
		'''         of the {@code elements} in the resulting {@code String} </param>
		''' <param name="elements"> an {@code Iterable} that will have its {@code elements}
		'''         joined together.
		''' </param>
		''' <returns> a new {@code String} that is composed from the {@code elements}
		'''         argument
		''' </returns>
		''' <exception cref="NullPointerException"> If {@code delimiter} or {@code elements}
		'''         is {@code null}
		''' </exception>
		''' <seealso cref=    #join(CharSequence,CharSequence...) </seealso>
		''' <seealso cref=    java.util.StringJoiner
		''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static String join(CharSequence delimiter, Iterable(Of ? As CharSequence) elements)
			java.util.Objects.requireNonNull(delimiter)
			java.util.Objects.requireNonNull(elements)
			Dim joiner As New java.util.StringJoiner(delimiter)
			For Each cs As CharSequence In elements
				joiner.add(cs)
			Next cs
			Return joiner.ToString()

		''' <summary>
		''' Converts all of the characters in this {@code String} to lower
		''' case using the rules of the given {@code Locale}.  Case mapping is based
		''' on the Unicode Standard version specified by the <seealso cref="java.lang.Character Character"/>
		''' class. Since case mappings are not always 1:1 char mappings, the resulting
		''' {@code String} may be a different length than the original {@code String}.
		''' <p>
		''' Examples of lowercase  mappings are in the following table:
		''' <table border="1" summary="Lowercase mapping examples showing language code of locale, upper case, lower case, and description">
		''' <tr>
		'''   <th>Language Code of Locale</th>
		'''   <th>Upper Case</th>
		'''   <th>Lower Case</th>
		'''   <th>Description</th>
		''' </tr>
		''' <tr>
		'''   <td>tr (Turkish)</td>
		'''   <td>&#92;u0130</td>
		'''   <td>&#92;u0069</td>
		'''   <td>capital letter I with dot above -&gt; small letter i</td>
		''' </tr>
		''' <tr>
		'''   <td>tr (Turkish)</td>
		'''   <td>&#92;u0049</td>
		'''   <td>&#92;u0131</td>
		'''   <td>capital letter I -&gt; small letter dotless i </td>
		''' </tr>
		''' <tr>
		'''   <td>(all)</td>
		'''   <td>French Fries</td>
		'''   <td>french fries</td>
		'''   <td>lowercased all chars in String</td>
		''' </tr>
		''' <tr>
		'''   <td>(all)</td>
		'''   <td><img src="doc-files/capiota.gif" alt="capiota"><img src="doc-files/capchi.gif" alt="capchi">
		'''       <img src="doc-files/captheta.gif" alt="captheta"><img src="doc-files/capupsil.gif" alt="capupsil">
		'''       <img src="doc-files/capsigma.gif" alt="capsigma"></td>
		'''   <td><img src="doc-files/iota.gif" alt="iota"><img src="doc-files/chi.gif" alt="chi">
		'''       <img src="doc-files/theta.gif" alt="theta"><img src="doc-files/upsilon.gif" alt="upsilon">
		'''       <img src="doc-files/sigma1.gif" alt="sigma"></td>
		'''   <td>lowercased all chars in String</td>
		''' </tr>
		''' </table>
		''' </summary>
		''' <param name="locale"> use the case transformation rules for this locale </param>
		''' <returns> the {@code String}, converted to lowercase. </returns>
		''' <seealso cref=     java.lang.String#toLowerCase() </seealso>
		''' <seealso cref=     java.lang.String#toUpperCase() </seealso>
		''' <seealso cref=     java.lang.String#toUpperCase(Locale)
		''' @since   1.1 </seealso>
		public String ToLower(java.util.Locale locale)
			If locale Is Nothing Then Throw New NullPointerException

			Dim firstUpper As Integer
			Dim len As Integer = value.Length

			' Now check if there are any characters that need to be changed. 
			scan:
				firstUpper = 0
				Do While firstUpper < len
					Dim c As Char = value(firstUpper)
					If (c >= Character.MIN_HIGH_SURROGATE) AndAlso (c <= Character.MAX_HIGH_SURROGATE) Then
						Dim supplChar As Integer = codePointAt(firstUpper)
						If supplChar <> Char.ToLower(supplChar) Then GoTo scan
						firstUpper += Character.charCount(supplChar)
					Else
						If c <> Char.ToLower(c) Then GoTo scan
						firstUpper += 1
					End If
				Loop
				Return Me

			Dim result As Char() = New Char(len - 1){}
			Dim resultOffset As Integer = 0 ' result may grow, so i+resultOffset
	'                                * is the write location in result 

			' Just copy the first few lowerCase characters. 
			Array.Copy(value, 0, result, 0, firstUpper)

			Dim lang As String = locale.language
			Dim localeDependent As Boolean = (lang = "tr" OrElse lang = "az" OrElse lang = "lt")
			Dim lowerCharArray As Char()
			Dim lowerChar As Integer
			Dim srcChar As Integer
			Dim srcCount As Integer
			For i As Integer = firstUpper To len - 1 Step srcCount
				srcChar = AscW(value(i))
				If ChrW(srcChar) >= Character.MIN_HIGH_SURROGATE AndAlso ChrW(srcChar) <= Character.MAX_HIGH_SURROGATE Then
					srcChar = codePointAt(i)
					srcCount = Character.charCount(srcChar)
				Else
					srcCount = 1
				End If
				If localeDependent OrElse srcChar = ChrW(&H03A3) OrElse srcChar = ChrW(&H0130) Then ' LATIN CAPITAL LETTER I WITH DOT ABOVE -  GREEK CAPITAL LETTER SIGMA
					lowerChar = ConditionalSpecialCasing.toLowerCaseEx(Me, i, locale)
				Else
					lowerChar = AscW(Char.ToLower(srcChar))
				End If
				If (lowerChar = Character.ERROR) OrElse (lowerChar >= Character.MIN_SUPPLEMENTARY_CODE_POINT) Then
					If lowerChar = Character.ERROR Then
						lowerCharArray = ConditionalSpecialCasing.toLowerCaseCharArray(Me, i, locale)
					ElseIf srcCount = 2 Then
						resultOffset += Character.toChars(lowerChar, result, i + resultOffset) - srcCount
						Continue For
					Else
						lowerCharArray = Character.toChars(lowerChar)
					End If

					' Grow result if needed 
					Dim mapLen As Integer = lowerCharArray.Length
					If mapLen > srcCount Then
						Dim result2 As Char() = New Char(result.Length + mapLen - srcCount - 1){}
						Array.Copy(result, 0, result2, 0, i + resultOffset)
						result = result2
					End If
					For x As Integer = 0 To mapLen - 1
						result(i + resultOffset + x) = lowerCharArray(x)
					Next x
					resultOffset += (mapLen - srcCount)
				Else
					result(i + resultOffset) = ChrW(lowerChar)
				End If
			Next i
			Return New String(result, 0, len + resultOffset)

		''' <summary>
		''' Converts all of the characters in this {@code String} to lower
		''' case using the rules of the default locale. This is equivalent to calling
		''' {@code toLowerCase(Locale.getDefault())}.
		''' <p>
		''' <b>Note:</b> This method is locale sensitive, and may produce unexpected
		''' results if used for strings that are intended to be interpreted locale
		''' independently.
		''' Examples are programming language identifiers, protocol keys, and HTML
		''' tags.
		''' For instance, {@code "TITLE".toLowerCase()} in a Turkish locale
		''' returns {@code "t\u005Cu0131tle"}, where '\u005Cu0131' is the
		''' LATIN SMALL LETTER DOTLESS I character.
		''' To obtain correct results for locale insensitive strings, use
		''' {@code toLowerCase(Locale.ROOT)}.
		''' <p> </summary>
		''' <returns>  the {@code String}, converted to lowercase. </returns>
		''' <seealso cref=     java.lang.String#toLowerCase(Locale) </seealso>
		public String ToLower()
			Return ToLower(java.util.Locale.default)

		''' <summary>
		''' Converts all of the characters in this {@code String} to upper
		''' case using the rules of the given {@code Locale}. Case mapping is based
		''' on the Unicode Standard version specified by the <seealso cref="java.lang.Character Character"/>
		''' class. Since case mappings are not always 1:1 char mappings, the resulting
		''' {@code String} may be a different length than the original {@code String}.
		''' <p>
		''' Examples of locale-sensitive and 1:M case mappings are in the following table.
		''' 
		''' <table border="1" summary="Examples of locale-sensitive and 1:M case mappings. Shows Language code of locale, lower case, upper case, and description.">
		''' <tr>
		'''   <th>Language Code of Locale</th>
		'''   <th>Lower Case</th>
		'''   <th>Upper Case</th>
		'''   <th>Description</th>
		''' </tr>
		''' <tr>
		'''   <td>tr (Turkish)</td>
		'''   <td>&#92;u0069</td>
		'''   <td>&#92;u0130</td>
		'''   <td>small letter i -&gt; capital letter I with dot above</td>
		''' </tr>
		''' <tr>
		'''   <td>tr (Turkish)</td>
		'''   <td>&#92;u0131</td>
		'''   <td>&#92;u0049</td>
		'''   <td>small letter dotless i -&gt; capital letter I</td>
		''' </tr>
		''' <tr>
		'''   <td>(all)</td>
		'''   <td>&#92;u00df</td>
		'''   <td>&#92;u0053 &#92;u0053</td>
		'''   <td>small letter sharp s -&gt; two letters: SS</td>
		''' </tr>
		''' <tr>
		'''   <td>(all)</td>
		'''   <td>Fahrvergn&uuml;gen</td>
		'''   <td>FAHRVERGN&Uuml;GEN</td>
		'''   <td></td>
		''' </tr>
		''' </table> </summary>
		''' <param name="locale"> use the case transformation rules for this locale </param>
		''' <returns> the {@code String}, converted to uppercase. </returns>
		''' <seealso cref=     java.lang.String#toUpperCase() </seealso>
		''' <seealso cref=     java.lang.String#toLowerCase() </seealso>
		''' <seealso cref=     java.lang.String#toLowerCase(Locale)
		''' @since   1.1 </seealso>
		public String ToUpper(java.util.Locale locale)
			If locale Is Nothing Then Throw New NullPointerException

			Dim firstLower As Integer
			Dim len As Integer = value.Length

			' Now check if there are any characters that need to be changed. 
			scan:
				firstLower = 0
				Do While firstLower < len
					Dim c As Integer = AscW(value(firstLower))
					Dim srcCount As Integer
					If (c >= Character.MIN_HIGH_SURROGATE) AndAlso (c <= Character.MAX_HIGH_SURROGATE) Then
						c = codePointAt(firstLower)
						srcCount = Character.charCount(c)
					Else
						srcCount = 1
					End If
					Dim upperCaseChar As Integer = Character.toUpperCaseEx(c)
					If (upperCaseChar = Character.ERROR) OrElse (c <> upperCaseChar) Then GoTo scan
					firstLower += srcCount
				Loop
				Return Me

			' result may grow, so i+resultOffset is the write location in result 
			Dim resultOffset As Integer = 0
			Dim result As Char() = New Char(len - 1){} ' may grow

			' Just copy the first few upperCase characters. 
			Array.Copy(value, 0, result, 0, firstLower)

			Dim lang As String = locale.language
			Dim localeDependent As Boolean = (lang = "tr" OrElse lang = "az" OrElse lang = "lt")
			Dim upperCharArray As Char()
			Dim upperChar As Integer
			Dim srcChar As Integer
			Dim srcCount As Integer
			For i As Integer = firstLower To len - 1 Step srcCount
				srcChar = AscW(value(i))
				If ChrW(srcChar) >= Character.MIN_HIGH_SURROGATE AndAlso ChrW(srcChar) <= Character.MAX_HIGH_SURROGATE Then
					srcChar = codePointAt(i)
					srcCount = Character.charCount(srcChar)
				Else
					srcCount = 1
				End If
				If localeDependent Then
					upperChar = ConditionalSpecialCasing.toUpperCaseEx(Me, i, locale)
				Else
					upperChar = Character.toUpperCaseEx(srcChar)
				End If
				If (upperChar = Character.ERROR) OrElse (upperChar >= Character.MIN_SUPPLEMENTARY_CODE_POINT) Then
					If upperChar = Character.ERROR Then
						If localeDependent Then
							upperCharArray = ConditionalSpecialCasing.toUpperCaseCharArray(Me, i, locale)
						Else
							upperCharArray = Character.toUpperCaseCharArray(srcChar)
						End If
					ElseIf srcCount = 2 Then
						resultOffset += Character.toChars(upperChar, result, i + resultOffset) - srcCount
						Continue For
					Else
						upperCharArray = Character.toChars(upperChar)
					End If

					' Grow result if needed 
					Dim mapLen As Integer = upperCharArray.Length
					If mapLen > srcCount Then
						Dim result2 As Char() = New Char(result.Length + mapLen - srcCount - 1){}
						Array.Copy(result, 0, result2, 0, i + resultOffset)
						result = result2
					End If
					For x As Integer = 0 To mapLen - 1
						result(i + resultOffset + x) = upperCharArray(x)
					Next x
					resultOffset += (mapLen - srcCount)
				Else
					result(i + resultOffset) = ChrW(upperChar)
				End If
			Next i
			Return New String(result, 0, len + resultOffset)

		''' <summary>
		''' Converts all of the characters in this {@code String} to upper
		''' case using the rules of the default locale. This method is equivalent to
		''' {@code toUpperCase(Locale.getDefault())}.
		''' <p>
		''' <b>Note:</b> This method is locale sensitive, and may produce unexpected
		''' results if used for strings that are intended to be interpreted locale
		''' independently.
		''' Examples are programming language identifiers, protocol keys, and HTML
		''' tags.
		''' For instance, {@code "title".toUpperCase()} in a Turkish locale
		''' returns {@code "T\u005Cu0130TLE"}, where '\u005Cu0130' is the
		''' LATIN CAPITAL LETTER I WITH DOT ABOVE character.
		''' To obtain correct results for locale insensitive strings, use
		''' {@code toUpperCase(Locale.ROOT)}.
		''' <p> </summary>
		''' <returns>  the {@code String}, converted to uppercase. </returns>
		''' <seealso cref=     java.lang.String#toUpperCase(Locale) </seealso>
		public String ToUpper()
			Return ToUpper(java.util.Locale.default)

		''' <summary>
		''' Returns a string whose value is this string, with any leading and trailing
		''' whitespace removed.
		''' <p>
		''' If this {@code String} object represents an empty character
		''' sequence, or the first and last characters of character sequence
		''' represented by this {@code String} object both have codes
		''' greater than {@code '\u005Cu0020'} (the space character), then a
		''' reference to this {@code String} object is returned.
		''' <p>
		''' Otherwise, if there is no character with a code greater than
		''' {@code '\u005Cu0020'} in the string, then a
		''' {@code String} object representing an empty string is
		''' returned.
		''' <p>
		''' Otherwise, let <i>k</i> be the index of the first character in the
		''' string whose code is greater than {@code '\u005Cu0020'}, and let
		''' <i>m</i> be the index of the last character in the string whose code
		''' is greater than {@code '\u005Cu0020'}. A {@code String}
		''' object is returned, representing the substring of this string that
		''' begins with the character at index <i>k</i> and ends with the
		''' character at index <i>m</i>-that is, the result of
		''' {@code this.substring(k, m + 1)}.
		''' <p>
		''' This method may be used to trim whitespace (as defined above) from
		''' the beginning and end of a string.
		''' </summary>
		''' <returns>  A string whose value is this string, with any leading and trailing white
		'''          space removed, or this string if it has no leading or
		'''          trailing white space. </returns>
		public String Trim()
			Dim len As Integer = value.Length
			Dim st As Integer = 0
			Dim val As Char() = value ' avoid getfield opcode

			Do While (st < len) AndAlso (val(st) <= " "c)
				st += 1
			Loop
			Do While (st < len) AndAlso (val(len - 1) <= " "c)
				len -= 1
			Loop
			Return If((st > 0) OrElse (len < value.Length), Substring(st, len), Me)

		''' <summary>
		''' This object (which is already a string!) is itself returned.
		''' </summary>
		''' <returns>  the string itself. </returns>
		public String ToString()
			Return Me

		''' <summary>
		''' Converts this string to a new character array.
		''' </summary>
		''' <returns>  a newly allocated character array whose length is the length
		'''          of this string and whose contents are initialized to contain
		'''          the character sequence represented by this string. </returns>
		public Char() ToCharArray()
			' Cannot use Arrays.copyOf because of class initialization order issues
			Dim result As Char() = New Char(value.Length - 1){}
			Array.Copy(value, 0, result, 0, value.Length)
			Return result

		''' <summary>
		''' Returns a formatted string using the specified format string and
		''' arguments.
		''' 
		''' <p> The locale always used is the one returned by {@link
		''' java.util.Locale#getDefault() Locale.getDefault()}.
		''' </summary>
		''' <param name="format">
		'''         A <a href="../util/Formatter.html#syntax">format string</a>
		''' </param>
		''' <param name="args">
		'''         Arguments referenced by the format specifiers in the format
		'''         string.  If there are more arguments than format specifiers, the
		'''         extra arguments are ignored.  The number of arguments is
		'''         variable and may be zero.  The maximum number of arguments is
		'''         limited by the maximum dimension of a Java array as defined by
		'''         <cite>The Java&trade; Virtual Machine Specification</cite>.
		'''         The behaviour on a
		'''         {@code null} argument depends on the <a
		'''         href="../util/Formatter.html#syntax">conversion</a>.
		''' </param>
		''' <exception cref="java.util.IllegalFormatException">
		'''          If a format string contains an illegal syntax, a format
		'''          specifier that is incompatible with the given arguments,
		'''          insufficient arguments given the format string, or other
		'''          illegal conditions.  For specification of all possible
		'''          formatting errors, see the <a
		'''          href="../util/Formatter.html#detail">Details</a> section of the
		'''          formatter class specification.
		''' </exception>
		''' <returns>  A formatted string
		''' </returns>
		''' <seealso cref=  java.util.Formatter
		''' @since  1.5 </seealso>
		public static String format(String format, Object... args)
			Return (New java.util.Formatter).format(format, args).ToString()

		''' <summary>
		''' Returns a formatted string using the specified locale, format string,
		''' and arguments.
		''' </summary>
		''' <param name="l">
		'''         The <seealso cref="java.util.Locale locale"/> to apply during
		'''         formatting.  If {@code l} is {@code null} then no localization
		'''         is applied.
		''' </param>
		''' <param name="format">
		'''         A <a href="../util/Formatter.html#syntax">format string</a>
		''' </param>
		''' <param name="args">
		'''         Arguments referenced by the format specifiers in the format
		'''         string.  If there are more arguments than format specifiers, the
		'''         extra arguments are ignored.  The number of arguments is
		'''         variable and may be zero.  The maximum number of arguments is
		'''         limited by the maximum dimension of a Java array as defined by
		'''         <cite>The Java&trade; Virtual Machine Specification</cite>.
		'''         The behaviour on a
		'''         {@code null} argument depends on the
		'''         <a href="../util/Formatter.html#syntax">conversion</a>.
		''' </param>
		''' <exception cref="java.util.IllegalFormatException">
		'''          If a format string contains an illegal syntax, a format
		'''          specifier that is incompatible with the given arguments,
		'''          insufficient arguments given the format string, or other
		'''          illegal conditions.  For specification of all possible
		'''          formatting errors, see the <a
		'''          href="../util/Formatter.html#detail">Details</a> section of the
		'''          formatter class specification
		''' </exception>
		''' <returns>  A formatted string
		''' </returns>
		''' <seealso cref=  java.util.Formatter
		''' @since  1.5 </seealso>
		public static String format(java.util.Locale l, String format, Object... args)
			Return (New java.util.Formatter(l)).format(format, args).ToString()

		''' <summary>
		''' Returns the string representation of the {@code Object} argument.
		''' </summary>
		''' <param name="obj">   an {@code Object}. </param>
		''' <returns>  if the argument is {@code null}, then a string equal to
		'''          {@code "null"}; otherwise, the value of
		'''          {@code obj.toString()} is returned. </returns>
		''' <seealso cref=     java.lang.Object#toString() </seealso>
		public static String valueOf(Object obj)
			Return If(obj Is Nothing, "null", obj.ToString())

		''' <summary>
		''' Returns the string representation of the {@code char} array
		''' argument. The contents of the character array are copied; subsequent
		''' modification of the character array does not affect the returned
		''' string.
		''' </summary>
		''' <param name="data">     the character array. </param>
		''' <returns>  a {@code String} that contains the characters of the
		'''          character array. </returns>
		public static String valueOf(Char data())
			Return New String(data)

		''' <summary>
		''' Returns the string representation of a specific subarray of the
		''' {@code char} array argument.
		''' <p>
		''' The {@code offset} argument is the index of the first
		''' character of the subarray. The {@code count} argument
		''' specifies the length of the subarray. The contents of the subarray
		''' are copied; subsequent modification of the character array does not
		''' affect the returned string.
		''' </summary>
		''' <param name="data">     the character array. </param>
		''' <param name="offset">   initial offset of the subarray. </param>
		''' <param name="count">    length of the subarray. </param>
		''' <returns>  a {@code String} that contains the characters of the
		'''          specified subarray of the character array. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if {@code offset} is
		'''          negative, or {@code count} is negative, or
		'''          {@code offset+count} is larger than
		'''          {@code data.length}. </exception>
		public static String valueOf(Char data() , Integer offset, Integer count)
			Return New String(data, offset, count)

		''' <summary>
		''' Equivalent to <seealso cref="#valueOf(char[], int, int)"/>.
		''' </summary>
		''' <param name="data">     the character array. </param>
		''' <param name="offset">   initial offset of the subarray. </param>
		''' <param name="count">    length of the subarray. </param>
		''' <returns>  a {@code String} that contains the characters of the
		'''          specified subarray of the character array. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if {@code offset} is
		'''          negative, or {@code count} is negative, or
		'''          {@code offset+count} is larger than
		'''          {@code data.length}. </exception>
		public static String copyValueOf(Char data() , Integer offset, Integer count)
			Return New String(data, offset, count)

		''' <summary>
		''' Equivalent to <seealso cref="#valueOf(char[])"/>.
		''' </summary>
		''' <param name="data">   the character array. </param>
		''' <returns>  a {@code String} that contains the characters of the
		'''          character array. </returns>
		public static String copyValueOf(Char data())
			Return New String(data)

		''' <summary>
		''' Returns the string representation of the {@code boolean} argument.
		''' </summary>
		''' <param name="b">   a {@code boolean}. </param>
		''' <returns>  if the argument is {@code true}, a string equal to
		'''          {@code "true"} is returned; otherwise, a string equal to
		'''          {@code "false"} is returned. </returns>
		public static String valueOf(Boolean b)
			Return If(b, "true", "false")

		''' <summary>
		''' Returns the string representation of the {@code char}
		''' argument.
		''' </summary>
		''' <param name="c">   a {@code char}. </param>
		''' <returns>  a string of length {@code 1} containing
		'''          as its single character the argument {@code c}. </returns>
		public static String valueOf(Char c)
			Dim data As Char() = {c}
			Return New String(data, True)

		''' <summary>
		''' Returns the string representation of the {@code int} argument.
		''' <p>
		''' The representation is exactly the one returned by the
		''' {@code  java.lang.[Integer].toString} method of one argument.
		''' </summary>
		''' <param name="i">   an {@code int}. </param>
		''' <returns>  a string representation of the {@code int} argument. </returns>
		''' <seealso cref=     java.lang.Integer#toString(int, int) </seealso>
		public static String valueOf(Integer i)
			Return Convert.ToString(i)

		''' <summary>
		''' Returns the string representation of the {@code long} argument.
		''' <p>
		''' The representation is exactly the one returned by the
		''' {@code java.lang.[Long].toString} method of one argument.
		''' </summary>
		''' <param name="l">   a {@code long}. </param>
		''' <returns>  a string representation of the {@code long} argument. </returns>
		''' <seealso cref=     java.lang.Long#toString(long) </seealso>
		public static String valueOf(Long l)
			Return Convert.ToString(l)

		''' <summary>
		''' Returns the string representation of the {@code float} argument.
		''' <p>
		''' The representation is exactly the one returned by the
		''' {@code Float.toString} method of one argument.
		''' </summary>
		''' <param name="f">   a {@code float}. </param>
		''' <returns>  a string representation of the {@code float} argument. </returns>
		''' <seealso cref=     java.lang.Float#toString(float) </seealso>
		public static String valueOf(Single f)
			Return Convert.ToString(f)

		''' <summary>
		''' Returns the string representation of the {@code double} argument.
		''' <p>
		''' The representation is exactly the one returned by the
		''' {@code java.lang.[Double].toString} method of one argument.
		''' </summary>
		''' <param name="d">   a {@code double}. </param>
		''' <returns>  a  string representation of the {@code double} argument. </returns>
		''' <seealso cref=     java.lang.Double#toString(double) </seealso>
		public static String valueOf(Double d)
			Return Convert.ToString(d)

		''' <summary>
		''' Returns a canonical representation for the string object.
		''' <p>
		''' A pool of strings, initially empty, is maintained privately by the
		''' class {@code String}.
		''' <p>
		''' When the intern method is invoked, if the pool already contains a
		''' string equal to this {@code String} object as determined by
		''' the <seealso cref="#equals(Object)"/> method, then the string from the pool is
		''' returned. Otherwise, this {@code String} object is added to the
		''' pool and a reference to this {@code String} object is returned.
		''' <p>
		''' It follows that for any two strings {@code s} and {@code t},
		''' {@code s.intern() == t.intern()} is {@code true}
		''' if and only if {@code s.equals(t)} is {@code true}.
		''' <p>
		''' All literal strings and string-valued constant expressions are
		''' interned. String literals are defined in section 3.10.5 of the
		''' <cite>The Java&trade; Language Specification</cite>.
		''' </summary>
		''' <returns>  a string that has the same contents as this string, but is
		'''          guaranteed to be from a pool of unique strings. </returns>
		public native String intern()
	End Class

End Namespace
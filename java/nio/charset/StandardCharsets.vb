'
' * Copyright (c) 2011, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.nio.charset

	''' <summary>
	''' Constant definitions for the standard <seealso cref="Charset Charsets"/>. These
	''' charsets are guaranteed to be available on every implementation of the Java
	''' platform.
	''' </summary>
	''' <seealso cref= <a href="Charset#standard">Standard Charsets</a>
	''' @since 1.7 </seealso>
	Public NotInheritable Class StandardCharsets

		Private Sub New()
			Throw New AssertionError("No java.nio.charset.StandardCharsets instances for you!")
		End Sub
		''' <summary>
		''' Seven-bit ASCII, a.k.a. ISO646-US, a.k.a. the Basic Latin block of the
		''' Unicode character set
		''' </summary>
		Public Shared ReadOnly US_ASCII As Charset = Charset.forName("US-ASCII")
		''' <summary>
		''' ISO Latin Alphabet No. 1, a.k.a. ISO-LATIN-1
		''' </summary>
		Public Shared ReadOnly ISO_8859_1 As Charset = Charset.forName("ISO-8859-1")
		''' <summary>
		''' Eight-bit UCS Transformation Format
		''' </summary>
		Public Shared ReadOnly UTF_8 As Charset = Charset.forName("UTF-8")
		''' <summary>
		''' Sixteen-bit UCS Transformation Format, big-endian byte order
		''' </summary>
		Public Shared ReadOnly UTF_16BE As Charset = Charset.forName("UTF-16BE")
		''' <summary>
		''' Sixteen-bit UCS Transformation Format, little-endian byte order
		''' </summary>
		Public Shared ReadOnly UTF_16LE As Charset = Charset.forName("UTF-16LE")
		''' <summary>
		''' Sixteen-bit UCS Transformation Format, byte order identified by an
		''' optional byte-order mark
		''' </summary>
		Public Shared ReadOnly UTF_16 As Charset = Charset.forName("UTF-16")
	End Class

End Namespace
'
' * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.print.attribute.standard



	''' <summary>
	''' Class MediaSizeName is a subclass of Media.
	''' <P>
	''' This attribute can be used instead of specifying MediaName or MediaTray.
	''' <p>
	''' Class MediaSizeName currently declares a few standard media
	''' name values.
	''' <P>
	''' <B>IPP Compatibility:</B> MediaSizeName is a representation class for
	''' values of the IPP "media" attribute which names media sizes.
	''' The names of the media sizes correspond to those in the IPP 1.1 RFC
	''' <a HREF="http://www.ietf.org/rfc/rfc2911.txt">RFC 2911</a>
	''' <P>
	''' 
	''' </summary>
	Public Class MediaSizeName
		Inherits Media

		Private Const serialVersionUID As Long = 2778798329756942747L

		''' <summary>
		''' A0 size.
		''' </summary>
		Public Shared ReadOnly ISO_A0 As New MediaSizeName(0)
		''' <summary>
		''' A1 size.
		''' </summary>
		Public Shared ReadOnly ISO_A1 As New MediaSizeName(1)
		''' <summary>
		''' A2 size.
		''' </summary>
		Public Shared ReadOnly ISO_A2 As New MediaSizeName(2)
		''' <summary>
		''' A3 size.
		''' </summary>
		Public Shared ReadOnly ISO_A3 As New MediaSizeName(3)
		''' <summary>
		''' A4 size.
		''' </summary>
		Public Shared ReadOnly ISO_A4 As New MediaSizeName(4)
		''' <summary>
		''' A5 size.
		''' </summary>
		Public Shared ReadOnly ISO_A5 As New MediaSizeName(5)
		''' <summary>
		''' A6 size.
		''' </summary>
		Public Shared ReadOnly ISO_A6 As New MediaSizeName(6)
		''' <summary>
		''' A7 size.
		''' </summary>
		Public Shared ReadOnly ISO_A7 As New MediaSizeName(7)
		''' <summary>
		''' A8 size.
		''' </summary>
		Public Shared ReadOnly ISO_A8 As New MediaSizeName(8)
		''' <summary>
		''' A9 size.
		''' </summary>
		Public Shared ReadOnly ISO_A9 As New MediaSizeName(9)
		''' <summary>
		''' A10 size.
		''' </summary>
		Public Shared ReadOnly ISO_A10 As New MediaSizeName(10)

	   ''' <summary>
	   ''' ISO B0 size.
	   ''' </summary>
		Public Shared ReadOnly ISO_B0 As New MediaSizeName(11)
		''' <summary>
		''' ISO B1 size.
		''' </summary>
		Public Shared ReadOnly ISO_B1 As New MediaSizeName(12)
		''' <summary>
		''' ISO B2 size.
		''' </summary>
		Public Shared ReadOnly ISO_B2 As New MediaSizeName(13)
		''' <summary>
		''' ISO B3 size.
		''' </summary>
		Public Shared ReadOnly ISO_B3 As New MediaSizeName(14)
		''' <summary>
		''' ISO B4 size.
		''' </summary>
		Public Shared ReadOnly ISO_B4 As New MediaSizeName(15)
		''' <summary>
		''' ISO B5 size.
		''' </summary>
		Public Shared ReadOnly ISO_B5 As New MediaSizeName(16)
		''' <summary>
		''' ISO B6 size.
		''' </summary>
		Public Shared ReadOnly ISO_B6 As New MediaSizeName(17)
		''' <summary>
		''' ISO B7 size.
		''' </summary>
		Public Shared ReadOnly ISO_B7 As New MediaSizeName(18)
		''' <summary>
		''' ISO B8 size.
		''' </summary>
		Public Shared ReadOnly ISO_B8 As New MediaSizeName(19)
		''' <summary>
		''' ISO B9 size.
		''' </summary>
		Public Shared ReadOnly ISO_B9 As New MediaSizeName(20)
		''' <summary>
		''' ISO B10 size.
		''' </summary>
		Public Shared ReadOnly ISO_B10 As New MediaSizeName(21)

	   ''' <summary>
	   ''' JIS B0 size.
	   ''' </summary>
		Public Shared ReadOnly JIS_B0 As New MediaSizeName(22)
		''' <summary>
		''' JIS B1 size.
		''' </summary>
		Public Shared ReadOnly JIS_B1 As New MediaSizeName(23)
		''' <summary>
		''' JIS B2 size.
		''' </summary>
		Public Shared ReadOnly JIS_B2 As New MediaSizeName(24)
		''' <summary>
		''' JIS B3 size.
		''' </summary>
		Public Shared ReadOnly JIS_B3 As New MediaSizeName(25)
		''' <summary>
		''' JIS B4 size.
		''' </summary>
		Public Shared ReadOnly JIS_B4 As New MediaSizeName(26)
		''' <summary>
		''' JIS B5 size.
		''' </summary>
		Public Shared ReadOnly JIS_B5 As New MediaSizeName(27)
		''' <summary>
		''' JIS B6 size.
		''' </summary>
		Public Shared ReadOnly JIS_B6 As New MediaSizeName(28)
		''' <summary>
		''' JIS B7 size.
		''' </summary>
		Public Shared ReadOnly JIS_B7 As New MediaSizeName(29)
		''' <summary>
		''' JIS B8 size.
		''' </summary>
		Public Shared ReadOnly JIS_B8 As New MediaSizeName(30)
		''' <summary>
		''' JIS B9 size.
		''' </summary>
		Public Shared ReadOnly JIS_B9 As New MediaSizeName(31)
		''' <summary>
		''' JIS B10 size.
		''' </summary>
		Public Shared ReadOnly JIS_B10 As New MediaSizeName(32)

		''' <summary>
		''' ISO C0 size.
		''' </summary>
		Public Shared ReadOnly ISO_C0 As New MediaSizeName(33)
		''' <summary>
		''' ISO C1 size.
		''' </summary>
		Public Shared ReadOnly ISO_C1 As New MediaSizeName(34)
		''' <summary>
		''' ISO C2 size.
		''' </summary>
		Public Shared ReadOnly ISO_C2 As New MediaSizeName(35)
		''' <summary>
		''' ISO C3 size.
		''' </summary>
		Public Shared ReadOnly ISO_C3 As New MediaSizeName(36)
		''' <summary>
		''' ISO C4 size.
		''' </summary>
		Public Shared ReadOnly ISO_C4 As New MediaSizeName(37)
		''' <summary>
		''' ISO C5 size.
		''' </summary>
		Public Shared ReadOnly ISO_C5 As New MediaSizeName(38)
		''' <summary>
		'''   letter size.
		''' </summary>
		Public Shared ReadOnly ISO_C6 As New MediaSizeName(39)
		''' <summary>
		'''   letter size.
		''' </summary>
		Public Shared ReadOnly NA_LETTER As New MediaSizeName(40)

		''' <summary>
		'''  legal size .
		''' </summary>
		Public Shared ReadOnly NA_LEGAL As New MediaSizeName(41)

		''' <summary>
		'''  executive size .
		''' </summary>
		Public Shared ReadOnly EXECUTIVE As New MediaSizeName(42)

		''' <summary>
		'''  ledger size .
		''' </summary>
		Public Shared ReadOnly LEDGER As New MediaSizeName(43)

		''' <summary>
		'''  tabloid size .
		''' </summary>
		Public Shared ReadOnly TABLOID As New MediaSizeName(44)

		''' <summary>
		'''  invoice size .
		''' </summary>
		Public Shared ReadOnly INVOICE As New MediaSizeName(45)

		''' <summary>
		'''  folio size .
		''' </summary>
		Public Shared ReadOnly FOLIO As New MediaSizeName(46)

		''' <summary>
		'''  quarto size .
		''' </summary>
		Public Shared ReadOnly QUARTO As New MediaSizeName(47)

		''' <summary>
		'''  Japanese Postcard size.
		''' </summary>
		Public Shared ReadOnly JAPANESE_POSTCARD As New MediaSizeName(48)
	   ''' <summary>
	   '''  Japanese Double Postcard size.
	   ''' </summary>
		Public Shared ReadOnly JAPANESE_DOUBLE_POSTCARD As New MediaSizeName(49)

		''' <summary>
		'''  A size .
		''' </summary>
		Public Shared ReadOnly A As New MediaSizeName(50)

		''' <summary>
		'''  B size .
		''' </summary>
		Public Shared ReadOnly B As New MediaSizeName(51)

		''' <summary>
		'''  C size .
		''' </summary>
		Public Shared ReadOnly C As New MediaSizeName(52)

		''' <summary>
		'''  D size .
		''' </summary>
		Public Shared ReadOnly D As New MediaSizeName(53)

		''' <summary>
		'''  E size .
		''' </summary>
		Public Shared ReadOnly E As New MediaSizeName(54)

		''' <summary>
		'''  ISO designated long size .
		''' </summary>
		Public Shared ReadOnly ISO_DESIGNATED_LONG As New MediaSizeName(55)

		''' <summary>
		'''  Italy envelope size .
		''' </summary>
		Public Shared ReadOnly ITALY_ENVELOPE As New MediaSizeName(56) ' DESIGNATED_LONG?

		''' <summary>
		'''  monarch envelope size .
		''' </summary>
		Public Shared ReadOnly MONARCH_ENVELOPE As New MediaSizeName(57)
		''' <summary>
		''' personal envelope size .
		''' </summary>
		Public Shared ReadOnly PERSONAL_ENVELOPE As New MediaSizeName(58)
		''' <summary>
		'''  number 9 envelope size .
		''' </summary>
		Public Shared ReadOnly NA_NUMBER_9_ENVELOPE As New MediaSizeName(59)
		''' <summary>
		'''  number 10 envelope size .
		''' </summary>
		Public Shared ReadOnly NA_NUMBER_10_ENVELOPE As New MediaSizeName(60)
		''' <summary>
		'''  number 11 envelope size .
		''' </summary>
		Public Shared ReadOnly NA_NUMBER_11_ENVELOPE As New MediaSizeName(61)
		''' <summary>
		'''  number 12 envelope size .
		''' </summary>
		Public Shared ReadOnly NA_NUMBER_12_ENVELOPE As New MediaSizeName(62)
		''' <summary>
		'''  number 14 envelope size .
		''' </summary>
		Public Shared ReadOnly NA_NUMBER_14_ENVELOPE As New MediaSizeName(63)
	   ''' <summary>
	   '''  6x9 North American envelope size.
	   ''' </summary>
		Public Shared ReadOnly NA_6X9_ENVELOPE As New MediaSizeName(64)
	   ''' <summary>
	   '''  7x9 North American envelope size.
	   ''' </summary>
		Public Shared ReadOnly NA_7X9_ENVELOPE As New MediaSizeName(65)
	   ''' <summary>
	   '''  9x11 North American envelope size.
	   ''' </summary>
		Public Shared ReadOnly NA_9X11_ENVELOPE As New MediaSizeName(66)
		''' <summary>
		'''  9x12 North American envelope size.
		''' </summary>
		Public Shared ReadOnly NA_9X12_ENVELOPE As New MediaSizeName(67)

		''' <summary>
		'''  10x13 North American envelope size .
		''' </summary>
		Public Shared ReadOnly NA_10X13_ENVELOPE As New MediaSizeName(68)
		''' <summary>
		'''  10x14North American  envelope size .
		''' </summary>
		Public Shared ReadOnly NA_10X14_ENVELOPE As New MediaSizeName(69)
		''' <summary>
		'''  10x15 North American envelope size.
		''' </summary>
		Public Shared ReadOnly NA_10X15_ENVELOPE As New MediaSizeName(70)

		''' <summary>
		'''  5x7 North American paper.
		''' </summary>
		Public Shared ReadOnly NA_5X7 As New MediaSizeName(71)

		''' <summary>
		'''  8x10 North American paper.
		''' </summary>
		Public Shared ReadOnly NA_8X10 As New MediaSizeName(72)

		''' <summary>
		''' Construct a new media size enumeration value with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "iso-a0", "iso-a1", "iso-a2", "iso-a3", "iso-a4", "iso-a5", "iso-a6", "iso-a7", "iso-a8", "iso-a9", "iso-a10", "iso-b0", "iso-b1", "iso-b2", "iso-b3", "iso-b4", "iso-b5", "iso-b6", "iso-b7", "iso-b8", "iso-b9", "iso-b10", "jis-b0", "jis-b1", "jis-b2", "jis-b3", "jis-b4", "jis-b5", "jis-b6", "jis-b7", "jis-b8", "jis-b9", "jis-b10", "iso-c0", "iso-c1", "iso-c2", "iso-c3", "iso-c4", "iso-c5", "iso-c6", "na-letter", "na-legal", "executive", "ledger", "tabloid", "invoice", "folio", "quarto", "japanese-postcard", "oufuko-postcard", "a", "b", "c", "d", "e", "iso-designated-long", "italian-envelope", "monarch-envelope", "personal-envelope", "na-number-9-envelope", "na-number-10-envelope", "na-number-11-envelope", "na-number-12-envelope", "na-number-14-envelope", "na-6x9-envelope", "na-7x9-envelope", "na-9x11-envelope", "na-9x12-envelope", "na-10x13-envelope", "na-10x14-envelope", "na-10x15-envelope", "na-5x7", "na-8x10" }

		Private Shared ReadOnly myEnumValueTable As MediaSizeName() = { ISO_A0, ISO_A1, ISO_A2, ISO_A3, ISO_A4, ISO_A5, ISO_A6, ISO_A7, ISO_A8, ISO_A9, ISO_A10, ISO_B0, ISO_B1, ISO_B2, ISO_B3, ISO_B4, ISO_B5, ISO_B6, ISO_B7, ISO_B8, ISO_B9, ISO_B10, JIS_B0, JIS_B1, JIS_B2, JIS_B3, JIS_B4, JIS_B5, JIS_B6, JIS_B7, JIS_B8, JIS_B9, JIS_B10, ISO_C0, ISO_C1, ISO_C2, ISO_C3, ISO_C4, ISO_C5, ISO_C6, NA_LETTER, NA_LEGAL, EXECUTIVE, LEDGER, TABLOID, INVOICE, FOLIO, QUARTO, JAPANESE_POSTCARD, JAPANESE_DOUBLE_POSTCARD, A, B, C, D, E, ISO_DESIGNATED_LONG, ITALY_ENVELOPE, MONARCH_ENVELOPE, PERSONAL_ENVELOPE, NA_NUMBER_9_ENVELOPE, NA_NUMBER_10_ENVELOPE, NA_NUMBER_11_ENVELOPE, NA_NUMBER_12_ENVELOPE, NA_NUMBER_14_ENVELOPE, NA_6X9_ENVELOPE, NA_7X9_ENVELOPE, NA_9X11_ENVELOPE, NA_9X12_ENVELOPE, NA_10X13_ENVELOPE, NA_10X14_ENVELOPE, NA_10X15_ENVELOPE, NA_5X7, NA_8X10 }


		''' <summary>
		''' Returns the string table for class MediaSizeName.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return CType(myStringTable.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class MediaSizeName.
		''' </summary>
		Protected Friend Property Overrides enumValueTable As javax.print.attribute.EnumSyntax()
			Get
				Return CType(myEnumValueTable.clone(), javax.print.attribute.EnumSyntax())
			End Get
		End Property


	End Class

End Namespace
'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt


	''' <summary>
	''' A set of attributes which control the output of a printed page.
	''' <p>
	''' Instances of this class control the color state, paper size (media type),
	''' orientation, logical origin, print quality, and resolution of every
	''' page which uses the instance. Attribute names are compliant with the
	''' Internet Printing Protocol (IPP) 1.1 where possible. Attribute values
	''' are partially compliant where possible.
	''' <p>
	''' To use a method which takes an inner class type, pass a reference to
	''' one of the constant fields of the inner class. Client code cannot create
	''' new instances of the inner class types because none of those classes
	''' has a public constructor. For example, to set the color state to
	''' monochrome, use the following code:
	''' <pre>
	''' import java.awt.PageAttributes;
	''' 
	''' public class MonochromeExample {
	'''     public  Sub  setMonochrome(PageAttributes pageAttributes) {
	'''         pageAttributes.setColor(PageAttributes.ColorType.MONOCHROME);
	'''     }
	''' }
	''' </pre>
	''' <p>
	''' Every IPP attribute which supports an <i>attributeName</i>-default value
	''' has a corresponding <code>set<i>attributeName</i>ToDefault</code> method.
	''' Default value fields are not provided.
	''' 
	''' @author      David Mendenhall
	''' @since 1.3
	''' </summary>
	Public NotInheritable Class PageAttributes
		Implements Cloneable

		''' <summary>
		''' A type-safe enumeration of possible color states.
		''' @since 1.3
		''' </summary>
		Public NotInheritable Class ColorType
			Inherits AttributeValue

			Private Const I_COLOR As Integer = 0
			Private Const I_MONOCHROME As Integer = 1

			Private Shared ReadOnly NAMES As String() = { "color", "monochrome" }

			''' <summary>
			''' The ColorType instance to use for specifying color printing.
			''' </summary>
			Public Shared ReadOnly COLOR As New ColorType(I_COLOR)
			''' <summary>
			''' The ColorType instance to use for specifying monochrome printing.
			''' </summary>
			Public Shared ReadOnly MONOCHROME As New ColorType(I_MONOCHROME)

			Private Sub New(  type As Integer)
				MyBase.New(type, NAMES)
			End Sub
		End Class

		''' <summary>
		''' A type-safe enumeration of possible paper sizes. These sizes are in
		''' compliance with IPP 1.1.
		''' @since 1.3
		''' </summary>
		Public NotInheritable Class MediaType
			Inherits AttributeValue

			Private Const I_ISO_4A0 As Integer = 0
			Private Const I_ISO_2A0 As Integer = 1
			Private Const I_ISO_A0 As Integer = 2
			Private Const I_ISO_A1 As Integer = 3
			Private Const I_ISO_A2 As Integer = 4
			Private Const I_ISO_A3 As Integer = 5
			Private Const I_ISO_A4 As Integer = 6
			Private Const I_ISO_A5 As Integer = 7
			Private Const I_ISO_A6 As Integer = 8
			Private Const I_ISO_A7 As Integer = 9
			Private Const I_ISO_A8 As Integer = 10
			Private Const I_ISO_A9 As Integer = 11
			Private Const I_ISO_A10 As Integer = 12
			Private Const I_ISO_B0 As Integer = 13
			Private Const I_ISO_B1 As Integer = 14
			Private Const I_ISO_B2 As Integer = 15
			Private Const I_ISO_B3 As Integer = 16
			Private Const I_ISO_B4 As Integer = 17
			Private Const I_ISO_B5 As Integer = 18
			Private Const I_ISO_B6 As Integer = 19
			Private Const I_ISO_B7 As Integer = 20
			Private Const I_ISO_B8 As Integer = 21
			Private Const I_ISO_B9 As Integer = 22
			Private Const I_ISO_B10 As Integer = 23
			Private Const I_JIS_B0 As Integer = 24
			Private Const I_JIS_B1 As Integer = 25
			Private Const I_JIS_B2 As Integer = 26
			Private Const I_JIS_B3 As Integer = 27
			Private Const I_JIS_B4 As Integer = 28
			Private Const I_JIS_B5 As Integer = 29
			Private Const I_JIS_B6 As Integer = 30
			Private Const I_JIS_B7 As Integer = 31
			Private Const I_JIS_B8 As Integer = 32
			Private Const I_JIS_B9 As Integer = 33
			Private Const I_JIS_B10 As Integer = 34
			Private Const I_ISO_C0 As Integer = 35
			Private Const I_ISO_C1 As Integer = 36
			Private Const I_ISO_C2 As Integer = 37
			Private Const I_ISO_C3 As Integer = 38
			Private Const I_ISO_C4 As Integer = 39
			Private Const I_ISO_C5 As Integer = 40
			Private Const I_ISO_C6 As Integer = 41
			Private Const I_ISO_C7 As Integer = 42
			Private Const I_ISO_C8 As Integer = 43
			Private Const I_ISO_C9 As Integer = 44
			Private Const I_ISO_C10 As Integer = 45
			Private Const I_ISO_DESIGNATED_LONG As Integer = 46
			Private Const I_EXECUTIVE As Integer = 47
			Private Const I_FOLIO As Integer = 48
			Private Const I_INVOICE As Integer = 49
			Private Const I_LEDGER As Integer = 50
			Private Const I_NA_LETTER As Integer = 51
			Private Const I_NA_LEGAL As Integer = 52
			Private Const I_QUARTO As Integer = 53
			Private Const I_A As Integer = 54
			Private Const I_B As Integer = 55
			Private Const I_C As Integer = 56
			Private Const I_D As Integer = 57
			Private Const I_E As Integer = 58
			Private Const I_NA_10X15_ENVELOPE As Integer = 59
			Private Const I_NA_10X14_ENVELOPE As Integer = 60
			Private Const I_NA_10X13_ENVELOPE As Integer = 61
			Private Const I_NA_9X12_ENVELOPE As Integer = 62
			Private Const I_NA_9X11_ENVELOPE As Integer = 63
			Private Const I_NA_7X9_ENVELOPE As Integer = 64
			Private Const I_NA_6X9_ENVELOPE As Integer = 65
			Private Const I_NA_NUMBER_9_ENVELOPE As Integer = 66
			Private Const I_NA_NUMBER_10_ENVELOPE As Integer = 67
			Private Const I_NA_NUMBER_11_ENVELOPE As Integer = 68
			Private Const I_NA_NUMBER_12_ENVELOPE As Integer = 69
			Private Const I_NA_NUMBER_14_ENVELOPE As Integer = 70
			Private Const I_INVITE_ENVELOPE As Integer = 71
			Private Const I_ITALY_ENVELOPE As Integer = 72
			Private Const I_MONARCH_ENVELOPE As Integer = 73
			Private Const I_PERSONAL_ENVELOPE As Integer = 74

			Private Shared ReadOnly NAMES As String() = { "iso-4a0", "iso-2a0", "iso-a0", "iso-a1", "iso-a2", "iso-a3", "iso-a4", "iso-a5", "iso-a6", "iso-a7", "iso-a8", "iso-a9", "iso-a10", "iso-b0", "iso-b1", "iso-b2", "iso-b3", "iso-b4", "iso-b5", "iso-b6", "iso-b7", "iso-b8", "iso-b9", "iso-b10", "jis-b0", "jis-b1", "jis-b2", "jis-b3", "jis-b4", "jis-b5", "jis-b6", "jis-b7", "jis-b8", "jis-b9", "jis-b10", "iso-c0", "iso-c1", "iso-c2", "iso-c3", "iso-c4", "iso-c5", "iso-c6", "iso-c7", "iso-c8", "iso-c9", "iso-c10", "iso-designated-long", "executive", "folio", "invoice", "ledger", "na-letter", "na-legal", "quarto", "a", "b", "c", "d", "e", "na-10x15-envelope", "na-10x14-envelope", "na-10x13-envelope", "na-9x12-envelope", "na-9x11-envelope", "na-7x9-envelope", "na-6x9-envelope", "na-number-9-envelope", "na-number-10-envelope", "na-number-11-envelope", "na-number-12-envelope", "na-number-14-envelope", "invite-envelope", "italy-envelope", "monarch-envelope", "personal-envelope" }

			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS 4A0, 1682 x 2378 mm.
			''' </summary>
			Public Shared ReadOnly ISO_4A0 As New MediaType(I_ISO_4A0)
			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS 2A0, 1189 x 1682 mm.
			''' </summary>
			Public Shared ReadOnly ISO_2A0 As New MediaType(I_ISO_2A0)
			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS A0, 841 x 1189 mm.
			''' </summary>
			Public Shared ReadOnly ISO_A0 As New MediaType(I_ISO_A0)
			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS A1, 594 x 841 mm.
			''' </summary>
			Public Shared ReadOnly ISO_A1 As New MediaType(I_ISO_A1)
			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS A2, 420 x 594 mm.
			''' </summary>
			Public Shared ReadOnly ISO_A2 As New MediaType(I_ISO_A2)
			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS A3, 297 x 420 mm.
			''' </summary>
			Public Shared ReadOnly ISO_A3 As New MediaType(I_ISO_A3)
			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS A4, 210 x 297 mm.
			''' </summary>
			Public Shared ReadOnly ISO_A4 As New MediaType(I_ISO_A4)
			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS A5, 148 x 210 mm.
			''' </summary>
			Public Shared ReadOnly ISO_A5 As New MediaType(I_ISO_A5)
			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS A6, 105 x 148 mm.
			''' </summary>
			Public Shared ReadOnly ISO_A6 As New MediaType(I_ISO_A6)
			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS A7, 74 x 105 mm.
			''' </summary>
			Public Shared ReadOnly ISO_A7 As New MediaType(I_ISO_A7)
			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS A8, 52 x 74 mm.
			''' </summary>
			Public Shared ReadOnly ISO_A8 As New MediaType(I_ISO_A8)
			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS A9, 37 x 52 mm.
			''' </summary>
			Public Shared ReadOnly ISO_A9 As New MediaType(I_ISO_A9)
			''' <summary>
			''' The MediaType instance for ISO/DIN and JIS A10, 26 x 37 mm.
			''' </summary>
			Public Shared ReadOnly ISO_A10 As New MediaType(I_ISO_A10)
			''' <summary>
			''' The MediaType instance for ISO/DIN B0, 1000 x 1414 mm.
			''' </summary>
			Public Shared ReadOnly ISO_B0 As New MediaType(I_ISO_B0)
			''' <summary>
			''' The MediaType instance for ISO/DIN B1, 707 x 1000 mm.
			''' </summary>
			Public Shared ReadOnly ISO_B1 As New MediaType(I_ISO_B1)
			''' <summary>
			''' The MediaType instance for ISO/DIN B2, 500 x 707 mm.
			''' </summary>
			Public Shared ReadOnly ISO_B2 As New MediaType(I_ISO_B2)
			''' <summary>
			''' The MediaType instance for ISO/DIN B3, 353 x 500 mm.
			''' </summary>
			Public Shared ReadOnly ISO_B3 As New MediaType(I_ISO_B3)
			''' <summary>
			''' The MediaType instance for ISO/DIN B4, 250 x 353 mm.
			''' </summary>
			Public Shared ReadOnly ISO_B4 As New MediaType(I_ISO_B4)
			''' <summary>
			''' The MediaType instance for ISO/DIN B5, 176 x 250 mm.
			''' </summary>
			Public Shared ReadOnly ISO_B5 As New MediaType(I_ISO_B5)
			''' <summary>
			''' The MediaType instance for ISO/DIN B6, 125 x 176 mm.
			''' </summary>
			Public Shared ReadOnly ISO_B6 As New MediaType(I_ISO_B6)
			''' <summary>
			''' The MediaType instance for ISO/DIN B7, 88 x 125 mm.
			''' </summary>
			Public Shared ReadOnly ISO_B7 As New MediaType(I_ISO_B7)
			''' <summary>
			''' The MediaType instance for ISO/DIN B8, 62 x 88 mm.
			''' </summary>
			Public Shared ReadOnly ISO_B8 As New MediaType(I_ISO_B8)
			''' <summary>
			''' The MediaType instance for ISO/DIN B9, 44 x 62 mm.
			''' </summary>
			Public Shared ReadOnly ISO_B9 As New MediaType(I_ISO_B9)
			''' <summary>
			''' The MediaType instance for ISO/DIN B10, 31 x 44 mm.
			''' </summary>
			Public Shared ReadOnly ISO_B10 As New MediaType(I_ISO_B10)
			''' <summary>
			''' The MediaType instance for JIS B0, 1030 x 1456 mm.
			''' </summary>
			Public Shared ReadOnly JIS_B0 As New MediaType(I_JIS_B0)
			''' <summary>
			''' The MediaType instance for JIS B1, 728 x 1030 mm.
			''' </summary>
			Public Shared ReadOnly JIS_B1 As New MediaType(I_JIS_B1)
			''' <summary>
			''' The MediaType instance for JIS B2, 515 x 728 mm.
			''' </summary>
			Public Shared ReadOnly JIS_B2 As New MediaType(I_JIS_B2)
			''' <summary>
			''' The MediaType instance for JIS B3, 364 x 515 mm.
			''' </summary>
			Public Shared ReadOnly JIS_B3 As New MediaType(I_JIS_B3)
			''' <summary>
			''' The MediaType instance for JIS B4, 257 x 364 mm.
			''' </summary>
			Public Shared ReadOnly JIS_B4 As New MediaType(I_JIS_B4)
			''' <summary>
			''' The MediaType instance for JIS B5, 182 x 257 mm.
			''' </summary>
			Public Shared ReadOnly JIS_B5 As New MediaType(I_JIS_B5)
			''' <summary>
			''' The MediaType instance for JIS B6, 128 x 182 mm.
			''' </summary>
			Public Shared ReadOnly JIS_B6 As New MediaType(I_JIS_B6)
			''' <summary>
			''' The MediaType instance for JIS B7, 91 x 128 mm.
			''' </summary>
			Public Shared ReadOnly JIS_B7 As New MediaType(I_JIS_B7)
			''' <summary>
			''' The MediaType instance for JIS B8, 64 x 91 mm.
			''' </summary>
			Public Shared ReadOnly JIS_B8 As New MediaType(I_JIS_B8)
			''' <summary>
			''' The MediaType instance for JIS B9, 45 x 64 mm.
			''' </summary>
			Public Shared ReadOnly JIS_B9 As New MediaType(I_JIS_B9)
			''' <summary>
			''' The MediaType instance for JIS B10, 32 x 45 mm.
			''' </summary>
			Public Shared ReadOnly JIS_B10 As New MediaType(I_JIS_B10)
			''' <summary>
			''' The MediaType instance for ISO/DIN C0, 917 x 1297 mm.
			''' </summary>
			Public Shared ReadOnly ISO_C0 As New MediaType(I_ISO_C0)
			''' <summary>
			''' The MediaType instance for ISO/DIN C1, 648 x 917 mm.
			''' </summary>
			Public Shared ReadOnly ISO_C1 As New MediaType(I_ISO_C1)
			''' <summary>
			''' The MediaType instance for ISO/DIN C2, 458 x 648 mm.
			''' </summary>
			Public Shared ReadOnly ISO_C2 As New MediaType(I_ISO_C2)
			''' <summary>
			''' The MediaType instance for ISO/DIN C3, 324 x 458 mm.
			''' </summary>
			Public Shared ReadOnly ISO_C3 As New MediaType(I_ISO_C3)
			''' <summary>
			''' The MediaType instance for ISO/DIN C4, 229 x 324 mm.
			''' </summary>
			Public Shared ReadOnly ISO_C4 As New MediaType(I_ISO_C4)
			''' <summary>
			''' The MediaType instance for ISO/DIN C5, 162 x 229 mm.
			''' </summary>
			Public Shared ReadOnly ISO_C5 As New MediaType(I_ISO_C5)
			''' <summary>
			''' The MediaType instance for ISO/DIN C6, 114 x 162 mm.
			''' </summary>
			Public Shared ReadOnly ISO_C6 As New MediaType(I_ISO_C6)
			''' <summary>
			''' The MediaType instance for ISO/DIN C7, 81 x 114 mm.
			''' </summary>
			Public Shared ReadOnly ISO_C7 As New MediaType(I_ISO_C7)
			''' <summary>
			''' The MediaType instance for ISO/DIN C8, 57 x 81 mm.
			''' </summary>
			Public Shared ReadOnly ISO_C8 As New MediaType(I_ISO_C8)
			''' <summary>
			''' The MediaType instance for ISO/DIN C9, 40 x 57 mm.
			''' </summary>
			Public Shared ReadOnly ISO_C9 As New MediaType(I_ISO_C9)
			''' <summary>
			''' The MediaType instance for ISO/DIN C10, 28 x 40 mm.
			''' </summary>
			Public Shared ReadOnly ISO_C10 As New MediaType(I_ISO_C10)
			''' <summary>
			''' The MediaType instance for ISO Designated Long, 110 x 220 mm.
			''' </summary>
			Public Shared ReadOnly ISO_DESIGNATED_LONG As New MediaType(I_ISO_DESIGNATED_LONG)
			''' <summary>
			''' The MediaType instance for Executive, 7 1/4 x 10 1/2 in.
			''' </summary>
			Public Shared ReadOnly EXECUTIVE As New MediaType(I_EXECUTIVE)
			''' <summary>
			''' The MediaType instance for Folio, 8 1/2 x 13 in.
			''' </summary>
			Public Shared ReadOnly FOLIO As New MediaType(I_FOLIO)
			''' <summary>
			''' The MediaType instance for Invoice, 5 1/2 x 8 1/2 in.
			''' </summary>
			Public Shared ReadOnly INVOICE As New MediaType(I_INVOICE)
			''' <summary>
			''' The MediaType instance for Ledger, 11 x 17 in.
			''' </summary>
			Public Shared ReadOnly LEDGER As New MediaType(I_LEDGER)
			''' <summary>
			''' The MediaType instance for North American Letter, 8 1/2 x 11 in.
			''' </summary>
			Public Shared ReadOnly NA_LETTER As New MediaType(I_NA_LETTER)
			''' <summary>
			''' The MediaType instance for North American Legal, 8 1/2 x 14 in.
			''' </summary>
			Public Shared ReadOnly NA_LEGAL As New MediaType(I_NA_LEGAL)
			''' <summary>
			''' The MediaType instance for Quarto, 215 x 275 mm.
			''' </summary>
			Public Shared ReadOnly QUARTO As New MediaType(I_QUARTO)
			''' <summary>
			''' The MediaType instance for Engineering A, 8 1/2 x 11 in.
			''' </summary>
			Public Shared ReadOnly A As New MediaType(I_A)
			''' <summary>
			''' The MediaType instance for Engineering B, 11 x 17 in.
			''' </summary>
			Public Shared ReadOnly B As New MediaType(I_B)
			''' <summary>
			''' The MediaType instance for Engineering C, 17 x 22 in.
			''' </summary>
			Public Shared ReadOnly C As New MediaType(I_C)
			''' <summary>
			''' The MediaType instance for Engineering D, 22 x 34 in.
			''' </summary>
			Public Shared ReadOnly D As New MediaType(I_D)
			''' <summary>
			''' The MediaType instance for Engineering E, 34 x 44 in.
			''' </summary>
			Public Shared ReadOnly E As New MediaType(I_E)
			''' <summary>
			''' The MediaType instance for North American 10 x 15 in.
			''' </summary>
			Public Shared ReadOnly NA_10X15_ENVELOPE As New MediaType(I_NA_10X15_ENVELOPE)
			''' <summary>
			''' The MediaType instance for North American 10 x 14 in.
			''' </summary>
			Public Shared ReadOnly NA_10X14_ENVELOPE As New MediaType(I_NA_10X14_ENVELOPE)
			''' <summary>
			''' The MediaType instance for North American 10 x 13 in.
			''' </summary>
			Public Shared ReadOnly NA_10X13_ENVELOPE As New MediaType(I_NA_10X13_ENVELOPE)
			''' <summary>
			''' The MediaType instance for North American 9 x 12 in.
			''' </summary>
			Public Shared ReadOnly NA_9X12_ENVELOPE As New MediaType(I_NA_9X12_ENVELOPE)
			''' <summary>
			''' The MediaType instance for North American 9 x 11 in.
			''' </summary>
			Public Shared ReadOnly NA_9X11_ENVELOPE As New MediaType(I_NA_9X11_ENVELOPE)
			''' <summary>
			''' The MediaType instance for North American 7 x 9 in.
			''' </summary>
			Public Shared ReadOnly NA_7X9_ENVELOPE As New MediaType(I_NA_7X9_ENVELOPE)
			''' <summary>
			''' The MediaType instance for North American 6 x 9 in.
			''' </summary>
			Public Shared ReadOnly NA_6X9_ENVELOPE As New MediaType(I_NA_6X9_ENVELOPE)
			''' <summary>
			''' The MediaType instance for North American #9 Business Envelope,
			''' 3 7/8 x 8 7/8 in.
			''' </summary>
			Public Shared ReadOnly NA_NUMBER_9_ENVELOPE As New MediaType(I_NA_NUMBER_9_ENVELOPE)
			''' <summary>
			''' The MediaType instance for North American #10 Business Envelope,
			''' 4 1/8 x 9 1/2 in.
			''' </summary>
			Public Shared ReadOnly NA_NUMBER_10_ENVELOPE As New MediaType(I_NA_NUMBER_10_ENVELOPE)
			''' <summary>
			''' The MediaType instance for North American #11 Business Envelope,
			''' 4 1/2 x 10 3/8 in.
			''' </summary>
			Public Shared ReadOnly NA_NUMBER_11_ENVELOPE As New MediaType(I_NA_NUMBER_11_ENVELOPE)
			''' <summary>
			''' The MediaType instance for North American #12 Business Envelope,
			''' 4 3/4 x 11 in.
			''' </summary>
			Public Shared ReadOnly NA_NUMBER_12_ENVELOPE As New MediaType(I_NA_NUMBER_12_ENVELOPE)
			''' <summary>
			''' The MediaType instance for North American #14 Business Envelope,
			''' 5 x 11 1/2 in.
			''' </summary>
			Public Shared ReadOnly NA_NUMBER_14_ENVELOPE As New MediaType(I_NA_NUMBER_14_ENVELOPE)
			''' <summary>
			''' The MediaType instance for Invitation Envelope, 220 x 220 mm.
			''' </summary>
			Public Shared ReadOnly INVITE_ENVELOPE As New MediaType(I_INVITE_ENVELOPE)
			''' <summary>
			''' The MediaType instance for Italy Envelope, 110 x 230 mm.
			''' </summary>
			Public Shared ReadOnly ITALY_ENVELOPE As New MediaType(I_ITALY_ENVELOPE)
			''' <summary>
			''' The MediaType instance for Monarch Envelope, 3 7/8 x 7 1/2 in.
			''' </summary>
			Public Shared ReadOnly MONARCH_ENVELOPE As New MediaType(I_MONARCH_ENVELOPE)
			''' <summary>
			''' The MediaType instance for 6 3/4 envelope, 3 5/8 x 6 1/2 in.
			''' </summary>
			Public Shared ReadOnly PERSONAL_ENVELOPE As New MediaType(I_PERSONAL_ENVELOPE)
			''' <summary>
			''' An alias for ISO_A0.
			''' </summary>
			Public Shared ReadOnly A0 As MediaType = ISO_A0
			''' <summary>
			''' An alias for ISO_A1.
			''' </summary>
			Public Shared ReadOnly A1 As MediaType = ISO_A1
			''' <summary>
			''' An alias for ISO_A2.
			''' </summary>
			Public Shared ReadOnly A2 As MediaType = ISO_A2
			''' <summary>
			''' An alias for ISO_A3.
			''' </summary>
			Public Shared ReadOnly A3 As MediaType = ISO_A3
			''' <summary>
			''' An alias for ISO_A4.
			''' </summary>
			Public Shared ReadOnly A4 As MediaType = ISO_A4
			''' <summary>
			''' An alias for ISO_A5.
			''' </summary>
			Public Shared ReadOnly A5 As MediaType = ISO_A5
			''' <summary>
			''' An alias for ISO_A6.
			''' </summary>
			Public Shared ReadOnly A6 As MediaType = ISO_A6
			''' <summary>
			''' An alias for ISO_A7.
			''' </summary>
			Public Shared ReadOnly A7 As MediaType = ISO_A7
			''' <summary>
			''' An alias for ISO_A8.
			''' </summary>
			Public Shared ReadOnly A8 As MediaType = ISO_A8
			''' <summary>
			''' An alias for ISO_A9.
			''' </summary>
			Public Shared ReadOnly A9 As MediaType = ISO_A9
			''' <summary>
			''' An alias for ISO_A10.
			''' </summary>
			Public Shared ReadOnly A10 As MediaType = ISO_A10
			''' <summary>
			''' An alias for ISO_B0.
			''' </summary>
			Public Shared ReadOnly B0 As MediaType = ISO_B0
			''' <summary>
			''' An alias for ISO_B1.
			''' </summary>
			Public Shared ReadOnly B1 As MediaType = ISO_B1
			''' <summary>
			''' An alias for ISO_B2.
			''' </summary>
			Public Shared ReadOnly B2 As MediaType = ISO_B2
			''' <summary>
			''' An alias for ISO_B3.
			''' </summary>
			Public Shared ReadOnly B3 As MediaType = ISO_B3
			''' <summary>
			''' An alias for ISO_B4.
			''' </summary>
			Public Shared ReadOnly B4 As MediaType = ISO_B4
			''' <summary>
			''' An alias for ISO_B4.
			''' </summary>
			Public Shared ReadOnly ISO_B4_ENVELOPE As MediaType = ISO_B4
			''' <summary>
			''' An alias for ISO_B5.
			''' </summary>
			Public Shared ReadOnly B5 As MediaType = ISO_B5
			''' <summary>
			''' An alias for ISO_B5.
			''' </summary>
			Public Shared ReadOnly ISO_B5_ENVELOPE As MediaType = ISO_B5
			''' <summary>
			''' An alias for ISO_B6.
			''' </summary>
			Public Shared ReadOnly B6 As MediaType = ISO_B6
			''' <summary>
			''' An alias for ISO_B7.
			''' </summary>
			Public Shared ReadOnly B7 As MediaType = ISO_B7
			''' <summary>
			''' An alias for ISO_B8.
			''' </summary>
			Public Shared ReadOnly B8 As MediaType = ISO_B8
			''' <summary>
			''' An alias for ISO_B9.
			''' </summary>
			Public Shared ReadOnly B9 As MediaType = ISO_B9
			''' <summary>
			''' An alias for ISO_B10.
			''' </summary>
			Public Shared ReadOnly B10 As MediaType = ISO_B10
			''' <summary>
			''' An alias for ISO_C0.
			''' </summary>
			Public Shared ReadOnly C0 As MediaType = ISO_C0
			''' <summary>
			''' An alias for ISO_C0.
			''' </summary>
			Public Shared ReadOnly ISO_C0_ENVELOPE As MediaType = ISO_C0
			''' <summary>
			''' An alias for ISO_C1.
			''' </summary>
			Public Shared ReadOnly C1 As MediaType = ISO_C1
			''' <summary>
			''' An alias for ISO_C1.
			''' </summary>
			Public Shared ReadOnly ISO_C1_ENVELOPE As MediaType = ISO_C1
			''' <summary>
			''' An alias for ISO_C2.
			''' </summary>
			Public Shared ReadOnly C2 As MediaType = ISO_C2
			''' <summary>
			''' An alias for ISO_C2.
			''' </summary>
			Public Shared ReadOnly ISO_C2_ENVELOPE As MediaType = ISO_C2
			''' <summary>
			''' An alias for ISO_C3.
			''' </summary>
			Public Shared ReadOnly C3 As MediaType = ISO_C3
			''' <summary>
			''' An alias for ISO_C3.
			''' </summary>
			Public Shared ReadOnly ISO_C3_ENVELOPE As MediaType = ISO_C3
			''' <summary>
			''' An alias for ISO_C4.
			''' </summary>
			Public Shared ReadOnly C4 As MediaType = ISO_C4
			''' <summary>
			''' An alias for ISO_C4.
			''' </summary>
			Public Shared ReadOnly ISO_C4_ENVELOPE As MediaType = ISO_C4
			''' <summary>
			''' An alias for ISO_C5.
			''' </summary>
			Public Shared ReadOnly C5 As MediaType = ISO_C5
			''' <summary>
			''' An alias for ISO_C5.
			''' </summary>
			Public Shared ReadOnly ISO_C5_ENVELOPE As MediaType = ISO_C5
			''' <summary>
			''' An alias for ISO_C6.
			''' </summary>
			Public Shared ReadOnly C6 As MediaType = ISO_C6
			''' <summary>
			''' An alias for ISO_C6.
			''' </summary>
			Public Shared ReadOnly ISO_C6_ENVELOPE As MediaType = ISO_C6
			''' <summary>
			''' An alias for ISO_C7.
			''' </summary>
			Public Shared ReadOnly C7 As MediaType = ISO_C7
			''' <summary>
			''' An alias for ISO_C7.
			''' </summary>
			Public Shared ReadOnly ISO_C7_ENVELOPE As MediaType = ISO_C7
			''' <summary>
			''' An alias for ISO_C8.
			''' </summary>
			Public Shared ReadOnly C8 As MediaType = ISO_C8
			''' <summary>
			''' An alias for ISO_C8.
			''' </summary>
			Public Shared ReadOnly ISO_C8_ENVELOPE As MediaType = ISO_C8
			''' <summary>
			''' An alias for ISO_C9.
			''' </summary>
			Public Shared ReadOnly C9 As MediaType = ISO_C9
			''' <summary>
			''' An alias for ISO_C9.
			''' </summary>
			Public Shared ReadOnly ISO_C9_ENVELOPE As MediaType = ISO_C9
			''' <summary>
			''' An alias for ISO_C10.
			''' </summary>
			Public Shared ReadOnly C10 As MediaType = ISO_C10
			''' <summary>
			''' An alias for ISO_C10.
			''' </summary>
			Public Shared ReadOnly ISO_C10_ENVELOPE As MediaType = ISO_C10
			''' <summary>
			''' An alias for ISO_DESIGNATED_LONG.
			''' </summary>
			Public Shared ReadOnly ISO_DESIGNATED_LONG_ENVELOPE As MediaType = ISO_DESIGNATED_LONG
			''' <summary>
			''' An alias for INVOICE.
			''' </summary>
			Public Shared ReadOnly STATEMENT As MediaType = INVOICE
			''' <summary>
			''' An alias for LEDGER.
			''' </summary>
			Public Shared ReadOnly TABLOID As MediaType = LEDGER
			''' <summary>
			''' An alias for NA_LETTER.
			''' </summary>
			Public Shared ReadOnly LETTER As MediaType = NA_LETTER
			''' <summary>
			''' An alias for NA_LETTER.
			''' </summary>
			Public Shared ReadOnly NOTE As MediaType = NA_LETTER
			''' <summary>
			''' An alias for NA_LEGAL.
			''' </summary>
			Public Shared ReadOnly LEGAL As MediaType = NA_LEGAL
			''' <summary>
			''' An alias for NA_10X15_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_10X15 As MediaType = NA_10X15_ENVELOPE
			''' <summary>
			''' An alias for NA_10X14_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_10X14 As MediaType = NA_10X14_ENVELOPE
			''' <summary>
			''' An alias for NA_10X13_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_10X13 As MediaType = NA_10X13_ENVELOPE
			''' <summary>
			''' An alias for NA_9X12_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_9X12 As MediaType = NA_9X12_ENVELOPE
			''' <summary>
			''' An alias for NA_9X11_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_9X11 As MediaType = NA_9X11_ENVELOPE
			''' <summary>
			''' An alias for NA_7X9_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_7X9 As MediaType = NA_7X9_ENVELOPE
			''' <summary>
			''' An alias for NA_6X9_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_6X9 As MediaType = NA_6X9_ENVELOPE
			''' <summary>
			''' An alias for NA_NUMBER_9_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_9 As MediaType = NA_NUMBER_9_ENVELOPE
			''' <summary>
			''' An alias for NA_NUMBER_10_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_10 As MediaType = NA_NUMBER_10_ENVELOPE
			''' <summary>
			''' An alias for NA_NUMBER_11_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_11 As MediaType = NA_NUMBER_11_ENVELOPE
			''' <summary>
			''' An alias for NA_NUMBER_12_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_12 As MediaType = NA_NUMBER_12_ENVELOPE
			''' <summary>
			''' An alias for NA_NUMBER_14_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_14 As MediaType = NA_NUMBER_14_ENVELOPE
			''' <summary>
			''' An alias for INVITE_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_INVITE As MediaType = INVITE_ENVELOPE
			''' <summary>
			''' An alias for ITALY_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_ITALY As MediaType = ITALY_ENVELOPE
			''' <summary>
			''' An alias for MONARCH_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_MONARCH As MediaType = MONARCH_ENVELOPE
			''' <summary>
			''' An alias for PERSONAL_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ENV_PERSONAL As MediaType = PERSONAL_ENVELOPE
			''' <summary>
			''' An alias for INVITE_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly INVITE As MediaType = INVITE_ENVELOPE
			''' <summary>
			''' An alias for ITALY_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly ITALY As MediaType = ITALY_ENVELOPE
			''' <summary>
			''' An alias for MONARCH_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly MONARCH As MediaType = MONARCH_ENVELOPE
			''' <summary>
			''' An alias for PERSONAL_ENVELOPE.
			''' </summary>
			Public Shared ReadOnly PERSONAL As MediaType = PERSONAL_ENVELOPE

			Private Sub New(  type As Integer)
				MyBase.New(type, NAMES)
			End Sub
		End Class

		''' <summary>
		''' A type-safe enumeration of possible orientations. These orientations
		''' are in partial compliance with IPP 1.1.
		''' @since 1.3
		''' </summary>
		Public NotInheritable Class OrientationRequestedType
			Inherits AttributeValue

			Private Const I_PORTRAIT As Integer = 0
			Private Const I_LANDSCAPE As Integer = 1

			Private Shared ReadOnly NAMES As String() = { "portrait", "landscape" }

			''' <summary>
			''' The OrientationRequestedType instance to use for specifying a
			''' portrait orientation.
			''' </summary>
			Public Shared ReadOnly PORTRAIT As New OrientationRequestedType(I_PORTRAIT)
			''' <summary>
			''' The OrientationRequestedType instance to use for specifying a
			''' landscape orientation.
			''' </summary>
			Public Shared ReadOnly LANDSCAPE As New OrientationRequestedType(I_LANDSCAPE)

			Private Sub New(  type As Integer)
				MyBase.New(type, NAMES)
			End Sub
		End Class

		''' <summary>
		''' A type-safe enumeration of possible origins.
		''' @since 1.3
		''' </summary>
		Public NotInheritable Class OriginType
			Inherits AttributeValue

			Private Const I_PHYSICAL As Integer = 0
			Private Const I_PRINTABLE As Integer = 1

			Private Shared ReadOnly NAMES As String() = { "physical", "printable" }

			''' <summary>
			''' The OriginType instance to use for specifying a physical origin.
			''' </summary>
			Public Shared ReadOnly PHYSICAL As New OriginType(I_PHYSICAL)
			''' <summary>
			''' The OriginType instance to use for specifying a printable origin.
			''' </summary>
			Public Shared ReadOnly PRINTABLE As New OriginType(I_PRINTABLE)

			Private Sub New(  type As Integer)
				MyBase.New(type, NAMES)
			End Sub
		End Class

		''' <summary>
		''' A type-safe enumeration of possible print qualities. These print
		''' qualities are in compliance with IPP 1.1.
		''' @since 1.3
		''' </summary>
		Public NotInheritable Class PrintQualityType
			Inherits AttributeValue

			Private Const I_HIGH As Integer = 0
			Private Const I_NORMAL As Integer = 1
			Private Const I_DRAFT As Integer = 2

			Private Shared ReadOnly NAMES As String() = { "high", "normal", "draft" }

			''' <summary>
			''' The PrintQualityType instance to use for specifying a high print
			''' quality.
			''' </summary>
			Public Shared ReadOnly HIGH As New PrintQualityType(I_HIGH)
			''' <summary>
			''' The PrintQualityType instance to use for specifying a normal print
			''' quality.
			''' </summary>
			Public Shared ReadOnly NORMAL As New PrintQualityType(I_NORMAL)
			''' <summary>
			''' The PrintQualityType instance to use for specifying a draft print
			''' quality.
			''' </summary>
			Public Shared ReadOnly DRAFT As New PrintQualityType(I_DRAFT)

			Private Sub New(  type As Integer)
				MyBase.New(type, NAMES)
			End Sub
		End Class

		Private color_Renamed As ColorType
		Private media As MediaType
		Private orientationRequested As OrientationRequestedType
		Private origin As OriginType
		Private printQuality As PrintQualityType
		Private printerResolution As Integer()

		''' <summary>
		''' Constructs a PageAttributes instance with default values for every
		''' attribute.
		''' </summary>
		Public Sub New()
			color = ColorType.MONOCHROME
			mediaToDefaultult()
			orientationRequestedToDefaultult()
			origin = OriginType.PHYSICAL
			printQualityToDefaultult()
			printerResolutionToDefaultult()
		End Sub

		''' <summary>
		''' Constructs a PageAttributes instance which is a copy of the supplied
		''' PageAttributes.
		''' </summary>
		''' <param name="obj"> the PageAttributes to copy. </param>
		Public Sub New(  obj As PageAttributes)
			[set](obj)
		End Sub

		''' <summary>
		''' Constructs a PageAttributes instance with the specified values for
		''' every attribute.
		''' </summary>
		''' <param name="color"> ColorType.COLOR or ColorType.MONOCHROME. </param>
		''' <param name="media"> one of the constant fields of the MediaType class. </param>
		''' <param name="orientationRequested"> OrientationRequestedType.PORTRAIT or
		'''          OrientationRequestedType.LANDSCAPE. </param>
		''' <param name="origin"> OriginType.PHYSICAL or OriginType.PRINTABLE </param>
		''' <param name="printQuality"> PrintQualityType.DRAFT, PrintQualityType.NORMAL,
		'''          or PrintQualityType.HIGH </param>
		''' <param name="printerResolution"> an integer array of 3 elements. The first
		'''          element must be greater than 0. The second element must be
		'''          must be greater than 0. The third element must be either
		'''          <code>3</code> or <code>4</code>. </param>
		''' <exception cref="IllegalArgumentException"> if one or more of the above
		'''          conditions is violated. </exception>
		Public Sub New(  color_Renamed As ColorType,   media As MediaType,   orientationRequested As OrientationRequestedType,   origin As OriginType,   printQuality As PrintQualityType,   printerResolution As Integer())
			color = color_Renamed
			media = media
			orientationRequested = orientationRequested
			origin = origin
			printQuality = printQuality
			printerResolution = printerResolution
		End Sub

		''' <summary>
		''' Creates and returns a copy of this PageAttributes.
		''' </summary>
		''' <returns>  the newly created copy. It is safe to cast this Object into
		'''          a PageAttributes. </returns>
		Public Function clone() As Object
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' Since we implement Cloneable, this should never happen
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Sets all of the attributes of this PageAttributes to the same values as
		''' the attributes of obj.
		''' </summary>
		''' <param name="obj"> the PageAttributes to copy. </param>
		Public Sub [set](  obj As PageAttributes)
			color_Renamed = obj.color_Renamed
			media = obj.media
			orientationRequested = obj.orientationRequested
			origin = obj.origin
			printQuality = obj.printQuality
			' okay because we never modify the contents of printerResolution
			printerResolution = obj.printerResolution
		End Sub

		''' <summary>
		''' Returns whether pages using these attributes will be rendered in
		''' color or monochrome. This attribute is updated to the value chosen
		''' by the user.
		''' </summary>
		''' <returns>  ColorType.COLOR or ColorType.MONOCHROME. </returns>
		Public Property color As ColorType
			Get
				Return color_Renamed
			End Get
			Set(  color_Renamed As ColorType)
				If color_Renamed Is Nothing Then Throw New IllegalArgumentException("Invalid value for attribute " & "color")
				Me.color_Renamed = color_Renamed
			End Set
		End Property


		''' <summary>
		''' Returns the paper size for pages using these attributes. This
		''' attribute is updated to the value chosen by the user.
		''' </summary>
		''' <returns>  one of the constant fields of the MediaType class. </returns>
		Public Property media As MediaType
			Get
				Return media
			End Get
			Set(  media As MediaType)
				If media Is Nothing Then Throw New IllegalArgumentException("Invalid value for attribute " & "media")
				Me.media = media
			End Set
		End Property


		''' <summary>
		''' Sets the paper size for pages using these attributes to the default
		''' size for the default locale. The default size for locales in the
		''' United States and Canada is MediaType.NA_LETTER. The default size for
		''' all other locales is MediaType.ISO_A4.
		''' </summary>
		Public Sub setMediaToDefault()
			Dim defaultCountry As String = java.util.Locale.default.country
			If defaultCountry IsNot Nothing AndAlso (defaultCountry.Equals(java.util.Locale.US.country) OrElse defaultCountry.Equals(java.util.Locale.CANADA.country)) Then
				media = MediaType.NA_LETTER
			Else
				media = MediaType.ISO_A4
			End If
		End Sub

		''' <summary>
		''' Returns the print orientation for pages using these attributes. This
		''' attribute is updated to the value chosen by the user.
		''' </summary>
		''' <returns>  OrientationRequestedType.PORTRAIT or
		'''          OrientationRequestedType.LANDSCAPE. </returns>
		Public Property orientationRequested As OrientationRequestedType
			Get
				Return orientationRequested
			End Get
			Set(  orientationRequested As OrientationRequestedType)
				If orientationRequested Is Nothing Then Throw New IllegalArgumentException("Invalid value for attribute " & "orientationRequested")
				Me.orientationRequested = orientationRequested
			End Set
		End Property


		''' <summary>
		''' Specifies the print orientation for pages using these attributes.
		''' Specifying <code>3</code> denotes portrait. Specifying <code>4</code>
		''' denotes landscape. Specifying any other value will generate an
		''' IllegalArgumentException. Not specifying the property is equivalent
		''' to calling setOrientationRequested(OrientationRequestedType.PORTRAIT).
		''' </summary>
		''' <param name="orientationRequested"> <code>3</code> or <code>4</code> </param>
		''' <exception cref="IllegalArgumentException"> if orientationRequested is not
		'''          <code>3</code> or <code>4</code> </exception>
		Public Property orientationRequested As Integer
			Set(  orientationRequested As Integer)
				Select Case orientationRequested
				  Case 3
					orientationRequested = OrientationRequestedType.PORTRAIT
				  Case 4
					orientationRequested = OrientationRequestedType.LANDSCAPE
				  Case Else
					' This will throw an IllegalArgumentException
					orientationRequested = Nothing
				End Select
			End Set
		End Property

		''' <summary>
		''' Sets the print orientation for pages using these attributes to the
		''' default. The default orientation is portrait.
		''' </summary>
		Public Sub setOrientationRequestedToDefault()
			orientationRequested = OrientationRequestedType.PORTRAIT
		End Sub

		''' <summary>
		''' Returns whether drawing at (0, 0) to pages using these attributes
		''' draws at the upper-left corner of the physical page, or at the
		''' upper-left corner of the printable area. (Note that these locations
		''' could be equivalent.) This attribute cannot be modified by,
		''' and is not subject to any limitations of, the implementation or the
		''' target printer.
		''' </summary>
		''' <returns>  OriginType.PHYSICAL or OriginType.PRINTABLE </returns>
		Public Property origin As OriginType
			Get
				Return origin
			End Get
			Set(  origin As OriginType)
				If origin Is Nothing Then Throw New IllegalArgumentException("Invalid value for attribute " & "origin")
				Me.origin = origin
			End Set
		End Property


		''' <summary>
		''' Returns the print quality for pages using these attributes. This
		''' attribute is updated to the value chosen by the user.
		''' </summary>
		''' <returns>  PrintQualityType.DRAFT, PrintQualityType.NORMAL, or
		'''          PrintQualityType.HIGH </returns>
		Public Property printQuality As PrintQualityType
			Get
				Return printQuality
			End Get
			Set(  printQuality As PrintQualityType)
				If printQuality Is Nothing Then Throw New IllegalArgumentException("Invalid value for attribute " & "printQuality")
				Me.printQuality = printQuality
			End Set
		End Property


		''' <summary>
		''' Specifies the print quality for pages using these attributes.
		''' Specifying <code>3</code> denotes draft. Specifying <code>4</code>
		''' denotes normal. Specifying <code>5</code> denotes high. Specifying
		''' any other value will generate an IllegalArgumentException. Not
		''' specifying the property is equivalent to calling
		''' setPrintQuality(PrintQualityType.NORMAL).
		''' </summary>
		''' <param name="printQuality"> <code>3</code>, <code>4</code>, or <code>5</code> </param>
		''' <exception cref="IllegalArgumentException"> if printQuality is not <code>3
		'''          </code>, <code>4</code>, or <code>5</code> </exception>
		Public Property printQuality As Integer
			Set(  printQuality As Integer)
				Select Case printQuality
				  Case 3
					printQuality = PrintQualityType.DRAFT
				  Case 4
					printQuality = PrintQualityType.NORMAL
				  Case 5
					printQuality = PrintQualityType.HIGH
				  Case Else
					' This will throw an IllegalArgumentException
					printQuality = Nothing
				End Select
			End Set
		End Property

		''' <summary>
		''' Sets the print quality for pages using these attributes to the default.
		''' The default print quality is normal.
		''' </summary>
		Public Sub setPrintQualityToDefault()
			printQuality = PrintQualityType.NORMAL
		End Sub

		''' <summary>
		''' Returns the print resolution for pages using these attributes.
		''' Index 0 of the array specifies the cross feed direction resolution
		''' (typically the horizontal resolution). Index 1 of the array specifies
		''' the feed direction resolution (typically the vertical resolution).
		''' Index 2 of the array specifies whether the resolutions are in dots per
		''' inch or dots per centimeter. <code>3</code> denotes dots per inch.
		''' <code>4</code> denotes dots per centimeter.
		''' </summary>
		''' <returns>  an integer array of 3 elements. The first
		'''          element must be greater than 0. The second element must be
		'''          must be greater than 0. The third element must be either
		'''          <code>3</code> or <code>4</code>. </returns>
		Public Property printerResolution As Integer()
			Get
				' Return a copy because otherwise client code could circumvent the
				' the checks made in setPrinterResolution by modifying the
				' returned array.
				Dim copy As Integer() = New Integer(2){}
				copy(0) = printerResolution(0)
				copy(1) = printerResolution(1)
				copy(2) = printerResolution(2)
				Return copy
			End Get
			Set(  printerResolution As Integer())
				If printerResolution Is Nothing OrElse printerResolution.Length <> 3 OrElse printerResolution(0) <= 0 OrElse printerResolution(1) <= 0 OrElse (printerResolution(2) <> 3 AndAlso printerResolution(2) <> 4) Then Throw New IllegalArgumentException("Invalid value for attribute " & "printerResolution")
				' Store a copy because otherwise client code could circumvent the
				' the checks made above by holding a reference to the array and
				' modifying it after calling setPrinterResolution.
				Dim copy As Integer() = New Integer(2){}
				copy(0) = printerResolution(0)
				copy(1) = printerResolution(1)
				copy(2) = printerResolution(2)
				Me.printerResolution = copy
			End Set
		End Property


		''' <summary>
		''' Specifies the desired cross feed and feed print resolutions in dots per
		''' inch for pages using these attributes. The same value is used for both
		''' resolutions. The actual resolutions will be determined by the
		''' limitations of the implementation and the target printer. Not
		''' specifying the property is equivalent to specifying <code>72</code>.
		''' </summary>
		''' <param name="printerResolution"> an integer greater than 0. </param>
		''' <exception cref="IllegalArgumentException"> if printerResolution is less than or
		'''          equal to 0. </exception>
		Public Property printerResolution As Integer
			Set(  printerResolution As Integer)
				printerResolution = New Integer() { printerResolution, printerResolution, 3 }
			End Set
		End Property

		''' <summary>
		''' Sets the printer resolution for pages using these attributes to the
		''' default. The default is 72 dpi for both the feed and cross feed
		''' resolutions.
		''' </summary>
		Public Sub setPrinterResolutionToDefault()
			printerResolution = 72
		End Sub

		''' <summary>
		''' Determines whether two PageAttributes are equal to each other.
		''' <p>
		''' Two PageAttributes are equal if and only if each of their attributes are
		''' equal. Attributes of enumeration type are equal if and only if the
		''' fields refer to the same unique enumeration object. This means that
		''' an aliased media is equal to its underlying unique media. Printer
		''' resolutions are equal if and only if the feed resolution, cross feed
		''' resolution, and units are equal.
		''' </summary>
		''' <param name="obj"> the object whose equality will be checked. </param>
		''' <returns>  whether obj is equal to this PageAttribute according to the
		'''          above criteria. </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Not(TypeOf obj Is PageAttributes) Then Return False

			Dim rhs As PageAttributes = CType(obj, PageAttributes)

			Return (color_Renamed Is rhs.color_Renamed AndAlso media Is rhs.media AndAlso orientationRequested Is rhs.orientationRequested AndAlso origin Is rhs.origin AndAlso printQuality Is rhs.printQuality AndAlso printerResolution(0) = rhs.printerResolution(0) AndAlso printerResolution(1) = rhs.printerResolution(1) AndAlso printerResolution(2) = rhs.printerResolution(2))
		End Function

		''' <summary>
		''' Returns a hash code value for this PageAttributes.
		''' </summary>
		''' <returns>  the hash code. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return (color_Renamed.GetHashCode() << 31 Xor media.GetHashCode() << 24 Xor orientationRequested.GetHashCode() << 23 Xor origin.GetHashCode() << 22 Xor printQuality.GetHashCode() << 20 Xor printerResolution(2) >> 2 << 19 Xor printerResolution(1) << 10 Xor printerResolution(0))
		End Function

		''' <summary>
		''' Returns a string representation of this PageAttributes.
		''' </summary>
		''' <returns>  the string representation. </returns>
		Public Overrides Function ToString() As String
			' int[] printerResolution = getPrinterResolution();
			Return "color=" & color & ",media=" & media & ",orientation-requested=" & orientationRequested & ",origin=" & origin & ",print-quality=" & printQuality & ",printer-resolution=[" & printerResolution(0) & "," & printerResolution(1) & "," & printerResolution(2) & "]"
		End Function
	End Class

End Namespace
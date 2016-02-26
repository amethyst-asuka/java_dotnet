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
	''' Class MediaName is a subclass of Media, a printing attribute class (an
	''' enumeration) that specifies the media for a print job as a name.
	''' <P>
	''' This attribute can be used instead of specifying MediaSize or MediaTray.
	''' <p>
	''' Class MediaName currently declares a few standard media names.
	''' Implementation- or site-defined names for a media name attribute may also
	''' be created by defining a subclass of class MediaName.
	''' <P>
	''' <B>IPP Compatibility:</B> MediaName is a representation class for
	''' values of the IPP "media" attribute which names media.
	''' <P>
	''' 
	''' </summary>
	Public Class MediaName
		Inherits Media
		Implements javax.print.attribute.Attribute

		Private Const serialVersionUID As Long = 4653117714524155448L

		''' <summary>
		'''  white letter paper.
		''' </summary>
		Public Shared ReadOnly NA_LETTER_WHITE As New MediaName(0)

		''' <summary>
		'''  letter transparency.
		''' </summary>
		Public Shared ReadOnly NA_LETTER_TRANSPARENT As New MediaName(1)

		''' <summary>
		''' white A4 paper.
		''' </summary>
		Public Shared ReadOnly ISO_A4_WHITE As New MediaName(2)


		''' <summary>
		'''  A4 transparency.
		''' </summary>
		Public Shared ReadOnly ISO_A4_TRANSPARENT As New MediaName(3)


		''' <summary>
		''' Constructs a new media name enumeration value with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "na-letter-white", "na-letter-transparent", "iso-a4-white", "iso-a4-transparent" }

		Private Shared ReadOnly myEnumValueTable As MediaName() = { NA_LETTER_WHITE, NA_LETTER_TRANSPARENT, ISO_A4_WHITE, ISO_A4_TRANSPARENT }

		''' <summary>
		''' Returns the string table for class MediaTray. </summary>
		''' <returns> the String table. </returns>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return CType(myStringTable.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class MediaTray. </summary>
		''' <returns> the enumeration value table. </returns>
		Protected Friend Property Overrides enumValueTable As javax.print.attribute.EnumSyntax()
			Get
				Return CType(myEnumValueTable.clone(), javax.print.attribute.EnumSyntax())
			End Get
		End Property

	End Class

End Namespace
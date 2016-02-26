'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Class MediaTray is a subclass of Media.
	''' Class MediaTray is a printing attribute class, an enumeration, that
	''' specifies the media tray or bin for the job.
	''' This attribute can be used instead of specifying MediaSize or MediaName.
	''' <p>
	''' Class MediaTray declares keywords for standard media kind values.
	''' Implementation- or site-defined names for a media kind attribute may also
	''' be created by defining a subclass of class MediaTray.
	''' <P>
	''' <B>IPP Compatibility:</B> MediaTray is a representation class for
	''' values of the IPP "media" attribute which name paper trays.
	''' <P>
	''' 
	''' </summary>
	Public Class MediaTray
		Inherits Media
		Implements javax.print.attribute.Attribute

		Private Const serialVersionUID As Long = -982503611095214703L

		''' <summary>
		''' The top input tray in the printer.
		''' </summary>
		Public Shared ReadOnly TOP As New MediaTray(0)

		''' <summary>
		''' The middle input tray in the printer.
		''' </summary>
		Public Shared ReadOnly MIDDLE As New MediaTray(1)

		''' <summary>
		''' The bottom input tray in the printer.
		''' </summary>
		Public Shared ReadOnly BOTTOM As New MediaTray(2)

		''' <summary>
		''' The envelope input tray in the printer.
		''' </summary>
		Public Shared ReadOnly ENVELOPE As New MediaTray(3)

		''' <summary>
		''' The manual feed input tray in the printer.
		''' </summary>
		Public Shared ReadOnly MANUAL As New MediaTray(4)

		''' <summary>
		''' The large capacity input tray in the printer.
		''' </summary>
		Public Shared ReadOnly LARGE_CAPACITY As New MediaTray(5)

		''' <summary>
		''' The main input tray in the printer.
		''' </summary>
		Public Shared ReadOnly MAIN As New MediaTray(6)

		''' <summary>
		''' The side input tray.
		''' </summary>
		Public Shared ReadOnly SIDE As New MediaTray(7)

		''' <summary>
		''' Construct a new media tray enumeration value with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() ={ "top", "middle", "bottom", "envelope", "manual", "large-capacity", "main", "side" }

		Private Shared ReadOnly myEnumValueTable As MediaTray() = { TOP, MIDDLE, BOTTOM, ENVELOPE, MANUAL, LARGE_CAPACITY, MAIN, SIDE }

		''' <summary>
		''' Returns the string table for class MediaTray.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return CType(myStringTable.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class MediaTray.
		''' </summary>
		Protected Friend Property Overrides enumValueTable As javax.print.attribute.EnumSyntax()
			Get
				Return CType(myEnumValueTable.clone(), javax.print.attribute.EnumSyntax())
			End Get
		End Property


	End Class

End Namespace
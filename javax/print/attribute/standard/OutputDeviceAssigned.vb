Imports System

'
' * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' Class OutputDeviceAssigned is a printing attribute class, a text attribute,
	''' that identifies the output device to which the service has assigned this
	''' job. If an output device implements an embedded Print Service instance, the
	''' printer need not set this attribute. If a print server implements a
	''' Print Service instance, the value may be empty (zero- length string) or not
	''' returned until the service assigns an output device to the job. This
	''' attribute is particularly useful when a single service supports multiple
	''' devices (so called "fan-out").
	''' <P>
	''' <B>IPP Compatibility:</B> The string value gives the IPP name value. The
	''' locale gives the IPP natural language. The category name returned by
	''' <CODE>getName()</CODE> gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class OutputDeviceAssigned
		Inherits javax.print.attribute.TextSyntax
		Implements javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = 5486733778854271081L

		''' <summary>
		''' Constructs a new output device assigned attribute with the given device
		''' name and locale.
		''' </summary>
		''' <param name="deviceName">  Device name. </param>
		''' <param name="locale">      Natural language of the text string. null
		''' is interpreted to mean the default locale as returned
		''' by <code>Locale.getDefault()</code>
		''' </param>
		''' <exception cref="NullPointerException">
		'''   (unchecked exception) Thrown if <CODE>deviceName</CODE> is null. </exception>
		Public Sub New(ByVal deviceName As String, ByVal locale As java.util.Locale)

			MyBase.New(deviceName, locale)
		End Sub

		' Exported operations inherited and overridden from class Object.

		''' <summary>
		''' Returns whether this output device assigned attribute is equivalent to
		''' the passed in object. To be equivalent, all of the following conditions
		''' must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class OutputDeviceAssigned.
		''' <LI>
		''' This output device assigned attribute's underlying string and
		''' <CODE>object</CODE>'s underlying string are equal.
		''' <LI>
		''' This output device assigned attribute's locale and
		''' <CODE>object</CODE>'s locale are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this output
		'''          device assigned attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is OutputDeviceAssigned)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class OutputDeviceAssigned, the
		''' category is class OutputDeviceAssigned itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(OutputDeviceAssigned)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class OutputDeviceAssigned, the
		''' category name is <CODE>"output-device-assigned"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "output-device-assigned"
			End Get
		End Property

	End Class

End Namespace
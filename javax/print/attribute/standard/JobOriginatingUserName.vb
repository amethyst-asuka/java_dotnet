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
	''' Class JobOriginatingUserName is a printing attribute class, a text
	''' attribute, that contains the name of the end user that submitted the
	''' print job. If possible, the printer sets this attribute to the most
	''' authenticated printable user name that it can obtain from the
	''' authentication service that authenticated the submitted Print Request.
	''' If such is not available, the printer uses the value of the
	''' <seealso cref="RequestingUserName RequestingUserName"/>
	''' attribute supplied by the client in the Print Request's attribute set.
	''' If no authentication service is available, and the client did not supply
	''' a <seealso cref="RequestingUserName RequestingUserName"/> attribute,
	''' the printer sets the JobOriginatingUserName attribute to an empty
	''' (zero-length) string.
	''' <P>
	''' <B>IPP Compatibility:</B> The string value gives the IPP name value. The
	''' locale gives the IPP natural language. The category name returned by
	''' <CODE>getName()</CODE> gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class JobOriginatingUserName
		Inherits javax.print.attribute.TextSyntax
		Implements javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = -8052537926362933477L

		''' <summary>
		''' Constructs a new job originating user name attribute with the given
		''' user name and locale.
		''' </summary>
		''' <param name="userName">  User name. </param>
		''' <param name="locale">    Natural language of the text string. null
		''' is interpreted to mean the default locale as returned
		''' by <code>Locale.getDefault()</code>
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>userName</CODE> is null. </exception>
		Public Sub New(ByVal userName As String, ByVal locale As java.util.Locale)
			MyBase.New(userName, locale)
		End Sub

		''' <summary>
		''' Returns whether this job originating user name attribute is equivalent to
		''' the passed in object. To be equivalent, all of the following conditions
		''' must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class JobOriginatingUserName.
		''' <LI>
		''' This job originating user name attribute's underlying string and
		''' <CODE>object</CODE>'s underlying string are equal.
		''' <LI>
		''' This job originating user name attribute's locale and
		''' <CODE>object</CODE>'s locale are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this job
		'''          originating user name attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is JobOriginatingUserName)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class JobOriginatingUserName, the
		''' category is class JobOriginatingUserName itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(JobOriginatingUserName)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class JobOriginatingUserName, the
		''' category name is <CODE>"job-originating-user-name"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "job-originating-user-name"
			End Get
		End Property

	End Class

End Namespace
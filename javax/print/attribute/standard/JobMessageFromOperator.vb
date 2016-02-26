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
	''' Class JobMessageFromOperator is a printing attribute class, a text attribute,
	''' that provides a message from an operator, system administrator, or
	''' "intelligent" process to indicate to the end user the reasons for
	''' modification or other management action taken on a job.
	''' <P>
	''' A Print Job's attribute set includes zero instances or one instance of a
	''' JobMessageFromOperator attribute, not more than one instance. A new
	''' JobMessageFromOperator attribute replaces an existing JobMessageFromOperator
	''' attribute, if any. In other words, JobMessageFromOperator is not intended to
	''' be a history log. If it wishes, the client can detect changes to a Print
	''' Job's JobMessageFromOperator attribute and maintain the client's own history
	''' log of the JobMessageFromOperator attribute values.
	''' <P>
	''' <B>IPP Compatibility:</B> The string value gives the IPP name value. The
	''' locale gives the IPP natural language. The category name returned by
	''' <CODE>getName()</CODE> gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class JobMessageFromOperator
		Inherits javax.print.attribute.TextSyntax
		Implements javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = -4620751846003142047L

		''' <summary>
		''' Constructs a new job message from operator attribute with the given
		''' message and locale.
		''' </summary>
		''' <param name="message">  Message. </param>
		''' <param name="locale">   Natural language of the text string. null
		''' is interpreted to mean the default locale as returned
		''' by <code>Locale.getDefault()</code>
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>message</CODE> is null. </exception>
		Public Sub New(ByVal message As String, ByVal locale As java.util.Locale)
			MyBase.New(message, locale)
		End Sub

		''' <summary>
		''' Returns whether this job message from operator attribute is equivalent to
		''' the passed in object. To be equivalent, all of the following conditions
		''' must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class JobMessageFromOperator.
		''' <LI>
		''' This job message from operator attribute's underlying string and
		''' <CODE>object</CODE>'s underlying string are equal.
		''' <LI>
		''' This job message from operator attribute's locale and
		''' <CODE>object</CODE>'s locale are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this job
		'''          message from operator attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is JobMessageFromOperator)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class JobMessageFromOperator, the
		''' category is class JobMessageFromOperator itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(JobMessageFromOperator)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class JobMessageFromOperator, the
		''' category name is <CODE>"job-message-from-operator"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "job-message-from-operator"
			End Get
		End Property

	End Class

End Namespace
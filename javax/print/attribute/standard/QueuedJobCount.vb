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
	''' Class QueuedJobCount is an integer valued printing attribute that indicates
	''' the number of jobs in the printer whose <seealso cref="JobState JobState"/> is either
	''' PENDING, PENDING_HELD, PROCESSING, or PROCESSING_STOPPED.
	''' <P>
	''' <B>IPP Compatibility:</B> The integer value gives the IPP integer value.
	''' The category name returned by <CODE>getName()</CODE> gives the IPP
	''' attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class QueuedJobCount
		Inherits javax.print.attribute.IntegerSyntax
		Implements javax.print.attribute.PrintServiceAttribute

		Private Const serialVersionUID As Long = 7499723077864047742L

		''' <summary>
		''' Construct a new queued job count attribute with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''   (Unchecked exception) Thrown if <CODE>value</CODE> is less than 0. </exception>
		Public Sub New(ByVal value As Integer)
			MyBase.New(value, 0, Integer.MaxValue)
		End Sub

		''' <summary>
		''' Returns whether this queued job count attribute is equivalent to the
		''' passed in object. To be equivalent, all of the following conditions
		''' mus  be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class QueuedJobCount.
		''' <LI>
		''' This queued job count attribute's value and <CODE>object</CODE>'s
		''' value are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this queued job
		'''          count attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is QueuedJobCount)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class QueuedJobCount, the category is class QueuedJobCount itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(QueuedJobCount)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class QueuedJobCount, the
		''' category name is <CODE>"queued-job-count"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "queued-job-count"
			End Get
		End Property

	End Class

End Namespace
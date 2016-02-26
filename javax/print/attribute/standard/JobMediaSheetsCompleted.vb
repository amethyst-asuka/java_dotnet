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
	''' Class JobMediaSheetsCompleted is an integer valued printing attribute class
	''' that specifies the number of media sheets which have completed marking and
	''' stacking for the entire job so far, whether those sheets have been processed
	''' on one side or on both.
	''' <P>
	''' The JobMediaSheetsCompleted attribute describes the progress of the job. This
	''' attribute is intended to be a counter. That is, the JobMediaSheetsCompleted
	''' value for a job that has not started processing must be 0. When the job's
	''' <seealso cref="JobState JobState"/> is PROCESSING or PROCESSING_STOPPED, the
	''' JobMediaSheetsCompleted value is intended to increase as the job is
	''' processed; it indicates the amount of the job that has been processed at the
	''' time the Print Job's attribute set is queried or at the time a print job
	''' event is reported. When the job enters the COMPLETED, CANCELED, or ABORTED
	''' states, the JobMediaSheetsCompleted value is the final value for the job.
	''' <P>
	''' <B>IPP Compatibility:</B> The integer value gives the IPP integer value. The
	''' category name returned by <CODE>getName()</CODE> gives the IPP attribute
	''' name.
	''' <P>
	''' </summary>
	''' <seealso cref= JobMediaSheets </seealso>
	''' <seealso cref= JobMediaSheetsSupported </seealso>
	''' <seealso cref= JobKOctetsProcessed </seealso>
	''' <seealso cref= JobImpressionsCompleted
	''' 
	''' @author  Alan Kaminsky </seealso>
	Public NotInheritable Class JobMediaSheetsCompleted
		Inherits javax.print.attribute.IntegerSyntax
		Implements javax.print.attribute.PrintJobAttribute


		Private Const serialVersionUID As Long = 1739595973810840475L

		''' <summary>
		''' Construct a new job media sheets completed attribute with the given
		''' integer value.
		''' </summary>
		''' <param name="value">  Integer value.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''   (Unchecked exception) Thrown if <CODE>value</CODE> is less than 0. </exception>
		Public Sub New(ByVal value As Integer)
			MyBase.New(value, 0, Integer.MaxValue)
		End Sub

		''' <summary>
		''' Returns whether this job media sheets completed attribute is equivalent
		''' to the passed in object. To be equivalent, all of the following
		''' conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class JobMediaSheetsCompleted.
		''' <LI>
		''' This job media sheets completed attribute's value and
		''' <CODE>object</CODE>'s value are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this job media
		'''          sheets completed attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is JobMediaSheetsCompleted)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class JobMediaSheetsCompleted, the category is class
		''' JobMediaSheetsCompleted itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(JobMediaSheetsCompleted)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class JobMediaSheetsCompleted, the category name is
		''' <CODE>"job-media-sheets-completed"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "job-media-sheets-completed"
			End Get
		End Property
	End Class

End Namespace
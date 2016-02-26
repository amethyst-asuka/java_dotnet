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
	''' Class JobKOctetsProcessed is an integer valued printing attribute class that
	''' specifies the total number of print data octets processed so far in K octets,
	''' i.e., in units of 1024 octets. The value must be rounded up, so that a job
	''' between 1 and 1024 octets inclusive must be indicated as being 1K octets,
	''' 1025 to 2048 inclusive must be 2K, etc. For a multidoc print job (a job with
	''' multiple documents), the JobKOctetsProcessed value is computed by adding up
	''' the individual documents' number of octets processed so far, then rounding up
	''' to the next K octets value.
	''' <P>
	''' The JobKOctetsProcessed attribute describes the progress of the job. This
	''' attribute is intended to be a counter. That is, the JobKOctetsProcessed value
	''' for a job that has not started processing must be 0. When the job's {@link
	''' JobState JobState} is PROCESSING or PROCESSING_STOPPED, the
	''' JobKOctetsProcessed value is intended to increase as the job is processed; it
	''' indicates the amount of the job that has been processed at the time the Print
	''' Job's attribute set is queried or at the time a print job event is reported.
	''' When the job enters the COMPLETED, CANCELED, or ABORTED states, the
	''' JobKOctetsProcessed value is the final value for the job.
	''' <P>
	''' For implementations where multiple copies are produced by the interpreter
	''' with only a single pass over the data, the final value of the
	''' JobKOctetsProcessed attribute must be equal to the value of the {@link
	''' JobKOctets JobKOctets} attribute. For implementations where multiple copies
	''' are produced by the interpreter by processing the data for each copy, the
	''' final value must be a multiple of the value of the {@link JobKOctets
	''' JobKOctets} attribute.
	''' <P>
	''' <B>IPP Compatibility:</B> The integer value gives the IPP integer value. The
	''' category name returned by <CODE>getName()</CODE> gives the IPP attribute
	''' name.
	''' <P>
	''' </summary>
	''' <seealso cref= JobKOctets </seealso>
	''' <seealso cref= JobKOctetsSupported </seealso>
	''' <seealso cref= JobImpressionsCompleted </seealso>
	''' <seealso cref= JobMediaSheetsCompleted
	''' 
	''' @author  Alan Kaminsky </seealso>
	Public NotInheritable Class JobKOctetsProcessed
		Inherits javax.print.attribute.IntegerSyntax
		Implements javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = -6265238509657881806L

		''' <summary>
		''' Construct a new job K octets processed attribute with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''  (Unchecked exception) Thrown if <CODE>value</CODE> is less than 0. </exception>
		Public Sub New(ByVal value As Integer)
			MyBase.New(value, 0, Integer.MaxValue)
		End Sub

		''' <summary>
		''' Returns whether this job K octets processed attribute is equivalent to
		''' the passed in object. To be equivalent, all of the following conditions
		''' must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class JobKOctetsProcessed.
		''' <LI>
		''' This job K octets processed attribute's value and
		''' <CODE>object</CODE>'s value are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this job K
		'''          octets processed attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is JobKOctetsProcessed)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class JobKOctetsProcessed, the category is class
		''' JobKOctetsProcessed itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(JobKOctetsProcessed)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class JobKOctetsProcessed, the category name is
		''' <CODE>"job-k-octets-processed"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "job-k-octets-processed"
			End Get
		End Property

	End Class

End Namespace
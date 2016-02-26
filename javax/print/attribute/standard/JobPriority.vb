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
	''' Class JobPriority is an integer valued printing attribute class that
	''' specifies a print job's priority.
	''' <P>
	''' If a JobPriority attribute is specified for a Print Job, it specifies a
	''' priority for scheduling the job. A higher value specifies a higher priority.
	''' The value 1 indicates the lowest possible priority. The value 100 indicates
	''' the highest possible priority. Among those jobs that are ready to print, a
	''' printer must print all jobs with a priority value of <I>n</I> before printing
	''' those with a priority value of <I>n</I>-1 for all <I>n.</I>
	''' <P>
	''' If the client does not specify a JobPriority attribute for a Print Job and
	''' the printer does support the JobPriority attribute, the printer must use an
	''' implementation-defined default JobPriority value.
	''' <P>
	''' The client can always specify any job priority value from 1 to 100 for a job.
	''' However, a Print Service instance may support fewer than 100 different
	''' job priority levels. If this is the case, the Print Service instance
	''' automatically maps the client-specified job priority value to one of the
	''' supported job priority levels, dividing the 100 job priority values equally
	''' among the available job priority levels.
	''' <P>
	''' <B>IPP Compatibility:</B> The integer value gives the IPP integer value. The
	''' category name returned by <CODE>getName()</CODE> gives the IPP attribute
	''' name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class JobPriority
		Inherits javax.print.attribute.IntegerSyntax
		Implements javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = -4599900369040602769L

		''' <summary>
		''' Construct a new job priority attribute with the given integer value.
		''' </summary>
		''' <param name="value">  Integer value.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if <CODE>value</CODE> is less than 1
		'''     or greater than 100. </exception>
		Public Sub New(ByVal value As Integer)
			MyBase.New(value, 1, 100)
		End Sub

		''' <summary>
		''' Returns whether this job priority attribute is equivalent to the passed
		''' in object. To be equivalent, all of the following conditions must be
		''' true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class JobPriority.
		''' <LI>
		''' This job priority attribute's value and <CODE>object</CODE>'s value
		''' are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this job
		'''          priority attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is JobPriority)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class JobPriority, the category is class JobPriority itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(JobPriority)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class JobPriority, the category name is <CODE>"job-priority"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "job-priority"
			End Get
		End Property

	End Class

End Namespace
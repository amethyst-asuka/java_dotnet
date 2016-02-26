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
	''' Class JobImpressions is an integer valued printing attribute class that
	''' specifies the total size in number of impressions of the document(s) being
	''' submitted. An "impression" is the image (possibly many print-stream pages in
	''' different configurations) imposed onto a single media page.
	''' <P>
	''' The JobImpressions attribute describes the size of the job. This attribute is
	''' not intended to be a counter; it is intended to be useful routing and
	''' scheduling information if known. The printer may try to compute the
	''' JobImpressions attribute's value if it is not supplied in the Print Request.
	''' Even if the client does supply a value for the JobImpressions attribute in
	''' the Print Request, the printer may choose to change the value if the printer
	''' is able to compute a value which is more accurate than the client supplied
	''' value. The printer may be able to determine the correct value for the
	''' JobImpressions attribute either right at job submission time or at any later
	''' point in time.
	''' <P>
	''' As with <seealso cref="JobKOctets JobKOctets"/>, the JobImpressions value must not
	''' include the multiplicative factors contributed by the number of copies
	''' specified by the <seealso cref="Copies Copies"/> attribute, independent of whether the
	''' device can process multiple copies without making multiple passes over the
	''' job or document data and independent of whether the output is collated or
	''' not. Thus the value is independent of the implementation and reflects the
	''' size of the document(s) measured in impressions independent of the number of
	''' copies.
	''' <P>
	''' As with <seealso cref="JobKOctets JobKOctets"/>, the JobImpressions value must also not
	''' include the multiplicative factor due to a copies instruction embedded in the
	''' document data. If the document data actually includes replications of the
	''' document data, this value will include such replication. In other words, this
	''' value is always the number of impressions in the source document data, rather
	''' than a measure of the number of impressions to be produced by the job.
	''' <P>
	''' <B>IPP Compatibility:</B> The integer value gives the IPP integer value. The
	''' category name returned by <CODE>getName()</CODE> gives the IPP attribute
	''' name.
	''' <P>
	''' </summary>
	''' <seealso cref= JobImpressionsSupported </seealso>
	''' <seealso cref= JobImpressionsCompleted </seealso>
	''' <seealso cref= JobKOctets </seealso>
	''' <seealso cref= JobMediaSheets
	''' 
	''' @author  Alan Kaminsky </seealso>
	Public NotInheritable Class JobImpressions
		Inherits javax.print.attribute.IntegerSyntax
		Implements javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = 8225537206784322464L


		''' <summary>
		''' Construct a new job impressions attribute with the given integer value.
		''' </summary>
		''' <param name="value">  Integer value.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''  (Unchecked exception) Thrown if <CODE>value</CODE> is less than 0. </exception>
		Public Sub New(ByVal value As Integer)
			MyBase.New(value, 0, Integer.MaxValue)
		End Sub

		''' <summary>
		''' Returns whether this job impressions attribute is equivalent to the
		''' passed in object. To be equivalent, all of the following conditions must
		''' be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class JobImpressions.
		''' <LI>
		''' This job impressions attribute's value and <CODE>object</CODE>'s value
		''' are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this job
		'''          impressions attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return MyBase.Equals([object]) AndAlso TypeOf [object] Is JobImpressions
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class JobImpressions, the category is class JobImpressions itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(JobImpressions)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class JobImpressions, the category name is
		''' <CODE>"job-impressions"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "job-impressions"
			End Get
		End Property

	End Class

End Namespace
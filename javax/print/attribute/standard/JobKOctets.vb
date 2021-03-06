Imports System

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
	''' Class JobKOctets is an integer valued printing attribute class that specifies
	''' the total size of the document(s) in K octets, i.e., in units of 1024 octets
	''' requested to be processed in the job. The value must be rounded up, so that a
	''' job between 1 and 1024 octets must be indicated as being 1K octets, 1025 to
	''' 2048 must be 2K octets, etc. For a multidoc print job (a job with multiple
	''' documents), the JobKOctets value is computed by adding up the individual
	''' documents' sizes in octets, then rounding up to the next K octets value.
	''' <P>
	''' The JobKOctets attribute describes the size of the job. This attribute is not
	''' intended to be a counter; it is intended to be useful routing and scheduling
	''' information if known. The printer may try to compute the JobKOctets
	''' attribute's value if it is not supplied in the Print Request. Even if the
	''' client does supply a value for the JobKOctets attribute in the Print Request,
	''' the printer may choose to change the value if the printer is able to compute
	''' a value which is more accurate than the client supplied value. The printer
	''' may be able to determine the correct value for the JobKOctets attribute
	''' either right at job submission time or at any later point in time.
	''' <P>
	''' The JobKOctets value must not include the multiplicative factors contributed
	''' by the number of copies specified by the <seealso cref="Copies Copies"/> attribute,
	''' independent of whether the device can process multiple copies without making
	''' multiple passes over the job or document data and independent of whether the
	''' output is collated or not. Thus the value is independent of the
	''' implementation and indicates the size of the document(s) measured in K octets
	''' independent of the number of copies.
	''' <P>
	''' The JobKOctets value must also not include the multiplicative factor due to a
	''' copies instruction embedded in the document data. If the document data
	''' actually includes replications of the document data, this value will include
	''' such replication. In other words, this value is always the size of the source
	''' document data, rather than a measure of the hardcopy output to be produced.
	''' <P>
	''' The size of a doc is computed based on the print data representation class as
	''' specified by the doc's <seealso cref="javax.print.DocFlavor DocFlavor"/>, as
	''' shown in the table below.
	''' <P>
	''' <TABLE BORDER=1 CELLPADDING=2 CELLSPACING=1 SUMMARY="Table showing computation of doc sizes">
	''' <TR>
	''' <TH>Representation Class</TH>
	''' <TH>Document Size</TH>
	''' </TR>
	''' <TR>
	''' <TD>byte[]</TD>
	''' <TD>Length of the byte array</TD>
	''' </TR>
	''' <TR>
	''' <TD>java.io.InputStream</TD>
	''' <TD>Number of bytes read from the stream</TD>
	''' </TR>
	''' <TR>
	''' <TD>char[]</TD>
	''' <TD>Length of the character array x 2</TD>
	''' </TR>
	''' <TR>
	''' <TD>java.lang.String</TD>
	''' <TD>Length of the string x 2</TD>
	''' </TR>
	''' <TR>
	''' <TD>java.io.Reader</TD>
	''' <TD>Number of characters read from the stream x 2</TD>
	''' </TR>
	''' <TR>
	''' <TD>java.net.URL</TD>
	''' <TD>Number of bytes read from the file at the given URL address</TD>
	''' </TR>
	''' <TR>
	''' <TD>java.awt.image.renderable.RenderableImage</TD>
	''' <TD>Implementation dependent&#42;</TD>
	''' </TR>
	''' <TR>
	''' <TD>java.awt.print.Printable</TD>
	''' <TD>Implementation dependent&#42;</TD>
	''' </TR>
	''' <TR>
	''' <TD>java.awt.print.Pageable</TD>
	''' <TD>Implementation dependent&#42;</TD>
	''' </TR>
	''' </TABLE>
	''' <P>
	''' &#42; In these cases the Print Service itself generates the print data sent
	''' to the printer. If the Print Service supports the JobKOctets attribute, for
	''' these cases the Print Service itself must calculate the size of the print
	''' data, replacing any JobKOctets value the client specified.
	''' <P>
	''' <B>IPP Compatibility:</B> The integer value gives the IPP integer value. The
	''' category name returned by <CODE>getName()</CODE> gives the IPP attribute
	''' name.
	''' <P>
	''' </summary>
	''' <seealso cref= JobKOctetsSupported </seealso>
	''' <seealso cref= JobKOctetsProcessed </seealso>
	''' <seealso cref= JobImpressions </seealso>
	''' <seealso cref= JobMediaSheets
	''' 
	''' @author  Alan Kaminsky </seealso>
	Public NotInheritable Class JobKOctets
		Inherits javax.print.attribute.IntegerSyntax
		Implements javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = -8959710146498202869L

		''' <summary>
		''' Construct a new job K octets attribute with the given integer value.
		''' </summary>
		''' <param name="value">  Integer value.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''  (Unchecked exception) Thrown if <CODE>value</CODE> is less than 0. </exception>
		Public Sub New(ByVal value As Integer)
			MyBase.New(value, 0, Integer.MaxValue)
		End Sub

		''' <summary>
		''' Returns whether this job K octets attribute is equivalent to the passed
		''' in object. To be equivalent, all of the following conditions must be
		''' true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class JobKOctets.
		''' <LI>
		''' This job K octets attribute's value and <CODE>object</CODE>'s value
		''' are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this job K
		'''          octets attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return MyBase.Equals([object]) AndAlso TypeOf [object] Is JobKOctets
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class JobKOctets, the category is class JobKOctets itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(JobKOctets)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class JobKOctets, the category name is <CODE>"job-k-octets"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "job-k-octets"
			End Get
		End Property

	End Class

End Namespace
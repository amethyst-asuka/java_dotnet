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
	''' Class JobPrioritySupported is an integer valued printing attribute class
	''' that specifies whether a Print Service instance supports the {@link
	''' JobPriority JobPriority} attribute and the number of different job priority
	''' levels supported.
	''' <P>
	''' The client can always specify any <seealso cref="JobPriority JobPriority"/> value
	''' from 1 to 100 for a job. However, the Print Service instance may support
	''' fewer than 100 different job priority levels. If this is the case, the
	''' Print Service instance automatically maps the client-specified job priority
	''' value to one of the supported job priority levels, dividing the 100 job
	''' priority values equally among the available job priority levels.
	''' <P>
	''' <B>IPP Compatibility:</B> The integer value gives the IPP integer value.
	''' The category name returned by <CODE>getName()</CODE> gives the IPP
	''' attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class JobPrioritySupported
		Inherits javax.print.attribute.IntegerSyntax
		Implements javax.print.attribute.SupportedValuesAttribute

		Private Const serialVersionUID As Long = 2564840378013555894L


		''' <summary>
		''' Construct a new job priority supported attribute with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Number of different job priority levels supported.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if <CODE>value</CODE> is less than 1
		'''     or greater than 100. </exception>
		Public Sub New(ByVal value As Integer)
			MyBase.New(value, 1, 100)
		End Sub

		''' <summary>
		''' Returns whether this job priority supported attribute is equivalent to
		''' the passed in object. To be equivalent, all of the following conditions
		''' must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class JobPrioritySupported.
		''' <LI>
		''' This job priority supported attribute's value and
		''' <CODE>object</CODE>'s value are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this job
		'''          priority supported attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean

			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is JobPrioritySupported)
		End Function


		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class JobPrioritySupported, the
		''' category is class JobPrioritySupported itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(JobPrioritySupported)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class JobPrioritySupported, the
		''' category name is <CODE>"job-priority-supported"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "job-priority-supported"
			End Get
		End Property

	End Class

End Namespace
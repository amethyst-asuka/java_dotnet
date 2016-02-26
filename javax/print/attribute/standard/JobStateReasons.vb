Imports System
Imports System.Collections.Generic

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
	''' Class JobStateReasons is a printing attribute class, a set of enumeration
	''' values, that provides additional information about the job's current state,
	''' i.e., information that augments the value of the job's {@link JobState
	''' JobState} attribute.
	''' <P>
	''' Instances of <seealso cref="JobStateReason JobStateReason"/> do not appear in a Print
	''' Job's attribute set directly. Rather, a JobStateReasons attribute appears in
	''' the Print Job's attribute set. The JobStateReasons attribute contains zero,
	''' one, or more than one <seealso cref="JobStateReason JobStateReason"/> objects which
	''' pertain to the Print Job's status. The printer adds a {@link JobStateReason
	''' JobStateReason} object to the Print Job's JobStateReasons attribute when the
	''' corresponding condition becomes true of the Print Job, and the printer
	''' removes the <seealso cref="JobStateReason JobStateReason"/> object again when the
	''' corresponding condition becomes false, regardless of whether the Print Job's
	''' overall <seealso cref="JobState JobState"/> also changed.
	''' <P>
	''' Class JobStateReasons inherits its implementation from class {@link
	''' java.util.HashSet java.util.HashSet}. Unlike most printing attributes which
	''' are immutable once constructed, class JobStateReasons is designed to be
	''' mutable; you can add <seealso cref="JobStateReason JobStateReason"/> objects to an
	''' existing JobStateReasons object and remove them again. However, like class
	''' <seealso cref="java.util.HashSet java.util.HashSet"/>, class JobStateReasons is not
	''' multiple thread safe. If a JobStateReasons object will be used by multiple
	''' threads, be sure to synchronize its operations (e.g., using a synchronized
	''' set view obtained from class {@link java.util.Collections
	''' java.util.Collections}).
	''' <P>
	''' <B>IPP Compatibility:</B> The string value returned by each individual {@link
	''' JobStateReason JobStateReason} object's <CODE>toString()</CODE> method gives
	''' the IPP keyword value. The category name returned by <CODE>getName()</CODE>
	''' gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class JobStateReasons
		Inherits HashSet(Of JobStateReason)
		Implements javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = 8849088261264331812L

		''' <summary>
		''' Construct a new, empty job state reasons attribute; the underlying hash
		''' set has the default initial capacity and load factor.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Construct a new, empty job state reasons attribute; the underlying hash
		''' set has the given initial capacity and the default load factor.
		''' </summary>
		''' <param name="initialCapacity">  Initial capacity. </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity is less
		'''     than zero. </exception>
		Public Sub New(ByVal initialCapacity As Integer)
			MyBase.New(initialCapacity)
		End Sub

		''' <summary>
		''' Construct a new, empty job state reasons attribute; the underlying hash
		''' set has the given initial capacity and load factor.
		''' </summary>
		''' <param name="initialCapacity">  Initial capacity. </param>
		''' <param name="loadFactor">       Load factor. </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity is less
		'''     than zero. </exception>
		Public Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single)
			MyBase.New(initialCapacity, loadFactor)
		End Sub

		''' <summary>
		''' Construct a new job state reasons attribute that contains the same
		''' <seealso cref="JobStateReason JobStateReason"/> objects as the given collection.
		''' The underlying hash set's initial capacity and load factor are as
		''' specified in the superclass constructor {@link
		''' java.util.HashSet#HashSet(java.util.Collection)
		''' HashSet(Collection)}.
		''' </summary>
		''' <param name="collection">  Collection to copy.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>collection</CODE> is null or
		'''     if any element in <CODE>collection</CODE> is null. </exception>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if any element in
		'''     <CODE>collection</CODE> is not an instance of class {@link
		'''     JobStateReason JobStateReason}. </exception>
	   Public Sub New(ByVal collection As ICollection(Of JobStateReason))
		   MyBase.New(collection)
	   End Sub

		''' <summary>
		''' Adds the specified element to this job state reasons attribute if it is
		''' not already present. The element to be added must be an instance of class
		''' <seealso cref="JobStateReason JobStateReason"/>. If this job state reasons
		''' attribute already contains the specified element, the call leaves this
		''' job state reasons attribute unchanged and returns <tt>false</tt>.
		''' </summary>
		''' <param name="o">  Element to be added to this job state reasons attribute.
		''' </param>
		''' <returns>  <tt>true</tt> if this job state reasons attribute did not
		'''          already contain the specified element.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if the specified element is null. </exception>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if the specified element is not an
		'''     instance of class <seealso cref="JobStateReason JobStateReason"/>.
		''' @since 1.5 </exception>
		Public Function add(ByVal o As JobStateReason) As Boolean
			If o Is Nothing Then Throw New NullPointerException
			Return MyBase.add(CType(o, JobStateReason))
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class JobStateReasons, the category is class JobStateReasons itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(JobStateReasons)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class JobStateReasons, the category
		''' name is <CODE>"job-state-reasons"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "job-state-reasons"
			End Get
		End Property

	End Class

End Namespace
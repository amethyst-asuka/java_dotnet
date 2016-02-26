Imports System
Imports System.Collections
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
	''' Class PrinterStateReasons is a printing attribute class, a set of
	''' enumeration values, that provides additional information about the
	''' printer's current state, i.e., information that augments the value of the
	''' printer's <seealso cref="PrinterState PrinterState"/> attribute.
	''' <P>
	''' Instances of <seealso cref="PrinterStateReason PrinterStateReason"/> do not appear in
	'''  a Print Service's attribute set directly. Rather, a PrinterStateReasons
	''' attribute appears in the Print Service's attribute set. The
	''' PrinterStateReasons attribute contains zero, one, or more than one {@link
	''' PrinterStateReason PrinterStateReason} objects which pertain to the Print
	''' Service's status, and each <seealso cref="PrinterStateReason PrinterStateReason"/>
	''' object is associated with a <seealso cref="Severity Severity"/> level of REPORT
	'''  (least severe), WARNING, or ERROR (most severe). The printer adds a {@link
	''' PrinterStateReason PrinterStateReason} object to the Print Service's
	''' PrinterStateReasons attribute when the corresponding condition becomes true
	''' of the printer, and the printer removes the {@link PrinterStateReason
	''' PrinterStateReason} object again when the corresponding condition becomes
	''' false, regardless of whether the Print Service's overall
	''' <seealso cref="PrinterState PrinterState"/> also changed.
	''' <P>
	''' Class PrinterStateReasons inherits its implementation from class {@link
	''' java.util.HashMap java.util.HashMap}. Each entry in the map consists of a
	''' <seealso cref="PrinterStateReason PrinterStateReason"/> object (key) mapping to a
	''' <seealso cref="Severity Severity"/> object (value):
	''' <P>
	''' Unlike most printing attributes which are immutable once constructed, class
	''' PrinterStateReasons is designed to be mutable; you can add {@link
	''' PrinterStateReason PrinterStateReason} objects to an existing
	''' PrinterStateReasons object and remove them again. However, like class
	'''  <seealso cref="java.util.HashMap java.util.HashMap"/>, class PrinterStateReasons is
	''' not multiple thread safe. If a PrinterStateReasons object will be used by
	''' multiple threads, be sure to synchronize its operations (e.g., using a
	''' synchronized map view obtained from class {@link java.util.Collections
	''' java.util.Collections}).
	''' <P>
	''' <B>IPP Compatibility:</B> The string values returned by each individual
	''' <seealso cref="PrinterStateReason PrinterStateReason"/> object's and the associated
	''' <seealso cref="Severity Severity"/> object's <CODE>toString()</CODE> methods,
	''' concatenated
	''' together with a hyphen (<CODE>"-"</CODE>) in between, gives the IPP keyword
	''' value. The category name returned by <CODE>getName()</CODE> gives the IPP
	''' attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class PrinterStateReasons
		Inherits Dictionary(Of PrinterStateReason, Severity)
		Implements javax.print.attribute.PrintServiceAttribute

		Private Const serialVersionUID As Long = -3731791085163619457L

		''' <summary>
		''' Construct a new, empty printer state reasons attribute; the underlying
		''' hash map has the default initial capacity and load factor.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' super a new, empty printer state reasons attribute; the underlying
		''' hash map has the given initial capacity and the default load factor.
		''' </summary>
		''' <param name="initialCapacity">  Initial capacity.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity is less
		'''     than zero. </exception>
		Public Sub New(ByVal initialCapacity As Integer)
			MyBase.New(initialCapacity)
		End Sub

		''' <summary>
		''' Construct a new, empty printer state reasons attribute; the underlying
		''' hash map has the given initial capacity and load factor.
		''' </summary>
		''' <param name="initialCapacity">  Initial capacity. </param>
		''' <param name="loadFactor">       Load factor.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity is less
		'''     than zero. </exception>
		Public Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single)
			MyBase.New(initialCapacity, loadFactor)
		End Sub

		''' <summary>
		''' Construct a new printer state reasons attribute that contains the same
		''' <seealso cref="PrinterStateReason PrinterStateReason"/>-to-{@link Severity
		''' Severity} mappings as the given map. The underlying hash map's initial
		''' capacity and load factor are as specified in the superclass constructor
		''' {@link java.util.HashMap#HashMap(java.util.Map)
		''' HashMap(Map)}.
		''' </summary>
		''' <param name="map">  Map to copy.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>map</CODE> is null or if any
		'''     key or value in <CODE>map</CODE> is null. </exception>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if any key in <CODE>map</CODE> is not
		'''   an instance of class <seealso cref="PrinterStateReason PrinterStateReason"/> or
		'''     if any value in <CODE>map</CODE> is not an instance of class
		'''     <seealso cref="Severity Severity"/>. </exception>
		Public Sub New(ByVal map As IDictionary(Of PrinterStateReason, Severity))
			Me.New()
			For Each e As KeyValuePair(Of PrinterStateReason, Severity) In map
				put(e.Key, e.Value)
			Next e
		End Sub

		''' <summary>
		''' Adds the given printer state reason to this printer state reasons
		''' attribute, associating it with the given severity level. If this
		''' printer state reasons attribute previously contained a mapping for the
		''' given printer state reason, the old value is replaced.
		''' </summary>
		''' <param name="reason">    Printer state reason. This must be an instance of
		'''                    class <seealso cref="PrinterStateReason PrinterStateReason"/>. </param>
		''' <param name="severity">  Severity of the printer state reason. This must be
		'''                      an instance of class <seealso cref="Severity Severity"/>.
		''' </param>
		''' <returns>  Previous severity associated with the given printer state
		'''          reason, or <tt>null</tt> if the given printer state reason was
		'''          not present.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>reason</CODE> is null or
		'''     <CODE>severity</CODE> is null. </exception>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if <CODE>reason</CODE> is not an
		'''   instance of class <seealso cref="PrinterStateReason PrinterStateReason"/> or if
		'''     <CODE>severity</CODE> is not an instance of class {@link Severity
		'''     Severity}.
		''' @since 1.5 </exception>
		Public Function put(ByVal reason As PrinterStateReason, ByVal ___severity As Severity) As Severity
			If reason Is Nothing Then Throw New NullPointerException("reason is null")
			If ___severity Is Nothing Then Throw New NullPointerException("severity is null")
			Return MyBase.put(reason, ___severity)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class PrinterStateReasons, the
		''' category is class PrinterStateReasons itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PrinterStateReasons)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PrinterStateReasons, the
		''' category name is <CODE>"printer-state-reasons"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "printer-state-reasons"
			End Get
		End Property

		''' <summary>
		''' Obtain an unmodifiable set view of the individual printer state reason
		''' attributes at the given severity level in this PrinterStateReasons
		''' attribute. Each element in the set view is a {@link PrinterStateReason
		''' PrinterStateReason} object. The only elements in the set view are the
		''' <seealso cref="PrinterStateReason PrinterStateReason"/> objects that map to the
		''' given severity value. The set view is backed by this
		''' PrinterStateReasons attribute, so changes to this PrinterStateReasons
		''' attribute are reflected  in the set view.
		''' The set view does not support element insertion or
		''' removal. The set view's iterator does not support element removal.
		''' </summary>
		''' <param name="severity">  Severity level.
		''' </param>
		''' <returns>  Set view of the individual {@link PrinterStateReason
		'''          PrinterStateReason} attributes at the given {@link Severity
		'''          Severity} level.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>severity</CODE> is null. </exception>
		Public Function printerStateReasonSet(ByVal ___severity As Severity) As java.util.Set(Of PrinterStateReason)
			If ___severity Is Nothing Then Throw New NullPointerException("severity is null")
			Return New PrinterStateReasonSet(Me, ___severity, entrySet())
		End Function

		Private Class PrinterStateReasonSet
			Inherits java.util.AbstractSet(Of PrinterStateReason)

			Private ReadOnly outerInstance As PrinterStateReasons

			Private mySeverity As Severity
			Private myEntrySet As java.util.Set

			Public Sub New(ByVal outerInstance As PrinterStateReasons, ByVal ___severity As Severity, ByVal entrySet As java.util.Set)
					Me.outerInstance = outerInstance
				mySeverity = ___severity
				myEntrySet = entrySet
			End Sub

			Public Overridable Function size() As Integer
				Dim result As Integer = 0
				Dim iter As IEnumerator = [iterator]()
				Do While iter.hasNext()
					iter.next()
					result += 1
				Loop
				Return result
			End Function

			Public Overridable Function [iterator]() As IEnumerator
				Return New PrinterStateReasonSetIterator(mySeverity, myEntrySet.GetEnumerator())
			End Function
		End Class

		Private Class PrinterStateReasonSetIterator
			Implements IEnumerator

			Private ReadOnly outerInstance As PrinterStateReasons

			Private mySeverity As Severity
			Private myIterator As IEnumerator
			Private myEntry As DictionaryEntry

			Public Sub New(ByVal outerInstance As PrinterStateReasons, ByVal ___severity As Severity, ByVal [iterator] As IEnumerator)
					Me.outerInstance = outerInstance
				mySeverity = ___severity
				myIterator = [iterator]
				goToNext()
			End Sub

			Private Sub goToNext()
				myEntry = Nothing
				Do While myEntry Is Nothing AndAlso myIterator.hasNext()
					myEntry = CType(myIterator.next(), DictionaryEntry)
					If CType(myEntry.Value, Severity) IsNot mySeverity Then myEntry = Nothing
				Loop
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return myEntry IsNot Nothing
			End Function

			Public Overridable Function [next]() As Object
				If myEntry Is Nothing Then Throw New java.util.NoSuchElementException
				Dim result As Object = myEntry.Key
				goToNext()
				Return result
			End Function

			Public Overridable Sub remove()
				Throw New System.NotSupportedException
			End Sub
		End Class

	End Class

End Namespace
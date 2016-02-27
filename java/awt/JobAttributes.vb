'
' * Copyright (c) 1999, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt

	''' <summary>
	''' A set of attributes which control a print job.
	''' <p>
	''' Instances of this class control the number of copies, default selection,
	''' destination, print dialog, file and printer names, page ranges, multiple
	''' document handling (including collation), and multi-page imposition (such
	''' as duplex) of every print job which uses the instance. Attribute names are
	''' compliant with the Internet Printing Protocol (IPP) 1.1 where possible.
	''' Attribute values are partially compliant where possible.
	''' <p>
	''' To use a method which takes an inner class type, pass a reference to
	''' one of the constant fields of the inner class. Client code cannot create
	''' new instances of the inner class types because none of those classes
	''' has a public constructor. For example, to set the print dialog type to
	''' the cross-platform, pure Java print dialog, use the following code:
	''' <pre>
	''' import java.awt.JobAttributes;
	''' 
	''' public class PureJavaPrintDialogExample {
	'''     public void setPureJavaPrintDialog(JobAttributes jobAttributes) {
	'''         jobAttributes.setDialog(JobAttributes.DialogType.COMMON);
	'''     }
	''' }
	''' </pre>
	''' <p>
	''' Every IPP attribute which supports an <i>attributeName</i>-default value
	''' has a corresponding <code>set<i>attributeName</i>ToDefault</code> method.
	''' Default value fields are not provided.
	''' 
	''' @author      David Mendenhall
	''' @since 1.3
	''' </summary>
	Public NotInheritable Class JobAttributes
		Implements Cloneable

		''' <summary>
		''' A type-safe enumeration of possible default selection states.
		''' @since 1.3
		''' </summary>
		Public NotInheritable Class DefaultSelectionType
			Inherits AttributeValue

			Private Const I_ALL As Integer = 0
			Private Const I_RANGE As Integer = 1
			Private Const I_SELECTION As Integer = 2

			Private Shared ReadOnly NAMES As String() = { "all", "range", "selection" }

			''' <summary>
			''' The <code>DefaultSelectionType</code> instance to use for
			''' specifying that all pages of the job should be printed.
			''' </summary>
			Public Shared ReadOnly ALL As New DefaultSelectionType(I_ALL)
			''' <summary>
			''' The <code>DefaultSelectionType</code> instance to use for
			''' specifying that a range of pages of the job should be printed.
			''' </summary>
			Public Shared ReadOnly RANGE As New DefaultSelectionType(I_RANGE)
			''' <summary>
			''' The <code>DefaultSelectionType</code> instance to use for
			''' specifying that the current selection should be printed.
			''' </summary>
			Public Shared ReadOnly SELECTION As New DefaultSelectionType(I_SELECTION)

			Private Sub New(ByVal type As Integer)
				MyBase.New(type, NAMES)
			End Sub
		End Class

		''' <summary>
		''' A type-safe enumeration of possible job destinations.
		''' @since 1.3
		''' </summary>
		Public NotInheritable Class DestinationType
			Inherits AttributeValue

			Private Const I_FILE As Integer = 0
			Private Const I_PRINTER As Integer = 1

			Private Shared ReadOnly NAMES As String() = { "file", "printer" }

			''' <summary>
			''' The <code>DestinationType</code> instance to use for
			''' specifying print to file.
			''' </summary>
			Public Shared ReadOnly FILE As New DestinationType(I_FILE)
			''' <summary>
			''' The <code>DestinationType</code> instance to use for
			''' specifying print to printer.
			''' </summary>
			Public Shared ReadOnly PRINTER As New DestinationType(I_PRINTER)

			Private Sub New(ByVal type As Integer)
				MyBase.New(type, NAMES)
			End Sub
		End Class

		''' <summary>
		''' A type-safe enumeration of possible dialogs to display to the user.
		''' @since 1.3
		''' </summary>
		Public NotInheritable Class DialogType
			Inherits AttributeValue

			Private Const I_COMMON As Integer = 0
			Private Const I_NATIVE As Integer = 1
			Private Const I_NONE As Integer = 2

			Private Shared ReadOnly NAMES As String() = { "common", "native", "none" }

			''' <summary>
			''' The <code>DialogType</code> instance to use for
			''' specifying the cross-platform, pure Java print dialog.
			''' </summary>
			Public Shared ReadOnly COMMON As New DialogType(I_COMMON)
			''' <summary>
			''' The <code>DialogType</code> instance to use for
			''' specifying the platform's native print dialog.
			''' </summary>
			Public Shared ReadOnly NATIVE As New DialogType(I_NATIVE)
			''' <summary>
			''' The <code>DialogType</code> instance to use for
			''' specifying no print dialog.
			''' </summary>
			Public Shared ReadOnly NONE As New DialogType(I_NONE)

			Private Sub New(ByVal type As Integer)
				MyBase.New(type, NAMES)
			End Sub
		End Class

		''' <summary>
		''' A type-safe enumeration of possible multiple copy handling states.
		''' It is used to control how the sheets of multiple copies of a single
		''' document are collated.
		''' @since 1.3
		''' </summary>
		Public NotInheritable Class MultipleDocumentHandlingType
			Inherits AttributeValue

			Private Const I_SEPARATE_DOCUMENTS_COLLATED_COPIES As Integer = 0
			Private Const I_SEPARATE_DOCUMENTS_UNCOLLATED_COPIES As Integer = 1

			Private Shared ReadOnly NAMES As String() = { "separate-documents-collated-copies", "separate-documents-uncollated-copies" }

			''' <summary>
			''' The <code>MultipleDocumentHandlingType</code> instance to use for specifying
			''' that the job should be divided into separate, collated copies.
			''' </summary>
			Public Shared ReadOnly SEPARATE_DOCUMENTS_COLLATED_COPIES As New MultipleDocumentHandlingType(I_SEPARATE_DOCUMENTS_COLLATED_COPIES)
			''' <summary>
			''' The <code>MultipleDocumentHandlingType</code> instance to use for specifying
			''' that the job should be divided into separate, uncollated copies.
			''' </summary>
			Public Shared ReadOnly SEPARATE_DOCUMENTS_UNCOLLATED_COPIES As New MultipleDocumentHandlingType(I_SEPARATE_DOCUMENTS_UNCOLLATED_COPIES)

			Private Sub New(ByVal type As Integer)
				MyBase.New(type, NAMES)
			End Sub
		End Class

		''' <summary>
		''' A type-safe enumeration of possible multi-page impositions. These
		''' impositions are in compliance with IPP 1.1.
		''' @since 1.3
		''' </summary>
		Public NotInheritable Class SidesType
			Inherits AttributeValue

			Private Const I_ONE_SIDED As Integer = 0
			Private Const I_TWO_SIDED_LONG_EDGE As Integer = 1
			Private Const I_TWO_SIDED_SHORT_EDGE As Integer = 2

			Private Shared ReadOnly NAMES As String() = { "one-sided", "two-sided-long-edge", "two-sided-short-edge" }

			''' <summary>
			''' The <code>SidesType</code> instance to use for specifying that
			''' consecutive job pages should be printed upon the same side of
			''' consecutive media sheets.
			''' </summary>
			Public Shared ReadOnly ONE_SIDED As New SidesType(I_ONE_SIDED)
			''' <summary>
			''' The <code>SidesType</code> instance to use for specifying that
			''' consecutive job pages should be printed upon front and back sides
			''' of consecutive media sheets, such that the orientation of each pair
			''' of pages on the medium would be correct for the reader as if for
			''' binding on the long edge.
			''' </summary>
			Public Shared ReadOnly TWO_SIDED_LONG_EDGE As New SidesType(I_TWO_SIDED_LONG_EDGE)
			''' <summary>
			''' The <code>SidesType</code> instance to use for specifying that
			''' consecutive job pages should be printed upon front and back sides
			''' of consecutive media sheets, such that the orientation of each pair
			''' of pages on the medium would be correct for the reader as if for
			''' binding on the short edge.
			''' </summary>
			Public Shared ReadOnly TWO_SIDED_SHORT_EDGE As New SidesType(I_TWO_SIDED_SHORT_EDGE)

			Private Sub New(ByVal type As Integer)
				MyBase.New(type, NAMES)
			End Sub
		End Class

		Private copies As Integer
		Private defaultSelection As DefaultSelectionType
		Private destination As DestinationType
		Private dialog_Renamed As DialogType
		Private fileName As String
		Private fromPage As Integer
		Private maxPage As Integer
		Private minPage As Integer
		Private multipleDocumentHandling As MultipleDocumentHandlingType
		Private pageRanges As Integer()()
		Private prFirst As Integer
		Private prLast As Integer
		Private printer As String
		Private sides As SidesType
		Private toPage As Integer

		''' <summary>
		''' Constructs a <code>JobAttributes</code> instance with default
		''' values for every attribute.  The dialog defaults to
		''' <code>DialogType.NATIVE</code>.  Min page defaults to
		''' <code>1</code>.  Max page defaults to <code> java.lang.[Integer].MAX_VALUE</code>.
		''' Destination defaults to <code>DestinationType.PRINTER</code>.
		''' Selection defaults to <code>DefaultSelectionType.ALL</code>.
		''' Number of copies defaults to <code>1</code>. Multiple document handling defaults
		''' to <code>MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_UNCOLLATED_COPIES</code>.
		''' Sides defaults to <code>SidesType.ONE_SIDED</code>. File name defaults
		''' to <code>null</code>.
		''' </summary>
		Public Sub New()
			copiesToDefaultult()
			defaultSelection = DefaultSelectionType.ALL
			destination = DestinationType.PRINTER
			dialog = DialogType.NATIVE
			maxPage =  java.lang.[Integer].Max_Value
			minPage = 1
			multipleDocumentHandlingToDefaultult()
			sidesToDefaultult()
		End Sub

		''' <summary>
		''' Constructs a <code>JobAttributes</code> instance which is a copy
		''' of the supplied <code>JobAttributes</code>.
		''' </summary>
		''' <param name="obj"> the <code>JobAttributes</code> to copy </param>
		Public Sub New(ByVal obj As JobAttributes)
			[set](obj)
		End Sub

		''' <summary>
		''' Constructs a <code>JobAttributes</code> instance with the
		''' specified values for every attribute.
		''' </summary>
		''' <param name="copies"> an integer greater than 0 </param>
		''' <param name="defaultSelection"> <code>DefaultSelectionType.ALL</code>,
		'''          <code>DefaultSelectionType.RANGE</code>, or
		'''          <code>DefaultSelectionType.SELECTION</code> </param>
		''' <param name="destination"> <code>DesintationType.FILE</code> or
		'''          <code>DesintationType.PRINTER</code> </param>
		''' <param name="dialog"> <code>DialogType.COMMON</code>,
		'''          <code>DialogType.NATIVE</code>, or
		'''          <code>DialogType.NONE</code> </param>
		''' <param name="fileName"> the possibly <code>null</code> file name </param>
		''' <param name="maxPage"> an integer greater than zero and greater than or equal
		'''          to <i>minPage</i> </param>
		''' <param name="minPage"> an integer greater than zero and less than or equal
		'''          to <i>maxPage</i> </param>
		''' <param name="multipleDocumentHandling">
		'''     <code>MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_COLLATED_COPIES</code> or
		'''     <code>MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_UNCOLLATED_COPIES</code> </param>
		''' <param name="pageRanges"> an array of integer arrays of two elements; an array
		'''          is interpreted as a range spanning all pages including and
		'''          between the specified pages; ranges must be in ascending
		'''          order and must not overlap; specified page numbers cannot be
		'''          less than <i>minPage</i> nor greater than <i>maxPage</i>;
		'''          for example:
		'''          <pre>
		'''          (new int[][] { new int[] { 1, 3 }, new int[] { 5, 5 },
		'''                         new int[] { 15, 19 } }),
		'''          </pre>
		'''          specifies pages 1, 2, 3, 5, 15, 16, 17, 18, and 19. Note that
		'''          (<code>new int[][] { new int[] { 1, 1 }, new int[] { 1, 2 } }</code>),
		'''          is an invalid set of page ranges because the two ranges
		'''          overlap </param>
		''' <param name="printer"> the possibly <code>null</code> printer name </param>
		''' <param name="sides"> <code>SidesType.ONE_SIDED</code>,
		'''          <code>SidesType.TWO_SIDED_LONG_EDGE</code>, or
		'''          <code>SidesType.TWO_SIDED_SHORT_EDGE</code> </param>
		''' <exception cref="IllegalArgumentException"> if one or more of the above
		'''          conditions is violated </exception>
		Public Sub New(ByVal copies As Integer, ByVal defaultSelection As DefaultSelectionType, ByVal destination As DestinationType, ByVal dialog_Renamed As DialogType, ByVal fileName As String, ByVal maxPage As Integer, ByVal minPage As Integer, ByVal multipleDocumentHandling As MultipleDocumentHandlingType, ByVal pageRanges As Integer()(), ByVal printer As String, ByVal sides As SidesType)
			copies = copies
			defaultSelection = defaultSelection
			destination = destination
			dialog = dialog_Renamed
			fileName = fileName
			maxPage = maxPage
			minPage = minPage
			multipleDocumentHandling = multipleDocumentHandling
			pageRanges = pageRanges
			printer = printer
			sides = sides
		End Sub

		''' <summary>
		''' Creates and returns a copy of this <code>JobAttributes</code>.
		''' </summary>
		''' <returns>  the newly created copy; it is safe to cast this Object into
		'''          a <code>JobAttributes</code> </returns>
		Public Function clone() As Object
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' Since we implement Cloneable, this should never happen
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Sets all of the attributes of this <code>JobAttributes</code> to
		''' the same values as the attributes of obj.
		''' </summary>
		''' <param name="obj"> the <code>JobAttributes</code> to copy </param>
		Public Sub [set](ByVal obj As JobAttributes)
			copies = obj.copies
			defaultSelection = obj.defaultSelection
			destination = obj.destination
			dialog_Renamed = obj.dialog_Renamed
			fileName = obj.fileName
			fromPage = obj.fromPage
			maxPage = obj.maxPage
			minPage = obj.minPage
			multipleDocumentHandling = obj.multipleDocumentHandling
			' okay because we never modify the contents of pageRanges
			pageRanges = obj.pageRanges
			prFirst = obj.prFirst
			prLast = obj.prLast
			printer = obj.printer
			sides = obj.sides
			toPage = obj.toPage
		End Sub

		''' <summary>
		''' Returns the number of copies the application should render for jobs
		''' using these attributes. This attribute is updated to the value chosen
		''' by the user.
		''' </summary>
		''' <returns>  an integer greater than 0. </returns>
		Public Property copies As Integer
			Get
				Return copies
			End Get
			Set(ByVal copies As Integer)
				If copies <= 0 Then Throw New IllegalArgumentException("Invalid value for attribute " & "copies")
				Me.copies = copies
			End Set
		End Property


		''' <summary>
		''' Sets the number of copies the application should render for jobs using
		''' these attributes to the default. The default number of copies is 1.
		''' </summary>
		Public Sub setCopiesToDefault()
			copies = 1
		End Sub

		''' <summary>
		''' Specifies whether, for jobs using these attributes, the application
		''' should print all pages, the range specified by the return value of
		''' <code>getPageRanges</code>, or the current selection. This attribute
		''' is updated to the value chosen by the user.
		''' </summary>
		''' <returns>  DefaultSelectionType.ALL, DefaultSelectionType.RANGE, or
		'''          DefaultSelectionType.SELECTION </returns>
		Public Property defaultSelection As DefaultSelectionType
			Get
				Return defaultSelection
			End Get
			Set(ByVal defaultSelection As DefaultSelectionType)
				If defaultSelection Is Nothing Then Throw New IllegalArgumentException("Invalid value for attribute " & "defaultSelection")
				Me.defaultSelection = defaultSelection
			End Set
		End Property


		''' <summary>
		''' Specifies whether output will be to a printer or a file for jobs using
		''' these attributes. This attribute is updated to the value chosen by the
		''' user.
		''' </summary>
		''' <returns>  DesintationType.FILE or DesintationType.PRINTER </returns>
		Public Property destination As DestinationType
			Get
				Return destination
			End Get
			Set(ByVal destination As DestinationType)
				If destination Is Nothing Then Throw New IllegalArgumentException("Invalid value for attribute " & "destination")
				Me.destination = destination
			End Set
		End Property


		''' <summary>
		''' Returns whether, for jobs using these attributes, the user should see
		''' a print dialog in which to modify the print settings, and which type of
		''' print dialog should be displayed. DialogType.COMMON denotes a cross-
		''' platform, pure Java print dialog. DialogType.NATIVE denotes the
		''' platform's native print dialog. If a platform does not support a native
		''' print dialog, the pure Java print dialog is displayed instead.
		''' DialogType.NONE specifies no print dialog (i.e., background printing).
		''' This attribute cannot be modified by, and is not subject to any
		''' limitations of, the implementation or the target printer.
		''' </summary>
		''' <returns>  <code>DialogType.COMMON</code>, <code>DialogType.NATIVE</code>, or
		'''          <code>DialogType.NONE</code> </returns>
		Public Property dialog As DialogType
			Get
				Return dialog_Renamed
			End Get
			Set(ByVal dialog_Renamed As DialogType)
				If dialog_Renamed Is Nothing Then Throw New IllegalArgumentException("Invalid value for attribute " & "dialog")
				Me.dialog_Renamed = dialog_Renamed
			End Set
		End Property


		''' <summary>
		''' Specifies the file name for the output file for jobs using these
		''' attributes. This attribute is updated to the value chosen by the user.
		''' </summary>
		''' <returns>  the possibly <code>null</code> file name </returns>
		Public Property fileName As String
			Get
				Return fileName
			End Get
			Set(ByVal fileName As String)
				Me.fileName = fileName
			End Set
		End Property


		''' <summary>
		''' Returns, for jobs using these attributes, the first page to be
		''' printed, if a range of pages is to be printed. This attribute is
		''' updated to the value chosen by the user. An application should ignore
		''' this attribute on output, unless the return value of the <code>
		''' getDefaultSelection</code> method is DefaultSelectionType.RANGE. An
		''' application should honor the return value of <code>getPageRanges</code>
		''' over the return value of this method, if possible.
		''' </summary>
		''' <returns>  an integer greater than zero and less than or equal to
		'''          <i>toPage</i> and greater than or equal to <i>minPage</i> and
		'''          less than or equal to <i>maxPage</i>. </returns>
		Public Property fromPage As Integer
			Get
				If fromPage <> 0 Then
					Return fromPage
				ElseIf toPage <> 0 Then
					Return minPage
				ElseIf pageRanges IsNot Nothing Then
					Return prFirst
				Else
					Return minPage
				End If
			End Get
			Set(ByVal fromPage As Integer)
				If fromPage <= 0 OrElse (toPage <> 0 AndAlso fromPage > toPage) OrElse fromPage < minPage OrElse fromPage > maxPage Then Throw New IllegalArgumentException("Invalid value for attribute " & "fromPage")
				Me.fromPage = fromPage
			End Set
		End Property


		''' <summary>
		''' Specifies the maximum value the user can specify as the last page to
		''' be printed for jobs using these attributes. This attribute cannot be
		''' modified by, and is not subject to any limitations of, the
		''' implementation or the target printer.
		''' </summary>
		''' <returns>  an integer greater than zero and greater than or equal
		'''          to <i>minPage</i>. </returns>
		Public Property maxPage As Integer
			Get
				Return maxPage
			End Get
			Set(ByVal maxPage As Integer)
				If maxPage <= 0 OrElse maxPage < minPage Then Throw New IllegalArgumentException("Invalid value for attribute " & "maxPage")
				Me.maxPage = maxPage
			End Set
		End Property


		''' <summary>
		''' Specifies the minimum value the user can specify as the first page to
		''' be printed for jobs using these attributes. This attribute cannot be
		''' modified by, and is not subject to any limitations of, the
		''' implementation or the target printer.
		''' </summary>
		''' <returns>  an integer greater than zero and less than or equal
		'''          to <i>maxPage</i>. </returns>
		Public Property minPage As Integer
			Get
				Return minPage
			End Get
			Set(ByVal minPage As Integer)
				If minPage <= 0 OrElse minPage > maxPage Then Throw New IllegalArgumentException("Invalid value for attribute " & "minPage")
				Me.minPage = minPage
			End Set
		End Property


		''' <summary>
		''' Specifies the handling of multiple copies, including collation, for
		''' jobs using these attributes. This attribute is updated to the value
		''' chosen by the user.
		''' 
		''' @return
		'''     MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_COLLATED_COPIES or
		'''     MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_UNCOLLATED_COPIES.
		''' </summary>
		Public Property multipleDocumentHandling As MultipleDocumentHandlingType
			Get
				Return multipleDocumentHandling
			End Get
			Set(ByVal multipleDocumentHandling As MultipleDocumentHandlingType)
				If multipleDocumentHandling Is Nothing Then Throw New IllegalArgumentException("Invalid value for attribute " & "multipleDocumentHandling")
				Me.multipleDocumentHandling = multipleDocumentHandling
			End Set
		End Property


		''' <summary>
		''' Sets the handling of multiple copies, including collation, for jobs
		''' using these attributes to the default. The default handling is
		''' MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_UNCOLLATED_COPIES.
		''' </summary>
		Public Sub setMultipleDocumentHandlingToDefault()
			multipleDocumentHandling = MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_UNCOLLATED_COPIES
		End Sub

		''' <summary>
		''' Specifies, for jobs using these attributes, the ranges of pages to be
		''' printed, if a range of pages is to be printed. All range numbers are
		''' inclusive. This attribute is updated to the value chosen by the user.
		''' An application should ignore this attribute on output, unless the
		''' return value of the <code>getDefaultSelection</code> method is
		''' DefaultSelectionType.RANGE.
		''' </summary>
		''' <returns>  an array of integer arrays of 2 elements. An array
		'''          is interpreted as a range spanning all pages including and
		'''          between the specified pages. Ranges must be in ascending
		'''          order and must not overlap. Specified page numbers cannot be
		'''          less than <i>minPage</i> nor greater than <i>maxPage</i>.
		'''          For example:
		'''          (new int[][] { new int[] { 1, 3 }, new int[] { 5, 5 },
		'''                         new int[] { 15, 19 } }),
		'''          specifies pages 1, 2, 3, 5, 15, 16, 17, 18, and 19. </returns>
		Public Property pageRanges As Integer()()
			Get
				If pageRanges IsNot Nothing Then
					' Return a copy because otherwise client code could circumvent the
					' the checks made in setPageRanges by modifying the returned
					' array.
					Dim copy As Integer()() = RectangularArrays.ReturnRectangularIntegerArray(pageRanges.Length, 2)
					For i As Integer = 0 To pageRanges.Length - 1
						copy(i)(0) = pageRanges(i)(0)
						copy(i)(1) = pageRanges(i)(1)
					Next i
					Return copy
				ElseIf fromPage <> 0 OrElse toPage <> 0 Then
					Dim fromPage_Renamed As Integer = fromPage
					Dim toPage_Renamed As Integer = toPage
					Return New Integer() { New Integer() { fromPage_Renamed, toPage_Renamed } }
				Else
					Dim minPage_Renamed As Integer = minPage
					Return New Integer() { New Integer() { minPage_Renamed, minPage_Renamed } }
				End If
			End Get
			Set(ByVal pageRanges As Integer()())
				Dim xcp As String = "Invalid value for attribute pageRanges"
				Dim first As Integer = 0
				Dim last As Integer = 0
    
				If pageRanges Is Nothing Then Throw New IllegalArgumentException(xcp)
    
				For i As Integer = 0 To pageRanges.Length - 1
					If pageRanges(i) Is Nothing OrElse pageRanges(i).Length <> 2 OrElse pageRanges(i)(0) <= last OrElse pageRanges(i)(1) < pageRanges(i)(0) Then Throw New IllegalArgumentException(xcp)
					last = pageRanges(i)(1)
					If first = 0 Then first = pageRanges(i)(0)
				Next i
    
				If first < minPage OrElse last > maxPage Then Throw New IllegalArgumentException(xcp)
    
				' Store a copy because otherwise client code could circumvent the
				' the checks made above by holding a reference to the array and
				' modifying it after calling setPageRanges.
				Dim copy As Integer()() = RectangularArrays.ReturnRectangularIntegerArray(pageRanges.Length, 2)
				For i As Integer = 0 To pageRanges.Length - 1
					copy(i)(0) = pageRanges(i)(0)
					copy(i)(1) = pageRanges(i)(1)
				Next i
				Me.pageRanges = copy
				Me.prFirst = first
				Me.prLast = last
			End Set
		End Property


		''' <summary>
		''' Returns the destination printer for jobs using these attributes. This
		''' attribute is updated to the value chosen by the user.
		''' </summary>
		''' <returns>  the possibly null printer name. </returns>
		Public Property printer As String
			Get
				Return printer
			End Get
			Set(ByVal printer As String)
				Me.printer = printer
			End Set
		End Property


		''' <summary>
		''' Returns how consecutive pages should be imposed upon the sides of the
		''' print medium for jobs using these attributes. SidesType.ONE_SIDED
		''' imposes each consecutive page upon the same side of consecutive media
		''' sheets. This imposition is sometimes called <i>simplex</i>.
		''' SidesType.TWO_SIDED_LONG_EDGE imposes each consecutive pair of pages
		''' upon front and back sides of consecutive media sheets, such that the
		''' orientation of each pair of pages on the medium would be correct for
		''' the reader as if for binding on the long edge. This imposition is
		''' sometimes called <i>duplex</i>. SidesType.TWO_SIDED_SHORT_EDGE imposes
		''' each consecutive pair of pages upon front and back sides of consecutive
		''' media sheets, such that the orientation of each pair of pages on the
		''' medium would be correct for the reader as if for binding on the short
		''' edge. This imposition is sometimes called <i>tumble</i>. This attribute
		''' is updated to the value chosen by the user.
		''' </summary>
		''' <returns>  SidesType.ONE_SIDED, SidesType.TWO_SIDED_LONG_EDGE, or
		'''          SidesType.TWO_SIDED_SHORT_EDGE. </returns>
		Public Property sides As SidesType
			Get
				Return sides
			End Get
			Set(ByVal sides As SidesType)
				If sides Is Nothing Then Throw New IllegalArgumentException("Invalid value for attribute " & "sides")
				Me.sides = sides
			End Set
		End Property


		''' <summary>
		''' Sets how consecutive pages should be imposed upon the sides of the
		''' print medium for jobs using these attributes to the default. The
		''' default imposition is SidesType.ONE_SIDED.
		''' </summary>
		Public Sub setSidesToDefault()
			sides = SidesType.ONE_SIDED
		End Sub

		''' <summary>
		''' Returns, for jobs using these attributes, the last page (inclusive)
		''' to be printed, if a range of pages is to be printed. This attribute is
		''' updated to the value chosen by the user. An application should ignore
		''' this attribute on output, unless the return value of the <code>
		''' getDefaultSelection</code> method is DefaultSelectionType.RANGE. An
		''' application should honor the return value of <code>getPageRanges</code>
		''' over the return value of this method, if possible.
		''' </summary>
		''' <returns>  an integer greater than zero and greater than or equal
		'''          to <i>toPage</i> and greater than or equal to <i>minPage</i>
		'''          and less than or equal to <i>maxPage</i>. </returns>
		Public Property toPage As Integer
			Get
				If toPage <> 0 Then
					Return toPage
				ElseIf fromPage <> 0 Then
					Return fromPage
				ElseIf pageRanges IsNot Nothing Then
					Return prLast
				Else
					Return minPage
				End If
			End Get
			Set(ByVal toPage As Integer)
				If toPage <= 0 OrElse (fromPage <> 0 AndAlso toPage < fromPage) OrElse toPage < minPage OrElse toPage > maxPage Then Throw New IllegalArgumentException("Invalid value for attribute " & "toPage")
				Me.toPage = toPage
			End Set
		End Property


		''' <summary>
		''' Determines whether two JobAttributes are equal to each other.
		''' <p>
		''' Two JobAttributes are equal if and only if each of their attributes are
		''' equal. Attributes of enumeration type are equal if and only if the
		''' fields refer to the same unique enumeration object. A set of page
		''' ranges is equal if and only if the sets are of equal length, each range
		''' enumerates the same pages, and the ranges are in the same order.
		''' </summary>
		''' <param name="obj"> the object whose equality will be checked. </param>
		''' <returns>  whether obj is equal to this JobAttribute according to the
		'''          above criteria. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Not(TypeOf obj Is JobAttributes) Then Return False
			Dim rhs As JobAttributes = CType(obj, JobAttributes)

			If fileName Is Nothing Then
				If rhs.fileName IsNot Nothing Then Return False
			Else
				If Not fileName.Equals(rhs.fileName) Then Return False
			End If

			If pageRanges Is Nothing Then
				If rhs.pageRanges IsNot Nothing Then Return False
			Else
				If rhs.pageRanges Is Nothing OrElse pageRanges.Length <> rhs.pageRanges.Length Then Return False
				For i As Integer = 0 To pageRanges.Length - 1
					If pageRanges(i)(0) <> rhs.pageRanges(i)(0) OrElse pageRanges(i)(1) <> rhs.pageRanges(i)(1) Then Return False
				Next i
			End If

			If printer Is Nothing Then
				If rhs.printer IsNot Nothing Then Return False
			Else
				If Not printer.Equals(rhs.printer) Then Return False
			End If

			Return (copies = rhs.copies AndAlso defaultSelection Is rhs.defaultSelection AndAlso destination Is rhs.destination AndAlso dialog_Renamed Is rhs.dialog_Renamed AndAlso fromPage = rhs.fromPage AndAlso maxPage = rhs.maxPage AndAlso minPage = rhs.minPage AndAlso multipleDocumentHandling Is rhs.multipleDocumentHandling AndAlso prFirst = rhs.prFirst AndAlso prLast = rhs.prLast AndAlso sides Is rhs.sides AndAlso toPage = rhs.toPage)
		End Function

		''' <summary>
		''' Returns a hash code value for this JobAttributes.
		''' </summary>
		''' <returns>  the hash code. </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim rest As Integer = ((copies + fromPage + maxPage + minPage + prFirst + prLast + toPage) * 31) << 21
			If pageRanges IsNot Nothing Then
				Dim sum As Integer = 0
				For i As Integer = 0 To pageRanges.Length - 1
					sum += pageRanges(i)(0) + pageRanges(i)(1)
				Next i
				rest = rest Xor (sum * 31) << 11
			End If
			If fileName IsNot Nothing Then rest = rest Xor fileName.GetHashCode()
			If printer IsNot Nothing Then rest = rest Xor printer.GetHashCode()
			Return (defaultSelection.GetHashCode() << 6 Xor destination.GetHashCode() << 5 Xor dialog_Renamed.GetHashCode() << 3 Xor multipleDocumentHandling.GetHashCode() << 2 Xor sides.GetHashCode() Xor rest)
		End Function

		''' <summary>
		''' Returns a string representation of this JobAttributes.
		''' </summary>
		''' <returns>  the string representation. </returns>
		Public Overrides Function ToString() As String
			Dim pageRanges_Renamed As Integer()() = pageRanges
			Dim prStr As String = "["
			Dim first As Boolean = True
			For i As Integer = 0 To pageRanges_Renamed.Length - 1
				If first Then
					first = False
				Else
					prStr &= ","
				End If
				prStr += pageRanges_Renamed(i)(0) & ":" & pageRanges_Renamed(i)(1)
			Next i
			prStr &= "]"

			Return "copies=" & copies & ",defaultSelection=" & defaultSelection & ",destination=" & destination & ",dialog=" & dialog & ",fileName=" & fileName & ",fromPage=" & fromPage & ",maxPage=" & maxPage & ",minPage=" & minPage & ",multiple-document-handling=" & multipleDocumentHandling & ",page-ranges=" & prStr & ",printer=" & printer & ",sides=" & sides & ",toPage=" & toPage
		End Function
	End Class

End Namespace

'----------------------------------------------------------------------------------------
'	Copyright © 2007 - 2012 Tangible Software Solutions Inc.
'	This class can be used by anyone provided that the copyright notice remains intact.
'
'	This class provides the logic to simulate Java rectangular arrays, which are jagged
'	arrays with inner arrays of the same length.
'----------------------------------------------------------------------------------------
Partial Friend Class RectangularArrays
    Friend Shared Function ReturnRectangularIntegerArray(ByVal Size1 As Integer, ByVal Size2 As Integer) As Integer()()
        Dim Array As Integer()() = New Integer(Size1 - 1)() {}
        For Array1 As Integer = 0 To Size1 - 1
            Array(Array1) = New Integer(Size2 - 1) {}
        Next Array1
        Return Array
    End Function
End Class
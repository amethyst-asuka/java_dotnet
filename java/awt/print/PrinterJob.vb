Imports System

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.print




	''' <summary>
	''' The <code>PrinterJob</code> class is the principal class that controls
	''' printing. An application calls methods in this class to set up a job,
	''' optionally to invoke a print dialog with the user, and then to print
	''' the pages of the job.
	''' </summary>
	Public MustInherit Class PrinterJob

	 ' Public Class Methods 

		''' <summary>
		''' Creates and returns a <code>PrinterJob</code> which is initially
		''' associated with the default printer.
		''' If no printers are available on the system, a PrinterJob will still
		''' be returned from this method, but <code>getPrintService()</code>
		''' will return <code>null</code>, and calling
		''' <seealso cref="#print() print"/> with this <code>PrinterJob</code> might
		''' generate an exception.  Applications that need to determine if
		''' there are suitable printers before creating a <code>PrinterJob</code>
		''' should ensure that the array returned from
		''' <seealso cref="#lookupPrintServices() lookupPrintServices"/> is not empty. </summary>
		''' <returns> a new <code>PrinterJob</code>.
		''' </returns>
		''' <exception cref="SecurityException"> if a security manager exists and its
		'''          <seealso cref="java.lang.SecurityManager#checkPrintJobAccess"/>
		'''          method disallows this thread from creating a print job request </exception>
		PublicShared ReadOnly PropertyprinterJob As PrinterJob
			Get
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkPrintJobAccess()
				Return (PrinterJob) java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			End Get
		End Property

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As Object
				Dim nm As String = System.getProperty("java.awt.printerjob", Nothing)
				Try
					Return CType(Type.GetType(nm).newInstance(), PrinterJob)
				Catch e As  ClassNotFoundException
					Throw New java.awt.AWTError("PrinterJob not found: " & nm)
				Catch e As InstantiationException
				 Throw New java.awt.AWTError("Could not instantiate PrinterJob: " & nm)
				Catch e As IllegalAccessException
					Throw New java.awt.AWTError("Could not access PrinterJob: " & nm)
				End Try
			End Function
		End Class

		''' <summary>
		''' A convenience method which looks up 2D print services.
		''' Services returned from this method may be installed on
		''' <code>PrinterJob</code>s which support print services.
		''' Calling this method is equivalent to calling
		''' {@link javax.print.PrintServiceLookup#lookupPrintServices(
		''' DocFlavor, AttributeSet)
		''' PrintServiceLookup.lookupPrintServices()}
		''' and specifying a Pageable DocFlavor. </summary>
		''' <returns> a possibly empty array of 2D print services.
		''' @since     1.4 </returns>
		Public Shared Function lookupPrintServices() As javax.print.PrintService()
			Return javax.print.PrintServiceLookup.lookupPrintServices(javax.print.DocFlavor.SERVICE_FORMATTED.PAGEABLE, Nothing)
		End Function


		''' <summary>
		''' A convenience method which locates factories for stream print
		''' services which can image 2D graphics.
		''' Sample usage :
		''' <pre>{@code
		''' FileOutputStream outstream;
		''' StreamPrintService psPrinter;
		''' String psMimeType = "application/postscript";
		''' PrinterJob pj = PrinterJob.getPrinterJob();
		''' 
		''' StreamPrintServiceFactory[] factories =
		'''     PrinterJob.lookupStreamPrintServices(psMimeType);
		''' if (factories.length > 0) {
		'''     try {
		'''         outstream = new File("out.ps");
		'''         psPrinter =  factories[0].getPrintService(outstream);
		'''         // psPrinter can now be set as the service on a PrinterJob
		'''         pj.setPrintService(psPrinter)
		'''     } catch (Exception e) {
		'''         e.printStackTrace();
		'''     }
		''' }
		''' }</pre>
		''' Services returned from this method may be installed on
		''' <code>PrinterJob</code> instances which support print services.
		''' Calling this method is equivalent to calling
		''' {@link javax.print.StreamPrintServiceFactory#lookupStreamPrintServiceFactories(DocFlavor, String)
		''' StreamPrintServiceFactory.lookupStreamPrintServiceFactories()
		''' } and specifying a Pageable DocFlavor.
		''' </summary>
		''' <param name="mimeType"> the required output format, or null to mean any format. </param>
		''' <returns> a possibly empty array of 2D stream print service factories.
		''' @since     1.4 </returns>
		Public Shared Function lookupStreamPrintServices(  mimeType As String) As javax.print.StreamPrintServiceFactory()
			Return javax.print.StreamPrintServiceFactory.lookupStreamPrintServiceFactories(javax.print.DocFlavor.SERVICE_FORMATTED.PAGEABLE, mimeType)
		End Function


	 ' Public Methods 

		''' <summary>
		''' A <code>PrinterJob</code> object should be created using the
		''' static <seealso cref="#getPrinterJob() getPrinterJob"/> method.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Returns the service (printer) for this printer job.
		''' Implementations of this class which do not support print services
		''' may return null.  null will also be returned if no printers are
		''' available. </summary>
		''' <returns> the service for this printer job. </returns>
		''' <seealso cref= #setPrintService(PrintService) </seealso>
		''' <seealso cref= #getPrinterJob()
		''' @since     1.4 </seealso>
		Public Overridable Property printService As javax.print.PrintService
			Get
				Return Nothing
			End Get
			Set(  service As javax.print.PrintService)
					Throw New PrinterException("Setting a service is not supported on this class")
			End Set
		End Property


		''' <summary>
		''' Calls <code>painter</code> to render the pages.  The pages in the
		''' document to be printed by this
		''' <code>PrinterJob</code> are rendered by the <seealso cref="Printable"/>
		''' object, <code>painter</code>.  The <seealso cref="PageFormat"/> for each page
		''' is the default page format. </summary>
		''' <param name="painter"> the <code>Printable</code> that renders each page of
		''' the document. </param>
		Public MustOverride WriteOnly Property printable As Printable

		''' <summary>
		''' Calls <code>painter</code> to render the pages in the specified
		''' <code>format</code>.  The pages in the document to be printed by
		''' this <code>PrinterJob</code> are rendered by the
		''' <code>Printable</code> object, <code>painter</code>. The
		''' <code>PageFormat</code> of each page is <code>format</code>. </summary>
		''' <param name="painter"> the <code>Printable</code> called to render
		'''          each page of the document </param>
		''' <param name="format"> the size and orientation of each page to
		'''                   be printed </param>
		Public MustOverride Sub setPrintable(  painter As Printable,   format As PageFormat)

		''' <summary>
		''' Queries <code>document</code> for the number of pages and
		''' the <code>PageFormat</code> and <code>Printable</code> for each
		''' page held in the <code>Pageable</code> instance,
		''' <code>document</code>. </summary>
		''' <param name="document"> the pages to be printed. It can not be
		''' <code>null</code>. </param>
		''' <exception cref="NullPointerException"> the <code>Pageable</code> passed in
		''' was <code>null</code>. </exception>
		''' <seealso cref= PageFormat </seealso>
		''' <seealso cref= Printable </seealso>
		Public MustOverride WriteOnly Property pageable As Pageable

		''' <summary>
		''' Presents a dialog to the user for changing the properties of
		''' the print job.
		''' This method will display a native dialog if a native print
		''' service is selected, and user choice of printers will be restricted
		''' to these native print services.
		''' To present the cross platform print dialog for all services,
		''' including native ones instead use
		''' <code>printDialog(PrintRequestAttributeSet)</code>.
		''' <p>
		''' PrinterJob implementations which can use PrintService's will update
		''' the PrintService for this PrinterJob to reflect the new service
		''' selected by the user. </summary>
		''' <returns> <code>true</code> if the user does not cancel the dialog;
		''' <code>false</code> otherwise. </returns>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public MustOverride Function printDialog() As Boolean

		''' <summary>
		''' A convenience method which displays a cross-platform print dialog
		''' for all services which are capable of printing 2D graphics using the
		''' <code>Pageable</code> interface. The selected printer when the
		''' dialog is initially displayed will reflect the print service currently
		''' attached to this print job.
		''' If the user changes the print service, the PrinterJob will be
		''' updated to reflect this, unless the user cancels the dialog.
		''' As well as allowing the user to select the destination printer,
		''' the user can also select values of various print request attributes.
		''' <p>
		''' The attributes parameter on input will reflect the applications
		''' required initial selections in the user dialog. Attributes not
		''' specified display using the default for the service. On return it
		''' will reflect the user's choices. Selections may be updated by
		''' the implementation to be consistent with the supported values
		''' for the currently selected print service.
		''' <p>
		''' As the user scrolls to a new print service selection, the values
		''' copied are based on the settings for the previous service, together
		''' with any user changes. The values are not based on the original
		''' settings supplied by the client.
		''' <p>
		''' With the exception of selected printer, the PrinterJob state is
		''' not updated to reflect the user's changes.
		''' For the selections to affect a printer job, the attributes must
		''' be specified in the call to the
		''' <code>print(PrintRequestAttributeSet)</code> method. If using
		''' the Pageable interface, clients which intend to use media selected
		''' by the user must create a PageFormat derived from the user's
		''' selections.
		''' If the user cancels the dialog, the attributes will not reflect
		''' any changes made by the user. </summary>
		''' <param name="attributes"> on input is application supplied attributes,
		''' on output the contents are updated to reflect user choices.
		''' This parameter may not be null. </param>
		''' <returns> <code>true</code> if the user does not cancel the dialog;
		''' <code>false</code> otherwise. </returns>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <exception cref="NullPointerException"> if <code>attributes</code> parameter
		''' is null. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since     1.4
		'''  </seealso>
		Public Overridable Function printDialog(  attributes As javax.print.attribute.PrintRequestAttributeSet) As Boolean

			If attributes Is Nothing Then Throw New NullPointerException("attributes")
			Return printDialog()
		End Function

		''' <summary>
		''' Displays a dialog that allows modification of a
		''' <code>PageFormat</code> instance.
		''' The <code>page</code> argument is used to initialize controls
		''' in the page setup dialog.
		''' If the user cancels the dialog then this method returns the
		''' original <code>page</code> object unmodified.
		''' If the user okays the dialog then this method returns a new
		''' <code>PageFormat</code> object with the indicated changes.
		''' In either case, the original <code>page</code> object is
		''' not modified. </summary>
		''' <param name="page"> the default <code>PageFormat</code> presented to the
		'''                  user for modification </param>
		''' <returns>    the original <code>page</code> object if the dialog
		'''            is cancelled; a new <code>PageFormat</code> object
		'''            containing the format indicated by the user if the
		'''            dialog is acknowledged. </returns>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since     1.2 </seealso>
		Public MustOverride Function pageDialog(  page As PageFormat) As PageFormat

		''' <summary>
		''' A convenience method which displays a cross-platform page setup dialog.
		''' The choices available will reflect the print service currently
		''' set on this PrinterJob.
		''' <p>
		''' The attributes parameter on input will reflect the client's
		''' required initial selections in the user dialog. Attributes which are
		''' not specified display using the default for the service. On return it
		''' will reflect the user's choices. Selections may be updated by
		''' the implementation to be consistent with the supported values
		''' for the currently selected print service.
		''' <p>
		''' The return value will be a PageFormat equivalent to the
		''' selections in the PrintRequestAttributeSet.
		''' If the user cancels the dialog, the attributes will not reflect
		''' any changes made by the user, and the return value will be null. </summary>
		''' <param name="attributes"> on input is application supplied attributes,
		''' on output the contents are updated to reflect user choices.
		''' This parameter may not be null. </param>
		''' <returns> a page format if the user does not cancel the dialog;
		''' <code>null</code> otherwise. </returns>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <exception cref="NullPointerException"> if <code>attributes</code> parameter
		''' is null. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since     1.4
		'''  </seealso>
		Public Overridable Function pageDialog(  attributes As javax.print.attribute.PrintRequestAttributeSet) As PageFormat

			If attributes Is Nothing Then Throw New NullPointerException("attributes")
			Return pageDialog(defaultPage())
		End Function

		''' <summary>
		''' Clones the <code>PageFormat</code> argument and alters the
		''' clone to describe a default page size and orientation. </summary>
		''' <param name="page"> the <code>PageFormat</code> to be cloned and altered </param>
		''' <returns> clone of <code>page</code>, altered to describe a default
		'''                      <code>PageFormat</code>. </returns>
		Public MustOverride Function defaultPage(  page As PageFormat) As PageFormat

		''' <summary>
		''' Creates a new <code>PageFormat</code> instance and
		''' sets it to a default size and orientation. </summary>
		''' <returns> a <code>PageFormat</code> set to a default size and
		'''          orientation. </returns>
		Public Overridable Function defaultPage() As PageFormat
			Return defaultPage(New PageFormat)
		End Function

		''' <summary>
		''' Calculates a <code>PageFormat</code> with values consistent with those
		''' supported by the current <code>PrintService</code> for this job
		''' (ie the value returned by <code>getPrintService()</code>) and media,
		''' printable area and orientation contained in <code>attributes</code>.
		''' <p>
		''' Calling this method does not update the job.
		''' It is useful for clients that have a set of attributes obtained from
		''' <code>printDialog(PrintRequestAttributeSet attributes)</code>
		''' and need a PageFormat to print a Pageable object. </summary>
		''' <param name="attributes"> a set of printing attributes, for example obtained
		''' from calling printDialog. If <code>attributes</code> is null a default
		''' PageFormat is returned. </param>
		''' <returns> a <code>PageFormat</code> whose settings conform with
		''' those of the current service and the specified attributes.
		''' @since 1.6 </returns>
		Public Overridable Function getPageFormat(  attributes As javax.print.attribute.PrintRequestAttributeSet) As PageFormat

			Dim service As javax.print.PrintService = printService
			Dim pf As PageFormat = defaultPage()

			If service Is Nothing OrElse attributes Is Nothing Then Return pf

			Dim media As javax.print.attribute.standard.Media = CType(attributes.get(GetType(javax.print.attribute.standard.Media)), javax.print.attribute.standard.Media)
			Dim mpa As javax.print.attribute.standard.MediaPrintableArea = CType(attributes.get(GetType(javax.print.attribute.standard.MediaPrintableArea)), javax.print.attribute.standard.MediaPrintableArea)
			Dim orientReq As javax.print.attribute.standard.OrientationRequested = CType(attributes.get(GetType(javax.print.attribute.standard.OrientationRequested)), javax.print.attribute.standard.OrientationRequested)

			If media Is Nothing AndAlso mpa Is Nothing AndAlso orientReq Is Nothing Then Return pf
			Dim paper As Paper = pf.paper

	'         If there's a media but no media printable area, we can try
	'         * to retrieve the default value for mpa and use that.
	'         
			If mpa Is Nothing AndAlso media IsNot Nothing AndAlso service.isAttributeCategorySupported(GetType(javax.print.attribute.standard.MediaPrintableArea)) Then
				Dim mpaVals As Object = service.getSupportedAttributeValues(GetType(javax.print.attribute.standard.MediaPrintableArea), Nothing, attributes)
				If TypeOf mpaVals Is javax.print.attribute.standard.MediaPrintableArea() AndAlso CType(mpaVals, javax.print.attribute.standard.MediaPrintableArea()).Length > 0 Then mpa = CType(mpaVals, javax.print.attribute.standard.MediaPrintableArea())(0)
			End If

			If media IsNot Nothing AndAlso service.isAttributeValueSupported(media, Nothing, attributes) Then
				If TypeOf media Is javax.print.attribute.standard.MediaSizeName Then
					Dim msn As javax.print.attribute.standard.MediaSizeName = CType(media, javax.print.attribute.standard.MediaSizeName)
					Dim msz As javax.print.attribute.standard.MediaSize = javax.print.attribute.standard.MediaSize.getMediaSizeForName(msn)
					If msz IsNot Nothing Then
						Dim inch As Double = 72.0
						Dim paperWid As Double = msz.getX(javax.print.attribute.standard.MediaSize.INCH) * inch
						Dim paperHgt As Double = msz.getY(javax.print.attribute.standard.MediaSize.INCH) * inch
						paper.sizeize(paperWid, paperHgt)
						If mpa Is Nothing Then paper.imageableArearea(inch, inch, paperWid-2*inch, paperHgt-2*inch)
					End If
				End If
			End If

			If mpa IsNot Nothing AndAlso service.isAttributeValueSupported(mpa, Nothing, attributes) Then
				Dim printableArea As Single() = mpa.getPrintableArea(javax.print.attribute.standard.MediaPrintableArea.INCH)
				For i As Integer = 0 To printableArea.Length - 1
					printableArea(i) = printableArea(i)*72.0f
				Next i
				paper.imageableArearea(printableArea(0), printableArea(1), printableArea(2), printableArea(3))
			End If

			If orientReq IsNot Nothing AndAlso service.isAttributeValueSupported(orientReq, Nothing, attributes) Then
				Dim orient As Integer
				If orientReq.Equals(javax.print.attribute.standard.OrientationRequested.REVERSE_LANDSCAPE) Then
					orient = PageFormat.REVERSE_LANDSCAPE
				ElseIf orientReq.Equals(javax.print.attribute.standard.OrientationRequested.LANDSCAPE) Then
					orient = PageFormat.LANDSCAPE
				Else
					orient = PageFormat.PORTRAIT
				End If
				pf.orientation = orient
			End If

			pf.paper = paper
			pf = validatePage(pf)
			Return pf
		End Function

		''' <summary>
		''' Returns the clone of <code>page</code> with its settings
		''' adjusted to be compatible with the current printer of this
		''' <code>PrinterJob</code>.  For example, the returned
		''' <code>PageFormat</code> could have its imageable area
		''' adjusted to fit within the physical area of the paper that
		''' is used by the current printer. </summary>
		''' <param name="page"> the <code>PageFormat</code> that is cloned and
		'''          whose settings are changed to be compatible with
		'''          the current printer </param>
		''' <returns> a <code>PageFormat</code> that is cloned from
		'''          <code>page</code> and whose settings are changed
		'''          to conform with this <code>PrinterJob</code>. </returns>
		Public MustOverride Function validatePage(  page As PageFormat) As PageFormat

		''' <summary>
		''' Prints a set of pages. </summary>
		''' <exception cref="PrinterException"> an error in the print system
		'''            caused the job to be aborted. </exception>
		''' <seealso cref= Book </seealso>
		''' <seealso cref= Pageable </seealso>
		''' <seealso cref= Printable </seealso>
		Public MustOverride Sub print()

	   ''' <summary>
	   ''' Prints a set of pages using the settings in the attribute
	   ''' set. The default implementation ignores the attribute set.
	   ''' <p>
	   ''' Note that some attributes may be set directly on the PrinterJob
	   ''' by equivalent method calls, (for example), copies:
	   ''' <code>setcopies(int)</code>, job name: <code>setJobName(String)</code>
	   ''' and specifying media size and orientation though the
	   ''' <code>PageFormat</code> object.
	   ''' <p>
	   ''' If a supported attribute-value is specified in this attribute set,
	   ''' it will take precedence over the API settings for this print()
	   ''' operation only.
	   ''' The following behaviour is specified for PageFormat:
	   ''' If a client uses the Printable interface, then the
	   ''' <code>attributes</code> parameter to this method is examined
	   ''' for attributes which specify media (by size), orientation, and
	   ''' imageable area, and those are used to construct a new PageFormat
	   ''' which is passed to the Printable object's print() method.
	   ''' See <seealso cref="Printable"/> for an explanation of the required
	   ''' behaviour of a Printable to ensure optimal printing via PrinterJob.
	   ''' For clients of the Pageable interface, the PageFormat will always
	   ''' be as supplied by that interface, on a per page basis.
	   ''' <p>
	   ''' These behaviours allow an application to directly pass the
	   ''' user settings returned from
	   ''' <code>printDialog(PrintRequestAttributeSet attributes</code> to
	   ''' this print() method.
	   ''' <p>
	   ''' </summary>
	   ''' <param name="attributes"> a set of attributes for the job </param>
	   ''' <exception cref="PrinterException"> an error in the print system
	   '''            caused the job to be aborted. </exception>
	   ''' <seealso cref= Book </seealso>
	   ''' <seealso cref= Pageable </seealso>
	   ''' <seealso cref= Printable
	   ''' @since 1.4 </seealso>
		Public Overridable Sub print(  attributes As javax.print.attribute.PrintRequestAttributeSet)
			print()
		End Sub

		''' <summary>
		''' Sets the number of copies to be printed. </summary>
		''' <param name="copies"> the number of copies to be printed </param>
		''' <seealso cref= #getCopies </seealso>
		Public MustOverride Property copies As Integer


		''' <summary>
		''' Gets the name of the printing user. </summary>
		''' <returns> the name of the printing user </returns>
		Public MustOverride ReadOnly Property userName As String

		''' <summary>
		''' Sets the name of the document to be printed.
		''' The document name can not be <code>null</code>. </summary>
		''' <param name="jobName"> the name of the document to be printed </param>
		''' <seealso cref= #getJobName </seealso>
		Public MustOverride Property jobName As String


		''' <summary>
		''' Cancels a print job that is in progress.  If
		''' <seealso cref="#print() print"/> has been called but has not
		''' returned then this method signals
		''' that the job should be cancelled at the next
		''' chance. If there is no print job in progress then
		''' this call does nothing.
		''' </summary>
		Public MustOverride Sub cancel()

		''' <summary>
		''' Returns <code>true</code> if a print job is
		''' in progress, but is going to be cancelled
		''' at the next opportunity; otherwise returns
		''' <code>false</code>. </summary>
		''' <returns> <code>true</code> if the job in progress
		''' is going to be cancelled; <code>false</code> otherwise. </returns>
		Public MustOverride ReadOnly Property cancelled As Boolean

	End Class

End Namespace
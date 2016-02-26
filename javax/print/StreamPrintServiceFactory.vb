Imports System.Collections

'
' * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print





	''' <summary>
	''' A <code>StreamPrintServiceFactory</code> is the factory for
	''' <seealso cref="StreamPrintService"/> instances,
	''' which can print to an output stream in a particular
	''' document format described as a mime type.
	''' A typical output document format may be Postscript(TM).
	''' <p>
	''' This class is implemented by a service and located by the
	''' implementation using the
	''' <a href="../../../technotes/guides/jar/jar.html#Service Provider">
	''' SPI JAR File specification</a>.
	''' <p>
	''' Applications locate instances of this class by calling the
	''' <seealso cref="#lookupStreamPrintServiceFactories(DocFlavor, String)"/> method.
	''' <p>
	''' Applications can use a <code>StreamPrintService</code> obtained from a
	''' factory in place of a <code>PrintService</code> which represents a
	''' physical printer device.
	''' </summary>

	Public MustInherit Class StreamPrintServiceFactory

		Friend Class Services
			Private listOfFactories As ArrayList = Nothing
		End Class

		Private Property Shared services As Services
			Get
				Dim ___services As Services = CType(sun.awt.AppContext.appContext.get(GetType(Services)), Services)
				If ___services Is Nothing Then
					___services = New Services
					sun.awt.AppContext.appContext.put(GetType(Services), ___services)
				End If
				Return ___services
			End Get
		End Property

		Private Property Shared listOfFactories As ArrayList
			Get
				Return services.listOfFactories
			End Get
		End Property

		Private Shared Function initListOfFactories() As ArrayList
			Dim ___listOfFactories As New ArrayList
			services.listOfFactories = ___listOfFactories
			Return ___listOfFactories
		End Function

		''' <summary>
		''' Locates factories for print services that can be used with
		''' a print job to output a stream of data in the
		''' format specified by {@code outputMimeType}.
		''' <p>
		''' The {@code outputMimeType} parameter describes the document type that
		''' you want to create, whereas the {@code flavor} parameter describes the
		''' format in which the input data will be provided by the application
		''' to the {@code StreamPrintService}.
		''' <p>
		''' Although null is an acceptable value to use in the lookup of stream
		''' printing services, it's typical to search for a particular
		''' desired format, such as Postscript(TM).
		''' <p> </summary>
		''' <param name="flavor"> of the input document type - null means match all
		''' types. </param>
		''' <param name="outputMimeType"> representing the required output format, used to
		''' identify suitable stream printer factories. A value of null means
		''' match all formats. </param>
		''' <returns> - matching factories for stream print service instance,
		'''           empty if no suitable factories could be located. </returns>
		 Public Shared Function lookupStreamPrintServiceFactories(ByVal flavor As javax.print.DocFlavor, ByVal outputMimeType As String) As StreamPrintServiceFactory()

			 Dim list As ArrayList = getFactories(flavor, outputMimeType)
			 Return CType(list.ToArray(GetType(StreamPrintServiceFactory)), StreamPrintServiceFactory())
		 End Function

		''' <summary>
		''' Queries the factory for the document format that is emitted
		''' by printers obtained from this factory.
		''' </summary>
		''' <returns> the output format described as a mime type. </returns>
		Public MustOverride ReadOnly Property outputFormat As String

		''' <summary>
		''' Queries the factory for the document flavors that can be accepted
		''' by printers obtained from this factory. </summary>
		''' <returns> array of supported doc flavors. </returns>
		Public MustOverride ReadOnly Property supportedDocFlavors As javax.print.DocFlavor()

		''' <summary>
		''' Returns a <code>StreamPrintService</code> that can print to
		''' the specified output stream.
		''' The output stream is created and managed by the application.
		''' It is the application's responsibility to close the stream and
		''' to ensure that this Printer is not reused.
		''' The application should not close this stream until any print job
		''' created from the printer is complete. Doing so earlier may generate
		''' a <code>PrinterException</code> and an event indicating that the
		''' job failed.
		''' <p>
		''' Whereas a <code>PrintService</code> connected to a physical printer
		''' can be reused,
		''' a <code>StreamPrintService</code> connected to a stream cannot.
		''' The underlying <code>StreamPrintService</code> may be disposed by
		''' the print system with
		''' the <seealso cref="StreamPrintService#dispose() dispose"/> method
		''' before returning from the
		''' <seealso cref="DocPrintJob#print(Doc, javax.print.attribute.PrintRequestAttributeSet) print"/>
		''' method of <code>DocPrintJob</code> so that the print system knows
		''' this printer is no longer usable.
		''' This is equivalent to a physical printer going offline - permanently.
		''' Applications may supply a null print stream to create a queryable
		''' service. It is not valid to create a PrintJob for such a stream.
		''' Implementations which allocate resources on construction should examine
		''' the stream and may wish to only allocate resources if the stream is
		''' non-null.
		''' <p> </summary>
		''' <param name="out"> destination stream for generated output. </param>
		''' <returns> a PrintService which will generate the format specified by the
		''' DocFlavor supported by this Factory. </returns>
		Public MustOverride Function getPrintService(ByVal out As java.io.OutputStream) As StreamPrintService


		Private Property Shared allFactories As ArrayList
			Get
				SyncLock GetType(StreamPrintServiceFactory)
    
				  Dim ___listOfFactories As ArrayList = listOfFactories
					If ___listOfFactories IsNot Nothing Then
						Return ___listOfFactories
					Else
						___listOfFactories = initListOfFactories()
					End If
    
					Try
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'					java.security.AccessController.doPrivileged(New java.security.PrivilegedExceptionAction()
		'				{
		'						public Object run()
		'						{
		'							Iterator<StreamPrintServiceFactory> iterator = ServiceLoader.load(StreamPrintServiceFactory.class).iterator();
		'							ArrayList lof = getListOfFactories();
		'							while (iterator.hasNext())
		'							{
		'								try
		'								{
		'									lof.add(iterator.next());
		'								}
		'								catch (ServiceConfigurationError err)
		'								{
		'									 ' In the applet case, we continue 
		'									if (System.getSecurityManager() != Nothing)
		'									{
		'										err.printStackTrace();
		'									}
		'									else
		'									{
		'										throw err;
		'									}
		'								}
		'							}
		'							Return Nothing;
		'						}
		'				});
					Catch e As java.security.PrivilegedActionException
					End Try
					Return ___listOfFactories
				End SyncLock
			End Get
		End Property

		Private Shared Function isMember(ByVal flavor As javax.print.DocFlavor, ByVal flavors As javax.print.DocFlavor()) As Boolean
			For f As Integer = 0 To flavors.Length - 1
				If flavor.Equals(flavors(f)) Then Return True
			Next f
			Return False
		End Function

		Private Shared Function getFactories(ByVal flavor As javax.print.DocFlavor, ByVal outType As String) As ArrayList

			If flavor Is Nothing AndAlso outType Is Nothing Then Return allFactories

			Dim list As New ArrayList
			Dim [iterator] As IEnumerator = allFactories.GetEnumerator()
			Do While [iterator].hasNext()
				Dim factory As StreamPrintServiceFactory = CType([iterator].next(), StreamPrintServiceFactory)
				If (outType Is Nothing OrElse outType.ToUpper() = factory.outputFormat.ToUpper()) AndAlso (flavor Is Nothing OrElse isMember(flavor, factory.supportedDocFlavors)) Then list.Add(factory)
			Loop

			Return list
		End Function

	End Class

End Namespace
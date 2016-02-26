Imports System

'
' * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' This class extends <seealso cref="PrintService"/> and represents a
	''' print service that prints data in different formats to a
	''' client-provided output stream.
	''' This is principally intended for services where
	''' the output format is a document type suitable for viewing
	''' or archiving.
	''' The output format must be declared as a mime type.
	''' This is equivalent to an output document flavor where the
	''' representation class is always "java.io.OutputStream"
	''' An instance of the <code>StreamPrintService</code> class is
	''' obtained from a <seealso cref="StreamPrintServiceFactory"/> instance.
	''' <p>
	''' Note that a <code>StreamPrintService</code> is different from a
	''' <code>PrintService</code>, which supports a
	''' <seealso cref="javax.print.attribute.standard.Destination Destination"/>
	''' attribute.  A <code>StreamPrintService</code> always requires an output
	''' stream, whereas a <code>PrintService</code> optionally accepts a
	''' <code>Destination</code>. A <code>StreamPrintService</code>
	''' has no default destination for its formatted output.
	''' Additionally a <code>StreamPrintService</code> is expected to generate
	''' output in
	''' a format useful in other contexts.
	''' StreamPrintService's are not expected to support the Destination attribute.
	''' </summary>

	Public MustInherit Class StreamPrintService
		Implements PrintService

			Public MustOverride Function GetHashCode() As Integer
			Public MustOverride Function Equals(ByVal obj As Object) As Boolean
			Public MustOverride ReadOnly Property serviceUIFactory As ServiceUIFactory Implements PrintService.getServiceUIFactory
			Public MustOverride Function getUnsupportedAttributes(ByVal flavor As DocFlavor, ByVal attributes As javax.print.attribute.AttributeSet) As javax.print.attribute.AttributeSet Implements PrintService.getUnsupportedAttributes
			Public MustOverride Function isAttributeValueSupported(ByVal attrval As javax.print.attribute.Attribute, ByVal flavor As DocFlavor, ByVal attributes As javax.print.attribute.AttributeSet) As Boolean Implements PrintService.isAttributeValueSupported
			Public MustOverride Function getSupportedAttributeValues(ByVal category As Type, ByVal flavor As DocFlavor, ByVal attributes As javax.print.attribute.AttributeSet) As Object Implements PrintService.getSupportedAttributeValues
			Public MustOverride Function getDefaultAttributeValue(ByVal category As Type) As Object Implements PrintService.getDefaultAttributeValue
			Public MustOverride Function isAttributeCategorySupported(ByVal category As Type) As Boolean Implements PrintService.isAttributeCategorySupported
			Public MustOverride ReadOnly Property supportedAttributeCategories As Type() Implements PrintService.getSupportedAttributeCategories
			Public MustOverride Function isDocFlavorSupported(ByVal flavor As DocFlavor) As Boolean Implements PrintService.isDocFlavorSupported
			Public MustOverride ReadOnly Property supportedDocFlavors As DocFlavor() Implements PrintService.getSupportedDocFlavors
			Public MustOverride Function getAttribute(ByVal category As Type) As T Implements PrintService.getAttribute
			Public MustOverride ReadOnly Property attributes As javax.print.attribute.PrintServiceAttributeSet Implements PrintService.getAttributes
			Public MustOverride Sub removePrintServiceAttributeListener(ByVal listener As javax.print.event.PrintServiceAttributeListener) Implements PrintService.removePrintServiceAttributeListener
			Public MustOverride Sub addPrintServiceAttributeListener(ByVal listener As javax.print.event.PrintServiceAttributeListener) Implements PrintService.addPrintServiceAttributeListener
			Public MustOverride Function createPrintJob() As DocPrintJob Implements PrintService.createPrintJob
			Public MustOverride ReadOnly Property name As String Implements PrintService.getName

		Private outStream As java.io.OutputStream
		Private disposed As Boolean = False

		Private Sub New()
		End Sub

		''' <summary>
		''' Constructs a StreamPrintService object.
		''' </summary>
		''' <param name="out">  stream to which to send formatted print data. </param>
		Protected Friend Sub New(ByVal out As java.io.OutputStream)
			Me.outStream = out
		End Sub

		''' <summary>
		''' Gets the output stream.
		''' </summary>
		''' <returns> the stream to which this service will send formatted print data. </returns>
		Public Overridable Property outputStream As java.io.OutputStream
			Get
				Return outStream
			End Get
		End Property

		''' <summary>
		''' Returns the document format emitted by this print service.
		''' Must be in mimetype format, compatible with the mime type </summary>
		''' components of DocFlavors <seealso cref= DocFlavor. </seealso>
		''' <returns> mime type identifying the output format. </returns>
		Public MustOverride ReadOnly Property outputFormat As String

		''' <summary>
		''' Disposes this <code>StreamPrintService</code>.
		''' If a stream service cannot be re-used, it must be disposed
		''' to indicate this. Typically the client will call this method.
		''' Services which write data which cannot meaningfully be appended to
		''' may also dispose the stream. This does not close the stream. It
		''' just marks it as not for further use by this service.
		''' </summary>
		Public Overridable Sub dispose()
			disposed = True
		End Sub

		''' <summary>
		''' Returns a <code>boolean</code> indicating whether or not
		''' this <code>StreamPrintService</code> has been disposed.
		''' If this object has been disposed, will return true.
		''' Used by services and client applications to recognize streams
		''' to which no further data should be written. </summary>
		''' <returns> if this <code>StreamPrintService</code> has been disposed </returns>
		Public Overridable Property disposed As Boolean
			Get
				Return disposed
			End Get
		End Property

	End Class

End Namespace
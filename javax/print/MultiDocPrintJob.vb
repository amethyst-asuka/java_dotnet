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

Namespace javax.print


	''' 
	''' <summary>
	''' Obtained from a MultiDocPrintService, a MultiDocPrintJob can print a
	''' specified collection of documents as a single print job with a set of
	''' job attributes.
	''' <P>
	''' </summary>

	Public Interface MultiDocPrintJob
		Inherits DocPrintJob

	   ''' <summary>
	   ''' Print a MultiDoc with the specified job attributes.
	   ''' This method should only be called once for a given print job.
	   ''' Calling it again will not result in a new job being spooled to
	   ''' the printer. The service implementation will define policy
	   ''' for service interruption and recovery. Application clients which
	   ''' want to monitor the success or failure should register a
	   ''' PrintJobListener.
	   ''' </summary>
	   ''' <param name="multiDoc"> The documents to be printed. ALL must be a flavor
	   '''        supported by the PrintJob {@literal &} PrintService.
	   ''' </param>
	   ''' <param name="attributes"> The job attributes to be applied to this print job.
	   '''        If this parameter is null then the default attributes are used.
	   ''' </param>
	   ''' <exception cref="PrintException"> The exception additionally may implement
	   ''' an interfaces which more precisely describes the cause of the exception
	   ''' <ul>
	   ''' <li>FlavorException.
	   '''  If the document has a flavor not supported by this print job.
	   ''' <li>AttributeException.
	   '''  If one or more of the attributes are not valid for this print job.
	   ''' </ul> </exception>
		Sub print(ByVal multiDoc As MultiDoc, ByVal attributes As javax.print.attribute.PrintRequestAttributeSet)

	End Interface

End Namespace
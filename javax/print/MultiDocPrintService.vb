'
' * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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
	 ''' Interface MultiPrintService is the factory for a MultiDocPrintJob.
	 ''' A MultiPrintService
	 ''' describes the capabilities of a Printer and can be queried regarding
	 ''' a printer's supported attributes.
	 ''' </summary>
	Public Interface MultiDocPrintService
		Inherits PrintService

		''' <summary>
		''' Create a job which can print a multiDoc. </summary>
		''' <returns> a MultiDocPrintJob </returns>
		Function createMultiDocPrintJob() As MultiDocPrintJob

	End Interface

End Namespace
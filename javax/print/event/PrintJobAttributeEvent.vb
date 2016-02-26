'
' * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print.event


	''' <summary>
	''' Class PrintJobAttributeEvent encapsulates an event a PrintService
	''' reports to let the client know that one or more printing attributes for a
	''' PrintJob have changed.
	''' </summary>

	Public Class PrintJobAttributeEvent
		Inherits PrintEvent

		Private Const serialVersionUID As Long = -6534469883874742101L

		Private attributes As javax.print.attribute.PrintJobAttributeSet

		''' <summary>
		''' Constructs a PrintJobAttributeEvent object. </summary>
		''' <param name="source"> the print job generating  this event </param>
		''' <param name="attributes"> the attribute changes being reported </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is
		'''         <code>null</code>. </exception>
		Public Sub New(ByVal source As javax.print.DocPrintJob, ByVal attributes As javax.print.attribute.PrintJobAttributeSet)
			MyBase.New(source)

			Me.attributes = javax.print.attribute.AttributeSetUtilities.unmodifiableView(attributes)
		End Sub


		''' <summary>
		''' Determine the Print Job to which this print job event pertains.
		''' </summary>
		''' <returns>  Print Job object. </returns>
		Public Overridable Property printJob As javax.print.DocPrintJob
			Get
    
				Return CType(source, javax.print.DocPrintJob)
			End Get
		End Property


		''' <summary>
		''' Determine the printing attributes that changed and their new values.
		''' </summary>
		''' <returns>  Attributes containing the new values for the print job
		''' attributes that changed. The returned set may not be modifiable. </returns>
		Public Overridable Property attributes As javax.print.attribute.PrintJobAttributeSet
			Get
    
				Return attributes
    
			End Get
		End Property

	End Class

End Namespace
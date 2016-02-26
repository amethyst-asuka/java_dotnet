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


	''' 
	''' <summary>
	''' Class PrintServiceAttributeEvent encapsulates an event a
	''' Print Service instance reports to let the client know of
	''' changes in the print service state.
	''' </summary>

	Public Class PrintServiceAttributeEvent
		Inherits PrintEvent

		Private Const serialVersionUID As Long = -7565987018140326600L

		Private attributes As javax.print.attribute.PrintServiceAttributeSet

		''' <summary>
		''' Constructs a PrintServiceAttributeEvent object.
		''' </summary>
		''' <param name="source"> the print job generating  this event </param>
		''' <param name="attributes"> the attribute changes being reported </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is
		'''         <code>null</code>. </exception>
		Public Sub New(ByVal source As javax.print.PrintService, ByVal attributes As javax.print.attribute.PrintServiceAttributeSet)

			MyBase.New(source)
			Me.attributes = javax.print.attribute.AttributeSetUtilities.unmodifiableView(attributes)
		End Sub


		''' <summary>
		''' Returns the print service.
		''' </summary>
		''' <returns>  Print Service object. </returns>
		Public Overridable Property printService As javax.print.PrintService
			Get
    
				Return CType(source, javax.print.PrintService)
			End Get
		End Property


		''' <summary>
		''' Determine the printing service attributes that changed and their new
		''' values.
		''' </summary>
		''' <returns>  Attributes containing the new values for the service
		''' attributes that changed. The returned set may be unmodifiable. </returns>
		Public Overridable Property attributes As javax.print.attribute.PrintServiceAttributeSet
			Get
    
				Return attributes
			End Get
		End Property

	End Class

End Namespace
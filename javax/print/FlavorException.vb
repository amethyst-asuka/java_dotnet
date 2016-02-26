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
	''' Interface FlavorException is a mixin interface which a subclass of {@link
	''' PrintException PrintException} can implement to report an error condition
	''' involving a doc flavor or flavors (class {@link javax.print.DocFlavor
	''' DocFlavor}). The Print Service API does not define any print exception
	''' classes that implement interface FlavorException, that being left to the
	''' Print Service implementor's discretion.
	''' 
	''' </summary>
	Public Interface FlavorException

		''' <summary>
		''' Returns the unsupported flavors. </summary>
		''' <returns> the unsupported doc flavors. </returns>
		ReadOnly Property unsupportedFlavors As javax.print.DocFlavor()

	End Interface

End Namespace
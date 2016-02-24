'
' * Copyright (c) 1996, 1997, Oracle and/or its affiliates. All rights reserved.
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
	''' An abstract class which provides a print graphics context for a page.
	''' 
	''' @author      Amy Fowler
	''' </summary>
	Public Interface PrintGraphics

		''' <summary>
		''' Returns the PrintJob object from which this PrintGraphics
		''' object originated.
		''' </summary>
		ReadOnly Property printJob As PrintJob

	End Interface

End Namespace
'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.event


	''' <summary>
	''' An interface used by <code>ImageReader</code> implementations to
	''' notify callers of their image and thumbnail reading methods of
	''' warnings (non-fatal errors).  Fatal errors cause the relevant
	''' read method to throw an <code>IIOException</code>.
	''' 
	''' <p> Localization is handled by associating a <code>Locale</code>
	''' with each <code>IIOReadWarningListener</code> as it is registered
	''' with an <code>ImageReader</code>.  It is up to the
	''' <code>ImageReader</code> to provide localized messages.
	''' </summary>
	''' <seealso cref= javax.imageio.ImageReader#addIIOReadWarningListener </seealso>
	''' <seealso cref= javax.imageio.ImageReader#removeIIOReadWarningListener
	'''  </seealso>
	Public Interface IIOReadWarningListener
		Inherits java.util.EventListener

		''' <summary>
		''' Reports the occurrence of a non-fatal error in decoding.  Decoding
		''' will continue following the call to this method.  The application
		''' may choose to display a dialog, print the warning to the console,
		''' ignore the warning, or take any other action it chooses.
		''' </summary>
		''' <param name="source"> the <code>ImageReader</code> object calling this method. </param>
		''' <param name="warning"> a <code>String</code> containing the warning. </param>
		Sub warningOccurred(ByVal source As javax.imageio.ImageReader, ByVal warning As String)
	End Interface

End Namespace
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

Namespace javax.imageio.event


	''' <summary>
	''' An interface used by <code>ImageWriter</code> implementations to
	''' notify callers of their image and thumbnail reading methods of
	''' warnings (non-fatal errors).  Fatal errors cause the relevant
	''' read method to throw an <code>IIOException</code>.
	''' 
	''' <p> Localization is handled by associating a <code>Locale</code>
	''' with each <code>IIOWriteWarningListener</code> as it is registered
	''' with an <code>ImageWriter</code>.  It is up to the
	''' <code>ImageWriter</code> to provide localized messages.
	''' </summary>
	''' <seealso cref= javax.imageio.ImageWriter#addIIOWriteWarningListener </seealso>
	''' <seealso cref= javax.imageio.ImageWriter#removeIIOWriteWarningListener
	'''  </seealso>
	Public Interface IIOWriteWarningListener
		Inherits java.util.EventListener

		''' <summary>
		''' Reports the occurrence of a non-fatal error in encoding.  Encoding
		''' will continue following the call to this method.  The application
		''' may choose to display a dialog, print the warning to the console,
		''' ignore the warning, or take any other action it chooses.
		''' </summary>
		''' <param name="source"> the <code>ImageWriter</code> object calling this method. </param>
		''' <param name="imageIndex"> the index, starting with 0, of the image
		''' generating the warning. </param>
		''' <param name="warning"> a <code>String</code> containing the warning. </param>
		Sub warningOccurred(ByVal source As javax.imageio.ImageWriter, ByVal imageIndex As Integer, ByVal warning As String)
	End Interface

End Namespace
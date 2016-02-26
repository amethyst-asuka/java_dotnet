'
' * Copyright (c) 2003, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.accessibility


	''' 
	''' <summary>
	''' The <code>AccessibleStreamable</code> interface should be implemented
	''' by the <code>AccessibleContext</code> of any component that presents the
	''' raw stream behind a component on the display screen.  Examples of such
	''' components are HTML, bitmap images and MathML.  An object that implements
	''' <code>AccessibleStreamable</code> provides two things: a list of MIME
	''' types supported by the object and a streaming interface for each MIME type to
	''' get the data.
	''' 
	''' @author Lynn Monsanto
	''' @author Peter Korn
	''' </summary>
	''' <seealso cref= javax.accessibility.AccessibleContext
	''' @since 1.5 </seealso>
	Public Interface AccessibleStreamable
		''' <summary>
		''' Returns an array of DataFlavor objects for the MIME types
		''' this object supports.
		''' </summary>
		''' <returns> an array of DataFlavor objects for the MIME types
		''' this object supports. </returns>
		 ReadOnly Property mimeTypes As java.awt.datatransfer.DataFlavor()

		''' <summary>
		''' Returns an InputStream for a DataFlavor
		''' </summary>
		''' <param name="flavor"> the DataFlavor </param>
		''' <returns> an ImputStream if an ImputStream for this DataFlavor exists.
		''' Otherwise, null is returned. </returns>
		 Function getStream(ByVal flavor As java.awt.datatransfer.DataFlavor) As java.io.InputStream
	End Interface

End Namespace
'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.transform

	''' <summary>
	''' This interface is primarily for the purposes of reporting where
	''' an error occurred in the XML source or transformation instructions.
	''' </summary>
	Public Interface SourceLocator

		''' <summary>
		''' Return the public identifier for the current document event.
		''' 
		''' <p>The return value is the public identifier of the document
		''' entity or of the external parsed entity in which the markup that
		''' triggered the event appears.</p>
		''' </summary>
		''' <returns> A string containing the public identifier, or
		'''         null if none is available. </returns>
		''' <seealso cref= #getSystemId </seealso>
		ReadOnly Property publicId As String

		''' <summary>
		''' Return the system identifier for the current document event.
		''' 
		''' <p>The return value is the system identifier of the document
		''' entity or of the external parsed entity in which the markup that
		''' triggered the event appears.</p>
		''' 
		''' <p>If the system identifier is a URL, the parser must resolve it
		''' fully before passing it to the application.</p>
		''' </summary>
		''' <returns> A string containing the system identifier, or null
		'''         if none is available. </returns>
		''' <seealso cref= #getPublicId </seealso>
		ReadOnly Property systemId As String

		''' <summary>
		''' Return the line number where the current document event ends.
		''' 
		''' <p><strong>Warning:</strong> The return value from the method
		''' is intended only as an approximation for the sake of error
		''' reporting; it is not intended to provide sufficient information
		''' to edit the character content of the original XML document.</p>
		''' 
		''' <p>The return value is an approximation of the line number
		''' in the document entity or external parsed entity where the
		''' markup that triggered the event appears.</p>
		''' </summary>
		''' <returns> The line number, or -1 if none is available. </returns>
		''' <seealso cref= #getColumnNumber </seealso>
		ReadOnly Property lineNumber As Integer

		''' <summary>
		''' Return the character position where the current document event ends.
		''' 
		''' <p><strong>Warning:</strong> The return value from the method
		''' is intended only as an approximation for the sake of error
		''' reporting; it is not intended to provide sufficient information
		''' to edit the character content of the original XML document.</p>
		''' 
		''' <p>The return value is an approximation of the column number
		''' in the document entity or external parsed entity where the
		''' markup that triggered the event appears.</p>
		''' </summary>
		''' <returns> The column number, or -1 if none is available. </returns>
		''' <seealso cref= #getLineNumber </seealso>
		ReadOnly Property columnNumber As Integer
	End Interface

End Namespace
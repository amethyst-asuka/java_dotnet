'
' * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.annotation.processing

	''' <summary>
	''' A suggested <seealso cref="Processor#getCompletions <em>completion</em>"/> for an
	''' annotation.  A completion is text meant to be inserted into a
	''' program as part of an annotation.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
	Public Interface Completion

		''' <summary>
		''' Returns the text of the suggested completion. </summary>
		''' <returns> the text of the suggested completion. </returns>
		ReadOnly Property value As String

		''' <summary>
		''' Returns an informative message about the completion. </summary>
		''' <returns> an informative message about the completion. </returns>
		ReadOnly Property message As String
	End Interface

End Namespace
Imports System

'
' * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.metadata


	''' <summary>
	''' An <code>IIOInvalidTreeException</code> is thrown when an attempt
	''' by an <code>IIOMetadata</code> object to parse a tree of
	''' <code>IIOMetadataNode</code>s fails.  The node that led to the
	''' parsing error may be stored.  As with any parsing error, the actual
	''' error may occur at a different point that that where it is
	''' detected.  The node returned by <code>getOffendingNode</code>
	''' should merely be considered as a clue to the actual nature of the
	''' problem.
	''' </summary>
	''' <seealso cref= IIOMetadata#setFromTree </seealso>
	''' <seealso cref= IIOMetadata#mergeTree </seealso>
	''' <seealso cref= IIOMetadataNode
	'''  </seealso>
	Public Class IIOInvalidTreeException
		Inherits javax.imageio.IIOException

		''' <summary>
		''' The <code>Node</code> that led to the parsing error, or
		''' <code>null</code>.
		''' </summary>
		Protected Friend offendingNode As org.w3c.dom.Node = Nothing

		''' <summary>
		''' Constructs an <code>IIOInvalidTreeException</code> with a
		''' message string and a reference to the <code>Node</code> that
		''' caused the parsing error.
		''' </summary>
		''' <param name="message"> a <code>String</code> containing the reason for
		''' the parsing failure. </param>
		''' <param name="offendingNode"> the DOM <code>Node</code> that caused the
		''' exception, or <code>null</code>. </param>
		Public Sub New(ByVal message As String, ByVal offendingNode As org.w3c.dom.Node)
			MyBase.New(message)
			Me.offendingNode = offendingNode
		End Sub

		''' <summary>
		''' Constructs an <code>IIOInvalidTreeException</code> with a
		''' message string, a reference to an exception that caused this
		''' exception, and a reference to the <code>Node</code> that caused
		''' the parsing error.
		''' </summary>
		''' <param name="message"> a <code>String</code> containing the reason for
		''' the parsing failure. </param>
		''' <param name="cause"> the <code>Throwable</code> (<code>Error</code> or
		''' <code>Exception</code>) that caused this exception to occur,
		''' or <code>null</code>. </param>
		''' <param name="offendingNode"> the DOM <code>Node</code> that caused the
		''' exception, or <code>null</code>. </param>
		Public Sub New(ByVal message As String, ByVal cause As Exception, ByVal offendingNode As org.w3c.dom.Node)
			MyBase.New(message, cause)
			Me.offendingNode = offendingNode
		End Sub

		''' <summary>
		''' Returns the <code>Node</code> that caused the error in parsing.
		''' </summary>
		''' <returns> the offending <code>Node</code>. </returns>
		Public Overridable Property offendingNode As org.w3c.dom.Node
			Get
				Return offendingNode
			End Get
		End Property
	End Class

End Namespace
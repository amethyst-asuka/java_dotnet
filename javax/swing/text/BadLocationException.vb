Imports System

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text

	''' <summary>
	''' This exception is to report bad locations within a document model
	''' (that is, attempts to reference a location that doesn't exist).
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class BadLocationException
		Inherits Exception

		''' <summary>
		''' Creates a new BadLocationException object.
		''' </summary>
		''' <param name="s">         a string indicating what was wrong with the arguments </param>
		''' <param name="offs">      offset within the document that was requested &gt;= 0 </param>
		Public Sub New(ByVal s As String, ByVal offs As Integer)
			MyBase.New(s)
			Me.offs = offs
		End Sub

		''' <summary>
		''' Returns the offset into the document that was not legal.
		''' </summary>
		''' <returns> the offset &gt;= 0 </returns>
		Public Overridable Function offsetRequested() As Integer
			Return offs
		End Function

		Private offs As Integer
	End Class

End Namespace
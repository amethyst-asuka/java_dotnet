'
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

'
' * Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
' 

Namespace javax.xml.stream.util


	''' <summary>
	''' This is the base class for deriving an XMLEventReader
	''' filter.
	''' 
	''' This class is designed to sit between an XMLEventReader and an
	''' application's XMLEventReader.  By default each method
	''' does nothing but call the corresponding method on the
	''' parent interface.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= javax.xml.stream.XMLEventReader </seealso>
	''' <seealso cref= StreamReaderDelegate
	''' @since 1.6 </seealso>

	Public Class EventReaderDelegate
		Implements javax.xml.stream.XMLEventReader

	  Private reader As javax.xml.stream.XMLEventReader

	  ''' <summary>
	  ''' Construct an empty filter with no parent.
	  ''' </summary>
	  Public Sub New()
	  End Sub

	  ''' <summary>
	  ''' Construct an filter with the specified parent. </summary>
	  ''' <param name="reader"> the parent </param>
	  Public Sub New(ByVal reader As javax.xml.stream.XMLEventReader)
		Me.reader = reader
	  End Sub

	  ''' <summary>
	  ''' Set the parent of this instance. </summary>
	  ''' <param name="reader"> the new parent </param>
	  Public Overridable Property parent As javax.xml.stream.XMLEventReader
		  Set(ByVal reader As javax.xml.stream.XMLEventReader)
			Me.reader = reader
		  End Set
		  Get
			Return reader
		  End Get
	  End Property


	  Public Overridable Function nextEvent() As javax.xml.stream.events.XMLEvent
		Return reader.nextEvent()
	  End Function

	  Public Overridable Function [next]() As Object
		Return reader.next()
	  End Function

	  Public Overridable Function hasNext() As Boolean
		Return reader.hasNext()
	  End Function

	  Public Overridable Function peek() As javax.xml.stream.events.XMLEvent
		Return reader.peek()
	  End Function

	  Public Overridable Sub close()
		reader.close()
	  End Sub

	  Public Overridable Property elementText As String
		  Get
			Return reader.elementText
		  End Get
	  End Property

	  Public Overridable Function nextTag() As javax.xml.stream.events.XMLEvent
		Return reader.nextTag()
	  End Function

	  Public Overridable Function getProperty(ByVal name As String) As Object
		Return reader.getProperty(name)
	  End Function

	  Public Overridable Sub remove()
		reader.remove()
	  End Sub
	End Class

End Namespace
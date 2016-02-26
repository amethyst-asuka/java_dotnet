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

Namespace javax.xml.stream.events


	''' <summary>
	''' An interface that contains information about an attribute.  Attributes are reported
	''' as a set of events accessible from a StartElement.  Other applications may report
	''' Attributes as first-order events, for example as the results of an XPath expression.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= StartElement
	''' @since 1.6 </seealso>
	Public Interface Attribute
		Inherits XMLEvent

	  ''' <summary>
	  ''' Returns the QName for this attribute
	  ''' </summary>
	  ReadOnly Property name As javax.xml.namespace.QName

	  ''' <summary>
	  ''' Gets the normalized value of this attribute
	  ''' </summary>
	  ReadOnly Property value As String

	  ''' <summary>
	  ''' Gets the type of this attribute, default is
	  ''' the String "CDATA" </summary>
	  ''' <returns> the type as a String, default is "CDATA" </returns>
	  ReadOnly Property dTDType As String

	  ''' <summary>
	  ''' A flag indicating whether this attribute was actually
	  ''' specified in the start-tag of its element, or was defaulted from the schema. </summary>
	  ''' <returns> returns true if this was specified in the start element </returns>
	  ReadOnly Property specified As Boolean

	End Interface

End Namespace
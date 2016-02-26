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

Namespace javax.xml.stream

	''' <summary>
	''' This interface is used to resolve resources during an XML parse.  If an application wishes to
	''' perform custom entity resolution it must register an instance of this interface with
	''' the XMLInputFactory using the setXMLResolver method.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Interface XMLResolver

	  ''' <summary>
	  ''' Retrieves a resource.  This resource can be of the following three return types:
	  ''' (1) java.io.InputStream (2) javax.xml.stream.XMLStreamReader (3) java.xml.stream.XMLEventReader.
	  ''' If this method returns null the processor will attempt to resolve the entity using its
	  ''' default mechanism.
	  ''' </summary>
	  ''' <param name="publicID"> The public identifier of the external entity being referenced, or null if none was supplied. </param>
	  ''' <param name="systemID"> The system identifier of the external entity being referenced. </param>
	  ''' <param name="baseURI">  Absolute base URI associated with systemId. </param>
	  ''' <param name="namespace"> The namespace of the entity to resolve. </param>
	  ''' <returns> The resource requested or null. </returns>
	  ''' <exception cref="XMLStreamException"> if there was a failure attempting to resolve the resource. </exception>
	  Function resolveEntity(ByVal publicID As String, ByVal systemID As String, ByVal baseURI As String, ByVal [namespace] As String) As Object
	End Interface

End Namespace
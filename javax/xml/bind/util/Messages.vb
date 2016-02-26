'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind.util


	''' <summary>
	''' Formats error messages.
	''' </summary>
	Friend Class Messages
		Friend Shared Function format(ByVal [property] As String) As String
			Return format([property], Nothing)
		End Function

		Friend Shared Function format(ByVal [property] As String, ByVal arg1 As Object) As String
			Return format([property], New Object(){arg1})
		End Function

		Friend Shared Function format(ByVal [property] As String, ByVal arg1 As Object, ByVal arg2 As Object) As String
			Return format([property], New Object(){arg1,arg2})
		End Function

		Friend Shared Function format(ByVal [property] As String, ByVal arg1 As Object, ByVal arg2 As Object, ByVal arg3 As Object) As String
			Return format([property], New Object(){arg1,arg2,arg3})
		End Function

		' add more if necessary.

		''' <summary>
		''' Loads a string resource and formats it with specified arguments. </summary>
		Friend Shared Function format(ByVal [property] As String, ByVal args As Object()) As String
			Dim text As String = java.util.ResourceBundle.getBundle(GetType(Messages).name).getString([property])
			Return java.text.MessageFormat.format(text,args)
		End Function

	'
	'
	' Message resources
	'
	'
		Friend Const UNRECOGNIZED_SEVERITY As String = "ValidationEventCollector.UnrecognizedSeverity" ' 1 arg

		Friend Const RESULT_NULL_CONTEXT As String = "JAXBResult.NullContext" ' 0 args

		Friend Const RESULT_NULL_UNMARSHALLER As String = "JAXBResult.NullUnmarshaller" ' 0 arg

		Friend Const SOURCE_NULL_CONTEXT As String = "JAXBSource.NullContext" ' 0 args

		Friend Const SOURCE_NULL_CONTENT As String = "JAXBSource.NullContent" ' 0 arg

		Friend Const SOURCE_NULL_MARSHALLER As String = "JAXBSource.NullMarshaller" ' 0 arg

	End Class

End Namespace
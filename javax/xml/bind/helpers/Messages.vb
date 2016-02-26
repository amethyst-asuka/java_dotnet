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

Namespace javax.xml.bind.helpers


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
		Friend Const INPUTSTREAM_NOT_NULL As String = "AbstractUnmarshallerImpl.ISNotNull" ' 0 args

		Friend Const MUST_BE_BOOLEAN As String = "AbstractMarshallerImpl.MustBeBoolean" ' 1 arg

		Friend Const MUST_BE_STRING As String = "AbstractMarshallerImpl.MustBeString" ' 1 arg

		Friend Const SEVERITY_MESSAGE As String = "DefaultValidationEventHandler.SeverityMessage" ' 3 args

		Friend Const LOCATION_UNAVAILABLE As String = "DefaultValidationEventHandler.LocationUnavailable" ' 0 args

		Friend Const UNRECOGNIZED_SEVERITY As String = "DefaultValidationEventHandler.UnrecognizedSeverity" ' 1 arg

		Friend Const WARNING As String = "DefaultValidationEventHandler.Warning" ' 0 args

		Friend Const [ERROR] As String = "DefaultValidationEventHandler.Error" ' 0 args

		Friend Const FATAL_ERROR As String = "DefaultValidationEventHandler.FatalError" ' 0 args

		Friend Const ILLEGAL_SEVERITY As String = "ValidationEventImpl.IllegalSeverity" ' 0 args

		Friend Const MUST_NOT_BE_NULL As String = "Shared.MustNotBeNull" ' 1 arg
	End Class

End Namespace
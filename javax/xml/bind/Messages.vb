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

Namespace javax.xml.bind


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
		Friend Const PROVIDER_NOT_FOUND As String = "ContextFinder.ProviderNotFound" ' 1 arg

		Friend Const COULD_NOT_INSTANTIATE As String = "ContextFinder.CouldNotInstantiate" ' 2 args

		Friend Const CANT_FIND_PROPERTIES_FILE As String = "ContextFinder.CantFindPropertiesFile" ' 1 arg

		Friend Const CANT_MIX_PROVIDERS As String = "ContextFinder.CantMixProviders" ' 0 args

		Friend Const MISSING_PROPERTY As String = "ContextFinder.MissingProperty" ' 2 args

		Friend Const NO_PACKAGE_IN_CONTEXTPATH As String = "ContextFinder.NoPackageInContextPath" ' 0 args

		Friend Const NAME_VALUE As String = "PropertyException.NameValue" ' 2 args

		Friend Const CONVERTER_MUST_NOT_BE_NULL As String = "DatatypeConverter.ConverterMustNotBeNull" ' 0 args

		Friend Const ILLEGAL_CAST As String = "JAXBContext.IllegalCast" ' 2 args
	End Class

End Namespace
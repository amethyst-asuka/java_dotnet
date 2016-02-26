'
' * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.print.attribute

	''' <summary>
	''' Interface SupportedValuesAttribute is a tagging interface which a printing
	''' attribute class implements to indicate the attribute describes the supported
	''' values for another attribute. For example, if a Print Service instance
	''' supports the <seealso cref="javax.print.attribute.standard.Copies Copies"/>
	''' attribute, the Print Service instance will have a {@link
	''' javax.print.attribute.standard.CopiesSupported CopiesSupported} attribute,
	''' which is a SupportedValuesAttribute giving the legal values a client may
	''' specify for the <seealso cref="javax.print.attribute.standard.Copies Copies"/>
	''' attribute.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public Interface SupportedValuesAttribute
		Inherits Attribute

	End Interface

End Namespace
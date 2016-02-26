'
' * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind.annotation.adapters


	''' <summary>
	''' <seealso cref="XmlAdapter"/> for <tt>xs:hexBinary</tt>.
	''' 
	''' <p>
	''' This <seealso cref="XmlAdapter"/> binds <tt>byte[]</tt> to the hexBinary representation in XML.
	''' 
	''' @author Kohsuke Kawaguchi
	''' @since JAXB 2.0
	''' </summary>
	Public NotInheritable Class HexBinaryAdapter
		Inherits XmlAdapter(Of String, byte())

		Public Function unmarshal(ByVal s As String) As SByte()
			If s Is Nothing Then Return Nothing
			Return javax.xml.bind.DatatypeConverter.parseHexBinary(s)
		End Function

		Public Function marshal(ByVal bytes As SByte()) As String
			If bytes Is Nothing Then Return Nothing
			Return javax.xml.bind.DatatypeConverter.printHexBinary(bytes)
		End Function
	End Class

End Namespace
'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.transform

	''' <summary>
	''' <p>An object that implements this interface that can be called by the processor
	''' to turn a URI used in document(), xsl:import, or xsl:include into a Source object.
	''' </summary>
	Public Interface URIResolver

		''' <summary>
		''' Called by the processor when it encounters
		''' an xsl:include, xsl:import, or document() function.
		''' </summary>
		''' <param name="href"> An href attribute, which may be relative or absolute. </param>
		''' <param name="base"> The base URI against which the first argument will be made
		''' absolute if the absolute URI is required.
		''' </param>
		''' <returns> A Source object, or null if the href cannot be resolved,
		''' and the processor should try to resolve the URI itself.
		''' </returns>
		''' <exception cref="TransformerException"> if an error occurs when trying to
		''' resolve the URI. </exception>
		Function resolve(ByVal href As String, ByVal base As String) As Source
	End Interface

End Namespace
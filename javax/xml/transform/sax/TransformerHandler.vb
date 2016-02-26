'
' * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.transform.sax



	''' <summary>
	''' A TransformerHandler
	''' listens for SAX ContentHandler parse events and transforms
	''' them to a Result.
	''' </summary>
	Public Interface TransformerHandler
		Inherits org.xml.sax.ContentHandler, org.xml.sax.ext.LexicalHandler, org.xml.sax.DTDHandler

		''' <summary>
		''' <p>Set  the <code>Result</code> associated with this
		''' <code>TransformerHandler</code> to be used for the transformation.</p>
		''' </summary>
		''' <param name="result"> A <code>Result</code> instance, should not be
		'''   <code>null</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if result is invalid for some reason. </exception>
		WriteOnly Property result As javax.xml.transform.Result

		''' <summary>
		''' Set the base ID (URI or system ID) from where relative
		''' URLs will be resolved. </summary>
		''' <param name="systemID"> Base URI for the source tree. </param>
		Property systemId As String


		''' <summary>
		''' <p>Get the <code>Transformer</code> associated with this handler, which
		''' is needed in order to set parameters and output properties.</p>
		''' </summary>
		''' <returns> <code>Transformer</code> associated with this
		'''   <code>TransformerHandler</code>. </returns>
		ReadOnly Property transformer As javax.xml.transform.Transformer
	End Interface

End Namespace
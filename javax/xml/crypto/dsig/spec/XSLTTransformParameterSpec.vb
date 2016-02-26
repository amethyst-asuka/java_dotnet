'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
' * $Id: XSLTTransformParameterSpec.java,v 1.4 2005/05/10 16:40:18 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.spec


	''' <summary>
	''' Parameters for the <a href="http://www.w3.org/TR/1999/REC-xslt-19991116">
	''' XSLT Transform Algorithm</a>.
	''' The parameters include a namespace-qualified stylesheet element.
	''' 
	''' <p>An <code>XSLTTransformParameterSpec</code> is instantiated with a
	''' mechanism-dependent (ex: DOM) stylesheet element. For example:
	''' <pre>
	'''   DOMStructure stylesheet = new DOMStructure(element)
	'''   XSLTTransformParameterSpec spec = new XSLTransformParameterSpec(stylesheet);
	''' </pre>
	''' where <code>element</code> is an <seealso cref="org.w3c.dom.Element"/> containing
	''' the namespace-qualified stylesheet element.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= Transform </seealso>
	Public NotInheritable Class XSLTTransformParameterSpec
		Implements TransformParameterSpec

		Private stylesheet As javax.xml.crypto.XMLStructure

		''' <summary>
		''' Creates an <code>XSLTTransformParameterSpec</code> with the specified
		''' stylesheet.
		''' </summary>
		''' <param name="stylesheet"> the XSLT stylesheet to be used </param>
		''' <exception cref="NullPointerException"> if <code>stylesheet</code> is
		'''    <code>null</code> </exception>
		Public Sub New(ByVal stylesheet As javax.xml.crypto.XMLStructure)
			If stylesheet Is Nothing Then Throw New NullPointerException
			Me.stylesheet = stylesheet
		End Sub

		''' <summary>
		''' Returns the stylesheet.
		''' </summary>
		''' <returns> the stylesheet </returns>
		Public Property stylesheet As javax.xml.crypto.XMLStructure
			Get
				Return stylesheet
			End Get
		End Property
	End Class

End Namespace
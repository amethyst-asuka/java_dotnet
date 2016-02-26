'
' * Copyright (c) 2004, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.validation

	''' <summary>
	''' <p>Factory that creates <seealso cref="SchemaFactory"/>.</p>
	''' 
	''' <p><b>DO NOT USE THIS CLASS</b></p>
	''' 
	''' <p>
	''' This class was introduced as a part of an early proposal during the
	''' JSR-206 standardization process. The proposal was eventually abandoned
	''' but this class accidentally remained in the source tree, and made its
	''' way into the final version.
	''' </p><p>
	''' This class does not participate in any JAXP 1.3 or JAXP 1.4 processing.
	''' It must not be used by users or JAXP implementations.
	''' </p>
	''' 
	''' @author <a href="Kohsuke.Kawaguchi@Sun.com">Kohsuke Kawaguchi</a>
	''' @since 1.5
	''' </summary>
	Public MustInherit Class SchemaFactoryLoader

		''' <summary>
		''' A do-nothing constructor.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Creates a new <seealso cref="SchemaFactory"/> object for the specified
		''' schema language.
		''' </summary>
		''' <param name="schemaLanguage">
		'''      See <a href="SchemaFactory.html#schemaLanguage">
		'''      the list of available schema languages</a>.
		''' </param>
		''' <exception cref="NullPointerException">
		'''      If the <tt>schemaLanguage</tt> parameter is null.
		''' </exception>
		''' <returns> <code>null</code> if the callee fails to create one. </returns>
		Public MustOverride Function newFactory(ByVal schemaLanguage As String) As SchemaFactory
	End Class

End Namespace
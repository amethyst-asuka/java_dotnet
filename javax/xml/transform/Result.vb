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
	''' <p>An object that implements this interface contains the information
	''' needed to build a transformation result tree.</p>
	''' 
	''' @author <a href="Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' </summary>
	Public Interface Result

		''' <summary>
		''' The name of the processing instruction that is sent if the
		''' result tree disables output escaping.
		''' 
		''' <p>Normally, result tree serialization escapes & and < (and
		''' possibly other characters) when outputting text nodes.
		''' This ensures that the output is well-formed XML. However,
		''' it is sometimes convenient to be able to produce output that is
		''' almost, but not quite well-formed XML; for example,
		''' the output may include ill-formed sections that will
		''' be transformed into well-formed XML by a subsequent non-XML aware
		''' process. If a processing instruction is sent with this name,
		''' serialization should be output without any escaping. </p>
		''' 
		''' <p>Result DOM trees may also have PI_DISABLE_OUTPUT_ESCAPING and
		''' PI_ENABLE_OUTPUT_ESCAPING inserted into the tree.</p>
		''' </summary>
		''' <seealso cref= <a href="http://www.w3.org/TR/xslt#disable-output-escaping">disable-output-escaping in XSLT Specification</a> </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String PI_DISABLE_OUTPUT_ESCAPING = "javax.xml.transform.disable-output-escaping";

		''' <summary>
		''' The name of the processing instruction that is sent
		''' if the result tree enables output escaping at some point after having
		''' received a PI_DISABLE_OUTPUT_ESCAPING processing instruction.
		''' </summary>
		''' <seealso cref= <a href="http://www.w3.org/TR/xslt#disable-output-escaping">disable-output-escaping in XSLT Specification</a> </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String PI_ENABLE_OUTPUT_ESCAPING = "javax.xml.transform.enable-output-escaping";

		''' <summary>
		''' Set the system identifier for this Result.
		''' 
		''' <p>If the Result is not to be written to a file, the system identifier is optional.
		''' The application may still want to provide one, however, for use in error messages
		''' and warnings, or to resolve relative output identifiers.</p>
		''' </summary>
		''' <param name="systemId"> The system identifier as a URI string. </param>
		Property systemId As String

	End Interface

End Namespace
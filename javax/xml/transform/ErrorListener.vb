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
	''' <p>To provide customized error handling, implement this interface and
	''' use the <code>setErrorListener</code> method to register an instance of the
	''' implmentation with the <seealso cref="javax.xml.transform.Transformer"/>.  The
	''' <code>Transformer</code> then reports all errors and warnings through this
	''' interface.</p>
	''' 
	''' <p>If an application does <em>not</em> register its own custom
	''' <code>ErrorListener</code>, the default <code>ErrorListener</code>
	''' is used which reports all warnings and errors to <code>System.err</code>
	''' and does not throw any <code>Exception</code>s.
	''' Applications are <em>strongly</em> encouraged to register and use
	''' <code>ErrorListener</code>s that insure proper behavior for warnings and
	''' errors.</p>
	''' 
	''' <p>For transformation errors, a <code>Transformer</code> must use this
	''' interface instead of throwing an <code>Exception</code>: it is up to the
	''' application to decide whether to throw an <code>Exception</code> for
	''' different types of errors and warnings.  Note however that the
	''' <code>Transformer</code> is not required to continue with the transformation
	''' after a call to <seealso cref="#fatalError(TransformerException exception)"/>.</p>
	''' 
	''' <p><code>Transformer</code>s may use this mechanism to report XML parsing
	''' errors as well as transformation errors.</p>
	''' </summary>
	Public Interface ErrorListener

		''' <summary>
		''' Receive notification of a warning.
		''' 
		''' <p><seealso cref="javax.xml.transform.Transformer"/> can use this method to report
		''' conditions that are not errors or fatal errors.  The default behaviour
		''' is to take no action.</p>
		''' 
		''' <p>After invoking this method, the Transformer must continue with
		''' the transformation. It should still be possible for the
		''' application to process the document through to the end.</p>
		''' </summary>
		''' <param name="exception"> The warning information encapsulated in a
		'''                  transformer exception.
		''' </param>
		''' <exception cref="javax.xml.transform.TransformerException"> if the application
		''' chooses to discontinue the transformation.
		''' </exception>
		''' <seealso cref= javax.xml.transform.TransformerException </seealso>
		Sub warning(ByVal exception As TransformerException)

		''' <summary>
		''' Receive notification of a recoverable error.
		''' 
		''' <p>The transformer must continue to try and provide normal transformation
		''' after invoking this method.  It should still be possible for the
		''' application to process the document through to the end if no other errors
		''' are encountered.</p>
		''' </summary>
		''' <param name="exception"> The error information encapsulated in a
		'''                  transformer exception.
		''' </param>
		''' <exception cref="javax.xml.transform.TransformerException"> if the application
		''' chooses to discontinue the transformation.
		''' </exception>
		''' <seealso cref= javax.xml.transform.TransformerException </seealso>
		Sub [error](ByVal exception As TransformerException)

		''' <summary>
		''' <p>Receive notification of a non-recoverable error.</p>
		''' 
		''' <p>The processor may choose to continue, but will not normally
		''' proceed to a successful completion.</p>
		''' 
		''' <p>The method should throw an exception if it is unable to
		''' process the error, or if it wishes execution to terminate
		''' immediately. The processor will not necessarily honor this
		''' request.</p>
		''' </summary>
		''' <param name="exception"> The error information encapsulated in a
		'''    <code>TransformerException</code>.
		''' </param>
		''' <exception cref="javax.xml.transform.TransformerException"> if the application
		''' chooses to discontinue the transformation.
		''' </exception>
		''' <seealso cref= javax.xml.transform.TransformerException </seealso>
		Sub fatalError(ByVal exception As TransformerException)
	End Interface

End Namespace
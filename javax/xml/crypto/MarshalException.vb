Imports System

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
' * $Id: MarshalException.java,v 1.5 2005/05/10 15:47:42 mullan Exp $
' 
Namespace javax.xml.crypto


	''' <summary>
	''' Indicates an exceptional condition that occurred during the XML
	''' marshalling or unmarshalling process.
	''' 
	''' <p>A <code>MarshalException</code> can contain a cause: another
	''' throwable that caused this <code>MarshalException</code> to get thrown.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= XMLSignature#sign(XMLSignContext) </seealso>
	''' <seealso cref= XMLSignatureFactory#unmarshalXMLSignature(XMLValidateContext) </seealso>
	Public Class MarshalException
		Inherits Exception

		Private Const serialVersionUID As Long = -863185580332643547L

		''' <summary>
		''' The throwable that caused this exception to get thrown, or null if this
		''' exception was not caused by another throwable or if the causative
		''' throwable is unknown.
		''' 
		''' @serial
		''' </summary>
		Private cause As Exception

		''' <summary>
		''' Constructs a new <code>MarshalException</code> with
		''' <code>null</code> as its detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new <code>MarshalException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="message"> the detail message </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs a new <code>MarshalException</code> with the
		''' specified detail message and cause.
		''' <p>Note that the detail message associated with
		''' <code>cause</code> is <i>not</i> automatically incorporated in
		''' this exception's detail message.
		''' </summary>
		''' <param name="message"> the detail message </param>
		''' <param name="cause"> the cause (A <tt>null</tt> value is permitted, and
		'''        indicates that the cause is nonexistent or unknown.) </param>
		Public Sub New(ByVal message As String, ByVal cause As Exception)
			MyBase.New(message)
			Me.cause = cause
		End Sub

		''' <summary>
		''' Constructs a new <code>MarshalException</code> with the specified cause
		''' and a detail message of <code>(cause==null ? null : cause.toString())
		''' </code> (which typically contains the class and detail message of
		''' <code>cause</code>).
		''' </summary>
		''' <param name="cause"> the cause (A <tt>null</tt> value is permitted, and
		'''        indicates that the cause is nonexistent or unknown.) </param>
		Public Sub New(ByVal cause As Exception)
			MyBase.New(If(cause Is Nothing, Nothing, cause.ToString()))
			Me.cause = cause
		End Sub

		''' <summary>
		''' Returns the cause of this <code>MarshalException</code> or
		''' <code>null</code> if the cause is nonexistent or unknown.  (The
		''' cause is the throwable that caused this
		''' <code>MarshalException</code> to get thrown.)
		''' </summary>
		''' <returns> the cause of this <code>MarshalException</code> or
		'''         <code>null</code> if the cause is nonexistent or unknown. </returns>
		Public Overridable Property cause As Exception
			Get
				Return cause
			End Get
		End Property

		''' <summary>
		''' Prints this <code>MarshalException</code>, its backtrace and
		''' the cause's backtrace to the standard error stream.
		''' </summary>
		Public Overridable Sub printStackTrace()
			MyBase.printStackTrace()
			'XXX print backtrace of cause
		End Sub

		''' <summary>
		''' Prints this <code>MarshalException</code>, its backtrace and
		''' the cause's backtrace to the specified print stream.
		''' </summary>
		''' <param name="s"> <code>PrintStream</code> to use for output </param>
		Public Overridable Sub printStackTrace(ByVal s As java.io.PrintStream)
			MyBase.printStackTrace(s)
			'XXX print backtrace of cause
		End Sub

		''' <summary>
		''' Prints this <code>MarshalException</code>, its backtrace and
		''' the cause's backtrace to the specified print writer.
		''' </summary>
		''' <param name="s"> <code>PrintWriter</code> to use for output </param>
		Public Overridable Sub printStackTrace(ByVal s As java.io.PrintWriter)
			MyBase.printStackTrace(s)
			'XXX print backtrace of cause
		End Sub
	End Class

End Namespace
Imports System

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

Namespace javax.xml.xpath


	''' <summary>
	''' <code>XPathException</code> represents a generic XPath exception.</p>
	''' 
	''' @author  <a href="Norman.Walsh@Sun.com">Norman Walsh</a>
	''' @author <a href="mailto:Jeff.Suttor@Sun.COM">Jeff Suttor</a>
	''' @since 1.5
	''' </summary>
	Public Class XPathException
		Inherits Exception

		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("cause", GetType(Exception)) }

		''' <summary>
		''' <p>Stream Unique Identifier.</p>
		''' </summary>
		Private Const serialVersionUID As Long = -1837080260374986980L

		''' <summary>
		''' <p>Constructs a new <code>XPathException</code>
		''' with the specified detail <code>message</code>.</p>
		''' 
		''' <p>The <code>cause</code> is not initialized.</p>
		''' 
		''' <p>If <code>message</code> is <code>null</code>,
		''' then a <code>NullPointerException</code> is thrown.</p>
		''' </summary>
		''' <param name="message"> The detail message.
		''' </param>
		''' <exception cref="NullPointerException"> When <code>message</code> is
		'''   <code>null</code>. </exception>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
			If message Is Nothing Then Throw New NullPointerException("message can't be null")
		End Sub

		''' <summary>
		''' <p>Constructs a new <code>XPathException</code>
		''' with the specified <code>cause</code>.</p>
		''' 
		''' <p>If <code>cause</code> is <code>null</code>,
		''' then a <code>NullPointerException</code> is thrown.</p>
		''' </summary>
		''' <param name="cause"> The cause.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>cause</code> is <code>null</code>. </exception>
		Public Sub New(ByVal cause As Exception)
			MyBase.New(cause)
			If cause Is Nothing Then Throw New NullPointerException("cause can't be null")
		End Sub

		''' <summary>
		''' <p>Get the cause of this XPathException.</p>
		''' </summary>
		''' <returns> Cause of this XPathException. </returns>
		Public Overridable Property cause As Exception
			Get
				Return MyBase.cause
			End Get
		End Property

		''' <summary>
		''' Writes "cause" field to the stream.
		''' The cause is got from the parent class.
		''' </summary>
		''' <param name="out"> stream used for serialization. </param>
		''' <exception cref="IOException"> thrown by <code>ObjectOutputStream</code>
		'''  </exception>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("cause", CType(MyBase.cause, Exception))
			out.writeFields()
		End Sub

		''' <summary>
		''' Reads the "cause" field from the stream.
		''' And initializes the "cause" if it wasn't
		''' done before.
		''' </summary>
		''' <param name="in"> stream used for deserialization </param>
		''' <exception cref="IOException"> thrown by <code>ObjectInputStream</code> </exception>
		''' <exception cref="ClassNotFoundException">  thrown by <code>ObjectInputStream</code> </exception>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
			Dim scause As Exception = CType(fields.get("cause", Nothing), Exception)

			If MyBase.cause Is Nothing AndAlso scause IsNot Nothing Then
				Try
					MyBase.initCause(scause)
				Catch e As IllegalStateException
					Throw New java.io.InvalidClassException("Inconsistent state: two causes")
				End Try
			End If
		End Sub

		''' <summary>
		''' <p>Print stack trace to specified <code>PrintStream</code>.</p>
		''' </summary>
		''' <param name="s"> Print stack trace to this <code>PrintStream</code>. </param>
		Public Overridable Sub printStackTrace(ByVal s As java.io.PrintStream)
			If cause IsNot Nothing Then
				cause.printStackTrace(s)
			  s.println("--------------- linked to ------------------")
			End If

			MyBase.printStackTrace(s)
		End Sub

		''' <summary>
		''' <p>Print stack trace to <code>System.err</code>.</p>
		''' </summary>
		Public Overridable Sub printStackTrace()
			printStackTrace(System.err)
		End Sub

		''' <summary>
		''' <p>Print stack trace to specified <code>PrintWriter</code>.</p>
		''' </summary>
		''' <param name="s"> Print stack trace to this <code>PrintWriter</code>. </param>
		Public Overridable Sub printStackTrace(ByVal s As java.io.PrintWriter)

			If cause IsNot Nothing Then
				cause.printStackTrace(s)
			  s.println("--------------- linked to ------------------")
			End If

			MyBase.printStackTrace(s)
		End Sub
	End Class

End Namespace
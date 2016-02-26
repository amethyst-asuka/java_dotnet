Imports System
Imports System.Runtime.CompilerServices
Imports System.Text

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

Namespace javax.xml.transform


	''' <summary>
	''' This class specifies an exceptional condition that occured
	''' during the transformation process.
	''' </summary>
	Public Class TransformerException
		Inherits Exception

		''' <summary>
		''' Field locator specifies where the error occured </summary>
		Friend locator As SourceLocator

		''' <summary>
		''' Method getLocator retrieves an instance of a SourceLocator
		''' object that specifies where an error occured.
		''' </summary>
		''' <returns> A SourceLocator object, or null if none was specified. </returns>
		Public Overridable Property locator As SourceLocator
			Get
				Return locator
			End Get
			Set(ByVal location As SourceLocator)
				locator = location
			End Set
		End Property


		''' <summary>
		''' Field containedException specifies a wrapped exception.  May be null. </summary>
		Friend containedException As Exception

		''' <summary>
		''' This method retrieves an exception that this exception wraps.
		''' </summary>
		''' <returns> An Throwable object, or null. </returns>
		''' <seealso cref= #getCause </seealso>
		Public Overridable Property exception As Exception
			Get
				Return containedException
			End Get
		End Property

		''' <summary>
		''' Returns the cause of this throwable or <code>null</code> if the
		''' cause is nonexistent or unknown.  (The cause is the throwable that
		''' caused this throwable to get thrown.)
		''' </summary>
		Public Overridable Property cause As Exception
			Get
    
				Return (If(containedException Is Me, Nothing, containedException))
			End Get
		End Property

		''' <summary>
		''' Initializes the <i>cause</i> of this throwable to the specified value.
		''' (The cause is the throwable that caused this throwable to get thrown.)
		''' 
		''' <p>This method can be called at most once.  It is generally called from
		''' within the constructor, or immediately after creating the
		''' throwable.  If this throwable was created
		''' with <seealso cref="#TransformerException(Throwable)"/> or
		''' <seealso cref="#TransformerException(String,Throwable)"/>, this method cannot be called
		''' even once.
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method).  (A <code>null</code> value is
		'''         permitted, and indicates that the cause is nonexistent or
		'''         unknown.) </param>
		''' <returns>  a reference to this <code>Throwable</code> instance. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>cause</code> is this
		'''         throwable.  (A throwable cannot
		'''         be its own cause.) </exception>
		''' <exception cref="IllegalStateException"> if this throwable was
		'''         created with <seealso cref="#TransformerException(Throwable)"/> or
		'''         <seealso cref="#TransformerException(String,Throwable)"/>, or this method has already
		'''         been called on this throwable. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function initCause(ByVal cause As Exception) As Exception

			If Me.containedException IsNot Nothing Then Throw New IllegalStateException("Can't overwrite cause")

			If cause Is Me Then Throw New System.ArgumentException("Self-causation not permitted")

			Me.containedException = cause

			Return Me
		End Function

		''' <summary>
		''' Create a new TransformerException.
		''' </summary>
		''' <param name="message"> The error or warning message. </param>
		Public Sub New(ByVal message As String)

			MyBase.New(message)

			Me.containedException = Nothing
			Me.locator = Nothing
		End Sub

		''' <summary>
		''' Create a new TransformerException wrapping an existing exception.
		''' </summary>
		''' <param name="e"> The exception to be wrapped. </param>
		Public Sub New(ByVal e As Exception)

			MyBase.New(e.ToString())

			Me.containedException = e
			Me.locator = Nothing
		End Sub

		''' <summary>
		''' Wrap an existing exception in a TransformerException.
		''' 
		''' <p>This is used for throwing processor exceptions before
		''' the processing has started.</p>
		''' </summary>
		''' <param name="message"> The error or warning message, or null to
		'''                use the message from the embedded exception. </param>
		''' <param name="e"> Any exception </param>
		Public Sub New(ByVal message As String, ByVal e As Exception)

			MyBase.New(If((message Is Nothing) OrElse (message.Length = 0), e.ToString(), message))

			Me.containedException = e
			Me.locator = Nothing
		End Sub

		''' <summary>
		''' Create a new TransformerException from a message and a Locator.
		''' 
		''' <p>This constructor is especially useful when an application is
		''' creating its own exception from within a DocumentHandler
		''' callback.</p>
		''' </summary>
		''' <param name="message"> The error or warning message. </param>
		''' <param name="locator"> The locator object for the error or warning. </param>
		Public Sub New(ByVal message As String, ByVal locator As SourceLocator)

			MyBase.New(message)

			Me.containedException = Nothing
			Me.locator = locator
		End Sub

		''' <summary>
		''' Wrap an existing exception in a TransformerException.
		''' </summary>
		''' <param name="message"> The error or warning message, or null to
		'''                use the message from the embedded exception. </param>
		''' <param name="locator"> The locator object for the error or warning. </param>
		''' <param name="e"> Any exception </param>
		Public Sub New(ByVal message As String, ByVal locator As SourceLocator, ByVal e As Exception)

			MyBase.New(message)

			Me.containedException = e
			Me.locator = locator
		End Sub

		''' <summary>
		''' Get the error message with location information
		''' appended.
		''' </summary>
		''' <returns> A <code>String</code> representing the error message with
		'''         location information appended. </returns>
		Public Overridable Property messageAndLocation As String
			Get
    
				Dim sbuffer As New StringBuilder
				Dim message As String = MyBase.message
    
				If Nothing IsNot message Then sbuffer.Append(message)
    
				If Nothing IsNot locator Then
					Dim systemID As String = locator.systemId
					Dim line As Integer = locator.lineNumber
					Dim column As Integer = locator.columnNumber
    
					If Nothing IsNot systemID Then
						sbuffer.Append("; SystemID: ")
						sbuffer.Append(systemID)
					End If
    
					If 0 <> line Then
						sbuffer.Append("; Line#: ")
						sbuffer.Append(line)
					End If
    
					If 0 <> column Then
						sbuffer.Append("; Column#: ")
						sbuffer.Append(column)
					End If
				End If
    
				Return sbuffer.ToString()
			End Get
		End Property

		''' <summary>
		''' Get the location information as a string.
		''' </summary>
		''' <returns> A string with location info, or null
		''' if there is no location information. </returns>
		Public Overridable Property locationAsString As String
			Get
    
				If Nothing IsNot locator Then
					Dim sbuffer As New StringBuilder
					Dim systemID As String = locator.systemId
					Dim line As Integer = locator.lineNumber
					Dim column As Integer = locator.columnNumber
    
					If Nothing IsNot systemID Then
						sbuffer.Append("; SystemID: ")
						sbuffer.Append(systemID)
					End If
    
					If 0 <> line Then
						sbuffer.Append("; Line#: ")
						sbuffer.Append(line)
					End If
    
					If 0 <> column Then
						sbuffer.Append("; Column#: ")
						sbuffer.Append(column)
					End If
    
					Return sbuffer.ToString()
				Else
					Return Nothing
				End If
			End Get
		End Property

		''' <summary>
		''' Print the the trace of methods from where the error
		''' originated.  This will trace all nested exception
		''' objects, as well as this object.
		''' </summary>
		Public Overridable Sub printStackTrace()
			printStackTrace(New java.io.PrintWriter(System.err, True))
		End Sub

		''' <summary>
		''' Print the the trace of methods from where the error
		''' originated.  This will trace all nested exception
		''' objects, as well as this object. </summary>
		''' <param name="s"> The stream where the dump will be sent to. </param>
		Public Overridable Sub printStackTrace(ByVal s As java.io.PrintStream)
			printStackTrace(New java.io.PrintWriter(s))
		End Sub

		''' <summary>
		''' Print the the trace of methods from where the error
		''' originated.  This will trace all nested exception
		''' objects, as well as this object. </summary>
		''' <param name="s"> The writer where the dump will be sent to. </param>
		Public Overridable Sub printStackTrace(ByVal s As java.io.PrintWriter)

			If s Is Nothing Then s = New java.io.PrintWriter(System.err, True)

			Try
				Dim locInfo As String = locationAsString

				If Nothing IsNot locInfo Then s.println(locInfo)

				MyBase.printStackTrace(s)
			Catch e As Exception
			End Try

			Dim ___exception As Exception = exception

			Dim i As Integer = 0
			Do While (i < 10) AndAlso (Nothing IsNot ___exception)
				s.println("---------")

				Try
					If TypeOf ___exception Is TransformerException Then
						Dim locInfo As String = CType(___exception, TransformerException).locationAsString

						If Nothing IsNot locInfo Then s.println(locInfo)
					End If

					___exception.printStackTrace(s)
				Catch e As Exception
					s.println("Could not print stack trace...")
				End Try

				Try
					Dim meth As Method = CObj(___exception).GetType().GetMethod("getException", CType(Nothing, Type()))

					If Nothing IsNot meth Then
						Dim prev As Exception = ___exception

						___exception = CType(meth.invoke(___exception, CType(Nothing, Object())), Exception)

						If prev Is ___exception Then Exit Do
					Else
						___exception = Nothing
					End If
				Catch ite As InvocationTargetException
					___exception = Nothing
				Catch iae As IllegalAccessException
					___exception = Nothing
				Catch nsme As NoSuchMethodException
					___exception = Nothing
				End Try
				i += 1
			Loop
			' insure output is written
			s.flush()
		End Sub
	End Class

End Namespace
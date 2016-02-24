'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
' *
' *
' *
' *
' *
' * Copyright (c) 2008-2012, Stephen Colebourne & Michael Nascimento Santos
' *
' * All rights reserved.
' *
' * Redistribution and use in source and binary forms, with or without
' * modification, are permitted provided that the following conditions are met:
' *
' *  * Redistributions of source code must retain the above copyright notice,
' *    this list of conditions and the following disclaimer.
' *
' *  * Redistributions in binary form must reproduce the above copyright notice,
' *    this list of conditions and the following disclaimer in the documentation
' *    and/or other materials provided with the distribution.
' *
' *  * Neither the name of JSR-310 nor the names of its contributors
' *    may be used to endorse or promote products derived from this software
' *    without specific prior written permission.
' *
' * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
' * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
' * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
' * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
' * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
' * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
' * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
' * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
' * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
' * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
' * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
' 
Namespace java.time.format


	''' <summary>
	''' An exception thrown when an error occurs during parsing.
	''' <p>
	''' This exception includes the text being parsed and the error index.
	''' 
	''' @implSpec
	''' This class is intended for use in a single thread.
	''' 
	''' @since 1.8
	''' </summary>
	Public Class DateTimeParseException
		Inherits java.time.DateTimeException

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Shadows Const serialVersionUID As Long = 4304633501674722597L

		''' <summary>
		''' The text that was being parsed.
		''' </summary>
		Private ReadOnly parsedString As String
		''' <summary>
		''' The error index in the text.
		''' </summary>
		Private ReadOnly errorIndex As Integer

		''' <summary>
		''' Constructs a new exception with the specified message.
		''' </summary>
		''' <param name="message">  the message to use for this exception, may be null </param>
		''' <param name="parsedData">  the parsed text, should not be null </param>
		''' <param name="errorIndex">  the index in the parsed string that was invalid, should be a valid index </param>
		Public Sub New(ByVal message As String, ByVal parsedData As CharSequence, ByVal errorIndex As Integer)
			MyBase.New(message)
			Me.parsedString = parsedData.ToString()
			Me.errorIndex = errorIndex
		End Sub

		''' <summary>
		''' Constructs a new exception with the specified message and cause.
		''' </summary>
		''' <param name="message">  the message to use for this exception, may be null </param>
		''' <param name="parsedData">  the parsed text, should not be null </param>
		''' <param name="errorIndex">  the index in the parsed string that was invalid, should be a valid index </param>
		''' <param name="cause">  the cause exception, may be null </param>
		Public Sub New(ByVal message As String, ByVal parsedData As CharSequence, ByVal errorIndex As Integer, ByVal cause As Throwable)
			MyBase.New(message, cause)
			Me.parsedString = parsedData.ToString()
			Me.errorIndex = errorIndex
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns the string that was being parsed.
		''' </summary>
		''' <returns> the string that was being parsed, should not be null. </returns>
		Public Overridable Property parsedString As String
			Get
				Return parsedString
			End Get
		End Property

		''' <summary>
		''' Returns the index where the error was found.
		''' </summary>
		''' <returns> the index in the parsed string that was invalid, should be a valid index </returns>
		Public Overridable Property errorIndex As Integer
			Get
				Return errorIndex
			End Get
		End Property

	End Class

End Namespace
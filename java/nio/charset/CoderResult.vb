Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Collections.Generic
Imports java.lang

'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.charset



    ''' <summary>
    ''' A description of the result state of a coder.
    ''' 
    ''' <p> A charset coder, that is, either a decoder or an encoder, consumes bytes
    ''' (or characters) from an input buffer, translates them, and writes the
    ''' resulting characters (or bytes) to an output buffer.  A coding process
    ''' terminates for one of four categories of reasons, which are described by
    ''' instances of this class:
    ''' 
    ''' <ul>
    ''' 
    '''   <li><p> <i>Underflow</i> is reported when there is no more input to be
    '''   processed, or there is insufficient input and additional input is
    '''   required.  This condition is represented by the unique result object
    '''   <seealso cref="#UNDERFLOW"/>, whose <seealso cref="#isUnderflow() isUnderflow"/> method
    '''   returns <tt>true</tt>.  </p></li>
    ''' 
    '''   <li><p> <i>Overflow</i> is reported when there is insufficient room
    '''   remaining in the output buffer.  This condition is represented by the
    '''   unique result object <seealso cref="#OVERFLOW"/>, whose {@link #isOverflow()
    '''   isOverflow} method returns <tt>true</tt>.  </p></li>
    ''' 
    '''   <li><p> A <i>malformed-input error</i> is reported when a sequence of
    '''   input units is not well-formed.  Such errors are described by instances of
    '''   this class whose <seealso cref="#isMalformed() isMalformed"/> method returns
    '''   <tt>true</tt> and whose <seealso cref="#length() length"/> method returns the length
    '''   of the malformed sequence.  There is one unique instance of this class for
    '''   all malformed-input errors of a given length.  </p></li>
    ''' 
    '''   <li><p> An <i>unmappable-character error</i> is reported when a sequence
    '''   of input units denotes a character that cannot be represented in the
    '''   output charset.  Such errors are described by instances of this class
    '''   whose <seealso cref="#isUnmappable() isUnmappable"/> method returns <tt>true</tt> and
    '''   whose <seealso cref="#length() length"/> method returns the length of the input
    '''   sequence denoting the unmappable character.  There is one unique instance
    '''   of this class for all unmappable-character errors of a given length.
    '''   </p></li>
    ''' 
    ''' </ul>
    ''' 
    ''' <p> For convenience, the <seealso cref="#isError() isError"/> method returns <tt>true</tt>
    ''' for result objects that describe malformed-input and unmappable-character
    ''' errors but <tt>false</tt> for those that describe underflow or overflow
    ''' conditions.  </p>
    ''' 
    ''' 
    ''' @author Mark Reinhold
    ''' @author JSR-51 Expert Group
    ''' @since 1.4
    ''' </summary>

    Public Class CoderResult

        Private Const CR_UNDERFLOW As Integer = 0
        Private Const CR_OVERFLOW As Integer = 1
        Private Const CR_ERROR_MIN As Integer = 2
        Private Const CR_MALFORMED As Integer = 2
        Private Const CR_UNMAPPABLE As Integer = 3

        Private Shared ReadOnly names As String() = {"UNDERFLOW", "OVERFLOW", "MALFORMED", "UNMAPPABLE"}

        Private ReadOnly type As Integer
        Private ReadOnly length_Renamed As Integer

        Private Sub New(  type As Integer,   length As Integer)
            Me.type = type
            Me.length_Renamed = length
        End Sub

        ''' <summary>
        ''' Returns a string describing this coder result.
        ''' </summary>
        ''' <returns>  A descriptive string </returns>
        Public Overrides Function ToString() As String
            Dim nm As String = names(type)
            Return If([error], nm & "[" & length_Renamed & "]", nm)
        End Function

        ''' <summary>
        ''' Tells whether or not this object describes an underflow condition.
        ''' </summary>
        ''' <returns>  <tt>true</tt> if, and only if, this object denotes underflow </returns>
        Public Overridable Property underflow As Boolean
            Get
                Return (type = CR_UNDERFLOW)
            End Get
        End Property

        ''' <summary>
        ''' Tells whether or not this object describes an overflow condition.
        ''' </summary>
        ''' <returns>  <tt>true</tt> if, and only if, this object denotes overflow </returns>
        Public Overridable Property overflow As Boolean
            Get
                Return (type = CR_OVERFLOW)
            End Get
        End Property

        ''' <summary>
        ''' Tells whether or not this object describes an error condition.
        ''' </summary>
        ''' <returns>  <tt>true</tt> if, and only if, this object denotes either a
        '''          malformed-input error or an unmappable-character error </returns>
        Public Overridable Property [error] As Boolean
            Get
                Return (type >= CR_ERROR_MIN)
            End Get
        End Property

        ''' <summary>
        ''' Tells whether or not this object describes a malformed-input error.
        ''' </summary>
        ''' <returns>  <tt>true</tt> if, and only if, this object denotes a
        '''          malformed-input error </returns>
        Public Overridable Property malformed As Boolean
            Get
                Return (type = CR_MALFORMED)
            End Get
        End Property

        ''' <summary>
        ''' Tells whether or not this object describes an unmappable-character
        ''' error.
        ''' </summary>
        ''' <returns>  <tt>true</tt> if, and only if, this object denotes an
        '''          unmappable-character error </returns>
        Public Overridable Property unmappable As Boolean
            Get
                Return (type = CR_UNMAPPABLE)
            End Get
        End Property

        ''' <summary>
        ''' Returns the length of the erroneous input described by this
        ''' object&nbsp;&nbsp;<i>(optional operation)</i>.
        ''' </summary>
        ''' <returns>  The length of the erroneous input, a positive integer
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          If this object does not describe an error condition, that is,
        '''          if the <seealso cref="#isError() isError"/> does not return <tt>true</tt> </exception>
        Public Overridable Function length() As Integer
            If Not [error] Then Throw New UnsupportedOperationException
            Return length_Renamed
        End Function

        ''' <summary>
        ''' Result object indicating underflow, meaning that either the input buffer
        ''' has been completely consumed or, if the input buffer is not yet empty,
        ''' that additional input is required.
        ''' </summary>
        Public Shared ReadOnly UNDERFLOW As New CoderResult(CR_UNDERFLOW, 0)

        ''' <summary>
        ''' Result object indicating overflow, meaning that there is insufficient
        ''' room in the output buffer.
        ''' </summary>
        Public Shared ReadOnly OVERFLOW As New CoderResult(CR_OVERFLOW, 0)

        Private MustInherit Class Cache

            Private cache As IDictionary(Of Integer?, WeakReference(Of CoderResult)) = Nothing

            Protected Friend MustOverride Function create(  len As Integer) As CoderResult

            <MethodImpl(MethodImplOptions.Synchronized)>
            Private Function [get](  len As Integer) As CoderResult
                If len <= 0 Then Throw New IllegalArgumentException("Non-positive length")
                Dim k As Integer? = New Integer?(len)
                Dim w As WeakReference(Of CoderResult)
                Dim e As CoderResult = Nothing
                If cache Is Nothing Then
                    cache = New Dictionary(Of Integer?, WeakReference(Of CoderResult))
                Else
                    w = cache(k)
                    If w IsNot Nothing Then e = w.get()
                End If
                If e Is Nothing Then
                    e = create(len)
                    cache(k) = New WeakReference(Of CoderResult)(e)
                End If
                Return e
            End Function

        End Class

        Private Shared malformedCache As Cache = New CacheAnonymousInnerClassHelper

        Private Class CacheAnonymousInnerClassHelper
            Inherits Cache

            Public Overrides Function create(  len As Integer) As CoderResult
                Return New CoderResult(CR_MALFORMED, len)
            End Function
        End Class

        ''' <summary>
        ''' Static factory method that returns the unique object describing a
        ''' malformed-input error of the given length.
        ''' </summary>
        ''' <param name="length">
        '''          The given length
        ''' </param>
        ''' <returns>  The requested coder-result object </returns>
        Public Shared Function malformedForLength(  length As Integer) As CoderResult
            Return malformedCache.get(length)
        End Function

        Private Shared unmappableCache As Cache = New CacheAnonymousInnerClassHelper2

        Private Class CacheAnonymousInnerClassHelper2
            Inherits Cache

            Public Overrides Function create(  len As Integer) As CoderResult
                Return New CoderResult(CR_UNMAPPABLE, len)
            End Function
        End Class

        ''' <summary>
        ''' Static factory method that returns the unique result object describing
        ''' an unmappable-character error of the given length.
        ''' </summary>
        ''' <param name="length">
        '''          The given length
        ''' </param>
        ''' <returns>  The requested coder-result object </returns>
        Public Shared Function unmappableForLength(  length As Integer) As CoderResult
            Return unmappableCache.get(length)
        End Function

        ''' <summary>
        ''' Throws an exception appropriate to the result described by this object.
        ''' </summary>
        ''' <exception cref="BufferUnderflowException">
        '''          If this object is <seealso cref="#UNDERFLOW"/>
        ''' </exception>
        ''' <exception cref="BufferOverflowException">
        '''          If this object is <seealso cref="#OVERFLOW"/>
        ''' </exception>
        ''' <exception cref="MalformedInputException">
        '''          If this object represents a malformed-input error; the
        '''          exception's length value will be that of this object
        ''' </exception>
        ''' <exception cref="UnmappableCharacterException">
        '''          If this object represents an unmappable-character error; the
        '''          exceptions length value will be that of this object </exception>
        Public Overridable Sub throwException()
            Select Case type
                Case CR_UNDERFLOW
                    Throw New BufferUnderflowException
                Case CR_OVERFLOW
                    Throw New BufferOverflowException
                Case CR_MALFORMED
                    Throw New MalformedInputException(length_Renamed)
                Case CR_UNMAPPABLE
                    Throw New UnmappableCharacterException(length_Renamed)
                Case Else
                    Debug.Assert(False)
            End Select
        End Sub

    End Class

End Namespace
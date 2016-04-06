Imports System
Imports java.lang

'
' * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util

    ''' <summary>
    ''' <p>
    ''' The root class from which all event state objects shall be derived.
    ''' <p>
    ''' All Events are constructed with a reference to the object, the "source",
    ''' that is logically deemed to be the object upon which the Event in question
    ''' initially occurred upon.
    ''' 
    ''' @since JDK1.1
    ''' </summary>

    <Serializable>
    Public Class EventObject : Inherits java.lang.Object

        Private Const serialVersionUID As Long = 5516075349620653480L

        ''' <summary>
        ''' The object on which the Event initially occurred.
        ''' </summary>
        <NonSerialized>
        Protected Friend _source As Object

        ''' <summary>
        ''' Constructs a prototypical Event.
        ''' </summary>
        ''' <param name="source">    The object on which the Event initially occurred. </param>
        ''' <exception cref="IllegalArgumentException">  if source is null. </exception>
        Public Sub New(  source As Object)
            If source Is Nothing Then Throw New IllegalArgumentException("null source")

            Me._source = source
        End Sub

        ''' <summary>
        ''' The object on which the Event initially occurred.
        ''' </summary>
        ''' <returns>   The object on which the Event initially occurred. </returns>
        Public Overridable ReadOnly Property source As Object
            Get
                Return _source
            End Get
        End Property

        ''' <summary>
        ''' Returns a String representation of this EventObject.
        ''' </summary>
        ''' <returns>  A a String representation of this EventObject. </returns>
        Public Overrides Function ToString() As String
            Return Me.GetType().Name & "[source=" & source & "]"
        End Function
    End Class

End Namespace
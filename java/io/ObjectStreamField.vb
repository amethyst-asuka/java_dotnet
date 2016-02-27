'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io


    ''' <summary>
    ''' A description of a Serializable field from a Serializable class.  An array
    ''' of ObjectStreamFields is used to declare the Serializable fields of a class.
    ''' 
    ''' @author      Mike Warres
    ''' @author      Roger Riggs </summary>
    ''' <seealso cref= ObjectStreamClass
    ''' @since 1.2 </seealso>
    Public Class ObjectStreamField : Inherits java.lang.Object
        Implements Comparable(Of Object)

		''' <summary>
		''' field name </summary>
		Private ReadOnly name As String
		''' <summary>
		''' canonical JVM signature of field type </summary>
		Private ReadOnly signature As String
		''' <summary>
		''' field type (Object.class if unknown non-primitive type) </summary>
		Private ReadOnly type As  [Class]
		''' <summary>
		''' whether or not to (de)serialize field values as unshared </summary>
		Private ReadOnly unshared As Boolean
		''' <summary>
		''' corresponding reflective field object, if any </summary>
		Private ReadOnly field As Field
		''' <summary>
		''' offset of field value in enclosing field group </summary>
		Private offset As Integer = 0

		''' <summary>
		''' Create a Serializable field with the specified type.  This field should
		''' be documented with a <code>serialField</code> tag.
		''' </summary>
		''' <param name="name"> the name of the serializable field </param>
		''' <param name="type"> the <code>Class</code> object of the serializable field </param>
		Public Sub New(ByVal name As String, ByVal type As [Class])
			Me.New(name, type, False)
		End Sub

		''' <summary>
		''' Creates an ObjectStreamField representing a serializable field with the
		''' given name and type.  If unshared is false, values of the represented
		''' field are serialized and deserialized in the default manner--if the
		''' field is non-primitive, object values are serialized and deserialized as
		''' if they had been written and read by calls to writeObject and
		''' readObject.  If unshared is true, values of the represented field are
		''' serialized and deserialized as if they had been written and read by
		''' calls to writeUnshared and readUnshared.
		''' </summary>
		''' <param name="name"> field name </param>
		''' <param name="type"> field type </param>
		''' <param name="unshared"> if false, write/read field values in the same manner
		'''          as writeObject/readObject; if true, write/read in the same
		'''          manner as writeUnshared/readUnshared
		''' @since   1.4 </param>
		Public Sub New(ByVal name As String, ByVal type As [Class], ByVal unshared As Boolean)
			If name Is Nothing Then Throw New NullPointerException
			Me.name = name
			Me.type = type
			Me.unshared = unshared
			signature = getClassSignature(type).intern()
			field = Nothing
		End Sub

		''' <summary>
		''' Creates an ObjectStreamField representing a field with the given name,
		''' signature and unshared setting.
		''' </summary>
		Friend Sub New(ByVal name As String, ByVal signature As String, ByVal unshared As Boolean)
			If name Is Nothing Then Throw New NullPointerException
			Me.name = name
			Me.signature = signature.intern()
			Me.unshared = unshared
			field = Nothing

			Select Case signature.Chars(0)
				Case "Z"c
                    type = java.lang.[Boolean].TYPE
                Case "B"c
                    type = java.lang.[Byte].TYPE
                Case "C"c
                    type = Character.TYPE
                Case "S"c
                    type = java.lang.[Short].TYPE
                Case "I"c
                    type = java.lang.[Integer].TYPE
                Case "J"c
                    type = java.lang.[Long].TYPE
                Case "F"c
                    type = Float.TYPE
                Case "D"c
                    type = java.lang.[Double].TYPE
                Case "L"c, "["c
                    type = GetType(Object)
                Case Else
                    Throw New IllegalArgumentException("illegal signature")
            End Select
        End Sub

        ''' <summary>
        ''' Creates an ObjectStreamField representing the given field with the
        ''' specified unshared setting.  For compatibility with the behavior of
        ''' earlier serialization implementations, a "showType" parameter is
        ''' necessary to govern whether or not a getType() call on this
        ''' ObjectStreamField (if non-primitive) will return Object.class (as
        ''' opposed to a more specific reference type).
        ''' </summary>
        Friend Sub New(ByVal field As Field, ByVal unshared As Boolean, ByVal showType As Boolean)
            Me.field = field
            Me.unshared = unshared
            name = field.name
            Dim ftype As [Class] = field.type
            type = If(showType OrElse ftype.primitive, ftype, GetType(Object))
            signature = getClassSignature(ftype).Intern()
        End Sub

        ''' <summary>
        ''' Get the name of this field.
        ''' </summary>
        ''' <returns>  a <code>String</code> representing the name of the serializable
        '''          field </returns>
        Public Overridable Property name As String
            Get
                Return name
            End Get
        End Property

        ''' <summary>
        ''' Get the type of the field.  If the type is non-primitive and this
        ''' <code>ObjectStreamField</code> was obtained from a deserialized {@link
        ''' ObjectStreamClass} instance, then <code>Object.class</code> is returned.
        ''' Otherwise, the <code>Class</code> object for the type of the field is
        ''' returned.
        ''' </summary>
        ''' <returns>  a <code>Class</code> object representing the type of the
        '''          serializable field </returns>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Overridable Property type As [Class]
            Get
                If System.securityManager IsNot Nothing Then
                    Dim caller As [Class] = sun.reflect.Reflection.callerClass
                    If sun.reflect.misc.ReflectUtil.needsPackageAccessCheck(caller.classLoader, type.classLoader) Then sun.reflect.misc.ReflectUtil.checkPackageAccess(type)
                End If
                Return type
            End Get
        End Property

        ''' <summary>
        ''' Returns character encoding of field type.  The encoding is as follows:
        ''' <blockquote><pre>
        ''' B            byte
        ''' C            char
        ''' D            double
        ''' F            float
        ''' I            int
        ''' J            long
        ''' L            class or interface
        ''' S            short
        ''' Z            boolean
        ''' [            array
        ''' </pre></blockquote>
        ''' </summary>
        ''' <returns>  the typecode of the serializable field </returns>
        ' REMIND: deprecate?
        Public Overridable Property typeCode As Char
            Get
                Return signature.Chars(0)
            End Get
        End Property

        ''' <summary>
        ''' Return the JVM type signature.
        ''' </summary>
        ''' <returns>  null if this field has a primitive type. </returns>
        ' REMIND: deprecate?
        Public Overridable Property typeString As String
            Get
                Return If(primitive, Nothing, signature)
            End Get
        End Property

        ''' <summary>
        ''' Offset of field within instance data.
        ''' </summary>
        ''' <returns>  the offset of this field </returns>
        ''' <seealso cref= #setOffset </seealso>
        ' REMIND: deprecate?
        Public Overridable Property offset As Integer
            Get
                Return offset
            End Get
            Set(ByVal offset As Integer)
                Me.offset = offset
            End Set
        End Property

        ''' <summary>
        ''' Offset within instance data.
        ''' </summary>
        ''' <param name="offset"> the offset of the field </param>
        ''' <seealso cref= #getOffset </seealso>
        ' REMIND: deprecate?

        ''' <summary>
        ''' Return true if this field has a primitive type.
        ''' </summary>
        ''' <returns>  true if and only if this field corresponds to a primitive type </returns>
        ' REMIND: deprecate?
        Public Overridable Property primitive As Boolean
            Get
                Dim tcode As Char = signature.Chars(0)
                Return ((tcode <> "L"c) AndAlso (tcode <> "["c))
            End Get
        End Property

        ''' <summary>
        ''' Returns boolean value indicating whether or not the serializable field
        ''' represented by this ObjectStreamField instance is unshared.
        ''' </summary>
        ''' <returns> {@code true} if this field is unshared
        ''' 
        ''' @since 1.4 </returns>
        Public Overridable Property unshared As Boolean
            Get
                Return unshared
            End Get
        End Property

        ''' <summary>
        ''' Compare this field with another <code>ObjectStreamField</code>.  Return
        ''' -1 if this is smaller, 0 if equal, 1 if greater.  Types that are
        ''' primitives are "smaller" than object types.  If equal, the field names
        ''' are compared.
        ''' </summary>
        ' REMIND: deprecate?
        Public Overridable Function compareTo(ByVal obj As Object) As Integer Implements Comparable(Of Object).compareTo
            Dim other As ObjectStreamField = CType(obj, ObjectStreamField)
            Dim isPrim As Boolean = primitive
            If isPrim <> other.primitive Then Return If(isPrim, -1, 1)
            Return name.CompareTo(other.name)
        End Function

        ''' <summary>
        ''' Return a string that describes this field.
        ''' </summary>
        Public Overrides Function ToString() As String
            Return signature + AscW(" "c) + name
        End Function

        ''' <summary>
        ''' Returns field represented by this ObjectStreamField, or null if
        ''' ObjectStreamField is not associated with an actual field.
        ''' </summary>
        Friend Overridable Property field As Field
            Get
                Return field
            End Get
        End Property

        ''' <summary>
        ''' Returns JVM type signature of field (similar to getTypeString, except
        ''' that signature strings are returned for primitive fields as well).
        ''' </summary>
        Friend Overridable Property signature As String
            Get
                Return signature
            End Get
        End Property

        ''' <summary>
        ''' Returns JVM type signature for given class.
        ''' </summary>
        Private Shared Function getClassSignature(ByVal cl As [Class]) As String
            Dim sbuf As New StringBuilder
            Do While cl.array
                sbuf.append("["c)
                cl = cl.componentType
            Loop
            If cl.primitive Then
                If cl Is java.lang.[Integer].TYPE Then
                    sbuf.append("I"c)
                ElseIf cl Is java.lang.[Byte].TYPE Then
                    sbuf.append("B"c)
                ElseIf cl Is java.lang.[Long].TYPE Then
                    sbuf.append("J"c)
                ElseIf cl Is Float.TYPE Then
                    sbuf.append("F"c)
                ElseIf cl Is java.lang.[Double].TYPE Then
                    sbuf.append("D"c)
                ElseIf cl Is java.lang.[Short].TYPE Then
                    sbuf.append("S"c)
                ElseIf cl Is Character.TYPE Then
                    sbuf.append("C"c)
                ElseIf cl Is java.lang.[Boolean].TYPE Then
                    sbuf.append("Z"c)
                ElseIf cl Is Void.TYPE Then
                    sbuf.append("V"c)
                Else
                    Throw New InternalError
				End If
			Else
				sbuf.append(AscW("L"c) + cl.name.replace("."c, "/"c) + AscW(";"c))
			End If
			Return sbuf.ToString()
		End Function
	End Class

End Namespace
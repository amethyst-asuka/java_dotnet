Imports System

'
' * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang


    ''' <summary>
    ''' This is the common base class of all Java language enumeration types.
    ''' 
    ''' More information about enums, including descriptions of the
    ''' implicitly declared methods synthesized by the compiler, can be
    ''' found in section 8.9 of
    ''' <cite>The Java&trade; Language Specification</cite>.
    ''' 
    ''' <p> Note that when using an enumeration type as the type of a set
    ''' or as the type of the keys in a map, specialized and efficient
    ''' <seealso cref="java.util.EnumSet set"/> and {@linkplain
    ''' java.util.EnumMap map} implementations are available.
    ''' </summary>
    ''' @param <E> The enum type subclass
    ''' @author  Josh Bloch
    ''' @author  Neal Gafter </param>
    ''' <seealso cref=     Class#getEnumConstants() </seealso>
    ''' <seealso cref=     java.util.EnumSet </seealso>
    ''' <seealso cref=     java.util.EnumMap
    ''' @since   1.5 </seealso>
    <Serializable>
    Public MustInherit Class [Enum](Of E) : Inherits java.lang.Object
        Implements Comparable(Of E)

        ''' <summary>
        ''' The name of this enum constant, as declared in the enum declaration.
        ''' Most programmers should use the <seealso cref="#toString"/> method rather than
        ''' accessing this field.
        ''' </summary>
        Private ReadOnly name_Renamed As String

        ''' <summary>
        ''' Returns the name of this enum constant, exactly as declared in its
        ''' enum declaration.
        ''' 
        ''' <b>Most programmers should use the <seealso cref="#toString"/> method in
        ''' preference to this one, as the toString method may return
        ''' a more user-friendly name.</b>  This method is designed primarily for
        ''' use in specialized situations where correctness depends on getting the
        ''' exact name, which will not vary from release to release.
        ''' </summary>
        ''' <returns> the name of this enum constant </returns>
        Public Function name() As String
            Return name_Renamed
        End Function

        ''' <summary>
        ''' The ordinal of this enumeration constant (its position
        ''' in the enum declaration, where the initial constant is assigned
        ''' an ordinal of zero).
        ''' 
        ''' Most programmers will have no use for this field.  It is designed
        ''' for use by sophisticated enum-based data structures, such as
        ''' <seealso cref="java.util.EnumSet"/> and <seealso cref="java.util.EnumMap"/>.
        ''' </summary>
        Private ReadOnly ordinal_Renamed As Integer

        ''' <summary>
        ''' Returns the ordinal of this enumeration constant (its position
        ''' in its enum declaration, where the initial constant is assigned
        ''' an ordinal of zero).
        ''' 
        ''' Most programmers will have no use for this method.  It is
        ''' designed for use by sophisticated enum-based data structures, such
        ''' as <seealso cref="java.util.EnumSet"/> and <seealso cref="java.util.EnumMap"/>.
        ''' </summary>
        ''' <returns> the ordinal of this enumeration constant </returns>
        Public Function ordinal() As Integer
            Return ordinal_Renamed
        End Function

        ''' <summary>
        ''' Sole constructor.  Programmers cannot invoke this constructor.
        ''' It is for use by code emitted by the compiler in response to
        ''' enum type declarations.
        ''' </summary>
        ''' <param name="name"> - The name of this enum constant, which is the identifier
        '''               used to declare it. </param>
        ''' <param name="ordinal"> - The ordinal of this enumeration constant (its position
        '''         in the enum declaration, where the initial constant is assigned
        '''         an ordinal of zero). </param>
        Protected Friend Sub New(  name As String,   ordinal As Integer)
            Me.name_Renamed = name
            Me.ordinal_Renamed = ordinal
        End Sub

        ''' <summary>
        ''' Returns the name of this enum constant, as contained in the
        ''' declaration.  This method may be overridden, though it typically
        ''' isn't necessary or desirable.  An enum type should override this
        ''' method when a more "programmer-friendly" string form exists.
        ''' </summary>
        ''' <returns> the name of this enum constant </returns>
        Public Overrides Function ToString() As String
            Return name_Renamed
        End Function

        ''' <summary>
        ''' Returns true if the specified object is equal to this
        ''' enum constant.
        ''' </summary>
        ''' <param name="other"> the object to be compared for equality with this object. </param>
        ''' <returns>  true if the specified object is equal to this
        '''          enum constant. </returns>
        Public NotOverridable Overrides Function Equals(  other As Object) As Boolean
            Return Me Is other
        End Function

        ''' <summary>
        ''' Returns a hash code for this enum constant.
        ''' </summary>
        ''' <returns> a hash code for this enum constant. </returns>
        Public NotOverridable Overrides Function GetHashCode() As Integer
            Return MyBase.GetHashCode()
        End Function

        ''' <summary>
        ''' Throws CloneNotSupportedException.  This guarantees that enums
        ''' are never cloned, which is necessary to preserve their "singleton"
        ''' status.
        ''' </summary>
        ''' <returns> (never returns) </returns>
        Protected Friend Function clone() As Object
            Throw New CloneNotSupportedException
        End Function

        ''' <summary>
        ''' Compares this enum with the specified object for order.  Returns a
        ''' negative integer, zero, or a positive integer as this object is less
        ''' than, equal to, or greater than the specified object.
        ''' 
        ''' Enum constants are only comparable to other enum constants of the
        ''' same enum type.  The natural order implemented by this
        ''' method is the order in which the constants are declared.
        ''' </summary>
        Public Function compareTo(  o As E) As Integer Implements Comparable(Of E).compareTo
            Dim other As [Enum](Of E) = CType(DirectCast(o, Object), [Enum](Of E))
            Dim self As [Enum](Of E) = Me
            If self.GetType() IsNot other.GetType() AndAlso self.declaringClass IsNot other.declaringClass Then _ ' optimization Throw New ClassCastException
                Return self.ordinal_Renamed - other.ordinal_Renamed
        End Function

        ''' <summary>
        ''' Returns the Class object corresponding to this enum constant's
        ''' enum type.  Two enum constants e1 and  e2 are of the
        ''' same enum type if and only if
        '''   e1.getDeclaringClass() == e2.getDeclaringClass().
        ''' (The value returned by this method may differ from the one returned
        ''' by the <seealso cref="Object#getClass"/> method for enum constants with
        ''' constant-specific class bodies.)
        ''' </summary>
        ''' <returns> the Class object corresponding to this enum constant's
        '''     enum type </returns>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public ReadOnly Property declaringClass As [Class]
            Get
                Dim clazz As [Class] = Me.GetType()
                Dim zuper As [Class] = clazz.BaseType
                Return If(zuper Is GetType([Enum]), CType(clazz, [Class]), CType(zuper, [Class]))
            End Get
        End Property

        ''' <summary>
        ''' Returns the enum constant of the specified enum type with the
        ''' specified name.  The name must match exactly an identifier used
        ''' to declare an enum constant in this type.  (Extraneous whitespace
        ''' characters are not permitted.)
        ''' 
        ''' <p>Note that for a particular enum type {@code T}, the
        ''' implicitly declared {@code Public Shared T valueOf(String)}
        ''' method on that enum may be used instead of this method to map
        ''' from a name to the corresponding enum constant.  All the
        ''' constants of an enum type can be obtained by calling the
        ''' implicit {@code Public Shared T[] values()} method of that
        ''' type.
        ''' </summary>
        ''' @param <T> The enum type whose constant is to be returned </param>
        ''' <param name="enumType"> the {@code Class} object of the enum type from which
        '''      to return a constant </param>
        ''' <param name="name"> the name of the constant to return </param>
        ''' <returns> the enum constant of the specified enum type with the
        '''      specified name </returns>
        ''' <exception cref="IllegalArgumentException"> if the specified enum type has
        '''         no constant with the specified name, or the specified
        '''         class object does not represent an enum type </exception>
        ''' <exception cref="NullPointerException"> if {@code enumType} or {@code name}
        '''         is null
        ''' @since 1.5 </exception>
        Public Shared Function valueOf(Of T As [Enum](Of E))(  enumType As [Class],   name As String) As T
            Dim result As T = enumType.enumConstantDirectory()(name)
            If result IsNot Nothing Then Return result
            If name Is Nothing Then Throw New NullPointerException("Name is null")
            Throw New IllegalArgumentException("No enum constant " & enumType.canonicalName & "." & name)
        End Function

        ''' <summary>
        ''' enum classes cannot have finalize methods.
        ''' </summary>
        Protected Overrides Sub Finalize()
        End Sub

        ''' <summary>
        ''' prevent default deserialization
        ''' </summary>
        Private Sub readObject(  [in] As java.io.ObjectInputStream)
            Throw New java.io.InvalidObjectException("can't deserialize enum")
        End Sub

        Private Sub readObjectNoData()
            Throw New java.io.InvalidObjectException("can't deserialize enum")
        End Sub
    End Class

End Namespace
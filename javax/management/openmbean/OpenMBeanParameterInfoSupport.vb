Imports System
Imports javax.management.openmbean.OpenMBeanAttributeInfoSupport

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.management.openmbean


	' java import
	'

	' OpenMBeanAttributeInfoSupport and this class are very similar
	' but can't easily be refactored because there's no multiple inheritance.
	' The best we can do for refactoring is to put a bunch of static methods
	' in OpenMBeanAttributeInfoSupport and import them here.

	''' <summary>
	''' Describes a parameter used in one or more operations or
	''' constructors of an open MBean.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class OpenMBeanParameterInfoSupport
		Inherits javax.management.MBeanParameterInfo
		Implements OpenMBeanParameterInfo

		' Serial version 
		Friend Shadows Const serialVersionUID As Long = -7235016873758443122L

		''' <summary>
		''' @serial The open mbean parameter's <i>open type</i>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private openType As OpenType(Of ?)

		''' <summary>
		''' @serial The open mbean parameter's default value
		''' </summary>
		Private defaultValue As Object = Nothing

		''' <summary>
		''' @serial The open mbean parameter's legal values. This {@link
		''' Set} is unmodifiable
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private legalValues As java.util.Set(Of ?) = Nothing ' to be constructed unmodifiable

		''' <summary>
		''' @serial The open mbean parameter's min value
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private minValue As IComparable(Of ?) = Nothing

		''' <summary>
		''' @serial The open mbean parameter's max value
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private maxValue As IComparable(Of ?) = Nothing


		' As this instance is immutable, these two values need only
		' be calculated once.
		<NonSerialized> _
		Private myHashCode As Integer? = Nothing ' As this instance is immutable, these two values
		<NonSerialized> _
		Private myToString As String = Nothing ' need only be calculated once.


		''' <summary>
		''' Constructs an {@code OpenMBeanParameterInfoSupport} instance,
		''' which describes the parameter used in one or more operations or
		''' constructors of a class of open MBeans, with the specified
		''' {@code name}, {@code openType} and {@code description}.
		''' </summary>
		''' <param name="name">  cannot be a null or empty string.
		''' </param>
		''' <param name="description">  cannot be a null or empty string.
		''' </param>
		''' <param name="openType">  cannot be null.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code name} or {@code
		''' description} are null or empty string, or {@code openType} is
		''' null. </exception>
		Public Sub New(Of T1)(ByVal name As String, ByVal description As String, ByVal ___openType As OpenType(Of T1))
			Me.New(name, description, ___openType, CType(Nothing, javax.management.Descriptor))
		End Sub

		''' <summary>
		''' Constructs an {@code OpenMBeanParameterInfoSupport} instance,
		''' which describes the parameter used in one or more operations or
		''' constructors of a class of open MBeans, with the specified
		''' {@code name}, {@code openType}, {@code description},
		''' and {@code descriptor}.
		''' 
		''' <p>The {@code descriptor} can contain entries that will define
		''' the values returned by certain methods of this class, as
		''' explained in the <a href="package-summary.html#constraints">
		''' package description</a>.
		''' </summary>
		''' <param name="name">  cannot be a null or empty string.
		''' </param>
		''' <param name="description">  cannot be a null or empty string.
		''' </param>
		''' <param name="openType">  cannot be null.
		''' </param>
		''' <param name="descriptor"> The descriptor for the parameter.  This may be null
		''' which is equivalent to an empty descriptor.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code name} or {@code
		''' description} are null or empty string, or {@code openType} is
		''' null, or the descriptor entries are invalid as described in the
		''' <a href="package-summary.html#constraints">package
		''' description</a>.
		''' 
		''' @since 1.6 </exception>
		Public Sub New(Of T1)(ByVal name As String, ByVal description As String, ByVal ___openType As OpenType(Of T1), ByVal descriptor As javax.management.Descriptor)


			' Construct parent's state
			'
			MyBase.New(name,If(___openType Is Nothing, Nothing, ___openType.className), description, javax.management.ImmutableDescriptor.union(descriptor,If(___openType Is Nothing, Nothing, ___openType.descriptor)))

			' Initialize this instance's specific state
			'
			Me.openType = ___openType

			descriptor = descriptor ' replace null by empty
			Me.defaultValue = valueFrom(descriptor, "defaultValue", ___openType)
			Me.legalValues = valuesFrom(descriptor, "legalValues", ___openType)
			Me.minValue = comparableValueFrom(descriptor, "minValue", ___openType)
			Me.maxValue = comparableValueFrom(descriptor, "maxValue", ___openType)

			Try
				check(Me)
			Catch e As OpenDataException
				Throw New System.ArgumentException(e.Message, e)
			End Try
		End Sub


		''' <summary>
		''' Constructs an {@code OpenMBeanParameterInfoSupport} instance,
		''' which describes the parameter used in one or more operations or
		''' constructors of a class of open MBeans, with the specified
		''' {@code name}, {@code openType}, {@code description} and {@code
		''' defaultValue}.
		''' </summary>
		''' <param name="name">  cannot be a null or empty string.
		''' </param>
		''' <param name="description">  cannot be a null or empty string.
		''' </param>
		''' <param name="openType">  cannot be null.
		''' </param>
		''' <param name="defaultValue"> must be a valid value for the {@code
		''' openType} specified for this parameter; default value not
		''' supported for {@code ArrayType} and {@code TabularType}; can be
		''' null, in which case it means that no default value is set.
		''' </param>
		''' @param <T> allows the compiler to check that the {@code defaultValue},
		''' if non-null, has the correct Java type for the given {@code openType}.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code name} or {@code
		''' description} are null or empty string, or {@code openType} is
		''' null.
		''' </exception>
		''' <exception cref="OpenDataException"> if {@code defaultValue} is not a
		''' valid value for the specified {@code openType}, or {@code
		''' defaultValue} is non null and {@code openType} is an {@code
		''' ArrayType} or a {@code TabularType}. </exception>
		Public Sub New(Of T)(ByVal name As String, ByVal description As String, ByVal ___openType As OpenType(Of T), ByVal defaultValue As T)
			Me.New(name, description, ___openType, defaultValue, CType(Nothing, T()))
		End Sub

		''' <summary>
		''' <p>Constructs an {@code OpenMBeanParameterInfoSupport} instance,
		''' which describes the parameter used in one or more operations or
		''' constructors of a class of open MBeans, with the specified
		''' {@code name}, {@code openType}, {@code description}, {@code
		''' defaultValue} and {@code legalValues}.</p>
		''' 
		''' <p>The contents of {@code legalValues} are copied, so subsequent
		''' modifications of the array referenced by {@code legalValues}
		''' have no impact on this {@code OpenMBeanParameterInfoSupport}
		''' instance.</p>
		''' </summary>
		''' <param name="name">  cannot be a null or empty string.
		''' </param>
		''' <param name="description">  cannot be a null or empty string.
		''' </param>
		''' <param name="openType">  cannot be null.
		''' </param>
		''' <param name="defaultValue"> must be a valid value for the {@code
		''' openType} specified for this parameter; default value not
		''' supported for {@code ArrayType} and {@code TabularType}; can be
		''' null, in which case it means that no default value is set.
		''' </param>
		''' <param name="legalValues"> each contained value must be valid for the
		''' {@code openType} specified for this parameter; legal values not
		''' supported for {@code ArrayType} and {@code TabularType}; can be
		''' null or empty.
		''' </param>
		''' @param <T> allows the compiler to check that the {@code
		''' defaultValue} and {@code legalValues}, if non-null, have the
		''' correct Java type for the given {@code openType}.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code name} or {@code
		''' description} are null or empty string, or {@code openType} is
		''' null.
		''' </exception>
		''' <exception cref="OpenDataException"> if {@code defaultValue} is not a
		''' valid value for the specified {@code openType}, or one value in
		''' {@code legalValues} is not valid for the specified {@code
		''' openType}, or {@code defaultValue} is non null and {@code
		''' openType} is an {@code ArrayType} or a {@code TabularType}, or
		''' {@code legalValues} is non null and non empty and {@code
		''' openType} is an {@code ArrayType} or a {@code TabularType}, or
		''' {@code legalValues} is non null and non empty and {@code
		''' defaultValue} is not contained in {@code legalValues}. </exception>
		Public Sub New(Of T)(ByVal name As String, ByVal description As String, ByVal ___openType As OpenType(Of T), ByVal defaultValue As T, ByVal legalValues As T())
			Me.New(name, description, ___openType, defaultValue, legalValues, Nothing, Nothing)
		End Sub


		''' <summary>
		''' Constructs an {@code OpenMBeanParameterInfoSupport} instance,
		''' which describes the parameter used in one or more operations or
		''' constructors of a class of open MBeans, with the specified
		''' {@code name}, {@code openType}, {@code description}, {@code
		''' defaultValue}, {@code minValue} and {@code maxValue}.
		''' 
		''' It is possible to specify minimal and maximal values only for
		''' an open type whose values are {@code Comparable}.
		''' </summary>
		''' <param name="name">  cannot be a null or empty string.
		''' </param>
		''' <param name="description">  cannot be a null or empty string.
		''' </param>
		''' <param name="openType">  cannot be null.
		''' </param>
		''' <param name="defaultValue"> must be a valid value for the {@code
		''' openType} specified for this parameter; default value not
		''' supported for {@code ArrayType} and {@code TabularType}; can be
		''' null, in which case it means that no default value is set.
		''' </param>
		''' <param name="minValue"> must be valid for the {@code openType}
		''' specified for this parameter; can be null, in which case it
		''' means that no minimal value is set.
		''' </param>
		''' <param name="maxValue"> must be valid for the {@code openType}
		''' specified for this parameter; can be null, in which case it
		''' means that no maximal value is set.
		''' </param>
		''' @param <T> allows the compiler to check that the {@code
		''' defaultValue}, {@code minValue}, and {@code maxValue}, if
		''' non-null, have the correct Java type for the given {@code
		''' openType}.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code name} or {@code
		''' description} are null or empty string, or {@code openType} is
		''' null.
		''' </exception>
		''' <exception cref="OpenDataException"> if {@code defaultValue}, {@code
		''' minValue} or {@code maxValue} is not a valid value for the
		''' specified {@code openType}, or {@code defaultValue} is non null
		''' and {@code openType} is an {@code ArrayType} or a {@code
		''' TabularType}, or both {@code minValue} and {@code maxValue} are
		''' non-null and {@code minValue.compareTo(maxValue) > 0} is {@code
		''' true}, or both {@code defaultValue} and {@code minValue} are
		''' non-null and {@code minValue.compareTo(defaultValue) > 0} is
		''' {@code true}, or both {@code defaultValue} and {@code maxValue}
		''' are non-null and {@code defaultValue.compareTo(maxValue) > 0}
		''' is {@code true}. </exception>
		Public Sub New(Of T)(ByVal name As String, ByVal description As String, ByVal ___openType As OpenType(Of T), ByVal defaultValue As T, ByVal minValue As IComparable(Of T), ByVal maxValue As IComparable(Of T))
			Me.New(name, description, ___openType, defaultValue, Nothing, minValue, maxValue)
		End Sub

		Private Sub New(Of T)(ByVal name As String, ByVal description As String, ByVal ___openType As OpenType(Of T), ByVal defaultValue As T, ByVal legalValues As T(), ByVal minValue As IComparable(Of T), ByVal maxValue As IComparable(Of T))
			MyBase.New(name,If(___openType Is Nothing, Nothing, ___openType.className), description, makeDescriptor(___openType, defaultValue, legalValues, minValue, maxValue))

			Me.openType = ___openType

			Dim d As javax.management.Descriptor = descriptor
			Me.defaultValue = defaultValue
			Me.minValue = minValue
			Me.maxValue = maxValue
			' We already converted the array into an unmodifiable Set
			' in the descriptor.
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Me.legalValues = CType(d.getFieldValue("legalValues"), java.util.Set(Of ?))

			check(Me)
		End Sub

		''' <summary>
		''' An object serialized in a version of the API before Descriptors were
		''' added to this class will have an empty or null Descriptor.
		''' For consistency with our
		''' behavior in this version, we must replace the object with one
		''' where the Descriptors reflect the same values of openType, defaultValue,
		''' etc.
		''' 
		''' </summary>
		Private Function readResolve() As Object
			If descriptor.fieldNames.Length = 0 Then
				' This noise allows us to avoid "unchecked" warnings without
				' having to suppress them explicitly.
				Dim xopenType As OpenType(Of Object) = cast(openType)
				Dim xlegalValues As java.util.Set(Of Object) = cast(legalValues)
				Dim xminValue As IComparable(Of Object) = cast(minValue)
				Dim xmaxValue As IComparable(Of Object) = cast(maxValue)
				Return New OpenMBeanParameterInfoSupport(name, description, openType, makeDescriptor(xopenType, defaultValue, xlegalValues, xminValue, xmaxValue))
			Else
				Return Me
			End If
		End Function

		''' <summary>
		''' Returns the open type for the values of the parameter described
		''' by this {@code OpenMBeanParameterInfoSupport} instance.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property openType As OpenType(Of ?) Implements OpenMBeanParameterInfo.getOpenType
			Get
				Return openType
			End Get
		End Property

		''' <summary>
		''' Returns the default value for the parameter described by this
		''' {@code OpenMBeanParameterInfoSupport} instance, if specified,
		''' or {@code null} otherwise.
		''' </summary>
		Public Overridable Property defaultValue As Object Implements OpenMBeanParameterInfo.getDefaultValue
			Get
    
				' Special case for ArrayType and TabularType
				' [JF] TODO: clone it so that it cannot be altered,
				' [JF] TODO: if we decide to support defaultValue as an array itself.
				' [JF] As of today (oct 2000) it is not supported so
				' defaultValue is null for arrays. Nothing to do.
    
				Return defaultValue
			End Get
		End Property

		''' <summary>
		''' Returns an unmodifiable Set of legal values for the parameter
		''' described by this {@code OpenMBeanParameterInfoSupport}
		''' instance, if specified, or {@code null} otherwise.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property legalValues As java.util.Set(Of ?) Implements OpenMBeanParameterInfo.getLegalValues
			Get
    
				' Special case for ArrayType and TabularType
				' [JF] TODO: clone values so that they cannot be altered,
				' [JF] TODO: if we decide to support LegalValues as an array itself.
				' [JF] As of today (oct 2000) it is not supported so
				' legalValues is null for arrays. Nothing to do.
    
				' Returns our legalValues Set (set was constructed unmodifiable)
				Return (legalValues)
			End Get
		End Property

		''' <summary>
		''' Returns the minimal value for the parameter described by this
		''' {@code OpenMBeanParameterInfoSupport} instance, if specified,
		''' or {@code null} otherwise.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property minValue As IComparable(Of ?) Implements OpenMBeanParameterInfo.getMinValue
			Get
    
				' Note: only comparable values have a minValue, so that's not
				' the case of arrays and tabulars (always null).
    
				Return minValue
			End Get
		End Property

		''' <summary>
		''' Returns the maximal value for the parameter described by this
		''' {@code OpenMBeanParameterInfoSupport} instance, if specified,
		''' or {@code null} otherwise.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property maxValue As IComparable(Of ?) Implements OpenMBeanParameterInfo.getMaxValue
			Get
    
				' Note: only comparable values have a maxValue, so that's not
				' the case of arrays and tabulars (always null).
    
				Return maxValue
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this {@code
		''' OpenMBeanParameterInfoSupport} instance specifies a non-null
		''' default value for the described parameter, {@code false}
		''' otherwise.
		''' </summary>
		Public Overridable Function hasDefaultValue() As Boolean Implements OpenMBeanParameterInfo.hasDefaultValue

			Return (defaultValue IsNot Nothing)
		End Function

		''' <summary>
		''' Returns {@code true} if this {@code
		''' OpenMBeanParameterInfoSupport} instance specifies a non-null
		''' set of legal values for the described parameter, {@code false}
		''' otherwise.
		''' </summary>
		Public Overridable Function hasLegalValues() As Boolean Implements OpenMBeanParameterInfo.hasLegalValues

			Return (legalValues IsNot Nothing)
		End Function

		''' <summary>
		''' Returns {@code true} if this {@code
		''' OpenMBeanParameterInfoSupport} instance specifies a non-null
		''' minimal value for the described parameter, {@code false}
		''' otherwise.
		''' </summary>
		Public Overridable Function hasMinValue() As Boolean Implements OpenMBeanParameterInfo.hasMinValue

			Return (minValue IsNot Nothing)
		End Function

		''' <summary>
		''' Returns {@code true} if this {@code
		''' OpenMBeanParameterInfoSupport} instance specifies a non-null
		''' maximal value for the described parameter, {@code false}
		''' otherwise.
		''' </summary>
		Public Overridable Function hasMaxValue() As Boolean Implements OpenMBeanParameterInfo.hasMaxValue

			Return (maxValue IsNot Nothing)
		End Function


		''' <summary>
		''' Tests whether {@code obj} is a valid value for the parameter
		''' described by this {@code OpenMBeanParameterInfo} instance.
		''' </summary>
		''' <param name="obj"> the object to be tested.
		''' </param>
		''' <returns> {@code true} if {@code obj} is a valid value
		''' for the parameter described by this
		''' {@code OpenMBeanParameterInfo} instance,
		''' {@code false} otherwise. </returns>
		Public Overridable Function isValue(ByVal obj As Object) As Boolean Implements OpenMBeanParameterInfo.isValue
			Return OpenMBeanAttributeInfoSupport.isValue(Me, obj)
			' compiler bug? should be able to omit class name here
			' also below in toString and hashCode
		End Function


		' ***  Commodity methods from java.lang.Object  *** 


		''' <summary>
		''' <p>Compares the specified {@code obj} parameter with this {@code
		''' OpenMBeanParameterInfoSupport} instance for equality.</p>
		''' 
		''' <p>Returns {@code true} if and only if all of the following
		''' statements are true:
		''' 
		''' <ul>
		''' <li>{@code obj} is non null,</li>
		''' <li>{@code obj} also implements the {@code OpenMBeanParameterInfo}
		''' interface,</li>
		''' <li>their names are equal</li>
		''' <li>their open types are equal</li>
		''' <li>their default, min, max and legal values are equal.</li>
		''' </ul>
		''' This ensures that this {@code equals} method works properly for
		''' {@code obj} parameters which are different implementations of
		''' the {@code OpenMBeanParameterInfo} interface.
		''' 
		''' <p>If {@code obj} also implements <seealso cref="DescriptorRead"/>, then its
		''' <seealso cref="DescriptorRead#getDescriptor() getDescriptor()"/> method must
		''' also return the same value as for this object.</p>
		''' </summary>
		''' <param name="obj"> the object to be compared for equality with this
		''' {@code OpenMBeanParameterInfoSupport} instance.
		''' </param>
		''' <returns> {@code true} if the specified object is equal to this
		''' {@code OpenMBeanParameterInfoSupport} instance. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Not(TypeOf obj Is OpenMBeanParameterInfo) Then Return False

			Dim other As OpenMBeanParameterInfo = CType(obj, OpenMBeanParameterInfo)

			Return equal(Me, other)
		End Function

		''' <summary>
		''' <p>Returns the hash code value for this {@code
		''' OpenMBeanParameterInfoSupport} instance.</p>
		''' 
		''' <p>The hash code of an {@code OpenMBeanParameterInfoSupport}
		''' instance is the sum of the hash codes of all elements of
		''' information used in {@code equals} comparisons (ie: its name,
		''' its <i>open type</i>, its default, min, max and legal
		''' values, and its Descriptor).
		''' 
		''' <p>This ensures that {@code t1.equals(t2)} implies that {@code
		''' t1.hashCode()==t2.hashCode()} for any two {@code
		''' OpenMBeanParameterInfoSupport} instances {@code t1} and {@code
		''' t2}, as required by the general contract of the method {@link
		''' Object#hashCode() Object.hashCode()}.
		''' 
		''' <p>However, note that another instance of a class implementing
		''' the {@code OpenMBeanParameterInfo} interface may be equal to
		''' this {@code OpenMBeanParameterInfoSupport} instance as defined
		''' by <seealso cref="#equals(java.lang.Object)"/>, but may have a different
		''' hash code if it is calculated differently.
		''' 
		''' <p>As {@code OpenMBeanParameterInfoSupport} instances are
		''' immutable, the hash code for this instance is calculated once,
		''' on the first call to {@code hashCode}, and then the same value
		''' is returned for subsequent calls.
		''' </summary>
		''' <returns> the hash code value for this {@code
		''' OpenMBeanParameterInfoSupport} instance </returns>
		Public Overrides Function GetHashCode() As Integer

			' Calculate the hash code value if it has not yet been done
			' (ie 1st call to hashCode())
			'
			If myHashCode Is Nothing Then myHashCode = OpenMBeanAttributeInfoSupport.hashCode(Me)

			' return always the same hash code for this instance (immutable)
			'
			Return myHashCode
		End Function

		''' <summary>
		''' Returns a string representation of this
		''' {@code OpenMBeanParameterInfoSupport} instance.
		''' <p>
		''' The string representation consists of the name of this class (i.e.
		''' {@code javax.management.openmbean.OpenMBeanParameterInfoSupport}),
		''' the string representation of the name and open type of the described
		''' parameter, the string representation of its default, min, max and legal
		''' values and the string representation of its descriptor.
		''' <p>
		''' As {@code OpenMBeanParameterInfoSupport} instances are immutable,
		''' the string representation for this instance is calculated once,
		''' on the first call to {@code toString}, and then the same value
		''' is returned for subsequent calls.
		''' </summary>
		''' <returns> a string representation of this
		''' {@code OpenMBeanParameterInfoSupport} instance. </returns>
		Public Overrides Function ToString() As String

			' Calculate the string value if it has not yet been done (ie
			' 1st call to toString())
			'
			If myToString Is Nothing Then myToString = OpenMBeanAttributeInfoSupport.ToString(Me)

			' return always the same string representation for this
			' instance (immutable)
			'
			Return myToString
		End Function

	End Class

End Namespace
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2015, Oracle and/or its affiliates. All rights reserved.
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

	''' <summary>
	''' Describes an attribute of an open MBean.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class OpenMBeanAttributeInfoSupport
		Inherits javax.management.MBeanAttributeInfo
		Implements OpenMBeanAttributeInfo

		' Serial version 
		Friend Shadows Const serialVersionUID As Long = -4867215622149721849L

		''' <summary>
		''' @serial The open mbean attribute's <i>open type</i>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private openType As OpenType(Of ?)

		''' <summary>
		''' @serial The open mbean attribute's default value
		''' </summary>
		Private ReadOnly defaultValue As Object

		''' <summary>
		''' @serial The open mbean attribute's legal values. This {@link
		''' Set} is unmodifiable
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private ReadOnly legalValues As java.util.Set(Of ?) ' to be constructed unmodifiable

		''' <summary>
		''' @serial The open mbean attribute's min value
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private ReadOnly minValue As IComparable(Of ?)

		''' <summary>
		''' @serial The open mbean attribute's max value
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private ReadOnly maxValue As IComparable(Of ?)


		' As this instance is immutable, these two values need only
		' be calculated once.
		<NonSerialized> _
		Private myHashCode As Integer? = Nothing
		<NonSerialized> _
		Private myToString As String = Nothing


		''' <summary>
		''' Constructs an {@code OpenMBeanAttributeInfoSupport} instance,
		''' which describes the attribute of an open MBean with the
		''' specified {@code name}, {@code openType} and {@code
		''' description}, and the specified read/write access properties.
		''' </summary>
		''' <param name="name">  cannot be a null or empty string.
		''' </param>
		''' <param name="description">  cannot be a null or empty string.
		''' </param>
		''' <param name="openType">  cannot be null.
		''' </param>
		''' <param name="isReadable"> {@code true} if the attribute has a getter
		''' exposed for management.
		''' </param>
		''' <param name="isWritable"> {@code true} if the attribute has a setter
		''' exposed for management.
		''' </param>
		''' <param name="isIs"> {@code true} if the attribute's getter is of the
		''' form <tt>is<i>XXX</i></tt>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code name} or {@code
		''' description} are null or empty string, or {@code openType} is
		''' null. </exception>
		Public Sub New(Of T1)(ByVal name As String, ByVal description As String, ByVal ___openType As OpenType(Of T1), ByVal isReadable As Boolean, ByVal isWritable As Boolean, ByVal isIs As Boolean)
			Me.New(name, description, ___openType, isReadable, isWritable, isIs, CType(Nothing, javax.management.Descriptor))
		End Sub

		''' <summary>
		''' <p>Constructs an {@code OpenMBeanAttributeInfoSupport} instance,
		''' which describes the attribute of an open MBean with the
		''' specified {@code name}, {@code openType}, {@code
		''' description}, read/write access properties, and {@code Descriptor}.</p>
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
		''' <param name="isReadable"> {@code true} if the attribute has a getter
		''' exposed for management.
		''' </param>
		''' <param name="isWritable"> {@code true} if the attribute has a setter
		''' exposed for management.
		''' </param>
		''' <param name="isIs"> {@code true} if the attribute's getter is of the
		''' form <tt>is<i>XXX</i></tt>.
		''' </param>
		''' <param name="descriptor"> The descriptor for the attribute.  This may be null
		''' which is equivalent to an empty descriptor.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code name} or {@code
		''' description} are null or empty string, or {@code openType} is
		''' null, or the descriptor entries are invalid as described in the
		''' <a href="package-summary.html#constraints">package description</a>.
		''' 
		''' @since 1.6 </exception>
		Public Sub New(Of T1)(ByVal name As String, ByVal description As String, ByVal ___openType As OpenType(Of T1), ByVal isReadable As Boolean, ByVal isWritable As Boolean, ByVal isIs As Boolean, ByVal descriptor As javax.management.Descriptor)
			' Construct parent's state
			'
			MyBase.New(name,If(___openType Is Nothing, Nothing, ___openType.className), description, isReadable, isWritable, isIs, javax.management.ImmutableDescriptor.union(descriptor,If(___openType Is Nothing, Nothing, ___openType.descriptor)))

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
		''' Constructs an {@code OpenMBeanAttributeInfoSupport} instance,
		''' which describes the attribute of an open MBean with the
		''' specified {@code name}, {@code openType}, {@code description}
		''' and {@code defaultValue}, and the specified read/write access
		''' properties.
		''' </summary>
		''' <param name="name">  cannot be a null or empty string.
		''' </param>
		''' <param name="description">  cannot be a null or empty string.
		''' </param>
		''' <param name="openType">  cannot be null.
		''' </param>
		''' <param name="isReadable"> {@code true} if the attribute has a getter
		''' exposed for management.
		''' </param>
		''' <param name="isWritable"> {@code true} if the attribute has a setter
		''' exposed for management.
		''' </param>
		''' <param name="isIs"> {@code true} if the attribute's getter is of the
		''' form <tt>is<i>XXX</i></tt>.
		''' </param>
		''' <param name="defaultValue"> must be a valid value for the {@code
		''' openType} specified for this attribute; default value not
		''' supported for {@code ArrayType} and {@code TabularType}; can
		''' be null, in which case it means that no default value is set.
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
		Public Sub New(Of T)(ByVal name As String, ByVal description As String, ByVal ___openType As OpenType(Of T), ByVal isReadable As Boolean, ByVal isWritable As Boolean, ByVal isIs As Boolean, ByVal defaultValue As T)
			Me.New(name, description, ___openType, isReadable, isWritable, isIs, defaultValue, CType(Nothing, T()))
		End Sub


		''' <summary>
		''' <p>Constructs an {@code OpenMBeanAttributeInfoSupport} instance,
		''' which describes the attribute of an open MBean with the
		''' specified {@code name}, {@code openType}, {@code description},
		''' {@code defaultValue} and {@code legalValues}, and the specified
		''' read/write access properties.</p>
		''' 
		''' <p>The contents of {@code legalValues} are copied, so subsequent
		''' modifications of the array referenced by {@code legalValues}
		''' have no impact on this {@code OpenMBeanAttributeInfoSupport}
		''' instance.</p>
		''' </summary>
		''' <param name="name">  cannot be a null or empty string.
		''' </param>
		''' <param name="description">  cannot be a null or empty string.
		''' </param>
		''' <param name="openType">  cannot be null.
		''' </param>
		''' <param name="isReadable"> {@code true} if the attribute has a getter
		''' exposed for management.
		''' </param>
		''' <param name="isWritable"> {@code true} if the attribute has a setter
		''' exposed for management.
		''' </param>
		''' <param name="isIs"> {@code true} if the attribute's getter is of the
		''' form <tt>is<i>XXX</i></tt>.
		''' </param>
		''' <param name="defaultValue"> must be a valid value
		''' for the {@code
		''' openType} specified for this attribute; default value not
		''' supported for {@code ArrayType} and {@code TabularType}; can
		''' be null, in which case it means that no default value is set.
		''' </param>
		''' <param name="legalValues"> each contained value must be valid for the
		''' {@code openType} specified for this attribute; legal values
		''' not supported for {@code ArrayType} and {@code TabularType};
		''' can be null or empty.
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
		Public Sub New(Of T)(ByVal name As String, ByVal description As String, ByVal ___openType As OpenType(Of T), ByVal isReadable As Boolean, ByVal isWritable As Boolean, ByVal isIs As Boolean, ByVal defaultValue As T, ByVal legalValues As T())
			Me.New(name, description, ___openType, isReadable, isWritable, isIs, defaultValue, legalValues, Nothing, Nothing)
		End Sub


		''' <summary>
		''' Constructs an {@code OpenMBeanAttributeInfoSupport} instance,
		''' which describes the attribute of an open MBean, with the
		''' specified {@code name}, {@code openType}, {@code description},
		''' {@code defaultValue}, {@code minValue} and {@code maxValue}.
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
		''' <param name="isReadable"> {@code true} if the attribute has a getter
		''' exposed for management.
		''' </param>
		''' <param name="isWritable"> {@code true} if the attribute has a setter
		''' exposed for management.
		''' </param>
		''' <param name="isIs"> {@code true} if the attribute's getter is of the
		''' form <tt>is<i>XXX</i></tt>.
		''' </param>
		''' <param name="defaultValue"> must be a valid value for the {@code
		''' openType} specified for this attribute; default value not
		''' supported for {@code ArrayType} and {@code TabularType}; can be
		''' null, in which case it means that no default value is set.
		''' </param>
		''' <param name="minValue"> must be valid for the {@code openType}
		''' specified for this attribute; can be null, in which case it
		''' means that no minimal value is set.
		''' </param>
		''' <param name="maxValue"> must be valid for the {@code openType}
		''' specified for this attribute; can be null, in which case it
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
		Public Sub New(Of T)(ByVal name As String, ByVal description As String, ByVal ___openType As OpenType(Of T), ByVal isReadable As Boolean, ByVal isWritable As Boolean, ByVal isIs As Boolean, ByVal defaultValue As T, ByVal minValue As IComparable(Of T), ByVal maxValue As IComparable(Of T))
			Me.New(name, description, ___openType, isReadable, isWritable, isIs, defaultValue, Nothing, minValue, maxValue)
		End Sub

		Private Sub New(Of T)(ByVal name As String, ByVal description As String, ByVal ___openType As OpenType(Of T), ByVal isReadable As Boolean, ByVal isWritable As Boolean, ByVal isIs As Boolean, ByVal defaultValue As T, ByVal legalValues As T(), ByVal minValue As IComparable(Of T), ByVal maxValue As IComparable(Of T))
			MyBase.New(name,If(___openType Is Nothing, Nothing, ___openType.className), description, isReadable, isWritable, isIs, makeDescriptor(___openType, defaultValue, legalValues, minValue, maxValue))

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
				Dim xopenType As OpenType(Of Object) = cast(openType)
				Dim xlegalValues As java.util.Set(Of Object) = cast(legalValues)
				Dim xminValue As IComparable(Of Object) = cast(minValue)
				Dim xmaxValue As IComparable(Of Object) = cast(maxValue)
				Return New OpenMBeanAttributeInfoSupport(name, description, openType, readable, writable, [is], makeDescriptor(xopenType, defaultValue, xlegalValues, xminValue, xmaxValue))
			Else
				Return Me
			End If
		End Function

		Friend Shared Sub check(ByVal info As OpenMBeanParameterInfo)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim ___openType As OpenType(Of ?) = info.openType
			If ___openType Is Nothing Then Throw New System.ArgumentException("OpenType cannot be null")

			If info.name Is Nothing OrElse info.name.Trim().Equals("") Then Throw New System.ArgumentException("Name cannot be null or empty")

			If info.description Is Nothing OrElse info.description.Trim().Equals("") Then Throw New System.ArgumentException("Description cannot be null or empty")

			' Check and initialize defaultValue
			'
			If info.hasDefaultValue() Then
				' Default value not supported for ArrayType and TabularType
				' Cast to Object because "OpenType<T> instanceof" is illegal
				If ___openType.array OrElse TypeOf CObj(___openType) Is TabularType Then Throw New OpenDataException("Default value not supported " & "for ArrayType and TabularType")
				' Check defaultValue's class
				If Not ___openType.isValue(info.defaultValue) Then
					Dim msg As String = "Argument defaultValue's class [""" & info.defaultValue.GetType().name & """] does not match the one defined in openType[""" & ___openType.className & """]"
					Throw New OpenDataException(msg)
				End If
			End If

			' Check that we don't have both legalValues and min or max
			'
			If info.hasLegalValues() AndAlso (info.hasMinValue() OrElse info.hasMaxValue()) Then Throw New OpenDataException("cannot have both legalValue and " & "minValue or maxValue")

			' Check minValue and maxValue
			If info.hasMinValue() AndAlso (Not ___openType.isValue(info.minValue)) Then
				Dim msg As String = "Type of minValue [" & info.minValue.GetType().name & "] does not match OpenType [" & ___openType.className & "]"
				Throw New OpenDataException(msg)
			End If
			If info.hasMaxValue() AndAlso (Not ___openType.isValue(info.maxValue)) Then
				Dim msg As String = "Type of maxValue [" & info.maxValue.GetType().name & "] does not match OpenType [" & ___openType.className & "]"
				Throw New OpenDataException(msg)
			End If

			' Check that defaultValue is a legal value
			'
			If info.hasDefaultValue() Then
				Dim ___defaultValue As Object = info.defaultValue
				If info.hasLegalValues() AndAlso (Not info.legalValues.contains(___defaultValue)) Then Throw New OpenDataException("defaultValue is not contained " & "in legalValues")

				' Check that minValue <= defaultValue <= maxValue
				'
				If info.hasMinValue() Then
					If compare(info.minValue, ___defaultValue) > 0 Then Throw New OpenDataException("minValue cannot be greater " & "than defaultValue")
				End If
				If info.hasMaxValue() Then
					If compare(info.maxValue, ___defaultValue) < 0 Then Throw New OpenDataException("maxValue cannot be less " & "than defaultValue")
				End If
			End If

			' Check legalValues
			'
			If info.hasLegalValues() Then
				' legalValues not supported for TabularType and arrays
				If TypeOf CObj(___openType) Is TabularType OrElse ___openType.array Then Throw New OpenDataException("Legal values not supported " & "for TabularType and arrays")
				' Check legalValues are valid with openType
				For Each v As Object In info.legalValues
					If Not ___openType.isValue(v) Then
						Dim msg As String = "Element of legalValues [" & v & "] is not a valid value for the specified openType [" & ___openType.ToString() & "]"
						Throw New OpenDataException(msg)
					End If
				Next v
			End If


			' Check that, if both specified, minValue <= maxValue
			'
			If info.hasMinValue() AndAlso info.hasMaxValue() Then
				If compare(info.minValue, info.maxValue) > 0 Then Throw New OpenDataException("minValue cannot be greater " & "than maxValue")
			End If

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function compare(ByVal x As Object, ByVal y As Object) As Integer
			Return CType(x, IComparable).CompareTo(y)
		End Function

		Friend Shared Function makeDescriptor(Of T)(ByVal ___openType As OpenType(Of T), ByVal defaultValue As T, ByVal legalValues As T(), ByVal minValue As IComparable(Of T), ByVal maxValue As IComparable(Of T)) As javax.management.Descriptor
			Dim map As IDictionary(Of String, Object) = New Dictionary(Of String, Object)
			If defaultValue IsNot Nothing Then map("defaultValue") = defaultValue
			If legalValues IsNot Nothing Then
				Dim [set] As java.util.Set(Of T) = New HashSet(Of T)
				For Each v As T In legalValues
					[set].add(v)
				Next v
				[set] = java.util.Collections.unmodifiableSet([set])
				map("legalValues") = [set]
			End If
			If minValue IsNot Nothing Then map("minValue") = minValue
			If maxValue IsNot Nothing Then map("maxValue") = maxValue
			If map.Count = 0 Then
				Return ___openType.descriptor
			Else
				map("openType") = ___openType
				Return New javax.management.ImmutableDescriptor(map)
			End If
		End Function

		Friend Shared Function makeDescriptor(Of T)(ByVal ___openType As OpenType(Of T), ByVal defaultValue As T, ByVal legalValues As java.util.Set(Of T), ByVal minValue As IComparable(Of T), ByVal maxValue As IComparable(Of T)) As javax.management.Descriptor
			Dim legals As T()
			If legalValues Is Nothing Then
				legals = Nothing
			Else
				legals = cast(New Object(legalValues.size() - 1){})
				legalValues.ToArray(legals)
			End If
			Return makeDescriptor(___openType, defaultValue, legals, minValue, maxValue)
		End Function


		Friend Shared Function valueFrom(Of T)(ByVal d As javax.management.Descriptor, ByVal name As String, ByVal ___openType As OpenType(Of T)) As T
			Dim x As Object = d.getFieldValue(name)
			If x Is Nothing Then Return Nothing
			Try
				Return convertFrom(x, ___openType)
			Catch e As Exception
				Dim msg As String = "Cannot convert descriptor field " & name & "  to " & ___openType.typeName
				Throw com.sun.jmx.remote.util.EnvHelp.initCause(New System.ArgumentException(msg), e)
			End Try
		End Function

		Friend Shared Function valuesFrom(Of T)(ByVal d As javax.management.Descriptor, ByVal name As String, ByVal ___openType As OpenType(Of T)) As java.util.Set(Of T)
			Dim x As Object = d.getFieldValue(name)
			If x Is Nothing Then Return Nothing
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim coll As ICollection(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If TypeOf x Is java.util.Set(Of ?) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim [set] As java.util.Set(Of ?) = CType(x, java.util.Set(Of ?))
				Dim asis As Boolean = True
				For Each element As Object In [set]
					If Not ___openType.isValue(element) Then
						asis = False
						Exit For
					End If
				Next element
				If asis Then Return cast([set])
				coll = [set]
			ElseIf TypeOf x Is Object() Then
				coll = java.util.Arrays.asList(CType(x, Object()))
			Else
				Dim msg As String = "Descriptor value for " & name & " must be a Set or " & "an array: " & x.GetType().name
				Throw New System.ArgumentException(msg)
			End If

			Dim result As java.util.Set(Of T) = New HashSet(Of T)
			For Each element As Object In coll
				result.add(convertFrom(element, ___openType))
			Next element
			Return result
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend Shared Function comparableValueFrom(Of T)(ByVal d As javax.management.Descriptor, ByVal name As String, ByVal ___openType As OpenType(Of T)) As IComparable(Of ?)
			Dim t As T = valueFrom(d, name, ___openType)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If t Is Nothing OrElse TypeOf t Is IComparable(Of ?) Then Return CType(t, IComparable(Of ?))
			Dim msg As String = "Descriptor field " & name & " with value " & t & " is not Comparable"
			Throw New System.ArgumentException(msg)
		End Function

		Private Shared Function convertFrom(Of T)(ByVal x As Object, ByVal ___openType As OpenType(Of T)) As T
			If ___openType.isValue(x) Then
				Dim t As T = OpenMBeanAttributeInfoSupport.cast(Of T)(x)
				Return t
			End If
			Return convertFromStrings(x, ___openType)
		End Function

		Private Shared Function convertFromStrings(Of T)(ByVal x As Object, ByVal ___openType As OpenType(Of T)) As T
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If TypeOf ___openType Is ArrayType(Of ?) Then
				Return convertFromStringArray(x, ___openType)
			ElseIf TypeOf x Is String Then
				Return convertFromString(CStr(x), ___openType)
			End If
			Dim msg As String = "Cannot convert value " & x & " of type " & x.GetType().name & " to type " & ___openType.typeName
			Throw New System.ArgumentException(msg)
		End Function

		Private Shared Function convertFromString(Of T)(ByVal s As String, ByVal ___openType As OpenType(Of T)) As T
			Dim c As Type
			Try
				Dim className As String = ___openType.safeGetClassName()
				sun.reflect.misc.ReflectUtil.checkPackageAccess(className)
				c = cast(Type.GetType(className))
			Catch e As ClassNotFoundException
				Throw New NoClassDefFoundError(e.ToString()) ' can't happen
			End Try

			' Look for: public static T valueOf(String)
			Dim valueOf As Method
			Try
				' It is safe to call this plain Class.getMethod because the class "c"
				' was checked before by ReflectUtil.checkPackageAccess(openType.safeGetClassName());
				valueOf = c.GetMethod("valueOf", GetType(String))
				If (Not Modifier.isStatic(valueOf.modifiers)) OrElse valueOf.returnType IsNot c Then valueOf = Nothing
			Catch e As NoSuchMethodException
				valueOf = Nothing
			End Try
			If valueOf IsNot Nothing Then
				Try
					Return c.cast(sun.reflect.misc.MethodUtil.invoke(valueOf, Nothing, New Object() {s}))
				Catch e As Exception
					Dim msg As String = "Could not convert """ & s & """ using method: " & valueOf
					Throw New System.ArgumentException(msg, e)
				End Try
			End If

			' Look for: public T(String)
			Dim con As Constructor(Of T)
			Try
				' It is safe to call this plain Class.getConstructor because the class "c"
				' was checked before by ReflectUtil.checkPackageAccess(openType.safeGetClassName());
				con = c.GetConstructor(GetType(String))
			Catch e As NoSuchMethodException
				con = Nothing
			End Try
			If con IsNot Nothing Then
				Try
					Return con.newInstance(s)
				Catch e As Exception
					Dim msg As String = "Could not convert """ & s & """ using constructor: " & con
					Throw New System.ArgumentException(msg, e)
				End Try
			End If

			Throw New System.ArgumentException("Don't know how to convert " & "string to " & ___openType.typeName)
		End Function


	'     A Descriptor contained an array value encoded as Strings.  The
	'       Strings must be organized in an array corresponding to the desired
	'       array.  If the desired array has n dimensions, so must the String
	'       array.  We will convert element by element from String to desired
	'       component type. 
		Private Shared Function convertFromStringArray(Of T)(ByVal x As Object, ByVal ___openType As OpenType(Of T)) As T
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim ___arrayType As ArrayType(Of ?) = CType(___openType, ArrayType(Of ?))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim baseType As OpenType(Of ?) = ___arrayType.elementOpenType
			Dim [dim] As Integer = ___arrayType.dimension
			Dim squareBrackets As String = "["
			For i As Integer = 1 To [dim] - 1
				squareBrackets &= "["
			Next i
			Dim stringArrayClass As Type
			Dim targetArrayClass As Type
			Try
				Dim baseClassName As String = baseType.safeGetClassName()

				' check access to the provided base type class name and bail out early
				sun.reflect.misc.ReflectUtil.checkPackageAccess(baseClassName)

				stringArrayClass = Type.GetType(squareBrackets & "Ljava.lang.String;")
				targetArrayClass = Type.GetType(squareBrackets & "L" & baseClassName & ";")
			Catch e As ClassNotFoundException
				Throw New NoClassDefFoundError(e.ToString()) ' can't happen
			End Try
			If Not stringArrayClass.IsInstanceOfType(x) Then
				Dim msg As String = "Value for " & [dim] & "-dimensional array of " & baseType.typeName & " must be same type or a String " & "array with same dimensions"
				Throw New System.ArgumentException(msg)
			End If
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim componentOpenType As OpenType(Of ?)
			If [dim] = 1 Then
				componentOpenType = baseType
			Else
				Try
					componentOpenType = New ArrayType(Of T)([dim] - 1, baseType)
				Catch e As OpenDataException
					Throw New System.ArgumentException(e.Message, e)
					' can't happen
				End Try
			End If
			Dim n As Integer = Array.getLength(x)
			Dim targetArray As Object() = CType(Array.newInstance(targetArrayClass.GetElementType(), n), Object())
			For i As Integer = 0 To n - 1
				Dim stringish As Object = Array.get(x, i) ' String or String[] etc
				Dim converted As Object = convertFromStrings(stringish, componentOpenType)
				Array.set(targetArray, i, converted)
			Next i
			Return OpenMBeanAttributeInfoSupport.cast(Of T)(targetArray)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function cast(Of T)(ByVal x As Object) As T
			Return CType(x, T)
		End Function

		''' <summary>
		''' Returns the open type for the values of the attribute described
		''' by this {@code OpenMBeanAttributeInfoSupport} instance.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property openType As OpenType(Of ?) Implements OpenMBeanParameterInfo.getOpenType
			Get
				Return openType
			End Get
		End Property

		''' <summary>
		''' Returns the default value for the attribute described by this
		''' {@code OpenMBeanAttributeInfoSupport} instance, if specified,
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
		''' Returns an unmodifiable Set of legal values for the attribute
		''' described by this {@code OpenMBeanAttributeInfoSupport}
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
		''' Returns the minimal value for the attribute described by this
		''' {@code OpenMBeanAttributeInfoSupport} instance, if specified,
		''' or {@code null} otherwise.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property minValue As IComparable(Of ?) Implements OpenMBeanParameterInfo.getMinValue
			Get
    
				' Note: only comparable values have a minValue,
				' so that's not the case of arrays and tabulars (always null).
    
				Return minValue
			End Get
		End Property

		''' <summary>
		''' Returns the maximal value for the attribute described by this
		''' {@code OpenMBeanAttributeInfoSupport} instance, if specified,
		''' or {@code null} otherwise.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property maxValue As IComparable(Of ?) Implements OpenMBeanParameterInfo.getMaxValue
			Get
    
				' Note: only comparable values have a maxValue,
				' so that's not the case of arrays and tabulars (always null).
    
				Return maxValue
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this {@code
		''' OpenMBeanAttributeInfoSupport} instance specifies a non-null
		''' default value for the described attribute, {@code false}
		''' otherwise.
		''' </summary>
		Public Overridable Function hasDefaultValue() As Boolean Implements OpenMBeanParameterInfo.hasDefaultValue

			Return (defaultValue IsNot Nothing)
		End Function

		''' <summary>
		''' Returns {@code true} if this {@code
		''' OpenMBeanAttributeInfoSupport} instance specifies a non-null
		''' set of legal values for the described attribute, {@code false}
		''' otherwise.
		''' </summary>
		Public Overridable Function hasLegalValues() As Boolean Implements OpenMBeanParameterInfo.hasLegalValues

			Return (legalValues IsNot Nothing)
		End Function

		''' <summary>
		''' Returns {@code true} if this {@code
		''' OpenMBeanAttributeInfoSupport} instance specifies a non-null
		''' minimal value for the described attribute, {@code false}
		''' otherwise.
		''' </summary>
		Public Overridable Function hasMinValue() As Boolean Implements OpenMBeanParameterInfo.hasMinValue

			Return (minValue IsNot Nothing)
		End Function

		''' <summary>
		''' Returns {@code true} if this {@code
		''' OpenMBeanAttributeInfoSupport} instance specifies a non-null
		''' maximal value for the described attribute, {@code false}
		''' otherwise.
		''' </summary>
		Public Overridable Function hasMaxValue() As Boolean Implements OpenMBeanParameterInfo.hasMaxValue

			Return (maxValue IsNot Nothing)
		End Function


		''' <summary>
		''' Tests whether {@code obj} is a valid value for the attribute
		''' described by this {@code OpenMBeanAttributeInfoSupport}
		''' instance.
		''' </summary>
		''' <param name="obj"> the object to be tested.
		''' </param>
		''' <returns> {@code true} if {@code obj} is a valid value for
		''' the parameter described by this {@code
		''' OpenMBeanAttributeInfoSupport} instance, {@code false}
		''' otherwise. </returns>
		Public Overridable Function isValue(ByVal obj As Object) As Boolean Implements OpenMBeanParameterInfo.isValue
			Return isValue(Me, obj)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function isValue(ByVal info As OpenMBeanParameterInfo, ByVal obj As Object) As Boolean ' cast to Comparable
			If info.hasDefaultValue() AndAlso obj Is Nothing Then Return True
			Return info.openType.isValue(obj) AndAlso ((Not info.hasLegalValues()) OrElse info.legalValues.contains(obj)) AndAlso ((Not info.hasMinValue()) OrElse CType(info.minValue, IComparable).CompareTo(obj) <= 0) AndAlso ((Not info.hasMaxValue()) OrElse CType(info.maxValue, IComparable).CompareTo(obj) >= 0)
		End Function

		' ***  Commodity methods from java.lang.Object  *** 


		''' <summary>
		''' Compares the specified {@code obj} parameter with this {@code
		''' OpenMBeanAttributeInfoSupport} instance for equality.
		''' <p>
		''' Returns {@code true} if and only if all of the following statements are true:
		''' <ul>
		''' <li>{@code obj} is non null,</li>
		''' <li>{@code obj} also implements the {@code OpenMBeanAttributeInfo} interface,</li>
		''' <li>their names are equal</li>
		''' <li>their open types are equal</li>
		''' <li>their access properties (isReadable, isWritable and isIs) are equal</li>
		''' <li>their default, min, max and legal values are equal.</li>
		''' </ul>
		''' This ensures that this {@code equals} method works properly for
		''' {@code obj} parameters which are different implementations of
		''' the {@code OpenMBeanAttributeInfo} interface.
		''' 
		''' <p>If {@code obj} also implements <seealso cref="DescriptorRead"/>, then its
		''' <seealso cref="DescriptorRead#getDescriptor() getDescriptor()"/> method must
		''' also return the same value as for this object.</p>
		''' </summary>
		''' <param name="obj"> the object to be compared for equality with this
		''' {@code OpenMBeanAttributeInfoSupport} instance.
		''' </param>
		''' <returns> {@code true} if the specified object is equal to this
		''' {@code OpenMBeanAttributeInfoSupport} instance. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Not(TypeOf obj Is OpenMBeanAttributeInfo) Then Return False

			Dim other As OpenMBeanAttributeInfo = CType(obj, OpenMBeanAttributeInfo)

			Return Me.readable = other.readable AndAlso Me.writable = other.writable AndAlso Me.is = other.is AndAlso equal(Me, other)
		End Function

		Friend Shared Function equal(ByVal x1 As OpenMBeanParameterInfo, ByVal x2 As OpenMBeanParameterInfo) As Boolean
			If TypeOf x1 Is javax.management.DescriptorRead Then
				If Not(TypeOf x2 Is javax.management.DescriptorRead) Then Return False
				Dim d1 As javax.management.Descriptor = CType(x1, javax.management.DescriptorRead).descriptor
				Dim d2 As javax.management.Descriptor = CType(x2, javax.management.DescriptorRead).descriptor
				If Not d1.Equals(d2) Then Return False
			ElseIf TypeOf x2 Is javax.management.DescriptorRead Then
				Return False
			End If

			Return x1.name.Equals(x2.name) AndAlso x1.openType.Equals(x2.openType) AndAlso (If(x1.hasDefaultValue(), x1.defaultValue.Equals(x2.defaultValue), (Not x2.hasDefaultValue()))) AndAlso (If(x1.hasMinValue(), x1.minValue.Equals(x2.minValue), (Not x2.hasMinValue()))) AndAlso (If(x1.hasMaxValue(), x1.maxValue.Equals(x2.maxValue), (Not x2.hasMaxValue()))) AndAlso (If(x1.hasLegalValues(), x1.legalValues.Equals(x2.legalValues), (Not x2.hasLegalValues())))
		End Function

		''' <summary>
		''' <p>Returns the hash code value for this {@code
		''' OpenMBeanAttributeInfoSupport} instance.</p>
		''' 
		''' <p>The hash code of an {@code OpenMBeanAttributeInfoSupport}
		''' instance is the sum of the hash codes of all elements of
		''' information used in {@code equals} comparisons (ie: its name,
		''' its <i>open type</i>, its default, min, max and legal
		''' values, and its Descriptor).
		''' 
		''' <p>This ensures that {@code t1.equals(t2)} implies that {@code
		''' t1.hashCode()==t2.hashCode()} for any two {@code
		''' OpenMBeanAttributeInfoSupport} instances {@code t1} and {@code
		''' t2}, as required by the general contract of the method {@link
		''' Object#hashCode() Object.hashCode()}.
		''' 
		''' <p>However, note that another instance of a class implementing
		''' the {@code OpenMBeanAttributeInfo} interface may be equal to
		''' this {@code OpenMBeanAttributeInfoSupport} instance as defined
		''' by <seealso cref="#equals(java.lang.Object)"/>, but may have a different
		''' hash code if it is calculated differently.
		''' 
		''' <p>As {@code OpenMBeanAttributeInfoSupport} instances are
		''' immutable, the hash code for this instance is calculated once,
		''' on the first call to {@code hashCode}, and then the same value
		''' is returned for subsequent calls.
		''' </summary>
		''' <returns> the hash code value for this {@code
		''' OpenMBeanAttributeInfoSupport} instance </returns>
		Public Overrides Function GetHashCode() As Integer

			' Calculate the hash code value if it has not yet been done
			' (ie 1st call to hashCode())
			'
			If myHashCode Is Nothing Then myHashCode = hashCode(Me)

			' return always the same hash code for this instance (immutable)
			'
			Return myHashCode
		End Function

		Friend Shared Function GetHashCode(ByVal info As OpenMBeanParameterInfo) As Integer
			Dim ___value As Integer = 0
			___value += info.name.GetHashCode()
			___value += info.openType.GetHashCode()
			If info.hasDefaultValue() Then ___value += info.defaultValue.GetHashCode()
			If info.hasMinValue() Then ___value += info.minValue.GetHashCode()
			If info.hasMaxValue() Then ___value += info.maxValue.GetHashCode()
			If info.hasLegalValues() Then ___value += info.legalValues.GetHashCode()
			If TypeOf info Is javax.management.DescriptorRead Then ___value += CType(info, javax.management.DescriptorRead).descriptor.GetHashCode()
			Return ___value
		End Function

		''' <summary>
		''' Returns a string representation of this
		''' {@code OpenMBeanAttributeInfoSupport} instance.
		''' <p>
		''' The string representation consists of the name of this class (i.e.
		''' {@code javax.management.openmbean.OpenMBeanAttributeInfoSupport}),
		''' the string representation of the name and open type of the
		''' described parameter, the string representation of its
		''' default, min, max and legal values and the string
		''' representation of its descriptor.
		''' 
		''' <p>As {@code OpenMBeanAttributeInfoSupport} instances are
		''' immutable, the string representation for this instance is
		''' calculated once, on the first call to {@code toString}, and
		''' then the same value is returned for subsequent calls.
		''' </summary>
		''' <returns> a string representation of this
		''' {@code OpenMBeanAttributeInfoSupport} instance. </returns>
		Public Overrides Function ToString() As String

			' Calculate the string value if it has not yet been done
			' (ie 1st call to toString())
			'
			If myToString Is Nothing Then myToString = ToString(Me)

			' return always the same string representation for this
			' instance (immutable)
			'
			Return myToString
		End Function

		Friend Shared Function ToString(ByVal info As OpenMBeanParameterInfo) As String
			Dim d As javax.management.Descriptor = If(TypeOf info Is javax.management.DescriptorRead, CType(info, javax.management.DescriptorRead).descriptor, Nothing)
			Return info.GetType().name & "(name=" & info.name & ",openType=" & info.openType & ",default=" & info.defaultValue & ",minValue=" & info.minValue & ",maxValue=" & info.maxValue & ",legalValues=" & info.legalValues + (If(d Is Nothing, "", ",descriptor=" & d)) & ")"
		End Function
	End Class

End Namespace
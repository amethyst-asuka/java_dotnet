Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans





	''' <summary>
	''' The FeatureDescriptor class is the common baseclass for PropertyDescriptor,
	''' EventSetDescriptor, and MethodDescriptor, etc.
	''' <p>
	''' It supports some common information that can be set and retrieved for
	''' any of the introspection descriptors.
	''' <p>
	''' In addition it provides an extension mechanism so that arbitrary
	''' attribute/value pairs can be associated with a design feature.
	''' </summary>

	Public Class FeatureDescriptor
		Private Const TRANSIENT As String = "transient"

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private classRef As Reference(Of ? As [Class])

		''' <summary>
		''' Constructs a <code>FeatureDescriptor</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Gets the programmatic name of this feature.
		''' </summary>
		''' <returns> The programmatic name of the property/method/event </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
			Set(  name As String)
				Me.name = name
			End Set
		End Property


		''' <summary>
		''' Gets the localized display name of this feature.
		''' </summary>
		''' <returns> The localized display name for the property/method/event.
		'''  This defaults to the same as its programmatic name from getName. </returns>
		Public Overridable Property displayName As String
			Get
				If displayName Is Nothing Then Return name
				Return displayName
			End Get
			Set(  displayName As String)
				Me.displayName = displayName
			End Set
		End Property


		''' <summary>
		''' The "expert" flag is used to distinguish between those features that are
		''' intended for expert users from those that are intended for normal users.
		''' </summary>
		''' <returns> True if this feature is intended for use by experts only. </returns>
		Public Overridable Property expert As Boolean
			Get
				Return expert
			End Get
			Set(  expert As Boolean)
				Me.expert = expert
			End Set
		End Property


		''' <summary>
		''' The "hidden" flag is used to identify features that are intended only
		''' for tool use, and which should not be exposed to humans.
		''' </summary>
		''' <returns> True if this feature should be hidden from human users. </returns>
		Public Overridable Property hidden As Boolean
			Get
				Return hidden
			End Get
			Set(  hidden As Boolean)
				Me.hidden = hidden
			End Set
		End Property


		''' <summary>
		''' The "preferred" flag is used to identify features that are particularly
		''' important for presenting to humans.
		''' </summary>
		''' <returns> True if this feature should be preferentially shown to human users. </returns>
		Public Overridable Property preferred As Boolean
			Get
				Return preferred
			End Get
			Set(  preferred As Boolean)
				Me.preferred = preferred
			End Set
		End Property


		''' <summary>
		''' Gets the short description of this feature.
		''' </summary>
		''' <returns>  A localized short description associated with this
		'''   property/method/event.  This defaults to be the display name. </returns>
		Public Overridable Property shortDescription As String
			Get
				If shortDescription Is Nothing Then Return displayName
				Return shortDescription
			End Get
			Set(  text As String)
				shortDescription = text
			End Set
		End Property


		''' <summary>
		''' Associate a named attribute with this feature.
		''' </summary>
		''' <param name="attributeName">  The locale-independent name of the attribute </param>
		''' <param name="value">  The value. </param>
		Public Overridable Sub setValue(  attributeName As String,   value As Object)
			table(attributeName) = value
		End Sub

		''' <summary>
		''' Retrieve a named attribute with this feature.
		''' </summary>
		''' <param name="attributeName">  The locale-independent name of the attribute </param>
		''' <returns>  The value of the attribute.  May be null if
		'''     the attribute is unknown. </returns>
		Public Overridable Function getValue(  attributeName As String) As Object
			Return If(Me.table IsNot Nothing, Me.table(attributeName), Nothing)
		End Function

		''' <summary>
		''' Gets an enumeration of the locale-independent names of this
		''' feature.
		''' </summary>
		''' <returns>  An enumeration of the locale-independent names of any
		'''    attributes that have been registered with setValue. </returns>
		Public Overridable Function attributeNames() As System.Collections.IEnumerator(Of String)
			Return table.Keys.GetEnumerator()
		End Function

		''' <summary>
		''' Package-private constructor,
		''' Merge information from two FeatureDescriptors.
		''' The merged hidden and expert flags are formed by or-ing the values.
		''' In the event of other conflicts, the second argument (y) is
		''' given priority over the first argument (x).
		''' </summary>
		''' <param name="x">  The first (lower priority) MethodDescriptor </param>
		''' <param name="y">  The second (higher priority) MethodDescriptor </param>
		Friend Sub New(  x As FeatureDescriptor,   y As FeatureDescriptor)
			expert = x.expert Or y.expert
			hidden = x.hidden Or y.hidden
			preferred = x.preferred Or y.preferred
			name = y.name
			shortDescription = x.shortDescription
			If y.shortDescription IsNot Nothing Then shortDescription = y.shortDescription
			displayName = x.displayName
			If y.displayName IsNot Nothing Then displayName = y.displayName
			classRef = x.classRef
			If y.classRef IsNot Nothing Then classRef = y.classRef
			addTable(x.table)
			addTable(y.table)
		End Sub

	'    
	'     * Package-private dup constructor
	'     * This must isolate the new object from any changes to the old object.
	'     
		Friend Sub New(  old As FeatureDescriptor)
			expert = old.expert
			hidden = old.hidden
			preferred = old.preferred
			name = old.name
			shortDescription = old.shortDescription
			displayName = old.displayName
			classRef = old.classRef

			addTable(old.table)
		End Sub

		''' <summary>
		''' Copies all values from the specified attribute table.
		''' If some attribute is exist its value should be overridden.
		''' </summary>
		''' <param name="table">  the attribute table with new values </param>
		Private Sub addTable(  table As Dictionary(Of String, Object))
			If (table IsNot Nothing) AndAlso table.Count > 0 Then table.putAll(table)
		End Sub

		''' <summary>
		''' Returns the initialized attribute table.
		''' </summary>
		''' <returns> the initialized attribute table </returns>
		Private Property table As Dictionary(Of String, Object)
			Get
				If Me.table Is Nothing Then Me.table = New Dictionary(Of )
				Return Me.table
			End Get
		End Property

		''' <summary>
		''' Sets the "transient" attribute according to the annotation.
		''' If the "transient" attribute is already set
		''' it should not be changed.
		''' </summary>
		''' <param name="annotation">  the annotation of the element of the feature </param>
		Friend Overridable Property transient As Transient
			Set(  annotation As Transient)
				If (annotation IsNot Nothing) AndAlso (Nothing Is getValue(TRANSIENT)) Then valuelue(TRANSIENT, annotation.value())
			End Set
			Get
				Dim value_Renamed As Object = getValue(TRANSIENT)
				Return If(TypeOf value_Renamed Is Boolean?, CBool(value_Renamed), False)
			End Get
		End Property


		' Package private methods for recreating the weak/soft referent

		Friend Overridable Property class0 As  [Class]
			Set(  cls As [Class])
				Me.classRef = getWeakReference(cls)
			End Set
			Get
				Return If(Me.classRef IsNot Nothing, Me.classRef.get(), Nothing)
			End Get
		End Property


		''' <summary>
		''' Creates a new soft reference that refers to the given object.
		''' </summary>
		''' <returns> a new soft reference or <code>null</code> if object is <code>null</code>
		''' </returns>
		''' <seealso cref= SoftReference </seealso>
		Friend Shared Function getSoftReference(Of T)(  [object] As T) As Reference(Of T)
			Return If(object_Renamed IsNot Nothing, New SoftReference(Of )(object_Renamed), Nothing)
		End Function

		''' <summary>
		''' Creates a new weak reference that refers to the given object.
		''' </summary>
		''' <returns> a new weak reference or <code>null</code> if object is <code>null</code>
		''' </returns>
		''' <seealso cref= WeakReference </seealso>
		Friend Shared Function getWeakReference(Of T)(  [object] As T) As Reference(Of T)
			Return If(object_Renamed IsNot Nothing, New WeakReference(Of )(object_Renamed), Nothing)
		End Function

		''' <summary>
		''' Resolves the return type of the method.
		''' </summary>
		''' <param name="base">    the class that contains the method in the hierarchy </param>
		''' <param name="method">  the object that represents the method </param>
		''' <returns> a class identifying the return type of the method
		''' </returns>
		''' <seealso cref= Method#getGenericReturnType </seealso>
		''' <seealso cref= Method#getReturnType </seealso>
		Friend Shared Function getReturnType(  base As [Class],   method As Method) As  [Class]
			If base Is Nothing Then base = method.declaringClass
			Return com.sun.beans.TypeResolver.erase(com.sun.beans.TypeResolver.resolveInClass(base, method.genericReturnType))
		End Function

		''' <summary>
		''' Resolves the parameter types of the method.
		''' </summary>
		''' <param name="base">    the class that contains the method in the hierarchy </param>
		''' <param name="method">  the object that represents the method </param>
		''' <returns> an array of classes identifying the parameter types of the method
		''' </returns>
		''' <seealso cref= Method#getGenericParameterTypes </seealso>
		''' <seealso cref= Method#getParameterTypes </seealso>
		Friend Shared Function getParameterTypes(  base As [Class],   method As Method) As  [Class]()
			If base Is Nothing Then base = method.declaringClass
			Return com.sun.beans.TypeResolver.erase(com.sun.beans.TypeResolver.resolveInClass(base, method.genericParameterTypes))
		End Function

		Private expert As Boolean
		Private hidden As Boolean
		Private preferred As Boolean
		Private shortDescription As String
		Private name As String
		Private displayName As String
		Private table As Dictionary(Of String, Object)

		''' <summary>
		''' Returns a string representation of the object.
		''' </summary>
		''' <returns> a string representation of the object
		''' 
		''' @since 1.7 </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder(Me.GetType().name)
			sb.append("[name=").append(Me.name)
			appendTo(sb, "displayName", Me.displayName)
			appendTo(sb, "shortDescription", Me.shortDescription)
			appendTo(sb, "preferred", Me.preferred)
			appendTo(sb, "hidden", Me.hidden)
			appendTo(sb, "expert", Me.expert)
			If (Me.table IsNot Nothing) AndAlso Me.table.Count > 0 Then
				sb.append("; values={")
				For Each entry As KeyValuePair(Of String, Object) In Me.table
					sb.append(entry.Key).append("=").append(entry.Value).append("; ")
				Next entry
				sb.length = sb.length() - 2
				sb.append("}")
			End If
			appendTo(sb)
			Return sb.append("]").ToString()
		End Function

		Friend Overridable Sub appendTo(  sb As StringBuilder)
		End Sub

		Friend Shared Sub appendTo(Of T1)(  sb As StringBuilder,   name As String,   reference As Reference(Of T1))
			If reference IsNot Nothing Then appendTo(sb, name, reference.get())
		End Sub

		Friend Shared Sub appendTo(  sb As StringBuilder,   name As String,   value As Object)
			If value IsNot Nothing Then sb.append("; ").append(name).append("=").append(value)
		End Sub

		Friend Shared Sub appendTo(  sb As StringBuilder,   name As String,   value As Boolean)
			If value Then sb.append("; ").append(name)
		End Sub
	End Class

End Namespace
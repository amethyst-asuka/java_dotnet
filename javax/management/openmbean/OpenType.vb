Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2008, Oracle and/or its affiliates. All rights reserved.
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


	''' <summary>
	''' The <code>OpenType</code> class is the parent abstract class of all classes which describe the actual <i>open type</i>
	''' of open data values.
	''' <p>
	''' An <i>open type</i> is defined by:
	''' <ul>
	'''  <li>the fully qualified Java class name of the open data values this type describes;
	'''      note that only a limited set of Java classes is allowed for open data values
	'''      (see <seealso cref="#ALLOWED_CLASSNAMES_LIST ALLOWED_CLASSNAMES_LIST"/>),</li>
	'''  <li>its name,</li>
	'''  <li>its description.</li>
	''' </ul>
	''' </summary>
	''' @param <T> the Java type that instances described by this type must
	''' have.  For example, <seealso cref="SimpleType#INTEGER"/> is a {@code
	''' SimpleType<Integer>} which is a subclass of {@code OpenType<Integer>},
	''' meaning that an attribute, parameter, or return value that is described
	''' as a {@code SimpleType.INTEGER} must have Java type
	''' <seealso cref="Integer"/>.
	''' 
	''' @since 1.5 </param>
	<Serializable> _
	Public MustInherit Class OpenType(Of T)

		' Serial version 
		Friend Const serialVersionUID As Long = -9195195325186646468L


		''' <summary>
		''' List of the fully qualified names of the Java classes allowed for open
		''' data values. A multidimensional array of any one of these classes or
		''' their corresponding primitive types is also an allowed class for open
		''' data values.
		''' 
		'''   <pre>ALLOWED_CLASSNAMES_LIST = {
		'''    "java.lang.Void",
		'''    "java.lang.Boolean",
		'''    "java.lang.Character",
		'''    "java.lang.Byte",
		'''    "java.lang.Short",
		'''    "java.lang.Integer",
		'''    "java.lang.Long",
		'''    "java.lang.Float",
		'''    "java.lang.Double",
		'''    "java.lang.String",
		'''    "java.math.BigDecimal",
		'''    "java.math.BigInteger",
		'''    "java.util.Date",
		'''    "javax.management.ObjectName",
		'''    CompositeData.class.getName(),
		'''    TabularData.class.getName() } ;
		'''   </pre>
		''' 
		''' </summary>
		Public Shared ReadOnly ALLOWED_CLASSNAMES_LIST As IList(Of String) = java.util.Collections.unmodifiableList(java.util.Arrays.asList("java.lang.Void", "java.lang.Boolean", "java.lang.Character", "java.lang.Byte", "java.lang.Short", "java.lang.Integer", "java.lang.Long", "java.lang.Float", "java.lang.Double", "java.lang.String", "java.math.BigDecimal", "java.math.BigInteger", "java.util.Date", "javax.management.ObjectName", GetType(CompositeData).name, GetType(TabularData).name)) ' in case the package of these classes should change (who knows...) -  better refer to these two class names like this, rather than hardcoding a string,


		''' @deprecated Use <seealso cref="#ALLOWED_CLASSNAMES_LIST ALLOWED_CLASSNAMES_LIST"/> instead. 
		<Obsolete("Use <seealso cref="#ALLOWED_CLASSNAMES_LIST ALLOWED_CLASSNAMES_LIST"/> instead.")> _
		Public Shared ReadOnly ALLOWED_CLASSNAMES As String() = ALLOWED_CLASSNAMES_LIST.ToArray()


		''' <summary>
		''' @serial The fully qualified Java class name of open data values this
		'''         type describes.
		''' </summary>
		Private className As String

		''' <summary>
		''' @serial The type description (should not be null or empty).
		''' </summary>
		Private description As String

		''' <summary>
		''' @serial The name given to this type (should not be null or empty).
		''' </summary>
		Private typeName As String

		''' <summary>
		''' Tells if this type describes an array (checked in constructor).
		''' </summary>
		<NonSerialized> _
		Private ___isArray As Boolean = False

		''' <summary>
		''' Cached Descriptor for this OpenType, constructed on demand.
		''' </summary>
		<NonSerialized> _
		Private descriptor As javax.management.Descriptor

		' *** Constructor *** 

		''' <summary>
		''' Constructs an <code>OpenType</code> instance (actually a subclass instance as <code>OpenType</code> is abstract),
		''' checking for the validity of the given parameters.
		''' The validity constraints are described below for each parameter.
		''' <br>&nbsp; </summary>
		''' <param name="className">  The fully qualified Java class name of the open data values this open type describes.
		'''                    The valid Java class names allowed for open data values are listed in
		'''                    <seealso cref="#ALLOWED_CLASSNAMES_LIST ALLOWED_CLASSNAMES_LIST"/>.
		'''                    A multidimensional array of any one of these classes
		'''                    or their corresponding primitive types is also an allowed class,
		'''                    in which case the class name follows the rules defined by the method
		'''                    <seealso cref="Class#getName() getName()"/> of <code>java.lang.Class</code>.
		'''                    For example, a 3-dimensional array of Strings has for class name
		'''                    &quot;<code>[[[Ljava.lang.String;</code>&quot; (without the quotes).
		''' <br>&nbsp; </param>
		''' <param name="typeName">  The name given to the open type this instance represents; cannot be a null or empty string.
		''' <br>&nbsp; </param>
		''' <param name="description">  The human readable description of the open type this instance represents;
		'''                      cannot be a null or empty string.
		''' <br>&nbsp; </param>
		''' <exception cref="IllegalArgumentException">  if <var>className</var>, <var>typeName</var> or <var>description</var>
		'''                                   is a null or empty string
		''' <br>&nbsp; </exception>
		''' <exception cref="OpenDataException">  if <var>className</var> is not one of the allowed Java class names for open data </exception>
		Protected Friend Sub New(ByVal className As String, ByVal typeName As String, ByVal description As String)
			checkClassNameOverride()
			Me.typeName = valid("typeName", typeName)
			Me.description = valid("description", description)
			Me.className = validClassName(className)
			Me.___isArray = (Me.className IsNot Nothing AndAlso Me.className.StartsWith("["))
		End Sub

		' Package-private constructor for callers we trust to get it right. 
		Friend Sub New(ByVal className As String, ByVal typeName As String, ByVal description As String, ByVal isArray As Boolean)
			Me.className = valid("className",className)
			Me.typeName = valid("typeName", typeName)
			Me.description = valid("description", description)
			Me.___isArray = isArray
		End Sub

		Private Sub checkClassNameOverride()
			If Me.GetType().classLoader Is Nothing Then Return ' We trust bootstrap classes.
			If overridesGetClassName(Me.GetType()) Then
				Dim getExtendOpenTypes As New com.sun.jmx.mbeanserver.GetPropertyAction("jmx.extend.open.types")
				If java.security.AccessController.doPrivileged(getExtendOpenTypes) Is Nothing Then Throw New SecurityException("Cannot override getClassName() " & "unless -Djmx.extend.open.types")
			End If
		End Sub

		Private Shared Function overridesGetClassName(ByVal c As Type) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<java.lang.Boolean>()
	'		{
	'			public java.lang.Boolean run()
	'			{
	'				try
	'				{
	'					Return (c.getMethod("getClassName").getDeclaringClass() != OpenType.class);
	'				}
	'				catch (Exception e)
	'				{
	'					Return True; ' fail safe
	'				}
	'			}
	'		});
		End Function

		Private Shared Function validClassName(ByVal className As String) As String
			className = valid("className", className)

			' Check if className describes an array class, and determines its elements' class name.
			' (eg: a 3-dimensional array of Strings has for class name: "[[[Ljava.lang.String;")
			'
			Dim n As Integer = 0
			Do While className.StartsWith("[", n)
				n += 1
			Loop
			Dim eltClassName As String ' class name of array elements
			Dim isPrimitiveArray As Boolean = False
			If n > 0 Then
				If className.StartsWith("L", n) AndAlso className.EndsWith(";") Then
					' removes the n leading '[' + the 'L' characters
					' and the last ';' character
					eltClassName = className.Substring(n+1, className.Length-1 - (n+1))
				ElseIf n = className.Length - 1 Then
					' removes the n leading '[' characters
					eltClassName = className.Substring(n, className.Length - n)
					isPrimitiveArray = True
				Else
					Throw New OpenDataException("Argument className=""" & className & """ is not a valid class name")
				End If
			Else
				' not an array
				eltClassName = className
			End If

			' Check that eltClassName's value is one of the allowed basic data types for open data
			'
			Dim ok As Boolean = False
			If isPrimitiveArray Then
				ok = ArrayType.isPrimitiveContentType(eltClassName)
			Else
				ok = ALLOWED_CLASSNAMES_LIST.Contains(eltClassName)
			End If
			If Not ok Then Throw New OpenDataException("Argument className=""" & className & """ is not one of the allowed Java class names for open data.")

			Return className
		End Function

	'     Return argValue.trim() provided argValue is neither null nor empty;
	'       otherwise throw IllegalArgumentException.  
		Private Shared Function valid(ByVal argName As String, ByVal argValue As String) As String
			argValue = argValue.Trim()
			If argValue Is Nothing OrElse argValue .Equals("") Then Throw New System.ArgumentException("Argument " & argName & " cannot be null or empty")
			Return argValue
		End Function

		' Package-private access to a Descriptor containing this OpenType. 
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Property descriptor As javax.management.Descriptor
			Get
				If descriptor Is Nothing Then descriptor = New javax.management.ImmutableDescriptor(New String() {"openType"}, New Object() {Me})
				Return descriptor
			End Get
		End Property

		' *** Open type information methods *** 

		''' <summary>
		''' Returns the fully qualified Java class name of the open data values
		''' this open type describes.
		''' The only possible Java class names for open data values are listed in
		''' <seealso cref="#ALLOWED_CLASSNAMES_LIST ALLOWED_CLASSNAMES_LIST"/>.
		''' A multidimensional array of any one of these classes or their
		''' corresponding primitive types is also an allowed class,
		''' in which case the class name follows the rules defined by the method
		''' <seealso cref="Class#getName() getName()"/> of <code>java.lang.Class</code>.
		''' For example, a 3-dimensional array of Strings has for class name
		''' &quot;<code>[[[Ljava.lang.String;</code>&quot; (without the quotes),
		''' a 3-dimensional array of Integers has for class name
		''' &quot;<code>[[[Ljava.lang.Integer;</code>&quot; (without the quotes),
		''' and a 3-dimensional array of int has for class name
		''' &quot;<code>[[[I</code>&quot; (without the quotes)
		''' </summary>
		''' <returns> the class name. </returns>
		Public Overridable Property className As String
			Get
				Return className
			End Get
		End Property

		' A version of getClassName() that can only be called from within this
		' package and that cannot be overridden.
		Friend Overridable Function safeGetClassName() As String
			Return className
		End Function

		''' <summary>
		''' Returns the name of this <code>OpenType</code> instance.
		''' </summary>
		''' <returns> the type name. </returns>
		Public Overridable Property typeName As String
			Get
    
				Return typeName
			End Get
		End Property

		''' <summary>
		''' Returns the text description of this <code>OpenType</code> instance.
		''' </summary>
		''' <returns> the description. </returns>
		Public Overridable Property description As String
			Get
    
				Return description
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the open data values this open
		''' type describes are arrays, <code>false</code> otherwise.
		''' </summary>
		''' <returns> true if this is an array type. </returns>
		Public Overridable Property array As Boolean
			Get
    
				Return ___isArray
			End Get
		End Property

		''' <summary>
		''' Tests whether <var>obj</var> is a value for this open type.
		''' </summary>
		''' <param name="obj"> the object to be tested for validity.
		''' </param>
		''' <returns> <code>true</code> if <var>obj</var> is a value for this
		''' open type, <code>false</code> otherwise. </returns>
		Public MustOverride Function isValue(ByVal obj As Object) As Boolean

		''' <summary>
		''' Tests whether values of the given type can be assigned to this open type.
		''' The default implementation of this method returns true only if the
		''' types are equal.
		''' </summary>
		''' <param name="ot"> the type to be tested.
		''' </param>
		''' <returns> true if {@code ot} is assignable to this open type. </returns>
		Friend Overridable Function isAssignableFrom(Of T1)(ByVal ot As OpenType(Of T1)) As Boolean
			Return Me.Equals(ot)
		End Function

		' *** Methods overriden from class Object *** 

		''' <summary>
		''' Compares the specified <code>obj</code> parameter with this
		''' open type instance for equality.
		''' </summary>
		''' <param name="obj"> the object to compare to.
		''' </param>
		''' <returns> true if this object and <code>obj</code> are equal. </returns>
		Public MustOverride Function Equals(ByVal obj As Object) As Boolean

		Public MustOverride Function GetHashCode() As Integer

		''' <summary>
		''' Returns a string representation of this open type instance.
		''' </summary>
		''' <returns> the string representation. </returns>
		Public MustOverride Function ToString() As String

		''' <summary>
		''' Deserializes an <seealso cref="OpenType"/> from an <seealso cref="java.io.ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			checkClassNameOverride()
			Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
			Dim classNameField As String
			Dim descriptionField As String
			Dim typeNameField As String
			Try
				classNameField = validClassName(CStr(fields.get("className", Nothing)))
				descriptionField = valid("description", CStr(fields.get("description", Nothing)))
				typeNameField = valid("typeName", CStr(fields.get("typeName", Nothing)))
			Catch e As Exception
				Dim e2 As java.io.IOException = New java.io.InvalidObjectException(e.Message)
				e2.initCause(e)
				Throw e2
			End Try
			className = classNameField
			description = descriptionField
			typeName = typeNameField
			___isArray = (className.StartsWith("["))
		End Sub
	End Class

End Namespace
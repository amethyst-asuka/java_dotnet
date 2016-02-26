Imports System
Imports System.Text

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


	''' <summary>
	''' The <code>ArrayType</code> class is the <i>open type</i> class whose instances describe
	''' all <i>open data</i> values which are n-dimensional arrays of <i>open data</i> values.
	''' <p>
	''' Examples of valid {@code ArrayType} instances are:
	''' <pre>{@code
	''' // 2-dimension array of java.lang.String
	''' ArrayType<String[][]> a1 = new ArrayType<String[][]>(2, SimpleType.STRING);
	''' 
	''' // 1-dimension array of int
	''' ArrayType<int[]> a2 = new ArrayType<int[]>(SimpleType.INTEGER, true);
	''' 
	''' // 1-dimension array of java.lang.Integer
	''' ArrayType<Integer[]> a3 = new ArrayType<Integer[]>(SimpleType.INTEGER, false);
	''' 
	''' // 4-dimension array of int
	''' ArrayType<int[][][][]> a4 = new ArrayType<int[][][][]>(3, a2);
	''' 
	''' // 4-dimension array of java.lang.Integer
	''' ArrayType<Integer[][][][]> a5 = new ArrayType<Integer[][][][]>(3, a3);
	''' 
	''' // 1-dimension array of java.lang.String
	''' ArrayType<String[]> a6 = new ArrayType<String[]>(SimpleType.STRING, false);
	''' 
	''' // 1-dimension array of long
	''' ArrayType<long[]> a7 = new ArrayType<long[]>(SimpleType.LONG, true);
	''' 
	''' // 1-dimension array of java.lang.Integer
	''' ArrayType<Integer[]> a8 = ArrayType.getArrayType(SimpleType.INTEGER);
	''' 
	''' // 2-dimension array of java.lang.Integer
	''' ArrayType<Integer[][]> a9 = ArrayType.getArrayType(a8);
	''' 
	''' // 2-dimension array of int
	''' ArrayType<int[][]> a10 = ArrayType.getPrimitiveArrayType(int[][].class);
	''' 
	''' // 3-dimension array of int
	''' ArrayType<int[][][]> a11 = ArrayType.getArrayType(a10);
	''' 
	''' // 1-dimension array of float
	''' ArrayType<float[]> a12 = ArrayType.getPrimitiveArrayType(float[].class);
	''' 
	''' // 2-dimension array of float
	''' ArrayType<float[][]> a13 = ArrayType.getArrayType(a12);
	''' 
	''' // 1-dimension array of javax.management.ObjectName
	''' ArrayType<ObjectName[]> a14 = ArrayType.getArrayType(SimpleType.OBJECTNAME);
	''' 
	''' // 2-dimension array of javax.management.ObjectName
	''' ArrayType<ObjectName[][]> a15 = ArrayType.getArrayType(a14);
	''' 
	''' // 3-dimension array of java.lang.String
	''' ArrayType<String[][][]> a16 = new ArrayType<String[][][]>(3, SimpleType.STRING);
	''' 
	''' // 1-dimension array of java.lang.String
	''' ArrayType<String[]> a17 = new ArrayType<String[]>(1, SimpleType.STRING);
	''' 
	''' // 2-dimension array of java.lang.String
	''' ArrayType<String[][]> a18 = new ArrayType<String[][]>(1, a17);
	''' 
	''' // 3-dimension array of java.lang.String
	''' ArrayType<String[][][]> a19 = new ArrayType<String[][][]>(1, a18);
	''' }</pre>
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	'
	'  Generification note: we could have defined a type parameter that is the
	'  element type, with class ArrayType<E> extends OpenType<E[]>.  However,
	'  that doesn't buy us all that much.  We can't say
	'    public OpenType<E> getElementOpenType()
	'  because this ArrayType could be a multi-dimensional array.
	'  For example, if we had
	'    ArrayType(2, SimpleType.INTEGER)
	'  then E would have to be Integer[], while getElementOpenType() would
	'  return SimpleType.INTEGER, which is an OpenType<Integer>.
	'
	'  Furthermore, we would like to support int[] (as well as Integer[]) as
	'  an Open Type (RFE 5045358).  We would want this to be an OpenType<int[]>
	'  which can't be expressed as <E[]> because E can't be a primitive type
	'  like int.
	' 
	Public Class ArrayType(Of T)
		Inherits OpenType(Of T)

		' Serial version 
		Friend Const serialVersionUID As Long = 720504429830309770L

		''' <summary>
		''' @serial The dimension of arrays described by this <seealso cref="ArrayType"/>
		'''         instance.
		''' </summary>
		Private dimension As Integer

		''' <summary>
		''' @serial The <i>open type</i> of element values contained in the arrays
		'''         described by this <seealso cref="ArrayType"/> instance.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private elementType As OpenType(Of ?)

		''' <summary>
		''' @serial This flag indicates whether this <seealso cref="ArrayType"/>
		'''         describes a primitive array.
		''' 
		''' @since 1.6
		''' </summary>
		Private primitiveArray As Boolean

		<NonSerialized> _
		Private myHashCode As Integer? = Nothing ' As this instance is immutable, these two values
		<NonSerialized> _
		Private myToString As String = Nothing ' need only be calculated once.

		' indexes refering to columns in the PRIMITIVE_ARRAY_TYPES table.
		Private Const PRIMITIVE_WRAPPER_NAME_INDEX As Integer = 0
		Private Const PRIMITIVE_TYPE_NAME_INDEX As Integer = 1
		Private Const PRIMITIVE_TYPE_KEY_INDEX As Integer = 2
		Private Const PRIMITIVE_OPEN_TYPE_INDEX As Integer = 3

		Private Shared ReadOnly PRIMITIVE_ARRAY_TYPES As Object()() = { New Object() { GetType(Boolean).name, GetType(Boolean).name, "Z", SimpleType.BOOLEAN }, New Object() { GetType(Char?).name, GetType(Char).name, "C", SimpleType.CHARACTER }, New Object() { GetType(SByte?).name, GetType(SByte).name, "B", SimpleType.BYTE }, New Object() { GetType(Short).name, GetType(Short).name, "S", SimpleType.SHORT }, New Object() { GetType(Integer).name, GetType(Integer).name, "I", SimpleType.INTEGER }, New Object() { GetType(Long).name, GetType(Long).name, "J", SimpleType.LONG }, New Object() { GetType(Single?).name, GetType(Single).name, "F", SimpleType.FLOAT }, New Object() { GetType(Double).name, GetType(Double).name, "D", SimpleType.DOUBLE } }

		Friend Shared Function isPrimitiveContentType(ByVal primitiveKey As String) As Boolean
			For Each typeDescr As Object() In PRIMITIVE_ARRAY_TYPES
				If typeDescr(PRIMITIVE_TYPE_KEY_INDEX).Equals(primitiveKey) Then Return True
			Next typeDescr
			Return False
		End Function

		''' <summary>
		''' Return the key used to identify the element type in
		''' arrays - e.g. "Z" for boolean, "C" for char etc... </summary>
		''' <param name="elementClassName"> the wrapper class name of the array
		'''        element ("Boolean",  "Character", etc...) </param>
		''' <returns> the key corresponding to the given type ("Z", "C", etc...)
		'''         return null if the given elementClassName is not a primitive
		'''         wrapper class name.
		'''  </returns>
		Friend Shared Function getPrimitiveTypeKey(ByVal elementClassName As String) As String
			For Each typeDescr As Object() In PRIMITIVE_ARRAY_TYPES
				If elementClassName.Equals(typeDescr(PRIMITIVE_WRAPPER_NAME_INDEX)) Then Return CStr(typeDescr(PRIMITIVE_TYPE_KEY_INDEX))
			Next typeDescr
			Return Nothing
		End Function

		''' <summary>
		''' Return the primitive type name corresponding to the given wrapper class.
		''' e.g. "boolean" for "Boolean", "char" for "Character" etc... </summary>
		''' <param name="elementClassName"> the type of the array element ("Boolean",
		'''        "Character", etc...) </param>
		''' <returns> the primitive type name corresponding to the given wrapper class
		'''         ("boolean", "char", etc...)
		'''         return null if the given elementClassName is not a primitive
		'''         wrapper type name.
		'''  </returns>
		Friend Shared Function getPrimitiveTypeName(ByVal elementClassName As String) As String
			For Each typeDescr As Object() In PRIMITIVE_ARRAY_TYPES
				If elementClassName.Equals(typeDescr(PRIMITIVE_WRAPPER_NAME_INDEX)) Then Return CStr(typeDescr(PRIMITIVE_TYPE_NAME_INDEX))
			Next typeDescr
			Return Nothing
		End Function

		''' <summary>
		''' Return the primitive open type corresponding to the given primitive type.
		''' e.g. SimpleType.BOOLEAN for "boolean", SimpleType.CHARACTER for
		''' "char", etc... </summary>
		''' <param name="primitiveTypeName"> the primitive type of the array element ("boolean",
		'''        "char", etc...) </param>
		''' <returns> the OpenType corresponding to the given primitive type name
		'''         (SimpleType.BOOLEAN, SimpleType.CHARACTER, etc...)
		'''         return null if the given elementClassName is not a primitive
		'''         type name.
		'''  </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend Shared Function getPrimitiveOpenType(ByVal primitiveTypeName As String) As SimpleType(Of ?)
			For Each typeDescr As Object() In PRIMITIVE_ARRAY_TYPES
				If primitiveTypeName.Equals(typeDescr(PRIMITIVE_TYPE_NAME_INDEX)) Then Return CType(typeDescr(PRIMITIVE_OPEN_TYPE_INDEX), SimpleType(Of ?))
			Next typeDescr
			Return Nothing
		End Function

		' *** Constructor *** 

		''' <summary>
		''' Constructs an <tt>ArrayType</tt> instance describing <i>open data</i> values which are
		''' arrays with dimension <var>dimension</var> of elements whose <i>open type</i> is <var>elementType</var>.
		''' <p>
		''' When invoked on an <tt>ArrayType</tt> instance, the <seealso cref="OpenType#getClassName() getClassName"/> method
		''' returns the class name of the array instances it describes (following the rules defined by the
		''' <seealso cref="Class#getName() getName"/> method of <code>java.lang.Class</code>), not the class name of the array elements
		''' (which is returned by a call to <tt>getElementOpenType().getClassName()</tt>).
		''' <p>
		''' The internal field corresponding to the type name of this <code>ArrayType</code> instance is also set to
		''' the class name of the array instances it describes.
		''' In other words, the methods <code>getClassName</code> and <code>getTypeName</code> return the same string value.
		''' The internal field corresponding to the description of this <code>ArrayType</code> instance is set to a string value
		''' which follows the following template:
		''' <ul>
		''' <li>if non-primitive array: <tt><i>&lt;dimension&gt;</i>-dimension array of <i>&lt;element_class_name&gt;</i></tt></li>
		''' <li>if primitive array: <tt><i>&lt;dimension&gt;</i>-dimension array of <i>&lt;primitive_type_of_the_element_class_name&gt;</i></tt></li>
		''' </ul>
		''' <p>
		''' As an example, the following piece of code:
		''' <pre>{@code
		''' ArrayType<String[][][]> t = new ArrayType<String[][][]>(3, SimpleType.STRING);
		''' System.out.println("array class name       = " + t.getClassName());
		''' System.out.println("element class name     = " + t.getElementOpenType().getClassName());
		''' System.out.println("array type name        = " + t.getTypeName());
		''' System.out.println("array type description = " + t.getDescription());
		''' }</pre>
		''' would produce the following output:
		''' <pre>{@code
		''' array class name       = [[[Ljava.lang.String;
		''' element class name     = java.lang.String
		''' array type name        = [[[Ljava.lang.String;
		''' array type description = 3-dimension array of java.lang.String
		''' }</pre>
		''' And the following piece of code which is equivalent to the one listed
		''' above would also produce the same output:
		''' <pre>{@code
		''' ArrayType<String[]> t1 = new ArrayType<String[]>(1, SimpleType.STRING);
		''' ArrayType<String[][]> t2 = new ArrayType<String[][]>(1, t1);
		''' ArrayType<String[][][]> t3 = new ArrayType<String[][][]>(1, t2);
		''' System.out.println("array class name       = " + t3.getClassName());
		''' System.out.println("element class name     = " + t3.getElementOpenType().getClassName());
		''' System.out.println("array type name        = " + t3.getTypeName());
		''' System.out.println("array type description = " + t3.getDescription());
		''' }</pre>
		''' </summary>
		''' <param name="dimension">  the dimension of arrays described by this <tt>ArrayType</tt> instance;
		'''                    must be greater than or equal to 1.
		''' </param>
		''' <param name="elementType">  the <i>open type</i> of element values contained
		'''                      in the arrays described by this <tt>ArrayType</tt>
		'''                      instance; must be an instance of either
		'''                      <tt>SimpleType</tt>, <tt>CompositeType</tt>,
		'''                      <tt>TabularType</tt> or another <tt>ArrayType</tt>
		'''                      with a <tt>SimpleType</tt>, <tt>CompositeType</tt>
		'''                      or <tt>TabularType</tt> as its <tt>elementType</tt>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code dimension} is not a positive
		'''                                  integer. </exception>
		''' <exception cref="OpenDataException">  if <var>elementType's className</var> is not
		'''                            one of the allowed Java class names for open
		'''                            data. </exception>
		Public Sub New(Of T1)(ByVal dimension As Integer, ByVal elementType As OpenType(Of T1))
			' Check and construct state defined by parent.
			' We can't use the package-private OpenType constructor because
			' we don't know if the elementType parameter is sane.
			MyBase.New(buildArrayClassName(dimension, elementType), buildArrayClassName(dimension, elementType), buildArrayDescription(dimension, elementType))

			' Check and construct state specific to ArrayType
			'
			If elementType.array Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim at As ArrayType(Of ?) = CType(elementType, ArrayType(Of ?))
				Me.dimension = at.dimension + dimension
				Me.elementType = at.elementOpenType
				Me.primitiveArray = at.primitiveArray
			Else
				Me.dimension = dimension
				Me.elementType = elementType
				Me.primitiveArray = False
			End If
		End Sub

		''' <summary>
		''' Constructs a unidimensional {@code ArrayType} instance for the
		''' supplied {@code SimpleType}.
		''' <p>
		''' This constructor supports the creation of arrays of primitive
		''' types when {@code primitiveArray} is {@code true}.
		''' <p>
		''' For primitive arrays the <seealso cref="#getElementOpenType()"/> method
		''' returns the <seealso cref="SimpleType"/> corresponding to the wrapper
		''' type of the primitive type of the array.
		''' <p>
		''' When invoked on an <tt>ArrayType</tt> instance, the <seealso cref="OpenType#getClassName() getClassName"/> method
		''' returns the class name of the array instances it describes (following the rules defined by the
		''' <seealso cref="Class#getName() getName"/> method of <code>java.lang.Class</code>), not the class name of the array elements
		''' (which is returned by a call to <tt>getElementOpenType().getClassName()</tt>).
		''' <p>
		''' The internal field corresponding to the type name of this <code>ArrayType</code> instance is also set to
		''' the class name of the array instances it describes.
		''' In other words, the methods <code>getClassName</code> and <code>getTypeName</code> return the same string value.
		''' The internal field corresponding to the description of this <code>ArrayType</code> instance is set to a string value
		''' which follows the following template:
		''' <ul>
		''' <li>if non-primitive array: <tt>1-dimension array of <i>&lt;element_class_name&gt;</i></tt></li>
		''' <li>if primitive array: <tt>1-dimension array of <i>&lt;primitive_type_of_the_element_class_name&gt;</i></tt></li>
		''' </ul>
		''' <p>
		''' As an example, the following piece of code:
		''' <pre>{@code
		''' ArrayType<int[]> t = new ArrayType<int[]>(SimpleType.INTEGER, true);
		''' System.out.println("array class name       = " + t.getClassName());
		''' System.out.println("element class name     = " + t.getElementOpenType().getClassName());
		''' System.out.println("array type name        = " + t.getTypeName());
		''' System.out.println("array type description = " + t.getDescription());
		''' }</pre>
		''' would produce the following output:
		''' <pre>{@code
		''' array class name       = [I
		''' element class name     = java.lang.Integer
		''' array type name        = [I
		''' array type description = 1-dimension array of int
		''' }</pre>
		''' </summary>
		''' <param name="elementType"> the {@code SimpleType} of the element values
		'''                    contained in the arrays described by this
		'''                    {@code ArrayType} instance.
		''' </param>
		''' <param name="primitiveArray"> {@code true} when this array describes
		'''                       primitive arrays.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code dimension} is not a positive
		''' integer. </exception>
		''' <exception cref="OpenDataException"> if {@code primitiveArray} is {@code true} and
		''' {@code elementType} is not a valid {@code SimpleType} for a primitive
		''' type.
		''' 
		''' @since 1.6 </exception>
		Public Sub New(Of T1)(ByVal elementType As SimpleType(Of T1), ByVal primitiveArray As Boolean)

			' Check and construct state defined by parent.
			' We can call the package-private OpenType constructor because the
			' set of SimpleTypes is fixed and SimpleType can't be subclassed.
			MyBase.New(buildArrayClassName(1, elementType, primitiveArray), buildArrayClassName(1, elementType, primitiveArray), buildArrayDescription(1, elementType, primitiveArray), True)

			' Check and construct state specific to ArrayType
			'
			Me.dimension = 1
			Me.elementType = elementType
			Me.primitiveArray = primitiveArray
		End Sub

		' Package-private constructor for callers we trust to get it right. 
		Friend Sub New(Of T1)(ByVal className As String, ByVal typeName As String, ByVal description As String, ByVal dimension As Integer, ByVal elementType As OpenType(Of T1), ByVal primitiveArray As Boolean)
			MyBase.New(className, typeName, description, True)
			Me.dimension = dimension
			Me.elementType = elementType
			Me.primitiveArray = primitiveArray
		End Sub

		Private Shared Function buildArrayClassName(Of T1)(ByVal dimension As Integer, ByVal elementType As OpenType(Of T1)) As String
			Dim isPrimitiveArray As Boolean = False
			If elementType.array Then isPrimitiveArray = CType(elementType, ArrayType(Of ?)).primitiveArray
			Return buildArrayClassName(dimension, elementType, isPrimitiveArray)
		End Function

		Private Shared Function buildArrayClassName(Of T1)(ByVal dimension As Integer, ByVal elementType As OpenType(Of T1), ByVal isPrimitiveArray As Boolean) As String
			If dimension < 1 Then Throw New System.ArgumentException("Value of argument dimension must be greater than 0")
			Dim result As New StringBuilder
			Dim elementClassName As String = elementType.className
			' Add N (= dimension) additional '[' characters to the existing array
			For i As Integer = 1 To dimension
				result.Append("["c)
			Next i
			If elementType.array Then
				result.Append(elementClassName)
			Else
				If isPrimitiveArray Then
					Dim key As String = getPrimitiveTypeKey(elementClassName)
					' Ideally we should throw an IllegalArgumentException here,
					' but for compatibility reasons we throw an OpenDataException.
					' (used to be thrown by OpenType() constructor).
					'
					If key Is Nothing Then Throw New OpenDataException("Element type is not primitive: " & elementClassName)
					result.Append(key)
				Else
					result.Append("L")
					result.Append(elementClassName)
					result.Append(";"c)
				End If
			End If
			Return result.ToString()
		End Function

		Private Shared Function buildArrayDescription(Of T1)(ByVal dimension As Integer, ByVal elementType As OpenType(Of T1)) As String
			Dim isPrimitiveArray As Boolean = False
			If elementType.array Then isPrimitiveArray = CType(elementType, ArrayType(Of ?)).primitiveArray
			Return buildArrayDescription(dimension, elementType, isPrimitiveArray)
		End Function

		Private Shared Function buildArrayDescription(Of T1)(ByVal dimension As Integer, ByVal elementType As OpenType(Of T1), ByVal isPrimitiveArray As Boolean) As String
			If elementType.array Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim at As ArrayType(Of ?) = CType(elementType, ArrayType(Of ?))
				dimension += at.dimension
				elementType = at.elementOpenType
				isPrimitiveArray = at.primitiveArray
			End If
			Dim result As New StringBuilder(dimension & "-dimension array of ")
			Dim elementClassName As String = elementType.className
			If isPrimitiveArray Then
				' Convert from wrapper type to primitive type
				Dim primitiveType As String = getPrimitiveTypeName(elementClassName)

				' Ideally we should throw an IllegalArgumentException here,
				' but for compatibility reasons we throw an OpenDataException.
				' (used to be thrown by OpenType() constructor).
				'
				If primitiveType Is Nothing Then Throw New OpenDataException("Element is not a primitive type: " & elementClassName)
				result.Append(primitiveType)
			Else
				result.Append(elementClassName)
			End If
			Return result.ToString()
		End Function

		' *** ArrayType specific information methods *** 

		''' <summary>
		''' Returns the dimension of arrays described by this <tt>ArrayType</tt> instance.
		''' </summary>
		''' <returns> the dimension. </returns>
		Public Overridable Property dimension As Integer
			Get
    
				Return dimension
			End Get
		End Property

		''' <summary>
		''' Returns the <i>open type</i> of element values contained in the arrays described by this <tt>ArrayType</tt> instance.
		''' </summary>
		''' <returns> the element type. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property elementOpenType As OpenType(Of ?)
			Get
    
				Return elementType
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the open data values this open
		''' type describes are primitive arrays, <code>false</code> otherwise.
		''' </summary>
		''' <returns> true if this is a primitive array type.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property primitiveArray As Boolean
			Get
    
				Return primitiveArray
			End Get
		End Property

		''' <summary>
		''' Tests whether <var>obj</var> is a value for this <code>ArrayType</code>
		''' instance.
		''' <p>
		''' This method returns <code>true</code> if and only if <var>obj</var>
		''' is not null, <var>obj</var> is an array and any one of the following
		''' is <tt>true</tt>:
		''' 
		''' <ul>
		''' <li>if this <code>ArrayType</code> instance describes an array of
		''' <tt>SimpleType</tt> elements or their corresponding primitive types,
		''' <var>obj</var>'s class name is the same as the className field defined
		''' for this <code>ArrayType</code> instance (i.e. the class name returned
		''' by the <seealso cref="OpenType#getClassName() getClassName"/> method, which
		''' includes the dimension information),<br>&nbsp;</li>
		''' <li>if this <code>ArrayType</code> instance describes an array of
		''' classes implementing the {@code TabularData} interface or the
		''' {@code CompositeData} interface, <var>obj</var> is assignable to
		''' such a declared array, and each element contained in {<var>obj</var>
		''' is either null or a valid value for the element's open type specified
		''' by this <code>ArrayType</code> instance.</li>
		''' </ul>
		''' </summary>
		''' <param name="obj"> the object to be tested.
		''' </param>
		''' <returns> <code>true</code> if <var>obj</var> is a value for this
		''' <code>ArrayType</code> instance. </returns>
		Public Overridable Function isValue(ByVal obj As Object) As Boolean

			' if obj is null, return false
			'
			If obj Is Nothing Then Return False

			Dim objClass As Type = obj.GetType()
			Dim objClassName As String = objClass.name

			' if obj is not an array, return false
			'
			If Not objClass.IsArray Then Return False

			' Test if obj's class name is the same as for the array values that this instance describes
			' (this is fine if elements are of simple types, which are final classes)
			'
			If Me.className.Equals(objClassName) Then Return True

			' In case this ArrayType instance describes an array of classes implementing the TabularData or CompositeData interface,
			' we first check for the assignability of obj to such an array of TabularData or CompositeData,
			' which ensures that:
			'  . obj is of the the same dimension as this ArrayType instance,
			'  . it is declared as an array of elements which are either all TabularData or all CompositeData.
			'
			' If the assignment check is positive,
			' then we have to check that each element in obj is of the same TabularType or CompositeType
			' as the one described by this ArrayType instance.
			'
			' [About assignment check, note that the call below returns true: ]
			' [Class.forName("[Lpackage.CompositeData;").isAssignableFrom(Class.forName("[Lpackage.CompositeDataImpl;)")); ]
			'
			If (Me.elementType.className.Equals(GetType(TabularData).name)) OrElse (Me.elementType.className.Equals(GetType(CompositeData).name)) Then

				Dim isTabular As Boolean = (elementType.className.Equals(GetType(TabularData).name))
				Dim dims As Integer() = New Integer(dimension - 1){}
				Dim elementClass As Type = If(isTabular, GetType(TabularData), GetType(CompositeData))
				Dim targetClass As Type = Array.newInstance(elementClass, dims).GetType()

				' assignment check: return false if negative
				If Not targetClass.IsAssignableFrom(objClass) Then Return False

				' check that all elements in obj are valid values for this ArrayType
				If Not checkElementsType(CType(obj, Object()), Me.dimension) Then ' we know obj's dimension is this.dimension Return False

				Return True
			End If

			' if previous tests did not return, then obj is not a value for this ArrayType instance
			Return False
		End Function

		''' <summary>
		''' Returns true if and only if all elements contained in the array argument x_dim_Array of dimension dim
		''' are valid values (ie either null or of the right openType)
		''' for the element open type specified by this ArrayType instance.
		''' 
		''' This method's implementation uses recursion to go down the dimensions of the array argument.
		''' </summary>
		Private Function checkElementsType(ByVal x_dim_Array As Object(), ByVal [dim] As Integer) As Boolean

			' if the elements of x_dim_Array are themselves array: go down recursively....
			If [dim] > 1 Then
				For i As Integer = 0 To x_dim_Array.Length - 1
					If Not checkElementsType(CType(x_dim_Array(i), Object()), [dim]-1) Then Return False
				Next i
				Return True
			' ...else, for a non-empty array, each element must be a valid value: either null or of the right openType
			Else
				For i As Integer = 0 To x_dim_Array.Length - 1
					If (x_dim_Array(i) IsNot Nothing) AndAlso ((Not Me.elementOpenType.isValue(x_dim_Array(i)))) Then Return False
				Next i
				Return True
			End If
		End Function

		Friend Overrides Function isAssignableFrom(Of T1)(ByVal ot As OpenType(Of T1)) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If Not(TypeOf ot Is ArrayType(Of ?)) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim at As ArrayType(Of ?) = CType(ot, ArrayType(Of ?))
			Return (at.dimension = dimension AndAlso at.primitiveArray = primitiveArray AndAlso elementOpenType.IsSubclassOf(at.elementOpenType))
		End Function


		' *** Methods overriden from class Object *** 

		''' <summary>
		''' Compares the specified <code>obj</code> parameter with this
		''' <code>ArrayType</code> instance for equality.
		''' <p>
		''' Two <code>ArrayType</code> instances are equal if and only if they
		''' describe array instances which have the same dimension, elements'
		''' open type and primitive array flag.
		''' </summary>
		''' <param name="obj"> the object to be compared for equality with this
		'''            <code>ArrayType</code> instance; if <var>obj</var>
		'''            is <code>null</code> or is not an instance of the
		'''            class <code>ArrayType</code> this method returns
		'''            <code>false</code>.
		''' </param>
		''' <returns> <code>true</code> if the specified object is equal to
		'''         this <code>ArrayType</code> instance. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean

			' if obj is null, return false
			'
			If obj Is Nothing Then Return False

			' if obj is not an ArrayType, return false
			'
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If Not(TypeOf obj Is ArrayType(Of ?)) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim other As ArrayType(Of ?) = CType(obj, ArrayType(Of ?))

			' if other's dimension is different than this instance's, return false
			'
			If Me.dimension <> other.dimension Then Return False

			' Test if other's elementType field is the same as for this instance
			'
			If Not Me.elementType.Equals(other.elementType) Then Return False

			' Test if other's primitiveArray flag is the same as for this instance
			'
			Return Me.primitiveArray = other.primitiveArray
		End Function

		''' <summary>
		''' Returns the hash code value for this <code>ArrayType</code> instance.
		''' <p>
		''' The hash code of an <code>ArrayType</code> instance is the sum of the
		''' hash codes of all the elements of information used in <code>equals</code>
		''' comparisons (i.e. dimension, elements' open type and primitive array flag).
		''' The hashcode for a primitive value is the hashcode of the corresponding boxed
		''' object (e.g. the hashcode for <tt>true</tt> is <tt>Boolean.TRUE.hashCode()</tt>).
		''' This ensures that <code> t1.equals(t2) </code> implies that
		''' <code> t1.hashCode()==t2.hashCode() </code> for any two
		''' <code>ArrayType</code> instances <code>t1</code> and <code>t2</code>,
		''' as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.
		''' <p>
		''' As <code>ArrayType</code> instances are immutable, the hash
		''' code for this instance is calculated once, on the first call
		''' to <code>hashCode</code>, and then the same value is returned
		''' for subsequent calls.
		''' </summary>
		''' <returns>  the hash code value for this <code>ArrayType</code> instance </returns>
		Public Overrides Function GetHashCode() As Integer

			' Calculate the hash code value if it has not yet been done (ie 1st call to hashCode())
			'
			If myHashCode Is Nothing Then
				Dim ___value As Integer = 0
				___value += dimension
				___value += elementType.GetHashCode()
				___value += Convert.ToBoolean(primitiveArray).GetHashCode()
				myHashCode = Convert.ToInt32(___value)
			End If

			' return always the same hash code for this instance (immutable)
			'
			Return myHashCode
		End Function

		''' <summary>
		''' Returns a string representation of this <code>ArrayType</code> instance.
		''' <p>
		''' The string representation consists of the name of this class (i.e.
		''' <code>javax.management.openmbean.ArrayType</code>), the type name,
		''' the dimension, the elements' open type and the primitive array flag
		''' defined for this instance.
		''' <p>
		''' As <code>ArrayType</code> instances are immutable, the
		''' string representation for this instance is calculated
		''' once, on the first call to <code>toString</code>, and
		''' then the same value is returned for subsequent calls.
		''' </summary>
		''' <returns> a string representation of this <code>ArrayType</code> instance </returns>
		Public Overrides Function ToString() As String

			' Calculate the string representation if it has not yet been done (ie 1st call to toString())
			'
			If myToString Is Nothing Then myToString = Me.GetType().name & "(name=" & typeName & ",dimension=" & dimension & ",elementType=" & elementType & ",primitiveArray=" & primitiveArray & ")"

			' return always the same string representation for this instance (immutable)
			'
			Return myToString
		End Function

		''' <summary>
		''' Create an {@code ArrayType} instance in a type-safe manner.
		''' <p>
		''' Multidimensional arrays can be built up by calling this method as many
		''' times as necessary.
		''' <p>
		''' Calling this method twice with the same parameters may return the same
		''' object or two equal but not identical objects.
		''' <p>
		''' As an example, the following piece of code:
		''' <pre>{@code
		''' ArrayType<String[]> t1 = ArrayType.getArrayType(SimpleType.STRING);
		''' ArrayType<String[][]> t2 = ArrayType.getArrayType(t1);
		''' ArrayType<String[][][]> t3 = ArrayType.getArrayType(t2);
		''' System.out.println("array class name       = " + t3.getClassName());
		''' System.out.println("element class name     = " + t3.getElementOpenType().getClassName());
		''' System.out.println("array type name        = " + t3.getTypeName());
		''' System.out.println("array type description = " + t3.getDescription());
		''' }</pre>
		''' would produce the following output:
		''' <pre>{@code
		''' array class name       = [[[Ljava.lang.String;
		''' element class name     = java.lang.String
		''' array type name        = [[[Ljava.lang.String;
		''' array type description = 3-dimension array of java.lang.String
		''' }</pre>
		''' </summary>
		''' <param name="elementType">  the <i>open type</i> of element values contained
		'''                      in the arrays described by this <tt>ArrayType</tt>
		'''                      instance; must be an instance of either
		'''                      <tt>SimpleType</tt>, <tt>CompositeType</tt>,
		'''                      <tt>TabularType</tt> or another <tt>ArrayType</tt>
		'''                      with a <tt>SimpleType</tt>, <tt>CompositeType</tt>
		'''                      or <tt>TabularType</tt> as its <tt>elementType</tt>.
		''' </param>
		''' <exception cref="OpenDataException"> if <var>elementType's className</var> is not
		'''                           one of the allowed Java class names for open
		'''                           data.
		''' 
		''' @since 1.6 </exception>
		Public Shared Function getArrayType(Of E)(ByVal elementType As OpenType(Of E)) As ArrayType(Of E())
			Return New ArrayType(Of E())(1, elementType)
		End Function

		''' <summary>
		''' Create an {@code ArrayType} instance in a type-safe manner.
		''' <p>
		''' Calling this method twice with the same parameters may return the
		''' same object or two equal but not identical objects.
		''' <p>
		''' As an example, the following piece of code:
		''' <pre>{@code
		''' ArrayType<int[][][]> t = ArrayType.getPrimitiveArrayType(int[][][].class);
		''' System.out.println("array class name       = " + t.getClassName());
		''' System.out.println("element class name     = " + t.getElementOpenType().getClassName());
		''' System.out.println("array type name        = " + t.getTypeName());
		''' System.out.println("array type description = " + t.getDescription());
		''' }</pre>
		''' would produce the following output:
		''' <pre>{@code
		''' array class name       = [[[I
		''' element class name     = java.lang.Integer
		''' array type name        = [[[I
		''' array type description = 3-dimension array of int
		''' }</pre>
		''' </summary>
		''' <param name="arrayClass"> a primitive array class such as {@code int[].class},
		'''                   {@code boolean[][].class}, etc. The {@link
		'''                   #getElementOpenType()} method of the returned
		'''                   {@code ArrayType} returns the <seealso cref="SimpleType"/>
		'''                   corresponding to the wrapper type of the primitive
		'''                   type of the array.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <var>arrayClass</var> is not
		'''                                  a primitive array.
		''' 
		''' @since 1.6 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function getPrimitiveArrayType(Of T)(ByVal arrayClass As Type) As ArrayType(Of T) ' can't get appropriate T for primitive array
			' Check if the supplied parameter is an array
			'
			If Not arrayClass.IsArray Then Throw New System.ArgumentException("arrayClass must be an array")

			' Calculate array dimension and component type name
			'
			Dim n As Integer = 1
			Dim componentType As Type = arrayClass.GetElementType()
			Do While componentType.IsArray
				n += 1
				componentType = componentType.GetElementType()
			Loop
			Dim componentTypeName As String = componentType.name

			' Check if the array's component type is a primitive type
			'
			If Not componentType.IsPrimitive Then Throw New System.ArgumentException("component type of the array must be a primitive type")

			' Map component type name to corresponding SimpleType
			'
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim ___simpleType As SimpleType(Of ?) = getPrimitiveOpenType(componentTypeName)

			' Build primitive array
			'
			Try
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim at As New ArrayType(___simpleType, True)
				If n > 1 Then at = New ArrayType(Of T)(n - 1, at)
				Return at
			Catch e As OpenDataException
				Throw New System.ArgumentException(e) ' should not happen
			End Try
		End Function

		''' <summary>
		''' Replace/resolve the object read from the stream before it is returned
		''' to the caller.
		''' 
		''' @serialData The new serial form of this class defines a new serializable
		''' {@code boolean} field {@code primitiveArray}. In order to guarantee the
		''' interoperability with previous versions of this class the new serial
		''' form must continue to refer to primitive wrapper types even when the
		''' {@code ArrayType} instance describes a primitive type array. So when
		''' {@code primitiveArray} is {@code true} the {@code className},
		''' {@code typeName} and {@code description} serializable fields
		''' are converted into primitive types before the deserialized
		''' {@code ArrayType} instance is return to the caller. The
		''' {@code elementType} field always returns the {@code SimpleType}
		''' corresponding to the primitive wrapper type of the array's
		''' primitive type.
		''' <p>
		''' Therefore the following serializable fields are deserialized as follows:
		''' <ul>
		'''   <li>if {@code primitiveArray} is {@code true} the {@code className}
		'''       field is deserialized by replacing the array's component primitive
		'''       wrapper type by the corresponding array's component primitive type,
		'''       e.g. {@code "[[Ljava.lang.Integer;"} will be deserialized as
		'''       {@code "[[I"}.</li>
		'''   <li>if {@code primitiveArray} is {@code true} the {@code typeName}
		'''       field is deserialized by replacing the array's component primitive
		'''       wrapper type by the corresponding array's component primitive type,
		'''       e.g. {@code "[[Ljava.lang.Integer;"} will be deserialized as
		'''       {@code "[[I"}.</li>
		'''   <li>if {@code primitiveArray} is {@code true} the {@code description}
		'''       field is deserialized by replacing the array's component primitive
		'''       wrapper type by the corresponding array's component primitive type,
		'''       e.g. {@code "2-dimension array of java.lang.Integer"} will be
		'''       deserialized as {@code "2-dimension array of int"}.</li>
		''' </ul>
		''' 
		''' @since 1.6
		''' </summary>
		Private Function readResolve() As Object
			If primitiveArray Then
				Return convertFromWrapperToPrimitiveTypes()
			Else
				Return Me
			End If
		End Function

		Private Function convertFromWrapperToPrimitiveTypes(Of T)() As ArrayType(Of T)
			Dim cn As String = className
			Dim tn As String = typeName
			Dim d As String = description
			For Each typeDescr As Object() In PRIMITIVE_ARRAY_TYPES
				If cn.IndexOf(CStr(typeDescr(PRIMITIVE_WRAPPER_NAME_INDEX))) <> -1 Then
					cn = cn.replaceFirst("L" & typeDescr(PRIMITIVE_WRAPPER_NAME_INDEX) & ";", CStr(typeDescr(PRIMITIVE_TYPE_KEY_INDEX)))
					tn = tn.replaceFirst("L" & typeDescr(PRIMITIVE_WRAPPER_NAME_INDEX) & ";", CStr(typeDescr(PRIMITIVE_TYPE_KEY_INDEX)))
					d = d.replaceFirst(CStr(typeDescr(PRIMITIVE_WRAPPER_NAME_INDEX)), CStr(typeDescr(PRIMITIVE_TYPE_NAME_INDEX)))
					Exit For
				End If
			Next typeDescr
			Return New ArrayType(Of T)(cn, tn, d, dimension, elementType, primitiveArray)
		End Function

		''' <summary>
		''' Nominate a replacement for this object in the stream before the object
		''' is written.
		''' 
		''' @serialData The new serial form of this class defines a new serializable
		''' {@code boolean} field {@code primitiveArray}. In order to guarantee the
		''' interoperability with previous versions of this class the new serial
		''' form must continue to refer to primitive wrapper types even when the
		''' {@code ArrayType} instance describes a primitive type array. So when
		''' {@code primitiveArray} is {@code true} the {@code className},
		''' {@code typeName} and {@code description} serializable fields
		''' are converted into wrapper types before the serialized
		''' {@code ArrayType} instance is written to the stream. The
		''' {@code elementType} field always returns the {@code SimpleType}
		''' corresponding to the primitive wrapper type of the array's
		''' primitive type.
		''' <p>
		''' Therefore the following serializable fields are serialized as follows:
		''' <ul>
		'''   <li>if {@code primitiveArray} is {@code true} the {@code className}
		'''       field is serialized by replacing the array's component primitive
		'''       type by the corresponding array's component primitive wrapper type,
		'''       e.g. {@code "[[I"} will be serialized as
		'''       {@code "[[Ljava.lang.Integer;"}.</li>
		'''   <li>if {@code primitiveArray} is {@code true} the {@code typeName}
		'''       field is serialized by replacing the array's component primitive
		'''       type by the corresponding array's component primitive wrapper type,
		'''       e.g. {@code "[[I"} will be serialized as
		'''       {@code "[[Ljava.lang.Integer;"}.</li>
		'''   <li>if {@code primitiveArray} is {@code true} the {@code description}
		'''       field is serialized by replacing the array's component primitive
		'''       type by the corresponding array's component primitive wrapper type,
		'''       e.g. {@code "2-dimension array of int"} will be serialized as
		'''       {@code "2-dimension array of java.lang.Integer"}.</li>
		''' </ul>
		''' 
		''' @since 1.6
		''' </summary>
		Private Function writeReplace() As Object
			If primitiveArray Then
				Return convertFromPrimitiveToWrapperTypes()
			Else
				Return Me
			End If
		End Function

		Private Function convertFromPrimitiveToWrapperTypes(Of T)() As ArrayType(Of T)
			Dim cn As String = className
			Dim tn As String = typeName
			Dim d As String = description
			For Each typeDescr As Object() In PRIMITIVE_ARRAY_TYPES
				If cn.IndexOf(CStr(typeDescr(PRIMITIVE_TYPE_KEY_INDEX))) <> -1 Then
					cn = cn.replaceFirst(CStr(typeDescr(PRIMITIVE_TYPE_KEY_INDEX)), "L" & typeDescr(PRIMITIVE_WRAPPER_NAME_INDEX) & ";")
					tn = tn.replaceFirst(CStr(typeDescr(PRIMITIVE_TYPE_KEY_INDEX)), "L" & typeDescr(PRIMITIVE_WRAPPER_NAME_INDEX) & ";")
					d = d.replaceFirst(CStr(typeDescr(PRIMITIVE_TYPE_NAME_INDEX)), CStr(typeDescr(PRIMITIVE_WRAPPER_NAME_INDEX)))
					Exit For
				End If
			Next typeDescr
			Return New ArrayType(Of T)(cn, tn, d, dimension, elementType, primitiveArray)
		End Function
	End Class

End Namespace
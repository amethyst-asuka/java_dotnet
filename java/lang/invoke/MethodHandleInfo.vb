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

Namespace java.lang.invoke


	''' <summary>
	''' A symbolic reference obtained by cracking a direct method handle
	''' into its consitutent symbolic parts.
	''' To crack a direct method handle, call <seealso cref="Lookup#revealDirect Lookup.revealDirect"/>.
	''' <h1><a name="directmh"></a>Direct Method Handles</h1>
	''' A <em>direct method handle</em> represents a method, constructor, or field without
	''' any intervening argument bindings or other transformations.
	''' The method, constructor, or field referred to by a direct method handle is called
	''' its <em>underlying member</em>.
	''' Direct method handles may be obtained in any of these ways:
	''' <ul>
	''' <li>By executing an {@code ldc} instruction on a {@code CONSTANT_MethodHandle} constant.
	'''     (See the Java Virtual Machine Specification, sections 4.4.8 and 5.4.3.)
	''' <li>By calling one of the <a href="MethodHandles.Lookup.html#lookups">Lookup Factory Methods</a>,
	'''     such as <seealso cref="Lookup#findVirtual Lookup.findVirtual"/>,
	'''     to resolve a symbolic reference into a method handle.
	'''     A symbolic reference consists of a [Class], name string, and type.
	''' <li>By calling the factory method <seealso cref="Lookup#unreflect Lookup.unreflect"/>
	'''     or <seealso cref="Lookup#unreflectSpecial Lookup.unreflectSpecial"/>
	'''     to convert a <seealso cref="Method"/> into a method handle.
	''' <li>By calling the factory method <seealso cref="Lookup#unreflectConstructor Lookup.unreflectConstructor"/>
	'''     to convert a <seealso cref="Constructor"/> into a method handle.
	''' <li>By calling the factory method <seealso cref="Lookup#unreflectGetter Lookup.unreflectGetter"/>
	'''     or <seealso cref="Lookup#unreflectSetter Lookup.unreflectSetter"/>
	'''     to convert a <seealso cref="Field"/> into a method handle.
	''' </ul>
	''' 
	''' <h1>Restrictions on Cracking</h1>
	''' Given a suitable {@code Lookup} object, it is possible to crack any direct method handle
	''' to recover a symbolic reference for the underlying method, constructor, or field.
	''' Cracking must be done via a {@code Lookup} object equivalent to that which created
	''' the target method handle, or which has enough access permissions to recreate
	''' an equivalent method handle.
	''' <p>
	''' If the underlying method is <a href="MethodHandles.Lookup.html#callsens">caller sensitive</a>,
	''' the direct method handle will have been "bound" to a particular caller [Class], the
	''' <seealso cref="java.lang.invoke.MethodHandles.Lookup#lookupClass() lookup class"/>
	''' of the lookup object used to create it.
	''' Cracking this method handle with a different lookup class will fail
	''' even if the underlying method is public (like {@code Class.forName}).
	''' <p>
	''' The requirement of lookup object matching provides a "fast fail" behavior
	''' for programs which may otherwise trust erroneous revelation of a method
	''' handle with symbolic information (or caller binding) from an unexpected scope.
	''' Use <seealso cref="java.lang.invoke.MethodHandles#reflectAs"/> to override this limitation.
	''' 
	''' <h1><a name="refkinds"></a>Reference kinds</h1>
	''' The <a href="MethodHandles.Lookup.html#lookups">Lookup Factory Methods</a>
	''' correspond to all major use cases for methods, constructors, and fields.
	''' These use cases may be distinguished using small integers as follows:
	''' <table border=1 cellpadding=5 summary="reference kinds">
	''' <tr><th>reference kind</th><th>descriptive name</th><th>scope</th><th>member</th><th>behavior</th></tr>
	''' <tr>
	'''     <td>{@code 1}</td><td>{@code REF_getField}</td><td>{@code class}</td>
	'''     <td>{@code FT f;}</td><td>{@code (T) this.f;}</td>
	''' </tr>
	''' <tr>
	'''     <td>{@code 2}</td><td>{@code REF_getStatic}</td><td>{@code class} or {@code interface}</td>
	'''     <td>{@code static}<br>{@code FT f;}</td><td>{@code (T) C.f;}</td>
	''' </tr>
	''' <tr>
	'''     <td>{@code 3}</td><td>{@code REF_putField}</td><td>{@code class}</td>
	'''     <td>{@code FT f;}</td><td>{@code this.f = x;}</td>
	''' </tr>
	''' <tr>
	'''     <td>{@code 4}</td><td>{@code REF_putStatic}</td><td>{@code class}</td>
	'''     <td>{@code static}<br>{@code FT f;}</td><td>{@code C.f = arg;}</td>
	''' </tr>
	''' <tr>
	'''     <td>{@code 5}</td><td>{@code REF_invokeVirtual}</td><td>{@code class}</td>
	'''     <td>{@code T m(A*);}</td><td>{@code (T) this.m(arg*);}</td>
	''' </tr>
	''' <tr>
	'''     <td>{@code 6}</td><td>{@code REF_invokeStatic}</td><td>{@code class} or {@code interface}</td>
	'''     <td>{@code static}<br>{@code T m(A*);}</td><td>{@code (T) C.m(arg*);}</td>
	''' </tr>
	''' <tr>
	'''     <td>{@code 7}</td><td>{@code REF_invokeSpecial}</td><td>{@code class} or {@code interface}</td>
	'''     <td>{@code T m(A*);}</td><td>{@code (T) super.m(arg*);}</td>
	''' </tr>
	''' <tr>
	'''     <td>{@code 8}</td><td>{@code REF_newInvokeSpecial}</td><td>{@code class}</td>
	'''     <td>{@code C(A*);}</td><td>{@code new C(arg*);}</td>
	''' </tr>
	''' <tr>
	'''     <td>{@code 9}</td><td>{@code REF_invokeInterface}</td><td>{@code interface}</td>
	'''     <td>{@code T m(A*);}</td><td>{@code (T) this.m(arg*);}</td>
	''' </tr>
	''' </table>
	''' @since 1.8
	''' </summary>
	Public Interface MethodHandleInfo
		''' <summary>
		''' A direct method handle reference kind,
		''' as defined in the <a href="MethodHandleInfo.html#refkinds">table above</a>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int REF_getField = Constants.REF_getField, REF_getStatic = Constants.REF_getStatic, REF_putField = Constants.REF_putField, REF_putStatic = Constants.REF_putStatic, REF_invokeVirtual = Constants.REF_invokeVirtual, REF_invokeStatic = Constants.REF_invokeStatic, REF_invokeSpecial = Constants.REF_invokeSpecial, REF_newInvokeSpecial = Constants.REF_newInvokeSpecial, REF_invokeInterface = Constants.REF_invokeInterface;

		''' <summary>
		''' Returns the reference kind of the cracked method handle, which in turn
		''' determines whether the method handle's underlying member was a constructor, method, or field.
		''' See the <a href="MethodHandleInfo.html#refkinds">table above</a> for definitions. </summary>
		''' <returns> the integer code for the kind of reference used to access the underlying member </returns>
		ReadOnly Property referenceKind As Integer

		''' <summary>
		''' Returns the class in which the cracked method handle's underlying member was defined. </summary>
		''' <returns> the declaring class of the underlying member </returns>
		ReadOnly Property declaringClass As  [Class]

		''' <summary>
		''' Returns the name of the cracked method handle's underlying member.
		''' This is {@code "&lt;init&gt;"} if the underlying member was a constructor,
		''' else it is a simple method name or field name. </summary>
		''' <returns> the simple name of the underlying member </returns>
		ReadOnly Property name As String

		''' <summary>
		''' Returns the nominal type of the cracked symbolic reference, expressed as a method type.
		''' If the reference is to a constructor, the return type will be {@code void}.
		''' If it is to a non-static method, the method type will not mention the {@code this} parameter.
		''' If it is to a field and the requested access is to read the field,
		''' the method type will have no parameters and return the field type.
		''' If it is to a field and the requested access is to write the field,
		''' the method type will have one parameter of the field type and return {@code void}.
		''' <p>
		''' Note that original direct method handle may include a leading {@code this} parameter,
		''' or (in the case of a constructor) will replace the {@code void} return type
		''' with the constructed class.
		''' The nominal type does not include any {@code this} parameter,
		''' and (in the case of a constructor) will return {@code void}. </summary>
		''' <returns> the type of the underlying member, expressed as a method type </returns>
		ReadOnly Property methodType As MethodType

		' Utility methods.
		' NOTE: class/name/type and reference kind constitute a symbolic reference
		' member and modifiers are an add-on, derived from Core Reflection (or the equivalent)

		''' <summary>
		''' Reflects the underlying member as a method, constructor, or field object.
		''' If the underlying member is public, it is reflected as if by
		''' {@code getMethod}, {@code getConstructor}, or {@code getField}.
		''' Otherwise, it is reflected as if by
		''' {@code getDeclaredMethod}, {@code getDeclaredConstructor}, or {@code getDeclaredField}.
		''' The underlying member must be accessible to the given lookup object. </summary>
		''' @param <T> the desired type of the result, either <seealso cref="Member"/> or a subtype </param>
		''' <param name="expected"> a class object representing the desired result type {@code T} </param>
		''' <param name="lookup"> the lookup object that created this MethodHandleInfo, or one with equivalent access privileges </param>
		''' <returns> a reference to the method, constructor, or field object </returns>
		''' <exception cref="ClassCastException"> if the member is not of the expected type </exception>
		''' <exception cref="NullPointerException"> if either argument is {@code null} </exception>
		''' <exception cref="IllegalArgumentException"> if the underlying member is not accessible to the given lookup object </exception>
		 Function reflectAs(Of T As Member)(  expected As [Class],   lookup As Lookup) As T

		''' <summary>
		''' Returns the access modifiers of the underlying member. </summary>
		''' <returns> the Java language modifiers for underlying member,
		'''         or -1 if the member cannot be accessed </returns>
		''' <seealso cref= Modifier </seealso>
		''' <seealso cref= #reflectAs </seealso>
		ReadOnly Property modifiers As Integer

		''' <summary>
		''' Determines if the underlying member was a variable arity method or constructor.
		''' Such members are represented by method handles that are varargs collectors.
		''' @implSpec
		''' This produces a result equivalent to:
		''' <pre>{@code
		'''     getReferenceKind() >= REF_invokeVirtual && Modifier.isTransient(getModifiers())
		''' }</pre>
		''' 
		''' </summary>
		''' <returns> {@code true} if and only if the underlying member was declared with variable arity. </returns>
		' spelling derived from java.lang.reflect.Executable, not MethodHandle.isVarargsCollector
		ReadOnly Property default varArgs As Boolean
			' fields are never varargs:
			Sub [New](MethodHandleNatives.refKindIsField((SByte) getReferenceKind())    As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				Return False;
			' not in the public API: Modifier.VARARGS
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			final int ACC_VARARGS = &H80; ' from JVMS 4.6 (Table 4.20)
			Sub [New](ACC_VARARGS ==   Modifier.TRANSIENT As )
			Function Modifier.isTransient(getModifiers()    As ) As [Return]

		''' <summary>
		''' Returns the descriptive name of the given reference kind,
		''' as defined in the <a href="MethodHandleInfo.html#refkinds">table above</a>.
		''' The conventional prefix "REF_" is omitted. </summary>
		''' <param name="referenceKind"> an integer code for a kind of reference used to access a class member </param>
		''' <returns> a mixed-case string such as {@code "getField"} </returns>
		''' <exception cref="IllegalArgumentException"> if the argument is not a valid
		'''            <a href="MethodHandleInfo.html#refkinds">reference kind number</a> </exception>
		Shared Function referenceKindToString(  referenceKind As Integer) As String
			Sub [New](!MethodHandleNatives.refKindIsValid(referenceKind)    As )
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				throw newIllegalArgumentException("invalid reference kind", referenceKind);
			Function MethodHandleNatives.refKindName((SByte)   referenceKind As ) As [Return]

		''' <summary>
		''' Returns a string representation for a {@code MethodHandleInfo},
		''' given the four parts of its symbolic reference.
		''' This is defined to be of the form {@code "RK C.N:MT"}, where {@code RK} is the
		''' <seealso cref="#referenceKindToString reference kind string"/> for {@code kind},
		''' {@code C} is the <seealso cref="java.lang.Class#getName name"/> of {@code defc}
		''' {@code N} is the {@code name}, and
		''' {@code MT} is the {@code type}.
		''' These four values may be obtained from the
		''' <seealso cref="#getReferenceKind reference kind"/>,
		''' <seealso cref="#getDeclaringClass declaring class"/>,
		''' <seealso cref="#getName member name"/>,
		''' and <seealso cref="#getMethodType method type"/>
		''' of a {@code MethodHandleInfo} object.
		''' 
		''' @implSpec
		''' This produces a result equivalent to:
		''' <pre>{@code
		'''     String.format("%s %s.%s:%s", referenceKindToString(kind), defc.getName(), name, type)
		''' }</pre>
		''' </summary>
		''' <param name="kind"> the <seealso cref="#getReferenceKind reference kind"/> part of the symbolic reference </param>
		''' <param name="defc"> the <seealso cref="#getDeclaringClass declaring class"/> part of the symbolic reference </param>
		''' <param name="name"> the <seealso cref="#getName member name"/> part of the symbolic reference </param>
		''' <param name="type"> the <seealso cref="#getMethodType method type"/> part of the symbolic reference </param>
		''' <returns> a string of the form {@code "RK C.N:MT"} </returns>
		''' <exception cref="IllegalArgumentException"> if the first argument is not a valid
		'''            <a href="MethodHandleInfo.html#refkinds">reference kind number</a> </exception>
		''' <exception cref="NullPointerException"> if any reference argument is {@code null} </exception>
		Shared Function ToString(  kind As Integer,   defc As [Class],   name As String,   type As MethodType) As String
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Objects.requireNonNull(name);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Objects.requireNonNull(type);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return String.format("%s %s.%s:%s", referenceKindToString(kind), defc.getName(), name, type);
	End Interface

End Namespace
Imports System
Imports System.Diagnostics

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	'''   <p>An <seealso cref="InvocationHandler"/> that forwards getter methods to a
	'''   <seealso cref="CompositeData"/>.  If you have an interface that contains
	'''   only getter methods (such as {@code String getName()} or
	'''   {@code boolean isActive()}) then you can use this class in
	'''   conjunction with the <seealso cref="Proxy"/> class to produce an implementation
	'''   of the interface where each getter returns the value of the
	'''   corresponding item in a {@code CompositeData}.</p>
	''' 
	'''   <p>For example, suppose you have an interface like this:
	''' 
	'''   <blockquote>
	'''   <pre>
	'''   public interface NamedNumber {
	'''       public int getNumber();
	'''       public String getName();
	'''   }
	'''   </pre>
	'''   </blockquote>
	''' 
	'''   and a {@code CompositeData} constructed like this:
	''' 
	'''   <blockquote>
	'''   <pre>
	'''   CompositeData cd =
	'''       new <seealso cref="CompositeDataSupport"/>(
	'''           someCompositeType,
	'''           new String[] {"number", "name"},
	'''           new Object[] {<b>5</b>, "five"}
	'''       );
	'''   </pre>
	'''   </blockquote>
	''' 
	'''   then you can construct an object implementing {@code NamedNumber}
	'''   and backed by the object {@code cd} like this:
	''' 
	'''   <blockquote>
	'''   <pre>
	'''   InvocationHandler handler =
	'''       new CompositeDataInvocationHandler(cd);
	'''   NamedNumber nn = (NamedNumber)
	'''       Proxy.newProxyInstance(NamedNumber.class.getClassLoader(),
	'''                              new Class[] {NamedNumber.class},
	'''                              handler);
	'''   </pre>
	'''   </blockquote>
	''' 
	'''   A call to {@code nn.getNumber()} will then return <b>5</b>.
	''' 
	'''   <p>If the first letter of the property defined by a getter is a
	'''   capital, then this handler will look first for an item in the
	'''   {@code CompositeData} beginning with a capital, then, if that is
	'''   not found, for an item beginning with the corresponding lowercase
	'''   letter or code point.  For a getter called {@code getNumber()}, the
	'''   handler will first look for an item called {@code Number}, then for
	'''   {@code number}.  If the getter is called {@code getnumber()}, then
	'''   the item must be called {@code number}.</p>
	''' 
	'''   <p>If the method given to <seealso cref="#invoke invoke"/> is the method
	'''   {@code boolean equals(Object)} inherited from {@code Object}, then
	'''   it will return true if and only if the argument is a {@code Proxy}
	'''   whose {@code InvocationHandler} is also a {@code
	'''   CompositeDataInvocationHandler} and whose backing {@code
	'''   CompositeData} is equal (not necessarily identical) to this
	'''   object's.  If the method given to {@code invoke} is the method
	'''   {@code int hashCode()} inherited from {@code Object}, then it will
	'''   return a value that is consistent with this definition of {@code
	'''   equals}: if two objects are equal according to {@code equals}, then
	'''   they will have the same {@code hashCode}.</p>
	''' 
	'''   @since 1.6
	''' </summary>
	Public Class CompositeDataInvocationHandler
		Implements InvocationHandler

		''' <summary>
		'''   <p>Construct a handler backed by the given {@code
		'''   CompositeData}.</p>
		''' </summary>
		'''   <param name="compositeData"> the {@code CompositeData} that will supply
		'''   information to getters.
		''' </param>
		'''   <exception cref="IllegalArgumentException"> if {@code compositeData}
		'''   is null. </exception>
		Public Sub New(ByVal compositeData As CompositeData)
			Me.New(compositeData, Nothing)
		End Sub

		''' <summary>
		'''   <p>Construct a handler backed by the given {@code
		'''   CompositeData}.</p>
		''' </summary>
		'''   <param name="compositeData"> the {@code CompositeData} that will supply
		'''   information to getters.
		''' </param>
		'''   <exception cref="IllegalArgumentException"> if {@code compositeData}
		'''   is null. </exception>
		Friend Sub New(ByVal compositeData As CompositeData, ByVal lookup As com.sun.jmx.mbeanserver.MXBeanLookup)
			If compositeData Is Nothing Then Throw New System.ArgumentException("compositeData")
			Me.compositeData = compositeData
			Me.lookup = lookup
		End Sub

		''' <summary>
		'''   Return the {@code CompositeData} that was supplied to the
		'''   constructor. </summary>
		'''   <returns> the {@code CompositeData} that this handler is backed
		'''   by.  This is never null. </returns>
		Public Overridable Property compositeData As CompositeData
			Get
				Debug.Assert(compositeData IsNot Nothing)
				Return compositeData
			End Get
		End Property

		Public Overridable Function invoke(ByVal proxy As Object, ByVal method As Method, ByVal args As Object()) As Object
			Dim methodName As String = method.name

			' Handle the methods from java.lang.Object
			If method.declaringClass = GetType(Object) Then
				If methodName.Equals("toString") AndAlso args Is Nothing Then
					Return "Proxy[" & compositeData & "]"
				ElseIf methodName.Equals("hashCode") AndAlso args Is Nothing Then
					Return compositeData.GetHashCode() + &H43444948
				ElseIf methodName.Equals("equals") AndAlso args.Length = 1 AndAlso method.parameterTypes(0) = GetType(Object) Then
					Return Equals(proxy, args(0))
				Else
	'                 Either someone is calling invoke by hand, or
	'                   it is a non-final method from Object overriden
	'                   by the generated Proxy.  At the time of writing,
	'                   the only non-final methods in Object that are not
	'                   handled above are finalize and clone, and these
	'                   are not overridden in generated proxies.  
					' this plain Method.invoke is called only if the declaring class
					' is Object and so it's safe.
					Return method.invoke(Me, args)
				End If
			End If

			Dim propertyName As String = com.sun.jmx.mbeanserver.DefaultMXBeanMappingFactory.propertyName(method)
			If propertyName Is Nothing Then Throw New System.ArgumentException("Method is not getter: " & method.name)
			Dim openValue As Object
			If compositeData.containsKey(propertyName) Then
				openValue = compositeData.get(propertyName)
			Else
				Dim decap As String = com.sun.jmx.mbeanserver.DefaultMXBeanMappingFactory.decapitalize(propertyName)
				If compositeData.containsKey(decap) Then
					openValue = compositeData.get(decap)
				Else
					Dim msg As String = "No CompositeData item " & propertyName + (If(decap.Equals(propertyName), "", " or " & decap)) & " to match " & methodName
					Throw New System.ArgumentException(msg)
				End If
			End If
			Dim mapping As com.sun.jmx.mbeanserver.MXBeanMapping = com.sun.jmx.mbeanserver.MXBeanMappingFactory.DEFAULT.mappingForType(method.genericReturnType, com.sun.jmx.mbeanserver.MXBeanMappingFactory.DEFAULT)
			Return mapping.fromOpenValue(openValue)
		End Function

	'     This method is called when equals(Object) is
	'     * called on our proxy and hence forwarded to us.  For example, if we
	'     * are a proxy for an interface like this:
	'     * public interface GetString {
	'     *     public String string();
	'     * }
	'     * then we must compare equal to another CompositeDataInvocationHandler
	'     * proxy for the same interface and where string() returns the same value.
	'     *
	'     * You might think that we should also compare equal to another
	'     * object that implements GetString directly rather than using
	'     * Proxy, provided that its string() returns the same result as
	'     * ours, and in fact an earlier version of this class did that (by
	'     * converting the other object into a CompositeData and comparing
	'     * that with ours).  But in fact that doesn't make a great deal of
	'     * sense because there's absolutely no guarantee that the
	'     * resulting equals would be reflexive (otherObject.equals(this)
	'     * might be false even if this.equals(otherObject) is true), and,
	'     * especially, there's no way we could generate a hashCode() that
	'     * would be equal to otherObject.hashCode() when
	'     * this.equals(otherObject), because we don't know how
	'     * otherObject.hashCode() is computed.
	'     
		Private Overrides Function Equals(ByVal proxy As Object, ByVal other As Object) As Boolean
			If other Is Nothing Then Return False

			Dim proxyClass As Type = proxy.GetType()
			Dim otherClass As Type = other.GetType()
			If proxyClass IsNot otherClass Then Return False
			Dim otherih As InvocationHandler = Proxy.getInvocationHandler(other)
			If Not(TypeOf otherih Is CompositeDataInvocationHandler) Then Return False
			Dim othercdih As CompositeDataInvocationHandler = CType(otherih, CompositeDataInvocationHandler)
			Return compositeData.Equals(othercdih.compositeData)
		End Function

		Private ReadOnly compositeData As CompositeData
		Private ReadOnly lookup As com.sun.jmx.mbeanserver.MXBeanLookup
	End Class

End Namespace
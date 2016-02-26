Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
import static sun.reflect.misc.ReflectUtil.isPackageAccessible

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
Namespace java.beans








	'
	' * Like the <code>Intropector</code>, the <code>MetaData</code> class
	' * contains <em>meta</em> objects that describe the way
	' * classes should express their state in terms of their
	' * own public APIs.
	' *
	' * @see java.beans.Intropector
	' *
	' * @author Philip Milne
	' * @author Steve Langley
	' 
	Friend Class MetaData

	Friend NotInheritable Class NullPersistenceDelegate
		Inherits PersistenceDelegate

		' Note this will be called by all classes when they reach the
		' top of their superclass chain.
		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
		End Sub
		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Return Nothing
		End Function

		Public Overrides Sub writeObject(ByVal oldInstance As Object, ByVal out As Encoder)
		' System.out.println("NullPersistenceDelegate:writeObject " + oldInstance);
		End Sub
	End Class

	''' <summary>
	''' The persistence delegate for <CODE>enum</CODE> classes.
	''' 
	''' @author Sergey A. Malenkov
	''' </summary>
	Friend NotInheritable Class EnumPersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return oldInstance Is newInstance
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim e As System.Enum(Of ?) = CType(oldInstance, Enum(Of ?))
			Return New Expression(e, GetType(System.Enum), "valueOf", New Object(){e.declaringClass, e.name()})
		End Function
	End Class

	Friend NotInheritable Class PrimitivePersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return oldInstance.Equals(newInstance)
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Return New Expression(oldInstance, oldInstance.GetType(), "new", New Object(){oldInstance.ToString()})
		End Function
	End Class

	Friend NotInheritable Class ArrayPersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return (newInstance IsNot Nothing AndAlso oldInstance.GetType() Is newInstance.GetType() AndAlso Array.getLength(oldInstance) = Array.getLength(newInstance)) ' Also ensures the subtype is correct.
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			' System.out.println("instantiate: " + type + " " + oldInstance);
			Dim oldClass As  [Class] = oldInstance.GetType()
			Return New Expression(oldInstance, GetType(Array), "newInstance", New Object(){oldClass.componentType, New Integer?(Array.getLength(oldInstance))})
		End Function

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			Dim n As Integer = Array.getLength(oldInstance)
			For i As Integer = 0 To n - 1
				Dim index As Object = New Integer?(i)
				' Expression oldGetExp = new Expression(Array.class, "get", new Object[]{oldInstance, index});
				' Expression newGetExp = new Expression(Array.class, "get", new Object[]{newInstance, index});
				Dim oldGetExp As New Expression(oldInstance, "get", New Object(){index})
				Dim newGetExp As New Expression(newInstance, "get", New Object(){index})
				Try
					Dim oldValue As Object = oldGetExp.value
					Dim newValue As Object = newGetExp.value
					out.writeExpression(oldGetExp)
					If Not Objects.Equals(newValue, out.get(oldValue)) Then DefaultPersistenceDelegate.invokeStatement(oldInstance, "set", New Object(){index, oldValue}, out)
				Catch e As Exception
					' System.err.println("Warning:: failed to write: " + oldGetExp);
					out.exceptionListener.exceptionThrown(e)
				End Try
			Next i
		End Sub
	End Class

	Friend NotInheritable Class ProxyPersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Dim type As  [Class] = oldInstance.GetType()
			Dim p As java.lang.reflect.Proxy = CType(oldInstance, java.lang.reflect.Proxy)
			' This unappealing hack is not required but makes the
			' representation of EventHandlers much more concise.
			Dim ih As java.lang.reflect.InvocationHandler = java.lang.reflect.Proxy.getInvocationHandler(p)
			If TypeOf ih Is EventHandler Then
				Dim eh As EventHandler = CType(ih, EventHandler)
				Dim args As New Vector(Of Object)
				args.add(type.interfaces(0))
				args.add(eh.target)
				args.add(eh.action)
				If eh.eventPropertyName IsNot Nothing Then args.add(eh.eventPropertyName)
				If eh.listenerMethodName IsNot Nothing Then
					args.size = 4
					args.add(eh.listenerMethodName)
				End If
				Return New Expression(oldInstance, GetType(EventHandler), "create", args.ToArray())
			End If
			Return New Expression(oldInstance, GetType(java.lang.reflect.Proxy), "newProxyInstance", New Object(){type.classLoader, type.interfaces, ih})
		End Function
	End Class

	' Strings
	Friend NotInheritable Class java_lang_String_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Return Nothing
		End Function

		Public Overrides Sub writeObject(ByVal oldInstance As Object, ByVal out As Encoder)
			' System.out.println("NullPersistenceDelegate:writeObject " + oldInstance);
		End Sub
	End Class

	' Classes
	Friend NotInheritable Class java_lang_Class_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return oldInstance.Equals(newInstance)
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Dim c As  [Class] = CType(oldInstance, [Class])
			' As of 1.3 it is not possible to call Class.forName("int"),
			' so we have to generate different code for primitive types.
			' This is needed for arrays whose subtype may be primitive.
			If c.primitive Then
				Dim field As Field = Nothing
				Try
					field = com.sun.beans.finder.PrimitiveWrapperMap.getType(c.name).getDeclaredField("TYPE")
				Catch ex As NoSuchFieldException
					Console.Error.WriteLine("Unknown primitive type: " & c)
				End Try
				Return New Expression(oldInstance, field, "get", New Object(){Nothing})
			ElseIf oldInstance Is GetType(String) Then
				Return New Expression(oldInstance, "", "getClass", New Object(){})
			ElseIf oldInstance Is GetType(Class) Then
				Return New Expression(oldInstance, GetType(String), "getClass", New Object(){})
			Else
				Dim newInstance As New Expression(oldInstance, GetType(Class), "forName", New Object() { c.name })
				newInstance.loader = c.classLoader
				Return newInstance
			End If
		End Function
	End Class

	' Fields
	Friend NotInheritable Class java_lang_reflect_Field_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return oldInstance.Equals(newInstance)
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Dim f As Field = CType(oldInstance, Field)
			Return New Expression(oldInstance, f.declaringClass, "getField", New Object(){f.name})
		End Function
	End Class

	' Methods
	Friend NotInheritable Class java_lang_reflect_Method_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return oldInstance.Equals(newInstance)
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Dim m As Method = CType(oldInstance, Method)
			Return New Expression(oldInstance, m.declaringClass, "getMethod", New Object(){m.name, m.parameterTypes})
		End Function
	End Class

	' Dates

	''' <summary>
	''' The persistence delegate for <CODE>java.util.Date</CODE> classes.
	''' Do not extend DefaultPersistenceDelegate to improve performance and
	''' to avoid problems with <CODE>java.sql.Date</CODE>,
	''' <CODE>java.sql.Time</CODE> and <CODE>java.sql.Timestamp</CODE>.
	''' 
	''' @author Sergey A. Malenkov
	''' </summary>
	Friend Class java_util_Date_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			If Not MyBase.mutatesTo(oldInstance, newInstance) Then Return False
			Dim oldDate As Date = CDate(oldInstance)
			Dim newDate As Date = CDate(newInstance)

			Return oldDate.time = newDate.time
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Dim date_Renamed As Date = CDate(oldInstance)
			Return New Expression(date_Renamed, date_Renamed.GetType(), "new", New Object() {date_Renamed.time})
		End Function
	End Class

	''' <summary>
	''' The persistence delegate for <CODE>java.sql.Timestamp</CODE> classes.
	''' It supports nanoseconds.
	''' 
	''' @author Sergey A. Malenkov
	''' </summary>
	Friend NotInheritable Class java_sql_Timestamp_PersistenceDelegate
		Inherits java_util_Date_PersistenceDelegate

		Private Shared ReadOnly getNanosMethod_Renamed As Method = nanosMethod

		Private Property Shared nanosMethod As Method
			Get
				Try
					Dim c As  [Class] = Type.GetType("java.sql.Timestamp", True, Nothing)
					Return c.getMethod("getNanos")
				Catch e As  [Class]NotFoundException
					Return Nothing
				Catch e As NoSuchMethodException
					Throw New AssertionError(e)
				End Try
			End Get
		End Property

		''' <summary>
		''' Invoke Timstamp getNanos.
		''' </summary>
		Private Shared Function getNanos(ByVal obj As Object) As Integer
			If getNanosMethod_Renamed Is Nothing Then Throw New AssertionError("Should not get here")
			Try
				Return CInt(Fix(getNanosMethod_Renamed.invoke(obj)))
			Catch e As InvocationTargetException
				Dim cause As Throwable = e.InnerException
				If TypeOf cause Is RuntimeException Then Throw CType(cause, RuntimeException)
				If TypeOf cause Is Error Then Throw CType(cause, [Error])
				Throw New AssertionError(e)
			Catch iae As IllegalAccessException
				Throw New AssertionError(iae)
			End Try
		End Function

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			' assumes oldInstance and newInstance are Timestamps
			Dim nanos_Renamed As Integer = getNanos(oldInstance)
			If nanos_Renamed <> getNanos(newInstance) Then out.writeStatement(New Statement(oldInstance, "setNanos", New Object() {nanos_Renamed}))
		End Sub
	End Class

	' Collections

	'
	'The Hashtable and AbstractMap classes have no common ancestor yet may
	'be handled with a single persistence delegate: one which uses the methods
	'of the Map insterface exclusively. Attatching the persistence delegates
	'to the interfaces themselves is fraught however since, in the case of
	'the Map, both the AbstractMap and HashMap classes are declared to
	'implement the Map interface, leaving the obvious implementation prone
	'to repeating their initialization. These issues and questions around
	'the ordering of delegates attached to interfaces have lead us to
	'ignore any delegates attached to interfaces and force all persistence
	'delegates to be registered with concrete classes.
	'

	''' <summary>
	''' The base class for persistence delegates for inner classes
	''' that can be created using <seealso cref="Collections"/>.
	''' 
	''' @author Sergey A. Malenkov
	''' </summary>
	Private MustInherit Class java_util_Collections
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			If Not MyBase.mutatesTo(oldInstance, newInstance) Then Return False
			If (TypeOf oldInstance Is List) OrElse (TypeOf oldInstance Is Set) OrElse (TypeOf oldInstance Is Map) Then Return oldInstance.Equals(newInstance)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim oldC As Collection(Of ?) = CType(oldInstance, Collection(Of ?))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim newC As Collection(Of ?) = CType(newInstance, Collection(Of ?))
			Return (oldC.size() = newC.size()) AndAlso oldC.containsAll(newC)
		End Function

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			' do not initialize these custom collections in default way
		End Sub

		Friend NotInheritable Class EmptyList_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
				Return New Expression(oldInstance, GetType(Collections), "emptyList", Nothing)
			End Function
		End Class

		Friend NotInheritable Class EmptySet_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
				Return New Expression(oldInstance, GetType(Collections), "emptySet", Nothing)
			End Function
		End Class

		Friend NotInheritable Class EmptyMap_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
				Return New Expression(oldInstance, GetType(Collections), "emptyMap", Nothing)
			End Function
		End Class

		Friend NotInheritable Class SingletonList_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim list As List(Of ?) = CType(oldInstance, List(Of ?))
				Return New Expression(oldInstance, GetType(Collections), "singletonList", New Object(){list.get(0)})
			End Function
		End Class

		Friend NotInheritable Class SingletonSet_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim [set] As [Set](Of ?) = CType(oldInstance, Set(Of ?))
				Return New Expression(oldInstance, GetType(Collections), "singleton", New Object(){[set].GetEnumerator().next()})
			End Function
		End Class

		Friend NotInheritable Class SingletonMap_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim map As Map(Of ?, ?) = CType(oldInstance, Map(Of ?, ?))
				Dim key As Object = map.Keys.GetEnumerator().next()
				Return New Expression(oldInstance, GetType(Collections), "singletonMap", New Object(){key, map.get(key)})
			End Function
		End Class

		Friend NotInheritable Class UnmodifiableCollection_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim list As List(Of ?) = New List(Of ?)(CType(oldInstance, Collection(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "unmodifiableCollection", New Object(){list})
			End Function
		End Class

		Friend NotInheritable Class UnmodifiableList_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim list As List(Of ?) = New LinkedList(Of ?)(CType(oldInstance, Collection(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "unmodifiableList", New Object(){list})
			End Function
		End Class

		Friend NotInheritable Class UnmodifiableRandomAccessList_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim list As List(Of ?) = New List(Of ?)(CType(oldInstance, Collection(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "unmodifiableList", New Object(){list})
			End Function
		End Class

		Friend NotInheritable Class UnmodifiableSet_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim [set] As [Set](Of ?) = New HashSet(Of ?)(CType(oldInstance, Set(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "unmodifiableSet", New Object(){[set]})
			End Function
		End Class

		Friend NotInheritable Class UnmodifiableSortedSet_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim [set] As SortedSet(Of ?) = New TreeSet(Of ?)(CType(oldInstance, SortedSet(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "unmodifiableSortedSet", New Object(){[set]})
			End Function
		End Class

		Friend NotInheritable Class UnmodifiableMap_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim map As Map(Of ?, ?) = New HashMap(Of ?, ?)(CType(oldInstance, Map(Of ?, ?)))
				Return New Expression(oldInstance, GetType(Collections), "unmodifiableMap", New Object(){map})
			End Function
		End Class

		Friend NotInheritable Class UnmodifiableSortedMap_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim map As SortedMap(Of ?, ?) = New TreeMap(Of ?, ?)(CType(oldInstance, SortedMap(Of ?, ?)))
				Return New Expression(oldInstance, GetType(Collections), "unmodifiableSortedMap", New Object(){map})
			End Function
		End Class

		Friend NotInheritable Class SynchronizedCollection_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim list As List(Of ?) = New List(Of ?)(CType(oldInstance, Collection(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "synchronizedCollection", New Object(){list})
			End Function
		End Class

		Friend NotInheritable Class SynchronizedList_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim list As List(Of ?) = New LinkedList(Of ?)(CType(oldInstance, Collection(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "synchronizedList", New Object(){list})
			End Function
		End Class

		Friend NotInheritable Class SynchronizedRandomAccessList_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim list As List(Of ?) = New List(Of ?)(CType(oldInstance, Collection(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "synchronizedList", New Object(){list})
			End Function
		End Class

		Friend NotInheritable Class SynchronizedSet_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim [set] As [Set](Of ?) = New HashSet(Of ?)(CType(oldInstance, Set(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "synchronizedSet", New Object(){[set]})
			End Function
		End Class

		Friend NotInheritable Class SynchronizedSortedSet_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim [set] As SortedSet(Of ?) = New TreeSet(Of ?)(CType(oldInstance, SortedSet(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "synchronizedSortedSet", New Object(){[set]})
			End Function
		End Class

		Friend NotInheritable Class SynchronizedMap_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim map As Map(Of ?, ?) = New HashMap(Of ?, ?)(CType(oldInstance, Map(Of ?, ?)))
				Return New Expression(oldInstance, GetType(Collections), "synchronizedMap", New Object(){map})
			End Function
		End Class

		Friend NotInheritable Class SynchronizedSortedMap_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim map As SortedMap(Of ?, ?) = New TreeMap(Of ?, ?)(CType(oldInstance, SortedMap(Of ?, ?)))
				Return New Expression(oldInstance, GetType(Collections), "synchronizedSortedMap", New Object(){map})
			End Function
		End Class

		Friend NotInheritable Class CheckedCollection_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
				Dim type As Object = MetaData.getPrivateFieldValue(oldInstance, "java.util.Collections$CheckedCollection.type")
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim list As List(Of ?) = New List(Of ?)(CType(oldInstance, Collection(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "checkedCollection", New Object(){list, type})
			End Function
		End Class

		Friend NotInheritable Class CheckedList_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
				Dim type As Object = MetaData.getPrivateFieldValue(oldInstance, "java.util.Collections$CheckedCollection.type")
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim list As List(Of ?) = New LinkedList(Of ?)(CType(oldInstance, Collection(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "checkedList", New Object(){list, type})
			End Function
		End Class

		Friend NotInheritable Class CheckedRandomAccessList_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
				Dim type As Object = MetaData.getPrivateFieldValue(oldInstance, "java.util.Collections$CheckedCollection.type")
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim list As List(Of ?) = New List(Of ?)(CType(oldInstance, Collection(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "checkedList", New Object(){list, type})
			End Function
		End Class

		Friend NotInheritable Class CheckedSet_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
				Dim type As Object = MetaData.getPrivateFieldValue(oldInstance, "java.util.Collections$CheckedCollection.type")
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim [set] As [Set](Of ?) = New HashSet(Of ?)(CType(oldInstance, Set(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "checkedSet", New Object(){[set], type})
			End Function
		End Class

		Friend NotInheritable Class CheckedSortedSet_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
				Dim type As Object = MetaData.getPrivateFieldValue(oldInstance, "java.util.Collections$CheckedCollection.type")
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim [set] As SortedSet(Of ?) = New TreeSet(Of ?)(CType(oldInstance, SortedSet(Of ?)))
				Return New Expression(oldInstance, GetType(Collections), "checkedSortedSet", New Object(){[set], type})
			End Function
		End Class

		Friend NotInheritable Class CheckedMap_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
				Dim keyType As Object = MetaData.getPrivateFieldValue(oldInstance, "java.util.Collections$CheckedMap.keyType")
				Dim valueType As Object = MetaData.getPrivateFieldValue(oldInstance, "java.util.Collections$CheckedMap.valueType")
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim map As Map(Of ?, ?) = New HashMap(Of ?, ?)(CType(oldInstance, Map(Of ?, ?)))
				Return New Expression(oldInstance, GetType(Collections), "checkedMap", New Object(){map, keyType, valueType})
			End Function
		End Class

		Friend NotInheritable Class CheckedSortedMap_PersistenceDelegate
			Inherits java_util_Collections

			Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
				Dim keyType As Object = MetaData.getPrivateFieldValue(oldInstance, "java.util.Collections$CheckedMap.keyType")
				Dim valueType As Object = MetaData.getPrivateFieldValue(oldInstance, "java.util.Collections$CheckedMap.valueType")
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim map As SortedMap(Of ?, ?) = New TreeMap(Of ?, ?)(CType(oldInstance, SortedMap(Of ?, ?)))
				Return New Expression(oldInstance, GetType(Collections), "checkedSortedMap", New Object(){map, keyType, valueType})
			End Function
		End Class
	End Class

	''' <summary>
	''' The persistence delegate for <CODE>java.util.EnumMap</CODE> classes.
	''' 
	''' @author Sergey A. Malenkov
	''' </summary>
	Friend NotInheritable Class java_util_EnumMap_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return MyBase.mutatesTo(oldInstance, newInstance) AndAlso ([getType](oldInstance) Is [getType](newInstance))
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Return New Expression(oldInstance, GetType(EnumMap), "new", New Object() {[getType](oldInstance)})
		End Function

		Private Shared Function [getType](ByVal instance As Object) As Object
			Return MetaData.getPrivateFieldValue(instance, "java.util.EnumMap.keyType")
		End Function
	End Class

	''' <summary>
	''' The persistence delegate for <CODE>java.util.EnumSet</CODE> classes.
	''' 
	''' @author Sergey A. Malenkov
	''' </summary>
	Friend NotInheritable Class java_util_EnumSet_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return MyBase.mutatesTo(oldInstance, newInstance) AndAlso ([getType](oldInstance) Is [getType](newInstance))
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Return New Expression(oldInstance, GetType(EnumSet), "noneOf", New Object() {[getType](oldInstance)})
		End Function

		Private Shared Function [getType](ByVal instance As Object) As Object
			Return MetaData.getPrivateFieldValue(instance, "java.util.EnumSet.elementType")
		End Function
	End Class

	' Collection
	Friend Class java_util_Collection_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim oldO As ICollection(Of ?) = CType(oldInstance, ICollection)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim newO As ICollection(Of ?) = CType(newInstance, ICollection)

			If newO.Count <> 0 Then invokeStatement(oldInstance, "clear", New Object(){}, out)
			Dim i As [Iterator](Of ?) = oldO.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Do While i.MoveNext()
				invokeStatement(oldInstance, "add", New Object(){i.Current}, out)
			Loop
		End Sub
	End Class

	' List
	Friend Class java_util_List_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim oldO As IList(Of ?) = CType(oldInstance, IList(Of ?))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim newO As IList(Of ?) = CType(newInstance, IList(Of ?))
			Dim oldSize As Integer = oldO.Count
			Dim newSize As Integer = If(newO Is Nothing, 0, newO.Count)
			If oldSize < newSize Then
				invokeStatement(oldInstance, "clear", New Object(){}, out)
				newSize = 0
			End If
			For i As Integer = 0 To newSize - 1
				Dim index As Object = New Integer?(i)

				Dim oldGetExp As New Expression(oldInstance, "get", New Object(){index})
				Dim newGetExp As New Expression(newInstance, "get", New Object(){index})
				Try
					Dim oldValue As Object = oldGetExp.value
					Dim newValue As Object = newGetExp.value
					out.writeExpression(oldGetExp)
					If Not Objects.Equals(newValue, out.get(oldValue)) Then invokeStatement(oldInstance, "set", New Object(){index, oldValue}, out)
				Catch e As Exception
					out.exceptionListener.exceptionThrown(e)
				End Try
			Next i
			For i As Integer = newSize To oldSize - 1
				invokeStatement(oldInstance, "add", New Object(){oldO(i)}, out)
			Next i
		End Sub
	End Class


	' Map
	Friend Class java_util_Map_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			' System.out.println("Initializing: " + newInstance);
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim oldMap As IDictionary(Of ?, ?) = CType(oldInstance, IDictionary)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim newMap As IDictionary(Of ?, ?) = CType(newInstance, IDictionary)
			' Remove the new elements.
			' Do this first otherwise we undo the adding work.
			If newMap IsNot Nothing Then
				For Each newKey As Object In newMap.Keys.ToArray()
				   ' PENDING: This "key" is not in the right environment.
					If Not oldMap.ContainsKey(newKey) Then invokeStatement(oldInstance, "remove", New Object(){newKey}, out)
				Next newKey
			End If
			' Add the new elements.
			For Each oldKey As Object In oldMap.Keys
				Dim oldGetExp As New Expression(oldInstance, "get", New Object(){oldKey})
				' Pending: should use newKey.
				Dim newGetExp As New Expression(newInstance, "get", New Object(){oldKey})
				Try
					Dim oldValue As Object = oldGetExp.value
					Dim newValue As Object = newGetExp.value
					out.writeExpression(oldGetExp)
					If Not Objects.Equals(newValue, out.get(oldValue)) Then
						invokeStatement(oldInstance, "put", New Object(){oldKey, oldValue}, out)
					ElseIf (newValue Is Nothing) AndAlso (Not newMap.ContainsKey(oldKey)) Then
						' put oldValue(=null?) if oldKey is absent in newMap
						invokeStatement(oldInstance, "put", New Object(){oldKey, oldValue}, out)
					End If
				Catch e As Exception
					out.exceptionListener.exceptionThrown(e)
				End Try
			Next oldKey
		End Sub
	End Class

	Friend NotInheritable Class java_util_AbstractCollection_PersistenceDelegate
		Inherits java_util_Collection_PersistenceDelegate

	End Class
	Friend NotInheritable Class java_util_AbstractList_PersistenceDelegate
		Inherits java_util_List_PersistenceDelegate

	End Class
	Friend NotInheritable Class java_util_AbstractMap_PersistenceDelegate
		Inherits java_util_Map_PersistenceDelegate

	End Class
	Friend NotInheritable Class java_util_Hashtable_PersistenceDelegate
		Inherits java_util_Map_PersistenceDelegate

	End Class


	' Beans
	Friend NotInheritable Class java_beans_beancontext_BeanContextSupport_PersistenceDelegate
		Inherits java_util_Collection_PersistenceDelegate

	End Class

	' AWT

	''' <summary>
	''' The persistence delegate for <seealso cref="Insets"/>.
	''' It is impossible to use <seealso cref="DefaultPersistenceDelegate"/>
	''' because this class does not have any properties.
	''' 
	''' @author Sergey A. Malenkov
	''' </summary>
	Friend NotInheritable Class java_awt_Insets_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return oldInstance.Equals(newInstance)
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Dim insets As java.awt.Insets = CType(oldInstance, java.awt.Insets)
			Dim args As Object() = { insets.top, insets.left, insets.bottom, insets.right }
			Return New Expression(insets, insets.GetType(), "new", args)
		End Function
	End Class

	''' <summary>
	''' The persistence delegate for <seealso cref="Font"/>.
	''' It is impossible to use <seealso cref="DefaultPersistenceDelegate"/>
	''' because size of the font can be float value.
	''' 
	''' @author Sergey A. Malenkov
	''' </summary>
	Friend NotInheritable Class java_awt_Font_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return oldInstance.Equals(newInstance)
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Dim font As java.awt.Font = CType(oldInstance, java.awt.Font)

			Dim count As Integer = 0
			Dim family As String = Nothing
			Dim style As Integer = java.awt.Font.PLAIN
			Dim size As Integer = 12

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim basic As Map(Of java.awt.font.TextAttribute, ?) = font.attributes
			Dim clone As Map(Of java.awt.font.TextAttribute, Object) = New HashMap(Of java.awt.font.TextAttribute, Object)(basic.size())
			For Each key As java.awt.font.TextAttribute In basic.Keys
				Dim value As Object = basic.get(key)
				If value IsNot Nothing Then clone.put(key, value)
				If key Is java.awt.font.TextAttribute.FAMILY Then
					If TypeOf value Is String Then
						count += 1
						family = CStr(value)
					End If
				ElseIf key Is java.awt.font.TextAttribute.WEIGHT Then
					If java.awt.font.TextAttribute.WEIGHT_REGULAR.Equals(value) Then
						count += 1
					ElseIf java.awt.font.TextAttribute.WEIGHT_BOLD.Equals(value) Then
						count += 1
						style = style Or java.awt.Font.BOLD
					End If
				ElseIf key Is java.awt.font.TextAttribute.POSTURE Then
					If java.awt.font.TextAttribute.POSTURE_REGULAR.Equals(value) Then
						count += 1
					ElseIf java.awt.font.TextAttribute.POSTURE_OBLIQUE.Equals(value) Then
						count += 1
						style = style Or java.awt.Font.ITALIC
					End If
				ElseIf key Is java.awt.font.TextAttribute.SIZE Then
					If TypeOf value Is Number Then
						Dim number As Number = CType(value, Number)
						size = number
						If size = number Then count += 1
					End If
				End If
			Next key
			Dim type As  [Class] = font.GetType()
			If count = clone.size() Then Return New Expression(font, type, "new", New Object(){family, style, size})
			If type Is GetType(java.awt.Font) Then Return New Expression(font, type, "getFont", New Object(){clone})
			Return New Expression(font, type, "new", New Object(){java.awt.Font.getFont(clone)})
		End Function
	End Class

	''' <summary>
	''' The persistence delegate for <seealso cref="AWTKeyStroke"/>.
	''' It is impossible to use <seealso cref="DefaultPersistenceDelegate"/>
	''' because this class have no public constructor.
	''' 
	''' @author Sergey A. Malenkov
	''' </summary>
	Friend NotInheritable Class java_awt_AWTKeyStroke_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return oldInstance.Equals(newInstance)
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Dim key As java.awt.AWTKeyStroke = CType(oldInstance, java.awt.AWTKeyStroke)

			Dim ch As Char = key.keyChar
			Dim code As Integer = key.keyCode
			Dim mask As Integer = key.modifiers
			Dim onKeyRelease As Boolean = key.onKeyRelease

			Dim args As Object() = Nothing
			If ch = java.awt.event.KeyEvent.CHAR_UNDEFINED Then
				args = If((Not onKeyRelease), New Object(){code, mask}, New Object){code, mask, onKeyRelease}
			ElseIf code = java.awt.event.KeyEvent.VK_UNDEFINED Then
				If Not onKeyRelease Then
					args = If(mask = 0, New Object(){ch}, New Object){ch, mask}
				ElseIf mask = 0 Then
					args = New Object(){ch, onKeyRelease}
				End If
			End If
			If args Is Nothing Then Throw New IllegalStateException("Unsupported KeyStroke: " & key)
			Dim type As  [Class] = key.GetType()
			Dim name As String = type.name
			' get short name of the class
			Dim index As Integer = name.LastIndexOf("."c) + 1
			If index > 0 Then name = name.Substring(index)
			Return New Expression(key, type, "get" & name, args)
		End Function
	End Class

	Friend Class StaticFieldsPersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overridable Sub installFields(ByVal out As Encoder, ByVal cls As [Class])
			If Modifier.isPublic(cls.modifiers) AndAlso isPackageAccessible(cls) Then
				Dim fields As Field() = cls.fields
				For i As Integer = 0 To fields.Length - 1
					Dim field As Field = fields(i)
					' Don't install primitives, their identity will not be preserved
					' by wrapping.
					If field.type.IsSubclassOf(GetType(Object)) Then out.writeExpression(New Expression(field, "get", New Object(){Nothing}))
				Next i
			End If
		End Sub

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Throw New RuntimeException("Unrecognized instance: " & oldInstance)
		End Function

		Public Overrides Sub writeObject(ByVal oldInstance As Object, ByVal out As Encoder)
			If out.getAttribute(Me) Is Nothing Then
				out.attributeute(Me, Boolean.TRUE)
				installFields(out, oldInstance.GetType())
			End If
			MyBase.writeObject(oldInstance, out)
		End Sub
	End Class

	' SystemColor
	Friend NotInheritable Class java_awt_SystemColor_PersistenceDelegate
		Inherits StaticFieldsPersistenceDelegate

	End Class

	' TextAttribute
	Friend NotInheritable Class java_awt_font_TextAttribute_PersistenceDelegate
		Inherits StaticFieldsPersistenceDelegate

	End Class

	' MenuShortcut
	Friend NotInheritable Class java_awt_MenuShortcut_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return oldInstance.Equals(newInstance)
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Dim m As java.awt.MenuShortcut = CType(oldInstance, java.awt.MenuShortcut)
			Return New Expression(oldInstance, m.GetType(), "new", New Object(){New Integer?(m.key), Convert.ToBoolean(m.usesShiftModifier())})
		End Function
	End Class

	' Component
	Friend NotInheritable Class java_awt_Component_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			Dim c As java.awt.Component = CType(oldInstance, java.awt.Component)
			Dim c2 As java.awt.Component = CType(newInstance, java.awt.Component)
			' The "background", "foreground" and "font" properties.
			' The foreground and font properties of Windows change from
			' null to defined values after the Windows are made visible -
			' special case them for now.
			If Not(TypeOf oldInstance Is java.awt.Window) Then
				Dim oldBackground As Object = If(c.backgroundSet, c.background, Nothing)
				Dim newBackground As Object = If(c2.backgroundSet, c2.background, Nothing)
				If Not Objects.Equals(oldBackground, newBackground) Then invokeStatement(oldInstance, "setBackground", New Object() { oldBackground }, out)
				Dim oldForeground As Object = If(c.foregroundSet, c.foreground, Nothing)
				Dim newForeground As Object = If(c2.foregroundSet, c2.foreground, Nothing)
				If Not Objects.Equals(oldForeground, newForeground) Then invokeStatement(oldInstance, "setForeground", New Object() { oldForeground }, out)
				Dim oldFont As Object = If(c.fontSet, c.font, Nothing)
				Dim newFont As Object = If(c2.fontSet, c2.font, Nothing)
				If Not Objects.Equals(oldFont, newFont) Then invokeStatement(oldInstance, "setFont", New Object() { oldFont }, out)
			End If

			' Bounds
			Dim p As java.awt.Container = c.parent
			If p Is Nothing OrElse p.layout Is Nothing Then
				' Use the most concise construct.
				Dim locationCorrect As Boolean = c.location.Equals(c2.location)
				Dim sizeCorrect As Boolean = c.size.Equals(c2.size)
				If (Not locationCorrect) AndAlso (Not sizeCorrect) Then
					invokeStatement(oldInstance, "setBounds", New Object(){c.bounds}, out)
				ElseIf Not locationCorrect Then
					invokeStatement(oldInstance, "setLocation", New Object(){c.location}, out)
				ElseIf Not sizeCorrect Then
					invokeStatement(oldInstance, "setSize", New Object(){c.size}, out)
				End If
			End If
		End Sub
	End Class

	' Container
	Friend NotInheritable Class java_awt_Container_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			' Ignore the children of a JScrollPane.
			' Pending(milne) find a better way to do this.
			If TypeOf oldInstance Is javax.swing.JScrollPane Then Return
			Dim oldC As java.awt.Container = CType(oldInstance, java.awt.Container)
			Dim oldChildren As java.awt.Component() = oldC.components
			Dim newC As java.awt.Container = CType(newInstance, java.awt.Container)
			Dim newChildren As java.awt.Component() = If(newC Is Nothing, New java.awt.Component(){}, newC.components)

			Dim layout As java.awt.BorderLayout = If(TypeOf oldC.layout Is java.awt.BorderLayout, CType(oldC.layout, java.awt.BorderLayout), Nothing)

			Dim oldLayeredPane As javax.swing.JLayeredPane = If(TypeOf oldInstance Is javax.swing.JLayeredPane, CType(oldInstance, javax.swing.JLayeredPane), Nothing)

			' Pending. Assume all the new children are unaltered.
			For i As Integer = newChildren.Length To oldChildren.Length - 1
				Dim args As Object() = If(layout IsNot Nothing, New Object() {oldChildren(i), layout.getConstraints(oldChildren(i))}, If(oldLayeredPane IsNot Nothing, New Object() {oldChildren(i), oldLayeredPane.getLayer(oldChildren(i)), Convert.ToInt32(-1)}, New Object)){oldChildren(i)}

				invokeStatement(oldInstance, "add", args, out)
			Next i
		End Sub
	End Class

	' Choice
	Friend NotInheritable Class java_awt_Choice_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			Dim m As java.awt.Choice = CType(oldInstance, java.awt.Choice)
			Dim n As java.awt.Choice = CType(newInstance, java.awt.Choice)
			For i As Integer = n.itemCount To m.itemCount - 1
				invokeStatement(oldInstance, "add", New Object(){m.getItem(i)}, out)
			Next i
		End Sub
	End Class

	' Menu
	Friend NotInheritable Class java_awt_Menu_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			Dim m As java.awt.Menu = CType(oldInstance, java.awt.Menu)
			Dim n As java.awt.Menu = CType(newInstance, java.awt.Menu)
			For i As Integer = n.itemCount To m.itemCount - 1
				invokeStatement(oldInstance, "add", New Object(){m.getItem(i)}, out)
			Next i
		End Sub
	End Class

	' MenuBar
	Friend NotInheritable Class java_awt_MenuBar_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			Dim m As java.awt.MenuBar = CType(oldInstance, java.awt.MenuBar)
			Dim n As java.awt.MenuBar = CType(newInstance, java.awt.MenuBar)
			For i As Integer = n.menuCount To m.menuCount - 1
				invokeStatement(oldInstance, "add", New Object(){m.getMenu(i)}, out)
			Next i
		End Sub
	End Class

	' List
	Friend NotInheritable Class java_awt_List_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			Dim m As java.awt.List = CType(oldInstance, java.awt.List)
			Dim n As java.awt.List = CType(newInstance, java.awt.List)
			For i As Integer = n.itemCount To m.itemCount - 1
				invokeStatement(oldInstance, "add", New Object(){m.getItem(i)}, out)
			Next i
		End Sub
	End Class


	' LayoutManagers

	' BorderLayout
	Friend NotInheritable Class java_awt_BorderLayout_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Private Shared ReadOnly CONSTRAINTS As String() = { java.awt.BorderLayout.NORTH, java.awt.BorderLayout.SOUTH, java.awt.BorderLayout.EAST, java.awt.BorderLayout.WEST, java.awt.BorderLayout.CENTER, java.awt.BorderLayout.PAGE_START, java.awt.BorderLayout.PAGE_END, java.awt.BorderLayout.LINE_START, java.awt.BorderLayout.LINE_END }
		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			Dim oldLayout As java.awt.BorderLayout = CType(oldInstance, java.awt.BorderLayout)
			Dim newLayout As java.awt.BorderLayout = CType(newInstance, java.awt.BorderLayout)
			For Each constraints_Renamed As String In CONSTRAINTS
				Dim oldC As Object = oldLayout.getLayoutComponent(constraints_Renamed)
				Dim newC As Object = newLayout.getLayoutComponent(constraints_Renamed)
				' Pending, assume any existing elements are OK.
				If oldC IsNot Nothing AndAlso newC Is Nothing Then invokeStatement(oldInstance, "addLayoutComponent", New Object() { oldC, constraints_Renamed }, out)
			Next constraints_Renamed
		End Sub
	End Class

	' CardLayout
	Friend NotInheritable Class java_awt_CardLayout_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			If getVector(newInstance).empty Then
				For Each card As Object In getVector(oldInstance)
					Dim args As Object() = {MetaData.getPrivateFieldValue(card, "java.awt.CardLayout$Card.name"), MetaData.getPrivateFieldValue(card, "java.awt.CardLayout$Card.comp")}
					invokeStatement(oldInstance, "addLayoutComponent", args, out)
				Next card
			End If
		End Sub
		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return MyBase.mutatesTo(oldInstance, newInstance) AndAlso getVector(newInstance).empty
		End Function
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function getVector(ByVal instance As Object) As Vector(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return CType(MetaData.getPrivateFieldValue(instance, "java.awt.CardLayout.vector"), Vector(Of ?))
		End Function
	End Class

	' GridBagLayout
	Friend NotInheritable Class java_awt_GridBagLayout_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			If getHashtable(newInstance).empty Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				For Each entry As KeyValuePair(Of ?, ?) In getHashtable(oldInstance).entrySet()
					Dim args As Object() = {entry.Key, entry.Value}
					invokeStatement(oldInstance, "addLayoutComponent", args, out)
				Next entry
			End If
		End Sub
		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return MyBase.mutatesTo(oldInstance, newInstance) AndAlso getHashtable(newInstance).empty
		End Function
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function getHashtable(ByVal instance As Object) As Dictionary(Of ?, ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return CType(MetaData.getPrivateFieldValue(instance, "java.awt.GridBagLayout.comptable"), Dictionary(Of ?, ?))
		End Function
	End Class

	' Swing

	' JFrame (If we do this for Window instead of JFrame, the setVisible call
	' will be issued before we have added all the children to the JFrame and
	' will appear blank).
	Friend NotInheritable Class javax_swing_JFrame_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			Dim oldC As java.awt.Window = CType(oldInstance, java.awt.Window)
			Dim newC As java.awt.Window = CType(newInstance, java.awt.Window)
			Dim oldV As Boolean = oldC.visible
			Dim newV As Boolean = newC.visible
			If newV <> oldV Then
				' false means: don't execute this statement at write time.
				Dim executeStatements As Boolean = out.executeStatements
				out.executeStatements = False
				invokeStatement(oldInstance, "setVisible", New Object(){Convert.ToBoolean(oldV)}, out)
				out.executeStatements = executeStatements
			End If
		End Sub
	End Class

	' Models

	' DefaultListModel
	Friend NotInheritable Class javax_swing_DefaultListModel_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			' Note, the "size" property will be set here.
			MyBase.initialize(type, oldInstance, newInstance, out)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim m As javax.swing.DefaultListModel(Of ?) = CType(oldInstance, javax.swing.DefaultListModel(Of ?))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim n As javax.swing.DefaultListModel(Of ?) = CType(newInstance, javax.swing.DefaultListModel(Of ?))
			For i As Integer = n.size To m.size - 1
				invokeStatement(oldInstance, "add", New Object(){m.getElementAt(i)}, out) ' Can also use "addElement".
			Next i
		End Sub
	End Class

	' DefaultComboBoxModel
	Friend NotInheritable Class javax_swing_DefaultComboBoxModel_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim m As javax.swing.DefaultComboBoxModel(Of ?) = CType(oldInstance, javax.swing.DefaultComboBoxModel(Of ?))
			For i As Integer = 0 To m.size - 1
				invokeStatement(oldInstance, "addElement", New Object(){m.getElementAt(i)}, out)
			Next i
		End Sub
	End Class


	' DefaultMutableTreeNode
	Friend NotInheritable Class javax_swing_tree_DefaultMutableTreeNode_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			Dim m As javax.swing.tree.DefaultMutableTreeNode = CType(oldInstance, javax.swing.tree.DefaultMutableTreeNode)
			Dim n As javax.swing.tree.DefaultMutableTreeNode = CType(newInstance, javax.swing.tree.DefaultMutableTreeNode)
			For i As Integer = n.childCount To m.childCount - 1
				invokeStatement(oldInstance, "add", New Object(){m.getChildAt(i)}, out)
			Next i
		End Sub
	End Class

	' ToolTipManager
	Friend NotInheritable Class javax_swing_ToolTipManager_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Return New Expression(oldInstance, GetType(javax.swing.ToolTipManager), "sharedInstance", New Object(){})
		End Function
	End Class

	' JTabbedPane
	Friend NotInheritable Class javax_swing_JTabbedPane_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			Dim p As javax.swing.JTabbedPane = CType(oldInstance, javax.swing.JTabbedPane)
			For i As Integer = 0 To p.tabCount - 1
				invokeStatement(oldInstance, "addTab", New Object(){ p.getTitleAt(i), p.getIconAt(i), p.getComponentAt(i)}, out)
			Next i
		End Sub
	End Class

	' Box
	Friend NotInheritable Class javax_swing_Box_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return MyBase.mutatesTo(oldInstance, newInstance) AndAlso getAxis(oldInstance).Equals(getAxis(newInstance))
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Return New Expression(oldInstance, oldInstance.GetType(), "new", New Object() {getAxis(oldInstance)})
		End Function

		Private Function getAxis(ByVal [object] As Object) As Integer?
			Dim box As javax.swing.Box = CType(object_Renamed, javax.swing.Box)
			Return CInt(Fix(MetaData.getPrivateFieldValue(box.layout, "javax.swing.BoxLayout.axis")))
		End Function
	End Class

	' JMenu
	' Note that we do not need to state the initialiser for
	' JMenuItems since the getComponents() method defined in
	' Container will return all of the sub menu items that
	' need to be added to the menu item.
	' Not so for JMenu apparently.
	Friend NotInheritable Class javax_swing_JMenu_PersistenceDelegate
		Inherits DefaultPersistenceDelegate

		Protected Friend Overrides Sub initialize(ByVal type As [Class], ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			MyBase.initialize(type, oldInstance, newInstance, out)
			Dim m As javax.swing.JMenu = CType(oldInstance, javax.swing.JMenu)
			Dim c As java.awt.Component() = m.menuComponents
			For i As Integer = 0 To c.Length - 1
				invokeStatement(oldInstance, "add", New Object(){c(i)}, out)
			Next i
		End Sub
	End Class

	''' <summary>
	''' The persistence delegate for <seealso cref="MatteBorder"/>.
	''' It is impossible to use <seealso cref="DefaultPersistenceDelegate"/>
	''' because this class does not have writable properties.
	''' 
	''' @author Sergey A. Malenkov
	''' </summary>
	Friend NotInheritable Class javax_swing_border_MatteBorder_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Dim border As javax.swing.border.MatteBorder = CType(oldInstance, javax.swing.border.MatteBorder)
			Dim insets As java.awt.Insets = border.borderInsets
			Dim object_Renamed As Object = border.tileIcon
			If object_Renamed Is Nothing Then object_Renamed = border.matteColor
			Dim args As Object() = { insets.top, insets.left, insets.bottom, insets.right, object_Renamed }
			Return New Expression(border, border.GetType(), "new", args)
		End Function
	End Class

	' XXX - doens't seem to work. Debug later.
	'static final class javax_swing_JMenu_PersistenceDelegate extends DefaultPersistenceDelegate {
	'    protected void initialize(Class<?> type, Object oldInstance, Object newInstance, Encoder out) {
	'        super.initialize(type, oldInstance, newInstance, out);
	'        javax.swing.JMenu m = (javax.swing.JMenu)oldInstance;
	'        javax.swing.JMenu n = (javax.swing.JMenu)newInstance;
	'        for (int i = n.getItemCount(); i < m.getItemCount(); i++) {
	'            invokeStatement(oldInstance, "add", new Object[]{m.getItem(i)}, out);
	'        }
	'    }
	'}
	'

	''' <summary>
	''' The persistence delegate for <seealso cref="PrintColorUIResource"/>.
	''' It is impossible to use <seealso cref="DefaultPersistenceDelegate"/>
	''' because this class has special rule for serialization:
	''' it should be converted to <seealso cref="ColorUIResource"/>.
	''' </summary>
	''' <seealso cref= PrintColorUIResource#writeReplace
	''' 
	''' @author Sergey A. Malenkov </seealso>
	Friend NotInheritable Class sun_swing_PrintColorUIResource_PersistenceDelegate
		Inherits PersistenceDelegate

		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			Return oldInstance.Equals(newInstance)
		End Function

		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Dim color As java.awt.Color = CType(oldInstance, java.awt.Color)
			Dim args As Object() = {color.rGB}
			Return New Expression(color, GetType(javax.swing.plaf.ColorUIResource), "new", args)
		End Function
	End Class

		Private Shared ReadOnly fields As Map(Of String, Field) = Collections.synchronizedMap(New WeakHashMap(Of String, Field))
		Private Shared internalPersistenceDelegates As New Dictionary(Of String, PersistenceDelegate)

		Private Shared nullPersistenceDelegate As PersistenceDelegate = New NullPersistenceDelegate
		Private Shared enumPersistenceDelegate As PersistenceDelegate = New EnumPersistenceDelegate
		Private Shared primitivePersistenceDelegate As PersistenceDelegate = New PrimitivePersistenceDelegate
		Private Shared defaultPersistenceDelegate_Renamed As PersistenceDelegate = New DefaultPersistenceDelegate
		Private Shared arrayPersistenceDelegate As PersistenceDelegate
		Private Shared proxyPersistenceDelegate As PersistenceDelegate

		Shared Sub New()

			internalPersistenceDelegates.put("java.net.URI", New PrimitivePersistenceDelegate)

			' it is possible because MatteBorder is assignable from MatteBorderUIResource
			internalPersistenceDelegates.put("javax.swing.plaf.BorderUIResource$MatteBorderUIResource", New javax_swing_border_MatteBorder_PersistenceDelegate)

			' it is possible because FontUIResource is supported by java_awt_Font_PersistenceDelegate
			internalPersistenceDelegates.put("javax.swing.plaf.FontUIResource", New java_awt_Font_PersistenceDelegate)

			' it is possible because KeyStroke is supported by java_awt_AWTKeyStroke_PersistenceDelegate
			internalPersistenceDelegates.put("javax.swing.KeyStroke", New java_awt_AWTKeyStroke_PersistenceDelegate)

			internalPersistenceDelegates.put("java.sql.Date", New java_util_Date_PersistenceDelegate)
			internalPersistenceDelegates.put("java.sql.Time", New java_util_Date_PersistenceDelegate)

			internalPersistenceDelegates.put("java.util.JumboEnumSet", New java_util_EnumSet_PersistenceDelegate)
			internalPersistenceDelegates.put("java.util.RegularEnumSet", New java_util_EnumSet_PersistenceDelegate)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Function getPersistenceDelegate(ByVal type As [Class]) As PersistenceDelegate
			If type Is Nothing Then Return nullPersistenceDelegate
			If type.IsSubclassOf(GetType(System.Enum)) Then Return enumPersistenceDelegate
			If Nothing IsNot XMLEncoder.primitiveTypeFor(type) Then Return primitivePersistenceDelegate
			' The persistence delegate for arrays is non-trivial; instantiate it lazily.
			If type.array Then
				If arrayPersistenceDelegate Is Nothing Then arrayPersistenceDelegate = New ArrayPersistenceDelegate
				Return arrayPersistenceDelegate
			End If
			' Handle proxies lazily for backward compatibility with 1.2.
			Try
				If java.lang.reflect.Proxy.isProxyClass(type) Then
					If proxyPersistenceDelegate Is Nothing Then proxyPersistenceDelegate = New ProxyPersistenceDelegate
					Return proxyPersistenceDelegate
				End If
			Catch e As Exception
			End Try
			' else if (type.getDeclaringClass() != null) {
			'     return new DefaultPersistenceDelegate(new String[]{"this$0"});
			' }

			Dim typeName As String = type.name
			Dim pd As PersistenceDelegate = CType(getBeanAttribute(type, "persistenceDelegate"), PersistenceDelegate)
			If pd Is Nothing Then
				pd = internalPersistenceDelegates.get(typeName)
				If pd IsNot Nothing Then Return pd
				internalPersistenceDelegates.put(typeName, defaultPersistenceDelegate_Renamed)
				Try
					Dim name As String = type.name
					Dim c As  [Class] = Type.GetType("java.beans.MetaData$" & name.replace("."c, "_"c) & "_PersistenceDelegate")
					pd = CType(c.newInstance(), PersistenceDelegate)
					internalPersistenceDelegates.put(typeName, pd)
				Catch e As  [Class]NotFoundException
					Dim properties As String() = getConstructorProperties(type)
					If properties IsNot Nothing Then
						pd = New DefaultPersistenceDelegate(properties)
						internalPersistenceDelegates.put(typeName, pd)
					End If
				Catch e As Exception
					Console.Error.WriteLine("Internal error: " & e)
				End Try
			End If

			Return If(pd IsNot Nothing, pd, defaultPersistenceDelegate_Renamed)
		End Function

		Private Shared Function getConstructorProperties(ByVal type As [Class]) As String()
			Dim names As String() = Nothing
			Dim length As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each constructor As Constructor(Of ?) In type.constructors
				Dim value As String() = getAnnotationValue(constructor)
				If (value IsNot Nothing) AndAlso (length < value.Length) AndAlso isValid(constructor, value) Then
					names = value
					length = value.Length
				End If
			Next constructor
			Return names
		End Function

		Private Shared Function getAnnotationValue(Of T1)(ByVal constructor As Constructor(Of T1)) As String()
			Dim annotation As ConstructorProperties = constructor.getAnnotation(GetType(ConstructorProperties))
			Return If(annotation IsNot Nothing, annotation.value(), Nothing)
		End Function

		Private Shared Function isValid(Of T1)(ByVal constructor As Constructor(Of T1), ByVal names As String()) As Boolean
			Dim parameters As  [Class]() = constructor.parameterTypes
			If names.Length <> parameters.Length Then Return False
			For Each name As String In names
				If name Is Nothing Then Return False
			Next name
			Return True
		End Function

		Private Shared Function getBeanAttribute(ByVal type As [Class], ByVal attribute As String) As Object
			Try
				Return Introspector.getBeanInfo(type).beanDescriptor.getValue(attribute)
			Catch exception_Renamed As IntrospectionException
				Return Nothing
			End Try
		End Function

		Friend Shared Function getPrivateFieldValue(ByVal instance As Object, ByVal name As String) As Object
			Dim field As Field = fields.get(name)
			If field Is Nothing Then
				Dim index As Integer = name.LastIndexOf("."c)
				Dim className As String = name.Substring(0, index)
				Dim fieldName As String = name.Substring(1 + index)
				field = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
				fields.put(name, field)
			End If
			Try
				Return field.get(instance)
			Catch exception_Renamed As IllegalAccessException
				Throw New IllegalStateException("Could not get value of the field", exception_Renamed)
			End Try
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Field
				Try
					Dim field As Field = Type.GetType(className).getDeclaredField(fieldName)
					field.accessible = True
					Return field
				Catch exception_Renamed As  [Class]NotFoundException
					Throw New IllegalStateException("Could not find class", exception_Renamed)
				Catch exception_Renamed As NoSuchFieldException
					Throw New IllegalStateException("Could not find field", exception_Renamed)
				End Try
			End Function
		End Class
	End Class

End Namespace
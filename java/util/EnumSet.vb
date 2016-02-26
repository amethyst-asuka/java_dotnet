Imports System

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util


	''' <summary>
	''' A specialized <seealso cref="Set"/> implementation for use with enum types.  All of
	''' the elements in an enum set must come from a single enum type that is
	''' specified, explicitly or implicitly, when the set is created.  Enum sets
	''' are represented internally as bit vectors.  This representation is
	''' extremely compact and efficient. The space and time performance of this
	''' class should be good enough to allow its use as a high-quality, typesafe
	''' alternative to traditional <tt>int</tt>-based "bit flags."  Even bulk
	''' operations (such as <tt>containsAll</tt> and <tt>retainAll</tt>) should
	''' run very quickly if their argument is also an enum set.
	''' 
	''' <p>The iterator returned by the <tt>iterator</tt> method traverses the
	''' elements in their <i>natural order</i> (the order in which the enum
	''' constants are declared).  The returned iterator is <i>weakly
	''' consistent</i>: it will never throw <seealso cref="ConcurrentModificationException"/>
	''' and it may or may not show the effects of any modifications to the set that
	''' occur while the iteration is in progress.
	''' 
	''' <p>Null elements are not permitted.  Attempts to insert a null element
	''' will throw <seealso cref="NullPointerException"/>.  Attempts to test for the
	''' presence of a null element or to remove one will, however, function
	''' properly.
	''' 
	''' <P>Like most collection implementations, <tt>EnumSet</tt> is not
	''' synchronized.  If multiple threads access an enum set concurrently, and at
	''' least one of the threads modifies the set, it should be synchronized
	''' externally.  This is typically accomplished by synchronizing on some
	''' object that naturally encapsulates the enum set.  If no such object exists,
	''' the set should be "wrapped" using the <seealso cref="Collections#synchronizedSet"/>
	''' method.  This is best done at creation time, to prevent accidental
	''' unsynchronized access:
	''' 
	''' <pre>
	''' Set&lt;MyEnum&gt; s = Collections.synchronizedSet(EnumSet.noneOf(MyEnum.class));
	''' </pre>
	''' 
	''' <p>Implementation note: All basic operations execute in constant time.
	''' They are likely (though not guaranteed) to be much faster than their
	''' <seealso cref="HashSet"/> counterparts.  Even bulk operations execute in
	''' constant time if their argument is also an enum set.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @author Josh Bloch
	''' @since 1.5 </summary>
	''' <seealso cref= EnumMap
	''' @serial exclude </seealso>
	<Serializable> _
	Public MustInherit Class EnumSet(Of E As System.Enum(Of E))
		Inherits AbstractSet(Of E)
		Implements Cloneable

		''' <summary>
		''' The class of all the elements of this set.
		''' </summary>
		Friend ReadOnly elementType As  [Class]

		''' <summary>
		''' All of the values comprising T.  (Cached for performance.)
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend ReadOnly universe As System.Enum(Of ?)()

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared ZERO_LENGTH_ENUM_ARRAY As System.Enum(Of ?)() = New [Enum](Of ?)(){}

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		EnumSet(ClasselementType, Enum<?>() universe)
			Me.elementType = elementType
			Me.universe = universe

		''' <summary>
		''' Creates an empty enum set with the specified element type.
		''' </summary>
		''' @param <E> The class of the elements in the set </param>
		''' <param name="elementType"> the class object of the element type for this enum
		'''     set </param>
		''' <returns> An empty enum set of the specified type. </returns>
		''' <exception cref="NullPointerException"> if <tt>elementType</tt> is null </exception>
		public static (Of E As System.Enum(Of E)) EnumSet(Of E) noneOf(Class elementType)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim universe_Renamed As System.Enum(Of ?)() = getUniverse(elementType)
			If universe_Renamed Is Nothing Then Throw New ClassCastException(elementType & " not an enum")

			If universe_Renamed.Length <= 64 Then
				Return New RegularEnumSet(Of )(elementType, universe_Renamed)
			Else
				Return New JumboEnumSet(Of )(elementType, universe_Renamed)
			End If

		''' <summary>
		''' Creates an enum set containing all of the elements in the specified
		''' element type.
		''' </summary>
		''' @param <E> The class of the elements in the set </param>
		''' <param name="elementType"> the class object of the element type for this enum
		'''     set </param>
		''' <returns> An enum set containing all the elements in the specified type. </returns>
		''' <exception cref="NullPointerException"> if <tt>elementType</tt> is null </exception>
		public static (Of E As System.Enum(Of E)) EnumSet(Of E) allOf(Class elementType)
			Dim result As EnumSet(Of E) = noneOf(elementType)
			result.addAll()
			Return result

		''' <summary>
		''' Adds all of the elements from the appropriate enum type to this enum
		''' set, which is empty prior to the call.
		''' </summary>
		abstract void addAll()

		''' <summary>
		''' Creates an enum set with the same element type as the specified enum
		''' set, initially containing the same elements (if any).
		''' </summary>
		''' @param <E> The class of the elements in the set </param>
		''' <param name="s"> the enum set from which to initialize this enum set </param>
		''' <returns> A copy of the specified enum set. </returns>
		''' <exception cref="NullPointerException"> if <tt>s</tt> is null </exception>
		public static (Of E As System.Enum(Of E)) EnumSet(Of E) copyOf(EnumSet(Of E) s)
			Return s.clone()

		''' <summary>
		''' Creates an enum set initialized from the specified collection.  If
		''' the specified collection is an <tt>EnumSet</tt> instance, this static
		''' factory method behaves identically to <seealso cref="#copyOf(EnumSet)"/>.
		''' Otherwise, the specified collection must contain at least one element
		''' (in order to determine the new enum set's element type).
		''' </summary>
		''' @param <E> The class of the elements in the collection </param>
		''' <param name="c"> the collection from which to initialize this enum set </param>
		''' <returns> An enum set initialized from the given collection. </returns>
		''' <exception cref="IllegalArgumentException"> if <tt>c</tt> is not an
		'''     <tt>EnumSet</tt> instance and contains no elements </exception>
		''' <exception cref="NullPointerException"> if <tt>c</tt> is null </exception>
		public static (Of E As System.Enum(Of E)) EnumSet(Of E) copyOf(Collection(Of E) c)
			If TypeOf c Is EnumSet Then
				Return CType(c, EnumSet(Of E)).clone()
			Else
				If c.empty Then Throw New IllegalArgumentException("Collection is empty")
				Dim i As [Iterator](Of E) = c.GetEnumerator()
				Dim first As E = i.next()
				Dim result As EnumSet(Of E) = EnumSet.of(first)
				Do While i.hasNext()
					result.add(i.next())
				Loop
				Return result
			End If

		''' <summary>
		''' Creates an enum set with the same element type as the specified enum
		''' set, initially containing all the elements of this type that are
		''' <i>not</i> contained in the specified set.
		''' </summary>
		''' @param <E> The class of the elements in the enum set </param>
		''' <param name="s"> the enum set from whose complement to initialize this enum set </param>
		''' <returns> The complement of the specified set in this set </returns>
		''' <exception cref="NullPointerException"> if <tt>s</tt> is null </exception>
		public static (Of E As System.Enum(Of E)) EnumSet(Of E) complementOf(EnumSet(Of E) s)
			Dim result As EnumSet(Of E) = copyOf(s)
			result.complement()
			Return result

		''' <summary>
		''' Creates an enum set initially containing the specified element.
		''' 
		''' Overloadings of this method exist to initialize an enum set with
		''' one through five elements.  A sixth overloading is provided that
		''' uses the varargs feature.  This overloading may be used to create
		''' an enum set initially containing an arbitrary number of elements, but
		''' is likely to run slower than the overloadings that do not use varargs.
		''' </summary>
		''' @param <E> The class of the specified element and of the set </param>
		''' <param name="e"> the element that this set is to contain initially </param>
		''' <exception cref="NullPointerException"> if <tt>e</tt> is null </exception>
		''' <returns> an enum set initially containing the specified element </returns>
		public static (Of E As System.Enum(Of E)) EnumSet(Of E) [of](E e)
			Dim result As EnumSet(Of E) = noneOf(e.declaringClass)
			result.add(e)
			Return result

		''' <summary>
		''' Creates an enum set initially containing the specified elements.
		''' 
		''' Overloadings of this method exist to initialize an enum set with
		''' one through five elements.  A sixth overloading is provided that
		''' uses the varargs feature.  This overloading may be used to create
		''' an enum set initially containing an arbitrary number of elements, but
		''' is likely to run slower than the overloadings that do not use varargs.
		''' </summary>
		''' @param <E> The class of the parameter elements and of the set </param>
		''' <param name="e1"> an element that this set is to contain initially </param>
		''' <param name="e2"> another element that this set is to contain initially </param>
		''' <exception cref="NullPointerException"> if any parameters are null </exception>
		''' <returns> an enum set initially containing the specified elements </returns>
		public static (Of E As System.Enum(Of E)) EnumSet(Of E) [of](E e1, E e2)
			Dim result As EnumSet(Of E) = noneOf(e1.declaringClass)
			result.add(e1)
			result.add(e2)
			Return result

		''' <summary>
		''' Creates an enum set initially containing the specified elements.
		''' 
		''' Overloadings of this method exist to initialize an enum set with
		''' one through five elements.  A sixth overloading is provided that
		''' uses the varargs feature.  This overloading may be used to create
		''' an enum set initially containing an arbitrary number of elements, but
		''' is likely to run slower than the overloadings that do not use varargs.
		''' </summary>
		''' @param <E> The class of the parameter elements and of the set </param>
		''' <param name="e1"> an element that this set is to contain initially </param>
		''' <param name="e2"> another element that this set is to contain initially </param>
		''' <param name="e3"> another element that this set is to contain initially </param>
		''' <exception cref="NullPointerException"> if any parameters are null </exception>
		''' <returns> an enum set initially containing the specified elements </returns>
		public static (Of E As System.Enum(Of E)) EnumSet(Of E) [of](E e1, E e2, E e3)
			Dim result As EnumSet(Of E) = noneOf(e1.declaringClass)
			result.add(e1)
			result.add(e2)
			result.add(e3)
			Return result

		''' <summary>
		''' Creates an enum set initially containing the specified elements.
		''' 
		''' Overloadings of this method exist to initialize an enum set with
		''' one through five elements.  A sixth overloading is provided that
		''' uses the varargs feature.  This overloading may be used to create
		''' an enum set initially containing an arbitrary number of elements, but
		''' is likely to run slower than the overloadings that do not use varargs.
		''' </summary>
		''' @param <E> The class of the parameter elements and of the set </param>
		''' <param name="e1"> an element that this set is to contain initially </param>
		''' <param name="e2"> another element that this set is to contain initially </param>
		''' <param name="e3"> another element that this set is to contain initially </param>
		''' <param name="e4"> another element that this set is to contain initially </param>
		''' <exception cref="NullPointerException"> if any parameters are null </exception>
		''' <returns> an enum set initially containing the specified elements </returns>
		public static (Of E As System.Enum(Of E)) EnumSet(Of E) [of](E e1, E e2, E e3, E e4)
			Dim result As EnumSet(Of E) = noneOf(e1.declaringClass)
			result.add(e1)
			result.add(e2)
			result.add(e3)
			result.add(e4)
			Return result

		''' <summary>
		''' Creates an enum set initially containing the specified elements.
		''' 
		''' Overloadings of this method exist to initialize an enum set with
		''' one through five elements.  A sixth overloading is provided that
		''' uses the varargs feature.  This overloading may be used to create
		''' an enum set initially containing an arbitrary number of elements, but
		''' is likely to run slower than the overloadings that do not use varargs.
		''' </summary>
		''' @param <E> The class of the parameter elements and of the set </param>
		''' <param name="e1"> an element that this set is to contain initially </param>
		''' <param name="e2"> another element that this set is to contain initially </param>
		''' <param name="e3"> another element that this set is to contain initially </param>
		''' <param name="e4"> another element that this set is to contain initially </param>
		''' <param name="e5"> another element that this set is to contain initially </param>
		''' <exception cref="NullPointerException"> if any parameters are null </exception>
		''' <returns> an enum set initially containing the specified elements </returns>
		public static (Of E As System.Enum(Of E)) EnumSet(Of E) [of](E e1, E e2, E e3, E e4, E e5)
			Dim result As EnumSet(Of E) = noneOf(e1.declaringClass)
			result.add(e1)
			result.add(e2)
			result.add(e3)
			result.add(e4)
			result.add(e5)
			Return result

		''' <summary>
		''' Creates an enum set initially containing the specified elements.
		''' This factory, whose parameter list uses the varargs feature, may
		''' be used to create an enum set initially containing an arbitrary
		''' number of elements, but it is likely to run slower than the overloadings
		''' that do not use varargs.
		''' </summary>
		''' @param <E> The class of the parameter elements and of the set </param>
		''' <param name="first"> an element that the set is to contain initially </param>
		''' <param name="rest"> the remaining elements the set is to contain initially </param>
		''' <exception cref="NullPointerException"> if any of the specified elements are null,
		'''     or if <tt>rest</tt> is null </exception>
		''' <returns> an enum set initially containing the specified elements </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public static (Of E As System.Enum(Of E)) EnumSet(Of E) [of](E first, E... rest)
			Dim result As EnumSet(Of E) = noneOf(first.declaringClass)
			result.add(first)
			For Each e As E In rest
				result.add(e)
			Next e
			Return result

		''' <summary>
		''' Creates an enum set initially containing all of the elements in the
		''' range defined by the two specified endpoints.  The returned set will
		''' contain the endpoints themselves, which may be identical but must not
		''' be out of order.
		''' </summary>
		''' @param <E> The class of the parameter elements and of the set </param>
		''' <param name="from"> the first element in the range </param>
		''' <param name="to"> the last element in the range </param>
		''' <exception cref="NullPointerException"> if {@code from} or {@code to} are null </exception>
		''' <exception cref="IllegalArgumentException"> if {@code from.compareTo(to) > 0} </exception>
		''' <returns> an enum set initially containing all of the elements in the
		'''         range defined by the two specified endpoints </returns>
		public static (Of E As System.Enum(Of E)) EnumSet(Of E) range(E from, E to)
			If from.CompareTo(to) > 0 Then Throw New IllegalArgumentException(from & " > " & to)
			Dim result As EnumSet(Of E) = noneOf(from.declaringClass)
			result.addRange(from, to)
			Return result

		''' <summary>
		''' Adds the specified range to this enum set, which is empty prior
		''' to the call.
		''' </summary>
		abstract void addRange(E from, E to)

		''' <summary>
		''' Returns a copy of this set.
		''' </summary>
		''' <returns> a copy of this set </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public EnumSet(Of E) clone()
			Try
				Return CType(MyBase.clone(), EnumSet(Of E))
			Catch e As CloneNotSupportedException
				Throw New AssertionError(e)
			End Try

		''' <summary>
		''' Complements the contents of this enum set.
		''' </summary>
		abstract void complement()

		''' <summary>
		''' Throws an exception if e is not of the correct type for this enum set.
		''' </summary>
		final void typeCheck(E e)
			Dim eClass As  [Class] = e.GetType()
			If eClass IsNot elementType AndAlso eClass.BaseType IsNot elementType Then Throw New ClassCastException(eClass & " != " & elementType)

		''' <summary>
		''' Returns all of the values comprising E.
		''' The result is uncloned, cached, and shared by all callers.
		''' </summary>
		private static (Of E As System.Enum(Of E)) E() getUniverse(Class elementType)
			Return sun.misc.SharedSecrets.javaLangAccess.getEnumConstantsShared(elementType)

		''' <summary>
		''' This class is used to serialize all EnumSet instances, regardless of
		''' implementation type.  It captures their "logical contents" and they
		''' are reconstructed using public static factories.  This is necessary
		''' to ensure that the existence of a particular implementation type is
		''' an implementation detail.
		''' 
		''' @serial include
		''' </summary>
		private static class SerializationProxy (Of E As System.Enum(Of E)) implements java.io.Serializable
			''' <summary>
			''' The element type of this enum set.
			''' 
			''' @serial
			''' </summary>
			private final Class elementType

			''' <summary>
			''' The elements contained in this enum set.
			''' 
			''' @serial
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			private final Enum(Of ?)() elements

			SerializationProxy(EnumSet(Of E) set)
				elementType = set.elementType
				elements = set.ToArray(ZERO_LENGTH_ENUM_ARRAY)

			' instead of cast to E, we should perhaps use elementType.cast()
			' to avoid injection of forged stream, but it will slow the implementation
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			private Object readResolve()
				Dim result As EnumSet(Of E) = EnumSet.noneOf(elementType)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				For Each e As System.Enum(Of ?) In elements
					result.add(CType(e, E))
				Next e
				Return result

			private static final Long serialVersionUID = 362491234563181265L

		Object writeReplace()
			Return New SerializationProxy(Of )(Me)

		' readObject method for the serialization proxy pattern
		' See Effective Java, Second Ed., Item 78.
		private void readObject(java.io.ObjectInputStream stream) throws java.io.InvalidObjectException
			Throw New java.io.InvalidObjectException("Proxy required")
	End Class

End Namespace
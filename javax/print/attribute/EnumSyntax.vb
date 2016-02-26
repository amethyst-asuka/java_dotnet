Imports System

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


Namespace javax.print.attribute


	''' <summary>
	''' Class EnumSyntax is an abstract base class providing the common
	''' implementation of all "type safe enumeration" objects. An enumeration class
	''' (which extends class EnumSyntax) provides a group of enumeration values
	''' (objects) that are singleton instances of the enumeration class; for example:
	''' <PRE>
	'''     public class Bach extends EnumSyntax {
	'''         public static final Bach JOHANN_SEBASTIAN     = new Bach(0);
	'''         public static final Bach WILHELM_FRIEDEMANN   = new Bach(1);
	'''         public static final Bach CARL_PHILIP_EMMANUEL = new Bach(2);
	'''         public static final Bach JOHANN_CHRISTIAN     = new Bach(3);
	'''         public static final Bach P_D_Q                = new Bach(4);
	''' 
	'''         private static final String[] stringTable = {
	'''             "Johann Sebastian Bach",
	'''              "Wilhelm Friedemann Bach",
	'''              "Carl Philip Emmanuel Bach",
	'''              "Johann Christian Bach",
	'''              "P.D.Q. Bach"
	'''         };
	''' 
	'''         protected String[] getStringTable() {
	'''             return stringTable;
	'''         }
	''' 
	'''         private static final Bach[] enumValueTable = {
	'''             JOHANN_SEBASTIAN,
	'''              WILHELM_FRIEDEMANN,
	'''              CARL_PHILIP_EMMANUEL,
	'''              JOHANN_CHRISTIAN,
	'''              P_D_Q
	'''         };
	''' 
	'''         protected EnumSyntax[] getEnumValueTable() {
	'''             return enumValueTable;
	'''         }
	'''     }
	''' </PRE>
	''' You can then write code that uses the <CODE>==</CODE> and <CODE>!=</CODE>
	''' operators to test enumeration values; for example:
	''' <PRE>
	'''     Bach theComposer;
	'''     . . .
	'''     if (theComposer == Bach.JOHANN_SEBASTIAN) {
	'''         System.out.println ("The greatest composer of all time!");
	'''     }
	''' </PRE>
	''' The <CODE>equals()</CODE> method for an enumeration class just does a test
	''' for identical objects (<CODE>==</CODE>).
	''' <P>
	''' You can convert an enumeration value to a string by calling {@link
	''' #toString() toString()}. The string is obtained from a table
	''' supplied by the enumeration class.
	''' <P>
	''' Under the hood, an enumeration value is just an integer, a different integer
	''' for each enumeration value within an enumeration class. You can get an
	''' enumeration value's integer value by calling {@link #getValue()
	''' getValue()}. An enumeration value's integer value is established
	''' when it is constructed (see {@link #EnumSyntax(int)
	''' EnumSyntax(int)}). Since the constructor is protected, the only
	''' possible enumeration values are the singleton objects declared in the
	''' enumeration class; additional enumeration values cannot be created at run
	''' time.
	''' <P>
	''' You can define a subclass of an enumeration class that extends it with
	''' additional enumeration values. The subclass's enumeration values' integer
	''' values need not be distinct from the superclass's enumeration values' integer
	''' values; the <CODE>==</CODE>, <CODE>!=</CODE>, <CODE>equals()</CODE>, and
	''' <CODE>toString()</CODE> methods will still work properly even if the subclass
	''' uses some of the same integer values as the superclass. However, the
	''' application in which the enumeration class and subclass are used may need to
	''' have distinct integer values in the superclass and subclass.
	''' <P>
	''' 
	''' @author  David Mendenhall
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public MustInherit Class EnumSyntax
		Implements ICloneable

		Private Const serialVersionUID As Long = -2739521845085831642L

		''' <summary>
		''' This enumeration value's integer value.
		''' @serial
		''' </summary>
		Private value As Integer

		''' <summary>
		''' Construct a new enumeration value with the given integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			Me.value = value
		End Sub

		''' <summary>
		''' Returns this enumeration value's integer value. </summary>
		''' <returns> the value </returns>
		Public Overridable Property value As Integer
			Get
				Return value
			End Get
		End Property

		''' <summary>
		''' Returns a clone of this enumeration value, which to preserve the
		''' semantics of enumeration values is the same object as this enumeration
		''' value.
		''' </summary>
		Public Overridable Function clone() As Object
			Return Me
		End Function

		''' <summary>
		''' Returns a hash code value for this enumeration value. The hash code is
		''' just this enumeration value's integer value.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return value
		End Function

		''' <summary>
		''' Returns a string value corresponding to this enumeration value.
		''' </summary>
		Public Overrides Function ToString() As String

			Dim theTable As String() = stringTable
			Dim theIndex As Integer = value - offset
			Return If(theTable IsNot Nothing AndAlso theIndex >= 0 AndAlso theIndex < theTable.Length, theTable(theIndex), Convert.ToString(value))
		End Function

		''' <summary>
		''' During object input, convert this deserialized enumeration instance to
		''' the proper enumeration value defined in the enumeration attribute class.
		''' </summary>
		''' <returns>  The enumeration singleton value stored at index
		'''          <I>i</I>-<I>L</I> in the enumeration value table returned by
		'''          <seealso cref="#getEnumValueTable() getEnumValueTable()"/>,
		'''          where <I>i</I> is this enumeration value's integer value and
		'''          <I>L</I> is the value returned by {@link #getOffset()
		'''          getOffset()}.
		''' </returns>
		''' <exception cref="ObjectStreamException"> if the stream can't be deserialised </exception>
		''' <exception cref="InvalidObjectException">
		'''     Thrown if the enumeration value table is null, this enumeration
		'''     value's integer value does not correspond to an element in the
		'''     enumeration value table, or the corresponding element in the
		'''     enumeration value table is null. (Note: {@link
		'''     java.io.InvalidObjectException InvalidObjectException} is a subclass
		'''     of <seealso cref="java.io.ObjectStreamException ObjectStreamException"/>, which
		'''     <CODE>readResolve()</CODE> is declared to throw.) </exception>
		Protected Friend Overridable Function readResolve() As Object

			Dim theTable As EnumSyntax() = enumValueTable

			If theTable Is Nothing Then Throw New java.io.InvalidObjectException("Null enumeration value table for class " & Me.GetType())

			Dim theOffset As Integer = offset
			Dim theIndex As Integer = value - theOffset

			If 0 > theIndex OrElse theIndex >= theTable.Length Then Throw New java.io.InvalidObjectException("Integer value = " & value & " not in valid range " & theOffset & ".." & (theOffset + theTable.Length - 1) & "for class " & Me.GetType())

			Dim result As EnumSyntax = theTable(theIndex)
			If result Is Nothing Then Throw New java.io.InvalidObjectException("No enumeration value for integer value = " & value & "for class " & Me.GetType())
			Return result
		End Function

		' Hidden operations to be implemented in a subclass.

		''' <summary>
		''' Returns the string table for this enumeration value's enumeration class.
		''' The enumeration class's integer values are assumed to lie in the range
		''' <I>L</I>..<I>L</I>+<I>N</I>-1, where <I>L</I> is the value returned by
		''' <seealso cref="#getOffset() getOffset()"/> and <I>N</I> is the length
		''' of the string table. The element in the string table at index
		''' <I>i</I>-<I>L</I> is the value returned by {@link #toString()
		''' toString()} for the enumeration value whose integer value
		''' is <I>i</I>. If an integer within the above range is not used by any
		''' enumeration value, leave the corresponding table element null.
		''' <P>
		''' The default implementation returns null. If the enumeration class (a
		''' subclass of class EnumSyntax) does not override this method to return a
		''' non-null string table, and the subclass does not override the {@link
		''' #toString() toString()} method, the base class {@link
		''' #toString() toString()} method will return just a string
		''' representation of this enumeration value's integer value. </summary>
		''' <returns> the string table </returns>
		Protected Friend Overridable Property stringTable As String()
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for this enumeration value's
		''' enumeration class. The enumeration class's integer values are assumed to
		''' lie in the range <I>L</I>..<I>L</I>+<I>N</I>-1, where <I>L</I> is the
		''' value returned by <seealso cref="#getOffset() getOffset()"/> and
		''' <I>N</I> is the length of the enumeration value table. The element in the
		''' enumeration value table at index <I>i</I>-<I>L</I> is the enumeration
		''' value object whose integer value is <I>i</I>; the {@link #readResolve()
		''' readResolve()} method needs this to preserve singleton
		''' semantics during deserialization of an enumeration instance. If an
		''' integer within the above range is not used by any enumeration value,
		''' leave the corresponding table element null.
		''' <P>
		''' The default implementation returns null. If the enumeration class (a
		''' subclass of class EnumSyntax) does not override this method to return
		''' a non-null enumeration value table, and the subclass does not override
		''' the <seealso cref="#readResolve() readResolve()"/> method, the base
		''' class <seealso cref="#readResolve() readResolve()"/> method will throw
		''' an exception whenever an enumeration instance is deserialized from an
		''' object input stream. </summary>
		''' <returns> the value table </returns>
		Protected Friend Overridable Property enumValueTable As EnumSyntax()
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the lowest integer value used by this enumeration value's
		''' enumeration class.
		''' <P>
		''' The default implementation returns 0. If the enumeration class (a
		''' subclass of class EnumSyntax) uses integer values starting at other than
		''' 0, override this method in the subclass. </summary>
		''' <returns> the offset of the lowest enumeration value. </returns>
		Protected Friend Overridable Property offset As Integer
			Get
				Return 0
			End Get
		End Property

	End Class

End Namespace
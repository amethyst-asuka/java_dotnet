Imports System
Imports System.Collections.Generic

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
Namespace javax.swing


	''' <summary>
	''' <code>RowFilter</code> is used to filter out entries from the
	''' model so that they are not shown in the view.  For example, a
	''' <code>RowFilter</code> associated with a <code>JTable</code> might
	''' only allow rows that contain a column with a specific string. The
	''' meaning of <em>entry</em> depends on the component type.
	''' For example, when a filter is
	''' associated with a <code>JTable</code>, an entry corresponds to a
	''' row; when associated with a <code>JTree</code>, an entry corresponds
	''' to a node.
	''' <p>
	''' Subclasses must override the <code>include</code> method to
	''' indicate whether the entry should be shown in the
	''' view.  The <code>Entry</code> argument can be used to obtain the values in
	''' each of the columns in that entry.  The following example shows an
	''' <code>include</code> method that allows only entries containing one or
	''' more values starting with the string "a":
	''' <pre>
	''' RowFilter&lt;Object,Object&gt; startsWithAFilter = new RowFilter&lt;Object,Object&gt;() {
	'''   public boolean include(Entry&lt;? extends Object, ? extends Object&gt; entry) {
	'''     for (int i = entry.getValueCount() - 1; i &gt;= 0; i--) {
	'''       if (entry.getStringValue(i).startsWith("a")) {
	'''         // The value starts with "a", include it
	'''         return true;
	'''       }
	'''     }
	'''     // None of the columns start with "a"; return false so that this
	'''     // entry is not shown
	'''     return false;
	'''   }
	''' };
	''' </pre>
	''' <code>RowFilter</code> has two formal type parameters that allow
	''' you to create a <code>RowFilter</code> for a specific model. For
	''' example, the following assumes a specific model that is wrapping
	''' objects of type <code>Person</code>.  Only <code>Person</code>s
	''' with an age over 20 will be shown:
	''' <pre>
	''' RowFilter&lt;PersonModel,Integer&gt; ageFilter = new RowFilter&lt;PersonModel,Integer&gt;() {
	'''   public boolean include(Entry&lt;? extends PersonModel, ? extends Integer&gt; entry) {
	'''     PersonModel personModel = entry.getModel();
	'''     Person person = personModel.getPerson(entry.getIdentifier());
	'''     if (person.getAge() &gt; 20) {
	'''       // Returning true indicates this row should be shown.
	'''       return true;
	'''     }
	'''     // Age is &lt;= 20, don't show it.
	'''     return false;
	'''   }
	''' };
	''' PersonModel model = createPersonModel();
	''' TableRowSorter&lt;PersonModel&gt; sorter = new TableRowSorter&lt;PersonModel&gt;(model);
	''' sorter.setRowFilter(ageFilter);
	''' </pre>
	''' </summary>
	''' @param <M> the type of the model; for example <code>PersonModel</code> </param>
	''' @param <I> the type of the identifier; when using
	'''            <code>TableRowSorter</code> this will be <code>Integer</code> </param>
	''' <seealso cref= javax.swing.table.TableRowSorter
	''' @since 1.6 </seealso>
	Public MustInherit Class RowFilter(Of M, I)
		''' <summary>
		''' Enumeration of the possible comparison values supported by
		''' some of the default <code>RowFilter</code>s.
		''' </summary>
		''' <seealso cref= RowFilter
		''' @since 1.6 </seealso>
		Public Enum ComparisonType
			''' <summary>
			''' Indicates that entries with a value before the supplied
			''' value should be included.
			''' </summary>
			BEFORE

			''' <summary>
			''' Indicates that entries with a value after the supplied
			''' value should be included.
			''' </summary>
			AFTER

			''' <summary>
			''' Indicates that entries with a value equal to the supplied
			''' value should be included.
			''' </summary>
			EQUAL

			''' <summary>
			''' Indicates that entries with a value not equal to the supplied
			''' value should be included.
			''' </summary>
			NOT_EQUAL
		End Enum

		''' <summary>
		''' Throws an IllegalArgumentException if any of the values in
		''' columns are {@literal <} 0.
		''' </summary>
		Private Shared Sub checkIndices(ByVal columns As Integer())
			For i As Integer = columns.Length - 1 To 0 Step -1
				If columns(i) < 0 Then Throw New System.ArgumentException("Index must be >= 0")
			Next i
		End Sub

		''' <summary>
		''' Returns a <code>RowFilter</code> that uses a regular
		''' expression to determine which entries to include.  Only entries
		''' with at least one matching value are included.  For
		''' example, the following creates a <code>RowFilter</code> that
		''' includes entries with at least one value starting with
		''' "a":
		''' <pre>
		'''   RowFilter.regexFilter("^a");
		''' </pre>
		''' <p>
		''' The returned filter uses <seealso cref="java.util.regex.Matcher#find"/>
		''' to test for inclusion.  To test for exact matches use the
		''' characters '^' and '$' to match the beginning and end of the
		''' string respectively.  For example, "^foo$" includes only rows whose
		''' string is exactly "foo" and not, for example, "food".  See
		''' <seealso cref="java.util.regex.Pattern"/> for a complete description of
		''' the supported regular-expression constructs.
		''' </summary>
		''' <param name="regex"> the regular expression to filter on </param>
		''' <param name="indices"> the indices of the values to check.  If not supplied all
		'''               values are evaluated </param>
		''' <returns> a <code>RowFilter</code> implementing the specified criteria </returns>
		''' <exception cref="NullPointerException"> if <code>regex</code> is
		'''         <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if any of the <code>indices</code>
		'''         are &lt; 0 </exception>
		''' <exception cref="PatternSyntaxException"> if <code>regex</code> is
		'''         not a valid regular expression. </exception>
		''' <seealso cref= java.util.regex.Pattern </seealso>
		Public Shared Function regexFilter(Of M, I)(ByVal regex As String, ParamArray ByVal indices As Integer()) As RowFilter(Of M, I)
			Return CType(New RegexFilter(java.util.regex.Pattern.compile(regex), indices), RowFilter(Of M, I))
		End Function

		''' <summary>
		''' Returns a <code>RowFilter</code> that includes entries that
		''' have at least one <code>Date</code> value meeting the specified
		''' criteria.  For example, the following <code>RowFilter</code> includes
		''' only entries with at least one date value after the current date:
		''' <pre>
		'''   RowFilter.dateFilter(ComparisonType.AFTER, new Date());
		''' </pre>
		''' </summary>
		''' <param name="type"> the type of comparison to perform </param>
		''' <param name="date"> the date to compare against </param>
		''' <param name="indices"> the indices of the values to check.  If not supplied all
		'''               values are evaluated </param>
		''' <returns> a <code>RowFilter</code> implementing the specified criteria </returns>
		''' <exception cref="NullPointerException"> if <code>date</code> is
		'''          <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if any of the <code>indices</code>
		'''         are &lt; 0 or <code>type</code> is
		'''         <code>null</code> </exception>
		''' <seealso cref= java.util.Calendar </seealso>
		''' <seealso cref= java.util.Date </seealso>
		Public Shared Function dateFilter(Of M, I)(ByVal type As ComparisonType, ByVal [date] As DateTime, ParamArray ByVal indices As Integer()) As RowFilter(Of M, I)
			Return CType(New DateFilter(type, [date], indices), RowFilter(Of M, I))
		End Function

		''' <summary>
		''' Returns a <code>RowFilter</code> that includes entries that
		''' have at least one <code>Number</code> value meeting the
		''' specified criteria.  For example, the following
		''' filter will only include entries with at
		''' least one number value equal to 10:
		''' <pre>
		'''   RowFilter.numberFilter(ComparisonType.EQUAL, 10);
		''' </pre>
		''' </summary>
		''' <param name="type"> the type of comparison to perform </param>
		''' <param name="indices"> the indices of the values to check.  If not supplied all
		'''               values are evaluated </param>
		''' <returns> a <code>RowFilter</code> implementing the specified criteria </returns>
		''' <exception cref="IllegalArgumentException"> if any of the <code>indices</code>
		'''         are &lt; 0, <code>type</code> is <code>null</code>
		'''         or <code>number</code> is <code>null</code> </exception>
		Public Shared Function numberFilter(Of M, I)(ByVal type As ComparisonType, ByVal number As Number, ParamArray ByVal indices As Integer()) As RowFilter(Of M, I)
			Return CType(New NumberFilter(type, number, indices), RowFilter(Of M, I))
		End Function

		''' <summary>
		''' Returns a <code>RowFilter</code> that includes entries if any
		''' of the supplied filters includes the entry.
		''' <p>
		''' The following example creates a <code>RowFilter</code> that will
		''' include any entries containing the string "foo" or the string
		''' "bar":
		''' <pre>
		'''   List&lt;RowFilter&lt;Object,Object&gt;&gt; filters = new ArrayList&lt;RowFilter&lt;Object,Object&gt;&gt;(2);
		'''   filters.add(RowFilter.regexFilter("foo"));
		'''   filters.add(RowFilter.regexFilter("bar"));
		'''   RowFilter&lt;Object,Object&gt; fooBarFilter = RowFilter.orFilter(filters);
		''' </pre>
		''' </summary>
		''' <param name="filters"> the <code>RowFilter</code>s to test </param>
		''' <exception cref="IllegalArgumentException"> if any of the filters
		'''         are <code>null</code> </exception>
		''' <exception cref="NullPointerException"> if <code>filters</code> is null </exception>
		''' <returns> a <code>RowFilter</code> implementing the specified criteria </returns>
		''' <seealso cref= java.util.Arrays#asList </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function orFilter(Of M, I, T1 As RowFilter(Of ?, ?)(ByVal filters As IEnumerable(Of T1)) As RowFilter(Of M, I)
			Return New OrFilter(Of M, I)(filters)
		End Function

		''' <summary>
		''' Returns a <code>RowFilter</code> that includes entries if all
		''' of the supplied filters include the entry.
		''' <p>
		''' The following example creates a <code>RowFilter</code> that will
		''' include any entries containing the string "foo" and the string
		''' "bar":
		''' <pre>
		'''   List&lt;RowFilter&lt;Object,Object&gt;&gt; filters = new ArrayList&lt;RowFilter&lt;Object,Object&gt;&gt;(2);
		'''   filters.add(RowFilter.regexFilter("foo"));
		'''   filters.add(RowFilter.regexFilter("bar"));
		'''   RowFilter&lt;Object,Object&gt; fooBarFilter = RowFilter.andFilter(filters);
		''' </pre>
		''' </summary>
		''' <param name="filters"> the <code>RowFilter</code>s to test </param>
		''' <returns> a <code>RowFilter</code> implementing the specified criteria </returns>
		''' <exception cref="IllegalArgumentException"> if any of the filters
		'''         are <code>null</code> </exception>
		''' <exception cref="NullPointerException"> if <code>filters</code> is null </exception>
		''' <seealso cref= java.util.Arrays#asList </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function andFilter(Of M, I, T1 As RowFilter(Of ?, ?)(ByVal filters As IEnumerable(Of T1)) As RowFilter(Of M, I)
			Return New AndFilter(Of M, I)(filters)
		End Function

		''' <summary>
		''' Returns a <code>RowFilter</code> that includes entries if the
		''' supplied filter does not include the entry.
		''' </summary>
		''' <param name="filter"> the <code>RowFilter</code> to negate </param>
		''' <returns> a <code>RowFilter</code> implementing the specified criteria </returns>
		''' <exception cref="IllegalArgumentException"> if <code>filter</code> is
		'''         <code>null</code> </exception>
		Public Shared Function notFilter(Of M, I)(ByVal filter As RowFilter(Of M, I)) As RowFilter(Of M, I)
			Return New NotFilter(Of M, I)(filter)
		End Function

		''' <summary>
		''' Returns true if the specified entry should be shown;
		''' returns false if the entry should be hidden.
		''' <p>
		''' The <code>entry</code> argument is valid only for the duration of
		''' the invocation.  Using <code>entry</code> after the call returns
		''' results in undefined behavior.
		''' </summary>
		''' <param name="entry"> a non-<code>null</code> object that wraps the underlying
		'''              object from the model </param>
		''' <returns> true if the entry should be shown </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public MustOverride Function include(Of T1 As M, ? As I)(ByVal entry As Entry(Of T1)) As Boolean

		'
		' WARNING:
		' Because of the method signature of dateFilter/numberFilter/regexFilter
		' we can NEVER add a method to RowFilter that returns M,I. If we were
		' to do so it would be possible to get a ClassCastException during normal
		' usage.
		'

		''' <summary>
		''' An <code>Entry</code> object is passed to instances of
		''' <code>RowFilter</code>, allowing the filter to get the value of the
		''' entry's data, and thus to determine whether the entry should be shown.
		''' An <code>Entry</code> object contains information about the model
		''' as well as methods for getting the underlying values from the model.
		''' </summary>
		''' @param <M> the type of the model; for example <code>PersonModel</code> </param>
		''' @param <I> the type of the identifier; when using
		'''            <code>TableRowSorter</code> this will be <code>Integer</code> </param>
		''' <seealso cref= javax.swing.RowFilter </seealso>
		''' <seealso cref= javax.swing.DefaultRowSorter#setRowFilter(javax.swing.RowFilter)
		''' @since 1.6 </seealso>
		Public MustInherit Class Entry(Of M, I)
			''' <summary>
			''' Creates an <code>Entry</code>.
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Returns the underlying model.
			''' </summary>
			''' <returns> the model containing the data that this entry represents </returns>
			Public MustOverride ReadOnly Property model As M

			''' <summary>
			''' Returns the number of values in the entry.  For
			''' example, when used with a table this corresponds to the
			''' number of columns.
			''' </summary>
			''' <returns> number of values in the object being filtered </returns>
			Public MustOverride ReadOnly Property valueCount As Integer

			''' <summary>
			''' Returns the value at the specified index.  This may return
			''' <code>null</code>.  When used with a table, index
			''' corresponds to the column number in the model.
			''' </summary>
			''' <param name="index"> the index of the value to get </param>
			''' <returns> value at the specified index </returns>
			''' <exception cref="IndexOutOfBoundsException"> if index &lt; 0 or
			'''         &gt;= getValueCount </exception>
			Public MustOverride Function getValue(ByVal index As Integer) As Object

			''' <summary>
			''' Returns the string value at the specified index.  If
			''' filtering is being done based on <code>String</code> values
			''' this method is preferred to that of <code>getValue</code>
			''' as <code>getValue(index).toString()</code> may return a
			''' different result than <code>getStringValue(index)</code>.
			''' <p>
			''' This implementation calls <code>getValue(index).toString()</code>
			''' after checking for <code>null</code>.  Subclasses that provide
			''' different string conversion should override this method if
			''' necessary.
			''' </summary>
			''' <param name="index"> the index of the value to get </param>
			''' <returns> {@code non-null} string at the specified index </returns>
			''' <exception cref="IndexOutOfBoundsException"> if index &lt; 0 ||
			'''         &gt;= getValueCount </exception>
			Public Overridable Function getStringValue(ByVal index As Integer) As String
				Dim ___value As Object = getValue(index)
				Return If(___value Is Nothing, "", ___value.ToString())
			End Function

			''' <summary>
			''' Returns the identifer (in the model) of the entry.
			''' For a table this corresponds to the index of the row in the model,
			''' expressed as an <code>Integer</code>.
			''' </summary>
			''' <returns> a model-based (not view-based) identifier for
			'''         this entry </returns>
			Public MustOverride ReadOnly Property identifier As I
		End Class


		Private MustInherit Class GeneralFilter
			Inherits RowFilter(Of Object, Object)

			Private columns As Integer()

			Friend Sub New(ByVal columns As Integer())
				checkIndices(columns)
				Me.columns = columns
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function include(Of T1 As Object, ? As Object)(ByVal value As Entry(Of T1)) As Boolean
				Dim count As Integer = value.valueCount
				If columns.Length > 0 Then
					For i As Integer = columns.Length - 1 To 0 Step -1
						Dim index As Integer = columns(i)
						If index < count Then
							If include(value, index) Then Return True
						End If
					Next i
				Else
					count -= 1
					Do While count >= 0
						If include(value, count) Then Return True
						count -= 1
					Loop
				End If
				Return False
			End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Protected Friend MustOverride Function include(Of T1 As Object, ? As Object)(ByVal value As Entry(Of T1), ByVal index As Integer) As Boolean
		End Class


		Private Class RegexFilter
			Inherits GeneralFilter

			Private matcher As java.util.regex.Matcher

			Friend Sub New(ByVal regex As java.util.regex.Pattern, ByVal columns As Integer())
				MyBase.New(columns)
				If regex Is Nothing Then Throw New System.ArgumentException("Pattern must be non-null")
				matcher = regex.matcher("")
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Protected Friend Overrides Function include(Of T1 As Object, ? As Object)(ByVal value As Entry(Of T1), ByVal index As Integer) As Boolean
				matcher.reset(value.getStringValue(index))
				Return matcher.find()
			End Function
		End Class


		Private Class DateFilter
			Inherits GeneralFilter

			Private [date] As Long
			Private type As ComparisonType

			Friend Sub New(ByVal type As ComparisonType, ByVal [date] As Long, ByVal columns As Integer())
				MyBase.New(columns)
				If type Is Nothing Then Throw New System.ArgumentException("type must be non-null")
				Me.type = type
				Me.date = [date]
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Protected Friend Overrides Function include(Of T1 As Object, ? As Object)(ByVal value As Entry(Of T1), ByVal index As Integer) As Boolean
				Dim v As Object = value.getValue(index)

				If TypeOf v Is DateTime Then
					Dim vDate As Long = CDate(v).time
					Select Case type
					Case ComparisonType.BEFORE
						Return (vDate < [date])
					Case ComparisonType.AFTER
						Return (vDate > [date])
					Case ComparisonType.EQUAL
						Return (vDate = [date])
					Case ComparisonType.NOT_EQUAL
						Return (vDate <> [date])
					Case Else
					End Select
				End If
				Return False
			End Function
		End Class




		Private Class NumberFilter
			Inherits GeneralFilter

			Private isComparable As Boolean
			Private number As Number
			Private type As ComparisonType

			Friend Sub New(ByVal type As ComparisonType, ByVal number As Number, ByVal columns As Integer())
				MyBase.New(columns)
				If type Is Nothing OrElse number Is Nothing Then Throw New System.ArgumentException("type and number must be non-null")
				Me.type = type
				Me.number = number
				isComparable = (TypeOf number Is IComparable)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Protected Friend Overrides Function include(Of T1 As Object, ? As Object)(ByVal value As Entry(Of T1), ByVal index As Integer) As Boolean
				Dim v As Object = value.getValue(index)

				If TypeOf v Is Number Then
					Dim compared As Boolean = True
					Dim compareResult As Integer
					Dim vClass As Type = v.GetType()
					If number.GetType() Is vClass AndAlso isComparable Then
						compareResult = CType(number, IComparable).CompareTo(v)
					Else
						compareResult = longCompare(CType(v, Number))
					End If
					Select Case type
					Case ComparisonType.BEFORE
						Return (compareResult > 0)
					Case ComparisonType.AFTER
						Return (compareResult < 0)
					Case ComparisonType.EQUAL
						Return (compareResult = 0)
					Case ComparisonType.NOT_EQUAL
						Return (compareResult <> 0)
					Case Else
					End Select
				End If
				Return False
			End Function

			Private Function longCompare(ByVal o As Number) As Integer
				Dim diff As Long = number - o

				If diff < 0 Then
					Return -1
				ElseIf diff > 0 Then
					Return 1
				End If
				Return 0
			End Function
		End Class


		Private Class OrFilter(Of M, I)
			Inherits RowFilter(Of M, I)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend filters As IList(Of RowFilter(Of ?, ?))

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Sub New(Of T1 As RowFilter(Of ?, ?)(ByVal filters As IEnumerable(Of T1))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Me.filters = New List(Of RowFilter(Of ?, ?))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				For Each filter As RowFilter(Of ?, ?) In filters
					If filter Is Nothing Then Throw New System.ArgumentException("Filter must be non-null")
					Me.filters.Add(filter)
				Next filter
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function include(Of T1 As M, ? As I)(ByVal value As Entry(Of T1)) As Boolean
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				For Each filter As RowFilter(Of ?, ?) In filters
					If filter.include(value) Then Return True
				Next filter
				Return False
			End Function
		End Class


		Private Class AndFilter(Of M, I)
			Inherits OrFilter(Of M, I)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Sub New(Of T1 As RowFilter(Of ?, ?)(ByVal filters As IEnumerable(Of T1))
				MyBase.New(filters)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function include(Of T1 As M, ? As I)(ByVal value As Entry(Of T1)) As Boolean
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				For Each filter As RowFilter(Of ?, ?) In filters
					If Not filter.include(value) Then Return False
				Next filter
				Return True
			End Function
		End Class


		Private Class NotFilter(Of M, I)
			Inherits RowFilter(Of M, I)

			Private filter As RowFilter(Of M, I)

			Friend Sub New(ByVal filter As RowFilter(Of M, I))
				If filter Is Nothing Then Throw New System.ArgumentException("filter must be non-null")
				Me.filter = filter
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function include(Of T1 As M, ? As I)(ByVal value As Entry(Of T1)) As Boolean
				Return Not filter.include(value)
			End Function
		End Class
	End Class

End Namespace
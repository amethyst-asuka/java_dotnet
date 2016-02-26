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

Namespace javax.swing



	''' <summary>
	''' A <code>SpinnerModel</code> for sequences of numbers.
	''' The upper and lower bounds of the sequence are defined
	''' by properties called <code>minimum</code> and
	''' <code>maximum</code>. The size of the increase or decrease
	''' computed by the <code>nextValue</code> and
	''' <code>previousValue</code> methods is defined by a property called
	''' <code>stepSize</code>.  The <code>minimum</code> and
	''' <code>maximum</code> properties can be <code>null</code>
	''' to indicate that the sequence has no lower or upper limit.
	''' All of the properties in this class are defined in terms of two
	''' generic types: <code>Number</code> and
	''' <code>Comparable</code>, so that all Java numeric types
	''' may be accommodated.  Internally, there's only support for
	''' values whose type is one of the primitive <code>Number</code> types:
	''' <code>Double</code>, <code>Float</code>, <code>Long</code>,
	''' <code>Integer</code>, <code>Short</code>, or <code>Byte</code>.
	''' <p>
	''' To create a <code>SpinnerNumberModel</code> for the integer
	''' range zero to one hundred, with
	''' fifty as the initial value, one could write:
	''' <pre>
	''' Integer value = new Integer(50);
	''' Integer min = new Integer(0);
	''' Integer max = new Integer(100);
	''' Integer step = new Integer(1);
	''' SpinnerNumberModel model = new SpinnerNumberModel(value, min, max, step);
	''' int fifty = model.getNumber().intValue();
	''' </pre>
	''' <p>
	''' Spinners for integers and doubles are common, so special constructors
	''' for these cases are provided.  For example to create the model in
	''' the previous example, one could also write:
	''' <pre>
	''' SpinnerNumberModel model = new SpinnerNumberModel(50, 0, 100, 1);
	''' </pre>
	''' <p>
	''' This model inherits a <code>ChangeListener</code>.
	''' The <code>ChangeListeners</code> are notified
	''' whenever the model's <code>value</code>, <code>stepSize</code>,
	''' <code>minimum</code>, or <code>maximum</code> properties changes.
	''' </summary>
	''' <seealso cref= JSpinner </seealso>
	''' <seealso cref= SpinnerModel </seealso>
	''' <seealso cref= AbstractSpinnerModel </seealso>
	''' <seealso cref= SpinnerListModel </seealso>
	''' <seealso cref= SpinnerDateModel
	''' 
	''' @author Hans Muller
	''' @since 1.4 </seealso>
	<Serializable> _
	Public Class SpinnerNumberModel
		Inherits AbstractSpinnerModel

		Private stepSize, value As Number
		Private minimum, maximum As IComparable


		''' <summary>
		''' Constructs a <code>SpinnerModel</code> that represents
		''' a closed sequence of
		''' numbers from <code>minimum</code> to <code>maximum</code>.  The
		''' <code>nextValue</code> and <code>previousValue</code> methods
		''' compute elements of the sequence by adding or subtracting
		''' <code>stepSize</code> respectively.  All of the parameters
		''' must be mutually <code>Comparable</code>, <code>value</code>
		''' and <code>stepSize</code> must be instances of <code>Integer</code>
		''' <code>Long</code>, <code>Float</code>, or <code>Double</code>.
		''' <p>
		''' The <code>minimum</code> and <code>maximum</code> parameters
		''' can be <code>null</code> to indicate that the range doesn't
		''' have an upper or lower bound.
		''' If <code>value</code> or <code>stepSize</code> is <code>null</code>,
		''' or if both <code>minimum</code> and <code>maximum</code>
		''' are specified and <code>minimum &gt; maximum</code> then an
		''' <code>IllegalArgumentException</code> is thrown.
		''' Similarly if <code>(minimum &lt;= value &lt;= maximum</code>) is false,
		''' an <code>IllegalArgumentException</code> is thrown.
		''' </summary>
		''' <param name="value"> the current (non <code>null</code>) value of the model </param>
		''' <param name="minimum"> the first number in the sequence or <code>null</code> </param>
		''' <param name="maximum"> the last number in the sequence or <code>null</code> </param>
		''' <param name="stepSize"> the difference between elements of the sequence
		''' </param>
		''' <exception cref="IllegalArgumentException"> if stepSize or value is
		'''     <code>null</code> or if the following expression is false:
		'''     <code>minimum &lt;= value &lt;= maximum</code> </exception>
		Public Sub New(ByVal value As Number, ByVal minimum As IComparable, ByVal maximum As IComparable, ByVal stepSize As Number)
			If (value Is Nothing) OrElse (stepSize Is Nothing) Then Throw New System.ArgumentException("value and stepSize must be non-null")
			If Not(((minimum Is Nothing) OrElse (minimum.CompareTo(value) <= 0)) AndAlso ((maximum Is Nothing) OrElse (maximum.CompareTo(value) >= 0))) Then Throw New System.ArgumentException("(minimum <= value <= maximum) is false")
			Me.value = value
			Me.minimum = minimum
			Me.maximum = maximum
			Me.stepSize = stepSize
		End Sub


		''' <summary>
		''' Constructs a <code>SpinnerNumberModel</code> with the specified
		''' <code>value</code>, <code>minimum</code>/<code>maximum</code> bounds,
		''' and <code>stepSize</code>.
		''' </summary>
		''' <param name="value"> the current value of the model </param>
		''' <param name="minimum"> the first number in the sequence </param>
		''' <param name="maximum"> the last number in the sequence </param>
		''' <param name="stepSize"> the difference between elements of the sequence </param>
		''' <exception cref="IllegalArgumentException"> if the following expression is false:
		'''     <code>minimum &lt;= value &lt;= maximum</code> </exception>
		Public Sub New(ByVal value As Integer, ByVal minimum As Integer, ByVal maximum As Integer, ByVal stepSize As Integer)
			Me.New(Convert.ToInt32(value), Convert.ToInt32(minimum), Convert.ToInt32(maximum), Convert.ToInt32(stepSize))
		End Sub


		''' <summary>
		''' Constructs a <code>SpinnerNumberModel</code> with the specified
		''' <code>value</code>, <code>minimum</code>/<code>maximum</code> bounds,
		''' and <code>stepSize</code>.
		''' </summary>
		''' <param name="value"> the current value of the model </param>
		''' <param name="minimum"> the first number in the sequence </param>
		''' <param name="maximum"> the last number in the sequence </param>
		''' <param name="stepSize"> the difference between elements of the sequence </param>
		''' <exception cref="IllegalArgumentException">   if the following expression is false:
		'''     <code>minimum &lt;= value &lt;= maximum</code> </exception>
		Public Sub New(ByVal value As Double, ByVal minimum As Double, ByVal maximum As Double, ByVal stepSize As Double)
			Me.New(New Double?(value), New Double?(minimum), New Double?(maximum), New Double?(stepSize))
		End Sub


		''' <summary>
		''' Constructs a <code>SpinnerNumberModel</code> with no
		''' <code>minimum</code> or <code>maximum</code> value,
		''' <code>stepSize</code> equal to one, and an initial value of zero.
		''' </summary>
		Public Sub New()
			Me.New(Convert.ToInt32(0), Nothing, Nothing, Convert.ToInt32(1))
		End Sub


		''' <summary>
		''' Changes the lower bound for numbers in this sequence.
		''' If <code>minimum</code> is <code>null</code>,
		''' then there is no lower bound.  No bounds checking is done here;
		''' the new <code>minimum</code> value may invalidate the
		''' <code>(minimum &lt;= value &lt;= maximum)</code>
		''' invariant enforced by the constructors.  This is to simplify updating
		''' the model, naturally one should ensure that the invariant is true
		''' before calling the <code>getNextValue</code>,
		''' <code>getPreviousValue</code>, or <code>setValue</code> methods.
		''' <p>
		''' Typically this property is a <code>Number</code> of the same type
		''' as the <code>value</code> however it's possible to use any
		''' <code>Comparable</code> with a <code>compareTo</code>
		''' method for a <code>Number</code> with the same type as the value.
		''' For example if value was a <code>Long</code>,
		''' <code>minimum</code> might be a Date subclass defined like this:
		''' <pre>
		''' MyDate extends Date {  // Date already implements Comparable
		'''     public int compareTo(Long o) {
		'''         long t = getTime();
		'''         return (t &lt; o.longValue() ? -1 : (t == o.longValue() ? 0 : 1));
		'''     }
		''' }
		''' </pre>
		''' <p>
		''' This method fires a <code>ChangeEvent</code>
		''' if the <code>minimum</code> has changed.
		''' </summary>
		''' <param name="minimum"> a <code>Comparable</code> that has a
		'''     <code>compareTo</code> method for <code>Number</code>s with
		'''     the same type as <code>value</code> </param>
		''' <seealso cref= #getMinimum </seealso>
		''' <seealso cref= #setMaximum </seealso>
		''' <seealso cref= SpinnerModel#addChangeListener </seealso>
		Public Overridable Property minimum As IComparable
			Set(ByVal minimum As IComparable)
				If If(minimum Is Nothing, (Me.minimum IsNot Nothing), (Not minimum.Equals(Me.minimum))) Then
					Me.minimum = minimum
					fireStateChanged()
				End If
			End Set
			Get
				Return minimum
			End Get
		End Property




		''' <summary>
		''' Changes the upper bound for numbers in this sequence.
		''' If <code>maximum</code> is <code>null</code>, then there
		''' is no upper bound.  No bounds checking is done here; the new
		''' <code>maximum</code> value may invalidate the
		''' <code>(minimum &lt;= value &lt; maximum)</code>
		''' invariant enforced by the constructors.  This is to simplify updating
		''' the model, naturally one should ensure that the invariant is true
		''' before calling the <code>next</code>, <code>previous</code>,
		''' or <code>setValue</code> methods.
		''' <p>
		''' Typically this property is a <code>Number</code> of the same type
		''' as the <code>value</code> however it's possible to use any
		''' <code>Comparable</code> with a <code>compareTo</code>
		''' method for a <code>Number</code> with the same type as the value.
		''' See <a href="#setMinimum(java.lang.Comparable)">
		''' <code>setMinimum</code></a> for an example.
		''' <p>
		''' This method fires a <code>ChangeEvent</code> if the
		''' <code>maximum</code> has changed.
		''' </summary>
		''' <param name="maximum"> a <code>Comparable</code> that has a
		'''     <code>compareTo</code> method for <code>Number</code>s with
		'''     the same type as <code>value</code> </param>
		''' <seealso cref= #getMaximum </seealso>
		''' <seealso cref= #setMinimum </seealso>
		''' <seealso cref= SpinnerModel#addChangeListener </seealso>
		Public Overridable Property maximum As IComparable
			Set(ByVal maximum As IComparable)
				If If(maximum Is Nothing, (Me.maximum IsNot Nothing), (Not maximum.Equals(Me.maximum))) Then
					Me.maximum = maximum
					fireStateChanged()
				End If
			End Set
			Get
				Return maximum
			End Get
		End Property




		''' <summary>
		''' Changes the size of the value change computed by the
		''' <code>getNextValue</code> and <code>getPreviousValue</code>
		''' methods.  An <code>IllegalArgumentException</code>
		''' is thrown if <code>stepSize</code> is <code>null</code>.
		''' <p>
		''' This method fires a <code>ChangeEvent</code> if the
		''' <code>stepSize</code> has changed.
		''' </summary>
		''' <param name="stepSize"> the size of the value change computed by the
		'''     <code>getNextValue</code> and <code>getPreviousValue</code> methods </param>
		''' <seealso cref= #getNextValue </seealso>
		''' <seealso cref= #getPreviousValue </seealso>
		''' <seealso cref= #getStepSize </seealso>
		''' <seealso cref= SpinnerModel#addChangeListener </seealso>
		Public Overridable Property stepSize As Number
			Set(ByVal stepSize As Number)
				If stepSize Is Nothing Then Throw New System.ArgumentException("null stepSize")
				If Not stepSize.Equals(Me.stepSize) Then
					Me.stepSize = stepSize
					fireStateChanged()
				End If
			End Set
			Get
				Return stepSize
			End Get
		End Property




		Private Function incrValue(ByVal dir As Integer) As Number
			Dim newValue As Number
			If (TypeOf value Is Single?) OrElse (TypeOf value Is Double?) Then
				Dim v As Double = value + (stepSize * CDbl(dir))
				If TypeOf value Is Double? Then
					newValue = New Double?(v)
				Else
					newValue = New Single?(v)
				End If
			Else
				Dim v As Long = value + (stepSize * CLng(dir))

				If TypeOf value Is Long? Then
					newValue = Convert.ToInt64(v)
				ElseIf TypeOf value Is Integer? Then
					newValue = Convert.ToInt32(CInt(v))
				ElseIf TypeOf value Is Short? Then
					newValue = Convert.ToInt16(CShort(v))
				Else
					newValue = Convert.ToByte(CByte(v))
				End If
			End If

			If (maximum IsNot Nothing) AndAlso (maximum.CompareTo(newValue) < 0) Then Return Nothing
			If (minimum IsNot Nothing) AndAlso (minimum.CompareTo(newValue) > 0) Then
				Return Nothing
			Else
				Return newValue
			End If
		End Function


		''' <summary>
		''' Returns the next number in the sequence.
		''' </summary>
		''' <returns> <code>value + stepSize</code> or <code>null</code> if the sum
		'''     exceeds <code>maximum</code>.
		''' </returns>
		''' <seealso cref= SpinnerModel#getNextValue </seealso>
		''' <seealso cref= #getPreviousValue </seealso>
		''' <seealso cref= #setStepSize </seealso>
		Public Property Overrides nextValue As Object
			Get
				Return incrValue(+1)
			End Get
		End Property


		''' <summary>
		''' Returns the previous number in the sequence.
		''' </summary>
		''' <returns> <code>value - stepSize</code>, or
		'''     <code>null</code> if the sum is less
		'''     than <code>minimum</code>.
		''' </returns>
		''' <seealso cref= SpinnerModel#getPreviousValue </seealso>
		''' <seealso cref= #getNextValue </seealso>
		''' <seealso cref= #setStepSize </seealso>
		Public Property Overrides previousValue As Object
			Get
				Return incrValue(-1)
			End Get
		End Property


		''' <summary>
		''' Returns the value of the current element of the sequence.
		''' </summary>
		''' <returns> the value property </returns>
		''' <seealso cref= #setValue </seealso>
		Public Overridable Property number As Number
			Get
				Return value
			End Get
		End Property


		''' <summary>
		''' Returns the value of the current element of the sequence.
		''' </summary>
		''' <returns> the value property </returns>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= #getNumber </seealso>
		Public Property Overrides value As Object
			Get
				Return value
			End Get
			Set(ByVal value As Object)
				If (value Is Nothing) OrElse Not(TypeOf value Is Number) Then Throw New System.ArgumentException("illegal value")
				If Not value.Equals(Me.value) Then
					Me.value = CType(value, Number)
					fireStateChanged()
				End If
			End Set
		End Property


	End Class

End Namespace
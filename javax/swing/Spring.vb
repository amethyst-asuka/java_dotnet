Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
	'''  An instance of the <code>Spring</code> class holds three properties that
	'''  characterize its behavior: the <em>minimum</em>, <em>preferred</em>, and
	'''  <em>maximum</em> values. Each of these properties may be involved in
	'''  defining its fourth, <em>value</em>, property based on a series of rules.
	'''  <p>
	'''  An instance of the <code>Spring</code> class can be visualized as a
	'''  mechanical spring that provides a corrective force as the spring is compressed
	'''  or stretched away from its preferred value. This force is modelled
	'''  as linear function of the distance from the preferred value, but with
	'''  two different constants -- one for the compressional force and one for the
	'''  tensional one. Those constants are specified by the minimum and maximum
	'''  values of the spring such that a spring at its minimum value produces an
	'''  equal and opposite force to that which is created when it is at its
	'''  maximum value. The difference between the <em>preferred</em> and
	'''  <em>minimum</em> values, therefore, represents the ease with which the
	'''  spring can be compressed and the difference between its <em>maximum</em>
	'''  and <em>preferred</em> values, indicates the ease with which the
	'''  <code>Spring</code> can be extended.
	'''  See the <seealso cref="#sum"/> method for details.
	''' 
	'''  <p>
	'''  By defining simple arithmetic operations on <code>Spring</code>s,
	'''  the behavior of a collection of <code>Spring</code>s
	'''  can be reduced to that of an ordinary (non-compound) <code>Spring</code>. We define
	'''  the "+", "-", <em>max</em>, and <em>min</em> operators on
	'''  <code>Spring</code>s so that, in each case, the result is a <code>Spring</code>
	'''  whose characteristics bear a useful mathematical relationship to its constituent
	'''  springs.
	''' 
	'''  <p>
	'''  A <code>Spring</code> can be treated as a pair of intervals
	'''  with a single common point: the preferred value.
	'''  The following rules define some of the
	'''  arithmetic operators that can be applied to intervals
	'''  (<code>[a, b]</code> refers to the interval
	'''  from <code>a</code>
	'''  to <code>b</code>,
	'''  where <code>a &lt;= b</code>).
	''' 
	'''  <pre>
	'''          [a1, b1] + [a2, b2] = [a1 + a2, b1 + b2]
	''' 
	'''                      -[a, b] = [-b, -a]
	''' 
	'''      max([a1, b1], [a2, b2]) = [max(a1, a2), max(b1, b2)]
	'''  </pre>
	'''  <p>
	''' 
	'''  If we denote <code>Spring</code>s as <code>[a, b, c]</code>,
	'''  where <code>a &lt;= b &lt;= c</code>, we can define the same
	'''  arithmetic operators on <code>Spring</code>s:
	''' 
	'''  <pre>
	'''          [a1, b1, c1] + [a2, b2, c2] = [a1 + a2, b1 + b2, c1 + c2]
	''' 
	'''                           -[a, b, c] = [-c, -b, -a]
	''' 
	'''      max([a1, b1, c1], [a2, b2, c2]) = [max(a1, a2), max(b1, b2), max(c1, c2)]
	'''  </pre>
	'''  <p>
	'''  With both intervals and <code>Spring</code>s we can define "-" and <em>min</em>
	'''  in terms of negation:
	''' 
	'''  <pre>
	'''      X - Y = X + (-Y)
	''' 
	'''      min(X, Y) = -max(-X, -Y)
	'''  </pre>
	'''  <p>
	'''  For the static methods in this class that embody the arithmetic
	'''  operators, we do not actually perform the operation in question as
	'''  that would snapshot the values of the properties of the method's arguments
	'''  at the time the static method is called. Instead, the static methods
	'''  create a new <code>Spring</code> instance containing references to
	'''  the method's arguments so that the characteristics of the new spring track the
	'''  potentially changing characteristics of the springs from which it
	'''  was made. This is a little like the idea of a <em>lazy value</em>
	'''  in a functional language.
	''' <p>
	''' If you are implementing a <code>SpringLayout</code> you
	''' can find further information and examples in
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/layout/spring.html">How to Use SpringLayout</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= SpringLayout </seealso>
	''' <seealso cref= SpringLayout.Constraints
	''' 
	''' @author      Philip Milne
	''' @since       1.4 </seealso>
	Public MustInherit Class Spring

		''' <summary>
		''' An integer value signifying that a property value has not yet been calculated.
		''' </summary>
		Public Shared ReadOnly UNSET As Integer = Integer.MIN_VALUE

		''' <summary>
		''' Used by factory methods to create a <code>Spring</code>.
		''' </summary>
		''' <seealso cref= #constant(int) </seealso>
		''' <seealso cref= #constant(int, int, int) </seealso>
		''' <seealso cref= #max </seealso>
		''' <seealso cref= #minus </seealso>
		''' <seealso cref= #sum </seealso>
		''' <seealso cref= SpringLayout.Constraints </seealso>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns the <em>minimum</em> value of this <code>Spring</code>.
		''' </summary>
		''' <returns> the <code>minimumValue</code> property of this <code>Spring</code> </returns>
		Public MustOverride ReadOnly Property minimumValue As Integer

		''' <summary>
		''' Returns the <em>preferred</em> value of this <code>Spring</code>.
		''' </summary>
		''' <returns> the <code>preferredValue</code> of this <code>Spring</code> </returns>
		Public MustOverride ReadOnly Property preferredValue As Integer

		''' <summary>
		''' Returns the <em>maximum</em> value of this <code>Spring</code>.
		''' </summary>
		''' <returns> the <code>maximumValue</code> property of this <code>Spring</code> </returns>
		Public MustOverride ReadOnly Property maximumValue As Integer

		''' <summary>
		''' Returns the current <em>value</em> of this <code>Spring</code>.
		''' </summary>
		''' <returns>  the <code>value</code> property of this <code>Spring</code>
		''' </returns>
		''' <seealso cref= #setValue </seealso>
		Public MustOverride Property value As Integer


		Private Function range(ByVal contract As Boolean) As Double
			Return If(contract, (preferredValue - minimumValue), (maximumValue - preferredValue))
		End Function

		'pp
	 Friend Overridable Property strain As Double
		 Get
				Dim delta As Double = (value - preferredValue)
				Return delta/range(value < preferredValue)
		 End Get
		 Set(ByVal strain As Double)
				value = preferredValue + CInt(Fix(strain * range(strain < 0)))
		 End Set
	 End Property

		'pp

		'pp
	 Friend Overridable Function isCyclic(ByVal l As SpringLayout) As Boolean
			Return False
	 End Function

		'pp
	 Friend MustInherit Class AbstractSpring
		 Inherits Spring

			Protected Friend size As Integer = UNSET

			Public Property Overrides value As Integer
				Get
					Return If(size <> UNSET, size, preferredValue)
				End Get
				Set(ByVal size As Integer)
					If Me.size = size Then Return
					If size = UNSET Then
						clear()
					Else
						nonClearValue = size
					End If
				End Set
			End Property


			Protected Friend Overridable Sub clear()
				size = UNSET
			End Sub

			Protected Friend Overridable Property nonClearValue As Integer
				Set(ByVal size As Integer)
					Me.size = size
				End Set
			End Property
	 End Class

		Private Class StaticSpring
			Inherits AbstractSpring

			Protected Friend min As Integer
			Protected Friend pref As Integer
			Protected Friend max As Integer

			Public Sub New(ByVal pref As Integer)
				Me.New(pref, pref, pref)
			End Sub

			Public Sub New(ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer)
				Me.min = min
				Me.pref = pref
				Me.max = max
			End Sub

			 Public Overrides Function ToString() As String
				 Return "StaticSpring [" & min & ", " & pref & ", " & max & "]"
			 End Function

			 Public Property Overrides minimumValue As Integer
				 Get
					Return min
				 End Get
			 End Property

			Public Property Overrides preferredValue As Integer
				Get
					Return pref
				End Get
			End Property

			Public Property Overrides maximumValue As Integer
				Get
					Return max
				End Get
			End Property
		End Class

		Private Class NegativeSpring
			Inherits Spring

			Private s As Spring

			Public Sub New(ByVal s As Spring)
				Me.s = s
			End Sub

	' Note the use of max value rather than minimum value here.
	' See the opening preamble on arithmetic with springs.

			Public Property Overrides minimumValue As Integer
				Get
					Return -s.maximumValue
				End Get
			End Property

			Public Property Overrides preferredValue As Integer
				Get
					Return -s.preferredValue
				End Get
			End Property

			Public Property Overrides maximumValue As Integer
				Get
					Return -s.minimumValue
				End Get
			End Property

			Public Property Overrides value As Integer
				Get
					Return -s.value
				End Get
				Set(ByVal size As Integer)
					' No need to check for UNSET as
					' Integer.MIN_VALUE == -Integer.MIN_VALUE.
					s.value = -size
				End Set
			End Property


			'pp
	 Friend Overrides Function isCyclic(ByVal l As SpringLayout) As Boolean
				Return s.isCyclic(l)
	 End Function
		End Class

		Private Class ScaleSpring
			Inherits Spring

			Private s As Spring
			Private factor As Single

			Private Sub New(ByVal s As Spring, ByVal factor As Single)
				Me.s = s
				Me.factor = factor
			End Sub

			Public Property Overrides minimumValue As Integer
				Get
					Return Math.Round((If(factor < 0, s.maximumValue, s.minimumValue)) * factor)
				End Get
			End Property

			Public Property Overrides preferredValue As Integer
				Get
					Return Math.Round(s.preferredValue * factor)
				End Get
			End Property

			Public Property Overrides maximumValue As Integer
				Get
					Return Math.Round((If(factor < 0, s.minimumValue, s.maximumValue)) * factor)
				End Get
			End Property

			Public Property Overrides value As Integer
				Get
					Return Math.Round(s.value * factor)
				End Get
				Set(ByVal value As Integer)
					If value = UNSET Then
						s.value = UNSET
					Else
						s.value = Math.Round(value / factor)
					End If
				End Set
			End Property


			'pp
	 Friend Overrides Function isCyclic(ByVal l As SpringLayout) As Boolean
				Return s.isCyclic(l)
	 End Function
		End Class

		'pp
	 Friend Class WidthSpring
		 Inherits AbstractSpring

			'pp
	 Friend c As java.awt.Component

			Public Sub New(ByVal c As java.awt.Component)
				Me.c = c
			End Sub

			Public Property Overrides minimumValue As Integer
				Get
					Return c.minimumSize.width
				End Get
			End Property

			Public Property Overrides preferredValue As Integer
				Get
					Return c.preferredSize.width
				End Get
			End Property

			Public Property Overrides maximumValue As Integer
				Get
					' We will be doing arithmetic with the results of this call,
					' so if a returned value is Integer.MAX_VALUE we will get
					' arithmetic overflow. Truncate such values.
					Return Math.Min(Short.MaxValue, c.maximumSize.width)
				End Get
			End Property
	 End Class

		 'pp
	  Friend Class HeightSpring
		  Inherits AbstractSpring

			'pp
	 Friend c As java.awt.Component

			Public Sub New(ByVal c As java.awt.Component)
				Me.c = c
			End Sub

			Public Property Overrides minimumValue As Integer
				Get
					Return c.minimumSize.height
				End Get
			End Property

			Public Property Overrides preferredValue As Integer
				Get
					Return c.preferredSize.height
				End Get
			End Property

			Public Property Overrides maximumValue As Integer
				Get
					Return Math.Min(Short.MaxValue, c.maximumSize.height)
				End Get
			End Property
	  End Class

	   'pp
	 Friend MustInherit Class SpringMap
		 Inherits Spring

		   Private s As Spring

		   Public Sub New(ByVal s As Spring)
			   Me.s = s
		   End Sub

		   Protected Friend MustOverride Function map(ByVal i As Integer) As Integer

		   Protected Friend MustOverride Function inv(ByVal i As Integer) As Integer

		   Public Property Overrides minimumValue As Integer
			   Get
				   Return map(s.minimumValue)
			   End Get
		   End Property

		   Public Property Overrides preferredValue As Integer
			   Get
				   Return map(s.preferredValue)
			   End Get
		   End Property

		   Public Property Overrides maximumValue As Integer
			   Get
				   Return Math.Min(Short.MaxValue, map(s.maximumValue))
			   End Get
		   End Property

		   Public Property Overrides value As Integer
			   Get
				   Return map(s.value)
			   End Get
			   Set(ByVal value As Integer)
				   If value = UNSET Then
					   s.value = UNSET
				   Else
					   s.value = inv(value)
				   End If
			   End Set
		   End Property


		   'pp
	 Friend Overrides Function isCyclic(ByVal l As SpringLayout) As Boolean
			   Return s.isCyclic(l)
	 End Function
	 End Class

	' Use the instance variables of the StaticSpring superclass to
	' cache values that have already been calculated.
		'pp
	 Friend MustInherit Class CompoundSpring
		 Inherits StaticSpring

			Protected Friend s1 As Spring
			Protected Friend s2 As Spring

			Public Sub New(ByVal s1 As Spring, ByVal s2 As Spring)
				MyBase.New(UNSET)
				Me.s1 = s1
				Me.s2 = s2
			End Sub

			Public Overrides Function ToString() As String
				Return "CompoundSpring of " & s1 & " and " & s2
			End Function

			Protected Friend Overrides Sub clear()
				MyBase.clear()
					max = UNSET
						pref = max
						min = pref
				s1.value = UNSET
				s2.value = UNSET
			End Sub

			Protected Friend MustOverride Function op(ByVal x As Integer, ByVal y As Integer) As Integer

			Public Property Overrides minimumValue As Integer
				Get
					If min = UNSET Then min = op(s1.minimumValue, s2.minimumValue)
					Return min
				End Get
			End Property

			Public Property Overrides preferredValue As Integer
				Get
					If pref = UNSET Then pref = op(s1.preferredValue, s2.preferredValue)
					Return pref
				End Get
			End Property

			Public Property Overrides maximumValue As Integer
				Get
					If max = UNSET Then max = op(s1.maximumValue, s2.maximumValue)
					Return max
				End Get
			End Property

			Public Property Overrides value As Integer
				Get
					If size = UNSET Then size = op(s1.value, s2.value)
					Return size
				End Get
			End Property

			'pp
	 Friend Overrides Function isCyclic(ByVal l As SpringLayout) As Boolean
				Return l.isCyclic(s1) OrElse l.isCyclic(s2)
	 End Function
	 End Class

		 Private Class SumSpring
			 Inherits CompoundSpring

			 Public Sub New(ByVal s1 As Spring, ByVal s2 As Spring)
				 MyBase.New(s1, s2)
			 End Sub

			 Protected Friend Overrides Function op(ByVal x As Integer, ByVal y As Integer) As Integer
				 Return x + y
			 End Function

			 Protected Friend Overrides Property nonClearValue As Integer
				 Set(ByVal size As Integer)
					 MyBase.nonClearValue = size
					 s1.strain = Me.strain
					 s2.value = size - s1.value
				 End Set
			 End Property
		 End Class

		Private Class MaxSpring
			Inherits CompoundSpring

			Public Sub New(ByVal s1 As Spring, ByVal s2 As Spring)
				MyBase.New(s1, s2)
			End Sub

			Protected Friend Overrides Function op(ByVal x As Integer, ByVal y As Integer) As Integer
				Return Math.Max(x, y)
			End Function

			Protected Friend Overrides Property nonClearValue As Integer
				Set(ByVal size As Integer)
					MyBase.nonClearValue = size
					s1.value = size
					s2.value = size
				End Set
			End Property
		End Class

		''' <summary>
		''' Returns a strut -- a spring whose <em>minimum</em>, <em>preferred</em>, and
		''' <em>maximum</em> values each have the value <code>pref</code>.
		''' </summary>
		''' <param name="pref"> the <em>minimum</em>, <em>preferred</em>, and
		'''         <em>maximum</em> values of the new spring </param>
		''' <returns> a spring whose <em>minimum</em>, <em>preferred</em>, and
		'''         <em>maximum</em> values each have the value <code>pref</code>
		''' </returns>
		''' <seealso cref= Spring </seealso>
		 Public Shared Function constant(ByVal pref As Integer) As Spring
			 Return constant(pref, pref, pref)
		 End Function

		''' <summary>
		''' Returns a spring whose <em>minimum</em>, <em>preferred</em>, and
		''' <em>maximum</em> values have the values: <code>min</code>, <code>pref</code>,
		''' and <code>max</code> respectively.
		''' </summary>
		''' <param name="min"> the <em>minimum</em> value of the new spring </param>
		''' <param name="pref"> the <em>preferred</em> value of the new spring </param>
		''' <param name="max"> the <em>maximum</em> value of the new spring </param>
		''' <returns> a spring whose <em>minimum</em>, <em>preferred</em>, and
		'''         <em>maximum</em> values have the values: <code>min</code>, <code>pref</code>,
		'''         and <code>max</code> respectively
		''' </returns>
		''' <seealso cref= Spring </seealso>
		 Public Shared Function constant(ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer) As Spring
			 Return New StaticSpring(min, pref, max)
		 End Function


		''' <summary>
		''' Returns <code>-s</code>: a spring running in the opposite direction to <code>s</code>.
		''' </summary>
		''' <returns> <code>-s</code>: a spring running in the opposite direction to <code>s</code>
		''' </returns>
		''' <seealso cref= Spring </seealso>
		Public Shared Function minus(ByVal s As Spring) As Spring
			Return New NegativeSpring(s)
		End Function

		''' <summary>
		''' Returns <code>s1+s2</code>: a spring representing <code>s1</code> and <code>s2</code>
		''' in series. In a sum, <code>s3</code>, of two springs, <code>s1</code> and <code>s2</code>,
		''' the <em>strains</em> of <code>s1</code>, <code>s2</code>, and <code>s3</code> are maintained
		''' at the same level (to within the precision implied by their integer <em>value</em>s).
		''' The strain of a spring in compression is:
		''' <pre>
		'''         value - pref
		'''         ------------
		'''          pref - min
		''' </pre>
		''' and the strain of a spring in tension is:
		''' <pre>
		'''         value - pref
		'''         ------------
		'''          max - pref
		''' </pre>
		''' When <code>setValue</code> is called on the sum spring, <code>s3</code>, the strain
		''' in <code>s3</code> is calculated using one of the formulas above. Once the strain of
		''' the sum is known, the <em>value</em>s of <code>s1</code> and <code>s2</code> are
		''' then set so that they are have a strain equal to that of the sum. The formulas are
		''' evaluated so as to take rounding errors into account and ensure that the sum of
		''' the <em>value</em>s of <code>s1</code> and <code>s2</code> is exactly equal to
		''' the <em>value</em> of <code>s3</code>.
		''' </summary>
		''' <returns> <code>s1+s2</code>: a spring representing <code>s1</code> and <code>s2</code> in series
		''' </returns>
		''' <seealso cref= Spring </seealso>
		 Public Shared Function sum(ByVal s1 As Spring, ByVal s2 As Spring) As Spring
			 Return New SumSpring(s1, s2)
		 End Function

		''' <summary>
		''' Returns <code>max(s1, s2)</code>: a spring whose value is always greater than (or equal to)
		'''         the values of both <code>s1</code> and <code>s2</code>.
		''' </summary>
		''' <returns> <code>max(s1, s2)</code>: a spring whose value is always greater than (or equal to)
		'''         the values of both <code>s1</code> and <code>s2</code> </returns>
		''' <seealso cref= Spring </seealso>
		Public Shared Function max(ByVal s1 As Spring, ByVal s2 As Spring) As Spring
			Return New MaxSpring(s1, s2)
		End Function

		' Remove these, they're not used often and can be created using minus -
		' as per these implementations.

		'pp
	 Shared Function difference(ByVal s1 As Spring, ByVal s2 As Spring) As Spring
			Return sum(s1, minus(s2))
	 End Function

	'    
	'    public static Spring min(Spring s1, Spring s2) {
	'        return minus(max(minus(s1), minus(s2)));
	'    }
	'    

		''' <summary>
		''' Returns a spring whose <em>minimum</em>, <em>preferred</em>, <em>maximum</em>
		''' and <em>value</em> properties are each multiples of the properties of the
		''' argument spring, <code>s</code>. Minimum and maximum properties are
		''' swapped when <code>factor</code> is negative (in accordance with the
		''' rules of interval arithmetic).
		''' <p>
		''' When factor is, for example, 0.5f the result represents 'the mid-point'
		''' of its input - an operation that is useful for centering components in
		''' a container.
		''' </summary>
		''' <param name="s"> the spring to scale </param>
		''' <param name="factor"> amount to scale by. </param>
		''' <returns>  a spring whose properties are those of the input spring <code>s</code>
		''' multiplied by <code>factor</code> </returns>
		''' <exception cref="NullPointerException"> if <code>s</code> is null
		''' @since 1.5 </exception>
		Public Shared Function scale(ByVal s As Spring, ByVal factor As Single) As Spring
			checkArg(s)
			Return New ScaleSpring(s, factor)
		End Function

		''' <summary>
		''' Returns a spring whose <em>minimum</em>, <em>preferred</em>, <em>maximum</em>
		''' and <em>value</em> properties are defined by the widths of the <em>minimumSize</em>,
		''' <em>preferredSize</em>, <em>maximumSize</em> and <em>size</em> properties
		''' of the supplied component. The returned spring is a 'wrapper' implementation
		''' whose methods call the appropriate size methods of the supplied component.
		''' The minimum, preferred, maximum and value properties of the returned spring
		''' therefore report the current state of the appropriate properties in the
		''' component and track them as they change.
		''' </summary>
		''' <param name="c"> Component used for calculating size </param>
		''' <returns>  a spring whose properties are defined by the horizontal component
		''' of the component's size methods. </returns>
		''' <exception cref="NullPointerException"> if <code>c</code> is null
		''' @since 1.5 </exception>
		Public Shared Function width(ByVal c As java.awt.Component) As Spring
			checkArg(c)
			Return New WidthSpring(c)
		End Function

		''' <summary>
		''' Returns a spring whose <em>minimum</em>, <em>preferred</em>, <em>maximum</em>
		''' and <em>value</em> properties are defined by the heights of the <em>minimumSize</em>,
		''' <em>preferredSize</em>, <em>maximumSize</em> and <em>size</em> properties
		''' of the supplied component. The returned spring is a 'wrapper' implementation
		''' whose methods call the appropriate size methods of the supplied component.
		''' The minimum, preferred, maximum and value properties of the returned spring
		''' therefore report the current state of the appropriate properties in the
		''' component and track them as they change.
		''' </summary>
		''' <param name="c"> Component used for calculating size </param>
		''' <returns>  a spring whose properties are defined by the vertical component
		''' of the component's size methods. </returns>
		''' <exception cref="NullPointerException"> if <code>c</code> is null
		''' @since 1.5 </exception>
		Public Shared Function height(ByVal c As java.awt.Component) As Spring
			checkArg(c)
			Return New HeightSpring(c)
		End Function


		''' <summary>
		''' If <code>s</code> is null, this throws an NullPointerException.
		''' </summary>
		Private Shared Sub checkArg(ByVal s As Object)
			If s Is Nothing Then Throw New NullPointerException("Argument must not be null")
		End Sub
	End Class

End Namespace
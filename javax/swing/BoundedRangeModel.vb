Imports javax.swing.event

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Defines the data model used by components like <code>Slider</code>s
	''' and <code>ProgressBar</code>s.
	''' Defines four interrelated integer properties: minimum, maximum, extent
	''' and value.  These four integers define two nested ranges like this:
	''' <pre>
	''' minimum &lt;= value &lt;= value+extent &lt;= maximum
	''' </pre>
	''' The outer range is <code>minimum,maximum</code> and the inner
	''' range is <code>value,value+extent</code>.  The inner range
	''' must lie within the outer one, i.e. <code>value</code> must be
	''' less than or equal to <code>maximum</code> and <code>value+extent</code>
	''' must greater than or equal to <code>minimum</code>, and <code>maximum</code>
	''' must be greater than or equal to <code>minimum</code>.
	''' There are a few features of this model that one might find a little
	''' surprising.  These quirks exist for the convenience of the
	''' Swing BoundedRangeModel clients, such as <code>Slider</code> and
	''' <code>ScrollBar</code>.
	''' <ul>
	''' <li>
	'''   The minimum and maximum set methods "correct" the other
	'''   three properties to accommodate their new value argument.  For
	'''   example setting the model's minimum may change its maximum, value,
	'''   and extent properties (in that order), to maintain the constraints
	'''   specified above.
	''' 
	''' <li>
	'''   The value and extent set methods "correct" their argument to
	'''   fit within the limits defined by the other three properties.
	'''   For example if <code>value == maximum</code>, <code>setExtent(10)</code>
	'''   would change the extent (back) to zero.
	''' 
	''' <li>
	'''   The four BoundedRangeModel values are defined as Java Beans properties
	'''   however Swing ChangeEvents are used to notify clients of changes rather
	'''   than PropertyChangeEvents. This was done to keep the overhead of monitoring
	'''   a BoundedRangeModel low. Changes are often reported at MouseDragged rates.
	''' </ul>
	''' 
	''' <p>
	''' 
	''' For an example of specifying custom bounded range models used by sliders,
	''' see <a
	''' href="http://www.oracle.com/technetwork/java/architecture-142923.html#separable">Separable model architecture</a>
	''' in <em>A Swing Architecture Overview.</em>
	''' 
	''' @author Hans Muller </summary>
	''' <seealso cref= DefaultBoundedRangeModel </seealso>
	Public Interface BoundedRangeModel
		''' <summary>
		''' Returns the minimum acceptable value.
		''' </summary>
		''' <returns> the value of the minimum property </returns>
		''' <seealso cref= #setMinimum </seealso>
		Property minimum As Integer




		''' <summary>
		''' Returns the model's maximum.  Note that the upper
		''' limit on the model's value is (maximum - extent).
		''' </summary>
		''' <returns> the value of the maximum property. </returns>
		''' <seealso cref= #setMaximum </seealso>
		''' <seealso cref= #setExtent </seealso>
		Property maximum As Integer




		''' <summary>
		''' Returns the model's current value.  Note that the upper
		''' limit on the model's value is <code>maximum - extent</code>
		''' and the lower limit is <code>minimum</code>.
		''' </summary>
		''' <returns>  the model's value </returns>
		''' <seealso cref=     #setValue </seealso>
		Property value As Integer




		''' <summary>
		''' This attribute indicates that any upcoming changes to the value
		''' of the model should be considered a single event. This attribute
		''' will be set to true at the start of a series of changes to the value,
		''' and will be set to false when the value has finished changing.  Normally
		''' this allows a listener to only take action when the final value change in
		''' committed, instead of having to do updates for all intermediate values.
		''' <p>
		''' Sliders and scrollbars use this property when a drag is underway.
		''' </summary>
		''' <param name="b"> true if the upcoming changes to the value property are part of a series </param>
		Property valueIsAdjusting As Boolean




		''' <summary>
		''' Returns the model's extent, the length of the inner range that
		''' begins at the model's value.
		''' </summary>
		''' <returns>  the value of the model's extent property </returns>
		''' <seealso cref=     #setExtent </seealso>
		''' <seealso cref=     #setValue </seealso>
		Property extent As Integer





		''' <summary>
		''' This method sets all of the model's data with a single method call.
		''' The method results in a single change event being generated. This is
		''' convenient when you need to adjust all the model data simultaneously and
		''' do not want individual change events to occur.
		''' </summary>
		''' <param name="value">  an int giving the current value </param>
		''' <param name="extent"> an int giving the amount by which the value can "jump" </param>
		''' <param name="min">    an int giving the minimum value </param>
		''' <param name="max">    an int giving the maximum value </param>
		''' <param name="adjusting"> a boolean, true if a series of changes are in
		'''                    progress
		''' </param>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= #setExtent </seealso>
		''' <seealso cref= #setMinimum </seealso>
		''' <seealso cref= #setMaximum </seealso>
		''' <seealso cref= #setValueIsAdjusting </seealso>
		Sub setRangeProperties(ByVal value As Integer, ByVal extent As Integer, ByVal min As Integer, ByVal max As Integer, ByVal adjusting As Boolean)


		''' <summary>
		''' Adds a ChangeListener to the model's listener list.
		''' </summary>
		''' <param name="x"> the ChangeListener to add </param>
		''' <seealso cref= #removeChangeListener </seealso>
		Sub addChangeListener(ByVal x As ChangeListener)


		''' <summary>
		''' Removes a ChangeListener from the model's listener list.
		''' </summary>
		''' <param name="x"> the ChangeListener to remove </param>
		''' <seealso cref= #addChangeListener </seealso>
		Sub removeChangeListener(ByVal x As ChangeListener)

	End Interface

End Namespace
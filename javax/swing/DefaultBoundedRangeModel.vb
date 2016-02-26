Imports System
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
	''' A generic implementation of BoundedRangeModel.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author David Kloba
	''' @author Hans Muller </summary>
	''' <seealso cref= BoundedRangeModel </seealso>
	<Serializable> _
	Public Class DefaultBoundedRangeModel
		Implements BoundedRangeModel

		''' <summary>
		''' Only one <code>ChangeEvent</code> is needed per model instance since the
		''' event's only (read-only) state is the source property.  The source
		''' of events generated here is always "this".
		''' </summary>
		<NonSerialized> _
		Protected Friend changeEvent As ChangeEvent = Nothing

		''' <summary>
		''' The listeners waiting for model changes. </summary>
		Protected Friend listenerList As New EventListenerList

		Private value As Integer = 0
		Private extent As Integer = 0
		Private min As Integer = 0
		Private max As Integer = 100
		Private isAdjusting As Boolean = False


		''' <summary>
		''' Initializes all of the properties with default values.
		''' Those values are:
		''' <ul>
		''' <li><code>value</code> = 0
		''' <li><code>extent</code> = 0
		''' <li><code>minimum</code> = 0
		''' <li><code>maximum</code> = 100
		''' <li><code>adjusting</code> = false
		''' </ul>
		''' </summary>
		Public Sub New()
		End Sub


		''' <summary>
		''' Initializes value, extent, minimum and maximum. Adjusting is false.
		''' Throws an <code>IllegalArgumentException</code> if the following
		''' constraints aren't satisfied:
		''' <pre>
		''' min &lt;= value &lt;= value+extent &lt;= max
		''' </pre>
		''' </summary>
		Public Sub New(ByVal value As Integer, ByVal extent As Integer, ByVal min As Integer, ByVal max As Integer)
			If (max >= min) AndAlso (value >= min) AndAlso ((value + extent) >= value) AndAlso ((value + extent) <= max) Then
				Me.value = value
				Me.extent = extent
				Me.min = min
				Me.max = max
			Else
				Throw New System.ArgumentException("invalid range properties")
			End If
		End Sub


		''' <summary>
		''' Returns the model's current value. </summary>
		''' <returns> the model's current value </returns>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= BoundedRangeModel#getValue </seealso>
		Public Overridable Property value As Integer Implements BoundedRangeModel.getValue
			Get
			  Return value
			End Get
			Set(ByVal n As Integer)
				n = Math.Min(n, Integer.MaxValue - extent)
    
				Dim newValue As Integer = Math.Max(n, min)
				If newValue + extent > max Then newValue = max - extent
				rangePropertiesies(newValue, extent, min, max, isAdjusting)
			End Set
		End Property


		''' <summary>
		''' Returns the model's extent. </summary>
		''' <returns> the model's extent </returns>
		''' <seealso cref= #setExtent </seealso>
		''' <seealso cref= BoundedRangeModel#getExtent </seealso>
		Public Overridable Property extent As Integer Implements BoundedRangeModel.getExtent
			Get
			  Return extent
			End Get
			Set(ByVal n As Integer)
				Dim newExtent As Integer = Math.Max(0, n)
				If value + newExtent > max Then newExtent = max - value
				rangePropertiesies(value, newExtent, min, max, isAdjusting)
			End Set
		End Property


		''' <summary>
		''' Returns the model's minimum. </summary>
		''' <returns> the model's minimum </returns>
		''' <seealso cref= #setMinimum </seealso>
		''' <seealso cref= BoundedRangeModel#getMinimum </seealso>
		Public Overridable Property minimum As Integer Implements BoundedRangeModel.getMinimum
			Get
			  Return min
			End Get
			Set(ByVal n As Integer)
				Dim newMax As Integer = Math.Max(n, max)
				Dim newValue As Integer = Math.Max(n, value)
				Dim newExtent As Integer = Math.Min(newMax - newValue, extent)
				rangePropertiesies(newValue, newExtent, n, newMax, isAdjusting)
			End Set
		End Property


		''' <summary>
		''' Returns the model's maximum. </summary>
		''' <returns>  the model's maximum </returns>
		''' <seealso cref= #setMaximum </seealso>
		''' <seealso cref= BoundedRangeModel#getMaximum </seealso>
		Public Overridable Property maximum As Integer Implements BoundedRangeModel.getMaximum
			Get
				Return max
			End Get
			Set(ByVal n As Integer)
				Dim newMin As Integer = Math.Min(n, min)
				Dim newExtent As Integer = Math.Min(n - newMin, extent)
				Dim newValue As Integer = Math.Min(n - newExtent, value)
				rangePropertiesies(newValue, newExtent, newMin, n, isAdjusting)
			End Set
		End Property










		''' <summary>
		''' Sets the <code>valueIsAdjusting</code> property.
		''' </summary>
		''' <seealso cref= #getValueIsAdjusting </seealso>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= BoundedRangeModel#setValueIsAdjusting </seealso>
		Public Overridable Property valueIsAdjusting Implements BoundedRangeModel.setValueIsAdjusting As Boolean
			Set(ByVal b As Boolean)
				rangePropertiesies(value, extent, min, max, b)
			End Set
			Get
				Return isAdjusting
			End Get
		End Property




		''' <summary>
		''' Sets all of the <code>BoundedRangeModel</code> properties after forcing
		''' the arguments to obey the usual constraints:
		''' <pre>
		'''     minimum &lt;= value &lt;= value+extent &lt;= maximum
		''' </pre>
		''' <p>
		''' At most, one <code>ChangeEvent</code> is generated.
		''' </summary>
		''' <seealso cref= BoundedRangeModel#setRangeProperties </seealso>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= #setExtent </seealso>
		''' <seealso cref= #setMinimum </seealso>
		''' <seealso cref= #setMaximum </seealso>
		''' <seealso cref= #setValueIsAdjusting </seealso>
		Public Overridable Sub setRangeProperties(ByVal newValue As Integer, ByVal newExtent As Integer, ByVal newMin As Integer, ByVal newMax As Integer, ByVal adjusting As Boolean) Implements BoundedRangeModel.setRangeProperties
			If newMin > newMax Then newMin = newMax
			If newValue > newMax Then newMax = newValue
			If newValue < newMin Then newMin = newValue

	'         Convert the addends to long so that extent can be
	'         * Integer.MAX_VALUE without rolling over the sum.
	'         * A JCK test covers this, see bug 4097718.
	'         
			If (CLng(newExtent) + CLng(newValue)) > newMax Then newExtent = newMax - newValue

			If newExtent < 0 Then newExtent = 0

			Dim isChange As Boolean = (newValue <> value) OrElse (newExtent <> extent) OrElse (newMin <> min) OrElse (newMax <> max) OrElse (adjusting <> isAdjusting)

			If isChange Then
				value = newValue
				extent = newExtent
				min = newMin
				max = newMax
				isAdjusting = adjusting

				fireStateChanged()
			End If
		End Sub


		''' <summary>
		''' Adds a <code>ChangeListener</code>.  The change listeners are run each
		''' time any one of the Bounded Range model properties changes.
		''' </summary>
		''' <param name="l"> the ChangeListener to add </param>
		''' <seealso cref= #removeChangeListener </seealso>
		''' <seealso cref= BoundedRangeModel#addChangeListener </seealso>
		Public Overridable Sub addChangeListener(ByVal l As ChangeListener) Implements BoundedRangeModel.addChangeListener
			listenerList.add(GetType(ChangeListener), l)
		End Sub


		''' <summary>
		''' Removes a <code>ChangeListener</code>.
		''' </summary>
		''' <param name="l"> the <code>ChangeListener</code> to remove </param>
		''' <seealso cref= #addChangeListener </seealso>
		''' <seealso cref= BoundedRangeModel#removeChangeListener </seealso>
		Public Overridable Sub removeChangeListener(ByVal l As ChangeListener) Implements BoundedRangeModel.removeChangeListener
			listenerList.remove(GetType(ChangeListener), l)
		End Sub


		''' <summary>
		''' Returns an array of all the change listeners
		''' registered on this <code>DefaultBoundedRangeModel</code>.
		''' </summary>
		''' <returns> all of this model's <code>ChangeListener</code>s
		'''         or an empty
		'''         array if no change listeners are currently registered
		''' </returns>
		''' <seealso cref= #addChangeListener </seealso>
		''' <seealso cref= #removeChangeListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property changeListeners As ChangeListener()
			Get
				Return listenerList.getListeners(GetType(ChangeListener))
			End Get
		End Property


		''' <summary>
		''' Runs each <code>ChangeListener</code>'s <code>stateChanged</code> method.
		''' </summary>
		''' <seealso cref= #setRangeProperties </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireStateChanged()
			Dim ___listeners As Object() = listenerList.listenerList
			For i As Integer = ___listeners.Length - 2 To 0 Step -2
				If ___listeners(i) Is GetType(ChangeListener) Then
					If changeEvent Is Nothing Then changeEvent = New ChangeEvent(Me)
					CType(___listeners(i+1), ChangeListener).stateChanged(changeEvent)
				End If
			Next i
		End Sub


		''' <summary>
		''' Returns a string that displays all of the
		''' <code>BoundedRangeModel</code> properties.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim modelString As String = "value=" & value & ", " & "extent=" & extent & ", " & "min=" & minimum & ", " & "max=" & maximum & ", " & "adj=" & valueIsAdjusting

			Return Me.GetType().name & "[" & modelString & "]"
		End Function

		''' <summary>
		''' Returns an array of all the objects currently registered as
		''' <code><em>Foo</em>Listener</code>s
		''' upon this model.
		''' <code><em>Foo</em>Listener</code>s
		''' are registered using the <code>add<em>Foo</em>Listener</code> method.
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a <code>DefaultBoundedRangeModel</code>
		''' instance <code>m</code>
		''' for its change listeners
		''' with the following code:
		''' 
		''' <pre>ChangeListener[] cls = (ChangeListener[])(m.getListeners(ChangeListener.class));</pre>
		''' 
		''' If no such listeners exist,
		''' this method returns an empty array.
		''' </summary>
		''' <param name="listenerType">  the type of listeners requested;
		'''          this parameter should specify an interface
		'''          that descends from <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s
		'''          on this model,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code> doesn't
		'''          specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getChangeListeners
		''' 
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function
	End Class

End Namespace
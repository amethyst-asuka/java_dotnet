Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

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
Namespace java.awt




	''' <summary>
	''' This class represents the state of a horizontal or vertical
	''' scrollbar of a <code>ScrollPane</code>.  Objects of this class are
	''' returned by <code>ScrollPane</code> methods.
	''' 
	''' @since       1.4
	''' </summary>
	<Serializable> _
	Public Class ScrollPaneAdjustable
		Implements Adjustable

		''' <summary>
		''' The <code>ScrollPane</code> this object is a scrollbar of.
		''' @serial
		''' </summary>
		Private sp As ScrollPane

		''' <summary>
		''' Orientation of this scrollbar.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getOrientation </seealso>
		''' <seealso cref= java.awt.Adjustable#HORIZONTAL </seealso>
		''' <seealso cref= java.awt.Adjustable#VERTICAL </seealso>
		Private orientation As Integer

		''' <summary>
		''' The value of this scrollbar.
		''' <code>value</code> should be greater than <code>minimum</code>
		''' and less than <code>maximum</code>
		''' 
		''' @serial </summary>
		''' <seealso cref= #getValue </seealso>
		''' <seealso cref= #setValue </seealso>
		Private value As Integer

		''' <summary>
		''' The minimum value of this scrollbar.
		''' This value can only be set by the <code>ScrollPane</code>.
		''' <p>
		''' <strong>ATTN:</strong> In current implementation
		''' <code>minimum</code> is always <code>0</code>.  This field can
		''' only be altered via <code>setSpan</code> method and
		''' <code>ScrollPane</code> always calls that method with
		''' <code>0</code> for the minimum.  <code>getMinimum</code> method
		''' always returns <code>0</code> without checking this field.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMinimum </seealso>
		''' <seealso cref= #setSpan(int, int, int) </seealso>
		Private minimum As Integer

		''' <summary>
		''' The maximum value of this scrollbar.
		''' This value can only be set by the <code>ScrollPane</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMaximum </seealso>
		''' <seealso cref= #setSpan(int, int, int) </seealso>
		Private maximum As Integer

		''' <summary>
		''' The size of the visible portion of this scrollbar.
		''' This value can only be set by the <code>ScrollPane</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getVisibleAmount </seealso>
		''' <seealso cref= #setSpan(int, int, int) </seealso>
		Private visibleAmount As Integer

		''' <summary>
		''' The adjusting status of the <code>Scrollbar</code>.
		''' True if the value is in the process of changing as a result of
		''' actions being taken by the user.
		''' </summary>
		''' <seealso cref= #getValueIsAdjusting </seealso>
		''' <seealso cref= #setValueIsAdjusting
		''' @since 1.4 </seealso>
		<NonSerialized> _
		Private isAdjusting As Boolean

		''' <summary>
		''' The amount by which the scrollbar value will change when going
		''' up or down by a line.
		''' This value should be a non negative  java.lang.[Integer].
		''' 
		''' @serial </summary>
		''' <seealso cref= #getUnitIncrement </seealso>
		''' <seealso cref= #setUnitIncrement </seealso>
		Private unitIncrement As Integer = 1

		''' <summary>
		''' The amount by which the scrollbar value will change when going
		''' up or down by a page.
		''' This value should be a non negative  java.lang.[Integer].
		''' 
		''' @serial </summary>
		''' <seealso cref= #getBlockIncrement </seealso>
		''' <seealso cref= #setBlockIncrement </seealso>
		Private blockIncrement As Integer = 1

		Private adjustmentListener As java.awt.event.AdjustmentListener

		''' <summary>
		''' Error message for <code>AWTError</code> reported when one of
		''' the public but unsupported methods is called.
		''' </summary>
		Private Const SCROLLPANE_ONLY As String = "Can be set by scrollpane only"


		''' <summary>
		''' Initialize JNI field and method ids.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

		Shared Sub New()
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setScrollPaneAdjustableAccessor(New sun.awt.AWTAccessor.ScrollPaneAdjustableAccessor()
	'		{
	'			public  Sub  setTypedValue(final ScrollPaneAdjustable adj, final int v, final int type)
	'			{
	'				adj.setTypedValue(v, type);
	'			}
	'		});
		End Sub

		''' <summary>
		''' JDK 1.1 serialVersionUID.
		''' </summary>
		Private Const serialVersionUID As Long = -3359745691033257079L


		''' <summary>
		''' Constructs a new object to represent specified scrollabar
		''' of the specified <code>ScrollPane</code>.
		''' Only ScrollPane creates instances of this class. </summary>
		''' <param name="sp">           <code>ScrollPane</code> </param>
		''' <param name="l">            <code>AdjustmentListener</code> to add upon creation. </param>
		''' <param name="orientation">  specifies which scrollbar this object represents,
		'''                     can be either  <code>Adjustable.HORIZONTAL</code>
		'''                     or <code>Adjustable.VERTICAL</code>. </param>
		Friend Sub New(  sp As ScrollPane,   l As java.awt.event.AdjustmentListener,   orientation As Integer)
			Me.sp = sp
			Me.orientation = orientation
			addAdjustmentListener(l)
		End Sub

		''' <summary>
		''' This is called by the scrollpane itself to update the
		''' <code>minimum</code>, <code>maximum</code> and
		''' <code>visible</code> values.  The scrollpane is the only one
		''' that should be changing these since it is the source of these
		''' values.
		''' </summary>
		Friend Overridable Sub setSpan(  min As Integer,   max As Integer,   visible As Integer)
			' adjust the values to be reasonable
			minimum = min
			maximum = System.Math.Max(max, minimum + 1)
			visibleAmount = System.Math.Min(visible, maximum - minimum)
			visibleAmount = System.Math.Max(visibleAmount, 1)
			blockIncrement = System.Math.Max(CInt(Fix(visible *.90)), 1)
			value = value
		End Sub

		''' <summary>
		''' Returns the orientation of this scrollbar. </summary>
		''' <returns>    the orientation of this scrollbar, either
		'''            <code>Adjustable.HORIZONTAL</code> or
		'''            <code>Adjustable.VERTICAL</code> </returns>
		Public Overridable Property orientation As Integer Implements Adjustable.getOrientation
			Get
				Return orientation
			End Get
		End Property

		''' <summary>
		''' This method should <strong>NOT</strong> be called by user code.
		''' This method is public for this class to properly implement
		''' <code>Adjustable</code> interface.
		''' </summary>
		''' <exception cref="AWTError"> Always throws an error when called. </exception>
		Public Overridable Property minimum Implements Adjustable.setMinimum As Integer
			Set(  min As Integer)
				Throw New AWTError(SCROLLPANE_ONLY)
			End Set
			Get
				' XXX: This relies on setSpan always being called with 0 for
				' the minimum (which is currently true).
				Return 0
			End Get
		End Property


		''' <summary>
		''' This method should <strong>NOT</strong> be called by user code.
		''' This method is public for this class to properly implement
		''' <code>Adjustable</code> interface.
		''' </summary>
		''' <exception cref="AWTError"> Always throws an error when called. </exception>
		Public Overridable Property maximum Implements Adjustable.setMaximum As Integer
			Set(  max As Integer)
				Throw New AWTError(SCROLLPANE_ONLY)
			End Set
			Get
				Return maximum
			End Get
		End Property


		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property unitIncrement Implements Adjustable.setUnitIncrement As Integer
			Set(  u As Integer)
				If u <> unitIncrement Then
					unitIncrement = u
					If sp.peer IsNot Nothing Then
						Dim peer As java.awt.peer.ScrollPanePeer = CType(sp.peer, java.awt.peer.ScrollPanePeer)
						peer.unitIncrementent(Me, u)
					End If
				End If
			End Set
			Get
				Return unitIncrement
			End Get
		End Property


		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property blockIncrement Implements Adjustable.setBlockIncrement As Integer
			Set(  b As Integer)
				blockIncrement = b
			End Set
			Get
				Return blockIncrement
			End Get
		End Property


		''' <summary>
		''' This method should <strong>NOT</strong> be called by user code.
		''' This method is public for this class to properly implement
		''' <code>Adjustable</code> interface.
		''' </summary>
		''' <exception cref="AWTError"> Always throws an error when called. </exception>
		Public Overridable Property visibleAmount Implements Adjustable.setVisibleAmount As Integer
			Set(  v As Integer)
				Throw New AWTError(SCROLLPANE_ONLY)
			End Set
			Get
				Return visibleAmount
			End Get
		End Property



		''' <summary>
		''' Sets the <code>valueIsAdjusting</code> property.
		''' </summary>
		''' <param name="b"> new adjustment-in-progress status </param>
		''' <seealso cref= #getValueIsAdjusting
		''' @since 1.4 </seealso>
		Public Overridable Property valueIsAdjusting As Boolean
			Set(  b As Boolean)
				If isAdjusting <> b Then
					isAdjusting = b
					Dim e As New java.awt.event.AdjustmentEvent(Me, java.awt.event.AdjustmentEvent.ADJUSTMENT_VALUE_CHANGED, java.awt.event.AdjustmentEvent.TRACK, value, b)
					adjustmentListener.adjustmentValueChanged(e)
				End If
			End Set
			Get
				Return isAdjusting
			End Get
		End Property


		''' <summary>
		''' Sets the value of this scrollbar to the specified value.
		''' <p>
		''' If the value supplied is less than the current minimum or
		''' greater than the current maximum, then one of those values is
		''' substituted, as appropriate.
		''' </summary>
		''' <param name="v"> the new value of the scrollbar </param>
		Public Overridable Property value Implements Adjustable.setValue As Integer
			Set(  v As Integer)
				typedValuelue(v, java.awt.event.AdjustmentEvent.TRACK)
			End Set
			Get
				Return value
			End Get
		End Property

		''' <summary>
		''' Sets the value of this scrollbar to the specified value.
		''' <p>
		''' If the value supplied is less than the current minimum or
		''' greater than the current maximum, then one of those values is
		''' substituted, as appropriate. Also, creates and dispatches
		''' the AdjustementEvent with specified type and value.
		''' </summary>
		''' <param name="v"> the new value of the scrollbar </param>
		''' <param name="type"> the type of the scrolling operation occurred </param>
		Private Sub setTypedValue(  v As Integer,   type As Integer)
			v = System.Math.Max(v, minimum)
			v = System.Math.Min(v, maximum - visibleAmount)

			If v <> value Then
				value = v
				' Synchronously notify the listeners so that they are
				' guaranteed to be up-to-date with the Adjustable before
				' it is mutated again.
				Dim e As New java.awt.event.AdjustmentEvent(Me, java.awt.event.AdjustmentEvent.ADJUSTMENT_VALUE_CHANGED, type, value, isAdjusting)
				adjustmentListener.adjustmentValueChanged(e)
			End If
		End Sub


		''' <summary>
		''' Adds the specified adjustment listener to receive adjustment
		''' events from this <code>ScrollPaneAdjustable</code>.
		''' If <code>l</code> is <code>null</code>, no exception is thrown
		''' and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l">   the adjustment listener. </param>
		''' <seealso cref=      #removeAdjustmentListener </seealso>
		''' <seealso cref=      #getAdjustmentListeners </seealso>
		''' <seealso cref=      java.awt.event.AdjustmentListener </seealso>
		''' <seealso cref=      java.awt.event.AdjustmentEvent </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addAdjustmentListener(  l As java.awt.event.AdjustmentListener)
			If l Is Nothing Then Return
			adjustmentListener = AWTEventMulticaster.add(adjustmentListener, l)
		End Sub

		''' <summary>
		''' Removes the specified adjustment listener so that it no longer
		''' receives adjustment events from this <code>ScrollPaneAdjustable</code>.
		''' If <code>l</code> is <code>null</code>, no exception is thrown
		''' and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l">     the adjustment listener. </param>
		''' <seealso cref=           #addAdjustmentListener </seealso>
		''' <seealso cref=           #getAdjustmentListeners </seealso>
		''' <seealso cref=           java.awt.event.AdjustmentListener </seealso>
		''' <seealso cref=           java.awt.event.AdjustmentEvent
		''' @since         JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeAdjustmentListener(  l As java.awt.event.AdjustmentListener)
			If l Is Nothing Then Return
			adjustmentListener = AWTEventMulticaster.remove(adjustmentListener, l)
		End Sub

		''' <summary>
		''' Returns an array of all the adjustment listeners
		''' registered on this <code>ScrollPaneAdjustable</code>.
		''' </summary>
		''' <returns> all of this <code>ScrollPaneAdjustable</code>'s
		'''         <code>AdjustmentListener</code>s
		'''         or an empty array if no adjustment
		'''         listeners are currently registered
		''' </returns>
		''' <seealso cref=           #addAdjustmentListener </seealso>
		''' <seealso cref=           #removeAdjustmentListener </seealso>
		''' <seealso cref=           java.awt.event.AdjustmentListener </seealso>
		''' <seealso cref=           java.awt.event.AdjustmentEvent
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property adjustmentListeners As java.awt.event.AdjustmentListener()
			Get
				Return CType(AWTEventMulticaster.getListeners(adjustmentListener, GetType(java.awt.event.AdjustmentListener)), java.awt.event.AdjustmentListener())
			End Get
		End Property

		''' <summary>
		''' Returns a string representation of this scrollbar and its values. </summary>
		''' <returns>    a string representation of this scrollbar. </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[" & paramString() & "]"
		End Function

		''' <summary>
		''' Returns a string representing the state of this scrollbar.
		''' This method is intended to be used only for debugging purposes,
		''' and the content and format of the returned string may vary
		''' between implementations.  The returned string may be empty but
		''' may not be <code>null</code>.
		''' </summary>
		''' <returns>      the parameter string of this scrollbar. </returns>
		Public Overridable Function paramString() As String
			Return ((If(orientation = Adjustable.VERTICAL, "vertical,", "horizontal,")) & "[0.." & maximum & "]" & ",val=" & value & ",vis=" & visibleAmount & ",unit=" & unitIncrement & ",block=" & blockIncrement & ",isAdjusting=" & isAdjusting)
		End Function
	End Class

End Namespace
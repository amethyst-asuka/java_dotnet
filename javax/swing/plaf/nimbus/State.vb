Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.nimbus


	''' <summary>
	''' <p>Represents a built in, or custom, state in Nimbus.</p>
	''' 
	''' <p>Synth provides several built in states, which are:
	''' <ul>
	'''  <li>Enabled</li>
	'''  <li>Mouse Over</li>
	'''  <li>Pressed</li>
	'''  <li>Disabled</li>
	'''  <li>Focused</li>
	'''  <li>Selected</li>
	'''  <li>Default</li>
	''' </ul>
	''' 
	''' <p>However, there are many more states that could be described in a LookAndFeel, and it
	''' would be nice to style components differently based on these different states.
	''' For example, a progress bar could be "indeterminate". It would be very convenient
	''' to allow this to be defined as a "state".</p>
	''' 
	''' <p>This class, State, is intended to be used for such situations.
	''' Simply implement the abstract #isInState method. It returns true if the given
	''' JComponent is "in this state", false otherwise. This method will be called
	''' <em>many</em> times in <em>performance sensitive loops</em>. It must execute
	''' very quickly.</p>
	''' 
	''' <p>For example, the following might be an implementation of a custom
	''' "Indeterminate" state for JProgressBars:</p>
	''' 
	''' <pre><code>
	'''     public final class IndeterminateState extends State&lt;JProgressBar&gt; {
	'''         public IndeterminateState() {
	'''             super("Indeterminate");
	'''         }
	''' 
	'''         &#64;Override
	'''         protected boolean isInState(JProgressBar c) {
	'''             return c.isIndeterminate();
	'''         }
	'''     }
	''' </code></pre>
	''' </summary>
	Public MustInherit Class State(Of T As javax.swing.JComponent)
		Friend Shared ReadOnly standardStates As IDictionary(Of String, StandardState) = New Dictionary(Of String, StandardState)(7)
		Friend Shared ReadOnly Enabled As State = New StandardState(javax.swing.plaf.synth.SynthConstants.ENABLED)
		Friend Shared ReadOnly MouseOver As State = New StandardState(javax.swing.plaf.synth.SynthConstants.MOUSE_OVER)
		Friend Shared ReadOnly Pressed As State = New StandardState(javax.swing.plaf.synth.SynthConstants.PRESSED)
		Friend Shared ReadOnly Disabled As State = New StandardState(javax.swing.plaf.synth.SynthConstants.DISABLED)
		Friend Shared ReadOnly Focused As State = New StandardState(javax.swing.plaf.synth.SynthConstants.FOCUSED)
		Friend Shared ReadOnly Selected As State = New StandardState(javax.swing.plaf.synth.SynthConstants.SELECTED)
		Friend Shared ReadOnly [Default] As State = New StandardState(javax.swing.plaf.synth.SynthConstants.DEFAULT)

		Private name As String

		''' <summary>
		''' <p>Create a new custom State. Specify the name for the state. The name should
		''' be unique within the states set for any one particular component.
		''' The name of the state should coincide with the name used in UIDefaults.</p>
		''' 
		''' <p>For example, the following would be correct:</p>
		''' <pre><code>
		'''     defaults.put("Button.States", "Enabled, Foo, Disabled");
		'''     defaults.put("Button.Foo", new FooState("Foo"));
		''' </code></pre>
		''' </summary>
		''' <param name="name"> a simple user friendly name for the state, such as "Indeterminate"
		'''        or "EmbeddedPanel" or "Blurred". It is customary to use camel case,
		'''        with the first letter capitalized. </param>
		Protected Friend Sub New(ByVal name As String)
			Me.name = name
		End Sub

		Public Overrides Function ToString() As String
			Return name
		End Function

		''' <summary>
		''' <p>This is the main entry point, called by NimbusStyle.</p>
		''' 
		''' <p>There are both custom states and standard states. Standard states
		''' correlate to the states defined in SynthConstants. When a UI delegate
		''' constructs a SynthContext, it specifies the state that the component is
		''' in according to the states defined in SynthConstants. Our NimbusStyle
		''' will then take this state, and query each State instance in the style
		''' asking whether isInState(c, s).</p>
		''' 
		''' <p>Now, only the standard states care about the "s" param. So we have
		''' this odd arrangement:</p>
		''' <ul>
		'''     <li>NimbusStyle calls State.isInState(c, s)</li>
		'''     <li>State.isInState(c, s) simply delegates to State.isInState(c)</li>
		'''     <li><em>EXCEPT</em>, StandardState overrides State.isInState(c, s) and
		'''         returns directly from that method after checking its state, and
		'''         does not call isInState(c) (since it is not needed for standard states).</li>
		''' </ul>
		''' </summary>
		Friend Overridable Function isInState(ByVal c As T, ByVal s As Integer) As Boolean
			Return isInState(c)
		End Function

		''' <summary>
		''' <p>Gets whether the specified JComponent is in the custom state represented
		''' by this class. <em>This is an extremely performance sensitive loop.</em>
		''' Please take proper precautions to ensure that it executes quickly.</p>
		''' 
		''' <p>Nimbus uses this method to help determine what state a JComponent is
		''' in. For example, a custom State could exist for JProgressBar such that
		''' it would return <code>true</code> when the progress bar is indeterminate.
		''' Such an implementation of this method would simply be:</p>
		''' 
		''' <pre><code> return c.isIndeterminate();</code></pre>
		''' </summary>
		''' <param name="c"> the JComponent to test. This will never be null. </param>
		''' <returns> true if <code>c</code> is in the custom state represented by
		'''         this <code>State</code> instance </returns>
		Protected Friend MustOverride Function isInState(ByVal c As T) As Boolean

		Friend Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		Friend Shared Function isStandardStateName(ByVal name As String) As Boolean
			Return standardStates.ContainsKey(name)
		End Function

		Friend Shared Function getStandardState(ByVal name As String) As StandardState
			Return standardStates(name)
		End Function

		Friend NotInheritable Class StandardState
			Inherits State(Of javax.swing.JComponent)

			Private ___state As Integer

			Private Sub New(ByVal ___state As Integer)
				MyBase.New(ToString(___state))
				Me.___state = ___state
				standardStates(name) = Me
			End Sub

			Public Property state As Integer
				Get
					Return ___state
				End Get
			End Property

			Friend Overrides Function isInState(ByVal c As javax.swing.JComponent, ByVal s As Integer) As Boolean
				Return (s And ___state) = ___state
			End Function

			Protected Friend Overrides Function isInState(ByVal c As javax.swing.JComponent) As Boolean
				Throw New AssertionError("This method should never be called")
			End Function

			Private Shared Function ToString(ByVal ___state As Integer) As String
				Dim buffer As New StringBuilder
				If (___state And javax.swing.plaf.synth.SynthConstants.DEFAULT) = javax.swing.plaf.synth.SynthConstants.DEFAULT Then buffer.Append("Default")
				If (___state And javax.swing.plaf.synth.SynthConstants.DISABLED) = javax.swing.plaf.synth.SynthConstants.DISABLED Then
					If buffer.Length > 0 Then buffer.Append("+")
					buffer.Append("Disabled")
				End If
				If (___state And javax.swing.plaf.synth.SynthConstants.ENABLED) = javax.swing.plaf.synth.SynthConstants.ENABLED Then
					If buffer.Length > 0 Then buffer.Append("+")
					buffer.Append("Enabled")
				End If
				If (___state And javax.swing.plaf.synth.SynthConstants.FOCUSED) = javax.swing.plaf.synth.SynthConstants.FOCUSED Then
					If buffer.Length > 0 Then buffer.Append("+")
					buffer.Append("Focused")
				End If
				If (___state And javax.swing.plaf.synth.SynthConstants.MOUSE_OVER) = javax.swing.plaf.synth.SynthConstants.MOUSE_OVER Then
					If buffer.Length > 0 Then buffer.Append("+")
					buffer.Append("MouseOver")
				End If
				If (___state And javax.swing.plaf.synth.SynthConstants.PRESSED) = javax.swing.plaf.synth.SynthConstants.PRESSED Then
					If buffer.Length > 0 Then buffer.Append("+")
					buffer.Append("Pressed")
				End If
				If (___state And javax.swing.plaf.synth.SynthConstants.SELECTED) = javax.swing.plaf.synth.SynthConstants.SELECTED Then
					If buffer.Length > 0 Then buffer.Append("+")
					buffer.Append("Selected")
				End If
				Return buffer.ToString()
			End Function
		End Class
	End Class

End Namespace
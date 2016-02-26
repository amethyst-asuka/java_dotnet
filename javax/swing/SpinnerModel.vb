Imports javax.swing.event

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
	''' A model for a potentially unbounded sequence of object values.  This model
	''' is similar to <code>ListModel</code> however there are some important differences:
	''' <ul>
	''' <li> The number of sequence elements isn't necessarily bounded.
	''' <li> The model doesn't support indexed random access to sequence elements.
	'''      Only three sequence values are accessible at a time: current, next and
	'''      previous.
	''' <li> The current sequence element, can be set.
	''' </ul>
	''' <p>
	''' A <code>SpinnerModel</code> has three properties, only the first is read/write.
	''' <dl>
	'''   <dt><code>value</code>
	'''   <dd>The current element of the sequence.
	''' 
	'''   <dt><code>nextValue</code>
	'''   <dd>The following element or null if <code>value</code> is the
	'''     last element of the sequence.
	''' 
	'''   <dt><code>previousValue</code>
	'''   <dd>The preceding element or null if <code>value</code> is the
	'''     first element of the sequence.
	''' </dl>
	''' When the the <code>value</code> property changes,
	''' <code>ChangeListeners</code> are notified.  <code>SpinnerModel</code> may
	''' choose to notify the <code>ChangeListeners</code> under other circumstances.
	''' </summary>
	''' <seealso cref= JSpinner </seealso>
	''' <seealso cref= AbstractSpinnerModel </seealso>
	''' <seealso cref= SpinnerListModel </seealso>
	''' <seealso cref= SpinnerNumberModel </seealso>
	''' <seealso cref= SpinnerDateModel
	''' 
	''' @author Hans Muller
	''' @since 1.4 </seealso>
	Public Interface SpinnerModel
		''' <summary>
		''' The <i>current element</i> of the sequence.  This element is usually
		''' displayed by the <code>editor</code> part of a <code>JSpinner</code>.
		''' </summary>
		''' <returns> the current spinner value. </returns>
		''' <seealso cref= #setValue </seealso>
		Property value As Object




		''' <summary>
		''' Return the object in the sequence that comes after the object returned
		''' by <code>getValue()</code>. If the end of the sequence has been reached
		''' then return null.  Calling this method does not effect <code>value</code>.
		''' </summary>
		''' <returns> the next legal value or null if one doesn't exist </returns>
		''' <seealso cref= #getValue </seealso>
		''' <seealso cref= #getPreviousValue </seealso>
		ReadOnly Property nextValue As Object


		''' <summary>
		''' Return the object in the sequence that comes before the object returned
		''' by <code>getValue()</code>.  If the end of the sequence has been reached then
		''' return null. Calling this method does not effect <code>value</code>.
		''' </summary>
		''' <returns> the previous legal value or null if one doesn't exist </returns>
		''' <seealso cref= #getValue </seealso>
		''' <seealso cref= #getNextValue </seealso>
		ReadOnly Property previousValue As Object


		''' <summary>
		''' Adds a <code>ChangeListener</code> to the model's listener list.  The
		''' <code>ChangeListeners</code> must be notified when models <code>value</code>
		''' changes.
		''' </summary>
		''' <param name="l"> the ChangeListener to add </param>
		''' <seealso cref= #removeChangeListener </seealso>
		Sub addChangeListener(ByVal l As ChangeListener)


		''' <summary>
		''' Removes a <code>ChangeListener</code> from the model's listener list.
		''' </summary>
		''' <param name="l"> the ChangeListener to remove </param>
		''' <seealso cref= #addChangeListener </seealso>
		Sub removeChangeListener(ByVal l As ChangeListener)
	End Interface

End Namespace
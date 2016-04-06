'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The interface for objects which have an adjustable numeric value
	''' contained within a bounded range of values.
	''' 
	''' @author Amy Fowler
	''' @author Tim Prinzing
	''' </summary>
	Public Interface Adjustable

		''' <summary>
		''' Indicates that the <code>Adjustable</code> has horizontal orientation.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int HORIZONTAL = 0;

		''' <summary>
		''' Indicates that the <code>Adjustable</code> has vertical orientation.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int VERTICAL = 1;

		''' <summary>
		''' Indicates that the <code>Adjustable</code> has no orientation.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int NO_ORIENTATION = 2;

		''' <summary>
		''' Gets the orientation of the adjustable object. </summary>
		''' <returns> the orientation of the adjustable object;
		'''   either <code>HORIZONTAL</code>, <code>VERTICAL</code>,
		'''   or <code>NO_ORIENTATION</code> </returns>
		ReadOnly Property orientation As Integer

		''' <summary>
		''' Sets the minimum value of the adjustable object. </summary>
		''' <param name="min"> the minimum value </param>
		Property minimum As Integer


		''' <summary>
		''' Sets the maximum value of the adjustable object. </summary>
		''' <param name="max"> the maximum value </param>
		Property maximum As Integer


		''' <summary>
		''' Sets the unit value increment for the adjustable object. </summary>
		''' <param name="u"> the unit increment </param>
		Property unitIncrement As Integer


		''' <summary>
		''' Sets the block value increment for the adjustable object. </summary>
		''' <param name="b"> the block increment </param>
		Property blockIncrement As Integer


		''' <summary>
		''' Sets the length of the proportional indicator of the
		''' adjustable object. </summary>
		''' <param name="v"> the length of the indicator </param>
		Property visibleAmount As Integer


		''' <summary>
		''' Sets the current value of the adjustable object. If
		''' the value supplied is less than <code>minimum</code>
		''' or greater than <code>maximum</code> - <code>visibleAmount</code>,
		''' then one of those values is substituted, as appropriate.
		''' <p>
		''' Calling this method does not fire an
		''' <code>AdjustmentEvent</code>.
		''' </summary>
		''' <param name="v"> the current value, between <code>minimum</code>
		'''    and <code>maximum</code> - <code>visibleAmount</code> </param>
		Property value As Integer


		''' <summary>
		''' Adds a listener to receive adjustment events when the value of
		''' the adjustable object changes. </summary>
		''' <param name="l"> the listener to receive events </param>
		''' <seealso cref= AdjustmentEvent </seealso>
		Sub addAdjustmentListener(  l As AdjustmentListener)

		''' <summary>
		''' Removes an adjustment listener. </summary>
		''' <param name="l"> the listener being removed </param>
		''' <seealso cref= AdjustmentEvent </seealso>
		Sub removeAdjustmentListener(  l As AdjustmentListener)

	End Interface

End Namespace
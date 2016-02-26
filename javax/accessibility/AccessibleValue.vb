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

Namespace javax.accessibility

	''' <summary>
	''' The AccessibleValue interface should be supported by any object
	''' that supports a numerical value (e.g., a scroll bar).  This interface
	''' provides the standard mechanism for an assistive technology to determine
	''' and set the numerical value as well as get the minimum and maximum values.
	''' Applications can determine
	''' if an object supports the AccessibleValue interface by first
	''' obtaining its AccessibleContext (see
	''' <seealso cref="Accessible"/>) and then calling the
	''' <seealso cref="AccessibleContext#getAccessibleValue"/> method.
	''' If the return value is not null, the object supports this interface.
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleValue
	''' 
	''' @author      Peter Korn
	''' @author      Hans Muller
	''' @author      Willie Walker </seealso>
	Public Interface AccessibleValue

		''' <summary>
		''' Get the value of this object as a Number.  If the value has not been
		''' set, the return value will be null.
		''' </summary>
		''' <returns> value of the object </returns>
		''' <seealso cref= #setCurrentAccessibleValue </seealso>
		ReadOnly Property currentAccessibleValue As Number

		''' <summary>
		''' Set the value of this object as a Number.
		''' </summary>
		''' <param name="n"> the number to use for the value </param>
		''' <returns> True if the value was set; else False </returns>
		''' <seealso cref= #getCurrentAccessibleValue </seealso>
		Function setCurrentAccessibleValue(ByVal n As Number) As Boolean

	'    /**
	'     * Get the description of the value of this object.
	'     *
	'     * @return description of the value of the object
	'     */
	'    public String getAccessibleValueDescription();

		''' <summary>
		''' Get the minimum value of this object as a Number.
		''' </summary>
		''' <returns> Minimum value of the object; null if this object does not
		''' have a minimum value </returns>
		''' <seealso cref= #getMaximumAccessibleValue </seealso>
		ReadOnly Property minimumAccessibleValue As Number

		''' <summary>
		''' Get the maximum value of this object as a Number.
		''' </summary>
		''' <returns> Maximum value of the object; null if this object does not
		''' have a maximum value </returns>
		''' <seealso cref= #getMinimumAccessibleValue </seealso>
		ReadOnly Property maximumAccessibleValue As Number
	End Interface

End Namespace
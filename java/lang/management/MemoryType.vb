'
' * Copyright (c) 2003, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.management

	''' <summary>
	''' Types of <seealso cref="MemoryPoolMXBean memory pools"/>.
	''' 
	''' @author  Mandy Chung
	''' @since   1.5
	''' </summary>
	Public Enum MemoryType

		''' <summary>
		''' Heap memory type.
		''' <p>
		''' The Java virtual machine has a <i>heap</i>
		''' that is the runtime data area from which
		''' memory for all class instances and arrays are allocated.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		HEAP("Heap memory"),

		''' <summary>
		''' Non-heap memory type.
		''' <p>
		''' The Java virtual machine manages memory other than the heap
		''' (referred as <i>non-heap memory</i>).  The non-heap memory includes
		''' the <i>method area</i> and memory required for the internal
		''' processing or optimization for the Java virtual machine.
		''' It stores per-class structures such as a runtime
		''' constant pool, field and method data, and the code for
		''' methods and constructors.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_HEAP("Non-heap memory");

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final String description;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private MemoryType(String s)
	'	{
	'		Me.description = s;
	'	}

		''' <summary>
		''' Returns the string representation of this <tt>MemoryType</tt>. </summary>
		''' <returns> the string representation of this <tt>MemoryType</tt>. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public String toString()
	'	{
	'		Return description;
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static final long serialVersionUID = 6992337162326171013L;
	End Enum

End Namespace
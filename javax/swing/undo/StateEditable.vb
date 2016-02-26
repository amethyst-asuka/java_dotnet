Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.undo



	''' <summary>
	''' StateEditable defines the interface for objects that can have
	''' their state undone/redone by a StateEdit.
	''' </summary>
	''' <seealso cref= StateEdit </seealso>

	Public Interface StateEditable

		''' <summary>
		''' Resource ID for this class. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String RCSID = "$Id: StateEditable.java,v 1.2 1997/09/08 19:39:08 marklin Exp $";

		''' <summary>
		''' Upon receiving this message the receiver should place any relevant
		''' state into <EM>state</EM>.
		''' </summary>
		Sub storeState(ByVal state As Dictionary(Of Object, Object))

		''' <summary>
		''' Upon receiving this message the receiver should extract any relevant
		''' state out of <EM>state</EM>.
		''' </summary>
		Sub restoreState(Of T1)(ByVal state As Dictionary(Of T1))
	End Interface ' End of interface StateEditable

End Namespace
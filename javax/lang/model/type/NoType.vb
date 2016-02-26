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

Namespace javax.lang.model.type



	''' <summary>
	''' A pseudo-type used where no actual type is appropriate.
	''' The kinds of {@code NoType} are:
	''' <ul>
	''' <li><seealso cref="TypeKind#VOID VOID"/> - corresponds to the keyword {@code void}.
	''' <li><seealso cref="TypeKind#PACKAGE PACKAGE"/> - the pseudo-type of a package element.
	''' <li><seealso cref="TypeKind#NONE NONE"/> - used in other cases
	'''   where no actual type is appropriate; for example, the superclass
	'''   of {@code java.lang.Object}.
	''' </ul>
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= ExecutableElement#getReturnType()
	''' @since 1.6 </seealso>

	Public Interface NoType
		Inherits TypeMirror

	End Interface

End Namespace
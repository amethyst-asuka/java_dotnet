Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.lang.model.element


	''' <summary>
	''' Represents a formal type parameter of a generic class, interface, method,
	''' or constructor element.
	''' A type parameter declares a <seealso cref="TypeVariable"/>.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= TypeVariable
	''' @since 1.6 </seealso>
	Public Interface TypeParameterElement
		Inherits Element

		''' <summary>
		''' Returns the generic class, interface, method, or constructor that is
		''' parameterized by this type parameter.
		''' </summary>
		''' <returns> the generic class, interface, method, or constructor that is
		''' parameterized by this type parameter </returns>
		ReadOnly Property genericElement As Element

		''' <summary>
		''' Returns the bounds of this type parameter.
		''' These are the types given by the {@code extends} clause
		''' used to declare this type parameter.
		''' If no explicit {@code extends} clause was used,
		''' then {@code java.lang.Object} is considered to be the sole bound.
		''' </summary>
		''' <returns> the bounds of this type parameter, or an empty list if
		''' there are none </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property bounds As IList(Of ? As javax.lang.model.type.TypeMirror)

		''' <summary>
		''' Returns the <seealso cref="TypeParameterElement#getGenericElement generic element"/> of this type parameter.
		''' </summary>
		''' <returns> the generic element of this type parameter </returns>
		ReadOnly Property Overrides enclosingElement As Element
	End Interface

End Namespace
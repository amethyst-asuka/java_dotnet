'
' * Copyright (c) 1999, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming

	''' <summary>
	''' This interface is used for parsing names from a hierarchical
	''' namespace.  The NameParser contains knowledge of the syntactic
	''' information (like left-to-right orientation, name separator, etc.)
	''' needed to parse names.
	'''  
	''' The equals() method, when used to compare two NameParsers, returns
	''' true if and only if they serve the same namespace.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= CompoundName </seealso>
	''' <seealso cref= Name
	''' @since 1.3 </seealso>

	Public Interface NameParser
			''' <summary>
			''' Parses a name into its components.
			''' </summary>
			''' <param name="name"> The non-null string name to parse. </param>
			''' <returns> A non-null parsed form of the name using the naming convention
			''' of this parser. </returns>
			''' <exception cref="InvalidNameException"> If name does not conform to
			'''     syntax defined for the namespace. </exception>
			''' <exception cref="NamingException"> If a naming exception was encountered. </exception>
			Function parse(ByVal name As String) As Name
	End Interface

End Namespace
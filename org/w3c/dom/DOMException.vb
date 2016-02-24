Imports System

'
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

'
' *
' *
' *
' *
' *
' * Copyright (c) 2004 World Wide Web Consortium,
' *
' * (Massachusetts Institute of Technology, European Research Consortium for
' * Informatics and Mathematics, Keio University). All Rights Reserved. This
' * work is distributed under the W3C(r) Software License [1] in the hope that
' * it will be useful, but WITHOUT ANY WARRANTY; without even the implied
' * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
' *
' * [1] http://www.w3.org/Consortium/Legal/2002/copyright-software-20021231
' 

Namespace org.w3c.dom

	''' <summary>
	''' DOM operations only raise exceptions in "exceptional" circumstances, i.e.,
	''' when an operation is impossible to perform (either for logical reasons,
	''' because data is lost, or because the implementation has become unstable).
	''' In general, DOM methods return specific error values in ordinary
	''' processing situations, such as out-of-bound errors when using
	''' <code>NodeList</code>.
	''' <p>Implementations should raise other exceptions under other circumstances.
	''' For example, implementations should raise an implementation-dependent
	''' exception if a <code>null</code> argument is passed when <code>null</code>
	'''  was not expected.
	''' <p>Some languages and object systems do not support the concept of
	''' exceptions. For such systems, error conditions may be indicated using
	''' native error reporting mechanisms. For some bindings, for example,
	''' methods may return error codes similar to those listed in the
	''' corresponding method descriptions.
	''' <p>See also the <a href='http://www.w3.org/TR/2004/REC-DOM-Level-3-Core-20040407'>Document Object Model (DOM) Level 3 Core Specification</a>.
	''' </summary>
	Public Class DOMException
		Inherits Exception

		Public Sub New(ByVal code As Short, ByVal message As String)
		   MyBase.New(message)
		   Me.code = code
		End Sub
		Public code As Short
		' ExceptionCode
		''' <summary>
		''' If index or size is negative, or greater than the allowed value.
		''' </summary>
		Public Const INDEX_SIZE_ERR As Short = 1
		''' <summary>
		''' If the specified range of text does not fit into a
		''' <code>DOMString</code>.
		''' </summary>
		Public Const DOMSTRING_SIZE_ERR As Short = 2
		''' <summary>
		''' If any <code>Node</code> is inserted somewhere it doesn't belong.
		''' </summary>
		Public Const HIERARCHY_REQUEST_ERR As Short = 3
		''' <summary>
		''' If a <code>Node</code> is used in a different document than the one
		''' that created it (that doesn't support it).
		''' </summary>
		Public Const WRONG_DOCUMENT_ERR As Short = 4
		''' <summary>
		''' If an invalid or illegal character is specified, such as in an XML name.
		''' </summary>
		Public Const INVALID_CHARACTER_ERR As Short = 5
		''' <summary>
		''' If data is specified for a <code>Node</code> which does not support
		''' data.
		''' </summary>
		Public Const NO_DATA_ALLOWED_ERR As Short = 6
		''' <summary>
		''' If an attempt is made to modify an object where modifications are not
		''' allowed.
		''' </summary>
		Public Const NO_MODIFICATION_ALLOWED_ERR As Short = 7
		''' <summary>
		''' If an attempt is made to reference a <code>Node</code> in a context
		''' where it does not exist.
		''' </summary>
		Public Const NOT_FOUND_ERR As Short = 8
		''' <summary>
		''' If the implementation does not support the requested type of object or
		''' operation.
		''' </summary>
		Public Const NOT_SUPPORTED_ERR As Short = 9
		''' <summary>
		''' If an attempt is made to add an attribute that is already in use
		''' elsewhere.
		''' </summary>
		Public Const INUSE_ATTRIBUTE_ERR As Short = 10
		''' <summary>
		''' If an attempt is made to use an object that is not, or is no longer,
		''' usable.
		''' @since DOM Level 2
		''' </summary>
		Public Const INVALID_STATE_ERR As Short = 11
		''' <summary>
		''' If an invalid or illegal string is specified.
		''' @since DOM Level 2
		''' </summary>
		Public Const SYNTAX_ERR As Short = 12
		''' <summary>
		''' If an attempt is made to modify the type of the underlying object.
		''' @since DOM Level 2
		''' </summary>
		Public Const INVALID_MODIFICATION_ERR As Short = 13
		''' <summary>
		''' If an attempt is made to create or change an object in a way which is
		''' incorrect with regard to namespaces.
		''' @since DOM Level 2
		''' </summary>
		Public Const NAMESPACE_ERR As Short = 14
		''' <summary>
		''' If a parameter or an operation is not supported by the underlying
		''' object.
		''' @since DOM Level 2
		''' </summary>
		Public Const INVALID_ACCESS_ERR As Short = 15
		''' <summary>
		''' If a call to a method such as <code>insertBefore</code> or
		''' <code>removeChild</code> would make the <code>Node</code> invalid
		''' with respect to "partial validity", this exception would be raised
		''' and the operation would not be done. This code is used in [<a href='http://www.w3.org/TR/2004/REC-DOM-Level-3-Val-20040127/'>DOM Level 3 Validation</a>]
		''' . Refer to this specification for further information.
		''' @since DOM Level 3
		''' </summary>
		Public Const VALIDATION_ERR As Short = 16
		''' <summary>
		'''  If the type of an object is incompatible with the expected type of the
		''' parameter associated to the object.
		''' @since DOM Level 3
		''' </summary>
		Public Const TYPE_MISMATCH_ERR As Short = 17

		' Added serialVersionUID to preserve binary compatibility
		Friend Const serialVersionUID As Long = 6627732366795969916L
	End Class

End Namespace
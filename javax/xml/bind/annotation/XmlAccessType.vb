'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind.annotation



	''' <summary>
	''' Used by XmlAccessorType to control serialization of fields or
	''' properties.
	''' 
	''' @author Sekhar Vajjhala, Sun Microsystems, Inc.
	''' @since JAXB2.0 </summary>
	''' <seealso cref= XmlAccessorType </seealso>

	Public Enum XmlAccessType
		''' <summary>
		''' Every getter/setter pair in a JAXB-bound class will be automatically
		''' bound to XML, unless annotated by <seealso cref="XmlTransient"/>.
		''' 
		''' Fields are bound to XML only when they are explicitly annotated
		''' by some of the JAXB annotations.
		''' </summary>
		[PROPERTY]
		''' <summary>
		''' Every non static, non transient field in a JAXB-bound class will be automatically
		''' bound to XML, unless annotated by <seealso cref="XmlTransient"/>.
		''' 
		''' Getter/setter pairs are bound to XML only when they are explicitly annotated
		''' by some of the JAXB annotations.
		''' </summary>
		FIELD
		''' <summary>
		''' Every public getter/setter pair and every public field will be
		''' automatically bound to XML, unless annotated by <seealso cref="XmlTransient"/>.
		''' 
		''' Fields or getter/setter pairs that are private, protected, or
		''' defaulted to package-only access are bound to XML only when they are
		''' explicitly annotated by the appropriate JAXB annotations.
		''' </summary>
		PUBLIC_MEMBER
		''' <summary>
		''' None of the fields or properties is bound to XML unless they
		''' are specifically  annotated with some of the JAXB annotations.
		''' </summary>
		NONE
	End Enum

End Namespace
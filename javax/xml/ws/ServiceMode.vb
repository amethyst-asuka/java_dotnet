'
' * Copyright (c) 2005, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws


	''' <summary>
	''' Used to indicate whether a <seealso cref="Provider"/> implementation wishes to work
	''' with entire protocol messages or just with protocol message payloads.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := False, Inherited := True> _
	Public Class ServiceMode
		Inherits System.Attribute

	  ''' <summary>
	  ''' Service mode. <code>PAYLOAD</code> indicates that the <code>Provider</code> implementation
	  ''' wishes to work with protocol message payloads only. <code>MESSAGE</code> indicates
	  ''' that the <code>Provider</code> implementation wishes to work with entire protocol
	  ''' messages.
	  ''' 
	  ''' </summary>
	  public Service.Mode value() default Service.Mode.PAYLOAD
	End Class

End Namespace
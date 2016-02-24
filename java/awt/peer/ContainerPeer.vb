'
' * Copyright (c) 1995, 2005, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt.peer


	''' <summary>
	''' The peer interface for <seealso cref="Container"/>. This is the parent interface
	''' for all container like widgets.
	''' 
	''' The peer interfaces are intended only for use in porting
	''' the AWT. They are not intended for use by application
	''' developers, and developers should not implement peers
	''' nor invoke any of the peer methods directly on the peer
	''' instances.
	''' </summary>
	Public Interface ContainerPeer
		Inherits ComponentPeer

		''' <summary>
		''' Returns the insets of this container. Insets usually is the space that
		''' is occupied by things like borders.
		''' </summary>
		''' <returns> the insets of this container </returns>
		ReadOnly Property insets As Insets

		''' <summary>
		''' Notifies the peer that validation of the component tree is about to
		''' begin.
		''' </summary>
		''' <seealso cref= Container#validate() </seealso>
		Sub beginValidate()

		''' <summary>
		''' Notifies the peer that validation of the component tree is finished.
		''' </summary>
		''' <seealso cref= Container#validate() </seealso>
		Sub endValidate()

		''' <summary>
		''' Notifies the peer that layout is about to begin. This is called
		''' before the container itself and its children are laid out.
		''' </summary>
		''' <seealso cref= Container#validateTree() </seealso>
		Sub beginLayout()

		''' <summary>
		''' Notifies the peer that layout is finished. This is called after the
		''' container and its children have been laid out.
		''' </summary>
		''' <seealso cref= Container#validateTree() </seealso>
		Sub endLayout()
	End Interface

End Namespace
'
' * Copyright (c) 1999, 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt



	Friend MustInherit Class GraphicsCallback
		Inherits sun.awt.SunGraphicsCallback

		Friend NotInheritable Class PaintCallback
			Inherits GraphicsCallback

            Private Shared _instance As New PaintCallback

            Private Sub New()
			End Sub
			Public Sub run(ByVal comp As Component, ByVal cg As Graphics)
				comp.paint(cg)
			End Sub
            Public Shared ReadOnly Property instance As PaintCallback
                Get
                    Return _instance
                End Get
            End Property
        End Class
		Friend NotInheritable Class PrintCallback
			Inherits GraphicsCallback

            Private Shared _instance As New PrintCallback

            Private Sub New()
			End Sub
			Public Sub run(ByVal comp As Component, ByVal cg As Graphics)
				comp.print(cg)
			End Sub
            Public Shared ReadOnly Property instance As PrintCallback
                Get
                    Return _instance
                End Get
            End Property
        End Class
		Friend NotInheritable Class PaintAllCallback
			Inherits GraphicsCallback

            Private Shared _instance As New PaintAllCallback

            Private Sub New()
			End Sub
			Public Sub run(ByVal comp As Component, ByVal cg As Graphics)
				comp.paintAll(cg)
			End Sub
            Public Shared ReadOnly Property instance As PaintAllCallback
                Get
                    Return _instance
                End Get
            End Property
        End Class
		Friend NotInheritable Class PrintAllCallback
			Inherits GraphicsCallback

            Private Shared _instance As New PrintAllCallback

            Private Sub New()
			End Sub
			Public Sub run(ByVal comp As Component, ByVal cg As Graphics)
				comp.printAll(cg)
			End Sub
            Public Shared ReadOnly Property instance As PrintAllCallback
                Get
                    Return _instance
                End Get
            End Property
        End Class
		Friend NotInheritable Class PeerPaintCallback
			Inherits GraphicsCallback

            Private Shared _instance As New PeerPaintCallback

            Private Sub New()
			End Sub
			Public Sub run(ByVal comp As Component, ByVal cg As Graphics)
				comp.validate()
				If TypeOf comp.peer Is java.awt.peer.LightweightPeer Then
					comp.lightweightPaint(cg)
				Else
					comp.peer.paint(cg)
				End If
			End Sub
            Public Shared ReadOnly Property instance As PeerPaintCallback
                Get
                    Return _instance
                End Get
            End Property
        End Class
		Friend NotInheritable Class PeerPrintCallback
			Inherits GraphicsCallback

            Private Shared _instance As New PeerPrintCallback

            Private Sub New()
			End Sub
			Public Sub run(ByVal comp As Component, ByVal cg As Graphics)
				comp.validate()
				If TypeOf comp.peer Is java.awt.peer.LightweightPeer Then
					comp.lightweightPrint(cg)
				Else
					comp.peer.print(cg)
				End If
			End Sub
            Public Shared ReadOnly Property instance As PeerPrintCallback
                Get
                    Return _instance
                End Get
            End Property
        End Class
		Friend NotInheritable Class PaintHeavyweightComponentsCallback
			Inherits GraphicsCallback

            Private Shared _instance As New PaintHeavyweightComponentsCallback

            Private Sub New()
			End Sub
			Public Sub run(ByVal comp As Component, ByVal cg As Graphics)
				If TypeOf comp.peer Is java.awt.peer.LightweightPeer Then
					comp.paintHeavyweightComponents(cg)
				Else
					comp.paintAll(cg)
				End If
			End Sub
            Public Shared ReadOnly Property instance As PaintHeavyweightComponentsCallback
                Get
                    Return _instance
                End Get
            End Property
        End Class
		Friend NotInheritable Class PrintHeavyweightComponentsCallback
			Inherits GraphicsCallback

            Private Shared _instance As New PrintHeavyweightComponentsCallback

            Private Sub New()
			End Sub
			Public Sub run(ByVal comp As Component, ByVal cg As Graphics)
				If TypeOf comp.peer Is java.awt.peer.LightweightPeer Then
					comp.printHeavyweightComponents(cg)
				Else
					comp.printAll(cg)
				End If
			End Sub
            Public Shared ReadOnly Property instance As PrintHeavyweightComponentsCallback
                Get
                    Return _instance
                End Get
            End Property
        End Class
	End Class

End Namespace
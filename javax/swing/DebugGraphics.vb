Imports System
Imports System.Threading

'
' * Copyright (c) 1997, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing


	''' <summary>
	''' Graphics subclass supporting graphics debugging. Overrides most methods
	''' from Graphics.  DebugGraphics objects are rarely created by hand.  They
	''' are most frequently created automatically when a JComponent's
	''' debugGraphicsOptions are changed using the setDebugGraphicsOptions()
	''' method.
	''' <p>
	''' NOTE: You must turn off double buffering to use DebugGraphics:
	'''       RepaintManager repaintManager = RepaintManager.currentManager(component);
	'''       repaintManager.setDoubleBufferingEnabled(false);
	''' </summary>
	''' <seealso cref= JComponent#setDebugGraphicsOptions </seealso>
	''' <seealso cref= RepaintManager#currentManager </seealso>
	''' <seealso cref= RepaintManager#setDoubleBufferingEnabled
	''' 
	''' @author Dave Karlton </seealso>
	Public Class DebugGraphics
		Inherits Graphics

		Friend graphics As Graphics
		Friend buffer As Image
		Friend debugOptions As Integer
		Friend graphicsID As Integer = graphicsCount
		Friend graphicsCount += 1
		Friend xOffset, yOffset As Integer
		Private Shared graphicsCount As Integer = 0
		Private Shared imageLoadingIcon As New ImageIcon

		''' <summary>
		''' Log graphics operations. </summary>
		Public Shared ReadOnly LOG_OPTION As Integer = 1 << 0
		''' <summary>
		''' Flash graphics operations. </summary>
		Public Shared ReadOnly FLASH_OPTION As Integer = 1 << 1
		''' <summary>
		''' Show buffered operations in a separate <code>Frame</code>. </summary>
		Public Shared ReadOnly BUFFERED_OPTION As Integer = 1 << 2
		''' <summary>
		''' Don't debug graphics operations. </summary>
		Public Const NONE_OPTION As Integer = -1

		Shared Sub New()
			JComponent.DEBUG_GRAPHICS_LOADED = True
		End Sub

		''' <summary>
		''' Constructs a new debug graphics context that supports slowed
		''' down drawing.
		''' </summary>
		Public Sub New()
			MyBase.New()
			buffer = Nothing
				yOffset = 0
				xOffset = yOffset
		End Sub

		''' <summary>
		''' Constructs a debug graphics context from an existing graphics
		''' context that slows down drawing for the specified component.
		''' </summary>
		''' <param name="graphics">  the Graphics context to slow down </param>
		''' <param name="component"> the JComponent to draw slowly </param>
		Public Sub New(ByVal graphics As Graphics, ByVal component As JComponent)
			Me.New(graphics)
			debugOptions = component.shouldDebugGraphics()
		End Sub

		''' <summary>
		''' Constructs a debug graphics context from an existing graphics
		''' context that supports slowed down drawing.
		''' </summary>
		''' <param name="graphics">  the Graphics context to slow down </param>
		Public Sub New(ByVal graphics As Graphics)
			Me.New()
			Me.graphics = graphics
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.create</code> to return a DebugGraphics object.
		''' </summary>
		Public Overridable Function create() As Graphics
			Dim ___debugGraphics As DebugGraphics

			___debugGraphics = New DebugGraphics
			___debugGraphics.graphics = graphics.create()
			___debugGraphics.debugOptions = debugOptions
			___debugGraphics.buffer = buffer

			Return ___debugGraphics
		End Function

		''' <summary>
		''' Overrides <code>Graphics.create</code> to return a DebugGraphics object.
		''' </summary>
		Public Overridable Function create(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As Graphics
			Dim ___debugGraphics As DebugGraphics

			___debugGraphics = New DebugGraphics
			___debugGraphics.graphics = graphics.create(x, y, width, height)
			___debugGraphics.debugOptions = debugOptions
			___debugGraphics.buffer = buffer
			___debugGraphics.xOffset = xOffset + x
			___debugGraphics.yOffset = yOffset + y

			Return ___debugGraphics
		End Function


		'------------------------------------------------
		'  NEW METHODS
		'------------------------------------------------

		''' <summary>
		''' Sets the Color used to flash drawing operations.
		''' </summary>
		Public Shared Property flashColor As Color
			Set(ByVal flashColor As Color)
				info().flashColor = flashColor
			End Set
		End Property

		''' <summary>
		''' Returns the Color used to flash drawing operations. </summary>
		''' <seealso cref= #setFlashColor </seealso>
		Public Shared Function flashColor() As Color
			Return info().flashColor
		End Function

		''' <summary>
		''' Sets the time delay of drawing operation flashing.
		''' </summary>
		Public Shared Property flashTime As Integer
			Set(ByVal flashTime As Integer)
				info().flashTime = flashTime
			End Set
		End Property

		''' <summary>
		''' Returns the time delay of drawing operation flashing. </summary>
		''' <seealso cref= #setFlashTime </seealso>
		Public Shared Function flashTime() As Integer
			Return info().flashTime
		End Function

		''' <summary>
		''' Sets the number of times that drawing operations will flash.
		''' </summary>
		Public Shared Property flashCount As Integer
			Set(ByVal flashCount As Integer)
				info().flashCount = flashCount
			End Set
		End Property

		''' <summary>
		''' Returns the number of times that drawing operations will flash. </summary>
		''' <seealso cref= #setFlashCount </seealso>
		Public Shared Function flashCount() As Integer
			Return info().flashCount
		End Function

		''' <summary>
		''' Sets the stream to which the DebugGraphics logs drawing operations.
		''' </summary>
		Public Shared Property logStream As java.io.PrintStream
			Set(ByVal stream As java.io.PrintStream)
				info().stream = stream
			End Set
		End Property

		''' <summary>
		''' Returns the stream to which the DebugGraphics logs drawing operations. </summary>
		''' <seealso cref= #setLogStream </seealso>
		Public Shared Function logStream() As java.io.PrintStream
			Return info().stream
		End Function

		''' <summary>
		''' Sets the Font used for text drawing operations.
		''' </summary>
		Public Overridable Property font As Font
			Set(ByVal aFont As Font)
				If debugLog() Then info().log(toShortString() & " Setting font: " & aFont)
				graphics.font = aFont
			End Set
			Get
				Return graphics.font
			End Get
		End Property


		''' <summary>
		''' Sets the color to be used for drawing and filling lines and shapes.
		''' </summary>
		Public Overridable Property color As Color
			Set(ByVal aColor As Color)
				If debugLog() Then info().log(toShortString() & " Setting color: " & aColor)
				graphics.color = aColor
			End Set
			Get
				Return graphics.color
			End Get
		End Property



		'-----------------------------------------------
		' OVERRIDDEN METHODS
		'------------------------------------------------

		''' <summary>
		''' Overrides <code>Graphics.getFontMetrics</code>.
		''' </summary>
		Public Overridable Property fontMetrics As FontMetrics
			Get
				Return graphics.fontMetrics
			End Get
		End Property

		''' <summary>
		''' Overrides <code>Graphics.getFontMetrics</code>.
		''' </summary>
		Public Overridable Function getFontMetrics(ByVal f As Font) As FontMetrics
			Return graphics.getFontMetrics(f)
		End Function

		''' <summary>
		''' Overrides <code>Graphics.translate</code>.
		''' </summary>
		Public Overridable Sub translate(ByVal x As Integer, ByVal y As Integer)
			If debugLog() Then info().log(toShortString() & " Translating by: " & New Point(x, y))
			xOffset += x
			yOffset += y
			graphics.translate(x, y)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.setPaintMode</code>.
		''' </summary>
		Public Overridable Sub setPaintMode()
			If debugLog() Then info().log(toShortString() & " Setting paint mode")
			graphics.paintModeode()
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.setXORMode</code>.
		''' </summary>
		Public Overridable Property xORMode As Color
			Set(ByVal aColor As Color)
				If debugLog() Then info().log(toShortString() & " Setting XOR mode: " & aColor)
				graphics.xORMode = aColor
			End Set
		End Property

		''' <summary>
		''' Overrides <code>Graphics.getClipBounds</code>.
		''' </summary>
		Public Overridable Property clipBounds As Rectangle
			Get
				Return graphics.clipBounds
			End Get
		End Property

		''' <summary>
		''' Overrides <code>Graphics.clipRect</code>.
		''' </summary>
		Public Overridable Sub clipRect(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			graphics.clipRect(x, y, width, height)
			If debugLog() Then info().log(toShortString() & " Setting clipRect: " & (New Rectangle(x, y, width, height)) & " New clipRect: " & graphics.clip)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.setClip</code>.
		''' </summary>
		Public Overridable Sub setClip(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			graphics.cliplip(x, y, width, height)
			If debugLog() Then info().log(toShortString() & " Setting new clipRect: " & graphics.clip)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.getClip</code>.
		''' </summary>
		Public Overridable Property clip As Shape
			Get
				Return graphics.clip
			End Get
			Set(ByVal clip As Shape)
				graphics.clip = clip
				If debugLog() Then info().log(toShortString() & " Setting new clipRect: " & graphics.clip)
			End Set
		End Property


		''' <summary>
		''' Overrides <code>Graphics.drawRect</code>.
		''' </summary>
		Public Overridable Sub drawRect(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Drawing rect: " & New Rectangle(x, y, width, height))

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawRect(x, y, width, height)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.drawRect(x, y, width, height)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.drawRect(x, y, width, height)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.fillRect</code>.
		''' </summary>
		Public Overridable Sub fillRect(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Filling rect: " & New Rectangle(x, y, width, height))

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.fillRect(x, y, width, height)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.fillRect(x, y, width, height)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.fillRect(x, y, width, height)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.clearRect</code>.
		''' </summary>
		Public Overridable Sub clearRect(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Clearing rect: " & New Rectangle(x, y, width, height))

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.clearRect(x, y, width, height)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.clearRect(x, y, width, height)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.clearRect(x, y, width, height)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.drawRoundRect</code>.
		''' </summary>
		Public Overridable Sub drawRoundRect(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal arcWidth As Integer, ByVal arcHeight As Integer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Drawing round rect: " & New Rectangle(x, y, width, height) & " arcWidth: " & arcWidth & " archHeight: " & arcHeight)
			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawRoundRect(x, y, width, height, arcWidth, arcHeight)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.drawRoundRect(x, y, width, height, arcWidth, arcHeight)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.drawRoundRect(x, y, width, height, arcWidth, arcHeight)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.fillRoundRect</code>.
		''' </summary>
		Public Overridable Sub fillRoundRect(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal arcWidth As Integer, ByVal arcHeight As Integer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Filling round rect: " & New Rectangle(x, y, width, height) & " arcWidth: " & arcWidth & " archHeight: " & arcHeight)
			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.fillRoundRect(x, y, width, height, arcWidth, arcHeight)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.fillRoundRect(x, y, width, height, arcWidth, arcHeight)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.fillRoundRect(x, y, width, height, arcWidth, arcHeight)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.drawLine</code>.
		''' </summary>
		Public Overridable Sub drawLine(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Drawing line: from " & pointToString(x1, y1) & " to " & pointToString(x2, y2))

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawLine(x1, y1, x2, y2)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.drawLine(x1, y1, x2, y2)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.drawLine(x1, y1, x2, y2)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.draw3DRect</code>.
		''' </summary>
		Public Overridable Sub draw3DRect(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal raised As Boolean)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Drawing 3D rect: " & New Rectangle(x, y, width, height) & " Raised bezel: " & raised)
			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.draw3DRect(x, y, width, height, raised)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.draw3DRect(x, y, width, height, raised)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.draw3DRect(x, y, width, height, raised)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.fill3DRect</code>.
		''' </summary>
		Public Overridable Sub fill3DRect(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal raised As Boolean)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Filling 3D rect: " & New Rectangle(x, y, width, height) & " Raised bezel: " & raised)
			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.fill3DRect(x, y, width, height, raised)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.fill3DRect(x, y, width, height, raised)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.fill3DRect(x, y, width, height, raised)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.drawOval</code>.
		''' </summary>
		Public Overridable Sub drawOval(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Drawing oval: " & New Rectangle(x, y, width, height))
			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawOval(x, y, width, height)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.drawOval(x, y, width, height)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.drawOval(x, y, width, height)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.fillOval</code>.
		''' </summary>
		Public Overridable Sub fillOval(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Filling oval: " & New Rectangle(x, y, width, height))
			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.fillOval(x, y, width, height)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.fillOval(x, y, width, height)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.fillOval(x, y, width, height)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.drawArc</code>.
		''' </summary>
		Public Overridable Sub drawArc(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal startAngle As Integer, ByVal arcAngle As Integer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Drawing arc: " & New Rectangle(x, y, width, height) & " startAngle: " & startAngle & " arcAngle: " & arcAngle)
			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawArc(x, y, width, height, startAngle, arcAngle)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.drawArc(x, y, width, height, startAngle, arcAngle)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.drawArc(x, y, width, height, startAngle, arcAngle)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.fillArc</code>.
		''' </summary>
		Public Overridable Sub fillArc(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal startAngle As Integer, ByVal arcAngle As Integer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Filling arc: " & New Rectangle(x, y, width, height) & " startAngle: " & startAngle & " arcAngle: " & arcAngle)
			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.fillArc(x, y, width, height, startAngle, arcAngle)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.fillArc(x, y, width, height, startAngle, arcAngle)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.fillArc(x, y, width, height, startAngle, arcAngle)
		End Sub

		''' <summary>
		''' Overrides <code>Graphics.drawPolyline</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void drawPolyline(int xPoints() , int yPoints(), int nPoints)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Drawing polyline: " & " nPoints: " & nPoints & " X's: " & xPoints & " Y's: " & yPoints)
			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawPolyline(xPoints, yPoints, nPoints)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.drawPolyline(xPoints, yPoints, nPoints)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.drawPolyline(xPoints, yPoints, nPoints)

		''' <summary>
		''' Overrides <code>Graphics.drawPolygon</code>.
		''' </summary>
		public void drawPolygon(Integer xPoints() , Integer yPoints(), Integer nPoints)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Drawing polygon: " & " nPoints: " & nPoints & " X's: " & xPoints & " Y's: " & yPoints)
			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawPolygon(xPoints, yPoints, nPoints)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.drawPolygon(xPoints, yPoints, nPoints)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.drawPolygon(xPoints, yPoints, nPoints)

		''' <summary>
		''' Overrides <code>Graphics.fillPolygon</code>.
		''' </summary>
		public void fillPolygon(Integer xPoints() , Integer yPoints(), Integer nPoints)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Filling polygon: " & " nPoints: " & nPoints & " X's: " & xPoints & " Y's: " & yPoints)
			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.fillPolygon(xPoints, yPoints, nPoints)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.fillPolygon(xPoints, yPoints, nPoints)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.fillPolygon(xPoints, yPoints, nPoints)

		''' <summary>
		''' Overrides <code>Graphics.drawString</code>.
		''' </summary>
		public void drawString(String aString, Integer x, Integer y)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Drawing string: """ & aString & """ at: " & New Point(x, y))

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawString(aString, x, y)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.drawString(aString, x, y)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.drawString(aString, x, y)

		''' <summary>
		''' Overrides <code>Graphics.drawString</code>.
		''' </summary>
		public void drawString(java.text.AttributedCharacterIterator iterator, Integer x, Integer y)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info().log(toShortString() & " Drawing text: """ & iterator & """ at: " & New Point(x, y))

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawString(iterator, x, y)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.drawString(iterator, x, y)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.drawString(iterator, x, y)

		''' <summary>
		''' Overrides <code>Graphics.drawBytes</code>.
		''' </summary>
		public void drawBytes(SByte data() , Integer offset, Integer length, Integer x, Integer y)
			Dim info As DebugGraphicsInfo = info()

			Dim ___font As Font = graphics.font

			If debugLog() Then info().log(toShortString() & " Drawing bytes at: " & New Point(x, y))

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawBytes(data, offset, length, x, y)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.drawBytes(data, offset, length, x, y)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.drawBytes(data, offset, length, x, y)

		''' <summary>
		''' Overrides <code>Graphics.drawChars</code>.
		''' </summary>
		public void drawChars(Char data() , Integer offset, Integer length, Integer x, Integer y)
			Dim info As DebugGraphicsInfo = info()

			Dim ___font As Font = graphics.font

			If debugLog() Then info().log(toShortString() & " Drawing chars at " & New Point(x, y))

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawChars(data, offset, length, x, y)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim oldColor As Color = color
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1

				For i = 0 To count - 1
					graphics.color = If((i Mod 2) = 0, info.flashColor, oldColor)
					graphics.drawChars(data, offset, length, x, y)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
				graphics.color = oldColor
			End If
			graphics.drawChars(data, offset, length, x, y)

		''' <summary>
		''' Overrides <code>Graphics.drawImage</code>.
		''' </summary>
		public Boolean drawImage(Image img, Integer x, Integer y, ImageObserver observer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info.log(toShortString() & " Drawing image: " & img & " at: " & New Point(x, y))

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawImage(img, x, y, observer)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1
				Dim oldProducer As ImageProducer = img.source
				Dim newProducer As ImageProducer = New FilteredImageSource(oldProducer, New DebugGraphicsFilter(info.flashColor))
				Dim newImage As Image = Toolkit.defaultToolkit.createImage(newProducer)
				Dim imageObserver As New DebugGraphicsObserver

				Dim imageToDraw As Image
				For i = 0 To count - 1
					imageToDraw = If((i Mod 2) = 0, newImage, img)
					loadImage(imageToDraw)
					graphics.drawImage(imageToDraw, x, y, imageObserver)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
			End If
			Return graphics.drawImage(img, x, y, observer)

		''' <summary>
		''' Overrides <code>Graphics.drawImage</code>.
		''' </summary>
		public Boolean drawImage(Image img, Integer x, Integer y, Integer width, Integer height, ImageObserver observer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info.log(toShortString() & " Drawing image: " & img & " at: " & New Rectangle(x, y, width, height))

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawImage(img, x, y, width, height, observer)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1
				Dim oldProducer As ImageProducer = img.source
				Dim newProducer As ImageProducer = New FilteredImageSource(oldProducer, New DebugGraphicsFilter(info.flashColor))
				Dim newImage As Image = Toolkit.defaultToolkit.createImage(newProducer)
				Dim imageObserver As New DebugGraphicsObserver

				Dim imageToDraw As Image
				For i = 0 To count - 1
					imageToDraw = If((i Mod 2) = 0, newImage, img)
					loadImage(imageToDraw)
					graphics.drawImage(imageToDraw, x, y, width, height, imageObserver)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
			End If
			Return graphics.drawImage(img, x, y, width, height, observer)

		''' <summary>
		''' Overrides <code>Graphics.drawImage</code>.
		''' </summary>
		public Boolean drawImage(Image img, Integer x, Integer y, Color bgcolor, ImageObserver observer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info.log(toShortString() & " Drawing image: " & img & " at: " & New Point(x, y) & ", bgcolor: " & bgcolor)

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawImage(img, x, y, bgcolor, observer)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1
				Dim oldProducer As ImageProducer = img.source
				Dim newProducer As ImageProducer = New FilteredImageSource(oldProducer, New DebugGraphicsFilter(info.flashColor))
				Dim newImage As Image = Toolkit.defaultToolkit.createImage(newProducer)
				Dim imageObserver As New DebugGraphicsObserver

				Dim imageToDraw As Image
				For i = 0 To count - 1
					imageToDraw = If((i Mod 2) = 0, newImage, img)
					loadImage(imageToDraw)
					graphics.drawImage(imageToDraw, x, y, bgcolor, imageObserver)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
			End If
			Return graphics.drawImage(img, x, y, bgcolor, observer)

		''' <summary>
		''' Overrides <code>Graphics.drawImage</code>.
		''' </summary>
		public Boolean drawImage(Image img, Integer x, Integer y,Integer width, Integer height, Color bgcolor, ImageObserver observer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info.log(toShortString() & " Drawing image: " & img & " at: " & New Rectangle(x, y, width, height) & ", bgcolor: " & bgcolor)

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawImage(img, x, y, width, height, bgcolor, observer)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1
				Dim oldProducer As ImageProducer = img.source
				Dim newProducer As ImageProducer = New FilteredImageSource(oldProducer, New DebugGraphicsFilter(info.flashColor))
				Dim newImage As Image = Toolkit.defaultToolkit.createImage(newProducer)
				Dim imageObserver As New DebugGraphicsObserver

				Dim imageToDraw As Image
				For i = 0 To count - 1
					imageToDraw = If((i Mod 2) = 0, newImage, img)
					loadImage(imageToDraw)
					graphics.drawImage(imageToDraw, x, y, width, height, bgcolor, imageObserver)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
			End If
			Return graphics.drawImage(img, x, y, width, height, bgcolor, observer)

		''' <summary>
		''' Overrides <code>Graphics.drawImage</code>.
		''' </summary>
		public Boolean drawImage(Image img, Integer dx1, Integer dy1, Integer dx2, Integer dy2, Integer sx1, Integer sy1, Integer sx2, Integer sy2, ImageObserver observer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info.log(toShortString() & " Drawing image: " & img & " destination: " & New Rectangle(dx1, dy1, dx2, dy2) & " source: " & New Rectangle(sx1, sy1, sx2, sy2))

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawImage(img, dx1, dy1, dx2, dy2, sx1, sy1, sx2, sy2, observer)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1
				Dim oldProducer As ImageProducer = img.source
				Dim newProducer As ImageProducer = New FilteredImageSource(oldProducer, New DebugGraphicsFilter(info.flashColor))
				Dim newImage As Image = Toolkit.defaultToolkit.createImage(newProducer)
				Dim imageObserver As New DebugGraphicsObserver

				Dim imageToDraw As Image
				For i = 0 To count - 1
					imageToDraw = If((i Mod 2) = 0, newImage, img)
					loadImage(imageToDraw)
					graphics.drawImage(imageToDraw, dx1, dy1, dx2, dy2, sx1, sy1, sx2, sy2, imageObserver)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
			End If
			Return graphics.drawImage(img, dx1, dy1, dx2, dy2, sx1, sy1, sx2, sy2, observer)

		''' <summary>
		''' Overrides <code>Graphics.drawImage</code>.
		''' </summary>
		public Boolean drawImage(Image img, Integer dx1, Integer dy1, Integer dx2, Integer dy2, Integer sx1, Integer sy1, Integer sx2, Integer sy2, Color bgcolor, ImageObserver observer)
			Dim info As DebugGraphicsInfo = info()

			If debugLog() Then info.log(toShortString() & " Drawing image: " & img & " destination: " & New Rectangle(dx1, dy1, dx2, dy2) & " source: " & New Rectangle(sx1, sy1, sx2, sy2) & ", bgcolor: " & bgcolor)

			If drawingBuffer Then
				If debugBuffered() Then
					Dim ___debugGraphics As Graphics = debugGraphics()

					___debugGraphics.drawImage(img, dx1, dy1, dx2, dy2, sx1, sy1, sx2, sy2, bgcolor, observer)
					___debugGraphics.Dispose()
				End If
			ElseIf debugFlash() Then
				Dim i As Integer, count As Integer = (info.flashCount * 2) - 1
				Dim oldProducer As ImageProducer = img.source
				Dim newProducer As ImageProducer = New FilteredImageSource(oldProducer, New DebugGraphicsFilter(info.flashColor))
				Dim newImage As Image = Toolkit.defaultToolkit.createImage(newProducer)
				Dim imageObserver As New DebugGraphicsObserver

				Dim imageToDraw As Image
				For i = 0 To count - 1
					imageToDraw = If((i Mod 2) = 0, newImage, img)
					loadImage(imageToDraw)
					graphics.drawImage(imageToDraw, dx1, dy1, dx2, dy2, sx1, sy1, sx2, sy2, bgcolor, imageObserver)
					Toolkit.defaultToolkit.sync()
					sleep(info.flashTime)
				Next i
			End If
			Return graphics.drawImage(img, dx1, dy1, dx2, dy2, sx1, sy1, sx2, sy2, bgcolor, observer)

		static void loadImage(Image img)
			imageLoadingIcon.loadImage(img)


		''' <summary>
		''' Overrides <code>Graphics.copyArea</code>.
		''' </summary>
		public void copyArea(Integer x, Integer y, Integer width, Integer height, Integer destX, Integer destY)
			If debugLog() Then info().log(toShortString() & " Copying area from: " & New Rectangle(x, y, width, height) & " to: " & New Point(destX, destY))
			graphics.copyArea(x, y, width, height, destX, destY)

		final void sleep(Integer mSecs)
			Try
				Thread.Sleep(mSecs)
			Catch e As Exception
			End Try

		''' <summary>
		''' Overrides <code>Graphics.dispose</code>.
		''' </summary>
		public void Dispose()
			graphics.Dispose()
			graphics = Nothing

		' ALERT!
		''' <summary>
		''' Returns the drawingBuffer value.
		''' </summary>
		''' <returns> true if this object is drawing from a Buffer </returns>
		public Boolean drawingBuffer
			Return buffer IsNot Nothing

		String toShortString()
			Return "Graphics" & (If(drawingBuffer, "<B>", "")) & "(" & graphicsID & "-" & debugOptions & ")"

		String pointToString(Integer x, Integer y)
			Return "(" & x & ", " & y & ")"

		''' <summary>
		''' Enables/disables diagnostic information about every graphics
		''' operation. The value of <b>options</b> indicates how this information
		''' should be displayed. LOG_OPTION causes a text message to be printed.
		''' FLASH_OPTION causes the drawing to flash several times. BUFFERED_OPTION
		''' creates a new Frame that shows each operation on an
		''' offscreen buffer. The value of <b>options</b> is bitwise OR'd into
		''' the current value. To disable debugging use NONE_OPTION.
		''' </summary>
		public void debugOptionsons(Integer options)
			If options <> 0 Then
				If options = NONE_OPTION Then
					If debugOptions <> 0 Then
						Console.Error.WriteLine(toShortString() & " Disabling debug")
						debugOptions = 0
					End If
				Else
					If debugOptions <> options Then
						debugOptions = debugOptions Or options
						If debugLog() Then Console.Error.WriteLine(toShortString() & " Enabling debug")
					End If
				End If
			End If

		''' <summary>
		''' Returns the current debugging options for this DebugGraphics. </summary>
		''' <seealso cref= #setDebugOptions </seealso>
		public Integer debugOptions
			Return debugOptions

		''' <summary>
		''' Static wrapper method for DebugGraphicsInfo.setDebugOptions(). Stores
		''' options on a per component basis.
		''' </summary>
		static void debugOptionsons(JComponent component, Integer options)
			info().debugOptionsons(component, options)

		''' <summary>
		''' Static wrapper method for DebugGraphicsInfo.getDebugOptions().
		''' </summary>
		static Integer getDebugOptions(JComponent component)
			Dim debugGraphicsInfo As DebugGraphicsInfo = info()
			If debugGraphicsInfo Is Nothing Then
				Return 0
			Else
				Return debugGraphicsInfo.getDebugOptions(component)
			End If

		''' <summary>
		''' Returns non-zero if <b>component</b> should display with DebugGraphics,
		''' zero otherwise. Walks the JComponent's parent tree to determine if
		''' any debugging options have been set.
		''' </summary>
		static Integer shouldComponentDebug(JComponent component)
			Dim info As DebugGraphicsInfo = info()
			If info Is Nothing Then
				Return 0
			Else
				Dim container As Container = CType(component, Container)
				Dim ___debugOptions As Integer = 0

				Do While container IsNot Nothing AndAlso (TypeOf container Is JComponent)
					___debugOptions = ___debugOptions Or info.getDebugOptions(CType(container, JComponent))
					container = container.parent
				Loop

				Return ___debugOptions
			End If

		''' <summary>
		''' Returns the number of JComponents that have debugging options turned
		''' on.
		''' </summary>
		static Integer debugComponentCount()
			Dim debugGraphicsInfo As DebugGraphicsInfo = info()
			If debugGraphicsInfo IsNot Nothing AndAlso debugGraphicsInfo.componentToDebug IsNot Nothing Then
				Return debugGraphicsInfo.componentToDebug.Count
			Else
				Return 0
			End If

		Boolean debugLog()
			Return (debugOptions And LOG_OPTION) = LOG_OPTION

		Boolean debugFlash()
			Return (debugOptions And FLASH_OPTION) = FLASH_OPTION

		Boolean debugBuffered()
			Return (debugOptions And BUFFERED_OPTION) = BUFFERED_OPTION

		''' <summary>
		''' Returns a DebugGraphics for use in buffering window.
		''' </summary>
		private Graphics debugGraphics()
			Dim ___debugGraphics As DebugGraphics
			Dim info As DebugGraphicsInfo = info()
			Dim debugFrame As JFrame

			If info.debugFrame Is Nothing Then
				info.debugFrame = New JFrame
				info.debugFrame.sizeize(500, 500)
			End If
			debugFrame = info.debugFrame
			debugFrame.show()
			___debugGraphics = New DebugGraphics(debugFrame.graphics)
			___debugGraphics.font = font
			___debugGraphics.color = color
			___debugGraphics.translate(xOffset, yOffset)
			___debugGraphics.clip = clipBounds
			If debugFlash() Then ___debugGraphics.debugOptions = FLASH_OPTION
			Return ___debugGraphics

		''' <summary>
		''' Returns DebugGraphicsInfo, or creates one if none exists.
		''' </summary>
		static DebugGraphicsInfo info()
			Dim debugGraphicsInfo As DebugGraphicsInfo = CType(SwingUtilities.appContextGet(debugGraphicsInfoKey), DebugGraphicsInfo)
			If debugGraphicsInfo Is Nothing Then
				debugGraphicsInfo = New DebugGraphicsInfo
				SwingUtilities.appContextPut(debugGraphicsInfoKey, debugGraphicsInfo)
			End If
			Return debugGraphicsInfo
		private static final Type debugGraphicsInfoKey = GetType(DebugGraphicsInfo)


	End Class

End Namespace
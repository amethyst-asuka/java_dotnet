Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports java.io
Imports java.security

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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





    ''' <summary>
    ''' A class to encapsulate the bitmap representation of the mouse cursor.
    ''' </summary>
    ''' <seealso cref= Component#setCursor
    ''' @author      Amy Fowler </seealso>
    <Serializable>
    Public Class Cursor

        ''' <summary>
        ''' The default cursor type (gets set if no cursor is defined).
        ''' </summary>
        Public Const DEFAULT_CURSOR As Integer = 0

        ''' <summary>
        ''' The crosshair cursor type.
        ''' </summary>
        Public Const CROSSHAIR_CURSOR As Integer = 1

        ''' <summary>
        ''' The text cursor type.
        ''' </summary>
        Public Const TEXT_CURSOR As Integer = 2

        ''' <summary>
        ''' The wait cursor type.
        ''' </summary>
        Public Const WAIT_CURSOR As Integer = 3

        ''' <summary>
        ''' The south-west-resize cursor type.
        ''' </summary>
        Public Const SW_RESIZE_CURSOR As Integer = 4

        ''' <summary>
        ''' The south-east-resize cursor type.
        ''' </summary>
        Public Const SE_RESIZE_CURSOR As Integer = 5

        ''' <summary>
        ''' The north-west-resize cursor type.
        ''' </summary>
        Public Const NW_RESIZE_CURSOR As Integer = 6

        ''' <summary>
        ''' The north-east-resize cursor type.
        ''' </summary>
        Public Const NE_RESIZE_CURSOR As Integer = 7

        ''' <summary>
        ''' The north-resize cursor type.
        ''' </summary>
        Public Const N_RESIZE_CURSOR As Integer = 8

        ''' <summary>
        ''' The south-resize cursor type.
        ''' </summary>
        Public Const S_RESIZE_CURSOR As Integer = 9

        ''' <summary>
        ''' The west-resize cursor type.
        ''' </summary>
        Public Const W_RESIZE_CURSOR As Integer = 10

        ''' <summary>
        ''' The east-resize cursor type.
        ''' </summary>
        Public Const E_RESIZE_CURSOR As Integer = 11

        ''' <summary>
        ''' The hand cursor type.
        ''' </summary>
        Public Const HAND_CURSOR As Integer = 12

        ''' <summary>
        ''' The move cursor type.
        ''' </summary>
        Public Const MOVE_CURSOR As Integer = 13

        ''' @deprecated As of JDK version 1.7, the <seealso cref="#getPredefinedCursor(int)"/>
        ''' method should be used instead. 
        <Obsolete("As of JDK version 1.7, the <seealso cref=""#getPredefinedCursor(int)""/>")>
        Protected Friend Shared predefined As Cursor() = New Cursor(13) {}

        ''' <summary>
        ''' This field is a private replacement for 'predefined' array.
        ''' </summary>
        Private Shared ReadOnly predefinedPrivate As Cursor() = New Cursor(13) {}

        ' Localization names and default values 
        Friend Shared ReadOnly cursorProperties As String()() = {New String() {"AWT.DefaultCursor", "Default Cursor"}, New String() {"AWT.CrosshairCursor", "Crosshair Cursor"}, New String() {"AWT.TextCursor", "Text Cursor"}, New String() {"AWT.WaitCursor", "Wait Cursor"}, New String() {"AWT.SWResizeCursor", "Southwest Resize Cursor"}, New String() {"AWT.SEResizeCursor", "Southeast Resize Cursor"}, New String() {"AWT.NWResizeCursor", "Northwest Resize Cursor"}, New String() {"AWT.NEResizeCursor", "Northeast Resize Cursor"}, New String() {"AWT.NResizeCursor", "North Resize Cursor"}, New String() {"AWT.SResizeCursor", "South Resize Cursor"}, New String() {"AWT.WResizeCursor", "West Resize Cursor"}, New String() {"AWT.EResizeCursor", "East Resize Cursor"}, New String() {"AWT.HandCursor", "Hand Cursor"}, New String() {"AWT.MoveCursor", "Move Cursor"}}

        ''' <summary>
        ''' The chosen cursor type initially set to
        ''' the <code>DEFAULT_CURSOR</code>.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getType() </seealso>
        Friend _type As Integer = DEFAULT_CURSOR

        ''' <summary>
        ''' The type associated with all custom cursors.
        ''' </summary>
        Public Const CUSTOM_CURSOR As Integer = -1

        '    
        '     * hashtable, filesystem dir prefix, filename, and properties for custom cursors support
        '     

        Private Shared ReadOnly systemCustomCursors As New Dictionary(Of String, Cursor)(1)
        Private Shared ReadOnly systemCustomCursorDirPrefix As String = initCursorDir()

        Private Shared Function initCursorDir() As String
            Dim jhome As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("java.home"))
            Return jhome + File.separator & "lib" & File.separator & "images" & File.separator & "cursors" & File.separator
        End Function

        Private Shared ReadOnly systemCustomCursorPropertiesFile As String = systemCustomCursorDirPrefix & "cursors.properties"

        Private Shared systemCustomCursorProperties As java.util.Properties = Nothing

        Private Const CursorDotPrefix As String = "Cursor."
        Private Const DotFileSuffix As String = ".File"
        Private Const DotHotspotSuffix As String = ".HotSpot"
        Private Const DotNameSuffix As String = ".Name"

        '    
        '     * JDK 1.1 serialVersionUID
        '     
        Private Const serialVersionUID As Long = 8028237497568985504L

        Private Shared ReadOnly log As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.Cursor")

        Shared Sub New()
            ' ensure that the necessary native libraries are loaded 
            Toolkit.loadLibraries()
            If Not GraphicsEnvironment.headless Then initIDs()

            'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
            '			sun.awt.AWTAccessor.setCursorAccessor(New sun.awt.AWTAccessor.CursorAccessor()
            '		{
            '				public long getPData(Cursor cursor)
            '				{
            '					Return cursor.pData;
            '				}
            '
            '				public void setPData(Cursor cursor, long pData)
            '				{
            '					cursor.pData = pData;
            '				}
            '
            '				public int getType(Cursor cursor)
            '				{
            '					Return cursor.type;
            '				}
            '			});
        End Sub

        ''' <summary>
        ''' Initialize JNI field and method IDs for fields that may be
        ''' accessed from C.
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub initIDs()
        End Sub

        ''' <summary>
        ''' Hook into native data.
        ''' </summary>
        <NonSerialized>
        Private pData As Long

        <NonSerialized>
        Private anchor As New Object

        Friend Class CursorDisposer
            Implements sun.java2d.DisposerRecord

            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            Friend pData As Long
            Public Sub New(ByVal pData As Long)
                Me.pData = pData
            End Sub
            Public Overridable Sub dispose()
                If pData <> 0 Then finalizeImpl(pData)
            End Sub
        End Class
        <NonSerialized>
        Friend disposer As CursorDisposer
        Private WriteOnly Property pData As Long
            Set(ByVal pData As Long)
                Me.pData = pData
                If GraphicsEnvironment.headless Then Return
                If disposer Is Nothing Then
                    disposer = New CursorDisposer(pData)
                    ' anchor is null after deserialization
                    If anchor Is Nothing Then anchor = New Object
                    sun.java2d.Disposer.addRecord(anchor, disposer)
                Else
                    disposer.pData = pData
                End If
            End Set
        End Property

        ''' <summary>
        ''' The user-visible name of the cursor.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getName() </seealso>
        Protected Friend _name As String

        ''' <summary>
        ''' Returns a cursor object with the specified predefined type.
        ''' </summary>
        ''' <param name="type"> the type of predefined cursor </param>
        ''' <returns> the specified predefined cursor </returns>
        ''' <exception cref="IllegalArgumentException"> if the specified cursor type is
        '''         invalid </exception>
        Public Shared Function getPredefinedCursor(ByVal type As Integer) As Cursor
            If type < Cursor.DEFAULT_CURSOR OrElse type > Cursor.MOVE_CURSOR Then Throw New IllegalArgumentException("illegal cursor type")
            Dim c As Cursor = predefinedPrivate(type)
            If c Is Nothing Then
                c = New Cursor(type)
                predefinedPrivate(type) = c
            End If
            ' fill 'predefined' array for backwards compatibility.
            If predefined(type) Is Nothing Then predefined(type) = c
            Return c
        End Function

        ''' <summary>
        ''' Returns a system-specific custom cursor object matching the
        ''' specified name.  Cursor names are, for example: "Invalid.16x16"
        ''' </summary>
        ''' <param name="name"> a string describing the desired system-specific custom cursor </param>
        ''' <returns> the system specific custom cursor named </returns>
        ''' <exception cref="HeadlessException"> if
        ''' <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
        Public Shared Function getSystemCustomCursor(ByVal name As String) As Cursor
            GraphicsEnvironment.checkHeadless()
            Dim cursor_Renamed As Cursor = systemCustomCursors(name)

            If cursor_Renamed Is Nothing Then
                SyncLock systemCustomCursors
                    If systemCustomCursorProperties Is Nothing Then loadSystemCustomCursorProperties()
                End SyncLock

                Dim prefix As String = CursorDotPrefix + name
                Dim key As String = prefix + DotFileSuffix

                If Not systemCustomCursorProperties.containsKey(key) Then
                    If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then log.finer("Cursor.getSystemCustomCursor(" & name & ") returned null")
                    Return Nothing
                End If

                Dim fileName As String = systemCustomCursorProperties.getProperty(key)

                Dim localized As String = systemCustomCursorProperties.getProperty(prefix + DotNameSuffix)

                If localized Is Nothing Then localized = name

                Dim hotspot As String = systemCustomCursorProperties.getProperty(prefix + DotHotspotSuffix)

                If hotspot Is Nothing Then Throw New AWTException("no hotspot property defined for cursor: " & name)

                Dim st As New java.util.StringTokenizer(hotspot, ",")

                If st.countTokens() <> 2 Then Throw New AWTException("failed to parse hotspot property for cursor: " & name)

                Dim x As Integer = 0
                Dim y As Integer = 0

                Try
                    x = Convert.ToInt32(st.nextToken())
                    y = Convert.ToInt32(st.nextToken())
                Catch nfe As NumberFormatException
                    Throw New AWTException("failed to parse hotspot property for cursor: " & name)
                End Try

                Try
                    Dim fx As Integer = x
                    Dim fy As Integer = y
                    Dim flocalized As String = localized

                    cursor_Renamed = java.security.AccessController.doPrivileged(Of Cursor)(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
                Catch e As Exception
                    Throw New AWTException("Exception: " & e.GetType() & " " & e.Message & " occurred while creating cursor " & name)
                End Try

                If cursor_Renamed Is Nothing Then
                    If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then log.finer("Cursor.getSystemCustomCursor(" & name & ") returned null")
                Else
                    systemCustomCursors(name) = cursor_Renamed
                End If
            End If

            Return cursor_Renamed
        End Function

        Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
            Implements PrivilegedExceptionAction(Of T)

            Public Overridable Function run() As Cursor
                Dim toolkit_Renamed As Toolkit = Toolkit.defaultToolkit
                Dim image_Renamed As image = toolkit_Renamed.getImage(systemCustomCursorDirPrefix + fileName)
                Return toolkit_Renamed.createCustomCursor(image_Renamed, New Point(fx, fy), flocalized)
            End Function
        End Class

        ''' <summary>
        ''' Return the system default cursor.
        ''' </summary>
        Public Shared Property defaultCursor As Cursor
            Get
                Return getPredefinedCursor(Cursor.DEFAULT_CURSOR)
            End Get
        End Property

        ''' <summary>
        ''' Creates a new cursor object with the specified type. </summary>
        ''' <param name="type"> the type of cursor </param>
        ''' <exception cref="IllegalArgumentException"> if the specified cursor type
        ''' is invalid </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Sub New(ByVal type As Integer)
            If type < Cursor.DEFAULT_CURSOR OrElse type > Cursor.MOVE_CURSOR Then Throw New IllegalArgumentException("illegal cursor type")
            Me.type = type

            ' Lookup localized name.
            name = Toolkit.getProperty(cursorProperties(type)(0), cursorProperties(type)(1))
        End Sub

        ''' <summary>
        ''' Creates a new custom cursor object with the specified name.<p>
        ''' Note:  this constructor should only be used by AWT implementations
        ''' as part of their support for custom cursors.  Applications should
        ''' use Toolkit.createCustomCursor(). </summary>
        ''' <param name="name"> the user-visible name of the cursor. </param>
        ''' <seealso cref= java.awt.Toolkit#createCustomCursor </seealso>
        Protected Friend Sub New(ByVal name As String)
            Me.type = Cursor.CUSTOM_CURSOR
            Me.name = name
        End Sub

        ''' <summary>
        ''' Returns the type for this cursor.
        ''' </summary>
        Public Overridable Property type As Integer
            Get
                Return _type
            End Get
            Protected Set(value As Integer)
                _type = value
            End Set
        End Property

        ''' <summary>
        ''' Returns the name of this cursor. </summary>
        ''' <returns>    a localized description of this cursor.
        ''' @since     1.2 </returns>
        Public Overridable Property name As String
            Get
                Return _name
            End Get
            Protected Set(value As String)
                _name = value
            End Set
        End Property

        ''' <summary>
        ''' Returns a string representation of this cursor. </summary>
        ''' <returns>    a string representation of this cursor.
        ''' @since     1.2 </returns>
        Public Overrides Function ToString() As String
            Return Me.GetType().Name & "[" & name & "]"
        End Function

        '    
        '     * load the cursor.properties file
        '     
        Private Shared Sub loadSystemCustomCursorProperties()
            SyncLock systemCustomCursors
                systemCustomCursorProperties = New java.util.Properties

                Try
                    java.security.AccessController.doPrivileged(Of Object)(New PrivilegedExceptionActionAnonymousInnerClassHelper2(Of T)
                Catch e As Exception
                    systemCustomCursorProperties = Nothing
                    Throw New AWTException("Exception: " & e.GetType() & " " & e.Message & " occurred while loading: " & systemCustomCursorPropertiesFile)
                End Try
            End SyncLock
        End Sub

        Private Class PrivilegedExceptionActionAnonymousInnerClassHelper2(Of T)
            Implements PrivilegedExceptionAction(Of T)

            Public Overridable Function run() As T Implements PrivilegedExceptionAction(Of T).run
                Dim fis As java.io.FileInputStream = Nothing
                Try
                    fis = New java.io.FileInputStream(systemCustomCursorPropertiesFile)
                    systemCustomCursorProperties.load(fis)
                Finally
                    If fis IsNot Nothing Then fis.close()
                End Try
                Return Nothing
            End Function
        End Class

        <DllImport("unknown")>
        Private Shared Sub finalizeImpl(ByVal pData As Long)
        End Sub
    End Class

End Namespace
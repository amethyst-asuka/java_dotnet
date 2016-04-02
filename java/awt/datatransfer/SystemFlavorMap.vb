Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports java.lang.ref

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.datatransfer







    ''' <summary>
    ''' The SystemFlavorMap is a configurable map between "natives" (Strings), which
    ''' correspond to platform-specific data formats, and "flavors" (DataFlavors),
    ''' which correspond to platform-independent MIME types. This mapping is used
    ''' by the data transfer subsystem to transfer data between Java and native
    ''' applications, and between Java applications in separate VMs.
    ''' <p>
    '''
    ''' @since 1.2
    ''' </summary>
    Public NotInheritable Class SystemFlavorMap
        Implements FlavorMap, FlavorTable

        ''' <summary>
        ''' Constant prefix used to tag Java types converted to native platform
        ''' type.
        ''' </summary>
        Private Shared JavaMIME As String = "JAVA_DATAFLAVOR:"

        Private Shared ReadOnly FLAVOR_MAP_KEY As New Object

        ''' <summary>
        ''' Copied from java.util.Properties.
        ''' </summary>
        Private Shared ReadOnly keyValueSeparators As String = "=: " & vbTab & vbCrLf & vbFormFeed
        Private Const strictKeyValueSeparators As String = "=:"
        Private Shared ReadOnly whiteSpaceChars As String = " " & vbTab & vbCrLf & vbFormFeed

        ''' <summary>
        ''' The list of valid, decoded text flavor representation classes, in order
        ''' from best to worst.
        ''' </summary>
        Private Shared ReadOnly UNICODE_TEXT_CLASSES As String() = {"java.io.Reader", "java.lang.String", "java.nio.CharBuffer", """[C"""}

        ''' <summary>
        ''' The list of valid, encoded text flavor representation classes, in order
        ''' from best to worst.
        ''' </summary>
        Private Shared ReadOnly ENCODED_TEXT_CLASSES As String() = {"java.io.InputStream", "java.nio.ByteBuffer", """[B"""}

        ''' <summary>
        ''' A String representing text/plain MIME type.
        ''' </summary>
        Private Const TEXT_PLAIN_BASE_TYPE As String = "text/plain"

        ''' <summary>
        ''' A String representing text/html MIME type.
        ''' </summary>
        Private Const HTML_TEXT_BASE_TYPE As String = "text/html"

        ''' <summary>
        ''' Maps native Strings to Lists of DataFlavors (or base type Strings for
        ''' text DataFlavors).
        ''' Do not use the field directly, use getNativeToFlavor() instead.
        ''' </summary>
        Private ReadOnly nativeToFlavor As IDictionary(Of String, java.util.LinkedHashSet(Of DataFlavor)) = New Dictionary(Of String, java.util.LinkedHashSet(Of DataFlavor))

        ''' <summary>
        ''' Accessor to nativeToFlavor map.  Since we use lazy initialization we must
        ''' use this accessor instead of direct access to the field which may not be
        ''' initialized yet.  This method will initialize the field if needed.
        ''' </summary>
        ''' <returns> nativeToFlavor </returns>
        Private Property nativeToFlavor As IDictionary(Of String, java.util.LinkedHashSet(Of DataFlavor))
            Get
                If Not isMapInitialized Then initSystemFlavorMap()
                Return nativeToFlavor
            End Get
        End Property

        ''' <summary>
        ''' Maps DataFlavors (or base type Strings for text DataFlavors) to Lists of
        ''' native Strings.
        ''' Do not use the field directly, use getFlavorToNative() instead.
        ''' </summary>
        Private ReadOnly flavorToNative As IDictionary(Of DataFlavor, java.util.LinkedHashSet(Of String)) = New Dictionary(Of DataFlavor, java.util.LinkedHashSet(Of String))

        ''' <summary>
        ''' Accessor to flavorToNative map.  Since we use lazy initialization we must
        ''' use this accessor instead of direct access to the field which may not be
        ''' initialized yet.  This method will initialize the field if needed.
        ''' </summary>
        ''' <returns> flavorToNative </returns>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Private Property flavorToNative As IDictionary(Of DataFlavor, java.util.LinkedHashSet(Of String))
            Get
                If Not isMapInitialized Then initSystemFlavorMap()
                Return flavorToNative
            End Get
        End Property

        ''' <summary>
        ''' Maps a text DataFlavor primary mime-type to the native. Used only to store
        ''' standard mappings registered in the flavormap.properties
        ''' Do not use this field directly, use getTextTypeToNative() instead.
        ''' </summary>
        Private textTypeToNative As IDictionary(Of String, java.util.LinkedHashSet(Of String)) = New Dictionary(Of String, java.util.LinkedHashSet(Of String))

        ''' <summary>
        ''' Shows if the object has been initialized.
        ''' </summary>
        Private isMapInitialized As Boolean = False

        ''' <summary>
        ''' An accessor to textTypeToNative map.  Since we use lazy initialization we
        ''' must use this accessor instead of direct access to the field which may not
        ''' be initialized yet. This method will initialize the field if needed.
        ''' </summary>
        ''' <returns> textTypeToNative </returns>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Private Property textTypeToNative As IDictionary(Of String, java.util.LinkedHashSet(Of String))
            Get
                If Not isMapInitialized Then
                    initSystemFlavorMap()
                    ' From this point the map should not be modified
                    textTypeToNative = java.util.Collections.unmodifiableMap(textTypeToNative)
                End If
                Return textTypeToNative
            End Get
        End Property

        ''' <summary>
        ''' Caches the result of getNativesForFlavor(). Maps DataFlavors to
        ''' SoftReferences which reference LinkedHashSet of String natives.
        ''' </summary>
        Private ReadOnly nativesForFlavorCache As New SoftCache(Of DataFlavor, String)

        ''' <summary>
        ''' Caches the result getFlavorsForNative(). Maps String natives to
        ''' SoftReferences which reference LinkedHashSet of DataFlavors.
        ''' </summary>
        Private ReadOnly flavorsForNativeCache As New SoftCache(Of String, DataFlavor)

        ''' <summary>
        ''' Dynamic mapping generation used for text mappings should not be applied
        ''' to the DataFlavors and String natives for which the mappings have been
        ''' explicitly specified with setFlavorsForNative() or
        ''' setNativesForFlavor(). This keeps all such keys.
        ''' </summary>
        Private disabledMappingGenerationKeys As java.util.Set(Of Object) = New HashSet(Of Object)

        ''' <summary>
        ''' Returns the default FlavorMap for this thread's ClassLoader.
        ''' </summary>
        Public Shared ReadOnly Property defaultFlavorMap As FlavorMap
            Get
                Dim context As sun.awt.AppContext = sun.awt.AppContext.appContext
                Dim fm As FlavorMap = CType(context.get(FLAVOR_MAP_KEY), FlavorMap)
                If fm Is Nothing Then
                    fm = New SystemFlavorMap
                    context.put(FLAVOR_MAP_KEY, fm)
                End If
                Return fm
            End Get
        End Property

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Initializes a SystemFlavorMap by reading flavormap.properties and
        ''' AWT.DnD.flavorMapFileURL.
        ''' For thread-safety must be called under lock on this.
        ''' </summary>
        Private Sub initSystemFlavorMap()
            If isMapInitialized Then Return

            isMapInitialized = True
            Dim flavormapDotProperties As java.io.BufferedReader = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T))

            Dim url As String = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T))

            If flavormapDotProperties IsNot Nothing Then
                Try
                    parseAndStoreReader(flavormapDotProperties)
                Catch e As java.io.IOException
                    Console.Error.WriteLine("IOException:" & e & " while parsing default flavormap.properties file")
                End Try
            End If

            Dim flavormapURL As java.io.BufferedReader = Nothing
            If url IsNot Nothing Then
                Try
                    flavormapURL = New java.io.BufferedReader(New java.io.InputStreamReader((New java.net.URL(url)).openStream(), "ISO-8859-1"))
                Catch e As java.net.MalformedURLException
                    Console.Error.WriteLine("MalformedURLException:" & e & " while reading AWT.DnD.flavorMapFileURL:" & url)
                Catch e As java.io.IOException
                    Console.Error.WriteLine("IOException:" & e & " while reading AWT.DnD.flavorMapFileURL:" & url)
                Catch e As SecurityException
                    ' ignored
                End Try
            End If

            If flavormapURL IsNot Nothing Then
                Try
                    parseAndStoreReader(flavormapURL)
                Catch e As java.io.IOException
                    Console.Error.WriteLine("IOException:" & e & " while parsing AWT.DnD.flavorMapFileURL")
                End Try
            End If
        End Sub

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements PrivilegedAction(Of T)

            Public Overridable Function run() As java.io.BufferedReader
                Dim fileName As String = System.getProperty("java.home") + File.separator & "lib" & File.separator & "flavormap.properties"
                Try
                    Return New java.io.BufferedReader(New java.io.InputStreamReader((New File(fileName)).toURI().toURL().openStream(), "ISO-8859-1"))
                Catch e As java.net.MalformedURLException
                    Console.Error.WriteLine("MalformedURLException:" & e & " while loading default flavormap.properties file:" & fileName)
                Catch e As java.io.IOException
                    Console.Error.WriteLine("IOException:" & e & " while loading default flavormap.properties file:" & fileName)
                End Try
                Return Nothing
            End Function
        End Class

        Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
            Implements PrivilegedAction(Of T)

            Public Overridable Function run() As String
                Return java.awt.Toolkit.getProperty("AWT.DnD.flavorMapFileURL", Nothing)
            End Function
        End Class
        ''' <summary>
        ''' Copied code from java.util.Properties. Parsing the data ourselves is the
        ''' only way to handle duplicate keys and values.
        ''' </summary>
        Private Sub parseAndStoreReader(ByVal [in] As java.io.BufferedReader)
            Do
                ' Get next line
                Dim line As String = [in].readLine()
                If line Is Nothing Then Return

                If line.Length() > 0 Then
                    ' Continue lines that end in slashes if they are not comments
                    Dim firstChar As Char = line.Chars(0)
                    If firstChar <> "#"c AndAlso firstChar <> "!"c Then
                        Do While continueLine(line)
                            Dim nextLine As String = [in].readLine()
                            If nextLine Is Nothing Then nextLine = ""
                            Dim loppedLine As String = line.Substring(0, line.Length() - 1)
                            ' Advance beyond whitespace on new line
                            Dim startIndex As Integer = 0
                            Do While startIndex < nextLine.Length()
                                If whiteSpaceChars.IndexOf(nextLine.Chars(startIndex)) = -1 Then Exit Do
                                startIndex += 1
                            Loop
                            nextLine = nextLine.Substring(startIndex, nextLine.Length() - startIndex)
                            line = loppedLine + nextLine
                        Loop

                        ' Find start of key
                        Dim len As Integer = line.Length()
                        Dim keyStart As Integer = 0
                        Do While keyStart < len
                            If whiteSpaceChars.IndexOf(line.Chars(keyStart)) = -1 Then Exit Do
                            keyStart += 1
                        Loop

                        ' Blank lines are ignored
                        If keyStart = len Then Continue Do

                        ' Find separation between key and value
                        Dim separatorIndex As Integer = keyStart
                        Do While separatorIndex < len
                            Dim currentChar As Char = line.Chars(separatorIndex)
                            If currentChar = "\"c Then
                                separatorIndex += 1
                            ElseIf keyValueSeparators.IndexOf(currentChar) <> -1 Then
                                Exit Do
                            End If
                            separatorIndex += 1
                        Loop

                        ' Skip over whitespace after key if any
                        Dim valueIndex As Integer = separatorIndex
                        Do While valueIndex < len
                            If whiteSpaceChars.IndexOf(line.Chars(valueIndex)) = -1 Then Exit Do
                            valueIndex += 1
                        Loop

                        ' Skip over one non whitespace key value separators if any
                        If valueIndex < len Then
                            If strictKeyValueSeparators.IndexOf(line.Chars(valueIndex)) <> -1 Then valueIndex += 1
                        End If

                        ' Skip over white space after other separators if any
                        Do While valueIndex < len
                            If whiteSpaceChars.IndexOf(line.Chars(valueIndex)) = -1 Then Exit Do
                            valueIndex += 1
                        Loop

                        Dim key As String = line.Substring(keyStart, separatorIndex - keyStart)
                        Dim value As String = If(separatorIndex < len, line.Substring(valueIndex, len - valueIndex), "")

                        ' Convert then store key and value
                        key = loadConvert(key)
                        value = loadConvert(value)

                        Try
                            Dim mime As New MimeType(value)
                            If "text".Equals(mime.primaryType) Then
                                Dim charset As String = mime.getParameter("charset")
                                If sun.awt.datatransfer.DataTransferer.doesSubtypeSupportCharset(mime.subType, charset) Then
                                    ' We need to store the charset and eoln
                                    ' parameters, if any, so that the
                                    ' DataTransferer will have this information
                                    ' for conversion into the native format.
                                    Dim transferer As sun.awt.datatransfer.DataTransferer = sun.awt.datatransfer.DataTransferer.instance
                                    If transferer IsNot Nothing Then transferer.registerTextFlavorProperties(key, charset, mime.getParameter("eoln"), mime.getParameter("terminators"))
                                End If

                                ' But don't store any of these parameters in the
                                ' DataFlavor itself for any text natives (even
                                ' non-charset ones). The SystemFlavorMap will
                                ' synthesize the appropriate mappings later.
                                mime.removeParameter("charset")
                                mime.removeParameter("class")
                                mime.removeParameter("eoln")
                                mime.removeParameter("terminators")
                                value = mime.ToString()
                            End If
                        Catch e As MimeTypeParseException
                            Console.WriteLine(e.ToString())
                            Console.Write(e.StackTrace)
                            Continue Do
                        End Try

                        Dim flavor As DataFlavor
                        Try
                            flavor = New DataFlavor(value)
                        Catch e As Exception
                            Try
                                flavor = New DataFlavor(value, Nothing)
                            Catch ee As Exception
                                ee.printStackTrace()
                                Continue Do
                            End Try
                        End Try

                        Dim dfs As New java.util.LinkedHashSet(Of DataFlavor)
                        dfs.add(flavor)

                        If "text".Equals(flavor.primaryType) Then
                            dfs.addAll(convertMimeTypeToDataFlavors(value))
                            store(flavor.mimeType.baseType, key, textTypeToNative)
                        End If

                        For Each df As DataFlavor In dfs
                            store(df, key, flavorToNative)
                            store(key, df, nativeToFlavor)
                        Next df
                    End If
                End If
            Loop
        End Sub

        ''' <summary>
        ''' Copied from java.util.Properties.
        ''' </summary>
        Private Function continueLine(ByVal line As String) As Boolean
            Dim slashCount As Integer = 0
            Dim index As Integer = line.Length() - 1
            Dim tempVar As Boolean = (index >= 0) AndAlso (line.Chars(index) = "\"c)
            index -= 1
            Do While tempVar
                slashCount += 1
                tempVar = (index >= 0) AndAlso (line.Chars(index) = "\"c)
                index -= 1
            Loop
            Return (slashCount Mod 2 = 1)
        End Function

        ''' <summary>
        ''' Copied from java.util.Properties.
        ''' </summary>
        Private Function loadConvert(ByVal theString As String) As String
            Dim aChar As Char
            Dim len As Integer = theString.Length()
            Dim outBuffer As New StringBuilder(len)

            Dim x As Integer = 0
            Do While x < len
                aChar = theString.Chars(x)
                x += 1
                If aChar = "\"c Then
                    aChar = theString.Chars(x)
                    x += 1
                    If aChar = "u"c Then
                        ' Read the xxxx
                        Dim value As Integer = 0
                        For i As Integer = 0 To 3
                            aChar = theString.Chars(x)
                            x += 1
                            Select Case aChar
                                Case "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c
                                    value = (value << 4) + AscW(aChar) - AscW("0"c)
                                    Exit Select
                                Case "a"c, "b"c, "c"c, "d"c, "e"c, "f"c
                                    value = (value << 4) + 10 + AscW(aChar) - AscW("a"c)
                                    Exit Select
                                Case "A"c, "B"c, "C"c, "D"c, "E"c, "F"c
                                    value = (value << 4) + 10 + AscW(aChar) - AscW("A"c)
                                    Exit Select
                                Case Else
                                    Throw New IllegalArgumentException("Malformed \uxxxx encoding.")
                            End Select
                        Next i
                        outBuffer.append(ChrW(value))
                    Else
                        If aChar = "t"c Then
                            aChar = ControlChars.Tab
                        ElseIf aChar = "r"c Then
                            aChar = ControlChars.Cr
                        ElseIf aChar = "n"c Then
                            aChar = ControlChars.Lf
                        ElseIf aChar = "f"c Then
                            aChar = ControlChars.FormFeed
                        End If
                        outBuffer.append(aChar)
                    End If
                Else
                    outBuffer.append(aChar)
                End If
            Loop
            Return outBuffer.ToString()
        End Function

        ''' <summary>
        ''' Stores the listed object under the specified hash key in map. Unlike a
        ''' standard map, the listed object will not replace any object already at
        ''' the appropriate Map location, but rather will be appended to a List
        ''' stored in that location.
        ''' </summary>
        Private Sub store(Of H, L)(ByVal hashed As H, ByVal listed As L, ByVal map As IDictionary(Of H, java.util.LinkedHashSet(Of L)))
            Dim list_Renamed As java.util.LinkedHashSet(Of L) = map(hashed)
            If list_Renamed Is Nothing Then
                list_Renamed = New java.util.LinkedHashSet(Of )(1)
                map(hashed) = list_Renamed
            End If
            If Not list_Renamed.contains(listed) Then list_Renamed.add(listed)
        End Sub

        ''' <summary>
        ''' Semantically equivalent to 'nativeToFlavor.get(nat)'. This method
        ''' handles the case where 'nat' is not found in 'nativeToFlavor'. In that
        ''' case, a new DataFlavor is synthesized, stored, and returned, if and
        ''' only if the specified native is encoded as a Java MIME type.
        ''' </summary>
        Private Function nativeToFlavorLookup(ByVal nat As String) As java.util.LinkedHashSet(Of DataFlavor)
            Dim flavors As java.util.LinkedHashSet(Of DataFlavor) = nativeToFlavor(nat)


            If nat IsNot Nothing AndAlso (Not disabledMappingGenerationKeys.contains(nat)) Then
                Dim transferer As sun.awt.datatransfer.DataTransferer = sun.awt.datatransfer.DataTransferer.instance
                If transferer IsNot Nothing Then
                    Dim platformFlavors As java.util.LinkedHashSet(Of DataFlavor) = transferer.getPlatformMappingsForNative(nat)
                    If platformFlavors.Count > 0 Then
                        If flavors IsNot Nothing Then platformFlavors.addAll(flavors)
                        flavors = platformFlavors
                    End If
                End If
            End If

            If flavors Is Nothing AndAlso isJavaMIMEType(nat) Then
                Dim decoded As String = decodeJavaMIMEType(nat)
                Dim flavor As DataFlavor = Nothing

                Try
                    flavor = New DataFlavor(decoded)
                Catch e As Exception
                    Console.Error.WriteLine("Exception """ & e.GetType().Name & ": " & e.Message & """while constructing DataFlavor for: " & decoded)
                End Try

                If flavor IsNot Nothing Then
                    flavors = New java.util.LinkedHashSet(Of )(1)
                    nativeToFlavor(nat) = flavors
                    flavors.add(flavor)
                    flavorsForNativeCache.remove(nat)

                    Dim natives As java.util.LinkedHashSet(Of String) = flavorToNative.Get(flavor)
                    If natives Is Nothing Then
                        natives = New java.util.LinkedHashSet(Of )(1)
                        flavorToNative.put(flavor, natives)
                    End If
                    natives.add(nat)
                    nativesForFlavorCache.remove(flavor)
                End If
            End If

            Return If(flavors IsNot Nothing, flavors, New java.util.LinkedHashSet(Of )(0))
        End Function

        ''' <summary>
        ''' Semantically equivalent to 'flavorToNative.get(flav)'. This method
        ''' handles the case where 'flav' is not found in 'flavorToNative' depending
        ''' on the value of passes 'synthesize' parameter. If 'synthesize' is
        ''' SYNTHESIZE_IF_NOT_FOUND a native is synthesized, stored, and returned by
        ''' encoding the DataFlavor's MIME type. Otherwise an empty List is returned
        ''' and 'flavorToNative' remains unaffected.
        ''' </summary>
        Private Function flavorToNativeLookup(ByVal flav As DataFlavor, ByVal synthesize As Boolean) As java.util.LinkedHashSet(Of String)

            Dim natives As java.util.LinkedHashSet(Of String) = flavorToNative.Get(flav)

            If flav IsNot Nothing AndAlso (Not disabledMappingGenerationKeys.contains(flav)) Then
                Dim transferer As sun.awt.datatransfer.DataTransferer = sun.awt.datatransfer.DataTransferer.instance
                If transferer IsNot Nothing Then
                    Dim platformNatives As java.util.LinkedHashSet(Of String) = transferer.getPlatformMappingsForFlavor(flav)
                    If platformNatives.Count > 0 Then
                        If natives IsNot Nothing Then platformNatives.addAll(natives)
                        natives = platformNatives
                    End If
                End If
            End If

            If natives Is Nothing Then
                If synthesize Then
                    Dim encoded As String = encodeDataFlavor(flav)
                    natives = New java.util.LinkedHashSet(Of )(1)
                    flavorToNative.put(flav, natives)
                    natives.add(encoded)

                    Dim flavors As java.util.LinkedHashSet(Of DataFlavor) = nativeToFlavor(encoded)
                    If flavors Is Nothing Then
                        flavors = New java.util.LinkedHashSet(Of )(1)
                        nativeToFlavor(encoded) = flavors
                    End If
                    flavors.add(flav)

                    nativesForFlavorCache.remove(flav)
                    flavorsForNativeCache.remove(encoded)
                Else
                    natives = New java.util.LinkedHashSet(Of )(0)
                End If
            End If

            Return New java.util.LinkedHashSet(Of )(natives)
        End Function

        ''' <summary>
        ''' Returns a <code>List</code> of <code>String</code> natives to which the
        ''' specified <code>DataFlavor</code> can be translated by the data transfer
        ''' subsystem. The <code>List</code> will be sorted from best native to
        ''' worst. That is, the first native will best reflect data in the specified
        ''' flavor to the underlying native platform.
        ''' <p>
        ''' If the specified <code>DataFlavor</code> is previously unknown to the
        ''' data transfer subsystem and the data transfer subsystem is unable to
        ''' translate this <code>DataFlavor</code> to any existing native, then
        ''' invoking this method will establish a
        ''' mapping in both directions between the specified <code>DataFlavor</code>
        ''' and an encoded version of its MIME type as its native.
        ''' </summary>
        ''' <param name="flav"> the <code>DataFlavor</code> whose corresponding natives
        '''        should be returned. If <code>null</code> is specified, all
        '''        natives currently known to the data transfer subsystem are
        '''        returned in a non-deterministic order. </param>
        ''' <returns> a <code>java.util.List</code> of <code>java.lang.String</code>
        '''         objects which are platform-specific representations of platform-
        '''         specific data formats
        ''' </returns>
        ''' <seealso cref= #encodeDataFlavor
        ''' @since 1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function getNativesForFlavor(ByVal flav As DataFlavor) As IList(Of String) Implements FlavorTable.getNativesForFlavor
            Dim retval As java.util.LinkedHashSet(Of String) = nativesForFlavorCache.check(flav)
            If retval IsNot Nothing Then Return New List(Of )(retval)

            If flav Is Nothing Then
                retval = New java.util.LinkedHashSet(Of )(nativeToFlavor.Keys)
            ElseIf disabledMappingGenerationKeys.contains(flav) Then
                ' In this case we shouldn't synthesize a native for this flavor,
                ' since its mappings were explicitly specified.
                retval = flavorToNativeLookup(flav, False)
            ElseIf sun.awt.datatransfer.DataTransferer.isFlavorCharsetTextType(flav) Then
                retval = New java.util.LinkedHashSet(Of )(0)

                ' For text/* flavors, flavor-to-native mappings specified in
                ' flavormap.properties are stored per flavor's base type.
                If "text".Equals(flav.primaryType) Then
                    Dim textTypeNatives As java.util.LinkedHashSet(Of String) = textTypeToNative.Get(flav.mimeType.baseType)
                    If textTypeNatives IsNot Nothing Then retval.addAll(textTypeNatives)
                End If

                ' Also include text/plain natives, but don't duplicate Strings
                Dim textTypeNatives As java.util.LinkedHashSet(Of String) = textTypeToNative.Get(TEXT_PLAIN_BASE_TYPE)
                If textTypeNatives IsNot Nothing Then retval.addAll(textTypeNatives)

                If retval.Count = 0 Then
                    retval = flavorToNativeLookup(flav, True)
                Else
                    ' In this branch it is guaranteed that natives explicitly
                    ' listed for flav's MIME type were added with
                    ' addUnencodedNativeForFlavor(), so they have lower priority.
                    retval.addAll(flavorToNativeLookup(flav, False))
                End If
            ElseIf sun.awt.datatransfer.DataTransferer.isFlavorNoncharsetTextType(flav) Then
                retval = textTypeToNative.Get(flav.mimeType.baseType)

                If retval Is Nothing OrElse retval.Count = 0 Then
                    retval = flavorToNativeLookup(flav, True)
                Else
                    ' In this branch it is guaranteed that natives explicitly
                    ' listed for flav's MIME type were added with
                    ' addUnencodedNativeForFlavor(), so they have lower priority.
                    retval.addAll(flavorToNativeLookup(flav, False))
                End If
            Else
                retval = flavorToNativeLookup(flav, True)
            End If

            nativesForFlavorCache.put(flav, retval)
            ' Create a copy, because client code can modify the returned list.
            Return New List(Of )(retval)
        End Function

        ''' <summary>
        ''' Returns a <code>List</code> of <code>DataFlavor</code>s to which the
        ''' specified <code>String</code> native can be translated by the data
        ''' transfer subsystem. The <code>List</code> will be sorted from best
        ''' <code>DataFlavor</code> to worst. That is, the first
        ''' <code>DataFlavor</code> will best reflect data in the specified
        ''' native to a Java application.
        ''' <p>
        ''' If the specified native is previously unknown to the data transfer
        ''' subsystem, and that native has been properly encoded, then invoking this
        ''' method will establish a mapping in both directions between the specified
        ''' native and a <code>DataFlavor</code> whose MIME type is a decoded
        ''' version of the native.
        ''' <p>
        ''' If the specified native is not a properly encoded native and the
        ''' mappings for this native have not been altered with
        ''' <code>setFlavorsForNative</code>, then the contents of the
        ''' <code>List</code> is platform dependent, but <code>null</code>
        ''' cannot be returned.
        ''' </summary>
        ''' <param name="nat"> the native whose corresponding <code>DataFlavor</code>s
        '''        should be returned. If <code>null</code> is specified, all
        '''        <code>DataFlavor</code>s currently known to the data transfer
        '''        subsystem are returned in a non-deterministic order. </param>
        ''' <returns> a <code>java.util.List</code> of <code>DataFlavor</code>
        '''         objects into which platform-specific data in the specified,
        '''         platform-specific native can be translated
        ''' </returns>
        ''' <seealso cref= #encodeJavaMIMEType
        ''' @since 1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function getFlavorsForNative(ByVal nat As String) As IList(Of DataFlavor) Implements FlavorTable.getFlavorsForNative
            Dim returnValue As java.util.LinkedHashSet(Of DataFlavor) = flavorsForNativeCache.check(nat)
            If returnValue IsNot Nothing Then
                Return New util.List(Of DataFlavor)(returnValue)
            Else
                returnValue = New java.util.LinkedHashSet(Of DataFlavor)
            End If

            If nat Is Nothing Then
                For Each n As String In getNativesForFlavor(Nothing)
                    returnValue.addAll(getFlavorsForNative(n))
                Next n
            Else
                Dim flavors As java.util.LinkedHashSet(Of DataFlavor) = nativeToFlavorLookup(nat)
                If disabledMappingGenerationKeys.contains(nat) Then Return New util.List(Of DataFlavor)(flavors)

                Dim flavorsWithSynthesized As java.util.LinkedHashSet(Of DataFlavor) = nativeToFlavorLookup(nat)

                For Each df As DataFlavor In flavorsWithSynthesized
                    returnValue.add(df)
                    If "text".Equals(df.primaryType) Then
                        Dim baseType As String = df.mimeType.baseType
                        returnValue.addAll(convertMimeTypeToDataFlavors(baseType))
                    End If
                Next df
            End If
            flavorsForNativeCache.put(nat, returnValue)
            Return New util.List(Of DataFlavor)(returnValue)
        End Function

        Private Shared Function convertMimeTypeToDataFlavors(ByVal baseType As String) As java.util.Set(Of DataFlavor)

            Dim returnValue As java.util.Set(Of DataFlavor) = New java.util.LinkedHashSet(Of DataFlavor)

            Dim subType As String = Nothing

            Try
                Dim mimeType_Renamed As New MimeType(baseType)
                subType = mimeType_Renamed.subType
            Catch mtpe As MimeTypeParseException
                ' Cannot happen, since we checked all mappings
                ' on load from flavormap.properties.
            End Try

            If sun.awt.datatransfer.DataTransferer.doesSubtypeSupportCharset(subType, Nothing) Then
                If TEXT_PLAIN_BASE_TYPE.Equals(baseType) Then returnValue.add(DataFlavor.stringFlavor)

                For Each unicodeClassName As String In UNICODE_TEXT_CLASSES
                    Dim mimeType_Renamed As String = baseType & ";charset=Unicode;class=" & unicodeClassName

                    Dim mimeTypes As java.util.LinkedHashSet(Of String) = handleHtmlMimeTypes(baseType, mimeType_Renamed)
                    For Each mt As String In mimeTypes
                        Dim toAdd As DataFlavor = Nothing
                        Try
                            toAdd = New DataFlavor(mt)
                        Catch cannotHappen As ClassNotFoundException
                        End Try
                        returnValue.add(toAdd)
                    Next mt
                Next unicodeClassName

                For Each charset As String In sun.awt.datatransfer.DataTransferer.standardEncodings()

                    For Each encodedTextClass As String In ENCODED_TEXT_CLASSES
                        Dim mimeType_Renamed As String = baseType & ";charset=" & charset & ";class=" & encodedTextClass

                        Dim mimeTypes As java.util.LinkedHashSet(Of String) = handleHtmlMimeTypes(baseType, mimeType_Renamed)

                        For Each mt As String In mimeTypes

                            Dim df As DataFlavor = Nothing

                            Try
                                df = New DataFlavor(mt)
                                ' Check for equality to plainTextFlavor so
                                ' that we can ensure that the exact charset of
                                ' plainTextFlavor, not the canonical charset
                                ' or another equivalent charset with a
                                ' different name, is used.
                                If df.Equals(DataFlavor.plainTextFlavor) Then df = DataFlavor.plainTextFlavor
                            Catch cannotHappen As ClassNotFoundException
                            End Try

                            returnValue.add(df)
                        Next mt
                    Next encodedTextClass
                Next charset

                If TEXT_PLAIN_BASE_TYPE.Equals(baseType) Then returnValue.add(DataFlavor.plainTextFlavor)
            Else
                ' Non-charset text natives should be treated as
                ' opaque, 8-bit data in any of its various
                ' representations.
                For Each encodedTextClassName As String In ENCODED_TEXT_CLASSES
                    Dim toAdd As DataFlavor = Nothing
                    Try
                        toAdd = New DataFlavor(baseType & ";class=" & encodedTextClassName)
                    Catch cannotHappen As ClassNotFoundException
                    End Try
                    returnValue.add(toAdd)
                Next encodedTextClassName
            End If
            Return returnValue
        End Function

        Private Shared ReadOnly htmlDocumntTypes As String() = {"all", "selection", "fragment"}

        Private Shared Function handleHtmlMimeTypes(ByVal baseType As String, ByVal mimeType_Renamed As String) As java.util.LinkedHashSet(Of String)

            Dim returnValues As New java.util.LinkedHashSet(Of String)

            If HTML_TEXT_BASE_TYPE.Equals(baseType) Then
                For Each documentType As String In htmlDocumntTypes
                    returnValues.add(mimeType_Renamed & ";document=" & documentType)
                Next documentType
            Else
                returnValues.add(mimeType_Renamed)
            End If

            Return returnValues
        End Function

        ''' <summary>
        ''' Returns a <code>Map</code> of the specified <code>DataFlavor</code>s to
        ''' their most preferred <code>String</code> native. Each native value will
        ''' be the same as the first native in the List returned by
        ''' <code>getNativesForFlavor</code> for the specified flavor.
        ''' <p>
        ''' If a specified <code>DataFlavor</code> is previously unknown to the
        ''' data transfer subsystem, then invoking this method will establish a
        ''' mapping in both directions between the specified <code>DataFlavor</code>
        ''' and an encoded version of its MIME type as its native.
        ''' </summary>
        ''' <param name="flavors"> an array of <code>DataFlavor</code>s which will be the
        '''        key set of the returned <code>Map</code>. If <code>null</code> is
        '''        specified, a mapping of all <code>DataFlavor</code>s known to the
        '''        data transfer subsystem to their most preferred
        '''        <code>String</code> natives will be returned. </param>
        ''' <returns> a <code>java.util.Map</code> of <code>DataFlavor</code>s to
        '''         <code>String</code> natives
        ''' </returns>
        ''' <seealso cref= #getNativesForFlavor </seealso>
        ''' <seealso cref= #encodeDataFlavor </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function getNativesForFlavors(ByVal flavors As DataFlavor()) As IDictionary(Of DataFlavor, String) Implements FlavorMap.getNativesForFlavors
            ' Use getNativesForFlavor to generate extra natives for text flavors
            ' and stringFlavor

            If flavors Is Nothing Then
                Dim flavor_list As IList(Of DataFlavor) = getFlavorsForNative(Nothing)
                flavors = New DataFlavor(flavor_list.Count - 1) {}
                flavor_list.ToArray(flavors)
            End If

            Dim retval As IDictionary(Of DataFlavor, String) = New Dictionary(Of DataFlavor, String)(flavors.Length, 1.0F)
            For Each flavor As DataFlavor In flavors
                Dim natives As IList(Of String) = getNativesForFlavor(flavor)
                Dim nat As String = If(natives.Count = 0, Nothing, natives(0))
                retval(flavor) = nat
            Next flavor

            Return retval
        End Function

        ''' <summary>
        ''' Returns a <code>Map</code> of the specified <code>String</code> natives
        ''' to their most preferred <code>DataFlavor</code>. Each
        ''' <code>DataFlavor</code> value will be the same as the first
        ''' <code>DataFlavor</code> in the List returned by
        ''' <code>getFlavorsForNative</code> for the specified native.
        ''' <p>
        ''' If a specified native is previously unknown to the data transfer
        ''' subsystem, and that native has been properly encoded, then invoking this
        ''' method will establish a mapping in both directions between the specified
        ''' native and a <code>DataFlavor</code> whose MIME type is a decoded
        ''' version of the native.
        ''' </summary>
        ''' <param name="natives"> an array of <code>String</code>s which will be the
        '''        key set of the returned <code>Map</code>. If <code>null</code> is
        '''        specified, a mapping of all supported <code>String</code> natives
        '''        to their most preferred <code>DataFlavor</code>s will be
        '''        returned. </param>
        ''' <returns> a <code>java.util.Map</code> of <code>String</code> natives to
        '''         <code>DataFlavor</code>s
        ''' </returns>
        ''' <seealso cref= #getFlavorsForNative </seealso>
        ''' <seealso cref= #encodeJavaMIMEType </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function getFlavorsForNatives(ByVal natives As String()) As IDictionary(Of String, DataFlavor) Implements FlavorMap.getFlavorsForNatives
            ' Use getFlavorsForNative to generate extra flavors for text natives
            If natives Is Nothing Then
                Dim nativesList As IList(Of String) = getNativesForFlavor(Nothing)
                natives = New String(nativesList.Count - 1) {}
                nativesList.ToArray(natives)
            End If

            Dim retval As IDictionary(Of String, DataFlavor) = New Dictionary(Of String, DataFlavor)(natives.Length, 1.0F)
            For Each aNative As String In natives
                Dim flavors As IList(Of DataFlavor) = getFlavorsForNative(aNative)
                Dim flav As DataFlavor = If(flavors.Count = 0, Nothing, flavors(0))
                retval(aNative) = flav
            Next aNative
            Return retval
        End Function

        ''' <summary>
        ''' Adds a mapping from the specified <code>DataFlavor</code> (and all
        ''' <code>DataFlavor</code>s equal to the specified <code>DataFlavor</code>)
        ''' to the specified <code>String</code> native.
        ''' Unlike <code>getNativesForFlavor</code>, the mapping will only be
        ''' established in one direction, and the native will not be encoded. To
        ''' establish a two-way mapping, call
        ''' <code>addFlavorForUnencodedNative</code> as well. The new mapping will
        ''' be of lower priority than any existing mapping.
        ''' This method has no effect if a mapping from the specified or equal
        ''' <code>DataFlavor</code> to the specified <code>String</code> native
        ''' already exists.
        ''' </summary>
        ''' <param name="flav"> the <code>DataFlavor</code> key for the mapping </param>
        ''' <param name="nat"> the <code>String</code> native value for the mapping </param>
        ''' <exception cref="NullPointerException"> if flav or nat is <code>null</code>
        ''' </exception>
        ''' <seealso cref= #addFlavorForUnencodedNative
        ''' @since 1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Sub addUnencodedNativeForFlavor(ByVal flav As DataFlavor, ByVal nat As String)
            java.util.Objects.requireNonNull(nat, "Null native not permitted")
            java.util.Objects.requireNonNull(flav, "Null flavor not permitted")

            Dim natives As java.util.LinkedHashSet(Of String) = flavorToNative.Get(flav)
            If natives Is Nothing Then
                natives = New java.util.LinkedHashSet(Of )(1)
                flavorToNative.put(flav, natives)
            End If
            natives.add(nat)
            nativesForFlavorCache.remove(flav)
        End Sub

        ''' <summary>
        ''' Discards the current mappings for the specified <code>DataFlavor</code>
        ''' and all <code>DataFlavor</code>s equal to the specified
        ''' <code>DataFlavor</code>, and creates new mappings to the
        ''' specified <code>String</code> natives.
        ''' Unlike <code>getNativesForFlavor</code>, the mappings will only be
        ''' established in one direction, and the natives will not be encoded. To
        ''' establish two-way mappings, call <code>setFlavorsForNative</code>
        ''' as well. The first native in the array will represent the highest
        ''' priority mapping. Subsequent natives will represent mappings of
        ''' decreasing priority.
        ''' <p>
        ''' If the array contains several elements that reference equal
        ''' <code>String</code> natives, this method will establish new mappings
        ''' for the first of those elements and ignore the rest of them.
        ''' <p>
        ''' It is recommended that client code not reset mappings established by the
        ''' data transfer subsystem. This method should only be used for
        ''' application-level mappings.
        ''' </summary>
        ''' <param name="flav"> the <code>DataFlavor</code> key for the mappings </param>
        ''' <param name="natives"> the <code>String</code> native values for the mappings </param>
        ''' <exception cref="NullPointerException"> if flav or natives is <code>null</code>
        '''         or if natives contains <code>null</code> elements
        ''' </exception>
        ''' <seealso cref= #setFlavorsForNative
        ''' @since 1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Sub setNativesForFlavor(ByVal flav As DataFlavor, ByVal natives As String())
            java.util.Objects.requireNonNull(natives, "Null natives not permitted")
            java.util.Objects.requireNonNull(flav, "Null flavors not permitted")

            flavorToNative.Remove(flav)
            For Each aNative As String In natives
                addUnencodedNativeForFlavor(flav, aNative)
            Next aNative
            disabledMappingGenerationKeys.add(flav)
            nativesForFlavorCache.remove(flav)
        End Sub

        ''' <summary>
        ''' Adds a mapping from a single <code>String</code> native to a single
        ''' <code>DataFlavor</code>. Unlike <code>getFlavorsForNative</code>, the
        ''' mapping will only be established in one direction, and the native will
        ''' not be encoded. To establish a two-way mapping, call
        ''' <code>addUnencodedNativeForFlavor</code> as well. The new mapping will
        ''' be of lower priority than any existing mapping.
        ''' This method has no effect if a mapping from the specified
        ''' <code>String</code> native to the specified or equal
        ''' <code>DataFlavor</code> already exists.
        ''' </summary>
        ''' <param name="nat"> the <code>String</code> native key for the mapping </param>
        ''' <param name="flav"> the <code>DataFlavor</code> value for the mapping </param>
        ''' <exception cref="NullPointerException"> if nat or flav is <code>null</code>
        ''' </exception>
        ''' <seealso cref= #addUnencodedNativeForFlavor
        ''' @since 1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Sub addFlavorForUnencodedNative(ByVal nat As String, ByVal flav As DataFlavor)
            java.util.Objects.requireNonNull(nat, "Null native not permitted")
            java.util.Objects.requireNonNull(flav, "Null flavor not permitted")

            Dim flavors As java.util.LinkedHashSet(Of DataFlavor) = nativeToFlavor(nat)
            If flavors Is Nothing Then
                flavors = New java.util.LinkedHashSet(Of DataFlavor)(1)
                nativeToFlavor(nat) = flavors
            End If
            flavors.add(flav)
            flavorsForNativeCache.remove(nat)
        End Sub

        ''' <summary>
        ''' Discards the current mappings for the specified <code>String</code>
        ''' native, and creates new mappings to the specified
        ''' <code>DataFlavor</code>s. Unlike <code>getFlavorsForNative</code>, the
        ''' mappings will only be established in one direction, and the natives need
        ''' not be encoded. To establish two-way mappings, call
        ''' <code>setNativesForFlavor</code> as well. The first
        ''' <code>DataFlavor</code> in the array will represent the highest priority
        ''' mapping. Subsequent <code>DataFlavor</code>s will represent mappings of
        ''' decreasing priority.
        ''' <p>
        ''' If the array contains several elements that reference equal
        ''' <code>DataFlavor</code>s, this method will establish new mappings
        ''' for the first of those elements and ignore the rest of them.
        ''' <p>
        ''' It is recommended that client code not reset mappings established by the
        ''' data transfer subsystem. This method should only be used for
        ''' application-level mappings.
        ''' </summary>
        ''' <param name="nat"> the <code>String</code> native key for the mappings </param>
        ''' <param name="flavors"> the <code>DataFlavor</code> values for the mappings </param>
        ''' <exception cref="NullPointerException"> if nat or flavors is <code>null</code>
        '''         or if flavors contains <code>null</code> elements
        ''' </exception>
        ''' <seealso cref= #setNativesForFlavor
        ''' @since 1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Sub setFlavorsForNative(ByVal nat As String, ByVal flavors As DataFlavor())
            java.util.Objects.requireNonNull(nat, "Null native not permitted")
            java.util.Objects.requireNonNull(flavors, "Null flavors not permitted")

            nativeToFlavor.Remove(nat)
            For Each flavor As DataFlavor In flavors
                addFlavorForUnencodedNative(nat, flavor)
            Next flavor
            disabledMappingGenerationKeys.add(nat)
            flavorsForNativeCache.remove(nat)
        End Sub

        ''' <summary>
        ''' Encodes a MIME type for use as a <code>String</code> native. The format
        ''' of an encoded representation of a MIME type is implementation-dependent.
        ''' The only restrictions are:
        ''' <ul>
        ''' <li>The encoded representation is <code>null</code> if and only if the
        ''' MIME type <code>String</code> is <code>null</code>.</li>
        ''' <li>The encoded representations for two non-<code>null</code> MIME type
        ''' <code>String</code>s are equal if and only if these <code>String</code>s
        ''' are equal according to <code>String.equals(Object)</code>.</li>
        ''' </ul>
        ''' <p>
        ''' The reference implementation of this method returns the specified MIME
        ''' type <code>String</code> prefixed with <code>JAVA_DATAFLAVOR:</code>.
        ''' </summary>
        ''' <param name="mimeType"> the MIME type to encode </param>
        ''' <returns> the encoded <code>String</code>, or <code>null</code> if
        '''         mimeType is <code>null</code> </returns>
        Public Shared Function encodeJavaMIMEType(ByVal mimeType_Renamed As String) As String
            Return If(mimeType_Renamed IsNot Nothing, JavaMIME + mimeType_Renamed, Nothing)
        End Function

        ''' <summary>
        ''' Encodes a <code>DataFlavor</code> for use as a <code>String</code>
        ''' native. The format of an encoded <code>DataFlavor</code> is
        ''' implementation-dependent. The only restrictions are:
        ''' <ul>
        ''' <li>The encoded representation is <code>null</code> if and only if the
        ''' specified <code>DataFlavor</code> is <code>null</code> or its MIME type
        ''' <code>String</code> is <code>null</code>.</li>
        ''' <li>The encoded representations for two non-<code>null</code>
        ''' <code>DataFlavor</code>s with non-<code>null</code> MIME type
        ''' <code>String</code>s are equal if and only if the MIME type
        ''' <code>String</code>s of these <code>DataFlavor</code>s are equal
        ''' according to <code>String.equals(Object)</code>.</li>
        ''' </ul>
        ''' <p>
        ''' The reference implementation of this method returns the MIME type
        ''' <code>String</code> of the specified <code>DataFlavor</code> prefixed
        ''' with <code>JAVA_DATAFLAVOR:</code>.
        ''' </summary>
        ''' <param name="flav"> the <code>DataFlavor</code> to encode </param>
        ''' <returns> the encoded <code>String</code>, or <code>null</code> if
        '''         flav is <code>null</code> or has a <code>null</code> MIME type </returns>
        Public Shared Function encodeDataFlavor(ByVal flav As DataFlavor) As String
            Return If(flav IsNot Nothing, SystemFlavorMap.encodeJavaMIMEType(flav.mimeType), Nothing)
        End Function

        ''' <summary>
        ''' Returns whether the specified <code>String</code> is an encoded Java
        ''' MIME type.
        ''' </summary>
        ''' <param name="str"> the <code>String</code> to test </param>
        ''' <returns> <code>true</code> if the <code>String</code> is encoded;
        '''         <code>false</code> otherwise </returns>
        Public Shared Function isJavaMIMEType(ByVal str As String) As Boolean
            Return (str IsNot Nothing AndAlso str.StartsWith(JavaMIME, 0))
        End Function

        ''' <summary>
        ''' Decodes a <code>String</code> native for use as a Java MIME type.
        ''' </summary>
        ''' <param name="nat"> the <code>String</code> to decode </param>
        ''' <returns> the decoded Java MIME type, or <code>null</code> if nat is not
        '''         an encoded <code>String</code> native </returns>
        Public Shared Function decodeJavaMIMEType(ByVal nat As String) As String
            Return If(isJavaMIMEType(nat), nat.Substring(JavaMIME.Length(), nat.Length() - (JavaMIME.Length())).Trim(), Nothing)
        End Function

        ''' <summary>
        ''' Decodes a <code>String</code> native for use as a
        ''' <code>DataFlavor</code>.
        ''' </summary>
        ''' <param name="nat"> the <code>String</code> to decode </param>
        ''' <returns> the decoded <code>DataFlavor</code>, or <code>null</code> if
        '''         nat is not an encoded <code>String</code> native </returns>
        Public Shared Function decodeDataFlavor(ByVal nat As String) As DataFlavor
            Dim retval_str As String = SystemFlavorMap.decodeJavaMIMEType(nat)
            Return If(retval_str IsNot Nothing, New DataFlavor(retval_str), Nothing)
        End Function

        Private NotInheritable Class SoftCache(Of K, V)
            Friend cache As IDictionary(Of K, SoftReference(Of java.util.LinkedHashSet(Of V)))

            Public Sub put(ByVal key As K, ByVal value As java.util.LinkedHashSet(Of V))
                If cache Is Nothing Then cache = New Dictionary(Of )(1)
                cache(key) = New SoftReference(Of )(value)
            End Sub

            Public Sub remove(ByVal key As K)
                If cache Is Nothing Then Return
                cache.Remove(Nothing)
                cache.Remove(key)
            End Sub

            Public Function check(ByVal key As K) As java.util.LinkedHashSet(Of V)
                If cache Is Nothing Then Return Nothing
                Dim ref As SoftReference(Of java.util.LinkedHashSet(Of V)) = cache(key)
                If ref IsNot Nothing Then Return ref.get()
                Return Nothing
            End Function
        End Class
    End Class

End Namespace
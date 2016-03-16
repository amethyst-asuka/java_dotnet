Imports System
Imports System.Collections.Generic
Imports java.lang.invoke.LambdaForm

'
' * Copyright (c) 2013, 2014, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.invoke


    ''' <summary>
    ''' Working storage for an LF that is being transformed.
    '''  Similarly to a StringBuffer, the editing can take place in multiple steps.
    ''' </summary>
    Friend NotInheritable Class LambdaFormBuffer
        Private arity, length As Integer
        Private names As Name()
        Private originalNames As Name() ' snapshot of pre-transaction names
        Private flags As SByte
        Private firstChange As Integer
        Private resultName As Name
        Private debugName As String
        Private dups As List(Of Name)

        Private Const F_TRANS As Integer = &H10, F_OWNED As Integer = &H3

        Friend Sub New(ByVal lf As LambdaForm)
            Me.arity = lf.arity_Renamed
            names = lf.names
            Dim result_Renamed As Integer = lf.result
            If result_Renamed = LAST_RESULT Then result_Renamed = length - 1
            If result_Renamed >= 0 AndAlso lf.names(result_Renamed).type_Renamed <> V_TYPE Then resultName = lf.names(result_Renamed)
            debugName = lf.debugName
            Assert(lf.nameRefsAreLegal())
        End Sub

        Private Function lambdaForm() As LambdaForm
            Assert((Not inTrans())) ' need endEdit call to tidy things up
            Return New LambdaForm(debugName, arity, nameArray(), resultIndex())
        End Function

        Friend Function name(ByVal i As Integer) As Name
            Assert(i < length)
            Return names(i)
        End Function

        Friend Function nameArray() As Name()
            Return java.util.Arrays.copyOf(names, length)
        End Function

        Friend Function resultIndex() As Integer
            If resultName Is Nothing Then Return VOID_RESULT
            Dim index As Integer = indexOf(resultName, names)
            Assert(index >= 0)
            Return index
        End Function

        Friend Property names As Name()
            Set(ByVal names2 As Name())
                originalNames = names2
                names = originalNames
                length = names2.Length
                flags = 0
            End Set
        End Property

        Private Function verifyArity() As Boolean
            Dim i As Integer = 0
            Do While i < arity AndAlso i < firstChange
                Assert(names(i).param) :   "#" & i & "=" & names(i)
				i += 1
            Loop
            For i As Integer = arity To length - 1
                Assert((Not names(i).param)) :   "#" & i & "=" & names(i)
			Next i
            For i As Integer = length To names.Length - 1
                Assert(names(i) Is Nothing) :   "#" & i & "=" & names(i)
			Next i
            ' check resultName also
            If resultName IsNot Nothing Then
                Dim resultIndex As Integer = indexOf(resultName, names)
                Assert(resultIndex >= 0) :   "not found: " & resultName.exprString() + java.util.Arrays.asList(names)
				Assert(names(resultIndex) Is resultName)
            End If
            Return True
        End Function

        Private Function verifyFirstChange() As Boolean
            Assert(inTrans())
            For i As Integer = 0 To length - 1
                If names(i) IsNot originalNames(i) Then
                    Assert(firstChange = i) : java.util.Arrays.asList(firstChange, i, originalNames(i).exprString(), java.util.Arrays.asList(names))
                    Return True
                End If
            Next i
            Assert(firstChange = length) : java.util.Arrays.asList(firstChange, java.util.Arrays.asList(names))
            Return True
        End Function

        Private Shared Function indexOf(ByVal fn As NamedFunction, ByVal fns As NamedFunction()) As Integer
            For i As Integer = 0 To fns.Length - 1
                If fns(i) Is fn Then Return i
            Next i
            Return -1
        End Function

        Private Shared Function indexOf(ByVal n As Name, ByVal ns As Name()) As Integer
            For i As Integer = 0 To ns.Length - 1
                If ns(i) Is n Then Return i
            Next i
            Return -1
        End Function

        Friend Function inTrans() As Boolean
            Return (flags And F_TRANS) <> 0
        End Function

        Friend Function ownedCount() As Integer
            Return flags And F_OWNED
        End Function

        Friend Sub growNames(ByVal insertPos As Integer, ByVal growLength As Integer)
            Dim oldLength As Integer = length
            Dim newLength As Integer = oldLength + growLength
            Dim oc As Integer = ownedCount()
            If oc = 0 OrElse newLength > names.Length Then
                names = java.util.Arrays.copyOf(names, (names.Length + growLength) * 5 \ 4)
                If oc = 0 Then
                    flags += 1
                    oc += 1
                    Assert(ownedCount() = oc)
                End If
            End If
            If originalNames IsNot Nothing AndAlso originalNames.Length < names.Length Then
                originalNames = java.util.Arrays.copyOf(originalNames, names.Length)
                If oc = 1 Then
                    flags += 1
                    oc += 1
                    Assert(ownedCount() = oc)
                End If
            End If
            If growLength = 0 Then Return
            Dim insertEnd As Integer = insertPos + growLength
            Dim tailLength As Integer = oldLength - insertPos
            Array.Copy(names, insertPos, names, insertEnd, tailLength)
            java.util.Arrays.fill(names, insertPos, insertEnd, Nothing)
            If originalNames IsNot Nothing Then
                Array.Copy(originalNames, insertPos, originalNames, insertEnd, tailLength)
                java.util.Arrays.fill(originalNames, insertPos, insertEnd, Nothing)
            End If
            length = newLength
            If firstChange >= insertPos Then firstChange += growLength
        End Sub

        Friend Function lastIndexOf(ByVal n As Name) As Integer
            Dim result_Renamed As Integer = -1
            For i As Integer = 0 To length - 1
                If names(i) Is n Then result_Renamed = i
            Next i
            Return result_Renamed
        End Function

        ''' <summary>
        ''' We have just overwritten the name at pos1 with the name at pos2.
        '''  This means that there are two copies of the name, which we will have to fix later.
        ''' </summary>
        Private Sub noteDuplicate(ByVal pos1 As Integer, ByVal pos2 As Integer)
            Dim n As Name = names(pos1)
            Assert(n Is names(pos2))
            Assert(originalNames(pos1) IsNot Nothing) ' something was replaced at pos1
            Assert(originalNames(pos2) Is Nothing OrElse originalNames(pos2) Is n)
            If dups Is Nothing Then dups = New List(Of )
            dups.Add(n)
        End Sub

        ''' <summary>
        ''' Replace duplicate names by nulls, and remove all nulls. </summary>
        Private Sub clearDuplicatesAndNulls()
            If dups IsNot Nothing Then
                ' Remove duplicates.
                Assert(ownedCount() >= 1)
                For Each dup As Name In dups
                    For i As Integer = firstChange To length - 1
                        If names(i) Is dup AndAlso originalNames(i) IsNot dup Then
                            names(i) = Nothing
                            Assert(java.util.Arrays.asList(names).contains(dup))
                            Exit For ' kill only one dup
                        End If
                    Next i
                Next dup
                dups.Clear()
            End If
            ' Now that we are done with originalNames, remove "killed" names.
            Dim oldLength As Integer = length
            Dim i As Integer = firstChange
            Do While i < length
                If names(i) Is Nothing Then
                    length -= 1
                    Array.Copy(names, i + 1, names, i, (length - i))
                    i -= 1 ' restart loop at this position
                End If
                i += 1
            Loop
            If length < oldLength Then java.util.Arrays.fill(names, length, oldLength, Nothing)
            Assert((Not java.util.Arrays.asList(names).subList(0, length).contains(Nothing)))
        End Sub

        ''' <summary>
        ''' Create a private, writable copy of names.
        '''  Preserve the original copy, for reference.
        ''' </summary>
        Friend Sub startEdit()
            Assert(verifyArity())
            Dim oc As Integer = ownedCount()
            Assert((Not inTrans())) ' no nested transactions
            flags = flags Or F_TRANS
            Dim oldNames As Name() = names
            Dim ownBuffer As Name() = (If(oc = 2, originalNames, Nothing))
            Assert(ownBuffer <> oldNames)
            If ownBuffer IsNot Nothing AndAlso ownBuffer.Length >= length Then
                names = copyNamesInto(ownBuffer)
            Else
                ' make a new buffer to hold the names
                Const SLOP As Integer = 2
                names = java.util.Arrays.copyOf(oldNames, System.Math.max(length + SLOP, oldNames.Length))
                If oc < 2 Then flags += 1
                Assert(ownedCount() = oc + 1)
            End If
            originalNames = oldNames
            Assert(originalNames <> names)
            firstChange = length
            Assert(inTrans())
        End Sub

        Private Sub changeName(ByVal i As Integer, ByVal name As Name)
            Assert(inTrans())
            Assert(i < length)
            Dim oldName As Name = names(i)
            Assert(oldName Is originalNames(i)) ' no multiple changes
            Assert(verifyFirstChange())
            If ownedCount() = 0 Then growNames(0, 0)
            names(i) = name
            If firstChange > i Then firstChange = i
            If resultName IsNot Nothing AndAlso resultName Is oldName Then resultName = name
        End Sub

        ''' <summary>
        ''' Change the result name.  Null means a  Sub  result. </summary>
        Friend Property result As Name
            Set(ByVal name As Name)
                Assert(name Is Nothing OrElse lastIndexOf(name) >= 0)
                resultName = name
            End Set
        End Property

        ''' <summary>
        ''' Finish a transaction. </summary>
        Friend Function endEdit() As LambdaForm
            Assert(verifyFirstChange())
            ' Assuming names have been changed pairwise from originalNames[i] to names[i],
            ' update arguments to ensure referential integrity.
            For i As Integer = System.Math.max(firstChange, arity) To length - 1
                Dim name As Name = names(i)
                If name Is Nothing Then ' space for removed duplicate Continue For
                    Dim newName As Name = name.replaceNames(originalNames, names, firstChange, i)
                    If newName IsNot name Then
                        names(i) = newName
                        If resultName Is name Then resultName = newName
                    End If
            Next i
            Assert(inTrans())
            flags = flags And Not F_TRANS
            clearDuplicatesAndNulls()
            originalNames = Nothing
            ' If any parameters have been changed, then reorder them as needed.
            ' This is a "sheep-and-goats" stable sort, pushing all non-parameters
            ' to the right of all parameters.
            If firstChange < arity Then
                Dim exprs As Name() = New Name(arity - firstChange - 1) {}
                Dim argp As Integer = firstChange, exprp As Integer = 0
                For i As Integer = firstChange To arity - 1
                    Dim name As Name = names(i)
                    If name.param Then
                        names(argp) = name
                        argp += 1
                    Else
                        exprs(exprp) = name
                        exprp += 1
                    End If
                Next i
                Assert(exprp = (arity - argp))
                ' copy the exprs just after the last remaining param
                Array.Copy(exprs, 0, names, argp, exprp)
                ' adjust arity
                arity -= exprp
            End If
            Assert(verifyArity())
            Return lambdaForm()
        End Function

        Private Function copyNamesInto(ByVal buffer As Name()) As Name()
            Array.Copy(names, 0, buffer, 0, length)
            java.util.Arrays.fill(buffer, length, buffer.Length, Nothing)
            Return buffer
        End Function

        ''' <summary>
        ''' Replace any Name whose function is in oldFns with a copy
        '''  whose function is in the corresponding position in newFns.
        '''  Only do this if the arguments are exactly equal to the given.
        ''' </summary>
        Friend Function replaceFunctions(ByVal oldFns As NamedFunction(), ByVal newFns As NamedFunction(), ParamArray ByVal forArguments As Object()) As LambdaFormBuffer
            Assert(inTrans())
            If oldFns.Length = 0 Then Return Me
            For i As Integer = arity To length - 1
                Dim n As Name = names(i)
                Dim nfi As Integer = indexOf(n.function, oldFns)
                If nfi >= 0 AndAlso java.util.Arrays.Equals(n.arguments, forArguments) Then changeName(i, New Name(newFns(nfi), n.arguments))
            Next i
            Return Me
        End Function

        Private Sub replaceName(ByVal pos As Integer, ByVal binding As Name)
            Assert(inTrans())
            Assert(verifyArity())
            Assert(pos < arity)
            Dim param As Name = names(pos)
            Assert(param.param)
            Assert(param.type = binding.type)
            changeName(pos, binding)
        End Sub

        ''' <summary>
        ''' Replace a parameter by a fresh parameter. </summary>
        Friend Function renameParameter(ByVal pos As Integer, ByVal newParam As Name) As LambdaFormBuffer
            Assert(newParam.param)
            replaceName(pos, newParam)
            Return Me
        End Function

        ''' <summary>
        ''' Replace a parameter by a fresh expression. </summary>
        Friend Function replaceParameterByNewExpression(ByVal pos As Integer, ByVal binding As Name) As LambdaFormBuffer
            Assert((Not binding.param))
            Assert(lastIndexOf(binding) < 0) ' else use replaceParameterByCopy
            replaceName(pos, binding)
            Return Me
        End Function

        ''' <summary>
        ''' Replace a parameter by another parameter or expression already in the form. </summary>
        Friend Function replaceParameterByCopy(ByVal pos As Integer, ByVal valuePos As Integer) As LambdaFormBuffer
            Assert(pos <> valuePos)
            replaceName(pos, names(valuePos))
            noteDuplicate(pos, valuePos) ' temporarily, will occur twice in the names array
            Return Me
        End Function

        Private Sub insertName(ByVal pos As Integer, ByVal expr As Name, ByVal isParameter As Boolean)
            Assert(inTrans())
            Assert(verifyArity())
            Assert(If(isParameter, pos <= arity, pos >= arity))
            growNames(pos, 1)
            If isParameter Then arity += 1
            changeName(pos, expr)
        End Sub

        ''' <summary>
        ''' Insert a fresh expression. </summary>
        Friend Function insertExpression(ByVal pos As Integer, ByVal expr As Name) As LambdaFormBuffer
            Assert((Not expr.param))
            insertName(pos, expr, False)
            Return Me
        End Function

        ''' <summary>
        ''' Insert a fresh parameter. </summary>
        Friend Function insertParameter(ByVal pos As Integer, ByVal param As Name) As LambdaFormBuffer
            Assert(param.param)
            insertName(pos, param, True)
            Return Me
        End Function
    End Class

End Namespace
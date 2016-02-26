Imports System
Imports System.Text

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

Namespace javax.swing.tree


	''' <summary>
	''' {@code TreePath} represents an array of objects that uniquely
	''' identify the path to a node in a tree. The elements of the array
	''' are ordered with the root as the first element of the array. For
	''' example, a file on the file system is uniquely identified based on
	''' the array of parent directories and the name of the file. The path
	''' {@code /tmp/foo/bar} could be represented by a {@code TreePath} as
	''' {@code new TreePath(new Object[] {"tmp", "foo", "bar"})}.
	''' <p>
	''' {@code TreePath} is used extensively by {@code JTree} and related classes.
	''' For example, {@code JTree} represents the selection as an array of
	''' {@code TreePath}s. When used with {@code JTree}, the elements of the
	''' path are the objects returned from the {@code TreeModel}. When {@code JTree}
	''' is paired with {@code DefaultTreeModel}, the elements of the
	''' path are {@code TreeNode}s. The following example illustrates extracting
	''' the user object from the selection of a {@code JTree}:
	''' <pre>
	'''   DefaultMutableTreeNode root = ...;
	'''   DefaultTreeModel model = new DefaultTreeModel(root);
	'''   JTree tree = new JTree(model);
	'''   ...
	'''   TreePath selectedPath = tree.getSelectionPath();
	'''   DefaultMutableTreeNode selectedNode =
	'''       ((DefaultMutableTreeNode)selectedPath.getLastPathComponent()).
	'''       getUserObject();
	''' </pre>
	''' Subclasses typically need override only {@code
	''' getLastPathComponent}, and {@code getParentPath}. As {@code JTree}
	''' internally creates {@code TreePath}s at various points, it's
	''' generally not useful to subclass {@code TreePath} and use with
	''' {@code JTree}.
	''' <p>
	''' While {@code TreePath} is serializable, a {@code
	''' NotSerializableException} is thrown if any elements of the path are
	''' not serializable.
	''' <p>
	''' For further information and examples of using tree paths,
	''' see <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/tree.html">How to Use Trees</a>
	''' in <em>The Java Tutorial.</em>
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Scott Violet
	''' @author Philip Milne
	''' </summary>
	<Serializable> _
	Public Class TreePath
		Inherits Object

		''' <summary>
		''' Path representing the parent, null if lastPathComponent represents
		''' the root. 
		''' </summary>
		Private parentPath As TreePath
		''' <summary>
		''' Last path component. </summary>
		Private lastPathComponent As Object

		''' <summary>
		''' Creates a {@code TreePath} from an array. The array uniquely
		''' identifies the path to a node.
		''' </summary>
		''' <param name="path"> an array of objects representing the path to a node </param>
		''' <exception cref="IllegalArgumentException"> if {@code path} is {@code null},
		'''         empty, or contains a {@code null} value </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal path As Object())
			If path Is Nothing OrElse path.Length = 0 Then Throw New System.ArgumentException("path in TreePath must be non null and not empty.")
			lastPathComponent = path(path.Length - 1)
			If lastPathComponent Is Nothing Then Throw New System.ArgumentException("Last path component must be non-null")
			If path.Length > 1 Then parentPath = New TreePath(path, path.Length - 1)
		End Sub

		''' <summary>
		''' Creates a {@code TreePath} containing a single element. This is
		''' used to construct a {@code TreePath} identifying the root.
		''' </summary>
		''' <param name="lastPathComponent"> the root </param>
		''' <seealso cref= #TreePath(Object[]) </seealso>
		''' <exception cref="IllegalArgumentException"> if {@code lastPathComponent} is
		'''         {@code null} </exception>
		Public Sub New(ByVal lastPathComponent As Object)
			If lastPathComponent Is Nothing Then Throw New System.ArgumentException("path in TreePath must be non null.")
			Me.lastPathComponent = lastPathComponent
			parentPath = Nothing
		End Sub

		''' <summary>
		''' Creates a {@code TreePath} with the specified parent and element.
		''' </summary>
		''' <param name="parent"> the path to the parent, or {@code null} to indicate
		'''        the root </param>
		''' <param name="lastPathComponent"> the last path element </param>
		''' <exception cref="IllegalArgumentException"> if {@code lastPathComponent} is
		'''         {@code null} </exception>
		Protected Friend Sub New(ByVal parent As TreePath, ByVal lastPathComponent As Object)
			If lastPathComponent Is Nothing Then Throw New System.ArgumentException("path in TreePath must be non null.")
			parentPath = parent
			Me.lastPathComponent = lastPathComponent
		End Sub

		''' <summary>
		''' Creates a {@code TreePath} from an array. The returned
		''' {@code TreePath} represents the elements of the array from
		''' {@code 0} to {@code length - 1}.
		''' <p>
		''' This constructor is used internally, and generally not useful outside
		''' of subclasses.
		''' </summary>
		''' <param name="path"> the array to create the {@code TreePath} from </param>
		''' <param name="length"> identifies the number of elements in {@code path} to
		'''        create the {@code TreePath} from </param>
		''' <exception cref="NullPointerException"> if {@code path} is {@code null} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code length - 1} is
		'''         outside the range of the array </exception>
		''' <exception cref="IllegalArgumentException"> if any of the elements from
		'''         {@code 0} to {@code length - 1} are {@code null} </exception>
		Protected Friend Sub New(ByVal path As Object(), ByVal length As Integer)
			lastPathComponent = path(length - 1)
			If lastPathComponent Is Nothing Then Throw New System.ArgumentException("Path elements must be non-null")
			If length > 1 Then parentPath = New TreePath(path, length - 1)
		End Sub

		''' <summary>
		''' Creates an empty {@code TreePath}.  This is provided for
		''' subclasses that represent paths in a different
		''' manner. Subclasses that use this constructor must override
		''' {@code getLastPathComponent}, and {@code getParentPath}.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns an ordered array of the elements of this {@code TreePath}.
		''' The first element is the root.
		''' </summary>
		''' <returns> an array of the elements in this {@code TreePath} </returns>
		Public Overridable Property path As Object()
			Get
				Dim i As Integer = pathCount
				Dim result As Object() = New Object(i - 1){}
				i -= 1
    
				Dim ___path As TreePath = Me
				Do While ___path IsNot Nothing
					result(i) = ___path.lastPathComponent
					i -= 1
					___path = ___path.parentPath
				Loop
				Return result
			End Get
		End Property

		''' <summary>
		''' Returns the last element of this path.
		''' </summary>
		''' <returns> the last element in the path </returns>
		Public Overridable Property lastPathComponent As Object
			Get
				Return lastPathComponent
			End Get
		End Property

		''' <summary>
		''' Returns the number of elements in the path.
		''' </summary>
		''' <returns> the number of elements in the path </returns>
		Public Overridable Property pathCount As Integer
			Get
				Dim result As Integer = 0
				Dim ___path As TreePath = Me
				Do While ___path IsNot Nothing
					result += 1
					___path = ___path.parentPath
				Loop
				Return result
			End Get
		End Property

		''' <summary>
		''' Returns the path element at the specified index.
		''' </summary>
		''' <param name="index"> the index of the element requested </param>
		''' <returns> the element at the specified index </returns>
		''' <exception cref="IllegalArgumentException"> if the index is outside the
		'''         range of this path </exception>
		Public Overridable Function getPathComponent(ByVal index As Integer) As Object
			Dim pathLength As Integer = pathCount

			If index < 0 OrElse index >= pathLength Then Throw New System.ArgumentException("Index " & index & " is out of the specified range")

			Dim ___path As TreePath = Me

			Dim i As Integer = pathLength-1
			Do While i <> index
				___path = ___path.parentPath
				i -= 1
			Loop
			Return ___path.lastPathComponent
		End Function

		''' <summary>
		''' Compares this {@code TreePath} to the specified object. This returns
		''' {@code true} if {@code o} is a {@code TreePath} with the exact
		''' same elements (as determined by using {@code equals} on each
		''' element of the path).
		''' </summary>
		''' <param name="o"> the object to compare </param>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If TypeOf o Is TreePath Then
				Dim oTreePath As TreePath = CType(o, TreePath)

				If pathCount <> oTreePath.pathCount Then Return False
				Dim ___path As TreePath = Me
				Do While ___path IsNot Nothing
					If Not(___path.lastPathComponent.Equals(oTreePath.lastPathComponent)) Then Return False
					oTreePath = oTreePath.parentPath
					___path = ___path.parentPath
				Loop
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Returns the hash code of this {@code TreePath}. The hash code of a
		''' {@code TreePath} is the hash code of the last element in the path.
		''' </summary>
		''' <returns> the hashCode for the object </returns>
		Public Overrides Function GetHashCode() As Integer
			Return lastPathComponent.GetHashCode()
		End Function

		''' <summary>
		''' Returns true if <code>aTreePath</code> is a
		''' descendant of this
		''' {@code TreePath}. A {@code TreePath} {@code P1} is a descendant of a
		''' {@code TreePath} {@code P2}
		''' if {@code P1} contains all of the elements that make up
		''' {@code P2's} path.
		''' For example, if this object has the path {@code [a, b]},
		''' and <code>aTreePath</code> has the path {@code [a, b, c]},
		''' then <code>aTreePath</code> is a descendant of this object.
		''' However, if <code>aTreePath</code> has the path {@code [a]},
		''' then it is not a descendant of this object.  By this definition
		''' a {@code TreePath} is always considered a descendant of itself.
		''' That is, <code>aTreePath.isDescendant(aTreePath)</code> returns
		''' {@code true}.
		''' </summary>
		''' <param name="aTreePath"> the {@code TreePath} to check </param>
		''' <returns> true if <code>aTreePath</code> is a descendant of this path </returns>
		Public Overridable Function isDescendant(ByVal aTreePath As TreePath) As Boolean
			If aTreePath Is Me Then Return True

			If aTreePath IsNot Nothing Then
				Dim pathLength As Integer = pathCount
				Dim oPathLength As Integer = aTreePath.pathCount

				If oPathLength < pathLength Then Return False
				Dim tempVar As Boolean = oPathLength > pathLength
				oPathLength -= 1
				Do While tempVar
					aTreePath = aTreePath.parentPath
					tempVar = oPathLength > pathLength
					oPathLength -= 1
				Loop
				Return Equals(aTreePath)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a new path containing all the elements of this path
		''' plus <code>child</code>. <code>child</code> is the last element
		''' of the newly created {@code TreePath}.
		''' </summary>
		''' <param name="child"> the path element to add </param>
		''' <exception cref="NullPointerException"> if {@code child} is {@code null} </exception>
		Public Overridable Function pathByAddingChild(ByVal child As Object) As TreePath
			If child Is Nothing Then Throw New NullPointerException("Null child not allowed")

			Return New TreePath(Me, child)
		End Function

		''' <summary>
		''' Returns the {@code TreePath} of the parent. A return value of
		''' {@code null} indicates this is the root node.
		''' </summary>
		''' <returns> the parent path </returns>
		Public Overridable Property parentPath As TreePath
			Get
				Return parentPath
			End Get
		End Property

		''' <summary>
		''' Returns a string that displays and identifies this
		''' object's properties.
		''' </summary>
		''' <returns> a String representation of this object </returns>
		Public Overrides Function ToString() As String
			Dim tempSpot As New StringBuilder("[")

			Dim counter As Integer = 0
			Dim maxCounter As Integer = pathCount
			Do While counter < maxCounter
				If counter > 0 Then tempSpot.Append(", ")
				tempSpot.Append(getPathComponent(counter))
				counter += 1
			Loop
			tempSpot.Append("]")
			Return tempSpot.ToString()
		End Function
	End Class

End Namespace
Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>MediaTracker</code> class is a utility class to track
	''' the status of a number of media objects. Media objects could
	''' include audio clips as well as images, though currently only
	''' images are supported.
	''' <p>
	''' To use a media tracker, create an instance of
	''' <code>MediaTracker</code> and call its <code>addImage</code>
	''' method for each image to be tracked. In addition, each image can
	''' be assigned a unique identifier. This identifier controls the
	''' priority order in which the images are fetched. It can also be used
	''' to identify unique subsets of the images that can be waited on
	''' independently. Images with a lower ID are loaded in preference to
	''' those with a higher ID number.
	''' 
	''' <p>
	''' 
	''' Tracking an animated image
	''' might not always be useful
	''' due to the multi-part nature of animated image
	''' loading and painting,
	''' but it is supported.
	''' <code>MediaTracker</code> treats an animated image
	''' as completely loaded
	''' when the first frame is completely loaded.
	''' At that point, the <code>MediaTracker</code>
	''' signals any waiters
	''' that the image is completely loaded.
	''' If no <code>ImageObserver</code>s are observing the image
	''' when the first frame has finished loading,
	''' the image might flush itself
	''' to conserve resources
	''' (see <seealso cref="Image#flush()"/>).
	''' 
	''' <p>
	''' Here is an example of using <code>MediaTracker</code>:
	''' <p>
	''' <hr><blockquote><pre>{@code
	''' import java.applet.Applet;
	''' import java.awt.Color;
	''' import java.awt.Image;
	''' import java.awt.Graphics;
	''' import java.awt.MediaTracker;
	''' 
	''' public class ImageBlaster extends Applet implements Runnable {
	'''      MediaTracker tracker;
	'''      Image bg;
	'''      Image anim[] = new Image[5];
	'''      int index;
	'''      Thread animator;
	''' 
	'''      // Get the images for the background (id == 0)
	'''      // and the animation frames (id == 1)
	'''      // and add them to the MediaTracker
	'''      public  Sub  init() {
	'''          tracker = new MediaTracker(this);
	'''          bg = getImage(getDocumentBase(),
	'''                  "images/background.gif");
	'''          tracker.addImage(bg, 0);
	'''          for (int i = 0; i < 5; i++) {
	'''              anim[i] = getImage(getDocumentBase(),
	'''                      "images/anim"+i+".gif");
	'''              tracker.addImage(anim[i], 1);
	'''          }
	'''      }
	''' 
	'''      // Start the animation thread.
	'''      public  Sub  start() {
	'''          animator = new Thread(this);
	'''          animator.start();
	'''      }
	''' 
	'''      // Stop the animation thread.
	'''      public  Sub  stop() {
	'''          animator = null;
	'''      }
	''' 
	'''      // Run the animation thread.
	'''      // First wait for the background image to fully load
	'''      // and paint.  Then wait for all of the animation
	'''      // frames to finish loading. Finally, loop and
	'''      // increment the animation frame index.
	'''      public  Sub  run() {
	'''          try {
	'''              tracker.waitForID(0);
	'''              tracker.waitForID(1);
	'''          } catch (InterruptedException e) {
	'''              return;
	'''          }
	'''          Thread me = Thread.currentThread();
	'''          while (animator == me) {
	'''              try {
	'''                  Thread.sleep(100);
	'''              } catch (InterruptedException e) {
	'''                  break;
	'''              }
	'''              synchronized (this) {
	'''                  index++;
	'''                  if (index >= anim.length) {
	'''                      index = 0;
	'''                  }
	'''              }
	'''              repaint();
	'''          }
	'''      }
	''' 
	'''      // The background image fills the frame so we
	'''      // don't need to clear the applet on repaints.
	'''      // Just call the paint method.
	'''      public  Sub  update(Graphics g) {
	'''          paint(g);
	'''      }
	''' 
	'''      // Paint a large red rectangle if there are any errors
	'''      // loading the images.  Otherwise always paint the
	'''      // background so that it appears incrementally as it
	'''      // is loading.  Finally, only paint the current animation
	'''      // frame if all of the frames (id == 1) are done loading,
	'''      // so that we don't get partial animations.
	'''      public  Sub  paint(Graphics g) {
	'''          if ((tracker.statusAll(false) & MediaTracker.ERRORED) != 0) {
	'''              g.setColor(Color.red);
	'''              g.fillRect(0, 0, size().width, size().height);
	'''              return;
	'''          }
	'''          g.drawImage(bg, 0, 0, this);
	'''          if (tracker.statusID(1, false) == MediaTracker.COMPLETE) {
	'''              g.drawImage(anim[index], 10, 10, this);
	'''          }
	'''      }
	''' }
	''' } </pre></blockquote><hr>
	''' 
	''' @author      Jim Graham
	''' @since       JDK1.0
	''' </summary>
	<Serializable> _
	Public Class MediaTracker

		''' <summary>
		''' A given <code>Component</code> that will be
		''' tracked by a media tracker where the image will
		''' eventually be drawn.
		''' 
		''' @serial </summary>
		''' <seealso cref= #MediaTracker(Component) </seealso>
		Friend target As java.awt.Component
		''' <summary>
		''' The head of the list of <code>Images</code> that is being
		''' tracked by the <code>MediaTracker</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #addImage(Image, int) </seealso>
		''' <seealso cref= #removeImage(Image) </seealso>
		Friend head As MediaEntry

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -483174189758638095L

		''' <summary>
		''' Creates a media tracker to track images for a given component. </summary>
		''' <param name="comp"> the component on which the images
		'''                     will eventually be drawn </param>
		Public Sub New(ByVal comp As java.awt.Component)
			target = comp
		End Sub

		''' <summary>
		''' Adds an image to the list of images being tracked by this media
		''' tracker. The image will eventually be rendered at its default
		''' (unscaled) size. </summary>
		''' <param name="image">   the image to be tracked </param>
		''' <param name="id">      an identifier used to track this image </param>
		Public Overridable Sub addImage(ByVal image_Renamed As java.awt.Image, ByVal id As Integer)
			addImage(image_Renamed, id, -1, -1)
		End Sub

		''' <summary>
		''' Adds a scaled image to the list of images being tracked
		''' by this media tracker. The image will eventually be
		''' rendered at the indicated width and height.
		''' </summary>
		''' <param name="image">   the image to be tracked </param>
		''' <param name="id">   an identifier that can be used to track this image </param>
		''' <param name="w">    the width at which the image is rendered </param>
		''' <param name="h">    the height at which the image is rendered </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addImage(ByVal image_Renamed As java.awt.Image, ByVal id As Integer, ByVal w As Integer, ByVal h As Integer)
			addImageImpl(image_Renamed, id, w, h)
			Dim rvImage As java.awt.Image = getResolutionVariant(image_Renamed)
			If rvImage IsNot Nothing Then addImageImpl(rvImage, id,If(w = -1, -1, 2 * w),If(h = -1, -1, 2 * h))
		End Sub

		Private Sub addImageImpl(ByVal image_Renamed As java.awt.Image, ByVal id As Integer, ByVal w As Integer, ByVal h As Integer)
			head = MediaEntry.insert(head, New ImageMediaEntry(Me, image_Renamed, id, w, h))
		End Sub
		''' <summary>
		''' Flag indicating that media is currently being loaded. </summary>
		''' <seealso cref=         java.awt.MediaTracker#statusAll </seealso>
		''' <seealso cref=         java.awt.MediaTracker#statusID </seealso>
		Public Const LOADING As Integer = 1

		''' <summary>
		''' Flag indicating that the downloading of media was aborted. </summary>
		''' <seealso cref=         java.awt.MediaTracker#statusAll </seealso>
		''' <seealso cref=         java.awt.MediaTracker#statusID </seealso>
		Public Const ABORTED As Integer = 2

		''' <summary>
		''' Flag indicating that the downloading of media encountered
		''' an error. </summary>
		''' <seealso cref=         java.awt.MediaTracker#statusAll </seealso>
		''' <seealso cref=         java.awt.MediaTracker#statusID </seealso>
		Public Const ERRORED As Integer = 4

		''' <summary>
		''' Flag indicating that the downloading of media was completed
		''' successfully. </summary>
		''' <seealso cref=         java.awt.MediaTracker#statusAll </seealso>
		''' <seealso cref=         java.awt.MediaTracker#statusID </seealso>
		Public Const COMPLETE As Integer = 8

		Friend Shared ReadOnly DONE As Integer = (ABORTED Or ERRORED Or COMPLETE)

		''' <summary>
		''' Checks to see if all images being tracked by this media tracker
		''' have finished loading.
		''' <p>
		''' This method does not start loading the images if they are not
		''' already loading.
		''' <p>
		''' If there is an error while loading or scaling an image, then that
		''' image is considered to have finished loading. Use the
		''' <code>isErrorAny</code> or <code>isErrorID</code> methods to
		''' check for errors. </summary>
		''' <returns>      <code>true</code> if all images have finished loading,
		'''                       have been aborted, or have encountered
		'''                       an error; <code>false</code> otherwise </returns>
		''' <seealso cref=         java.awt.MediaTracker#checkAll(boolean) </seealso>
		''' <seealso cref=         java.awt.MediaTracker#checkID </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorAny </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorID </seealso>
		Public Overridable Function checkAll() As Boolean
			Return checkAll(False, True)
		End Function

		''' <summary>
		''' Checks to see if all images being tracked by this media tracker
		''' have finished loading.
		''' <p>
		''' If the value of the <code>load</code> flag is <code>true</code>,
		''' then this method starts loading any images that are not yet
		''' being loaded.
		''' <p>
		''' If there is an error while loading or scaling an image, that
		''' image is considered to have finished loading. Use the
		''' <code>isErrorAny</code> and <code>isErrorID</code> methods to
		''' check for errors. </summary>
		''' <param name="load">   if <code>true</code>, start loading any
		'''                       images that are not yet being loaded </param>
		''' <returns>      <code>true</code> if all images have finished loading,
		'''                       have been aborted, or have encountered
		'''                       an error; <code>false</code> otherwise </returns>
		''' <seealso cref=         java.awt.MediaTracker#checkID </seealso>
		''' <seealso cref=         java.awt.MediaTracker#checkAll() </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorAny() </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorID(int) </seealso>
		Public Overridable Function checkAll(ByVal load As Boolean) As Boolean
			Return checkAll(load, True)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function checkAll(ByVal load As Boolean, ByVal verify As Boolean) As Boolean
			Dim cur As MediaEntry = head
			Dim done_Renamed As Boolean = True
			Do While cur IsNot Nothing
				If (cur.getStatus(load, verify) And DONE) = 0 Then done_Renamed = False
				cur = cur.next
			Loop
			Return done_Renamed
		End Function

		''' <summary>
		''' Checks the error status of all of the images. </summary>
		''' <returns>   <code>true</code> if any of the images tracked
		'''                  by this media tracker had an error during
		'''                  loading; <code>false</code> otherwise </returns>
		''' <seealso cref=      java.awt.MediaTracker#isErrorID </seealso>
		''' <seealso cref=      java.awt.MediaTracker#getErrorsAny </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property errorAny As Boolean
			Get
				Dim cur As MediaEntry = head
				Do While cur IsNot Nothing
					If (cur.getStatus(False, True) And ERRORED) <> 0 Then Return True
					cur = cur.next
				Loop
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns a list of all media that have encountered an error. </summary>
		''' <returns>       an array of media objects tracked by this
		'''                        media tracker that have encountered
		'''                        an error, or <code>null</code> if
		'''                        there are none with errors </returns>
		''' <seealso cref=          java.awt.MediaTracker#isErrorAny </seealso>
		''' <seealso cref=          java.awt.MediaTracker#getErrorsID </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property errorsAny As Object()
			Get
				Dim cur As MediaEntry = head
				Dim numerrors As Integer = 0
				Do While cur IsNot Nothing
					If (cur.getStatus(False, True) And ERRORED) <> 0 Then numerrors += 1
					cur = cur.next
				Loop
				If numerrors = 0 Then Return Nothing
				Dim errors As Object() = New Object(numerrors - 1){}
				cur = head
				numerrors = 0
				Do While cur IsNot Nothing
					If (cur.getStatus(False, False) And ERRORED) <> 0 Then
						errors(numerrors) = cur.media
						numerrors += 1
					End If
					cur = cur.next
				Loop
				Return errors
			End Get
		End Property

		''' <summary>
		''' Starts loading all images tracked by this media tracker. This
		''' method waits until all the images being tracked have finished
		''' loading.
		''' <p>
		''' If there is an error while loading or scaling an image, then that
		''' image is considered to have finished loading. Use the
		''' <code>isErrorAny</code> or <code>isErrorID</code> methods to
		''' check for errors. </summary>
		''' <seealso cref=         java.awt.MediaTracker#waitForID(int) </seealso>
		''' <seealso cref=         java.awt.MediaTracker#waitForAll(long) </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorAny </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorID </seealso>
		''' <exception cref="InterruptedException">  if any thread has
		'''                                     interrupted this thread </exception>
		Public Overridable Sub waitForAll()
			waitForAll(0)
		End Sub

		''' <summary>
		''' Starts loading all images tracked by this media tracker. This
		''' method waits until all the images being tracked have finished
		''' loading, or until the length of time specified in milliseconds
		''' by the <code>ms</code> argument has passed.
		''' <p>
		''' If there is an error while loading or scaling an image, then
		''' that image is considered to have finished loading. Use the
		''' <code>isErrorAny</code> or <code>isErrorID</code> methods to
		''' check for errors. </summary>
		''' <param name="ms">       the number of milliseconds to wait
		'''                       for the loading to complete </param>
		''' <returns>      <code>true</code> if all images were successfully
		'''                       loaded; <code>false</code> otherwise </returns>
		''' <seealso cref=         java.awt.MediaTracker#waitForID(int) </seealso>
		''' <seealso cref=         java.awt.MediaTracker#waitForAll(long) </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorAny </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorID </seealso>
		''' <exception cref="InterruptedException">  if any thread has
		'''                                     interrupted this thread. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function waitForAll(ByVal ms As Long) As Boolean
			Dim [end] As Long = System.currentTimeMillis() + ms
			Dim first As Boolean = True
			Do
				Dim status As Integer = statusAll(first, first)
				If (status And LOADING) = 0 Then Return (status = COMPLETE)
				first = False
				Dim timeout As Long
				If ms = 0 Then
					timeout = 0
				Else
					timeout = [end] - System.currentTimeMillis()
					If timeout <= 0 Then Return False
				End If
				wait(timeout)
			Loop
		End Function

		''' <summary>
		''' Calculates and returns the bitwise inclusive <b>OR</b> of the
		''' status of all media that are tracked by this media tracker.
		''' <p>
		''' Possible flags defined by the
		''' <code>MediaTracker</code> class are <code>LOADING</code>,
		''' <code>ABORTED</code>, <code>ERRORED</code>, and
		''' <code>COMPLETE</code>. An image that hasn't started
		''' loading has zero as its status.
		''' <p>
		''' If the value of <code>load</code> is <code>true</code>, then
		''' this method starts loading any images that are not yet being loaded.
		''' </summary>
		''' <param name="load">   if <code>true</code>, start loading
		'''                            any images that are not yet being loaded </param>
		''' <returns>       the bitwise inclusive <b>OR</b> of the status of
		'''                            all of the media being tracked </returns>
		''' <seealso cref=          java.awt.MediaTracker#statusID(int, boolean) </seealso>
		''' <seealso cref=          java.awt.MediaTracker#LOADING </seealso>
		''' <seealso cref=          java.awt.MediaTracker#ABORTED </seealso>
		''' <seealso cref=          java.awt.MediaTracker#ERRORED </seealso>
		''' <seealso cref=          java.awt.MediaTracker#COMPLETE </seealso>
		Public Overridable Function statusAll(ByVal load As Boolean) As Integer
			Return statusAll(load, True)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function statusAll(ByVal load As Boolean, ByVal verify As Boolean) As Integer
			Dim cur As MediaEntry = head
			Dim status As Integer = 0
			Do While cur IsNot Nothing
				status = status Or cur.getStatus(load, verify)
				cur = cur.next
			Loop
			Return status
		End Function

		''' <summary>
		''' Checks to see if all images tracked by this media tracker that
		''' are tagged with the specified identifier have finished loading.
		''' <p>
		''' This method does not start loading the images if they are not
		''' already loading.
		''' <p>
		''' If there is an error while loading or scaling an image, then that
		''' image is considered to have finished loading. Use the
		''' <code>isErrorAny</code> or <code>isErrorID</code> methods to
		''' check for errors. </summary>
		''' <param name="id">   the identifier of the images to check </param>
		''' <returns>      <code>true</code> if all images have finished loading,
		'''                       have been aborted, or have encountered
		'''                       an error; <code>false</code> otherwise </returns>
		''' <seealso cref=         java.awt.MediaTracker#checkID(int, boolean) </seealso>
		''' <seealso cref=         java.awt.MediaTracker#checkAll() </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorAny() </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorID(int) </seealso>
		Public Overridable Function checkID(ByVal id As Integer) As Boolean
			Return checkID(id, False, True)
		End Function

		''' <summary>
		''' Checks to see if all images tracked by this media tracker that
		''' are tagged with the specified identifier have finished loading.
		''' <p>
		''' If the value of the <code>load</code> flag is <code>true</code>,
		''' then this method starts loading any images that are not yet
		''' being loaded.
		''' <p>
		''' If there is an error while loading or scaling an image, then that
		''' image is considered to have finished loading. Use the
		''' <code>isErrorAny</code> or <code>isErrorID</code> methods to
		''' check for errors. </summary>
		''' <param name="id">       the identifier of the images to check </param>
		''' <param name="load">     if <code>true</code>, start loading any
		'''                       images that are not yet being loaded </param>
		''' <returns>      <code>true</code> if all images have finished loading,
		'''                       have been aborted, or have encountered
		'''                       an error; <code>false</code> otherwise </returns>
		''' <seealso cref=         java.awt.MediaTracker#checkID(int, boolean) </seealso>
		''' <seealso cref=         java.awt.MediaTracker#checkAll() </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorAny() </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorID(int) </seealso>
		Public Overridable Function checkID(ByVal id As Integer, ByVal load As Boolean) As Boolean
			Return checkID(id, load, True)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function checkID(ByVal id As Integer, ByVal load As Boolean, ByVal verify As Boolean) As Boolean
			Dim cur As MediaEntry = head
			Dim done_Renamed As Boolean = True
			Do While cur IsNot Nothing
				If cur.iD = id AndAlso (cur.getStatus(load, verify) And DONE) = 0 Then done_Renamed = False
				cur = cur.next
			Loop
			Return done_Renamed
		End Function

		''' <summary>
		''' Checks the error status of all of the images tracked by this
		''' media tracker with the specified identifier. </summary>
		''' <param name="id">   the identifier of the images to check </param>
		''' <returns>       <code>true</code> if any of the images with the
		'''                          specified identifier had an error during
		'''                          loading; <code>false</code> otherwise </returns>
		''' <seealso cref=          java.awt.MediaTracker#isErrorAny </seealso>
		''' <seealso cref=          java.awt.MediaTracker#getErrorsID </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function isErrorID(ByVal id As Integer) As Boolean
			Dim cur As MediaEntry = head
			Do While cur IsNot Nothing
				If cur.iD = id AndAlso (cur.getStatus(False, True) And ERRORED) <> 0 Then Return True
				cur = cur.next
			Loop
			Return False
		End Function

		''' <summary>
		''' Returns a list of media with the specified ID that
		''' have encountered an error. </summary>
		''' <param name="id">   the identifier of the images to check </param>
		''' <returns>      an array of media objects tracked by this media
		'''                       tracker with the specified identifier
		'''                       that have encountered an error, or
		'''                       <code>null</code> if there are none with errors </returns>
		''' <seealso cref=         java.awt.MediaTracker#isErrorID </seealso>
		''' <seealso cref=         java.awt.MediaTracker#isErrorAny </seealso>
		''' <seealso cref=         java.awt.MediaTracker#getErrorsAny </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getErrorsID(ByVal id As Integer) As Object()
			Dim cur As MediaEntry = head
			Dim numerrors As Integer = 0
			Do While cur IsNot Nothing
				If cur.iD = id AndAlso (cur.getStatus(False, True) And ERRORED) <> 0 Then numerrors += 1
				cur = cur.next
			Loop
			If numerrors = 0 Then Return Nothing
			Dim errors As Object() = New Object(numerrors - 1){}
			cur = head
			numerrors = 0
			Do While cur IsNot Nothing
				If cur.iD = id AndAlso (cur.getStatus(False, False) And ERRORED) <> 0 Then
					errors(numerrors) = cur.media
					numerrors += 1
				End If
				cur = cur.next
			Loop
			Return errors
		End Function

		''' <summary>
		''' Starts loading all images tracked by this media tracker with the
		''' specified identifier. This method waits until all the images with
		''' the specified identifier have finished loading.
		''' <p>
		''' If there is an error while loading or scaling an image, then that
		''' image is considered to have finished loading. Use the
		''' <code>isErrorAny</code> and <code>isErrorID</code> methods to
		''' check for errors. </summary>
		''' <param name="id">   the identifier of the images to check </param>
		''' <seealso cref=           java.awt.MediaTracker#waitForAll </seealso>
		''' <seealso cref=           java.awt.MediaTracker#isErrorAny() </seealso>
		''' <seealso cref=           java.awt.MediaTracker#isErrorID(int) </seealso>
		''' <exception cref="InterruptedException">  if any thread has
		'''                          interrupted this thread. </exception>
		Public Overridable Sub waitForID(ByVal id As Integer)
			waitForID(id, 0)
		End Sub

		''' <summary>
		''' Starts loading all images tracked by this media tracker with the
		''' specified identifier. This method waits until all the images with
		''' the specified identifier have finished loading, or until the
		''' length of time specified in milliseconds by the <code>ms</code>
		''' argument has passed.
		''' <p>
		''' If there is an error while loading or scaling an image, then that
		''' image is considered to have finished loading. Use the
		''' <code>statusID</code>, <code>isErrorID</code>, and
		''' <code>isErrorAny</code> methods to check for errors. </summary>
		''' <param name="id">   the identifier of the images to check </param>
		''' <param name="ms">   the length of time, in milliseconds, to wait
		'''                           for the loading to complete </param>
		''' <seealso cref=           java.awt.MediaTracker#waitForAll </seealso>
		''' <seealso cref=           java.awt.MediaTracker#waitForID(int) </seealso>
		''' <seealso cref=           java.awt.MediaTracker#statusID </seealso>
		''' <seealso cref=           java.awt.MediaTracker#isErrorAny() </seealso>
		''' <seealso cref=           java.awt.MediaTracker#isErrorID(int) </seealso>
		''' <exception cref="InterruptedException">  if any thread has
		'''                          interrupted this thread. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function waitForID(ByVal id As Integer, ByVal ms As Long) As Boolean
			Dim [end] As Long = System.currentTimeMillis() + ms
			Dim first As Boolean = True
			Do
				Dim status As Integer = statusID(id, first, first)
				If (status And LOADING) = 0 Then Return (status = COMPLETE)
				first = False
				Dim timeout As Long
				If ms = 0 Then
					timeout = 0
				Else
					timeout = [end] - System.currentTimeMillis()
					If timeout <= 0 Then Return False
				End If
				wait(timeout)
			Loop
		End Function

		''' <summary>
		''' Calculates and returns the bitwise inclusive <b>OR</b> of the
		''' status of all media with the specified identifier that are
		''' tracked by this media tracker.
		''' <p>
		''' Possible flags defined by the
		''' <code>MediaTracker</code> class are <code>LOADING</code>,
		''' <code>ABORTED</code>, <code>ERRORED</code>, and
		''' <code>COMPLETE</code>. An image that hasn't started
		''' loading has zero as its status.
		''' <p>
		''' If the value of <code>load</code> is <code>true</code>, then
		''' this method starts loading any images that are not yet being loaded. </summary>
		''' <param name="id">   the identifier of the images to check </param>
		''' <param name="load">   if <code>true</code>, start loading
		'''                            any images that are not yet being loaded </param>
		''' <returns>       the bitwise inclusive <b>OR</b> of the status of
		'''                            all of the media with the specified
		'''                            identifier that are being tracked </returns>
		''' <seealso cref=          java.awt.MediaTracker#statusAll(boolean) </seealso>
		''' <seealso cref=          java.awt.MediaTracker#LOADING </seealso>
		''' <seealso cref=          java.awt.MediaTracker#ABORTED </seealso>
		''' <seealso cref=          java.awt.MediaTracker#ERRORED </seealso>
		''' <seealso cref=          java.awt.MediaTracker#COMPLETE </seealso>
		Public Overridable Function statusID(ByVal id As Integer, ByVal load As Boolean) As Integer
			Return statusID(id, load, True)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function statusID(ByVal id As Integer, ByVal load As Boolean, ByVal verify As Boolean) As Integer
			Dim cur As MediaEntry = head
			Dim status As Integer = 0
			Do While cur IsNot Nothing
				If cur.iD = id Then status = status Or cur.getStatus(load, verify)
				cur = cur.next
			Loop
			Return status
		End Function

		''' <summary>
		''' Removes the specified image from this media tracker.
		''' All instances of the specified image are removed,
		''' regardless of scale or ID. </summary>
		''' <param name="image">     the image to be removed </param>
		''' <seealso cref=     java.awt.MediaTracker#removeImage(java.awt.Image, int) </seealso>
		''' <seealso cref=     java.awt.MediaTracker#removeImage(java.awt.Image, int, int, int)
		''' @since   JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeImage(ByVal image_Renamed As java.awt.Image)
			removeImageImpl(image_Renamed)
			Dim rvImage As java.awt.Image = getResolutionVariant(image_Renamed)
			If rvImage IsNot Nothing Then removeImageImpl(rvImage)
			notifyAll() ' Notify in case remaining images are "done".
		End Sub

		Private Sub removeImageImpl(ByVal image_Renamed As java.awt.Image)
			Dim cur As MediaEntry = head
			Dim prev As MediaEntry = Nothing
			Do While cur IsNot Nothing
				Dim [next] As MediaEntry = cur.next
				If cur.media Is image_Renamed Then
					If prev Is Nothing Then
						head = [next]
					Else
						prev.next = [next]
					End If
					cur.cancel()
				Else
					prev = cur
				End If
				cur = [next]
			Loop
		End Sub

		''' <summary>
		''' Removes the specified image from the specified tracking
		''' ID of this media tracker.
		''' All instances of <code>Image</code> being tracked
		''' under the specified ID are removed regardless of scale. </summary>
		''' <param name="image"> the image to be removed </param>
		''' <param name="id"> the tracking ID from which to remove the image </param>
		''' <seealso cref=        java.awt.MediaTracker#removeImage(java.awt.Image) </seealso>
		''' <seealso cref=        java.awt.MediaTracker#removeImage(java.awt.Image, int, int, int)
		''' @since      JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeImage(ByVal image_Renamed As java.awt.Image, ByVal id As Integer)
			removeImageImpl(image_Renamed, id)
			Dim rvImage As java.awt.Image = getResolutionVariant(image_Renamed)
			If rvImage IsNot Nothing Then removeImageImpl(rvImage, id)
			notifyAll() ' Notify in case remaining images are "done".
		End Sub

		Private Sub removeImageImpl(ByVal image_Renamed As java.awt.Image, ByVal id As Integer)
			Dim cur As MediaEntry = head
			Dim prev As MediaEntry = Nothing
			Do While cur IsNot Nothing
				Dim [next] As MediaEntry = cur.next
				If cur.iD = id AndAlso cur.media Is image_Renamed Then
					If prev Is Nothing Then
						head = [next]
					Else
						prev.next = [next]
					End If
					cur.cancel()
				Else
					prev = cur
				End If
				cur = [next]
			Loop
		End Sub

		''' <summary>
		''' Removes the specified image with the specified
		''' width, height, and ID from this media tracker.
		''' Only the specified instance (with any duplicates) is removed. </summary>
		''' <param name="image"> the image to be removed </param>
		''' <param name="id"> the tracking ID from which to remove the image </param>
		''' <param name="width"> the width to remove (-1 for unscaled) </param>
		''' <param name="height"> the height to remove (-1 for unscaled) </param>
		''' <seealso cref=     java.awt.MediaTracker#removeImage(java.awt.Image) </seealso>
		''' <seealso cref=     java.awt.MediaTracker#removeImage(java.awt.Image, int)
		''' @since   JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeImage(ByVal image_Renamed As java.awt.Image, ByVal id As Integer, ByVal width As Integer, ByVal height As Integer)
			removeImageImpl(image_Renamed, id, width, height)
			Dim rvImage As java.awt.Image = getResolutionVariant(image_Renamed)
			If rvImage IsNot Nothing Then removeImageImpl(rvImage, id,If(width = -1, -1, 2 * width),If(height = -1, -1, 2 * height))
			notifyAll() ' Notify in case remaining images are "done".
		End Sub

		Private Sub removeImageImpl(ByVal image_Renamed As java.awt.Image, ByVal id As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim cur As MediaEntry = head
			Dim prev As MediaEntry = Nothing
			Do While cur IsNot Nothing
				Dim [next] As MediaEntry = cur.next
				If cur.iD = id AndAlso TypeOf cur Is ImageMediaEntry AndAlso CType(cur, ImageMediaEntry).matches(image_Renamed, width, height) Then
					If prev Is Nothing Then
						head = [next]
					Else
						prev.next = [next]
					End If
					cur.cancel()
				Else
					prev = cur
				End If
				cur = [next]
			Loop
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Sub setDone()
			notifyAll()
		End Sub

		Private Shared Function getResolutionVariant(ByVal image_Renamed As java.awt.Image) As java.awt.Image
			If TypeOf image_Renamed Is sun.awt.image.MultiResolutionToolkitImage Then Return CType(image_Renamed, sun.awt.image.MultiResolutionToolkitImage).resolutionVariant
			Return Nothing
		End Function
	End Class

	Friend MustInherit Class MediaEntry
		Friend tracker As MediaTracker
		Friend ID As Integer
		Friend [next] As MediaEntry

		Friend status As Integer
		Friend cancelled As Boolean

		Friend Sub New(ByVal mt As MediaTracker, ByVal id As Integer)
			tracker = mt
			Me.ID = id
		End Sub

		Friend MustOverride ReadOnly Property media As Object

		Shared Function insert(ByVal head As MediaEntry, ByVal [me] As MediaEntry) As MediaEntry
			Dim cur As MediaEntry = head
			Dim prev As MediaEntry = Nothing
			Do While cur IsNot Nothing
				If cur.ID > [me].ID Then Exit Do
				prev = cur
				cur = cur.next
			Loop
			[me].next = cur
			If prev Is Nothing Then
				head = [me]
			Else
				prev.next = [me]
			End If
			Return head
		End Function

		Friend Overridable Property iD As Integer
			Get
				Return ID
			End Get
		End Property

		Friend MustOverride Sub startLoad()

		Friend Overridable Sub cancel()
			cancelled = True
		End Sub

		Friend Const LOADING As Integer = MediaTracker.LOADING
		Friend Const ABORTED As Integer = MediaTracker.ABORTED
		Friend Const ERRORED As Integer = MediaTracker.ERRORED
		Friend Const COMPLETE As Integer = MediaTracker.COMPLETE

		Friend Shared ReadOnly LOADSTARTED As Integer = (LOADING Or ERRORED Or COMPLETE)
		Friend Shared ReadOnly DONE As Integer = (ABORTED Or ERRORED Or COMPLETE)

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Function getStatus(ByVal doLoad As Boolean, ByVal doVerify As Boolean) As Integer
			If doLoad AndAlso ((status And LOADSTARTED) = 0) Then
				status = (status And (Not ABORTED)) Or LOADING
				startLoad()
			End If
			Return status
		End Function

		Friend Overridable Property status As Integer
			Set(ByVal flag As Integer)
				SyncLock Me
					status = flag
				End SyncLock
				tracker.doneone()
			End Set
		End Property
	End Class

	<Serializable> _
	Friend Class ImageMediaEntry
		Inherits MediaEntry
		Implements java.awt.image.ImageObserver

		Friend image_Renamed As java.awt.Image
		Friend width As Integer
		Friend height As Integer

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = 4739377000350280650L

		Friend Sub New(ByVal mt As MediaTracker, ByVal img As java.awt.Image, ByVal c As Integer, ByVal w As Integer, ByVal h As Integer)
			MyBase.New(mt, c)
			image_Renamed = img
			width = w
			height = h
		End Sub

		Friend Overridable Function matches(ByVal img As java.awt.Image, ByVal w As Integer, ByVal h As Integer) As Boolean
			Return (image_Renamed Is img AndAlso width = w AndAlso height = h)
		End Function

		Friend  Overrides ReadOnly Property  media As Object
			Get
				Return image_Renamed
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Function getStatus(ByVal doLoad As Boolean, ByVal doVerify As Boolean) As Integer
			If doVerify Then
				Dim flags As Integer = tracker.target.checkImage(image_Renamed, width, height, Nothing)
				Dim s As Integer = parseflags(flags)
				If s = 0 Then
					If (status And (ERRORED Or COMPLETE)) <> 0 Then status = ABORTED
				ElseIf s <> status Then
					status = s
				End If
			End If
			Return MyBase.getStatus(doLoad, doVerify)
		End Function

		Friend Overrides Sub startLoad()
			If tracker.target.prepareImage(image_Renamed, width, height, Me) Then status = COMPLETE
		End Sub

		Friend Overridable Function parseflags(ByVal infoflags As Integer) As Integer
			If (infoflags And ERROR) <> 0 Then
				Return ERRORED
			ElseIf (infoflags And ABORT) <> 0 Then
				Return ABORTED
			ElseIf (infoflags And (ALLBITS Or FRAMEBITS)) <> 0 Then
				Return COMPLETE
			End If
			Return 0
		End Function

		Public Overridable Function imageUpdate(ByVal img As java.awt.Image, ByVal infoflags As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
			If cancelled Then Return False
			Dim s As Integer = parseflags(infoflags)
			If s <> 0 AndAlso s <> status Then status = s
			Return ((status And LOADING) <> 0)
		End Function
	End Class

End Namespace
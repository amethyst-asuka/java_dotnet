Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Text

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
Namespace javax.swing





	''' <summary>
	''' An abstract class to perform lengthy GUI-interaction tasks in a
	''' background thread. Several background threads can be used to execute such
	''' tasks. However, the exact strategy of choosing a thread for any particular
	''' {@code SwingWorker} is unspecified and should not be relied on.
	''' <p>
	''' When writing a multi-threaded application using Swing, there are
	''' two constraints to keep in mind:
	''' (refer to
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">
	'''   Concurrency in Swing
	''' </a> for more details):
	''' <ul>
	'''   <li> Time-consuming tasks should not be run on the <i>Event
	'''        Dispatch Thread</i>. Otherwise the application becomes unresponsive.
	'''   </li>
	'''   <li> Swing components should be accessed  on the <i>Event
	'''        Dispatch Thread</i> only.
	'''   </li>
	''' </ul>
	''' 
	''' 
	''' <p>
	''' These constraints mean that a GUI application with time intensive
	''' computing needs at least two threads:  1) a thread to perform the lengthy
	''' task and 2) the <i>Event Dispatch Thread</i> (EDT) for all GUI-related
	''' activities.  This involves inter-thread communication which can be
	''' tricky to implement.
	''' 
	''' <p>
	''' {@code SwingWorker} is designed for situations where you need to have a long
	''' running task run in a background thread and provide updates to the UI
	''' either when done, or while processing.
	''' Subclasses of {@code SwingWorker} must implement
	''' the <seealso cref="#doInBackground"/> method to perform the background computation.
	''' 
	''' 
	''' <p>
	''' <b>Workflow</b>
	''' <p>
	''' There are three threads involved in the life cycle of a
	''' {@code SwingWorker} :
	''' <ul>
	''' <li>
	''' <p>
	''' <i>Current</i> thread: The <seealso cref="#execute"/> method is
	''' called on this thread. It schedules {@code SwingWorker} for the execution on a
	''' <i>worker</i>
	''' thread and returns immediately. One can wait for the {@code SwingWorker} to
	''' complete using the <seealso cref="#get get"/> methods.
	''' <li>
	''' <p>
	''' <i>Worker</i> thread: The <seealso cref="#doInBackground"/>
	''' method is called on this thread.
	''' This is where all background activities should happen. To notify
	''' {@code PropertyChangeListeners} about bound properties changes use the
	''' <seealso cref="#firePropertyChange firePropertyChange"/> and
	''' <seealso cref="#getPropertyChangeSupport"/> methods. By default there are two bound
	''' properties available: {@code state} and {@code progress}.
	''' <li>
	''' <p>
	''' <i>Event Dispatch Thread</i>:  All Swing related activities occur
	''' on this thread. {@code SwingWorker} invokes the
	''' <seealso cref="#process process"/> and <seealso cref="#done"/> methods and notifies
	''' any {@code PropertyChangeListeners} on this thread.
	''' </ul>
	''' 
	''' <p>
	''' Often, the <i>Current</i> thread is the <i>Event Dispatch
	''' Thread</i>.
	''' 
	''' 
	''' <p>
	''' Before the {@code doInBackground} method is invoked on a <i>worker</i> thread,
	''' {@code SwingWorker} notifies any {@code PropertyChangeListeners} about the
	''' {@code state} property change to {@code StateValue.STARTED}.  After the
	''' {@code doInBackground} method is finished the {@code done} method is
	''' executed.  Then {@code SwingWorker} notifies any {@code PropertyChangeListeners}
	''' about the {@code state} property change to {@code StateValue.DONE}.
	''' 
	''' <p>
	''' {@code SwingWorker} is only designed to be executed once.  Executing a
	''' {@code SwingWorker} more than once will not result in invoking the
	''' {@code doInBackground} method twice.
	''' 
	''' <p>
	''' <b>Sample Usage</b>
	''' <p>
	''' The following example illustrates the simplest use case.  Some
	''' processing is done in the background and when done you update a Swing
	''' component.
	''' 
	''' <p>
	''' Say we want to find the "Meaning of Life" and display the result in
	''' a {@code JLabel}.
	''' 
	''' <pre>
	'''   final JLabel label;
	'''   class MeaningOfLifeFinder extends SwingWorker&lt;String, Object&gt; {
	'''       {@code @Override}
	'''       public String doInBackground() {
	'''           return findTheMeaningOfLife();
	'''       }
	''' 
	'''       {@code @Override}
	'''       protected void done() {
	'''           try {
	'''               label.setText(get());
	'''           } catch (Exception ignore) {
	'''           }
	'''       }
	'''   }
	''' 
	'''   (new MeaningOfLifeFinder()).execute();
	''' </pre>
	''' 
	''' <p>
	''' The next example is useful in situations where you wish to process data
	''' as it is ready on the <i>Event Dispatch Thread</i>.
	''' 
	''' <p>
	''' Now we want to find the first N prime numbers and display the results in a
	''' {@code JTextArea}.  While this is computing, we want to update our
	''' progress in a {@code JProgressBar}.  Finally, we also want to print
	''' the prime numbers to {@code System.out}.
	''' <pre>
	''' class PrimeNumbersTask extends
	'''         SwingWorker&lt;List&lt;Integer&gt;, Integer&gt; {
	'''     PrimeNumbersTask(JTextArea textArea, int numbersToFind) {
	'''         //initialize
	'''     }
	''' 
	'''     {@code @Override}
	'''     public List&lt;Integer&gt; doInBackground() {
	'''         while (! enough &amp;&amp; ! isCancelled()) {
	'''                 number = nextPrimeNumber();
	'''                 publish(number);
	'''                 setProgress(100 * numbers.size() / numbersToFind);
	'''             }
	'''         }
	'''         return numbers;
	'''     }
	''' 
	'''     {@code @Override}
	'''     protected void process(List&lt;Integer&gt; chunks) {
	'''         for (int number : chunks) {
	'''             textArea.append(number + &quot;\n&quot;);
	'''         }
	'''     }
	''' }
	''' 
	''' JTextArea textArea = new JTextArea();
	''' final JProgressBar progressBar = new JProgressBar(0, 100);
	''' PrimeNumbersTask task = new PrimeNumbersTask(textArea, N);
	''' task.addPropertyChangeListener(
	'''     new PropertyChangeListener() {
	'''         public  void propertyChange(PropertyChangeEvent evt) {
	'''             if (&quot;progress&quot;.equals(evt.getPropertyName())) {
	'''                 progressBar.setValue((Integer)evt.getNewValue());
	'''             }
	'''         }
	'''     });
	''' 
	''' task.execute();
	''' System.out.println(task.get()); //prints all prime numbers we have got
	''' </pre>
	''' 
	''' <p>
	''' Because {@code SwingWorker} implements {@code Runnable}, a
	''' {@code SwingWorker} can be submitted to an
	''' <seealso cref="java.util.concurrent.Executor"/> for execution.
	''' 
	''' @author Igor Kushnirskiy
	''' </summary>
	''' @param <T> the result type returned by this {@code SwingWorker's}
	'''        {@code doInBackground} and {@code get} methods </param>
	''' @param <V> the type used for carrying out intermediate results by this
	'''        {@code SwingWorker's} {@code publish} and {@code process} methods
	''' 
	''' @since 1.6 </param>
	Public MustInherit Class SwingWorker(Of T, V)
		Implements RunnableFuture(Of T)

		''' <summary>
		''' number of worker threads.
		''' </summary>
		Private Const MAX_WORKER_THREADS As Integer = 10

		''' <summary>
		''' current progress.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private progress As Integer

		''' <summary>
		''' current state.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private state As StateValue

		''' <summary>
		''' everything is run inside this FutureTask. Also it is used as
		''' a delegatee for the Future API.
		''' </summary>
		Private ReadOnly future As FutureTask(Of T)

		''' <summary>
		''' all propertyChangeSupport goes through this.
		''' </summary>
		Private ReadOnly propertyChangeSupport As java.beans.PropertyChangeSupport

		''' <summary>
		''' handler for {@code process} mehtod.
		''' </summary>
		Private doProcess As sun.swing.AccumulativeRunnable(Of V)

		''' <summary>
		''' handler for progress property change notifications.
		''' </summary>
		Private doNotifyProgressChange As sun.swing.AccumulativeRunnable(Of Integer?)

		Private ReadOnly doSubmit As sun.swing.AccumulativeRunnable(Of Runnable) = doSubmit

		''' <summary>
		''' Values for the {@code state} bound property.
		''' @since 1.6
		''' </summary>
		Public Enum StateValue
			''' <summary>
			''' Initial {@code SwingWorker} state.
			''' </summary>
			PENDING
			''' <summary>
			''' {@code SwingWorker} is {@code STARTED}
			''' before invoking {@code doInBackground}.
			''' </summary>
			STARTED

			''' <summary>
			''' {@code SwingWorker} is {@code DONE}
			''' after {@code doInBackground} method
			''' is finished.
			''' </summary>
			DONE
		End Enum

		''' <summary>
		''' Constructs this {@code SwingWorker}.
		''' </summary>
		Public Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Callable<T> callable = New Callable<T>()
	'		{
	'					public T call() throws Exception
	'					{
	'						setState(StateValue.STARTED);
	'						Return doInBackground();
	'					}
	'				};

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			future = New FutureTask<T>(callable)
	'		{
	'					   @Override protected void done()
	'					   {
	'						   doneEDT();
	'						   setState(StateValue.DONE);
	'					   }
	'				   };

		   state = StateValue.PENDING
		   propertyChangeSupport = New SwingWorkerPropertyChangeSupport(Me, Me)
		   doProcess = Nothing
		   doNotifyProgressChange = Nothing
		End Sub

		''' <summary>
		''' Computes a result, or throws an exception if unable to do so.
		''' 
		''' <p>
		''' Note that this method is executed only once.
		''' 
		''' <p>
		''' Note: this method is executed in a background thread.
		''' 
		''' </summary>
		''' <returns> the computed result </returns>
		''' <exception cref="Exception"> if unable to compute a result
		'''  </exception>
		Protected Friend MustOverride Function doInBackground() As T

		''' <summary>
		''' Sets this {@code Future} to the result of computation unless
		''' it has been cancelled.
		''' </summary>
		Public Sub run()
			future.run()
		End Sub

		''' <summary>
		''' Sends data chunks to the <seealso cref="#process"/> method. This method is to be
		''' used from inside the {@code doInBackground} method to deliver
		''' intermediate results
		''' for processing on the <i>Event Dispatch Thread</i> inside the
		''' {@code process} method.
		''' 
		''' <p>
		''' Because the {@code process} method is invoked asynchronously on
		''' the <i>Event Dispatch Thread</i>
		''' multiple invocations to the {@code publish} method
		''' might occur before the {@code process} method is executed. For
		''' performance purposes all these invocations are coalesced into one
		''' invocation with concatenated arguments.
		''' 
		''' <p>
		''' For example:
		''' 
		''' <pre>
		''' publish(&quot;1&quot;);
		''' publish(&quot;2&quot;, &quot;3&quot;);
		''' publish(&quot;4&quot;, &quot;5&quot;, &quot;6&quot;);
		''' </pre>
		''' 
		''' might result in:
		''' 
		''' <pre>
		''' process(&quot;1&quot;, &quot;2&quot;, &quot;3&quot;, &quot;4&quot;, &quot;5&quot;, &quot;6&quot;)
		''' </pre>
		''' 
		''' <p>
		''' <b>Sample Usage</b>. This code snippet loads some tabular data and
		''' updates {@code DefaultTableModel} with it. Note that it safe to mutate
		''' the tableModel from inside the {@code process} method because it is
		''' invoked on the <i>Event Dispatch Thread</i>.
		''' 
		''' <pre>
		''' class TableSwingWorker extends
		'''         SwingWorker&lt;DefaultTableModel, Object[]&gt; {
		'''     private final DefaultTableModel tableModel;
		''' 
		'''     public TableSwingWorker(DefaultTableModel tableModel) {
		'''         this.tableModel = tableModel;
		'''     }
		''' 
		'''     {@code @Override}
		'''     protected DefaultTableModel doInBackground() throws Exception {
		'''         for (Object[] row = loadData();
		'''                  ! isCancelled() &amp;&amp; row != null;
		'''                  row = loadData()) {
		'''             publish((Object[]) row);
		'''         }
		'''         return tableModel;
		'''     }
		''' 
		'''     {@code @Override}
		'''     protected void process(List&lt;Object[]&gt; chunks) {
		'''         for (Object[] row : chunks) {
		'''             tableModel.addRow(row);
		'''         }
		'''     }
		''' }
		''' </pre>
		''' </summary>
		''' <param name="chunks"> intermediate results to process
		''' </param>
		''' <seealso cref= #process
		'''  </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Sub publish(ParamArray ByVal chunks As V()) ' Passing chunks to add is safe
			SyncLock Me
				If doProcess Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					doProcess = New sun.swing.AccumulativeRunnable<V>()
	'				{
	'					@Override public void run(List<V> args)
	'					{
	'						process(args);
	'					}
	'					@Override protected void submit()
	'					{
	'						doSubmit.add(Me);
	'					}
	'				};
				End If
			End SyncLock
			doProcess.add(chunks)
		End Sub

		''' <summary>
		''' Receives data chunks from the {@code publish} method asynchronously on the
		''' <i>Event Dispatch Thread</i>.
		''' 
		''' <p>
		''' Please refer to the <seealso cref="#publish"/> method for more details.
		''' </summary>
		''' <param name="chunks"> intermediate results to process
		''' </param>
		''' <seealso cref= #publish
		'''  </seealso>
		Protected Friend Overridable Sub process(ByVal chunks As IList(Of V))
		End Sub

		''' <summary>
		''' Executed on the <i>Event Dispatch Thread</i> after the {@code doInBackground}
		''' method is finished. The default
		''' implementation does nothing. Subclasses may override this method to
		''' perform completion actions on the <i>Event Dispatch Thread</i>. Note
		''' that you can query status inside the implementation of this method to
		''' determine the result of this task or whether this task has been cancelled.
		''' </summary>
		''' <seealso cref= #doInBackground </seealso>
		''' <seealso cref= #isCancelled() </seealso>
		''' <seealso cref= #get </seealso>
		Protected Friend Overridable Sub done()
		End Sub

		''' <summary>
		''' Sets the {@code progress} bound property.
		''' The value should be from 0 to 100.
		''' 
		''' <p>
		''' Because {@code PropertyChangeListener}s are notified asynchronously on
		''' the <i>Event Dispatch Thread</i> multiple invocations to the
		''' {@code setProgress} method might occur before any
		''' {@code PropertyChangeListeners} are invoked. For performance purposes
		''' all these invocations are coalesced into one invocation with the last
		''' invocation argument only.
		''' 
		''' <p>
		''' For example, the following invokations:
		''' 
		''' <pre>
		''' setProgress(1);
		''' setProgress(2);
		''' setProgress(3);
		''' </pre>
		''' 
		''' might result in a single {@code PropertyChangeListener} notification with
		''' the value {@code 3}.
		''' </summary>
		''' <param name="progress"> the progress value to set </param>
		''' <exception cref="IllegalArgumentException"> is value not from 0 to 100 </exception>
		Protected Friend Property progress As Integer
			Set(ByVal progress As Integer)
				If progress < 0 OrElse progress > 100 Then Throw New System.ArgumentException("the value should be from 0 to 100")
				If Me.progress = progress Then Return
				Dim oldProgress As Integer = Me.progress
				Me.progress = progress
				If Not propertyChangeSupport.hasListeners("progress") Then Return
				SyncLock Me
					If doNotifyProgressChange Is Nothing Then
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'					doNotifyProgressChange = New sun.swing.AccumulativeRunnable<java.lang.Integer>()
		'				{
		'						@Override public void run(List<java.lang.Integer> args)
		'						{
		'							firePropertyChange("progress", args.get(0), args.get(args.size() - 1));
		'						}
		'						@Override protected void submit()
		'						{
		'							doSubmit.add(Me);
		'						}
		'					};
					End If
				End SyncLock
				doNotifyProgressChange.add(oldProgress, progress)
			End Set
			Get
				Return progress
			End Get
		End Property


		''' <summary>
		''' Schedules this {@code SwingWorker} for execution on a <i>worker</i>
		''' thread. There are a number of <i>worker</i> threads available. In the
		''' event all <i>worker</i> threads are busy handling other
		''' {@code SwingWorkers} this {@code SwingWorker} is placed in a waiting
		''' queue.
		''' 
		''' <p>
		''' Note:
		''' {@code SwingWorker} is only designed to be executed once.  Executing a
		''' {@code SwingWorker} more than once will not result in invoking the
		''' {@code doInBackground} method twice.
		''' </summary>
		Public Sub execute()
			workersExecutorService.execute(Me)
		End Sub

		' Future methods START
		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Function cancel(ByVal mayInterruptIfRunning As Boolean) As Boolean
			Return future.cancel(mayInterruptIfRunning)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property cancelled As Boolean
			Get
				Return future.cancelled
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property done As Boolean
			Get
				Return future.done
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' Note: calling {@code get} on the <i>Event Dispatch Thread</i> blocks
		''' <i>all</i> events, including repaints, from being processed until this
		''' {@code SwingWorker} is complete.
		''' 
		''' <p>
		''' When you want the {@code SwingWorker} to block on the <i>Event
		''' Dispatch Thread</i> we recommend that you use a <i>modal dialog</i>.
		''' 
		''' <p>
		''' For example:
		''' 
		''' <pre>
		''' class SwingWorkerCompletionWaiter extends PropertyChangeListener {
		'''     private JDialog dialog;
		''' 
		'''     public SwingWorkerCompletionWaiter(JDialog dialog) {
		'''         this.dialog = dialog;
		'''     }
		''' 
		'''     public void propertyChange(PropertyChangeEvent event) {
		'''         if (&quot;state&quot;.equals(event.getPropertyName())
		'''                 &amp;&amp; SwingWorker.StateValue.DONE == event.getNewValue()) {
		'''             dialog.setVisible(false);
		'''             dialog.dispose();
		'''         }
		'''     }
		''' }
		''' JDialog dialog = new JDialog(owner, true);
		''' swingWorker.addPropertyChangeListener(
		'''     new SwingWorkerCompletionWaiter(dialog));
		''' swingWorker.execute();
		''' //the dialog will be visible until the SwingWorker is done
		''' dialog.setVisible(true);
		''' </pre>
		''' </summary>
		Public Function [get]() As T
			Return future.get()
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' Please refer to <seealso cref="#get"/> for more details.
		''' </summary>
		Public Function [get](ByVal timeout As Long, ByVal unit As TimeUnit) As T
			Return future.get(timeout, unit)
		End Function

		' Future methods END

		' PropertyChangeSupports methods START
		''' <summary>
		''' Adds a {@code PropertyChangeListener} to the listener list. The listener
		''' is registered for all properties. The same listener object may be added
		''' more than once, and will be called as many times as it is added. If
		''' {@code listener} is {@code null}, no exception is thrown and no action is taken.
		''' 
		''' <p>
		''' Note: This is merely a convenience wrapper. All work is delegated to
		''' {@code PropertyChangeSupport} from <seealso cref="#getPropertyChangeSupport"/>.
		''' </summary>
		''' <param name="listener"> the {@code PropertyChangeListener} to be added </param>
		Public Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			propertyChangeSupport.addPropertyChangeListener(listener)
		End Sub

		''' <summary>
		''' Removes a {@code PropertyChangeListener} from the listener list. This
		''' removes a {@code PropertyChangeListener} that was registered for all
		''' properties. If {@code listener} was added more than once to the same
		''' event source, it will be notified one less time after being removed. If
		''' {@code listener} is {@code null}, or was never added, no exception is
		''' thrown and no action is taken.
		''' 
		''' <p>
		''' Note: This is merely a convenience wrapper. All work is delegated to
		''' {@code PropertyChangeSupport} from <seealso cref="#getPropertyChangeSupport"/>.
		''' </summary>
		''' <param name="listener"> the {@code PropertyChangeListener} to be removed </param>
		Public Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			propertyChangeSupport.removePropertyChangeListener(listener)
		End Sub

		''' <summary>
		''' Reports a bound property update to any registered listeners. No event is
		''' fired if {@code old} and {@code new} are equal and non-null.
		''' 
		''' <p>
		''' This {@code SwingWorker} will be the source for
		''' any generated events.
		''' 
		''' <p>
		''' When called off the <i>Event Dispatch Thread</i>
		''' {@code PropertyChangeListeners} are notified asynchronously on
		''' the <i>Event Dispatch Thread</i>.
		''' <p>
		''' Note: This is merely a convenience wrapper. All work is delegated to
		''' {@code PropertyChangeSupport} from <seealso cref="#getPropertyChangeSupport"/>.
		''' 
		''' </summary>
		''' <param name="propertyName"> the programmatic name of the property that was
		'''        changed </param>
		''' <param name="oldValue"> the old value of the property </param>
		''' <param name="newValue"> the new value of the property </param>
		Public Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			propertyChangeSupport.firePropertyChange(propertyName, oldValue, newValue)
		End Sub

		''' <summary>
		''' Returns the {@code PropertyChangeSupport} for this {@code SwingWorker}.
		''' This method is used when flexible access to bound properties support is
		''' needed.
		''' <p>
		''' This {@code SwingWorker} will be the source for
		''' any generated events.
		''' 
		''' <p>
		''' Note: The returned {@code PropertyChangeSupport} notifies any
		''' {@code PropertyChangeListener}s asynchronously on the <i>Event Dispatch
		''' Thread</i> in the event that {@code firePropertyChange} or
		''' {@code fireIndexedPropertyChange} are called off the <i>Event Dispatch
		''' Thread</i>.
		''' </summary>
		''' <returns> {@code PropertyChangeSupport} for this {@code SwingWorker} </returns>
		Public Property propertyChangeSupport As java.beans.PropertyChangeSupport
			Get
				Return propertyChangeSupport
			End Get
		End Property

		' PropertyChangeSupports methods END

		''' <summary>
		''' Returns the {@code SwingWorker} state bound property.
		''' </summary>
		''' <returns> the current state </returns>
		Public Property state As StateValue
			Get
		'        
		'         * DONE is a speacial case
		'         * to keep getState and isDone is sync
		'         
				If done Then
					Return StateValue.DONE
				Else
					Return state
				End If
			End Get
			Set(ByVal state As StateValue)
				Dim old As StateValue = Me.state
				Me.state = state
				firePropertyChange("state", old, state)
			End Set
		End Property


		''' <summary>
		''' Invokes {@code done} on the EDT.
		''' </summary>
		Private Sub doneEDT()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Runnable doDone = New Runnable()
	'		{
	'				public void run()
	'				{
	'					done();
	'				}
	'			};
			If javax.swing.SwingUtilities.eventDispatchThread Then
				doDone.run()
			Else
				doSubmit.add(doDone)
			End If
		End Sub


		''' <summary>
		''' returns workersExecutorService.
		''' 
		''' returns the service stored in the appContext or creates it if
		''' necessary.
		''' </summary>
		''' <returns> ExecutorService for the {@code SwingWorkers} </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property Shared workersExecutorService As ExecutorService
			Get
				Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
				Dim executorService As ExecutorService = CType(appContext.get(GetType(SwingWorker)), ExecutorService)
				If executorService Is Nothing Then
					'this creates daemon threads.
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'				ThreadFactory threadFactory = New ThreadFactory()
		'			{
		'					final ThreadFactory defaultFactory = Executors.defaultThreadFactory();
		'					public Thread newThread(final Runnable r)
		'					{
		'						Thread thread = defaultFactory.newThread(r);
		'						thread.setName("SwingWorker-" + thread.getName());
		'						thread.setDaemon(True);
		'						Return thread;
		'					}
		'				};
    
					executorService = New ThreadPoolExecutor(MAX_WORKER_THREADS, MAX_WORKER_THREADS, 10L, TimeUnit.MINUTES, New LinkedBlockingQueue(Of Runnable), threadFactory)
					appContext.put(GetType(SwingWorker), executorService)
    
					' Don't use ShutdownHook here as it's not enough. We should track
					' AppContext disposal instead of JVM shutdown, see 6799345 for details
					Dim es As ExecutorService = executorService
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'				appContext.addPropertyChangeListener(sun.awt.AppContext.DISPOSED_PROPERTY_NAME, New java.beans.PropertyChangeListener()
		'			{
		'					@Override public void propertyChange(PropertyChangeEvent pce)
		'					{
		'						boolean disposed = (java.lang.Boolean)pce.getNewValue();
		'						if (disposed)
		'						{
		'							final WeakReference<ExecutorService> executorServiceRef = New WeakReference<ExecutorService>(es);
		'							final ExecutorService executorService = executorServiceRef.get();
		'							if (executorService != Nothing)
		'							{
		'								AccessController.doPrivileged(New PrivilegedAction<Void>()
		'								{
		'										public Void run()
		'										{
		'											executorService.shutdown();
		'											Return Nothing;
		'										}
		'									}
		'							   );
		'							}
		'						}
		'					}
		'				}
				   )
				End If
				Return executorService
			End Get
		End Property

		Private Shared ReadOnly DO_SUBMIT_KEY As Object = New StringBuilder("doSubmit")
		Private Property Shared doSubmit As sun.swing.AccumulativeRunnable(Of Runnable)
			Get
				SyncLock DO_SUBMIT_KEY
					Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
					Dim ___doSubmit As Object = appContext.get(DO_SUBMIT_KEY)
					If ___doSubmit Is Nothing Then
						___doSubmit = New DoSubmitAccumulativeRunnable
						appContext.put(DO_SUBMIT_KEY, ___doSubmit)
					End If
					Return CType(___doSubmit, sun.swing.AccumulativeRunnable(Of Runnable))
				End SyncLock
			End Get
		End Property
		Private Class DoSubmitAccumulativeRunnable
			Inherits sun.swing.AccumulativeRunnable(Of Runnable)
			Implements ActionListener

			Private Const DELAY As Integer = 1000 \ 30
			Protected Friend Overrides Sub run(ByVal args As IList(Of Runnable))
				For Each runnable As Runnable In args
					runnable.run()
				Next runnable
			End Sub
			Protected Friend Overrides Sub submit()
				Dim ___timer As New Timer(DELAY, Me)
				___timer.repeats = False
				___timer.start()
			End Sub
			Public Overridable Sub actionPerformed(ByVal [event] As ActionEvent)
				run()
			End Sub
		End Class

		Private Class SwingWorkerPropertyChangeSupport
			Inherits java.beans.PropertyChangeSupport

			Private ReadOnly outerInstance As SwingWorker

			Friend Sub New(ByVal outerInstance As SwingWorker, ByVal source As Object)
					Me.outerInstance = outerInstance
				MyBase.New(source)
			End Sub
			Public Overrides Sub firePropertyChange(ByVal evt As java.beans.PropertyChangeEvent)
				If javax.swing.SwingUtilities.eventDispatchThread Then
					MyBase.firePropertyChange(evt)
				Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					doSubmit.add(New Runnable()
	'				{
	'						public void run()
	'						{
	'							outerInstance.firePropertyChange(evt);
	'						}
	'					});
				End If
			End Sub
		End Class
	End Class

End Namespace
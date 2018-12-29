﻿
Imports System.Threading
Imports NLog
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports Algo2TradeCore.Controller
Imports System.ComponentModel
Public Class frmMain
#Region "Logging and Status Progress"
    Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

#Region "Common Delegates"
    Delegate Sub SetGridDisplayIndex_Delegate(ByVal [grd] As DataGridView, ByVal [colName] As String, ByVal [value] As Integer)
    Public Sub SetGridDisplayIndex_ThreadSafe(ByVal [grd] As DataGridView, ByVal [colName] As String, ByVal [value] As Integer)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [grd].InvokeRequired Then
            Dim MyDelegate As New SetGridDisplayIndex_Delegate(AddressOf SetGridDisplayIndex_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[grd], [colName], [value]})
        Else
            [grd].Columns([colName]).DisplayIndex = [value]
        End If
    End Sub
    Delegate Function GetGridColumnCount_Delegate(ByVal [grd] As DataGridView) As String
    Public Function GetGridColumnCount_ThreadSafe(ByVal [grd] As DataGridView) As String
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [grd].InvokeRequired Then
            Dim MyDelegate As New GetGridColumnCount_Delegate(AddressOf GetGridColumnCount_ThreadSafe)
            Return Me.Invoke(MyDelegate, New Object() {[grd]})
        Else
            Return [grd].Columns.Count
        End If
    End Function

    Delegate Sub SetGridDataBind_Delegate(ByVal [grd] As DataGridView, ByVal [value] As Object)
    Public Sub SetGridDataBind_ThreadSafe(ByVal [grd] As DataGridView, ByVal [value] As Object)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [grd].InvokeRequired Then
            Dim MyDelegate As New SetGridDataBind_Delegate(AddressOf SetGridDataBind_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[grd], [value]})
        Else
            [grd].DataSource = [value]
        End If
    End Sub

    Delegate Sub SetListAddItem_Delegate(ByVal [lst] As ListBox, ByVal [value] As Object)
    Public Sub SetListAddItem_ThreadSafe(ByVal [lst] As ListBox, ByVal [value] As Object)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [lst].InvokeRequired Then
            Dim MyDelegate As New SetListAddItem_Delegate(AddressOf SetListAddItem_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {lst, [value]})
        Else
            [lst].Items.Insert(0, [value])
        End If
    End Sub
    Delegate Sub SetObjectEnableDisable_Delegate(ByVal [obj] As Object, ByVal [value] As Boolean)
    Public Sub SetObjectEnableDisable_ThreadSafe(ByVal [obj] As Object, ByVal [value] As Boolean)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [obj].InvokeRequired Then
            Dim MyDelegate As New SetObjectEnableDisable_Delegate(AddressOf SetObjectEnableDisable_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[obj], [value]})
        Else
            [obj].Enabled = [value]
        End If
    End Sub

    Delegate Sub SetLabelText_Delegate(ByVal [label] As Label, ByVal [text] As String)
    Public Sub SetLabelText_ThreadSafe(ByVal [label] As Label, ByVal [text] As String)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [label].InvokeRequired Then
            Dim MyDelegate As New SetLabelText_Delegate(AddressOf SetLabelText_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[label], [text]})
        Else
            [label].Text = [text]
        End If
    End Sub
    Delegate Function GetLabelText_Delegate(ByVal [label] As Label) As String

    Public Function GetLabelText_ThreadSafe(ByVal [label] As Label) As String
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [label].InvokeRequired Then
            Dim MyDelegate As New GetLabelText_Delegate(AddressOf GetLabelText_ThreadSafe)
            Return Me.Invoke(MyDelegate, New Object() {[label]})
        Else
            Return [label].Text
        End If
    End Function

    Delegate Sub SetLabelTag_Delegate(ByVal [label] As Label, ByVal [tag] As String)
    Public Sub SetLabelTag_ThreadSafe(ByVal [label] As Label, ByVal [tag] As String)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [label].InvokeRequired Then
            Dim MyDelegate As New SetLabelTag_Delegate(AddressOf SetLabelTag_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[label], [tag]})
        Else
            [label].Tag = [tag]
        End If
    End Sub
    Delegate Function GetLabelTag_Delegate(ByVal [label] As Label) As String

    Public Function GetLabelTag_ThreadSafe(ByVal [label] As Label) As String
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [label].InvokeRequired Then
            Dim MyDelegate As New GetLabelTag_Delegate(AddressOf GetLabelTag_ThreadSafe)
            Return Me.Invoke(MyDelegate, New Object() {[label]})
        Else
            Return [label].Tag
        End If
    End Function
    Delegate Sub SetToolStripLabel_Delegate(ByVal [toolStrip] As StatusStrip, ByVal [label] As ToolStripStatusLabel, ByVal [text] As String)
    Public Sub SetToolStripLabel_ThreadSafe(ByVal [toolStrip] As StatusStrip, ByVal [label] As ToolStripStatusLabel, ByVal [text] As String)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [toolStrip].InvokeRequired Then
            Dim MyDelegate As New SetToolStripLabel_Delegate(AddressOf SetToolStripLabel_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[toolStrip], [label], [text]})
        Else
            [label].Text = [text]
        End If
    End Sub

    Delegate Function GetToolStripLabel_Delegate(ByVal [toolStrip] As StatusStrip, ByVal [label] As ToolStripLabel) As String
    Public Function GetToolStripLabel_ThreadSafe(ByVal [toolStrip] As StatusStrip, ByVal [label] As ToolStripLabel) As String
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [toolStrip].InvokeRequired Then
            Dim MyDelegate As New GetToolStripLabel_Delegate(AddressOf GetToolStripLabel_ThreadSafe)
            Return Me.Invoke(MyDelegate, New Object() {[toolStrip], [label]})
        Else
            Return [label].Text
        End If
    End Function
#End Region

#Region "Event Handlers"
    Public Sub ProgressStatus(ByVal msg As String)
        SyncLock Me
            If Not msg.EndsWith("...") Then msg = String.Format("{0}...", msg)
            SetListAddItem_ThreadSafe(lstLog, String.Format("{0}-{1}", Format(Now, "yyyy-MM-dd HH:mm:ss"), msg))
            logger.Info(msg)
        End SyncLock
    End Sub
    Private Sub OnHeartbeat(msg As String)
        'Update detailed status on the first part, dont append if the text starts with <
        If msg.Contains("<<<") Then
            msg = Replace(msg, "<<<", Nothing)
            ProgressStatus(msg)
        Else
            ProgressStatus(msg)
        End If
        msg = Nothing
    End Sub
    Private Sub OnWaitingFor(elapsedSecs As Integer, totalSecs As Integer, msg As String)
        If msg.Contains("...") Then msg = msg.Replace("...", "")
        ProgressStatus(String.Format("{0}, waiting {1}/{2} secs", msg, elapsedSecs, totalSecs))
    End Sub
    Private Sub OnDocumentRetryStatus(currentTry As Integer, totalTries As Integer)
        'ProgressStatus(String.Format("Try #{0}/{1}: Connecting", currentTry, totalTries))
    End Sub
    Private Sub OnDocumentDownloadComplete()
    End Sub
    Private Sub OnErrorOcurred(ByVal msg As String, ByVal ex As Exception)
        MsgBox(ex.Message)
        End
    End Sub
#End Region

#Region "Private Attributes"
    Private _cts As CancellationTokenSource
    Private _lastLoggedMessage As String = Nothing
    Private _controller As APIStrategyController = Nothing
    Private _connection As IConnection = Nothing

    'Private _subscriber As APIInstrumentSubscriber = Nothing
#End Region

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        GlobalDiagnosticsContext.Set("appname", My.Application.Info.AssemblyName)
        GlobalDiagnosticsContext.Set("version", My.Application.Info.Version.ToString)
        logger.Trace("*************************** Logging started ***************************")
    End Sub

    Private Async Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        _cts = New CancellationTokenSource()
        _cts.Token.ThrowIfCancellationRequested()

        Try
            Dim currentUser As New ZerodhaUser With {.UserId = "DK4056",
                .Password = "Zerodha@123a",
                .APIVersion = "3",
                .APIKey = "hcwmefsivttbchla",
                .APISecret = "t9rd8wut44ija2vp15y87hln28h5oppb",
                .API2FA = Nothing}
            _controller = New ZerodhaStrategyController(currentUser, _cts)

            RemoveHandler _controller.Heartbeat, AddressOf OnHeartbeat
            RemoveHandler _controller.WaitingFor, AddressOf OnWaitingFor
            RemoveHandler _controller.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
            RemoveHandler _controller.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
            RemoveHandler _controller.TickerClose, AddressOf OnTickerClose
            RemoveHandler _controller.TickerConnect, AddressOf OnTickerConnect
            RemoveHandler _controller.TickerError, AddressOf OnTickerError
            RemoveHandler _controller.TickerErrorWithStatus, AddressOf OnTickerErrorWithStatus
            RemoveHandler _controller.TickerNoReconnect, AddressOf OnTickerNoReconnect

            AddHandler _controller.Heartbeat, AddressOf OnHeartbeat
            AddHandler _controller.WaitingFor, AddressOf OnWaitingFor
            AddHandler _controller.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
            AddHandler _controller.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
            AddHandler _controller.TickerClose, AddressOf OnTickerClose
            AddHandler _controller.TickerConnect, AddressOf OnTickerConnect
            AddHandler _controller.TickerError, AddressOf OnTickerError
            AddHandler _controller.TickerErrorWithStatus, AddressOf OnTickerErrorWithStatus
            AddHandler _controller.TickerNoReconnect, AddressOf OnTickerNoReconnect
            AddHandler _controller.TickerReconnect, AddressOf OnTickerReconnect

#Region "Login"
            Dim loginMessage As String = Nothing
            While True
                _connection = Nothing
                loginMessage = Nothing
                Try
                    OnHeartbeat("Attempting to get connection to Zerodha API")
                    _connection = Await _controller.LoginAsync().ConfigureAwait(False)
                Catch ex As Exception
                    loginMessage = ex.Message
                    logger.Error(ex)
                End Try
                If _connection Is Nothing Then
                    If loginMessage IsNot Nothing AndAlso loginMessage.ToUpper.Contains("password".ToUpper) Then
                        'No need to retry as its a password failure
                        OnHeartbeat(String.Format("Loging process failed:{0}", loginMessage))
                        Exit While
                    Else
                        OnHeartbeat(String.Format("Loging process failed:{0} | Waiting for 10 seconds before retrying connection", loginMessage))
                        Await Task.Delay(10000)
                    End If
                Else
                    Exit While
                End If
            End While
            If _connection Is Nothing Then
                If loginMessage IsNot Nothing Then
                    Throw New ApplicationException(String.Format("No connection to Zerodha API could be established | Details:{0}", loginMessage))
                Else
                    Throw New ApplicationException("No connection to Zerodha API could be established")
                End If
            End If
#End Region
            OnHeartbeat("Completing all pre-automation requirements")
            Dim isPreProcessingDone As Boolean = Await _controller.PrepareToRunStrategyAsync().ConfigureAwait(False)

            If Not isPreProcessingDone Then Throw New ApplicationException("PrepareToRunStrategyAsync did not succeed, cannot progress")


            Dim ohlStrategyToExecute As New OHLStrategy(_controller, _cts)
            OnHeartbeat(String.Format("Running strategy:{0}", ohlStrategyToExecute.ToString))

            Await _controller.ExecuteStrategyAsync(ohlStrategyToExecute)

            Dim dashboadList As SortableBindingList(Of OHLStrategyInstrument) = New SortableBindingList(Of OHLStrategyInstrument)(ohlStrategyToExecute.TradableStrategyInstruments)
            SetGridDataBind_ThreadSafe(dgMainDashboard, dashboadList)
            SetGridDisplayIndex_ThreadSafe(dgMainDashboard, "OHL", GetGridColumnCount_ThreadSafe(dgMainDashboard) - 1)
            Exit Sub

            'Await CType(_controller, ZerodhaStrategyController).TestAsync.ConfigureAwait(False)
            'OnHeartbeat("Getting all instruments for the day")
            'Dim allInstruments As IEnumerable(Of IInstrument) = Await _adapter.GetAllInstrumentsAsync().ConfigureAwait(False)
            'If allInstruments Is Nothing OrElse allInstruments.Count = 0 Then
            '    Throw New ApplicationException("No instrument master list could be retrieved from Zerodha")
            'End If

            'OnHeartbeat("Getting tradable instruments as per strategy")
            'Dim mrStrategyInstruments As List(Of MomentumReversalStrategyInstrument) =
            '    Await MomentumReversalStrategyInstrument.GetAllTradableInstrumentsAsync(allInstruments,
            '                                                                        _adapter,
            '                                                                        _cts).ConfigureAwait(False)
            'If mrStrategyInstruments Is Nothing OrElse mrStrategyInstruments.Count = 0 Then
            '    Throw New ApplicationException("No tradable strategy instruments could be created")
            'End If

            'OnHeartbeat("Opening subcriber")
            '_subscriber = New ZerodhaInstrumentSubscriber(_adapter, _cts)
            'AddHandler _subscriber.Heartbeat, AddressOf OnHeartbeat
            'AddHandler _subscriber.WaitingFor, AddressOf OnWaitingFor
            'AddHandler _subscriber.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
            'AddHandler _subscriber.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete

            'OnHeartbeat(String.Format("Subscribing strategy instruments, count:{0}", mrStrategyInstruments.Count))
            'For Each runningStrategyInstrument As MomentumReversalStrategyInstrument In mrStrategyInstruments
            '    _subscriber.SubscribeStrategy(runningStrategyInstrument)
            'Next
            ''Await _adapter.ConnectTickerAsync(_subscriber).ConfigureAwait(False)
            'Dim t As Task
            't = New Task(Async Sub() Await _adapter.ConnectTickerAsync(_subscriber).ConfigureAwait(False))
            't.Start()
            ''Await t.ConfigureAwait(False)
            ''For Each mr In mrStrategyInstruments
            ''    '_adapter.TestAsync(mr.TradableInstrument.InstrumentIdentifier)
            ''    _adapter.GetAllTradesAsync(New Dictionary(Of String, Object) From {{"xxx", mr.TradableInstrument.InstrumentIdentifier}}, retryEnabled:=False)
            ''Next
            ''Await Task.Delay(4509934).ConfigureAwait(False)
            'Await _subscriber.RunAdditionalStrategyTriggersAsync().ConfigureAwait(False)
        Catch cx As OperationCanceledException
            logger.Error(cx)
            MsgBox(String.Format("The following error occurred: {0}", cx.Message), MsgBoxStyle.Critical)
        Catch ex As Exception
            logger.Error(ex)
            MsgBox(String.Format("The following error occurred: {0}", ex.Message), MsgBoxStyle.Critical)
        Finally
            ProgressStatus("No pending actions")
        End Try
    End Sub

    Private Sub OnTickerClose()
        blbTickerStatus.Color = Color.Pink
        OnHeartbeat("Ticker:Closed")
    End Sub
    Private Sub OnTickerConnect()
        blbTickerStatus.Color = Color.Lime
        OnHeartbeat("Ticker:Connected")
    End Sub
    Private Sub OnTickerErrorWithStatus(ByVal isConnected As Boolean, ByVal errorMsg As String)
        If Not isConnected Then blbTickerStatus.Color = Color.Pink
    End Sub
    Private Sub OnTickerError(ByVal errorMsg As String)
        'Nothing to do
        OnHeartbeat(String.Format("Ticker:Error:{0}", errorMsg))
    End Sub
    Private Sub OnTickerNoReconnect()
        'Nothing to do
    End Sub
    Private Sub OnTickerReconnect()
        blbTickerStatus.Color = Color.Yellow
        OnHeartbeat("Ticker:Reconnected")
    End Sub

    Private Sub tmrTickerStatus_Tick(sender As Object, e As EventArgs) Handles tmrTickerStatus.Tick
        tmrTickerStatus.Enabled = False
        If tmrTickerStatus.Interval = 700 Then
            tmrTickerStatus.Interval = 2000
            blbTickerStatus.Visible = True
        Else
            tmrTickerStatus.Interval = 700
            blbTickerStatus.Visible = False
        End If
        tmrTickerStatus.Enabled = True
    End Sub


    Private Sub dgMainDashboard_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgMainDashboard.CellFormatting
        'If e.ColumnIndex = 5 Then
        'dgMainDashboard.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.BackColor = Color.LightGreen
        'End If
    End Sub

    Private Sub dgMainDashboard_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles dgMainDashboard.DataBindingComplete
        'For Each column As DataGridViewColumn In CType(sender, DataGridView).Columns
        '    column.SortMode = DataGridViewColumnSortMode.Automatic
        'Next
    End Sub
End Class

﻿Imports System.Threading
Imports Algo2TradeCore.Controller
Imports NLog

Namespace Adapter
    Public MustInherit Class APIHistoricalDataFetcher

#Region "Events/Event handlers"
        Public Event DocumentDownloadComplete()
        Public Event DocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
        Public Event Heartbeat(ByVal msg As String)
        Public Event WaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
        'The below functions are needed to allow the derived classes to raise the above two events
        Protected Overridable Sub OnDocumentDownloadComplete()
            RaiseEvent DocumentDownloadComplete()
        End Sub
        Protected Overridable Sub OnDocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
            RaiseEvent DocumentRetryStatus(currentTry, totalTries)
        End Sub
        Protected Overridable Sub OnHeartbeat(ByVal msg As String)
            RaiseEvent Heartbeat(msg)
        End Sub
        Protected Overridable Sub OnWaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
            RaiseEvent WaitingFor(elapsedSecs, totalSecs, msg)
        End Sub
#End Region

#Region "Logging and Status Progress"
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Protected _cts As CancellationTokenSource
        Public Property ParentController As APIStrategyController
        Protected _subscribedInstruments As List(Of String) 'The unique tokens/instrument identifier
        Protected _instrumentIdentifer As String 'To allow this to process each instrument seperately
        Protected _isPollRunning As Boolean
        Protected _stopPollRunning As Boolean
        Public Sub New(ByVal associatedParentcontroller As APIStrategyController,
                       ByVal canceller As CancellationTokenSource)
            Me.ParentController = associatedParentcontroller
            _cts = canceller
        End Sub
        Public Sub New(ByVal associatedParentController As APIStrategyController,
                       ByVal instrumentIdentifier As String,
                       ByVal canceller As CancellationTokenSource)
            Me.New(associatedParentController, canceller)
            Me._instrumentIdentifer = instrumentIdentifier
        End Sub
        Public MustOverride Async Function ConnectFetcherAsync() As Task
        Public MustOverride Async Function SubscribeAsync(ByVal instrumentIdentifiers As List(Of String)) As Task
        Public MustOverride Overrides Function ToString() As String
        Public MustOverride Sub ClearLocalUniqueSubscriptionList()
        Public MustOverride Function IsConnected() As Boolean
        Public MustOverride Async Function CloseFetcherIfConnectedAsync() As Task
        Public MustOverride Async Function StartPollingAsync() As Task
        Protected MustOverride Async Function GetHistoricalCandleStick() As Task

        Public Sub RefreshCancellationToken(ByVal canceller As CancellationTokenSource)
            _cts = canceller
        End Sub
    End Class
End Namespace
﻿Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog


Namespace Controller
    Public MustInherit Class APIStrategyController

#Region "Events/Event handlers"
        Public Event DocumentDownloadComplete()
        Public Event DocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
        Public Event Heartbeat(ByVal msg As String)
        Public Event WaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
        Public Event DocumentDownloadCompleteEx(ByVal source As List(Of Object))
        Public Event DocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
        Public Event HeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
        Public Event WaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
        'Create the events for UI to handle the way it needs to show the ticker
        Public Event TickerConnect()
        Public Event TickerClose()
        Public Event TickerErrorWithStatus(ByVal isConnected As Boolean, ByVal errorMessage As String)
        Public Event TickerError(ByVal errorMessage As String)
        Public Event TickerNoReconnect()
        Public Event TickerReconnect()
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
        'The below functions are needed to allow the derived classes to raise the above two events
        Protected Overridable Sub OnDocumentDownloadCompleteEx(ByVal source As List(Of Object))
            RaiseEvent DocumentDownloadCompleteEx(source)
        End Sub
        Protected Overridable Sub OnDocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, source)
        End Sub
        Protected Overridable Sub OnHeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
            RaiseEvent HeartbeatEx(msg, source)
        End Sub
        Protected Overridable Sub OnWaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
            RaiseEvent WaitingForEx(elapsedSecs, totalSecs, msg, source)
        End Sub
        Public Overridable Sub OnTickerConnect()
            RaiseEvent TickerConnect()
        End Sub
        Public Overridable Sub OnTickerClose()
            RaiseEvent TickerClose()
        End Sub
        Public Overridable Sub OnTickerError(ByVal errorMessage As String)
            RaiseEvent TickerError(errorMessage)
        End Sub
        Public Overridable Sub OnTickerErrorWithStatus(ByVal isConnected As Boolean, ByVal errorMessage As String)
            RaiseEvent TickerErrorWithStatus(isConnected, errorMessage)
        End Sub
        Public Overridable Sub OnTickerNoReconnect()
            RaiseEvent TickerNoReconnect()
        End Sub
        Public Overridable Sub OnTickerReconnect()
            RaiseEvent TickerReconnect()
        End Sub
#End Region

#Region "Logging and Status Progress"
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Protected _currentUser As IUser
        Protected _cts As CancellationTokenSource
        Protected _MaxReTries As Integer = 20
        Protected _WaitDurationOnConnectionFailure As TimeSpan = TimeSpan.FromSeconds(5)
        Protected _WaitDurationOnServiceUnavailbleFailure As TimeSpan = TimeSpan.FromSeconds(30)
        Protected _WaitDurationOnAnyFailure As TimeSpan = TimeSpan.FromSeconds(10)
        Protected _LoginURL As String
        Protected _LoginThreads As Integer
        Public Property APIConnection As IConnection
        Protected _APIAdapter As APIAdapter
        Protected _APITicker As APITicker
        Protected _AllInstruments As IEnumerable(Of IInstrument)
        Protected _AllStrategies As List(Of Strategy)
        Protected _subscribedStrategyInstruments As Dictionary(Of String, List(Of StrategyInstrument))
        Public Sub New(ByVal validatedUser As IUser,
                       ByVal canceller As CancellationTokenSource)
            _currentUser = validatedUser
            _cts = canceller
            _LoginThreads = 0
        End Sub
        Public MustOverride Function GetErrorResponse(ByVal response As Object) As String
        Public MustOverride Async Function CloseTickerIfConnectedAsync() As Task
#Region "Login"
        Protected MustOverride Function GetLoginURL() As String
        Public MustOverride Async Function LoginAsync() As Task(Of IConnection)
        Public MustOverride Async Function ExecuteStrategyAsync(ByVal strategyToRun As Strategy) As Task
        Public MustOverride Async Function PrepareToRunStrategyAsync() As Task(Of Boolean)
#End Region

    End Class
End Namespace
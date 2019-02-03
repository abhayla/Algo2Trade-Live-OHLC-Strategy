﻿Imports System.ComponentModel.DataAnnotations
Imports System.Threading
Imports Algo2TradeCore
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog

Public Class MomentumReversalStrategyInstrument
    Inherits StrategyInstrument
    Implements IDisposable

#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Public Sub New(ByVal associatedInstrument As IInstrument, ByVal associatedParentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
        MyBase.New(associatedInstrument, associatedParentStrategy, canceller)
        _APIAdapter = New ZerodhaAdapter(ParentStrategy.ParentController, _cts)
        AddHandler _APIAdapter.Heartbeat, AddressOf OnHeartbeat
        AddHandler _APIAdapter.WaitingFor, AddressOf OnWaitingFor
        AddHandler _APIAdapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
        AddHandler _APIAdapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
    End Sub
    Public Overrides Function ToString() As String
        Return String.Format("{0}_{1}", ParentStrategy.ToString, TradableInstrument.ToString)
    End Function
    Public Overrides Function GenerateTag() As String
        Return String.Format("{0}_{1}", ParentStrategy.StrategyIdentifier, TradableInstrument.TradingSymbol)
    End Function
    Public Overrides Async Function HandleTickTriggerToUIETCAsync() As Task
        'logger.Debug("ProcessTickAsync, tickData:{0}", Utilities.Strings.JsonSerialize(tickData))
        _cts.Token.ThrowIfCancellationRequested()
        '_LastTick = tickData
        Await MyBase.HandleTickTriggerToUIETCAsync().ConfigureAwait(False)
        _cts.Token.ThrowIfCancellationRequested()
    End Function
    Public Overrides Async Function ProcessOrderAsync(ByVal orderData As IBusinessOrder) As Task
        _cts.Token.ThrowIfCancellationRequested()
        Await MyBase.ProcessOrderAsync(orderData).ConfigureAwait(False)
        _cts.Token.ThrowIfCancellationRequested()
    End Function
    Public Overrides Async Function MonitorAsync() As Task
        Dim lastException As Exception = Nothing
        Try
            While True
                If Me.ParentStrategy.ParentController.OrphanException IsNot Nothing Then
                    Throw Me.ParentStrategy.ParentController.OrphanException
                End If
                Dim r As Random = New Random()
                Dim x = r.Next(0, 11)
                _cts.Token.ThrowIfCancellationRequested()
                If x = 7 Then
                    While Me.ParentStrategy.ParentController.APIConnection Is Nothing
                        _cts.Token.ThrowIfCancellationRequested()
                        logger.Debug("Waiting for fresh token:{0}", TradableInstrument.InstrumentIdentifier)
                        Await Task.Delay(500).ConfigureAwait(False)
                    End While

                    _APIAdapter.SetAPIAccessToken(Me.ParentStrategy.ParentController.APIConnection.AccessToken)
                    _cts.Token.ThrowIfCancellationRequested()
                    Dim allTrades As IEnumerable(Of ITrade) = Await _APIAdapter.GetAllTradesAsync().ConfigureAwait(False)
                End If
                _cts.Token.ThrowIfCancellationRequested()
                Await Task.Delay(1000)
            End While
        Catch ex As Exception
            logger.Error("Strategy Instrument:{0}, error:{1}", Me.ToString, ex.ToString)
            Throw ex
        End Try

    End Function
    Protected Overrides Function IsTriggerReceivedForPlaceOrder() As Tuple(Of Boolean, PlaceOrderParameters)
        Dim ret As Tuple(Of Boolean, PlaceOrderParameters) = Nothing
        Throw New NotImplementedException("IsTriggerReceivedForPlaceOrderAsync-MR")
    End Function
    Protected Overrides Function IsTriggerReceivedForModifyStoplossOrder() As List(Of Tuple(Of Boolean, String, Decimal))
        Dim ret As List(Of Tuple(Of Boolean, String, Decimal)) = Nothing
        Throw New NotImplementedException("IsTriggerReceivedForModifyStoplossOrder-MR")
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If _APIAdapter IsNot Nothing Then
                    RemoveHandler _APIAdapter.Heartbeat, AddressOf OnHeartbeat
                    RemoveHandler _APIAdapter.WaitingFor, AddressOf OnWaitingFor
                    RemoveHandler _APIAdapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                    RemoveHandler _APIAdapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
